FeatureScript 3029;
import(path : "onshape/std/common.fs", version : "3029.0");

export function ifNil(val, fallback) { if (val == undefined) { return fallback; } return val; }

export function arrayIncludes(arr is array, target) returns boolean {
  for (var item in arr) {
    if (item == target) { return true; }
  }
  return false;
}

export function pick(bag is map, keys is array) returns map {
    var result = {};
    for (var k in keys) {
      result[k] = bag[k];
    }
    return result;
}

export function pickDefined(bag is map, keys is array) returns map {
    var result = {};
    for (var k in keys) {
      if (bag[k] != undefined) { result[k] = bag[k]; }
    }
    return result;
}

export function isPresent(val) { return val != undefined; }

export function truthy(val) { return (val != undefined) && (val != false); }
