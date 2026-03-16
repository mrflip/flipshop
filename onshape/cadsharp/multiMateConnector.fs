
//_______________________________________________________________________________________________________________________________________________
//
// This FeatureScript is owned by Konstantin and is distributed by CADSharp LLC.
// You may not redistribute it for commercial purposes without the permission of said owner and CADSharp LLC. Copyright (c) 2023 Konstantin.
//_______________________________________________________________________________________________________________________________________________


FeatureScript 2796; //2796
import(path : "onshape/std/geometry.fs", version : "2796.0");
import(path : "f902670516b3542414e3ab98/0c105b72466a3dfa2898f304/81858a9ae2c1683847d4a5b0", version : "3d8eeffcddc71039071b7723");

// CADSharp
export import(path : "cbeb3dcf671e00785597bd76/409d65a3744fe434f32bdffc/a75ab01def146a42f55baa7f", version : "381046010d5aea697e433948");

icon::import(path : "7fb08a323ede99ecf3483d32", version : "8831ccfa83ed31b7253cd5d5");

export enum MCDefinitionType
{
    annotation { "Name" : "Default" }
    DEFAULT,
    annotation { "Name" : "Centroid" }
    CENTROID,
    annotation { "Name" : "Path" }
    PATH,
    annotation { "Name" : "Face" }
    FACE
}

export enum MCAxisDefinitionType
{
    annotation { "Name" : "Default" }
    DEFAULT,
    annotation { "Name" : "Direction" }
    DIRECTION,
    annotation { "Name" : "Reference vertex" }
    REF_VERTEX,
    annotation { "Name" : "Face normal" }
    REF_FACES,
    annotation { "Name" : "Edge tangent" }
    REF_EDGES
}

annotation { "Feature Type Name" : "MultiMateConnector",
        "Icon" : icon::BLOB_DATA,
        "Feature Type Description" : "<b> Summary </b> <br> An advanced way to create mates. <br>",
        "Description Image" : cadsharpLogo::BLOB_DATA,
        "Editing Logic Function" : "cadsharpUrlEditLogic"
    }
export const mMateConnector = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Definition type", "UIHint" : UIHint.HORIZONTAL_ENUM }
        definition.defType is MCDefinitionType;

        annotation { "Name" : "Construction", "Default" : true }
        definition.isConstruction is boolean;

        annotation { "Group Name" : "My Group", "Driving Parameter" : "isConstruction", "Collapsed By Default" : false }
        {
            if (!definition.isConstruction)
            {
                annotation { "Name" : "Owner parts",
                            "Filter" : EntityType.BODY && (BodyType.SOLID || BodyType.SHEET || GeometryType.MESH) && AllowMeshGeometry.YES && ModifiableEntityOnly.YES }
                definition.ownerParts is Query;
            }
        }

        annotation { "Name" : "Triad" }
        definition.addTriad is boolean;

        if (definition.defType == MCDefinitionType.DEFAULT)
        {
            annotation { "Name" : "Mate connector", "Filter" : BodyType.MATE_CONNECTOR, "MaxNumberOfPicks" : 1 }
            definition.mateConnector is Query;
        }
        else if (definition.defType == MCDefinitionType.CENTROID)
        {
            annotation { "Name" : "Centroid origin", "Filter" : EntityType.VERTEX || EntityType.EDGE || EntityType.FACE || EntityType.BODY }
            definition.centroidOrigin is Query;
        }
        if (definition.defType == MCDefinitionType.PATH)
        {
            annotation { "Name" : "Path", "Filter" : EntityType.EDGE && ConstructionObject.NO }
            definition.pathQuery is Query;

            annotation { "Name" : "Length or parameter" }
            isAnything(definition.lengthParameter);

            annotation { "Name" : "Flip path direction", "UIHint" : UIHint.OPPOSITE_DIRECTION }
            definition.flipPath is boolean;
        }
        else if (definition.defType == MCDefinitionType.FACE)
        {
            annotation { "Name" : "Face", "Filter" : EntityType.FACE, "MaxNumberOfPicks" : 1 }
            definition.face is Query;

            annotation { "Name" : "U" }
            isReal(definition.U, POSITIVE_REAL_BOUNDS);

            annotation { "Name" : "V" }
            isReal(definition.V, POSITIVE_REAL_BOUNDS);
        }

        annotation { "Group Name" : "Primary axis", "Collapsed By Default" : true }
        {
            annotation { "Name" : "Primary axis definition", "UIHint" : UIHint.SHOW_LABEL }
            definition.primaryAxisDefType is MCAxisDefinitionType;

            if (definition.primaryAxisDefType == MCAxisDefinitionType.DIRECTION)
            {
                annotation { "Name" : "Primary axis direction", "Filter" : QueryFilterCompound.ALLOWS_DIRECTION, "MaxNumberOfPicks" : 1 }
                definition.zAxisDirection is Query;
            }
            else if (definition.primaryAxisDefType == MCAxisDefinitionType.REF_VERTEX)
            {
                annotation { "Name" : "Primary axis reference vertex", "Filter" : EntityType.VERTEX }
                definition.zAxisRefVertex is Query;
            }
            else if (definition.primaryAxisDefType == MCAxisDefinitionType.REF_FACES)
            {
                annotation { "Name" : "Primary axis reference faces", "Filter" : EntityType.FACE }
                definition.zAxisRefFaces is Query;
            }
            if (definition.primaryAxisDefType == MCAxisDefinitionType.REF_EDGES)
            {
                annotation { "Name" : "Primary axis reference edges", "Filter" : EntityType.EDGE }
                definition.zAxisRefEdges is Query;
            }

            annotation { "Name" : "Flip primary axis", "UIHint" : UIHint.OPPOSITE_DIRECTION, "UIHint" : UIHint.DISPLAY_SHORT }
            definition.flipPrimary is boolean;
        }

        annotation { "Group Name" : "Secondary axis", "Collapsed By Default" : true }
        {
            annotation { "Name" : "Secondary axis definition", "UIHint" : UIHint.SHOW_LABEL }
            definition.secondaryAxisDefType is MCAxisDefinitionType;

            if (definition.secondaryAxisDefType == MCAxisDefinitionType.DIRECTION)
            {
                annotation { "Name" : "Secondary axis direction", "Filter" : QueryFilterCompound.ALLOWS_DIRECTION, "MaxNumberOfPicks" : 1 }
                definition.xAxisDirection is Query;
            }
            else if (definition.secondaryAxisDefType == MCAxisDefinitionType.REF_VERTEX)
            {
                annotation { "Name" : "Secondary axis reference vertex", "Filter" : EntityType.VERTEX }
                definition.xAxisRefVertex is Query;
            }
            else if (definition.secondaryAxisDefType == MCAxisDefinitionType.REF_FACES)
            {
                annotation { "Name" : "Secondary axis reference faces", "Filter" : EntityType.FACE }
                definition.xAxisRefFaces is Query;
            }
            if (definition.secondaryAxisDefType == MCAxisDefinitionType.REF_EDGES)
            {
                annotation { "Name" : "Secondary axis reference edges", "Filter" : EntityType.EDGE }
                definition.xAxisRefEdges is Query;
            }

            annotation { "Name" : "Flip secondary axis", "UIHint" : UIHint.OPPOSITE_DIRECTION, "UIHint" : UIHint.DISPLAY_SHORT }
            definition.flipSecondary is boolean;
        }

        annotation { "Name" : "Transform array", "Item name" : "Transform", "Item label template" : "Active: #isActive [#dX, #dY, #dZ] [#rotX, #rotY, #rotZ]" }
        definition.trArray is array;
        for (var trDef in definition.trArray)
        {
            annotation { "Name" : "Active", "Default" : true }
            trDef.isActive is boolean;

            annotation { "Name" : "dX" /*, "UIHint" : [UIHint.DISPLAY_SHORT, UIHint.FIRST_IN_ROW]*/ }
            isLength(trDef.dX, ZERO_DEFAULT_LENGTH_BOUNDS);

            annotation { "Name" : "dY" }
            isLength(trDef.dY, ZERO_DEFAULT_LENGTH_BOUNDS);

            annotation { "Name" : "dZ" }
            isLength(trDef.dZ, ZERO_DEFAULT_LENGTH_BOUNDS);

            annotation { "Name" : "rotX" }
            isAngle(trDef.rotX, ANGLE_360_ZERO_DEFAULT_BOUNDS);

            annotation { "Name" : "rotY" }
            isAngle(trDef.rotY, ANGLE_360_ZERO_DEFAULT_BOUNDS);

            annotation { "Name" : "rotZ" }
            isAngle(trDef.rotZ, ANGLE_360_ZERO_DEFAULT_BOUNDS);
        }

        cadsharpUrlPredicate(definition);
    }
    {
        const remainingTransform = getRemainderPatternTransform(context, { "references" : mMCReferences(id, definition) });

        //Origin and default axes evaluation
        var cSys = WORLD_COORD_SYSTEM;
        if (definition.defType == MCDefinitionType.DEFAULT)
        {
            cSys = evMateConnector(context, { "mateConnector" : definition.mateConnector });
        }
        else if (definition.defType == MCDefinitionType.CENTROID)
        {
            cSys.origin = evApproximateCentroid(context, { "entities" : definition.centroidOrigin });
        }
        else if (definition.defType == MCDefinitionType.PATH)
        {
            var path = constructPath(context, definition.pathQuery);
            if (definition.flipPath)
                path = reverse(path);

            const pathLength = evPathLength(context, path);

            const pathParameter = evPathParameter(definition.lengthParameter, pathLength, 0);
            const edgeParam = evPathEdgeParameter(context, path, pathParameter);

            cSys = evEdgeCurvature(context, edgeParam).frame;
        }
        else if (definition.defType == MCDefinitionType.FACE)
        {
            const tPlane = evFaceTangentPlane(context, {
                        "face" : definition.face,
                        "parameter" : vector(definition.U % 1, definition.V % 1)
                    });
            cSys = coordSystem(tPlane);
        }

        //Realigning primary axis
        cSys = adjustCoordSystemAxis(context, cSys, "zAxis", {
                    "axisDefType" : definition.primaryAxisDefType,
                    "axisDirection" : definition.zAxisDirection,
                    "axisRefVertex" : definition.zAxisRefVertex,
                    "axisRefFaces" : definition.zAxisRefFaces,
                    "axisRefEdges" : definition.zAxisRefEdges,
                    "flipAxis" : definition.flipPrimary
                });

        //Realigning secondary axis
        cSys = adjustCoordSystemAxis(context, cSys, "xAxis", {
                    "axisDefType" : definition.secondaryAxisDefType,
                    "axisDirection" : definition.xAxisDirection,
                    "axisRefVertex" : definition.xAxisRefVertex,
                    "axisRefFaces" : definition.xAxisRefFaces,
                    "axisRefEdges" : definition.xAxisRefEdges,
                    "flipAxis" : definition.flipSecondary
                });

        //Applying transforms
        for (var trDef in definition.trArray)
        {
            if (trDef.isActive)
            {
                //Linear transform
                cSys = transform(toWorld(cSys) * vector(trDef.dX, trDef.dY, trDef.dZ) - cSys.origin) * cSys;
                //Rotational transform
                cSys = rotationAround(line(cSys.origin, cSys.xAxis), trDef.rotX) * cSys;
                cSys = rotationAround(line(cSys.origin, yAxis(cSys)), trDef.rotY) * cSys;
                cSys = rotationAround(line(cSys.origin, cSys.zAxis), trDef.rotZ) * cSys;
            }
        }

        var coordSystems = { "" : cSys };
        if (definition.addTriad)
        {
            coordSystems.X = coordSystem(cSys.origin, yAxis(cSys), cSys.xAxis);
            coordSystems.Y = coordSystem(cSys.origin, cSys.zAxis, yAxis(cSys));
        }

        const ownerParts = definition.isConstruction ? [qNothing()] : evaluateQuery(context, definition.ownerParts);

        for (var i, ownerPart in ownerParts)
        {
            for (var suf, cSys in coordSystems)
            {
                opMateConnector(context, id + ("mateConnector" ~ (i + 1) ~ suf), { "coordSystem" : cSys, "owner" : ownerPart });
                //opMateConnector(context, id + i + suf, { "coordSystem" : cSys, "owner" : ownerPart });
            }
        }

        transformResultIfNecessary(context, id, remainingTransform);
    });

/**
   Rerurns edege parameter by the given path parameter
   Related to Path utils library: https://cad.onshape.com/documents/f902670516b3542414e3ab98/w/1a850a1464f5ab837bf919a9/e/81858a9ae2c1683847d4a5b0
 */
function evPathEdgeParameter(context is Context, path is Path, pathParam is number) returns map
{
    var pathLength = evPathLength(context, path);
    var fullEdgesLengthParam = 0;
    var i = 0;
    for (var edge in path.edges)
    {
        const edgeLength = evLength(context, { "entities" : edge });
        const endEdgePathParam = fullEdgesLengthParam + edgeLength / pathLength;

        if (fullEdgesLengthParam <= pathParam && endEdgePathParam >= pathParam - TOLERANCE.zeroLength * meter / pathLength)
        {
            var edgeLengthParam = (pathParam - fullEdgesLengthParam) * pathLength / edgeLength;
            edgeLengthParam = path.flipped[i] ? 1 - edgeLengthParam : edgeLengthParam;

            return { "edge" : edge, "parameter" : edgeLengthParam };
        }
        fullEdgesLengthParam = endEdgePathParam;
        i += 1;
    }
}

/**
 * Returns component of source vector ortogonal to the reference vector
 */
function adjustCoordSystemAxis(context is Context, cSys is CoordSystem, axisName is string, definition is map) returns CoordSystem
precondition
{
    definition.axisDefType is MCAxisDefinitionType;
    definition.axisDirection is Query || definition.axisDirection is undefined;
    definition.axisRefVertex is Query || definition.axisDirection is undefined;
    definition.axisRefFaces is Query || definition.axisRefFaces is undefined;
    definition.axisRefEdges is Query || definition.axisRefEdges is undefined;
    definition.flipAxis is boolean;
}
{
    var axisDir = cSys[axisName];
    const yDir = yAxis(cSys);

    if (definition.axisDefType == MCAxisDefinitionType.DIRECTION)
    {
        axisDir = extractDirection(context, definition.axisDirection);
    }
    else if (definition.axisDefType == MCAxisDefinitionType.REF_VERTEX)
    {
        definition.axisRefVertex = evVertexPoint(context, { "vertex" : definition.axisRefVertex });
        axisDir = normalize(definition.axisRefVertex - cSys.origin);
    }
    else if (definition.axisDefType == MCAxisDefinitionType.REF_FACES)
    {
        const distResult = evDistance(context, {
                    "side0" : cSys.origin,
                    "side1" : definition.axisRefFaces
                });

        axisDir = evFaceTangentPlane(context, {
                        "face" : qNthElement(definition.axisRefFaces, distResult.sides[1].index),
                        "parameter" : distResult.sides[1].parameter
                    }).normal;
    }
    else if (definition.axisDefType == MCAxisDefinitionType.REF_EDGES)
    {
        const distResult = evDistance(context, {
                    "side0" : cSys.origin,
                    "side1" : definition.axisRefEdges
                });

        axisDir = evEdgeTangentLine(context, {
                        "edge" : qNthElement(definition.axisRefEdges, distResult.sides[1].index),
                        "parameter" : distResult.sides[1].parameter
                    }).direction;
    }

    if (definition.flipAxis)
        axisDir *= -1;

    cSys[axisName] = axisDir;

    //Normalization and orthogonalization of xAxis relatively to zAxis by default
    cSys.xAxis = try silent(ortogonalize(cSys.xAxis, cSys.zAxis, true));

    //means that old xAxis is parallel to new zAxis
    if (cSys.xAxis is undefined)
        cSys.xAxis = cross(yDir, cSys.zAxis);

    return cSys;
}

function mMCReferences(id, definition is map) returns Query
{
    var references = qNothing();

    if (definition.defType == MCDefinitionType.DEFAULT)
        references = qUnion(references, definition.mateConnector);
    else if (definition.defType == MCDefinitionType.CENTROID)
        references = qUnion(references, definition.centroidOrigin);
    if (definition.defType == MCDefinitionType.PATH)
        references = qUnion(references, definition.pathQuery);
    else if (definition.defType == MCDefinitionType.FACE)
        references = qUnion(references, definition.face);

    if (definition.primaryAxisDefType == MCAxisDefinitionType.DIRECTION)
        references = qUnion(references, definition.zAxisDirection);
    else if (definition.primaryAxisDefType == MCAxisDefinitionType.REF_VERTEX)
        references = qUnion(references, definition.zAxisRefVertex);
    else if (definition.primaryAxisDefType == MCAxisDefinitionType.REF_FACES)
        references = qUnion(references, definition.zAxisRefFaces);
    if (definition.primaryAxisDefType == MCAxisDefinitionType.REF_EDGES)
        references = qUnion(references, definition.zAxisRefEdges);

    if (definition.secondaryAxisDefType == MCAxisDefinitionType.DIRECTION)
        references = qUnion(references, definition.xAxisDirection);
    else if (definition.secondaryAxisDefType == MCAxisDefinitionType.REF_VERTEX)
        references = qUnion(references, definition.xAxisRefVertex);
    else if (definition.secondaryAxisDefType == MCAxisDefinitionType.REF_FACES)
        references = qUnion(references, definition.xAxisRefFaces);
    else if (definition.secondaryAxisDefType == MCAxisDefinitionType.REF_EDGES)
        references = qUnion(references, definition.xAxisRefEdges);

    if (definition.isConstruction)
        definition.ownerParts = qCreatedBy(id + "point", EntityType.BODY);

    references = qUnion(references, definition.ownerParts);

    return references;
}
