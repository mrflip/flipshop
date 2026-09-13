FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
import(path : "4ebdc64943b566160ea5cc28", version : "3684960ff67d0db556614489");
import(path : "6fcd20533bd2df7c4094a0bf", version : "5849e1654325cddeda6f7440");
import(path : "dd812faf6ff4099cda4aa0eb", version : "ccf1aec9c08b9ad59691b4d4");
import(path : "66e287bede293cb227dfb89c", version : "e6c9aacc05cf0a6f15c7d4c7");
import(path : "08b6ba15b8255611bafe7520", version : "2a3b23fb0cdd7a00cb549f43");
import(path : "f6ab954150609e1e2e014a1e", version : "cefd17945c168add0f500dad");

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
  return runTests(context, "noop", verbose, NoopCases, function(args is array) { return noop(); });
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
