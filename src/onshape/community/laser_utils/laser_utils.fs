FeatureScript 701;
import(path : "onshape/std/geometry.fs", version : "701.0");

/**
 * Given a list of items, a function of the form f(context, item) returns sortMetric and a sort direction,
 * returns a sorted set of indices to later be used with qNthElement.
 *
 *   context: Context in which to evaluate the sorting metric
 *   items: Query for multiple items for which to produce a sorted index list
 *   fn: function of form fn(C, x) which for a single item `x` returns some sort metric evaluated in context `C`
 *   ascending: enum selecting whether to sort in ascending or descending order
 */
export function getSortIndices(context is Context, items is Query, fn is function, ascending is SortDirection)
{
    var N = size(evaluateQuery(context, items));
    var sign = (ascending == SortDirection.ASCENDING) ? -1 : 1;
    var indices = makeArray(N);

    for (var i = 0; i < N; i += 1)
    {
        indices[i] = i;
    }

    var sortedIndices = sort(indices, function(j, k)
    {
        return sign * (fn(context, qNthElement(items, k)) - fn(context, qNthElement(items, j)));
    });

    return sortedIndices;
}


/**
 * Given a body, returns a sorted array of all planar faces owned by the body.
 *  context: Context in which to evaluate the Query
 *  body: Query specifying the body whose faces to return
 *  ascending: enum selecting whether to sort faces in ascending or descending order
 *
 * returns:
 *  Array
 */
export function getSortedFaceList(context is Context, body is Query, ascending is SortDirection) returns array
{
    var faces = evaluateQuery(context, qGeometry(qOwnedByBody(body, EntityType.FACE), GeometryType.PLANE));
    var sign = (ascending == SortDirection.ASCENDING) ? -1 : 1;

    var sortedFaces = sort(faces, function(face1, face2)
    {
        return sign * (evArea(context, { "entities" : face2 }) - evArea(context, { "entities" : face1 }));
    });

    return sortedFaces;
}

/**
 *
 *
 */
export function getOuterLoopFaces(context is Context, body is Query) returns Query
{
    // All faces of the tab, after pins have been cut
    var allFaces = qOwnedByBody(body, EntityType.FACE);
    // The large faces of the laser-cuttable tab part.  Assumed to have equal area and orientation.
    var largeFaces = qLargest(qGeometry(allFaces, GeometryType.PLANE));
    // All faces bounding the largest faces. By assumption, this should be all other faces of the part.
    var boundingFaces = qEdgeAdjacent(largeFaces, EntityType.FACE);

    var facePlane = evPlane(context, { "face" : qNthElement(largeFaces, 0) });
    var faceCoord = coordSystem(facePlane);

    var unchecked = boundingFaces;
    var outerLoopFaces = qNothing();

    var maxBox = 0 * meter;

    while (size(evaluateQuery(context, unchecked)) > 0)
    {
        var loop = qFaceOrEdgeBoundedFaces(qUnion([qNthElement(unchecked, 0), largeFaces]));
        unchecked = qSubtraction(unchecked, loop);

        var bbox = evBox3d(context, {
                "topology" : loop,
                "cSys" : faceCoord,
                "tight" : true
            });

        var tMaxBox = evDistance(context, {
                    "side0" : bbox.minCorner,
                    "side1" : bbox.maxCorner
                }).distance;

        if (tMaxBox > maxBox)
        {
            maxBox = tMaxBox;
            outerLoopFaces = loop;
        }
    }

    return outerLoopFaces;
}

/**
 * Given a body, returns a Query selecting the largest planar face of that body.
 *  context: Context in which to evaluate the Query
 *  body: Query specifiying body whose largest surface to return
 *
 * returns:
 *  Query
 */
export function getLargestFace(context is Context, body is Query) returns Query
{
    var faces = qGeometry(qOwnedByBody(body, EntityType.FACE), GeometryType.PLANE);

    return qNthElement(qLargest(faces), 0);
}

/**
 * Given a body, returns the area of the largest planar face of that body.
 *  context: Context in which to evaluate the Query
 *  body: Query specifiying body whose largest surface area to return
 *
 * returns:
 *  ValueWithUnits (area)
 */
export function getLargestArea(context is Context, body is Query) returns ValueWithUnits
{
    var largest = getLargestFace(context, body);
    return evArea(context, {
                "entities" : qNthElement(largest, 0)
            });
}

/**
 * Given a body, returns the thickness of that body.
 * Assumes that body selects a part which satisfies canBeLaserCut (see below).
 *  context: Context in which to evaluate the Query
 *  body: Query specifiying body whose thickness to return
 *
 * returns:
 *  ValueWithUnits (length)
 */
export function getThickness(context is Context, body is Query) returns ValueWithUnits
{
    var largest = qLargest(qOwnedByBody(body, EntityType.FACE));
    return evDistance(context, {
                    "side0" : qNthElement(largest, 0),
                    "side1" : qNthElement(largest, 1)
                }).distance;
}

/**
 * Check whether the normal of face is parallel to dir.
 *  face: The Plane whose normal to check
 *  dir: 3D Vector giving a direction
 */
export predicate isOriented(face is Plane, dir is Vector)
{
    canBePlane(face);
    is3dDirection(dir);
    parallelVectors(face.normal, dir);
}

/**
 * Checks whether body is geometry that can be laser-cut.
 *
 * This function defines a part able to be laser cut as:
 *      A part that could be created by a single extrude feature,
 *      such that the cap faces of the extrude are those faces with the largest area
 *
 *  context: Context in which to evaluate body
 *  body: Query selecting the part to test
 *
 * returns:
 *  boolean
 */
export function canBeLaserCut(context is Context, body is Query) returns boolean
{
    var allFaces = qOwnedByBody(body, EntityType.FACE);

    // Minimum number of faces for a laser-cut part:
    //   two cap circles plus cylinder connecting
    if (size(evaluateQuery(context, allFaces)) < 3)
    {
        return false;
    }

    var largest = qLargest(allFaces);

    // Cap faces (assumed to be the two largest faces) must have the same area
    if (size(evaluateQuery(context, largest)) != 2)
    {
        return false;
    }

    // Cap faces must be parallel
    if (!parallelVectors(
                evPlane(context, { "face" : qNthElement(largest, 0) }).normal,
                evPlane(context, { "face" : qNthElement(largest, 1) }).normal
            ))
    {
        return false;
    }

    // All edges not part of the largest faces must be perpendicular to the largest faces
    // These edges must therefore be lines
    var cutPlane = evPlane(context, { "face" : qNthElement(largest, 0) });
    var orthogonalEdges = qSubtraction(qOwnedByBody(body, EntityType.EDGE), qEdgeAdjacent(largest, EntityType.EDGE));
    var linearEdges = qGeometry(orthogonalEdges, GeometryType.LINE);

    // Reject body if other edges are not linear
    if (size(evaluateQuery(context, orthogonalEdges)) != size(evaluateQuery(context, linearEdges)))
    {
        return false;
    }

    // Reject body if other edges are not orthogonal
    for (var edge in evaluateQuery(context, orthogonalEdges))
    {
        if (!isOriented(cutPlane, evEdgeTangentLine(context, {
                                "edge" : edge,
                                "parameter" : 0.5
                            }).direction))
        {
            return false;
        }
    }

    return true;
}

/**
 * Checks whether a collision calculated between two bodies is a collision type suited to creating a laser cur finger joint
 *  collInfo: the result of running evCollision on two bodies.
 *
 * returns:
 *  boolean
 */
export function isJoinableCollision(collInfo is array) returns boolean
{
    var M = size(collInfo);
    var isValid = false;
    if (M > 0)
    {

        for (var i = 0; i < M; i += 1)
        {
            var collision = collInfo[i];
            if (collision["type"] == ClashType.INTERFERE)
            {
                isValid = true;
                break;
            }
        }
    }

    return isValid;
}

/**
 * Calculates the distance a hole can be cut through continuous material.
 * Used for calculating hole termination points when drilling through multiple clamped sheets.
 *  context: the Context in which to work
 *  id: a unique base id to create a temporary feature
 *  params:
 *      csys: CoordSystem whose origin defines the start of the hole, and whose Z axis defines the direction of the hole
 *      D: diameter of the hole
 *
 * returns:
 *  ValueWithUnits the distance the hole can be extruded while passing through material
 */
export function getContinuousDepth(context is Context, id is Id, params is map) returns ValueWithUnits
precondition
{
    params.csys is CoordSystem;
    params.D is ValueWithUnits;
}
{
    startFeature(context, id);

    var smallOffset = 0.001 * millimeter;

    opBoolean(context, id + "union", {
                "tools" : qBodyType(qEverything(EntityType.BODY), BodyType.SOLID),
                "operationType" : BooleanOperationType.UNION
            });

    var sketch = newSketchOnPlane(context, id + "sketch", {
            "sketchPlane" : plane(params.csys)
        });

    skCircle(sketch, "testCircle", {
                "center" : vector(0, 0) * inch,
                "radius" : 0.5 * params.D
            });

    skSolve(sketch);

    opExtrude(context, id + "holeCast", {
                "entities" : qSketchRegion(id + "sketch"),
                "direction" : params.csys.zAxis,
                "endBound" : BoundingType.THROUGH_ALL
            });

    var nextBodyAndCast = qContainsPoint(qBodyType(qEverything(EntityType.BODY), BodyType.SOLID), params.csys.origin + smallOffset * params.csys.zAxis);

    var depth = 0 * meter;
    if (size(evaluateQuery(context, nextBodyAndCast)) < 2)
    {
        depth = 0 * meter;
    }
    else
    {
        opBoolean(context, id + "intersect", {
                    "tools" : nextBodyAndCast,
                    "operationType" : BooleanOperationType.INTERSECTION
                });
        depth = evBox3d(context, {
                        "topology" : qBodyType(qCreatedBy(id + "intersect", EntityType.BODY), BodyType.SOLID),
                        "cSys" : params.csys
                    }).maxCorner[2];
    }
    abortFeature(context, id);

    return depth;
}

/**
 * Returns those elements of subquery which satisfy fn in context.
 *  subquery: Query resolving to some number of entities to filter
 *  context:
 *  fn: Function which returns true for entities to keep in the final Query
 *
 * returns:
 *  Query resolving to those entities from subquery which satisfy fn
 */
export function qFilterFunction(subquery is Query, context is Context, fn is function) returns Query
{
    var queries = evaluateQuery(context, subquery);
    var filtQueries = filter(queries, fn);
    return qUnion(filtQueries);
}

export enum SortDirection
{
    ASCENDING,
    DESCENDING
}

