FeatureScript 2878;
import(path : "onshape/std/common.fs", version : "2878.0");

export import(path : "cbeb3dcf671e00785597bd76/409d65a3744fe434f32bdffc/a75ab01def146a42f55baa7f", version : "381046010d5aea697e433948");
icon::import(path : "73914d313766a0019052d713", version : "23394a73b1885556b65452ac");

annotation {
        "Feature Type Name" : "Capture faces",
        "Icon" : icon::BLOB_DATA,
        "Feature Type Description" : "<br> <b>Summary</b> <br> Copy faces with desired spread for performance boosts when representing complex parts like engines. <br> Works best when exporting faces then re-importing without original overhead.<br>",
        "Description Image" : cadsharpLogo::BLOB_DATA,
        "Editing Logic Function" : "editLogic" }
export const proximityPattern = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Faces", "Item name" : "Face", "UIHint" : UIHint.COLLAPSE_ARRAY_ITEMS,
                    "Driven query" : "entities", "Item label template" : "#entities" }
        definition.myWidgets is array;
        for (var widget in definition.myWidgets)
        {
            annotation { "Name" : "Entities", "Filter" : EntityType.FACE }
            widget.entities is Query;
        }

        annotation { "Name" : "Spread", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
        isInteger(definition.spread, { (unitless) : [0, 5, 10000] } as IntegerBoundSpec);

        annotation { "Name" : "Composite", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
        definition.composite is boolean;

        annotation { "Name" : "Delete everything", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
        definition.deleteEverything is boolean;

        annotation { "Name" : "Suppress" }
        definition.suppress is boolean;

        cadsharpUrlPredicate(definition);
    }
    {
        if (definition.suppress)
            return;

        var toCapture = qNothing();

        for (var data in definition.myWidgets)
        {
            var currentLayer = data.entities;
            toCapture = qUnion([toCapture, currentLayer]);

            for (var i = 0; i < definition.spread; i += 1)
            {
                var nextLayer = qAdjacent(currentLayer, AdjacencyType.EDGE, EntityType.FACE);
                nextLayer = qSubtraction(nextLayer, qUnion([toCapture, data.entities]));
                toCapture = qUnion([toCapture, nextLayer]);

                currentLayer = nextLayer;

                // Safety check: if no new faces are found, stop searching
                if (evaluateQuery(context, currentLayer) == [])
                    break;
            }
        }

        for (var i = 0; i < 3; i += 1)
            addDebugEntities(context, toCapture, DebugColor.CYAN);

        opExtractSurface(context, id + "extractFace", {
                    "faces" : toCapture,
                    "offset" : 0 * inch });

        const createdBodies = qCreatedBy(id + "extractFace", EntityType.BODY);
        var compositePart = qNothing();

        if (definition.composite)
        {
            opCreateCompositePart(context, id + "compositePart1", {
                        "bodies" : createdBodies,
                        "closed" : true
                    });

            compositePart = qCreatedBy(id + "compositePart1", EntityType.BODY);
        }

        if (definition.deleteEverything)
        {
            opDeleteBodies(context, id + "deleteBodies1", {
                        "entities" : qSubtraction(qEverything(EntityType.BODY), qUnion([createdBodies, compositePart]))
                    });
        }

        setProperty(context, {
                    "entities" : qUnion([createdBodies, compositePart]),
                    "propertyType" : PropertyType.APPEARANCE,
                    "value" : color(0, 1, 1)
                });
    });

export function editLogic(context is Context, id is Id, oldDefinition is map, definition is map, isCreating is boolean, specifiedParameters is map) returns map
{
    definition = cadsharpUrlFunctionForPreExistingEditLogic(oldDefinition, definition);



    return definition;
}

