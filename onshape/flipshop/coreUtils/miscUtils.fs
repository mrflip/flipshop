FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
//
import(path : "54590bc1c9cee0141b968fbb", version : "972034451094efe0cf2307f8"); // clxnUtiles, for objectify, pick, mapValues
import(path : "607f97fc690581579d1d4a08", version : "7bdc77843983df15c42b55ba"); // clxnGetset, for getAt
import(path : "dd812faf6ff4099cda4aa0eb", version : "a7311f6cf30fb8456ecc98c3"); // stringUtils, for strTake
import(path : "66e287bede293cb227dfb89c", version : "92efbb7ccaa5d62b7bde80f1"); // typeUtils, for ifNil &c

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


// == [Function arity currying] ==

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
// --

// == [JSON parsing] ==

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

//--

// == [Lodash Util ports] -- attempt, cond, conforms/conformsTo, constant, identity, iteratee, matches/matchesProperty, over/overEvery/overSome, property/propertyOf, times

/**
 * Calls `func` with no arguments, returning its result — or the error it throws, caught instead
 * of propagated. Lodash's `attempt` also forwards extra arguments to `func`; FeatureScript has no
 * variadic call syntax to do that with, so this only covers the zero-argument case.
 * @example
 *   attempt(function() { return 42; });    // => 42
 *   attempt(function() { throw "boom"; }); // => "boom"
 */
export function attempt(func is function) {
  try {
    return func();
  } catch (error) {
    return error;
  }
}

/**
 * Builds a function that tries `pairs` (`[predicate, handler]`) in order, calling and returning
 * the first `handler` whose `predicate` holds `val`, or `undefined` if none does.
 * @example
 *   const grade = cond([
 *     [(score) => score >= 90, constant("A")],
 *     [(score) => score >= 80, constant("B")],
 *     [constant(true),         constant("F")]
 *   ]);
 *   grade(95); // => "A"
 *   grade(70); // => "F"
 */
export function cond(pairs is array) returns function {
  return function(val) {
    for (var pair in pairs) {
      if (pair[0](val)) { return pair[1](val); }
    }
    return undefined;
  };
}

/**
 * Curried `conformsTo`: builds a predicate that checks whether a given map conforms to `source`'s
 * per-key predicates.
 * @example
 *   const isAdult = conforms({ "age": (age) => age >= 18 });
 *   isAdult({ "age": 20 }); // => true
 */
export function conforms(source is map) returns function {
  return (obj is map) => conformsTo(obj, source);
}

/**
 * Whether every predicate in `source` holds against the same-keyed value of `obj` — a key in
 * `source` but absent from `obj` reads as `undefined`, same as any other missing-key read.
 * @example
 *   conformsTo({ "a": 1, "b": 2 }, { "b": (n) => n > 1 }); // => true
 *   conformsTo({ "a": 1, "b": 2 }, { "b": (n) => n > 2 }); // => false
 */
export function conformsTo(obj is map, source is map) returns boolean {
  return all(keys(source), (key) => source[key](obj[key]));
}

/**
 * Builds a single-argument function that always returns `val`, ignoring the argument it's called
 * with — FeatureScript calls a function with exactly its declared arity, so unlike lodash's
 * `constant` this only fits a one-argument slot (e.g. `mapArray`/`filter`/a `cond` handler); lift
 * it into a `(val, seq)` slot with `curry2to1(constant(val))`.
 * @example
 *   times(3, constant(0)); // => [0, 0, 0]
 */
export function constant(val, arity is number) returns function {
  if (arity == 1) { return (_x) => val; }
  if (arity == 2) { return (_x, _y) => val; }
  if (arity == 3) { return (_x, _y, _z) => val; }
  return () => val;
}
export function constant(val) returns function {
  return (_x, _y) => val;
}

/** Returns `val` unchanged — the fallback `iteratee` reaches for when nothing more specific applies. */
export const identity  = ((val, _seq) => val);
export const identity1 = ((val) => val);
export const identity2 = identity;
export const identity3 = ((val, _x, _y) => val);

/** Returns `undefined`, regardless of arguments received. */
export const noop  = ((val, _seq)        => undefined);
export const noop0 = (()                 => undefined);
export const noop1 = ((val)              => undefined);
export const noop2 = noop;
export const noop3 = ((val1, val2, val3) => undefined);

/**
 * Coerces `spec` into a callable iteratee: a function passes through unchanged, a map becomes a
 * `matches` predicate, a string becomes a `property` accessor, and anything else falls back to
 * `identity`.
 * @example
 *   iteratee("a")({ "a": 1 });               // => 1
 *   iteratee({ "a": 1 })({ "a": 1, "b": 2 }); // => true
 */
export function iteratee(spec) returns function {
  if (spec is function) { return spec; }
  if (spec is map) { return matches(spec); }
  if (spec is string) { return property(spec); }
  return identity;
}

/**
 * Builds a predicate that's `true` for any map holding `source`'s entries — a partial deep match,
 * via the `pick(obj, keys(source)) == source` trick (`==` is already deep structural equality).
 * @example
 *   matches({ "a": 1 })({ "a": 1, "b": 2 }); // => true
 *   matches({ "a": 1 })({ "a": 2, "b": 2 }); // => false
 */
export function matches(source is map) returns function {
  return (obj is map) => (pick(obj, keys(source)) == source);
}
/**
 * Builds a predicate that's `true` when `path` of a given object equals `srcValue`, via `getAt`.
 * @example
 *   matchesProperty("a.b", 1)({ "a": { "b": 1 } }); // => true
 */
export function matchesProperty(path, srcValue) returns function {
  return (obj) => (getAt(obj, path) == srcValue);
}

/**
 * Builds a function that calls every function in `funcs` with `val`, collecting results into an
 * array in `funcs`' order.
 * @example
 *   over([(val) => val + 1, (val) => val - 1])(5); // => [6, 4]
 */
export function over(funcs is array) returns function {
  return (val) => mapValues(funcs, (func, _seq) => func(val));
}
/**
 * Builds a predicate that's `true` only when every function in `funcs` returns truthy for `val`.
 * @example
 *   overEvery([(val) => val > 0, (val) => val < 10])(5); // => true
 */
export function overEvery(funcs is array) returns function {
  return (val) => all(funcs, (func) => func(val));
}
/**
 * Builds a predicate that's `true` when any function in `funcs` returns truthy for `val`.
 * @example
 *   overSome([(val) => val < 0, (val) => val > 10])(5); // => false
 */
export function overSome(funcs is array) returns function {
  return (val) => any(funcs, (func) => func(val));
}

/**
 * Builds a function that reads `path` off whatever it's given, via `getAt`.
 * @example
 *   property("a.b")({ "a": { "b": 1 } }); // => 1
 */
export function property(path) returns function {
  return (obj) => getAt(obj, path);
}
/**
 * The reverse of `property`: fixes the object up front and builds a function that reads whatever
 * path it's given off of it.
 * @example
 *   propertyOf({ "a": { "b": 1 } })("a.b"); // => 1
 */
export function propertyOf(obj) returns function {
  return (path) => getAt(obj, path);
}

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

/**
 * Calls `func(seq)` for `seq` from `0` to `count - 1`, collecting results — `count < 1` returns
 * `[]`. With no `func` given, defaults to `identity`, so `times(3)` is just `[0, 1, 2]`.
 * @example
 *   times(3, (seq) => seq * seq); // => [0, 1, 4]
 *   times(3);                     // => [0, 1, 2]
 */
export function times(count is number, func is function) returns array {
  if (count < 1) { return []; }
  return mapArray(range(0, count - 1), func);
}
export function times(count is number) returns array {
  return times(count, identity);
}
//--

// == [Function values] --

export const UtilsFuncs = {
  "attempt":         (func)           => attempt(func),
  "cond":            (pairs)          => cond(pairs),
  "conforms":        (source)         => conforms(source),
  "conformsTo":      (obj, source)    => conformsTo(obj, source),
  "constant":        (val)            => constant(val),
  "identity":        identity,
  "identity2":       identity2,
  "identity3":       identity3,
  "iteratee":        (spec)           => iteratee(spec),
  "matches":         (source)         => matches(source),
  "matchesProperty": (path, srcValue) => matchesProperty(path, srcValue),
  "noop":            noop,
  "noop1":           noop1,
  "noop2":           noop2,
  "noop3":           noop3,
  "over":            (funcs)          => over(funcs),
  "overEvery":       (funcs)          => overEvery(funcs),
  "overSome":        (funcs)          => overSome(funcs),
  "property":        (path)           => property(path),
  "propertyOf":      (obj)            => propertyOf(obj),
  "rangeRight":      (from, to)       => rangeRight(from, to),
  "times":           (count, func)    => times(count, func),
};
