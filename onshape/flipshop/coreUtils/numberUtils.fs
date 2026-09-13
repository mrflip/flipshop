FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");

/**
 * Whether `num` falls in `[start, end)` (or `[end, start)` if `end < start`) — the bounds are
 * always ordered before checking, matching lodash's own auto-swap.
 * @example
 *   inRange(3, 5);     // => true  (implicit start of 0)
 *   inRange(3, 1, 5);  // => true
 *   inRange(5, 1, 5);  // => false, upper bound is exclusive
 */
export function inRange(num is number, start is number, end is number) returns boolean {
  return (num >= min(start, end)) && (num < max(start, end));
}
export function inRange(num is number, end is number) returns boolean {
  return inRange(num, 0, end);
}

/**
 * Element of `arr` for which `iteratee(val)` is greatest, or `undefined` for an empty `arr` —
 * std's array `max` picks the greatest value itself; this picks the element behind the greatest
 * *computed* value.
 * @example
 *   maxBy([{ "n": 1 }, { "n": 3 }, { "n": 2 }], (val) => val.n); // => { "n": 3 }
 */
export function maxBy(arr is array, iteratee is function) {
  var bestVal = undefined;
  var bestKey = undefined;
  for (var val in arr) {
    const key = iteratee(val);
    if (bestKey == undefined || key > bestKey) {
      bestVal = val;
      bestKey = key;
    }
  }
  return bestVal;
}

/**
 * Average of `iteratee(val)` across `arr` — `average` *(std)*, iteratee-mapped.
 * @example
 *   meanBy([{ "n": 2 }, { "n": 4 }], (val) => val.n); // => 3
 */
export function meanBy(arr is array, iteratee is function) {
  return average(mapArray(arr, iteratee));
}

/**
 * `maxBy`'s counterpart: element of `arr` for which `iteratee(val)` is least, or `undefined` for
 * an empty `arr`.
 * @example
 *   minBy([{ "n": 1 }, { "n": 3 }, { "n": 2 }], (val) => val.n); // => { "n": 1 }
 */
export function minBy(arr is array, iteratee is function) {
  var bestVal = undefined;
  var bestKey = undefined;
  for (var val in arr) {
    const key = iteratee(val);
    if (bestKey == undefined || key < bestKey) {
      bestVal = val;
      bestKey = key;
    }
  }
  return bestVal;
}

/**
 * Sum of `iteratee(val)` across `arr` — `sum` *(std)*, iteratee-mapped.
 * @example
 *   sumBy([{ "n": 2 }, { "n": 4 }], (val) => val.n); // => 6
 */
export function sumBy(arr is array, iteratee is function) {
  return sum(mapArray(arr, iteratee));
}
