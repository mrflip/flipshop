FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
//
import(path : "f6ab954150609e1e2e014a1e", version : "939862c0f6583971525024d5"); // runTests
// Testing this:
import(path : "66e287bede293cb227dfb89c", version : "92efbb7ccaa5d62b7bde80f1"); // typeUtils (testing this)

const SuiteTitle = "Type Utils";

// == [Run tests] ==

annotation { "Feature Type Name": 'Run ' ~ SuiteTitle ~ ' Tests', "Feature Type Description": "Internal Tests" }
export const runTypeUtilsTestsFS = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Verbose", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE, "Default": false }
  definition.verbose is boolean;
}
{
  const verbose = ifNil(definition.verbose, false);
  try {
    runIfNilTests(context, verbose);
    runIfBlankTests(context, verbose);
    runTruthyTests(context, verbose);
    runIsPresentTests(context, verbose);
    runIsNilTests(context, verbose);
    runStrBlankTests(context, verbose);
    runIfZeroNumberTests(context, verbose);
    runIfZeroVWUTests(context, verbose);
    runIsEmptyTests(context, verbose);
    runVector2Tests(context, verbose);
    runCastArrayTests(context, verbose);
    //
    if (! verbose) { debug(context, starbanner('** ' ~ SuiteTitle ~ ' Tests ran successfully **')); }
  } catch (err) {
    debug(context, starbanner('** Error in ' ~ SuiteTitle ~ ' Tests: ' ~ err ~ ' **'));
  }
});

// == [ifNil / ifBlank] ==

export const IfNilCases = [
  [[undefined, "fallback"], "fallback"],
  [[1,         "fallback"], 1],
  [[0,         "fallback"], 0, '0 is not nil'],
  [["",        "fallback"], "", 'empty string is not nil'],
];
export function runIfNilTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "ifNil", verbose, IfNilCases, function(args is array) { return ifNil(args[0], args[1]); });
}

export const IfBlankCases = [
  [[undefined, "fallback"], "fallback"],
  [["",        "fallback"], "fallback"],
  [["hi",      "fallback"], "hi"],
];
export function runIfBlankTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "ifBlank", verbose, IfBlankCases, function(args is array) { return ifBlank(args[0], args[1]); });
}

// == [truthy / isPresent / isNil / strBlank] ==

export const TruthyCases = [
  [[undefined], false],
  [[false],     false],
  [[true],      true],
  [[0],         true, '0 is truthy, unlike JS'],
  [[""],        true, 'empty string is truthy, unlike JS'],
];
export function runTruthyTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "truthy", verbose, TruthyCases, function(args is array) { return truthy(args[0]); });
}

export const IsPresentCases = [
  [[undefined], false],
  [[0],         true],
  [[false],     true],
  [[""],        true],
];
export function runIsPresentTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "isPresent", verbose, IsPresentCases, function(args is array) { return isPresent(args[0]); });
}

export const IsNilCases = [
  [[undefined], true],
  [[0],         false],
  [[false],     false],
  [[""],        false],
];
export function runIsNilTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "isNil", verbose, IsNilCases, function(args is array) { return isNil(args[0]); });
}

export const StrBlankCases = [
  [[undefined], true],
  [[""],        true],
  [["x"],       false],
];
export function runStrBlankTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "strBlank", verbose, StrBlankCases, function(args is array) { return strBlank(args[0]); });
}

// == [ifZero] ==

export const IfZeroNumberCases = [
  [[0,         "fallback"], "fallback"],
  [[undefined, "fallback"], "fallback"],
  [[5,         "fallback"], 5],
];
export function runIfZeroNumberTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "ifZero (number)", verbose, IfZeroNumberCases, function(args is array) { return ifZero(args[0], args[1]); });
}

export const IfZeroVWUCases = [
  [[0 * millimeter,         "fallback"], "fallback"],
  [[undefined,              "fallback"], "fallback"],
  [[5 * millimeter,         "fallback"], 5 * millimeter],
];
export function runIfZeroVWUTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "ifZero (ValueWithUnits)", verbose, IfZeroVWUCases, function(args is array) { return ifZero(args[0], args[1]); });
}

// == [isEmpty] ==

export const IsEmptyCases = [
  [[undefined],   true],
  [[""],          true],
  [[[]],          true],
  [[{}],          true],
  [[[1]],         false],
  [[{ "a": 1 }],  false],
  [[true],        false, 'unlike lodash, a boolean is never empty'],
  [[1],           false, 'unlike lodash, a number is never empty'],
];
export function runIsEmptyTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "isEmpty", verbose, IsEmptyCases, function(args is array) { return isEmpty(args[0]); });
}

// == [vector2] ==

export const Vector2Cases = [
  [[vector(1, 2, 3), vector(1, 2)], true],
  [[vector(0, 0, 5), vector(0, 0)], true],
  [[vector(1, 2, 3), vector(9, 9)], false],
];
export function runVector2Tests(context is Context, verbose is boolean) returns map {
  return runTests(context, "vector2", verbose, Vector2Cases, function(args is array) { return tolerantEquals(vector2(args[0]), args[1]); });
}

// == [castArray] ==

export const CastArrayCases = [
  [[1],       [1]],
  [[[1, 2]],  [1, 2]],
  [[undefined], [undefined]],
];
export function runCastArrayTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "castArray", verbose, CastArrayCases, function(args is array) { return castArray(args[0]); });
}
