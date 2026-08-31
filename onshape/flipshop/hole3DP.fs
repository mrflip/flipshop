FeatureScript 2837;
import(path : "onshape/std/geometry.fs", version : "2837.0");

// Import Hole Tools
BasicHole::import(path : "42452d0d1f5d09a3406f73ac", version : "c49bb9e8c205683f91a7bf7a");
SelfTapping::import(path : "ca0f63acd016b867f9c2aa8b", version : "b4e38e23c37449b507c62026");
TearHole::import(path : "a76776f84ec6cffe1563a34a", version : "51f22b8d7dd01ec71b95d5a0");
IconNamespace::import(path : "bc5e3a00dc2e900fd9de64f9", version : "3aed6b7999f0466af80794c5");


// Define Variables
//const mm = millimeter;
//const deg = degree;

export enum FDMHoleEndStyle
{
    annotation { "Name" : "Blind" }
    BLIND,
    annotation { "Name" : "Up to next" }
    UP_TO_NEXT,
    annotation { "Name" : "Up to entity" }
    UP_TO_ENTITY,
    annotation { "Name" : "Through all" }
    THROUGH
}


/**
 * Defines the shape of the cut hole.
 * @value SPLIT : This adds a vertical cut through the hole.
 * @value THREAD : This adds pillars to the hole where the thread of
 * the screw can bind into.
 * @value TEAR : The Hole has a tear shape with an angle on that side.
 */
export enum HoleType
{
    annotation { "Name" : "Simple Hole" }
    SIMPLE,
    annotation { "Name" : "Splited Hole" }
    SPLIT,
    annotation { "Name" : "Self Tapping" }
    THREAD,
    annotation { "Name" : "Tear Shaped" }
    TEAR
}

/**
 * Defines thread sizes available for the THREAD HoleType
 */
export enum ThreadType
{
    annotation { "Name" : "M2" }
    M2,
    annotation { "Name" : "M2.5" }
    M2_5,
    annotation { "Name" : "M3" }
    M3,
    annotation { "Name" : "M4" }
    M4,
    annotation { "Name" : "M5" }
    M5
}

//, "Icon" : IconNamespace::BLOB_DATA
annotation { "Feature Type Name" : "3D Printing Hole", "Feature Type Description" : "Create Custom Holes designed for 3D Printing", "Icon" : IconNamespace::BLOB_DATA }
export const FDMHoleFeature = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Hole Method" }
        definition.method is HoleType;

        annotation { "Name" : "Plane", "Filter" : EntityType.FACE, "MaxNumberOfPicks" : 1 }
        definition.plane is Query;
        annotation { "Name" : "Opposite direction", "UIHint" : "OPPOSITE_DIRECTION" }
        definition.flipDir is boolean;

        annotation { "Name" : "Sketch Points", "Filter" : EntityType.VERTEX }
        definition.skPoints is Query;

        annotation { "Name" : "Target Bodies", "Filter" : EntityType.BODY }
        definition.targetBody is Query;


        if (definition.method == HoleType.SIMPLE || definition.method == HoleType.SPLIT || definition.method == HoleType.TEAR)
        {
            annotation { "Name" : "diameter" }
            isLength(definition.diameter, { (millimeter) : [0.001, 5, 1000] } as LengthBoundSpec);

            //annotation { "Name" : "Blind" }
            //isLength(definition.depth, { (millimeter) : [0.001, 25, 1000] } as LengthBoundSpec);

            annotation { "Name" : "Termination", "UIHint" : ["REMEMBER_PREVIOUS_VALUE", "SHOW_LABEL"] }
            definition.endStyle is FDMHoleEndStyle;

            if (definition.endStyle == FDMHoleEndStyle.UP_TO_ENTITY || definition.endStyle == FDMHoleEndStyle.UP_TO_NEXT)
            {
                if (definition.endStyle == FDMHoleEndStyle.UP_TO_ENTITY)
                {
                    annotation { "Name" : "Up to entity or mate connector",
                                "Filter" : (EntityType.FACE && SketchObject.NO && AllowMeshGeometry.YES) || QueryFilterCompound.ALLOWS_VERTEX,
                                "MaxNumberOfPicks" : 1 }
                    definition.endBoundEntity is Query;
                }

                annotation { "Name" : "Offset from tip", "Column Name" : "Has offset", "UIHint" : ["DISPLAY_SHORT", "FIRST_IN_ROW"] }
                definition.offset is boolean;

                if (definition.offset)
                {
                    annotation { "Name" : "Offset from tip", "UIHint" : UIHint.DISPLAY_SHORT }
                    isLength(definition.offsetDistance, ZERO_INCLUSIVE_OFFSET_BOUNDS);

                    annotation { "Name" : "Opposite direction", "Column Name" : "Offset opposite direction", "UIHint" : UIHint.OPPOSITE_DIRECTION }
                    definition.oppositeOffsetDirection is boolean;
                }
            }

            if (definition.endStyle == FDMHoleEndStyle.BLIND)
            {
                annotation { "Name" : "Distance" }
                isLength(definition.depth, ZERO_INCLUSIVE_OFFSET_BOUNDS);

            }


            if (definition.method == HoleType.SPLIT)
            {

                annotation { "Name" : "Split Height" }
                isLength(definition.split_height, { (millimeter) : [0.001, 0.5, 1000] } as LengthBoundSpec);


            }
            else if (definition.method == HoleType.TEAR)
            {

                annotation { "Name" : "layerHeight" }
                isLength(definition.layerHeight, { (millimeter) : [0.001, 0.2, 1000] } as LengthBoundSpec);

                annotation { "Name" : "Angle" }
                isAngle(definition.angle, { (degree) : [60, 120, 179] } as AngleBoundSpec);

                annotation { "Name" : "Bottom Teardrop" }
                definition.bottomTear is boolean;

                annotation { "Name" : "Chamfer Distance" }
                isLength(definition.chamferDist, { (millimeter) : [0.001, 0.6, 1000] } as LengthBoundSpec);
            }


        }
        else if (definition.method == HoleType.THREAD)
        {
            annotation { "Name" : "Thread" }
            definition.thread is ThreadType;

            annotation { "Name" : "Blind" }
            isLength(definition.depthT, { (millimeter) : [0.001, 25, 1000] } as LengthBoundSpec);

            annotation { "Name" : "Count" }
            isInteger(definition.thread_count, { (unitless) : [2, 3, 6] } as IntegerBoundSpec);

        }
        annotation { "Name" : "Hole Rotation" }
        isAngle(definition.rotation, { (degree) : [-360, 0, 360] } as AngleBoundSpec);

    }
    {
        // Specify what the feature does when regenerating
        forEachEntity(context, id + "operation", definition.skPoints, function(entity is Query, id is Id)
            {
                // Get Target Coordinate System
                var anchorPoint = evVertexPoint(context, { "vertex" : entity });

                var facePlane = evPlane(context, { "face" : definition.plane });
                var faceNormal = facePlane.normal;
                faceNormal = definition.flipDir ? -faceNormal : faceNormal;

                var targetPlane = plane(anchorPoint, faceNormal);
                var targetCSYS = coordSystem(targetPlane);

                var sourceCSYS = WORLD_COORD_SYSTEM;

                var transformMatrix = toWorld(targetCSYS) * fromWorld(sourceCSYS);


                // --- Calculate Dynamic Depth Based on End Style ---
                var computedDepth = 0 * millimeter;

                if (definition.method == HoleType.THREAD)
                {
                    computedDepth = definition.depthT;
                }
                else
                {
                    if (definition.endStyle == FDMHoleEndStyle.BLIND)
                    {
                        computedDepth = definition.depth;
                    }
                    else if (definition.endStyle == FDMHoleEndStyle.UP_TO_NEXT)
                    {
                        // Raycast to find the first face intersection along the hole axis
                        var hits = evRaycast(context, {
                                "ray" : line(anchorPoint + normalize(faceNormal) * 0.001 * millimeter, faceNormal),
                                "entities" : definition.targetBody
                            });
                        var validHitFound = false;
                        for (var hit in hits)
                        {
                            // Ignore the starting face intersection (near 0mm distance)
                            if (hit.distance > 0.001 * millimeter)
                            {
                                computedDepth = hit.distance;
                                validHitFound = true;
                                break;
                            }
                        }

                        println(hits);

                        if (!validHitFound)
                        {
                            computedDepth = 5 * millimeter; // Fallback default value
                            println("Can't find next entity!");
                        }
                    }
                    else if (definition.endStyle == FDMHoleEndStyle.UP_TO_ENTITY)
                    {
                        // Raycast directly against the selected target entity
                        var hits = evRaycast(context, {
                                "ray" : line(anchorPoint, faceNormal),
                                "entities" : definition.endBoundEntity
                            });
                        var baseDistance = 0 * millimeter;
                        var validHitFound = false;

                        for (var hit in hits)
                        {
                            if (hit.distance > 0.05 * millimeter)
                            {
                                baseDistance = hit.distance;
                                validHitFound = true;
                                break;
                            }
                        }

                        if (!validHitFound)
                        {
                            // Fallback to minimum distance evaluation if raycast misses surface boundaries
                            var distResult = evDistance(context, {
                                    "sideA" : entity,
                                    "sideB" : definition.endBoundEntity
                                });
                            baseDistance = distResult.distance;
                        }

                        // Apply optional offset logic
                        if (definition.offset)
                        {
                            if (definition.oppositeOffsetDirection)
                            {
                                baseDistance -= definition.offsetDistance;
                            }
                            else
                            {
                                baseDistance += definition.offsetDistance;
                            }
                        }
                        computedDepth = baseDistance;
                    }
                    else if (definition.endStyle == FDMHoleEndStyle.THROUGH)
                    {
                        var hits = evRaycast(context, {
                                "ray" : line(anchorPoint, faceNormal),
                                "entities" : definition.targetBody
                            });

                        var validHitFound = false;
                        // Scan backwards to find the last valid exit boundary hit
                        for (var i = size(hits) - 1; i >= 0; i -= 1)
                        {
                            if (hits[i].distance > 0.05 * millimeter)
                            {
                                computedDepth = hits[i].distance + 1 * millimeter;
                                validHitFound = true;
                                break;
                            }
                        }

                        if (!validHitFound)
                        {
                            computedDepth = 50 * millimeter; // Fallback default value
                        }
                    }
                }

                // Safety constraint to prevent non-positive depth failures in the tool builds
                if (computedDepth < 0.001 * millimeter)
                {
                    computedDepth = 0.001 * millimeter;
                }
                // --- End of Depth Calculation ---


                // define special cases
                var splited = false;
                if (definition.method == HoleType.SPLIT)
                {
                    splited = true;
                }

                // start instancing
                const instantiator = newInstantiator(id + "cutter");

                var METHOD;
                var CONFIG;

                if (definition.method == HoleType.THREAD)
                {
                    METHOD = SelfTapping::build;
                    CONFIG = {
                            "config" : definition.thread,
                            "depth" : computedDepth,
                            "threads" : definition.thread_count
                        };
                }
                else if (definition.method == HoleType.SIMPLE || definition.method == HoleType.SPLIT)
                {
                    METHOD = BasicHole::build;
                    CONFIG = {
                            "diameter" : definition.diameter,
                            "depth" : computedDepth,
                            "split" : splited,
                            "height" : definition.split_height
                        };

                }
                else if (definition.method == HoleType.TEAR)
                {
                    METHOD = TearHole::build;
                    CONFIG = {
                            "diameter" : definition.diameter,
                            "depth" : computedDepth,
                            "angle" : definition.angle,
                            "layerHeight": definition.layerHeight,
                            "chamferDist": definition.chamferDist,
                            "bottomTear": definition.bottomTear,

                        };
                }

                var firstQuery = addInstance(instantiator, METHOD, {
                        "configuration" : CONFIG,
                        "transform" : transformMatrix
                    });

                instantiate(context, instantiator);

                opTransform(context, id + "transform1", {
                            "bodies" : firstQuery,
                            "transform" : rotationAround(line(anchorPoint, faceNormal), definition.rotation)
                        });


                opBoolean(context, id + "boolean1", {
                            "tools" : firstQuery,
                            "targets" : definition.targetBody,
                            "operationType" : BooleanOperationType.SUBTRACTION
                        });

            });

    });

