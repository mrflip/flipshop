FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");

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

// == [Collection Inspection] -- sizeof, hasKey

/**
 * Size of `val`: length for a string, element count for an array, key count for a map, `0` for
 * `undefined`.
 * @example
 *   sizeof([0, 1, 2]);  // => 3
 *   sizeof({ "a": 1 }); // => 1
 *   sizeof("12345");    // => 5
 *   sizeof(undefined);  // => 0
 */
export function sizeof(val is map) returns number {
    return size(keys(val));
}
export function sizeof(val is string) returns number {
    return length(val);
}
export function sizeof(val is array) returns number {
    return size(val);
}
export function sizeof(val is undefined) returns number {
    return 0;
}

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

/**
 * Whether `target` appears anywhere in `arr`. Unlike lodash's `includes`, this is array-only —
 * no substring search on a string, no value search over a map — and comparison is plain `==`,
 * which is deep structural equality on maps and arrays.
 * @example
 *   arrayIncludes([1, 2, 3], 2);              // => true
 *   arrayIncludes([{ "a": 1 }], { "a": 1 });  // => true
 */
export function arrayIncludes(arr is array, target) returns boolean {
  for (var item in arr) {
    if (item == target) { return true; }
  }
  return false;
}
// --

// == [Collection retrieve many] -- pick, pickDefined, arrLast

/**
 * A map with just `bag`'s entries at `keylist` — like lodash's `pick`, an absent key is simply
 * missing from the result rather than present with an `undefined` value.
 *
 * `pickDefined` additionally drops a key whose value is `undefined` — for a map this is the same
 * result as `pick`, since a map can never hold an `undefined` value to differ over. Unlike
 * lodash's `pickBy`, the predicate isn't customizable and the keys considered are exactly
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
