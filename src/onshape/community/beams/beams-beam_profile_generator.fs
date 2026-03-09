FeatureScript 1096;
import(path : "onshape/std/common.fs", version : "1096.0");

/**
 * IMPORTANT NOTE TO USERS:
 *
 * The beams feature now has a custom profile option which allows using your own sketched profiles, from
 * any Onshape document, with the original beam feature.
 *
 * The new, preferred workflow for using custom profiles is simply:
 * 1. Create a (possibly configured) Part Studio including a sketch of the beam profile on the Top Plane
 * 2. Use the beam feature, select "custom", and choose that Part Studio
 *
 * The old workflow was:
 * 1. Create a copy of this document
 * 2. Create a Part Studio including a sketch of the beam profile on the Top Plane
 * 3. Use this profile generator to select the profile and generate data
 * 4. Open the FeatureScript notices panel and copy the data
 * 5. Paste the data into a specific section of the Beam Profiles tab
 * 6. Use the copied beam feature document for creating custom profiles
 *
 * We strongly suggest you use the new workflow, as it adds flexibility, configurability, and keeps you
 * up-to-date with any future changes or fixes Onshape makes to this feature.
 *
 * The Beam Profile Generator below is left only in order to maintain existing features that used the old
 * workflow.
 */

// This script will take a sketch profile and output the vertices for use in the Beam Profiles LookUpTable

annotation { "Feature Type Name" : "Beam Profile Generator", "Feature Name Template" : "Profile #profileName" }
export const beamProfile = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Name" }
        definition.profileName is string;

        annotation { "Name" : "Unique profile", "UIHint" : "REMEMBER_PREVIOUS_VALUE" }
        definition.unique is boolean;

        annotation { "Name" : "Profile face", "Filter" : EntityType.FACE, "MaxNumberOfPicks" : 1 }
        definition.face is Query;

        annotation { "Name" : "Units", "Default" : profileUnits.MM, "UIHint" : "REMEMBER_PREVIOUS_VALUE" }
        definition.units is profileUnits;
    }
    {
        setFeatureComputedParameter(context, id, { "name" : "profileName", "value" : definition.profileName });

        const allEdges = qEdgeAdjacent(definition.face, EntityType.EDGE);
        const allFaces = qEdgeAdjacent(allEdges, EntityType.FACE);
        const innerFaces = qSubtraction(allFaces, definition.face);
        var outerEdges = qSubtraction(allEdges, qEdgeAdjacent(innerFaces, EntityType.EDGE));

        var units = [1000, 4, "millimeter"]; // need this minimum decimal places to ensure tangent arcs

        if (definition.units == profileUnits.INCH)
        {
            units = [1000 / 25.4, 5, "inch"];
        }

        reportFeatureInfo(context, id, "Open the FeatureScript notices panel and Copy & Paste the data into the Beam Profiles tab.");

        var edges = [[qNthElement(outerEdges, 0), 1]];
        var count = 0;
        var numEdges = size(evaluateQuery(context, outerEdges));

        var edgeData = processEdge(context, edges[0][0], edges[0][1], units, true);
        var sequence = edgeData[0];
        var points = edgeData[1];

        // Process outer loop first
        while (count < numEdges - 1)
        {
            const lastEdge = edges[size(edges) - 1][0];
            const lastEndPoint = evEdgeTangentLine(context, { "edge" : lastEdge, "parameter" : edges[size(edges) - 1][1], "arcLengthParameterization" : false }).origin;
            const nextEdge = qClosestTo(qSubtraction(outerEdges, lastEdge), lastEndPoint);
            const nextEndPoint = evEdgeTangentLine(context, { "edge" : nextEdge, "parameter" : 0, "arcLengthParameterization" : false });

            edges = append(edges, [nextEdge, coincident(nextEndPoint.origin, lastEndPoint, units) ? 1 : 0]);

            outerEdges = qSubtraction(outerEdges, lastEdge);

            edgeData = processEdge(context, edges[size(edges) - 1][0], edges[size(edges) - 1][1], units, false);
            sequence = sequence ~ edgeData[0];
            points = points ~ ", " ~ edgeData[1];
            count += 1;
        }

        for (var i = 0; i < size(evaluateQuery(context, innerFaces)); i += 1)
        {
            count = 0;
            var innerEdges = qEdgeAdjacent(qNthElement(innerFaces, i), EntityType.EDGE);
            edges = [[qNthElement(innerEdges, 0), 1]];
            edgeData = processEdge(context, edges[0][0], edges[0][1], units, true);

            const lastEdgeType = splitIntoCharacters(sequence)[length(sequence) - 1];

            sequence = sequence ~ (edgeData[0] == "C" || lastEdgeType == "C" ? "" : "-") ~ edgeData[0];
            points = points ~ ", " ~ edgeData[1];
            numEdges = size(evaluateQuery(context, innerEdges));

            // Process inner loops
            while (count < numEdges - 1)
            {
                const lastEdge = edges[size(edges) - 1][0];
                const lastEndPoint = evEdgeTangentLine(context, { "edge" : lastEdge, "parameter" : edges[size(edges) - 1][1], "arcLengthParameterization" : false }).origin;
                const nextEdge = qClosestTo(qSubtraction(innerEdges, lastEdge), lastEndPoint);
                const nextEndPoint = evEdgeTangentLine(context, { "edge" : nextEdge, "parameter" : 0, "arcLengthParameterization" : false });

                edges = append(edges, [nextEdge, coincident(nextEndPoint.origin, lastEndPoint, units) ? 1 : 0]);

                innerEdges = qSubtraction(innerEdges, lastEdge);

                edgeData = processEdge(context, edges[size(edges) - 1][0], edges[size(edges) - 1][1], units, false);
                sequence = sequence ~ edgeData[0];
                points = points ~ ", " ~ edgeData[1];
                count += 1;
            }
        }

        println("\"units\" : " ~ units[2] ~ ",");
        if (!definition.unique)
            println("\"sequence\" : \"" ~ sequence ~ "\",");
        println("\"entries\" : {");
        println("\"" ~ definition.profileName ~ "\" : {" ~ (definition.unique ? "\n\t\"sequence\" : \"" ~ sequence ~ "\"," : "") ~ "\n\t\"points\" : [" ~ points ~ "] \n},");
    });

function processEdge(context is Context, edge is Query, vertexNumber is number, units is array, startEnd is boolean) returns array
{
    var edgeData = ["", ""];
    const edgeType = evCurveDefinition(context, { "edge" : edge });

    const points = evEdgeTangentLines(context, { "edge" : edge, "parameters" : [1 - vertexNumber, 0.5, vertexNumber] });
    var startPoint = [rnd(points[0].origin[0].value, units), rnd(points[0].origin[1].value, units)];
    var midPoint = [rnd(points[1].origin[0].value, units), rnd(points[1].origin[1].value, units)];
    var endPoint = [rnd(points[2].origin[0].value, units), rnd(points[2].origin[1].value, units)];

    if (edgeType is Line)
    {
        edgeData[0] = "L";
        if (startEnd)
            edgeData[1] = "" ~ startPoint[0] ~ ", " ~ startPoint[1] ~ ", ";
        edgeData[1] = edgeData[1] ~ endPoint[0] ~ ", " ~ endPoint[1];
    }

    if (edgeType is Circle)
    {
        if (size(evaluateQuery(context, qVertexAdjacent(edge, EntityType.VERTEX))) == 0) // no vertices equals full circle
        {
            edgeData[0] = "C";
            startPoint = [rnd(edgeType.coordSystem.origin[0].value, units), rnd(edgeType.coordSystem.origin[1].value, units)];
            edgeData[1] = "" ~ startPoint[0] ~ ", " ~ startPoint[1] ~ ", " ~ rnd(edgeType.radius.value, units);
        }
        else
        {
            edgeData[0] = "A";
            if (startEnd)
                edgeData[1] = "" ~ startPoint[0] ~ ", " ~ startPoint[1] ~ ", ";
            edgeData[1] = edgeData[1] ~ midPoint[0] ~ ", " ~ midPoint[1] ~ ", " ~ endPoint[0] ~ ", " ~ endPoint[1];
        }
    }

    return edgeData;
}

function rnd(value is number, units is array) returns number
{
    //return round(value * units[0] * 10 ^ units[1]) / 10 ^ units[1];
    return roundToPrecision(value * units[0], units[1]);
}

function coincident(point1 is Vector, point2 is Vector, units is array)
{
    // /10 added to avoid floating point rounding errors
    return (rnd(point1[0].value / 10, units) == rnd(point2[0].value / 10, units) &&
            rnd(point1[1].value / 10, units) == rnd(point2[1].value / 10, units));
}

export enum profileUnits
{
    annotation { "Name" : "Inch" }
    INCH,
    annotation { "Name" : "Millimeter" }
    MM
}
