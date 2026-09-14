FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
//
import(path : "f6ab954150609e1e2e014a1e", version : "939862c0f6583971525024d5"); // runTests
import(path : "08b6ba15b8255611bafe7520", version : "25c448c89a44ac841877ddc3"); // miscUtils (For noop, curryX)
import(path : "607f97fc690581579d1d4a08", version : "af42bc272fc425c5435bf347"); // for hasKey/hasPresentKey
//
import(path : "a92a3694a1d67e78c541bed5", version : "8091b95c1e95e0c9560f4c40"); // testClxnGetset: we run their tests
import(path : "b87a9ac25313676049cf04cb", version : "bfc5deb303dfdab8392e5eba"); // testClxnReshape: we run their tests
import(path : "54590bc1c9cee0141b968fbb", version : "656aceb1a703e20bf00c28bd"); // clxnUtils: testing this

// == [Run tests] ==

const SuiteTitle = "Collection Utils";

/**
 * Part feature: extrudes `text` sized and aligned within a bounding plate at each selected plane.
 * Delegates all geometry to @see `textChip`.
 * @param definition {{
 *   @field json {string} : stringified JSON to produce
 * }}
 */
annotation { "Feature Type Name": 'Run ' ~ SuiteTitle ~ ' Tests', "Feature Type Description": "Internal Tests" }
export const runClxnTestsFS = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Verbose", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE, "Default": false }
  definition.verbose is boolean;
}
{
  try {
    const verbose = ifNil(definition.verbose, false);
    //
    runUndotMapTests(context, verbose);
    if (verbose) { runUndotMapThrowsTests(context, verbose); }
    runDotMapTests(context, verbose);
    runRoundTripTests(context, verbose);
    //
    runBuildNestedChoicesTests(context, verbose);
    //
    runHasKeyMapTests(context,        verbose);
    runHasPresentKeyMapTests(context, verbose);
    runHasKeyArrTests(context,        verbose);
    runHasPresentKeyArrTests(context, verbose);
    runArrayIncludesTests(context,    verbose);
    //
    runPickTests(context,        verbose);
    runPickDefinedTests(context, verbose);
    runArrFirstTests(context,    verbose);
    runArrLastTests(context,     verbose);
    //
    runMapValuesTests(context,  verbose);
    runMapValues3Tests(context, verbose);
    runValuesAtTests(context,   verbose);
    runForEachBreakTests(context,        verbose);
    runForEachKeylistBreakTests(context, verbose);
    runObjectifyTests(context, verbose);
    runRebagTests(context, verbose);
    //
    runBoxarrPushTests(context,    verbose);
    runBoxarrUnshiftTests(context, verbose);
    //
    runPathForKeyTests(context, verbose);
    runDeepMergeTests(context, verbose);
    runGetAtPathTests(context, verbose);
    runGetAtArrTests(context, verbose);
    runSetAtTests(context, verbose);
    if (verbose) { runSetAtThrowsTests(context, verbose); }
    runUpdateTests(context, verbose);
    runUpdateWithTests(context, verbose);
    runAssignWithTests(context, verbose);
    runMergeWithTests(context, verbose);
    //
    runCountByTests(context, verbose);
    runFindTests(context, verbose);
    runFindLastTests(context, verbose);
    runFlatMapTests(context, verbose);
    runFlatMapDeepTests(context, verbose);
    runFlatMapDepthTests(context, verbose);
    runForEachRightTests(context, verbose);
    runGroupByTests(context, verbose);
    runPartitionTests(context, verbose);
    runReduceRightTests(context, verbose);
    runRejectTests(context, verbose);
    //
    if (! verbose) { debug(context, starbanner('** ' ~ SuiteTitle ~ ' Tests ran successfully **')); }
  } catch (err) {
    debug(context, starbanner('** Error in ' ~ SuiteTitle ~ ' Tests: ' ~ err ~ ' **'));
  }
});

const mapValuesInspector2 = function(val, key)      { return [val, key]; };
const mapValuesInspector3 = function(val, key, seq) { return [val, key, seq]; };

export function runMapValues3Tests(context is Context, verbose is boolean) returns map {
    return runTests(context, "mapValues3", verbose, [
        [[ {},                             noop3],    {} ],
        [[ {},                             mapValuesInspector3], {} ],
        [[ { a: 11, b: 22 },               mapValuesInspector3], { a: [11, "a", 0], b: [22, "b", 1] } ],
        [[ { b: 22, a: 11 },               mapValuesInspector3], { a: [11, "a", 0], b: [22, "b", 1] } ],
        [[ { b: 22, a: 11, d: undefined }, mapValuesInspector3], { a: [11, "a", 0], b: [22, "b", 1] } ],
        //
        [[ {},               [],           noop3],    {} ],
        [[ {},               [],           mapValuesInspector3], {} ],
        [[ { a: 11, b: 22 }, [],           mapValuesInspector3], {} ],
        [[ { a: 11, b: 22 }, ["a", "b"],   mapValuesInspector3], { a: [11, "a", 0], b: [22, "b", 1] } ],
        [[ { a: 11, b: 22 }, ["b", "a"],   mapValuesInspector3], { a: [11, "a", 1], b: [22, "b", 0] } ],
        [[ { a: 11, b: 22 }, ["b"],        mapValuesInspector3], {                  b: [22, "b", 0] } ],
        //
        [[ {},               ["c"],        noop3],     { c: undefined } ],
        [[ {},               ["c"],        mapValuesInspector3], { c: [undefined, "c", 0] } ],
        [[ { a: 11, b: 22 }, ["c"],        mapValuesInspector3], { c: [undefined, "c", 0] } ],
        [[ { a: 11, b: 22 }, ["a", "c"],   mapValuesInspector3], { a: [11,        "a", 0], c: [undefined, "c", 1] } ],
        [[ { a: 11, b: 22 }, ["c", "b"],   mapValuesInspector3], { c: [undefined, "c", 0], b: [22, "b", 1] } ],
        //
        [[ [],               noop],                [] ],
        [[ [],               mapValuesInspector3], [] ],
        [[ [11, 22],         mapValuesInspector3], [[11, 0, 0], [22, 1, 1]] ],
        //
        [[ {},                             MissingPolicy.USE_UNDEFINED, noop3],     {} ],
        [[ {},                             MissingPolicy.USE_UNDEFINED, mapValuesInspector3], {} ],
        [[ { a: 11, b: 22 },               MissingPolicy.USE_UNDEFINED, mapValuesInspector3], { a: [11, "a", 0], b: [22, "b", 1] } ],
        [[ { b: 22, a: 11 },               MissingPolicy.USE_UNDEFINED, mapValuesInspector3], { a: [11, "a", 0], b: [22, "b", 1] } ],
        [[ { b: 22, a: 11, d: undefined }, MissingPolicy.USE_UNDEFINED, mapValuesInspector3], { a: [11, "a", 0], b: [22, "b", 1] } ],
        //
        [[ {},               [],           MissingPolicy.USE_UNDEFINED, noop3],     {} ],
        [[ {},               [],           MissingPolicy.USE_UNDEFINED, mapValuesInspector3], {} ],
        [[ { a: 11, b: 22 }, [],           MissingPolicy.USE_UNDEFINED, mapValuesInspector3], {} ],
        [[ { a: 11, b: 22 }, ["a", "b"],   MissingPolicy.USE_UNDEFINED, mapValuesInspector3], { a: [11, "a", 0], b: [22, "b", 1] } ],
        [[ { a: 11, b: 22 }, ["b", "a"],   MissingPolicy.USE_UNDEFINED, mapValuesInspector3], { b: [22, "b", 0], a: [11, "a", 1] } ],
        [[ { a: 11, b: 22 }, ["b"],        MissingPolicy.USE_UNDEFINED, mapValuesInspector3], { b: [22, "b", 0] } ],
        //
        [[ {},               ["c"],        MissingPolicy.USE_UNDEFINED, noop3],     { c: undefined } ],
        [[ {},               ["c"],        MissingPolicy.USE_UNDEFINED, mapValuesInspector3], { c: [undefined, "c", 0] } ],
        [[ { a: 11, b: 22 }, ["c"],        MissingPolicy.USE_UNDEFINED, mapValuesInspector3], { c: [undefined, "c", 0] } ],
        [[ { a: 11, b: 22 }, ["a", "c"],   MissingPolicy.USE_UNDEFINED, mapValuesInspector3], { a: [11,        "a", 0], c: [undefined, "c", 1] } ],
        [[ { a: 11, b: 22 }, ["c", "b"],   MissingPolicy.USE_UNDEFINED, mapValuesInspector3], { c: [undefined, "c", 0], b: [22, "b", 1] } ],
        //
        [[ [],                             MissingPolicy.USE_UNDEFINED,  noop],                [] ],
        [[ [],                             MissingPolicy.USE_UNDEFINED,  mapValuesInspector3], [] ],
        [[ [11, 22],                       MissingPolicy.USE_UNDEFINED,  mapValuesInspector3], [[11, 0, 0], [22, 1, 1]] ],
        //
        [[ {},                             MissingPolicy.SKIP, noop3],     {} ],
        [[ {},                             MissingPolicy.SKIP, mapValuesInspector3], {} ],
        [[ { a: 11, b: 22 },               MissingPolicy.SKIP, mapValuesInspector3], { a: [11, "a", 0], b: [22, "b", 1] } ],
        [[ { b: 22, a: 11 },               MissingPolicy.SKIP, mapValuesInspector3], { a: [11, "a", 0], b: [22, "b", 1] } ],
        [[ { b: 22, a: 11, d: undefined }, MissingPolicy.SKIP, mapValuesInspector3], { a: [11, "a", 0], b: [22, "b", 1] } ],
        //
        [[ {},               [],           MissingPolicy.SKIP, noop3],     {} ],
        [[ {},               [],           MissingPolicy.SKIP, mapValuesInspector3], {} ],
        [[ { a: 11, b: 22 }, [],           MissingPolicy.SKIP, mapValuesInspector3], {} ],
        [[ { a: 11, b: 22 }, ["a", "b"],   MissingPolicy.SKIP, mapValuesInspector3], { a: [11, "a", 0], b: [22, "b", 1] } ],
        [[ { a: 11, b: 22 }, ["b", "a"],   MissingPolicy.SKIP, mapValuesInspector3], { a: [11, "a", 1], b: [22, "b", 0] } ],
        [[ { a: 11, b: 22 }, ["b"],        MissingPolicy.SKIP, mapValuesInspector3], {                  b: [22, "b", 0] } ],
        //
        [[ {},               ["c"],        MissingPolicy.SKIP, noop3],     { } ],
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
    return runTests(context, "mapValues", verbose, [
        [[ { fred: 40, pebbles: 1 },       function(age, _seq) { return age * 2; } ], { fred: 80, pebbles: 2 }, 'doc example: doubles each value, keys unchanged'],
        [[ [4, 8],                         function(nn, _seq) { return nn * nn; } ], [16, 64],                  'doc example: squares each element'],
        //
        [[ {},                             noop2],     {} ],
        [[ {},                             mapValuesInspector2], {} ],
        [[ { a: 11, b: 22 },               mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        [[ { b: 22, a: 11 },               mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        [[ { b: 22, a: 11, d: undefined }, mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        //
        [[ {},               [],           noop2],     {} ],
        [[ {},               [],           mapValuesInspector2], {} ],
        [[ { a: 11, b: 22 }, [],           mapValuesInspector2], {} ],
        [[ { a: 11, b: 22 }, ["a", "b"],   mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        [[ { a: 11, b: 22 }, ["b", "a"],   mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        [[ { a: 11, b: 22 }, ["b"],        mapValuesInspector2], {               b: [22, "b"] } ],
        //
        [[ {},               ["c"],        noop2],     { c: undefined } ],
        [[ {},               ["c"],        mapValuesInspector2], { c: [undefined, "c"] } ],
        [[ { a: 11, b: 22 }, ["c"],        mapValuesInspector2], { c: [undefined, "c"] } ],
        [[ { a: 11, b: 22 }, ["a", "c"],   mapValuesInspector2], { a: [11,        "a"], c: [undefined, "c"] } ],
        [[ { a: 11, b: 22 }, ["c", "b"],   mapValuesInspector2], { c: [undefined, "c"], b: [22, "b"] } ],
        //
        [[ [],                             noop],               [] ],
        [[ [],                             mapValuesInspector2], [] ],
        [[ [11, 22],                       mapValuesInspector2], [[11, 0], [22, 1]] ],
        //
        [[ {},                             MissingPolicy.USE_UNDEFINED, noop2],     {} ],
        [[ {},                             MissingPolicy.USE_UNDEFINED, mapValuesInspector2], {} ],
        [[ { a: 11, b: 22 },               MissingPolicy.USE_UNDEFINED, mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        [[ { b: 22, a: 11 },               MissingPolicy.USE_UNDEFINED, mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        [[ { b: 22, a: 11, d: undefined }, MissingPolicy.USE_UNDEFINED, mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        //
        [[ {},               [],           MissingPolicy.USE_UNDEFINED, noop2],     {} ],
        [[ {},               [],           MissingPolicy.USE_UNDEFINED, mapValuesInspector2], {} ],
        [[ { a: 11, b: 22 }, [],           MissingPolicy.USE_UNDEFINED, mapValuesInspector2], {} ],
        [[ { a: 11, b: 22 }, ["a", "b"],   MissingPolicy.USE_UNDEFINED, mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        [[ { a: 11, b: 22 }, ["b", "a"],   MissingPolicy.USE_UNDEFINED, mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        [[ { a: 11, b: 22 }, ["b"],        MissingPolicy.USE_UNDEFINED, mapValuesInspector2], {               b: [22, "b"] } ],
        //
        [[ {},               ["c"],        MissingPolicy.USE_UNDEFINED, noop2],     { c: undefined } ],
        [[ {},               ["c"],        MissingPolicy.USE_UNDEFINED, mapValuesInspector2], { c: [undefined, "c"] } ],
        [[ { a: 11, b: 22 }, ["c"],        MissingPolicy.USE_UNDEFINED, mapValuesInspector2], { c: [undefined, "c"] } ],
        [[ { a: 11, b: 22 }, ["a", "c"],   MissingPolicy.USE_UNDEFINED, mapValuesInspector2], { a: [11,        "a"], c: [undefined, "c"] } ],
        [[ { a: 11, b: 22 }, ["c", "b"],   MissingPolicy.USE_UNDEFINED, mapValuesInspector2], { c: [undefined, "c"], b: [22, "b"] } ],
        //
        [[ [],                             MissingPolicy.USE_UNDEFINED,  noop],                [] ],
        [[ [],                             MissingPolicy.USE_UNDEFINED,  mapValuesInspector2], [] ],
        [[ [11, 22],                       MissingPolicy.USE_UNDEFINED,  mapValuesInspector2], [[11, 0], [22, 1]] ],
        //
        [[ {},                             MissingPolicy.SKIP, noop2],     {} ],
        [[ {},                             MissingPolicy.SKIP, mapValuesInspector2], {} ],
        [[ { a: 11, b: 22 },               MissingPolicy.SKIP, mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        [[ { b: 22, a: 11 },               MissingPolicy.SKIP, mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        [[ { b: 22, a: 11, d: undefined }, MissingPolicy.SKIP, mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        //
        [[ {},               [],           MissingPolicy.SKIP, noop2],     {} ],
        [[ {},               [],           MissingPolicy.SKIP, mapValuesInspector2], {} ],
        [[ { a: 11, b: 22 }, [],           MissingPolicy.SKIP, mapValuesInspector2], {} ],
        [[ { a: 11, b: 22 }, ["a", "b"],   MissingPolicy.SKIP, mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        [[ { a: 11, b: 22 }, ["b", "a"],   MissingPolicy.SKIP, mapValuesInspector2], { a: [11, "a"], b: [22, "b"] } ],
        [[ { a: 11, b: 22 }, ["b"],        MissingPolicy.SKIP, mapValuesInspector2], {               b: [22, "b"] } ],
        //
        [[ {},               ["c"],        MissingPolicy.SKIP, noop2],     { } ],
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

const rebagArrInspector = function(val, seq is number)      { if (val == "skip") return undefined; return ["" ~ seq ~ seq, [val, seq]]; };
const rebagMapInspector = function(val, key is string)      { if (val == "skip") return undefined; return ["" ~ key ~ key, [val, key]]; };
export function runRebagTests(context is Context, verbose is boolean) returns map {
    return runTests(context, "rebag", verbose, [
        [[ {},                             noop2],   {} ],
        [[ { a: 11, b: 22 },               noop2],   {} ],
        [[ {},                             rebagMapInspector], {} ],
        [[ { a: 11, b: 22 },               rebagMapInspector], { aa: [11, "a"], bb: [22, "b"] } ],
        [[ { b: 22, a: 11 },               rebagMapInspector], { aa: [11, "a"], bb: [22, "b"] } ],
        [[ { b: 22, a: 11, d: undefined }, rebagMapInspector], { aa: [11, "a"], bb: [22, "b"] } ],
        [[ { b: 22, a: 11, s: "skip" },    rebagMapInspector], { aa: [11, "a"], bb: [22, "b"] } ],
        //
        [[ [],                             noop2],   {} ],
        [[ [11, 22],                       noop2],   {} ],
        [[ [],                             rebagArrInspector], {} ],
        [[ ["skip"],                       rebagArrInspector], {} ],
        [[ [11, 22],                       rebagArrInspector], { '00': [11, 0], '11': [22, 1] } ],
        [[ [11, 22, "skip"],               rebagArrInspector], { '00': [11, 0], '11': [22, 1] } ],
        //
        //
    ], function(args is array) { return rebag(args[0], args[1]);  });
}

export const ObjectifyCases = [
  [[ ["a", "b", "c"], (val, seq) => seq ], { a: 0, b: 1, c: 2 }, 'keys the result by the array values themselves'],
  [[ [],              (val, seq) => seq ], {} ],
  [[ ["x", "x"],      (val, seq) => seq ], { x: 1 },             'a repeated value collides on the same key; the later one wins'],
  [[ ["a", "b"],      (val, seq) => val ~ seq ], { a: "a0", b: "b1" }, 'func may combine val and seq into the result'],
];
export function runObjectifyTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "objectify", verbose, ObjectifyCases, function(args is array) { return objectify(args[0], args[1]); });
}

// == [hasKey / hasPresentKey / arrayIncludes] ==

export const HasKeyMapCases = [
  [[ {},               "a" ], false ],
  [[ { a: 1 },          "a" ], true ],
  [[ { a: 1 },          "b" ], false ],
  [[ { a: undefined },  "a" ], false, 'FeatureScript elides an undefined value on write, so there is no key here to find'],
  //
  [[ { a: 1 }, "a", MissingPolicy.SKIP],          true,  'missingPolicy never changes the answer for a map'],
  [[ { a: 1 }, "a", MissingPolicy.USE_UNDEFINED], true],
  [[ {},       "a", MissingPolicy.SKIP],          false],
];
export const HasPresentKeyMapCases = [
  [[ {},      "a" ], false ],
  [[ { a: 1}, "a" ], true ],
];
export const HasKeyArrCases = [
  [[ [1, 2, 3], 0 ],  true ],
  [[ [1, 2, 3], 2 ],  true ],
  [[ [1, 2, 3], 3 ],  false ],
  [[ [1, 2, 3], -1 ], false, 'no negative-index support here, unlike getAt'],
  [[ [1, undefined, 3], 1 ], true, 'present but undefined still counts as having the key'],
  //
  [[ [1, undefined, 3], 1, MissingPolicy.USE_UNDEFINED ], true ],
  [[ [1, undefined, 3], 1, MissingPolicy.SKIP ],          false, 'SKIP defers to hasPresentKey: present-but-undefined does not count'],
  [[ [1, 2, 3],          5, MissingPolicy.SKIP ],          false ],
];
export const HasPresentKeyArrCases = [
  [[ [1, 2, 3], 1 ],          true ],
  [[ [1, undefined, 3], 1 ], false ],
  [[ [1, 2, 3], 5 ],          false ],
  [[ [1, 2, 3], -1 ],         false, 'no negative-index support here, unlike getAt'],
  [[ [],         0 ],         false, 'empty array: nothing is in bounds'],
];
export const ArrayIncludesCases = [
  [[ [1, 2, 3], 2 ], true ],
  [[ [1, 2, 3], 5 ], false ],
  [[ [], 1 ],        false ],
  [[ [undefined, 1], undefined ], true ],
  [[ [{ a: 1 }], { a: 1 } ],      true, '`==` is deep structural equality, so a matching map counts'],
  [[ [[1, 2]], [1, 2] ],          true, 'deep structural equality also applies to nested arrays'],
  //
  [[ { a: 1, b: 2, c: 3 }, 2 ], true,  'map form searches values, not keys'],
  [[ { a: 1, b: 2, c: 3 }, "b" ], false ],
  [[ {}, 1 ],                     false ],
];

const runHasKeyTest        = ((args is array) => ((size(args) <= 2) ? hasKey(args[0], args[1]) : hasKey(args[0], args[1], args[2])));
const runHasPresentKeyTest = ((args is array) => hasPresentKey(args[0], args[1]));
const runArrayIncludesTest = ((args is array) => arrayIncludes(args[0], args[1]));

export function runHasKeyMapTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "hasKey (map)", verbose, HasKeyMapCases, runHasKeyTest);
}
export function runHasPresentKeyMapTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "hasPresentKey (map)", verbose, HasPresentKeyMapCases, runHasPresentKeyTest);
}
export function runHasKeyArrTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "hasKey (array)", verbose, HasKeyArrCases, runHasKeyTest);
}
export function runHasPresentKeyArrTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "hasPresentKey (array)", verbose, HasPresentKeyArrCases, runHasPresentKeyTest);
}
export function runArrayIncludesTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "arrayIncludes", verbose, ArrayIncludesCases, runArrayIncludesTest);
}

// == [pick / pickDefined / arrLast] ==

export const PickCases = [
  [[ { a: 1, b: 2, c: 3 }, ["a", "c"] ],  { a: 1, c: 3 } ],
  [[ { a: 1 },             [] ],          {} ],
  [[ { a: 1 },             ["missing"] ], {}, 'a key absent from bag is elided from the result rather than set to undefined'],
  [[ {},                   ["a"] ],       {}, 'empty bag: every key is absent'],
  [[ { a: 1, b: 2 },       ["a", "missing"] ], { a: 1 }, 'present keys are kept, absent ones dropped, in the same call'],
  [[ { a: 1, b: 2 },       ["a", "a"] ],  { a: 1 },       'a keylist entry repeated twice just writes the same key twice'],
];
export const PickDefinedCases = [
  [[ { a: 1, b: undefined, c: 3 }, ["a", "b", "c"] ], { a: 1, c: 3 } ],
  [[ { a: 1 },                     ["missing"] ],     {} ],
  [[ {},                           [] ],               {}, 'empty bag and empty keylist'],
  [[ { a: 1, b: 2 },               [] ],               {}, 'empty keylist returns empty map regardless of bag contents'],
];

export const ArrFirstCases = [
  [[ []                   ],  undefined,              'empty input returns undefined'],
  [[ ["a"]                ],  "a"],
  [[ ["a", "b", "c", "d"] ],  "a"],
  [[ [undefined, "b"]     ],  undefined,              'undefined as a real first element is indistinguishable from the empty case'],
  [[ [[], "b"]            ],  []],
  [[ [[1, []], "b"]       ],  [1, []]],
  [[ [{}, 9]              ],  {}],
  [[ []               ],  undefined,  'empty array returns undefined'],
  [[ ["a"]            ],  "a"],
  [[ ["a", "b", "c"]  ],  "a"],
  [[ [undefined, "b"] ],  undefined,  'a genuine undefined first element is indistinguishable from empty'],
  [[ [[], {}, 9]      ],  []],
  [[ [[[1]], 2]       ],  [[1]]],
];
export const ArrLastCases = [
  [[ [1, 2, 3] ], 3 ],
  [[ [1] ],       1 ],
  [[ [] ],        undefined ],
  [[ []                   ],  undefined,              'empty input returns undefined'],
  [[ ["a"]                ],  "a"],
  [[ ["a", "b", "c", "d"] ],  "d"],
  [[ ["a", undefined]     ],  undefined,              'undefined as a real last element is indistinguishable from the empty case'],
  [[ ["a", []]            ],  []],
  [[ ["a", [1, []]]       ],  [1, []]],
  [[ [1, [], {}, 9]       ],  9],
];

export function runPickTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "pick", verbose, PickCases, function(args is array) { return pick(args[0], args[1]); });
}
export function runPickDefinedTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "pickDefined", verbose, PickDefinedCases, function(args is array) { return pickDefined(args[0], args[1]); });
}
export function runArrFirstTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "arrFirst", verbose, ArrFirstCases, function(args is array) { return arrFirst(args[0]); });
}
export function runArrLastTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "arrLast", verbose, ArrLastCases, function(args is array) { return arrLast(args[0]); });
}
// == [boxarrPush / boxarrUnshift] ==

export const BoxarrPushCases = [
  [[ [1, 2], 3 ], [3, [1, 2, 3]] ],
  [[ [],     1 ], [1, [1]] ],
  [[ [1],    undefined ], [undefined, [1, undefined]], 'pushing undefined still appends a slot to the array'],
  [[ [1],    [2, 3] ],     [[2, 3], [1, [2, 3]]],       'val is appended as a single element, not spread'],
];
export const BoxarrUnshiftCases = [
  [[ [1, 2], 3 ], [3, [3, 1, 2]] ],
  [[ [],     1 ], [1, [1]] ],
  [[ [1],    undefined ], [undefined, [undefined, 1]], 'unshifting undefined still prepends a slot to the array'],
  [[ [1],    [2, 3] ],     [[2, 3], [[2, 3], 1]],       'val is prepended as a single element, not spread'],
];

export function runBoxarrPushTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "boxarrPush", verbose, BoxarrPushCases, function(args is array) {
    const arrRef = new box(args[0]);
    const returned = boxarrPush(arrRef, args[1]);
    return [returned, arrRef[]];
  });
}
export function runBoxarrUnshiftTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "boxarrUnshift", verbose, BoxarrUnshiftCases, function(args is array) {
    const arrRef = new box(args[0]);
    const returned = boxarrUnshift(arrRef, args[1]);
    return [returned, arrRef[]];
  });
}

// == [forEach BREAK] ==
// mapValues/mapValues3/rebag above visit every entry and never return NextStepAction.BREAK, so
// none of those cases exercise the early-exit path at all. These do.

export const ForEachBreakCases = [
  [[ { a: 1, b: 2, c: 3 }, "b" ],
   [["a", 1], ["b", 2]],
   'map: stops right after the matching key, "c" is never visited'],
  [[ { a: 1, b: 2, c: 3 }, "zz" ],
   [["a", 1], ["b", 2], ["c", 3]],
   'map: never matches, so every entry is visited'],
  [[ {}, "zz" ],
   [],
   'map: empty bag is never visited'],
  [[ ["x", "y", "z"], "y" ],
   [["x", 0], ["y", 1]],
   'array: stops right after the matching value, "z" is never visited'],
  [[ ["x", "y", "z"], "zz" ],
   [["x", 0], ["y", 1], ["z", 2]],
   'array: never matches, so every entry is visited'],
  [[ [], "zz" ],
   [],
   'array: empty input is never visited'],
];
export function runForEachBreakTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "forEach BREAK", verbose, ForEachBreakCases, function(args is array) {
    const container = args[0];
    const target = args[1];
    const seen = new box([]);
    if (container is map) {
      forEach(container, (val, key) => {
        boxarrPush(seen, [key, val]);
        if (key == target) { return NextStepAction.BREAK; }
      });
    } else {
      forEach(container, (val, seq) => {
        boxarrPush(seen, [val, seq]);
        if (val == target) { return NextStepAction.BREAK; }
      });
    }
    return seen[];
  });
}

/**
 * BREAK under the `keylist` and `missingPolicy` overloads: `keylist` order (not `keys(bag)`
 * order) is what BREAK stops early against, and a SKIP'd undefined entry does not consume a
 * `seq` slot on the way to the match.
 */
export const ForEachKeylistBreakCases = [
  [[ { a: 1, b: 2, c: 3 }, ["c", "a", "b"], "a" ],
   [["c", 3, 0], ["a", 1, 1]],
   'walks keylist order, not keys(bag) order, and still stops at the match'],
  [[ { a: 1, b: 2, c: 3 }, ["c", "a", "b"], MissingPolicy.SKIP, "a" ],
   [["c", 3, 0], ["a", 1, 1]],
   'SKIP only matters for undefined entries; the match still stops the walk the same way'],
  [[ { a: 1, b: undefined, c: 3 }, ["a", "b", "c"], MissingPolicy.SKIP, "c" ],
   [["a", 1, 0], ["c", 3, 1]],
   'SKIP passes over the undefined "b" entry without counting it toward seq, then stops at "c"'],
  [[ { a: 1, b: undefined, c: 3 }, ["a", "b", "c"], MissingPolicy.USE_UNDEFINED, "b" ],
   [["a", 1, 0], ["b", undefined, 1]],
   'USE_UNDEFINED visits the undefined "b" entry (consuming a seq slot) and can still match on it'],
  [[ { a: 1, b: 2 }, ["a", "a", "b", "z", "b", "z"], "b" ],
   [["a", 1, 0], ["a", 1, 1], ["b", 2, 2]],
   'a keylist entry repeated twice is visited (and can match) twice, each consuming its own seq slot, and BREAK at the first "b" leaves the trailing "z"/"b"/"z" unvisited'],
];
export function runForEachKeylistBreakTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "forEach(keylist) BREAK", verbose, ForEachKeylistBreakCases, function(args is array) {
    const bag = args[0];
    const keylist = args[1];
    const target = (size(args) <= 3) ? args[2] : args[3];
    const seen = new box([]);
    const recordBreak = function(val, key, seq) {
      boxarrPush(seen, [key, val, seq]);
      if (key == target) { return NextStepAction.BREAK; }
    };
    if (size(args) <= 3) {
      forEach3(bag, keylist, recordBreak);
    } else {
      forEach3(bag, keylist, args[2], recordBreak);
    }
    return seen[];
  });
}

// == [Lodash Collection ports] ==

const isEven = curry2to1(function(val) { return val % 2 == 0; });
const parityKey = function(val, seq) { return isEven(val, seq) ? "even" : "odd"; };

export const CountByCases = [
  [[ [1, 2, 3, 4], parityKey ], { "odd": 2, "even": 2 } ],
  [[ [],           parityKey ], {} ],
  [[ [1],          parityKey ], { "odd": 1 },  'single element' ],
  [[ [2, 4, 6],    parityKey ], { "even": 3 }, 'every element lands in the same bucket'],
  //
  [[ { a: 1, b: 2, c: 3, d: 4 }, parityKey ], { "odd": 2, "even": 2 }, 'map form counts values the same way'],
  [[ {},                         parityKey ], {} ],
];
export function runCountByTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "countBy", verbose, CountByCases, function(args is array) { return countBy(args[0], args[1]); });
}

export const FindCases = [
  [[ [1, 2, 3], function(val, _seq) { return val > 1; } ], 2 ],
  [[ [1, 2, 3], function(val, _seq) { return val > 9; } ], undefined ],
  [[ [],        function(val, _seq) { return true; } ],   undefined, 'empty array: never matches'],
  [[ [1, 2, 3], function(val, _seq) { return val > 0; } ], 1,         'multiple matches: returns the first'],
  //
  [[ { a: 1, b: 2, c: 3 }, function(val, _seq) { return val > 1; } ], 2, 'map form returns the value, not the key'],
  [[ { a: 1, b: 2, c: 3 }, function(val, _seq) { return val > 9; } ], undefined ],
  [[ {},                   function(val, _seq) { return true; } ],   undefined, 'empty map: never matches'],
  [[ { a: 1, b: 2, c: 3 }, function(val, _seq) { return val > 0; } ], 1,         'multiple matches: returns the first in keys(bag) order'],
];
export function runFindTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "find", verbose, FindCases, function(args is array) { return find(args[0], args[1]); });
}

export const FindLastCases = [
  [[ [1, 2, 3], function(val, _seq) { return val < 3; } ], 2 ],
  [[ [1, 2, 3], function(val, _seq) { return val > 9; } ], undefined ],
  [[ [],        function(val, _seq) { return true; } ],   undefined, 'empty array: never matches'],
  [[ [1, 2, 3], function(val, _seq) { return val > 0; } ], 3,         'multiple matches: returns the last'],
  //
  [[ { a: 1, b: 2, c: 3 }, function(val, _seq) { return val < 3; } ], 2 ],
  [[ { a: 1, b: 2, c: 3 }, function(val, _seq) { return val > 9; } ], undefined ],
  [[ {},                   function(val, _seq) { return true; } ],   undefined, 'empty map: never matches'],
  [[ { a: 1, b: 2, c: 3 }, function(val, _seq) { return val > 0; } ], 3,         'multiple matches: returns the last in keys(bag) order'],
];
export function runFindLastTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "findLast", verbose, FindLastCases, function(args is array) { return findLast(args[0], args[1]); });
}

const duplicated = function(val, _seq) { return [val, val]; };
const nestedPair = function(val, _seq) { return [[val], [val]]; };

export const FlatMapCases = [
  [[ [1, 2], duplicated ], [1, 1, 2, 2] ],
  [[ [],     duplicated ], [] ],
  [[ [1, 2], function(val, _seq) { return []; } ], [], 'an iteratee returning [] contributes nothing to the result'],
  //
  [[ { a: 1, b: 2 }, duplicated ], [1, 1, 2, 2], 'map form flattens over values in keys(bag) order'],
  [[ {},             duplicated ], [] ],
];
export function runFlatMapTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "flatMap", verbose, FlatMapCases, function(args is array) { return flatMap(args[0], args[1]); });
}

export const FlatMapDeepCases = [
  [[ [1, 2], nestedPair ], [1, 1, 2, 2] ],
  [[ [],     nestedPair ], [] ],
  //
  [[ { a: 1, b: 2 }, nestedPair ], [1, 1, 2, 2] ],
  [[ {},             nestedPair ], [] ],
];
export function runFlatMapDeepTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "flatMapDeep", verbose, FlatMapDeepCases, function(args is array) { return flatMapDeep(args[0], args[1]); });
}

export const FlatMapDepthCases = [
  [[ [1, 2], nestedPair, 1 ], [[1], [1], [2], [2]] ],
  [[ [1, 2], nestedPair, 0 ], [[[1], [1]], [[2], [2]]], 'depth 0 leaves the mapped-but-unflattened result unchanged'],
  [[ [],     nestedPair, 1 ], [] ],
  //
  [[ { a: 1, b: 2 }, nestedPair, 1 ], [[1], [1], [2], [2]] ],
  [[ {},             nestedPair, 1 ], [] ],
];
export function runFlatMapDepthTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "flatMapDepth", verbose, FlatMapDepthCases, function(args is array) { return flatMapDepth(args[0], args[1], args[2]); });
}

export const ForEachRightCases = [
  [[ [1, 2, 3] ], [3, 2, 1] ],
  [[ [] ],        [] ],
  //
  [[ { a: 1, b: 2, c: 3 } ], [3, 2, 1], 'map form walks keys(bag) in reverse'],
  [[ {} ],                   [] ],
];
export function runForEachRightTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "forEachRight", verbose, ForEachRightCases, function(args is array) {
    const seen = new box([]);
    forEachRight(args[0], (val, seq) => { boxarrPush(seen, val); });
    return seen[];
  });
}

export const GroupByCases = [
  [[ [1, 2, 3, 4], parityKey ], { "odd": [1, 3], "even": [2, 4] } ],
  [[ [],           parityKey ], {} ],
  [[ [2, 4],       parityKey ], { "even": [2, 4] }, 'every element lands in the same group'],
  //
  [[ { a: 1, b: 2, c: 3, d: 4 }, parityKey ], { "odd": [1, 3], "even": [2, 4] }, 'map form groups values in keys(bag) order'],
  [[ {},                         parityKey ], {} ],
];
export function runGroupByTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "groupBy", verbose, GroupByCases, function(args is array) { return groupBy(args[0], args[1]); });
}

export const PartitionCases = [
  [[ [1, 2, 3, 4], isEven ], [[2, 4], [1, 3]] ],
  [[ [],           isEven ], [[], []] ],
  [[ [2, 4],       isEven ], [[2, 4], []], 'every element passes: the failed side is empty'],
  [[ [1, 3],       isEven ], [[], [1, 3]], 'no elements pass: the passed side is empty'],
  //
  [[ { a: 1, b: 2, c: 3, d: 4 }, isEven ], [[2, 4], [1, 3]], 'map form partitions values, keys(bag) order'],
  [[ {},                         isEven ], [[], []] ],
];
export function runPartitionTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "partition", verbose, PartitionCases, function(args is array) { return partition(args[0], args[1]); });
}

const concatString = function(acc, val, _seq) { return acc ~ val; };
export const ReduceRightCases = [
  [[ [1, 2, 3], "", concatString ],           "321" ],
  [[ [],        "seed", concatString ],       "seed", 'empty array with a seed: the seed passes through unchanged'],
  [[ [],        concatString ],               undefined, 'empty array, no seed: nothing to fold, returns undefined'],
  [[ ["1"],     concatString ],               "1",       'single-element array: no-seed overload just returns that element'],
  [[ ["1", "2", "3"], concatString ],         "321", 'no-seed overload starts from the last element'],
  //
  [[ { a: 1, b: 2, c: 3 }, "", concatString ],              "321", 'map form folds keys(bag) in reverse'],
  [[ {},                   "seed", concatString ],          "seed", 'empty map with a seed: the seed passes through unchanged'],
  [[ {},                   concatString ],                  undefined, 'empty map, no seed: nothing to fold, returns undefined'],
  [[ { a: "1" },           concatString ],                  "1",       'single-entry map: no-seed overload just returns that value'],
  [[ { a: "1", b: "2", c: "3" }, concatString ],            "321", 'no-seed overload (map) starts from the last-keyed element'],
];
export function runReduceRightTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "reduceRight", verbose, ReduceRightCases, function(args is array) {
    return (size(args) <= 2) ? reduceRight(args[0], args[1]) : reduceRight(args[0], args[1], args[2]);
  });
}

export const RejectCases = [
  [[ [1, 2, 3, 4], isEven ], [1, 3] ],
  [[ [],           isEven ], [] ],
  [[ [2, 4],       isEven ], [], 'every element rejected: nothing survives'],
  [[ [1, 3],       isEven ], [1, 3], 'nothing rejected: input survives unchanged'],
  //
  [[ { a: 1, b: 2, c: 3, d: 4 }, isEven ], [1, 3], 'map form rejects by value, returns an array'],
  [[ {},                         isEven ], [] ],
];
export function runRejectTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "reject", verbose, RejectCases, function(args is array) { return reject(args[0], args[1]); });
}
