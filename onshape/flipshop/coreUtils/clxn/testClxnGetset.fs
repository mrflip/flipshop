FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");

import(path : "4ebdc64943b566160ea5cc28", version : "3684960ff67d0db556614489");
import(path : "f6ab954150609e1e2e014a1e", version : "cefd17945c168add0f500dad");
import(path : "fb2ebd26998aed1d4a76e115", version : "61f6dfc83f92e38721d9a67d");
// Testing this:
import(path : "9263ab535d0213ef0f0e9d5e", version : "4acbb4ada80b5866d2b009b9");
import(path : "607f97fc690581579d1d4a08", version : "6afa9ed6ec4a413d9bf06340");

/**
 * what the path forms, the resolver and the splitter add.
 */
export const PathForKeyCases = [
  [["a.b.c"], ["a", "b", "c"], 'the ordinary case'],
  [["foo"],   ["foo"],         'no dot is one segment'],
  [[""],      [""],            'empty key never reaches the splitter, so it stays one literal empty key'],
  [[".foo"],  ["", "foo"],     'a leading empty segment is kept'],
  [["a..b"],  ["a", "", "b"],  'an empty segment in the middle names an empty key'],
  [["foo."],  ["foo"],         'unhandled: the terminal empty segment is discarded, so this reads as "foo"'],
  [[".."],    ["", ""],        'unhandled: three empty keys would be right, one terminal segment is discarded'],
  [["2.1mm"], ["2", "1mm"],    'a dot in a size name is a dot like any other'],
];

export const MergeCases = [
  [[{ "a": 1 },                   { "b": 2 }],          { "a": 1, "b": 2 },           'disjoint keys'],
  [[{ "a": { "x": 1 } },          { "a": { "y": 2 } }], { "a": { "x": 1, "y": 2 } },  'maps combine a level down, which is exactly where mergeMaps clobbers'],
  [[{ "a": 1 },                   { "a": 2 }],          { "a": 2 },                   'incoming wins a leaf, which under the depth sort means the deeper key wins'],
  [[{ "a": { "x": 1 } },          { "a": 2 }],          { "a": 2 },                   'a map is replaced by a scalar rather than defended'],
  [[{ "a": [1, 2] },              { "a": [3] }],        { "a": [3, 2] },              'arrays merge index by index, same as lodash; existing tail past incoming survives'],
  [[1,                            { "a": 2 }],          { "a": 2 },                   'neither side a map: incoming wins'],
  [[{ "a": 1 },                   undefined],           { "a": 1 },                   'an undefined source leaves what is there alone'],
  [[{ "a": { "x": { "p": 1 } } }, { "a": { "x":         { "q": 2 } } }], { "a": { "x": { "p": 1, "q": 2 } } },  'and all the way down, not just one level'],
];

const GetAtTestCases = [
  [[[1, 2, 3], 0], 1],
  [[[1, 2, 3], 1], 2],
  [[[1, 2, 3], 2], 3],
  [[[1, 2, 3], -1], 3],
  [[[1, 2, 3], -2], 2],
  [[[1, 2, 3], -3], 1],
  [[[1, 2, 3], 3], undefined],
  [[[1, 2, 3], -4], undefined],
  [[[], 0], undefined],
  [[[], -1], undefined],
  [[[], 1], undefined],
  [[[1, 2, 3], 100], undefined],
  [[[1, undefined, 3], 0], 1],
  [[[1, undefined, 3], 1], undefined],
  [[[1, undefined, 3], 2], 3],

  [[[1, 2, 3], 0, undefined], 1],
  [[[1, 2, 3], 1, undefined], 2],
  [[[1, 2, 3], 2, undefined], 3],
  [[[1, 2, 3], -1, undefined], 3],
  [[[1, 2, 3], -2, undefined], 2],
  [[[1, 2, 3], -3, undefined], 1],
  [[[1, 2, 3], 3, undefined], undefined],
  [[[1, 2, 3], -4, undefined], undefined],
  [[[], 0, undefined], undefined],
  [[[], -1, undefined], undefined],
  [[[], 1, undefined], undefined],
  [[[1, 2, 3], 100, undefined], undefined],
  [[[1, undefined, 3], 0, undefined], 1],
  [[[1, undefined, 3], 1, undefined], undefined],
  [[[1, undefined, 3], 2, undefined], 3],

  [[[1, 2, 3], 0, 42],  1],
  [[[1, 2, 3], 1, 42],  2],
  [[[1, 2, 3], 2, 42],  3],
  [[[1, 2, 3], -1, 42], 3],
  [[[1, 2, 3], -2, 42], 2],
  [[[1, 2, 3], -3, 42], 1],
  [[[1, 2, 3], 3, 42],  42],
  [[[1, 2, 3], -4, 42], 42],
  [[[], 0, 42], 42],
  [[[], -1, 42], 42],
  [[[], 1, 42], 42],
  [[[1, 2, 3], 100, 42], 42],
  [[[1, undefined, 3], 0, 42], 1],
  [[[1, undefined, 3], 1, 42], undefined, "If element is present, it's returned (not the fallback), even if it's undefined"],
  [[[1, undefined, 3], 2, 42], 3],

];

function runGetAtArrTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "getAt", verbose, GetAtTestCases, function(args is array) { return size(args) <= 2 ? getAt(args[0], args[1]) : getAt(args[0], args[1], args[2]); });
}

export const GetAtPathCases = [
  [[{ "a": { "b": 1 } },               "a.b"],                       1,                             'dotted path into nested maps'],
  [[{ "a": { "b": 1 } },               ["a", "b"]],                  1,                             'key list is the same path spelled out'],
  [[{ "a.b": 1 },                      ["a.b"]],                     1,                             'key list takes keys literally: the way to reach a key with a dot in it'],
  [[{ "a.b": 1 },                      "a.b"],                       undefined,                     'the same map read as a path finds nothing, which is the tradeoff'],
  [[{ "a": { "b": { "c": 1 } } },      []],                          { "a": { "b": { "c": 1 } } },  'empty path is the identity, so a fold over segments composes'],
  [[{ "rows": [{ "cells": [7, 8] }] }, "rows.0.cells.1"],            8,                             'integer segments index arrays met on the way down'],
  [[{ "rows": [7] },                   ["rows", 0]],                 7,                             'a key list can mix strings and numbers'],
  [[{ "": { "": 1 } },                 "."],                         { "": 1 },                     'empty segments read the empty keys they name'],
  [[{ "": { "": 1 } },                 ".."],                        1,                             'empty segments read the empty keys they name'],
  [[{ "a": 1 },                        "a.b",    42],                42,                            'stepping into a scalar is a miss, not an error'],
  [[{ "a": { "b": 1 } },               "a.c.d",  42],                42,                            'a miss partway down stops there rather than descending the fallback'],
  [[{ "a": { "b": 1 } },               "a.c",    { "not": "met" }],  { "not": "met" },              'a fallback that is itself a map comes back whole'],
  [[{ "rows": [{ "cells": [7, 8] }] }, "rows.-1.cells.-1"],          8,                             'negative indexes count from the end mid-path'],
  [[[[1, 2], [3, 4]],                   "1.0"],                      3,                             'a path works from an array at the top too'],
  [[[1, 2, 3],                         "-1"],                        3,                             'integer string indexes like the number'],
  [[[1, undefined, 3],                 "1",                          42], undefined,                'present and undefined beats the fallback here as well'],
];

export const SetAtCases = [
  [[{},                          "a.b.c",    1],         { "a": { "b": { "c": 1 } } },    'autovivifies the whole path'],
  [[{ "a": { "x": 1 } },         "a.b",      2],         { "a": { "x": 1, "b": 2 } },     'descends what is there and leaves siblings alone'],
  [[{ "a": { "b": { "x": 1 } } },"a.b",      2],         { "a": { "b": 2 } },             'no resolver: the subtree under a.b is replaced'],
  [[{ "a": 1 },                  "a.b",      2],         { "a": { "b": 2 } },             'a scalar in the way is replaced by a map, the way lodash set does it'],
  [[{ "a": { "b": 1 } },         ["a.b"],    2],         { "a": { "b": 1 }, "a.b": 2 },   'key list writes the literal dotted key alongside the nested one'],
  [[{ "a": { "b": 1 } },         "a.b",      undefined], { "a": {} },                     'FeatureScript elides an undefined value, so setting one deletes the key instead of storing it'],
  [[{},                          "a.0.b",    1],         { "a": [{ "b": 1 }] },           'autovivifies an array for an integer segment, same as lodash'],
  [[{ "rows": [{ "x": 1 }] },    "rows.0.x", 2],         { "rows": [{ "x": 2 }] },        'writes through an array met on the way down'],
  [[{ "rows": [1, 2, 3] },       "rows.-1",  9],         { "rows": [1, 2, 9] },           'negative index writes from the end'],
  [[[1, 2],                      "1",        9],         [1, 9],                          'integer string writes like the number'],
  [[[1, 2],                      4,          9],         [1, 2, undefined, undefined, 9], 'writing past the end pads with undefined'],
  [[[1, 2],                      2,          9],         [1, 2, 9],                       'writing one past the end is a plain append'],

  [[{ "a": { "x": 1 } }, "a", { "y": 2 }, ((existing, incoming) => deepMerge(existing, incoming))],
   { "a": { "x": 1, "y": 2 } },
   'with merge as the resolver the two maps combine instead of one replacing the other'],
  [[{ "a": { "x": 1 } }, "a", { "y": 2 }, lastInWins],
   { "a": { "y": 2 } },
   'and lastInWins spelled out is the same as leaving the resolver off'],
  [[{ "a": { "x": 1 } }, "a.b", 2, ((existing, incoming) => deepMerge(existing, incoming))],
   { "a": { "x": 1, "b": 2 } },
   'the resolver sees the leaf, not the steps taken to reach it'],
  [[{ "a": 1 }, "a.b", 2, ((existing, incoming) => deepMerge(existing, incoming))],
   { "a": { "b": 2 } },
   'a scalar in the way of a deeper path is replaced without the resolver being asked'],
  [[{ "a": { "x": 1 } }, "a", { "y": 2 }, ((existing, incoming) => 'collided')],
   { "a": 'collided' },
   'the resolver can return anything at all; only its answer is placed'],
];

export const SetAtThrows = [
  [[[1, 2], "foo", 9],
   'Index foo is not a valid key for [ 1 , 2 ]',
   'a non-index key has nowhere to go in an array'],
  [[[1, 2], -5, 9],
   'Index -5 is before the start of a 2-element array',
   'a negative index past the start has no meaning, and padding the front would renumber the rest'],
  [
    [{ "rows": [7] }, "rows.foo", 42],
    'Index foo is not a valid key for [ 7 ]',
  ],
  [
      [[1, 2, 3], "foo", 42],
      "Index foo is not a valid key for [ 1 , 2 , 3 ]",
  ],
];

const runPathForKeyTest = ((args is array) => pathForKey(args[0]));
const runMergeTest      = ((args is array) => deepMerge(args[0], args[1]));
const runGetAtTest      = ((args is array) => ((size(args) <= 2) ? getAt(args[0], args[1]) : getAt(args[0], args[1], args[2])));
const runSetAtTest      = ((args is array) => ((size(args) <= 3) ? setAt(args[0], args[1], args[2]) : setAt(args[0], args[1], args[2], args[3])));

function runPathForKeyTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "pathForKey", verbose, PathForKeyCases, runPathForKeyTest);
}

function runDeepMergeTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "merge", verbose, MergeCases, runMergeTest);
}

function runGetAtPathTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "getAt paths", verbose, GetAtPathCases, runGetAtTest);
}

function runSetAtTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "setAt", verbose, SetAtCases, runSetAtTest);
}

function runSetAtThrowsTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "setAt throws", verbose, SetAtThrows, assertThrows(runSetAtTest));
}

export const UpdateCases = [
  [[{ "a": 1 },          "a",   function(val) { return val + 1; }],           { "a": 2 }],
  [[{ "a": { "b": 1 } }, "a.b", function(val) { return val + 1; }],           { "a": { "b": 2 } }],
  [[{},                  "a.b", function(val) { return ifNil(val, 0) + 1; }], { "a": { "b": 1 } }, 'a missing path autovivifies, reading as undefined first'],
];
function runUpdateTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "update", verbose, UpdateCases, function(args is array) { return update(args[0], args[1], args[2]); });
}

const sumIfExisting = function(existingVal, incomingVal) {
  return (existingVal is number) ? (existingVal + incomingVal) : undefined;
};

export const UpdateWithCases = [
  [[{ "a": 1 }, "a",   function(val) { return val + 1; }, sumIfExisting],
   { "a": 3 },
   'the resolver sees the old value and the updater result when the leaf already exists'],
  [[{},         "a.b", function(val) { return ifNil(val, 0) + 1; }, sumIfExisting],
   { "a": { "b": 1 } },
   'a freshly autovivified path never collides, so the resolver is not consulted'],
];
function runUpdateWithTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "updateWith", verbose, UpdateWithCases, function(args is array) { return updateWith(args[0], args[1], args[2], args[3]); });
}

export const AssignWithCases = [
  [[{ "a": 1, "b": 2 }, { "a": 10 }, sumIfExisting], { "a": 11, "b": 2 }, 'the resolver combines a key present in both'],
  [[{ "a": 1 },         { "b": 2 },  sumIfExisting], { "a": 1, "b": 2 },  'the resolver returns undefined for a key only in incoming, which falls back to incoming winning'],
];
function runAssignWithTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "assignWith", verbose, AssignWithCases, function(args is array) { return assignWith(args[0], args[1], args[2]); });
}

export const MergeWithCases = [
  [[{ "a": 1 },          { "a": 2 },          sumIfExisting], { "a": 3 },                  'the resolver combines a leaf present in both'],
  [[{ "a": { "x": 1 } }, { "a": { "y": 2 } }, sumIfExisting], { "a": { "x": 1, "y": 2 } }, 'a non-number pair falls back to the ordinary deep-merge rule'],
  [[{ "a": 1 },          undefined,           sumIfExisting], { "a": 1 },                  'an undefined source leaves what is there alone, same as deepMerge'],
];
function runMergeWithTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "mergeWith", verbose, MergeWithCases, function(args is array) { return mergeWith(args[0], args[1], args[2]); });
}
