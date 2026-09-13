FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
//
import(path : "4ebdc64943b566160ea5cc28", version : "49324f8ccdd2bc8f00f68a0f");
import(path : "f6ab954150609e1e2e014a1e", version : "17748a1c9444555f3047e363");
//
import(path : "c6c66ffbc9a017f4d26adb08", version : "bc443926a8055a7e94712dd6"); // testing this

const SuiteTitle = "Array Utils";

// == [Run tests] ==

annotation { "Feature Type Name": 'Run ' ~ SuiteTitle ~ ' Tests', "Feature Type Description": "Internal Tests" }
export const runArrayUtilsTestsFS = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Verbose", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE, "Default": false }
  definition.verbose is boolean;
}
{
  const verbose = ifNil(definition.verbose, false);
  try {
    runChunkTests(context, verbose);
    runCompactTests(context, verbose);
    runDifferenceTests(context, verbose);
    runDifferenceByTests(context, verbose);
    runDifferenceWithTests(context, verbose);
    runDropTests(context, verbose);
    runDropRightTests(context, verbose);
    runDropRightWhileTests(context, verbose);
    runDropWhileTests(context, verbose);
    runFindIndexTests(context, verbose);
    runFindLastIndexTests(context, verbose);
    runFlattenTests(context, verbose);
    runFlattenDeepTests(context, verbose);
    runFlattenDepthTests(context, verbose);
    runFromPairsTests(context, verbose);
    runInitialTests(context, verbose);
    runIntersectionTests(context, verbose);
    runIntersectionByTests(context, verbose);
    runIntersectionWithTests(context, verbose);
    runLastIndexOfTests(context, verbose);
    runNthTests(context, verbose);
    runTailTests(context, verbose);
    runTakeTests(context, verbose);
    runTakeRightTests(context, verbose);
    runTakeRightWhileTests(context, verbose);
    runTakeWhileTests(context, verbose);
    runUnionTests(context, verbose);
    runUnionByTests(context, verbose);
    runUnionWithTests(context, verbose);
    runUniqByTests(context, verbose);
    runUniqWithTests(context, verbose);
    runUnzipTests(context, verbose);
    runUnzipWithTests(context, verbose);
    runWithoutTests(context, verbose);
    runXorTests(context, verbose);
    runXorByTests(context, verbose);
    runXorWithTests(context, verbose);
    runZipObjectTests(context, verbose);
    runZipWithTests(context, verbose);
    //
    if (! verbose) { debug(context, starbanner('** ' ~ SuiteTitle ~ ' Tests ran successfully **')); }
  } catch (err) {
    debug(context, starbanner('** Error in ' ~ SuiteTitle ~ ' Tests: ' ~ err ~ ' **'));
  }
});

// == [chunk] ==

export const ChunkCases = [
  [[["a", "b", "c", "d"], 2],  [["a", "b"], ["c", "d"]]],
  [[["a", "b", "c", "d"], 3],  [["a", "b", "c"], ["d"]]],
  [[[],                   2],  []],
  [[["a", "b"],           0],  [],  'chunkSize < 1 returns an empty array'],
];
export function runChunkTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "chunk", verbose, ChunkCases, function(args is array) { return chunk(args[0], args[1]); });
}

// == [compact] ==

export const CompactCases = [
  [[[0, 1, false, 2, "", 3]],  [0, 1, 2, "", 3],  'only undefined/false are dropped'],
  [[[false, undefined]],       [],                'all-falsey input compacts to empty'],
];
export function runCompactTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "compact", verbose, CompactCases, function(args is array) { return compact(args[0]); });
}

// == [difference / differenceBy] ==

export const DifferenceCases = [
  [[[2, 1], [2, 3]],  [1]],
  [[[1, 2], []],      [1, 2]],
];
export function runDifferenceTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "difference", verbose, DifferenceCases, function(args is array) { return difference(args[0], args[1]); });
}

export const DifferenceByCases = [
  [[[2.1, 1.2], [2.3, 3.4], function(val) { return floor(val); }],  [1.2]],
];
export function runDifferenceByTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "differenceBy", verbose, DifferenceByCases, function(args is array) { return differenceBy(args[0], args[1], args[2]); });
}

export const DifferenceWithCases = [
  [[[{ "x": 1 }, { "x": 2 }], [{ "x": 1 }], function(aa, bb) { return aa.x == bb.x; }],  [{ "x": 2 }]],
];
export function runDifferenceWithTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "differenceWith", verbose, DifferenceWithCases, function(args is array) { return differenceWith(args[0], args[1], args[2]); });
}

// == [drop / dropRight / dropWhile / dropRightWhile] ==

export const DropCases = [
  [[[1, 2, 3], 2],  [3]],
  [[[1, 2, 3], 5],  []],
  [[[1, 2, 3], 0],  [1, 2, 3]],
];
export function runDropTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "drop", verbose, DropCases, function(args is array) { return drop(args[0], args[1]); });
}

export const DropRightCases = [
  [[[1, 2, 3], 2],  [1]],
  [[[1, 2, 3], 5],  []],
  [[[1, 2, 3], 0],  [1, 2, 3]],
];
export function runDropRightTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "dropRight", verbose, DropRightCases, function(args is array) { return dropRight(args[0], args[1]); });
}

export const DropRightWhileCases = [
  [[[1, 2, 3, 4], function(val) { return val > 2; }],  [1, 2]],
  [[[1, 2, 3, 4], function(val) { return val > 9; }],  [1, 2, 3, 4]],
  [[[1, 2, 3, 4], function(val) { return val > 0; }],  []],
];
export function runDropRightWhileTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "dropRightWhile", verbose, DropRightWhileCases, function(args is array) { return dropRightWhile(args[0], args[1]); });
}

export const DropWhileCases = [
  [[[1, 2, 3, 4], function(val) { return val < 3; }],  [3, 4]],
  [[[1, 2, 3, 4], function(val) { return val < 0; }],  [1, 2, 3, 4]],
];
export function runDropWhileTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "dropWhile", verbose, DropWhileCases, function(args is array) { return dropWhile(args[0], args[1]); });
}

// == [findIndex / findLastIndex] ==

export const FindIndexCases = [
  [[[1, 2, 3], function(val) { return val > 1; }],  1],
  [[[1, 2, 3], function(val) { return val > 9; }],  -1],
];
export function runFindIndexTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "findIndex", verbose, FindIndexCases, function(args is array) { return findIndex(args[0], args[1]); });
}

export const FindLastIndexCases = [
  [[[1, 2, 3], function(val) { return val < 3; }],  1],
  [[[1, 2, 3], function(val) { return val > 9; }],  -1],
];
export function runFindLastIndexTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "findLastIndex", verbose, FindLastIndexCases, function(args is array) { return findLastIndex(args[0], args[1]); });
}

// == [flatten / flattenDeep / flattenDepth] ==

export const FlattenCases = [
  [[[1, [2, [3, [4]], 5]]],  [1, 2, [3, [4]], 5]],
];
export function runFlattenTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "flatten", verbose, FlattenCases, function(args is array) { return flatten(args[0]); });
}

export const FlattenDeepCases = [
  [[[1, [2, [3, [4]], 5]]],  [1, 2, 3, 4, 5]],
];
export function runFlattenDeepTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "flattenDeep", verbose, FlattenDeepCases, function(args is array) { return flattenDeep(args[0]); });
}

export const FlattenDepthCases = [
  [[[1, [2, [3, [4]], 5]], 2],  [1, 2, 3, [4], 5]],
  [[[1, [2, [3]]],         0],  [1, [2, [3]]],  'depth <= 0 returns arr unchanged'],
];
export function runFlattenDepthTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "flattenDepth", verbose, FlattenDepthCases, function(args is array) { return flattenDepth(args[0], args[1]); });
}

// == [fromPairs] ==

export const FromPairsCases = [
  [[[["a", 1], ["b", 2]]],  { "a": 1, "b": 2 }],
  [[[["a", 1], ["a", 2]]],  { "a": 2 },  'a repeated key keeps the last pair'],
  [[[]],                    {}],
];
export function runFromPairsTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "fromPairs", verbose, FromPairsCases, function(args is array) { return fromPairs(args[0]); });
}

// == [initial] ==

export const InitialCases = [
  [[[1, 2, 3]],  [1, 2]],
  [[[1]],        []],
  [[[]],         []],
];
export function runInitialTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "initial", verbose, InitialCases, function(args is array) { return initial(args[0]); });
}

// == [intersection / intersectionBy] ==

export const IntersectionCases = [
  [[[[2, 1], [2, 3], [1, 2]]],  [2]],
  [[[[1, 2], []]],               []],
  [[[]],                         []],
];
export function runIntersectionTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "intersection", verbose, IntersectionCases, function(args is array) { return intersection(args[0]); });
}

export const IntersectionByCases = [
  [[[[2.1, 1.2], [2.3, 3.4]], function(val) { return floor(val); }],  [2.1]],
];
export function runIntersectionByTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "intersectionBy", verbose, IntersectionByCases, function(args is array) { return intersectionBy(args[0], args[1]); });
}

export const IntersectionWithCases = [
  [[[[{ "x": 1 }, { "x": 2 }], [{ "x": 2 }]], function(aa, bb) { return aa.x == bb.x; }],  [{ "x": 2 }]],
];
export function runIntersectionWithTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "intersectionWith", verbose, IntersectionWithCases, function(args is array) { return intersectionWith(args[0], args[1]); });
}

// == [lastIndexOf] ==

export const LastIndexOfCases = [
  [[[1, 2, 1], 1],  2],
  [[[1, 2, 3], 9],  -1],
];
export function runLastIndexOfTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "lastIndexOf", verbose, LastIndexOfCases, function(args is array) { return lastIndexOf(args[0], args[1]); });
}

// == [nth] ==

export const NthCases = [
  [[[1, 2, 3], 1],   2],
  [[[1, 2, 3], -1],  3],
  [[[1, 2, 3], 9],   undefined],
];
export function runNthTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "nth", verbose, NthCases, function(args is array) { return nth(args[0], args[1]); });
}

// == [tail] ==

export const TailCases = [
  [[[1, 2, 3]],  [2, 3]],
  [[[1]],        []],
  [[[]],         []],
];
export function runTailTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "tail", verbose, TailCases, function(args is array) { return tail(args[0]); });
}

// == [take / takeRight / takeWhile / takeRightWhile] ==

export const TakeCases = [
  [[[1, 2, 3], 2],  [1, 2]],
  [[[1, 2, 3], 0],  []],
  [[[1, 2, 3], 9],  [1, 2, 3]],
];
export function runTakeTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "take", verbose, TakeCases, function(args is array) { return take(args[0], args[1]); });
}

export const TakeRightCases = [
  [[[1, 2, 3], 2],  [2, 3]],
  [[[1, 2, 3], 0],  []],
];
export function runTakeRightTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "takeRight", verbose, TakeRightCases, function(args is array) { return takeRight(args[0], args[1]); });
}

export const TakeRightWhileCases = [
  [[[1, 2, 3, 4], function(val) { return val > 2; }],  [3, 4]],
];
export function runTakeRightWhileTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "takeRightWhile", verbose, TakeRightWhileCases, function(args is array) { return takeRightWhile(args[0], args[1]); });
}

export const TakeWhileCases = [
  [[[1, 2, 3, 4], function(val) { return val < 3; }],  [1, 2]],
];
export function runTakeWhileTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "takeWhile", verbose, TakeWhileCases, function(args is array) { return takeWhile(args[0], args[1]); });
}

// == [union / unionBy] ==

export const UnionCases = [
  [[[[2], [1, 2], [2, 3]]],  [2, 1, 3]],
  [[[]],                      []],
];
export function runUnionTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "union", verbose, UnionCases, function(args is array) { return union(args[0]); });
}

export const UnionByCases = [
  [[[[2.1], [1.2, 2.3]], function(val) { return floor(val); }],  [2.1, 1.2]],
];
export function runUnionByTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "unionBy", verbose, UnionByCases, function(args is array) { return unionBy(args[0], args[1]); });
}

export const UnionWithCases = [
  [[[[{ "x": 1 }], [{ "x": 1 }, { "x": 2 }]], function(aa, bb) { return aa.x == bb.x; }],  [{ "x": 1 }, { "x": 2 }]],
];
export function runUnionWithTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "unionWith", verbose, UnionWithCases, function(args is array) { return unionWith(args[0], args[1]); });
}

// == [uniqBy / uniqWith] ==

export const UniqByCases = [
  [[[2.1, 1.2, 2.3], function(val) { return floor(val); }],  [2.1, 1.2]],
];
export function runUniqByTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "uniqBy", verbose, UniqByCases, function(args is array) { return uniqBy(args[0], args[1]); });
}

export const UniqWithCases = [
  [[[{ "x": 1 }, { "x": 1 }, { "x": 2 }], function(aa, bb) { return aa.x == bb.x; }],  [{ "x": 1 }, { "x": 2 }]],
];
export function runUniqWithTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "uniqWith", verbose, UniqWithCases, function(args is array) { return uniqWith(args[0], args[1]); });
}

// == [unzip / unzipWith] ==

export const UnzipCases = [
  [[[["a", 1, true], ["b", 2, false]]],  [["a", "b"], [1, 2], [true, false]]],
];
export function runUnzipTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "unzip", verbose, UnzipCases, function(args is array) { return unzip(args[0]); });
}

export const UnzipWithCases = [
  [[[[1, 10], [2, 20]], function(col) { return sum(col); }],  [3, 30]],
];
export function runUnzipWithTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "unzipWith", verbose, UnzipWithCases, function(args is array) { return unzipWith(args[0], args[1]); });
}

// == [without] ==

export const WithoutCases = [
  [[[2, 1, 2, 3], [1, 2]],  [3]],
];
export function runWithoutTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "without", verbose, WithoutCases, function(args is array) { return without(args[0], args[1]); });
}

// == [xor] ==

export const XorCases = [
  [[[[2, 1], [2, 3]]],  [1, 3]],
  [[[[1], [1]]],        []],
];
export function runXorTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "xor", verbose, XorCases, function(args is array) { return xor(args[0]); });
}

export const XorByCases = [
  [[[[2.1, 1.2], [2.3, 3.4]], function(val) { return floor(val); }],  [1.2, 3.4]],
];
export function runXorByTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "xorBy", verbose, XorByCases, function(args is array) { return xorBy(args[0], args[1]); });
}

export const XorWithCases = [
  [[[[{ "x": 1 }, { "x": 2 }], [{ "x": 2 }]], function(aa, bb) { return aa.x == bb.x; }],  [{ "x": 1 }]],
];
export function runXorWithTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "xorWith", verbose, XorWithCases, function(args is array) { return xorWith(args[0], args[1]); });
}

// == [zipObject / zipWith] ==

export const ZipObjectCases = [
  [[["a", "b"], [1, 2]],       { "a": 1, "b": 2 }],
  [[["a", "b"], [1]],          { "a": 1 },  'a keylist entry past valuelist is absent (FS drops undefined-valued keys)'],
];
export function runZipObjectTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "zipObject", verbose, ZipObjectCases, function(args is array) { return zipObject(args[0], args[1]); });
}

export const ZipWithCases = [
  [[[[1, 2], [10, 20]], function(row) { return sum(row); }],  [11, 22]],
];
export function runZipWithTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "zipWith", verbose, ZipWithCases, function(args is array) { return zipWith(args[0], args[1]); });
}
