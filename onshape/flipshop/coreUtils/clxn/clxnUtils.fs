FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
//
import(path : "66e287bede293cb227dfb89c", version : "25bf5ea59817ea0aa1737abd"); // typeUtils, for ifNil &c
import(path : "08b6ba15b8255611bafe7520", version : "1bac0c6e6336bad36c5a5a53"); // helperFuncs, for iteratee
import(path : "607f97fc690581579d1d4a08", version : "6afa9ed6ec4a413d9bf06340"); // clxnGetset, for getAt/setAt

// boolean, number, string, array, map, box, function, builtin, and undefined

/**
 * Sentinel a `forEach`/`mapValues`/etc. callback returns to stop the walk early — whatever
 * hasn't been visited yet is simply skipped, not visited-and-discarded.
 */
export enum NextStepAction {
    annotation { "Name": "Break out of the loop early" }
    BREAK
}

/**
 * How the walking functions below treat an entry whose value is `undefined` — because it's
 * missing outright, or because it's present and genuinely set to `undefined`.
 */
export enum MissingPolicy {
    annotation { "Name": "Treat set-but-undefined values the same as absent values: they should not appear in the result" }
    SKIP,
    annotation { "Name": "Treat set-but-undefined values as present (include them in results)" }
    USE_UNDEFINED
}

// == [Collection Inspection] -- hasKey

/**

/**
 * Whether `key` is present in `obj`. Unlike lodash's `has`, `key` is a single literal key or
 * index — never a dotted path — and a map only ever contains `key` when its value isn't
 * `undefined`, since FeatureScript elides one on the way in; the `missingPolicy` map overload
 * exists only for symmetry with the array one below and never changes the answer.
 *
 * For an array, `missingPolicy` decides whether an in-bounds slot holding `undefined` counts:
 * `MissingPolicy.USE_UNDEFINED` (the default) says yes; `MissingPolicy.SKIP` says no, the same as
 * `hasPresentKey`. Neither array overload accepts a negative index, unlike @see `getAt`.
 *
 * `hasPresentKey` is `hasKey` pinned to the stricter policy.
 *
 * @example
 *   hasKey({ "a": 1 }, "a");         // => true
 *   hasKey({ "a": undefined }, "a"); // => false
 *   hasKey([1, 2, 3], 2);            // => true
 *   hasKey([1, 2, 3], -1);           // => false
 *   hasKey([1, undefined, 3], 1, MissingPolicy.SKIP); // => false
 */
export function hasKey(obj is map, key is string) returns boolean {
    return (obj[key] != undefined);
}
export function hasKey(obj is map, key is string, missingPolicy is MissingPolicy) returns boolean {
    return (obj[key] != undefined); // offered for symmetry with the array case
}
export function hasPresentKey(obj is map, key is string) returns boolean {
  return hasKey(obj, key); // FS does not retain keys with undefined values
}

export function hasKey(obj is array, key is number, missingPolicy is MissingPolicy) returns boolean {
  if (missingPolicy == MissingPolicy.SKIP) { return hasPresentKey(obj, key); }
  return (key >= 0) && (key < size(obj));
}
export function hasKey(obj is array, key is number) returns boolean {
  return (key >= 0) && (key < size(obj));
}
export function hasPresentKey(obj is array, key is number) returns boolean {
  return (key >= 0) && (key < size(obj)) && (obj[key] != undefined);
}

// --

// == [Collection retrieve many] -- pick, pickDefined, arrLast

/**
 * A map with just `bag`'s entries at `keylist` — like lodash's `pick`, an absent key is simply
 * missing from the result rather than present with an `undefined` value. Each entry of `keylist`
 * is resolved via `getAt`/`setAt`, so a dotted string or key-path array reaches into a nested
 * structure and rebuilds the same nesting in the result — `pick({ "a": { "b": 1 } }, ["a.b"])` is
 * `{ "a": { "b": 1 } }`, not a flat `{ "a.b": 1 }`. This is lossy against a key that already
 * contains a literal dot, same caveat as `dotMap`/`undotMap`.
 *
 * `pickDefined` additionally drops a key whose value is `undefined` — for a map this is the same
 * result as `pick`, since a map can never hold an `undefined` value to differ over. Unlike
 * `pickBy`, the rule isn't customizable and the keys considered are exactly `keylist`, not every
 * key of `bag`. `pickDefined` only accepts a literal top-level key, not a dotted path.
 *
 * @example
 *   pick({ "a": 1, "b": 2, "c": 3 }, ["a", "c"]); // => { "a": 1, "c": 3 }
 *   pick({ "a": { "b": 1, "c": 2 } }, ["a.b"]); // => { "a": { "b": 1 } }
 *   pickDefined({ "a": 1, "b": undefined, "c": 3 }, ["a", "b", "c"]); // => { "a": 1, "c": 3 }
 */
export function pick(bag is map, keylist is array) returns map {
    var result = {};
    for (var pathSpec in keylist) {
      const val = getAt(bag, pathSpec, Sentinel.ABSENT);
      if (val != Sentinel.ABSENT) { result = setAt(result, pathSpec, val); }
    }
    return result;
}

export function pickDefined(bag is map, keylist is array) returns map {
    var result = {};
    for (var k in keylist) {
      if (bag[k] != undefined) { result[k] = bag[k]; }
    }
    return result;
}

/**
 * Last element of `arr`, or `undefined` if it's empty.
 * @example
 *   arrLast([1, 2, 3]); // => 3
 *   arrLast([]);        // => undefined
 */
export const arrLast = (function(arr is array) {
    if (size(arr) <= 0) { return undefined; }
    return arr[size(arr) - 1];
});

/**
 * First element of `arr`, or `undefined` if it's empty.
 * @example
 *   arrFirst([1, 2, 3]); // => 1
 *   arrFirst([]);        // => undefined
 */
export const arrFirst = (function(arr is array) {
    if (size(arr) <= 0) { return undefined; }
    return arr[0];
});

// --

// == [valuesAt]

/**
 * Array of `bag`'s (or `arr`'s) values at `keylist`, in that order. Unlike lodash's `at`, there's
 * no path traversal — each entry of `keylist` is a literal key or index, not a dotted path — and
 * an array index must be non-negative and in bounds.
 *
 * `missingPolicy` decides what happens at a key/index with nothing there: `USE_UNDEFINED` (the
 * default) fills the slot with `undefined`, so the result stays the same length as `keylist`;
 * `SKIP` drops the slot instead, so the result can come back shorter.
 *
 * @example
 *   valuesAt({ "a": 11, "b": 22 }, ["b", "a"]);                      // => [22, 11]
 *   valuesAt({ "a": 11, "b": 22 }, ["c"]);                           // => [undefined]
 *   valuesAt({ "a": 11, "b": 22 }, ["c", "b"], MissingPolicy.SKIP);  // => [22]
 */
export function valuesAt(arr is array, keylist is array) returns array {
  return mapValues(keylist, (seq is number, _) => hasKey(arr, seq) ? arr[seq] : undefined);
}

export function valuesAt(arr is array, keylist is array, missingPolicy is MissingPolicy) returns array {
  if (missingPolicy == MissingPolicy.USE_UNDEFINED) {
    return mapValues(keylist, (seq is number, _) => hasKey(arr, seq) ? arr[seq] : undefined);
  }
  const result = new box([]);
  forEach(keylist, (seq is number, _) => { if (hasPresentKey(arr, seq)) { boxarrPush(result, arr[seq]); } });
  return result[];
}

export function valuesAt(bag is map, keylist is array, missingPolicy is MissingPolicy) returns array {
  if (missingPolicy == MissingPolicy.USE_UNDEFINED) {
    return mapValues(keylist, (key is string, _) => bag[key]);
  }
  const result = new box([]);
  forEach(keylist, (key is string, _) => { if (bag[key] != undefined) { boxarrPush(result, bag[key]); } });
  return result[];
}

export function valuesAt(bag is map, keylist is array) returns array {
  return mapValues(keylist, (key is string, _) => bag[key]);
}

//--

// == [Collection Walking] -- mapValues, objectify, rebag

/**
 * Iterates over `bag`/`arr`, invoking `func` for each entry as `func(val, key)` (map) or
 * `func(val, seq)` (array) — the same two-argument shape every iterator in this file uses.
 * Unlike lodash's `forEach`, where an iteratee may exit early by returning `false`, iteration
 * here stops only when `func` returns `NextStepAction.BREAK`.
 *
 * `missingPolicy` (`USE_UNDEFINED`, the default, or `SKIP`) decides whether an entry whose value
 * is `undefined` gets visited at all. A `keylist` overload walks exactly those keys, in that
 * order, instead of `keys(bag)`.
 *
 * @example
 *   forEach({ "a": 1, "b": 2, "c": 3 }, function(val, key) {
 *     if (key == "b") { return NextStepAction.BREAK; }
 *   }); // visits "a" then "b"; "c" is never reached
 */
export function forEach(bag is map, keylist is array, missingPolicy is MissingPolicy, func is function) {
  if (missingPolicy == MissingPolicy.SKIP) {
      for (var key in keylist) {
        const val = bag[key];
        if (val == undefined) { continue; }
        const result = func(val, key);
        if (result == NextStepAction.BREAK) { break; }
      }
  } else {
      for (var key in keylist) {
        const val = bag[key];
        const result = func(val, key);
        if (result == NextStepAction.BREAK) { break; }
      }
  }
}

export function forEach(bag is map, missingPolicy is MissingPolicy, func is function) {
    return forEach(bag, keys(bag), MissingPolicy.USE_UNDEFINED, func);
}

export function forEach(bag is map, keylist is array, func is function) {
  for (var key in keylist) {
    const val = bag[key];
    const result = func(val, key);
    if (result == NextStepAction.BREAK) { break; }
  }
}

export function forEach(bag is map, func is function) {
  return forEach(bag, keys(bag), func);
}

export function forEach(arr is array, missingPolicy is MissingPolicy, func is function) {
  if (missingPolicy == MissingPolicy.SKIP) {
      for (var seq = 0; seq < size(arr); seq += 1) {
        if (arr[seq] == undefined) { continue; }
        const result = func(arr[seq], seq);
        if (result == NextStepAction.BREAK) { break; }
      }
  } else {
      for (var seq = 0; seq < size(arr); seq += 1) {
        const result = func(arr[seq], seq);
        if (result == NextStepAction.BREAK) { break; }
      }
  }
}

export function forEach(arr is array, func is function) {
  for (var seq = 0; seq < size(arr); seq += 1) {
    const result = func(arr[seq], seq);
    if (result == NextStepAction.BREAK) { break; }
  }
}

/**
 * `forEach`, handing `func` a third argument: a 0-based visit-count `seq`, distinct from `key`
 * for a map (a `SKIP`'d entry consumes a `keylist` slot but not a `seq` one) — `seq` fills both
 * trailing slots for an array, `func(val, seq, seq)`, matching the map callback's arity.
 */
export function forEach3(bag is map, keylist is array, missingPolicy is MissingPolicy, func is function) {
  var seq = 0;
  if (missingPolicy == MissingPolicy.SKIP) {
      for (var key in keylist) {
        const val = bag[key];
        if (val == undefined) { continue; }
        const result = func(val, key, seq);
        if (result == NextStepAction.BREAK) { break; }
        seq += 1;
      }
  } else {
      for (var key in keylist) {
        const val = bag[key];
        const result = func(val, key, seq);
        if (result == NextStepAction.BREAK) { break; }
        seq += 1;
      }
  }
}
export function forEach3(bag is map, missingPolicy is MissingPolicy, func is function) {
  return forEach3(bag, keys(bag), MissingPolicy.USE_UNDEFINED, func);
}
export function forEach3(bag is map, keylist is array, func is function) {
  var seq = 0;
  for (var key in keylist) {
    const val = bag[key];
    const result = func(val, key, seq);
    if (result == NextStepAction.BREAK) { break; }
    seq += 1;
  }
}
export function forEach3(bag is map, func is function) {
  return forEach3(bag, keys(bag), func);
}
export function forEach3(arr is array, missingPolicy is MissingPolicy, func is function) {
  if (missingPolicy == MissingPolicy.SKIP) {
      for (var seq = 0; seq < size(arr); seq += 1) {
        if (arr[seq] == undefined) { continue; }
        const result = func(arr[seq], seq, seq);
        if (result == NextStepAction.BREAK) { break; }
      }
  } else {
      for (var seq = 0; seq < size(arr); seq += 1) {
        const result = func(arr[seq], seq, seq);
        if (result == NextStepAction.BREAK) { break; }
      }
  }
}
export function forEach3(arr is array, func is function) {
  for (var seq = 0; seq < size(arr); seq += 1) {
    const result = func(arr[seq], seq, seq);
    if (result == NextStepAction.BREAK) { break; }
  }
}

/**
 * `bag`/`arr` with every value replaced by `func`'s result. Lodash splits this into two
 * functions — `mapValues` keeps an object's keys, `map` returns a new array — unified here under
 * one dispatch, since they share every other behavior: `func` is `func(val, key)` for a map or
 * `func(val, seq)` for an array, the same two-argument shape every iterator in this file uses.
 * `mapValues3` instead hands `func` all three of `val`, `key`, and a 0-based visit-count `seq`
 * (for an array, `seq` fills both slots) when a map form needs to distinguish visit order from
 * key order. The `keylist` and `missingPolicy` overloads follow `forEach`'s rules: a `keylist`
 * walks exactly those keys, and `missingPolicy` decides whether an `undefined` value is mapped
 * (`USE_UNDEFINED`, the default) or its key dropped from the result entirely (`SKIP`). `func` is
 * coerced through `iteratee`, so a property-path string, `[path, srcValue]` array, or partial-
 * match map works in place of a literal function — `mapValues(users, 'name')` extracts a `name`
 * field from each.
 *
 * @example
 *   mapValues({ "fred": 40, "pebbles": 1 }, function(age) { return age * 2; });
 *   // => { "fred": 80, "pebbles": 2 }
 *   mapValues([4, 8], function(n) { return n * n; }); // => [16, 64]
 */
export function mapValues(bag is map, keylist is array, missingPolicy is MissingPolicy, func) returns map {
  const fn = iteratee(func);
  const result = new box({});
  forEach(bag, keylist, missingPolicy, (val, key is string) => { result[][key] = fn(val, key); });
  return result[];
}
export function mapValues3(bag is map, keylist is array, missingPolicy is MissingPolicy, func) returns map {
  const fn = iteratee(func);
  const result = new box({});
  forEach3(bag, keylist, missingPolicy, (val, key is string, seq is number) => { result[][key] = fn(val, key, seq); });
  return result[];
}

export function mapValues(bag is map, keylist is array, func) returns map {
  const fn = iteratee(func);
  const result = new box({});
  forEach(bag, keylist, (val, key is string) => { result[][key] = fn(val, key); });
  return result[];
}
export function mapValues3(bag is map, keylist is array, func) returns map {
  const fn = iteratee(func);
  const result = new box({});
  forEach3(bag, keylist, (val, key is string, seq is number) => { result[][key] = fn(val, key, seq); });
  return result[];
}
export function mapValues(bag is map, missingPolicy is MissingPolicy, func) returns map {
  return mapValues(bag, keys(bag), missingPolicy, func);
}
export function mapValues3(bag is map, missingPolicy is MissingPolicy, func) returns map {
  return mapValues3(bag, keys(bag), missingPolicy, func);
}
export function mapValues(bag is map, func) returns map {
  return mapValues(bag, keys(bag), func);
}
export function mapValues3(bag is map, func) returns map {
  return mapValues3(bag, keys(bag), func);
}

export function mapValues(arr is array, missingPolicy is MissingPolicy, func) returns array {
  const fn = iteratee(func);
  var result = new box(makeArray(size(arr)));
  forEach(arr, missingPolicy, (val, seq is number) => {
      result[][seq] = fn(val, seq);
  });
  return result[];
}

export function mapValues3(arr is array, missingPolicy is MissingPolicy, func) returns array {
  const fn = iteratee(func);
  var result = new box(makeArray(size(arr)));
  forEach(arr, missingPolicy, (val, seq is number) => {
      result[][seq] = fn(val, seq, seq);
  });
  return result[];
}

export function mapValues(arr is array, func) returns array {
  const fn = iteratee(func);
  var result = makeArray(size(arr));
  for (var seq = 0; seq < size(arr); seq += 1) {
    result[seq] = fn(arr[seq], seq);
  }
  return result;
}
export function mapValues3(arr is array, func) returns array {
  const fn = iteratee(func);
  var result = makeArray(size(arr));
  for (var seq = 0; seq < size(arr); seq += 1) {
    result[seq] = fn(arr[seq], seq, seq);
  }
  return result;
}

/**
 * Creates a map keyed by each element of `arr` itself, with the value at that key set to
 * `func(val, seq)` — the last element responsible for a given key wins on a collision. This is
 * `_.keyBy` with the key and value roles swapped: lodash's `keyBy` keys by `iteratee(value)` and
 * keeps `value` itself, where `objectify` keys by `value` and lets `func` compute the result.
 *
 * @example
 *   objectify(["a", "b", "c"], function(val, seq) { return seq; }); // => { "a": 0, "b": 1, "c": 2 }
 *   objectify(["x", "x"],      function(val, seq) { return seq; }); // => { "x": 1 }
 */
export function objectify(arr is array, func is function) returns map {
    return rebag(arr, (val, seq is number) => [val, func(val, seq)]);
}

/**
 * Rebuilds a map (from a map or an array) by asking `func(val, key?)` — `func(val, seq)` for an
 * array, `func(val, key)` for a map, the same two-argument shape every iterator in this file
 * uses — for the `[newKey, newVal]` pair each entry becomes; an entry where `func` returns
 * `undefined` (rather than a pair) is dropped rather than written under an `undefined` key.
 * Where two entries land on the same `newKey`, the later one wins — `keys(bag)` order for a map,
 * index order for an array.
 */
export function rebag(arr is array, func is function) returns map {
    var result = new box({});
    forEach(arr, (val, seq is number) => { const kv = func(val, seq); if (kv != undefined) { result[][kv[0]] = kv[1]; } });
    return result[];
}
export function rebag(bag is map, func is function) returns map {
    var result = new box({});
    forEach(bag, (val, key is string) => { const kv = func(val, key); if (kv != undefined) { result[][kv[0]] = kv[1]; } });
    return result[];
}
// --

// == [Box Array Utils]

/**
 * Mutates the array inside `arrRef` in place and returns `val` — a pair so a `forEach`/
 * `mapValues` callback can accumulate into an outer array without a `var` the closure would
 * have to capture and reassign. `boxarrPush` appends; `boxarrUnshift` prepends.
 */
export function boxarrPush(arrRef is box, val) {
  arrRef[] = append(arrRef[], val);
  return val;
}
export function boxarrUnshift(arrRef is box, val) {
  arrRef[] = concatenateArrays([val], arrRef[]);
  return val;
}

//--

// == [Lodash Collection ports] -- countBy, find, findLast, flatMap*, forEachRight, groupBy, partition, reduceRight, reject

/**
 * Map of `iterateeSpec(val, seq)` (array) / `iterateeSpec(val, key)` (map) results to how many
 * elements of `arr`/`bag` produced that result — `seq`/`key` match `forEach`'s split between
 * index and key. `iterateeSpec` is coerced through `iteratee`, so a property-path string,
 * `[path, srcValue]` array, or partial-match map works in place of a literal function.
 * @example
 *   countBy([1, 2, 3, 4], (val, _seq) => (val % 2 == 0) ? "even" : "odd"); // => { "odd": 2, "even": 2 }
 *   countBy({ a: 1, b: 2 }, (val, _key) => (val % 2 == 0) ? "even" : "odd"); // => { "odd": 1, "even": 1 }
 */
export function countBy(arr is array, iterateeSpec) returns map {
  const fn = iteratee(iterateeSpec);
  var result = {};
  for (var seq = 0; seq < size(arr); seq += 1) {
    const key = fn(arr[seq], seq);
    result[key] = ifNil(result[key], 0) + 1;
  }
  return result;
}
export function countBy(bag is map, iterateeSpec) returns map {
  const fn = iteratee(iterateeSpec);
  var result = {};
  for (var key in keys(bag)) {
    const groupKey = fn(bag[key], key);
    result[groupKey] = ifNil(result[groupKey], 0) + 1;
  }
  return result;
}

/**
 * First element of `bag`/`arr` for which `rule` holds, or `undefined` if none does — the array
 * form dereferences `arrayUtils`' `findIndex`; the map form walks `keys(bag)` and hands `rule`
 * the key as a second argument. `findLast` scans from the end instead. `rule` is coerced through
 * `iteratee`, so a property-path string, `[path, srcValue]` array, or partial-match map works in
 * place of a literal function.
 * @example
 *   find([1, 2, 3], (val) => val > 1); // => 2
 *   find({ a: 1, b: 2 }, (val, key) => key == "b"); // => 2
 */
export function find(arr is array, rule) {
  const seq = findIndex(arr, iteratee(rule));
  return (seq == -1) ? undefined : arr[seq];
}
export function find(bag is map, rule) {
  const fn = iteratee(rule);
  for (var key in keys(bag)) {
    if (fn(bag[key], key)) { return bag[key]; }
  }
  return undefined;
}
/** `find`, scanning from the end — `findLastIndex`, dereferenced, or `keys(bag)` walked in reverse. */
export function findLast(arr is array, rule) {
  const seq = findLastIndex(arr, iteratee(rule));
  return (seq == -1) ? undefined : arr[seq];
}
export function findLast(bag is map, rule) {
  const fn = iteratee(rule);
  const keylist = keys(bag);
  for (var seq = size(keylist) - 1; seq >= 0; seq -= 1) {
    const key = keylist[seq];
    if (fn(bag[key], key)) { return bag[key]; }
  }
  return undefined;
}

/**
 * `bag`/`arr` mapped through `iterateeSpec`, then flattened one level — @see `arrayUtils`'
 * `flatten`. The map form walks `keys(bag)`, hands `iterateeSpec` the key as a second argument,
 * and always returns an array, same as lodash's collection form. `flatMapDeep`/`flatMapDepth`
 * flatten fully / to `depth` levels instead. `iterateeSpec` is coerced through `iteratee`, so a
 * property-path string, `[path, srcValue]` array, or partial-match map works in place of a
 * literal function.
 * @example
 *   flatMap([1, 2], (val) => [val, val]); // => [1, 1, 2, 2]
 *   flatMap({ a: 1, b: 2 }, (val) => [val, val]); // => [1, 1, 2, 2]
 */
export function flatMap(arr is array, iterateeSpec) returns array {
  return flatten(mapValues(arr, iteratee(iterateeSpec)));
}
export function flatMap(bag is map, iterateeSpec) returns array {
  const fn = iteratee(iterateeSpec);
  return flatten(mapValues(keys(bag), (key is string, _seq is number) => fn(bag[key], key)));
}
export function flatMapDeep(arr is array, iterateeSpec) returns array {
  return flattenDeep(mapValues(arr, iteratee(iterateeSpec)));
}
export function flatMapDeep(bag is map, iterateeSpec) returns array {
  const fn = iteratee(iterateeSpec);
  return flattenDeep(mapValues(keys(bag), (key is string, _seq is number) => fn(bag[key], key)));
}
export function flatMapDepth(arr is array, iterateeSpec, depth is number) returns array {
  return flattenDepth(mapValues(arr, iteratee(iterateeSpec)), depth);
}
export function flatMapDepth(bag is map, iterateeSpec, depth is number) returns array {
  const fn = iteratee(iterateeSpec);
  return flattenDepth(mapValues(keys(bag), (key is string, _seq is number) => fn(bag[key], key)), depth);
}

/**
 * `forEach`, back-to-front — otherwise identical, including the `NextStepAction.BREAK` early
 * exit. The map form walks `keys(bag)` in reverse and hands `func` the key where the array form
 * hands the index.
 * @example
 *   forEachRight([1, 2, 3], function(val, seq) { debug(context, val); }); // visits 3, then 2, then 1
 *   forEachRight({ a: 1, b: 2 }, function(val, key) { debug(context, key); }); // visits "b", then "a"
 */
export function forEachRight(arr is array, func is function) {
  for (var seq = size(arr) - 1; seq >= 0; seq -= 1) {
    const result = func(arr[seq], seq);
    if (result == NextStepAction.BREAK) { break; }
  }
}
export function forEachRight(bag is map, func is function) {
  const keylist = keys(bag);
  for (var seq = size(keylist) - 1; seq >= 0; seq -= 1) {
    const key = keylist[seq];
    const result = func(bag[key], key);
    if (result == NextStepAction.BREAK) { break; }
  }
}

/**
 * Map of `iterateeSpec(val, seq)` (array) / `iterateeSpec(val, key)` (map) results to the
 * elements of `arr`/`bag` that produced each one, via std's `insertIntoMapOfArrays`.
 * `iterateeSpec` is coerced through `iteratee`, so a property-path string, `[path, srcValue]`
 * array, or partial-match map works in place of a literal function.
 * @example
 *   groupBy([1, 2, 3, 4], (val, _seq) => (val % 2 == 0) ? "even" : "odd");
 *   // => { "odd": [1, 3], "even": [2, 4] }
 */
export function groupBy(arr is array, iterateeSpec) returns map {
  const fn = iteratee(iterateeSpec);
  var result = {};
  for (var seq = 0; seq < size(arr); seq += 1) {
    const val = arr[seq];
    result = insertIntoMapOfArrays(result, fn(val, seq), val);
  }
  return result;
}
export function groupBy(bag is map, iterateeSpec) returns map {
  const fn = iteratee(iterateeSpec);
  var result = {};
  for (var key in keys(bag)) {
    result = insertIntoMapOfArrays(result, fn(bag[key], key), bag[key]);
  }
  return result;
}

/**
 * `[passed, failed]` — `bag`/`arr` split into the elements for which `rule` holds and the ones
 * for which it doesn't, keeping visiting order. `rule` gets `(val, seq)` (array) or `(val, key)`
 * (map); the map form returns values only, same as lodash's collection form. `rule` is coerced
 * through `iteratee`, so a property-path string, `[path, srcValue]` array, or partial-match map
 * works in place of a literal function.
 * @example
 *   partition([1, 2, 3, 4], (val, _seq) => val % 2 == 0); // => [[2, 4], [1, 3]]
 */
export function partition(arr is array, rule) returns array {
  const fn = iteratee(rule);
  var passed = [];
  var failed = [];
  for (var seq = 0; seq < size(arr); seq += 1) {
    const val = arr[seq];
    if (fn(val, seq)) {
      passed = append(passed, val);
    } else {
      failed = append(failed, val);
    }
  }
  return [passed, failed];
}
export function partition(bag is map, rule) returns array {
  const fn = iteratee(rule);
  var passed = [];
  var failed = [];
  for (var key in keys(bag)) {
    if (fn(bag[key], key)) {
      passed = append(passed, bag[key]);
    } else {
      failed = append(failed, bag[key]);
    }
  }
  return [passed, failed];
}

/**
 * `bag`/`arr` reduced right-to-left through `foldFunction(accumulator, val, seq|key)` — a manual
 * reversed walk rather than std's `foldArray`, which only ever hands `foldFunction` two arguments,
 * so that both forms uniformly offer the index/key as a third argument.
 * @example
 *   reduceRight([1, 2, 3], "", function(acc, val, _seq) { return acc ~ val; }); // => "321"
 */
export function reduceRight(arr is array, seed, foldFunction is function) {
  var acc = seed;
  for (var seq = size(arr) - 1; seq >= 0; seq -= 1) {
    acc = foldFunction(acc, arr[seq], seq);
  }
  return acc;
}
export function reduceRight(bag is map, seed, foldFunction is function) {
  var acc = seed;
  for (var key in reverse(keys(bag))) {
    acc = foldFunction(acc, bag[key], key);
  }
  return acc;
}
/** `reduceRight`, seeded from the last-visited element — `undefined` for an empty `arr`/`bag`. */
export function reduceRight(arr is array, foldFunction is function) {
  if (size(arr) == 0) { return undefined; }
  var acc = arr[size(arr) - 1];
  for (var seq = size(arr) - 2; seq >= 0; seq -= 1) {
    acc = foldFunction(acc, arr[seq], seq);
  }
  return acc;
}
export function reduceRight(bag is map, foldFunction is function) {
  const keylist = reverse(keys(bag));
  if (size(keylist) == 0) { return undefined; }
  var acc = bag[keylist[0]];
  for (var seq = 1; seq < size(keylist); seq += 1) {
    const key = keylist[seq];
    acc = foldFunction(acc, bag[key], key);
  }
  return acc;
}

/**
 * Elements of `bag`/`arr` for which `rule` does *not* hold — the inverse of `filter` *(std)*, whose
 * `filterFunction` only ever gets one argument, so this walks `arr` by index instead in order to
 * hand `rule` `(val, seq)`; the map form gets `(val, key)` and returns values only. `rule` is
 * coerced through `iteratee`, so a property-path string, `[path, srcValue]` array, or partial-
 * match map works in place of a literal function.
 * @example
 *   reject([1, 2, 3, 4], (val, _seq) => val % 2 == 0); // => [1, 3]
 */
export function reject(arr is array, rule) returns array {
  const fn = iteratee(rule);
  var result = [];
  for (var seq = 0; seq < size(arr); seq += 1) {
    if (! fn(arr[seq], seq)) { result = append(result, arr[seq]); }
  }
  return result;
}
export function reject(bag is map, rule) returns array {
  const fn = iteratee(rule);
  var result = [];
  for (var key in keys(bag)) {
    if (! fn(bag[key], key)) { result = append(result, bag[key]); }
  }
  return result;
}
//--

// == [Array Utils]

/**
 * Whether `target` appears anywhere in `bag`'s values or `arr`'s elements. Unlike lodash's
 * `includes`, there's no substring search on a string — only the array and map collection forms
 * — and comparison is plain `==`, which is deep structural equality on maps and arrays.
 * @example
 *   arrayIncludes([1, 2, 3], 2);              // => true
 *   arrayIncludes([{ "a": 1 }], { "a": 1 });  // => true
 *   arrayIncludes({ "x": 1, "y": 2 }, 2);     // => true
 */
export function arrayIncludes(arr is array, target) returns boolean {
  for (var item in arr) {
    if (item == target) { return true; }
  }
  return false;
}
export function arrayIncludes(bag is map, target) returns boolean {
  for (var key in keys(bag)) {
    if (bag[key] == target) { return true; }
  }
  return false;
}

/**
 * Splits `arr` into groups of `chunkSize` elements each; the last group holds whatever's left
 * over. `chunkSize < 1` returns an empty array.
 * @example
 *   chunk(["a", "b", "c", "d"], 2); // => [["a", "b"], ["c", "d"]]
 *   chunk(["a", "b", "c", "d"], 3); // => [["a", "b", "c"], ["d"]]
 */
export function chunk(arr is array, chunkSize is number) returns array {
  if (chunkSize < 1) { return []; }
  var result = [];
  for (var beg = 0; beg < size(arr); beg += chunkSize) {
    result = append(result, subArray(arr, beg, min(beg + chunkSize, size(arr))));
  }
  return result;
}

/**
 * `arr` with every falsey element removed, per typeUtils' `truthy` — narrower than lodash's own
 * falsey set, since FeatureScript treats only `undefined` and `false` as falsey (`0` and `""`
 * stay truthy).
 * @example
 *   compact([0, 1, false, 2, "", 3]); // => [0, 1, 2, "", 3]
 *   compact([0, false, undefined]);   // => [0]
 */
export function compact(arr is array) returns array {
  return filter(arr, (val) => truthy(val));
}

/**
 * `arr`'s values that don't appear in `excludeArr`, order taken from `arr`. Lodash's `difference`
 * takes the exclusion values as trailing variadic arrays; FeatureScript has no varargs, so they're
 * a single array here — call with `concatenateArrays([...])` to exclude from several sources at
 * once.
 * @example
 *   difference([2, 1], [2, 3]); // => [1]
 */
export function difference(arr is array, excludeArr is array) returns array {
  return filter(arr, (val) => (! arrayIncludes(excludeArr, val)));
}

/**
 * `difference`, comparing `arr` and `excludeArr` by `iterateeSpec(val, seq)` instead of `val`
 * itself — the same two-argument shape every iterator in this file uses; `filter` *(std)* only
 * ever hands a callback one argument, so this walks `arr` by index instead. `iterateeSpec` is
 * coerced through `iteratee`, so a property-path string, `[path, srcValue]` array, or
 * partial-match map works in place of a literal function.
 * @example
 *   differenceBy([2.1, 1.2], [2.3, 3.4], (val, _seq) => floor(val)); // => [1.2]
 */
export function differenceBy(arr is array, excludeArr is array, iterateeSpec) returns array {
  const fn = iteratee(iterateeSpec);
  const excludeKeys = mapValues(excludeArr, fn);
  var result = [];
  for (var seq = 0; seq < size(arr); seq += 1) {
    const val = arr[seq];
    if (! arrayIncludes(excludeKeys, fn(val, seq))) { result = append(result, val); }
  }
  return result;
}

/**
 * `difference`, comparing `arr` and `excludeArr` with `comparator(val, other)` instead of `==` —
 * the same argument order every comparator in this file uses.
 * @example
 *   differenceWith([{ "x": 1 }, { "x": 2 }], [{ "x": 1 }], (aa, bb) => aa.x == bb.x);
 *   // => [{ "x": 2 }]
 */
export function differenceWith(arr is array, excludeArr is array, comparator is function) returns array {
  return filter(arr, (val) => (! any(excludeArr, (other) => comparator(val, other))));
}

/**
 * `arr` with the first `dropCount` elements removed; `dropCount <= 0` returns `arr` unchanged.
 * @example
 *   drop([1, 2, 3], 2); // => [3]
 *   drop([1, 2, 3], 5); // => []
 */
export function drop(arr is array, dropCount is number) returns array {
  return subArray(arr, clamp(dropCount, 0, size(arr)));
}
export function drop(arr is array) returns array { return drop(arr, 1); }

/**
 * `arr` with the last `dropCount` elements removed; `dropCount <= 0` returns `arr` unchanged.
 * @example
 *   dropRight([1, 2, 3], 2); // => [1]
 *   dropRight([1, 2, 3], 5); // => []
 */
export function dropRight(arr is array, dropCount is number) returns array {
  return subArray(arr, 0, size(arr) - clamp(dropCount, 0, size(arr)));
}
export function dropRight(arr is array) returns array { return dropRight(arr, 1); }

/**
 * `arr` with elements dropped from the end for as long as `rule` holds; the first
 * (rightmost-scanned) element `rule` rejects, and everything before it, is kept.
 * @example
 *   dropRightWhile([1, 2, 3, 4], (val) => val > 2); // => [1, 2]
 */
export function dropRightWhile(arr is array, rule is function) returns array {
  var seq = size(arr);
  for (; seq > 0; seq -= 1) {
    if (! rule(arr[seq - 1], seq)) { break; }
  }
  return subArray(arr, 0, seq);
}
export function dropRightWhile(arr is array) returns array { return dropRightWhile(arr, curry2to1(truthy)); }

/**
 * `arr` with elements dropped from the beginning for as long as `rule` holds.
 * @example
 *   dropWhile([1, 2, 3, 4], (val) => val < 3); // => [3, 4]
 */
export function dropWhile(arr is array, rule is function) returns array {
  var seq = 0;
  for (; seq < size(arr); seq += 1) {
    if (! rule(arr[seq], seq)) { break; }
  }
  return subArray(arr, seq);
}
export function dropWhile(arr is array) returns array { return dropWhile(arr, curry2to1(truthy)); }

/**
 * Index of the first element of `arr` for which `rule` holds, or `-1` if none does.
 * @example
 *   findIndex([1, 2, 3], (val) => val > 1); // => 1
 *   findIndex([1, 2, 3], (val) => val > 9); // => -1
 */
export function findIndex(arr is array, rule is function) returns number {
  for (var seq = 0; seq < size(arr); seq += 1) {
    if (rule(arr[seq], seq)) { return seq; }
  }
  return -1;
}

/**
 * `findIndex`, scanning from the end: index of the last element of `arr` for which `rule`
 * holds, or `-1` if none does.
 * @example
 *   findLastIndex([1, 2, 3], (val) => val < 3); // => 1
 */
export function findLastIndex(arr is array, rule is function) returns number {
  for (var seq = size(arr) - 1; seq >= 0; seq -= 1) {
    if (rule(arr[seq], seq)) { return seq; }
  }
  return -1;
}

/**
 * `arr` with one level of array nesting removed. @see `flattenDeep`, `flattenDepth`.
 * @example
 *   flatten([1, [2, [3, [4]], 5]]); // => [1, 2, [3, [4]], 5]
 */
export function flatten(arr is array) returns array {
  return flattenDepth(arr, 1);
}

/**
 * `arr`, with every level of array nesting removed.
 * @example
 *   flattenDeep([1, [2, [3, [4]], 5]]); // => [1, 2, 3, 4, 5]
 */
export function flattenDeep(arr is array) returns array {
  var result = [];
  for (var val in arr) {
    if (val is array) {
      result = concatenateArrays(result, flattenDeep(val));
    } else {
      result = append(result, val);
    }
  }
  return result;
}

/**
 * `arr` with up to `depth` levels of array nesting removed; `depth <= 0` returns `arr` unchanged.
 * @example
 *   flattenDepth([1, [2, [3, [4]], 5]], 2); // => [1, 2, 3, [4], 5]
 */
export function flattenDepth(arr is array, depth is number) returns array {
  if (depth <= 0) { return arr; }
  var result = [];
  for (var val in arr) {
    if (val is array) {
      result = concatenateArrays(result, flattenDepth(val, depth - 1));
    } else {
      result = append(result, val);
    }
  }
  return result;
}

/**
 * Map built from `pairs` — `[[key, val], ...]` — the inverse of iterating a map's entries.
 * A repeated key keeps its last pair's value.
 * @example
 *   fromPairs([["a", 1], ["b", 2]]); // => { "a": 1, "b": 2 }
 */
export function fromPairs(pairs is array) returns map {
  var result = {};
  for (var pair in pairs) {
    result[pair[0]] = pair[1];
  }
  return result;
}

/**
 * `arr` without its last element; `[]` for an empty or single-element `arr`.
 * @example
 *   initial([1, 2, 3]); // => [1, 2]
 */
export function initial(arr is array) returns array {
  return subArray(arr, 0, max(size(arr) - 1, 0));
}

/**
 * Values present in every array of `arrList`, deduplicated, ordered as they occur in
 * `arrList[0]`.
 * @example
 *   intersection([[2, 1], [2, 3], [1, 2]]); // => [2]
 */
export function intersection(arrList is array) returns array {
  if (size(arrList) == 0) { return []; }
  const first = arrList[0];
  const rest = subArray(arrList, 1);
  const kept = filter(first, (val) => all(rest, (other is array) => arrayIncludes(other, val)));
  return deduplicate(kept);
}

/**
 * `intersection`, comparing elements by `iterateeSpec(val, seq)` instead of `val` itself — the
 * same two-argument shape every iterator in this file uses; `filter` *(std)* only ever hands a
 * callback one argument, so `first` is walked by index instead. `iterateeSpec` is coerced through
 * `iteratee`, so a property-path string, `[path, srcValue]` array, or partial-match map works in
 * place of a literal function.
 * @example
 *   intersectionBy([[2.1, 1.2], [2.3, 3.4]], (val, _seq) => floor(val)); // => [2.1]
 */
export function intersectionBy(arrList is array, iterateeSpec) returns array {
  if (size(arrList) == 0) { return []; }
  const fn = iteratee(iterateeSpec);
  const first = arrList[0];
  const restKeys = mapValues(subArray(arrList, 1), (other is array, _seq is number) => mapValues(other, fn));
  var kept = [];
  for (var seq = 0; seq < size(first); seq += 1) {
    const val = first[seq];
    if (all(restKeys, (otherKeys is array) => arrayIncludes(otherKeys, fn(val, seq)))) { kept = append(kept, val); }
  }
  return uniqBy(kept, fn);
}

/**
 * `intersection`, comparing elements with `comparator(val, other)` instead of `==`.
 * @example
 *   intersectionWith([[{ "x": 1 }, { "x": 2 }], [{ "x": 2 }]], (aa, bb) => aa.x == bb.x);
 *   // => [{ "x": 2 }]
 */
export function intersectionWith(arrList is array, comparator is function) returns array {
  if (size(arrList) == 0) { return []; }
  const first = arrList[0];
  const rest = subArray(arrList, 1);
  const kept = filter(first, (val) => all(rest, (other is array) => any(other, (otherVal) => comparator(val, otherVal))));
  return uniqWith(kept, comparator);
}

/**
 * Index of the last occurrence of `val` in `arr`, searching from the end, or `-1` if absent.
 * @example
 *   lastIndexOf([1, 2, 1], 1); // => 2
 */
export function lastIndexOf(arr is array, val) returns number {
  for (var seq = size(arr) - 1; seq >= 0; seq -= 1) {
    if (arr[seq] == val) { return seq; }
  }
  return -1;
}

/**
 * Element of `arr` for which `iterateeSpec(val, seq)` is greatest, or `undefined` for an empty
 * `arr` — std's array `max` picks the greatest value itself; this picks the element behind the
 * greatest *computed* value. `iterateeSpec` is coerced through `iteratee`, so a property-path
 * string, `[path, srcValue]` array, or partial-match map works in place of a literal function.
 * @example
 *   maxBy([{ "n": 1 }, { "n": 3 }, { "n": 2 }], (val) => val.n); // => { "n": 3 }
 */
export function maxBy(arr is array, iterateeSpec) {
  const fn = iteratee(iterateeSpec);
  var bestVal = undefined;
  var bestKey = undefined;
  for (var seq = 0; seq < size(arr); seq += 1) {
    const val = arr[seq];
    const key = fn(val, seq);
    if (bestKey == undefined || key > bestKey) {
      bestVal = val;
      bestKey = key;
    }
  }
  return bestVal;
}

/**
 * Average of `iterateeSpec(val, seq)` across `arr` — `average` *(std)*, mapped via `mapValues`,
 * which also supplies `mapValues`' own `iteratee` coercion: a property-path string,
 * `[path, srcValue]` array, or partial-match map works in place of a literal function.
 * @example
 *   meanBy([{ "n": 2 }, { "n": 4 }], (val) => val.n); // => 3
 */
export function meanBy(arr is array, iterateeSpec) {
  return average(mapValues(arr, iterateeSpec));
}

/**
 * `maxBy`'s counterpart: element of `arr` for which `iterateeSpec(val, seq)` is least, or
 * `undefined` for an empty `arr`. `iterateeSpec` is coerced through `iteratee`, so a
 * property-path string, `[path, srcValue]` array, or partial-match map works in place of a
 * literal function.
 * @example
 *   minBy([{ "n": 1 }, { "n": 3 }, { "n": 2 }], (val) => val.n); // => { "n": 1 }
 */
export function minBy(arr is array, iterateeSpec) {
  const fn = iteratee(iterateeSpec);
  var bestVal = undefined;
  var bestKey = undefined;
  for (var seq = 0; seq < size(arr); seq += 1) {
    const val = arr[seq];
    const key = fn(val, seq);
    if (bestKey == undefined || key < bestKey) {
      bestVal = val;
      bestKey = key;
    }
  }
  return bestVal;
}
/**
 * Element of `arr` at `seq`; a negative `seq` counts back from the end. `undefined` if `seq`,
 * after that adjustment, is out of bounds.
 * @example
 *   nth([1, 2, 3], 1);  // => 2
 *   nth([1, 2, 3], -1); // => 3
 */
export function nth(arr is array, seq is number) {
  const idx = (seq < 0) ? (size(arr) + seq) : seq;
  if (idx < 0 || idx >= size(arr)) { return undefined; }
  return arr[idx];
}

/**
 * Sum of `iterateeSpec(val, seq)` across `arr` — `sum` *(std)*, mapped via `mapValues`, which
 * also supplies `mapValues`' own `iteratee` coercion: a property-path string,
 * `[path, srcValue]` array, or partial-match map works in place of a literal function.
 * @example
 *   sumBy([{ "n": 2 }, { "n": 4 }], (val) => val.n); // => 6
 */
export function sumBy(arr is array, iterateeSpec) {
  return sum(mapValues(arr, iterateeSpec));
}


/**
 * `arr` without its first element; `[]` for an empty or single-element `arr`.
 * @example
 *   tail([1, 2, 3]); // => [2, 3]
 */
export function tail(arr is array) returns array {
  return subArray(arr, min(1, size(arr)));
}

/**
 * First `takeCount` elements of `arr`; `takeCount <= 0` returns `[]`. With no `takeCount` given,
 * defaults to `1`, matching `drop`'s own default.
 * @example
 *   take([1, 2, 3], 2); // => [1, 2]
 *   take([1, 2, 3]);    // => [1]
 */
export function take(arr is array, takeCount is number) returns array {
  return subArray(arr, 0, clamp(takeCount, 0, size(arr)));
}
export function take(arr is array) returns array { return take(arr, 1); }

/**
 * Last `takeCount` elements of `arr`; `takeCount <= 0` returns `[]`. With no `takeCount` given,
 * defaults to `1`, matching `dropRight`'s own default.
 * @example
 *   takeRight([1, 2, 3], 2); // => [2, 3]
 *   takeRight([1, 2, 3]);    // => [3]
 */
export function takeRight(arr is array, takeCount is number) returns array {
  return subArray(arr, size(arr) - clamp(takeCount, 0, size(arr)));
}
export function takeRight(arr is array) returns array { return takeRight(arr, 1); }

/**
 * Elements taken from the end of `arr` for as long as `rule` holds. With no `rule` given,
 * defaults to `truthy`, matching `dropRightWhile`'s own default.
 * @example
 *   takeRightWhile([1, 2, 3, 4], (val) => val > 2); // => [3, 4]
 */
export function takeRightWhile(arr is array, rule is function) returns array {
  var seq = size(arr);
  for (; seq > 0; seq -= 1) {
    if (! rule(arr[seq - 1], seq)) { break; }
  }
  return subArray(arr, seq);
}
export function takeRightWhile(arr is array) returns array { return takeRightWhile(arr, curry2to1(truthy)); }

/**
 * Elements taken from the beginning of `arr` for as long as `rule` holds. With no `rule` given,
 * defaults to `truthy`, matching `dropWhile`'s own default.
 * @example
 *   takeWhile([1, 2, 3, 4], (val) => val < 3); // => [1, 2]
 */
export function takeWhile(arr is array, rule is function) returns array {
  var seq = 0;
  for (; seq < size(arr); seq += 1) {
    if (! rule(arr[seq], seq)) { break; }
  }
  return subArray(arr, 0, seq);
}
export function takeWhile(arr is array) returns array { return takeWhile(arr, curry2to1(truthy)); }

/**
 * Deduplicated concatenation of every array in `arrList`, ordered by first occurrence.
 * @example
 *   union([[2], [1, 2], [2, 3]]); // => [2, 1, 3]
 */
export function union(arrList is array) returns array {
  return deduplicate(concatenateArrays(arrList));
}

/**
 * `union`, deduplicating by `iterateeSpec(val)` instead of `val` itself. `iterateeSpec` is
 * coerced through `iteratee` (by `uniqBy`, which this delegates to), so a property-path string,
 * `[path, srcValue]` array, or partial-match map works in place of a literal function.
 * @example
 *   unionBy([[2.1], [1.2, 2.3]], (val) => floor(val)); // => [2.1, 1.2]
 */
export function unionBy(arrList is array, iterateeSpec) returns array {
  return uniqBy(concatenateArrays(arrList), iterateeSpec);
}

/**
 * `union`, deduplicating with `comparator(val, kept)` instead of `==`.
 * @example
 *   unionWith([[{ "x": 1 }], [{ "x": 1 }, { "x": 2 }]], (aa, bb) => aa.x == bb.x);
 *   // => [{ "x": 1 }, { "x": 2 }]
 */
export function unionWith(arrList is array, comparator is function) returns array {
  return uniqWith(concatenateArrays(arrList), comparator);
}

/**
 * `arr` with duplicate elements removed, keeping the first occurrence — like std's
 * `deduplicate`, but comparing `iterateeSpec(val, seq)` instead of `val` itself — the same
 * two-argument shape every iterator in this file uses. `iterateeSpec` is coerced through
 * `iteratee`, so a property-path string, `[path, srcValue]` array, or partial-match map works in
 * place of a literal function.
 * @example
 *   uniqBy([2.1, 1.2, 2.3], (val, _seq) => floor(val)); // => [2.1, 1.2]
 */
export function uniqBy(arr is array, iterateeSpec) returns array {
  const fn = iteratee(iterateeSpec);
  var seenKeys = [];
  var result = [];
  for (var seq = 0; seq < size(arr); seq += 1) {
    const val = arr[seq];
    const key = fn(val, seq);
    if (! arrayIncludes(seenKeys, key)) {
      seenKeys = append(seenKeys, key);
      result = append(result, val);
    }
  }
  return result;
}

/**
 * `arr` with duplicate elements removed, keeping the first occurrence, where two elements count
 * as duplicates when `comparator(val, kept)` is `true` — the same argument order every
 * comparator in this file uses.
 * @example
 *   uniqWith([{ "x": 1 }, { "x": 1 }, { "x": 2 }], (aa, bb) => aa.x == bb.x);
 *   // => [{ "x": 1 }, { "x": 2 }]
 */
export function uniqWith(arr is array, comparator is function) returns array {
  var result = [];
  for (var val in arr) {
    if (! any(result, (kept) => comparator(val, kept))) {
      result = append(result, val);
    }
  }
  return result;
}

/**
 * Inverse of `zip` *(std)* — ungroups `arr`'s rows back into columns. `zip`'s grouping is its own
 * inverse (transposing rows and columns twice returns the original shape), so `unzip` is just
 * `zip` under lodash's name for the reverse direction.
 * @example
 *   unzip([["a", 1, true], ["b", 2, false]]); // => [["a", "b"], [1, 2], [true, false]]
 */
export function unzip(arr is array) returns array {
  return zip(arr);
}

/**
 * `unzip`, passing each ungrouped column through `iteratee` before collecting it.
 * @example
 *   unzipWith([[1, 10], [2, 20]], (col) => sum(col)); // => [3, 30]
 */
export function unzipWith(arr is array, iteratee is function) returns array {
  return mapValues(unzip(arr), iteratee);
}

/**
 * `arr` without any element equal to one in `excludeArr`. Lodash's `without` takes the exclusion
 * values as trailing variadic arguments; here they're a single array, which makes this identical
 * to @see `difference` — kept under its own name to match lodash's vocabulary.
 * @example
 *   without([2, 1, 2, 3], [1, 2]); // => [3]
 */
export function without(arr is array, excludeArr is array) returns array {
  return difference(arr, excludeArr);
}

/**
 * Symmetric difference: values that appear in exactly one array of `arrList`, deduplicated, in
 * first-occurrence order. Lodash's `xor` takes the arrays as trailing variadic arguments; here
 * they're a single array of arrays.
 * @example
 *   xor([[2, 1], [2, 3]]); // => [1, 3]
 */
export function xor(arrList is array) returns array {
  const allVals = deduplicate(concatenateArrays(arrList));
  return filter(allVals, (val) returns boolean => {
    var count = 0;
    for (var other in arrList) {
      if (arrayIncludes(other, val)) { count += 1; }
    }
    return count == 1;
  });
}

/**
 * `xor`, comparing by `iterateeSpec(val, seq)` instead of `val` itself — the same two-argument
 * shape every iterator in this file uses; `filter`/`any` *(std)* only ever hand a callback one
 * argument, so this walks `allVals` and each candidate `other` array by index instead, via
 * `findIndex`. `iterateeSpec` is coerced through `iteratee`, so a property-path string,
 * `[path, srcValue]` array, or partial-match map works in place of a literal function.
 * @example
 *   xorBy([[2.1, 1.2], [2.3, 3.4]], (val, _seq) => floor(val)); // => [1.2, 3.4]
 */
export function xorBy(arrList is array, iterateeSpec) returns array {
  const fn = iteratee(iterateeSpec);
  const allVals = uniqBy(concatenateArrays(arrList), fn);
  var result = [];
  for (var seq = 0; seq < size(allVals); seq += 1) {
    const val = allVals[seq];
    const key = fn(val, seq);
    var count = 0;
    for (var other in arrList) {
      if (findIndex(other, (otherVal, otherSeq is number) => (fn(otherVal, otherSeq) == key)) != -1) { count += 1; }
    }
    if (count == 1) { result = append(result, val); }
  }
  return result;
}
/**
 * `xor`, comparing with `comparator(val, otherVal)` instead of `==`.
 * @example
 *   xorWith([[{ "x": 1 }, { "x": 2 }], [{ "x": 2 }]], (aa, bb) => aa.x == bb.x);
 *   // => [{ "x": 1 }]
 */
export function xorWith(arrList is array, comparator is function) returns array {
  const allVals = uniqWith(concatenateArrays(arrList), comparator);
  return filter(allVals, (val) returns boolean => {
    var count = 0;
    for (var other in arrList) {
      if (any(other, (otherVal) => comparator(val, otherVal))) { count += 1; }
    }
    return count == 1;
  });
}

/**
 * Map pairing up `keylist` and `valuelist` by position: `zipObject(["a","b"], [1,2])` is
 * `{"a": 1, "b": 2}`. A `keylist` entry past the end of `valuelist` is simply absent from the
 * result — FeatureScript maps drop a key written to `undefined`; a `valuelist` entry past the end
 * of `keylist` is dropped.
 * @example
 *   zipObject(["a", "b"], [1, 2]); // => { "a": 1, "b": 2 }
 *   zipObject(["a", "b"], [1]);    // => { "a": 1 }
 */
export function zipObject(keylist is array, valuelist is array) returns map {
  var result = {};
  for (var seq = 0; seq < size(keylist); seq += 1) {
    result[keylist[seq]] = (seq < size(valuelist)) ? valuelist[seq] : undefined;
  }
  return result;
}

/**
 * `zip` *(std)* on `arrList`, passing each grouped row through `iteratee` before collecting it —
 * the same shape as `unzipWith`, under lodash's name for the zipping direction.
 * @example
 *   zipWith([[1, 2], [10, 20]], (row) => sum(row)); // => [11, 22]
 */
export function zipWith(arrList is array, iteratee is function) returns array {
  return mapValues(zip(arrList), iteratee);
}

// --

// == [Object Utils] ==

/**
 * First key of `bag` whose value satisfies `rule(val, key)`, or `undefined` if none does.
 * `findLastKey` scans in the reverse of `keys(bag)` order. `rule` is coerced through `iteratee`,
 * so a property-path string, `[path, srcValue]` array, or partial-match map works in place of a
 * literal function.
 * @example
 *   findKey({ "a": 1, "b": 2, "c": 3 }, function(val, key) { return val > 1; }); // => "b"
 */
export function findKey(bag is map, rule) {
  const fn = iteratee(rule);
  for (var key in keys(bag)) {
    if (fn(bag[key], key)) { return key; }
  }
  return undefined;
}

/**
 * Last key of `bag` whose value satisfies `rule(val, key)`, or `undefined` if none does.
 * `findKey` scans in the reverse of `keys(bag)` order. `rule` is coerced through `iteratee`, so a
 * property-path string, `[path, srcValue]` array, or partial-match map works in place of a
 * literal function.
 * @example
 *   findLastKey({ "a": 1, "b": 2, "c": 3 }, function(val, key) { return val > 1; }); // => "b"
 */
export function findLastKey(bag is map, rule) {
  const fn = iteratee(rule);
  const keylist = keys(bag);
  for (var seq = size(keylist) - 1; seq >= 0; seq -= 1) {
    const key = keylist[seq];
    if (fn(bag[key], key)) { return key; }
  }
  return undefined;
}

/**
 * `bag` with its keys and values swapped: `{"a": "x", "b": "x"}` → `{"x": "b"}` — a value that
 * occurs more than once keeps only its last key, same as lodash. A non-string value is
 * stringified into its new key, matching how lodash's own object keys coerce. `invertBy` collects
 * every key instead of just the last one, grouped under `iterateeSpec(val, key)` rather than
 * `val` itself — the same two-argument shape every iterator in this file uses. `iterateeSpec` is
 * coerced through `iteratee`, so a property-path string, `[path, srcValue]` array, or
 * partial-match map works in place of a literal function.
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
export function invertBy(bag is map, iterateeSpec) returns map {
  const fn = iteratee(iterateeSpec);
  var result = {};
  for (var key in keys(bag)) {
    result = insertIntoMapOfArrays(result, fn(bag[key], key), key);
  }
  return result;
}

/**
 * `bag`'s values, replacing each key with `iterateeSpec(val, key)` — `mapValues`' sibling for
 * keys instead of values. A collision on the computed key keeps the last entry that produced it.
 * `iterateeSpec` is coerced through `iteratee`, so a property-path string, `[path, srcValue]`
 * array, or partial-match map works in place of a literal function.
 * @example
 *   mapKeys({ "a": 1, "b": 2 }, function(val, key) { return key ~ val; }); // => { "a1": 1, "b2": 2 }
 */
export function mapKeys(bag is map, iterateeSpec) returns map {
  const fn = iteratee(iterateeSpec);
  var result = {};
  for (var key in keys(bag)) {
    result[fn(bag[key], key)] = bag[key];
  }
  return result;
}

/**
 * `bag` without the entries at `keylist` — the inverse of `pick`. Each entry of `keylist` is
 * resolved via `getAt`/`setAt`, same as `pick`, so a dotted string or key-path array deletes a
 * nested leaf without disturbing its siblings; a path with nothing currently at it is skipped
 * rather than autovivifying empty maps along the way. `omitBy` instead drops any entry for which
 * `rule(val, key)` holds, the inverse of `pickDefined`'s spirit but with a caller-supplied rule
 * rather than a fixed "is defined" check. `rule` is coerced through `iteratee`, so a
 * property-path string, `[path, srcValue]` array, or partial-match map works in place of a
 * literal function.
 * @example
 *   omit({ "a": 1, "b": 2, "c": 3 }, ["b"]); // => { "a": 1, "c": 3 }
 *   omit({ "a": { "b": 1, "c": 2 } }, ["a.b"]); // => { "a": { "c": 2 } }
 */
export function omit(bag is map, keylist is array) returns map {
  var result = bag;
  for (var pathSpec in keylist) {
    if (getAt(result, pathSpec, Sentinel.ABSENT) != Sentinel.ABSENT) {
      result = setAt(result, pathSpec, undefined);
    }
  }
  return result;
}
export function omitBy(bag is map, rule) returns map {
  const fn = iteratee(rule);
  var result = {};
  for (var key in keys(bag)) {
    if (! fn(bag[key], key)) { result[key] = bag[key]; }
  }
  return result;
}

/**
 * `bag`'s entries for which `rule(val, key)` holds — the inverse of `omitBy`, and the
 * generic-rule sibling of `pickDefined`'s fixed "is defined" check, matching lodash's
 * `pickBy(object, [predicate=_.identity])`. `rule` is coerced through `iteratee`, so a
 * property-path string, `[path, srcValue]` array, or partial-match map works in place of a
 * literal function.
 * @example
 *   pickBy({ "a": 1, "b": 2, "c": 3 }, (val, _key) => val > 1); // => { "b": 2, "c": 3 }
 */
export function pickBy(bag is map, rule) returns map {
  const fn = iteratee(rule);
  var result = {};
  for (var key in keys(bag)) {
    if (fn(bag[key], key)) { result[key] = bag[key]; }
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
  return mapValues(keys(bag), (key is string, _seq is number) => [key, bag[key]]);
}

// --

// == [Function values] --

export const ClxnUtilsFuncs = {
  "hasKey":            (obj, key)                      => hasKey(obj, key),
  "hasPresentKey":     (obj, key)                      => hasPresentKey(obj, key),
  "pick":              (bag, keylist)                  => pick(bag, keylist),
  "pickDefined":       (bag, keylist)                  => pickDefined(bag, keylist),
  "arrLast":           (arr)                           => arrLast(arr),
  "arrFirst":          (arr)                           => arrFirst(arr),
  "valuesAt":          (bagOrArr, keylist)             => valuesAt(bagOrArr, keylist),
  "forEach":           (bagOrArr, func)                => forEach(bagOrArr, func),
  "forEach3":          (bagOrArr, func)                => forEach3(bagOrArr, func),
  "mapValues":         (bagOrArr, func)                => mapValues(bagOrArr, func),
  "mapValues3":        (bagOrArr, func)                => mapValues3(bagOrArr, func),
  "objectify":         (arr, func)                     => objectify(arr, func),
  "rebag":             (arrOrBag, func)                => rebag(arrOrBag, func),
  "boxarrPush":        (arrRef, val)                   => boxarrPush(arrRef, val),
  "boxarrUnshift":     (arrRef, val)                   => boxarrUnshift(arrRef, val),
  "countBy":           (arrOrBag, iteratee)            => countBy(arrOrBag, iteratee),
  "find":              (arrOrBag, rule)                => find(arrOrBag, rule),
  "findLast":          (arrOrBag, rule)                => findLast(arrOrBag, rule),
  "flatMap":           (arrOrBag, iteratee)            => flatMap(arrOrBag, iteratee),
  "flatMapDeep":       (arrOrBag, iteratee)            => flatMapDeep(arrOrBag, iteratee),
  "flatMapDepth":      (arrOrBag, iteratee, depth)     => flatMapDepth(arrOrBag, iteratee, depth),
  "forEachRight":      (arrOrBag, func)                => forEachRight(arrOrBag, func),
  "groupBy":           (arrOrBag, iteratee)            => groupBy(arrOrBag, iteratee),
  "partition":         (arrOrBag, rule)                => partition(arrOrBag, rule),
  "reduceRight":       (arrOrBag, seed, foldFunction)  => reduceRight(arrOrBag, seed, foldFunction),
  "reject":            (arrOrBag, rule)                => reject(arrOrBag, rule),
  "arrayIncludes":     (arrOrBag, target)              => arrayIncludes(arrOrBag, target),
  "chunk":             (arr, chunkSize)                => chunk(arr, chunkSize),
  "compact":           (arr)                           => compact(arr),
  "difference":        (arr, excludeArr)               => difference(arr, excludeArr),
  "differenceBy":      (arr, excludeArr, iteratee)     => differenceBy(arr, excludeArr, iteratee),
  "differenceWith":    (arr, excludeArr, comparator)   => differenceWith(arr, excludeArr, comparator),
  "drop":              (arr, dropCount)                => drop(arr, dropCount),
  "dropRight":         (arr, dropCount)                => dropRight(arr, dropCount),
  "dropRightWhile":    (arr, rule)                     => dropRightWhile(arr, rule),
  "dropWhile":         (arr, rule)                     => dropWhile(arr, rule),
  "findIndex":         (arr, rule)                     => findIndex(arr, rule),
  "findLastIndex":     (arr, rule)                     => findLastIndex(arr, rule),
  "flatten":           (arr)                           => flatten(arr),
  "flattenDeep":       (arr)                           => flattenDeep(arr),
  "flattenDepth":      (arr, depth)                    => flattenDepth(arr, depth),
  "fromPairs":         (pairs)                         => fromPairs(pairs),
  "initial":           (arr)                           => initial(arr),
  "intersection":      (arrList)                       => intersection(arrList),
  "intersectionBy":    (arrList, iteratee)             => intersectionBy(arrList, iteratee),
  "intersectionWith":  (arrList, comparator)           => intersectionWith(arrList, comparator),
  "lastIndexOf":       (arr, val)                      => lastIndexOf(arr, val),
  "maxBy":             (arr, iteratee)                 => maxBy(arr, iteratee),
  "meanBy":            (arr, iteratee)                 => meanBy(arr, iteratee),
  "minBy":             (arr, iteratee)                 => minBy(arr, iteratee),
  "nth":               (arr, seq)                      => nth(arr, seq),
  "sumBy":             (arr, iteratee)                 => sumBy(arr, iteratee),
  "tail":              (arr)                           => tail(arr),
  "take":              (arr, takeCount)                => take(arr, takeCount),
  "takeRight":         (arr, takeCount)                => takeRight(arr, takeCount),
  "takeRightWhile":    (arr, rule)                     => takeRightWhile(arr, rule),
  "takeWhile":         (arr, rule)                     => takeWhile(arr, rule),
  "union":             (arrList)                       => union(arrList),
  "unionBy":           (arrList, iteratee)             => unionBy(arrList, iteratee),
  "unionWith":         (arrList, comparator)           => unionWith(arrList, comparator),
  "uniqBy":            (arr, iteratee)                 => uniqBy(arr, iteratee),
  "uniqWith":          (arr, comparator)               => uniqWith(arr, comparator),
  "unzip":             (arr)                           => unzip(arr),
  "unzipWith":         (arr, iteratee)                 => unzipWith(arr, iteratee),
  "without":           (arr, excludeArr)               => without(arr, excludeArr),
  "xor":               (arrList)                       => xor(arrList),
  "xorBy":             (arrList, iteratee)             => xorBy(arrList, iteratee),
  "xorWith":           (arrList, comparator)           => xorWith(arrList, comparator),
  "zipObject":         (keylist, valuelist)            => zipObject(keylist, valuelist),
  "zipWith":           (arrList, iteratee)             => zipWith(arrList, iteratee),
  "findKey":           (bag, rule)                     => findKey(bag, rule),
  "findLastKey":       (bag, rule)                     => findLastKey(bag, rule),
  "invert":            (bag)                           => invert(bag),
  "invertBy":          (bag, iteratee)                 => invertBy(bag, iteratee),
  "mapKeys":           (bag, iteratee)                 => mapKeys(bag, iteratee),
  "omit":              (bag, keylist)                  => omit(bag, keylist),
  "omitBy":            (bag, rule)                     => omitBy(bag, rule),
  "pickBy":            (bag, rule)                     => pickBy(bag, rule),
  "toPairs":           (bag)                           => toPairs(bag),
};
