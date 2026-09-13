//_______________________________________________________________________________________________________________________________________________
//
// This FeatureScript is owned by Michael Pascoe and is distributed by CADSharp LLC.
// You may not redistribute it for commercial purposes without the permission of said owner and CADSharp LLC. Copyright (c) 2023 Michael Pascoe.
//_______________________________________________________________________________________________________________________________________________

FeatureScript 2180;
import(path : "onshape/std/common.fs", version : "2180.0");
icon::import(path : "8f497b8002155949a8edb31c", version : "f15134a863b72095d00f1d79");

// CADSharp
export import(path : "cbeb3dcf671e00785597bd76/409d65a3744fe434f32bdffc/a75ab01def146a42f55baa7f", version : "381046010d5aea697e433948");

export enum EvaluationType
{
    annotation { "Name" : "Angle" }
    ANGLE,
    annotation { "Name" : "Distance" }
    DISTANCE,
    annotation { "Name" : "Volume" }
    VOLUME,
    annotation { "Name" : "Area" }
    AREA,
    annotation { "Name" : "Touching" }
    TOUCHING,
    annotation { "Name" : "Centroid position" }
    CENTROID_POSITION,
    annotation { "Name" : "Boolean expression" }
    BOOL_EXPRESSION,
}

export enum CompareSymbol
{
    annotation { "Name" : "<" }
    LESS_THAN,
    annotation { "Name" : "<=" }
    LESS_THAN_OR_EQUAL,
    annotation { "Name" : "=" }
    EQUAL,
    annotation { "Name" : "!=" }
    NOT_EQUAL,
    annotation { "Name" : ">=" }
    GREATER_THAN_OR_EQUAL,
    annotation { "Name" : ">" }
    GREATER_THAN,
}

annotation {
        "Feature Type Name" : "Validator",
        "Icon" : icon::BLOB_DATA,
        "Feature Name Template" : "#name #d",
        "Editing Logic Function" : "editLogic",
        "Feature Type Description" : "<br> <b>Summary</b> <br> Returns a true or false variable based on the specified conditions.",
        "Description Image" : cadsharpLogo::BLOB_DATA,
    }
export const validator = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "isValid", "UIHint" : UIHint.ALWAYS_HIDDEN }
        definition.isValid is boolean;

        annotation { "Name" : "name", "Default" : "Validator_", "UIHint" : UIHint.ALWAYS_HIDDEN }
        definition.name is string;

        annotation { "Name" : "Name", "Default" : "ev" }
        definition.suffix is string;

        annotation { "Name" : "Evaluation type", "UIHint" : UIHint.SHOW_LABEL }
        definition.evalType is EvaluationType;

        if (definition.evalType != EvaluationType.BOOL_EXPRESSION)
        {
            annotation { "Name" : "Entity A", "Filter" : EntityType.FACE || EntityType.BODY || EntityType.VERTEX || EntityType.EDGE, "MaxNumberOfPicks" : 1 }
            definition.entityA is Query;

            annotation { "Name" : "Compare symbol" }
            definition.compareSymbol is CompareSymbol;

            annotation { "Name" : "Entity B", "Filter" : EntityType.FACE || EntityType.BODY || EntityType.VERTEX || EntityType.EDGE, "MaxNumberOfPicks" : 1 }
            definition.entityB is Query;
        }
        else
        {
            annotation { "Name" : "Expression" }
            definition.boolExpression is boolean;
        }

        annotation { "Name" : "Turn feature red if not valid" }
        definition.red is boolean;

        annotation { "Name" : "Description" }
        definition.description is string;

        cadsharpUrlPredicate(definition);
    }
    {
        const isValid is boolean = checkValidity(context, id, definition);

        if (definition.evalType != EvaluationType.BOOL_EXPRESSION)
        {
            addDebugEntities(context, qUnion([definition.entityA, definition.entityB]), isValid ? DebugColor.GREEN : DebugColor.RED);
        }

        if (definition.red && !isValid)
        {
            reportFeatureWarning(context, id, length(definition.description) == 0 ? "Validation failed" : "Validation failed: " ~ definition.description);
        }

        setVariable(context, definition.name ~ definition.suffix, isValid);

        setFeatureComputedParameter(context, id, { "name" : "d", "value" : definition.suffix ~ "_" ~ isValid->toString() });
    });

/**
 * Compares two values with units using a CompareSymbol.
 */
function compare(valueA is ValueWithUnits, valueB is ValueWithUnits, symbol is CompareSymbol) returns boolean
{
    if (symbol == CompareSymbol.LESS_THAN)
        return valueA < valueB;
    if (symbol == CompareSymbol.LESS_THAN_OR_EQUAL)
        return valueA <= valueB;
    if (symbol == CompareSymbol.EQUAL)
        return tolerantEquals(valueA, valueB);
    if (symbol == CompareSymbol.NOT_EQUAL)
        return !tolerantEquals(valueA, valueB);
    if (symbol == CompareSymbol.GREATER_THAN_OR_EQUAL)
        return valueA >= valueB;
    if (symbol == CompareSymbol.GREATER_THAN)
        return valueA > valueB;
    return false; // Default case
}

function checkValidity(context is Context, id is Id, definition is map) returns boolean
{

    if (definition.evalType == EvaluationType.BOOL_EXPRESSION)
    {
        return definition.boolExpression;
    }

    // Ensure entities are selected before attempting to evaluate
    if (isQueryEmpty(context, definition.entityA) || isQueryEmpty(context, definition.entityB))
        return false;

    var isValid = false;

    if (definition.evalType == EvaluationType.VOLUME)
    {
        const volumeA = evVolume(context, { "entities" : definition.entityA });
        const volumeB = evVolume(context, { "entities" : definition.entityB });
        isValid = compare(volumeA, volumeB, definition.compareSymbol);
    }
    else if (definition.evalType == EvaluationType.AREA)
    {
        const areaA = evArea(context, { "entities" : definition.entityA });
        const areaB = evArea(context, { "entities" : definition.entityB });
        isValid = compare(areaA, areaB, definition.compareSymbol);
    }
    else if (definition.evalType == EvaluationType.ANGLE)
    {
        // For ANGLE, the measurement is between the two selected entities.
        // NOTE: This implementation compares the resulting angle to zero, similar to a parallel check.
        // For full modularity, you would need to add a parameter for the angle to compare against.
        try
        {
            const dirA = extractDirection(context, definition.entityA);
            const dirB = extractDirection(context, definition.entityB);
            const angle = angleBetween(dirA, dirB);
            isValid = compare(angle, 0 * radian, definition.compareSymbol);
        }
    }
    else if (definition.evalType == EvaluationType.DISTANCE)
    {
        // For DISTANCE, the measurement is between the two selected entities.
        // NOTE: This implementation compares the resulting distance to zero.
        // For full modularity, you would need to add a parameter for the distance to compare against.
        const distance = measureDistance(context, { "entities" : qUnion([definition.entityA, definition.entityB]) }).distance;
        isValid = compare(distance, 0 * meter, definition.compareSymbol);
    }
    else if (definition.evalType == EvaluationType.TOUCHING)
    {
        // This is a special case of DISTANCE <= 0 and ignores the selected CompareSymbol.
        const distance = measureDistance(context, { "entities" : qUnion([definition.entityA, definition.entityB]) }).distance;
        isValid = (distance <= 0 * meter);
    }
    else if (definition.evalType == EvaluationType.CENTROID_POSITION)
    {
        // Comparison operators like < or > are not defined for vector positions.
        // This logic only handles equality and inequality.
        const centroidA = evApproximateCentroid(context, { "entities" : definition.entityA });
        const centroidB = evApproximateCentroid(context, { "entities" : definition.entityB });

        if (definition.compareSymbol == CompareSymbol.EQUAL)
            isValid = tolerantEquals(centroidA, centroidB);
        else if (definition.compareSymbol == CompareSymbol.NOT_EQUAL)
            isValid = !tolerantEquals(centroidA, centroidB);
        // For other compare symbols, isValid remains false.
    }

    return isValid;
}

export function editLogic(context is Context, id is Id, oldDefinition is map, definition is map, isCreating is boolean, specifiedParameters is map) returns map
{
    definition = cadsharpUrlFunctionForPreExistingEditLogic(oldDefinition, definition);

    definition.isValid = checkValidity(context, id, definition);

    return definition;
}
