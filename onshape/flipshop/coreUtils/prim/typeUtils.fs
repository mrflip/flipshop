FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");

/** Millimeter unit shorthand, and its zero. */
export const mm  = millimeter;
export const zero = 0 * mm;

/**
 * Sentinel for "nothing at that step", so a fallback that is itself a map or an array can
 * never be mistaken for something found in the bag and descended into.
 */
export enum Sentinel { ABSENT }

/**
 * Drops `vec`'s z component.
 * @example
 *   vector2(vector(1, 2, 3)); // => vector(1, 2)
 */
export const vector2 = (function(vec is Vector) returns Vector {
  return vector(vec[0], vec[1]);
});

/**
 * `val`, or `fallback` if `val` is `undefined` — lodash's `defaultTo`, narrowed to the one
 * nullish value FeatureScript actually has.
 * @example
 *   ifNil(undefined, 10); // => 10
 *   ifNil(1, 10);         // => 1
 */
export const ifNil = (function(val, fallback) {
  if (val == undefined) { return fallback; }
  return val;
});

/**
 * `val`, or `fallback` if `val` is `undefined` or an empty string.
 * @example
 *   ifBlank("", "default");   // => "default"
 *   ifBlank("hi", "default"); // => "hi"
 */
export const ifBlank = (function(val, fallback) {
  if (strBlank(val)) { return fallback; }
  return val;
});

/** Whether `val` is neither `undefined` nor `false` — everything else, `0` and `""` included, is truthy. */
export const truthy = (function(val) returns boolean    { return (val != undefined) && (val != false); });

/** Whether `val` is not `undefined`. */
export const isPresent = (function(val) returns boolean { return (val != undefined); });
/** Whether `val` is `undefined` — lodash's `isNil`, narrowed to the one nullish value FeatureScript actually has. */
export const isNil = (function(val) returns boolean     { return (val == undefined); });

/** Whether `val` is `undefined` or an empty string. */
export const strBlank = (function(val) returns boolean  { return isUndefinedOrEmptyString(val); });

/**
 * `val`, or `fallback` if `val` is `undefined` or (within tolerance) zero.
 * @example
 *   ifZero(0, "default");              // => "default"
 *   ifZero(5, "default");              // => 5
 *   ifZero(0 * millimeter, "default"); // => "default"
 */
export function ifZero(val is number,         fallback) { if ((val == undefined) || tolerantEquals(val, 0))              { return fallback; } return val; }
export function ifZero(val is ValueWithUnits, fallback) { if ((val == undefined) || tolerantEquals(val, 0 * millimeter)) { return fallback; } return val; }
export function ifZero(val is undefined,      fallback) { return fallback; }

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
export const isEmpty = (function(val) returns boolean {
  if (val is undefined) { return true; }
  if (val is map || val is string || val is array) { return sizeof(val) <= 0; }
  return false;
});

export const isEqual = (function(aa, bb) returns boolean {
  return aa == bb;
});

/**
 * `val` unchanged if it's already an array, otherwise `[val]` — a non-array value wrapped in a
 * single-element array.
 * @example
 *   castArray(1);      // => [1]
 *   castArray([1, 2]); // => [1, 2]
 */
export const castArray = (function(val) returns array {
  if (val is array) { return val; }
  return [val];
});

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

// == [Function values] --

export const TypeUtilsFuncs = {
  "vector2":    (vec)           => vector2(vec),
  "isEqual":    (aa, bb)        => isEqual(aa, bb),
  "sizeof":     (val)           => sizeof(val),
  "castArray":  (val)           => castArray(val),
  "ifNil":      (val, fallback) => ifNil(val, fallback),
  "ifBlank":    (val, fallback) => ifBlank(val, fallback),
  "truthy":     (val)           => truthy(val),
  "isPresent":  (val)           => isPresent(val),
  "isNil":      (val)           => isNil(val),
  "strBlank":   (val)           => strBlank(val),
  "ifZero":     (val, fallback) => ifZero(val, fallback),
  "isEmpty":    (val)           => isEmpty(val),
};