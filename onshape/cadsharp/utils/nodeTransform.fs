FeatureScript 1589;
import(path : "onshape/std/geometry.fs", version : "1589.0");

//Reference point manipulators from Freeform spline by Evan Reese, Thanks Evan!
// https://cad.onshape.com/documents/cd51f29c6937305f86a9df95/v/434140acea902a64b822f76d/e/27d79de4b4e2ddcdba150ebe

const NODE_ID_ARRAY = [
        "Back, bottom, mid", // 0
        "Back, bottom, left", // 1
        "Back, bottom, right", // 2
        "Back, center", // 3
        "Back, mid, left", // 4
        "Back, mid, right", // 5
        "Back, top, mid", // 6
        "Back, top, left", // 7
        "Back, top, right", // 8
        "Front, bottom, mid", // 9
        "Front, bottom, left", // 10
        "Front, bottom, right", // 11
        "Front, center", // 12
        "Front, mid, left", // 13
        "Front, mid, right", // 14
        "Front, top, mid", // 15
        "Front, top, left", // 16
        "Front, top, right", // 17
        "Bottom, center", // 18
        "Mid, bottom, left", // 19
        "Mid, bottom, right", // 20
        "Center", // 21
        "Left, center", // 22
        "Right, center", // 23
        "Top, center", // 24
        "Mid, top, left", // 25
        "Mid, top, right" // 26
    ];

annotation { "Feature Type Name" : "Node transform",
        "Manipulator Change Function" : "manipulatorChange" }
export const nodeTransform = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        // Hidden parameter that stores the axis direction plane of the angular manipulators
        annotation { "Name" : "Manipulator axis plane", "UIHint" : UIHint.ALWAYS_HIDDEN }
        isAnything(definition.axisPlane);

        // This hidden number changes the index of the selected point when it's clicked.
        annotation { "Name" : "Node index", "UIHint" : UIHint.ALWAYS_HIDDEN }
        isInteger(definition.pointIndex, { (unitless) : [0, 18, 1000000] } as IntegerBoundSpec);

        annotation { "Name" : "Node:", "UIHint" : UIHint.READ_ONLY, "Default" : "Bottom, center" }
        definition.currentPoint is string;

        annotation { "Name" : "Same studio entities", "Default" : true }
        definition.sameStudioBoolean is boolean;

        if (definition.sameStudioBoolean)
        {
            annotation { "Name" : "Entities", "Filter" : EntityType.BODY }
            definition.sameStudioBody is Query;

            //When selecting mate connectors, I prefer having the default option to pick an existing mate connector, then a button to create a connector. See below:
            annotation { "Name" : "Profile origin mate", "Filter" : BodyType.MATE_CONNECTOR || BodyType.MATE_CONNECTOR, "MaxNumberOfPicks" : 1 }
            definition.sameStudioBodyOriginMate is Query;
        }
        else
        {
            annotation { "Name" : "Entities" }
            definition.otherStudioBodyDef is PartStudioData;

            // annotation { "Name" : "Derive origin" }
            // definition.deriveOrigin is boolean;

            // if (definition.deriveOrigin)
            // {
            //     annotation { "Name" : "Part Origin" }
            //     definition.otherOriginDef is PartStudioData;
            // }

            // annotation { "Name" : "Profile Origin", "UIHint" : "HORIZONTAL_ENUM" }
            // definition.otherStudioOrigin is OTHER_STUDIO_ORIGIN;

            // if (definition.otherStudioOrigin == OTHER_STUDIO_ORIGIN.PART)
            // {
            //     annotation { "Name" : "Origin Placement" }
            //     definition.otherOriginPlacement is OTHER_ORIGIN_PLACEMENT;
            // }

            //_______________________________________________________________________________

            annotation { "Name" : "Offset x" }
            isLength(definition.dx, { (inch) : [-1e5, 0, 1e5] } as LengthBoundSpec);

            // annotation { "Name" : "Rotate around x axis", "UIHint" : UIHint.MATE_CONNECTOR_AXIS_TYPE, "Default" : AxisType.PLUS_X } //The default enum selection is PLUS_X
            // definition.rotateX is AxisType;

            //_______________________________________________________________________________

            annotation { "Name" : "Offset y" }
            isLength(definition.dy, { (inch) : [-1e5, 0, 1e5] } as LengthBoundSpec);

            // annotation { "Name" : "Rotate around y axis", "UIHint" : UIHint.MATE_CONNECTOR_AXIS_TYPE, "Default" : AxisType.PLUS_X } //The default enum selection is PLUS_X
            // definition.rotateY is AxisType;

            //_______________________________________________________________________________

            annotation { "Name" : "Offset z" }
            isLength(definition.dz, { (inch) : [-1e5, 0, 1e5] } as LengthBoundSpec);

            // annotation { "Name" : "Rotate around z axis", "UIHint" : UIHint.MATE_CONNECTOR_AXIS_TYPE, "Default" : AxisType.PLUS_X } //The default enum selection is PLUS_X
            // definition.rotateZ is AxisType;

            //_______________________________________________________________________________

        }
        //Flip & rotate buttons reference: https://cad.onshape.com/documents/12312312345abcabcabcdeff/w/a855e4161c814f2e9ab3698a/e/81869edac6224be29410b2a3
        //to have the flip arrow on the next row use this: "UIHint" : [UIHint.PRIMARY_AXIS, UIHint.FIRST_IN_ROW] }

        // annotation { "Name" : "Flip primary axis", "UIHint" : [UIHint.PRIMARY_AXIS, UIHint.FIRST_IN_ROW] }
        // definition.oppositeDirectionMateAxis is boolean;

        annotation { "Name" : "My Length" }
        isLength(definition.depth, LENGTH_BOUNDS);

        annotation { "Name" : "Opposite direction", "UIHint" : UIHint.OPPOSITE_DIRECTION }
        definition.shouldFlip is boolean;

        annotation { "Name" : "Angle" }
        isAngle(definition.angle, ANGLE_360_BOUNDS);

        annotation { "Name" : "Opposite direction", "UIHint" : UIHint.OPPOSITE_DIRECTION }
        definition.shouldFlipAngle is boolean;

        annotation { "Name" : "Locations", "Filter" : EntityType.VERTEX || BodyType.MATE_CONNECTOR }
        definition.locations is Query;

        annotation { "Name" : "Keep original", "Default" : true }
        definition.keepOriginal is boolean;

    }
    {
        const bodies = nodeTransformFunctionPascoe(id, context, definition);

        setProperty(context, {
                "entities" : bodies,
                "propertyType" : PropertyType.CUSTOM,
                "customPropertyId" : "test",
                "value" : "This is a test"
        });
    });

export function nodeTransformFunctionPascoe(id is Id, context is Context, definition)
{
    var bodies = {};
    var originPlane = plane(vector(definition.dx, definition.dy, definition.dz), XY_PLANE.normal, XY_PLANE.x);

    //___________________________________________________________________________________________________________
    //
    //                                         Prepare other studio bodies
    //___________________________________________________________________________________________________________

    var body;
    var otherStudioProfileOrigin = WORLD_COORD_SYSTEM;
    var nodeVector;
    var nodePlane;

    if (!definition.sameStudioBoolean)
    {
        const instantiator = newInstantiator(id + "inst", {});
        var queries = [];

        var q = addInstance(instantiator, definition.otherStudioBodyDef, { "configuration" : {}, "transform" : transform(vector(0, 0, 0) * inch) });
        queries = append(queries, q);
        q = append(queries, q);
        instantiate(context, instantiator);
        body = q[0];

    }

    // Initial user transform
    opTransform(context, id + "transform1", {
                "bodies" : body,
                "transform" : transform(vector(definition.dx, definition.dy, definition.dz))
            });

    // debug(context, body, DebugColor.BLACK);

    //___________________________________________________________________________________________________________
    //
    //                                          Bounding box and nodes
    //___________________________________________________________________________________________________________


    const origin = coordSystem(originPlane);

    const bbox = evBox3d(context, {
                "topology" : body,
                "tight" : true,
                "cSys" : origin
            });

    debug(context, bbox, DebugColor.YELLOW);

    const bboxCenter = box3dCenter(bbox);
    const bboxX = bbox.maxCorner[0] - bbox.minCorner[0];
    const bboxY = bbox.maxCorner[1] - bbox.minCorner[1];
    const bboxZ = bbox.maxCorner[2] - bbox.minCorner[2];
    const halfX = bboxX / 2;
    const halfY = bboxY / 2;
    const halfZ = bboxZ / 2;

    // The bounding box is divided into three layers, front, middle, back
    // Each layer has a point at each corner and mid point

    var node = {};

    node.frontCenter = vector(0 * inch, -halfY, 0 * inch);
    node.frontLeft = vector(-halfX, -halfY, 0 * inch);
    node.frontRight = vector(halfX, -halfY, 0 * inch);
    node.frontTop = vector(0 * inch, -halfY, halfZ);
    node.frontTopLeft = vector(-halfX, -halfY, halfZ);
    node.frontTopRight = vector(halfX, -halfY, halfZ);
    node.frontBottom = vector(0 * inch, -halfY, -halfZ);
    node.frontBottomLeft = vector(-halfX, -halfY, -halfZ);
    node.frontBottomRight = vector(halfX, -halfY, -halfZ);

    node.backCenter = vector(0 * inch, halfY, 0 * inch);
    node.backLeft = vector(-halfX, halfY, 0 * inch);
    node.backRight = vector(halfX, halfY, 0 * inch);
    node.backTop = vector(0 * inch, halfY, halfZ);
    node.backTopLeft = vector(-halfX, halfY, halfZ);
    node.backTopRight = vector(halfX, halfY, halfZ);
    node.backBottom = vector(0 * inch, halfY, -halfZ);
    node.backBottomLeft = vector(-halfX, halfY, -halfZ);
    node.backBottomRight = vector(halfX, halfY, -halfZ);

    node.middleCenter = vector(0 * inch, 0 * inch, 0 * inch);
    node.middleLeft = vector(-halfX, 0 * inch, 0 * inch);
    node.middleRight = vector(halfX, 0 * inch, 0 * inch);
    node.middleTop = vector(0 * inch, 0 * inch, halfZ);
    node.middleTopLeft = vector(-halfX, 0 * inch, halfZ);
    node.middleTopRight = vector(halfX, 0 * inch, halfZ);
    node.middleBottom = vector(0 * inch, 0 * inch, -halfZ);
    node.middleBottomLeft = vector(-halfX, 0 * inch, -halfZ);
    node.middleBottomRight = vector(halfX, 0 * inch, -halfZ);

    var nodeArray = [];
    for (var point in node)
    {
        // adds the center to the node location for correct positioning, then updates the nodeArray with this value
        nodeArray = append(nodeArray, bboxCenter + point.value);
    }

    var axisPlane = definition.axisPlane == 0 ? XY_PLANE : definition.axisPlane;
    const axisPlaneZ = vector(axisPlane.normal[0], axisPlane.normal[1], axisPlane.normal[2]) * inch;
    const axisPlaneX = vector(axisPlane.x[0], axisPlane.x[1], axisPlane.x[2]) * inch;
    const axisCoord = coordSystem(XY_PLANE.origin, axisPlaneX, axisPlaneZ);

    var pointManip = pointsManipulator({
            "points" : nodeArray,
            "index" : definition.pointIndex
        });

    //addManipulators(context, id, { "pointManip" : pointManip });

    // Gets the vector of the selected node
    nodeVector = pointManip.points[pointManip.index];
    nodePlane = plane(nodeVector, axisPlaneZ, axisPlaneX);
    originPlane = plane(nodeVector, originPlane.normal, originPlane.x);

    // var extrudeManipulator is Manipulator = linearManipulator({
    //         "base" : nodeVector,
    //         "direction" : XY_PLANE.normal,
    //         "offset" : definition.shouldFlip ? definition.depth : -definition.depth,
    //         "primaryParameterId" : "depth",
    //         "minValue" : -.5 * inch,
    //         "maxValue" : .5 * inch
    //     });

    var rotManipZ = angularManipulator({
            "axisOrigin" : nodeVector,
            "axisDirection" : axisCoord.zAxis,
            "rotationOrigin" : nodeVector + vector(1, 0, 0) * inch,
            "angle" : definition.shouldFlipAngle ? round(definition.angle, 90 * degree) : -round(definition.angle, 90 * degree),
            "primaryParameterId" : "angle"
        });

    addManipulators(context, id, {
                // "linearZ" : extrudeManipulator,
                "pointManip" : pointManip,
                "rotManipZ" : rotManipZ
            });

    //___________________________________________________________________________________________________________
    //
    //                                               Rotate buttons
    //___________________________________________________________________________________________________________

    //Flip & rotate buttons reference: https://cad.onshape.com/documents/12312312345abcabcabcdeff/w/a855e4161c814f2e9ab3698a/e/81869edac6224be29410b2a3

    const c2 = coordSystem(nodePlane); //definition.sameStudioBoolean ? evMateConnector(context, { "mateConnector" : definition.sameStudioBodyOriginMate }) : otherStudioProfileOrigin;
    var xAxis = c2.xAxis;
    var zAxis = c2.zAxis;
    var yAx = yAxis(c2);

    // const rotate = rotations(c2, definition.rotateX, definition.rotateY, definition.rotateZ);



    // Rotate body
    // opTransform(context, id + "rotX", {
    //             "bodies" : body,
    //             "transform" : rotationAround(line(c2.origin, xAxis), rotate.x)
    //         });

    // opTransform(context, id + "rotY", {
    //             "bodies" : body,
    //             "transform" : rotationAround(line(c2.origin, yAx), rotate.y)
    //         });

    // opTransform(context, id + "rotZ", {
    //             "bodies" : body,
    //             "transform" : rotationAround(line(c2.origin, zAxis), rotate.z)
    //         });


    opMateConnector(context, id + "mateConnector1", {
                "coordSystem" : c2,
                "owner" : evaluateQuery(context, body)[0]
            });



    originPlane = plane(nodeVector, originPlane.normal, originPlane.x);
    // originPlane = rotationAround(line(originPlane.origin, originPlane.x), rotate.x) * originPlane;
    // originPlane = rotationAround(line(originPlane.origin, yAxis(originPlane)), rotate.y) * originPlane;
    // originPlane = rotationAround(line(originPlane.origin, originPlane.normal), rotate.z) * originPlane;

    // Color axis
    // debug(context, line(originPlane.origin, originPlane.x), DebugColor.RED);
    // debug(context, line(originPlane.origin, yAxis(originPlane)), DebugColor.GREEN);
    // debug(context, line(originPlane.origin, originPlane.normal), DebugColor.BLUE);

    // debug(context, originPlane, DebugColor.MAGENTA);
    // debug(context, originPlane.x, DebugColor.MAGENTA);
    // debug(context, yAxis(originPlane), DebugColor.YELLOW);

    //___________________________________________________________________________________________________________
    //
    //                                           Pattern to locations
    //___________________________________________________________________________________________________________

    var transformMap = [];
    var transformIdMap = [];
    const locations = evaluateQuery(context, definition.locations);

    for (var i = 0; i < size(locations); i += 1)
    {
        var toPlane;
        const isVertex = size(evaluateQuery(context, qEntityFilter(locations[i], EntityType.VERTEX))) == 1;

        if (!isVertex)
        {

            const mateCoord = evMateConnector(context, {
                        "mateConnector" : locations[i]
                    });

            toPlane = plane(mateCoord);
        }
        else
        {
            const toPoint = evVertexPoint(context, {
                        "vertex" : locations[i]
                    });

            toPlane = plane(toPoint, XY_PLANE.normal, XY_PLANE.x);
        }

        transformMap = append(transformMap, transform(originPlane, toPlane));
        transformIdMap = append(transformIdMap, "copy" ~ i);
    }

    opPattern(context, id + "pattern1", {
                "entities" : body,
                "transforms" : transformMap,
                "instanceNames" : transformIdMap
            });
    bodies = qCreatedBy(id + "pattern1", EntityType.BODY);

    if (!definition.keepOriginal)
    {
        opDeleteBodies(context, id + "deleteBodies1", {
                    "entities" : body
                });
    }



    // for (var i = 0; i < size(transformToPlanesArray); i += 1)
    // {
    //     const thisProfile = qPatternInstances(id + "pattern1", "profile1" ~ i, EntityType.FACE);

    //     profiles = mergeMaps(profiles, { "profile" ~ i : thisProfile });

    //     surfacesToDelete = qUnion([surfacesToDelete, thisProfile]);
    // }

    return bodies;
}

function rotations(c2, rotX, rotY, rotZ)
{
    var rotationMap = {};

    // Rotate around X
    if (rotX == AxisType.PLUS_X)
    {
        rotationMap.x = 0 * degree;
    }
    else if (rotX == AxisType.PLUS_Y)
    {
        rotationMap.x = -90 * degree;
    }
    else if (rotX == AxisType.MINUS_X)
    {
        rotationMap.x = -180 * degree;
    }
    else if (rotX == AxisType.MINUS_Y)
    {
        rotationMap.x = -270 * degree;
    }

    //Rotate around Y
    if (rotY == AxisType.PLUS_X)
    {
        rotationMap.y = 0 * degree;
    }
    else if (rotY == AxisType.PLUS_Y)
    {
        rotationMap.y = -90 * degree;
    }
    else if (rotY == AxisType.MINUS_X)
    {
        rotationMap.y = -180 * degree;
    }
    else if (rotY == AxisType.MINUS_Y)
    {
        rotationMap.y = -270 * degree;
    }

    // Rotate around Z
    if (rotZ == AxisType.PLUS_X)
    {
        rotationMap.z = 0 * degree;
    }
    else if (rotZ == AxisType.PLUS_Y)
    {
        rotationMap.z = -90 * degree;
    }
    else if (rotZ == AxisType.MINUS_X)
    {
        rotationMap.z = -180 * degree;
    }
    else if (rotZ == AxisType.MINUS_Y)
    {
        rotationMap.z = -270 * degree;
    }

    return rotationMap;
}


export enum OTHER_STUDIO_ORIGIN
{
    annotation { "Name" : "Part origin" }
    PART,
    annotation { "Name" : "World origin" }
    WORLD,
}

export enum OTHER_ORIGIN_PLACEMENT
{
    annotation { "Name" : "Center" }
    CENTER,
    annotation { "Name" : "Bottom Left" }
    BOTTOM_LEFT,
    annotation { "Name" : "Top Left" }
    TOP_LEFT,
    annotation { "Name" : "Top Right" }
    TOP_RIGHT,
    annotation { "Name" : "Bottom Right" }
    BOTTOM_RIGHT,
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


export function manipulatorChange(context is Context, definition is map, newManipulators is map) returns map
{

    if (newManipulators["pointManip"] is map)
    {
        definition.pointIndex = newManipulators["pointManip"].index;
        definition.currentPoint = NODE_ID_ARRAY[newManipulators["pointManip"].index];
    }

    // if (newManipulators["linearZ"] is map)
    // {
    //     var newDepth is ValueWithUnits = newManipulators["linearZ"].offset;
    //     definition.depth = abs(newDepth);
    //     definition.shouldFlip = newDepth > 0;
    // }

    var axisPlane = definition.axisPlane == 0 ? XY_PLANE : definition.axisPlane;
    const axisPlaneZ = vector(axisPlane.normal[0], axisPlane.normal[1], axisPlane.normal[2]) * inch;
    const axisPlaneX = vector(axisPlane.x[0], axisPlane.x[1], axisPlane.x[2]) * inch;
    const axisPlaneOrigin = vector(axisPlane.origin[0], axisPlane.origin[1], axisPlane.origin[2]) * inch;

    if (newManipulators["rotManipZ"] is map)
    {
        var newAngle is ValueWithUnits = newManipulators["rotManipZ"].angle;
        definition.angle = abs(newAngle);
        definition.shouldFlipAngle = newAngle > 0;


        axisPlane = rotationAround(line(axisPlaneOrigin, axisPlaneZ), definition.angle) * axisPlane;
        definition.axisPlane = axisPlane;

    }

    //var points = createFreeSplinePointsList(context, definition);

    // var xArray = [];
    // var yArray = [];
    // var zArray = [];

    // for (var i = 0; i < size(points); i += 1)
    // {
    //     var point = points[i];
    //     xArray = append(xArray, point[0]);
    //     yArray = append(yArray, point[1]);
    //     zArray = append(zArray, point[2]);
    // }


    return definition;
}
