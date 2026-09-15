FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
//
import(path : "f6ab954150609e1e2e014a1e", version : "939862c0f6583971525024d5"); // runTests
// Testing this:
import(path : "8c588debec029dab0d734198", version : "b49eb315047d1da4cb9defd0"); // colorUtils: testing this

const SuiteTitle = "Color Utils";

// `setColor` needs a live Query and isn't covered here — everything else in colorUtils.fs is
// pure and table-tested below.

// == [Run tests] ==

annotation { "Feature Type Name": 'Run ' ~ SuiteTitle ~ ' Tests', "Feature Type Description": "Internal Tests" }
export const runColorUtilsTestsFS = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Verbose", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE, "Default": false }
  definition.verbose is boolean;
}
{
  const verbose = ifNil(definition.verbose, false);
  try {
    runHexcolorToColorTests(context, verbose);
    runTuplestrToColorTests(context, verbose);
    runUnitcolorToColorTests(context, verbose);
    runToColorTests(context, verbose);
    runToHexcolorTests(context, verbose);
    runToUnitcolorTests(context, verbose);
    runToTuplecolorTests(context, verbose);
    runHexpairToIntTests(context, verbose);
    runIntToHexpairTests(context, verbose);
    runSameColorTests(context, verbose);
    runIsHexcolorTests(context, verbose);
    //
    if (! verbose) { debug(context, starbanner('** ' ~ SuiteTitle ~ ' Tests ran successfully **')); }
  } catch (err) {
    debug(context, starbanner('** Error in ' ~ SuiteTitle ~ ' Tests: ' ~ err ~ ' **'));
  }
});

// == [hexcolorToColor / tuplestrToColor / unitcolorToColor] ==

export const HexcolorToColorCases = [
  [["#ff0000",   color(1, 0, 0)],                true],
  [["ff0000",    color(1, 0, 0)],                true, 'leading # is optional'],
  [["#FF0000",   color(1, 0, 0)],                true, 'uppercase hex digits are accepted'],
  [["#00ff0080", color(0, 1, 0, 128 / 255)],      true, '8-digit form carries alpha'],
  [["not a color", color(1, 0, 0)],               true, 'no match: falls back to bright red (OopsColor)'],
];
function runHexcolorToColorTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "hexcolorToColor", verbose, HexcolorToColorCases, function(args is array) {
    return sameColor(hexcolorToColor(args[0]), args[1]);
  });
}

export const TuplestrToColorCases = [
  [["200,99,100",       color(200 / 255, 99 / 255, 100 / 255)],             true],
  [["[0, 1, 255]",      color(0, 1 / 255, 1)],                              true, 'brackets and spaces are optional'],
  [["200,99,100,33",    color(200 / 255, 99 / 255, 100 / 255, 33 / 255)],   true, 'fourth value is alpha'],
  [["not a color",      color(1, 0, 0)],                                   true, 'no match: falls back to bright red'],
];
function runTuplestrToColorTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "tuplestrToColor", verbose, TuplestrToColorCases, function(args is array) {
    return sameColor(tuplestrToColor(args[0]), args[1]);
  });
}

export const UnitcolorToColorCases = [
  [["0.5, 0.8675309, 1.0",     color(0.5, 0.8675309, 1.0)],       true],
  [["[0.0, 0.1, 1.0, 1.0]",    color(0.0, 0.1, 1.0, 1.0)],        true],
  [["not a color",             color(1, 0, 0)],                  true, 'no match: falls back to bright red'],
];
function runUnitcolorToColorTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "unitcolorToColor", verbose, UnitcolorToColorCases, function(args is array) {
    return sameColor(unitcolorToColor(args[0]), args[1]);
  });
}

// == [toColor] ==

export const ToColorCases = [
  [[color(0.2, 0.4, 0.6),  color(0.2, 0.4, 0.6)],              true, 'a Color passes through unchanged'],
  [[[1, 0, 0],             color(1, 0, 0)],                    true, '3-element array, values used as-is (no 0-255 detection)'],
  [[[1, 0, 0, 0.5],        color(1, 0, 0, 0.5)],                true, '4-element array with alpha'],
  [[[],                    color(1, 0, 0)],                    true, 'wrong-length array: falls back to bright red'],
  [["#00ff00",             color(0, 1, 0)],                    true, 'hex string'],
  [["1,0,0",               color(1, 0, 0)],                    true, 'ambiguous 0/1 tuple reads as a unit-scale color, not a 0-255 one'],
  [["200,99,100",          color(200 / 255, 99 / 255, 100 / 255)], true, 'tuple-scale string'],
];
function runToColorTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "toColor", verbose, ToColorCases, function(args is array) { return sameColor(toColor(args[0]), args[1]); });
}

// == [toHexcolor / toUnitcolor / toTuplecolor] ==

export const ToHexcolorCases = [
  [[color(1, 0, 0)],           "#ff0000ff"],
  [[color(0, 1, 0, 128 / 255)], "#00ff0080"],
];
function runToHexcolorTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "toHexcolor", verbose, ToHexcolorCases, function(args is array) { return toHexcolor(args[0]); });
}

export const ToUnitcolorCases = [
  [[color(0.5, 0.25, 1.0)],       [0.5, 0.25, 1.0, 1.0]],
  [[color(0, 1, 0, 0.5)],         [0, 1, 0, 0.5]],
];
function runToUnitcolorTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "toUnitcolor", verbose, ToUnitcolorCases, function(args is array) { return toUnitcolor(args[0]); });
}

export const ToTuplecolorCases = [
  [[color(1, 0, 0)],            [255, 0, 0, 255]],
  [[color(0, 1, 0, 128 / 255)], [0, 255, 0, 128]],
];
function runToTuplecolorTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "toTuplecolor", verbose, ToTuplecolorCases, function(args is array) { return toTuplecolor(args[0]); });
}

// == [hexpairToInt / intToHexpair] ==

export const HexpairToIntCases = [
  [["ff"],  255],
  [["FF"],  255, 'case-insensitive'],
  [["00"],  0],
  [["a0"],  160],
  [["zz", -1], -1, 'invalid pair falls back to the given fallback'],
];
function runHexpairToIntTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "hexpairToInt", verbose, HexpairToIntCases, function(args is array) {
    return (size(args) <= 1) ? hexpairToInt(args[0]) : hexpairToInt(args[0], args[1]);
  });
}

export const IntToHexpairCases = [
  [[255], "ff"],
  [[0],   "00"],
  [[160], "a0"],
];
function runIntToHexpairTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "intToHexpair", verbose, IntToHexpairCases, function(args is array) { return intToHexpair(args[0]); });
}

// == [sameColor / isHexcolor] ==

export const SameColorCases = [
  [[color(1, 0, 0),                     color(1, 0, 0)],                     true],
  [[color(1, 0, 0),                     color(0, 1, 0)],                     false],
  [[color(1, 0, 0),                     color(1, 0, 0, 0.5)],                false, 'alpha counts too'],
  [[color(200 / 255, 0, 0),             color(200 / 255 + 0.0001 / 255, 0, 0)], true, 'within the default tolerance'],
];
function runSameColorTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "sameColor", verbose, SameColorCases, function(args is array) { return sameColor(args[0], args[1]); });
}

export const IsHexcolorCases = [
  [["#ff0000"],  true],
  [["#FF0000"],  true, 'uppercase is recognized'],
  [["ff0000"],   true],
  [["not hex"],  false],
  [[42],         false, 'non-string input'],
];
function runIsHexcolorTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "isHexcolor", verbose, IsHexcolorCases, function(args is array) { return isHexcolor(args[0]); });
}
