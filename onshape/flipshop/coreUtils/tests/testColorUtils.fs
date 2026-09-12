FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");

import(path : "4ebdc64943b566160ea5cc28", version : "3684960ff67d0db556614489");
import(path : "f6ab954150609e1e2e014a1e", version : "cefd17945c168add0f500dad");
import(path : "fb2ebd26998aed1d4a76e115", version : "61f6dfc83f92e38721d9a67d");
// Testing this:
import(path : "8c588debec029dab0d734198", version : "2c669528a2f3a8e07f6f4493");

const SuiteTitle = "Color Utils";

// == [Color Test Feature Harness]

/**
 * Part feature: extrudes `text` sized and aligned within a bounding plate at each selected plane.
 * Delegates all geometry to @see `textChip`.
 * @param definition {{
 *   @field json {string} : stringified JSON to produce
 * }}
 */
annotation { "Feature Type Name": 'Run ' ~ SuiteTitle ~ ' Tests', "Feature Type Description": "Internal Tests" }
export const runStringUtilsTestsFS = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Verbose", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE, "Default": false }
  definition.verbose is boolean;
}
{
  const verbose = ifNil(definition.verbose, false);
  try {
    runColorRoundtripTests(context, verbose);
    //
    if (! verbose) { debug(context, starbanner('** ' ~ SuiteTitle ~ ' Tests ran successfully **')); }
  } catch (err) {
    debug(context, starbanner('** Error in ' ~ SuiteTitle ~ ' Tests: ' ~ err ~ ' **'));
    return;
  }
});
// --

// == [Color Roundtrip Tests]

export function runColorRoundtripTests(context is Context, verbose is boolean) {
  var oops = [];
  for (var ii = 200; ii <= 203; ii += 1) {
    try {
      const hexpair  = intToHexpair(ii);
      const unhexed  = hexpairToInt(hexpair);
      const uphexed  = hexpairToInt(upcase(hexpair));
      const lohexed  = hexpairToInt(upcase(downcase(upcase(hexpair))));
      if (unhexed != ii) { oops = append(oops, ['unhexed', ii, hexpair, unhexed]); }
      if (uphexed != ii) { oops = append(oops, ['uphexed', ii, hexpair, uphexed]); }
      if (lohexed != ii) { oops = append(oops, ['lohexed', ii, hexpair, lohexed]); }
      const clr      = toColor("" ~ [ii/255 + 0.0001, (255-ii) / 255, ii / 500]);
      const c1       = toColor(toHexcolor(clr));
      const c2       = toColor('' ~ toTuplecolor(clr));
      const c3       = toColor('' ~ toUnitcolor(clr));
      if (! sameColor(clr, c1)) { oops = append(oops, ['toColor(toHexcolor())',   ii, c1, clr, toHexcolor(clr)]); }
      if (! sameColor(clr, c2)) { oops = append(oops, ['toColor(toTuplecolor())', ii, c2, clr, toHexcolor(clr)]); }
      if (! sameColor(clr, c3)) { oops = append(oops, ['toColor(toUnitcolor())',  ii, c2, clr, toHexcolor(clr)]); }
    }
  }
  return {};
}
// --