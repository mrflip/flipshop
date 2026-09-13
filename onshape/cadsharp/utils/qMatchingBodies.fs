FeatureScript 2368;
import(path : "onshape/std/common.fs", version : "2368.0");


/**
 * Groups like bodies into queries within an array.
 *
 * @param definition {{
 *      @field entities {Query} : Entities to search.
 *      @field tolerance {number} : Relative tolerance for matching parts.
 * }}
 *
 * @returns {array} : Returns an array of queries.
 */
export function qMatchingBodies(context is Context, definition is map) returns array
{
    var entitiesArray = evaluateQuery(context, definition.entities);
    var clusterQueries = [];

    var clusters = clusterBodies(context, {
            "bodies" : definition.entities,
            "relativeTolerance" : definition.tolerance
        });


    for (var i = 0; i < size(clusters); i += 1)
    {
        var thisClusterQuery = qNothing();

        for (var k = 0; k < size(clusters[i]); k += 1)
        {
            var index = clusters[i][k];
            thisClusterQuery = qUnion([thisClusterQuery, entitiesArray[index]]);
        }

        clusterQueries = append(clusterQueries, thisClusterQuery);
    }

    return clusterQueries;
}

