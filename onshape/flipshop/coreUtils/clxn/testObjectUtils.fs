FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
//
import(path : "f6ab954150609e1e2e014a1e", version : "fdc8b0e93654776959ce7a8a"); // runTests
// Testing this:
import(path : "607f97fc690581579d1d4a08", version : "941ddac27a61e6b665cfeba6"); // clxnGetset <- testing this
import(path : "54590bc1c9cee0141b968fbb", version : "54952c334c14106eac53e310"); // clxnUtils  <- testing this

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

const gtOne = function(val, _key) { return val > 1; };

export const FindKeyCases = [
  [[{ "a": 1, "b": 2, "c": 3 }, gtOne],  "b"],
  [[{ "a": 1 },                 gtOne],  undefined],
];
function runFindKeyTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "findKey", verbose, FindKeyCases, function(args is array) { return findKey(args[0], args[1]); });
}

export const FindLastKeyCases = [
  [[{ "a": 1, "b": 2, "c": 3 }, gtOne],  "c"],
  [[{ "a": 1 },                 gtOne],  undefined],
];
function runFindLastKeyTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "findLastKey", verbose, FindLastKeyCases, function(args is array) { return findLastKey(args[0], args[1]); });
}

// == [invert / invertBy] ==

export const InvertCases = [
  [[{ "a": 1, "b": 2, "c": 1 }],  { "1": "c", "2": "b" },  'a repeated value keeps only its last key'],
];
function runInvertTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "invert", verbose, InvertCases, function(args is array) { return invert(args[0]); });
}

export const InvertByCases = [
  [[{ "a": 1, "b": 2, "c": 1 }, function(val, _key) { return "" ~ val; }],  { "1": ["a", "c"], "2": ["b"] }],
];
function runInvertByTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "invertBy", verbose, InvertByCases, function(args is array) { return invertBy(args[0], args[1]); });
}

// == [mapKeys] ==

export const MapKeysCases = [
  [[{ "a": 1, "b": 2 }, function(val, key) { return key ~ val; }],  { "a1": 1, "b2": 2 }],
];
function runMapKeysTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "mapKeys", verbose, MapKeysCases, function(args is array) { return mapKeys(args[0], args[1]); });
}

// == [omit / omitBy] ==

export const OmitCases = [
  [[{ "a": 1, "b": 2, "c": 3 }, ["b"]],  { "a": 1, "c": 3 }],
  [[{ "a": 1 },                 []],     { "a": 1 }],
];
function runOmitTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "omit", verbose, OmitCases, function(args is array) { return omit(args[0], args[1]); });
}

export const OmitByCases = [
  [[{ "a": 1, "b": 2, "c": 3 }, gtOne],  { "a": 1 }],
];
function runOmitByTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "omitBy", verbose, OmitByCases, function(args is array) { return omitBy(args[0], args[1]); });
}

// == [toPairs] ==

export const ToPairsCases = [
  [[{ "a": 1, "b": 2 }],  [["a", 1], ["b", 2]]],
  [[{}],                  []],
];
function runToPairsTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "toPairs", verbose, ToPairsCases, function(args is array) { return toPairs(args[0]); });
}
