/*
    Laser Joint

    This custom feature creates a finger joint between one or more tab parts
    and a single base part.

    Adapted in part from code in "Box Joint", written by Neil Cooke.

            0.1     - June 24, 2016 - Arul Suresh - Initial port
            0.2     - June 26, 2016 - Arul Suresh - Support for multiple intersections between two parts
    Version 1.0     - June 27, 2016 - Arul Suresh - Added adaptive pin spacing
    Version 1.9     - June 30, 2016 - Arul Suresh - Refactored code for clarity, implemented automated mode, corner overcuts, additional clearance.
                                                    Algorithm for overcuts adapted from "Dogbone and Corner Overcut Features" by Jonathon Duerig.
    Version 1.9.1   - July  1, 2016 - Arul Suresh - Confirmed that angled joints function as intended, beginning testing.
    Version 2.0     - July  4, 2016 - Arul Suresh - Automatic mode can still fail in complex geometries, but single joints behave as expected.
                                                    Added showcase for reasonably complex automatic joints, which fails if pin sense is inverted.
                                                    Version for second release in forums.
    Version 2.1     - July 14, 2016 - Arul Suresh - Spun out Laser Joint into separate workspace for public release.
                                                    Added comments throughout, minor refactoring.
    Version 2.2     - Sept 18, 2016 - Arul Suresh - Updated reference to new Laser Utils build v1.2.
    Version 2.2.1   - Jan  13, 2017 - Arul Suresh - Updated to new FS version 477, modified mode operation to use new Horizontal Enum display.
    Version 2.2.2   - Feb  17, 2017 - Arul Suresh - Updated documentation for Horizontal Enum UI, updated to Laser Utils V2.0
    Version 2.3     - Oct  31, 2017 - Arul Suresh - Updated to FS 701, changed tab cut method for performance. (~25% over 2.2.2)
    Version 3.0     - Nov   7, 2017 - Arul Suresh - Added pin face offset, pin chamfer.  Reordered UI elements.  Updated to LaserUtils V3.0
    Version 3.1     - Jan   9, 2018 - Arul Suresh - Bug fix for not correctly finding mating tab faces when applying allowance.
    Version 4.0     - Feb  10, 2018 - Arul Suresh - Added fixed pin width, overcut fillet, pin fillet. Reordered some UI elements.
    Version 4.1     - Feb  12, 2018 - Arul Suresh - Fixed bug with pin face offsets, updated to Laser Utils V4.0
    Version 4.2     - May  28, 2019 - Arul Suresh - Updated to FS 1077, added parameter grouping in UI.
    Version 4.2.1   - Jun   2, 2020 - Arul Suresh - Added custom icon.
    Version 4.3     - Aug   8, 2020 - Arul Suresh - Updated to FS 1337, update icon, add feature and parameter descriptions from latest OS release.
    Version 4.3.1   - Dec  11, 2020 - Arul Suresh - Updated operation type to remember previous so that adaptive pins can remember previous.
    Version 4.4     - Apr  22, 2020 - Arul Suresh - Updated to latest Laser Utils and FS version to implement tolerantSort throughout.
    Version 4.4.1   - Dec   8, 2023 - Arul Suresh - Refactored internal Corner Overcut feature to function to allow publishing only Laser Joint.
*/

FeatureScript 1494;
import(path : "onshape/std/geometry.fs", version : "1494.0");

// Import LaserJointOperationType definition
export import(path : "9727aac6e366e401ee94d964", version : "6843bfb12a6120409f897477"); // laser_joint/laser_joint-operation-type.fs

// Import PinModificationType definition
export import(path : "bc2f9a7ecc77ef7d9f4580d4", version : "20feb6e1e24d46a7b95bbed5"); // laser_joint/laser_joint-pin-modification-type.fs

// Import LaserUtils
import(path : "f4e7238da5afaf5a3f1498c0/26bc76e776d09af99fb63803/22d17eb94c85900576fbf53e", version : "a642e9ecd058283f5eb2bcf6"); // laser_utils/laser_utils.fs

// Import Icon
IconData::import(path : "df7df65f89230b222d5c81d7", version : "b50ae2d69dc6546d7bd12485"); // laser_joint/laser_joint_icon.svg

// Volume tolerance for sorting parts
const SORT_TOL = 1e-8 * meter ^ 3;


annotation { "Feature Type Name" : "Laser Joint",
        "Feature Type Description" : "Creates a finger joint in interlocking planar parts.<br>" ~
        "Implements manual and automatic joint generation, with " ~
        "options for corner overcuts, allowances for fit tolerancing, " ~
        "and adaptive sizing of pins.<br><br>" ~
        "This feature assumes that the parts <b>are planar</b> " ~
        "and that the <b>largest face determines the cutting plane</b>.",
        "Editing Logic Function" : "onDefinitionChange",
        "Filter Selector" : ["fs", "lj", "laser"],
        "Icon" : IconData::BLOB_DATA
    }
export const laserJoint = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Operation type", "UIHint" : ["HORIZONTAL_ENUM", "REMEMBER_PREVIOUS_VALUE"],
                    "Description" : "<b>Single:</b> Create a joint between specified tab and base parts. <br> <b>Automatic:</b> Create joints for all selected parts, adapting pins to meet constraints." }
        definition.operationType is LaserJointOperationType;

        if (definition.operationType == LaserJointOperationType.SINGLE)
        {
            annotation { "Name" : "Select tab part", "Filter" : EntityType.BODY && BodyType.SOLID, "MaxNumberOfPicks" : 1,
                        "Description" : "The tab part contains the pins, and is the reference for any edge offsets." }
            definition.tab is Query;

            annotation { "Name" : "Select base part", "Filter" : EntityType.BODY && BodyType.SOLID, "MaxNumberOfPicks" : 1,
                        "Description" : "The base part has slots to accept the tab's pins." }
            definition.base is Query;
        }
        else
        {
            annotation { "Name" : "Select parts to join", "Filter" : EntityType.BODY && BodyType.SOLID,
                        "Description" : "Joints will be computed for all intersections between selected parts." }
            definition.partList is Query;
        }

        annotation { "Name" : "Number of pins", "UIHint" : "REMEMBER_PREVIOUS_VALUE",
                    "Description" : "The number of pins on the tab part.<br>May be overridden if automatic mode and adaptive pin sizing are both enabled." }
        isInteger(definition.numPins, PIN_BOUNDS);

        annotation { "Name" : "Toggle pin/gap", "UIHint" : "OPPOSITE_DIRECTION",
                    "Description" : "Determines whether the tab part has pins at the edges, or gaps at the edges." }
        definition.pinSense is boolean;

        if (definition.operationType == LaserJointOperationType.SINGLE)
        {

            annotation { "Name" : "Fixed pin width", "UIHint" : "REMEMBER_PREVIOUS_VALUE",
                        "Description" : "When enabled, use defined pin width. Default is to evenly distribute pins and gaps." }
            definition.fixedPinWidth is boolean;


            if (definition.fixedPinWidth)
            {
                annotation { "Group Name" : "Pin Width", "Collapsed By Default" : false, "Driving Parameter" : "fixedPinWidth" }
                {
                    annotation { "Name" : "Pin width", "UIHint" : "REMEMBER_PREVIOUS_VALUE",
                                "Description" : "Width of each pin on the tab part." }
                    isLength(definition.pinW, PIN_WIDTH_MIN_BOUNDS);
                }
            }

            // Define Gap width only if we are also defining pin width
            if (definition.fixedPinWidth)
            {
                annotation { "Name" : "Fixed gap width", "UIHint" : "REMEMBER_PREVIOUS_VALUE",
                            "Description" : "When enabled, use defined gap width. Default is to evenly distribute pins and gaps." }
                definition.fixedGapWidth is boolean;

                if (definition.fixedGapWidth)
                {
                    annotation { "Group Name" : "Gap Width", "Collapsed By Default" : false, "Driving Parameter" : "fixedGapWidth" }
                    {
                        annotation { "Name" : "Gap width", "UIHint" : "REMEMBER_PREVIOUS_VALUE", "Description" : "Width of each gap on the tab part." }
                        isLength(definition.gapW, PIN_WIDTH_MIN_BOUNDS);
                    }
                }
            }
        }

        if (definition.operationType == LaserJointOperationType.AUTO)
        {
            annotation { "Name" : "Adaptive pin sizing", "UIHint" : "REMEMBER_PREVIOUS_VALUE",
                        "Description" : "When enabled, automatically modifies pin sizing to meet minimum/maximum constraints.<br>If limits cannot be achieved using the desired number of pins, overrides the number of pins until constraints are satisfied." }
            definition.adaptive is boolean;

            if (definition.adaptive)
            {
                annotation { "Group Name" : "Adaptive Options", "Collapsed By Default" : false, "Driving Parameter" : "adaptive" }
                {
                    annotation { "Name" : "Minimum pin width", "UIHint" : "REMEMBER_PREVIOUS_VALUE",
                                "Description" : "Pins will not be generated with width less than this dimension." }
                    isLength(definition.pinMinW, PIN_WIDTH_MIN_BOUNDS);

                    annotation { "Name" : "Maximum pin width", "UIHint" : "REMEMBER_PREVIOUS_VALUE",
                                "Description" : "Pins will not be generated with width greater than this dimension." }
                    isLength(definition.pinMaxW, PIN_WIDTH_MAX_BOUNDS);
                }
            }
        }

        annotation { "Name" : "Add allowance", "UIHint" : "REMEMBER_PREVIOUS_VALUE",
                    "Description" : "Add allowances to one or more faces itnernal to the joint to adjust the fit." }
        definition.allowance is boolean;

        if (definition.allowance)
        {
            annotation { "Group Name" : "Allowance Options", "Collapsed By Default" : false, "Driving Parameter" : "allowance" }
            {
                annotation { "Name" : "Allowance", "UIHint" : "REMEMBER_PREVIOUS_VALUE",
                            "Description" : "Amount of allowance to apply; by default will be applied only to faces within the joint." }
                isLength(definition.allowanceVal, FIT_OFFSET_LENGTH_BOUNDS);

                annotation { "Name" : "Apply to tab faces", "UIHint" : "REMEMBER_PREVIOUS_VALUE",
                            "Description" : "Also apply allowance to faces on the tab part which mate to the surface of the base part." }
                definition.tabFaceAllowance is boolean;

                annotation { "Name" : "Apply to base faces", "UIHint" : "REMEMBER_PREVIOUS_VALUE",
                            "Description" : "Also apply allowance to faces on the base part which mate to the surface of the tab part." }
                definition.baseFaceAllowance is boolean;
            }
        }

        annotation { "Name" : "Edge offset", "UIHint" : ["DISPLAY_SHORT", "REMEMBER_PREVIOUS_VALUE"],
                    "Description" : "Offset the outermost pins on the tab part inwards along the computed joint.<br>" }
        definition.edgeOffset is boolean;

        if (definition.edgeOffset)
        {
            annotation { "Group Name" : "Edge Offset", "Collapsed By Default" : false, "Driving Parameter" : "edgeOffset" }
            {
                annotation { "Name" : "Offset distance", "UIHint" : ["REMEMBER_PREVIOUS_VALUE"],
                            "Description" : "Distance from the edge of the tab part to offset the outermost pin.<br>Applied on a per-intersection basis in the case of tab parts which intersect the base part at multiple locations." }
                isLength(definition.edgeOffsetVal, ZERO_DEFAULT_LENGTH_BOUNDS);
            }
        }

        annotation { "Name" : "Tail offset", "UIHint" : ["DISPLAY_SHORT", "REMEMBER_PREVIOUS_VALUE"],
                    "Description" : "Offset the outermost pins on the tab part inwards along the computed joint.<br>" }
        definition.tailOffset is boolean;

        if (definition.tailOffset)
        {
            annotation { "Group Name" : "Tail Offset", "Collapsed By Default" : false, "Driving Parameter" : "tailOffset" }
            {
                annotation { "Name" : "Tail Offset distance", "UIHint" : ["REMEMBER_PREVIOUS_VALUE"],
                            "Description" : "Distance from the other edge of the tab part to offset the lastmost pin.<br>Applied on a per-intersection basis in the case of tab parts which intersect the base part at multiple locations." }
                isLength(definition.tailOffsetVal, ZERO_DEFAULT_LENGTH_BOUNDS);
            }
        }

        annotation { "Name" : "Pin face offset", "UIHint" : ["DISPLAY_SHORT", "REMEMBER_PREVIOUS_VALUE"],
                    "Description" : "Offsets the ends of the pins on the tab part." }
        definition.pinFaceOffset is boolean;

        if (definition.pinFaceOffset)
        {
            annotation { "Group Name" : "Pin Face Offset", "Collapsed By Default" : false, "Driving Parameter" : "pinFaceOffset" }
            {
                annotation { "Name" : "Offset distance", "UIHint" : ["REMEMBER_PREVIOUS_VALUE"],
                            "Description" : "Distance to offset the end faces of the pins on the tab part." }
                isLength(definition.pinFaceOffsetVal, ZERO_DEFAULT_LENGTH_BOUNDS);
            }
        }

        annotation { "Name" : "Corner overcut", "UIHint" : "REMEMBER_PREVIOUS_VALUE",
                    "Description" : "When enabled, creates a corner overcut on all concave edges created by the joint." }
        definition.overcut is boolean;

        if (definition.overcut)
        {
            annotation { "Group Name" : "Corner Overcut Options", "Collapsed By Default" : false, "Driving Parameter" : "overcut" }
            {
                annotation { "Name" : "Diameter", "UIHint" : "REMEMBER_PREVIOUS_VALUE",
                            "Description" : "Diameter of the circular corner overcut." }
                isLength(definition.overcutD, CORNER_OVERCUT_LENGTH_BOUNDS);

                annotation { "Name" : "Fillet overcuts", "UIHint" : ["DISPLAY_SHORT", "REMEMBER_PREVIOUS_VALUE"],
                            "Description" : "When enabled, applies a fillet to sharp corners created by the overcut, allowing for smoother cutting paths." }
                definition.filletOvercut is boolean;

                if (definition.filletOvercut)
                {
                    annotation { "Name" : "Radius", "UIHint" : ["DISPLAY_SHORT", "REMEMBER_PREVIOUS_VALUE"],
                                "Description" : "Radius to use when filleting overcuts." }
                    isLength(definition.filletRadius, CORNER_OVERCUT_LENGTH_BOUNDS);
                }
            }
        }

        annotation { "Name" : "Pin chamfer/fillet", "UIHint" : "REMEMBER_PREVIOUS_VALUE",
                    "Description" : "When enabled, applies a fillet or a chamfer to the ends of the pins on the tab part.<br>For example, this could be used to aid assembly of parts." }
        definition.pinMod is boolean;

        if (definition.pinMod)
        {
            annotation { "Group Name" : "Pin Options", "Collapsed By Default" : false, "Driving Parameter" : "pinMod" }
            {
                annotation { "Name" : "Pin Modification Type", "UIHint" : ["DISPLAY_SHORT", "REMEMBER_PREVIOUS_VALUE"] }
                definition.pinModType is PinModificationType;

                annotation { "Name" : "Distance", "UIHint" : ["DISPLAY_SHORT", "REMEMBER_PREVIOUS_VALUE"],
                            "Description" : "Fillet radius or symmetric chamfer dimension to apply to pin ends." }
                isLength(definition.pinModD, CORNER_OVERCUT_LENGTH_BOUNDS);
            }
        }

        annotation { "Name" : "Clean tolerance", "UIHint" : "ALWAYS_HIDDEN" }
        isReal(definition.cleanTol, TOL_BOUNDS);
    }

    {
        if (definition.operationType == LaserJointOperationType.SINGLE)
        {
            createSingleJoint(context, id, definition);
        }
        else if (definition.operationType == LaserJointOperationType.AUTO)
        {
            // Heuristic: join parts from smallest to largest
            // First, get indices of parts in sorted order
            var sortFun = function(C, x)
                {
                    return evVolume(C, { "entities" : x });
                };
            var sortedIndices = getSortIndices(context, definition.partList, sortFun, SORT_TOL, SortDirection.ASCENDING);

            var N = size(sortedIndices);

            // Iterate through possible combinations
            for (var i = 0; i < N; i += 1)
            {
                for (var j = 0; j < i; j += 1)
                {
                    var collisionInfo = evCollision(context, {
                            "tools" : qNthElement(definition.partList, sortedIndices[i]),
                            "targets" : qNthElement(definition.partList, sortedIndices[j])
                        });

                    if (isJoinableCollision(collisionInfo))
                    {
                        // If we have a valid intersection between this pair, set the smaller one as the tab and create a joint
                        var localDefinition = definition;
                        localDefinition.tab = qNthElement(definition.partList, sortedIndices[j]);
                        localDefinition.base = qNthElement(definition.partList, sortedIndices[i]);

                        try
                        {
                            createSingleJoint(context, id + i + j, localDefinition);
                        }
                        catch (error)
                        {
                            throw error;
                        }
                    }
                }
            }
        }
    });

/**
 * Performs all operations to create a laser joint between two intersecting parts
 */
function createSingleJoint(context is Context, id is Id, definition is map)
{
    // Cut the fingers in the tab part and save the created edges/faces
    var tabGeometry = cutTabSlots(context, id + "tab", definition);

    // Cut the fingers in the base part and save the created edges/faces
    var baseGeometry = cutBaseSlots(context, id + "base", definition);

    // Offset pin faces if enabled
    if (definition.pinFaceOffset)
    {
        if (size(evaluateQuery(context, tabGeometry.pinFaces)) > 0)
        {
            opOffsetFace(context, id + "tabOffsetPinFace", {
                        "moveFaces" : tabGeometry.pinFaces,
                        "offsetDistance" : -1 * definition.pinFaceOffsetVal
                    });
        }
    }

    // Offset joint surfaces for allowances
    if (definition.allowance)
    {
        // Faces internal to the joint
        opOffsetFace(context, id + "baseOffsetJointFace", {
                    "moveFaces" : baseGeometry.jointFaces,
                    "offsetDistance" : -1 * definition.allowanceVal
                });

        // Mating faces on the tab part
        if (definition.tabFaceAllowance)
        {
            if (size(evaluateQuery(context, tabGeometry.faces)) > 0)
            {
                opOffsetFace(context, id + "tabOffsetMatingFace", {
                            "moveFaces" : tabGeometry.faces,
                            "offsetDistance" : -1 * definition.allowanceVal
                        });
            }
        }

        // Mating faces on the base part
        if (definition.baseFaceAllowance)
        {
            if (size(evaluateQuery(context, baseGeometry.matingFaces)) > 0)
            {
                opOffsetFace(context, id + "baseOffsetMatingFace", {
                            "moveFaces" : baseGeometry.matingFaces,
                            "offsetDistance" : -1 * definition.allowanceVal
                        });
            }
        }
    }

    // Chamfer pin edges if enabled
    if (definition.pinMod)
    {
        if (size(evaluateQuery(context, tabGeometry.pinEdges)) > 0)
        {
            if (definition.pinModType == PinModificationType.CHAMFER)
            {
                opChamfer(context, id + "tabPinChamfer", {
                            "entities" : tabGeometry.pinEdges,
                            "chamferType" : ChamferType.EQUAL_OFFSETS,
                            "width" : definition.pinModD
                        });

                tabGeometry.edges = qUnion([tabGeometry.edges, findConcaveCreatedEdges(context, id + "tabPinChamfer", definition.tab)]);
            }
            else
            {
                opFillet(context, id + "tabPinChamfer", {
                            "entities" : tabGeometry.pinEdges,
                            "radius" : definition.pinModD
                        });
            }
        }
    }

    if (definition.overcut)
    {
        if (size(evaluateQuery(context, tabGeometry.edges)) > 0)
        {
            cornerOvercut(context, id + "tabOvercut", {
                        "edges" : tabGeometry.edges,
                        "D" : definition.overcutD,
                        "applyFillet" : definition.filletOvercut,
                        "Rfill" : definition.filletRadius
                    });
        }

        cornerOvercut(context, id + "baseOvercut", {
                    "edges" : baseGeometry.edges,
                    "D" : definition.overcutD,
                    "applyFillet" : definition.filletOvercut,
                    "Rfill" : definition.filletRadius
                });
    }
}

/**
 * Function cuts gaps into tab part
 * returns map containing, based on operation mode:
 *      edges: concave edges created by the cut for optional overcut
 *      faces: faces created by this operation that mate with the face of the base part
 *             for optional allowance
 *      pinEdges: edges at the tips of the pins for pin chamfer
 *      pinFaces: faces at the ends of the pins for optional pin face offset
 */
function cutTabSlots(context is Context, id is Id, definition is map) returns map
{
    // create intersections of tab with base
    opBoolean(context, id + "intersect", {
                "tools" : qUnion([definition.tab, definition.base]),
                "operationType" : BooleanOperationType.INTERSECTION,
                "keepTools" : true
            });

    var intersectBodies = qCreatedBy(id + "intersect", EntityType.BODY);
    var M = size(evaluateQuery(context, intersectBodies));

    var createdEdges = qNothing();
    var createdFaces = qNothing();
    var pinEdges = qNothing();
    var pinFaces = qNothing();

    if (M == 0)
    {
        throw regenError("Operation requires two intersecting parts");
    }
    else
    {
        // Make laser-cuttable slots in tab part for each intersection
        for (var j = 0; j < M; j += 1)
        {
            // Create coordinate system on tab part
            var facePlane = evPlane(context, {
                    "face" : getLargestFace(context, definition.tab)
                });
            var cSys = coordSystem(facePlane.origin, getXAxis(context, id + j + "x", definition.tab, definition.base), facePlane.normal);

            // Compute sections to cut out
            var intersectionBody = qNthElement(intersectBodies, j);
            var cutPattern = computePatternSeed(context, id + j, definition, {
                    "csys" : cSys,
                    "intersect" : intersectionBody
                });

            var edgeCuboids = qNothing();
            const edgeOffsetIsSet = (! tolerantEquals(cutPattern.edgeOffset, 0 * inch));
            const tailOffsetIsSet = (! tolerantEquals(cutPattern.tailOffset, 0 * inch));
            if (edgeOffsetIsSet)
            {
                fCuboid(context, id + j + "edgeCuboidA", {
                            "corner1" : cutPattern.cuboidCorners.edgeOffsetA1,
                            "corner2" : cutPattern.cuboidCorners.edgeOffsetA2
                        });
                edgeCuboids = qUnion([qCreatedBy(id + j + "edgeCuboidA", EntityType.BODY)]);
            }
            if (tailOffsetIsSet)
            {
                fCuboid(context, id + j + "edgeCuboidB", {
                            "corner1" : cutPattern.cuboidCorners.edgeOffsetB1,
                            "corner2" : cutPattern.cuboidCorners.edgeOffsetB2
                        });
                if (edgeOffsetIsSet)
                {
                    edgeCuboids = qUnion([qCreatedBy(id + j + "edgeCuboidA", EntityType.BODY), qCreatedBy(id + j + "edgeCuboidB", EntityType.BODY)]);
                }
                else
                {
                    edgeCuboids = qUnion([qCreatedBy(id + j + "edgeCuboidA", EntityType.BODY), qCreatedBy(id + j + "edgeCuboidB", EntityType.BODY)]);
                }
            }


            var cuboid = qNothing();
            if (cutPattern.N > 1 || definition.pinSense)
            {
                fCuboid(context, id + j + "cuboid", {
                            "corner1" : cutPattern.cuboidCorners.seed1,
                            "corner2" : cutPattern.cuboidCorners.seed2
                        });

                cuboid = qCreatedBy(id + j + "cuboid", EntityType.BODY);
            }

            if (size(evaluateQuery(context, qUnion([edgeCuboids, cuboid]))) > 0)
            {
                opTransform(context, id + j + "transform", {
                            "bodies" : qUnion([cuboid, edgeCuboids]),
                            "transform" : cutPattern.worldXform
                        });
            }

            if (cutPattern.patternInfo.N > 0)
            {
                opPattern(context, id + j + "pattern", {
                            "entities" : cuboid,
                            "transforms" : cutPattern.patternInfo.transforms,
                            "instanceNames" : cutPattern.patternInfo.instanceNames
                        });
            }

            if (size(evaluateQuery(context, qUnion([qCreatedBy(id + j + "pattern", EntityType.BODY), edgeCuboids, cuboid]))) > 0)
            {
                opBoolean(context, id + j + "subtract", {
                            "tools" : qUnion([qCreatedBy(id + j + "pattern", EntityType.BODY), edgeCuboids, cuboid]),
                            "targets" : definition.tab,
                            "operationType" : BooleanOperationType.SUBTRACTION,
                            "keepTools" : false
                        });

                // Check if this cut created disconnected components
                var createdBodies = qBodyType(qCreatedBy(id + j + "subtract", EntityType.BODY), BodyType.SOLID);
                if (size(evaluateQuery(context, createdBodies)) > 0)
                {
                    debug(context, createdBodies);
                }


                // If corner overcut is enabled, find concave edges created by this cut
                if (definition.overcut)
                {
                    createdEdges = qUnion([createdEdges, findConcaveCreatedEdges(context, id + j + "subtract", definition.tab)]);
                }

                // If allowances enabled, find faces that will touch the large face of the base part
                if (definition.allowance)
                {
                    createdFaces = qUnion([createdFaces, findOrientedCreatedFaces(context, id + j + "subtract", definition.tab, yAxis(cSys))]);
                }
            }

            if (definition.pinFaceOffset || definition.pinMod)
            {
                var newFaces = findCreatedPinFaces(context, intersectionBody, cSys, definition);
                var newEdges = findCreatedPinEdges(context, id + j + "subtract", intersectionBody, cSys, newFaces, definition);

                if (definition.pinFaceOffset)
                {
                    pinFaces = qUnion([pinFaces, newFaces]);
                }

                if (definition.pinMod)
                {
                    pinEdges = qUnion([pinEdges, newEdges]);
                }
            }
        }

        // Clean up the bodies created to check intersections
        opDeleteBodies(context, id + "deleteIntersection", {
                    "entities" : intersectBodies
                });
    }

    return { "edges" : createdEdges, "faces" : createdFaces, "pinEdges" : pinEdges, "pinFaces" : pinFaces };
}

/**
 * Function cuts gaps into base part.
 * Assumes slots in tab have already been cut.
 * returns map containing:
 *      edges: concave edges created by the cut for optional overcut
 *      jointFaces: faces created by this operation internal to the joint for optional allowance
 *      matingFaces: faces created by this operation that mate with the face of the tab part
 *             for optional allowance
 */
function cutBaseSlots(context is Context, id is Id, definition is map) returns map
{
    // create intersections of base with tab part
    opBoolean(context, id + "intersect", {
                "tools" : qUnion([definition.tab, definition.base]),
                "operationType" : BooleanOperationType.INTERSECTION,
                "keepTools" : true
            });

    // find the intersections between the base and tab part
    var intersectBodies = qCreatedBy(id + "intersect", EntityType.BODY);
    var M = size(evaluateQuery(context, intersectBodies));

    // Create sketch on base part
    var baseFacePlane = evPlane(context, {
            "face" : getLargestFace(context, definition.base)
        });
    var baseCSys = coordSystem(baseFacePlane.origin, getXAxis(context, id + "x", definition.tab, definition.base), baseFacePlane.normal);
    var baseSketchPlane is Plane = plane(baseCSys);

    var baseSketch = newSketchOnPlane(context, id + "sketch", {
            "sketchPlane" : baseSketchPlane
        });

    // Generate regions for each intersection
    for (var j = 0; j < M; j += 1)
    {
        var intersectBound is Box3d = evBox3d(context, {
                "topology" : qNthElement(intersectBodies, j),
                "cSys" : baseCSys
            });
        skRectangle(baseSketch, ("rectangle" ~ j), {
                    "firstCorner" : vector(intersectBound.minCorner[0], intersectBound.minCorner[1]),
                    "secondCorner" : vector(intersectBound.maxCorner[0], intersectBound.maxCorner[1])
                });
    }

    skSolve(baseSketch);

    // Cut out areas in base part
    opExtrude(context, id + "extrude", {
                "entities" : qSketchRegion(id + "sketch"),
                "direction" : baseSketchPlane.normal,
                "endBound" : BoundingType.THROUGH_ALL,
                "startBound" : BoundingType.THROUGH_ALL
            });

    opBoolean(context, id + "subtract", {
                "tools" : qCreatedBy(id + "extrude", EntityType.BODY),
                "targets" : definition.base,
                "operationType" : BooleanOperationType.SUBTRACTION,
                "keepTools" : false
            });

    opDeleteBodies(context, id + "deleteSketch", {
                "entities" : qCreatedBy(id + "sketch", EntityType.BODY)
            });

    opDeleteBodies(context, id + "deleteBodies", {
                "entities" : intersectBodies
            });

    // Find and remove disconnected components created by this cut
    cleanDisconnectedComponents(context, id, id + "subtract", definition.cleanTol);

    // Find concave edges created by this cut
    var edges = qOwnedByBody(qCreatedBy(id + "subtract", EntityType.EDGE), definition.base);
    var filter = mapArray(evaluateQuery(context, edges), function(x)
    {
        return evEdgeConvexity(context, {
                        "edge" : x
                    }) == EdgeConvexityType.CONCAVE ? x : qNothing();
    });
    var createdEdges = qUnion(filter);

    // Find faces created by this cut, and sort into joint and mating faces
    var faces = qOwnedByBody(qCreatedBy(id + "subtract", EntityType.FACE), definition.base);
    var faceFilter = mapArray(evaluateQuery(context, faces), function(x)
    {
        return isOriented(evPlane(context, { "face" : x }), baseCSys.xAxis) ? x : qNothing();
    });

    var jFaces = qUnion(faceFilter);
    var mFaces = qSubtraction(faces, jFaces);

    var baseGeometry is map = {
        "jointFaces" : jFaces,
        "matingFaces" : mFaces,
        "edges" : createdEdges
    };

    return baseGeometry;
}

/**
 * Internal feature for generating corner overcuts on any number of edges.
 * Edges provided are along the axis of the overcut.
 * Assumes all edges belong to the same parent body.
 */
// V4.4.1: refactor to function to prevent export and allow publishing Laser Joint
// annotation { "Feature Type Name" : "Corner Overcut" }
function cornerOvercut(context is Context, id is Id, definition is map)
// precondition
// {
//     annotation { "Name" : "Select corners", "Filter" : EntityType.EDGE && EdgeTopology.TWO_SIDED && GeometryType.LINE }
//     definition.edges is Query;

//     annotation { "Name" : "Diameter" }
//     isLength(definition.D, CORNER_OVERCUT_LENGTH_BOUNDS);

//     annotation { "Name" : "Apply Fillet" }
//     definition.applyFillet is boolean;

//     annotation { "Name" : "Fillet Radius" }
//     isLength(definition.Rfill, CORNER_OVERCUT_LENGTH_BOUNDS);
// }
{
    var partBody = qOwnerBody(definition.edges);
    var cylinders = qNothing();

    var offsetDist = 0.5 * definition.D;

    var I = size(evaluateQuery(context, definition.edges));
    for (var i = 0; i < I; i += 1)
    {
        var edge = qNthElement(definition.edges, i);
        var endpoints = evaluateQuery(context, qAdjacent(edge, AdjacencyType.VERTEX, EntityType.VERTEX));

        // Calculate direction to offset cylinder from edge
        var faces = qAdjacent(edge, AdjacencyType.EDGE, EntityType.FACE);
        var offsetDir = vector(0, 0, 0);
        var J = size(evaluateQuery(context, faces));
        for (var j = 0; j < J; j += 1)
        {
            offsetDir += evFaceNormalAtEdge(context, {
                        "edge" : edge,
                        "face" : qNthElement(faces, j),
                        "parameter" : 0.5
                    });
        }
        offsetDir = normalize(offsetDir);

        // Create cylinder along edge
        fCylinder(context, id + ("cylinder" ~ i), {
                    "topCenter" : evVertexPoint(context, { "vertex" : endpoints[0] }) + offsetDir * offsetDist,
                    "bottomCenter" : evVertexPoint(context, { "vertex" : endpoints[1] }) + offsetDir * offsetDist,
                    "radius" : 0.5 * definition.D
                });

        // Keep a list of generated cylinders
        cylinders = qUnion([cylinders, qCreatedBy(id + ("cylinder" ~ i), EntityType.BODY)]);
    }

    // Subtract volume of all the cylinders from the base part
    opBoolean(context, id + "subtract", {
                "tools" : cylinders,
                "targets" : partBody,
                "operationType" : BooleanOperationType.SUBTRACTION
            });
    if (definition.applyFillet)
    {
        var edges = qGeometry(qCreatedBy(id + "subtract", EntityType.EDGE), GeometryType.LINE);
        opFillet(context, id + "fillet", {
                    "entities" : edges,
                    "radius" : definition.Rfill
                });
    }
}

/**
 * Function calculates geometry between pin/slot regions of the joint.
 * returns cuboid bounds, world transform, and pattern transforms
 */
function computePatternSeed(context is Context, id is Id, definition is map, parameters is map) returns map
{
    var bound is Box3d = evBox3d(context, {
            "topology" : parameters.intersect,
            "cSys" : parameters.csys
        });
    const origEdgeOffset = definition.edgeOffset                            ? definition.edgeOffsetVal : 0 * meter;
    const origTailOffset = (definition.edgeOffset && definition.tailOffset) ? definition.tailOffsetVal : origEdgeOffset;
    var info is map = {
        "L" : bound.maxCorner[0] - bound.minCorner[0],
        "N" : definition.numPins,
        "offset" : definition.edgeOffset,
        "tailoff": definition.edgeOffset && definition.tailOffset,
        "O"      : origEdgeOffset,
        "OT"     : origTailOffset,
        "sense"  : definition.pinSense,
        "adaptive" : definition.adaptive,
        "fixedPin" : definition.fixedPinWidth,
        "fixedGap" : definition.fixedPinWidth && definition.fixedGapWidth,
        "Pfix" : definition.pinW,
        "Gfix" : definition.gapW,
        "Pmin" : definition.pinMinW,
        "Pmax" : definition.pinMaxW,
        "gapAdjust" : definition.gapAdjust,
        "Gmin" : definition.gapMinW,
        "xmin" : bound.minCorner[0],
        "xmax" : bound.maxCorner[0],
        "ymin" : bound.minCorner[1],
        "ymax" : bound.maxCorner[1],
        "zmin" : bound.minCorner[2],
        "zmax" : bound.maxCorner[2]
    };
    // Pin sense as number
    const pinsAreFirst = (! info.sense);
    var S = (pinsAreFirst) ? 1 : -1; // one extra or one less for pins first / last

    if (info.fixedGap) {
        const availL = info.L - info.O - info.OT;
        const outLen = pinsAreFirst ? info.Pfix : info.Gfix;
        const innLen = pinsAreFirst ? info.Gfix : info.Pfix;
        const cellSize = outLen + innLen;
        const nCells   = floor((availL - outLen) / cellSize);
        info.N = nCells + (pinsAreFirst ? 1 : 0);
        const shouldCenter = (! info.tailoff);
        const coveredL   = outLen + nCells * cellSize;
        const uncoveredL = availL - coveredL;
        if (shouldCenter) {
          info.offset  = true; info.O  += (uncoveredL / 2);
          info.tailoff = true; info.OT += (uncoveredL / 2);
        }
        const inmm = 1000 / 25.4;
        const lenPinGaps = (info.N * (info.Pfix + info.Gfix)) + (pinsAreFirst ? info.Pfix : info.Gfix);
        debug(context, ["calculating fixedGap params!", {
            "offsEdgeOrig": origEdgeOffset.value * inmm, "offsEdge": info.O.value  * inmm,
            "offsTailOrig": origTailOffset.value * inmm, "offsTail": info.OT.value * inmm,
            "n": info.N, "nCells": nCells,
            "lenOuter": outLen.value * inmm, "lenInner": innLen.value * inmm,
            "lenCell": cellSize.value * inmm,
            "len": info.L.value * inmm,
            "lenInnerOuter": coveredL.value * inmm,
            "lenPinGaps": lenPinGaps.value * inmm,
            "sense": info.sense, "sensePinsAreFirst": pinsAreFirst, "senseDelta": S,
        }]);
    }

    // Adjust parameters to meet offset
    if (info.offset)
    {
        info.L -= (info.O + info.OT);

        if (info.L <= 0)
        {
            throw regenError("Edge offset + Tail Offset greater than joint length", ["edgeOffsetVal"]);
        }
    }
    else
    {
        info.O  = 0 * meter;
        info.OT = 0 * meter;
    }

    // Pin spacing as set in feature
    // R = #pins + #slots
    //      #pins = N
    //      #slots = N - 1 or N + 1 depending on pin sense (whether joint starts with a pin or a slot)
    // U is the computed pin width
    // V is the computed pin spacing
    var R = 2 * info.N - S;
    var U = info.L / R;
    var V = U;
    var Np = info.N;

    // If adaptive, check spacing meets constraints
    if (info.adaptive)
    {
        if (U > info.Pmax) // Pins are too large
        {
            // Calculate how many pins we need to be under maximum pin width
            Np = ceil(0.5 * ((info.L / info.Pmax) + S));
        }
        else if (U < info.Pmin) // Pins are too small
        {
            // Calculate how many pins are needed to be above minimum pin width
            Np = floor(0.5 * ((info.L / info.Pmin) + S));
        }
        else
        {
            // Current number of pins meets constraints
            Np = info.N;
        }

        R = 2 * Np - S;
        U = info.L / R;
        V = U;
    }

    if (info.fixedGap)
    {
        U = info.Pfix;
        V = info.Gfix;
    }
    else if (info.fixedPin)
    {
        U = info.Pfix;
        V = (info.L - (Np * U)) / (Np - S);

        if (V <= 0)
        {
            throw regenError("Pin width too large for given number of pins", ["pinW"]);
        }
    }

    var SO = (info.sense) ? 0 * meter : U;

    var cuboidCorners = { "seed1" : vector(info.xmin + info.O + SO, info.ymin, info.zmin),
        "seed2" : vector(info.xmin + info.O + SO + V, info.ymax, info.zmax),
        "edgeOffsetA1" : vector(info.xmin,           info.ymin, info.zmin),
        "edgeOffsetA2" : vector(info.xmin + info.O,  info.ymax, info.zmax),
        "edgeOffsetB1" : vector(info.xmax - info.OT, info.ymin, info.zmin),
        "edgeOffsetB2" : vector(info.xmax,           info.ymax, info.zmax) };

    var worldXform = toWorld(parameters.csys);

    var transforms is array = makeArray(Np - S);
    var instanceNames is array = makeArray(Np - S);

    for (var i = 0; i < Np - S; i += 1)
    {
        transforms[i] = transform((U + V) * i * parameters.csys.xAxis);
        instanceNames[i] = "cuboid" ~ i;
    }

    return { "N" : Np, "cuboidCorners" : cuboidCorners, "worldXform" : worldXform, "patternInfo" : { "N" : Np - S, "transforms" : transforms, "instanceNames" : instanceNames }, edgeOffset: info.O, tailOffset: info.OT };
}

/**
 * Function finds all edges on `ownerBody` created by `id` which are concave.
 */
function findConcaveCreatedEdges(context is Context, id is Id, ownerBody is Query) returns Query
{
    var edges = qOwnedByBody(qCreatedBy(id, EntityType.EDGE), ownerBody);
    return qFilterFunction(edges, context, function(x)
        {
            return evEdgeConvexity(context, { "edge" : x }) == EdgeConvexityType.CONCAVE;
        });
}

/**
 * Function finds all planar faces on `ownerBody` which have normal parallel to `normal`
 */
function findOrientedCreatedFaces(context is Context, id is Id, ownerBody is Query, normal is Vector) returns Query
{
    return qParallelPlanes(qOwnedByBody(qCreatedBy(id, EntityType.FACE), ownerBody), normal);
}

/**
 * Function finds the end faces of pins created by a cut defined by the parameters in `definition`.
 * End faces are defined as those which have all vertices lying on or within `intersectionBody`, which
 * is the intersection volume prior to cutting pins.
 */
function findCreatedPinFaces(context is Context, intersectionBody is Query, cSys is CoordSystem, definition is map) returns Query
{
    // All faces of the tab, after pins have been cut
    // var allFaces = qOwnedByBody(definition.tab, EntityType.FACE);
    // The large faces of the laser-cuttable tab part.  Assumed to have equal area and orientation.
    // var largeFaces = qLargest(qGeometry(allFaces, GeometryType.PLANE));
    // All faces bounding the outer loop of the largest faces.
    var loopFaces = getOuterLoopFaces(context, definition.tab);
    // All faces oriented with a normal parallel to the axis of the joint.
    var jointFaces = qParallelPlanes(loopFaces, cSys.xAxis, true);

    // Compute the position of the joint in the tab coordinate frame
    var intersectionBox = evBox3d(context, {
            "topology" : intersectionBody,
            "cSys" : cSys,
            "tight" : true
        });


    // Define a function to filter faces to find pin terminal faces.
    // This gets slightly complicated becase the pin faces are not technically created by the cut; they're what's left over after the cut happens.

    // Default to returning a filter function that rejects all faces
    var facePositionFunction = function(x)
        {
            return false;
        };

    // The coordinate system has X along the intersection line, Z normal to the face, and Y either toward or away from the joint.
    // Assume that the tab is inserted into the base at an edge of the tab part; in this case, the entire joint will be in +ve or -ve Y,
    // and not around the origin.  For joints with intersection geometry in -ve Y coordinates, the joint "insertion direction" is also
    // along negative Y, and similarly for joints in +ve Y coordinates.
    // NOTE: This will yield unexpected behavior in unusual cases where the tab is nonconvex and the joint occurs in -ve Y coordinates but
    // with a +ve "insertion direction" and vice versa.
    //
    // First, decide which case we're dealing with
    if (intersectionBox.minCorner[1] < 0 && intersectionBox.maxCorner[1] < 0)
    {
        facePositionFunction = function(x)
            {
                // Find the tangent plane at the center of the face we're testing
                var testPlane = evFaceTangentPlane(context, {
                        "face" : x,
                        "parameter" : vector(0.5, 0.5)
                    });
                // Since the joint is in negative Y space, The normal of a pin face should be in the -ve Y direction
                // Note that in principle these faces could be curved, and so don't require the faces to be planes or the
                // normal to be exactly oriented along the Y axis.
                // Otherwise this face is not a pin face, so return false
                if (dot(testPlane.normal, yAxis(cSys)) > 0)
                {
                    return false;
                }

                // This is a design decision: There will still be some loop faces satisfying the previous criteria; here
                // we filter to faces that lie entirely on the surface of the intersection volume.
                // In cases where the tab part is wider than the base, this will have the effect of removing pin faces which are not
                // entirely internal to the joint.
                var testVertices = qAdjacent(x, AdjacencyType.VERTEX, EntityType.VERTEX);
                for (var vertex in evaluateQuery(context, testVertices))
                {
                    if (size(evaluateQuery(context, qContainsPoint(intersectionBody, evVertexPoint(context, { "vertex" : vertex })))) == 0)
                    {
                        return false;
                    }
                }

                // Correcting for a corner case: if the selections are reversed, and the "base" part is actually smaller, and
                // inserted into the middle of the "tab" part, then faces on the holes in the tab part will be selected as "pin faces"
                // when in fact the  "pins" end up on the "base" part.  While in practice the user should reverse the selection of parts
                // with that geometry, the feature should not fail. We know that the pin faces must occur on the side of the intersection
                // away from the coordinate system origin (by earlier assumption, and so we check to make sure the face is farther than the
                // minimum distance to the intersection volume.  By construction, we know that in the joint coordinate system, the min/max
                // corner of the intersection volume is on a planar face of the intersection volume aligned with the joint XZ plane.
                //
                // Use a dot prouct and tolerantEquals to check that the face is at the far side of the intersection volume
                if (tolerantEquals(dot(fromWorld(cSys, testPlane.origin) - intersectionBox.maxCorner, vector(0, 1, 0)), 0 * meter))
                {
                    return false;
                }

                return true;
            };
    } //As before, but with the intersection in +ve Y coordinates.
    else if (intersectionBox.minCorner[1] > 0 && intersectionBox.maxCorner[1] > 0)
    {
        facePositionFunction = function(x)
            {
                var testPlane = evFaceTangentPlane(context, {
                        "face" : x,
                        "parameter" : vector(0.5, 0.5)
                    });

                // Sign flipped from above
                if (dot(testPlane.normal, yAxis(cSys)) < 0)
                {
                    return false;
                }

                var testVertices = qAdjacent(x, AdjacencyType.VERTEX, EntityType.VERTEX);
                for (var vertex in evaluateQuery(context, testVertices))
                {
                    if (size(evaluateQuery(context, qContainsPoint(intersectionBody, evVertexPoint(context, { "vertex" : vertex })))) == 0)
                    {
                        return false;
                    }
                }
                if (tolerantEquals(dot(fromWorld(cSys, testPlane.origin) - intersectionBox.minCorner, vector(0, 1, 0)), 0 * meter))
                {
                    return false;
                }

                return true;
            };
    } // This else is just to avoid silent failures in weird cases; if this error comes up a lot, then that means a fix is needed.
    else
    {
        throw "Error: Could not determine Y-Axis to find pin end faces";
    }

    return qFilterFunction(qSubtraction(loopFaces, jointFaces), context, facePositionFunction);
}

/**
 * Function returns all edges on `ownerBody` created by `id` which are convex and at the end of a pin.
 */
function findCreatedPinEdges(context is Context, id is Id, intersectionBody is Query, cSys is CoordSystem, pinFaces is Query, definition is map) returns Query
{
    var edges = qOwnedByBody(qCreatedBy(id, EntityType.EDGE), definition.tab);

    // All faces of the tab, after pins have been cut
    var allFaces = qOwnedByBody(definition.tab, EntityType.FACE);
    // The large faces of the laser-cuttable tab part.  Assumed to have equal area and orientation.
    var largeFaces = qLargest(qGeometry(allFaces, GeometryType.PLANE));
    // All edges bounding the largest faces.
    var loopEdges = qAdjacent(largeFaces, AdjacencyType.EDGE, EntityType.EDGE);
    // All edges adjacent to pin faces that are on the edge of the part.
    var pinEdges = qAdjacent(pinFaces, AdjacencyType.EDGE, EntityType.EDGE);

    // All edges of interest that are oriented correctly.
    var orientedEdges = qSubtraction(qUnion([edges, pinEdges]), loopEdges);

    // Just those that are convex.
    var convexEdges = qFilterFunction(orientedEdges, context, function(x)
    {
        return evEdgeConvexity(context, { "edge" : x }) == EdgeConvexityType.CONVEX;
    });

    // Compute the position of the joint in the tab coordinate frame
    var intersectionBox = evBox3d(context, {
            "topology" : intersectionBody,
            "cSys" : cSys,
            "tight" : true
        });


    // Define a function to filter faces to find pin terminal edges.

    // Default to returning a filter function that rejects all edges
    var edgePositionFunction = function(x)
        {
            return false;
        };

    // As in findCreatedPinFaces, decide which case we're dealing with
    if (intersectionBox.minCorner[1] < 0 && intersectionBox.maxCorner[1] < 0)
    {
        edgePositionFunction = function(x)
            {
                // Find the point at the center of the face we're testing
                var testLine = evEdgeTangentLine(context, {
                        "edge" : x,
                        "parameter" : 0.5
                    });

                // Use a dot product and tolerantEquals to check that the edge is not at the near side of the intersection volume
                if (tolerantEquals(dot(fromWorld(cSys, testLine.origin) - intersectionBox.maxCorner, vector(0, 1, 0)), 0 * meter))
                {
                    return false;
                }

                return true;
            };
    } //As before, but with the intersection in +ve Y coordinates.
    else if (intersectionBox.minCorner[1] > 0 && intersectionBox.maxCorner[1] > 0)
    {
        edgePositionFunction = function(x)
            {
                // Find the point at the center of the face we're testing
                var testLine = evEdgeTangentLine(context, {
                        "edge" : x,
                        "parameter" : 0.5
                    });

                // Use a dot product and tolerantEquals to check that the edge is not at the near side of the intersection volume
                if (tolerantEquals(dot(fromWorld(cSys, testLine.origin) - intersectionBox.minCorner, vector(0, 1, 0)), 0 * meter))
                {
                    return false;
                }

                return true;
            };
    } // This else is just to avoid silent failures in weird cases; if this error comes up a lot, then that means a fix is needed.
    else
    {
        throw "Error: Could not determine Y-Axis to find pin end faces";
    }

    return qFilterFunction(convexEdges, context, edgePositionFunction);
}


/**
 * Function finds and deletes small disconnected components created by certain multi-part joint geometries.
 * Parts will be deleted if they are less that 0.1% the volume of the parent body.
 * Disconnected components larger than this will not be deleted, and will generate a warning instead.
 */
function cleanDisconnectedComponents(context is Context, id is Id, cutId is Id, tol is number)
{
    var components = qOwnerBody(qCreatedBy(cutId));

    // If there's one (or somehow zero) resulting parts, do no further work
    if (size(evaluateQuery(context, components)) < 2)
    {
        return;
    }

    var rootPart = qLargest(components);

    var rootVolume = evVolume(context, {
            "entities" : rootPart
        });
    var disconnectedVolume = evVolume(context, {
            "entities" : qSubtraction(components, rootPart)
        });

    var ratio = disconnectedVolume / rootVolume;

    if (ratio < tol)
    {
        opDeleteBodies(context, id + "deleteDisconnectedComponents", {
                    "entities" : qSubtraction(components, rootPart)
                });

        reportFeatureInfo(context, newId() + id[0], "Note: Laser Joint created small disconnected components which were deleted.");
    }
    else
    {
        reportFeatureWarning(context, newId() + id[0], "Warning: Laser Joint created large disconnected components. Check result to ensure correct geometry.");
    }
}


/**
 * Function returns a 3D direction Vector along the direction of the joint
 */
function getXAxis(context is Context, id is Id, tab is Query, base is Query) returns Vector
{
    var tabFace = getLargestFace(context, tab);
    var baseFace = getLargestFace(context, base);

    var tabPlane = evPlane(context, {
            "face" : tabFace
        });
    var basePlane = evPlane(context, {
            "face" : baseFace
        });

    return normalize(cross(tabPlane.normal, basePlane.normal));
}

/**
 * Ensure that in single mode adaptive is disabled, and in automatic fixed pin is disabled.
 */
export function onDefinitionChange(context is Context, id is Id, oldDefinition is map, definition is map,
    isCreating is boolean, specifiedParameters is map, hiddenBodies is Query) returns map
{
    if (definition.operationType == LaserJointOperationType.SINGLE)
    {
        if (definition.adaptive)
        {
            definition.adaptive = false;
        }
    }

    if (definition.operationType == LaserJointOperationType.AUTO)
    {
        if (definition.fixedPinWidth)
        {
            definition.fixedPinWidth = false;
        }
    }

    return definition;
}

export const PIN_BOUNDS =
{
            (unitless) : [1, 3, 12]
        } as IntegerBoundSpec;

export const PIN_WIDTH_MIN_BOUNDS =
{
            (meter) : [1e-5, 0.01, 500],
            (centimeter) : 1.0,
            (millimeter) : 10.0,
            (inch) : 0.375,
            (foot) : 0.1,
            (yard) : 0.1
        } as LengthBoundSpec;

export const PIN_WIDTH_MAX_BOUNDS =
{
            (meter) : [1e-5, 0.05, 500],
            (centimeter) : 5.0,
            (millimeter) : 50.0,
            (inch) : 2.0,
            (foot) : 1,
            (yard) : 1
        } as LengthBoundSpec;

export const CORNER_OVERCUT_LENGTH_BOUNDS =
{
            (meter) : [1e-5, 0.001, 500],
            (centimeter) : 0.1,
            (millimeter) : 1.0,
            (inch) : 0.03,
            (foot) : 0.01,
            (yard) : 0.01
        } as LengthBoundSpec;

export const FIT_OFFSET_LENGTH_BOUNDS =
{
            (meter) : [-500, 0.001, 500],
            (centimeter) : 0.01,
            (millimeter) : 0.1,
            (inch) : 0.005,
            (foot) : 0.001,
            (yard) : 0.001
        } as LengthBoundSpec;

export const TOL_BOUNDS =
{
            (unitless) : [0, 0.001, 1]
        } as RealBoundSpec;

