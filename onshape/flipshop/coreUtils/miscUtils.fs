FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
import(path : "4ebdc64943b566160ea5cc28", version : "3684960ff67d0db556614489");

export function idsFor(id is Id, tags is array) returns map {
    return objectify(tags, (tag, _) => id + tag);
}

export const noop = function() {};

export function curry3to0(func is function) { return (_a, _b, _c) => func();     }
export function curry3to1(func is function) { return (a,  _b, _c) => func(a);    }
export function curry3to2(func is function) { return (a,   b, _c) => func(a, b); }

export function curry2to0(func is function) { return (_a, _b) => func();     }
export function curry2to1(func is function) { return (a,  _b) => func(a);    }
export function curry2to2(func is function) { return (a,   b) => func(a, b); }

export function parseJsonSafely(rawjson is string, opts is map) {
  const mopts = mergeMaps({ "detectUnits": false, "story": "because" }, ifNil(opts, {}));
  var jsondata = undefined;
  // debug(context, [varname, rawjson, description, detectUnits, definition]);
  try {
    jsondata = mopts.detectUnits ? parseJsonWithUnits(rawjson) : parseJson(rawjson);
    return jsondata;
    // println(jsondata);
  } catch (error) {
    println(["Error parsing JSON", mopts.story, error]);
    println(rawjson);
    throw regenError("Could not parse JSON " ~ mopts.story ~ ": " ~ error ~ " -- " ~ strTake(rawjson, 10000));
  }
}
export function parseJsonSafely(rawjson is string) { return parseJsonSafely(rawjson, { "detectUnits": false }); }
