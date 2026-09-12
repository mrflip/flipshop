FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
import(path : "66e287bede293cb227dfb89c", version : "e6c9aacc05cf0a6f15c7d4c7");
import(path : "607f97fc690581579d1d4a08", version : "6afa9ed6ec4a413d9bf06340");
import(path : "dd812faf6ff4099cda4aa0eb", version : "ccf1aec9c08b9ad59691b4d4");

export function undotMap(obj is map) returns map {
  return undotMap(obj, ((existing, incoming) => deepMerge(existing, incoming)));
}

/**
 * Expands the dotted top-level keys of `obj` into nested maps: `{ "a.b": 1, "a": { "c": 2 } }`
 * becomes `{ "a": { "b": 1, "c": 2 } }`. One setAt per key, and the whole of the behavior is
 * in three choices below.
 *
 * Keys are applied shallowest first. That is what makes the result independent of map
 * iteration order, which FeatureScript does not promise: two keys of equal depth can never
 * have one path be a prefix of the other, so their writes cannot interact, and across depths
 * the deeper key always lands later. Shallow keys lay down structure, deeper keys refine it.
 *
 * Where two keys land on the same path, `onCollision(existing, incoming)` decides, defaulting
 * to @see `merge` so that two maps combine rather than one replacing the other. Pass
 * `lastInWins` for plain lodash-set behavior, or your own resolver to complain instead. The
 * resolver is a leaf rule only: a scalar in the way of a deeper key is replaced either way, so
 * `{ "foo": 1, "foo.bar": 2 }` is `{ "foo": { "bar": 2 } }` and does not report the 1.
 *
 * Only the keys of `obj` itself are split. Dots inside a value's own keys are left alone, so
 * `{ "socket_bit.inthex": { "H2.5mm": leafData } }` keeps `H2.5mm` in one piece. A key ending
 * in a dot is unhandled; @see `pathForKey`.
 *
 * If you are handing in a large map whose keys are mostly undotted, note that Onshape has an
 * `intersectMaps`, which keeps only the keys present in both of two maps. Partition with that
 * and pass in just the dotted ones.
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
 * Parses a flexible level definition array/string into a normalized map structure.
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
 * Transforms a raw nested map of uniform data types into the nested choice structure
 * required by the custom feature.
 *
 * @param levels {array}: Level configuration (e.g., ['socket_kind', ['drive_kind', { inthex: 'Int Hex' }]])
 * @param tree {map}: The raw deeply-nested map.
 */
export function buildNestedChoices(levels is array, tree is map) returns map {
  if (size(levels) < 2) { return tree; }
  var parsedLevels = [];
  for (var lvl in levels) {
    parsedLevels = append(parsedLevels, parseLevelDef(lvl));
  }
  return buildChoicesRecursive(parsedLevels, 0, tree);
}

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
      // Base case: The bottom level acts as the leaf data mapping[cite: 1]
      entry["entries"] = subTree;
    } else {
      // Recursive case: Continue descending the tree[cite: 1]
      entry["entries"] = buildChoicesRecursive(levels, levelIdx + 1, subTree);
    }
    result[translatedKey] = entry;
  }
  return result;
}