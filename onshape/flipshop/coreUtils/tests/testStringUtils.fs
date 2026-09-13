FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");

import(path : "4ebdc64943b566160ea5cc28", version : "3684960ff67d0db556614489");
import(path : "f6ab954150609e1e2e014a1e", version : "cefd17945c168add0f500dad");
import(path : "fb2ebd26998aed1d4a76e115", version : "61f6dfc83f92e38721d9a67d");
// Testing this:
import(path : "9263ab535d0213ef0f0e9d5e", version : "4acbb4ada80b5866d2b009b9");
import(path : "607f97fc690581579d1d4a08", version : "6afa9ed6ec4a413d9bf06340");

const SuiteTitle = "String Utils";

annotation { "Feature Type Name": 'Run ' ~ SuiteTitle ~ ' Tests', "Feature Type Description": "Internal Tests" }
export const runStringUtilsTestsFS = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Verbose", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE, "Default": false }
  definition.verbose is boolean;
}
{
  const verbose = ifNil(definition.verbose, false);
  try {
    runStrSliceTests(context, verbose);
    runStrTakeTests(context, verbose);
    runStrTakeRightTests(context, verbose);
    //
    runTitleCaseTests(context, verbose);
    runPadTests(context, verbose);
    runStrRepeatTests(context, verbose);
    runUpcaseTests(context, verbose);
    runDowncaseTests(context, verbose);
    runHasMatchTests(context, verbose);
    runStarbannerTests(context, verbose);
    //
    if (! verbose) { debug(context, starbanner('** ' ~ SuiteTitle ~ ' Tests ran successfully **')); }
  } catch (err) {
    debug(context, starbanner('** Error in ' ~ SuiteTitle ~ ' Tests: ' ~ err ~ ' **'));
    return;
  }
});

// == [Title Case Tests]

const runTitleCaseTest = ((args is array) => ((size(args) <= 1) ? titleCase(args[0]) : titleCase(args[0], args[1])));

export const TitleCaseCases = [
    [["socket_kind"], "Socket Kind", 'default spaces: converts underscore to space and capitalizes'],
    [["drive-kind"], "Drive Kind", 'default spaces: converts hyphen to space and capitalizes'],
    [["mixed_case-string"], "Mixed Case String", 'default spaces: handles multiple delimiters in one string'],
    [["already_Title_Case"], "Already Title Case", 'maintains capitalization if already title cased or mixed'],
    [["hello.world", { "spaces": "." }], "Hello World", 'custom spaces: uses provided space delimiters'],
    [["a_b_c", { "tr": { "a": "X", "b": "Y" } }], "X Y C", 'translation map: replaces characters literally without capitalizing them further'],
    [[""], "", 'empty string returns empty string'],
];
export function runTitleCaseTests(context is Context, verbose is boolean) returns map {
    return runTests(context, "titleCase", verbose, TitleCaseCases, runTitleCaseTest);
}
// --

// == [Pad Tests]

const PadTestCases = [
  [["", 0],                       "",                    'empty string, length: 0, default padding: returns input'],
  [["", 0, " "],                  "",                    'empty string, length 0, space padding explicit: returns input'],
  [["hello world", 0],            "hello world",         'string, length: 0, default padding: returns input'],
  [["", -5],                      "",                    'empty string, negative length: returns input'],
  [["", -5, " "],                 "",                    'empty string, negative length, space padding explicit: returns input'],
  [["hello world", -5],           "hello world",         'string, negative length: returns input (not truncated)'],
  [["", 3],                       "   ",                 'empty string, length 3, space padding implicit: 3 spaces'],
  [["", 3, " "],                  "   ",                 'empty string, length 3, space padding explicit: 3 spaces'],
  [["", 3, "  "],                 "   ",                 'empty string, length 3, 2 spaces padding explicit: 3 spaces (not four)'],
  [["", 3, "1234"],               "123",                 'empty string, length 3, 4 digits padding explicit: 3 digits (not four)'],
  [["hello world", 11],           "hello world",         'string, length less than its own: returns input'],
  [["hello world", 11],           "hello world",         'string, length equal to its own: returns input'],
  [["hello world", 11, "!"],      "hello world",         'string, length equal to its own, with pad string: returns input'],
];
const LeftPadTestCases = [
  [["hello world", 12],           " hello world",        'string, length one more than its own, default padding: adds one space'],
  [["hello world", 12, "!"],      "!hello world",        'string, length one more than its own, with one pad character: adds one character of padding'],
  [["hello world", 12, "!!!"],    "!hello world",        'string, length one more than its own, with three pad characters: adds one character of padding'],
  [["hello world", 13, "!!!"],    "!!hello world",       'string, length two more than its own, with three pad characters: adds two characters of padding'],
  [["hello world", 14, "!!!"],    "!!!hello world",      'string, length three more than its own, with three pad characters: adds three characters of padding'],
  [["hello world", 15, "!!!"],    "!!!!hello world",     'string, length four more than its own, with three pad characters: adds four characters of padding'],
  [["hello world", 16, "!!!"],    "!!!!!hello world",    'string, length five more than its own, with three pad characters: adds five characters of padding'],
  [["hello world", 17, "!!!"],    "!!!!!!hello world",   'string, length six more than its own, with three pad characters: adds six characters of padding'],
  [["hello world", 18, "!!!"],    "!!!!!!!hello world",  'string, length seven more than its own, with three pad characters: adds seven characters of padding'],
  [["hey world!!", 11, "!!!"],    "!!hey world",         'string, length same as its own, shares characters with padding: returns input'],
  [["hey world!!", 14, "!!!"],    "!!!hey world",        'string, length one more than its own, shares characters with padding: adds one character of padding'],
  [["hey world!!", 18, "!!!"],    "!!!!!!!hey world",    'string, length five more than its own, with three pad characters: adds five characters of padding'],
];
const RightPadTestCases = [
  [["hello world",  11, "!!!"],    "hello world!",       'string, length same as its own, shares characters with padding: returns input'],
  [["hello world",  12],           "hello world ",       'string, length one more than its own, default padding: adds one space'],
  [["hello world",  12, "!"],      "hello world!",       'string, length one more than its own, with one pad character: adds one character of padding'],
  [["hello world",  12, "!!!"],    "hello world!",       'string, length one more than its own, with three pad characters: adds one character of padding'],
  [["hello world",  13, "!!!"],    "hello world!!",      'string, length two more than its own, with three pad characters: adds two characters of padding'],
  [["hello world",  14, "!!!"],    "hello world!!!",     'string, length three more than its own, with three pad characters: adds three characters of padding'],
  [["hello world",  15, "!!!"],    "hello world!!!!",    'string, length four more than its own, with three pad characters: adds four characters of padding'],
  [["hello world",  16, "!!!"],    "hello world!!!!!",   'string, length five more than its own, with three pad characters: adds five characters of padding'],
  [["hello world",  17, "!!!"],    "hello world!!!!!!",  'string, length six more than its own, with three pad characters: adds six characters of padding'],
  [["hello world",  18, "!!!"],    "hello world!!!!!!!", 'string, length seven more than its own, with three pad characters: adds seven characters of padding'],
  [["hey world!!",  11, "!!!"],    "hey world!!",        'string, length same as its own, shares characters with padding: returns input'],
  [["hey world!!",  14, "!!!"],    "hey world!!!",       'string, length one more than its own, shares characters with padding: adds one character of padding'],
  [["hey world!!",  18, "!!!"],    "hey world!!!!!!!",   'string, length five more than its own, with three pad characters: adds five characters of padding'],
];

export function runPadTests(context is Context, verbose is boolean) returns map {
  runTests(context, "padLeft (trivial cases)",  verbose, PadTestCases,      function(args is array) { return size(args) <= 2 ? padLeft(args[0],  args[1]) : padLeft(args[0],  args[1], args[2]); });
  runTests(context, "padRight (trivial cases)", verbose, PadTestCases,      function(args is array) { return size(args) <= 2 ? padRight(args[0], args[1]) : padRight(args[0], args[1], args[2]); });
  runTests(context, "padLeft (padding cases)",  verbose, LeftPadTestCases,  function(args is array) { return size(args) <= 2 ? padLeft(args[0],  args[1]) : padLeft(args[0],  args[1], args[2]); });
  return runTests(context, "padRight (padding cases)", verbose, RightPadTestCases, function(args is array) { return size(args) <= 2 ? padRight(args[0], args[1]) : padRight(args[0], args[1], args[2]); });
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

export const StrRepeatCases = [
  [["*", 3],   "***"],
  [["abc", 2], "abcabc"],
  [["abc", 0], ""],
];
export function runStrRepeatTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "strRepeat", verbose, StrRepeatCases, function(args is array) { return strRepeat(args[0], args[1]); });
}

export const UpcaseCases = [
  [["fooBar"],       "FOOBAR"],
  [["--foo-bar--"],  "--FOO-BAR--"],
];
export const DowncaseCases = [
  [["fooBar"],       "foobar"],
  [["--FOO-BAR--"],  "--foo-bar--"],
];
export function runUpcaseTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "upcase", verbose, UpcaseCases, function(args is array) { return upcase(args[0]); });
}
export function runDowncaseTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "downcase", verbose, DowncaseCases, function(args is array) { return downcase(args[0]); });
}

export const StarbannerCases = [
  [["hi"], "\n**\nhi\n**\n\n"],
];
export function runStarbannerTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "starbanner", verbose, StarbannerCases, function(args is array) { return starbanner(args[0]); });
}

export const HasMatchCases = [
  [["hello", "ell"], true],
  [["hello", "^h"],  true],
  [["hello", "^e"],  false],
  [[undefined, "x"], false, 'undefined input never throws'],
];
export function runHasMatchTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "hasMatch", verbose, HasMatchCases, function(args is array) { return hasMatch(args[0], args[1]); });
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

// --
