FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
//
import(path : "54590bc1c9cee0141b968fbb", version : "103c8c30f1046331d65db3e3"); // clxnWalking, for forEach
import(path : "3b81563d40faaff8be820296", version : "95c758559e6e54348e4472ec"); // arrayUtils, for arrayIncludes

/**
 * First key of `bag` whose value satisfies `rule(val, key)`, or `undefined` if none does.
 * `findLastKey` scans in the reverse of `keys(bag)` order.
 * @example
 *   findKey({ "a": 1, "b": 2, "c": 3 }, function(val, key) { return val > 1; }); // => "b"
 */
export function findKey(bag is map, rule is function) {
  for (var key in keys(bag)) {
    if (rule(bag[key], key)) { return key; }
  }
  return undefined;
}

/**
 * Last key of `bag` whose value satisfies `rule(val, key)`, or `undefined` if none does.
 * `findKey` scans in the reverse of `keys(bag)` order.
 * @example
 *   findLastKey({ "a": 1, "b": 2, "c": 3 }, function(val, key) { return val > 1; }); // => "b"
 */
export function findLastKey(bag is map, rule is function) {
  const keylist = keys(bag);
  for (var seq = size(keylist) - 1; seq >= 0; seq -= 1) {
    const key = keylist[seq];
    if (rule(bag[key], key)) { return key; }
  }
  return undefined;
}

/**
 * `bag` with its keys and values swapped: `{"a": "x", "b": "x"}` → `{"x": "b"}` — a value that
 * occurs more than once keeps only its last key, same as lodash. A non-string value is
 * stringified into its new key, matching how lodash's own object keys coerce. `invertBy` collects
 * every key instead of just the last one, grouped under `iteratee(val)` rather than `val` itself.
 * @example
 *   invert({ "a": 1, "b": 2, "c": 1 }); // => { "1": "c", "2": "b" }
 */
export function invert(bag is map) returns map {
  var result = {};
  for (var key in keys(bag)) {
    result['' ~ bag[key]] = key;
  }
  return result;
}
export function invertBy(bag is map, iteratee is function) returns map {
  var result = {};
  for (var key in keys(bag)) {
    result = insertIntoMapOfArrays(result, iteratee(bag[key]), key);
  }
  return result;
}

/**
 * `bag`'s values, replacing each key with `iteratee(val, key)` — `mapValues`' sibling for keys
 * instead of values. A collision on the computed key keeps the last entry that produced it.
 * @example
 *   mapKeys({ "a": 1, "b": 2 }, function(val, key) { return key ~ val; }); // => { "a1": 1, "b2": 2 }
 */
export function mapKeys(bag is map, iteratee is function) returns map {
  var result = {};
  for (var key in keys(bag)) {
    result[iteratee(bag[key], key)] = bag[key];
  }
  return result;
}

/**
 * `bag` without the entries at `keylist` — the inverse of `pick`. `omitBy` instead drops any
 * entry for which `rule(val, key)` holds, the inverse of `pickDefined`'s spirit but with a
 * caller-supplied rule rather than a fixed "is defined" check.
 * @example
 *   omit({ "a": 1, "b": 2, "c": 3 }, ["b"]); // => { "a": 1, "c": 3 }
 */
export function omit(bag is map, keylist is array) returns map {
  var result = {};
  for (var key in keys(bag)) {
    if (! arrayIncludes(keylist, key)) { result[key] = bag[key]; }
  }
  return result;
}
export function omitBy(bag is map, rule is function) returns map {
  var result = {};
  for (var key in keys(bag)) {
    if (! rule(bag[key], key)) { result[key] = bag[key]; }
  }
  return result;
}

/**
 * `bag` flattened into `[[key, val], ...]` pairs, in `keys(bag)` order — the inverse of
 * `fromPairs` *(arrayUtils)*. FeatureScript maps have no own/inherited distinction, so this
 * covers lodash's `entries`, `entriesIn`, and `toPairsIn` as well as `toPairs`.
 * @example
 *   toPairs({ "a": 1, "b": 2 }); // => [["a", 1], ["b", 2]]
 */
export function toPairs(bag is map) returns array {
  return mapValues(keys(bag), (key, _seq) => [key, bag[key]]);
