FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
//
import(path : "f6ab954150609e1e2e014a1e", version : "4aa5dee775898ca8543bc9fc"); // runTests
// Testing this:
import(path : "dcee57677ef58f0c8e045269", version : "1b435d947cb9e47cf0480a69"); // sortUtils <- testing this
import(path : "08b6ba15b8255611bafe7520", version : "1e6e0baae135d1e3f89931d2");

const SuiteTitle = "Sort Utils";

// == [Run tests] ==

annotation { "Feature Type Name": 'Run ' ~ SuiteTitle ~ ' Tests', "Feature Type Description": "Internal Tests" }
export const runSortUtilsTestsFS = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Verbose", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE, "Default": false }
  definition.verbose is boolean;
}
{
  const verbose = ifNil(definition.verbose, false);
  try {
    runCmpArrayDefaultTests(context, verbose);
    runCmpToArrayComparatorTests(context, verbose);
    runCmpStringTests(context, verbose);
    runCmpNumberTests(context, verbose);
    runCmpValueWithUnitsTests(context, verbose);
    runCmpBooleanTests(context, verbose);
    runCmpUndefinedTests(context, verbose);
    runCmpMapByKeysOnlyTests(context, verbose);
    runCmpMapTests(context, verbose);
    runCmpMapGenericTests(context, verbose);
    runCmpBoxGenericTests(context, verbose);
    runCmpErrorTests(context, verbose);
    runCmpAnyTests(context, verbose);
    runCmpThreadingTests(context, verbose);
    runOrderByTests(context, verbose);
    runOrderByErrorTests(context, verbose);
    runOrderAnyByTests(context, verbose);
    //
    if (! verbose) { debug(context, starbanner('** ' ~ SuiteTitle ~ ' Tests ran successfully **')); }
  } catch (err) {
    debug(context, starbanner('** Error in ' ~ SuiteTitle ~ ' Tests: ' ~ err ~ ' **'));
  }
});

// == [cmp / cmpTo: array] ==

export const CmpArrayDefaultCases = [
  [[[], []],                          0,  'two empty arrays are equal'],
  [[[1, 2, 3], [1, 2, 3]],            0,  'identical arrays are equal'],
  [[[1, 2, 3], [1, 2, 4]],           -1,  'first differing element decides; later elements are never examined'],
  [[[1, 2, 9], [1, 2, 3]],            1],
  [[[1, 2], [1, 2, 3]],              -1,  'a prefix is smaller than the longer array it is a prefix of -- it "ends first"'],
  [[[1, 2, 3], [1, 2]],               1],
  [[["a", "b"], ["a", "c"]],         -1,  'string elements compare through cmp too, exactly like top-level cmpTo(string,string)'],
  [[[[1, 2], [3]], [[1, 2], [4]]],   -1,  'nested arrays recurse through cmp element-by-element'],
];
export function runCmpArrayDefaultTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "cmp (array)", verbose, CmpArrayDefaultCases, function(args is array) { return cmp(args[0], args[1]); });
}

export const CmpToArrayComparatorCases = [
  [[[1, 2, 3], [1, 2, 3], cmp],                          0],
  [[[3, 1, 2], [3, 1, 2], (aa, bb) => -cmp(aa, bb)],      0,  'a reversing comparator still finds equal arrays equal'],
  [[[1, 2], [9, 9], function(aa, bb) { return 0; }],      0,  'a comparator that calls every element equal falls through to the length tiebreaker -- equal lengths tie'],
  [[[1, 2], [1, 2, 3], function(aa, bb) { return 0; }],  -1,  'same all-equal comparator, but now the shorter array loses the length tiebreaker'],
  [[[5, 1], [1, 5], (aa, bb) => cmp(bb, aa)],            -1,  'a reversing comparator can change which array reads as smaller'],
];
export function runCmpToArrayComparatorTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "cmpTo (array, comparator)", verbose, CmpToArrayComparatorCases, function(args is array) { return cmpTo(args[0], args[1], args[2]); });
}

// == [cmp / cmpTo: string] ==

export const CmpStringCases = [
  [["a", "a"],                0],
  [["a", "b"],               -1],
  [["b", "a"],                1],
  [["apple", "applesauce"],  -1,  'a prefix sorts before the longer string it is a prefix of'],
  [["applesauce", "apple"],   1],
  [["a", "b", cmp],          -1,  'a comparator argument is accepted but ignored for strings'],
];
export function runCmpStringTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "cmpTo (string)", verbose, CmpStringCases,
    function(args is array) { return (size(args) <= 2) ? cmpTo(args[0], args[1]) : cmpTo(args[0], args[1], args[2]); });
}

// == [cmp / cmpTo: number] ==

export const CmpNumberCases = [
  [[1, 1],       0],
  [[1, 2],      -1],
  [[2, 1],       1],
  [[-5, 3],     -1],
  [[0, -0],      0],
  [[3, 1, cmp],  1,  'a comparator argument is accepted but ignored for numbers'],
];
export function runCmpNumberTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "cmpTo (number)", verbose, CmpNumberCases,
    function(args is array) { return (size(args) <= 2) ? cmpTo(args[0], args[1]) : cmpTo(args[0], args[1], args[2]); });
}

// == [cmp / cmpTo: ValueWithUnits] ==

export const CmpValueWithUnitsCases = [
  [[1 * meter, 1 * meter],           0],
  [[1 * meter, 2 * meter],          -1],
  [[2 * meter, 1 * meter],           1],
  [[1 * meter, 1000 * millimeter],   0,  'compatible units convert before comparing'],
  [[1500 * millimeter, 1 * meter],   1,  '1.5m compares greater than 1m once converted to the same units'],
  [[1 * meter, 2 * meter, cmp],     -1,  'a comparator argument is accepted but ignored'],
];
export function runCmpValueWithUnitsTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "cmpTo (ValueWithUnits)", verbose, CmpValueWithUnitsCases,
    function(args is array) { return (size(args) <= 2) ? cmpTo(args[0], args[1]) : cmpTo(args[0], args[1], args[2]); });
}

// == [cmp / cmpTo: boolean] ==

export const CmpBooleanCases = [
  [[true, true],          0],
  [[false, false],        0],
  [[true, false],         1,  'true is greater than false'],
  [[false, true],        -1],
  [[true, false, cmp],    1,  'a comparator argument is accepted but ignored for booleans'],
];
export function runCmpBooleanTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "cmpTo (boolean)", verbose, CmpBooleanCases,
    function(args is array) { return (size(args) <= 2) ? cmpTo(args[0], args[1]) : cmpTo(args[0], args[1], args[2]); });
}

// == [cmp / cmpTo: undefined] ==

export const CmpUndefinedCases = [
  [[undefined, undefined],       0,  'undefined is equal to itself'],
  [[undefined, undefined, cmp],  0,  'a comparator argument is accepted but ignored for undefined'],
];
export function runCmpUndefinedTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "cmpTo (undefined)", verbose, CmpUndefinedCases,
    function(args is array) { return (size(args) <= 2) ? cmpTo(args[0], args[1]) : cmpTo(args[0], args[1], args[2]); });
}

// == [cmpMapByKeysOnly] ==

export const CmpMapByKeysOnlyCases = [
  [[{ "a": 1, "b": 2 }, { "a": 9, "b": 9 }],                        0,  'values are ignored entirely -- only the key lists matter'],
  [[{ "a": 1 },         { "b": 1 }],                               -1,  'key "a" sorts before key "b"'],
  [[{ "a": 1, "b": 1 }, { "a": 1 }],                                1,  'same shared key, but the map with the extra key is "longer" and so greater'],
  [[{ "a": 1 },         { "a": 1 }, (aa, bb) => -cmp(aa, bb)],   0,  'a comparator is honored for key comparison, though equal keys stay equal either way'],
];
export function runCmpMapByKeysOnlyTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "cmpMapByKeysOnly", verbose, CmpMapByKeysOnlyCases,
    function(args is array) { return (size(args) <= 2) ? cmpMapByKeysOnly(args[0], args[1]) : cmpMapByKeysOnly(args[0], args[1], args[2]); });
}

// == [cmpMap] ==

export const CmpMapCases = [
  [[{ "a": 1, "b": 2 }, { "a": 1, "b": 2 }],                        0,  'same keys, same values'],
  [[{ "a": 1, "b": 2 }, { "a": 1, "b": 3 }],                       -1,  'same keys; first differing value decides'],
  [[{ "a": 1, "b": 2 }, { "a": 9, "b": 0 }],                       -1,  'key "a" differs first and decides, even though "b" would go the other way'],
  [[{ "a": 1 },         { "b": 1 }],                               -1,  'differing keys decide the whole comparison; values are never even reached'],
  [[{ "a": 1 },         { "a": 2 }, (aa, bb) => -cmpTo(aa, bb)],    1,  'a comparator is threaded through both the key comparison and the value comparison'],
];
export function runCmpMapTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "cmpMap", verbose, CmpMapCases,
    function(args is array) { return (size(args) <= 2) ? cmpTo(args[0], args[1]) : cmpTo(args[0], args[1], args[2]); });
}

// == [cmp: map / box, per cmp's own doc block] ==

export const CmpMapGenericCases = [
  [[{ "a": 1, "b": 2 }, { "a": 1, "b": 2 }],   0,  'cmp on two maps compares keys then values, matching cmpMap'],
  [[{ "a": 1 },         { "b": 1 }],          -1,  'differing keys decide first'],
  [[{ "a": 1, "b": 2 }, { "a": 1, "b": 3 }],  -1,  'same keys; values decide'],
];
export function runCmpMapGenericTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "cmp (map, per doc)", verbose, CmpMapGenericCases, function(args is array) { return cmp(args[0], args[1]); });
}

export function runCmpBoxGenericTests(context is Context, verbose is boolean) returns map {
    const CmpBoxGenericCases = [
      [[new box(1), new box(2)],                  -1,  'cmp on two boxes compares their contents'],
      [[new box(5), new box(5)],                   0],
      [[new box(undefined), new box(undefined)],   0,  'two boxes holding undefined compare as equal'],
    ];
  return runTests(context, "cmp (box, per doc)", verbose, CmpBoxGenericCases, function(args is array) { return cmp(args[0], args[1]); });
}

// == [cmp / cmpTo: incompatible types are errors] ==

export const CmpErrorCases = [
  [[undefined,  5           ], "Execution error", 'undefined is equal only to itself -- comparing it with anything else is an error'],
  [[5,          undefined   ], "Execution error", 'same error with the arguments reversed'],
  [[true,       1           ], "Execution error", 'booleans cannot compare with anything else, even a numerically-equivalent number'],
  [[1 * meter,  1 * second  ], "Execution error", 'incompatible units cannot be compared'],
  [[1,          "1"         ], "Execution error", "comparing a number with a string is exactly the example cmp's own doc block gives for a method-not-found error"],
  [[[1, 2],     "12"        ], "Execution error", 'an array and a string have no obvious ordering between them either'],
];
export function runCmpErrorTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "cmp/cmpTo errors", verbose, CmpErrorCases, function(args is array) { return attempt(() => cmp(args[0], args[1])); });
}

// == [cmpAny] ==
// cmpAny gives a total ordering even across incompatible types, so unlike cmp it never errors --
// it falls back to fstypenum order. It is the same kind of end-user entry point as cmp is; cmpTo
// stays mostly behind the veil for both of them.


export function runCmpAnyTests(context is Context, verbose is boolean) returns map {
  const CmpAnyCases = [
  // same type: equal values short-circuit, otherwise delegates to cmpTo, exactly like cmp does
  [[5, 5],                    0,  'equal values short-circuit before any type dispatch happens'],
  [[1, 2],                   -1,  'same type: delegates to cmpTo, just like cmp'],
  [["a", "b"],               -1],
  [[true, false],             1],
  [[undefined, undefined],    0,  'equal (both undefined) short-circuits too, so this never reaches the "undefined errors" rule cmp has'],
  [[[1, 2], [1, 3]],         -1,  'arrays of the same type recurse through cmpTo(..., cmpAny), not cmpTo(..., cmp)'],
  // box transparency -- note the asymmetry: only the LEFT argument is unwrapped
  [[new box(1), new box(2)], -1,  'boxes are compared by their contents'],
  [[new box(5), new box(5)],  0],
  [[new box(5), 5],           0,  'a box on the left is transparently unwrapped and compares equal to the bare value'],
  [[5, new box(5)],          -1,  'unwrapping only checks the LEFT argument -- a box on the right is never unwrapped, so it reads as a different type (number sorts before box)'],
  // ValueWithUnits
  [[1 * meter, 1000 * millimeter],  0,  'compatible units compare by value once normalized to the same base unit'],
  [[1 * meter, 1 * second],        -1,  'incompatible units fall back to comparing the unit strings lexicographically -- "meter" < "second"'],
  // cross-type: never an error, always fstypenum order
  [[undefined, 5],           -1,  'unlike cmp, an incompatible-type comparison is never an error -- undefined sorts before number'],
  [[true, 5],                -1,  'boolean sorts before number'],
  [[5, "5"],                 -1,  'number sorts before string, even though the values print the same'],
  [[[1], { "a": 1 }],        -1,  'array sorts before map'],
];
  return runTests(context, "cmpAny", verbose, CmpAnyCases, function(args is array) { return cmpAny(args[0], args[1]); });
}

// == [comparator threading] ==
// cmp and cmpAny both work by threading themselves down as the comparator argument every time
// cmpTo descends into an array, map, or box (`cmpTo(aa, bb) { return cmpTo(aa, bb, cmp); }` and
// the cmpAny equivalent) -- cmp never re-invokes itself at a scalar leaf, only at each collection
// layer on the way down. That plumbing is shared by both, so proving it once here for a custom
// comparator is what keeps cmp and cmpAny consistent with each other as they descend.
//
// countingCmp below plays the role cmp/cmpAny play internally: it counts itself, then re-threads
// itself (not cmp) through cmpTo so every nested array/map/box comparison is also counted. Note
// the case data below is inert -- plain arrays, maps, and boxes -- and the counter, the comparator
// closure, and the actual cmpTo call are all built fresh inside the test runner, not while this
// array is being constructed.

function countingCmp(counterBox is box, aa, bb) returns number {
  counterBox[] = counterBox[] + 1;
  return cmpTo(aa, bb, (xx, yy) => countingCmp(counterBox, xx, yy));
}


export function runCmpThreadingTests(context is Context, verbose is boolean) returns map {
     const CmpThreadingCases = [
  [
    [
      // array of maps of maps of boxes of arrays of mixed scalar type
      [{ "j": { "k": new box([7, "dog", true]) } }, { "j": { "k": new box([10, "cat", false]) } }, 42],
      [{ "j": { "k": new box([7, "dog", true]) } }, { "j": { "k": new box([10, "cat", true]) } }, "forty-two"],
    ],
    [-1, 16],
    'index 0 is a fully-equal deep structure (8 calls: outer map pair, key "j", inner map pair, '
    ~ 'key "k", box pair, and all 3 leaf scalars -- every layer is visited because nothing differs '
    ~ 'until the very last leaf, so nothing short-circuits). Index 1 is the same shape but differs '
    ~ 'in its last leaf (also 8 calls, same reasoning, result -1). Index 2 is a number vs. a string '
    ~ '-- comparing them directly would throw -- but the array overload already returned -1 at '
    ~ 'index 1, so index 2 is never reached: 8 + 8 + 0 = 16 calls total, and no throw',
  ],
  [
    [
      { "a": 1, "b": "mismatched-value" },
      { "a": 99, "c": [1, 2, 3] },
    ],
    [-1, 2],
    'keys ["a","b"] vs ["a","c"] differ at "b"/"c", so cmpTo(map,map) returns off the key '
    ~ 'comparison alone -- only 2 comparator calls (the two key pairs). The values are '
    ~ 'deliberately type-mismatched (a string vs. an array) to prove they are never reached: '
    ~ 'comparing them would throw, and cmpMapByKeysOnly short-circuits before that can happen',
  ],
];
  return runTests(context, "comparator threading", verbose, CmpThreadingCases, function(args is array) {
    const counter = new box(0);
    const result = cmpTo(args[0], args[1], (aa, bb) => countingCmp(counter, aa, bb));
    return [result, counter[]];
  });
}

// == [orderBy] ==
// orderBy(vals, iterateeSpec?, order?, comparator?) sorts an array's elements, or a map's values
// (keys discarded), by iterateeSpec's result for each -- defaulting to identity, ascending, and
// cmp. Every arity below the full 4 is a plain defaulting wrapper, so exercising each arity once
// is exercising the whole chain; the bulk of these cases instead probe iterateeSpec's three forms
// (function/string/map), the order sign (including the order=0 neuter case), stability of ties,
// and that the comparator is handed iterateeSpec's *results*, not the original elements.

export const OrderByCases = [
  [[[3, 1, 2]],                                   [1, 2, 3],  'default: ascending, by identity, via cmp -- the 1-arg form'],
  [[[3, 1, 2], identity, -1],                     [3, 2, 1],  'order = -1 sorts descending'],
  [[[3, 1, 2], identity, 0],                      [3, 1, 2],  'order = 0 is a neuter pass-through -- stable, so the input comes back unchanged'],
  [[[10, 5, 20], (val, _seq) => -val],            [20, 10, 5], 'function iterateeSpec: ascending by -val sorts descending by val -- the 2-arg form'],
  [
    [[{ "n": 3 }, { "n": 1 }, { "n": 2 }], "n"],
    [{ "n": 1 }, { "n": 2 }, { "n": 3 }],
    'string iterateeSpec is a property accessor',
  ],
  [
    [[{ "a": 1, "b": "X" }, { "a": 9, "b": "Y" }, { "a": 1, "b": "Z" }], { "a": 1 }],
    [{ "a": 9, "b": "Y" }, { "a": 1, "b": "X" }, { "a": 1, "b": "Z" }],
    'map iterateeSpec is a matches predicate; false sorts before true; elements with equal criteria keep their original relative order (stability)',
  ],
  [
    [[{ "n": 3 }, { "n": 1 }, { "n": 2 }], "n", 1, (aa, bb) => aa - bb],
    [{ "n": 1 }, { "n": 2 }, { "n": 3 }],
    'the 4-arg form: a custom comparator receives the already-extracted numeric criteria, not the original maps -- subtracting a map would throw',
  ],
  [[{ "z": 3, "a": 1, "m": 2 }],                  [1, 2, 3],  'a map is sorted by its values; keys are discarded'],
  [
    [{ "aaa": 1, "b": 2, "c": 3 }, (val, key) => length(key)],
    [2, 3, 1],
    'the map form hands the iteratee (val, key), not (val, seq) -- sorting by key length puts "b" and "c" (length 1) '
    ~ 'before "aaa" (length 3); if a numeric position were passed instead of the string key, length(...) would throw',
  ],
  [[[3 * meter, 1 * meter, 2 * meter]],           [1 * meter, 2 * meter, 3 * meter], 'ValueWithUnits sort via the default cmp comparator'],
  [[[]],                                          [],         'empty array sorts to an empty array'],
  [[{}],                                          [],         'empty map sorts to an empty array'],
];
export function runOrderByTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "orderBy", verbose, OrderByCases, function(args is array) {
    if (size(args) == 1) { return orderBy(args[0]); }
    if (size(args) == 2) { return orderBy(args[0], args[1]); }
    if (size(args) == 3) { return orderBy(args[0], args[1], args[2]); }
    return orderBy(args[0], args[1], args[2], args[3]);
  });
}

export const OrderByErrorCases = [
  [[[1, "2", 3]], "Execution error", 'the default comparator is cmp, so mixed number/string criteria throws exactly as cmp does; see orderAnyBy for a version that never throws'],
];
export function runOrderByErrorTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "orderBy errors", verbose, OrderByErrorCases, function(args is array) { return attempt(() => orderBy(args[0])); });
}

// == [orderAnyBy] ==
// orderBy with cmpAny as the comparator -- same iterateeSpec/order handling, but never throws on
// mixed-type criteria, falling back to fstypenum order instead.

export const OrderAnyByCases = [
  [[[1, "2", 0]],              [0, 1, "2"], 'never throws on mixed types -- falls back to fstypenum order, matching cmpAny (number sorts before string)'],
  [[[3, 1, 2]],                [1, 2, 3],   'same-type values still sort ascending, same as orderBy'],
  [[[3, 1, 2], identity, -1],  [3, 2, 1],   'order still controls direction'],
  [[{ "a": "x", "b": 1 }],     [1, "x"],    'map values sorted via cmpAny, discarding keys -- number sorts before string'],
];
export function runOrderAnyByTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "orderAnyBy", verbose, OrderAnyByCases, function(args is array) {
    if (size(args) == 1) { return orderAnyBy(args[0]); }
    if (size(args) == 2) { return orderAnyBy(args[0], args[1]); }
    return orderAnyBy(args[0], args[1], args[2]);
  });
}
