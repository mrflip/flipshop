FeatureScript 347;
import(path : "onshape/std/geometry.fs", version : "347.0");

/**
 * Creates an  ANSI Type B, or a standard washer.
 *
 * Input is a point on sketch, which forms the center of the washer.
 *
 *
 *
 * Future Enhancements:
 *   -- detect correct diamter from input cylinder?
 *   --allow axis and intersecting plane instead of just a plane
 *
 * Copyright (c) Parametric Products Intellectual Holdings, LLC - All Rights Reserved
 *
 *
 * NOTICE:  All information contained herein is, and remains
 * the property of Parametric Products Intellectual Holdings, LLC ("PPIH") and its suppliers,
 * if any.  The intellectual and technical concepts contained
 * herein are proprietary to Parametric Products Intellectual Holdings, LLC
 * and its suppliers and may be covered by U.S. and Foreign Patents,
 * patents in process, and are protected by trade secret or copyright law.
 * Dissemination of this information or reproduction of this material
 * is strictly forbidden unless prior written permission is obtained
 * from Parametric Products Intellectual Holdings, LLC.
 *
 */

 //washerdata
import(path : "d9e449e07522374d3a449ca1/e97335d4bdf059d05c1bbafa/0dc001b527bdec2e78360668", version : "a47bf2c7f721ca8261183c59");


//debug
export import(path : "cc388262b3a9229cb95e75a7/d9904f855935a96ba6d7a5c4/00fafd10c20f72cf8b91a052", version : "e13aa8440e093caffa9bde93");

annotation { "Feature Type Name" : "ANSI Washer Type B" }
export const dcAnsiWasher = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "ANSI TYPE" }
        definition.ansiType is AnsiWasherType;

        annotation { "Name" : "Locations", "Filter" : EntityType.VERTEX && SketchObject.YES }
        definition.centerPoints is Query;
    }
    {
        var dims = lookupDims( definition.ansiType);


        definition.innerDiameter = dims[0] * inch;
        definition.outerDiameter = dims[1] * inch;
        definition.thickness = dims[2] * inch;
        _debug(context,"innerDiameter=" ,definition.innerDiameter );
        _debug(context,"outerDiameter=" ,definition.outerDiameter );
        _debug(context,"thickness=" ,definition.thickness );
        dcWasher(context, id, definition );

    }, {

    });


annotation { "Feature Type Name" : "Generic Washer" }
export const dcWasher = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Locations", "Filter" : EntityType.VERTEX && SketchObject.YES }
        definition.centerPoints is Query;

        // The minor radius of the torus
        annotation { "Name" : "Inner Diameter" }
        isLength(definition.innerDiameter, LENGTH_BOUNDS);

        // The major radius of the torus
        annotation { "Name" : "Outer Diameter" }
        isLength(definition.outerDiameter, LENGTH_BOUNDS);

        // The major radius of the torus
        annotation { "Name" : "Thickness" }
        isLength(definition.thickness, LENGTH_BOUNDS);
    }
    {
        if (  definition.innerDiameter.value > definition.outerDiameter.value ){

            //MISSING: ability to give a message in addition to flagging the fields!
            print("ERROR: Inner Diameter  must be smaller than Outer Diamter " );
            throw regenError(ErrorStringEnum.INVALID_INPUT,["innerDiameter","outerDiameter"]);
        }

        var centerPoints = evaluateQuery(context, definition.centerPoints);
        var counter = 1;
        for (var centerPoint in centerPoints)
        {
            //a plane is {origin, normal, x }
            //create a line with origin, direction
            var sketchPlane = evOwnerSketchPlane ( context, { entity: centerPoint } );
            var point is Vector = evVertexPoint(context, { "vertex" : centerPoint });
            createWasher(context,id + (  "_washer" ~ counter) , point, sketchPlane.normal, definition.innerDiameter, definition.outerDiameter, definition.thickness);
            counter += 1;
        }
    }, { /* default parameters */ });




/**
 *
 * Creates a washer located at the point, given the specified normal
 * TODO: move this to a separate file
 */
function createWasher(context, id, point, sketchPlaneNormal, innerDiameter, outerDiameter, thickness){

    //create sketch plane perpendicular to this one, and a sketch on it
    var profilePlane = plane(point,sketchPlaneNormal);
    var profileSketchId = id + "_profilesketch";
    _debug(context,"plane",profilePlane);
    _debug(context,"thickness=",thickness);
    _debug(context,"innerdiameter=",innerDiameter);
    _debug(context,"outerdiamter=",outerDiameter);
    _debug(context,"sketchPlaneNormal=",sketchPlaneNormal );
    var profileSketch = newSketchOnPlane(context,profileSketchId, { sketchPlane: profilePlane } );
    skCircle(profileSketch,"inner", { center: vector(0*meter ,0 * meter) , radius: innerDiameter / 2.0 } );
    skCircle(profileSketch,"outer", { center: vector(0*meter ,0 * meter) , radius: outerDiameter / 2.0 } );

    skSolve(profileSketch);

    var skEntities = qSketchRegion(profileSketchId);
    _debug(context,"entityQuery",skEntities);

    //extrude
    opExtrude(context,id + "washer",{
        direction :sketchPlaneNormal,
        entities: skEntities ,
        endDepth: thickness * meter,
        endBound : BoundingType.BLIND,
        startBound : BoundingType.BLIND,
        startDepth : 0 *meter
    });
    _debug(context,"Extrude Complete","");

    var deleteSketch = id + "delete_sketch";
    opDeleteBodies(context, deleteSketch, {
            "entities" : qCreatedBy(profileSketchId)
    });
}