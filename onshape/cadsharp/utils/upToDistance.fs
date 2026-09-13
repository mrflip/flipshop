FeatureScript 1634;
import(path : "onshape/std/geometry.fs", version : "1634.0");

export function upToDistancePascoe(id is Id, context is Context, loopCount, upToDef, offsetDef, fromPlane, fromPlaneNegative, preset1)
{
    var studioVariable = offsetDef;
    var entityDistance = 0 * inch;

    if (!isQueryEmpty(context, upToDef))
    {

        const signCompare = evDistance(context, {
                        "side0" : fromPlaneNegative,
                        "side1" : upToDef
                    }).distance;

        entityDistance = evDistance(context, {
                        "side0" : fromPlane,
                        "side1" : upToDef
                    }).distance;

        entityDistance = signCompare < entityDistance ? -entityDistance : entityDistance;
    }

    studioVariable = -offsetDef - entityDistance + preset1;

    return studioVariable;
}