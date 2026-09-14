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
export function curry3to0(func is function) returns function { return (_arg1, _arg2, _arg3) => func();               }
export function curry3to1(func is function) returns function { return (arg1,  _arg2, _arg3) => func(arg1);           }
export function curry3to2(func is function) returns function { return (arg1,  arg2,  _arg3) => func(arg1, arg2);     }

export function curry2to0(func is function) returns function { return (_arg1, _arg2) => func();           }
export function curry2to1(func is function) returns function { return (arg1,  _arg2) => func(arg1);       }
export function curry2to2(func is function) returns function { return (arg1,  arg2) => func(arg1, arg2);  }
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
export const attempt = (function(func is function) {
  try {
    return func();
  } catch (error) {
    return error;
  }
});
/** attempt, but it leaves the error in the log */
export const attemptLoudly = (function(func is function) {
  try {
    return func();
  } catch (err) {
    return err;
  }
});

/** Shared closure body for both `cond` overloads, once `pairs` is a `[rule, handler]` array with each `rule` already coerced through `iteratee`. */
function condClosure(pairs is array) returns function {
  return function(val, seq) {
    for (var pair in pairs) {
      if (pair[0](val, seq)) { return pair[1](val, seq); }
    }
    return Sentinel.ABSENT;
  };
}

/**
 * Builds a function that tries `pairs` in order, calling and returning the first `handler` whose
 * `rule` holds `val`, or `undefined` if none does. Each `rule` is coerced through `iteratee`, so a
 * property-path string, `[path, srcValue]` array, or partial-match map works in place of a
 * literal `rule(val, seq) => boolean` function — matching lodash's own `_.cond`, which runs
 * `_.iteratee` on every predicate.
 * @example
 *   const grade = cond([
 *     [(score, _seq) => score >= 90, constant("A")],
 *     [(score, _seq) => score >= 80, constant("B")],
 *     [constant(true),               constant("F")]
 *   ]);
 *   grade(95, 0); // => "A"
 *   grade(70, 0); // => "F"
 */
export function cond(pairs is array) returns function {
  return condClosure(mapValues(pairs, (pair, _seq) => [iteratee(pair[0]), pair[1]]));
}
/**
 * `cond`, keyed by rule instead of ordered by array position — each map key doubles as its own
 * `rule` (coerced through `iteratee`, so a key is naturally a property-path string), paired with
 * its value as the `handler`.
 * @example
 *   const speak = cond({ "isDog": constant("Woof"), "isCat": constant("Meow") });
 *   speak({ "isDog": true, "isCat": false }, 0); // => "Woof"
 */
export function cond(pairs is map) returns function {
  return condClosure(mapValues(keys(pairs), (rule is string, _seq) => [iteratee(rule), pairs[rule]]));
}

/**
 * Curried `conformsTo`: builds a rule that checks whether a given map conforms to `source`'s
 * per-key rules. Returns `(obj, seq)`, discarding `seq`, so it can sit in the same slot
 * every iterator in this file does.
 * @example
 *   const isAdult = conforms({ "age": (age, _key) => age >= 18 });
 *   isAdult({ "age": 20 }, 0); // => true
 */
export function conforms(source is map) returns function {
  return (obj is map, _seq) => conformsTo(obj, source);
}

/**
 * Whether every rule in `rules` holds against the same-keyed value of `obj` — a key in
 * `rules` but absent from `obj` reads as `undefined`, same as any other missing-key read.
 * Each rule gets `(val, key)`, matching how every other object-land iterator in this file
 * hands a value's key as the second argument.
 * @example
 *   conformsTo({ "a": 1, "b": 2 }, { "b": (n, _key) => n > 1 }); // => true
 *   conformsTo({ "a": 1, "b": 2 }, { "b": (n, _key) => n > 2 }); // => false
 */
export function conformsTo(obj is map, rules is map) returns boolean {
  return all(keys(rules), (key) => rules[key](obj[key], key));
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
 * Coerces `spec` into a callable iteratee: a function passes through unchanged, a map becomes a
 * `matches` rule, a string becomes a `property` accessor, a `[path, srcValue]` array becomes a
 * `matchesProperty` rule, and anything else falls back to `identity`.
 * @example
 *   iteratee("a")({ "a": 1 });                    // => 1
 *   iteratee({ "a": 1 })({ "a": 1, "b": 2 });      // => true
 *   iteratee(["a", 1])({ "a": 1, "b": 2 });        // => true
 */
export function iteratee(spec) returns function {
  if (spec is function) { return spec; }
  if (spec is map)      { return matches(spec); }
  if (spec is string)   { return property(spec); }
  if (spec is array)    { return matchesProperty(spec[0], spec[1]); }
  return identity;
}

/**
 * Builds a rule that's `true` for any map holding `source`'s entries — a partial deep match,
 * via the `pick(obj, keys(source)) == source` trick (`==` is already deep structural equality).
 * Returns `(obj, seq)`, discarding `seq`, so it can sit in the same slot every iterator in this
 * file does.
 * @example
 *   matches({ "a": 1 })({ "a": 1, "b": 2 }, 0); // => true
 *   matches({ "a": 1 })({ "a": 2, "b": 2 }, 0); // => false
 */
export function matches(source is map) returns function {
  return (obj is map, _seq) => (pick(obj, keys(source)) == source);
}
/**
 * Builds a rule that's `true` when `path` of a given object equals `srcValue`, via `getAt`.
 * Returns `(obj, seq)`, discarding `seq`, so it can sit in the same slot every iterator in this
 * file does.
 * @example
 *   matchesProperty("a.b", 1)({ "a": { "b": 1 } }, 0); // => true
 */
export function matchesProperty(path, srcValue) returns function {
  return (obj, _seq) => (getAt(obj, path) == srcValue);
}

/**
 * Builds a function that calls every function in `funcs` with `(val, seq)`, collecting results
 * into an array in `funcs`' order — the same two-argument shape every iterator in this file
 * uses, forwarded to each of `funcs` in turn. Each element of `funcs` is coerced through
 * `iteratee`, so a property-path string, `[path, srcValue]` array, or partial-match map can stand
 * in for a literal function, matching lodash's own `over`/`overEvery`/`overSome` docs.
 * @example
 *   over([(val, _seq) => val + 1, (val, _seq) => val - 1])(5, 0); // => [6, 4]
 */
export function over(funcs is array) returns function {
  const coercedFuncs = mapValues(funcs, (func, _idx) => iteratee(func));
  return (val, seq) => mapValues(coercedFuncs, (func, _idx) => func(val, seq));
}
/**
 * Builds a rule that's `true` only when every function in `funcs` returns truthy for
 * `(val, seq)`. Each element of `funcs` is coerced through `iteratee`; @see `over`.
 * @example
 *   overEvery([(val, _seq) => val > 0, (val, _seq) => val < 10])(5, 0); // => true
 */
export function overEvery(funcs is array) returns function {
  const coercedFuncs = mapValues(funcs, (func, _idx) => iteratee(func));
  return (val, seq) => all(coercedFuncs, (func) => func(val, seq));
}
/**
 * Builds a rule that's `true` when any function in `funcs` returns truthy for `(val, seq)`. Each
 * element of `funcs` is coerced through `iteratee`; @see `over`.
 * @example
 *   overSome([(val, _seq) => val < 0, (val, _seq) => val > 10])(5, 0); // => false
 */
export function overSome(funcs is array) returns function {
  const coercedFuncs = mapValues(funcs, (func, _idx) => iteratee(func));
  return (val, seq) => any(coercedFuncs, (func) => func(val, seq));
}

/**
 * Builds a function that reads `path` off whatever it's given, via `getAt`. Returns
 * `(obj, seq)`, discarding `seq`, so it can sit in the same slot every iterator in this file does.
 * @example
 *   property("a.b")({ "a": { "b": 1 } }, 0); // => 1
 */
export const property = (function(path) returns function {
  return (obj, _seq) => getAt(obj, path);
});

/**
 * The reverse of `property`: fixes the object up front and builds a function that reads whatever
 * path it's given off of it. Returns `(path, seq)`, discarding `seq`, for the same reason.
 * @example
 *   propertyOf({ "a": { "b": 1 } })("a.b", 0); // => 1
 */
export const propertyOf = (function(obj) returns function {
  return (path, _seq) => getAt(obj, path);
});

/**
 * `range` *(std)*, descending, via a plain `reverse` — inherits std `range`'s own inclusive-of-
 * `to` convention rather than lodash's exclusive one, matching how `range` itself is already
 * mapped in this project.
 * @example
 *   rangeRight(0, 3); // => [3, 2, 1, 0]
 */
export const rangeRight = (function(from is number, to is number) returns array {
  return reverse(range(from, to));
});

/**
 * Calls `func(seq, seq)` for `seq` from `0` to `count - 1`, collecting results — `count < 1`
 * returns `[]`. There's no second value to offer alongside the index, so `seq` fills both slots,
 * the same two-argument shape every iterator in this file uses (a manual loop rather than std's
 * `mapArray`, which only ever hands a callback one argument). With no `func` given, defaults to
 * `identity`, so `times(3)` is just `[0, 1, 2]`.
 * @example
 *   times(3, (seq, _seq2) => seq * seq); // => [0, 1, 4]
 *   times(3);                            // => [0, 1, 2]
 */
export function times(count is number, func is function) returns array {
  if (count < 1) { return []; }
  var result = makeArray(count);
  for (var seq = 0; seq < count; seq += 1) {
    result[seq] = func(seq, seq);
  }
  return result;
}
export function times(count is number) returns array {
  return range(0, count - 1);
}
export const doMany = (function(count is number, func is function) returns array {
  return times(count, func);
});

//--

// == [Function values] --

export const UtilsFuncs = {
  "attempt":         attempt,
  "attemptLoudly":   attemptLoudly,
  "cond":            cond,
  "conforms":        conforms,
  "conformsTo":      conformsTo,
  "constant":        constant,
  "doMany":          (count, func)    => doMany(count, func),
  "identity":        identity,
  "identity1":       identity1,
  "identity2":       identity2,
  "identity3":       identity3,
  "idsFor":          (id, tags)       => idsFor(id, tags),
  "inRange":         inRange,
  "iteratee":        (spec)           => iteratee(spec),
  "matches":         (source)         => matches(source),
  "matchesProperty": (path, srcValue) => matchesProperty(path, srcValue),
  "noop":            noop,
  "noop0":           noop0,
  "noop1":           noop1,
  "noop2":           noop2,
  "noop3":           noop3,
  "over":            (funcs)          => over(funcs),
  "overEvery":       (funcs)          => overEvery(funcs),
  "overSome":        (funcs)          => overSome(funcs),
  "parseJsonSafely": (rawjson, opts)  => parseJsonSafely(rawjson, opts),
  "property":        (path)           => property(path),
  "propertyOf":      (obj)            => propertyOf(obj),
  "rangeRight":      (from, to)       => rangeRight(from, to),
  "times":           (count, func)    => times(count, func),
};
