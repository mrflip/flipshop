FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
//
import(path : "3b81563d40faaff8be820296", version : "95c758559e6e54348e4472ec"); // arrayUtils, for findIndex, findLastIndex &c
import(path : "66e287bede293cb227dfb89c", version : "25bf5ea59817ea0aa1737abd"); // typeUtils, for ifNil &c

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
 * missing from the result rather than present with an `undefined` value.
 *
 * `pickDefined` additionally drops a key whose value is `undefined` — for a map this is the same
 * result as `pick`, since a map can never hold an `undefined` value to differ over. Unlike
 * lodash's `pickBy`, the rule isn't customizable and the keys considered are exactly
 * `keylist`, not every key of `bag`.
 *
 * @example
 *   pick({ "a": 1, "b": 2, "c": 3 }, ["a", "c"]); // => { "a": 1, "c": 3 }
 *   pickDefined({ "a": 1, "b": undefined, "c": 3 }, ["a", "b", "c"]); // => { "a": 1, "c": 3 }
 */
export function pick(bag is map, keylist is array) returns map {
    var result = {};
    for (var kk in keylist) {
      result[kk] = bag[kk];
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
export function arrLast(arr is array) {
    if (size(arr) <= 0) { return undefined; }
    return arr[size(arr) - 1];
}
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
 * Iterates over `bag`/`arr`, invoking `func` for each entry as `func(val, key, seq)` (map) or
 * `func(val, seq)` (array) — `seq` is always a 0-based visit count. Unlike lodash's `forEach`,
 * where an iteratee may exit early by returning `false`, iteration here stops only when `func`
 * returns `NextStepAction.BREAK`.
 *
 * `missingPolicy` (`USE_UNDEFINED`, the default, or `SKIP`) decides whether an entry whose value
 * is `undefined` gets visited at all; when given for an array, `func` also receives the index
 * twice (`func(val, seq, seq)`), matching the map callback's arity. A `keylist` overload walks
 * exactly those keys, in that order, instead of `keys(bag)`.
 *
 * @example
 *   forEach({ "a": 1, "b": 2, "c": 3 }, function(val, key, seq) {
 *     if (key == "b") { return NextStepAction.BREAK; }
 *   }); // visits "a" then "b"; "c" is never reached
 */
export function forEach(bag is map, keylist is array, missingPolicy is MissingPolicy, func is function) {
  var seq  = 0;
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

export function forEach(bag is map, missingPolicy is MissingPolicy, func is function) {
    return forEach(bag, keys(bag), MissingPolicy.USE_UNDEFINED, func);
}

export function forEach(bag is map, keylist is array, func is function) {
  var seq  = 0;
  for (var key in keylist) {
    const val = bag[key];
    const result = func(val, key, seq);
    if (result == NextStepAction.BREAK) { break; }
    seq += 1;
  }
}

export function forEach(bag is map, func is function) {
  return forEach(bag, keys(bag), func);
}

export function forEach(arr is array, missingPolicy is MissingPolicy, func is function) {
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

export function forEach(arr is array, func is function) {
  for (var seq = 0; seq < size(arr); seq += 1) {
    const result = func(arr[seq], seq);
    if (result == NextStepAction.BREAK) { break; }
  }
}

/**
 * `bag`/`arr` with every value replaced by `func`'s result. Lodash splits this into two
 * functions — `mapValues` keeps an object's keys, `map` returns a new array — unified here under
 * one dispatch, since they share every other behavior: `func` is `func(val, key)` for a map or
 * `func(val, seq)` for an array. `mapValues3` adds the trailing 0-based `seq` @see `forEach`
 * does, for either container shape. The `keylist` and `missingPolicy` overloads follow
 * `forEach`'s rules: a `keylist` walks exactly those keys, and `missingPolicy` decides whether
 * an `undefined` value is mapped (`USE_UNDEFINED`, the default) or its key dropped from the
 * result entirely (`SKIP`).
 *
 * @example
 *   mapValues({ "fred": 40, "pebbles": 1 }, function(age) { return age * 2; });
 *   // => { "fred": 80, "pebbles": 2 }
 *   mapValues([4, 8], function(n) { return n * n; }); // => [16, 64]
 */
export function mapValues(bag is map, keylist is array, missingPolicy is MissingPolicy, func is function) returns map {
  const result = new box({});
  forEach(bag, keylist, missingPolicy, (val, key, seq is number) => { result[][key] = func(val, key); });
  return result[];
}
export function mapValues3(bag is map, keylist is array, missingPolicy is MissingPolicy, func is function) returns map {
  const result = new box({});
  forEach(bag, keylist, missingPolicy, (val, key, seq is number) => { result[][key] = func(val, key, seq); });
  return result[];
}

export function mapValues(bag is map, keylist is array, func is function) returns map {
  const result = new box({});
  forEach(bag, keylist, (val, key, seq is number) => { result[][key] = func(val, key); });
  return result[];
}
export function mapValues3(bag is map, keylist is array, func is function) returns map {
  const result = new box({});
  forEach(bag, keylist, (val, key, seq is number) => { result[][key] = func(val, key, seq); });
  return result[];
}
export function mapValues(bag is map, missingPolicy is MissingPolicy, func is function) returns map {
  return mapValues(bag, keys(bag), missingPolicy, func);
}
export function mapValues3(bag is map, missingPolicy is MissingPolicy, func is function) returns map {
  return mapValues3(bag, keys(bag), missingPolicy, func);
}
export function mapValues(bag is map, func is function) returns map {
  return mapValues(bag, keys(bag), func);
}
export function mapValues3(bag is map, func is function) returns map {
  return mapValues3(bag, keys(bag), func);
}

export function mapValues(arr is array, missingPolicy is MissingPolicy, func is function) returns array {
  var result = new box(makeArray(size(arr)));
  forEach(arr, missingPolicy, (val, seq, _) => {
      result[][seq] = func(val, seq);
  });
  return result[];
}

export function mapValues3(arr is array, missingPolicy is MissingPolicy, func is function) returns array {
  var result = new box(makeArray(size(arr)));
  forEach(arr, missingPolicy, (val, seq, _) => {
      result[][seq] = func(val, seq, seq);
  });
  return result[];
}

export function mapValues(arr is array, func is function) returns array {
  var result = makeArray(size(arr));
  for (var seq = 0; seq < size(arr); seq += 1) {
    result[seq] = func(arr[seq], seq);
  }
  return result;
}
export function mapValues3(arr is array, func is function) returns array {
  var result = makeArray(size(arr));
  for (var seq = 0; seq < size(arr); seq += 1) {
    result[seq] = func(arr[seq], seq, seq);
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
 * Rebuilds a map (from a map or an array) by asking `func(val, key?, seq)` for the
 * `[newKey, newVal]` pair each entry becomes; an entry where `func` returns `undefined` (rather
 * than a pair) is dropped rather than written under an `undefined` key. Where two entries land
 * on the same `newKey`, the later one wins — `keys(bag)` order for a map, index order for an
 * array.
 */
export function rebag(arr is array, func is function) returns map {
    var result = new box({});
    forEach(arr, (val, seq is number)                => { const kv = func(val, seq);      if (kv != undefined) { result[][kv[0]] = kv[1]; } });
    return result[];
}
export function rebag(bag is map, func is function) returns map {
    var result = new box({});
    forEach(bag, (val, key is string, seq is number) => { const kv = func(val, key, seq); if (kv != undefined) { result[][kv[0]] = kv[1]; } });
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
 * Map of `iteratee(val, key?)` results to how many elements of `bag`/`arr` produced that
 * result. `key` is only offered to the map form — the array form has no analogue, matching
 * `forEach`'s split between key and index.
 * @example
 *   countBy([1, 2, 3, 4], (val) => (val % 2 == 0) ? "even" : "odd"); // => { "odd": 2, "even": 2 }
 *   countBy({ a: 1, b: 2 }, (val) => (val % 2 == 0) ? "even" : "odd"); // => { "odd": 1, "even": 1 }
 */
export function countBy(arr is array, iteratee is function) returns map {
  var result = {};
  for (var val in arr) {
    const key = iteratee(val);
    result[key] = ifNil(result[key], 0) + 1;
  }
  return result;
}
export function countBy(bag is map, iteratee is function) returns map {
  var result = {};
  for (var key in keys(bag)) {
    const groupKey = iteratee(bag[key], key);
    result[groupKey] = ifNil(result[groupKey], 0) + 1;
  }
  return result;
}

/**
 * First element of `bag`/`arr` for which `rule` holds, or `undefined` if none does — the array
 * form dereferences `arrayUtils`' `findIndex`; the map form walks `keys(bag)` and hands `rule`
 * the key as a second argument. `findLast` scans from the end instead.
 * @example
 *   find([1, 2, 3], (val) => val > 1); // => 2
 *   find({ a: 1, b: 2 }, (val, key) => key == "b"); // => 2
 */
export function find(arr is array, rule is function) {
  const seq = findIndex(arr, rule);
  return (seq == -1) ? undefined : arr[seq];
}
export function find(bag is map, rule is function) {
  for (var key in keys(bag)) {
    if (rule(bag[key], key)) { return bag[key]; }
  }
  return undefined;
}
/** `find`, scanning from the end — `findLastIndex`, dereferenced, or `keys(bag)` walked in reverse. */
export function findLast(arr is array, rule is function) {
  const seq = findLastIndex(arr, rule);
  return (seq == -1) ? undefined : arr[seq];
}
export function findLast(bag is map, rule is function) {
  const keylist = keys(bag);
  for (var seq = size(keylist) - 1; seq >= 0; seq -= 1) {
    const key = keylist[seq];
    if (rule(bag[key], key)) { return bag[key]; }
  }
  return undefined;
}

/**
 * `bag`/`arr` mapped through `iteratee`, then flattened one level — @see `arrayUtils`' `flatten`.
 * The map form walks `keys(bag)`, hands `iteratee` the key as a second argument, and always
 * returns an array, same as lodash's collection form. `flatMapDeep`/`flatMapDepth` flatten fully
 * / to `depth` levels instead.
 * @example
 *   flatMap([1, 2], (val) => [val, val]); // => [1, 1, 2, 2]
 *   flatMap({ a: 1, b: 2 }, (val) => [val, val]); // => [1, 1, 2, 2]
 */
export function flatMap(arr is array, iteratee is function) returns array {
  return flatten(mapValues(arr, iteratee));
}
export function flatMap(bag is map, iteratee is function) returns array {
  return flatten(mapValues(keys(bag), (key, _seq) => iteratee(bag[key], key)));
}
export function flatMapDeep(arr is array, iteratee is function) returns array {
  return flattenDeep(mapValues(arr, iteratee));
}
export function flatMapDeep(bag is map, iteratee is function) returns array {
  return flattenDeep(mapValues(keys(bag), (key, _seq) => iteratee(bag[key], key)));
}
export function flatMapDepth(arr is array, iteratee is function, depth is number) returns array {
  return flattenDepth(mapValues(arr, iteratee), depth);
}
export function flatMapDepth(bag is map, iteratee is function, depth is number) returns array {
  return flattenDepth(mapValues(keys(bag), (key, _seq) => iteratee(bag[key], key)), depth);
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
 * Map of `iteratee(val, key?)` results to the elements of `bag`/`arr` that produced each one,
 * via std's `insertIntoMapOfArrays`. The map form hands `iteratee` the key as a second argument.
 * @example
 *   groupBy([1, 2, 3, 4], (val) => (val % 2 == 0) ? "even" : "odd");
 *   // => { "odd": [1, 3], "even": [2, 4] }
 */
export function groupBy(arr is array, iteratee is function) returns map {
  var result = {};
  for (var val in arr) {
    result = insertIntoMapOfArrays(result, iteratee(val), val);
  }
  return result;
}
export function groupBy(bag is map, iteratee is function) returns map {
  var result = {};
  for (var key in keys(bag)) {
    result = insertIntoMapOfArrays(result, iteratee(bag[key], key), bag[key]);
  }
  return result;
}

/**
 * `[passed, failed]` — `bag`/`arr` split into the elements for which `rule` holds and the ones
 * for which it doesn't, keeping visiting order. The map form hands `rule` the key as a second
 * argument and returns values only, same as lodash's collection form.
 * @example
 *   partition([1, 2, 3, 4], (val) => val % 2 == 0); // => [[2, 4], [1, 3]]
 */
export function partition(arr is array, rule is function) returns array {
  var passed = [];
  var failed = [];
  for (var val in arr) {
    if (rule(val)) {
      passed = append(passed, val);
    } else {
      failed = append(failed, val);
    }
  }
  return [passed, failed];
}
export function partition(bag is map, rule is function) returns array {
  var passed = [];
  var failed = [];
  for (var key in keys(bag)) {
    if (rule(bag[key], key)) {
      passed = append(passed, bag[key]);
    } else {
      failed = append(failed, bag[key]);
    }
  }
  return [passed, failed];
}

/**
 * `foldArray` *(std)*, back-to-front: `bag`/`arr` reduced right-to-left through
 * `foldFunction(accumulator, val, key?)`. The map form walks `keys(bag)` in reverse and hands
 * `foldFunction` the key as a third argument.
 * @example
 *   reduceRight([1, 2, 3], "", function(acc, val) { return acc ~ val; }); // => "321"
 */
export function reduceRight(arr is array, seed, foldFunction is function) {
  return foldArray(reverse(arr), seed, foldFunction);
}
export function reduceRight(bag is map, seed, foldFunction is function) {
  var acc = seed;
  for (var key in reverse(keys(bag))) {
    acc = foldFunction(acc, bag[key], key);
  }
  return acc;
}
/** `reduceRight`, seeded from the last-visited element — @see `foldArray`'s single-seed overload. */
export function reduceRight(arr is array, foldFunction is function) {
  return foldArray(reverse(arr), foldFunction);
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
 * Elements of `bag`/`arr` for which `rule` does *not* hold — the inverse of `filter` *(std)*. The
 * map form hands `rule` the key as a second argument and returns values only.
 * @example
 *   reject([1, 2, 3, 4], (val) => val % 2 == 0); // => [1, 3]
 */
export function reject(arr is array, rule is function) returns array {
  return filter(arr, (val) => (! rule(val)));
}
export function reject(bag is map, rule is function) returns array {
  var result = [];
  for (var key in keys(bag)) {
    if (! rule(bag[key], key)) { result = append(result, bag[key]); }
  }
  return result;
}
//--
