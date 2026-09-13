FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
import(path : "66e287bede293cb227dfb89c", version : "25bf5ea59817ea0aa1737abd"); // typeUtils

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
 * `difference`, comparing `arr` and `excludeArr` by `iteratee(val)` instead of `val` itself.
 * @example
 *   differenceBy([2.1, 1.2], [2.3, 3.4], (val) => floor(val)); // => [1.2]
 */
export function differenceBy(arr is array, excludeArr is array, iteratee is function) returns array {
  const excludeKeys = mapArray(excludeArr, iteratee);
  return filter(arr, (val) => (! arrayIncludes(excludeKeys, iteratee(val))));
}

/**
 * `difference`, comparing `arr` and `excludeArr` with `comparator(val, other)` instead of `==`.
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

/**
 * `arr` with the last `dropCount` elements removed; `dropCount <= 0` returns `arr` unchanged.
 * @example
 *   dropRight([1, 2, 3], 2); // => [1]
 *   dropRight([1, 2, 3], 5); // => []
 */
export function dropRight(arr is array, dropCount is number) returns array {
  return subArray(arr, 0, size(arr) - clamp(dropCount, 0, size(arr)));
}

/**
 * `arr` with elements dropped from the end for as long as `rule` holds; the first
 * (rightmost-scanned) element `rule` rejects, and everything before it, is kept.
 * @example
 *   dropRightWhile([1, 2, 3, 4], (val) => val > 2); // => [1, 2]
 */
export function dropRightWhile(arr is array, rule is function) returns array {
  var end = size(arr);
  for (; end > 0; end -= 1) {
    if (! rule(arr[end - 1])) { break; }
  }
  return subArray(arr, 0, end);
}

/**
 * `arr` with elements dropped from the beginning for as long as `rule` holds.
 * @example
 *   dropWhile([1, 2, 3, 4], (val) => val < 3); // => [3, 4]
 */
export function dropWhile(arr is array, rule is function) returns array {
  var beg = 0;
  for (; beg < size(arr); beg += 1) {
    if (! rule(arr[beg], _seq)) { break; }
  }
  return subArray(arr, beg);
}

/**
 * Index of the first element of `arr` for which `rule` holds, or `-1` if none does.
 * @example
 *   findIndex([1, 2, 3], (val) => val > 1); // => 1
 *   findIndex([1, 2, 3], (val) => val > 9); // => -1
 */
export function findIndex(arr is array, rule is function) returns number {
  for (var seq = 0; seq < size(arr); seq += 1) {
    if (rule(arr[seq], _seq)) { return seq; }
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
    if (rule(arr[seq])) { return seq; }
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
  const kept = filter(first, (val) => all(rest, (other) => arrayIncludes(other, val)));
  return deduplicate(kept);
}

/**
 * `intersection`, comparing elements by `iteratee(val)` instead of `val` itself.
 * @example
 *   intersectionBy([[2.1, 1.2], [2.3, 3.4]], (val) => floor(val)); // => [2.1]
 */
export function intersectionBy(arrList is array, iteratee is function) returns array {
  if (size(arrList) == 0) { return []; }
  const first = arrList[0];
  const restKeys = mapArray(subArray(arrList, 1), (other) => mapArray(other, iteratee));
  const kept = filter(first, (val) => all(restKeys, (otherKeys) => arrayIncludes(otherKeys, iteratee(val))));
  return uniqBy(kept, iteratee);
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
  const kept = filter(first, (val) => all(rest, (other) => any(other, (otherVal) => comparator(val, otherVal))));
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
 * `arr` without its first element; `[]` for an empty or single-element `arr`.
 * @example
 *   tail([1, 2, 3]); // => [2, 3]
 */
export function tail(arr is array) returns array {
  return subArray(arr, min(1, size(arr)));
}

/**
 * First `takeCount` elements of `arr`; `takeCount <= 0` returns `[]`.
 * @example
 *   take([1, 2, 3], 2); // => [1, 2]
 */
export function take(arr is array, takeCount is number) returns array {
  return subArray(arr, 0, clamp(takeCount, 0, size(arr)));
}

/**
 * Last `takeCount` elements of `arr`; `takeCount <= 0` returns `[]`.
 * @example
 *   takeRight([1, 2, 3], 2); // => [2, 3]
 */
export function takeRight(arr is array, takeCount is number) returns array {
  return subArray(arr, size(arr) - clamp(takeCount, 0, size(arr)));
}

/**
 * Elements taken from the end of `arr` for as long as `rule` holds.
 * @example
 *   takeRightWhile([1, 2, 3, 4], (val) => val > 2); // => [3, 4]
 */
export function takeRightWhile(arr is array, rule is function) returns array {
  var beg = size(arr);
  for (; beg > 0; beg -= 1) {
    if (! rule(arr[beg - 1])) { break; }
  }
  return subArray(arr, beg);
}

/**
 * Elements taken from the beginning of `arr` for as long as `rule` holds.
 * @example
 *   takeWhile([1, 2, 3, 4], (val) => val < 3); // => [1, 2]
 */
export function takeWhile(arr is array, rule is function) returns array {
  var end = 0;
  for (; end < size(arr); end += 1) {
    if (! rule(arr[end])) { break; }
  }
  return subArray(arr, 0, end);
}

/**
 * Deduplicated concatenation of every array in `arrList`, ordered by first occurrence.
 * @example
 *   union([[2], [1, 2], [2, 3]]); // => [2, 1, 3]
 */
export function union(arrList is array) returns array {
  return deduplicate(concatenateArrays(arrList));
}

/**
 * `union`, deduplicating by `iteratee(val)` instead of `val` itself.
 * @example
 *   unionBy([[2.1], [1.2, 2.3]], (val) => floor(val)); // => [2.1, 1.2]
 */
export function unionBy(arrList is array, iteratee is function) returns array {
  return uniqBy(concatenateArrays(arrList), iteratee);
}

/**
 * `union`, deduplicating with `comparator(kept, val)` instead of `==`.
 * @example
 *   unionWith([[{ "x": 1 }], [{ "x": 1 }, { "x": 2 }]], (aa, bb) => aa.x == bb.x);
 *   // => [{ "x": 1 }, { "x": 2 }]
 */
export function unionWith(arrList is array, comparator is function) returns array {
  return uniqWith(concatenateArrays(arrList), comparator);
}

/**
 * `arr` with duplicate elements removed, keeping the first occurrence — like std's
 * `deduplicate`, but comparing `iteratee(val)` instead of `val` itself.
 * @example
 *   uniqBy([2.1, 1.2, 2.3], (val) => floor(val)); // => [2.1, 1.2]
 */
export function uniqBy(arr is array, iteratee is function) returns array {
  var seenKeys = [];
  var result = [];
  for (var val in arr) {
    const key = iteratee(val);
    if (! arrayIncludes(seenKeys, key)) {
      seenKeys = append(seenKeys, key);
      result = append(result, val);
    }
  }
  return result;
}

/**
 * `arr` with duplicate elements removed, keeping the first occurrence, where two elements count
 * as duplicates when `comparator(kept, val)` is `true`.
 * @example
 *   uniqWith([{ "x": 1 }, { "x": 1 }, { "x": 2 }], (aa, bb) => aa.x == bb.x);
 *   // => [{ "x": 1 }, { "x": 2 }]
 */
export function uniqWith(arr is array, comparator is function) returns array {
  var result = [];
  for (var val in arr) {
    if (! any(result, (kept) => comparator(kept, val))) {
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
  return mapArray(unzip(arr), iteratee);
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
 * `xor`, comparing by `iteratee(val)` instead of `val` itself.
 * @example
 *   xorBy([[2.1, 1.2], [2.3, 3.4]], (val) => floor(val)); // => [1.2, 3.4]
 */
export function xorBy(arrList is array, iteratee is function) returns array {
  const allVals = uniqBy(concatenateArrays(arrList), iteratee);
  return filter(allVals, (val) returns boolean => {
    const key = iteratee(val);
    var count = 0;
    for (var other in arrList) {
      if (any(other, (otherVal) => (iteratee(otherVal) == key))) { count += 1; }
    }
    return count == 1;
  });
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
  return mapArray(zip(arrList), iteratee);
}
