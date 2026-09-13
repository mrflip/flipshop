FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
import(path : "4ebdc64943b566160ea5cc28", version : "3684960ff67d0db556614489");
import(path : "6fcd20533bd2df7c4094a0bf", version : "5849e1654325cddeda6f7440");
import(path : "dd812faf6ff4099cda4aa0eb", version : "ccf1aec9c08b9ad59691b4d4");
import(path : "66e287bede293cb227dfb89c", version : "e6c9aacc05cf0a6f15c7d4c7");
import(path : "08b6ba15b8255611bafe7520", version : "2a3b23fb0cdd7a00cb549f43");
import(path : "f6ab954150609e1e2e014a1e", version : "cefd17945c168add0f500dad");

const SuiteTitle = "Object Utils";

// == [Run tests] ==

annotation { "Feature Type Name": 'Run ' ~ SuiteTitle ~ ' Tests', "Feature Type Description": "Internal Tests" }
export const runObjectUtilsTestsFS = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Verbose", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE, "Default": false }
  definition.verbose is boolean;
}
{
  const verbose = ifNil(definition.verbose, false);
  try {
    runFindKeyTests(context, verbose);
    runFindLastKeyTests(context, verbose);
    runInvertTests(context, verbose);
    runInvertByTests(context, verbose);
    runMapKeysTests(context, verbose);
    runOmitTests(context, verbose);
    runOmitByTests(context, verbose);
    runToPairsTests(context, verbose);
    //
    if (! verbose) { debug(context, starbanner('** ' ~ SuiteTitle ~ ' Tests ran successfully **')); }
  } catch (err) {
    debug(context, starbanner('** Error in ' ~ SuiteTitle ~ ' Tests: ' ~ err ~ ' **'));
  }
});

// == [findKey / findLastKey] ==

const gtOne = function(val) { return val > 1; };

export const FindKeyCases = [
  [[{ "a": 1, "b": 2, "c": 3 }, gtOne],  "b"],
  [[{ "a": 1 },                 gtOne],  undefined],
];
export function runFindKeyTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "findKey", verbose, FindKeyCases, function(args is array) { return findKey(args[0], args[1]); });
}

export const FindLastKeyCases = [
  [[{ "a": 1, "b": 2, "c": 3 }, gtOne],  "c"],
  [[{ "a": 1 },                 gtOne],  undefined],
];
export function runFindLastKeyTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "findLastKey", verbose, FindLastKeyCases, function(args is array) { return findLastKey(args[0], args[1]); });
}

// == [invert / invertBy] ==

export const InvertCases = [
  [[{ "a": 1, "b": 2, "c": 1 }],  { "1": "c", "2": "b" },  'a repeated value keeps only its last key'],
];
export function runInvertTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "invert", verbose, InvertCases, function(args is array) { return invert(args[0]); });
}

export const InvertByCases = [
  [[{ "a": 1, "b": 2, "c": 1 }, function(val) { return "" ~ val; }],  { "1": ["a", "c"], "2": ["b"] }],
];
export function runInvertByTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "invertBy", verbose, InvertByCases, function(args is array) { return invertBy(args[0], args[1]); });
}

// == [mapKeys] ==

export const MapKeysCases = [
  [[{ "a": 1, "b": 2 }, function(val, key) { return key ~ val; }],  { "a1": 1, "b2": 2 }],
];
export function runMapKeysTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "mapKeys", verbose, MapKeysCases, function(args is array) { return mapKeys(args[0], args[1]); });
}

// == [omit / omitBy] ==

export const OmitCases = [
  [[{ "a": 1, "b": 2, "c": 3 }, ["b"]],  { "a": 1, "c": 3 }],
  [[{ "a": 1 },                 []],     { "a": 1 }],
];
export function runOmitTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "omit", verbose, OmitCases, function(args is array) { return omit(args[0], args[1]); });
}

export const OmitByCases = [
  [[{ "a": 1, "b": 2, "c": 3 }, gtOne],  { "a": 1 }],
];
export function runOmitByTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "omitBy", verbose, OmitByCases, function(args is array) { return omitBy(args[0], args[1]); });
}

// == [toPairs] ==

export const ToPairsCases = [
  [[{ "a": 1, "b": 2 }],  [["a", 1], ["b", 2]]],
  [[{}],                  []],
];
export function runToPairsTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "toPairs", verbose, ToPairsCases, function(args is array) { return toPairs(args[0]); });
}
