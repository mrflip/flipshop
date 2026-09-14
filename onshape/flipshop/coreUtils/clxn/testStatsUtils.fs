FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
//
import(path : "4ebdc64943b566160ea5cc28", version : "49324f8ccdd2bc8f00f68a0f");
import(path : "f6ab954150609e1e2e014a1e", version : "17748a1c9444555f3047e363");
//
import(path : "54590bc1c9cee0141b968fbb", version : "502cf2d68de5897e8d3b8586"); // clxnUtils

const SuiteTitle = "Number Utils";

// == [Run tests] ==

annotation { "Feature Type Name": 'Run ' ~ SuiteTitle ~ ' Tests', "Feature Type Description": "Internal Tests" }
export const runNumberUtilsTestsFS = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Verbose", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE, "Default": false }
  definition.verbose is boolean;
}
{
  const verbose = ifNil(definition.verbose, false);
  try {
    runInRangeTests(context, verbose);
    runMaxByTests(context, verbose);
    runMeanByTests(context, verbose);
    runMinByTests(context, verbose);
    runSumByTests(context, verbose);
    //
    if (! verbose) { debug(context, starbanner('** ' ~ SuiteTitle ~ ' Tests ran successfully **')); }
  } catch (err) {
    debug(context, starbanner('** Error in ' ~ SuiteTitle ~ ' Tests: ' ~ err ~ ' **'));
  }
});

// == [inRange] ==

export const InRangeCases = [
  [[3, 5],     true,   'implicit start of 0'],
  [[3, 1, 5],  true],
  [[5, 1, 5],  false,  'upper bound is exclusive'],
  [[1, 1, 5],  true,   'lower bound is inclusive'],
  [[3, 5, 1],  true,   'bounds auto-swap when start > end'],
];
export function runInRangeTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "inRange", verbose, InRangeCases, function(args is array) {
    return (size(args) <= 2) ? inRange(args[0], args[1]) : inRange(args[0], args[1], args[2]);
  });
}

// == [maxBy / minBy] ==

const byN = function(val, _seq) { return val.n; };

export const MaxByCases = [
  [[[{ n: 1 }, { n: 3 }, { n: 2 }], byN],  { n: 3 }],
  [[[], byN],                              undefined],
];
export function runMaxByTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "maxBy", verbose, MaxByCases, function(args is array) { return maxBy(args[0], args[1]); });
}

export const MinByCases = [
  [[[{ n: 1 }, { n: 3 }, { n: 2 }], byN],  { n: 1 }],
  [[[], byN],                              undefined],
];
export function runMinByTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "minBy", verbose, MinByCases, function(args is array) { return minBy(args[0], args[1]); });
}

// == [meanBy / sumBy] ==

export const MeanByCases = [
  [[[{ n: 2 }, { n: 4 }], byN],  3],
];
export function runMeanByTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "meanBy", verbose, MeanByCases, function(args is array) { return meanBy(args[0], args[1]); });
}

export const SumByCases = [
  [[[{ n: 2 }, { n: 4 }], byN],  6],
];
export function runSumByTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "sumBy", verbose, SumByCases, function(args is array) { return sumBy(args[0], args[1]); });
}
