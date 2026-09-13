
//_______________________________________________________________________________________________________________________________________________
//
// This FeatureScript is owned by Michael Pascoe and is distributed by CADSharp LLC.
// You may not redistribute it for commercial purposes without the permission of said owner and CADSharp LLC. Copyright (c) 2023 Michael Pascoe.
//_______________________________________________________________________________________________________________________________________________


FeatureScript 1521;
import(path : "onshape/std/geometry.fs", version : "1521.0");

// CADSharp
export import(path : "cbeb3dcf671e00785597bd76/409d65a3744fe434f32bdffc/a75ab01def146a42f55baa7f", version : "381046010d5aea697e433948");

icon::import(path : "21abc3b4b8f40d72983d8e63", version : "2aa7774e97bb1ea00dfb15a1");
export import(path : "c7c08274a0d273b9a5f5b47d/0e2196c32b0b5b68264fb055/f0868abba1da15c8f6e98137", version : "e4fb18949b9851941fef2cd4");

export enum entryMethod
{
    annotation { "Name" : "Manual" }
    MANUAL,
    annotation { "Name" : "Reference" }
    REFERENCE,
    annotation { "Name" : "Equal" }
    EQUAL,
}

annotation {
        "Feature Type Name" : "Shelves",
        "Editing Logic Function" : "autoSection",
        "Icon" : icon::BLOB_DATA,
        "Feature Type Description" : "<br> <b>Summary</b> <br> Creates shelves between two parts.",
        "Description Image" : cadsharpLogo::BLOB_DATA,
    }
export const Shelves = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Group Name" : "Hole Settings", "Collapsed By Default" : true }
        {
            annotation { "Name" : "Hole diameter" }
            isLength(definition.diameter, { (inch) : [0, .25, 1e5] } as LengthBoundSpec);

            annotation { "Name" : "Hole depth" }
            isLength(definition.depth, { (inch) : [0, .5, 1e5] } as LengthBoundSpec);

            annotation { "Name" : "Hole distance from front" }
            isLength(definition.fromFront, { (inch) : [0, 2, 1e5] } as LengthBoundSpec);

            annotation { "Name" : "Hole distance from back" }
            isLength(definition.fromBack, { (inch) : [0, 2, 1e5] } as LengthBoundSpec);
        }

        annotation { "Group Name" : "Shelf Settings", "Collapsed By Default" : true }
        {
            annotation { "Name" : "Shelf thickness" }
            isLength(definition.shelfThickness, { (inch) : [0, .75, 1e5] } as LengthBoundSpec);

            annotation { "Name" : "Front clearance" }
            isLength(definition.frontClearance, { (inch) : [0, .25, 1e5] } as LengthBoundSpec);

            annotation { "Name" : "Back clearance" }
            isLength(definition.backClearance, { (inch) : [0, .125, 1e5] } as LengthBoundSpec);

            annotation { "Name" : "Side clearance" }
            isLength(definition.sideClearance, { (inch) : [0, .0625, 1e5] } as LengthBoundSpec);

            annotation { "Name" : "Nosing" }
            definition.nosing is boolean;

            if (definition.nosing)
            {
                annotation { "Name" : "Nosing depth" }
                isLength(definition.nosingDepth, { (inch) : [0, 1, 1e5] } as LengthBoundSpec);

                annotation { "Name" : "Nosing height" }
                isLength(definition.nosingHeight, { (inch) : [0, .75, 1e5] } as LengthBoundSpec);
            }
        }

        // annotation { "Name" : "Collapse groups" }
        // definition.collapse is boolean;

        annotation { "Name" : "Instances", "Item name" : "Shelves & holes", "Item label template" : "Shelves & holes", "UIHint" : UIHint.COLLAPSE_ARRAY_ITEMS }
        definition.groups is array;
        for (var group in definition.groups)
        {
            annotation { "Name" : "Side faces (Left then right)", "Filter" : EntityType.FACE, "MaxNumberOfPicks" : 2 }
            group.sideFaces is Query;

            annotation { "Name" : "Merge scope", "Filter" : EntityType.BODY, "MaxNumberOfPicks" : 2 }
            group.mergeScope is Query;

            annotation { "Name" : "Entry method", "UIHint" : UIHint.HORIZONTAL_ENUM }
            group.entryMethod is entryMethod;

            if (group.entryMethod == entryMethod.REFERENCE || group.entryMethod == entryMethod.EQUAL)
            {
                annotation { "Name" : "Reference entities", "Filter" : EntityType.FACE || EntityType.VERTEX || EntityType.EDGE, "MaxNumberOfPicks" : 2 }
                group.referenceEntities is Query;

                if (group.entryMethod == entryMethod.REFERENCE)
                {
                    annotation { "Name" : "Holes between ref" }
                    isInteger(group.holeQtyBetweenReferences, { (unitless) : [1, 5, 5000] } as IntegerBoundSpec);

                    annotation { "Name" : "1st pin from ref" }
                    isInteger(group.referenceStartingPin, { (unitless) : [-5000, 1, 5000] } as IntegerBoundSpec);

                    annotation { "Name" : "Offset from ref" }
                    isLength(group.offsetFromRef, { (inch) : [-1e5, 0, 1e5] } as LengthBoundSpec);
                }
                if (group.entryMethod == entryMethod.EQUAL)
                {
                    annotation { "Name" : "Holes between shelves" }
                    isInteger(group.holesBetweenShelves, { (unitless) : [1, 5, 5000] } as IntegerBoundSpec);

                    annotation { "Name" : "Holes below" }
                    isInteger(group.holesBelow, { (unitless) : [1, 3, 5000] } as IntegerBoundSpec);

                    annotation { "Name" : "Holes above" }
                    isInteger(group.holesAbove, { (unitless) : [1, 3, 5000] } as IntegerBoundSpec);
                }
            }

            if (group.entryMethod == entryMethod.MANUAL)
            {
                annotation { "Name" : "Hole spacing" }
                isLength(group.spacing, { (inch) : [0, 2, 1e5] } as LengthBoundSpec);

                annotation { "Name" : "Distance from bottom" }
                isLength(group.fromBottom, { (inch) : [0, 4, 1e5] } as LengthBoundSpec);

            }

            if (group.entryMethod != entryMethod.EQUAL)
            {
                annotation { "Name" : "Hole quantity" }
                isInteger(group.holeQty, { (unitless) : [1, 10, 5000] } as IntegerBoundSpec);

                annotation { "Name" : "First shelf starting pin" }
                isInteger(group.firstShelf, POSITIVE_COUNT_BOUNDS);

                annotation { "Name" : "Shelf spacing" }
                isInteger(group.shelfSpacing, { (unitless) : [1, 3, 5000] } as IntegerBoundSpec);
            }

            annotation { "Name" : "Shelf quantity" }
            isInteger(group.shelfQty, { (unitless) : [1, 3, 5000] } as IntegerBoundSpec);

            annotation { "Name" : "Override hole spacing" }
            group.overrideHoleSpacing is boolean;

            if (group.overrideHoleSpacing)
            {
                annotation { "Name" : "Hole spacing" }
                isLength(group.equalSingleHoleSpacing, { (inch) : [0, 2, 1e5] } as LengthBoundSpec);
            }
        }

        cadsharpUrlPredicate(definition);
    }
    {

        const nosingDepth = definition.nosing ? definition.nosingDepth : 0 * inch;

        var bodiesToDelete;
        const verticalLine = opFitSpline(context, id + "fitSpline1", {
                    "points" : [
                        vector(0, 0, 0) * inch,
                        vector(0, 0, 1) * inch
                    ]
                });
        const verticalEdge = qCreatedBy(id + "fitSpline1", EntityType.EDGE);

        bodiesToDelete = verticalEdge;

        for (var i = 0; i < size(definition.groups); i += 1)
        {
            try
            {

                const sideFaces = evaluateQuery(context, definition.groups[i].sideFaces);

                const evalPlane = evFaceTangentPlane(context, {
                            "face" : sideFaces[0],
                            "parameter" : vector(0.5, 0.5)
                        });

                //const evalPlaneYdirection = vector(0, 0, 1);//cross(-evalPlane.normal, evalPlane.x); //Y direction of a plane
                const localCsys = planeToCSys(evalPlane);
                const bbox = evBox3d(context, {
                            "topology" : definition.groups[i].sideFaces,
                            "tight" : true,
                            "cSys" : localCsys
                        });

                const bboxX = bbox.maxCorner[0] - bbox.minCorner[0];
                const bboxY = bbox.maxCorner[1] - bbox.minCorner[1];
                const bboxZ = bbox.maxCorner[2] - bbox.minCorner[2];

                var holeSpacing;
                var distanceFromBottom;
                var holeQty;
                var firstShelf;
                var shelfSpacing;

                if (definition.groups[i].entryMethod == entryMethod.MANUAL)
                {
                    holeSpacing = definition.groups[i].spacing;
                    distanceFromBottom = definition.groups[i].fromBottom;
                    holeQty = definition.groups[i].holeQty;
                    firstShelf = definition.groups[i].firstShelf - 1;
                    shelfSpacing = definition.groups[i].shelfSpacing;
                }
                if (definition.groups[i].entryMethod == entryMethod.REFERENCE || definition.groups[i].entryMethod == entryMethod.EQUAL)
                {
                    var distanceBetween = measuringFunctionPascoe(id, context, i, definition.groups[i].referenceEntities, true);
                    const bottomPoint = evFaceTangentPlane(context, {
                                "face" : sideFaces[0],
                                "parameter" : vector(0.5, 0)
                            });
                    const bottomPlane = plane(bottomPoint.origin, cross(bottomPoint.x, bottomPoint.normal), bottomPoint.x);
                    const adjEdges = qAdjacent(sideFaces[0], AdjacencyType.EDGE, EntityType.EDGE);
                    const bottomEdge = qClosestTo(adjEdges, bottomPoint.origin);
                    const bottomReference = qClosestTo(definition.groups[i].referenceEntities, bottomPoint.origin);
                    const distanceToBottom = evDistance(context, {
                                    "side0" : bottomReference,
                                    "side1" : bottomPlane
                                }).distance; //measuringFunctionPascoe(id, context, i, qUnion([bottomReference, bottomEdge]), true);

                    if (definition.groups[i].entryMethod == entryMethod.REFERENCE)
                    {
                        holeSpacing = distanceBetween / (definition.groups[i].holeQtyBetweenReferences - 1);
                        distanceFromBottom = distanceToBottom - holeSpacing * (definition.groups[i].referenceStartingPin - 1) + definition.groups[i].offsetFromRef;
                        holeQty = definition.groups[i].holeQty;
                        firstShelf = definition.groups[i].firstShelf - 1;
                        shelfSpacing = definition.groups[i].shelfSpacing;
                    }
                    if (definition.groups[i].entryMethod == entryMethod.EQUAL)
                    {
                        var nosingDifference = definition.nosing ? definition.nosingHeight - definition.shelfThickness : 0 * inch;
                        var nosingLogic = definition.nosing ? 1 : 0;
                        const spaceBetweenShelves = (distanceBetween - (definition.shelfThickness + nosingDifference) * definition.groups[i].shelfQty) / (definition.groups[i].shelfQty + 1);

                        shelfSpacing = spaceBetweenShelves + definition.shelfThickness + nosingDifference;
                        holeSpacing = shelfSpacing / (definition.groups[i].holesBetweenShelves - 1);
                        distanceFromBottom = distanceToBottom + spaceBetweenShelves - definition.groups[i].holesBelow * holeSpacing + nosingDifference;
                        firstShelf = definition.groups[i].holesBelow;
                        holeQty = (shelfSpacing / holeSpacing) * (definition.groups[i].shelfQty - 1) + 1 + definition.groups[i].holesBelow + definition.groups[i].holesAbove;

                        if (definition.groups[i].overrideHoleSpacing && definition.groups[i].shelfQty == 1)
                        {
                            holeSpacing = definition.groups[i].equalSingleHoleSpacing;
                            holeQty = 1 + definition.groups[i].holesBelow + definition.groups[i].holesAbove;
                            distanceFromBottom = distanceToBottom + distanceBetween / 2 + nosingDifference - (nosingDifference + definition.shelfThickness) / 2 - (definition.groups[i].holesBelow * holeSpacing);
                            firstShelf = definition.groups[i].holesBelow;

                        }
                    }

                }
                if (definition.groups[i].entryMethod == entryMethod.EQUAL)
                {

                }

                const sketchEntities = newSketchOnPlane(context, id + i + "sketch1", {
                            "sketchPlane" : evalPlane
                        });

                const left = -bboxX / 2;
                const right = bboxX / 2;
                const bottom = -bboxY / 2;

                skCircle(sketchEntities, "circle1", {
                            "center" : vector(left + definition.fromFront, bottom + distanceFromBottom),
                            "radius" : definition.diameter
                        });

                skCircle(sketchEntities, "circle2", {
                            "center" : vector(right - definition.fromBack, bottom + distanceFromBottom),
                            "radius" : definition.diameter
                        });

                skRectangle(sketchEntities, "rectangle1", {
                            "firstCorner" : vector(left + definition.frontClearance + nosingDepth, bottom + distanceFromBottom + (firstShelf * holeSpacing)),
                            "secondCorner" : vector(right - definition.backClearance, bottom + distanceFromBottom + (firstShelf * holeSpacing) + definition.shelfThickness)
                        });

                if (definition.nosing)
                {
                    skRectangle(sketchEntities, "rectangle2", {
                                "firstCorner" : vector(-bboxX / 2 + definition.frontClearance, -bboxY / 2 + distanceFromBottom + (firstShelf * holeSpacing) + (definition.shelfThickness - definition.nosingHeight)),
                                "secondCorner" : vector(-bboxX / 2 + definition.frontClearance + nosingDepth - (.001 * inch), -bboxY / 2 + distanceFromBottom + (firstShelf * holeSpacing) + definition.shelfThickness)
                            });
                }

                skSolve(sketchEntities);
                const sketchItems = qCreatedBy(id + i + "sketch1", EntityType.FACE);
                var sketchedCircles = qSmallest(sketchItems);
                var sketchedRectangle = qSubtraction(sketchItems, sketchedCircles);

                opExtrude(context, id + i + "extrude1", {
                            "entities" : sketchedCircles,
                            "direction" : -evalPlane.normal,
                            "endBound" : BoundingType.BLIND,
                            "endDepth" : definition.depth,
                            "startBound" : BoundingType.BLIND,
                            "startDepth" : bboxZ + definition.depth
                        });

                opExtrude(context, id + i + "extrude2", {
                            "entities" : sketchedRectangle,
                            "direction" : evalPlane.normal,
                            "endBound" : BoundingType.BLIND,
                            "endDepth" : bboxZ - definition.sideClearance,
                            "startBound" : BoundingType.BLIND,
                            "startDepth" : -definition.sideClearance
                        });

                opBoolean(context, id + i + "boolean1", {
                            "tools" : qCreatedBy(id + i + "extrude1", EntityType.BODY),
                            "targets" : definition.groups[i].mergeScope,
                            "operationType" : BooleanOperationType.SUBTRACTION
                        });

                setProperty(context, {
                            "entities" : qCreatedBy(id + i + "extrude2", EntityType.BODY),
                            "propertyType" : PropertyType.APPEARANCE,
                            "value" : color(1, 1, 1)
                        });

                const holeFaces = qCreatedBy(id + i + "boolean1", EntityType.FACE);

                var transforms = [];
                var instanceNames = [];

                for (var k = 0; k < holeQty; k += 1)
                {
                    const holeTransform = transform(vector(0 * inch, 0 * inch, holeSpacing * (k + 1)));

                    transforms = append(transforms, holeTransform);
                    instanceNames = append(instanceNames, "face" ~ k);
                }

                linearPattern(context, id + i + "linearPattern1", {
                            "patternType" : PatternType.FACE,
                            "faces" : holeFaces,
                            "directionOne" : verticalEdge,
                            "oppositeDirection" : false,
                            "distance" : holeSpacing,
                            "instanceCount" : holeQty
                        });

                linearPattern(context, id + i + "linearPattern2", {
                            "patternType" : PatternType.PART,
                            "entities" : qCreatedBy(id + i + "extrude2", EntityType.BODY),
                            "directionOne" : verticalEdge,
                            "oppositeDirection" : false,
                            "distance" : shelfSpacing,
                            "instanceCount" : definition.groups[i].shelfQty
                        });

                bodiesToDelete = qUnion([bodiesToDelete, qCreatedBy(id + i + "sketch1", EntityType.EDGE)]);

            }
            catch
            {
            }
        }

        opDeleteBodies(context, id + "deleteBodies1", {
                    "entities" : bodiesToDelete
                });

    });


export function autoSection(context is Context, id is Id, oldDefinition is map, definition is map, isCreating is boolean, specifiedParameters is map) returns map
{
    definition = cadsharpUrlFunctionForPreExistingEditLogic(oldDefinition, definition);

    if (size(definition.groups) != 0)
    {
        for (var i = 0; i < size(definition.groups); i += 1)
        {
            //if (oldDefinition == {}) // Only do anything on preselection /////////////////
            //return definition;

            if (size(evaluateQuery(context, definition.groups[i].mergeScope)) == 0)
            {

                const sideFaces = evaluateQuery(context, definition.groups[i].sideFaces);
                const evalPlane = evFaceTangentPlane(context, {
                            "face" : sideFaces[0],
                            "parameter" : vector(0.5, 0.5)
                        });
                const autoMergeScope = qOwnerBody(qUnion([sideFaces[0], sideFaces[1]]));

                if (size(evaluateQuery(context, autoMergeScope)) != 0)
                {
                    definition.groups[i].mergeScope = autoMergeScope;
                }
            }
        }
    }

    return definition;
}
