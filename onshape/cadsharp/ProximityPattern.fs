FeatureScript 2878;
import(path : "onshape/std/common.fs", version : "2878.0");

export import(path : "cbeb3dcf671e00785597bd76/409d65a3744fe434f32bdffc/a75ab01def146a42f55baa7f", version : "381046010d5aea697e433948");
icon::import(path : "a76a995de447903cdc557313", version : "81abcba6a2088f9f1b7eed24");
QV::import(path : "12312312345abcabcabcdeff/30b599ee13557baf7abe062a/2f3802712bd8620b3f97f4d3", version : "74e4142ccc37b35a390b8e2c");

annotation {
        "Feature Type Name" : "Proximity pattern",
        "Icon" : icon::BLOB_DATA,
        "Feature Type Description" : "<br> <b>Summary</b> <br> Iterates through selected entities based on proximity to target. <br> <br>",
        "Description Image" : cadsharpLogo::BLOB_DATA,
        "Editing Logic Function" : "editLogic" }
export const proximityPattern = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Proximity origin", "Filter" : BodyType.MATE_CONNECTOR }
        definition.proximityOrigin is Query;

        annotation { "Name" : "Iterating counter name" }
        definition.iteratingCounterName is string;

        annotation { "Name" : "Initial query name" }
        definition.initialQueryName is string;

        annotation { "Name" : "Entities", "Filter" : EntityType.VERTEX || EntityType.EDGE || EntityType.FACE || EntityType.BODY }
        definition.entities is Query;

        annotation { "Name" : "Features to apply" }
        definition.featuresToApply is FeatureList;

        annotation { "Name" : "Reset count" }
        definition.resetCount is boolean;

        annotation { "Group Name" : "Reset count", "Driving Parameter" : "resetCount", "Collapsed By Default" : false }
        {
            if (definition.resetCount)
            {
                annotation { "Name" : "Starting count" } //, "UIHint" : UIHint.DISPLAY_SHORT }
                isInteger(definition.startingCount, { (unitless) : [0, 0, 10000] } as IntegerBoundSpec);
            }
        }

        cadsharpUrlPredicate(definition);
    }
    {
        const initialCounterValue = definition.resetCount ? definition.startingCount - 1 : getVariable(context, definition.iteratingCounterName);
        const entities = evaluateQuery(context, definition.entities);
        const seedQuery = QV::getQueryVariable(context, definition.initialQueryName);
        const proximityOrigin = evMateConnector(context, { "mateConnector" : definition.proximityOrigin });
        const sortedEntities = sortByDistance(context, definition.entities, proximityOrigin.origin);

        for (var i = 0; i < size(sortedEntities); i += 1)
        {
            const targetQuery = sortedEntities[i];
            QV::setQueryVariable(context, definition.initialQueryName, targetQuery);
            setVariable(context, definition.iteratingCounterName, initialCounterValue + i + 1);

            try
            {
                applyPattern(context, id + i + "pattern", {
                            "patternType" : PatternType.FEATURE,
                            "instanceFunction" : definition.featuresToApply,
                            "fullFeaturePattern" : true,
                            "transforms" : [identityTransform()],
                            "instanceNames" : ["instanceName"],
                            "sketchPatternInfo" : "Some sketch pattern info" //hidden parameter of new feature pattern
                        }, identityTransform());
            }
        }
    });

function sortByDistance(context is Context, entities is Query, targetVector is Vector)
{
    var evEntities = evaluateQuery(context, entities);

    const qty = size(evEntities);
    var sortedArray = [];

    for (var i = 0; i < qty; i += 1)
    {
        const closestEntity = qClosestTo(entities, targetVector)->qNthElement(0);
        sortedArray = append(sortedArray, closestEntity);
        entities = qSubtraction(entities, closestEntity);
    }

    return sortedArray;
}


export function editLogic(context is Context, id is Id, oldDefinition is map, definition is map, isCreating is boolean, specifiedParameters is map) returns map
{
    definition = cadsharpUrlFunctionForPreExistingEditLogic(oldDefinition, definition);



    return definition;
}

