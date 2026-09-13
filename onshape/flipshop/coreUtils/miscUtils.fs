FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
import(path : "4ebdc64943b566160ea5cc28", version : "3684960ff67d0db556614489");

/**
 * Map of `tags` to a same-named child of `id`: `idsFor(id, ["a", "b"])` is
 * `{ "a": id + "a", "b": id + "b" }` — the `ids` map every multi-sketch/multi-op feature
 * declares up front, built in one call instead of one line per key.
 * @param id {Id}: Base id.
 * @param tags {array}: Id-suffix strings, one per key.
 */
export function idsFor(id is Id, tags is array) returns map {
    return objectify(tags, (tag, _) => id + tag);
}

/** Returns `undefined`, regardless of arguments received. */
export const noop = function() {};

/**
 * `curryNtoM` wraps `func` to accept `N` arguments but call `func` with only the first `M` of
 * them — dropping trailing arguments so a fixed-arity callback (`func()`, `func(val)`, …) can sit
 * in a slot that always calls with `N` arguments, like a `forEach`/`mapValues` iteratee.
 * @example
 *   curry2to0(function() { return "called"; })("ignored1", "ignored2"); // => "called"
 *   curry3to1(function(val) { return val; })(1, 2, 3);                  // => 1
 */
export function curry3to0(func is function) { return (_arg1, _arg2, _arg3) => func();               }
export function curry3to1(func is function) { return (arg1,  _arg2, _arg3) => func(arg1);           }
export function curry3to2(func is function) { return (arg1,  arg2,  _arg3) => func(arg1, arg2);     }

export function curry2to0(func is function) { return (_arg1, _arg2) => func();           }
export function curry2to1(func is function) { return (arg1,  _arg2) => func(arg1);       }
export function curry2to2(func is function) { return (arg1,  arg2) => func(arg1, arg2);  }

/**
 * Parses `rawjson` into a map/array, wrapping a parse failure in a `regenError` (naming `story`,
 * a caller-supplied label) instead of surfacing `parseJson`'s own opaque throw.
 * @param opts {map}: keyword options
 *   - @field [detectUnits=false] {boolean}: Parse unit-bearing strings (e.g. `"3 inch"`) into a `ValueWithUnits`, via `parseJsonWithUnits`.
 *   - @field [story="because"] {string}: Label for the error message, so a caller can say what it was trying to do.
 * @example
 *   parseJsonSafely('{"a": 1}'); // => { "a": 1 }
 */
export function parseJsonSafely(rawjson is string, opts is map) {
  const mopts = mergeMaps({ "detectUnits": false, "story": "because" }, ifNil(opts, {}));
  var jsondata = undefined;
  try {
    jsondata = mopts.detectUnits ? parseJsonWithUnits(rawjson) : parseJson(rawjson);
    return jsondata;
  } catch (error) {
    println(["Error parsing JSON", mopts.story, error]);
    println(rawjson);
    throw regenError("Could not parse JSON " ~ mopts.story ~ ": " ~ error ~ " -- " ~ strTake(rawjson, 10000));
  }
}
export function parseJsonSafely(rawjson is string) { return parseJsonSafely(rawjson, { "detectUnits": false }); }

/**
 * `range` *(std)*, descending, via a plain `reverse` — inherits std `range`'s own inclusive-of-
 * `to` convention rather than lodash's exclusive one, matching how `range` itself is already
 * mapped in this project.
 * @example
 *   rangeRight(0, 3); // => [3, 2, 1, 0]
 */
export function rangeRight(from is number, to is number) returns array {
  return reverse(range(from, to));
}
