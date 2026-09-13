FeatureScript 2625;
import(path : "onshape/std/common.fs", version : "2625.0");

export enum SearchType
{
    annotation { "Name" : "Everything" }
    EVERYTHING,
    annotation { "Name" : "Attribute" }
    ATTRIBUTE,
    annotation { "Name" : "Feature" }
    FEATURE,
    annotation { "Name" : "Property" }
    PROPERTY,
    annotation { "Name" : "Identity" }
    IDENTITY,
    annotation { "Name" : "Transient ID" }
    TRANSIENT_ID,
}

export predicate QFinderPredicate(definition is map, suffix is string)
{
    annotation { "Name" : "QFinder Cached Queries", "UIHint" : [UIHint.ALWAYS_HIDDEN, UIHint.UNCONFIGURABLE] }
    definition["qFinderCachedQueries" ~ suffix] is Query;

    annotation { "Name" : "defaultsHaveBeenSet", "Default" : false, "UIHint" : [UIHint.ALWAYS_HIDDEN, UIHint.UNCONFIGURABLE] }
    definition["defaultsHaveBeenSet" ~ suffix] is boolean;

    annotation { "Name" : "Query finder", "Default" : false, "UIHint" : UIHint.OPPOSITE_DIRECTION }
    definition["queryFinder" ~ suffix] is boolean;

    if (definition["queryFinder" ~ suffix])
    {
        annotation { "Group Name" : "Query finder", "Collapsed By Default" : false }
        {
            if (definition["queryFinder" ~ suffix])
            {
                annotation { "Name" : "Search", "Default" : SearchType.ATTRIBUTE, "UIHint" : UIHint.HORIZONTAL_ENUM }
                definition["searchType" ~ suffix] is SearchType;

                if (definition["searchType" ~ suffix] != SearchType.EVERYTHING)
                {
                    if (definition["searchType" ~ suffix] == SearchType.ATTRIBUTE)
                    {
                        annotation { "Name" : "Attribute", "Default" : "", "UIHint" : UIHint.FOCUS_ON_VISIBLE }
                        isAnything(definition["attribute" ~ suffix]);
                    }
                    else if (definition["searchType" ~ suffix] == SearchType.FEATURE)
                    {
                        annotation { "Name" : "Features", "UIHint" : UIHint.FOCUS_ON_VISIBLE }
                        definition["features" ~ suffix] is FeatureList;
                    }
                    else if (definition["searchType" ~ suffix] == SearchType.IDENTITY)
                    {
                        annotation { "Name" : "Identity", "Default" : "", "UIHint" : UIHint.FOCUS_ON_VISIBLE }
                        isAnything(definition["identity" ~ suffix]);
                    }
                    else if (definition["searchType" ~ suffix] == SearchType.TRANSIENT_ID)
                    {
                        annotation { "Name" : "Transient ID", "Default" : "", "UIHint" : UIHint.FOCUS_ON_VISIBLE }
                        isAnything(definition["transientId" ~ suffix]);
                    }
                    else if (definition["searchType" ~ suffix] == SearchType.PROPERTY)
                    {
                        annotation { "Name" : "(A) Property ID", "Default" : "", "UIHint" : UIHint.FOCUS_ON_VISIBLE }
                        isAnything(definition["propertyID" ~ suffix]);

                        annotation { "Name" : "(B) Property Value", "Default" : "" }
                        isAnything(definition["propertyValue" ~ suffix]);
                    }
                }

                annotation { "Name" : "Exclude", "Default" : false, "UIHint" : [UIHint.ALWAYS_HIDDEN, UIHint.UNCONFIGURABLE] }
                definition["showExclude" ~ suffix] is boolean;

                if (definition["showExclude" ~ suffix])
                {
                    annotation { "Name" : "Entities to exclude", "Filter" : EntityType.VERTEX || EntityType.EDGE || EntityType.FACE || EntityType.BODY } //EntityType.BODY && (BodyType.SOLID || BodyType.SHEET || BodyType.WIRE || BodyType.COMPOSITE) }
                    definition["exclude" ~ suffix] is Query;
                }

                annotation { "Name" : "Include construction entities", "Default" : false, "UIHint" : [UIHint.ALWAYS_HIDDEN, UIHint.UNCONFIGURABLE] }
                definition["showIncludeConstructionEntities" ~ suffix] is boolean;

                if (definition["showIncludeConstructionEntities" ~ suffix])
                {
                    annotation { "Name" : "Include construction entities", "Default" : false }
                    definition["includeConstructionEntities" ~ suffix] is boolean;
                }

                annotation { "Name" : "Include sketch entities", "Default" : false, "UIHint" : [UIHint.ALWAYS_HIDDEN, UIHint.UNCONFIGURABLE] }
                definition["showIncludeSketchEntities" ~ suffix] is boolean;

                if (definition["showIncludeSketchEntities" ~ suffix])
                {
                    annotation { "Name" : "Include sketch entities", "Default" : false }
                    definition["includeSketchEntities" ~ suffix] is boolean;
                }

                annotation { "Name" : "Wires", "Default" : false, "UIHint" : [UIHint.ALWAYS_HIDDEN, UIHint.UNCONFIGURABLE] }
                definition["showWires" ~ suffix] is boolean;

                if (definition["showWires" ~ suffix])
                {
                    annotation { "Name" : "Wires", "Default" : false }
                    definition["wires" ~ suffix] is boolean;
                }

                annotation { "Name" : "Edges", "Default" : false, "UIHint" : [UIHint.ALWAYS_HIDDEN, UIHint.UNCONFIGURABLE] }
                definition["showEdges" ~ suffix] is boolean;

                if (definition["showEdges" ~ suffix])
                {
                    annotation { "Name" : "Edges", "Default" : false }
                    definition["edges" ~ suffix] is boolean;
                }

                annotation { "Name" : "Surfaces", "Default" : false, "UIHint" : [UIHint.ALWAYS_HIDDEN, UIHint.UNCONFIGURABLE] }
                definition["showSurfaces" ~ suffix] is boolean;

                if (definition["showSurfaces" ~ suffix])
                {
                    annotation { "Name" : "Surfaces", "Default" : false }
                    definition["surfaces" ~ suffix] is boolean;
                }

                annotation { "Name" : "Faces", "Default" : false, "UIHint" : [UIHint.ALWAYS_HIDDEN, UIHint.UNCONFIGURABLE] }
                definition["showFaces" ~ suffix] is boolean;

                if (definition["showFaces" ~ suffix])
                {
                    annotation { "Name" : "Faces", "Default" : false }
                    definition["faces" ~ suffix] is boolean;
                }

                annotation { "Name" : "Solids", "Default" : false, "UIHint" : [UIHint.ALWAYS_HIDDEN, UIHint.UNCONFIGURABLE] }
                definition["showSolids" ~ suffix] is boolean;

                if (definition["showSolids" ~ suffix])
                {
                    annotation { "Name" : "Solids", "Default" : false }
                    definition["solids" ~ suffix] is boolean;
                }

                annotation { "Name" : "Composites", "Default" : false, "UIHint" : [UIHint.ALWAYS_HIDDEN, UIHint.UNCONFIGURABLE] }
                definition["showComposites" ~ suffix] is boolean;

                if (definition["showComposites" ~ suffix])
                {
                    annotation { "Name" : "Composites", "Default" : false }
                    definition["composites" ~ suffix] is boolean;
                }

                annotation { "Name" : "Mate connectors", "Default" : false, "UIHint" : [UIHint.ALWAYS_HIDDEN, UIHint.UNCONFIGURABLE] }
                definition["showMateConnectors" ~ suffix] is boolean;

                if (definition["showMateConnectors" ~ suffix])
                {
                    annotation { "Name" : "Mate connectors", "Default" : false }
                    definition["mateConnectors" ~ suffix] is boolean;
                }
            }
        }
    }
}

export function QFinderFunction(context is Context, definition is map, suffix is string) returns Query
{
    var searchQuery = definition["searchType" ~ suffix] == SearchType.EVERYTHING ? qEverything() : qNothing();
    const all = qEverything();
    var everything = qNothing();
    var toQuery = qNothing();

    if (definition["queryFinder" ~ suffix])
    {
        if (definition["searchType" ~ suffix] != SearchType.EVERYTHING)
        {
            if (definition["searchType" ~ suffix] == SearchType.ATTRIBUTE)
            {
                searchQuery = qUnion([searchQuery, qHasAttribute(definition["attribute" ~ suffix])]);
            }
            else if (definition["searchType" ~ suffix] == SearchType.FEATURE)
            {
                const featureEntities = qCreatedBy(definition["features" ~ suffix]);
                searchQuery = qUnion([searchQuery, featureEntities]);
            }
            else if (definition["searchType" ~ suffix] == SearchType.IDENTITY)
            {
                searchQuery = qUnion([searchQuery, qNamed(definition["identity" ~ suffix])]);
            }
            else if (definition["searchType" ~ suffix] == SearchType.TRANSIENT_ID)
            {
                const transientQuery = definition["transientId" ~ suffix] as Query;
                searchQuery = qUnion([searchQuery, transientQuery]);
            }
            else if (definition["searchType" ~ suffix] == SearchType.PROPERTY)
            {
                searchQuery = qUnion([searchQuery, definition["qFinderCachedQueries" ~ suffix]]);
            }
        }

        if (!definition["includeConstructionEntities" ~ suffix])
            searchQuery = qSubtraction(searchQuery, qConstructionFilter(
                    qSubtraction(searchQuery, qSketchFilter(searchQuery, SketchObject.YES)), // Remove sketches from subtraction so that we can manually remove them later if requested
                    ConstructionObject.YES));

        if (!definition["includeSketchEntities" ~ suffix])
            searchQuery = qSubtraction(searchQuery, qSketchFilter(searchQuery, SketchObject.YES));

        if (isQueryEmpty(context, searchQuery))
            return searchQuery;

        if (definition["wires" ~ suffix])
        {
            everything = qUnion([everything, qAllWires(qEntityFilter(searchQuery, EntityType.BODY))]);
        }
        if (definition["edges" ~ suffix])
        {
            everything = qUnion([everything, qEntityFilter(searchQuery, EntityType.EDGE)]);
        }
        if (definition["surfaces" ~ suffix])
        {
            everything = qUnion([everything, qAllSurfaces(qEntityFilter(searchQuery, EntityType.BODY))]);
        }
        if (definition["faces" ~ suffix])
        {
            everything = qUnion([everything, qEntityFilter(searchQuery, EntityType.FACE)]);
        }
        if (definition["solids" ~ suffix])
        {
            everything = qUnion([everything, qBodyType(qEntityFilter(searchQuery, EntityType.BODY), BodyType.SOLID)]);
        }
        if (definition["composites" ~ suffix])
        {
            everything = qUnion([everything, qAllComposites(qEntityFilter(searchQuery, EntityType.BODY))]);
        }
        if (definition["mateConnectors" ~ suffix])
        {
            everything = qUnion([everything, qBodyType(qEntityFilter(searchQuery, EntityType.BODY), BodyType.MATE_CONNECTOR)]);
        }

        toQuery = qUnion([toQuery, everything]);
    }

    toQuery = qSubtraction(toQuery, definition["exclude" ~ suffix]);

    if (!isQueryEmpty(context, toQuery))
        addDebugEntities(context, toQuery, DebugColor.CYAN);

    if (!isQueryEmpty(context, definition["exclude" ~ suffix]))
    {
        addDebugEntities(context, definition["exclude" ~ suffix], DebugColor.RED);
        toQuery = qSubtraction(toQuery, definition["exclude" ~ suffix]);
    }

    return toQuery;
}

export function qAllSurfaces(query is Query)
{
    var surfaces = qBodyType(query, BodyType.SHEET);
    surfaces = qSubtraction(surfaces, qConstructionFilter(surfaces, ConstructionObject.YES));
    surfaces = qSubtraction(surfaces, qSketchFilter(surfaces, SketchObject.YES));

    return surfaces;
}

export function qAllWires(query is Query)
{
    var surfaces = qBodyType(query, BodyType.WIRE);
    surfaces = qSubtraction(surfaces, qConstructionFilter(surfaces, ConstructionObject.YES));
    surfaces = qSubtraction(surfaces, qSketchFilter(surfaces, SketchObject.YES));

    return surfaces;
}

export function qAllComposites(query)
{
    var surfaces = qBodyType(query, BodyType.COMPOSITE);

    return surfaces;
}


/**
 * (For use within edit logic) A function for setting defaults and controlling visibility of the query finder inputs.
 *
 * @param context {Context} : The current studio context.
 * @param definition {map} : The current definition.
 * @param qFinderVisibleInputs {{
 *      @field showExclude {boolean} : Whether exclude inputs are visible. Default `true`. @eg `true`
 *      @field showIncludeConstructionEntities {boolean} : Whether include construction entity inputs inputs are visible. Default `false`. @eg `false`
 *      @field showIncludeSketchEntities {boolean} : Whether include sketch entitiy inputs inputs are visible. Default `false`. @eg `false`
 *      @field showWires {boolean} : Whether wire inputs inputs are visible. Default `false`. @eg `false`
 *      @field showEdges {boolean} : Whether edge inputs are visible. Default `false`. @eg `false`
 *      @field showSurfaces {boolean} : Whether surface inputs are visible. Default `false`. @eg `false`
 *      @field showFaces {boolean} : Whether face inputs are visible. Default `false`. @eg `false`
 *      @field showSolids {boolean} : Whether solid inputs are visible. Default `false`. @eg `false`
 *      @field showComposites {boolean} : Whether composite inputs are visible. Default `false`. @eg `false`
 *      @field showMateConnectors {boolean} : Whether mate connector inputs are visible. Default `false`. @eg `false`
 * }}
 * @param qFinderDefaultInputs {{
 *      @field queryFinder {boolean} : Default value for whether the qFinder. Default `false`. @eg `false`
 *      @field searchType {SearchType} : Default value for the search type. Default `SearchType.EVERYTHING`. @eg `SearchType.EVERYTHING`
 *      @field includeConstructionEntities {boolean} : Default value for include construction entity inputs. Default `false`. @eg `false`
 *      @field includeSketchEntities {boolean} : Default value for include sketch entitiy inputs. Default `false`. @eg `false`
 *      @field wires {boolean} : Default value for wire inputs. Default `false`. @eg `false`
 *      @field edges {boolean} : Default value for edge. Default `false`. @eg `false`
 *      @field surfaces {boolean} : Default value for surface. Default `false`. @eg `false`
 *      @field faces {boolean} : Default value for face. Default `false`. @eg `false`
 *      @field solids {boolean} : Default value for solid. Default `false`. @eg `false`
 *      @field composites {boolean} : Default value for composite. Default `false`. @eg `false`
 *      @field mateConnectors {boolean} : Default value for composite. Default `false`. @eg `false`
 * }}
 *
 * @return {{}}
 */
export function QFinderSetDefaultsAndVisibility(context is Context, definition is map, oldDefinition is map, qFinderVisibleInputs is map, qFinderDefaultInputs, suffix is string) returns map
{
    if (!definition["defaultsHaveBeenSet" ~ suffix] || definition["queryFinder" ~ suffix] != oldDefinition["queryFinder" ~ suffix])
    {
        definition["attribute" ~ suffix] = "";
        definition["identity" ~ suffix] = "";
        definition["transientId" ~ suffix] = "";
        definition["propertyID" ~ suffix] = "";
        definition["propertyValue" ~ suffix] = "";

        definition["showExclude" ~ suffix] = qFinderVisibleInputs.showExclude;
        definition["showIncludeConstructionEntities" ~ suffix] = qFinderVisibleInputs.showIncludeConstructionEntities;
        definition["showIncludeSketchEntities" ~ suffix] = qFinderVisibleInputs.showIncludeSketchEntities;
        definition["showWires" ~ suffix] = qFinderVisibleInputs.showWires;
        definition["showEdges" ~ suffix] = qFinderVisibleInputs.showEdges;
        definition["showSurfaces" ~ suffix] = qFinderVisibleInputs.showSurfaces;
        definition["showFaces" ~ suffix] = qFinderVisibleInputs.showFaces;
        definition["showSolids" ~ suffix] = qFinderVisibleInputs.showSolids;
        definition["showComposites" ~ suffix] = qFinderVisibleInputs.showComposites;
        definition["showMateConnectors" ~ suffix] = qFinderVisibleInputs.showMateConnectors;

        if (!definition["defaultsHaveBeenSet" ~ suffix])
            definition["queryFinder" ~ suffix] = qFinderDefaultInputs.queryFinder;

        definition["searchType" ~ suffix] = qFinderDefaultInputs.searchType;
        definition["includeConstructionEntities" ~ suffix] = qFinderDefaultInputs.includeConstructionEntities;
        definition["includeSketchEntities" ~ suffix] = qFinderDefaultInputs.includeSketchEntities;
        definition["wires" ~ suffix] = qFinderDefaultInputs.wires;
        definition["edges" ~ suffix] = qFinderDefaultInputs.edges;
        definition["surfaces" ~ suffix] = qFinderDefaultInputs.surfaces;
        definition["faces" ~ suffix] = qFinderDefaultInputs.faces;
        definition["solids" ~ suffix] = qFinderDefaultInputs.solids;
        definition["composites" ~ suffix] = qFinderDefaultInputs.composites;
        definition["mateConnectors" ~ suffix] = qFinderDefaultInputs.mateConnectors;

        definition["defaultsHaveBeenSet" ~ suffix] = true;
    }

    // Handle get property since it can only be handled within edit logic or tables
    if (!isUndefinedOrEmptyString(definition["propertyID" ~ suffix]))
    {
        const entities = qEverything();
        const evEntities = evaluateQuery(context, entities);
        const qty = size(evEntities);

        for (var i = 0; i < qty; i += 1)
        {
            try silent
            {
                const entity = evEntities[i];

                const propValue = getProperty(context, {
                            "entity" : entity,
                            "customPropertyId" : definition["propertyID" ~ suffix],
                            "propertyType" : PropertyType.CUSTOM
                        });

                if (propValue == definition["propertyValue" ~ suffix])
                {
                    definition["qFinderCachedQueries" ~ suffix] = qUnion([definition["qFinderCachedQueries" ~ suffix], entity]);
                }
            }
        }
    }

    return definition;
}

