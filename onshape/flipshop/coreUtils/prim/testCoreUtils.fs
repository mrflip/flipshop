FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
//
import(path : "f6ab954150609e1e2e014a1e", version : "939862c0f6583971525024d5"); // runTests
// Testing this:
import(path : "dcee57677ef58f0c8e045269", version : "0d87751b06d38b222b661b1a");

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
    runRegexTests(context, verbose);
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

// NOTE! this is not how you use runTests, it is generating the value *in* the fixture, not args to a function
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
      [[strCmp("a", "b")],          -1,                   'we wrote a dumb string comparison workaround'],
      //
      [[strOrderBy(["z", "", "a", "b", "a"])], ["", "a", "a", "b", "z"], ''],

      [[mergeMaps({ foo: { a: 1, b: 1 } }, { foo: { b: 2, c: 2 } })], { foo: { b:2, c: 2 } }, "mergeMaps does not merge maps, it clobbers any keys that collide even if their values are maps"],
  ], (args) => args[0]);
}

function runRegexTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "regex", verbose, regexTests, function(args is array) { return attempt(() => replace(args[0], args[1], args[2])); });
}

export const LookbehindCases = [
  [[ "Price is 50$ or $50",      "(?<=\\d)\\$",       "USD" ], "Price is 50USD or $50",  'Positive lookbehind: replaces $ only if preceded by a digit'],
  [[ "The cat and the tomcat",   "(?<!tom)cat",       "dog" ], "The dog and the tomcat", 'Negative lookbehind: replaces "cat" only if NOT preceded by "tom"'],
  [[ "a=1, b=2, c=3",            "(?<=[a-z])=",       ":"   ], "a:1, b:2, c:3",          'Positive lookbehind: replaces = with : only if preceded by a letter'],
  [[ "ab cb b", "(?<=a)b", "X" ],  "aX cb b",  'positive lookbehind: preceding a is checked but not consumed'],
  [[ "ab cb b", "(?<!a)b", "X" ],  "ab cX X",  'negative lookbehind: match b only when not preceded by a'],
];
export const NamedCaptureCases = [
  [[ "John Doe",                 "(?<first>\\w+) (?<last>\\w+)", "${last}, ${first}" ], "Doe, John",          'Named captures: swaps words using names instead of $1, $2'],
  [[ "2023-10-25",               "(?<y>\\d{4})-(?<m>\\d{2})-(?<d>\\d{2})", "${m}/${d}/${y}" ], "10/25/2023",    'Named captures: reorders date components by name'],
  [[ "foo bar",                  "(?<word>\\w+)",     "[$<word>]" ], "[foo] [bar]",      'Named captures: JS-style $<name> replacement syntax'],
  [[ "Ada Lovelace",             "(?<first>\\w+) (?<last>\\w+)",     "$2, $1" ],  "Lovelace, Ada",  'named captures also have numeric indices'],
  [[ "go go stop",               "\\b(?<word>\\w+)\\s+\\k<word>\\b", "$1"     ],  "go stop",        'named backreference matches the text captured as word'],
];
export const InlineModifierCases = [
  [[ "Color is color",           "(?i)color",         "hue" ], "hue is hue",             'Inline modifier: (?i) makes the pattern case-insensitive'],
  [[ "fooBARbaz",                "foo(?i:bar)baz",    "qux" ], "fooquxbaz",              'Scoped inline modifier: (?i:...) applies case-insensitivity only to that group'],
  [[ "A\nB\nC",                  "(?s)A.*C",          "X"   ], "X",                      'Inline modifier: (?s) makes dot match newlines (DOTALL mode)'],
  [[ "cat CAT Cat",              "(?i)cat",           "X" ],  "X X X",       'inline case-insensitive modifier'],
  [[ "catDOG CATDOG catdog",     "(?i:cat)DOG",       "X" ],  "X X catdog",  'scoped modifier: cat ignores case, but DOG remains case-sensitive'],
];
export const PossessiveQuantifierCases = [
  [[ "aaaaab",                   "a++b",              "X"   ], "X",                      'Possessive ++: grabs all "a"s, successfully matches "b", and replaces'],
  [[ "aaaa",                     "a++a",              "X"   ], "aaaa",                   'Possessive ++: grabs all "a"s, refuses to backtrack for the trailing "a", so match FAILS (greedy a+a would match)'],
  [[ "abc",                      "a*+b",              "X"   ], "Xc",                     'Possessive *: matches zero or more "a"s possessively, then matches "b"'],
  [[ "aaa",  "^a+a$",  "X" ],    "X",    'control: greedy a+ gives back one character so the final a can match'],
  [[ "aaa",  "^a++a$", "X" ],    "aaa",  'possessive a++ gives nothing back: no match, so input is unchanged'],
  [[ "aaa",  "^a*+a$", "X" ],    "aaa",  'possessive a*+ likewise refuses to backtrack'],
  [[ "aaab", "^a++b$", "X" ],    "X",    'possessive quantifier succeeds when no backtracking is needed'],
];
export const UnicodeGroupCases = [
  [[ "hello WORLD!!",            "(\\p{Lu})(\\p{Lu})",      "$2$1" ], "hello OWLRD!!",         'Unicode property: \\p{Lu} matches uppercase letters (swaps adjacent pairs)'],
  [[ "Café 123!",                "\\p{N}+",                 "#"   ], "Café #!",                'Unicode property: \\p{N} matches any numeric character across all scripts'],
  [[ "Hello, World!",            "\\p{P}",                  ""    ], "Hello World",            'Unicode property: \\p{P} matches any punctuation character'],
  [[ "A 🌍 B",                   "\\p{Emoji}",              "X"   ], "A X B",                  'Unicode property: \\p{Emoji} matches emoji characters'],
  [[ " hello WORLD!!",           "([\\p{Lu}])([\\p{Lu}])", "$2$1" ],  " hello OWLRD!!",  'swap uppercase pairs; the final D has no partner'],
  [[ "aÉβΩz",                    "(\\p{Lu})",              "[$1]" ],  "a[É]β[Ω]z",      'uppercase Unicode letters, not just A-Z'],
  [[ "x٣y४z",                    "\\p{Nd}",                "#"    ],  "x#y#z",          'Unicode decimal digits: Arabic-Indic and Devanagari'],
];
export const BackReferenceCases = [
  [[ "the the quick brown fox",  "\\b(\\w+) \\1\\b",  "$1"  ], "the quick brown fox",    'Standard backreference: \\1 in regex matches the exact text captured by group 1'],
  [[ "haha",                     "(ha)\\1",           "ho"  ], "ho",                     'Standard backreference: replaces duplicated syllables'],
  [[ "abc",                      "(a)(b)\\g{-1}",     "X"   ], "aXc",                    'Relative backreference: \\g{-1} refers to the immediately preceding capture group (b)'],
  [[ "go go stop stop",          "\\b(\\w+)\\s+\\1\\b", "$1"    ],  "go stop",     'numbered backreference in the pattern: repeated words collapse'],
  [[ "left:right",               "(\\w+):(\\w+)",       "$2/$1" ],  "right/left",  'capture references in the replacement can reorder captured text'],
];
export const PosixClassCases = [
  [[ "User123 logged in",        "[[:digit:]]+",      "#"   ], "User# logged in",        'POSIX class: [[:digit:]] matches digits (equivalent to \\d)'],
  [[ "Hello, World!",            "[[:punct:]]",       ""    ], "Hello World",            'POSIX class: [[:punct:]] matches punctuation characters'],
  [[ "abc 123 !@#",              "[[:alnum:]]+",      "X"   ], "X X !@#",                'POSIX class: [[:alnum:]] matches letters and numbers'],
  [[ "lower UPPER",              "[[:lower:]]+",      "x"   ], "x UPPER",                'POSIX class: [[:lower:]] matches lowercase letters only'],
  [[ "a12 b3",                   "[[:digit:]]+", "#" ],  "a# b#",    'POSIX digit class; the nested brackets are intentional'],
  [[ "abc_123 XYZ",              "[[:alpha:]]+", "#" ],  "#_123 #",  'POSIX alphabetic class; digits and underscore do not match'],
  [[ "a \t b\nc",                "[[:space:]]+", "_" ],  "a_b_c",    'POSIX whitespace class includes spaces, tabs, and newlines'],
];
export const ForwardReferenceCases = [
  [[ "a",  "^\\1(a)$", "[$1]" ],  "[a]",  'forward reference matches empty; group 1 then captures a'],
  [[ "aa", "^\\1(a)$", "[$1]" ],  "aa",   'does not match: the pattern consumes only one a'],
];
const regexTests = flatten([LookbehindCases, NamedCaptureCases, InlineModifierCases, PossessiveQuantifierCases, UnicodeGroupCases, BackReferenceCases, PosixClassCases, ForwardReferenceCases]);
