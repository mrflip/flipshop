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
    runWordsTests(context, verbose);
    runCamelCaseTests(context, verbose);
    runCapitalizeTests(context, verbose);
    runEscapeRegExpTests(context, verbose);
    runKebabCaseTests(context, verbose);
    runLowerCaseTests(context, verbose);
    runLowerFirstTests(context, verbose);
    runPadBothTests(context, verbose);
    runSnakeCaseTests(context, verbose);
    runTrimStartTests(context, verbose);
    runTrimEndTests(context, verbose);
    runTrimTests(context, verbose);
    runTruncateTests(context, verbose);
    runUpperCaseTests(context, verbose);
    runUpperFirstTests(context, verbose);
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
function runTitleCaseTests(context is Context, verbose is boolean) returns map {
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
  [["!hey world!", 11, "!!!"],    "!hey world!",         'string, length same as its own, shares characters with padding: returns input'],
  [["!hey world!", 14, "!!!"],    "!!!!hey world!",      'string, length one more than its own, shares characters with padding: adds one character of padding'],
  [["!hey world!", 18, "!!!"],    "!!!!!!!!hey world!",  'string, length five more than its own, with three pad characters: adds five characters of padding'],
  [["", 3, "1234"],               "123",                 'empty string, length 3, 4 digits padding explicit: 3 digits (not four)'],
];
const RightPadTestCases = [
  [["hello world",  11, "!!!"],    "hello world",        'string, length same as its own, shares characters with padding: returns input'],
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
  [["hey world!!",  14, "!!!"],    "hey world!!!!!",     'string, length one more than its own, shares characters with padding: adds one character of padding'],
  [["hey world!!",  18, "!!!"],    "hey world!!!!!!!!!", 'string, length five more than its own, with three pad characters: adds five characters of padding'],
  [["", 3, "1234"],                "234",                'empty string, length 3, 4 digits padding explicit: 3 digits (not four)'],
];

function runPadTests(context is Context, verbose is boolean) returns map {
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
function runStrRepeatTests(context is Context, verbose is boolean) returns map {
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
function runUpcaseTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "upcase", verbose, UpcaseCases, function(args is array) { return upcase(args[0]); });
}
function runDowncaseTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "downcase", verbose, DowncaseCases, function(args is array) { return downcase(args[0]); });
}

export const StarbannerCases = [
  [["hi"], "\n**\nhi\n**\n\n"],
];
function runStarbannerTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "starbanner", verbose, StarbannerCases, function(args is array) { return starbanner(args[0]); });
}

export const HasMatchCases = [
  [["hello", "ell"],     false,   '!!! matches are full string, always'],
  [["hello", "ell.*"],   false,   '!!! matches are full string, always'],
  [["hello", ".*ell"],   false,   '!!! matches are full string, always'],
  [["hello", ".*ell.*"], true,    '!!! matches are full string, always'],
  [["hello", "^h"],      false,    '!!! matches are full string, always'],
  [["hello", "^h....$"], true,    '!!! matches are full string, always'],
  [["hello", "^h.*"],    true,    '!!! matches are full string, always'],
  [["hello", "^e"],      false],
  [[undefined, "x"],     false, 'undefined input never throws'],
];
function runHasMatchTests(context is Context, verbose is boolean) returns map {
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

// == [Lodash String ports] ==

export const WordsCases = [
  [["foo-bar_baz qux"], ["foo", "bar", "baz", "qux"]],
  [["  foo  "],          ["foo"]],
  [[""],                 []],
];
function runWordsTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "words", verbose, WordsCases, function(args is array) { return words(args[0]); });
}

export const CamelCaseCases = [
  [["Foo Bar"], "fooBar"],
  [["foo-bar"], "fooBar"],
  [["foo_bar"], "fooBar"],
];
function runCamelCaseTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "camelCase", verbose, CamelCaseCases, function(args is array) { return camelCase(args[0]); });
}

export const CapitalizeCases = [
  [["FRED"], "Fred"],
  [["fred"], "Fred"],
  [[""],     ""],
];
function runCapitalizeTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "capitalize", verbose, CapitalizeCases, function(args is array) { return capitalize(args[0]); });
}

export const EscapeRegExpCases = [
  [["[lodash](https://lodash.com/)"], "\\[lodash\\]\\(https://lodash\\.com/\\)"],
  [["fred"],                          "fred", 'no metacharacters, nothing to escape'],
  [[""],                              ""],
];
function runEscapeRegExpTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "escapeRegExp", verbose, EscapeRegExpCases, function(args is array) { return escapeRegExp(args[0]); });
}

export const KebabCaseCases = [
  [["Foo Bar"], "foo-bar"],
  [["fooBar"],  "foobar", 'no camelCase-boundary splitting, unlike lodash'],
];
function runKebabCaseTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "kebabCase", verbose, KebabCaseCases, function(args is array) { return kebabCase(args[0]); });
}

export const LowerCaseCases = [
  [["Foo Bar"], "foo bar"],
  [["foo-bar"], "foo bar"],
];
function runLowerCaseTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "lowerCase", verbose, LowerCaseCases, function(args is array) { return lowerCase(args[0]); });
}

export const LowerFirstCases = [
  [["Fred"], "fred"],
];
function runLowerFirstTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "lowerFirst", verbose, LowerFirstCases, function(args is array) { return lowerFirst(args[0]); });
}

export const PadBothCases = [
  [["hi", 6],      "  hi  "],
  [["hi", 5],      " hi  "],
  [["hi", 1],      "hi",     'minlen shorter than str returns str unchanged'],
];
function runPadBothTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "pad", verbose, PadBothCases, function(args is array) { return pad(args[0], args[1]); });
}

export const SnakeCaseCases = [
  [["Foo Bar"], "foo_bar"],
  [["foo-bar"], "foo_bar"],
];
function runSnakeCaseTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "snakeCase", verbose, SnakeCaseCases, function(args is array) { return snakeCase(args[0]); });
}

export const TrimStartCases = [
  [[""],                          ""],
  [["  "],                        ""],
  [["hi"],                        "hi"],
  [["  hi"],                      "hi"],
  [["\n\r\t "],               ""],
  [["\n\r\t x\n\r\t "],   "x\n\r\t "],
  [["--hi--", ["-"]],    "hi--"],
];
function runTrimStartTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "trimStart", verbose, TrimStartCases, function(args is array) {
    return (size(args) <= 1) ? trimStart(args[0]) : trimStart(args[0], args[1]);
  });
}

export const TrimEndCases = [
  [["  hi  "],           "  hi"],
  [["--hi--", ["-"]],    "--hi"],
];
function runTrimEndTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "trimEnd", verbose, TrimEndCases, function(args is array) {
    return (size(args) <= 1) ? trimEnd(args[0]) : trimEnd(args[0], args[1]);
  });
}

export const TrimCases = [
  [["  hi  "],           "hi"],
  [["--hi--", ["-"]],    "hi"],
];
function runTrimTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "trim", verbose, TrimCases, function(args is array) {
    return (size(args) <= 1) ? trim(args[0]) : trim(args[0], args[1]);
  });
}

export const TruncateCases = [
  [["hello world", { "length": 8 }], "hello..."],
  [["hi"],                           "hi",  'shorter than the default length returns str unchanged'],
];
function runTruncateTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "truncate", verbose, TruncateCases, function(args is array) {
    return (size(args) <= 1) ? truncate(args[0]) : truncate(args[0], args[1]);
  });
}

export const UpperCaseCases = [
  [["foo-bar"], "FOO BAR"],
];
function runUpperCaseTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "upperCase", verbose, UpperCaseCases, function(args is array) { return upperCase(args[0]); });
}

export const UpperFirstCases = [
  [["fred"], "Fred"],
];
function runUpperFirstTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "upperFirst", verbose, UpperFirstCases, function(args is array) { return upperFirst(args[0]); });
}

// --
