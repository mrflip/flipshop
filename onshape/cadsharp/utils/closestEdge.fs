FeatureScript 1634;
import(path : "onshape/std/geometry.fs", version : "1634.0");

export function closestEdgePascoe(id is Id, context is Context, evEdges, closestToVector)
{
    var closest = qNothing();
    var d1;

    for (var i = 0; i < size(evEdges); i += 1)
    {
        const midPoint = evEdgeTangentLine(context, {
                        "edge" : evEdges[i],
                        "parameter" : .5
                    }).origin;

        const distance = evDistance(context, {
                        "side0" : midPoint,
                        "side1" : closestToVector
                    }).distance;

        if (isQueryEmpty(context, closest))
        {
            closest = evEdges[i];
            d1 = distance;
        }
        else
        {
            d1 = d1 < distance ? d1 : distance;
            closest = d1 < distance ? closest : evEdges[i];
        }
    }

    return closest;
}