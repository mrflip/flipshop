FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
//
import(path : "f6ab954150609e1e2e014a1e", version : "939862c0f6583971525024d5"); // runTests
// Testing this:
import(path : "08b6ba15b8255611bafe7520", version : "25c448c89a44ac841877ddc3");

const SuiteTitle = "Misc Utils";

// == [Run tests] ==

annotation { "Feature Type Name": 'Run ' ~ SuiteTitle ~ ' Tests', "Feature Type Description": "Internal Tests" }
export const runMiscUtilsTestsFS = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Verbose", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE, "Default": false }
  definition.verbose is boolean;
}
{
  const verbose = ifNil(definition.verbose, false);
  try {
    runIdsForTests(context, verbose);
    runNoopTests(context, verbose);
    runCurryTests(context, verbose);
    runParseJsonSafelyTests(context, verbose);
    runRangeRightTests(context, verbose);
    runAttemptTests(context, verbose);
    runCondTests(context, verbose);
    runConformsTests(context, verbose);
    runConformsToTests(context, verbose);
    runConstantTests(context, verbose);
    runIdentityTests(context, verbose);
    runIterateeTests(context, verbose);
    runMatchesTests(context, verbose);
    runMatchesPropertyTests(context, verbose);
    runOverTests(context, verbose);
    runOverEveryTests(context, verbose);
    runOverSomeTests(context, verbose);
    runPropertyTests(context, verbose);
    runPropertyOfTests(context, verbose);
    runTimesTests(context, verbose);
    //
    if (! verbose) { debug(context, starbanner('** ' ~ SuiteTitle ~ ' Tests ran successfully **')); }
  } catch (err) {
    debug(context, starbanner('** Error in ' ~ SuiteTitle ~ ' Tests: ' ~ err ~ ' **'));
  }
});

// == [idsFor] ==
// Id has no documented equality via `==`, so these check the key set idsFor produces rather
// than the Id values themselves.

export const IdsForCases = [
  [[newId(), ["a", "b", "c"]], ["a", "b", "c"]],
  [[newId(), []],              []],
  [[newId(), ["only"]],        ["only"]],
];
export function runIdsForTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "idsFor", verbose, IdsForCases, function(args is array) { return keys(idsFor(args[0], args[1])); });
}

// == [noop] ==

export const NoopCases = [
  [[], undefined],
];
export function runNoopTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "noop", verbose, NoopCases, function(args is array) { return noop0(); });
}

// == [curryNtoM] ==

export const CurryCases = [
  [[curry2to0(function() { return "called"; }),                 "ignored1", "ignored2"], "called",   'curry2to0 drops both arguments'],
  [[curry2to1(function(val) { return val; }),                   "keep",     "drop"],      "keep",     'curry2to1 keeps the first argument, drops the second'],
  [[curry2to2(function(val1, val2) { return [val1, val2]; }),   "x",        "y"],         ["x", "y"], 'curry2to2 keeps both arguments'],
  [[curry3to0(function() { return "called"; }),                 1, 2, 3], "called", 'curry3to0 drops all three arguments'],
  [[curry3to1(function(val) { return val; }),                   1, 2, 3], 1,        'curry3to1 keeps only the first argument'],
  [[curry3to2(function(val1, val2) { return [val1, val2]; }),   1, 2, 3], [1, 2],   'curry3to2 keeps the first two arguments, drops the third'],
];
export function runCurryTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "curryNtoM", verbose, CurryCases, function(args is array) {
    const curried = args[0];
    return (size(args) == 3) ? curried(args[1], args[2]) : curried(args[1], args[2], args[3]);
  });
}

// == [parseJsonSafely] ==

export const ParseJsonSafelyCases = [
  [['{"a": 1, "b": [1, 2, 3]}'],                       { "a": 1, "b": [1, 2, 3] }],
  [['{"a": 1, "b": [1, 2, 3]}', { "detectUnits": false }], { "a": 1, "b": [1, 2, 3] }],
];
export function runParseJsonSafelyTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "parseJsonSafely", verbose, ParseJsonSafelyCases, function(args is array) {
    return (size(args) <= 1) ? parseJsonSafely(args[0]) : parseJsonSafely(args[0], args[1]);
  });
}

// == [rangeRight] ==

export const RangeRightCases = [
  [[0, 3], [3, 2, 1, 0]],
  [[2, 2], [2],           'from == to is a single-element range'],
];
export function runRangeRightTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "rangeRight", verbose, RangeRightCases, function(args is array) { return rangeRight(args[0], args[1]); });
}

// == [attempt] ==

export const AttemptCases = [
  [[function() { return 42; }],    42],
  [[function() { throw "boom"; }], "boom", 'a thrown value comes back instead of propagating'],
];
export function runAttemptTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "attempt", verbose, AttemptCases, function(args is array) { return attempt(args[0]); });
}

// == [cond] ==

const gradeCond = cond([
  [(score, _seq) => score >= 90, constant("A")],
  [(score, _seq) => score >= 80, constant("B")],
  [constant(true),         constant("F")]
]);
export const CondCases = [
  [[gradeCond, 95], "A"],
  [[gradeCond, 82], "B"],
  [[gradeCond, 70], "F"],
];
export function runCondTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "cond", verbose, CondCases, function(args is array) { return args[0](args[1], 0); });
}

// == [conforms / conformsTo] ==

const isAdultConforms = conforms({ "age": (age, _seq) => age >= 18 });
export const ConformsCases = [
  [[isAdultConforms, { "age": 20 }], true],
  [[isAdultConforms, { "age": 10 }], false],
];
export function runConformsTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "conforms", verbose, ConformsCases, function(args is array) { return args[0](args[1], 0); });
}

export const ConformsToCases = [
  [[{ "a": 1, "b": 2 }, { "b": (n, _key) => n > 1 }], true],
  [[{ "a": 1, "b": 2 }, { "b": (n, _key) => n > 2 }], false],
];
export function runConformsToTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "conformsTo", verbose, ConformsToCases, function(args is array) { return conformsTo(args[0], args[1]); });
}

// == [constant] ==

export const ConstantCases = [
  [[constant(5)],           5],
  [[constant({ "a": 1 })], { "a": 1 }],
];
export function runConstantTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "constant", verbose, ConstantCases, function(args is array) { return args[0]("ignored", undefined); });
}

// == [identity] ==

export const IdentityCases = [
  [[5],           5],
  [[{ "a": 1 }], { "a": 1 }],
  [[[1, 2, 3]],  [1, 2, 3]],
];
export function runIdentityTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "identity", verbose, IdentityCases, function(args is array) { return identity(args[0], undefined); });
}

// == [iteratee] ==

export const IterateeCases = [
  [["a",         { "a": 1 }],            1,    'string spec becomes a property accessor'],
  [[{ "a": 1 },  { "a": 1, "b": 2 }],    true, 'map spec becomes a matches predicate'],
  [[identity,    5],                     5,    'function spec passes through unchanged'],
];
export function runIterateeTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "iteratee", verbose, IterateeCases, function(args is array) { return iteratee(args[0])(args[1], 0); });
}

// == [matches / matchesProperty] ==

export const MatchesCases = [
  [[{ "a": 1 }, { "a": 1, "b": 2 }], true],
  [[{ "a": 1 }, { "a": 2, "b": 2 }], false],
];
export function runMatchesTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "matches", verbose, MatchesCases, function(args is array) { return matches(args[0])(args[1], 0); });
}

export const MatchesPropertyCases = [
  [["a.b", 1, { "a": { "b": 1 } }], true],
  [["a.b", 2, { "a": { "b": 1 } }], false],
];
export function runMatchesPropertyTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "matchesProperty", verbose, MatchesPropertyCases, function(args is array) { return matchesProperty(args[0], args[1])(args[2], 0); });
}

// == [over / overEvery / overSome] ==

export const OverCases = [
  [[[(val, _seq) => val + 1, (val, _seq) => val - 1], 5], [6, 4]],
];
export function runOverTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "over", verbose, OverCases, function(args is array) { return over(args[0])(args[1], 0); });
}

export const OverEveryCases = [
  [[[(val, _seq) => val > 0, (val, _seq) => val < 10], 5],  true],
  [[[(val, _seq) => val > 0, (val, _seq) => val < 10], 15], false],
];
export function runOverEveryTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "overEvery", verbose, OverEveryCases, function(args is array) { return overEvery(args[0])(args[1], 0); });
}

export const OverSomeCases = [
  [[[(val, _seq) => val < 0, (val, _seq) => val > 10], 5],  false],
  [[[(val, _seq) => val < 0, (val, _seq) => val > 10], 15], true],
];
export function runOverSomeTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "overSome", verbose, OverSomeCases, function(args is array) { return overSome(args[0])(args[1], 0); });
}

// == [property / propertyOf] ==

export const PropertyCases = [
  [["a.b", { "a": { "b": 1 } }], 1],
  [["a.c", { "a": { "b": 1 } }], undefined],
];
export function runPropertyTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "property", verbose, PropertyCases, function(args is array) { return property(args[0])(args[1], 0); });
}

export const PropertyOfCases = [
  [[{ "a": { "b": 1 } }, "a.b"], 1],
];
export function runPropertyOfTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "propertyOf", verbose, PropertyOfCases, function(args is array) { return propertyOf(args[0])(args[1], 0); });
}

// == [times] ==

export const TimesCases = [
  [[3, (seq, _x) => seq * seq], [0, 1, 4]],
  [[3],                         [0, 1, 2], 'no func given defaults to identity'],
  [[0, identity],               []],
];
export function runTimesTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "times", verbose, TimesCases, function(args is array) {
    return (size(args) == 1) ? times(args[0]) : times(args[0], args[1]);
  });
}
