FeatureScript 3070;
import(path: "onshape/std/common.fs", version: "3070.0");
import(path : "14a20c5c0c7e0354a621f347/185949b8bdfeb193e13253d7/4ebdc64943b566160ea5cc28", version : "332a1753347ec00e4732ac5f");
import(path : "14a20c5c0c7e0354a621f347/185949b8bdfeb193e13253d7/6fcd20533bd2df7c4094a0bf", version : "566d908d563f598d3989a764");
import(path : "14a20c5c0c7e0354a621f347/185949b8bdfeb193e13253d7/cf9288dffb8a37e7d4b3d9fa", version : "69f307aa59ffeface1b20182");

// mergeMaps comes from onshape/std/containers.fs.
// ifNil and isPresent are expected to be in scope from your utility module.

/**
 * Location information, however it happens to be expressed. Only `mate` is
 * consumed today; `pos` is admitted so a caller can hand over a bare position
 * before gridding exists to produce mates of its own.
 */
export predicate canBeLoc(val) {
  val is map;
  val.mate is Query || val.pos is Vector;
}
export type Loc typecheck canBeLoc;

/**
 * Instructions for placing one thing: what to move, the location on it that
 * does the positioning, and every location to put it onto. More than one
 * `ontoLocs` entry means copies.
 */
export predicate canBeMultiPlacement(val) {
  val             is map;
  val.thing       is map;
  val.thing.query is Query;
  val.fromLoc     is Loc;
  val.ontoLocs    is array;
}
export type MultiPlacement typecheck canBeMultiPlacement;

/**
 * Default options for [placeThing].
 */
export const PLACE_THING_DEFAULTS = {
  "makeCopy": false
};

export enum PlaceManyMode {
    annotation { "Name": "Placing" }
    PLACING,
    annotation { "Name": "Stamping" }
    STAMPING,
    annotation { "Name": "Grid or Single Target" }
    GRID,
}

/**
 * Guesses the solid a mate belongs to, returning an unevaluated query so the
 * result can be stored in a feature parameter and re-resolved on each
 * regeneration.
 *
 * A mate is its own body of `BodyType.MATE_CONNECTOR`, so `qOwnerBody` only
 * finds a solid for mates that were not created live in a dialog. Failing that
 * the mate origin is tested against every modifiable solid, and failing that
 * the nearest solid wins, which is what rescues a mate sitting off its thing
 * (the middle of an "O", a mid-air datum).
 *
 * Every branch is narrowed with `qNthElement` so the result can never resolve
 * to more than one body.
 *
 * @param mateQ {Query}: A query resolving to a single mate.
 * @returns {Query}: A query for a single solid, or `undefined` when none can
 *      be found.
 */
function bestThingQForMateQ(context is Context, id is Id, mateQ is Query) {
  const owningThingsQ = qBodyType(qOwnerBody(mateQ), BodyType.SOLID);
  if (size(evaluateQuery(context, owningThingsQ)) > 0) {
    return qNthElement(owningThingsQ, 0);
  }

  const origin = evMateConnector(context, { "mateConnector": mateQ }).origin;

  const containingThingsQ = qContainsPoint(qAllModifiableSolidBodies(), origin);
  if (size(evaluateQuery(context, containingThingsQ)) > 0) {
    return qNthElement(containingThingsQ, 0);
  }

  const closestThingsQ = qClosestTo(qAllModifiableSolidBodies(), origin);
  if (size(evaluateQuery(context, closestThingsQ)) > 0) {
    return qNthElement(closestThingsQ, 0);
  }

  return undefined;
}

/**
 * Fills in the thing query of a mated thingLoc, idempotently.
 *
 * @param thingLoc {map}: `{ ..., loc: { mate }, thing?: { query? } }`.
 * @returns {map}: `{ ...thingLoc, thing: { query } }`, the input itself when
 *      `thing.query` was already set.
 */
export function bestThingQedForMated(context is Context, id is Id, thingLoc is map) returns map {
  if (thingLoc.thing is map && isPresent(thingLoc.thing.query)) {
    return thingLoc;
  }

  const thingQ = bestThingQForMateQ(context, id, thingLoc.loc.mate);
  if (! isPresent(thingQ)) {
    throw regenError("Could not work out which part this mate belongs to.");
  }

  return mergeMaps(thingLoc, {
    "thing": mergeMaps(ifNil(thingLoc.thing, {}), { "query": thingQ }),
  });
}

/**
 * Fills in the thing query of a thingLoc however its location happens to be
 * expressed, idempotently. Only mated locations are understood so far.
 *
 * @param thingLoc {map}: `{ ..., loc: Loc, thing?: { query? } }`.
 * @returns {map}: `{ ...thingLoc, thing: { query } }`.
 */
export function bestThingQedFor(context is Context, id is Id, thingLoc is map) returns map {
  if (thingLoc.thing is map && isPresent(thingLoc.thing.query)) {
    return thingLoc;
  }
  if (thingLoc.loc is map && thingLoc.loc.mate is Query) {
    return bestThingQedForMated(context, id, thingLoc);
  }
  throw regenError("bestThingQedFor: expected a thing'ed or mated loc, got " ~ toString(thingLoc));
}

/**
 * The thing query alone, for callers that want nothing else back.
 *
 * @param thingLoc {map}: `{ ..., loc: Loc, thing?: { query? } }`.
 * @returns {Query}
 */
export function bestThingQFor(context is Context, id is Id, thingLoc is map) returns Query {
  return bestThingQedFor(context, id, thingLoc).thing.query;
}

/**
 * @internal
 * Builds a [MultiPlacement] from a resolved thingLoc and the locations to place
 * it onto.
 */
function mergeMultiplacement(mp1 is map, mp2 is map) returns MultiPlacement {
  return mergeMaps(mp1, mp2) as MultiPlacement;
}

/**
 * Every thing onto every location: one placement per thing, each carrying the
 * whole `ontoLocs` list. Placing the parts of a knob at each of the places a
 * knob is needed.
 *
 * @param thingLocs {array}: Resolved thingLocs, `{ thing: { query }, loc: Loc }`.
 * @param ontoLocs {array}: The [Loc]s to place onto.
 * @returns {array}: [MultiPlacement]s.
 */
export function placementsForStamping(context is Context, id is Id, thingLocs is array, ontoLocs is array) returns array {
  var placements = [];
  for (var thingLoc in thingLocs) {
    placements = append(placements, mergeMultiplacement(thingLoc, { "ontoLocs": ontoLocs }));
  }
  return placements;
}

/**
 * Every thing onto its own cell of a grid anchored at one location. The grid is
 * one cell for now, so this is every thing onto that single location.
 *
 * @param thingLocs {array}: Resolved thingLocs, `{ thing: { query }, loc: Loc }`.
 * @param ontoLoc {Loc}: Where the grid is anchored.
 * @returns {array}: [MultiPlacement]s.
 */
export function placementsForGrid(context is Context, id is Id, thingLocs is array, ontoLoc is Loc) returns array {
  // TODO: expand ontoLoc into one Loc per cell and deal the things out over
  // them. Until then every thing lands on the anchor.
  var placements = [];
  for (var thingLoc in thingLocs) {
    placements = append(placements, mergeMultiplacement(thingLoc, { "ontoLocs": [ontoLoc] }));
  }
  return placements;
}

/**
 * Each thing onto the locations it brought with it.
 *
 * @param thingLocs {array}: Resolved thingLocs that also carry `ontoLocs`.
 * @returns {array}: [MultiPlacement]s.
 */
export function placementsForPlacing(context is Context, id is Id, thingLocs is array) returns array {
  var placements = [];
  for (var thingLoc in thingLocs) {
    placements = append(placements, mergeMultiplacement(thingLoc, { "ontoLoc": ifNil(thingLoc.ontoLocs, []) }));
  }
  return placements;
}

/**
 * Moves one thing so that its `fromLoc` lands on each of its `ontoLocs`, the
 * same placement `Transform` performs in its "Transform by mate connectors"
 * mode.
 *
 * The first location moves the thing itself; any location after it necessarily
 * gets a copy. `makeCopy` makes the first one a copy too, leaving the original
 * where it was.
 *
 * @param id {Id}: Each location placed gets its own sub id.
 * @param opts {{
 *      @field makeCopy {boolean}: @optional `true` to leave the original in
 *              place and move a copy instead. Default `false`.
 * }}
 */
export function placeThing(context is Context, id is Id, placement is MultiPlacement, opts is map) {
  const options = mergeMaps(PLACE_THING_DEFAULTS, opts);
  const thingQ = placement.thing.query;
  const fromCsys = evMateConnector(context, { "mateConnector": placement.fromLoc.mate });

  for (var ii = 0; ii < size(placement.ontoLocs); ii += 1) {
    const ontoCsys = evMateConnector(context, { "mateConnector": placement.ontoLocs[ii].mate });
    const transforms = [toWorld(ontoCsys) * inverse(toWorld(fromCsys))];
    const ontoId = id + ("onto" ~ toString(ii));

    if (options.makeCopy || ii > 0) {
      opPattern(context, ontoId, {
        "entities": thingQ,
        "transforms": transforms,
        "instanceNames": ["1"]
      });
      const thingName = getName(context, thingQ, "Part");
      const dupeName  = setReadableName(context, qCreatedBy(ontoId, EntityType.BODY), strSlice(thingName, 0, 95) ~ "." ~ ii, 99);
      debug(context, "Name: " ~ dupeName ~ " from " ~ thingName);
    } else {
      opTransform(context, ontoId, {
        "bodies": thingQ,
        "transform": transforms[0]
      });
    }
  }
}

/**
 * Carries out a list of [MultiPlacement]s. Each placement runs in its own `try`,
 * so a thing that fails is reported with `debug` (and highlighted in red) while
 * the rest are still placed.
 *
 * @param id {Id}: Each placement gets its own sub id.
 * @param placements {array}: [MultiPlacement]s.
 * @param opts {{
 *      @field makeCopy {boolean}: @optional Passed to [placeThing], and
 *              overridden by a placement's own `opts`. Default `false`.
 * }}
 */
export function placeMany(context is Context, id is Id, placements is array, opts is map) {
  for (var ii = 0; ii < size(placements); ii += 1) {
    const placement = placements[ii];
    try {
      placeThing(context, id + ("thing" ~ toString(ii)), placement, mergeMaps(opts, ifNil(placement.opts, {})));
    } catch (error) {
      highlightQuery(context, placement.thing.query, DebugColor.RED);
      debug(context, placement.thing.query, DebugColor.RED);
      debug(context, "placeMany: skipped thing " ~ toString(ii) ~ ": " ~ toString(error));
    }
  }
}

/**
 * @internal
 * Turns one row of the `thingLocs` array parameter into a thingLoc, resolving
 * the thing when the row does not name one. `ontoLocs` is present only in
 * PLACING mode, where each row brings its own destinations.
 */
function thingLocForRow(context is Context, id is Id, row is map) returns map {
  var ontoLocs = [];
  if (isPresent(row.ontoMatesQ)) {
    for (var qq in evaluateQuery(context, row.ontoMatesQ)) {
      ontoLocs = append(ontoLocs, { "mate": qq, "query": row.ontoMatesQ } as Loc);
    }
  }

  const thing = (isPresent(row.thingQ) && size(evaluateQuery(context, row.thingQ)) > 0)
    ? { "query": row.thingQ }
    : undefined;

  return bestThingQedFor(context, id, {
    "thing":    thing,
    "fromLoc":  { "mate": row.fromMateQ } as Loc,
    "ontoLocs": ontoLocs,
  });
}

/**
 * @internal
 * Collects the [Loc]s a mate query resolves to.
 */
function locsForMatesQ(context is Context, matesQ is Query) returns array {
  var locs = [];
  if (!isPresent(matesQ)) {
    return locs;
  }
  for (var qq in evaluateQuery(context, matesQ)) {
    locs = append(locs, { "mate": qq } as Loc);
  }
  return locs;
}

/**
 * @internal
 * Reports whether the user has explicitly touched the thing field of a given
 * `thingLocs` row, defensively, since `specifiedParameters` is not guaranteed
 * to mirror an array parameter that has just grown.
 */
function thingQWasSpecified(specifiedParameters is map, index is number) returns boolean {
  const specifiedRows = specifiedParameters.thingLocs;
  if (! (specifiedRows is array) || index >= size(specifiedRows)) {
    return false;
  }
  if (! (specifiedRows[index] is map)) {
    return false;
  }
  return specifiedRows[index].thingQ == true;
}

/**
 * @internal
 * Reads a field of the `thingLocs` row at `index` of an older definition,
 * returning `undefined` when the row did not exist or is a different shape.
 */
function previousRowField(oldDefinition is map, index is number, field is string) {
  const previousRows = oldDefinition.thingLocs;
  if (! (previousRows is array) || index >= size(previousRows)) {
    return undefined;
  }
  if (! (previousRows[index] is map)) {
    return undefined;
  }
  return previousRows[index][field];
}

/**
 * @internal
 * Editing logic for [PlaceManyFS]. Fills in the thing of each row from its
 * mate, leaving alone any thing the user picked themselves. A thing is
 * re-derived when it resolves to nothing or when its mate changed, so
 * re-picking a mate re-points the thing with it.
 *
 * `isCreating` is in the parameter list so the function is called on dialog
 * open as well as on every subsequent change.
 */
export function placeManyEditLogic(context is Context, id is Id, oldDefinition is map, definition is map,
    isCreating is boolean, specifiedParameters is map) returns map {
  if (!(definition.thingLocs is array)) {
    return definition;
  }

  var rows = definition.thingLocs;

  for (var ii = 0; ii < size(rows); ii += 1) {
    var row = rows[ii];
    if ((! (row is map)) || (isNil(row.fromMateQ))) {
      continue;
    }
    if (thingQWasSpecified(specifiedParameters, ii)) {
      continue;
    }

    const thingResolves = isPresent(row.thingQ) && size(ifNil(try silent(evaluateQuery(context, row.thingQ)), [])) > 0;
    const mateUnchanged = previousRowField(oldDefinition, ii, "fromMateQ") == row.fromMateQ;
    if (thingResolves && mateUnchanged) {
      continue;
    }

    const thingQ = try silent(bestThingQForMateQ(context, id, row.fromMateQ));
    if (isNil(thingQ)) { continue; }

    row.thingQ = thingQ;
    rows[ii] = row;
  }

  definition.thingLocs = rows;
  return definition;
}

/**
 * Places several things at once by mate. Each thing is paired with the mate
 * that positions it, so a mate sitting off its thing (the centre of an "O", a
 * mid-air datum) still works.
 *
 * PLACING gives every row its own destinations, for arranging things on a build
 * plate. STAMPING puts every thing onto every destination. GRID anchors a grid
 * at one destination, and until the gridding exists that grid is one cell.
 *
 * @param definition {{
 *      @field mode {PlaceManyMode}: @optional Default `PLACING`.
 *      @field thingLocs {array}: One row per thing, each a map of `fromMateQ`
 *          {Query}, the mate that positions the thing; `thingQ` {Query}, the
 *          thing itself, filled in by the editing logic and overridable; and,
 *          in PLACING mode, `ontoMatesQ` {Query}, that thing's destinations.
 *      @field ontoMatesQ {Query}: @requiredIf {`mode` is `STAMPING`}
 *          The mates every thing is placed onto, in selection order.
 *      @field ontoMateQ {Query}: @requiredIf {`mode` is `GRID`}
 *          The single mate the grid is anchored at.
 *      @field makeCopy {boolean}: @optional `true` to leave the originals in
 *          place and move copies. Default `false`.
 * }}
 */
annotation {
    "Feature Type Name": "Place many",
    "Filter Selector": "allparts",
    "Feature Name Template": "Place Many on #mode",
    "Editing Logic Function": "placeManyEditLogic",
}
export const PlaceManyFS = defineFeature(function(context is Context, id is Id, definition is map)
  precondition {

    annotation {
        "Name":    "Mode",
        "Default": PlaceManyMode.PLACING,
        "UIHint":  [UIHint.REMEMBER_PREVIOUS_VALUE],
    }
    definition.mode is PlaceManyMode;

    annotation {
        "Name":      "Things to place",
        "Item name": "thing",
        "UIHint":    [UIHint.INITIAL_FOCUS],
    }
    definition.thingLocs is array;
    for (var row in definition.thingLocs) {
      annotation {
          "Name":             "Location",
          "Filter":           BodyType.MATE_CONNECTOR && InContextObject.NO,
          "MaxNumberOfPicks": 1,
          "UIHint":           [UIHint.FIRST_IN_ROW],
      }
      row.fromMateQ is Query;

      annotation {
          "Name":             "Thing",
          "Filter":           EntityType.BODY && BodyType.SOLID && ModifiableEntityOnly.YES,
          "MaxNumberOfPicks": 1,
      }
      row.thingQ is Query;

      if (definition.mode == PlaceManyMode.PLACING) {
        annotation {
            "Name":   "Onto",
            "Filter": BodyType.MATE_CONNECTOR && InContextObject.NO,
            "UIHint": [UIHint.ALLOW_QUERY_ORDER],
        }
        row.ontoMatesQ is Query;
      }
    }

    if (definition.mode == PlaceManyMode.STAMPING) {
      annotation {
          "Name":   "Onto every one of",
          "Filter": BodyType.MATE_CONNECTOR && InContextObject.NO,
          "UIHint": [UIHint.ALLOW_QUERY_ORDER],
      }
      definition.stampingMatesQ is Query;
    } else if (definition.mode == PlaceManyMode.GRID) {
      annotation {
          "Name":             "Grid anchored at",
          "Filter":           BodyType.MATE_CONNECTOR && InContextObject.NO,
          "MaxNumberOfPicks": 1,
      }
      definition.ontoMateQ is Query;
    }

    annotation {
        "Name":    "Make Copy",
        "UIHint":  [UIHint.REMEMBER_PREVIOUS_VALUE],
    }
    definition.makeCopy is boolean;
  }
  {
    const mode = ifNil(definition.mode, PlaceManyMode.PLACING);
    const rows = ifNil(definition.thingLocs, []);
    if (size(rows) == 0) {
      throw regenError("Add at least one thing to place.", ["thingLocs"]);
    }

    var thingLocs = [];
    for (var ii = 0; ii < size(rows); ii += 1) {
      try {
        thingLocs = append(thingLocs, thingLocForRow(context, id, rows[ii]));
      } catch (error) {
        debug(context, "PlaceManyFS: skipped thing " ~ toString(ii) ~ ": " ~ toString(error));
      }
    }
    if (size(thingLocs) == 0) {
      throw regenError("None of these rows resolved to a thing to place.", ["thingLocs"]);
    }

    var placements = [];
    if (mode == PlaceManyMode.PLACING) {
      placements = placementsForPlacing(context, id, thingLocs);
    } else if (mode == PlaceManyMode.STAMPING) {
      const ontoLocs = locsForMatesQ(context, definition.stampingMatesQ);
      if (size(ontoLocs) == 0) {
        throw regenError("Select the mates to place everything onto.", ["ontoMatesQ"]);
      }
      placements = placementsForStamping(context, id, thingLocs, ontoLocs);
    } else {
      const ontoLocs = locsForMatesQ(context, definition.ontoMateQ);
      if (size(ontoLocs) == 0) {
        throw regenError("Select the mate to anchor the grid at.", ["ontoMateQ"]);
      }
      placements = placementsForGrid(context, id, thingLocs, ontoLocs[0]);
    }

    for (var ii = 0; ii < size(placements); ii += 1) {
      if (size(placements[ii].ontoLocs) == 0) {
        debug(context, placements[ii].thing.query, DebugColor.RED);
        debug(context, "PlaceManyFS: thing " ~ toString(ii) ~ " has nowhere to go; skipping it.");
      }
    }

    placeMany(context, id, placements, {
      "makeCopy": ifNil(definition.makeCopy, false),
    });
  },
  { "mode": PlaceManyMode.PLACING, "makeCopy": false }
 );
