
//_______________________________________________________________________________________________________________________________________________
//
// This FeatureScript is owned by Michael Pascoe and is distributed by CADSharp LLC.
// You may not redistribute it for commercial purposes without the permission of said owner and CADSharp LLC. Copyright (c) 2023 Michael Pascoe.
//_______________________________________________________________________________________________________________________________________________


FeatureScript 1777;
import(path : "onshape/std/geometry.fs", version : "1777.0");

// CADSharp
export import(path : "cbeb3dcf671e00785597bd76/409d65a3744fe434f32bdffc/a75ab01def146a42f55baa7f", version : "381046010d5aea697e433948");

icon::import(path : "0ee61af6de5335dcdd6f7a92", version : "3f92419b7143d0c9890cc1e1");

//Invisible character printable ASCII ->"      "

export enum SizeMethod
{
    annotation { "Name" : "Value" }
    VALUE,
    annotation { "Name" : "Up to entity" }
    UP_TO_ENTITY,
    // annotation { "Name" : "Between two entities" }
    // BETWEEN_TWO_ENTITIES
}

export enum Shape
{
    annotation { "Name" : "Polygon" }
    POLYGON,
    annotation { "Name" : "Cylinder" }
    CYLINDER,
    annotation { "Name" : "Sphere" }
    SPHERE,
    annotation { "Name" : "Cone" }
    CONE,
    annotation { "Name" : "Pyramid" }
    PYRAMID,
}

annotation {
        "Feature Type Name" : "Shape",
        "Icon" : icon::BLOB_DATA,
        "Feature Type Description" : "<br> <b>Summary</b> <br> Creates generic shapes.",
        "Description Image" : cadsharpLogo::BLOB_DATA,
        "Editing Logic Function" : "cadsharpUrlEditLogic"
    }
export const shape = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Shapes", "Item name" : "Shape", "Item label template" : "Shape" }
        definition.shapes is array;
        for (var shape in definition.shapes)
        {
            annotation { "Name" : "Shape", "UIHint" : [UIHint.REMEMBER_PREVIOUS_VALUE, UIHint.MATCH_LAST_ARRAY_ITEM] }
            shape.shape is Shape;

            annotation { "Name" : "Locations", "Filter" : BodyType.MATE_CONNECTOR }
            shape.locations is Query;

            annotation { "Name" : "Size input", "UIHint" : [UIHint.SHOW_LABEL, UIHint.REMEMBER_PREVIOUS_VALUE, UIHint.MATCH_LAST_ARRAY_ITEM] }
            shape.sizeMethod is SizeMethod;

            if (shape.sizeMethod == SizeMethod.VALUE)
            {
                annotation { "Name" : "Size" }
                isLength(shape.size, LENGTH_BOUNDS);
            }
            else if (shape.sizeMethod == SizeMethod.UP_TO_ENTITY)
            {
                annotation { "Name" : "Up to entity", "Filter" : EntityType.VERTEX || EntityType.EDGE || EntityType.FACE || EntityType.BODY, "MaxNumberOfPicks" : 1 }
                shape.upToEntity is Query;

                annotation { "Name" : "Offset" }
                isLength(shape.upToOffset, { (inch) : [-10000, 0, 10000] } as LengthBoundSpec);
            }

            if (shape.shape == Shape.POLYGON || shape.shape == Shape.PYRAMID)
            {
                annotation { "Name" : "Measure from inside", "UIHint" : UIHint.OPPOSITE_DIRECTION }
                shape.flipMeasureSide is boolean;
            }

            if (shape.shape != Shape.SPHERE)
            {
                annotation { "Name" : "Thickness" }
                isLength(shape.thickness, LENGTH_BOUNDS);
            }

            if (shape.shape == Shape.POLYGON || shape.shape == Shape.PYRAMID)
            {
                annotation { "Name" : "Sides" }
                isInteger(shape.sides, { (unitless) : [3, 6, 10000] } as IntegerBoundSpec);
            }
        }

        cadsharpUrlPredicate(definition);
    }
    {
        var count = 1;

        for (var k = 0; k < size(definition.shapes); k += 1)
        {
            count += 1;

            var def = definition.shapes[k];

            const evLocations = evaluateQuery(context, def.locations);

            for (var i = 0; i < size(evLocations); i += 1)
            {
                count += 1;

                try
                {
                    def.location = evLocations[i];

                    if (def.sizeMethod == SizeMethod.UP_TO_ENTITY)
                    {
                        def.size = (evDistance(context, {
                                                    "side0" : def.location,
                                                    "side1" : def.upToEntity
                                                }).distance + def.upToOffset) * 2;
                    }

                    shapePascoe(id + count, context, def.size, def.thickness, def.sides, def.location, def.shape, def.flipMeasureSide);
                }
            }
        }
    });

export function shapePascoe(id, context, size, thickness, sides, location, shape, flipMeasureSide)
{
    const localCsys = evMateConnector(context, {
                "mateConnector" : location
            });

    var part;
    var toDelete = qNothing();

    if (shape != Shape.SPHERE)
    {
        const localPlane = plane(localCsys);
        const sketch1 = newSketchOnPlane(context, id + "sketch1", {
                    "sketchPlane" : localPlane
                });

        if (shape == Shape.POLYGON || shape == Shape.PYRAMID)
        {
            const firstVertex = !flipMeasureSide ? vector(0 * inch, size / 2) : vector(0 * inch, size / sqrt(3));

            skRegularPolygon(sketch1, "polygon1", {
                        "center" : vector(0, 0) * inch,
                        "firstVertex" : firstVertex,
                        "sides" : sides
                    });
        }
        else
        {
            skCircle(sketch1, "circle1", {
                        "center" : vector(0, 0) * inch,
                        "radius" : size / 2
                    });
        }

        skSolve(sketch1);

        const profile = qCreatedBy(id + "sketch1", EntityType.FACE);
        toDelete = qUnion([toDelete, qCreatedBy(id + "sketch1", EntityType.EDGE)]);

        if (shape == Shape.POLYGON || shape == Shape.CYLINDER)
        {
            opExtrude(context, id + "extrude1", {
                        "entities" : profile,
                        "direction" : localPlane.normal,
                        "endBound" : BoundingType.BLIND,
                        "endDepth" : thickness
                    });

            part = qCreatedBy(id + "extrude1", EntityType.BODY);
        }
        else
        {
            opPoint(context, id + "point1", {
                        "point" : toWorld(localCsys, vector(0 * inch, 0 * inch, thickness))
                    });

            const loftPoint = qCreatedBy(id + "point1", EntityType.VERTEX);
            toDelete = qUnion([toDelete, loftPoint]);

            opLoft(context, id + "loft1", {
                        "profileSubqueries" : [profile, loftPoint]
                    });

            part = qCreatedBy(id + "loft1", EntityType.BODY);
        }
    }
    else if (shape == Shape.SPHERE)
    {
        opPoint(context, id + "originVertex", {
                    "point" : localCsys.origin
                });

        const originVertex = qCreatedBy(id + "originVertex", EntityType.VERTEX);
        toDelete = qUnion([toDelete, originVertex]);

        sphere(context, id + "sphere1", {
                    "center" : originVertex,
                    "radius" : size / 2
                });

        part = qCreatedBy(id + "sphere1", EntityType.BODY);
    }

    setProperty(context, {
                "entities" : part,
                "propertyType" : PropertyType.APPEARANCE,
                "value" : color(1, 1, 1)
            });

    opDeleteBodies(context, id + "deleteBodies1", {
                "entities" : toDelete
            });
}
