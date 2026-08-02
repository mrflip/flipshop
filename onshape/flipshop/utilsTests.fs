FeatureScript 3029;
import(path : "onshape/std/common.fs", version : "3029.0");
import(path : "58963520be3fe612d10b6d2e", version : "a3fca0f70c41654575775503");
import(path : "c50e2363725f9cf513e36928", version : "f7420b370e103a85d2d29de4");
import(path : "e313a0b67ecb3be0415d2186", version : "49ce2ecc79daf3ab8bef3f8a");
import(path : "075be6354063579d5fedb3b7", version : "16b6cffc519512a719cbc22e");

// == [Run tests] ==

/**
 * Part feature: extrudes `text` sized and aligned within a bounding plate at each selected plane.
 * Delegates all geometry to @see `textChip`.
 * @param definition {{
 *   @field json {string} : stringified JSON to produce
 * }}
 */
annotation { "Feature Type Name": "Run Utils Tests", "Feature Type Description": "Internal Tests" } // , "Feature Name Template": "Tests of #testname"
export const mapKeysF = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Verbose", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE, "Default": false }
  definition.verbose is boolean;
}
{
  const verbose = ifNil(definition.verbose, false);
  runStrSliceTests(context, verbose);
  runStrTakeTests(context, verbose);
  runStrTakeRightTests(context, verbose);
  runSizeofTests(context, verbose);
  runMapValuesTests(context, verbose);
  runMapValues3Tests(context, verbose);
  runValuesAtTests(context, verbose);
  runPadTests(context, verbose);
  if (! verbose) { debug(context, "\n**********************************\n** Utils Tests ran successfully **\n**********************************"); }
});

export function runTests(context is Context, testname is string, verbose is boolean, testCases is array, testfunc is function) returns map{
    var results = []; var counts = { "OK": 0, "MISMATCH": 0, "ERROR": 0, total: 0 };
    for (var testCase in testCases) {
      const args        = testCase[0];
      const wanted      = testCase[1];
      var   grade       = undefined;
      var   actual      = undefined;
      try {
        actual = testfunc(args);
        grade  = (actual == wanted) ? "OK" : "MISMATCH";
      } catch (error) {
        actual = error;
        grade  = "ERROR";
      }
      results  = append(results, concatenateArrays([[grade, actual, wanted], args]));
      counts[grade] += 1;
      counts.total  += 1;
      if (grade != "OK") {
        debug(context, "Test " ~ grade ~ " for " ~ testname ~ args);
        debug(context, "Actual: " ~ [actual]);
        debug(context, "Wanted: " ~ [wanted]);
      }
    }
    const ok = (counts.total == counts.OK);
    if (verbose || (! ok)) {
      debug(context, "Tests for " ~ testname ~ ": " ~ counts);
      debug(context, results);
    }
    return { "counts": counts, "results": results, "OK": ok };
}

const StrSliceTestCases = [
    [["hello world",    0,     3                    ], "hel"         ],
    [["hello world",   -5],                            "world"       ],
    [["hello world",    0,     -1                   ], "hello worl"  ],
    [["hello world",    0],                            "hello world" ],
    [["hello world",    0,     SequencePosition.END ], "hello world" ],
    [["hello world",    3],                            "lo world"    ],
    [["hello world",    3,     SequencePosition.END ], "lo world"    ],
    [["hello world", -100,     100                  ], "hello world" ],
    [["hello world",    5,     2                    ], ""            ],
    [["hello world",  100,     200                  ], ""            ],
    [["hello world",   -3,     -1                   ], "rl"          ],
    [["hello world",    0,     0                    ], ""            ],
    [["hello world",    2,     2                    ], ""            ],
    [["hello world",  -11,     -6                   ], "hello"       ],
    [["abc",            0,     3                    ], "abc"         ],
    [["abc",            1,     3                    ], "bc"          ],
    [["abc",            2,     3                    ], "c"           ],
    [["abc",            3,     3                    ], ""            ],
    [["abc",            4,     3                    ], ""            ],
    [["abc",           -1,     3                    ], "c"           ],
    [["abc",           -2,     3                    ], "bc"          ],
    [["abc",           -3,     3                    ], "abc"         ],
    [["abc",            0,     2                    ], "ab"          ],
    [["abc",            1,     2                    ], "b"           ],
    [["abc",            2,     2                    ], ""            ],
    [["abc",            3,     2                    ], ""            ],
    [["abc",           -1,     2                    ], ""            ],
    [["abc",           -2,     2                    ], "b"           ],
    [["abc",           -3,     2                    ], "ab"          ],
    [["",               0],                            ""            ],
    [["",               0,     -1                   ], ""            ],
    [["",               0,     SequencePosition.END ], ""            ],
    [["",               3,     0                    ], ""            ],
    [["",               3,     5                    ], ""            ],
    [["",               3],                            ""            ],
    [["",               3,     SequencePosition.END ], ""            ],
    [["",              -3,     0                    ], ""            ],
    [["",              -3,     5                    ], ""            ],
    [["",              -3],                            ""            ],
    [["",              -3,     SequencePosition.END ], ""            ],
];
function runStrSliceTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "String Slice", verbose, StrSliceTestCases, function (args is array) returns string {
    return (size(args) == 2) ? strSlice(args[0], args[1]) : strSlice(args[0], args[1], args[2]);
  });
}

function runStrTakeTests(context is Context, verbose is boolean) returns map {
    return runTests(context, "strTake", verbose, [
    [["hello world",    30                          ], "hello world"         ],
    [["hello world",    12                          ], "hello world"         ],
    [["hello world",    11                          ], "hello world"         ],
    [["hello world",    10                          ], "hello worl"          ],
    [["hello world",    2                           ], "he"                 ],
    [["hello world",    1                           ], "h"         ],
    [["hello world",    0],                            ""       ],
    [["hello world",   -1],                            ""       ],
    [["hello world",   -5],                            ""       ],
    [["",               3],                            ""            ],
    [["",               1],                            ""            ],
    [["",               0],                            ""            ],
    [["",               -1],                           ""            ],
    [["",               -5],                           ""            ],
  ], (args is array) returns string => {
    return strTake(args[0], args[1]);
  });
}


function runStrTakeRightTests(context is Context, verbose is boolean) returns map {
    return runTests(context, "strTake", verbose, [
    [["hello world",    30                          ], "hello world"         ],
    [["hello world",    12                          ], "hello world"         ],
    [["hello world",    11                          ], "hello world"         ],
    [["hello world",    10                          ], "ello world"          ],
    [["hello world",    2                           ], "ld"                  ],
    [["hello world",    1                           ], "d"                   ],
    [["hello world",    0],                            ""       ],
    [["hello world",   -1],                            ""       ],
    [["hello world",   -5],                            ""       ],
    [["",               3],                            ""            ],
    [["",               1],                            ""            ],
    [["",               0],                            ""            ],
    [["",               -1],                           ""            ],
    [["",               -5],                           ""            ],
  ], (args is array) returns string => {
    return strTakeRight(args[0], args[1]);
  });
}

export function runPadTests(context is Context, verbose is boolean) returns map {
    return runTests(context, "padLeft", verbose, [
        [["", 0],                       ""],
        [["", -5],                      ""],
        [["", 0, " "],                  ""],
        [["", -5, " "],                 ""],
        [["", 3],                       "   "],
        [["", 3, " "],                  "   "],
        [["", 3, "  "],                 "   "],
        [["", 3, "1234"],               "123"],
    ], function(args is array) { return size(args) <= 2 ? padLeft(args[0], args[1]) : padLeft(args[0], args[1], args[2]); });
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
const mapValuesInspector2 = function(val, key)      { return [val, key]; };
const mapValuesInspector3 = function(val, key, seq) { return [val, key, seq]; };

export function runMapValues3Tests(context is Context, verbose is boolean) returns map {
    return runTests(context, "mapValues3", verbose, [
        [[ {},                             curry3to0(noop)],    {} ],
        [[ {},                             mapValuesInspector3], {} ],
        [[ { a: 11, b: 22 },               mapValuesInspector3], { a: [11, "a", 0], b: [22, "b", 1] } ],
        [[ { b: 22, a: 11 },               mapValuesInspector3], { a: [11, "a", 0], b: [22, "b", 1] } ],
        [[ { b: 22, a: 11, d: undefined }, mapValuesInspector3], { a: [11, "a", 0], b: [22, "b", 1] } ],
        //
        [[ {},               [],           curry3to0(noop)],    {} ],
        [[ {},               [],           mapValuesInspector3], {} ],
        [[ { a: 11, b: 22 }, [],           mapValuesInspector3], {} ],
        [[ { a: 11, b: 22 }, ["a", "b"],   mapValuesInspector3], { a: [11, "a", 0], b: [22, "b", 1] } ],
        [[ { a: 11, b: 22 }, ["b", "a"],   mapValuesInspector3], { a: [11, "a", 1], b: [22, "b", 0] } ],
        [[ { a: 11, b: 22 }, ["b"],        mapValuesInspector3], {                  b: [22, "b", 0] } ],
        //
        [[ {},               ["c"],        curry3to0(noop)],     { c: undefined } ],
        [[ {},               ["c"],        mapValuesInspector3], { c: [undefined, "c", 0] } ],
        [[ { a: 11, b: 22 }, ["c"],        mapValuesInspector3], { c: [undefined, "c", 0] } ],
        [[ { a: 11, b: 22 }, ["a", "c"],   mapValuesInspector3], { a: [11,        "a", 0], c: [undefined, "c", 1] } ],
        [[ { a: 11, b: 22 }, ["c", "b"],   mapValuesInspector3], { c: [undefined, "c", 0], b: [22, "b", 1] } ],
        //
        [[ [],               noop],                [] ],
        [[ [],               mapValuesInspector3], [] ],
        [[ [11, 22],         mapValuesInspector3], [[11, 0, 0], [22, 1, 1]] ],
        //
        [[ {},                             MissingPolicy.USE_UNDEFINED, curry3to0(noop)],     {} ],
        [[ {},                             MissingPolicy.USE_UNDEFINED, mapValuesInspector3], {} ],
        [[ { a: 11, b: 22 },               MissingPolicy.USE_UNDEFINED, mapValuesInspector3], { a: [11, "a", 0], b: [22, "b", 1] } ],
        [[ { b: 22, a: 11 },               MissingPolicy.USE_UNDEFINED, mapValuesInspector3], { a: [11, "a", 0], b: [22, "b", 1] } ],
        [[ { b: 22, a: 11, d: undefined }, MissingPolicy.USE_UNDEFINED, mapValuesInspector3], { a: [11, "a", 0], b: [22, "b", 1] } ],
        //
        [[ {},               [],           MissingPolicy.USE_UNDEFINED, curry3to0(noop)],     {} ],
        [[ {},               [],           MissingPolicy.USE_UNDEFINED, mapValuesInspector3], {} ],
        [[ { a: 11, b: 22 }, [],           MissingPolicy.USE_UNDEFINED, mapValuesInspector3], {} ],
        [[ { a: 11, b: 22 }, ["a", "b"],   MissingPolicy.USE_UNDEFINED, mapValuesInspector3], { a: [11, "a", 0], b: [22, "b", 1] } ],
        [[ { a: 11, b: 22 }, ["b", "a"],   MissingPolicy.USE_UNDEFINED, mapValuesInspector3], { b: [22, "b", 0], a: [11, "a", 1] } ],
        [[ { a: 11, b: 22 }, ["b"],        MissingPolicy.USE_UNDEFINED, mapValuesInspector3], { b: [22, "b", 0] } ],
        //
        [[ {},               ["c"],        MissingPolicy.USE_UNDEFINED, curry3to0(noop)],     { c: undefined } ],
        [[ {},               ["c"],        MissingPolicy.USE_UNDEFINED, mapValuesInspector3], { c: [undefined, "c", 0] } ],
        [[ { a: 11, b: 22 }, ["c"],        MissingPolicy.USE_UNDEFINED, mapValuesInspector3], { c: [undefined, "c", 0] } ],
        [[ { a: 11, b: 22 }, ["a", "c"],   MissingPolicy.USE_UNDEFINED, mapValuesInspector3], { a: [11,        "a", 0], c: [undefined, "c", 1] } ],
        [[ { a: 11, b: 22 }, ["c", "b"],   MissingPolicy.USE_UNDEFINED, mapValuesInspector3], { c: [undefined, "c", 0], b: [22, "b", 1] } ],
        //
        [[ [],                             MissingPolicy.USE_UNDEFINED,  noop],                [] ],
        [[ [],                             MissingPolicy.USE_UNDEFINED,  mapValuesInspector3], [] ],
        [[ [11, 22],                       MissingPolicy.USE_UNDEFINED,  mapValuesInspector3], [[11, 0, 0], [22, 1, 1]] ],
        //
        [[ {},                             MissingPolicy.SKIP, curry3to0(noop)],     {} ],
        [[ {},                             MissingPolicy.SKIP, mapValuesInspector3], {} ],
        [[ { a: 11, b: 22 },               MissingPolicy.SKIP, mapValuesInspector3], { a: [11, "a", 0], b: [22, "b", 1] } ],
        [[ { b: 22, a: 11 },               MissingPolicy.SKIP, mapValuesInspector3], { a: [11, "a", 0], b: [22, "b", 1] } ],
        [[ { b: 22, a: 11, d: undefined }, MissingPolicy.SKIP, mapValuesInspector3], { a: [11, "a", 0], b: [22, "b", 1] } ],
        //
        [[ {},               [],           MissingPolicy.SKIP, curry3to0(noop)],     {} ],
        [[ {},               [],           MissingPolicy.SKIP, mapValuesInspector3], {} ],
        [[ { a: 11, b: 22 }, [],           MissingPolicy.SKIP, mapValuesInspector3], {} ],
        [[ { a: 11, b: 22 }, ["a", "b"],   MissingPolicy.SKIP, mapValuesInspector3], { a: [11, "a", 0], b: [22, "b", 1] } ],
        [[ { a: 11, b: 22 }, ["b", "a"],   MissingPolicy.SKIP, mapValuesInspector3], { a: [11, "a", 1], b: [22, "b", 0] } ],
        [[ { a: 11, b: 22 }, ["b"],        MissingPolicy.SKIP, mapValuesInspector3], {                  b: [22, "b", 0] } ],
        //
        [[ {},               ["c"],        MissingPolicy.SKIP, curry3to0(noop)],     { } ],
        [[ {},               ["c"],        MissingPolicy.SKIP, mapValuesInspector3], { } ],
        [[ { a: 11, b: 22 }, ["c"],        MissingPolicy.SKIP, mapValuesInspector3], { } ],
        [[ { a: 11, b: 22 }, ["a", "c"],   MissingPolicy.SKIP, mapValuesInspector3], { a: [11, "a", 0] } ],
        [[ { a: 11, b: 22 }, ["c", "b"],   MissingPolicy.SKIP, mapValuesInspector3], { b: [22, "b", 0] } ],
        //
        [[ [],                             MissingPolicy.SKIP,  noop],                [] ],
        [[ [],                             MissingPolicy.SKIP,  mapValuesInspector3], [] ],
        [[ [11, 22],                       MissingPolicy.SKIP,  mapValuesInspector3], [[11, 0, 0], [22, 1, 1]] ],
        //
    ], function(args is array) { return (size(args) <= 2) ? mapValues3(args[0], args[1]) : ((size(args) <= 3) ? mapValues3(args[0], args[1], args[2]) :  mapValues3(args[0], args[1], args[2], args[3])); });
}
export function runMapValuesTests(context is Context, verbose is boolean) returns map {
    return runTests(context, "mapValues3", verbose, [
        [[ {},                             curry2to0(noop)],     {} ],
        [[ {},                             mapValuesInspector2], {} ],
        [[ { a: 11, b: 22 },               mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        [[ { b: 22, a: 11 },               mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        [[ { b: 22, a: 11, d: undefined }, mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        //
        [[ {},               [],           curry2to0(noop)],     {} ],
        [[ {},               [],           mapValuesInspector2], {} ],
        [[ { a: 11, b: 22 }, [],           mapValuesInspector2], {} ],
        [[ { a: 11, b: 22 }, ["a", "b"],   mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        [[ { a: 11, b: 22 }, ["b", "a"],   mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        [[ { a: 11, b: 22 }, ["b"],        mapValuesInspector2], {               b: [22, "b"] } ],
        //
        [[ {},               ["c"],        curry2to0(noop)],     { c: undefined } ],
        [[ {},               ["c"],        mapValuesInspector2], { c: [undefined, "c"] } ],
        [[ { a: 11, b: 22 }, ["c"],        mapValuesInspector2], { c: [undefined, "c"] } ],
        [[ { a: 11, b: 22 }, ["a", "c"],   mapValuesInspector2], { a: [11,        "a"], c: [undefined, "c"] } ],
        [[ { a: 11, b: 22 }, ["c", "b"],   mapValuesInspector2], { c: [undefined, "c"], b: [22, "b"] } ],
        //
        [[ [],                             noop],               [] ],
        [[ [],                             mapValuesInspector2], [] ],
        [[ [11, 22],                       mapValuesInspector2], [[11, 0], [22, 1]] ],
        //
        [[ {},                             MissingPolicy.USE_UNDEFINED, curry2to0(noop)],     {} ],
        [[ {},                             MissingPolicy.USE_UNDEFINED, mapValuesInspector2], {} ],
        [[ { a: 11, b: 22 },               MissingPolicy.USE_UNDEFINED, mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        [[ { b: 22, a: 11 },               MissingPolicy.USE_UNDEFINED, mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        [[ { b: 22, a: 11, d: undefined }, MissingPolicy.USE_UNDEFINED, mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        //
        [[ {},               [],           MissingPolicy.USE_UNDEFINED, curry2to0(noop)],     {} ],
        [[ {},               [],           MissingPolicy.USE_UNDEFINED, mapValuesInspector2], {} ],
        [[ { a: 11, b: 22 }, [],           MissingPolicy.USE_UNDEFINED, mapValuesInspector2], {} ],
        [[ { a: 11, b: 22 }, ["a", "b"],   MissingPolicy.USE_UNDEFINED, mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        [[ { a: 11, b: 22 }, ["b", "a"],   MissingPolicy.USE_UNDEFINED, mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        [[ { a: 11, b: 22 }, ["b"],        MissingPolicy.USE_UNDEFINED, mapValuesInspector2], {               b: [22, "b"] } ],
        //
        [[ {},               ["c"],        MissingPolicy.USE_UNDEFINED, curry2to0(noop)],     { c: undefined } ],
        [[ {},               ["c"],        MissingPolicy.USE_UNDEFINED, mapValuesInspector2], { c: [undefined, "c"] } ],
        [[ { a: 11, b: 22 }, ["c"],        MissingPolicy.USE_UNDEFINED, mapValuesInspector2], { c: [undefined, "c"] } ],
        [[ { a: 11, b: 22 }, ["a", "c"],   MissingPolicy.USE_UNDEFINED, mapValuesInspector2], { a: [11,        "a"], c: [undefined, "c"] } ],
        [[ { a: 11, b: 22 }, ["c", "b"],   MissingPolicy.USE_UNDEFINED, mapValuesInspector2], { c: [undefined, "c"], b: [22, "b"] } ],
        //
        [[ [],                             MissingPolicy.USE_UNDEFINED,  noop],                [] ],
        [[ [],                             MissingPolicy.USE_UNDEFINED,  mapValuesInspector2], [] ],
        [[ [11, 22],                       MissingPolicy.USE_UNDEFINED,  mapValuesInspector2], [[11, 0], [22, 1]] ],
        //
        [[ {},                             MissingPolicy.SKIP, curry2to0(noop)],     {} ],
        [[ {},                             MissingPolicy.SKIP, mapValuesInspector2], {} ],
        [[ { a: 11, b: 22 },               MissingPolicy.SKIP, mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        [[ { b: 22, a: 11 },               MissingPolicy.SKIP, mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        [[ { b: 22, a: 11, d: undefined }, MissingPolicy.SKIP, mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        //
        [[ {},               [],           MissingPolicy.SKIP, curry2to0(noop)],     {} ],
        [[ {},               [],           MissingPolicy.SKIP, mapValuesInspector2], {} ],
        [[ { a: 11, b: 22 }, [],           MissingPolicy.SKIP, mapValuesInspector2], {} ],
        [[ { a: 11, b: 22 }, ["a", "b"],   MissingPolicy.SKIP, mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        [[ { a: 11, b: 22 }, ["b", "a"],   MissingPolicy.SKIP, mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        [[ { a: 11, b: 22 }, ["b"],        MissingPolicy.SKIP, mapValuesInspector2], {               b: [22, "b"] } ],
        //
        [[ {},               ["c"],        MissingPolicy.SKIP, curry2to0(noop)],     { } ],
        [[ {},               ["c"],        MissingPolicy.SKIP, mapValuesInspector2], { } ],
        [[ { a: 11, b: 22 }, ["c"],        MissingPolicy.SKIP, mapValuesInspector2], { } ],
        [[ { a: 11, b: 22 }, ["a", "c"],   MissingPolicy.SKIP, mapValuesInspector2], { a: [11, "a"] } ],
        [[ { a: 11, b: 22 }, ["c", "b"],   MissingPolicy.SKIP, mapValuesInspector2], { b: [22, "b"] } ],
        //
        [[ [],                             MissingPolicy.SKIP,  noop],                [] ],
        [[ [],                             MissingPolicy.SKIP,  mapValuesInspector2], [] ],
        [[ [11, 22],                       MissingPolicy.SKIP,  mapValuesInspector2], [[11, 0], [22, 1]] ],

    ], function(args is array) { return (size(args) <= 2) ? mapValues(args[0], args[1]) : ((size(args) <= 3) ? mapValues(args[0], args[1], args[2]) :  mapValues(args[0], args[1], args[2], args[3])); });
}

export function runValuesAtTests(context is Context, verbose is boolean) returns map {
    return runTests(context, "valuesAt", verbose, [
        //
        [[ {},               []              ], [] ],
        [[ { a: 11, b: 22 }, []              ], [] ],
        [[ { a: 11, b: 22 }, ["a", "b"]      ], [11, 22] ],
        [[ { a: 11, b: 22 }, ["b", "a"]      ], [22, 11] ],
        [[ { a: 11, b: 22 }, ["b"]           ], [22] ],
        //
        [[ {},               ["c"]           ], [undefined]     ],
        [[ { a: 11, b: 22 }, ["c"]           ], [undefined]     ],
        [[ { a: 11, b: 22 }, ["a", "c"]      ], [11, undefined] ],
        [[ { a: 11, b: 22 }, ["c", "b"]      ], [undefined, 22] ],
        //
        [[ [],               []              ], [] ],
        [[ [11, 22],         []              ], [] ],
        [[ [11, 22],         [0, 1]          ], [11, 22] ],
        [[ [11, 22],         [1, 0]          ], [22, 11] ],
        [[ [11, 22],         [1]             ], [22] ],
        [[ [11, 22],         [0]             ], [11] ],
        [[ [11, 22],         [-1]            ], [undefined] ],
        [[ [11, 22],         [-1, 0]         ], [undefined, 11] ],
        [[ [11, 22],         [1, -1]         ], [22, undefined] ],
        // Missing keys will get a value of undefined
        [[ {},               [],           MissingPolicy.USE_UNDEFINED],    [] ],
        [[ { a: 11, b: 22 }, [],           MissingPolicy.USE_UNDEFINED],    [] ],
        [[ { a: 11, b: 22 }, ["a", "b"],   MissingPolicy.USE_UNDEFINED],    [11, 22] ],
        [[ { a: 11, b: 22 }, ["b", "a"],   MissingPolicy.USE_UNDEFINED],    [22, 11] ],
        [[ { a: 11, b: 22 }, ["b"],        MissingPolicy.USE_UNDEFINED],    [22] ],
        //
        [[ {},               ["c"],        MissingPolicy.USE_UNDEFINED], [undefined]     ],
        [[ { a: 11, b: 22 }, ["c"],        MissingPolicy.USE_UNDEFINED], [undefined]     ],
        [[ { a: 11, b: 22 }, ["a", "c"],   MissingPolicy.USE_UNDEFINED], [11, undefined] ],
        [[ { a: 11, b: 22 }, ["c", "b"],   MissingPolicy.USE_UNDEFINED], [undefined, 22] ],
        //
        [[ [],               [],           MissingPolicy.USE_UNDEFINED], [] ],
        [[ [11, 22],         [],           MissingPolicy.USE_UNDEFINED], [] ],
        [[ [11, 22],         [0, 1],       MissingPolicy.USE_UNDEFINED], [11, 22] ],
        [[ [11, 22],         [1, 0],       MissingPolicy.USE_UNDEFINED], [22, 11] ],
        [[ [11, 22],         [1],          MissingPolicy.USE_UNDEFINED], [22] ],
        [[ [11, 22],         [0],          MissingPolicy.USE_UNDEFINED], [11] ],
        [[ [11, 22],         [-1],         MissingPolicy.USE_UNDEFINED], [undefined] ],
        [[ [11, 22],         [-1, 0],      MissingPolicy.USE_UNDEFINED], [undefined, 11] ],
        [[ [11, 22],         [1, -1],      MissingPolicy.USE_UNDEFINED], [22, undefined] ],
        // Missing keys will not appear in the result
        //
        [[ {},               [],           MissingPolicy.SKIP],    [] ],
        [[ { a: 11, b: 22 }, [],           MissingPolicy.SKIP],    [] ],
        [[ { a: 11, b: 22 }, ["a", "b"],   MissingPolicy.SKIP],    [11, 22] ],
        [[ { a: 11, b: 22 }, ["b", "a"],   MissingPolicy.SKIP],    [22, 11] ],
        [[ { a: 11, b: 22 }, ["b"],        MissingPolicy.SKIP],    [22] ],
        //
        [[ {},               ["c"],        MissingPolicy.SKIP], []     ],
        [[ { a: 11, b: 22 }, ["c"],        MissingPolicy.SKIP], []     ],
        [[ { a: 11, b: 22 }, ["a", "c"],   MissingPolicy.SKIP], [11] ],
        [[ { a: 11, b: 22 }, ["c", "b"],   MissingPolicy.SKIP], [22] ],
        //
        [[ [],               [],           MissingPolicy.SKIP], [] ],
        [[ [11, 22],         [],           MissingPolicy.SKIP], [] ],
        [[ [11, 22],         [0, 1],       MissingPolicy.SKIP], [11, 22] ],
        [[ [11, 22],         [1, 0],       MissingPolicy.SKIP], [22, 11] ],
        [[ [11, 22],         [1],          MissingPolicy.SKIP], [22] ],
        [[ [11, 22],         [0],          MissingPolicy.SKIP], [11] ],
        [[ [11, 22],         [-1],         MissingPolicy.SKIP], [] ],
        [[ [11, 22],         [-1, 0],      MissingPolicy.SKIP], [11] ],
        [[ [11, 22],         [1, -1],      MissingPolicy.SKIP], [22] ],
    ], function(args is array) { return size(args) <= 2 ? valuesAt(args[0], args[1]) : valuesAt(args[0], args[1], args[2]); });
}
