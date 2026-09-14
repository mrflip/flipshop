FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
//
import(path : "f6ab954150609e1e2e014a1e", version : "d514ce6f977d6c411ee07659"); // runTests
// Testing this:
import(path : "dcee57677ef58f0c8e045269", version : "95fce9e9c733cad8fcc1da28");
import(path : "54590bc1c9cee0141b968fbb", version : "9353a2660fe770844332b092"); // clxnUtils: flatten
import(path : "08b6ba15b8255611bafe7520", version : "e13611047ebf5355c1139c85");


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
    runRegexEngineTests(context, verbose);
    runUsefulRETests(context, verbose);
    if (! verbose) { debug(context, starbanner('** ' ~ SuiteTitle ~ ' Tests ran successfully **')); }
  } catch (err) {
    debug(context, starbanner('** Error in ' ~ SuiteTitle ~ ' Tests: ' ~ err ~ ' **'));
    throw err;
  }
});

function wthComparison(aa, bb) {
  try silent {
    return aa < bb;
  } catch (err) {
    return "" ~ err;
  }
}

function runLanguageStupiditiesTests(context is Context, verbose is boolean) returns map {

  return runTests(context, "testLanguageStupidities", verbose, [
      [[{ a: undefined }],          {},                    'keys with undefined values are silently dropped'],
      [[splitByRegexp("!!!", "!")], ["", "", ""],          'should have four segments; terminal empty segment discarded'],
      [[splitByRegexp("!", "!")],   [""],                  'should have two segments; terminal empty segment discarded'],
      [[splitByRegexp("a!", "!")],  ["a"],                 'should have two segments; terminal empty segment discarded'],
      [[splitByRegexp("!a", "!")],  ["", "a"],             'correctly has two segments; initial empty segment is kept'],
      //
      [[wthComparison(1,   2)],     true,                 'correctly compares two numbers'],
      [[wthComparison("a", "b")],   "Execution error",    'Can not compare string and string.'],
      [[wthComparison("a", 1)],     "Execution error",    'Can not compare string and number.'],
      [[wthComparison([], 1)],      "Execution error",    'Can not compare array and number.'],
      [[wthComparison({},  [])],    "Execution error",    'Can not compare map and array.'],
      [[wthComparison([1], [2])],   "Execution error",    'Can not compare array and array.'],
      [[cmpTo("a", "b")],          -1,                   'we wrote a dumb string comparison workaround'],
      //
      [[strOrderBy(["z", "", "a", "b", "a"])], ["", "a", "a", "b", "z"], ''],
      //
      [[replace("hello", "([hl])", "<$1>")], "<h>e<l><l>o"],
      [[replace("hello", "[hl]",   "<$&>")], "<h>e<l><l>o"],
      [[replace("hello", "[hl]+",  "<$&>")], "<h>e<ll>o"],
      //
      [["" ~ { "meter": 1, "kilogram": 1, "second": 1 }], '{ "kilogram" : 1 , "meter" : 1 , "second" : 1 }', 'concatenation of units'],

      //
      [[ isEqual(() => "a", () => "a") ], false],

      [[mergeMaps({ foo: { a: 1, b: 1 } }, { foo: { b: 2, c: 2 } })], { foo: { b:2, c: 2 } }, "mergeMaps ddoes not merge maps, it clobbers any keys that collide even if their values are maps"],
  ], (args) => args[0]);
}

export const UsefulRECases = [
  [["meter",                  BaseUnitsRE], true],
  [["meter,second",           BaseUnitsRE], true],
  [["meter,second,kelvin",    BaseUnitsRE], true],
  [["kelvin,second,meter",    BaseUnitsRE], true, 'order does not matter'],
  [[",meter,second,kelvin",   BaseUnitsRE], false, 'leading comma'],
  [["meter,second,kelvin,",   BaseUnitsRE], false, 'trailing comma'],
  [["meter,second, kelvin",   BaseUnitsRE], false, 'space after comma'],
  [["meter,second,,kelvin",   BaseUnitsRE], false, 'double comma'],
  [["METER,second,,kelvin",   BaseUnitsRE], false, 'uppercase unit'],
  [["METER",                  BaseUnitsRE], false, 'uppercase unit'],
  [["ampere",                 BaseUnitsRE], true],
  [["kelvin",                 BaseUnitsRE], true],
  [["kilogram",               BaseUnitsRE], true],
  [["meter",                  BaseUnitsRE], true],
  [["radian",                 BaseUnitsRE], true],
  [["second",                 BaseUnitsRE], true],
];

function runUsefulRETests(context is Context, verbose is boolean) returns map {
  return runTests(context, "usefulRE", verbose, UsefulRECases, function(args is array) { return hasMatch(args[0], args[1]); });
}

function runRegexEngineTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "regex", verbose, regexTests, function(args is array) { return attempt(() => replace(args[0], args[1], args[2])); });
}

export const LookbehindCases = [ // These fail:  Invalid regular expression: Invalid special open parenthesis..
  [[ "Price is 50$ or $50",       "(?<=\\d)\\$",               "USD" ], "Execution error",  /* not: "Price is 50USD or $50",  */ 'Positive lookbehind: replaces $ only if preceded by a digit'],
  [[ "a=1, b=2, c=3",             "(?<=[a-z])=",               ":"   ], "Execution error",  /* not: "a:1, b:2, c:3",          */ 'Positive lookbehind: replaces = with : only if preceded by a letter'],
  [[ "ab cb b",                   "(?<=a)b",                   "X"   ], "Execution error",  /* not: "aX cb b",                */ 'positive lookbehind: preceding a is checked but not consumed'],
  [[ "The cat and the tomcat",    "(?<!tom)cat",               "dog" ], "Execution error",  /* not: "The dog and the tomcat", */ 'Negative lookbehind: replaces "cat" only if NOT preceded by "tom"'],
  [[ "ab cb b",                   "(?<!a)b",                   "X"   ], "Execution error",  /* not: "ab cX X",                */ 'negative lookbehind: match b only when not preceded by a'],
];
export const NamedCaptureCases = [  // These fail:  Invalid regular expression: Invalid special open parenthesis..
  [[ "John Doe",     "(?<first>\\w+) (?<last>\\w+)",           "${last}, ${first}" ], 'Execution error', /* not: "Doe, John",      */  'Named captures: swaps words using names instead of $1, $2'],
  [[ "2023-10-25",   "(?<y>\\d{4})-(?<m>\\d{2})-(?<d>\\d{2})", "${m}/${d}/${y}"    ], 'Execution error', /* not: "10/25/2023",     */  'Named captures: reorders date components by name'],
  [[ "foo bar",      "(?<word>\\w+)",                          "[$<word>]"         ], 'Execution error', /* not: "[foo] [bar]",    */  'Named captures: JS-style $<name> replacement syntax'],
  [[ "Ada Lovelace", "(?<first>\\w+) (?<last>\\w+)",           "$2, $1"            ], 'Execution error', /* not:  "Lovelace, Ada", */  'named captures also have numeric indices'],
  [[ "go go stop",   "\\b(?<word>\\w+)\\s+\\k<word>\\b",        "$1"               ], 'Execution error', /* not:  "go stop",       */  'named backreference matches the text captured as word'],
];
export const InlineModifierCases = [ // These fail:  Invalid regular expression: Invalid special open parenthesis..
  [[ "Color is color",           "(?i)color",         "hue" ], "Execution error",  'Invalid regular expression: Invalid special open parenthesis', "hue is hue",             'Inline modifier: (?i) makes the pattern case-insensitive'],
  [[ "fooBARbaz",                "foo(?i:bar)baz",    "qux" ], "Execution error",  'Invalid regular expression: Invalid special open parenthesis', "fooquxbaz",              'Scoped inline modifier: (?i:...) applies case-insensitivity only to that group'],
  [[ "A\nB\nC",                  "(?s)A.*C",          "X"   ], "Execution error",  'Invalid regular expression: Invalid special open parenthesis', "X",                      'Inline modifier: (?s) makes dot match newlines (DOTALL mode)'],
  [[ "cat CAT Cat",              "(?i)cat",           "X" ], "Execution error",  'Invalid regular expression: Invalid special open parenthesis',  "X X X",       'inline case-insensitive modifier'],
  [[ "catDOG CATDOG catdog",     "(?i:cat)DOG",       "X" ], "Execution error",  'Invalid regular expression: Invalid special open parenthesis',  "X X catdog",  'scoped modifier: cat ignores case, but DOG remains case-sensitive'],
];
export const PossessiveQuantifierCases = [  // These don't throw and don't give expected results
  [[ "aaaaaba+b",                "a++b",              "X"    ], "Xa+b",             /* not "X", */               'SHOULD HAVE DONE: Possessive ++: grabs all "a"s, successfully matches "b", and replaces'],
  [[ "aaa",                      "^a++a$",            "X"    ], "X",                /* not "aaa", */             'SHOULD HAVE DONE: Possessive a++ gives nothing back: no match, so input is unchanged'],
  [[ "aaaa",                     "a++a",              "X"    ], "X",                /* not "aaaa", */            'SHOULD HAVE DONE: Possessive ++: grabs all "a"s, refuses to backtrack for the trailing "a", so match FAILS (greedy a+a would match)'],
  [[ "abc",                      "a*+b",              "X"    ], "Xc",                                            'SHOULD HAVE DONE: Possessive *: matches zero or more "a"s possessively, then matches "b"'],
  [[ "aaa",                      "^a*+a$",            "X"    ], "X",                /* not "aaa", */             'SHOULD HAVE DONE: possessive a*+ likewise refuses to backtrack'],
  // these actually pass, maybe they're not good tests?
  [[ "aaa",                      "^a+a$",             "X"    ], "X",                                             'control: greedy a+ gives back one character so the final a can match'],
  [[ "aaab",                     "^a++b$",            "X"    ], "X",                                             'possessive quantifier succeeds when no backtracking is needed'],
];
export const UnicodeGroupCases = [  // Give "Invalid regular expression: Unexpected character in brace expression."
  [[ "hello WORLD!!",            "(\\p{Lu})(\\p{Lu})", "$2$1" ], "Execution error", /* not: "hello OWLRD!!", */  'SHOULD HAVE DONE: Unicode property: \\p{Lu} matches uppercase letters (swaps adjacent pairs)'],
  [[ "Café 123!",                "\\p{N}+",            "#"    ], "Execution error", /* not: "Café #!",       */  'SHOULD HAVE DONE: Unicode property: \\p{N} matches any numeric character across all scripts'],
  [[ "Hello, World!",            "\\p{P}",             ""     ], "Execution error", /* not: "Hello World",   */  'SHOULD HAVE DONE: Unicode property: \\p{P} matches any punctuation character'],
  [[ "A 🌍 B",                   "\\p{Emoji}",         "X"    ], "Execution error", /* not: "A X B",         */  'SHOULD HAVE DONE: Unicode property: \\p{Emoji} matches emoji characters'],
  [[ "aÉβΩz",                    "(\\p{Lu})",          "[$1]" ], "Execution error", /* not: "a[É]β[Ω]z",     */  'SHOULD HAVE DONE: uppercase Unicode letters, not just A-Z'],
  [[ "x٣y४z",                    "\\p{Nd}",            "#"    ], "Execution error", /* not: "x#y#z",         */  'SHOULD HAVE DONE: Unicode decimal digits: Arabic-Indic and Devanagari'],
];
export const BackReferenceCases = [  // These all work!
  [[ "the the quick brown fox",  "\\b(\\w+) \\1\\b",    "$1"    ], "the quick brown fox",    'SHOULD HAVE DONE: Standard backreference: \\1 in regex matches the exact text captured by group 1'],
  [[ "haha",                     "(ha)\\1",             "ho"    ], "ho",                     'SHOULD HAVE DONE: Standard backreference: replaces duplicated syllables'],
  [[ "go go stop stop",          "\\b(\\w+)\\s+\\1\\b", "$1"    ], "go stop",                 'SHOULD HAVE DONE: numbered backreference in the pattern: repeated words collapse'],
  [[ "left:right",               "(\\w+):(\\w+)",       "$2/$1" ], "right/left",              'SHOULD HAVE DONE: capture references in the replacement can reorder captured text'],
  // This doesn't: "Invalid regular expression: Unexpected character in brace expression."
  [[ "abc",                      "(a)(b)\\g{-1}",       "X"     ], "Execution error", "aXc",   'SHOULD HAVE DONE: Relative backreference: \\g{-1} refers to the immediately preceding capture group (b)'],
];
export const PosixClassCases = [ // These all work!
  [[ "User123 logged in",        "[[:digit:]]+",      "#"         ], "User# logged in",        'POSIX class: [[:digit:]] matches digits (equivalent to \\d)'],
  [[ "Hello, World!",            "[[:punct:]]",       ""          ], "Hello World",            'POSIX class: [[:punct:]] matches punctuation characters'],
  [[ "abc 123 !@#",              "[[:alnum:]]+",      "X"         ], "X X !@#",                'POSIX class: [[:alnum:]] matches letters and numbers'],
  [[ "lower UPPER",              "[[:lower:]]+",      "x"         ], "x UPPER",                'POSIX class: [[:lower:]] matches lowercase letters only'],
  [[ "a12 b3",                   "[[:digit:]]+",      "#"         ],  "a# b#",                 'POSIX digit class; the nested brackets are intentional'],
  [[ "abc_123 XYZ",              "[[:alpha:]]+",      "#"         ],  "#_123 #",               'POSIX alphabetic class; digits and underscore do not match'],
  [[ "a \t b\nc",                "[[:space:]]+",      "_"         ],  "a_b_c",                 'POSIX whitespace class includes spaces, tabs, and newlines'],
];
export const ForwardReferenceCases = [ // Fail with Invalid regular expression: Back-reference index exceeds current sub-expression count..
  [[ "a",                        "^\\1(a)$",          "[$1]"       ], "Execution error",  "[a]",  'forward reference matches empty; group 1 then captures a'],
  [[ "aa",                       "^\\1(a)$",          "[$1]"       ], "Execution error",  "aa",   'does not match: the pattern consumes only one a'],
];
const regexTests = flatten([LookbehindCases, NamedCaptureCases, InlineModifierCases, PossessiveQuantifierCases, UnicodeGroupCases, BackReferenceCases, PosixClassCases, ForwardReferenceCases]);
