FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
import(path : "4ebdc64943b566160ea5cc28", version : "3684960ff67d0db556614489");
import(path : "6fcd20533bd2df7c4094a0bf", version : "5849e1654325cddeda6f7440");
import(path : "dd812faf6ff4099cda4aa0eb", version : "ccf1aec9c08b9ad59691b4d4");
import(path : "66e287bede293cb227dfb89c", version : "e6c9aacc05cf0a6f15c7d4c7");
import(path : "08b6ba15b8255611bafe7520", version : "2a3b23fb0cdd7a00cb549f43");
import(path : "f6ab954150609e1e2e014a1e", version : "cefd17945c168add0f500dad");

const SuiteTitle = "Metadata Utils";

// Only defaultMaybe/sanitize_varname/field_varname are covered here: everything else in
// metadataUtils.fs (setPropAndAttribute, setName, setReadableName, getAttrs, getAllAttrs,
// getBestAttr, getNameProps, getNames, getName, getNameProp, getNameOfBody) reads or writes
// entity properties/attributes on a live Query, and has no meaningful table-driven test without
// an actual part studio to run against.

// == [Run tests] ==

annotation { "Feature Type Name": 'Run ' ~ SuiteTitle ~ ' Tests', "Feature Type Description": "Internal Tests" }
export const runMetadataUtilsTestsFS = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Verbose", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE, "Default": false }
  definition.verbose is boolean;
}
{
  const verbose = ifNil(definition.verbose, false);
  try {
    runDefaultMaybeTests(context, verbose);
    runSanitizeVarnameTests(context, verbose);
    runFieldVarnameTests(context, verbose);
    runIdsForTests(context, verbose);
    //
    if (! verbose) { debug(context, starbanner('** ' ~ SuiteTitle ~ ' Tests ran successfully **')); }
  } catch (err) {
    debug(context, starbanner('** Error in ' ~ SuiteTitle ~ ' Tests: ' ~ err ~ ' **'));
  }
});

// == [defaultMaybe] ==

const nameKeysFn = (baseVal, _) => (baseVal ~ "_keys");

export const DefaultMaybeCases = [
  [[{}, { "varname": "existing" }, "bagname", "varname", nameKeysFn],
   "existing", 'no base value at all: dest is left alone'],
  [[{}, { "bagname": "foo" }, "bagname", "varname", nameKeysFn],
   "foo_keys", 'dest unset: derived fresh from the new base'],
  [[{}, { "bagname": "bar", "varname": "manual" }, "bagname", "varname", nameKeysFn],
   "manual", 'no old base to have derived from: dest is left alone'],
  [[{ "bagname": "foo", "varname": "foo_keys" }, { "bagname": "foo", "varname": "foo_keys" }, "bagname", "varname", nameKeysFn],
   "foo_keys", 'base unchanged: dest is left alone'],
  [[{ "bagname": "foo", "varname": "foo_keys" }, { "bagname": "bar", "varname": "foo_keys" }, "bagname", "varname", nameKeysFn],
   "bar_keys", 'dest was tracking its default: re-derived from the new base'],
  [[{ "bagname": "foo", "varname": "myKeys" }, { "bagname": "bar", "varname": "myKeys" }, "bagname", "varname", nameKeysFn],
   "myKeys", 'dest was hand-edited away from its default: left alone even though base changed'],
];
export function runDefaultMaybeTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "defaultMaybe", verbose, DefaultMaybeCases, function(args is array) {
    return defaultMaybe(args[0], args[1], args[2], args[3], args[4]);
  });
}

// == [sanitize_varname / field_varname] ==

export const SanitizeVarnameCases = [
  [["foo.bar"],  "foo_bar"],
  [["foo bar!"], "foo__bar__"],
  [["a..b"],     "a__b", 'each special character is replaced independently, not collapsed as a run'],
  [["plain"],    "plain"],
];
export function runSanitizeVarnameTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "sanitize_varname", verbose, SanitizeVarnameCases, function(args is array) { return sanitize_varname(args[0]); });
}

export const FieldVarnameCases = [
  [["coords", "x"],   "coords_x"],
  [["coords", "x.y"], "coords_x_y"],
  [["coords", "x y"], "coords_x__y"],
];
export function runFieldVarnameTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "field_varname", verbose, FieldVarnameCases, function(args is array) { return field_varname(args[0], args[1]); });
}

// == [idsFor] ==
// Id has no documented equality via `==`, so these check the key set idsFor produces rather
// than the Id values themselves.

export const IdsForCases = [
  [[newId(), ["a", "b", "c"]], ["a", "b", "c"]],
  [[newId(), []],              []],
  [[newId(), ["only"]],        ["only"]],
];
function runIdsForTests(context is Context, verbose is boolean) returns map {
  return runTests(context, "idsFor", verbose, IdsForCases, function(args is array) { return keys(idsFor(args[0], args[1])); });
}
