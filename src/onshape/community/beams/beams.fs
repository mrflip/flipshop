FeatureScript 1096;
import(path : "onshape/std/common.fs", version : "1096.0");
export import(path : "c89479836a06762d1552fe0d", version : "436fb3acaca8ef633f98e84d"); // beams/beams-beam_profiles.fs
icon::import(path : "9dfb700e13d016e813711ec6", version : "f27905cd44131c0083a4dfcf"); // beams/beams_icon.svg

const offsetTolerance = 0.02 * millimeter;

annotation { "Feature Type Name" : "Beam", "Manipulator Change Function" : "beamManipulators", "Filter Selector" : "allparts", "Icon" : icon::BLOB_DATA }
export const Beam = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Lines or arcs", "Filter" : EntityType.EDGE && (GeometryType.LINE || GeometryType.ARC) }
        definition.edges is Query;

        annotation { "Name" : "Profile type", "UIHint" : "HORIZONTAL_ENUM" }
        definition.profileType is ProfileType;

        if (definition.profileType == ProfileType.STANDARD)
        {
            annotation { "Name" : "Standard", "Lookup Table" : beamProfileTable, "UIHint" : "REMEMBER_PREVIOUS_VALUE" }
            definition.profile is LookupTablePath;
        }
        else if (definition.profileType == ProfileType.CUSTOM)
        {
            annotation { "Name" : "Profile sketch", "Filter" : PartStudioItemType.SKETCH, "MaxNumberOfPicks" : 1 }
            definition.customProfileSketch is PartStudioData;

            annotation { "Name" : "Enable coping", "Default" : false }
            definition.coping is boolean;
        }

        annotation { "Name" : "Angle" }
        isAngle(definition.angle, ANGLE_360_ZERO_DEFAULT_BOUNDS);

        annotation { "Name" : "Mirror about Y axis", "UIHint" : "OPPOSITE_DIRECTION" }
        definition.flipYAxis is boolean;

        annotation { "Name" : "X offset", "UIHint" : "ALWAYS_HIDDEN" }
        isLength(definition.x, ZERO_DEFAULT_LENGTH_BOUNDS);

        annotation { "Name" : "Y offset", "UIHint" : "ALWAYS_HIDDEN" }
        isLength(definition.y, ZERO_DEFAULT_LENGTH_BOUNDS);

        annotation { "Group Name" : "Joints", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Miter all", "Default" : false }
            definition.miterAll is boolean;

            if (!definition.miterAll)
            {
                annotation { "Name" : "Vertices for miter joints", "Filter" : EntityType.VERTEX }
                definition.miterJoints is Query;
            }

            annotation { "Name" : "Vertices for butt joints", "Filter" : EntityType.VERTEX }
            definition.buttJoints is Query;

            annotation { "Name" : "Flipped", "Filter" : EntityType.VERTEX, "UIHint" : "ALWAYS_HIDDEN" }
            definition.flipJoints is Query;

            if (definition.profileType == ProfileType.STANDARD || (definition.profileType == ProfileType.CUSTOM && definition.coping))
            {
                annotation { "Name" : "Cope", "Default" : false }
                definition.coped is boolean;
            }

            annotation { "Name" : "Merge", "Default" : false }
            definition.merge is boolean;
        }

        annotation { "Name" : "Trim", "Default" : false }
        definition.trim is boolean;

        if (definition.trim)
        {
            annotation { "Group Name" : "Trimming", "Collapsed By Default" : false, "Driving Parameter" : "trim" }
            {
                annotation { "Name" : "Planar faces to trim to", "Filter" : EntityType.FACE && GeometryType.PLANE }
                definition.trimFaces is Query;

                annotation { "Name" : "Parts to trim to", "Filter" : EntityType.BODY }
                definition.trimBeams is Query;
            }
        }

        annotation { "Name" : "Name", "Default" : false, "UIHint" : "REMEMBER_PREVIOUS_VALUE" }
        definition.nameBeams is boolean;

        if (definition.nameBeams)
        {
            annotation { "Group Name" : "Name", "Collapsed By Default" : false, "Driving Parameter" : "nameBeams" }
            {
                if (definition.profileType == ProfileType.STANDARD)
                {
                    annotation { "Name" : "Standard", "Default" : false, "UIHint" : "REMEMBER_PREVIOUS_VALUE" }
                    definition.nameStandard is boolean;

                    annotation { "Name" : "Profile", "Default" : true, "UIHint" : "REMEMBER_PREVIOUS_VALUE" }
                    definition.nameProfile is boolean;

                    annotation { "Name" : "Size", "Default" : true, "UIHint" : "REMEMBER_PREVIOUS_VALUE" }
                    definition.nameSize is boolean;
                }
                else if (definition.profileType == ProfileType.CUSTOM)
                {
                    annotation { "Name" : "Name", "Default" : "Custom Profile" }
                    definition.customName is string;
                }

                annotation { "Name" : "Length", "Default" : true, "UIHint" : ["REMEMBER_PREVIOUS_VALUE", "DISPLAY_SHORT"] }
                definition.nameLength is boolean;

                if (definition.nameLength)
                {
                    annotation { "Name" : "Units", "Default" : CUTLENGTH_UNITS.IN, "UIHint" : ["REMEMBER_PREVIOUS_VALUE", "DISPLAY_SHORT"] }
                    definition.nameUnits is CUTLENGTH_UNITS;
                }
            }
        }
    }
    {
        if (size(evaluateQuery(context, definition.edges)) == 0)
        {
            throw regenError("No lines or arcs selected", ["edges"]);
        }

        // Get the selected profile
        const profile = getProfile(context, id, beamProfileTable, definition);

        // Sort selected edges into a map with additional info
        const edgeLoops = edgeSort(context, definition);

        var manipulators = {};
        var x;
        var y;

        for (var loop = 0; loop < size(edgeLoops); loop += 1)
        {
            const edgeLoop = edgeLoops[loop].edges;
            const edgePlane = getPlane(context, definition, edgeLoop);

            for (var i = 0; i < size(edgeLoop); i += 1)
            {
                const beam = edgeLoop[i].edge;
                const edge = qNthElement(definition.edges, beam);
                const edgeVertex = edgeLoop[i].vertex[edgeLoop[i].index];
                var profilePlane;

                try silent
                {
                    profilePlane = plane(edgeVertex.origin, edgeVertex.direction * (edgeLoop[i].index == 0 ? -1 : 1), cross(edgeVertex.direction, edgePlane.normal * (edgeLoop[i].index == 0 ? 1 : -1)));
                }
                catch
                {
                    throw regenError("Edge loops must be planar", ["edges"]);
                }

                // Round manipulator input to the nearest half profile size increment
                x = round(definition.x / profile.xInc) * profile.xInc;
                y = round(definition.y / profile.yInc) * profile.yInc;

                if (x > profile.xMax)
                    x = profile.xMax;
                if (x < profile.xMin)
                    x = profile.xMin;
                if (y > profile.yMax)
                    y = profile.yMax;
                if (y < profile.yMin)
                    y = profile.yMin;

                // Move profile from world origin to edge vertex, then relative to edge vertex, then mirror if required, then rotate around edge
                var moveProfile = transform(evPlane(context, { "face" : qCreatedBy(makeId("Top"), EntityType.FACE) }), profilePlane);
                moveProfile = transform(edgeVertex.origin - toWorld(planeToCSys(profilePlane), vector(x, y, 0 * meter))) * moveProfile;
                if (definition.flipYAxis)
                    moveProfile = mirrorAcross(plane(edgeVertex.origin, cross(edgeVertex.direction, edgePlane.normal * (edgeLoop[i].index == 0 ? 1 : -1)))) * moveProfile;
                moveProfile = rotationAround(line(edgeVertex.origin, edgeVertex.direction * (edgeLoop[i].index == 0 ? -1 : 1)), definition.angle) * moveProfile;

                // Make a copy of the selected profile to be transformed onto each selected edge
                opPattern(context, id + beam + "profile", {
                            "entities" : profile.face,
                            "transforms" : [identityTransform()],
                            "instanceNames" : ["1"]
                        });

                // Move profile from origin to start vertex of edge
                opTransform(context, id + beam + "transform", {
                            "bodies" : qCreatedBy(id + beam + "profile", EntityType.BODY),
                            "transform" : moveProfile });

                opSweep(context, id + beam + "sweep", {
                            "profiles" : qClosestTo(qCreatedBy(id + beam + "profile", EntityType.FACE), moveProfile * evBox3d(context, { "topology" : profile.face }).minCorner),
                            "path" : edge });

                // Remove profile sketch
                opDeleteBodies(context, id + beam + "deleteProfile", {
                            "entities" : qCreatedBy(id + beam + "profile", EntityType.BODY) });

                if (definition.trim)
                {
                    trimBeam(context, definition, id + beam, profile.yInc);
                }

                if (i == 0 && loop == 0) // Add profile manipulators to the first selected edge and loop
                {
                    const manipulatorVertex = evEdgeTangentLine(context, { "edge" : edge, "parameter" : 0.5 });
                    const manipulatorPlane = plane(manipulatorVertex.origin, manipulatorVertex.direction * (edgeLoop[0].index == 0 ? -1 : 1), cross(manipulatorVertex.direction, edgePlane.normal * (edgeLoop[0].index == 0 ? 1 : -1)));
                    const manipulatorData = { "origin" : manipulatorVertex.origin, "normal" : manipulatorPlane.normal, "x" : -manipulatorPlane.x, "edge" : line(manipulatorVertex.origin, manipulatorVertex.direction) };
                    const rotation = rotationAround(manipulatorData.edge, -definition.angle);
                    manipulators["angle"] = angularManipulator({
                                "axisOrigin" : manipulatorData.origin,
                                "axisDirection" : manipulatorData.normal,
                                "angle" : definition.angle,
                                "minValue" : 0 * degree,
                                "maxValue" : 360 * degree,
                                "rotationOrigin" : manipulatorData.origin - manipulatorData.x * profile.xInc * 2 });
                    manipulators["x"] = linearManipulator(manipulatorData.origin, rotation.linear * -manipulatorData.x, 0 * meter);
                    manipulators["y"] = linearManipulator(manipulatorData.origin, rotation.linear * cross(manipulatorData.normal, -manipulatorData.x), 0 * meter);
                }
            }

            const corners = edgeLoops[loop].corners;

            for (var i = 0; i < size(corners); i += 1)
            {
                if (corners[i].cornerType["type"] != 0)
                {
                    try silent
                    {
                        const cnr = loop ~ i;

                        // Get the end faces of the beams at the corner
                        const endFace = [qEntityFilter(qCapEntity(id + corners[i].edge[0] + "sweep", corners[i].origin[0]), EntityType.FACE),
                                qEntityFilter(qCapEntity(id + corners[i].edge[1] + "sweep", corners[i].origin[1]), EntityType.FACE)];

                        // Convert faces to planes
                        const plane1 = evPlane(context, { "face" : endFace[0] });
                        const plane2 = evPlane(context, { "face" : endFace[1] });

                        if (corners[i].cornerType["type"] == 1) // miter corner
                        {
                            // Find midplane between end faces
                            const miterPlane = plane(0.5 * (plane1.origin + plane2.origin), plane1.normal - plane2.normal);

                            // Extend end faces up to midplane
                            extendFace(context, id + ("extendFace0" ~ cnr), endFace[0], miterPlane);
                            extendFace(context, id + ("extendFace1" ~ cnr), endFace[1], miterPlane);
                        }

                        if (corners[i].cornerType["type"] > 1) // butt joint
                        {
                            // Flip butt joint if vertex selected
                            const flip = corners[i].cornerType["type"] == 3 ? 1 : 0;

                            // The tighter the angle the more the beam needs to be extended
                            const cornerAngle = angleBetween(plane1.normal, plane2.normal);
                            const multiplier = 5 + abs(5 * ((cornerAngle - 90 * degree) / (90 * degree)));

                            // Extend both faces at corner
                            extendFace(context, id + ("extendFace0" ~ cnr), endFace[flip], profile.yInc * multiplier);
                            extendFace(context, id + ("extendFace1" ~ cnr), endFace[1 - flip], profile.yInc * multiplier);

                            if (!profile.trim) // for profiles like 80/20 which are not coped
                            {
                                for (var j = 0; j < 2; j += 1)
                                {
                                    var side = abs(j - flip);

                                    var farthestPoint = evDistance(context, {
                                            "extendSide0" : true,
                                            "side0" : endFace[side],
                                            "side1" : qFarthestAlong(qVertexAdjacent(endFace[1 - side], EntityType.VERTEX), evFaceTangentPlane(context, {
                                                                    "face" : endFace[side],
                                                                    "parameter" : vector(0.5, 0.5) }).normal * (j == 0 ? 1 : -1)) });

                                    // Trim face back to surface, offset by distance to closest point
                                    extendFace(context, id + ("trimFace" ~ i ~ j), endFace[side], -farthestPoint.distance);
                                }
                            }
                            else
                            {
                                // Get the intersection between both beams
                                // NOTE: Boolean intersection can fail for some beams created with move face operation.
                                // It worked fine in the version 536, so we use that here
                                opBoolean(context, id + ("intersectCorner" ~ cnr), {
                                            "tools" : qUnion([qCreatedBy(id + corners[i].edge[flip] + "sweep", EntityType.BODY), qCreatedBy(id + corners[i].edge[1 - flip] + "sweep", EntityType.BODY)]),
                                            "operationType" : BooleanOperationType.INTERSECTION,
                                            "keepTools" : true,
                                            "asVersion" : FeatureScriptVersionNumber.V636_CONIC_FILLET_API_UPDATE
                                        });

                                // Offset faces of intersection to overcome Parasolid tolerance
                                try silent
                                {
                                    opOffsetFace(context, id + ("offsetTrimTool" ~ cnr), {
                                                "moveFaces" : qOwnedByBody(qCreatedBy(id + ("intersectCorner" ~ cnr), EntityType.BODY), EntityType.FACE),
                                                "offsetDistance" : offsetTolerance
                                            });
                                }

                                // Trim one beam by the other
                                opBoolean(context, id + ("buttJoint" ~ cnr), {
                                            "tools" : qCreatedBy(id + ("intersectCorner" ~ cnr), EntityType.BODY),
                                            "targets" : qCreatedBy(id + corners[i].edge[1 - flip] + "sweep", EntityType.BODY),
                                            "keepTools" : false,
                                            "operationType" : BooleanOperationType.SUBTRACTION });

                                // Delete excess trimmed bodies except body closest to midpoint of beam
                                opDeleteBodies(context, id + ("deleteExcess" ~ cnr), {
                                            "entities" : qSubtraction(qCreatedBy(id + corners[i].edge[1 - flip] + "sweep", EntityType.BODY), qClosestTo(qCreatedBy(id + corners[i].edge[1 - flip] + "sweep", EntityType.BODY), evEdgeTangentLine(context, { "edge" : qNthElement(definition.edges, corners[i].edge[1 - flip]), "parameter" : 0.5 }).origin)) });

                                const farthestPoint = evDistance(context, {
                                                "side0" : endFace[flip],
                                                "side1" : qFarthestAlong(qCreatedBy(id + ("buttJoint" ~ cnr), EntityType.EDGE), evFaceTangentPlane(context, {
                                                                    "face" : endFace[flip],
                                                                    "parameter" : vector(0.5, 0.5) }).normal) }).sides[1].point;

                                // Create a surface from the edge to be trimmed back to
                                opExtrude(context, id + ("surfaceExtrude" ~ cnr), {
                                            "entities" : qNthElement(definition.edges, corners[i].edge[1 - flip]),
                                            "direction" : edgePlane.normal,
                                            "endBound" : BoundingType.BLIND,
                                            "endDepth" : 0.1 * meter });

                                const distToBeam = evDistance(context, {
                                                "side0" : qCreatedBy(id + ("surfaceExtrude" ~ cnr), EntityType.FACE),
                                                "extendSide0" : true,
                                                "side1" : farthestPoint }).distance;

                                // Trim face back to surface, offset by distance to closest point
                                extendFace(context, id + ("trimFace" ~ cnr), endFace[flip], {
                                            "face" : qCreatedBy(id + ("surfaceExtrude" ~ cnr), EntityType.FACE), "offset" : distToBeam });

                                if (!definition.coped)
                                {
                                    copedCut(context, id + ("copedFace" ~ cnr), qCreatedBy(id + ("buttJoint" ~ cnr), EntityType.FACE), cnr);
                                }
                            }

                            // Add butt joint manipulator
                            try silent
                            {
                                manipulators["butt" ~ corners[i].cornerType["index"]] = flipManipulator(corners[i].vertex, evEdgeTangentLine(context, { "edge" : qNthElement(definition.edges, corners[i].edge[flip]), "parameter" : 0 }).direction, flip == 1);
                            }
                        }
                    }

                }
            }

            // Set part names
            if (definition.nameBeams)
            {
                var beamUnits = getUnits(definition);
                var nameString = "";

                if (definition.profileType == ProfileType.STANDARD)
                {
                    if (definition.nameStandard)
                        nameString ~= definition.profile.standard;

                    if (definition.nameProfile)
                        nameString ~= " " ~ definition.profile.profile;

                    if (definition.nameSize)
                        nameString ~= " " ~ definition.profile.size;
                }
                else if (definition.profileType == ProfileType.CUSTOM)
                {
                    nameString ~= definition.customName;
                }

                if (definition.nameLength)
                    nameString ~= " (";

                if (nameString != "")
                {
                    var totalLength = 0 * meter;
                    var loopBodies = qNothing();
                    var approx = ")";

                    for (var i = 0; i < size(edgeLoop); i += 1)
                    {
                        var beam = edgeLoop[i].edge;
                        var pt0 = edgeLoop[i].vertex[0].origin;
                        var pt1 = edgeLoop[i].vertex[0].direction;
                        var pt2 = edgeLoop[i].vertex[1].direction;

                        var beamBodies = qBodyType(qEntityFilter(qCreatedBy(id + beam + "sweep"), EntityType.BODY), BodyType.SOLID);

                        loopBodies = qUnion([loopBodies, beamBodies]);

                        for (var j = 0; j < size(evaluateQuery(context, beamBodies)); j += 1)
                        {
                            var length = 0 * meter;
                            var lengthString = nameString;

                            if (!definition.merge)
                                approx = ")";

                            if (definition.nameLength)
                            {
                                if (!tolerantEquals(pt1, pt2)) // edge is not a line
                                {
                                    length = evLength(context, { "entities" : qNthElement(definition.edges, beam) });
                                    approx = ")*";
                                }
                                else
                                {
                                    var outer = evBox3d(context, { "topology" : qNthElement(beamBodies, j), "cSys" : planeToCSys(plane(pt0, pt1)), "tight" : true });

                                    length = (outer.maxCorner[2] - outer.minCorner[2]);
                                }

                                if (definition.merge)
                                {
                                    totalLength += length;
                                }
                                else
                                {
                                    totalLength = length;
                                }

                                lengthString ~= toString(roundToPrecision(totalLength / beamUnits.unit, 3)) ~ " " ~ beamUnits.text ~ approx;

                            }
                            if (!definition.merge)
                            {
                                setProperty(context, { "entities" : qNthElement(beamBodies, j), "propertyType" : PropertyType.NAME, "value" : lengthString });
                                setProperty(context, { "entities" : qNthElement(beamBodies, j), "propertyType" : PropertyType.DESCRIPTION, "value" : lengthString });
                            }
                            else
                            {
                                if (i == size(edgeLoop) - 1)
                                {
                                    setProperty(context, { "entities" : loopBodies, "propertyType" : PropertyType.NAME, "value" : lengthString });
                                    setProperty(context, { "entities" : loopBodies, "propertyType" : PropertyType.DESCRIPTION, "value" : lengthString });
                                }
                            }
                        }
                    }

                    if (definition.nameLength && approx == ")*")
                    {
                        reportFeatureInfo(context, id, "Curved or merged beam length is approximated (indicated by *)");
                    }
                }
            }
        }

        // Remove all traces of imported profiles
        opDeleteBodies(context, id + "deleteContext", { "entities" : qCreatedBy(id + "sketch") });

        if (definition.merge)
        {
            try silent
            {
                opBoolean(context, id + "merge", {
                            "tools" : qCreatedBy(id, EntityType.BODY),
                            "operationType" : BooleanOperationType.UNION
                        });
            }
        }

        addManipulators(context, id, manipulators);
    });

function trimBeam(context is Context, definition is map, id is Id, yInc is ValueWithUnits)
{
    const trimTools = qUnion([definition.trimBeams, qOwnerBody(definition.trimFaces)]);

    if (size(evaluateQuery(context, trimTools)) == 0)
        return;

    const offset = 5; // arbitrary number works well in testing
    var endFaces = [];

    for (var j = 0; j < 2; j += 1)
    {
        const endFace = qEntityFilter(qCapEntity(id + "sweep", j == 0 ? true : false), EntityType.FACE);

        // if end of beam is within a certain distance of the trim tool, extend it so it intersects completely
        if (evDistance(context, { "side0" : endFace, "side1" : trimTools }).distance < yInc)
        {
            extendFace(context, id + ("extendFace" ~ j), endFace, yInc * offset);
            endFaces = append(endFaces, endFace);
        }
    }

    for (var j = 0; j < size(evaluateQuery(context, definition.trimFaces)); j += 1)
    {
        if (size(evCollision(context, {
                            "tools" : qNthElement(definition.trimFaces, j),
                            "targets" : qCreatedBy(id + "sweep", EntityType.BODY) })) != 0 ||

            size(evaluateQuery(context, qConstructionFilter(qNthElement(definition.trimFaces, j), ConstructionObject.YES))) != 0)
        {
            opPlane(context, id + ("trimPlane" ~ j), {
                        "plane" : evPlane(context, { "face" : qNthElement(definition.trimFaces, j) }),
                        "width" : 0.1 * meter,
                        "height" : 0.1 * meter
                    });

            try silent
            {
                opSplitPart(context, id + ("splitJoint" ~ j), {
                            "targets" : qCreatedBy(id + "sweep", EntityType.BODY),
                            "tool" : qCreatedBy(id + ("trimPlane" ~ j), EntityType.BODY)
                        });
            }

            opDeleteBodies(context, id + ("deleteTrimPlane" ~ j), {
                        "entities" : qCreatedBy(id + ("trimPlane" ~ j))
                    });
        }
    }

    if (size(evaluateQuery(context, definition.trimBeams)) > 0)
    {
        for (var j = 0; j < size(evaluateQuery(context, definition.trimBeams)); j += 1)
        {
            var trimBeam = qNthElement(definition.trimBeams, j);

            for (var k = 0; k < size(evaluateQuery(context, qCreatedBy(id + "sweep", EntityType.BODY))); k += 1)
            {
                var beam = qNthElement(qCreatedBy(id + "sweep", EntityType.BODY), k);

                if (evDistance(context, { "side0" : beam, "side1" : trimBeam }).distance < offsetTolerance)
                {
                    opBoolean(context, id + "intersectTrim" + j + k, {
                                "tools" : qUnion([beam, trimBeam]),
                                "operationType" : BooleanOperationType.INTERSECTION,
                                "keepTools" : true
                            });
                }
            }
        }

        // Offset faces of intersecting bodies to overcome Parasolid tolerance
        try silent
        {
            opOffsetFace(context, id + "offsetBeam", {
                        "moveFaces" : qOwnedByBody(qOwnerBody(qCreatedBy(id + "intersectTrim")), EntityType.FACE),
                        "offsetDistance" : offsetTolerance
                    });
        }

        try silent
        {
            // Trim one beam by the other
            opBoolean(context, id + "trimJoint", {
                        "tools" : qOwnerBody(qCreatedBy(id + "intersectTrim")),
                        "targets" : qCreatedBy(id + "sweep", EntityType.BODY),
                        "keepTools" : false,
                        "operationType" : BooleanOperationType.SUBTRACTION });
        }
    }

    // Extend faces back again to help determine excess material
    for (var j = 0; j < size(endFaces); j += 1)
    {
        try silent
        {
            extendFace(context, id + ("extendFaceBack" ~ j), endFaces[j], -yInc * offset);
        }
        catch
        {
            // else just delete the excess
            try silent
            {
                opDeleteBodies(context, id + ("deleteEndFaces" ~ j), {
                            "entities" : qOwnerBody(endFaces[j])
                        });
            }
        }
    }

    var excess = [];

    const cutBodies = qCreatedBy(id + "sweep", EntityType.BODY);
    const numBodies = size(evaluateQuery(context, cutBodies));

    if (numBodies > 1)
    {
        var averageVolume = 0 * meter ^ 3;

        for (var j = 0; j < numBodies; j += 1)
        {
            averageVolume += evVolume(context, { "entities" : qNthElement(cutBodies, j) });
        }

        averageVolume /= numBodies + 0.5; // this factor to be confirmed, works reasonably well in testing

        for (var j = 0; j < numBodies; j += 1)
        {
            if (evVolume(context, { "entities" : qNthElement(cutBodies, j) }) < averageVolume)
            {
                excess = append(excess, qNthElement(cutBodies, j));
            }
        }
    }

    try silent
    {
        // Delete excess trimmed bodies
        opDeleteBodies(context, id + "deleteTrimExcess", {
                    "entities" : qUnion(excess) });
    }

    if (size(evaluateQuery(context, definition.trimBeams)) > 0 && !definition.coped)
    {
        copedCut(context, id + "copedFace", qCreatedBy(id + "trimJoint", EntityType.FACE), "");
    }
}

export function beamManipulators(context is Context, definition is map, newManipulators is map) returns map
{
    try silent
    {
        var newAngle is ValueWithUnits = newManipulators["angle"].angle;
        definition.angle = newAngle;
    }

    try silent
    {
        var newX is ValueWithUnits = newManipulators["x"].offset;
        definition.x = newX * (definition.flipYAxis ? 1 : -1);
    }

    try silent
    {
        var newY is ValueWithUnits = newManipulators["y"].offset;
        definition.y = newY * -1;
    }

    try silent
    {
        for (var i = 0; i < size(evaluateQuery(context, definition.buttJoints)); i += 1)
        {

            try silent
            {
                definition.flipJoints = newManipulators["butt" ~ i].flipped ? qUnion([definition.flipJoints, qNthElement(definition.buttJoints, i)]) :
                    qSubtraction(definition.flipJoints, qNthElement(definition.buttJoints, i));
            }
        }
    }

    return definition;
}

function edgeSort(context is Context, definition is map) returns array
{
    var numEdges = size(evaluateQuery(context, definition.edges));
    var edgeVertices = [];
    var edgeOrder = [];
    var loops = [];

    for (var i = 0; i < numEdges; i += 1)
    {
        var endPoints = evEdgeTangentLines(context, { "edge" : qNthElement(definition.edges, i), "parameters" : [0, 1], "arcLengthParameterization" : false });
        edgeVertices = append(edgeVertices, endPoints[0]);
        edgeVertices = append(edgeVertices, endPoints[1]);
    }

    while (size(edgeOrder) < numEdges)
    {
        var firstEdge;

        for (var i = 0; i < numEdges; i += 1)
        {
            if (!isIn(i, edgeOrder))
            {
                firstEdge = i;
                break;
            }
        }

        edgeOrder = append(edgeOrder, firstEdge);
        var corners = [];

        var loopEdges = [{ "edge" : firstEdge, "index" : 0, "vertex" : [edgeVertices[firstEdge * 2], edgeVertices[firstEdge * 2 + 1]] }];

        for (var i = 0; i < 2; i += 1)
        {
            var nextVertex;
            var flipped = false;
            var lastEdge = firstEdge;
            var lastVertex = edgeVertices[firstEdge * 2 + i].origin;
            var finalVertex = edgeVertices[firstEdge * 2 + 1].origin;

            for (var j = 0; j < numEdges; j += 1)
            {
                var nextEdge = -1;

                for (var k = 0; k < numEdges * 2; k += 1)
                {
                    if (!isIn(floor(k / 2), edgeOrder) && tolerantEquals(edgeVertices[k].origin, lastVertex))
                    {
                        nextEdge = floor(k / 2);
                        flipped = k % 2 == i;
                        nextVertex = edgeVertices[k + (k % 2 == 1 ? -1 : 1)].origin;
                        break;
                    }
                }

                if (nextEdge > 0)
                {
                    edgeOrder = append(edgeOrder, nextEdge);
                    loopEdges = append(loopEdges, { "edge" : nextEdge, "index" : flipped ? 1 : 0, "vertex" : [edgeVertices[nextEdge * 2], edgeVertices[nextEdge * 2 + 1]] });
                    corners = append(corners, { "edge" : [lastEdge, nextEdge], "origin" : [i == 0, i == 1], "vertex" : lastVertex, "cornerType" : cornerType(context, definition, lastVertex) });
                    lastEdge = nextEdge;
                    lastVertex = nextVertex;
                }
                else
                {
                    if (tolerantEquals(lastVertex, finalVertex) && j > 0)
                    {
                        corners = append(corners, { "edge" : [lastEdge, firstEdge], "origin" : [i == 0, i == 1], "vertex" : lastVertex, "cornerType" : cornerType(context, definition, lastVertex) });
                    }
                    break;
                }
            }
        }
        loops = append(loops, { "edges" : loopEdges, "corners" : corners });
    }
    return loops;
}

function cornerType(context is Context, definition is map, cornerVertex is Vector) returns map
{
    var cornerType = { "type" : 0, "index" : 0 }; // no corner treatment, index is irrelevant

    if (definition.miterAll || isInTolerance(context, definition.miterJoints, cornerVertex) >= 0)
        cornerType["type"] = 1; // miter corner

    if (isInTolerance(context, definition.buttJoints, cornerVertex) >= 0)
    {
        cornerType["type"] = 2; // butt joint
        cornerType["index"] = isInTolerance(context, definition.buttJoints, cornerVertex);

        if (isInTolerance(context, definition.flipJoints, cornerVertex) >= 0)
        {
            cornerType["type"] = 3; // flipped butt joint
        }
    }
    return cornerType;
}

function isInTolerance(context is Context, points is Query, value is Vector) returns number
{
    for (var i = 0; i < size(evaluateQuery(context, points)); i += 1)
    {
        if (tolerantEquals(evVertexPoint(context, { "vertex" : qNthElement(points, i) }), value))
            return i;
    }
    return -1;
}

function extendFace(context is Context, id is Id, face is Query, target)
{
    if (target is ValueWithUnits)
    {
        opOffsetFace(context, id + "offsetFace", {
                    "moveFaces" : face,
                    "offsetDistance" : target
                });
        return;
    }

    var targetFace = target.face;
    var offset = target.offset;
    var oppositeSense = false;

    if (offset == undefined)
        offset = 0;

    if (target is Plane)
    {
        opPlane(context, id + "miterPlane", { "plane" : target, "width" : 1 * meter, "height" : 1 * meter });
        targetFace = qCreatedBy(id + "miterPlane", EntityType.FACE);
        offset = 0;
    }

    const faceNormal = evFaceTangentPlane(context, {
                "face" : face,
                "parameter" : vector(0.5, 0.5) });

    const targetFaceNormal = evFaceTangentPlane(context, {
                "face" : targetFace,
                "parameter" : vector(0.5, 0.5) });

    if (angleBetween(faceNormal.normal, targetFaceNormal.normal) > PI / 2 * radian)
    {
        oppositeSense = true;
        offset *= -1;
    }

    opReplaceFace(context, id + "replaceFace", { "replaceFaces" : face, "templateFace" : targetFace, "oppositeSense" : oppositeSense, "offset" : offset });

    if (target is Plane)
    {
        opDeleteBodies(context, id + "deletePlane", { "entities" : qCreatedBy(id + "miterPlane") });
    }
    else
    {
        opDeleteBodies(context, id + "deletePlane", { "entities" : targetFace });
    }
}

function copedCut(context is Context, id is Id, faces is Query, cnr is string)
{
    // Check to see if corner has fillets made from an arced or straight beam
    var filletFaces = qUnion([qGeometry(faces, GeometryType.TORUS), qGeometry(faces, GeometryType.CYLINDER)]);
    var filletFaceCount = size(evaluateQuery(context, filletFaces));

    if (filletFaceCount == size(evaluateQuery(context, faces))) // Only one face most likely a full pipe
    {
        return; // Assumes intersecting pipes must be coped to get a good weld
    }

    var j = 0;

    while (filletFaceCount > 0)
    {
        const filletFace = qNthElement(filletFaces, filletFaceCount - 1);

        const flatFaces = qGeometry(qSubtraction(qEdgeAdjacent(qIntersection([qEdgeAdjacent(filletFace, EntityType.EDGE), qEdgeAdjacent(qSubtraction(qConcaveConnectedFaces(filletFace), filletFace), EntityType.EDGE)]), EntityType.FACE), filletFace), GeometryType.PLANE);

        var success = false;

        if (size(evaluateQuery(context, flatFaces)) > 1)
        {
            try silent // try delete face first, on concave face only
            {
                opDeleteFace(context, id + ("deleteFillet" ~ cnr ~ j), {
                            "deleteFaces" : filletFace,
                            "includeFillet" : false,
                            "capVoid" : false
                        });
                success = true;
            }
        }

        if (!success)
        {
            try silent // if delete face failed, try replace face using adjacent face
            {
                opReplaceFace(context, id + ("replaceConcave" ~ cnr ~ j), {
                            "replaceFaces" : filletFace,
                            "templateFace" : flatFaces
                        });
            }
            catch (e)
            {
                const edges = qIntersection([qEdgeAdjacent(filletFace, EntityType.EDGE), qEdgeAdjacent(qSubtraction(qGeometry(qTangentConnectedFaces(filletFace), GeometryType.PLANE), filletFace), EntityType.EDGE)]);

                if (e.message == "CANNOT_RESOLVE_ENTITIES") // the fillet is convex
                {
                    try silent
                    {
                        // create a chamfer by lofting between the two fillet edges
                        opLoft(context, id + ("chamfer" ~ cnr ~ j), {
                                    "profileSubqueries" : [qNthElement(edges, 0), qNthElement(edges, 1)],
                                    "bodyType" : ToolBodyType.SURFACE
                                });

                        extendFace(context, id + ("replaceConvex" ~ cnr ~ j), filletFace, { "face" : qCreatedBy(id + ("chamfer" ~ cnr ~ j), EntityType.FACE) });
                    }
                }
                else // fillet is concave but only has one tangent face so need to construct a face to use in replace face
                {
                    // Get midpoints of all 4 edges of untrimmed fillet
                    var edgeMidPoints = evFaceTangentPlanes(context, {
                            "face" : filletFace,
                            "parameters" : [vector(0, 0.5), vector(1, 0.5), vector(0.5, 0), vector(0.5, 1)] });

                    var side = 0;
                    var vectors = [];

                    // Get the distance between the lines normal to the midpoints between opposite facing edges
                    var distNormals = [evDistance(context, {
                                "side0" : line(edgeMidPoints[0].origin, edgeMidPoints[0].normal),
                                "side1" : line(edgeMidPoints[1].origin, edgeMidPoints[1].normal)
                            }), evDistance(context, {
                                "side0" : line(edgeMidPoints[2].origin, edgeMidPoints[2].normal),
                                "side1" : line(edgeMidPoints[3].origin, edgeMidPoints[3].normal) })];

                    // The lines closest together are the extents of the fillet radius
                    if (evDistance(context, {
                                        "side0" : edgeMidPoints[0].origin,
                                        "side1" : distNormals[0].sides[0].point
                                    }).distance > evDistance(context, {
                                        "side0" : edgeMidPoints[0].origin,
                                        "side1" : distNormals[1].sides[0].point }).distance)
                    {
                        side = 1;
                    }

                    for (var i = 0.1; i < 1; i += 0.1) // need a way to sample the edge along the untrimmed surface since there is no function to untrim an edge
                    {
                        if (side == 1)
                        {
                            vectors = append(vectors, vector(i, 0));
                            vectors = append(vectors, vector(i, 1));
                        }
                        else
                        {
                            vectors = append(vectors, vector(0, i));
                            vectors = append(vectors, vector(1, i));
                        }
                    }

                    const samples = evFaceTangentPlanes(context, {
                                "face" : filletFace,
                                "parameters" : vectors
                            });

                    var sweepEdge;

                    for (var i = 0; i < size(samples); i += 1)
                    {
                        sweepEdge = qIntersection([qSubtraction(qEdgeAdjacent(filletFace, EntityType.EDGE), edges), qContainsPoint(qEdgeAdjacent(filletFace, EntityType.EDGE), samples[i].origin)]);

                        if (size(evaluateQuery(context, sweepEdge)) > 0)
                            break;
                    }

                    try silent
                    {
                        const midSweepEdge = evEdgeTangentLine(context, { "edge" : sweepEdge, "parameter" : 0.5 });

                        // extrude a surface for use in replace face
                        opExtrude(context, id + ("extrudeSweep" ~ cnr ~ j), {
                                    "entities" : sweepEdge,
                                    "direction" : cross(midSweepEdge.direction, evFaceNormalAtEdge(context, { "edge" : sweepEdge, "face" : filletFace, "parameter" : 0 })),
                                    "endBound" : BoundingType.BLIND,
                                    "endDepth" : 1 * millimeter
                                });

                        extendFace(context, id + ("replaceFailedConcave" ~ cnr ~ j), filletFace, { "face" : qCreatedBy(id + ("extrudeSweep" ~ cnr ~ j), EntityType.FACE) });
                    }
                }
            }
        }

        filletFaceCount -= 1;

        if (size(evaluateQuery(context, filletFaces)) < filletFaceCount)
            filletFaceCount = size(evaluateQuery(context, filletFaces));

        j += 1;
    }
}

function getPlane(context is Context, definition is map, edgeLoop is array) returns Plane
{
    const firstEdge = qNthElement(definition.edges, edgeLoop[0].edge);

    var loopPlane;

    try
    {
        // check if the first entity is a sketch edge, if not check for solid or surface edge
        loopPlane = evOwnerSketchPlane(context, { "entity" : firstEdge });
    }
    catch
    {
        // this will be two faces so just have to return the first
        loopPlane = evPlane(context, { "face" : qGeometry(qEdgeAdjacent(firstEdge, EntityType.FACE), GeometryType.PLANE) });

        if (size(edgeLoop) > 1)
        {
            const point1 = edgeLoop[0].vertex[0].origin;
            const point2 = edgeLoop[0].vertex[1].origin;
            var point3;
            var normal;
            var success = false;

            for (var i = 1; i < 2; i += 1)
            {
                for (var j = 0; j < 2; j += 1)
                {
                    point3 = edgeLoop[i].vertex[j].origin;
                    normal = cross(point3 - point1, point2 - point1);
                    if (norm(normal).value > TOLERANCE.zeroLength)
                    {
                        success = true;
                        break;
                    }
                }
                if (success)
                    break;
            }
            loopPlane = plane(point1, normalize(normal), normalize(point2 - point1));
        }
    }

    return loopPlane;
}

function getProfile(context is Context, id is Id, beamProfileTable is map, definition is map) returns map
{
    var profileStandard = undefined;
    const profileId = id + "sketch";

    if (definition.profileType == ProfileType.STANDARD)
    {
        profileStandard = beamProfileTable.entries[definition.profile.standard];
        const profileData = profileStandard.entries[definition.profile.profile];
        if (profileData == undefined)
        {
            throw regenError("Select a beam profile");
        }
        const profileEntries = profileData.entries[definition.profile.size];
        const profilePoints = profileEntries.points;
        var profileUnits;

        if (profileStandard.units != undefined)
            profileUnits = profileStandard.units;

        if (profileData.units != undefined)
            profileUnits = profileData.units;

        var profileSequence;

        if (profileData.sequence != undefined)
            profileSequence = splitIntoCharacters(profileData.sequence);

        if (profileEntries.sequence != undefined)
            profileSequence = splitIntoCharacters(profileEntries.sequence);

        const sketch = newSketchOnPlane(context, profileId, { "sketchPlane" : plane(vector(0, 0, 0) * meter, vector(0, 0, 1)) });

        var sequenceNumber = 0;
        var lastSequence = "";

        for (var i = 0; i < size(profileSequence); i += 1)
        {
            if (profileSequence[i] == "-")
            {
                sequenceNumber += 2;
                continue;
            }

            if (profileSequence[i] == "L")
            {
                skLineSegment(sketch, "line" ~ i, {
                            "start" : vector(profilePoints[sequenceNumber], profilePoints[sequenceNumber + 1]) * profileUnits,
                            "end" : vector(profilePoints[sequenceNumber + 2], profilePoints[sequenceNumber + 3]) * profileUnits
                        });
                sequenceNumber += 2;
            }

            if (profileSequence[i] == "A")
            {
                skArc(sketch, "arc" ~ i, {
                            "start" : vector(profilePoints[sequenceNumber], profilePoints[sequenceNumber + 1]) * profileUnits,
                            "mid" : vector(profilePoints[sequenceNumber + 2], profilePoints[sequenceNumber + 3]) * profileUnits,
                            "end" : vector(profilePoints[sequenceNumber + 4], profilePoints[sequenceNumber + 5]) * profileUnits
                        });
                sequenceNumber += 4;
            }

            if (profileSequence[i] == "C")
            {
                if (lastSequence != "C" && sequenceNumber > 0)
                    sequenceNumber += 2;
                skCircle(sketch, "circle" ~ i, {
                            "center" : vector(profilePoints[sequenceNumber], profilePoints[sequenceNumber + 1]) * profileUnits,
                            "radius" : profilePoints[sequenceNumber + 2] * profileUnits,
                        });
                sequenceNumber += 3;
            }

            lastSequence = profileSequence[i];
        }

        skSolve(sketch);
    }
    else if (definition.profileType == ProfileType.CUSTOM)
    {
        if (definition.customProfileSketch.buildFunction == undefined)
        {
            throw regenError("Select a Part Studio with a sketched beam profile", [definition.customProfileSketch]);
        }

        const instantiator = newInstantiator(profileId);
        addInstance(instantiator, definition.customProfileSketch, {});
        instantiate(context, instantiator);
    }
    else
    {
        throw regenError("Unknown profile type: " ~ definition.profileType, ["profileType"]);
    }

    const box3d = evBox3d(context, { "topology" : qCreatedBy(profileId, EntityType.FACE) });
    var faceExtents = { "face" : qOwnerBody(qCreatedBy(profileId, EntityType.FACE)) };
    faceExtents.ext = norm(box3d.maxCorner - box3d.minCorner);
    faceExtents.xMin = box3d.minCorner[0];
    faceExtents.xMax = box3d.maxCorner[0];
    faceExtents.yMin = box3d.minCorner[1];
    faceExtents.yMax = box3d.maxCorner[1];
    faceExtents.xInc = (faceExtents.xMax - faceExtents.xMin) / 2;
    faceExtents.yInc = (faceExtents.yMax - faceExtents.yMin) / 2;

    faceExtents.trim = true;
    if (definition.profileType == ProfileType.CUSTOM && !definition.coping)
        faceExtents.trim = false;
    if (profileStandard != undefined && profileStandard.trim != undefined)
        faceExtents.trim = profileStandard.trim;

    return faceExtents;
}

function getUnits(definition is map) returns map
{
    var unitType = millimeter;
    var unitText = "mm";

    if (definition.nameUnits == CUTLENGTH_UNITS.IN)
    {
        unitType = inch;
        unitText = "in";
    }
    if (definition.nameUnits == CUTLENGTH_UNITS.M)
    {
        unitType = meter;
        unitText = "m";
    }
    if (definition.nameUnits == CUTLENGTH_UNITS.CM)
    {
        unitType = centimeter;
        unitText = "cm";
    }
    if (definition.nameUnits == CUTLENGTH_UNITS.FT)
    {
        unitType = foot;
        unitText = "ft";
    }
    if (definition.nameUnits == CUTLENGTH_UNITS.YD)
    {
        unitType = yard;
        unitText = "yd";
    }

    return { "unit" : unitType, "text" : unitText };
}

export enum CUTLENGTH_UNITS
{
    annotation { "Name" : "Centimeter" }
    CM,
    annotation { "Name" : "Foot" }
    FT,
    annotation { "Name" : "Inch" }
    IN,
    annotation { "Name" : "Meter" }
    M,
    annotation { "Name" : "Millimeter" }
    MM,
    annotation { "Name" : "Yard" }
    YD
}
