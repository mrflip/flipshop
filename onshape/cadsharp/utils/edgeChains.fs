FeatureScript 2815;
import(path : "onshape/std/common.fs", version : "2815.0");
// =======================================================
// ROUTED EDGE PATH:
// * Leaves planes by furthest vertex
// * Removes all edges closer to either plane than first step
// * BFS in interior region only
// =======================================================


// Midpoint of an edge (world coordinates)
function edgeMidpoint(context is Context, edge is Query) returns Vector
{
    const v0 = qEdgeVertex(edge, true);
    const v1 = qEdgeVertex(edge, false);

    const p0 = evVertexPoint(context, { "vertex" : v0 });
    const p1 = evVertexPoint(context, { "vertex" : v1 });

    return (p0 + p1) * 0.5;
}

// Find index of an edge inside an array (FS-safe)
function findEdgeIndex(context is Context,
                       edges is array,
                       target is Query) returns number
{
    for (var i = 0; i < size(edges); i += 1)
        if (!isQueryEmpty(context, qIntersection(edges[i], target)))
            return i;
    return -1;
}

// Signed distance from point to plane
function signedDistance(pt is Vector, pl is Plane) returns ValueWithUnits
{
    return dot(pl.normal, pt - pl.origin);
}

// Choose the vertex on an edge furthest from some origin
function furthestVertex(context is Context,
                        edge is Query,
                        origin is Vector) returns Query
{
    const vA = qEdgeVertex(edge, true);
    const vB = qEdgeVertex(edge, false);

    const pA = evVertexPoint(context, { "vertex" : vA });
    const pB = evVertexPoint(context, { "vertex" : vB });

    if (norm(pA - origin) > norm(pB - origin))
        return vA;
    else
        return vB;
}

// Adjacent edges by vertex, constrained to a set
function adjacentEdges(context is Context,
                       allowedEdges is Query,
                       edge is Query) returns Query
{
    const v0 = qEdgeVertex(edge, true);
    const v1 = qEdgeVertex(edge, false);

    var adj = qUnion([
            qAdjacent(v0, AdjacencyType.VERTEX, EntityType.EDGE),
            qAdjacent(v1, AdjacencyType.VERTEX, EntityType.EDGE)
        ]);

    adj = qIntersection(adj, allowedEdges);
    adj = qSubtraction(adj, edge);
    return adj;
}

// Pick closest adjacent edge to other side
function pickClosest(context is Context,
                     candidates is Query,
                     target is Query) returns Query
{
    if (isQueryEmpty(context, candidates))
        return qNothing();

    const candArr = evaluateQuery(context, candidates);
    const targetMid = edgeMidpoint(context, target);

    var best = candArr[0];
    var bestD = norm(edgeMidpoint(context, best) - targetMid);

    for (var i = 1; i < size(candArr); i += 1)
    {
        const d = norm(edgeMidpoint(context, candArr[i]) - targetMid);
        if (d < bestD)
        {
            bestD = d;
            best = candArr[i];
        }
    }

    return best;
}


// =======================================================
// MAIN ROUTER
// =======================================================
export function getEdgeChainBetween(context is Context,
                                  allEdges is Query,
                                  startEdge is Query,
                                  endEdge is Query,
                                  startPlane is Plane,
                                  endPlane is Plane) returns Query
{
    if (isQueryEmpty(context, startEdge) || isQueryEmpty(context, endEdge))
        return qNothing();

    // Same edge
    if (!isQueryEmpty(context, qIntersection(startEdge, endEdge)))
        return startEdge;

    // ---------------------------------------------------
    // 1) Choose furthest vertices from plane origins
    // ---------------------------------------------------
    const startVertex = furthestVertex(context, startEdge, startPlane.origin);
    const endVertex   = furthestVertex(context, endEdge,   endPlane.origin);

    // ---------------------------------------------------
    // 2) First interior edges
    // ---------------------------------------------------
    var startAdj = qAdjacent(startVertex, AdjacencyType.VERTEX, EntityType.EDGE);
    startAdj = qSubtraction(startAdj, startEdge);
    startAdj = qIntersection(startAdj, allEdges);
    const startFirst = pickClosest(context, startAdj, endEdge);

    var endAdj = qAdjacent(endVertex, AdjacencyType.VERTEX, EntityType.EDGE);
    endAdj = qSubtraction(endAdj, endEdge);
    endAdj = qIntersection(endAdj, allEdges);
    const endFirst = pickClosest(context, endAdj, startEdge);

    if (isQueryEmpty(context, startFirst) || isQueryEmpty(context, endFirst))
        return qUnion([startEdge, endEdge]);

    // ---------------------------------------------------
    // 3) Interior graph only (strip plane edges)
    // ---------------------------------------------------
    var interiorAll = qSubtraction(allEdges, qUnion([startEdge, endEdge]));
    const interiorArr = evaluateQuery(context, interiorAll);

    // ---------------------------------------------------
    // 4) Remove edges closer to planes than first step
    // ---------------------------------------------------
    const dStartBase = signedDistance(edgeMidpoint(context, startFirst), startPlane);
    const dEndBase   = signedDistance(edgeMidpoint(context, endFirst),   endPlane);

    const startSign = dStartBase >= 0 * inch ? 1 : -1;
    const endSign   = dEndBase   >= 0 * inch ? 1 : -1;

    var edges = [] as array;
    var allowedQ = qNothing();

    for (var i = 0; i < size(interiorArr); i += 1)
    {
        const e = interiorArr[i];

        if (!isQueryEmpty(context, qIntersection(e, startFirst)) ||
            !isQueryEmpty(context, qIntersection(e, endFirst)))
        {
            edges = append(edges, e);
            allowedQ = qUnion([allowedQ, e]);
            continue;
        }

        const m = edgeMidpoint(context, e);
        const ds = signedDistance(m, startPlane);
        const de = signedDistance(m, endPlane);

        var ok = true;

        // Must be further from startPlane than startFirst
        if (ds * startSign < abs(dStartBase))
            ok = false;

        // Must be further from endPlane than endFirst
        if (de * endSign < abs(dEndBase))
            ok = false;

        if (ok)
        {
            edges = append(edges, e);
            allowedQ = qUnion([allowedQ, e]);
        }
    }

    // ---------------------------------------------------
    // 5) BFS on remaining interior graph
    // ---------------------------------------------------
    const n = size(edges);
    const startIdx = findEdgeIndex(context, edges, startFirst);
    const endIdx   = findEdgeIndex(context, edges, endFirst);

    if (startIdx < 0 || endIdx < 0)
        return qUnion([startEdge, endEdge]);

    var visited = [] as array;
    var prev    = [] as array;

    for (var i = 0; i < n; i += 1)
    {
        visited = append(visited, false);
        prev    = append(prev, -1);
    }

    var queue = [] as array;
    var head  = 0;
    visited[startIdx] = true;
    queue = append(queue, startIdx);

    var found = false;

    while (head < size(queue))
    {
        const i = queue[head];
        head += 1;

        if (i == endIdx)
        {
            found = true;
            break;
        }

        const e = edges[i];
        const nbrs = evaluateQuery(context, adjacentEdges(context, allowedQ, e));

        for (var k = 0; k < size(nbrs); k += 1)
        {
            const ni = findEdgeIndex(context, edges, nbrs[k]);
            if (ni < 0 || visited[ni])
                continue;

            visited[ni] = true;
            prev[ni] = i;
            queue = append(queue, ni);
        }
    }

    // ---------------------------------------------------
    // 6) Rebuild interior + endpoints
    // ---------------------------------------------------
    var interiorPath = qNothing();
    if (found)
    {
        var idx = endIdx;
        while (idx != -1)
        {
            interiorPath = qUnion([interiorPath, edges[idx]]);
            idx = prev[idx];
        }
    }

    return qUnion([startEdge, interiorPath, endEdge]);
}


