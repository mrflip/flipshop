FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");

export const LowerToUppercase = {
  'a': 'A', 'b': 'B', 'c': 'C', 'd': 'D', 'e': 'E', 'f': 'F', 'g': 'G', 'h': 'H', 'i': 'I', 'j': 'J', 'k': 'K', 'l': 'L', 'm': 'M', 'n': 'N', 'o': 'O', 'p': 'P', 'q': 'Q', 'r': 'R', 's': 'S', 't': 'T', 'u': 'U', 'v': 'V', 'w': 'W', 'x': 'X', 'y': 'Y', 'z': 'Z',
};

export const UpperToLowercase = {
  'A': 'a', 'B': 'b', 'C': 'c', 'D': 'd', 'E': 'e', 'F': 'f', 'G': 'g', 'H': 'h', 'I': 'i', 'J': 'j', 'K': 'k', 'L': 'l', 'M': 'm', 'N': 'n', 'O': 'o', 'P': 'p', 'Q': 'q', 'R': 'r', 'S': 's', 'T': 't', 'U': 'u', 'V': 'v', 'W': 'w', 'X': 'x', 'Y': 'y', 'Z': 'z',
};

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
export enum SequencePosition { annotation { "Name": "End of String" } END }

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

export function strTake(str is string, len is number) returns string {
  if (len <= 0) { return ""; }
  return substring(str, 0, min(len, length(str)));
}
export function strTakeRight(str is string, len is number) returns string {
  if (len <= 0) { return ""; }
  const beg = max(0, length(str) - len);
  return substring(str, beg, length(str));
}

export function paddingFor(padstr is string, neededLen is number) returns string {
  const reps = ceil(neededLen / length(padstr));
  return repeatString(padstr, reps);
}
/**
 * Pads `str` on the left side if it's shorter than `minlen`. Padding characters are truncated if
 * they exceed `minlen`. Also overloaded for a `number`, which is stringified first.
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
 * if they exceed `minlen`. Also overloaded for a `number`, which is stringified first.
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
  var result = '';
  for (var i = 0; i < reps; i = i + 1) {
    result = result ~ str;
  }
  return result;
}

// repeats the string surrounded by "\n****************************************\n"
export function starbanner(str is string) {
  const stars = strRepeat("*", length(str));
  return "\n" ~ stars ~ "\n" ~ str ~ "\n" ~ stars ~ "\n\n";
}

export function hasMatch(str, regex is string) {
  if (str == undefined) { return false; }
  try {
    const mm = match(str, regex);
    return mm.hasMatch;
  } catch(err) { println("error " ~ err ~ " matching regex /" ~ regex ~ "/ against " ~ str); return false; }
}

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