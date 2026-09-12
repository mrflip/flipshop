FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");

import(path : "4ebdc64943b566160ea5cc28", version : "3684960ff67d0db556614489");
import(path : "f6ab954150609e1e2e014a1e", version : "cefd17945c168add0f500dad");
import(path : "fb2ebd26998aed1d4a76e115", version : "61f6dfc83f92e38721d9a67d");
// Testing this:
import(path : "9263ab535d0213ef0f0e9d5e", version : "4acbb4ada80b5866d2b009b9");
import(path : "607f97fc690581579d1d4a08", version : "6afa9ed6ec4a413d9bf06340");

/**
 * Each case is one scenario, not one more example of a scenario already covered.
 */
export const UndotMapCases = [
  [[{}], {}, 'empty map: returns an empty map'],
  [[{ "foo": 1 }], { "foo": 1 }, 'key with no dots: passed through untouched'],
  [[{ "": 1 }], { "": 1 }, 'empty key holds no dot, so it is a literal key, not an empty segment'],
  [[{ "foo.bar": 1 }], { "foo": { "bar": 1 } }, 'one dot: one level of nesting'],
  [[{ "a.b.c": 3 }], { "a": { "b": { "c": 3 } } }, 'several dots: nests all the way down'],
  [[{ "foo.bar": 1, "foo.baz": 2 }],
   { "foo": { "bar": 1, "baz": 2 } },
   'two keys under one prefix: siblings merge under a shared parent'],
  [[{ "foo": {}, "foo.bar": 1 }],
   { "foo": { "bar": 1 } },
   'empty map and a dotted child: the placeholder merges away'],
  [[{ "a.b.c": 1, "a": {} }],
   { "a": { "b": { "c": 1 } } },
   'empty map arriving over a populated branch: shallowest first, so it never clobbers'],
  [[{ "foo.bar": 1, "foo": { "baz": { "c": 3 } } }],
   { "foo": { "bar": 1, "baz": { "c": 3 } } },
   'dotted key and a nested literal at the same prefix: merged, not replaced'],
  [[{ "a": { "b": { "x": 1 } }, "a.b": { "y": 2 } }],
   { "a": { "b": { "x": 1, "y": 2 } } },
   'merge recurses past the end of the dotted key into the literal below it'],
  [[{ "a.b": { "x": 1 }, "a.b.y": 2 }],
   { "a": { "b": { "x": 1, "y": 2 } } },
   'deeper key extends a map a shallower key already placed'],
  [[{ "a": { "b": 1 }, "a.b": 1 }],
   { "a": { "b": 1 } },
   'same leaf by two routes with the same value: idempotent, no complaint'],
  [[{ "a.b.c": 3, "a.b.d": 4, "a.e": 5, "f": 6 }],
   { "a": { "b": { "c": 3, "d": 4 }, "e": 5 }, "f": 6 },
   'branches of four different depths all land in one tree'],
  [[{ "socket_bit.inthex": { "H2.5mm": 1 } }],
   { "socket_bit": { "inthex": { "H2.5mm": 1 } } },
   'dots inside a value are left alone: only the top level is split'],
  [[{ "foo.bar": ['and', 'how'] }],
   { "foo": { "bar": ['and', 'how'] } },
   'array value is a leaf, never descended'],
  [[{ "yup": true, "nope": undefined }],
   { "yup": true, "nope": undefined },
   'undefined value: same on both sides whether FeatureScript stores the key or elides it'],
  [[{ "Text Key.Is it.OK?: YEP!": 3 }],
   { "Text Key": { "Is it": { "OK?: YEP!": 3 } } },
   'only dots split: spaces, colons and question marks are ordinary characters'],
  [[{ "L'Iñtërnâtiôñàlizætiøñ.𝍔.c'est très.difficile": ['and', 'how'] }],
   { "L'Iñtërnâtiôñàlizætiøñ": { "𝍔": { "c'est très": { "difficile": ['and', 'how'] } } } },
   'non-ASCII segments, astral plane included, are just segments'],
  [[SocketWrenchesDotted],
   { "socket_bit": SocketWrenches["socket_bit"] },
   'real data: dotted and nested keys at five different depths merge into one tree'],
];

export const UndotMapThrows = [
  [[{ "foo": 1, "foo.bar": 2 }],
   'Key "foo" is not a map but key "foo.bar" wants it to be',
   'a scalar blocks a deeper key'],
  [[{ "foo": [], "foo.bar": 2 }],
   'Key "foo" is not a map but key "foo.bar" wants it to be',
   'an array is no more a map than a scalar is'],
  [[{ "a.b.c": 1, "a.b": 2 }],
   'Key "a.b" is not a map but key "a.b.c" wants it to be',
   'shallowest first: the shallow key lands, the deep one hits it, and the complaint is the same either way the map iterates'],
  [[{ "a": { "b": 1 }, "a.b": { "c": 2 } }],
   'Key "a.b" and another key disagree about "a.b": one wants a map, the other does not',
   'a nested literal put a scalar where a dotted key wants a map'],
  [[{ "a": { "b": { "c": 1 } }, "a.b": { "c": 2 } }],
   'Key "a.b" and another key disagree about what belongs at "a.b.c"',
   'the merge runs deeper than the key, so the path in the complaint is deeper too'],
  [[{ "foo.": 1 }], 'Key "foo." has an empty path segment', 'trailing dot is ambiguous, not tolerated'],
  [[{ ".foo": 1 }], 'Key ".foo" has an empty path segment', 'leading dot, same'],
  [[{ "a..b": 1 }], 'Key "a..b" has an empty path segment', 'doubled dot, same'],
];

export const DotMapCases = [
  [[{}], {}, 'empty map: returns an empty map'],
  [[{ "foo": 1 }], { "foo": 1 }, 'flat map: nothing to dot'],
  [[{ "foo": { "bar": 1 } }], { "foo.bar": 1 }, 'one level'],
  [[{ "a": { "b": { "c": 1 } } }], { "a.b.c": 1 }, 'no options: flattens all the way down'],
  [[{ "a": { "b": { "c": 1 } } }, { "maxDepth": 1 }],
   { "a.b": { "c": 1 } },
   'maxDepth 1: one dot gained, the rest left nested'],
  [[{ "a": { "b": { "c": 1 } } }, { "maxDepth": 0 }],
   { "a": { "b": { "c": 1 } } },
   'maxDepth 0: no-op'],
  [[{ "a": { "b": { "c": 1 } } }, { "maxDepth": -3 }],
   { "a": { "b": { "c": 1 } } },
   'negative maxDepth behaves as 0, the way lodash clamps flattenDepth'],
  [[{ "a": { "b": { "c": 1 } } }, { "maxDepth": 99 }],
   { "a.b.c": 1 },
   'maxDepth deeper than the map: stops when it runs out of map'],
  [[{ "a": { "b": { "c": 1 } } }, { "maxDepth": 1, "notAnOption": true }],
   { "a.b": { "c": 1 } },
   'unknown option keys are ignored rather than refused'],
  [[{ "shallow": { "x": 1 }, "deep": { "a": { "b": { "c": 2 } } } }, { "maxDepth": 2 }],
   { "shallow.x": 1, "deep.a.b": { "c": 2 } },
   'ragged structure: each branch collapses as far as it can, then stops'],
  [[{ "a": { "b": [1, { "c": 2 }] } }],
   { "a.b": [1, { "c": 2 }] },
   'arrays are leaves even when they contain maps'],
  [[{ "a": {}, "b": { "c": 1 } }],
   { "a": {}, "b.c": 1 },
   'empty map is a leaf: no inner key to dot with, and dropping the key would lose it'],
  [[{ "2.1mm": { "deep": 3 } }],
   { "2.1mm.deep": 3 },
   'a key that already has a dot is dotted anyway; this is the one dotMap does not care about'],
  [[{ "Special Chars": { "bar": 1 }, "baz": { "quux": 2 } }],
   { "Special Chars.bar": 1, "baz.quux": 2 },
   'spaces in keys survive the join'],
];

const runUndotMapTest = ((args is array) => undotMap(args[0]));
const runDotMapTest   = ((args is array) => ((size(args) <= 1) ? dotMap(args[0]) : dotMap(args[0], args[1])));

export function runUndotMapTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "undotMap", verbose, UndotMapCases, runUndotMapTest);
}

export function runUndotMapThrowsTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "undotMap throws", verbose, UndotMapThrows, assertThrows(runUndotMapTest));
}

export function runDotMapTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "dotMap", verbose, DotMapCases, runDotMapTest);
}

/**
 * The round trip is a property rather than a case list, and the second and third cases are the
 * boundary of it: your size keys hold dots of their own, so dotting down to them is lossy and
 * undotMap has no way to know it.
 */
export function runRoundTripTests(context is Context, verbose is boolean) returns map {
  const dotFree = {
    "socket_bit": {
      "inthex": { "metric": { "isq_0250in": { "H2mm": { "wt": 1 } } } },
    },
  };
  const cases = [
    [[dotMap(dotFree)],
      dotFree,
      'keys without dots of their own survive the round trip'],
    [[dotMap(SocketWrenches, { "maxDepth": 3 })],
      SocketWrenches,
      'real data round-trips while the dotting stops above the size level'],
    [[dotMap({ "isq_0250in": { "H2.5mm": { "wt": 1 } } }, { "maxDepth": 2 })],
     { "isq_0250in": { "H2": { "5mm": { "wt": 1 } } } },
     'one level further and the size key splits at its own dot: lossy, and silently so'],
  ];
  return runTests(context, "undotMap(dotMap(...))", verbose, cases,
                  (args is array) => undotMap(args[0]));
}

const miniTree = {
    "socket_bit": {
        "inthex": {
            "isq_0250in": {
                "H2mm": { "wt": 1 }
            }
        }
    },
    "socket_exthex": {
        "exthex": {
            "isq_0250in": {
                "4mm": { "wt": 2 }
            }
        }
    }
};

const miniLevels = [
    ['socket_kind', { socket_bit: 'Bit Socket', socket_exthex: 'Bolt Socket' }],
    ['drive_kind', { inthex: 'Int Hex', exthex: '6-Point' }],
    ['sqdrive_size', 'Square Drive Size', { isq_0250in: '1/4Dr' }],
    'sizing'
];

export const BuildNestedChoicesCases = [
    [
        [ ['drive_kind', 'sizing'], { "inthex": { "H2mm": 1 }, "exthex": { "4mm": 2 } } ],
        {
            "Inthex": { "name": "sizing", "displayName": "Sizing", "entries": { "H2mm": 1 } },
            "Exthex": { "name": "sizing", "displayName": "Sizing", "entries": { "4mm": 2 } }
        },
        'minimal structure: uses titleCase for display names when no translation map is provided'
    ],
    [
        [
            [['socket_kind', { socket_bit: 'Bit Socket' }], 'drive_kind'],
            { "socket_bit": { "inthex": 1 } }
        ],
        {
            "Bit Socket": { "name": "drive_kind", "displayName": "Drive Kind", "entries": { "inthex": 1 } }
        },
        'translation map: replaces top-level keys using the provided dictionary and formats the next level'
    ],
    [
        [ miniLevels, miniTree ],
        {
            "Bit Socket": { "name": "drive_kind", "displayName": "Drive Kind", "entries": {
                "Int Hex": { "name": "sqdrive_size", "displayName": "Square Drive Size", "entries": {
                    "1/4Dr": { "name": "sizing", "displayName": "Sizing", "entries": {
                        "H2mm": { "wt": 1 }
                    }}
                }}
            }},
            "Bolt Socket": { "name": "drive_kind", "displayName": "Drive Kind", "entries": {
                "6-Point": { "name": "sqdrive_size", "displayName": "Square Drive Size", "entries": {
                    "1/4Dr": { "name": "sizing", "displayName": "Sizing", "entries": {
                        "4mm": { "wt": 2 }
                    }}
                }}
            }}
        },
        'full depth: recursively processes uniform tree data into the strict custom feature choice structure'
    ],
    [
        [ ['sizing'], { "H2mm": { "wt": 1 } } ],
        { "H2mm": { "wt": 1 } },
        'base case: returns the tree unchanged if less than two levels are provided'
    ]
];

const runBuildNestedChoicesTest = ((args is array) => buildNestedChoices(args[0], args[1]));

export function runBuildNestedChoicesTests(context is Context, verbose is boolean) returns map {
    return runTests(context, "buildNestedChoices", verbose, BuildNestedChoicesCases, runBuildNestedChoicesTest);
}
