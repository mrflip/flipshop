//_______________________________________________________________________________________________________________________________________________
//
// This FeatureScript is owned by Michael Pascoe and is distributed by CADSharp LLC.
// You may not redistribute it for commercial purposes without the permission of said owner and CADSharp LLC. Copyright (c) 2025 Michael Pascoe.
//_______________________________________________________________________________________________________________________________________________

FeatureScript 2752;
import(path : "onshape/std/common.fs", version : "2752.0");
import(path : "12312312345abcabcabcdeff/afa81c655d65bb0d854a644e/2f3802712bd8620b3f97f4d3", version : "ee929dd8cd86f24173699e19");

icon::import(path : "8b742bc8b3f914a72f5e5169", version : "ee52e4419c1a1de8124276f3");

export import(path : "cbeb3dcf671e00785597bd76/409d65a3744fe434f32bdffc/a75ab01def146a42f55baa7f", version : "381046010d5aea697e433948");
export import(path : "c7c08274a0d273b9a5f5b47d/428044aea156e30f43a38edf/0abb9be049d15f1839c40841", version : "222ae82e6f1a0410eb2aa619");

export const suffix = "CADSharp";

annotation {
        "Feature Type Name" : "Set query attribute",
        "Icon" : icon::BLOB_DATA,
        "Feature Type Description" : "Assign queries to attributes for downstream use via query finder compatible features.",
        "Editing Logic Function" : "myEditLogic",
        "Feature Name Template" : "Set query ###name" }
export const setQueryFeature = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Name", "Default" : "mySelection" }
        definition.name is string;

        annotation { "Name" : "Description", "Default" : "" }
        definition.description is string;

        annotation { "Name" : "Convert to Query Variable" }
        definition.convertToQueryVariable is boolean;

        annotation { "Name" : "Entities", "Filter" : (EntityType.BODY || EntityType.FACE || EntityType.EDGE || EntityType.VERTEX) && (BodyType.SOLID || BodyType.SHEET || BodyType.WIRE || BodyType.POINT || BodyType.MATE_CONNECTOR || BodyType.COMPOSITE || ConstructionObject.YES || SketchObject.YES), "UIHint" : [UIHint.SHOW_CREATE_SELECTION, UIHint.PREVENT_CREATING_NEW_MATE_CONNECTORS] }
        definition.entities is Query;

        QFinderPredicate(definition, suffix, "Entities");

        // annotation { "Name" : "Store data as" }
        // definition.dataType is DataType;

        cadsharpUrlPredicate(definition);
    }
    {
        // Check for mate connector bodies
        const mateVertices = qBodyType(definition.entities, BodyType.MATE_CONNECTOR);
        const mateBodies = qOwnerBody(mateVertices);
        definition.entities = qSubtraction(definition.entities, mateVertices);
        definition.entities = qUnion([definition.entities, mateBodies]);

        var entities = qUnion([definition.entities, QFinderFunction(context, definition, suffix)]);

        entities = makeRobustQuery(context, entities);

        // const remainingTransform = getRemainderPatternTransform(context, {
        //             "references" : entities
        //         });

        if (!isQueryEmpty(context, entities))
        {
            setAttribute(context, {
                        "entities" : entities,
                        "name" : definition.name,
                        "attribute" : definition.name
                    });
        }


        if (definition.convertToQueryVariable)
        {
            setQueryVariable(context, definition.name, definition.description, entities);
        }
        else
        {
            setVariable(context, definition.name, definition.name, definition.description);
        }

        setFeatureComputedParameter(context, id, {
                    "name" : definition.name,
                    "value" : "name"
                });

        // transformResultIfNecessary(context, id, remainingTransform);
    });

export function myEditLogic(context is Context, id is Id, oldDefinition is map, definition is map, isCreating is boolean, specifiedParameters is map) returns map
{
    definition = cadsharpUrlFunctionForPreExistingEditLogic(oldDefinition, definition);

    definition = QFinderSetDefaultsAndVisibility(context, definition, oldDefinition, {
                "showExclude" : false,
                "showFilterEntities" : true,
                "showIncludeConstructionEntities" : true,
                "showIncludeSketchEntities" : true,
                "showWires" : true,
                "showVertices" : true,
                "showEdges" : true,
                "showSurfaces" : true,
                "showFaces" : true,
                "showSolids" : true,
                "showComposites" : true,
                "showMateConnectors" : true
            }, {
                "queryFinder" : false,
                "searchType" : SearchType.EVERYTHING,
                "filterEntities" : false,
                "includeConstructionEntities" : false,
                "includeSketchEntities" : false,
                "wires" : false,
                "vertices" : false,
                "edges" : false,
                "surfaces" : false,
                "faces" : false,
                "solids" : true,
                "composites" : false,
                "mateConnectors" : false
            }, suffix);

    return definition;
}
