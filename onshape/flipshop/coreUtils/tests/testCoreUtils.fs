FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
import(path : "4ebdc64943b566160ea5cc28", version : "3684960ff67d0db556614489");
import(path : "6fcd20533bd2df7c4094a0bf", version : "5849e1654325cddeda6f7440");
import(path : "dd812faf6ff4099cda4aa0eb", version : "ccf1aec9c08b9ad59691b4d4");
import(path : "66e287bede293cb227dfb89c", version : "e6c9aacc05cf0a6f15c7d4c7");
import(path : "08b6ba15b8255611bafe7520", version : "2a3b23fb0cdd7a00cb549f43");
import(path : "f6ab954150609e1e2e014a1e", version : "cefd17945c168add0f500dad");

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
  }
});

function runLanguageStupiditiesTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "testLanguageStupidities", verbose, [
      [[{ a: undefined }],          {},           'keys with undefined values are silently dropped'],
      [[splitByRegexp("!!!", "!")], ["", "", ""], 'should have four segments; terminal empty segment discarded'],
      [[splitByRegexp("!", "!")],   [""],         'should have two segments; terminal empty segment discarded'],
      [[splitByRegexp("a!", "!")],  ["a"],        'should have two segments; terminal empty segment discarded'],
      [[splitByRegexp("!a", "!")],  ["", "a"],    'correctly has two segments; initial empty segment is kept'],

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
