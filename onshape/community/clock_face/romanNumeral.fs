FeatureScript 336;
import(path : "onshape/std/geometry.fs", version : "336.0");

annotation { "Feature Type Name" : "Roman Numeral" }
export const romanNumeral = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Number" }
        isInteger(definition.num, POSITIVE_COUNT_BOUNDS);

        annotation { "Name" : "Center line", "Filter" : GeometryType.LINE && SketchObject.YES, "MaxNumberOfPicks" : 1 }
        definition.centerline is Query;

        annotation { "Name" : "Part to merge with", "Filter" : EntityType.BODY && BodyType.SOLID, "MaxNumberOfPicks" : 1 }
        definition.otherPart is Query;

    }
    {
        // -------------------
        // For feature pattern
        var remainingTransform = getRemainderPatternTransform(context, {
                "references" : definition.centerline
        });
        // -------------------

        var sketch1 = newSketchOnPlane(context, id + "sketch1", {
                "sketchPlane" : plane(vector(0, 0, 0) * inch, vector(0, 0, 1), vector(1, 0, 0))
        });
        skText(sketch1, "text", {
                "text" : toRoman(definition.num),
                "fontName" : "Tinos-Regular.ttf"
        });
        skSolve(sketch1);
        var bbox = evBox3d(context, {
                "topology" : qCreatedBy(id + "sketch1", EntityType.BODY)
        });
        const center = vector((bbox.minCorner[0] + bbox.maxCorner[0]) / 2, 0 * inch, 0 * inch);

        const startingCSys = coordSystem(center, vector(1, 0, 0), vector(0, 0, 1));

        var scale = length(toRoman(definition.num)) > 4 ? 0.4 : 0.6;

        var t = fromWorld(startingCSys);
        t = transform(identityMatrix(3) * (evLength(context, {
                "entities" : definition.centerline
        }) * scale / meter), vector(0, 0, 0) * inch) * t;

        const zAxis is Line   = line(vector(0, 0, 0) * inch, vector(0, 0, 1));
        t = rotationAround(zAxis, 90 * degree) * t;
        const endline = evEdgeTangentLine(context, {
                "edge" : definition.centerline,
                "parameter" : 1 - ((1 - scale) / 2)
        });

        const endingCSys = coordSystem(endline.origin, endline.direction, evOwnerSketchPlane(context, {
                "entity" : definition.centerline
        }).normal);
        t = toWorld(endingCSys) * t;


        opTransform(context, id + "transform1", {
                "bodies" : qCreatedBy(id + "sketch1", EntityType.BODY),
                "transform" : t
        });

        extrude(context, id + "extrude1", {
                "entities" : qSketchRegion(id + "sketch1"),
                "endBound" : BoundingType.BLIND,
                "depth" : 0.075 * inch
        });


        opDeleteBodies(context, id + "deleteBodies1", {
                "entities" : qCreatedBy(id + "sketch1", EntityType.BODY)
        });

        // -------------------
        // For feature pattern
        transformResultIfNecessary(context, id, remainingTransform);
        // -------------------


        opBoolean(context, id + "boolean1", {
                "tools" : qUnion([definition.otherPart, qCreatedBy(id + "extrude1", EntityType.BODY)]),
                "operationType" : BooleanOperationType.UNION
        });
    });

function toRoman(n is number) returns string
{
    return [
        "0",
        "I",
        "II",
        "III",
        "IV",
        "V",
        "VI",
        "VII",
        "VIII",
        "IX",
        "X",
        "XI",
        "XII",
        "XIII",
        "XIV",
        "XV",
        "XVI",
        "XVII",
        "XVIII",
        "XIX",
        "XX",
        "XXI",
        "XXII",
        "XXIII",
        "XXIV",
        "XXV",
        "XXVI",
        "XXVII",
        "XXVIII",
        "XXIX",
        "XXX",
        "XXXI",
        "XXXII",
        "XXXIII",
        "XXXIV",
        "XXXV",
        "XXXVI",
        "XXXVII",
        "XXXVIII",
        "XXXIX",
        "XL",
        "XLI",
        "XLII",
        "XLIII",
        "XLIV",
        "XLV",
        "XLVI",
        "XLVII",
        "XLVIII",
        "XLIX",
        "L"
    ][n];
}
