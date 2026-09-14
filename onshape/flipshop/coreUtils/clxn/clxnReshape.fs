FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
//
import(path : "66e287bede293cb227dfb89c", version : "e6c9aacc05cf0a6f15c7d4c7"); // typeUtils, for ifNil &c
import(path : "607f97fc690581579d1d4a08", version : "6afa9ed6ec4a413d9bf06340"); // clxnGetset, for deepMerge and pathForKey
import(path : "dd812faf6ff4099cda4aa0eb", version : "ccf1aec9c08b9ad59691b4d4"); // metadataUtils, for setAt

export function undotMap(obj is map) returns map {
  return undotMap(obj, ((existing, incoming) => deepMerge(existing, incoming)));
}

/**
 * Expands the dotted top-level keys of `obj` into nested maps: `{ "a.b": 1, "a": { "c": 2 } }`
 * becomes `{ "a": { "b": 1, "c": 2 } }`.
 *
 * Keys are applied shallowest first, so the result is independent of map iteration order (which
 * FeatureScript does not promise): two keys of equal depth can never have one path be a prefix of
 * the other, so their writes cannot interact, and across depths the deeper key always lands
 * later. Shallow keys lay down structure, deeper keys refine it.
 *
 * Where two keys land on the same path, `onCollision(existing, incoming)` decides, defaulting to
 * @see `deepMerge` so that two maps combine rather than one replacing the other. Pass
 * `lastInWins` for plain lodash-set behavior, or your own resolver to complain instead. The
 * resolver is a leaf rule only: a scalar in the way of a deeper key is replaced either way, so
 * `{ "foo": 1, "foo.bar": 2 }` is `{ "foo": { "bar": 2 } }` and does not report the 1.
 *
 * Only the keys of `obj` itself are split; dots inside a value's own keys are left alone, so
 * `{ "socket_bit.inthex": { "H2.5mm": leafData } }` keeps `H2.5mm` in one piece. A key ending in
 * a dot is unhandled — @see `pathForKey`.
 *
 * For a large map whose keys are mostly undotted, Onshape's `intersectMaps` keeps only the keys
 * present in both of two maps; partition with that and pass in just the dotted ones.
 */
export function undotMap(obj is map, onCollision is function) returns map {
  var pathsByKey = {};
  var deepest = 0;
  for (var keyStr in keys(obj)) {
    const keyPath = pathForKey(keyStr);
    pathsByKey[keyStr] = keyPath;
    if (size(keyPath) > deepest) { deepest = size(keyPath); }
  }
  var result = {};
  for (var depth = 1; depth <= deepest; depth += 1) {
    for (var keyStr in keys(obj)) {
      const keyPath = pathsByKey[keyStr];
      if (size(keyPath) != depth) { continue; }
      result = setAt(result, keyPath, obj[keyStr], onCollision);
    }
  }
  return result;
}

/**
 * Depth-first flatten of a nested map into dotted keys: `{ "a": { "b": 1 } }` becomes
 * `{ "a.b": 1 }`.
 *
 * `options.maxDepth` is how many levels to collapse, i.e. how many dots a key can gain, the
 * way lodash's flattenDepth counts. Absent means all the way down; 0 or less is a no-op, and
 * a depth deeper or shallower than the map is fine. Unknown option keys are ignored.
 *
 * Arrays are leaves, and so are empty maps: there is no inner key to dot with, and dropping
 * the key would lose it.
 *
 * Keys that already contain a dot are dotted anyway. undotMap cannot tell those dots from the
 * ones added here, so a key like `"H2.5mm"` does not survive the round trip. See the
 * RoundTripCases in the test module.
 */
export function dotMap(obj is map, options is map) returns map {
  const maxDepth = ifNil(options.maxDepth, UnboundedDotDepth);
  if (maxDepth <= 0) { return obj; }
  var result = {};
  for (var keyStr in keys(obj)) {
    const val = obj[keyStr];
    if ((val is map) && (size(val) > 0)) {
      const dotted = dotMap(val, { "maxDepth": maxDepth - 1 });
      for (var innerKeyStr in keys(dotted)) {
        result[keyStr ~ "." ~ innerKeyStr] = dotted[innerKeyStr];
      }
    } else {
      result[keyStr] = val;
    }
  }
  return result;
}

export function dotMap(obj is map) returns map {
  return dotMap(obj, {});
}

/**
 * Normalizes one level definition into `{ name, displayName, tr }`.
 * @param levelDef {string|array}: `name`, `[name]`, `[name, displayName]`, `[name, tr]`, or
 *   `[name, displayName, tr]`. `displayName` defaults to `titleCase(name)`; `tr` defaults to `{}`.
 */
function parseLevelDef(levelDef) returns map {
  var name = "";
  var displayName = "";
  var tr = {};
  if (levelDef is string) {
    name = levelDef;
  } else if (levelDef is array) {
    name = levelDef[0];
    if (getAt(levelDef, 1) is string) {
      displayName = levelDef[1];
      if (getAt(levelDef, 2) is map)      { tr = levelDef[2]; }
    } else if (getAt(levelDef, 1) is map) { tr = getAt(levelDef, 1); }
  }
  if (displayName == "") { displayName = titleCase(name); }
  return { "name": name, "displayName": displayName, "tr": tr };
}

/**
 * Nested choice-list structure (`{ name, displayName, entries }` per level) built by walking
 * `tree` according to `levels`. Returns `tree` unchanged if `levels` has fewer than 2 entries.
 * @param levels {array}: One entry per level — @see `parseLevelDef` for the accepted shapes.
 * @param tree {map}: Raw deeply-nested map, keyed the same way at each level as `levels` describes.
 * @example
 *   buildNestedChoices(['socket_kind', ['drive_kind', { "inthex": 'Int Hex' }]], tree);
 */
export function buildNestedChoices(levels is array, tree is map) returns map {
  if (size(levels) < 2) { return tree; }
  var parsedLevels = [];
  for (var lvl in levels) {
    parsedLevels = append(parsedLevels, parseLevelDef(lvl));
  }
  return buildChoicesRecursive(parsedLevels, 0, tree);
}

/**
 * One level of `buildNestedChoices`'s descent: wraps each key of `currentTree` in a
 * `{ name, displayName, entries }` choice entry, translating the key via `levels[levelIdx].tr`
 * (falling back to `titleCase`) and recursing into `entries` until `levelIdx` reaches the
 * second-to-last level, where `entries` is `currentTree`'s own subtree unchanged.
 */
function buildChoicesRecursive(levels is array, levelIdx is number, currentTree is map) returns map {
  var result = {};
  var currentDef = levels[levelIdx];
  var nextDef = levels[levelIdx + 1];
  var isLastLevel = (levelIdx == size(levels) - 2);
  for (var keyStr, subTree in currentTree) {
    // Evaluate the key display name using the level's translation map, or fallback to titleCase
    var translatedKey = currentDef.tr[keyStr] != undefined
      ? currentDef.tr[keyStr]
      : titleCase(keyStr);
    var entry = {
      "name":        nextDef.name,
      "displayName": nextDef.displayName
    };
    if (isLastLevel) {
      // Base case: the bottom level's entries are the leaf data itself
      entry["entries"] = subTree;
    } else {
      // Recursive case: keep descending the tree
      entry["entries"] = buildChoicesRecursive(levels, levelIdx + 1, subTree);
    }
    result[translatedKey] = entry;
  }
  return result;
}

// == [Function values] --

export const ClxnReshapeFuncs = {
  "undotMap":            (obj, onCollision) => undotMap(obj, onCollision),
  "dotMap":              (obj, options)     => dotMap(obj, options),
  "buildNestedChoices":  (levels, tree)     => buildNestedChoices(levels, tree),
};