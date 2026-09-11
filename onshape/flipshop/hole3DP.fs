FeatureScript 3044;
import(path : "onshape/std/common.fs", version : "3044.0");
// Utils
import(path : "14a20c5c0c7e0354a621f347/3a1e5c5e492768208bbb5694/8c588debec029dab0d734198", version : "3cc43cf8c59a339d5ce548d2");
import(path : "14a20c5c0c7e0354a621f347/3a1e5c5e492768208bbb5694/4ebdc64943b566160ea5cc28", version : "aa1e3063ddbe9d05678234b7");
import(path : "14a20c5c0c7e0354a621f347/3a1e5c5e492768208bbb5694/9935c9eba0658e8e5d6b672b", version : "3f735e5f0e03a21a2b73182f");

// Import Hole Tools
BasicHole::import(path : "42452d0d1f5d09a3406f73ac", version : "35bdde628945bd5114060e99");
SelfTapping::import(path : "ca0f63acd016b867f9c2aa8b", version : "b4e38e23c37449b507c62026");
TearHole::import(path : "a76776f84ec6cffe1563a34a", version : "32323d1ce06ad2ed580fbf34");
Wings::import(path : "526e2eb84c79796c0543822d", version : "b33866b6811b0b073b114b58");
IconNamespace::import(path : "bc5e3a00dc2e900fd9de64f9", version : "3aed6b7999f0466af80794c5");


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
const FDMHoleEndStyleTitles = { FDMHoleEndStyle.BLIND: "blind", FDMHoleEndStyle.UP_TO_NEXT: "⇒X", FDMHoleEndStyle.UP_TO_ENTITY: "⇒Y", FDMHoleEndStyle.THROUGH: "thru" };


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
const HoleTypeTitles = { HoleType.SIMPLE: "Hole", HoleType.TEAR: "Tear", HoleType.THREAD: "Thread", HoleType.SPLIT: "Split" };

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

/**
 * Applies the optional "offset from tip" to a raycast distance.
 */
function applyTipOffset(definition is map, distance is ValueWithUnits) returns ValueWithUnits
{
    if (definition.offset != true)
    {
        return distance;
    }
    if (definition.oppositeOffsetDirection)
    {
        return distance - definition.offsetDistance;
    }
    return distance + definition.offsetDistance;
}

annotation {
    "Feature Type Name" : "3D Printing Hole",
    "Feature Type Description" : "Create Custom Holes designed for 3D Printing, located and oriented by mate connectors",
    "Icon" : IconNamespace::BLOB_DATA,
    "Feature Name Template": "#displayTitle",
    "Editing Logic Function" : "fdmHoleFeatureEditLogic"
}
export const FDMHoleFeature = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Mate connectors", "Filter" : BodyType.MATE_CONNECTOR }
        definition.mateConnectors is Query;

        annotation { "Name" : "Opposite direction", "UIHint" : "OPPOSITE_DIRECTION" }
        definition.flipDir is boolean;

        annotation { "Name" : "Target Bodies", "Filter" : EntityType.BODY && BodyType.SOLID }
        definition.targetBody is Query;

        annotation { "Name" : "Hole Method" }
        definition.method is HoleType;

        annotation { "Name" : "Feature Display Title", "UIHint" : [UIHint.ALWAYS_HIDDEN] } // UIHint.READ_ONLY
        definition.displayTitle is string;

        if (definition.method == HoleType.SIMPLE || definition.method == HoleType.SPLIT || definition.method == HoleType.TEAR)
        {
            annotation { "Name" : "diameter" }
            isLength(definition.diameter, { (millimeter) : [0.001, 5, 1000] } as LengthBoundSpec);

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
        annotation { "Name" : "Chamfer Distance" }
        isLength(definition.chamferDist, { (millimeter) : [0, 0, 1000] } as LengthBoundSpec);
        annotation { "Name": "Prefill", "Default" : false }
        definition.prefill is boolean;
        if (definition.prefill)
        {
            annotation { "Name": "Prefill Diameter" }
            isLength(definition.prefillDiam,  { (millimeter) : [0, 0, 1000] } as LengthBoundSpec);
            annotation { "Name": "Prefill Depth" }
            isLength(definition.prefillDepth, { (millimeter) : [0, 0, 1000] } as LengthBoundSpec);
        }


        annotation { "Name": "Add Wings", "Default" : false }
        definition.hasWings is boolean;
        if (definition.hasWings)
        {
            annotation { "Name": "Wing Diameter Outset" }
            isLength(definition.wingMidDiamOutset,  { (millimeter) : [0.01, 1.6, 1000] } as LengthBoundSpec);
            annotation { "Name": "Wing Gap Thickness" }
            isLength(definition.wingGapThk,  { (millimeter) : [0.01, 0.1, 1000] } as LengthBoundSpec);
            annotation { "Name": "Wings Face Inset" }
            isLength(definition.wingFaceInset,  { (millimeter) : [0, 0.4, 1000] } as LengthBoundSpec);
            annotation { "Name": "Wings End Depth" }
            isLength(definition.wingEndDepth, { (millimeter) : [0, 0, 1000] } as LengthBoundSpec);
            annotation { "Name" : "Wing Spread Angle" }
            isAngle(definition.wingSpread, { (degree) : [2, 120, 179] } as AngleBoundSpec);
        }

        annotation { "Name": "Color Hole", "Default" : false, "UIHint" : ["REMEMBER_PREVIOUS_VALUE"] }
        definition.hasColor is boolean;
        if (definition.hasColor)
        {
            annotation { "Name": "Hole Color Spec", "Default": "#fcd899", "UIHint" : ["REMEMBER_PREVIOUS_VALUE"] }
            definition.holeColor is string;
        }

        annotation { "Name": "Highlight While Editing", "Default" : false, "UIHint" : ["REMEMBER_PREVIOUS_VALUE"] }
        definition.debugMe is boolean;
    }
    {
        // Specify what the feature does when regenerating
        forEachEntity(context, id + "operation", definition.mateConnectors, function(entity is Query, id is Id)
            {
                // The mate connector supplies origin, hole axis, and clocking in one shot
                var mateCsys = evMateConnector(context, { "mateConnector" : entity });

                if (definition.flipDir)
                {
                    // Reverse Z while keeping the user's X direction, so the CSYS stays right handed
                    mateCsys = coordSystem(mateCsys.origin, mateCsys.xAxis, -mateCsys.zAxis);
                }

                var anchorPoint = mateCsys.origin;
                var holeAxis = mateCsys.zAxis;

                var transformMatrix = toWorld(mateCsys);


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
                                "ray" : line(anchorPoint + holeAxis * 0.001 * millimeter, holeAxis),
                                "entities" : definition.targetBody
                            });
                        var baseDistance = 5 * millimeter; // Fallback default value
                        var validHitFound = false;
                        for (var hit in hits)
                        {
                            // Ignore the starting face intersection (near 0mm distance)
                            if (hit.distance > 0.001 * millimeter)
                            {
                                baseDistance = hit.distance;
                                validHitFound = true;
                                break;
                            }
                        }

                        if (!validHitFound)
                        {
                            println("Can't find next entity!");
                        }

                        computedDepth = applyTipOffset(definition, baseDistance);
                    }
                    else if (definition.endStyle == FDMHoleEndStyle.UP_TO_ENTITY)
                    {
                        // Raycast directly against the selected target entity
                        var hits = evRaycast(context, {
                                "ray" : line(anchorPoint, holeAxis),
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
                            highlightQuery(context, definition.endBoundEntity, DebugColor.RED);
                            // Fallback to minimum distance evaluation if raycast misses surface boundaries
                            var distResult = evDistance(context, {
                                    "side0" : anchorPoint,
                                    "side1" : definition.endBoundEntity,
                                    "extendSide1": true,
                                });
                            baseDistance = distResult.distance;
                        }

                        computedDepth = applyTipOffset(definition, baseDistance);
                    }
                    else if (definition.endStyle == FDMHoleEndStyle.THROUGH)
                    {
                        var hits = evRaycast(context, {
                                "ray" : line(anchorPoint, holeAxis),
                                "entities" : definition.targetBody
                            });

                        var validHitFound = false;
                        // Scan backwards to find the last valid exit boundary hit
                        for (var ii = size(hits) - 1; ii >= 0; ii -= 1)
                        {
                            if (hits[ii].distance > 0.05 * millimeter)
                            {
                                computedDepth = hits[ii].distance + 1 * millimeter;
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

                if (definition.prefill)
                {
                    const prefiller    =  newInstantiator(id + "prefiller");
                    const prefillDiam  = (definition.prefillDiam  == undefined || definition.prefillDiam  <= 0) ? definition.diameter + (1 * millimeter) : definition.prefillDiam;
                    const prefillDepth = (definition.prefillDepth == undefined || definition.prefillDepth <= 0) ? computedDepth                          : definition.prefillDepth;
                    var prefillQuery = addInstance(prefiller, BasicHole::build, {
                            "configuration" : {
                                "diameter" : prefillDiam,
                                "depth" :    prefillDepth,
                                "split" :    false,
                                "height" :   0 * millimeter,
                                "chamferDist" : definition.chamferDist,
                            },
                            "transform" : transformMatrix,
                        });

                    instantiate(context, prefiller);
                    highlightQuery(context, prefillQuery, DebugColor.YELLOW, definition.debugMe);

                    try {
                        opBoolean(context, id + "prefillBool", {
                                "tools" : qUnion([definition.targetBody, prefillQuery]),
                                "operationType" : BooleanOperationType.UNION
                            });
                    } catch (err) { debug(context, err); }
                }


                if (definition.hasWings)
                {
                    const wingman    =  newInstantiator(id + "wingman");
                    // const wing  = (definition.prefillDiam  == undefined || definition.prefillDiam  <= 0) ? definition.diameter : definition.prefillDiam;
                    var wingFaceInset = definition.wingFaceInset;
                    var wingEndDepth = (definition.wingEndDepth == undefined || definition.wingEndDepth <= 0) ? (computedDepth - wingFaceInset) : definition.wingEndDepth;
                    wingEndDepth = min(wingEndDepth, computedDepth);
                    wingFaceInset = max(wingFaceInset, wingFaceInset * 0.5 + definition.chamferDist * 0.6);
                    wingFaceInset = clamp(wingFaceInset, 0*millimeter, wingEndDepth - 0.01*millimeter);
                    var wingsQuery = addInstance(wingman, Wings::build, {
                            "configuration" : {
                                "diameter" : definition.diameter,
                                "wingMidDiamOutset": definition.wingMidDiamOutset,
                                "wingGapThk": definition.wingGapThk,
                                "wingEndDepth" : wingEndDepth,
                                "wingFaceInset": wingFaceInset,
                                "wingSpread":    definition.wingSpread,
                            },
                            "transform" : transformMatrix,
                        });

                    instantiate(context, wingman);
                    highlightQuery(context, wingsQuery, DebugColor.ORANGE, definition.debugMe);

                    try {
                        opBoolean(context, id + "wingsBool", {
                                "tools" : wingsQuery,
                                "targets": definition.targetBody,
                                "operationType" : BooleanOperationType.SUBTRACTION
                            });
                    } catch (err) { debug(context, err); }
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
                            "threads" : definition.thread_count,
                            "chamferDist" : definition.chamferDist,
                        };
                }
                else if (definition.method == HoleType.SIMPLE || definition.method == HoleType.SPLIT)
                {
                    METHOD = BasicHole::build;
                    CONFIG = {
                            "diameter" : definition.diameter,
                            "depth" : computedDepth,
                            "split" : splited,
                            "height" : definition.split_height,
                            "chamferDist" : definition.chamferDist,
                        };
                }
                else if (definition.method == HoleType.TEAR)
                {
                    METHOD = TearHole::build;
                    CONFIG = {
                            "diameter" : definition.diameter,
                            "depth" : computedDepth,
                            "angle" : definition.angle,
                            "layerHeight" : definition.layerHeight,
                            "chamferDist" : definition.chamferDist,
                            "bottomTear" : definition.bottomTear,
                        };
                }

                var cutterQuery = addInstance(instantiator, METHOD, {
                        "configuration" : CONFIG,
                        "transform" : transformMatrix,
                    });

                instantiate(context, instantiator);
                highlightQuery(context, cutterQuery, DebugColor.CYAN, definition.debugMe);

                try {
                    opBoolean(context, id + "boolean1", {
                            "tools" : cutterQuery,
                            "targets" : definition.targetBody,
                            "operationType" : BooleanOperationType.SUBTRACTION
                        });
                    if (definition.hasColor) {
                        try {
                            setColor(context, qCreatedBy(id + "boolean1", EntityType.FACE), definition.holeColor);
                            // testColorUtils(context);
                        } catch (err) { debug(context, [err, 'bad hole', definition.holeColor]); }
                    }
                } catch (err) { debug(context, err); }

            });
        holeFeatureName(context, id, definition);
    });

export function fdmHoleFeatureEditLogic(context is Context, id is Id, oldDefinition is map, definition is map, isCreating is boolean, specifiedParameters is map) returns map {
    return holeFeatureName(context, id, definition);
}

function holeFeatureName(context is Context, id is Id, definition is map) {
    var parts = [];
    var method = HoleTypeTitles[definition.method];
    parts = append(parts, method);
    const titleDiam = simpleNumber(definition.diameter);
    const titleLen  = (definition.endStyle == FDMHoleEndStyle.BLIND) ? ("•" ~ simpleNumber(definition.depth)) : (FDMHoleEndStyleTitles[definition.endStyle]);
    parts = append(parts, titleDiam ~ titleLen);
    //
    if (ifNil(definition.chamferDist, -1) > 0) { parts = append(parts, "C" ~ simpleNumber(definition.chamferDist)); }
    if (ifNil(definition.hasWings, false))     { parts = append(parts, "W"); }
    if (ifNil(definition.prefill,  false))     { parts = append(parts, "P" ~ simpleNumber(ifZero(definition.prefillDiam, definition.diameter))); }
    const displayTitle = join(parts, " ");
    setFeatureComputedParameter(context, id, { name: "displayTitle", value: displayTitle });
    definition.displayTitle = displayTitle;
    return definition;
}

export function simpleNumber(num is ValueWithUnits) returns string { return simpleNumber(num / millimeter); }
export function simpleNumber(num is number) returns string {
    if (tolerantEquals(num, floor(num))) { return "" ~ floor(num); }
    return "" ~ roundToPrecision(num, 1);
}
