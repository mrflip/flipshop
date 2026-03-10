FeatureScript 455;
import(path : "onshape/std/geometry.fs", version : "455.0");

export enum numberSpline
{
    annotation { "Name" : "SAE Standard 4-Spline" }
    fourSpline,
    annotation { "Name" : "SAE Standard 6-Spline" }
    sixSpline,
    annotation { "Name" : "SAE Standard 10-Spline" }
    tenSpline,
    annotation { "Name" : "SAE Standard 16-Spline" }
    sixteenSpline,
}

annotation { "Feature Type Name" : "SAE Straight-Sided Spline Feature" }
export const myFeature = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Spline Definition" }
        definition.numberSpline is numberSpline;

        annotation { "Name" : "Face", "Filter" : EntityType.FACE, "MaxNumberOfPicks" : 1 }
        definition.face is Query;

        annotation { "Name" : "Length" }
        isLength(definition.length, LENGTH_BOUNDS);
    }
    {
        var transforms = [];
        var instanceNames = [];
        var instanceCount;
        var face = definition.face;
        var profileArray;
        var midPointArray;
        var edge = qEdgeAdjacent(face, EntityType.EDGE);
        var curve = evCurveDefinition(context, { "edge" : edge });
        if (!(curve is Circle)) // if the selected curve is not a circle throw error
        {
            throw regenError("Selected face does not have a circular edge");
        }
        var curveRadius = curve.radius;
        if (definition.numberSpline == numberSpline.fourSpline)
        {
            instanceCount = 4;
            profileArray = fourSplineProfileArrays;
            midPointArray = fourSplineArcMidpointsArray;
        }
        if (definition.numberSpline == numberSpline.sixSpline)
        {
            instanceCount = 6;
            profileArray = sixSplineProfileArrays;
            midPointArray = sixSplineArcMidpointsArray;
        }
        if (definition.numberSpline == numberSpline.tenSpline)
        {
            instanceCount = 10;
            profileArray = tenSplineProfileArrays;
            midPointArray = tenSplineArcMidpointsArray;
        }
        if (definition.numberSpline == numberSpline.sixteenSpline)
        {
            instanceCount = 16;
            profileArray = sixteenSplineProfileArrays;
            midPointArray = sixteenSplineArcMidpointsArray;
        }
        var splineDefinition = findProfile(context, id, profileArray, instanceCount, midPointArray, curveRadius);
        var splineProfile = splineDefinition.splineProfile;
        if (splineProfile == undefined) // if we cannot find a matching value for face selected throw error
        {
            throw regenError("Face does not have a supported nominal diameter for the chosen standard");
        }
        var angle = splineDefinition.angle;
        var arcMidpoints = splineDefinition.arcMidPoints;
        var chamferSize = splineDefinition.chamferSize;
        var initPlane = evPlane(context, {
                "face" : face
            });
        definition.plane = initPlane;
        definition.length = -definition.length;
        definition.plane.origin += definition.plane.normal * definition.length;
        var ownerBody = qOwnerBody(definition.face);
        var profileSketch = newSketchOnPlane(context, id + "profileSketch", {
                "sketchPlane" : initPlane
            });
        var offsetSketch = newSketchOnPlane(context, id + "offsetSketch", {
                "sketchPlane" : definition.plane
            });

        skLineSegment(profileSketch, "line1", {
                    "start" : splineProfile[0],
                    "end" : splineProfile[1]
                });
        skArc(profileSketch, "arc1", {
                    "start" : splineProfile[1],
                    "mid" : arcMidpoints[1],
                    "end" : splineProfile[2]
                });
        skLineSegment(profileSketch, "line2", {
                    "start" : splineProfile[2],
                    "end" : splineProfile[3]
                });
        skArc(profileSketch, "arc2", {
                    "start" : splineProfile[3],
                    "mid" : arcMidpoints[0],
                    "end" : splineProfile[0]
                });

        skLineSegment(offsetSketch, "line1", {
                    "start" : splineProfile[0],
                    "end" : splineProfile[1]
                });
        skArc(offsetSketch, "arc1", {
                    "start" : splineProfile[1],
                    "mid" : arcMidpoints[1],
                    "end" : splineProfile[2]
                });
        skLineSegment(offsetSketch, "line2", {
                    "start" : splineProfile[2],
                    "end" : splineProfile[3]
                });
        skArc(offsetSketch, "arc2", {
                    "start" : splineProfile[3],
                    "mid" : arcMidpoints[0],
                    "end" : splineProfile[0]
                });
        skLineSegment(offsetSketch, "revolveLine", {
                    "start" : splineProfile[0] + vector(1, 0) * inch,
                    "end" : splineProfile[3] + vector(1, 0) * inch
                });
        skSolve(profileSketch);
        skSolve(offsetSketch);
        opChamfer(context, id + "chamfer1", {
                    "entities" : edge,
                    "chamferType" : ChamferType.EQUAL_OFFSETS,
                    "width" : chamferSize
                });
        extrude(context, id + "extrude1", {
                    "entities" : qSketchRegion(id + "profileSketch"),
                    "endBound" : BoundingType.BLIND,
                    "depth" : definition.length
                });
        var body = qCreatedBy(id + "extrude1", EntityType.BODY);
        var axisQuery = sketchEntityQuery(id + "offsetSketch", EntityType.EDGE, "revolveLine");
        revolve(context, id + "revolve1", {
                    "operationType" : NewBodyOperationType.NEW,
                    "entities" : qSketchRegion(id + "offsetSketch"),
                    "axis" : axisQuery,
                    "revolveType" : RevolveType.FULL,
                    "booleanScope" : body,
                    "defaultScope" : true });
        var rotationLine = line(initPlane.origin, initPlane.normal);
        var remainingTransform = getRemainderPatternTransform(context, { "references" : qCreatedBy(id + "extrude1", EntityType.BODY) });
        var revolveBody = qCreatedBy(id + "revolve1", EntityType.BODY);
        for (var i = 1; i < instanceCount; i += 1)
        {
            var instanceTransform = rotationAround(rotationLine, i * angle);
            transforms = append(transforms, instanceTransform);
            instanceNames = append(instanceNames, "" ~ i);
        }
        var toolsUnion = qUnion([body, revolveBody]);
        var patternMap = {
            "transforms" : transforms,
            "instanceNames" : instanceNames,
            "entities" : toolsUnion
        };
        applyPattern(context, id + "pattern1", patternMap, remainingTransform);
        var patternBodies = qUnion([qCreatedBy(id + "pattern1", EntityType.BODY), qCreatedBy(id + "extrude1", EntityType.BODY), qCreatedBy(id + "revolve1", EntityType.BODY)]);
        const booleanDefinition = {
                "operationType" : BooleanOperationType.SUBTRACTION,
                "tools" : patternBodies,
                "targets" : ownerBody,
                "keepTools" : false };
        opBoolean(context, id + "boolean2", booleanDefinition);
        opDeleteBodies(context, id + "delete_sketch", { "entities" : qUnion([qCreatedBy(id + "offsetSketch", EntityType.BODY), qCreatedBy(id + "profileSketch", EntityType.BODY)]) });
    });

function findProfile(context is Context, id is Id, profileArray is map, instanceCount is number, midPointArray is map, curveRadius is ValueWithUnits) returns map
{
    var profileDefinition = {};
    var splineRadius;
    for (var entry in profileArray)
    {
        if (tolerantEquals(entry.key, curveRadius))
        {
            splineRadius = entry.key;
            profileDefinition.splineProfile = entry.value;
            break;
        }
    }
    profileDefinition.angle = (360 / instanceCount) * degree;
    profileDefinition.arcMidPoints = midPointArray[splineRadius];
    profileDefinition.chamferSize = chamferDefinition[splineRadius];
    return profileDefinition;
}

// maps of spline vector information assosciated with shaft diameter
const fourSplineProfileArrays = {
        0.375 * inch : [vector(.251683 * inch, .124404 * inch), vector(.321055 * inch, .193775 * inch), vector(.321055 * inch, -.193775 * inch), vector(.251683 * inch, -.124404 * inch)],
        0.4375 * inch : [vector(.293786 * inch, .145293 * inch), vector(.374564 * inch, .226071 * inch), vector(.374564 * inch, -.226071 * inch), vector(.293786 * inch, -.145293 * inch)],
        0.5 * inch : [vector(.335888 * inch, .166183 * inch), vector(.428073 * inch, .258367 * inch), vector(.428073 * inch, -.258367 * inch), vector(.335888 * inch, -.166183 * inch)],
        0.5625 * inch : [vector(.377991 * inch, .187072 * inch), vector(.481582 * inch, .290663 * inch), vector(.481582 * inch, -.290663 * inch), vector(.377991 * inch, -.187072 * inch)],
        0.625 * inch : [vector(.41972 * inch, .207588 * inch), vector(.535091 * inch, .322959 * inch), vector(.535091 * inch, -.322959 * inch), vector(.41972 * inch, -.207588 * inch)],
        0.6875 * inch : [vector(.461823 * inch, .228478 * inch), vector(.5886 * inch, .355255 * inch), vector(.5886 * inch, -.355255 * inch), vector(.461823 * inch, -.228478 * inch)],
        0.750 * inch : [vector(.503926 * inch, .249367 * inch), vector(.642109 * inch, .387551 * inch), vector(.642109 * inch, -.387551 * inch), vector(.503926 * inch, -.249367 * inch)],
        0.8125 * inch : [vector(.546028 * inch, .270257 * inch), vector(.695618 * inch, .419847 * inch), vector(.695618 * inch, -.419847 * inch), vector(.546028 * inch, -.270257 * inch)],
        0.875 * inch : [vector(.587992 * inch, .2903 * inch), vector(.749394 * inch, .451702 * inch), vector(.749394 * inch, -.451702 * inch), vector(.587992 * inch, -.2903 * inch)],
        1.0 * inch : [vector(.671776 * inch, .332365 * inch), vector(.856146 * inch, .516734 * inch), vector(.856146 * inch, -.516734 * inch), vector(.671776 * inch, -.332365 * inch)],
        1.125 * inch : [vector(.755609 * inch, .373771 * inch), vector(.963164 * inch, .581326 * inch), vector(.963164 * inch, -.581326 * inch), vector(.755609 * inch, -.373771 * inch)],
        1.25 * inch : [vector(.839814 * inch, .41555 * inch), vector(1.070182 * inch, .645918 * inch), vector(1.070182 * inch, -.645918 * inch), vector(.839814 * inch, -.41555 * inch)],
        1.5 * inch : [vector(1.008085 * inch, .498261 * inch), vector(1.284485 * inch, .774661 * inch), vector(1.284485 * inch, -.774661 * inch), vector(1.008085 * inch, -.498261 * inch)],
    };

const fourSplineArcMidpointsArray = {
        0.375 * inch : [vector(.28075, 0) * inch, vector(.376, 0) * inch],
        0.4375 * inch : [vector(.32775, 0) * inch, vector(.4376, 0) * inch],
        0.5 * inch : [vector(.37475, 0) * inch, vector(.501, 0) * inch],
        0.5625 * inch : [vector(.42175, 0) * inch, vector(.5626, 0) * inch],
        0.625 * inch : [vector(.46825, 0) * inch, vector(.626, 0) * inch],
        0.6875 * inch : [vector(.51525, 0) * inch, vector(.6876, 0) * inch],
        0.75 * inch : [vector(.56225, 0) * inch, vector(.751, 0) * inch],
        0.8125 * inch : [vector(.60925, 0) * inch, vector(.8126, 0) * inch],
        0.875 * inch : [vector(.65575, 0) * inch, vector(.876, 0) * inch],
        1.0 * inch : [vector(.7495, 0) * inch, vector(1.001, 0) * inch],
        1.125 * inch : [vector(.843, 0) * inch, vector(1.126, 0) * inch],
        1.25 * inch : [vector(.937, 0) * inch, vector(1.251, 0) * inch],
        1.5 * inch : [vector(1.1245, 0) * inch, vector(1.501, 0) * inch],
    };

const sixSplineProfileArrays = {
        0.375 * inch : [vector(.293389 * inch, .061424 * inch), vector(.361253 * inch, .100605 * inch), vector(.361253 * inch, -.100605 * inch), vector(.293389 * inch, -.061424 * inch)],
        0.4375 * inch : [vector(.342307 * inch, .071769 * inch), vector(.421439 * inch, .117455 * inch), vector(.421439 * inch, -.117455 * inch), vector(.342307 * inch, -.071769 * inch)],
        0.5 * inch : [vector(.391225 * inch, .082114 * inch), vector(.481624 * inch, .134306 * inch), vector(.481624 * inch, -.134306 * inch), vector(.391225 * inch, -.082114 * inch)],
        0.5625 * inch : [vector(.440144 * inch, .092459 * inch), vector(.54181 * inch, .151156 * inch), vector(.54181 * inch, -.151156 * inch), vector(.440144 * inch, -.092459 * inch)],
        0.625 * inch : [vector(.48917 * inch, .102289 * inch), vector(.602134 * inch, .167509 * inch), vector(.602134 * inch, -.167509 * inch), vector(.48917 * inch, -.102289 * inch)],
        0.6875 * inch : [vector(.538088 * inch, .112634 * inch), vector(.66232 * inch, .18436 * inch), vector(.66232 * inch, -.18436 * inch), vector(.538088 * inch, -.112634 * inch)],
        0.75 * inch : [vector(.586898 * inch, .123494 * inch), vector(.722367 * inch, .201707 * inch), vector(.722367 * inch, -.201707 * inch), vector(.586898 * inch, -.123494 * inch)],
        0.8125 * inch : [vector(.635924 * inch, .133324 * inch), vector(.782691 * inch, .21806 * inch), vector(.782691 * inch, -.21806 * inch), vector(.635924 * inch, -.133324 * inch)],
        0.875 * inch : [vector(.68495 * inch, .143154 * inch), vector(.843016 * inch, .234413 * inch), vector(.843016 * inch, -.234413 * inch), vector(.68495 * inch, -.143154 * inch)],
        1.0 * inch : [vector(.782451 * inch, .164228 * inch), vector(.963249 * inch, .268611 * inch), vector(.963249 * inch, -.268611 * inch), vector(.782451 * inch, -.164228 * inch)],
        1.125 * inch : [vector(.880395 * inch, .184403 * inch), vector(1.083759 * inch, .301815 * inch), vector(1.083759 * inch, -.301815 * inch), vector(.880395 * inch, -.184403 * inch)],
        1.25 * inch : [vector(.978232 * inch, .205093 * inch), vector(1.20413 * inch, .335516 * inch), vector(1.20413 * inch, -.335516 * inch), vector(.978232 * inch, -.205093 * inch)],
        1.5 * inch : [vector(1.174012 * inch, .245958 * inch), vector(1.445012 * inch, .40242 * inch), vector(1.445012 * inch, -.40242 * inch), vector(1.174012 * inch, -.245958 * inch)],
    };

export const sixSplineArcMidpointsArray = {
        0.375 * inch : [vector(.29975, 0) * inch, vector(.376, 0) * inch],
        0.4375 * inch : [vector(.34975, 0) * inch, vector(.4376, 0) * inch],
        0.5 * inch : [vector(.39975, 0) * inch, vector(.501, 0) * inch],
        0.5625 * inch : [vector(.44975, 0) * inch, vector(.5626, 0) * inch],
        0.625 * inch : [vector(.49975, 0) * inch, vector(.626, 0) * inch],
        0.625 * inch : [vector(.49975, 0) * inch, vector(.626, 0) * inch],
        0.6875 * inch : [vector(.54975, 0) * inch, vector(.6876, 0) * inch],
        0.75 * inch : [vector(.59975, 0) * inch, vector(.751, 0) * inch],
        0.8125 * inch : [vector(.64975, 0) * inch, vector(.8126, 0) * inch],
        0.875 * inch : [vector(.69975, 0) * inch, vector(.876, 0) * inch],
        1.0 * inch : [vector(.7995, 0) * inch, vector(1.001, 0) * inch],
        1.125 * inch : [vector(.8995, 0) * inch, vector(1.126, 0) * inch],
        1.25 * inch : [vector(.9995, 0) * inch, vector(1.251, 0) * inch],
        1.5 * inch : [vector(1.1995, 0) * inch, vector(1.501, 0) * inch],
    };

const tenSplineProfileArrays = {
        0.375 * inch : [vector(.3017133 * inch, .037054 * inch), vector(.370278 * inch, .059326 * inch), vector(.370278 * inch, -.059326 * inch), vector(.3017133 * inch, -.037054 * inch)],
        0.4375 * inch : [vector(.351417 * inch, .042683 * inch), vector(.432044 * inch, .06888 * inch), vector(.432044 * inch, -.06888 * inch), vector(.351417 * inch, -.042683 * inch)],
        0.5 * inch : [vector(.402009 * inch, .049132 * inch), vector(.49373 * inch, .078934 * inch), vector(.49373 * inch, -.078934 * inch), vector(.402009 * inch, -.049132 * inch)],
        0.5625 * inch : [vector(.452177 * inch, .054918 * inch), vector(.555496 * inch, .088489 * inch), vector(.555496 * inch, -.088489 * inch), vector(.452177 * inch, -.054918 * inch)],
        0.625 * inch : [vector(.502769 * inch, .061368 * inch), vector(.617183 * inch, .098543 * inch), vector(.617183 * inch, -.098543 * inch), vector(.502769 * inch, -.061368 * inch)],
        0.6875 * inch : [vector(.552937 * inch, .067154 * inch), vector(.678949 * inch, .108097 * inch), vector(.678949 * inch, -.108097 * inch), vector(.552937 * inch, -.067154 * inch)],
        0.750 * inch : [vector(.603044 * inch, .073446 * inch), vector(.740635 * inch, .118152 * inch), vector(.740635 * inch, -.118152 * inch), vector(.603044 * inch, -.073446 * inch)],
        0.8125 * inch : [vector(.653212 * inch, .079232 * inch), vector(.802401 * inch, .127706 * inch), vector(.802401 * inch, -.127706 * inch), vector(.653212 * inch, -.079232 * inch)],
        0.875 * inch : [vector(.703804 * inch, .085681 * inch), vector(.864087 * inch, .13776 * inch), vector(.864087 * inch, -.13776 * inch), vector(.703804 * inch, -.085681 * inch)],
        1.0 * inch : [vector(.803533 * inch, .098107 * inch), vector(.98746 * inch, .157869 * inch), vector(.98746 * inch, -.157869 * inch), vector(.803533 * inch, -.098107 * inch)],
        1.125 * inch : [vector(.9049 * inch, .109488 * inch), vector(1.111072 * inch, .176478 * inch), vector(1.111072 * inch, -.176478 * inch), vector(.9049 * inch, -.109488 * inch)],
        1.25 * inch : [vector(1.005176 * inch, .121566 * inch), vector(1.234524 * inch, .196086 * inch), vector(1.234524 * inch, -.196086 * inch), vector(1.005176 * inch, -.121566 * inch)],
        1.5 * inch : [vector(1.206149 * inch, .146385 * inch), vector(1.48135 * inch, .235803 * inch), vector(1.48135 * inch, -.235803 * inch), vector(1.206149 * inch, -.146385 * inch)],
        1.75 * inch : [vector(1.406215 * inch, .170384 * inch), vector(1.728255 * inch, .27502 * inch), vector(1.728255 * inch, -.27502 * inch), vector(1.406215 * inch, -.170384 * inch)],
        2.0 * inch : [vector(1.607189 * inch, .195203 * inch), vector(1.97508 * inch, .314738 * inch), vector(1.97508 * inch, -.314738 * inch), vector(1.607189 * inch, -.195203 * inch)],
        2.25 * inch : [vector(1.808224 * inch, .219516 * inch), vector(2.221985 * inch, .353955 * inch), vector(2.221985 * inch, -.353955 * inch), vector(1.808224 * inch, -.219516 * inch)],
        2.5 * inch : [vector(2.009321 * inch, .243323 * inch), vector(2.468969 * inch, .392672 * inch), vector(2.468969 * inch, -.392672 * inch), vector(2.009321 * inch, -.243323 * inch)],
        2.75 * inch : [vector(2.210779 * inch, .2683 * inch), vector(2.715794 * inch, .432389 * inch), vector(2.715794 * inch, -.432389 * inch), vector(2.210779 * inch, -.2683 * inch)],
        3.0 * inch : [vector(2.41133 * inch, .292456 * inch), vector(2.962699 * inch, .471607 * inch), vector(2.962699 * inch, -.471607 * inch), vector(2.41133 * inch, -.292456 * inch)],
    };
export const tenSplineArcMidpointsArray = {
        0.375 * inch : [vector(.304, 0) * inch, vector(.376, 0) * inch],
        0.4375 * inch : [vector(.354, 0) * inch, vector(.4376, 0) * inch],
        0.5 * inch : [vector(.405, 0) * inch, vector(.501, 0) * inch],
        0.5625 * inch : [vector(.456, 0) * inch, vector(.5626, 0) * inch],
        0.625 * inch : [vector(.506, 0) * inch, vector(.626, 0) * inch],
        0.6875 * inch : [vector(.557, 0) * inch, vector(.6876, 0) * inch],
        0.750 * inch : [vector(.608, 0) * inch, vector(.751, 0) * inch],
        0.8125 * inch : [vector(.658, 0) * inch, vector(.8126, 0) * inch],
        0.875 * inch : [vector(.709, 0) * inch, vector(.876, 0) * inch],
        1.0 * inch : [vector(.81, 0) * inch, vector(1.001, 0) * inch],
        1.125 * inch : [vector(.911, 0) * inch, vector(1.126, 0) * inch],
        1.25 * inch : [vector(1.012, 0) * inch, vector(1.251, 0) * inch],
        1.5 * inch : [vector(1.215, 0) * inch, vector(1.501, 0) * inch],
        1.75 * inch : [vector(1.4165, 0) * inch, vector(1.751, 0) * inch],
        2.0 * inch : [vector(1.619, 0) * inch, vector(2.001, 0) * inch],
        2.25 * inch : [vector(1.8215, 0) * inch, vector(2.251, 0) * inch],
        2.5 * inch : [vector(2.024, 0) * inch, vector(2.501, 0) * inch],
        2.75 * inch : [vector(2.227, 0) * inch, vector(2.751, 0) * inch],
        3.0 * inch : [vector(2.429, 0) * inch, vector(3.001, 0) * inch],
    };

const sixteenSplineProfileArrays = {
        1.0 * inch : [vector(.806655 * inch, .061553 * inch), vector(.995084 * inch, .099034 * inch), vector(.995084 * inch, -.099034 * inch), vector(.806655 * inch, -.061553 * inch)],
        1.25 * inch : [vector(1.008585 * inch, .07674 * inch), vector(1.24388 * inch, .123543 * inch), vector(1.24388 * inch, -.123543 * inch), vector(1.008585 * inch, -.07674 * inch)],
        1.5 * inch : [vector(1.210515 * inch, .091926 * inch), vector(1.492676 * inch, .148051 * inch), vector(1.492676 * inch, -.148051 * inch), vector(1.210515 * inch, -.091926 * inch)],
        1.75 * inch : [vector(1.412444 * inch, .107112 * inch), vector(1.741472 * inch, .172576 * inch), vector(1.741472 * inch, -.172576 * inch), vector(1.412444 * inch, -.107112 * inch)],
        2.0 * inch : [vector(1.614374 * inch, .122299 * inch), vector(1.990267 * inch, .197069 * inch), vector(1.990267 * inch, -.197069 * inch), vector(1.614374 * inch, -.122299 * inch)],
        2.25 * inch : [vector(1.816304 * inch, .137485 * inch), vector(2.239063 * inch, .221577 * inch), vector(2.239063 * inch, -.221577 * inch), vector(1.816304 * inch, -.137485 * inch)],
        2.5 * inch : [vector(2.018234 * inch, .152671 * inch), vector(2.487859 * inch, .246086 * inch), vector(2.487859 * inch, -.246086 * inch), vector(2.018234 * inch, -.152671 * inch)],
        2.75 * inch : [vector(2.220164 * inch, .167858 * inch), vector(2.736655 * inch, .270594 * inch), vector(2.736655 * inch, -.270594 * inch), vector(2.220164 * inch, -.167858 * inch)],
        3.0 * inch : [vector(2.422587 * inch, .183142 * inch), vector(2.98545 * inch, .295103 * inch), vector(2.98545 * inch, -.295103 * inch), vector(2.422587 * inch, -.183142 * inch)],
    };

const sixteenSplineArcMidpointsArray = {
        1.0 * inch : [vector(.809, 0) * inch, vector(1.001, 0) * inch],
        1.25 * inch : [vector(1.0115, 0) * inch, vector(1.251, 0) * inch],
        1.5 * inch : [vector(1.214, 0) * inch, vector(1.501, 0) * inch],
        1.75 * inch : [vector(1.4165, 0) * inch, vector(1.751, 0) * inch],
        2.0 * inch : [vector(1.619, 0) * inch, vector(2.001, 0) * inch],
        2.25 * inch : [vector(1.8215, 0) * inch, vector(2.251, 0) * inch],
        2.5 * inch : [vector(2.024, 0) * inch, vector(2.501, 0) * inch],
        2.75 * inch : [vector(2.2265, 0) * inch, vector(2.751, 0) * inch],
        3.0 * inch : [vector(2.4295, 0) * inch, vector(3.001, 0) * inch],
    };

const chamferDefinition = {
        0.375 * inch : (.05 * inch),
        0.4375 * inch : (.06 * inch),
        0.5 * inch : (.07 * inch),
        0.5625 * inch : (.08 * inch),
        0.625 * inch : (.09 * inch),
        0.6875 * inch : (.1 * inch),
        0.750 * inch : (.11 * inch),
        0.8125 * inch : (.12 * inch),
        0.875 * inch : (.13 * inch),
        1.0 * inch : (.14 * inch),
        1.125 * inch : (.15 * inch),
        1.25 * inch : (.16 * inch),
        1.5 * inch : (.17 * inch),
        1.75 * inch : (.18 * inch),
        2.0 * inch : (.2 * inch),
        2.25 * inch : (.22 * inch),
        2.5 * inch : (.24 * inch),
        2.75 * inch : (.26 * inch),
        3.0 * inch : (.28 * inch),
    };
