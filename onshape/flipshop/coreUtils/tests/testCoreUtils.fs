FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
//
import(path : "f6ab954150609e1e2e014a1e", version : "939862c0f6583971525024d5"); // runTests
// Testing this:
import(path : "dcee57677ef58f0c8e045269", version : "0d87751b06d38b222b661b1a");

const SuiteTitle = "Core Utils";

// == [Run tests] ==

/**
 * Part feature: extrudes `text` sized and aligned within a bounding plate at each selected plane.
 * Delegates all geometry to @see `textChip`.
 * @param definition {{
 *   @field json {string} : stringified JSON to produce
 * }}
 */
annotation { "Feature Type Name": 'Run ' ~ SuiteTitle ~ ' Tests', "Feature Type Description": "Internal Tests" }
export const runCoreUtilsTestsFS = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Verbose", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE, "Default": false }
  definition.verbose is boolean;
}
{
  try {
    const verbose = ifNil(definition.verbose, false);
    runLanguageStupiditiesTests(context, verbose);
    runSizeofTests(context, verbose);
    if (! verbose) { debug(context, starbanner('** ' ~ SuiteTitle ~ ' Tests ran successfully **')); }
  } catch (err) {
    debug(context, starbanner('** Error in ' ~ SuiteTitle ~ ' Tests: ' ~ err ~ ' **'));
    throw err;
  }
});

function wthComparison(aa, bb) {
  try silent {
    return aa < bb;
  } catch (err) {
    return "" ~ err;
  }
}

function runLanguageStupiditiesTests(context is Context, verbose is boolean) returns map {

  return runTests(context, "testLanguageStupidities", verbose, [
      [[{ a: undefined }],          {},                    'keys with undefined values are silently dropped'],
      [[splitByRegexp("!!!", "!")], ["", "", ""],          'should have four segments; terminal empty segment discarded'],
      [[splitByRegexp("!", "!")],   [""],                  'should have two segments; terminal empty segment discarded'],
      [[splitByRegexp("a!", "!")],  ["a"],                 'should have two segments; terminal empty segment discarded'],
      [[splitByRegexp("!a", "!")],  ["", "a"],             'correctly has two segments; initial empty segment is kept'],
      //
      [[wthComparison(1,   2)],     true,                 'correctly compares two numbers'],
      [[wthComparison("a", "b")],   "Execution error",    'Can not compare string and string.'],
      [[wthComparison("a", 1)],     "Execution error",    'Can not compare string and number.'],
      [[wthComparison([], 1)],      "Execution error",    'Can not compare array and number.'],
      [[wthComparison({},  [])],    "Execution error",    'Can not compare map and array.'],
      [[wthComparison([1], [2])],   "Execution error",    'Can not compare array and array.'],
      [[strCmp("a", "b")],          -1,                   'we wrote a dumb string comparison workaround'],
      //
      [[strOrderBy(["z", "", "a", "b", "a"])], ["", "a", "a", "b", "z"], ''],

      [[mergeMaps({ foo: { a: 1, b: 1 } }, { foo: { b: 2, c: 2 } })], { foo: { b:2, c: 2 } }, "mergeMaps does not merge maps, it clobbers any keys that collide even if their values are maps"],
  ], (args) => args[0]);
}

export function runSizeofTests(context is Context, verbose is boolean) returns map {
    return runTests(context, "sizeof", verbose, [
        [[ []                             ],  0],
        [[ {}                             ],  0],
        [[ undefined                      ],  0],
        [[ ""                             ],  0],
        // arrays
        [[ [[]]                           ],  1],
        [[ [{}]                           ],  1],
        [[ [undefined]                    ],  1],
        [[ [0]                            ],  1],
        [[ [0, 1, 2]                      ],  3],
        // keys with undefined values are removed!!
        [[ { a: undefined }               ],  0],
        [[ { a: undefined, b: [], c: "" } ],  2],
        //
        [[ { a: 1 }                       ],  1],
        [[ { a: 1, b: 2 }                 ],  2],
        // strings
        [[ "12345"                        ],  5],
    ], function(args is array) { return sizeof(args[0]); });
}
