
//_______________________________________________________________________________________________________________________________________________
//
// This FeatureScript is owned by Michael Pascoe and is distributed by CADSharp LLC.
// You may not redistribute it for commercial purposes without the permission of said owner and CADSharp LLC. Copyright (c) 2023 Michael Pascoe.
//_______________________________________________________________________________________________________________________________________________


FeatureScript 1458;
import(path : "onshape/std/geometry.fs", version : "1458.0");

// CADSharp
export import(path : "cbeb3dcf671e00785597bd76/144bf6a7fdc989e9e28ce5ea/a75ab01def146a42f55baa7f", version : "dc78e9b85c9f16ea9e131d3f");

export import(path : "c7c08274a0d273b9a5f5b47d/9cc283975b51cc46c93878d1/dac5baeb73168dedecff52b9", version : "44ded57474bda15f5b3c3fbd");
icon::import(path : "128336cb1dd5ff2b8d05714d", version : "3a99347b82c2a261d113f0a7");

annotation {
        "Feature Type Name" : "Boolean composites",
        "Icon" : icon::BLOB_DATA,
        "Description Image" : cadsharpLogo::BLOB_DATA,
        "Feature Type Description" : "<b> Summary </b> <br> Boolean composite parts. <br>",
        "Editing Logic Function" : "editLogic" }
export const booleanComposites = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Boolean", "UIHint" : UIHint.HORIZONTAL_ENUM }
        definition.bool is Bool;

        annotation { "Name" : "Tools", "UIHint" : UIHint.ALLOW_QUERY_ORDER, "Filter" : BodyType.COMPOSITE || EntityType.BODY }
        definition.tools is Query;

        if (definition.bool != Bool.UNION)
        {
            annotation { "Name" : "Targets", "Filter" : BodyType.COMPOSITE || EntityType.BODY }
            definition.targets is Query;
        }

        annotation { "Name" : "Keep tools" }
        definition.keepTools is boolean;

        //_______________________________________________________________________
        //
        annotation { "Group Name" : "Suppress by logic", "Collapsed By Default" : true }
        {
            annotation { "Name" : "Suppress by logic (1 or 0)" }
            isReal(definition.suppressed, { (unitless) : [0, 1, 1] } as RealBoundSpec);

            annotation { "Name" : "Reverse logic", "Default" : false, "UIHint" : [UIHint.PRIMARY_AXIS, UIHint.DISPLAY_SHORT] }
            definition.reverseLogic is boolean;

            annotation { "Name" : "Status", "Default" : "Status: Unsuppressed", "UIHint" : [UIHint.READ_ONLY, UIHint.DISPLAY_SHORT] }
            definition.status is string;
        }
        //_______________________________________________________________________

        cadsharpUrlPredicate(definition);
    }
    {
        const suppressed = suppressByLogicFunctionPascoe(id, context, definition).bool;

        //Composite part reference: https://cad.onshape.com/documents/f2ea26b1b495cdf788cf4a7d/v/30412eadc6245d3e9059ca55/e/fef978324a54d5ac224c99dc?configuration=default
        if (!suppressed)
        {
            const compositeCheckTools = size(evaluateQuery(context, qBodyType(definition.tools, BodyType.COMPOSITE))) == 1;
            const compositeCheckTargets = size(evaluateQuery(context, qBodyType(definition.targets, BodyType.COMPOSITE))) == 1;
            var tools = compositeCheckTools ? qContainedInCompositeParts(definition.tools) : definition.tools;
            var targets = compositeCheckTargets ? qContainedInCompositeParts(definition.targets) : definition.targets;

            if (definition.bool == Bool.UNION)
            {
                const toolsArray = evaluateQuery(context, definition.tools);
                var orderedTools = qNothing();

                for (var i = 0; i < size(toolsArray); i += 1)
                {
                    var thisTool = toolsArray[i];

                    // If composite part, well that was frustrating...
                    if (evaluateQuery(context, qContainedInCompositeParts(thisTool))->size() != 0)
                    {
                        thisTool = qContainedInCompositeParts(thisTool);
                    }

                    orderedTools = qUnion([orderedTools, thisTool]);
                }

                opBoolean(context, id + "boolean1", {
                            "tools" : orderedTools,
                            "operationType" : BooleanOperationType.UNION,
                            "keepTools" : definition.keepTools == true ? true : false
                        });
            }
            else if (definition.bool == Bool.SUBTRACT)
            {
                opBoolean(context, id + "boolean1", {
                            "tools" : tools,
                            "targets" : targets,
                            "operationType" : BooleanOperationType.SUBTRACTION,
                            "keepTools" : definition.keepTools == true ? true : false
                        });
            }
            else
            {
                opBoolean(context, id + "boolean1", {
                            "tools" : tools,
                            "targets" : targets,
                            "operationType" : BooleanOperationType.SUBTRACT_COMPLEMENT,
                            "keepTools" : definition.keepTools == true ? true : false
                        });
            }
        }
    });

export enum Bool
{
    annotation { "Name" : "Union" }
    UNION,
    annotation { "Name" : "Subtract" }
    SUBTRACT,
    annotation { "Name" : "Intersect" }
    INTERSECT
}


export function editLogic(context is Context, id is Id, oldDefinition is map, definition is map, isCreating is boolean, specifiedParameters is map) returns map
{
    definition = cadsharpUrlFunctionForPreExistingEditLogic(oldDefinition, definition);

    definition.status = suppressByLogicFunctionPascoe(id, context, definition).status;

    return definition;
}

