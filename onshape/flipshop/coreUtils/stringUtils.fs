FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
//
import(path : "66e287bede293cb227dfb89c", version : "25bf5ea59817ea0aa1737abd"); // typeUtils, for ifNil &c

/** ASCII lowercase → uppercase, one character to one character. */
export const LowerToUppercase = {
  'a': 'A', 'b': 'B', 'c': 'C', 'd': 'D', 'e': 'E', 'f': 'F', 'g': 'G', 'h': 'H', 'i': 'I', 'j': 'J', 'k': 'K', 'l': 'L', 'm': 'M', 'n': 'N', 'o': 'O', 'p': 'P', 'q': 'Q', 'r': 'R', 's': 'S', 't': 'T', 'u': 'U', 'v': 'V', 'w': 'W', 'x': 'X', 'y': 'Y', 'z': 'Z',
};

/** ASCII uppercase → lowercase, one character to one character. */
export const UpperToLowercase = {
  'A': 'a', 'B': 'b', 'C': 'c', 'D': 'd', 'E': 'e', 'F': 'f', 'G': 'g', 'H': 'h', 'I': 'i', 'J': 'j', 'K': 'k', 'L': 'l', 'M': 'm', 'N': 'n', 'O': 'o', 'P': 'p', 'Q': 'q', 'R': 'r', 'S': 's', 'T': 't', 'U': 'u', 'V': 'v', 'W': 'w', 'X': 'x', 'Y': 'y', 'Z': 'z',
};

/**
 * Extracts the text from `begseq` up to, but not including, `endseq`, and returns it as a new
 * string — the semantics of JS's `String.prototype.slice`.
 *
 * If `begseq >= length(str)`, an empty string is returned. If `begseq < 0`, it's treated as
 * `max(length(str) + begseq, 0)` — counted from the end of the string; `endseq` follows the same
 * rule when negative. If `endseq` is omitted, or `>= length(str)`, this extracts to the end of
 * the string. If, after normalizing negative values, `endseq <= begseq`, an empty string is
 * returned.
 *
 * FeatureScript has no omitted-argument default the way JS does, so `SequencePosition.END` is an
 * explicit stand-in for "to the end of the string" at a call site that must supply all three
 * arguments (e.g. inside a fixed-arity callback).
 *
 * @example
 *   strSlice("hello world", 0, 3);     // => "hel"
 *   strSlice("hello world", -5);       // => "world"
 *   strSlice("hello world", 0, -1);    // => "hello worl"
 *   strSlice("hello world", -3, -1);   // => "rl"
 *   strSlice("hello world", 100, 200); // => ""
 */
export function strSlice(str is string, begseq is number, endseq is number) returns string {
  const len = length(str);
  // Negative indices count back from the end; positive indices clamp to len
  var beg = (begseq < 0) ? max(len + begseq, 0) : min(begseq, len);
  var end = (endseq < 0) ? max(len + endseq, 0) : min(endseq, len);
  //
  if (beg >= end) { return ""; }
  //
  return substring(str, beg, end);
}
/** Explicit stand-in for "to the end of the string," for a `strSlice` call site that must supply all three arguments. */
export enum SequencePosition { annotation { "Name": "End of String" } END }

/** Resolves `pos` (only `SequencePosition.END` is defined) against `beg`/`end`; `undefined` otherwise. */
export function rangedSequencePosition(pos is SequencePosition, beg is number, end is number) {
  if (pos == SequencePosition.END) { return end; }
  return undefined;
}

export function strSlice(str is string, begseq is number, endseq is SequencePosition) returns string {
  return strSlice(str, begseq, rangedSequencePosition(endseq, 0, length(str)));
}
export function strSlice(str is string, begseq is number) returns string {
  return strSlice(str, begseq, length(str));
}

/**
 * First `len` characters of `str`; `""` if `len <= 0`.
 * @example
 *   strTake("hello world", 2); // => "he"
 */
export function strTake(str is string, len is number) returns string {
  if (len <= 0) { return ""; }
  return substring(str, 0, min(len, length(str)));
}
/**
 * Last `len` characters of `str`; `""` if `len <= 0`.
 * @example
 *   strTakeRight("hello world", 2); // => "ld"
 */
export function strTakeRight(str is string, len is number) returns string {
  if (len <= 0) { return ""; }
  const beg = max(0, length(str) - len);
  return substring(str, beg, length(str));
}

/** `padstr` repeated enough times to reach at least `neededLen` characters — not truncated to it; that's the caller's job. */
export function paddingFor(padstr is string, neededLen is number) returns string {
  const reps = ceil(neededLen / length(padstr));
  return repeatString(padstr, reps);
}
/**
 * Pads `str` on the left side if it's shorter than `minlen`. Padding characters are truncated if
 * they exceed `minlen`. Overloads: `padLeft(num is number, minlen is number, padstr is string)`
 * and `padLeft(num is number, minlen is number)` call `padLeft` on the stringified value.
 * @example
 *   padLeft("hello world", 12);        // => " hello world"
 *   padLeft("hello world", 12, "!");   // => "!hello world"
 *   padLeft("hello world", 11);        // => "hello world"
 */
export function padLeft(str is string, minlen is number, padstr is string) returns string {
  if     (padstr == '') { throw "Padchar must not be blank (trying to pad " ~ str ~ ")"; }
  const  minlen0 = max(0, minlen);
  const  neededLen = minlen0 - length(str);
  if     (neededLen <= 0) { return str; }
  const  padding = strTake(paddingFor(padstr, neededLen), neededLen);
  return padding ~ str;
}
export function padLeft(str is string, minlen is number) returns string {
  return padLeft(str, minlen, ' ');
}
export function padLeft(num is number, minlen is number, padstr is string) returns string {
  return padLeft('' ~ num, minlen, padstr);
}
export function padLeft(num is number, minlen is number) returns string {
  return padLeft(num, minlen, ' ');
}

/**
 * Pads `str` on the right side if it's shorter than `minlen`. Padding characters are truncated
 * if they exceed `minlen`. Unlike `padLeft`, there's no numeric overload — a `number` has to be
 * stringified by the caller first.
 * @example
 *   padRight("hello world", 12);      // => "hello world "
 *   padRight("hello world", 12, "!"); // => "hello world!"
 */
export function padRight(str is string, minlen is number, padstr is string) returns string {
  if     (padstr == '') { throw "Padchar must not be blank (trying to pad " ~ str ~ ")"; }
  const  minlen0 = max(0, minlen);
  const  neededLen = minlen0 - length(str);
  if     (neededLen <= 0) { return str; }
  const  padding = strTakeRight(paddingFor(padstr, neededLen), neededLen);
  return str ~ padding;
}
export function padRight(str is string, minlen is number) returns string {
  return padRight(str, minlen, ' ');
}

/**
 * Repeats `str` `reps` times.
 * @example
 *   strRepeat("*", 3);   // => "***"
 *   strRepeat("abc", 2); // => "abcabc"
 *   strRepeat("abc", 0); // => ""
 */
export function strRepeat(str is string, reps is number) returns string {
  return repeatString(str, reps);
}

/**
 * Wraps `str` in a border of `*` characters matching its own length, for a `debug()` call that
 * wants to stand out.
 * @example
 *   starbanner("hi"); // => "\n**\nhi\n**\n\n"
 */
export function starbanner(str is string) {
  const stars = strRepeat("*", length(str));
  return "\n" ~ stars ~ "\n" ~ str ~ "\n" ~ stars ~ "\n\n";
}

/**
 * Whether `str` matches `regex` anywhere — a `match` wrapper that returns `false` instead of
 * throwing, on either a malformed `regex` or an `undefined` `str`.
 * @example
 *   hasMatch("hello", "ell"); // => true
 *   hasMatch("hello", "^e"); // => false
 */
export function hasMatch(str is string, regex is string) returns boolean {
  try {
    const mm = match(str, regex);
    return mm.hasMatch;
  } catch(err) { println("error " ~ err ~ " matching regex /" ~ regex ~ "/ against " ~ str); return false; }
}
export function hasMatch(str is undefined, regex is string) returns boolean { return false; }

function downcaseChar(char is string) returns string {
  const lochar = UpperToLowercase[char];
  if (lochar == undefined) { return char; }
  return lochar;
}

/**
 * Converts `str`, as a whole, to lower case. ASCII-only, via an explicit character lookup —
 * unlike lodash's `toLower`, there's no Unicode case folding, but a non-letter character is left
 * untouched rather than causing an error.
 * @example
 *   downcase("fooBar");      // => "foobar"
 *   downcase("--FOO-BAR--"); // => "--foo-bar--"
 */
export function downcase(str is string) returns string {
  var lower is string = '';
  for (var char in splitIntoCharacters(str)) {
    lower = lower ~ downcaseChar(char);
  }
  return lower;
}

function upcaseChar(char is string) returns string {
  const lochar = LowerToUppercase[char];
  if (lochar == undefined) { return char; }
  return lochar;
}

/**
 * Converts `str`, as a whole, to upper case. Same ASCII-only limitation as `downcase`.
 * @example
 *   upcase("fooBar");      // => "FOOBAR"
 *   upcase("--foo-bar--"); // => "--FOO-BAR--"
 */
export function upcase(str is string) returns string {
  var upper is string = '';
  for (var char in splitIntoCharacters(str)) {
    upper = upper ~ upcaseChar(char);
  }
  return upper;
}

/** `titleCase` with default options — @see the two-argument overload. */
export function titleCase(str is string) returns string {
  return titleCase(str, {});
}

/**
 * Converts `str` to start case, in the spirit of lodash's `startCase`: splits into words,
 * capitalizes each word's first letter, and lower-cases the rest. Unlike `startCase`, word
 * breaks come from a configurable set of delimiter characters rather than Unicode word-boundary
 * detection, and a single-character translation can run ahead of capitalization.
 * @param opts {map}: keyword options
 *   - @field [spaces="-_"] {string}: Characters that denote a word break.
 *   - @field [tr={}] {map}: Single-character to single-character translation, applied before capitalization.
 * @example
 *   titleCase("socket_kind");                             // => "Socket Kind"
 *   titleCase("hello.world", { "spaces": "." });          // => "Hello World"
 *   titleCase("a_b_c", { "tr": { "a": "X", "b": "Y" } }); // => "X Y C"
 */
export function titleCase(str is string, opts is map) returns string {
  const spaces = opts.spaces == undefined ? "-_" : opts.spaces;
  const tr = opts.tr == undefined ? {} : opts.tr;

  var chars = splitByRegexp(str, "");
  var spaceChars = splitByRegexp(spaces, "");
  var result = "";
  var capitalizeNext = true;
  //
  for (var i = 0; i < size(chars); i += 1) {
    var c = chars[i];
    if (c == "") { continue; }

    // Apply single-character translation if present
    if (tr[c] != undefined) {
      result ~= tr[c];
      capitalizeNext = false;
      continue;
    }

    // Check if character is a space delimiter
    var isSpace = false;
    for (var s in spaceChars) {
      if (s != "" && c == s) {
        isSpace = true;
      }
    }

    if (isSpace) {
      result ~= " ";
      capitalizeNext = true;
    } else {
      if (capitalizeNext) {
        result ~= upcaseChar(c);
        capitalizeNext = false;
      } else {
        result ~= downcaseChar(c);
      }
    }
  }
  return result;
}

// == [Lodash String ports] -- camelCase, capitalize, kebabCase, lowerCase, lowerFirst, pad, snakeCase, trim*, truncate, upperCase, upperFirst, words

/**
 * `str` split into words on runs of non-alphanumeric characters — the shared primitive behind
 * `camelCase`/`kebabCase`/`snakeCase`/`upperCase`/`lowerCase` below. Simpler than lodash's own
 * `words`: this splits only on delimiter characters, not on camelCase boundaries or digit runs.
 * @example
 *   words("foo-bar_baz qux"); // => ["foo", "bar", "baz", "qux"]
 */
export function words(str is string) returns array {
  return filter(splitByRegexp(str, "[^a-zA-Z0-9]+"), (word) => (word != ""));
}

/**
 * `str` split into words and rejoined in camelCase: the first word lowercased, every other word
 * capitalized, no separators.
 * @example
 *   camelCase("Foo Bar");  // => "fooBar"
 *   camelCase("foo-bar");  // => "fooBar"
 */
export function camelCase(str is string) returns string {
  const wordlist = words(str);
  if (size(wordlist) == 0) { return ""; }
  var result = downcase(wordlist[0]);
  for (var seq = 1; seq < size(wordlist); seq += 1) {
    result ~= capitalize(wordlist[seq]);
  }
  return result;
}

/**
 * `str` with its first character uppercased and the rest lowercased.
 * @example
 *   capitalize("FRED"); // => "Fred"
 */
export function capitalize(str is string) returns string {
  return upcase(strTake(str, 1)) ~ downcase(strSlice(str, 1));
}

/**
 * `str` with every regex metacharacter (`\ ^ $ . * + ? ( ) [ ] { } |`) preceded by a backslash,
 * so it can be dropped into `match`/`replace`/`splitByRegexp`'s `regExp` argument and matched
 * literally instead of interpreted. One `replace`, using `$&` to echo back whatever matched —
 * unlike lodash, which tests for a metacharacter before replacing so it can skip the replace when
 * there's nothing to do; skipped here since there'd be nothing to save by scanning `str` twice
 * instead of once.
 * @example
 *   escapeRegExp("[lodash](https://lodash.com/)"); // => "\\[lodash\\]\\(https://lodash\\.com/\\)"
 */
export function escapeRegExp(str is string) returns string {
  return replace(str, "[\\\\^$.*+?()[\\]{}|]", "\\$&");
}

/** `str` split into words, lowercased, and joined with `-`. */
export function kebabCase(str is string) returns string {
  return join(mapArray(words(str), (word) => downcase(word)), "-");
}

/** `str` split into words, lowercased, and joined with a space. */
export function lowerCase(str is string) returns string {
  return join(mapArray(words(str), (word) => downcase(word)), " ");
}

/**
 * `str` with only its first character lowercased, the rest left untouched — `upperFirst`'s
 * counterpart.
 * @example
 *   lowerFirst("Fred"); // => "fred"
 */
export function lowerFirst(str is string) returns string {
  return downcase(strTake(str, 1)) ~ strSlice(str, 1);
}

/**
 * Pads `str` on both sides if it's shorter than `minlen`, splitting the padding as evenly as
 * possible and favoring the right side when it's odd — `padLeft`/`padRight`'s two-sided sibling.
 * @example
 *   pad("hi", 6); // => "  hi  "
 *   pad("hi", 5); // => " hi  "
 */
export function pad(str is string, minlen is number, padstr is string) returns string {
  const neededLen = max(0, minlen - length(str));
  const leftLen = floor(neededLen / 2);
  return padRight(padLeft(str, length(str) + leftLen, padstr), minlen, padstr);
}
export function pad(str is string, minlen is number) returns string {
  return pad(str, minlen, ' ');
}

/** `str` split into words, lowercased, and joined with `_`. */
export function snakeCase(str is string) returns string {
  return join(mapArray(words(str), (word) => downcase(word)), "_");
}

/** Whitespace characters `trim`/`trimStart`/`trimEnd` strip by default. */
const WhitespaceChars = [" ", "\t", "\n", "\r"];

/**
 * `str` with any character in `chars` (default whitespace) removed from the front.
 * @example
 *   trimStart("  hi  "); // => "hi  "
 */
export function trimStart(str is string, chars is array) returns string {
  if (size(chars) == 0) { return str; }
  return replace(str, "^[" ~ join(mapArray(chars, escapeRegExp), "") ~ "]+", "");
}
export function trimStart(str is string) returns string {
  return trimStart(str, WhitespaceChars);
}

/** `trimStart`'s counterpart: strips from the back instead of the front. */
export function trimEnd(str is string, chars is array) returns string {
  if (size(chars) == 0) { return str; }
  return replace(str, "[" ~ join(mapArray(chars, escapeRegExp), "") ~ "]+$", "");
}
export function trimEnd(str is string) returns string {
  return trimEnd(str, WhitespaceChars);
}

/** `trimStart` and `trimEnd` together: strips from both ends. */
export function trim(str is string, chars is array) returns string {
  return trimEnd(trimStart(str, chars), chars);
}
export function trim(str is string) returns string {
  return trim(str, WhitespaceChars);
}

/**
 * `str` shortened to at most `opts.length` characters (the omission marker included), replacing
 * whatever got cut with `opts.omission`. Unlike lodash, there's no `separator` option to break at
 * a word/regex boundary instead of an exact character count.
 * @param opts {map}: keyword options
 *   - @field [length=30] {number}: Maximum result length, omission marker included.
 *   - @field [omission="..."] {string}: Marker appended when `str` is cut.
 * @example
 *   truncate("hello world", { "length": 8 }); // => "hello..."
 */
export function truncate(str is string, opts is map) returns string {
  const maxlen = ifNil(opts.length, 30);
  const omission = ifNil(opts.omission, "...");
  if (length(str) <= maxlen) { return str; }
  const keepLen = max(0, maxlen - length(omission));
  return strTake(str, keepLen) ~ omission;
}
export function truncate(str is string) returns string {
  return truncate(str, {});
}

/** `str` split into words, uppercased, and joined with a space. */
export function upperCase(str is string) returns string {
  return join(mapArray(words(str), (word) => upcase(word)), " ");
}

/**
 * `str` with only its first character uppercased, the rest left untouched — unlike `capitalize`,
 * everything after the first character is left as-is rather than lowercased.
 * @example
 *   upperFirst("fred"); // => "Fred"
 */
export function upperFirst(str is string) returns string {
  return upcase(strTake(str, 1)) ~ strSlice(str, 1);
}
