FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
//
import(path : "4ebdc64943b566160ea5cc28", version : "2dcc2fdbf288873168104c0d"); // coreUtils, for parseJsonSafely
import(path : "08b6ba15b8255611bafe7520", version : "a132b0ddb6abb95a28f02035"); // jsonVarF, for setVariable
import(path : "6fcd20533bd2df7c4094a0bf", version : "2a0cbea09ce40f274fc34d90"); // jsonVarF, for strTake

IconNamespace::import(path : "ce7a4dfe88a5752b501f7baf", version : "2f0856d649310aa73a24518e");


// == [JSON Var] ==

/**
 * Parses `rawjson` into a map/array and sets it as variable `varname`. Throws if `rawjson` isn't
 * well-formed JSON. Null values in the JSON are omitted rather than coming back as `undefined`
 * (regardless of what `parseJson`'s own docs say), and keys always come back in alphabetic
 * order, not insertion order — both are FeatureScript map behavior, not something this feature
 * adds.
 * @param definition {{
 *   @field varname {string} : Name of the variable to set.
 *   @field rawjson {string} : Stringified JSON to parse.
 *   @field detectUnits {boolean} : Parse unit-bearing strings (e.g. `"3 inch"`) into a `ValueWithUnits`.
 *   @field [description=default] {string} : Description for the variable; a snippet of `rawjson` if blank.
 * }}
 * @TODO Accept a default (a string to parse, or a map merged in directly) via editing logic.
 */
annotation { "Feature Type Name": "JSON Var", "Icon": IconNamespace::BLOB_DATA, "Feature Type Description": "Parse JSON into a variable", "Feature Name Template": "JSON -> #varname" }
export const jsonVarF = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Variable Name" }
  definition.varname is string;
  annotation { "Name": "JSON String" }
  definition.rawjson is string;
  annotation { "Name": "Detect Units",               "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
  definition.detectUnits is boolean;
  annotation { "Name": "Variable Description",       "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
  definition.description is string;
}
{
  const varname     = ifBlank(definition.varname, 'jsondata');
  const rawjson     = ifBlank(definition.rawjson, '{"oops":"empty input"}');
  const description = ifBlank(definition.description, strTake("Parsed from JSON: " ~ rawjson, 200));
  var jsondata = parseJsonSafely(rawjson, { "detectUnits": ifNil(definition.detectUnits, true), "story": "jsonVarF:" ~ varname });
  setVariable(context, varname, jsondata, description);
});

// == [Bag Keys] ==

/**
 * Sets variable `varname` to the ordered keys of variable `bagname` — `range(0, size(arr))` for
 * an array, `keys(map)` for a map. Throws if `bagname` names neither.
 * @param definition {{
 *   @field bagname {string} : Name of the source map/array variable.
 *   @field varname {string} : Name of the result variable.
 *   @field [description=default] {string} : Description for the variable; `"Keys of #bagname"` if blank.
 * }}
 */
annotation { "Feature Type Name": "Key List", "Icon": IconNamespace::BLOB_DATA, "Feature Type Description": "Ordered Keys of a Map", "Feature Name Template": "Keys of #bagname -> #varname", "Editing Logic Function" : "keylistEditLogic" }
export const keylistF = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Source Map Variable Name" }
  definition.bagname is string;
  annotation { "Name": "Result Keys Variable Name" }
  definition.varname is string;
  annotation { "Name": "Variable Description",       "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
  definition.description is string;
}
{
  const bagname     = ifBlank(definition.bagname, '');
  const description = ifBlank(definition.description, strTake("Keys of #" ~ bagname, 50));

  const bag         = strBlank(bagname) ? {} : getVariable(context, bagname);
  var keysarr;
  if        (bag is array) {
    keysarr = range(0, size(bag));
  } else if (bag is map) {
    keysarr = keys(bag);
  } else {
    throw "Map Keys feature needs a bag or an array but variable " ~ bagname ~ " was " ~ bag;
  }
  setVariable(context, definition.varname, keysarr, description);
});

/** Keeps `varname` at `bagname ~ "_keys"` for as long as it hasn't been hand-edited; @see `defaultMaybe`. */
export function keylistEditLogic(context is Context, id is Id, oldDefinition is map, newDefinition is map, isCreating is boolean, specifiedParameters is map) returns map {
  newDefinition.varname = defaultMaybe(oldDefinition, newDefinition, "bagname", "varname", (oname, _) => (oname ~ '_keys'));
  return newDefinition;
}

// == [SizeofF] ==

/**
 * Sets variable `varname` to `sizeof` variable `objname`. Unlike `keylistF`/`valuesAtF`, there's
 * no `description` field here — the description is always the auto-generated `"Size of
 * #objname"` and can't be hand-edited.
 * @param definition {{
 *   @field objname {string} : Name of the source string/map/array variable.
 *   @field varname {string} : Name of the result variable.
 * }}
 */
annotation {
  "Feature Type Name":        "Size of",
  "Icon":                     IconNamespace::BLOB_DATA,
  "Feature Type Description": "Size of a string/bag/list",
  "Feature Name Template":    "Size of #objname -> #varname",
  "Editing Logic Function":   "sizeofEditLogic",
}
export const sizeofF = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Source Map Variable Name" }
  definition.objname is string;
  annotation { "Name": "Result Size Variable Name" }
  definition.varname is string;
}
{
  const objname     = ifBlank(definition.objname, '');
  const description = ifBlank(definition.description, strTake("Size of #" ~ objname, 50));

  const obj         = strBlank(objname) ? {} : getVariable(context, objname);
  setVariable(context, definition.varname, sizeof(obj), description);
});

/** Keeps `varname` at `objname ~ "_size"` for as long as it hasn't been hand-edited; @see `defaultMaybe`. */
export function sizeofEditLogic(context is Context, id is Id, oldDefinition is map, newDefinition is map, isCreating is boolean, specifiedParameters is map) returns map {
  newDefinition.varname = defaultMaybe(oldDefinition, newDefinition, "objname", "varname", (oname, _) => (oname ~ '_size'));
  return newDefinition;
}

// == [Pick Values from a Bag] ==

/**
 * Sets variable `varname` to `valuesAt(bag, keylist)` — the values of variable `bagname` at the
 * keys/indexes in `keylistJSON` (a JSON array), in that order.
 * @param definition {{
 *   @field bagname {string} : Name of the source map/array variable.
 *   @field keylistJSON {string} : JSON array of keys/indexes to pick out, e.g. `["a", "b"]` or `[0, 2]`.
 *   @field varname {string} : Name of the result variable.
 *   @field [description=default] {string} : Description for the variable; a summary of the selection if blank.
 * }}
 */
annotation { "Feature Type Name": "Values At Keylist", "Icon": IconNamespace::BLOB_DATA, "Feature Type Description": "Array of selected map values", "Feature Name Template": "#bagname[...] -> #varname", "Editing Logic Function" : "valuesAtEditLogic" }
export const valuesAtF = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Source Map Variable Name" }
  definition.bagname is string;
  annotation { "Name": "JSON Ordered List of Keys/Indexes", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  definition.keylistJSON is string;
  annotation { "Name": "Result Values Variable Name" }
  definition.varname is string;
  annotation { "Name": "Variable Description",       "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
  definition.description is string;
}
{
  const bagname     = ifBlank(definition.bagname, '');
  // Deliberately redundant with valuesAtEditLogic's own varname default: kept as a fallback for
  // a definition that reaches this body before edit logic has run.
  const varname     = ifBlank(definition.varname, bagname ~ '_vals');
  const bag         = strBlank(bagname) ? {} : getVariable(context, bagname);
  //
  const keylist     = parseJson(ifBlank(definition.keylistJSON, "[]"));
  const vals        = valuesAt(bag, keylist);
  const description = ifBlank(definition.description, strTake("Selected Values of #" ~ bagname ~ "[" ~ definition.keylistJSON ~ "]", 200));
  //
  setVariable(context, varname, vals, description);
});

/**
 * Keeps `varname` at `bagname ~ "_vals"` and `description` at a summary of the selection, each
 * for as long as it hasn't been hand-edited; @see `defaultMaybe`.
 */
export function valuesAtEditLogic(context is Context, id is Id, oldDefinition is map, newDefinition is map, isCreating is boolean, specifiedParameters is map) returns map {
  newDefinition.varname = defaultMaybe(oldDefinition, newDefinition, "bagname", "varname", (oname, _) => (oname ~ '_vals'));
  newDefinition.description = defaultMaybe(oldDefinition, newDefinition, "bagname", "description", (bagname, defn) => (
      strTake("Selected Values of #" ~ bagname ~ ifBlank(defn.keylistJSON, ""), 200))
  );
  return newDefinition;
}


// == [Turn data container elements into eponymous variables] ==

/**
 * Sets one variable per entry of `objname`'s value. For an array, each becomes
 * `<objname>_<00-padded index>` (e.g. `positionVector_00`, `positionVector_01`). For a map, each
 * becomes `<objname>_<key>` if `prefixVarnames` (the default), or just `<key>` if not — either
 * way the key is run through `sanitize_varname` first. A no-op if `objname`'s value is empty.
 * @param definition {{
 *   @field objname {string} : Name of the source map/array variable.
 *   @field prefixVarnames {boolean} : Prefix each map-derived variable name with `objname`. Defaults to `true`. Ignored for an array, which is always prefixed.
 * }}
 */
annotation {
  "Feature Type Name":        "Splat Variables",
  "Icon":                     IconNamespace::BLOB_DATA,
  "Feature Type Description": "Variables for elements of map/array",
  "Feature Name Template":    "Splat #objname[...]",
  // "Editing Logic Function" :  "splatEditLogic"
}
export const splatF = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Source Data Variable Name" }
  definition.objname is string;
  annotation { "Name": "Container is an Array (else map)", "UIHint": [UIHint.ALWAYS_HIDDEN] }
  definition.objIsArray is boolean;
  annotation { "Name": "Prefix Variable Name",       "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE, "Default": true }
  definition.prefixVarnames is boolean;
}
{
  const objname        = ifBlank(definition.objname, '');
  const obj            = strBlank(objname) ? {} : getVariable(context, objname);
  //
  if (isEmpty(obj)) { return; }
  if (obj is array) {
    forEach(obj, MissingPolicy.SKIP, (val, seq) => {
      const keyname = objname ~ "_" ~ padLeft(seq, 2, '0');
      setVariable(context, keyname, val, objname ~ "[" ~ keyname ~ "]");
    });
  } else {
    const prefixVarnames = ifNil(definition.prefixVarnames, true);
    forEach(obj, MissingPolicy.SKIP, (val, keyname) => {
      const fieldvar = (prefixVarnames ? field_varname(objname, keyname) : sanitize_varname(keyname));
      setVariable(context, fieldvar, val, objname ~ "." ~ keyname);
    });
  }
});

// == [Function values] --
// Only the EditLogic functions get a dispatch entry: jsonVarF/keylistF/sizeofF/valuesAtF/splatF
// are `export const`-bound defineFeature values already, so they're nameable first-class values
// on their own — the dispatch-map workaround is for `export function`-declared names like these.

export const JsonVarFuncs = {
  "keylistEditLogic":  (context, id, oldDefinition, newDefinition, isCreating, specifiedParameters) =>
    keylistEditLogic(context, id, oldDefinition, newDefinition, isCreating, specifiedParameters),
  "sizeofEditLogic":   (context, id, oldDefinition, newDefinition, isCreating, specifiedParameters) =>
    sizeofEditLogic(context, id, oldDefinition, newDefinition, isCreating, specifiedParameters),
  "valuesAtEditLogic": (context, id, oldDefinition, newDefinition, isCreating, specifiedParameters) =>
    valuesAtEditLogic(context, id, oldDefinition, newDefinition, isCreating, specifiedParameters),
};
