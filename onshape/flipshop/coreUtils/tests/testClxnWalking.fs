FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
import(path : "4ebdc64943b566160ea5cc28", version : "3684960ff67d0db556614489");
import(path : "6fcd20533bd2df7c4094a0bf", version : "5849e1654325cddeda6f7440");
import(path : "dd812faf6ff4099cda4aa0eb", version : "ccf1aec9c08b9ad59691b4d4");
import(path : "66e287bede293cb227dfb89c", version : "e6c9aacc05cf0a6f15c7d4c7");
import(path : "08b6ba15b8255611bafe7520", version : "2a3b23fb0cdd7a00cb549f43");
import(path : "f6ab954150609e1e2e014a1e", version : "cefd17945c168add0f500dad");
//
import(path : "a92a3694a1d67e78c541bed5", version : "56ba169394d4d160246bdce5");
import(path : "b87a9ac25313676049cf04cb", version : "3e528d35b76d37ea2d572166");

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
    runMapValuesTests(context,  verbose);
    runMapValues3Tests(context, verbose);
    runValuesAtTests(context,   verbose);
    runRebagTests(context, verbose);
    //
    runPathForKeyTests(context, verbose);
    runDeepMergeTests(context, verbose);
    runGetAtPathTests(context, verbose);
    runGetAtArrTests(context, verbose);
    runSetAtTests(context, verbose);
    if (verbose) { runSetAtThrowsTests(context, verbose); }
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

const rebagArrInspector = function(val, seq is number)                { if (val == "skip") return undefined; return ["" ~ seq ~ seq, [val, seq]]; };
const rebagMapInspector = function(val, key is string, seq is number) { if (val == "skip") return undefined; return ["" ~ key ~ key, [val, key, seq]]; };
export function runRebagTests(context is Context, verbose is boolean) returns map {
    return runTests(context, "rebag", verbose, [
        [[ {},                             curry3to0(noop)],   {} ],
        [[ { a: 11, b: 22 },               curry3to0(noop)],   {} ],
        [[ {},                             rebagMapInspector], {} ],
        [[ { a: 11, b: 22 },               rebagMapInspector], { aa: [11, "a", 0], bb: [22, "b", 1] } ],
        [[ { b: 22, a: 11 },               rebagMapInspector], { aa: [11, "a", 0], bb: [22, "b", 1] } ],
        [[ { b: 22, a: 11, d: undefined }, rebagMapInspector], { aa: [11, "a", 0], bb: [22, "b", 1] } ],
        [[ { b: 22, a: 11, s: "skip" },    rebagMapInspector], { aa: [11, "a", 0], bb: [22, "b", 1] } ],
        //
        [[ [],                             curry2to0(noop)],   {} ],
        [[ [11, 22],                       curry2to0(noop)],   {} ],
        [[ [],                             rebagArrInspector], {} ],
        [[ ["skip"],                       rebagArrInspector], {} ],
        [[ [11, 22],                       rebagArrInspector], { '00': [11, 0], '11': [22, 1] } ],
        [[ [11, 22, "skip"],               rebagArrInspector], { '00': [11, 0], '11': [22, 1] } ],
        //
        //
    ], function(args is array) { return rebag(args[0], args[1]);  });
}
