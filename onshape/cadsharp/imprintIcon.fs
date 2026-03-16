
//_______________________________________________________________________________________________________________________________________________
//
// This FeatureScript is owned by Michael Pascoe and is distributed by CADSharp LLC.
// You may not redistribute it for commercial purposes without the permission of said owner and CADSharp LLC. Copyright (c) 2023 Michael Pascoe.
//_______________________________________________________________________________________________________________________________________________


FeatureScript 2180;
import(path : "onshape/std/common.fs", version : "2180.0");

icon::import(path : "2674c1d0e69800714c7c6573", version : "d4ddaaaee58e26e19a7f3c56");

// CADSharp
export import(path : "cbeb3dcf671e00785597bd76/144bf6a7fdc989e9e28ce5ea/a75ab01def146a42f55baa7f", version : "dc78e9b85c9f16ea9e131d3f");

// Tools
import(path : "c7c08274a0d273b9a5f5b47d/119d2395f2a061bfb69f36ad/9064c2e0210d7464ff67c66d", version : "d09cc7609a63197abf768549");


export enum ScopeType
{
    annotation { "Name" : "New" }
    NEW,
    annotation { "Name" : "Add" }
    ADD,
    annotation { "Name" : "Sub" }
    REMOVE,
    annotation { "Name" : "Intersect" }
    INTERSECT,
    annotation { "Name" : "Split" }
    SPLIT,
    annotation { "Name" : "Surface" }
    SURFACE
}

export enum SettingsMenu
{
    annotation { "Name" : "Scale" }
    SIZE,
    annotation { "Name" : "Move" }
    LOCATION,
    annotation { "Name" : "Other" }
    OTHER,
}

export enum SizeOrientation
{
    annotation { "Name" : "Width" }
    WIDTH,
    annotation { "Name" : "Height" }
    HEIGHT,
}

export enum Method
{
    annotation { "Name" : "Extrude up to part" }
    EXTRUDE_UP_TO_PART,
    annotation { "Name" : "Extrude up to face" }
    EXTRUDE_UP_TO_FACE,
    annotation { "Name" : "Thicken from face" }
    THICKEN_FROM_FACE,
}

export enum Position
{
    annotation { "Name" : "Top left" }
    TOP_LEFT,
    annotation { "Name" : "Top center" }
    TOP_CENTER,
    annotation { "Name" : "Top right" }
    TOP_RIGHT,
    annotation { "Name" : "Middle left" }
    MID_LEFT,
    annotation { "Name" : "Center" }
    CENTER,
    annotation { "Name" : "Middle right" }
    MID_RIGHT,
    annotation { "Name" : "Bottom left" }
    BOTTOM_LEFT,
    annotation { "Name" : "Bottom center" }
    BOTTOM_CENTER,
    annotation { "Name" : "Bottom right" }
    BOTTOM_RIGHT,
}

annotation {
        "Feature Type Name" : "Imprint",
        "Icon" : icon::BLOB_DATA,
        "Description Image" : cadsharpLogo::BLOB_DATA,
        "Feature Type Description" : "<b> Summary </b> <br> Imprints a sketch via extrude. <br>",
        "Editing Logic Function" : "EditLogic",
        "Manipulator Change Function" : "ManipulatorChange" }
export const imprint = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "isFace", "UIHint" : [UIHint.ALWAYS_HIDDEN, UIHint.UNCONFIGURABLE] }
        definition.isFace is boolean;

        annotation { "Name" : "Scope", "UIHint" : [UIHint.HORIZONTAL_ENUM, UIHint.REMEMBER_PREVIOUS_VALUE] }
        definition.scopeType is ScopeType;

        annotation { "Name" : "Target face", "Filter" : EntityType.FACE || BodyType.MATE_CONNECTOR, "MaxNumberOfPicks" : 1, "UIHint" : UIHint.INITIAL_FOCUS }
        definition.face is Query;

        if (!definition.isFace)
        {
            annotation { "Name" : "Up to face", "Filter" : EntityType.FACE, "MaxNumberOfPicks" : 1 }
            definition.referenceFace is Query;
        }

        annotation {
                    "Default Purpose" : "ONSHAPE_WELDMENT_PROFILE",
                    "Name" : "Sketch profile",
                    "Filter" : PartStudioItemType.SKETCH,
                    "MaxNumberOfPicks" : 1,
                    "UIHint" : [UIHint.REMEMBER_PREVIOUS_VALUE, UIHint.DISPLAY_SHORT]
                }
        definition.profileSketch is PartStudioData;

        annotation { "Group Name" : "Settings", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Settings", "UIHint" : UIHint.HORIZONTAL_ENUM }
            definition.settingsMenu is SettingsMenu;

            if (definition.settingsMenu == SettingsMenu.SIZE)
            {
                if (definition.isFace)
                {
                    annotation { "Name" : "Auto size", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
                    definition.autoSize is boolean;
                }

                if (!definition.autoSize || !definition.isFace)
                {
                    annotation { "Name" : "Size orientation", "UIHint" : [UIHint.SHOW_LABEL, UIHint.REMEMBER_PREVIOUS_VALUE] }
                    definition.sizeOrientation is SizeOrientation;
                }

                if (definition.autoSize && definition.isFace)
                {
                    annotation { "Name" : "Offset", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
                    isLength(definition.offset, { (inch) : [-10000, 0, 10000] } as LengthBoundSpec);
                }
                else
                {
                    annotation { "Name" : "Size", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
                    isLength(definition.size, { (inch) : [-10000, 1.0, 10000] } as LengthBoundSpec);
                }

                if (definition.scopeType == ScopeType.NEW || definition.scopeType == ScopeType.ADD || definition.scopeType == ScopeType.REMOVE || definition.scopeType == ScopeType.INTERSECT)
                {
                    annotation { "Name" : "Depth", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
                    isLength(definition.depth, { (inch) : [-10000, 0.1, 10000] } as LengthBoundSpec);
                }
            }
            else if (definition.settingsMenu == SettingsMenu.LOCATION)
            {
                annotation { "Name" : "Sketch reference", "Default" : Position.CENTER, "UIHint" : [UIHint.SHOW_LABEL, UIHint.REMEMBER_PREVIOUS_VALUE] }
                definition.sketchReference is Position;

                if (definition.isFace)
                {
                    annotation { "Name" : "X face position %", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
                    isInteger(definition.facePositionX, { (unitless) : [0, 50, 100] } as IntegerBoundSpec);

                    annotation { "Name" : "Y face position %", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
                    isInteger(definition.facePositionY, { (unitless) : [0, 50, 100] } as IntegerBoundSpec);
                }

                annotation { "Name" : "Offset X", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
                isLength(definition.dx, { (inch) : [-10000, 0, 10000] } as LengthBoundSpec);

                annotation { "Name" : "Offset X", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
                isLength(definition.dy, { (inch) : [-10000, 0, 10000] } as LengthBoundSpec);

                annotation { "Name" : "Offset X", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
                isLength(definition.dz, { (inch) : [-10000, 0, 10000] } as LengthBoundSpec);

                annotation { "Name" : "Rotation" }
                isAngle(definition.rotation, { (degree) : [-360, 0, 360] } as AngleBoundSpec);

                annotation { "Name" : "Reference direction", "Filter" : QueryFilterCompound.ALLOWS_DIRECTION, "MaxNumberOfPicks" : 1 }
                definition.referenceDirection is Query;
            }
            else if (definition.settingsMenu == SettingsMenu.OTHER)
            {
                annotation { "Name" : "Method", "UIHint" : [UIHint.SHOW_LABEL, UIHint.REMEMBER_PREVIOUS_VALUE] }
                definition.method is Method;

                annotation { "Name" : "Include inner sketch faces", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
                definition.includeInnerFaces is boolean;

                annotation { "Name" : "Color (Hex)", "UIHint" : [UIHint.REMEMBER_PREVIOUS_VALUE, UIHint.DISPLAY_SHORT], "Default" : true }
                definition.colorResults is boolean;

                annotation { "Name" : "Color", "UIHint" : [UIHint.REMEMBER_PREVIOUS_VALUE, UIHint.DISPLAY_SHORT], "Default" : "C0E431" }
                definition.color is string;

                annotation { "Name" : "Show hint", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE, "Default" : true }
                definition.showHelpNotice is boolean;
            }
        }

        cadsharpUrlPredicate(definition);
    }
    {
        var message = undefined;

        if (definition.showHelpNotice)
        {
            message = "HINT: This feature will remember your sketch selection if you store it in your Custom Frame Library.";
        }

        var toDelete = qNothing();

        const getSketchQueryMap = GetSketchQuery(context, id, definition);
        var sketchQuery = getSketchQueryMap.sketchQuery;
        toDelete = qUnion([toDelete, getSketchQueryMap.toDelete, sketchQuery]);

        var fromPlane = evOwnerSketchPlane(context, {
                "entity" : sketchQuery
            });

        var toPlane = XY_PLANE;

        // Size
        const bbox = evBox3d(context, {
                    "topology" : sketchQuery,
                    "cSys" : coordSystem(fromPlane),
                    "tight" : true
                });

        const boxWidth = bbox.maxCorner[0] - bbox.minCorner[0];
        const boxHeight = bbox.maxCorner[1] - bbox.minCorner[1];

        // Sketch starting origin
        var TRC = bbox.maxCorner; // Top-right corner
        var BLC = bbox.minCorner; // Bottom-left corner

        var TLC = vector(BLC[0], TRC[1], TRC[2]);
        var TCC = vector((BLC[0] + TRC[0]) / 2, TRC[1], TRC[2]);
        var MLC = vector(BLC[0], (BLC[1] + TRC[1]) / 2, TRC[2]);
        var Center = vector((BLC[0] + TRC[0]) / 2, (BLC[1] + TRC[1]) / 2, TRC[2]);
        var MRC = vector(TRC[0], (BLC[1] + TRC[1]) / 2, TRC[2]);
        var BCC = vector((BLC[0] + TRC[0]) / 2, BLC[1], TRC[2]);
        var BRC = vector(TRC[0], BLC[1], TRC[2]);

        var selectedSketchOrigin = switch (definition.sketchReference) {
                Position.TOP_LEFT : TLC,
                Position.TOP_CENTER : TCC,
                Position.TOP_RIGHT : TRC,
                Position.MID_LEFT : MLC,
                Position.CENTER : Center,
                Position.MID_RIGHT : MRC,
                Position.BOTTOM_LEFT : BLC,
                Position.BOTTOM_CENTER : BCC,
                Position.BOTTOM_RIGHT : BRC,
            };

        fromPlane = plane(selectedSketchOrigin, fromPlane.normal, fromPlane.x);

        if (definition.isFace && definition.autoSize)
        {
            const faceTanPlane = evFaceTangentPlane(context, {
                        "face" : definition.face,
                        "parameter" : vector(0.5, 0.5)
                    });

            const bboxFace = evBox3d(context, {
                        "topology" : definition.face,
                        "cSys" : coordSystem(faceTanPlane),
                        "tight" : true
                    });

            const boxWidthFace = bboxFace.maxCorner[0] - bboxFace.minCorner[0];
            const boxHeightFace = bboxFace.maxCorner[1] - bboxFace.minCorner[1];

            // Finds the smallest ratio
            definition.sizeOrientation = (boxWidthFace / boxWidth) < (boxHeightFace / boxHeight) ? SizeOrientation.WIDTH : SizeOrientation.HEIGHT;

            if (definition.sizeOrientation == SizeOrientation.WIDTH)
            {
                definition.size = boxWidthFace - (definition.offset * 2);
            }
            else
            {
                definition.size = boxHeightFace - (definition.offset * 2);
            }
        }

        var scaleFactor = 1;

        if (definition.sizeOrientation == SizeOrientation.WIDTH)
        {
            scaleFactor = definition.size / boxWidth;
        }
        else
        {
            scaleFactor = definition.size / boxHeight;
        }

        // Location
        if (definition.isFace)
        {
            toPlane = evFaceTangentPlane(context, {
                        "face" : definition.face,
                        "parameter" : vector(definition.facePositionX / 100, definition.facePositionY / 100)
                    });
        }
        else
        {
            toPlane = evMateConnector(context, {
                            "mateConnector" : definition.face
                        })->plane();
        }

        // Manipulator
        const manip1 = linearManipulator({
                    "base" : planeToWorld(toPlane, vector(0 * inch, definition.dy)),
                    "direction" : toPlane.x,
                    "offset" : definition.dx
                });

        const manip2 = linearManipulator({
                    "base" : planeToWorld(toPlane, vector(definition.dx, 0 * inch)),
                    "direction" : yAxis(toPlane),
                    "offset" : definition.dy
                });

        // Will not rotate with part. Using linear manip instead
        // const manip1 = triadManipulator({
        //             "base" : toPlane.origin,
        //             "offset" : vector(definition.x1, definition.y1, 0 * inch)
        //         });

        var manipulatorDef = {};
        manipulatorDef.manip1 = manip1;
        manipulatorDef.manip2 = manip2;
        addManipulators(context, id, manipulatorDef);

        // Offsets
        toPlane = plane(planeToWorld3D(toPlane) * vector(definition.dx, definition.dy, definition.dz), toPlane.normal, toPlane.x);

        // Move the toPlane away so that extrude upToFace and upToPart will work
        const tempToPlane = plane(toWorld(coordSystem(toPlane), vector(0 * inch, 0 * inch, 50 * inch)), toPlane.normal, toPlane.x);

        var normalPlane = toPlane;

        try silent
        {
            // Adjust toPlane to face normal
            const ray = evRaycast(context, {
                        "entities" : qOwnedByBody(qOwnerBody(definition.face), EntityType.FACE),
                        "ray" : line(tempToPlane.origin, -tempToPlane.normal),
                        "closest" : true
                    });

            const hitFace = ray[0].entity;

            normalPlane = evFaceTangentPlane(context, {
                        "face" : hitFace,
                        "parameter" : vector(ray[0].parameter[0], ray[0].parameter[1])
                    });
        }

        const normalPlaneOriginal = normalPlane;

        // Rotation
        var angle = definition.rotation;
        var normalPlaneAngle = angle;

        if (!isQueryEmpty(context, definition.referenceDirection))
        {
            angle = angle + angleBetween(toPlane.x, extractDirection(context, definition.referenceDirection), toPlane.normal);
            normalPlaneAngle = normalPlaneAngle + angleBetween(normalPlane.x, extractDirection(context, definition.referenceDirection), normalPlane.normal);
        }

        toPlane = rotationAround(line(toPlane.origin, toPlane.normal), angle) * toPlane;
        normalPlane = rotationAround(line(normalPlane.origin, normalPlane.normal), angle) * normalPlane;

        const debugProjectOrigin = toPlane.origin;
        const originalToPlane = toPlane;

        const transformScaleDebugNormal = scaleNonuniformly(scaleFactor, scaleFactor, 1, coordSystem(normalPlane));
        const transformLocationDebugNormal = transform(fromPlane, normalPlane);

        const transformScaleDebug = scaleNonuniformly(scaleFactor, scaleFactor, 1, coordSystem(toPlane));
        const transformLocationDebug = transform(fromPlane, toPlane);

        // Move the toPlane away so that extrude upToFace and upToPart will work
        toPlane = plane(toWorld(coordSystem(toPlane), vector(0 * inch, 0 * inch, 50 * inch)), toPlane.normal, toPlane.x);
        normalPlane = plane(toWorld(coordSystem(normalPlane), vector(0 * inch, 0 * inch, 50 * inch)), normalPlane.normal, normalPlane.x);

        const transformScale = scaleNonuniformly(scaleFactor, scaleFactor, 1, coordSystem(toPlane));
        const transformLocation = transform(fromPlane, toPlane);

        const transformScaleNormalPlane = scaleNonuniformly(scaleFactor, scaleFactor, 1, coordSystem(normalPlane));
        const transformLocationNormalPlane = transform(fromPlane, normalPlane);

        debug(context, boxWidth, DebugColor.RED);

        var patternId = "patternLogo";

        try
        {
            opPattern(context, id + patternId, {
                        "entities" : sketchQuery,
                        "transforms" : [transformScale * transformLocation, transformScaleDebug * transformLocationDebug, transformScaleNormalPlane * transformLocationNormalPlane, transformScaleDebugNormal * transformLocationDebugNormal, transformScaleDebug * transformLocationDebug],
                        "instanceNames" : ["sketchCopy", "sketchDebug", "normalPlaneSketch", "sketchDebugNormal", "simpleExtrude"]
                    });
        }
        catch
        {
            // Pattern would randomly not work if only some of the sketch faces were used.
            // This work around uses the faces from extracted surfaces instead.

            opExtractSurface(context, id + "extractFace1", {
                        "faces" : sketchQuery,
                        "tangentPropagation" : false,
                        "offset" : 0 * inch,
                        "useFacesAroundToTrimOffset" : false,
                    });

            sketchQuery = qCreatedBy(id + "extractFace1", EntityType.FACE);
            const extractedBodies = qCreatedBy(id + "extractFace1", EntityType.BODY);
            toDelete = qUnion([toDelete, extractedBodies]);

            patternId = "patternLogoTry2";

            opPattern(context, id + patternId, {
                        "entities" : sketchQuery,
                        "transforms" : [transformScale * transformLocation, transformScaleDebug * transformLocationDebug, transformScaleNormalPlane * transformLocationNormalPlane, transformScaleDebugNormal * transformLocationDebugNormal, transformScaleDebug * transformLocationDebug],
                        "instanceNames" : ["sketchCopy", "sketchDebug", "normalPlaneSketch", "sketchDebugNormal", "simpleExtrude"]
                    });
        }

        var patternedFaces = qPatternInstances(id + patternId, "sketchCopy", EntityType.FACE); // qCreatedBy(id + patternId, EntityType.FACE);
        const debugEdges = qPatternInstances(id + patternId, "sketchDebug", EntityType.EDGE);
        const normalPlanePatternedFaces = qPatternInstances(id + patternId, "normalPlaneSketch", EntityType.FACE);
        const debugEdgesNormal = qPatternInstances(id + patternId, "sketchDebugNormal", EntityType.EDGE);
        const simpleExtrudeFaces = qPatternInstances(id + patternId, "simpleExtrude", EntityType.FACE);
        toDelete = qUnion([toDelete, patternedFaces, debugEdges, normalPlanePatternedFaces, debugEdgesNormal, simpleExtrudeFaces]);

        var logoQuery;
        var toColor;

        if (!definition.isFace)
        {
            definition.face = definition.referenceFace;
        }

        if ((definition.scopeType == ScopeType.REMOVE || definition.scopeType == ScopeType.INTERSECT) && definition.depth >= 0 * inch)
        {
            definition.depth = -definition.depth;
        }
        else if (definition.scopeType == ScopeType.SPLIT || definition.scopeType == ScopeType.SURFACE)
        {
            definition.depth = .1 * inch;
        }

        var def = {
            "entities" : normalPlanePatternedFaces,
            "direction" : -normalPlane.normal,
            "endBound" : BoundingType.UP_TO_SURFACE,
            "endBoundEntity" : definition.face,
            "endTranslationalOffset" : 0 * inch,
            "startBound" : BoundingType.UP_TO_SURFACE,
            "startBoundEntity" : definition.face,
            "isStartBoundOpposite" : false,
            "startTranslationalOffset" : definition.depth
        };

        if (definition.depth < 0 * inch)
        {
            def = {
                    "entities" : normalPlanePatternedFaces,
                    "direction" : -normalPlane.normal,
                    "endBound" : BoundingType.UP_TO_SURFACE,
                    "endBoundEntity" : definition.face,
                    "endTranslationalOffset" : -definition.depth,
                    "startBound" : BoundingType.UP_TO_SURFACE,
                    "startBoundEntity" : definition.face,
                    "isStartBoundOpposite" : false,
                    "startTranslationalOffset" : 0 * inch
                };
        }

        var originalDef = def;

        if (definition.method == Method.EXTRUDE_UP_TO_PART)
        {
            def.endBoundEntity = qOwnerBody(definition.face);
            def.endBound = BoundingType.UP_TO_BODY;
            def.startBoundEntity = qOwnerBody(definition.face);
            def.startBound = BoundingType.UP_TO_BODY;
        }

        var failedExtrude = false;

        try
        {
            var extrudeId = "defaultExtrude";
            var usedNormalPlane = false;

            try silent
            {
                opExtrude(context, id + extrudeId, def);

                addDebugEntities(context, debugEdgesNormal, DebugColor.RED);
                addDebugLine(context, originalToPlane.origin, normalPlaneOriginal.origin, DebugColor.CYAN);
                addDebugPoint(context, normalPlaneOriginal.origin, DebugColor.CYAN);
                addDebugPoint(context, normalPlaneOriginal.origin, DebugColor.CYAN);
                addDebugPoint(context, normalPlaneOriginal.origin, DebugColor.CYAN);

                usedNormalPlane = true;
            }
            catch
            {
                try silent
                {
                    originalDef.entities = patternedFaces;
                    originalDef.direction = -toPlane.normal;
                    extrudeId = "secondaryExtrude";
                    opExtrude(context, id + extrudeId, originalDef);

                    // patternedFaces = normalPlanePatternedFaces;

                    if (definition.method == Method.EXTRUDE_UP_TO_PART)
                    {
                        message = "Projection \"Up to part\" failed: attempting \"Up to face\" instead. Make sure your logo is completely on the part.";
                    }

                    addDebugEntities(context, debugEdges, DebugColor.RED);
                    addDebugLine(context, originalToPlane.origin, debugProjectOrigin, DebugColor.CYAN);
                    addDebugPoint(context, originalToPlane.origin, DebugColor.CYAN);
                    addDebugPoint(context, originalToPlane.origin, DebugColor.CYAN);
                    addDebugPoint(context, originalToPlane.origin, DebugColor.CYAN);
                }
                catch
                {
                    def = {
                            "entities" : simpleExtrudeFaces,
                            "direction" : definition.depth < 0 ? -toPlane.normal : toPlane.normal,
                            "endBound" : BoundingType.BLIND,
                            "endDepth" : abs(definition.depth),
                        };

                    extrudeId = "simpleExtrude";
                    opExtrude(context, id + extrudeId, def);
                }

                addDebugEntities(context, debugEdges, DebugColor.RED);
                addDebugLine(context, originalToPlane.origin, debugProjectOrigin, DebugColor.CYAN);
                addDebugPoint(context, originalToPlane.origin, DebugColor.CYAN);
                addDebugPoint(context, originalToPlane.origin, DebugColor.CYAN);
                addDebugPoint(context, originalToPlane.origin, DebugColor.CYAN);

                usedNormalPlane = false;
            }

            logoQuery = qCreatedBy(id + extrudeId, EntityType.BODY);
            toColor = logoQuery;
            const extrudedFaces = qCreatedBy(id + extrudeId, EntityType.FACE);
            const capFaces = qCapEntity(id + extrudeId, CapType.END, EntityType.FACE);
            const capEdges = qCapEntity(id + extrudeId, CapType.END, EntityType.EDGE);

            if (definition.scopeType == ScopeType.ADD)
            {
                toColor = qCreatedBy(id + extrudeId, EntityType.FACE);
            }
            else if (definition.scopeType == ScopeType.SPLIT)
            {
                toDelete = qUnion([toDelete, logoQuery]);

                const facesToSplit = findFacesToSplit(context, id, definition, toPlane, patternedFaces);

                const tools = usedNormalPlane ? debugEdgesNormal : debugEdges;
                const direction = usedNormalPlane ? normalPlane.normal : toPlane.normal;

                opSplitFace(context, id + "splitFaceLogo", {
                            "faceTargets" : facesToSplit,
                            "edgeTools" : tools,
                            "projectionType" : "DIRECTION",
                            "direction" : direction,
                        });


                // Find matching split faces as qSplit wasn't working here
                const evalCapFaces = evaluateQuery(context, capFaces);
                toColor = qNothing();

                // Not working well for coloring split faces
                // for (var i = 0; i < size(evalCapFaces); i += 1)
                // {
                //     const thisFace = evalCapFaces[i];
                //     const reference = evApproximateCentroid(context, {
                //                 "entities" : thisFace
                //             });

                //     const closestFace = qClosestTo(qOwnedByBody(qOwnerBody(definition.face), EntityType.FACE), reference);
                //     toColor = qUnion([toColor, closestFace]);
                // }
            }
            else if (definition.scopeType == ScopeType.SURFACE)
            {
                toDelete = qUnion([toDelete, logoQuery]);

                opExtractSurface(context, id + "extractFace", {
                            "faces" : capFaces,
                            "tangentPropagation" : true,
                            "offset" : -0.001 * inch,
                            "useFacesAroundToTrimOffset" : false,
                        // "redundancyType" : ExtractSurfaceRedundancyTypeoptional
                        });

                logoQuery = qCreatedBy(id + "extractFace", EntityType.BODY);
                toColor = logoQuery;
            }

            // Boolean
            if (definition.scopeType == ScopeType.ADD || definition.scopeType == ScopeType.REMOVE || definition.scopeType == ScopeType.INTERSECT)
            {
                var booleanDef;
                var target = qOwnerBody(definition.face);

                if (definition.scopeType == ScopeType.ADD)
                {
                    booleanDef = {
                            "tools" : qUnion([target, logoQuery]),
                            "operationType" : BooleanOperationType.UNION
                        };
                }
                else if (definition.scopeType == ScopeType.REMOVE)
                {
                    booleanDef = {
                            "tools" : logoQuery,
                            "targets" : target,
                            "operationType" : BooleanOperationType.SUBTRACTION
                        };
                }
                else if (definition.scopeType == ScopeType.INTERSECT)
                {
                    booleanDef = {
                            "tools" : target,
                            "targets" : logoQuery,
                            "operationType" : BooleanOperationType.SUBTRACT_COMPLEMENT,
                            "keepTools" : true
                        };
                }

                opBoolean(context, id + "booleanLogo", booleanDef);

                if (definition.scopeType == ScopeType.INTERSECT)
                {
                    opBoolean(context, id + "booleanSubtractIntersect", {
                                "tools" : logoQuery,
                                "targets" : target,
                                "operationType" : BooleanOperationType.SUBTRACTION,
                                "keepTools" : true
                            });
                }

                if (definition.scopeType == ScopeType.REMOVE)
                {
                    toColor = qCreatedBy(id + "booleanLogo", EntityType.FACE);
                }
            }

            if (definition.scopeType == ScopeType.NEW || definition.scopeType == ScopeType.SURFACE)
            {
                opCreateCompositePart(context, id + "compositePartLogo", {
                            "bodies" : logoQuery,
                            "closed" : true
                        });

                logoQuery = qCreatedBy(id + "compositePartLogo", EntityType.BODY);
                toColor = logoQuery;

                setProperty(context, {
                            "entities" : logoQuery,
                            "propertyType" : PropertyType.NAME,
                            "value" : "Logo"
                        });
            }

            // Color
            var thisColor = color(1, 1, 1);

            if (definition.colorResults)
            {
                const rgb = hexToRGB(definition.color);

                thisColor = color(rgb[0] / 255, rgb[1] / 255, rgb[2] / 255);
            }

            try silent
            {
                if (definition.colorResults)
                {
                    setProperty(context, {
                                "entities" : toColor,
                                "propertyType" : PropertyType.APPEARANCE,
                                "value" : thisColor
                            });
                }
                else
                {
                    if (definition.scopeType == ScopeType.NEW || definition.scopeType == ScopeType.SURFACE)
                    {
                        setProperty(context, {
                                    "entities" : toColor,
                                    "propertyType" : PropertyType.APPEARANCE,
                                    "value" : color(1, 1, 1)
                                });
                    }
                }
            }
        }
        catch
        {
            message = "Projection failed: Make sure your logo is completely on the part.";
            failedExtrude = true;

            addDebugEntities(context, debugEdgesNormal, DebugColor.RED);
            addDebugLine(context, originalToPlane.origin, normalPlaneOriginal.origin, DebugColor.CYAN);
            addDebugPoint(context, normalPlaneOriginal.origin, DebugColor.CYAN);
            addDebugPoint(context, normalPlaneOriginal.origin, DebugColor.CYAN);
            addDebugPoint(context, normalPlaneOriginal.origin, DebugColor.CYAN);
        }

        // Clean up
        opDeleteBodies(context, id + "deleteBodies1", {
                    "entities" : toDelete
                });

        if (!isUndefinedOrEmptyString(message))
        {
            reportFeatureInfo(context, id, message);
        }
    });

function findFacesToSplit(context, id, definition, toPlane, patternedFaces)
{
    const checkLocations = [
            vector(0.0, 0.0),
            vector(0.0, 0.5),
            vector(0.0, 1.0),
            vector(0.5, 0.0),
            vector(0.5, 0.5),
            vector(0.5, 1.0),
            vector(1.0, 0.0),
            vector(1.0, 0.5),
            vector(1.0, 1.0)
        ];

    var toReturn = qNothing();
    const evFaces = evaluateQuery(context, patternedFaces);

    for (var i = 0; i < size(checkLocations); i += 1)
    {
        for (var k = 0; k < size(evFaces); k += 1)
        {
            const rayOrigin = evFaceTangentPlane(context, {
                            "face" : evFaces[k],
                            "parameter" : checkLocations[i]
                        }).origin;

            const ray = evRaycast(context, {
                        "entities" : qOwnedByBody(qOwnerBody(definition.face), EntityType.FACE),
                        "ray" : line(rayOrigin, -toPlane.normal),
                        "closest" : true
                    });

            const hitFace = ray[0].entity;

            toReturn = qUnion([toReturn, hitFace]);
        }
    }

    return toReturn;
}

// Borrowed from Neil's frame feature, Thanks Neil!
function GetSketchQuery(context is Context, id is Id, definition is map)
{
    var toReturn = {};
    toReturn.toDelete = qNothing();
    toReturn.sketchQuery = qNothing();

    const profileId = id + "sketch";
    const instantiator = newInstantiator(profileId);

    // Selections from element libraries pass in qEverything(EntityType.BODY), which needs to be filtered
    // Sketch selections outside element libraries should be unaffected.
    verify(definition.profileSketch.partQuery != undefined, ErrorStringEnum.FRAME_SELECT_PROFILE, {
                "faultyParameters" : ["profileSketch"] });
    definition.profileSketch.partQuery = definition.profileSketch.partQuery->qSketchFilter(SketchObject.YES);

    try silent
    {
        addInstance(instantiator, definition.profileSketch, {});
    }
    catch
    {
        throw regenError(ErrorStringEnum.FRAME_SELECT_PROFILE, ["profileSketch"]);
    }
    instantiate(context, instantiator);

    toReturn.sketchQuery = qCreatedBy(profileId, EntityType.FACE);
    toReturn.toDelete = qUnion([qCreatedBy(profileId, EntityType.VERTEX), qCreatedBy(profileId, EntityType.EDGE)]);

    if (!definition.includeInnerFaces)
    {
        // Get all one-sided edges of the input faces
        const oneSidedEdges = qEdgeTopologyFilter(qEdgeAdjacent(toReturn.sketchQuery, EntityType.EDGE), EdgeTopology.ONE_SIDED);

        // Get all the faces touching the one sided edges
        toReturn.sketchQuery = qAdjacent(oneSidedEdges, AdjacencyType.EDGE, EntityType.FACE);
    }

    return toReturn;
}

// Function to convert hex to RGB using splitIntoCharacters
function hexToRGB(hex)
{
    // Remove the '#' symbol if present
    // var hex = hexColor.replace("#", "");

    // Split the hex color string into an array of characters
    const hexArray = splitIntoCharacters(hex);

    // Ensure that the array has 6 characters (RRGGBB)
    if (size(hexArray) != 6)
    {
        // Handle invalid input here
        return [0, 0, 0]; // Default to black or another appropriate value
    }

    // Define a map to convert hex characters to decimal values
    const hexMap = {
            '0' : 0, '1' : 1, '2' : 2, '3' : 3, '4' : 4, '5' : 5, '6' : 6, '7' : 7,
            '8' : 8, '9' : 9, 'A' : 10, 'B' : 11, 'C' : 12, 'D' : 13, 'E' : 14, 'F' : 15,
            'a' : 10, 'b' : 11, 'c' : 12, 'd' : 13, 'e' : 14, 'f' : 15
        };

    // Convert each character to its corresponding decimal value
    const r = 16 * hexMap[hexArray[0]] + hexMap[hexArray[1]];
    const g = 16 * hexMap[hexArray[2]] + hexMap[hexArray[3]];
    const b = 16 * hexMap[hexArray[4]] + hexMap[hexArray[5]];

    return [r, g, b];
}

export function ManipulatorChange(context is Context, definition is map, newManipulators is map) returns map
{
    if (newManipulators["manip1"] is map)
        definition.dx = newManipulators["manip1"].offset;

    if (newManipulators["manip2"] is map)
        definition.dy = newManipulators["manip2"].offset;

    return definition;
}

export function EditLogic(context is Context, id is Id, oldDefinition is map, definition is map, isCreating is boolean, specifiedParameters is map) returns map
{
    cadsharpUrlFunctionForPreExistingEditLogic(oldDefinition, definition);

    if (oldDefinition.face != definition.face)
    {
        definition.dx = 0 * inch;
        definition.dy = 0 * inch;
        definition.dy = 0 * inch;

        if (!isQueryEmpty(context, qEntityFilter(definition.face, EntityType.FACE)) || isQueryEmpty(context, definition.face))
        {
            definition.isFace = true;
        }
        else
        {
            definition.isFace = false;
        }
    }

    return definition;
}

