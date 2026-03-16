
//_______________________________________________________________________________________________________________________________________________
//
// This FeatureScript is owned by Konstantin and is distributed by CADSharp LLC.
// You may not redistribute it for commercial purposes without the permission of said owner and CADSharp LLC. Copyright (c) 2023 Konstantin.
//_______________________________________________________________________________________________________________________________________________


FeatureScript 1364;
export import(path : "onshape/std/common.fs", version : "2180.0");

// CADSharp
export import(path : "cbeb3dcf671e00785597bd76/144bf6a7fdc989e9e28ce5ea/a75ab01def146a42f55baa7f", version : "dc78e9b85c9f16ea9e131d3f");

IconNamespace::import(path : "bb611484a330a94a5d513362", version : "96a4690fc60731b3a1941f1a");


/**
 * Performs a body, face, or feature pattern. Internally, performs
 * an [applyPattern], which in turn performs an [opPattern] or, for a feature
 * pattern, calls the feature function.
 *
 * @param id : @autocomplete `id + "circularPattern1"`
 * @param definition {{
 *      @field patternType {PatternType}: @optional
 *              Specifies a `PART`, `FEATURE`, or `FACE` pattern. Default is `PART`.
 *              @autocomplete `PatternType.PART`
 *      @field entities {Query}: @requiredif{`patternType` is `PART`}
 *              The parts to pattern.
 *              @eg `qCreatedBy(id + "extrude1", EntityType.BODY)`
 *      @field faces {Query}: @requiredif{`patternType` is `FACE`}
 *              The faces to pattern.
 *      @field instanceFunction {FeatureList}: @requiredif{`patternType` is `FEATURE`}
 *              The [FeatureList] of the features to pattern.
 *
 * }}
 */
annotation {
        "Feature Type Name" : "Transform pattern",
        "Filter Selector" : "allparts",
        "Icon" : IconNamespace::BLOB_DATA,
        "Feature Type Description" : "<b> Summary </b> <br> Pattern based on transforms. <br>",
        "Description Image" : cadsharpLogo::BLOB_DATA,
        "Editing Logic Function" : "cadsharpUrlEditLogic"
    }
export const transformPattern = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Pattern type" }
        definition.patternType is PatternType;

        if (definition.patternType == PatternType.PART)
        {
            booleanStepTypePredicate(definition);

            annotation { "Name" : "Entities to pattern", "Filter" : EntityType.BODY && AllowMeshGeometry.YES }
            definition.entities is Query;
        }
        else if (definition.patternType == PatternType.FACE)
        {
            annotation { "Name" : "Faces to pattern", "Filter" : EntityType.FACE && ConstructionObject.NO && SketchObject.NO && ModifiableEntityOnly.YES }
            definition.faces is Query;
        }
        else if (definition.patternType == PatternType.FEATURE)
        {
            annotation { "Name" : "Features to pattern" }
            definition.instanceFunction is FeatureList;
        }

        annotation { "Name" : "Reference point or mate connector", "Filter" : EntityType.VERTEX || BodyType.MATE_CONNECTOR, "MaxNumberOfPicks" : 1 }
        definition.refEntity is Query;

        annotation { "Name" : "Target points or mate connectors", "Filter" : EntityType.VERTEX || BodyType.MATE_CONNECTOR || BodyType.COMPOSITE }
        definition.targetEntities is Query;

        annotation { "Name" : "Keep orientation" }
        definition.keepOrientation is boolean;

        if (definition.patternType == PatternType.PART)
        {
            booleanStepScopePredicate(definition);
        }

        cadsharpUrlPredicate(definition);
    }
    {
        definition = adjustPatternDefinitionEntities(context, definition, false);
        definition = getVerticesFromComposites(context, definition);

        if (definition.patternType == PatternType.FEATURE)
            definition.fullFeaturePattern = true;

        const remainingTransform = getRemainderPatternTransform(context, { "references" : getReferencesForRemainderTransform(definition) });

        var transforms = [];
        var instanceNames = [];
        var i = 0;

        const refCS = getCoordSys(context, definition.refEntity, definition.keepOrientation);

        for (var targetEntity in evaluateQuery(context, definition.targetEntities))
        {
            const targetCS = getCoordSys(context, targetEntity, definition.keepOrientation);
            var instanceTransform = toWorld(targetCS) * fromWorld(refCS);
            transforms = append(transforms, instanceTransform);
            instanceNames = append(instanceNames, "" ~ i);
            i += 1;
        }

        definition.transforms = transforms;
        definition.instanceNames = instanceNames;
        definition.seed = definition.entities;


        applyPattern(context, id, definition, remainingTransform);

    }, { patternType : PatternType.PART, operationType : NewBodyOperationType.NEW });

function getCoordSys(context is Context, query is Query, keepOrientation is boolean) returns CoordSystem
{
    try silent
    {
        return evMateConnector(context, { "mateConnector" : query });
    }

    if (!keepOrientation)
    {
        try silent
        {
            const zAxis = evOwnerSketchPlane(context, { "entity" : query }).normal;
            const origin = evVertexPoint(context, { "vertex" : query });
            return coordSystem(origin, perpendicularVector(zAxis), zAxis);
        }
    }

    try silent
    {
        const origin = evVertexPoint(context, { "vertex" : query });
        return coordSystem(origin, vector(1, 0, 0), vector(0, 0, 1));
    }
}

function getVerticesFromComposites(context, definition)
{
    const targets = definition.targetEntities;
    const evTargets = evaluateQuery(context, targets);

    for (var i = 0; i < size(evTargets); i += 1)
    {
        const thisTarget = evTargets[i];
        const isComposite = !isQueryEmpty(context, qBodyType(thisTarget, BodyType.COMPOSITE));

        if (isComposite)
        {
            const parts = qContainedInCompositeParts(thisTarget);
            const vertices = qOwnedByBody(parts, EntityType.VERTEX);

            definition.targetEntities = qSubtraction(definition.targetEntities, thisTarget);
            definition.targetEntities = qUnion([definition.targetEntities, vertices]);
        }
    }

    return definition;
}

