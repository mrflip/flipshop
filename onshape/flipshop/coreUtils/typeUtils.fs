FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
import(path : "54590bc1c9cee0141b968fbb", version : "a8d11848557d891d016ec291");

export const mm  = millimeter;
export const zero = 0 * mm;

export function vector2(vec is Vector) {
  return vector(vec[0], vec[1]);
}

export function ifNil(val, fallback) {
  if (val == undefined) { return fallback; }
  return val;
}

export function ifBlank(val, fallback) {
  if (strBlank(val)) { return fallback; }
  return val;
}

export function truthy(val)    { return (val != undefined) && (val != false); }

export function isPresent(val) { return (val != undefined); }
export function isNil(val)     { return (val == undefined); }

export function strBlank(val)  { return isUndefinedOrEmptyString(val); }

export function ifZero(val is number,         fallback) { if ((val == undefined) || tolerantEquals(val, 0))              { return fallback; } return val; }
export function ifZero(val is ValueWithUnits, fallback) { if ((val == undefined) || tolerantEquals(val, 0 * millimeter)) { return fallback; } return val; }

export function isEmpty(val) returns boolean {
  if (val is undefined) { return true; }
  if (val is map || val is string || val is array) { return sizeof(val) <= 0; }
  return false;
}

function ifZero(val, fallback) { if ((val == undefined) || tolerantEquals(val, 0)) { return fallback; } return val; }
function ifZero(val is ValueWithUnits, fallback) { return ifZero(val / millimeter, fallback); }