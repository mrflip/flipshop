FeatureScript 3070;
import(path: "onshape/std/common.fs", version: "3070.0");
import(path : "14a20c5c0c7e0354a621f347/4be0609755e53334547de986/4ebdc64943b566160ea5cc28", version : "aa1e3063ddbe9d05678234b7");
import(path : "14a20c5c0c7e0354a621f347/4be0609755e53334547de986/6fcd20533bd2df7c4094a0bf", version : "761cb377fc942f7bc52bffd4");

// mergeMaps comes from onshape/std/containers.fs (pulled in by geometry.fs).
// ifNil and ifPresent are expected to be in scope from your utility module.

/**
 * Default options for [placeMany].
 */
export const PLACE_MANY_DEFAULTS = {
  "makeCopy": false
};

export enum PlaceManyMode {
    annotation { "Name": "Grid or Single Target" }
    GRID,
    annotation { "Name": "Multiple Targets" }
    MULTI,
}


/**
 * Finds the solid body a mate connector belongs to.
 *
 * A mate connector is its own point body, so `qOwnerBody` returns the connector
 * rather than the part it was placed on. When the owner is not a solid, the part
 * is found by testing which modifiable solid body contains the connector origin.
 *
 * @param connector {Query}: A query resolving to a single mate connector.
 * @returns {Query}: A query for a single solid body, or `undefined` when no
 *      body can be found.
 */
function bodyOwningConnector(context is Context, id is Id, connector is Query) {
  const ownerSolids = evaluateQuery(context, qBodyType(qOwnerBody(connector), BodyType.SOLID));
  if (size(ownerSolids) > 0) {
    debug(context, "Found " ~ size(ownerSolids) ~ " owner using qBodyType(qOwnerBody(connector), BodyType.SOLID) for " ~ id);
    return ownerSolids[0];
  }

  const center = evMateConnector(context, { "mateConnector": connector }).origin;
  const containingSolids = evaluateQuery(context, qContainsPoint(qAllModifiableSolidBodies(), center));
  if (size(containingSolids) > 0) {
    debug(context, "Found " ~ size(containingSolids) ~ " owner using qContainsPoint(qAllModifiableSolidBodies(), center) for " ~ id);
    return containingSolids[0];
  }
  debug(context, "Found no owner for " ~ id);
  return undefined;
}

/**
 * Moves each body so that its source mate connector lands on a destination mate
 * connector, the same placement `Transform` performs in its "Transform by mate
 * connectors" mode.
 *
 * `bodies` and `fromPts` are parallel: `bodies[ii]` carries `fromPts[ii]`.
 * `targetPts` is consumed cyclically, so five bodies over three targets are
 * placed on targets 0, 1, 2, 0, 1, and surplus targets are simply unused.
 *
 * Each placement runs in its own `try`, so a body that fails is reported with
 * `debug` (and highlighted in red) while the remaining bodies are still placed.
 *
 * @param id {Id}: The id of the calling feature. Each placement is given its
 *      own sub id.
 * @param targetPts {array}: Queries, each resolving to a single destination
 *      mate connector. Cycled when shorter than `bodies`.
 * @param bodies {array}: Queries, each resolving to the body being placed.
 * @param fromPts {array}: Queries, each resolving to a single mate connector
 *      belonging to the corresponding entry of `bodies`.
 * @param opts {{
 *      @field makeCopy {boolean}: @optional `true` to leave the original body
 *              in place and move a copy instead. Default `false`.
 * }}
 */
export function placeMany(context is Context, id is Id, targetPts is array, bodies is array, fromPts is array, opts is map) {
  const options = mergeMaps(PLACE_MANY_DEFAULTS, opts);

  const placementCount = size(fromPts);
  if (size(bodies) != placementCount) {
    throw regenError("placeMany requires bodies and fromPts to be the same length.");
  }
  if (size(targetPts) == 0) {
    throw regenError("placeMany requires at least one target point.");
  }

  for (var ii = 0; ii < placementCount; ii += 1) {
    try {
      const sourceCsys = evMateConnector(context, { "mateConnector": fromPts[ii] });
      const targetCsys = evMateConnector(context, { "mateConnector": targetPts[ii % size(targetPts)] });
      const placement = toWorld(targetCsys) * inverse(toWorld(sourceCsys));
      const placementId = id + ("place" ~ toString(ii));

      if (options.makeCopy) {
        opPattern(context, placementId, {
          "entities": bodies[ii],
          "transforms": [placement],
          "instanceNames": ["1"]
        });
      } else {
        opTransform(context, placementId, {
          "bodies": bodies[ii],
          "transform": placement
        });
      }
    } catch (error) {
      debug(context, bodies[ii], DebugColor.RED);
      debug(context, "placeMany: skipped placement " ~ toString(ii) ~ ": " ~ toString(error));
    }
  }
}

/**
 * Places several parts at once by mate connector. Every part is identified by
 * the mate connector selected on it, so there is no separate part selection.
 *
 * In scatter mode the destinations are dealt out cyclically in selection order:
 * five parts over three destinations land on A, B, C, A, B, and destinations
 * beyond the number of parts go unused. Grid mode takes a single destination and
 * will generate its own array of placements from it.
 *
 * @param definition {{
 *      @field mode {PlaceManyMode}: @optional `true` to place the parts on a grid
 *          built from one destination, `false` to deal them out over the
 *          selected destinations. Default `false`.
 *      @field fromLocs {Query}: Mate connectors on the parts to place.
 *          The part moved is the solid body owning each connector.
 *          @eg `qCreatedBy(id + "mateConnector1", EntityType.BODY)`
 *      @field ontoLocs {Query}: The mate connectors to place onto.
 *          Limited to a single pick when `gridMode` is `true`.
 *      @field makeCopy {boolean}: @optional `true` to leave the originals in
 *          place and move copies. Default `false`.
 * }}
 */
annotation {
    "Feature Type Name": "Place many",
    "Filter Selector": "allparts",
    "Feature Name Template": "Place Many on #mode",
}
export const PlaceManyFS = defineFeature(function(context is Context, id is Id, definition is map)
  precondition {

    annotation {
        "Name":    "Mode",
        "Default": PlaceManyMode.MULTI,
        "UIHint":  [UIHint.REMEMBER_PREVIOUS_VALUE],
    }
    definition.mode is PlaceManyMode;

    annotation {
        "Name":    "Initial Part Locations",
        "Filter":  BodyType.MATE_CONNECTOR && InContextObject.NO,
        "UIHint":  [UIHint.ALLOW_QUERY_ORDER, UIHint.INITIAL_FOCUS],
    }
    definition.fromLocs is Query;

    if (definition.mode == PlaceManyMode.GRID) {
      annotation {
        "Name":    "Target locations",
        "Filter":  BodyType.MATE_CONNECTOR && InContextObject.NO,
        "MaxNumberOfPicks": 1
      }
      definition.ontoLoc is Query;
    } else {
      annotation {
        "Name":    "Destination mate connectors",
        "Filter":  BodyType.MATE_CONNECTOR && InContextObject.NO,
        "UIHint":  [UIHint.ALLOW_QUERY_ORDER],
      }
      definition.ontoLocs is Query;
    }

    annotation {
        "Name":    "Copy parts",
        "UIHint":  [UIHint.REMEMBER_PREVIOUS_VALUE],
    }
    definition.makeCopy is boolean;
  }
  {
    const mode = ifNil(definition.gridMode, PlaceManyMode.MULTI);
    const fromLocsQ = definition.fromLocs;
    const fromLocs  = evaluateQuery(context, fromLocsQ);
    const ontoLocsQ = (mode == PlaceManyMode.MULTI ? definition.ontoLocs: definition.ontoLoc);
    const ontoLocs  = evaluateQuery(context, ontoLocsQ);
    if (size(fromLocs) == 0) {
      throw regenError("Select one or more mate connectors on the parts to place.", ["fromLocs"]);
    }
    if (size(ontoLocs) == 0) {
      throw regenError("Select a destination mate connector.", ["ontoLocs"]);
    }

    var bodies    = [];
    var fromPts = [];

    for (var ii = 0; ii < size(fromLocs); ii += 1) {
      const fromLoc  = fromLocs[ii];
      const body     = bodyOwningConnector(context, id, fromLoc);
      const bodyName = (body is Query ? getNameProp(context, body) : "missing Body");
      debug(context, [bodyName, body, (body == undefined ? "missing" : getAllAttrs(context, body))]);


      if (! isPresent(body)) {
        debug(context, fromLoc, DebugColor.RED);
        debug(context, "PlaceManyFS: no part found for mate connector " ~ toString(ii) ~ "; skipping it.");
        continue;
      }

      fromPts = append(fromPts, fromLoc);
      bodies = append(bodies, body);
    }

    if (size(bodies) == 0) {
      throw regenError("None of the selected mate connectors resolved to a part.", ["fromLocs"]);
    }

    placeMany(context, id, ontoLocs, bodies, fromPts, {
      "makeCopy": ifNil(definition.makeCopy, false),
    });
  },
  { "mode": PlaceManyMode.MULTI, "makeCopy": false }
 );