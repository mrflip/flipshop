
//_______________________________________________________________________________________________________________________________________________
//
// This FeatureScript is owned by Michael Pascoe and is distributed by CADSharp LLC.
// You may not redistribute it for commercial purposes without the permission of said owner and CADSharp LLC. Copyright (c) 2023 Michael Pascoe.
//_______________________________________________________________________________________________________________________________________________


FeatureScript 1403;
import(path : "onshape/std/geometry.fs", version : "1403.0");

// CADSharp
export import(path : "cbeb3dcf671e00785597bd76/409d65a3744fe434f32bdffc/a75ab01def146a42f55baa7f", version : "381046010d5aea697e433948");

Icon::import(path : "32653366b1560b3242dd56ac", version : "f9525ef2dd13b7c7d203e71d");

const MANIPULATOR_ID = "linear";

annotation {
        "Feature Type Name" : "Section",
        "Manipulator Change Function" :
        "manipulatorChange",
        "Icon" : Icon::BLOB_DATA,
        "Feature Type Description" : "<b> Summary </b> <br> Slices the part similar to a section view. <br>",
        "Editing Logic Function" : "autoSection" }
export const section = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Section type", "UIHint" : UIHint.HORIZONTAL_ENUM }
        definition.sectionType is sectionType;

        if (definition.sectionType == sectionType.PLANAR)
        {
            annotation { "Name" : "Plane or face", "Filter" : GeometryType.PLANE || EntityType.VERTEX || QueryFilterCompound.ALLOWS_PLANE, "MaxNumberOfPicks" : 2 }
            definition.rootPlane is Query;

            //Required to create a drop down menu.
            annotation { "Name" : "Section method" }
            definition.sectionMethod is sectionMethod;

            if (definition.sectionMethod == sectionMethod.UPTOVERTEX)
            {
                annotation { "Name" : "Up to vertex", "Filter" : (EntityType.VERTEX), "MaxNumberOfPicks" : 1 }
                definition.limitEntity is Query;
            }

            annotation { "Name" : "Offset" }
            isLength(definition.offset, { (inch) : [-1e5, 0, 1e5] } as LengthBoundSpec);
        }
        else
        {
            annotation { "Name" : "Section entity", "Filter" : EntityType.FACE || EntityType.EDGE && ConstructionObject.NO && SketchObject.YES || BodyType.SHEET && !EntityType.VERTEX }
            definition.sectionEntity is Query;
        }

        annotation { "Name" : "Opposite direction", "UIHint" : UIHint.OPPOSITE_DIRECTION }
        definition.oppositeDirection is boolean;

        annotation { "Name" : "Section all" }
        definition.sectionAll is boolean;

        if (definition.sectionAll == false)
        {
            annotation { "Name" : "Parts to section", "Filter" : EntityType.BODY }
            definition.partsToSection is Query;
        }
        if (definition.sectionType == sectionType.PROJECTED)
        {
            annotation { "Name" : "Keep tools" }
            definition.keepTools is boolean;
        }

        cadsharpUrlPredicate(definition);
    }
    {
        if (definition.sectionType == sectionType.PLANAR)
        {
            //________________________________________________________
            //
            //               Complex manipulator movement (Part 1)
            //
            // by Jake Rosenfeld - https://forum.onshape.com/discussion/15740/manipulator-offset#latest
            // Thank you Jake!
            //________________________________________________________

            var rootPlane = evPlane(context, {
                    "face" : definition.rootPlane
                });

            if (definition.sectionMethod == sectionMethod.UPTOVERTEX)
            {
                const limitPoint = evVertexPoint(context, { "vertex" : definition.limitEntity });
                const originalPlane = evPlane(context, { "face" : definition.rootPlane });
                rootPlane = plane(limitPoint, originalPlane.normal, originalPlane.x);
            }

            const manipulatorParameters = getManipulatorParameters(rootPlane, definition.offset, definition.oppositeDirection);
            const manipulator = linearManipulator({
                        "base" : manipulatorParameters.base,
                        "direction" : manipulatorParameters.direction,
                        "offset" : manipulatorParameters.offset,
                        "primaryParameterId" : "offset",
                    });
            addManipulators(context, id, { (MANIPULATOR_ID) : manipulator });

            const offsetPlane = plane(manipulator.base + (manipulator.offset * manipulator.direction), manipulator.direction);

            //________________________________________________________
            //
            //                      Split Parts
            //________________________________________________________

            var all = qAllNonMeshSolidBodies();
            const centroid3d = evApproximateCentroid(context, {
                        "entities" : all
                    });

            const centroid2d = project(offsetPlane, centroid3d);
            const localCsys = coordSystem(centroid2d, offsetPlane.x, offsetPlane.normal);
            const centroidPlane = plane(localCsys);
            const bbox = evBox3d(context, {
                        "topology" : all,
                        "tight" : false,
                        "cSys" : localCsys
                    });

            const bboxX = bbox.maxCorner[0] - bbox.minCorner[0];
            const bboxY = bbox.maxCorner[1] - bbox.minCorner[1];
            const bboxZ = bbox.maxCorner[2] - bbox.minCorner[2];

            const sketchEntities = newSketchOnPlane(context, id + "sketch1", {
                        "sketchPlane" : centroidPlane
                    });

            skRectangle(sketchEntities, "rectangle1", {
                        "firstCorner" : vector(bboxX, bboxY),
                        "secondCorner" : vector(-bboxX, -bboxY)
                    });

            skSolve(sketchEntities);
            const sketchItems = qCreatedBy(id + "sketch1", EntityType.FACE);
            const sketchToDelete = qCreatedBy(id + "sketch1", EntityType.EDGE);

            opExtrude(context, id + "extrude1", {
                        "entities" : sketchItems,
                        "direction" : offsetPlane.normal,
                        "endBound" : BoundingType.BLIND,
                        "endDepth" : bboxZ
                    });

            const removalTool = qCreatedBy(id + "extrude1", EntityType.BODY);

            var targets;

            if (evaluateQuery(context, definition.partsToSection) != [] && definition.sectionAll == false)
            {
                targets = definition.partsToSection;
            }

            if (definition.sectionAll == true)
            {
                targets = qSubtraction(all, removalTool);
            }

            opBoolean(context, id + "boolean1", {
                        "tools" : removalTool,
                        "operationType" : BooleanOperationType.SUBTRACTION,
                        "targets" : targets
                    });

            opDeleteBodies(context, id + "deleteBodies1", {
                        "entities" : sketchToDelete
                    });
        }
        else if (definition.sectionType == sectionType.PROJECTED)
        {
            //________________________________________________________
            //
            //                      Split Parts
            //________________________________________________________

            var allParts = qAllNonMeshSolidBodies();

            const sectionEntity = definition.sectionEntity;
            const face = evaluateQuery(context, qEntityFilter(sectionEntity, EntityType.FACE));
            const surface = evaluateQuery(context, qBodyType(sectionEntity, BodyType.SHEET));
            const sketchEntity = evaluateQuery(context, qEntityFilter(sectionEntity, EntityType.EDGE));

            var splitEntity;
            var toDelete = qNothing();

            if (size(face) > 0 || size(surface) > 0)
            {
                splitEntity = sectionEntity;
                debug(context, sectionEntity, DebugColor.GREEN);

                if (size(surface) > 0 && !definition.keepTools)
                {
                    toDelete = qUnion([toDelete, splitEntity]);
                }
            }
            else if (size(sketchEntity) > 0)
            {
                const solidEdges = qConstructionFilter(sectionEntity, ConstructionObject.NO);
                debug(context, solidEdges, DebugColor.GREEN);

                opExtrude(context, id + "extrudeSurface", {
                            "entities" : solidEdges,
                            "direction" : evOwnerSketchPlane(context, { "entity" : sectionEntity }).normal,
                            "endBound" : BoundingType.THROUGH_ALL,
                            "startBound" : BoundingType.THROUGH_ALL
                        });

                splitEntity = qCreatedBy(id + "extrudeSurface", EntityType.BODY);

                toDelete = qUnion([toDelete, splitEntity]);
            }

            try
            {
                opSplitPart(context, id + ("splitPart1"), {
                            "targets" : definition.sectionAll ? allParts : definition.partsToSection,
                            "tool" : splitEntity,
                        });

                //________________________________________________________
                //
                //                      Delete Half
                //________________________________________________________

                const halfDirection = definition.oppositeDirection ? true : false;
                const half = qSplitBy(id + ("splitPart1"), EntityType.BODY, halfDirection);

                toDelete = qUnion([toDelete, half]);

            }
            catch (error)
            {
                reportFeatureWarning(context, id, "Select parts to section");
            }

            if (size(evaluateQuery(context, toDelete)) != 0)
            {
                opDeleteBodies(context, id + "deleteBodies1", {
                            "entities" : toDelete,
                        });
            }
        }

    });

//________________________________________________________
//
//               Complex manipulator movement (Part 2)
//
// by Jake Rosenfeld - https://forum.onshape.com/discussion/15740/manipulator-offset#latest
// Thank you Jake!
//________________________________________________________

// Theoretically you could be more discerning about this by calculating it based off of evBox3d of the selected parts.
const A_VERY_LARGE_DISTANCE = 250 * meter;

function getManipulatorParameters(rootPlane is Plane, definitionOffset is ValueWithUnits, oppositeDirection is boolean) returns map
{
    // Manipulator will always face this direction
    const manipulatorDirection = (oppositeDirection ? -1 : 1) * rootPlane.normal;
    // Make sure manipulator does not flip, by placing the base point very far away
    const manipulatorBase = rootPlane.origin + (-manipulatorDirection * A_VERY_LARGE_DISTANCE);
    // Flipping `oppositeDirection` should not affect where on-screen the arrow is, but since we are moving
    // the base point all the way to the other side of the world, the offset of the manipulator (in reference
    // to the base point) has to change.  The simplest way to represent this mathematically is probably:
    // const worldSpaceBasePoint = rootPlane.origin + (definitionOffset * rootPlane.normal);
    // const manipulatorOffset = norm(worldSpaceBasePoint - manipulatorBase)
    // But that is fairly slow compared to just using the numbers directly
    const manipulatorOffset = A_VERY_LARGE_DISTANCE + ((oppositeDirection ? -1 : 1) * definitionOffset);

    return {
            "base" : manipulatorBase,
            "direction" : manipulatorDirection,
            "offset" : manipulatorOffset
        };
}

function getDefinitionOffsetFromManipulator(manipulatorOffset is ValueWithUnits, oppositeDirection is boolean) returns ValueWithUnits
{
    // Rearrange the equation from above:
    // manipulatorOffset = A_VERY_LARGE_DISTANCE + ((oppositeDirection ? -1 : 1) * definitionOffset);
    // (oppositeDirection ? -1 : 1) * (manipulatorOffset - A_VERY_LARGE_DISTANCE) = definitionOffset;
    return (oppositeDirection ? -1 : 1) * (manipulatorOffset - A_VERY_LARGE_DISTANCE);
}

export function manipulatorChange(context is Context, definition is map, newManipulators is map)
{
    const newOffset = newManipulators[MANIPULATOR_ID].offset;
    if (newOffset < 0 * meter)
    {
        // Some trickery here.  If the user clicks on the manipulator, treat that as flipping the oppositeDirection button.
        // this means that if the user ever drags past A_VERY_LARGE_DISTANCE, the manipulator will do something weird
        definition.oppositeDirection = !definition.oppositeDirection;
    }
    else
    {
        // User has dragged.  Update the offset.
        definition.offset = getDefinitionOffsetFromManipulator(newManipulators[MANIPULATOR_ID].offset, definition.oppositeDirection);
    }
    return definition;
}

//________________________________________________________
//
//                    Drop down menu
//________________________________________________________

export enum sectionMethod
{
    annotation { "Name" : "Blind" }
    BLIND,

    annotation { "Name" : "Up to vertex" }
    UPTOVERTEX
}

//________________________________________________________
//
//                    Type menu
//________________________________________________________

export enum sectionType
{
    annotation { "Name" : "Planar" }
    PLANAR,

    annotation { "Name" : "Projection / surface" }
    PROJECTED
}

//________________________________________________________
//
//                Auto detect section type
//________________________________________________________

// Reference: https://cad.onshape.com/documents/12312312345abcabcabcdeff/w/a855e4161c814f2e9ab3698a/e/577e2aaf45a542f98d9343c7
// Reference: Alex Kempen - https://forum.onshape.com/discussion/comment/71047#Comment_71047

export function autoSection(context is Context, id is Id, oldDefinition is map, definition is map, isCreating is boolean, specifiedParameters is map) returns map
{
    definition = cadsharpUrlFunctionForPreExistingEditLogic(oldDefinition, definition);

    if (oldDefinition != {}) // Only do anything on preselection
        return definition;

    if (size(evaluateQuery(context, definition.rootPlane)) == 2)
    {
        const autoVertex = qEntityFilter(definition.rootPlane, EntityType.VERTEX);
        const autoPlaneOrFace = qSubtraction(definition.rootPlane, autoVertex);
        const planeBody = qBodyType(qOwnerBody(definition.rootPlane), BodyType.SOLID);

        if (size(evaluateQuery(context, autoVertex)) == 1 && size(evaluateQuery(context, autoPlaneOrFace)) == 1)
        {
            definition.sectionMethod = sectionMethod.UPTOVERTEX;
            definition.limitEntity = autoVertex;
            definition.rootPlane = autoPlaneOrFace;
        }

        if (evaluateQuery(context, planeBody) != [] && evaluateQuery(context, definition.partsToSection) == [] && definition.sectionAll == false)
        {
            definition.partsToSection = planeBody;
        }
    }

    return definition;
}
