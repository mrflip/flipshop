
//_______________________________________________________________________________________________________________________________________________
//
// This FeatureScript is owned by Michael Pascoe and is distributed by CADSharp LLC.
// You may not redistribute it for commercial purposes without the permission of said owner and CADSharp LLC. Copyright (c) 2023 Michael Pascoe.
//_______________________________________________________________________________________________________________________________________________


FeatureScript 1378;
import(path : "onshape/std/geometry.fs", version : "1378.0");
icon::import(path : "e99d4ff6bc37dba049095aa5", version : "758ffe98ffe44ac7d5198c8c");
export import(path : "cbeb3dcf671e00785597bd76/144bf6a7fdc989e9e28ce5ea/a75ab01def146a42f55baa7f", version : "dc78e9b85c9f16ea9e131d3f");

annotation {
    "Feature Type Name" : "Pattern & Sweep",
    "Icon" : icon::BLOB_DATA,
    "Feature Type Description" : "<b> Summary </b> <br> Sweep along multiple paths at one time",
    "Description Image" : cadsharpLogo::BLOB_DATA,
    "Editing Logic Function" : "cadsharpUrlEditLogic"
    }
export const patternSweep = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        //Required to create a horizontal tab menu.
        annotation { "Name" : "Creation method", "UIHint" : "HORIZONTAL_ENUM" }
        definition.creationMethod is creationMethod;

        annotation { "Name" : "Mode", "UIHint" : UIHint.HORIZONTAL_ENUM }
        definition.mode is Mode;

        annotation { "Name" : "Same studio profile", "Default" : true }
        definition.sameStudio is boolean;
        if (definition.sameStudio == true)
        {
            annotation { "Name" : "Profile to sweep", "Filter" : EntityType.FACE }
            definition.profile is Query;

            //When selecting mate connectors, I prefer having the default option to pick an existing mate connector, then a button to make a create a connector. See below:
            annotation { "Name" : "Profile origin mate", "Filter" : BodyType.MATE_CONNECTOR || BodyType.MATE_CONNECTOR, "MaxNumberOfPicks" : 1 }
            definition.profileOrigin is Query;
        }
        else
        {
            annotation { "Name" : "Profile to sweep" }
            definition.otherStudioProfile is PartStudioData;

            annotation { "Name" : "Profile Origin", "UIHint" : "HORIZONTAL_ENUM" }
            definition.otherStudioOrigin is otherStudioOrigin;

            if (definition.otherStudioOrigin == otherStudioOrigin.PART)
            {
                annotation { "Name" : "Origin Placement" }
                definition.otherOriginPlacement is otherOriginPlacement;
            }

            annotation { "Name" : "Offset width" }
            isLength(definition.offsetWidth, { (inch) : [-1e5, 0, 1e5] } as LengthBoundSpec);

            annotation { "Name" : "Offset depth" }
            isLength(definition.offsetDepth, { (inch) : [-1e5, 0, 1e5] } as LengthBoundSpec);

        }

        //Flip & rotate buttons reference: https://cad.onshape.com/documents/12312312345abcabcabcdeff/w/a855e4161c814f2e9ab3698a/e/81869edac6224be29410b2a3
        //to have the flip arrow on the next row use this: "UIHint" : [UIHint.PRIMARY_AXIS, UIHint.FIRST_IN_ROW] }

        annotation { "Name" : "Flip primary axis", "UIHint" : [UIHint.PRIMARY_AXIS, UIHint.FIRST_IN_ROW] }
        definition.oppositeDirectionMateAxis is boolean;

        annotation { "Name" : "Reorient secondary axis", "UIHint" : UIHint.MATE_CONNECTOR_AXIS_TYPE, "Default" : AxisType.PLUS_X } //The default enum selection is PLUS_X
        definition.secondaryAxisType is AxisType;

        if (definition.mode == Mode.AROUND_FACES)
        {
            annotation { "Name" : "Path faces", "Filter" : EntityType.FACE }
            definition.face is Query;

            annotation { "Name" : "Exclude edges" }
            definition.excludeEdges is boolean;

            if (definition.excludeEdges == true)
            {
                annotation { "Name" : "Edges of path to exclude", "Filter" : EntityType.EDGE }
                definition.excludedEdges is Query;
            }
        }
        else
        {
            annotation { "Name" : "Sweeps", "Item name" : "Sweep", "Collapsed By Default" : true }
            definition.groups is array;
            for (var group in definition.groups)
            {
                annotation { "Name" : "Start origin mate", "Filter" : BodyType.MATE_CONNECTOR || BodyType.MATE_CONNECTOR, "MaxNumberOfPicks" : 1 }
                group.startLocation is Query;

                annotation { "Name" : "Path curves", "Filter" : EntityType.EDGE }
                group.curves is Query;
            }
        }

        annotation { "Name" : "Delete paths" }
        definition.deletePathTools is boolean;

        if (definition.creationMethod == creationMethod.ADD || definition.creationMethod == creationMethod.REMOVE)
        {
            annotation { "Name" : "Merge scope", "Filter" : EntityType.BODY }
            definition.mergeParts is Query;
        }

        if (definition.creationMethod == creationMethod.REMOVE)
        {
            annotation { "Name" : "Keep swept tools" }
            definition.keepTools is boolean;

            annotation { "Name" : "Swap tools" }
            definition.reverseTools is boolean;
        }

        cadsharpUrlPredicate(definition);
    }
    {

        //___________________________________________________________________________________________________________
        //
        //                                              Prepare other studio profiles
        //___________________________________________________________________________________________________________

        var otherStudioProfile;
        var otherStudioProfileOrigin = WORLD_COORD_SYSTEM;

        if (definition.sameStudio == false)
        {
            const instantiator = newInstantiator(id + "inst", {});
            var queries = [];

            var q = addInstance(instantiator, definition.otherStudioProfile, { "configuration" : {}, "transform" : transform(vector(0, 0, 0) * inch) });
            queries = append(queries, q);

            queries = append(queries, q);
            q = append(queries, q);
            instantiate(context, instantiator);

            otherStudioProfile = qOwnedByBody(q[0], EntityType.FACE);

            if (definition.otherStudioOrigin == otherStudioOrigin.PART)
            {
                const evalPlane = evPlane(context, {
                            "face" : otherStudioProfile
                        });
                const localCsys = planeToCSys(evalPlane);
                const profileBox = evBox3d(context, {
                            "topology" : otherStudioProfile,
                            "tight" : true,
                            "cSys" : localCsys
                        });

                var x;
                var y;

                if (definition.otherOriginPlacement == otherOriginPlacement.BOTTOM_LEFT)
                {
                    x = profileBox.minCorner[0];
                    y = profileBox.minCorner[1];
                }
                if (definition.otherOriginPlacement == otherOriginPlacement.TOP_LEFT)
                {
                    x = profileBox.minCorner[0];
                    y = profileBox.maxCorner[1];
                }
                if (definition.otherOriginPlacement == otherOriginPlacement.TOP_RIGHT)
                {
                    x = profileBox.maxCorner[0];
                    y = profileBox.maxCorner[1];
                }
                if (definition.otherOriginPlacement == otherOriginPlacement.BOTTOM_RIGHT)
                {
                    x = profileBox.maxCorner[0];
                    y = profileBox.minCorner[1];
                }

                //Reference Tim: https://cad.onshape.com/documents/f5cd9f4b2ec8e9eea7266f1e/v/7b36726274b62ee2f6b6f979/e/4626b80fb148952bd0b75c92

                var profileOrigin = toWorld(localCsys, vector(x, y, 0 * inch));
                const profileOriginPlane = plane(profileOrigin, evalPlane.normal, evalPlane.x);
                otherStudioProfileOrigin = coordSystem(profileOriginPlane);

            }

            opTransform(context, id + "transform1", {
                        "bodies" : q[0],
                        "transform" : transform(vector(definition.offsetWidth, definition.offsetDepth, 0 * inch))
                    });

        }


        //___________________________________________________________________________________________________________
        //
        //                                                  Flip & rotate buttons
        //___________________________________________________________________________________________________________

        //Flip & rotate buttons reference: https://cad.onshape.com/documents/12312312345abcabcabcdeff/w/a855e4161c814f2e9ab3698a/e/81869edac6224be29410b2a3

        const c2 = definition.sameStudio == true ? evMateConnector(context, { "mateConnector" : definition.profileOrigin }) : otherStudioProfileOrigin;
        var xAxis = c2.xAxis;
        var zAxis = definition.oppositeDirectionMateAxis ? -c2.zAxis : c2.zAxis;

        if (definition.secondaryAxisType == AxisType.PLUS_Y)
        {
            xAxis = cross(zAxis, xAxis);
        }
        else if (definition.secondaryAxisType == AxisType.MINUS_X)
        {
            xAxis = -xAxis;
        }
        else if (definition.secondaryAxisType == AxisType.MINUS_Y)
        {
            xAxis = -cross(zAxis, xAxis);
        }

        const originPlane = plane(c2.origin, zAxis, xAxis);

        //___________________________________________________________________________________________________________
        //
        //                                          Evaluate paths & prepare for transforms
        //___________________________________________________________________________________________________________

        var pathFaces = evaluateQuery(context, definition.face);

        //Required for opPattern.
        var queryPaths = [];
        var transforms = [];
        var instanceNames = [];

        if (definition.mode == Mode.AROUND_FACES)
        {
            var count = -1;
            for (var edges in pathFaces)
            {
                count = count + 1;

                var loopedEdges;
                var sketchEntityLogic = evaluateQuery(context, qSketchFilter(pathFaces[count], SketchObject.YES));
                //debug(context, size(sketchEntityLogic), DebugColor.RED);

                if (size(sketchEntityLogic) == 1)
                {
                    var trackingQs = [];
                    for (var edge in evaluateQuery(context, definition.excludedEdges))
                    {
                        const edgeTracking = startTracking(context, { "subquery" : edge, "lastOperationId" : lastModifyingOperationId(context, edge) });
                        trackingQs = append(trackingQs, edgeTracking);
                    }
                    loopedEdges = qSubtraction(qAdjacent(pathFaces[count], AdjacencyType.EDGE, EntityType.EDGE), qUnion(trackingQs));
                }
                else
                {
                    loopedEdges =
                        qSubtraction(
                        qAdjacent(pathFaces[count], AdjacencyType.EDGE, EntityType.EDGE),
                        definition.excludedEdges
                        );
                }

                var evalLoopedEdges = evaluateQuery(context, loopedEdges);
                var tangentPlane = evFaceTangentPlaneAtEdge(context, {
                        "edge" : evalLoopedEdges[0],
                        "face" : pathFaces[count],
                        "parameter" : .5,
                        "usingFaceOrientation" : true
                    });
                var edgeTangent = evEdgeTangentLine(context, {
                        "edge" : evalLoopedEdges[0],
                        "parameter" : .5,
                        "face" : pathFaces[count]
                    });
                var xDirection = -cross(edgeTangent.direction, tangentPlane.normal);
                var normalTangentPlane = plane(tangentPlane.origin, edgeTangent.direction, xDirection);

                //Required for opPattern.
                queryPaths = append(queryPaths, loopedEdges);
                transforms = append(transforms, transform(originPlane, normalTangentPlane));
                instanceNames = append(instanceNames, "face" ~ count);
            }
        }
        else
        {
            try
            {
                for (var i = 0; i < size(definition.groups); i += 1)
                {
                    const mate = evMateConnector(context, {
                                "mateConnector" : definition.groups[i].startLocation
                            });
                    const toPlane = plane(mate);
                    const curves = definition.groups[i].curves;

                    //Required for opPattern.
                    queryPaths = append(queryPaths, curves);
                    transforms = append(transforms, transform(originPlane, toPlane));
                    instanceNames = append(instanceNames, "face" ~ i);
                }
            }
        }

        //___________________________________________________________________________________________________________
        //
        //                                                  Prepare profiles
        //___________________________________________________________________________________________________________

        const profileDefinition = definition.sameStudio == true ? definition.profile : otherStudioProfile;
        var evalProfile = evaluateQuery(context, profileDefinition);
        var profileCount = -1;
        var surfaceProfiles;
        var surfacesToDelete;

        for (var profiles in evalProfile)
        {
            profileCount = profileCount + 1;
            var profileEdges = qLoopEdges(profiles);

            opFillSurface(context, id + "opFillSurface1" + profileCount, {
                        "edgesG0" : profileEdges,
                    });
            var currentProfile = qCreatedBy(id + "opFillSurface1" + profileCount, EntityType.FACE);
            var currentSurface = qCreatedBy(id + "opFillSurface1" + profileCount, EntityType.BODY);

            surfaceProfiles = profileCount == 0 ? currentProfile : qUnion([surfaceProfiles, currentProfile]);
            surfacesToDelete = profileCount == 0 ? currentSurface : qUnion([surfacesToDelete, currentSurface]);
        }

        //___________________________________________________________________________________________________________
        //
        //                                  Patterns profile to each selected path
        //___________________________________________________________________________________________________________

        opPattern(context, id + "pattern1", {
                    "entities" : surfaceProfiles,
                    "transforms" : transforms,
                    "instanceNames" : instanceNames
                });

        //___________________________________________________________________________________________________________
        //
        //                                            Sweeps profiles
        //___________________________________________________________________________________________________________

        var queryParts;
        var count2 = -1;

        for (var edges in transforms)
        {
            count2 = count2 + 1;

            //Finds each patterned instance. instanceNames is an array.
            var profileFace = qPatternInstances(id + "pattern1", instanceNames[count2], EntityType.FACE);

            try
            {
                opSweep(context, id + "sweep1" + count2, {
                            "profiles" : profileFace,
                            "path" : queryPaths[count2]
                        });

                var sweptParts = qCreatedBy(id + "sweep1" + count2, EntityType.BODY);
                queryParts = count2 == 0 ? sweptParts : qUnion([queryParts, sweptParts]);
            }

            var patternedSurfaces = qPatternInstances(id + "pattern1", instanceNames[count2], EntityType.FACE);
            surfacesToDelete = qUnion([surfacesToDelete, patternedSurfaces]);
        }

        //___________________________________________________________________________________________________________
        //
        //                                               Creation methods
        //___________________________________________________________________________________________________________

        if (definition.creationMethod == creationMethod.ADD)
        {
            opBoolean(context, id, {
                        "tools" : qUnion([queryParts, definition.mergeParts]),
                        "operationType" : BooleanOperationType.UNION,
                    });
        }

        if (definition.creationMethod == creationMethod.REMOVE)
        {
            var tools = definition.reverseTools ? definition.mergeParts : queryParts;
            var targets = definition.reverseTools ? queryParts : definition.mergeParts;
            opBoolean(context, id, {
                        "operationType" : BooleanOperationType.SUBTRACTION,
                        "tools" : tools,
                        "targets" : targets,
                        "keepTools" : definition.keepTools ? true : false
                    });
        }

        //___________________________________________________________________________________________________________
        //
        //                                                  Color parts
        //___________________________________________________________________________________________________________

        if (definition.creationMethod == creationMethod.NEW)
        {
            setProperty(context, {
                        "entities" : queryParts,
                        "propertyType" : PropertyType.APPEARANCE,
                        "value" : color(1, 1, 1)
                    });
        }

        //___________________________________________________________________________________________________________
        //
        //                                                  Delete parts
        //___________________________________________________________________________________________________________

        var partsToDelete = surfacesToDelete;

        if (definition.sameStudio == false)
        {
            partsToDelete = qUnion([partsToDelete, qOwnerBody(profileDefinition)]);
        }

        if (definition.deletePathTools == true)
        {
            var pathTools = qOwnerBody(definition.face);
            partsToDelete = qUnion([partsToDelete, pathTools]);
        }

        opDeleteBodies(context, id + "deleteBodies1", {
                    "entities" : partsToDelete
                });
    });

export enum otherStudioOrigin
{
    annotation { "Name" : "Part origin" }
    PART,

    annotation { "Name" : "World origin" }
    WORLD,
}

export enum otherOriginPlacement
{
    annotation { "Name" : "Bottom Left" }
    BOTTOM_LEFT,

    annotation { "Name" : "Top Left" }
    TOP_LEFT,

    annotation { "Name" : "Top Right" }
    TOP_RIGHT,

    annotation { "Name" : "Bottom Right" }
    BOTTOM_RIGHT,
}

//Required to create a horizontal tab menu.
export enum creationMethod
{
    annotation { "Name" : "New" }
    NEW,

    annotation { "Name" : "Add" }
    ADD,

    annotation { "Name" : "Remove" }
    REMOVE,

    // annotation { "Name" : "Intersect" }
    // INTERSECT,
}

export enum Mode
{
    annotation { "Name" : "Around faces" }
    AROUND_FACES,

    annotation { "Name" : "Along curves" }
    ALONG_EDGE
}

//Required for flip & rotate buttons
export enum AxisType
{
    annotation { "Name" : "+X" }
    PLUS_X,
    annotation { "Name" : "+Y" }
    PLUS_Y,
    annotation { "Name" : "-X" }
    MINUS_X,
    annotation { "Name" : "-Y" }
    MINUS_Y
}
