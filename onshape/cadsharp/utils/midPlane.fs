FeatureScript 1634;
import(path : "onshape/std/geometry.fs", version : "1634.0");

export function midPlaneFunctionPascoe(id is Id, context is Context, leftInput, rightInput, normalPlane)
{
    //Sketch a line that connects the two bottom points
    const leftPlane = evFaceTangentPlane(context, {
            "face" : leftInput,
            "parameter" : vector(0.5, 0.5)
    });

    const evPlane = plane(leftPlane.origin, leftPlane.normal, leftPlane.x);
    const sketch1 = newSketchOnPlane(context, id + "sketch1", {
                "sketchPlane" : evPlane
            });

    skLineSegment(sketch1, "line1", {
                "start" : worldToPlane(evPlane, evVertexPoint(context, {
                            "vertex" : leftInput
                        })),
                "end" : worldToPlane(evPlane, evVertexPoint(context, {
                            "vertex" : rightInput
                        }))
            });

    skSolve(sketch1);

    const connectedEdge = sketchEntityQuery(id + "sketch1", EntityType.EDGE, "line1");
    const connectedEdgeMid = evEdgeCurvature(context, {
                "edge" : connectedEdge,
                "parameter" : .5 });
    const startPlaneXDirection = connectedEdgeMid.frame.zAxis;
    const startPlane = plane(connectedEdgeMid.frame.origin, normalPlane.normal, startPlaneXDirection);

    return startPlane;
}