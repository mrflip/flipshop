FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
import(path : "54590bc1c9cee0141b968fbb", version : "a8d11848557d891d016ec291");

/** Millimeter unit shorthand, and its zero. */
export const mm  = millimeter;
export const zero = 0 * mm;

/**
 * Drops `vec`'s z component.
 * @example
 *   vector2(vector(1, 2, 3)); // => vector(1, 2)
 */
export function vector2(vec is Vector) {
  return vector(vec[0], vec[1]);
}

/**
 * `val`, or `fallback` if `val` is `undefined` — lodash's `defaultTo`, narrowed to the one
 * nullish value FeatureScript actually has.
 * @example
 *   ifNil(undefined, 10); // => 10
 *   ifNil(1, 10);         // => 1
 */
export function ifNil(val, fallback) {
  if (val == undefined) { return fallback; }
  return val;
}

/**
 * `val`, or `fallback` if `val` is `undefined` or an empty string.
 * @example
 *   ifBlank("", "default");   // => "default"
 *   ifBlank("hi", "default"); // => "hi"
 */
export function ifBlank(val, fallback) {
  if (strBlank(val)) { return fallback; }
  return val;
}

/** Whether `val` is neither `undefined` nor `false` — everything else, `0` and `""` included, is truthy. */
export function truthy(val)    { return (val != undefined) && (val != false); }

/** Whether `val` is not `undefined`. */
export function isPresent(val) { return (val != undefined); }
/** Whether `val` is `undefined` — lodash's `isNil`, narrowed to the one nullish value FeatureScript actually has. */
export function isNil(val)     { return (val == undefined); }

/** Whether `val` is `undefined` or an empty string. */
export function strBlank(val)  { return isUndefinedOrEmptyString(val); }

/**
 * `val`, or `fallback` if `val` is `undefined` or (within tolerance) zero.
 * @example
 *   ifZero(0, "default");              // => "default"
 *   ifZero(5, "default");              // => 5
 *   ifZero(0 * millimeter, "default"); // => "default"
 */
export function ifZero(val is number,         fallback) { if ((val == undefined) || tolerantEquals(val, 0))              { return fallback; } return val; }
export function ifZero(val is ValueWithUnits, fallback) { if ((val == undefined) || tolerantEquals(val, 0 * millimeter)) { return fallback; } return val; }

/**
 * Whether `val` is `undefined`, or a map/string/array with no entries. Unlike lodash's
 * `isEmpty`, a boolean or number is never empty — lodash calls every non-collection scalar empty
 * (`_.isEmpty(1)` is `true`); this returns `false` for one instead.
 * @example
 *   isEmpty(undefined); // => true
 *   isEmpty([]);        // => true
 *   isEmpty([1]);       // => false
 *   isEmpty(1);         // => false
 */
export function isEmpty(val) returns boolean {
  if (val is undefined) { return true; }
  if (val is map || val is string || val is array) { return sizeof(val) <= 0; }
  return false;
}

/**
 * `val` unchanged if it's already an array, otherwise `[val]` — a non-array value wrapped in a
 * single-element array.
 * @example
 *   castArray(1);      // => [1]
 *   castArray([1, 2]); // => [1, 2]
 */
export function castArray(val) returns array {
  if (val is array) { return val; }
  return [val];
}