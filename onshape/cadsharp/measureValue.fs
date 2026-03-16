
//_______________________________________________________________________________________________________________________________________________
//
// This FeatureScript is owned by Michael Pascoe and is distributed by CADSharp LLC.
// You may not redistribute it for commercial purposes without the permission of said owner and CADSharp LLC. Copyright (c) 2023 Michael Pascoe.
//_______________________________________________________________________________________________________________________________________________


FeatureScript 1389;
import(path : "onshape/std/geometry.fs", version : "1389.0");

// Developed by Konstantin Sh

// CADSharp
export import(path : "cbeb3dcf671e00785597bd76/a3325fbbabaf7b91c6426e93/a75ab01def146a42f55baa7f", version : "0ff94c45bfdcc4c8708eb3fc");

IconNamespace::import(path : "63d4be0a1f59761ee7a156bb", version : "96e41006d20f1a0fc0009246");

export enum MeasureType
{
    annotation { "Name" : "Distance" }
    DISTANCE,
    annotation { "Name" : "Angle" }
    ANGLE,
    annotation { "Name" : "Diameter" }
    DIAMETER,
    annotation { "Name" : "Perimeter" }
    PERIMETER,
    annotation { "Name" : "Area" }
    AREA,
    annotation { "Name" : "Volume" }
    VOLUME,
    annotation { "Name" : "Count" }
    COUNT,
    annotation { "Name" : "Coordinate" }
    COORDINATE,
    annotation { "Name" : "Centroid" }
    CENTROID,
    annotation { "Name" : "Bounding box" }
    BOX,
    annotation { "Name" : "Inertia" }
    MASS_PROPERTIES
}

export enum MeasureMethod
{
    annotation { "Name" : "Infinite propagation" }
    INF_PROP,
    annotation { "Name" : "Minimum" }
    MIN,
    annotation { "Name" : "Maximum" }
    MAX
}

export enum AxisName
{
    annotation { "Name" : "X axis" }
    X,
    annotation { "Name" : "Y axis" }
    Y,
    annotation { "Name" : "Z Axis" }
    Z
}

export enum CoordinateType
{
    annotation { "Name" : "Global" }
    GLOBAL,
    annotation { "Name" : "Local" }
    LOCAL,
    annotation { "Name" : "Along direction" }
    ALONG_DIR
}

/** Measure the distance between two entities and set the result to a variable */
annotation { "Feature Type Name" : "Measure value",
        "Feature Name Template" : "###name = #d",
        "Feature Type Description" : "<br> <b>Summary</b> <br> Creates variables of measured values",
        "Description Image" : cadsharpLogo::BLOB_DATA,
        "UIHint" : "NO_PREVIEW_PROVIDED",
        "Icon" : IconNamespace::BLOB_DATA,
        "Editing Logic Function" : "cadsharpUrlEditLogic" }
export const measureValue = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Name", "Default" : "d", "UIHint" : [UIHint.UNCONFIGURABLE, UIHint.VARIABLE_NAME] }
        definition.name is string;

        annotation { "Name" : "Measure type", "UIHint" : UIHint.SHOW_LABEL }
        definition.measureType is MeasureType;

        if (definition.measureType == MeasureType.DISTANCE)
        {
            annotation { "Name" : "Entities to measure distance",
                        "Filter" : (EntityType.VERTEX || EntityType.EDGE || EntityType.FACE || EntityType.BODY || BodyType.MATE_CONNECTOR) && AllowFlattenedGeometry.YES,
                        "UIHint" : UIHint.PREVENT_CREATING_NEW_MATE_CONNECTORS,
                        "MaxNumberOfPicks" : 2
                    }
            definition.entitiesForDist is Query;

            annotation { "Name" : "Center distance" }
            definition.distToAxis is boolean;

            if (!definition.distToAxis)
            {
                annotation { "Name" : "Measure method", "UIHint" : UIHint.SHOW_LABEL }
                definition.measureMethod is MeasureMethod;
            }
        }
        else if (definition.measureType == MeasureType.ANGLE)
        {
            annotation { "Name" : "Entities to measure angle", "Filter" : QueryFilterCompound.ALLOWS_AXIS || GeometryType.PLANE || EntityType.EDGE, "MaxNumberOfPicks" : 2 }
            definition.entitiesForAng is Query;

            annotation { "Name" : "Flip angle" }
            definition.flipAngle is boolean;
        }
        else if (definition.measureType == MeasureType.DIAMETER)
        {
            annotation { "Name" : "Circular edge, cylinder, sphere", "Filter" : GeometryType.CIRCLE || GeometryType.ARC || GeometryType.CYLINDER || GeometryType.SPHERE, "MaxNumberOfPicks" : 1 }
            definition.diameter is Query;
        }
        else if (definition.measureType == MeasureType.PERIMETER)
        {
            annotation { "Name" : "Edges or faces", "Filter" : EntityType.FACE || EntityType.EDGE || BodyType.WIRE }
            definition.perimQuery is Query;
        }
        else if (definition.measureType == MeasureType.AREA)
        {
            annotation { "Name" : "Faces", "Filter" : EntityType.FACE }
            definition.faceQuery is Query;
        }
        else if (definition.measureType == MeasureType.VOLUME)
        {
            annotation { "Name" : "Solids", "Filter" : EntityType.BODY && BodyType.SOLID }
            definition.solidQuery is Query;
        }
        else if (definition.measureType == MeasureType.COUNT)
        {
            annotation { "Name" : "Faces or edges", "Filter" : EntityType.FACE || EntityType.EDGE, "MaxNumberOfPicks" : 1 }
            definition.countQ is Query;
        }
        else if (definition.measureType == MeasureType.COORDINATE)
        {
            annotation { "Name" : "Vertex", "Filter" : EntityType.VERTEX, "MaxNumberOfPicks" : 1 }
            definition.vertex is Query;

            annotation { "Name" : "Coordinate type" }
            definition.coordType is CoordinateType;

            if (definition.coordType == CoordinateType.LOCAL)
            {
                annotation { "Name" : "Local coordinate system", "Filter" : BodyType.MATE_CONNECTOR, "MaxNumberOfPicks" : 1 }
                definition.mc is Query;
            }
            else if (definition.coordType == CoordinateType.ALONG_DIR)
            {
                annotation { "Name" : "Direction", "Filter" : QueryFilterCompound.ALLOWS_AXIS || GeometryType.PLANE, "MaxNumberOfPicks" : 1 }
                definition.dir is Query;
            }
        }
        else if (definition.measureType == MeasureType.CENTROID)
        {
            annotation { "Name" : "Entities", "Filter" : EntityType.BODY || EntityType.FACE || EntityType.EDGE || EntityType.VERTEX }
            definition.entitiesForCentroid is Query;
        }
        else if (definition.measureType == MeasureType.BOX)
        {
            annotation { "Name" : "Entities", "Filter" : EntityType.BODY || EntityType.FACE || EntityType.EDGE || EntityType.VERTEX || AllowMeshGeometry.YES }
            definition.entitiesForBox is Query;

            annotation { "Name" : "Coordinate system", "Filter" : BodyType.MATE_CONNECTOR, "MaxNumberOfPicks" : 1 }
            definition.boxMC is Query;
        }
        else if (definition.measureType == MeasureType.MASS_PROPERTIES)
        {
            annotation { "Name" : "Entities", "Filter" : EntityType.BODY || EntityType.FACE || EntityType.EDGE || EntityType.VERTEX }
            definition.massPropEntities is Query;

            annotation { "Name" : "Reference frame", "Filter" : BodyType.MATE_CONNECTOR, "MaxNumberOfPicks" : 1 }
            definition.refFrame is Query;

            annotation { "Name" : "Density" }
            isAnything(definition.density);

            annotation { "Name" : "Axis name", "UIHint" : UIHint.SHOW_LABEL }
            definition.axisName is AxisName;
        }

        annotation { "Name" : "Hide preview" }
        definition.hidePreview is boolean;

        if (definition.measureType == MeasureType.DISTANCE ||
            definition.measureType == MeasureType.DIAMETER ||
            definition.measureType == MeasureType.PERIMETER ||
            definition.measureType == MeasureType.AREA ||
            definition.measureType == MeasureType.VOLUME ||
            definition.measureType == MeasureType.ANGLE ||
            definition.measureType == MeasureType.COUNT)
        {
            annotation { "Name" : "Round" }
            definition.round is boolean;

            if (definition.round)
                annotation { "Name" : "Rounding multiple" }
                definition.roundingPrecision->isReal({ (unitless) : [10e-6, 0.001, 10e6] } as RealBoundSpec);
        }

        annotation { "Name" : "Invisible length", "UIHint" : UIHint.ALWAYS_HIDDEN }
        isLength(definition.lengthUnit, {
                        (inch) : [-10000, 1.0, 10000],
                        (meter) : 1,
                        (centimeter) : 1,
                        (millimeter) : 1,
                        (foot) : 1,
                        (yard) : 1 } as LengthBoundSpec);

        cadsharpUrlPredicate(definition);
    }
    {
        var result;

        if (!match(definition.name, '[a-zA-Z_][a-zA-Z_0-9]*').hasMatch)
            throw regenError(ErrorStringEnum.VARIABLE_NAME_INVALID);

        if (definition.measureType == MeasureType.DISTANCE)
        {
            var max = false;
            var infExt = false;
            var side0 = qNthElement(definition.entitiesForDist, 0);
            var side1 = qNthElement(definition.entitiesForDist, 1);

            if (definition.measureMethod == MeasureMethod.MAX)
                max = true;
            else if (definition.measureMethod == MeasureMethod.INF_PROP)
                infExt = true;

            if (definition.distToAxis)
            {
                try silent
                {
                    side0 = evAxis(context, { "axis" : side0 });
                }

                try silent
                {
                    side1 = evAxis(context, { "axis" : side1 });
                }

                try silent
                {
                    if (parallelVectors(side0.direction, side1.direction))
                        side0 = evDistance(context, {
                                        "side0" : side0,
                                        "side1" : qNthElement(definition.entitiesForDist, 1)
                                    }).sides[0].point;
                }
            }

            const distanceResult is DistanceResult = evDistance(context, {
                        "side0" : side0,
                        "extendSide0" : infExt,
                        "side1" : side1,
                        "extendSide1" : infExt,
                        "maximum" : max
                    });

            result = distanceResult.distance;

            const p0 = distanceResult.sides[0].point;
            const p1 = distanceResult.sides[1].point;
            if (!definition.hidePreview)
            {
                if (!tolerantEquals(p0, p1))
                    debug(context, p0, p1, DebugColor.BLUE);
                else // If it's the same point, just draw the point
                    addDebugPoint(context, p0, DebugColor.BLUE);
            }
        }
        else if (definition.measureType == MeasureType.ANGLE)
        {

            var firstDir = extractDirection(context, qNthElement(definition.entitiesForAng, 0));
            var secondDir = extractDirection(context, qNthElement(definition.entitiesForAng, 1));

            if (firstDir is undefined || secondDir is undefined)
            {
                if (size(evaluateQuery(context, qEntityFilter(definition.entitiesForAng, EntityType.EDGE))) != 2)
                    throw regenError("Two entities with direction or two edges expected");

                const distResult = evDistance(context, {
                            "side0" : qNthElement(definition.entitiesForAng, 0),
                            "side1" : qNthElement(definition.entitiesForAng, 1)
                        });
                println("cathc block");
                firstDir = evEdgeTangentLine(context, {
                                "edge" : qNthElement(definition.entitiesForAng, 0),
                                "parameter" : distResult.sides[0].parameter
                            }).direction;

                secondDir = evEdgeTangentLine(context, {
                                "edge" : qNthElement(definition.entitiesForAng, 1),
                                "parameter" : distResult.sides[1].parameter
                            }).direction;
            }

            result = angleBetween(firstDir, secondDir);

            if (definition.flipAngle)
                result = 180 * degree - result;
        }
        else if (definition.measureType == MeasureType.DIAMETER)
        {
            try silent
            {
                result = evCurveDefinition(context, { "edge" : definition.diameter }).radius;
            }
            try silent
            {
                result = evSurfaceDefinition(context, { "face" : definition.diameter }).radius;
            }
            result *= 2;
        }
        else if (definition.measureType == MeasureType.PERIMETER)
        {
            const edges = qUnion([
                        /*face perimeter edges*/
                        qLoopEdges(qEntityFilter(definition.perimQuery, EntityType.FACE)),
                        /*single edges*/
                        qEntityFilter(definition.perimQuery, EntityType.EDGE),
                        /*wire body edges*/
                        qOwnedByBody(qBodyType(definition.perimQuery, BodyType.WIRE), EntityType.EDGE)
                    ]);

            if (!definition.hidePreview)
                addDebugEntities(context, edges, DebugColor.BLUE);

            result = evLength(context, { "entities" : edges });
        }
        else if (definition.measureType == MeasureType.AREA)
        {
            result = evArea(context, { "entities" : definition.faceQuery });
        }
        else if (definition.measureType == MeasureType.VOLUME)
        {
            result = evVolume(context, { "entities" : definition.solidQuery });
        }
        else if (definition.measureType == MeasureType.COUNT)
        {
            var matchingQ = qMatching(definition.countQ);

            if (!definition.hidePreview)
                addDebugEntities(context, matchingQ, DebugColor.BLUE);

            result = size(evaluateQuery(context, matchingQ));
        }
        else if (definition.measureType == MeasureType.COORDINATE)
        {
            if (definition.coordType == CoordinateType.GLOBAL)
            {
                result = evVertexPoint(context, { "vertex" : definition.vertex });
            }
            else if (definition.coordType == CoordinateType.LOCAL)
            {
                result = evVertexPoint(context, { "vertex" : definition.vertex });
                definition.mc = evMateConnector(context, { "mateConnector" : definition.mc });
                result = fromWorld(definition.mc, result);
            }
            else if (definition.coordType == CoordinateType.ALONG_DIR)
            {
                result = evVertexPoint(context, { "vertex" : definition.vertex });
                definition.dir = extractDirection(context, definition.dir);
                result = dot(result, definition.dir);
            }
        }
        else if (definition.measureType == MeasureType.CENTROID)
        {
            result = evApproximateCentroid(context, { "entities" : definition.entitiesForCentroid });
            opPoint(context, id + "centroid", {
                        "point" : result
                    });
        }
        else if (definition.measureType == MeasureType.BOX)
        {
            definition.boxMC = evMateConnector(context, { "mateConnector" : definition.boxMC });

            var boundingBox = evBox3d(context, {
                    "topology" : definition.entitiesForBox,
                    "cSys" : definition.boxMC,
                    "tight" : true
                });

            result = boundingBox.maxCorner - boundingBox.minCorner;

            if (!definition.hidePreview)
            {
                boundingBox.minCorner = toWorld(definition.boxMC) * boundingBox.minCorner;
                boundingBox.maxCorner = toWorld(definition.boxMC) * boundingBox.maxCorner;
                debug(context, boundingBox, DebugColor.BLUE);
            }
        }
        else if (definition.measureType == MeasureType.MASS_PROPERTIES)
        {
            result = evApproximateMassProperties(context, {
                        "entities" : definition.massPropEntities,
                        "density" : definition.density,
                        "referenceFrame" : evMateConnector(context, { "mateConnector" : definition.refFrame })
                    });

            if (!definition.hidePreview)
                println(result.inertia);

            var axisNumber = { AxisName.X : 0, AxisName.Y : 1, AxisName.Z : 2 }[definition.axisName];

            result = get(result.inertia, axisNumber, axisNumber);
        }

        if (definition.measureType == MeasureType.DISTANCE ||
            definition.measureType == MeasureType.DIAMETER ||
            definition.measureType == MeasureType.PERIMETER ||
            definition.measureType == MeasureType.AREA ||
            definition.measureType == MeasureType.VOLUME ||
            definition.measureType == MeasureType.ANGLE ||
            definition.measureType == MeasureType.COUNT)
            if (definition.round)
        {
            var resultUnit = switch (definition.lengthUnit) {
                    (inch) : inch,
                    (meter) : meter,
                    (centimeter) : centimeter,
                    (millimeter) : millimeter,
                    (foot) : foot,
                    (yard) : yard,
                };

            if (definition.measureType == MeasureType.ANGLE)
                resultUnit = degree;
            else if (definition.measureType == MeasureType.AREA)
                resultUnit = resultUnit ^ 2;
            else if (definition.measureType == MeasureType.VOLUME)
                resultUnit = resultUnit ^ 3;
            else if (definition.measureType == MeasureType.COUNT)
                resultUnit = 1;

            var unitlessResult = result / resultUnit;

            result = round(unitlessResult, definition.roundingPrecision) * resultUnit;
        }

        setVariable(context, definition.name, result);
        setFeatureComputedParameter(context, id, { "name" : "d", "value" : result });
    });
