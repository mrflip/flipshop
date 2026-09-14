FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
//
import(path : "607f97fc690581579d1d4a08", version : "ddabef1e98b472ccc39c7916"); // clxnGetset, for getAt, iteratee, etc
import(path : "54590bc1c9cee0141b968fbb", version : "7092c264891606b4352b1f0d"); // clxnUtils, for mapValues
import(path : "dd812faf6ff4099cda4aa0eb", version : "592a13b20a544325ea81470d"); // stringUtils, for strTake
import(path : "66e287bede293cb227dfb89c", version : "e7ae3a5f5d4835f9c96161e5"); // typeUtils, for ifNil &c

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
  try silent {
    return func();
  } catch (error) {
    return error;
  }
});
/** Same as `attempt` — except Onshape logs the error due to the lack of the 'try silent' keyword */
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
 * per-key rules. Returns `(obj, seq)`, discarding `seq` — this file's iterators all take that
 * two-argument shape so they can sit in the same callback slot.
 * @example
 *   const isAdult = conforms({ "age": (age, _key) => age >= 18 });
 *   isAdult({ "age": 20 }, 0); // => true
 */
export const conforms = (function(source is map) returns function {
  return (obj is map, _seq) => conformsTo(obj, source);
});

/**
 * Whether every rule in `rules` holds against the same-keyed value of `obj` — a key in
 * `rules` but absent from `obj` reads as `undefined`, same as any other missing-key read.
 * Each rule gets `(val, key)`, matching how every other object-land iterator in this file
 * hands a value's key as the second argument.
 * @example
 *   conformsTo({ "a": 1, "b": 2 }, { "b": (n, _key) => n > 1 }); // => true
 *   conformsTo({ "a": 1, "b": 2 }, { "b": (n, _key) => n > 2 }); // => false
 */
export const conformsTo = (function(obj is map, rules is map) returns boolean {
  return all(keys(rules), (key) => rules[key](obj[key], key));
});

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
export const constantFunc = (function(val, arity is number) returns function { return constant(val, arity); });

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
 * Builds a function that calls every function in `funcs` with `(val, seq)`, collecting results
 * into an array in `funcs`' order. Each element of `funcs` is coerced through `iteratee`, so a
 * property-path string, `[path, srcValue]` array, or partial-match map can stand in for a literal
 * function, matching lodash's own `over`/`overEvery`/`overSome` docs.
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
 * `range` *(std)*, descending — inherits std `range`'s own inclusive-of-`to` convention rather
 * than lodash's exclusive one, matching how `range` itself is already mapped in this project.
 * @example
 *   rangeRight(0, 3); // => [3, 2, 1, 0]
 */
export const rangeRight = (function(from is number, to is number) returns array {
  return reverse(range(from, to));
});

/**
 * Calls `func(seq, seq)` for `seq` from `0` to `count - 1`, collecting results — `count < 1`
 * returns `[]`. There's no second value to offer alongside the index, so `seq` fills both slots.
 * With no `func` given, defaults to `identity`, so `times(3)` is just `[0, 1, 2]`.
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

export const HelperFuncs = {
  "attempt":         attempt,
  "attemptLoudly":   attemptLoudly,
  "cond":            (pairs)          => cond(pairs),
  "conforms":        conforms,
  "conformsTo":      conformsTo,
  "constantFunc":    constantFunc,
  "constant":        (val)           => constant(val),
  "doMany":          (count, func)    => doMany(count, func),
  "identity":        identity,
  "identity1":       identity1,
  "identity2":       identity2,
  "identity3":       identity3,
  "inRange":         (num, start, end) => inRange(num, start, end),
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
