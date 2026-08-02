FeatureScript 3029;
import(path : "onshape/std/common.fs", version : "3029.0");
import(path : "58963520be3fe612d10b6d2e", version : "a3fca0f70c41654575775503");
IconNamespace::import(path : "2c0ddd695d9e3591559a1803", version : "cc4cde41958f19c6ec8db006");

// == [JSON Var] ==

/**
 * Utility Feature: convert a JSON string to a named variable (either a map or array). Throws if the string is not well-formed JSON
 * As far as I can tell, the docs for parseJSON are wrong -- Null values in the JSON are omitted and NOT returned as undefined.
 * As elsewhere in Featurescript, keys are always returned in alphabetic order, not insertion order.
 * @param definition {{
 *   @field varname {string} : name of the variable to set
 *   @field rawjson {string} : stringified JSON to produce
 *   @field detectUnits {boolean} : Applicable strings are parsed into a ValueWithUnits. For instance, "3 inch" will map to a ValueWithUnits with length units that repreresents 3 inches.
 *   @field [description=default] {string} : description field for the variable; if blank, will show a snippet of the raw JSON
 * }}
 *
 * @TODO use editing logic to have it accept defaults -- either a string (partsed to JSON) or a map -- that are merged using mergeMaps()
 */
annotation { "Feature Type Name": "JSON Var", "Icon": IconNamespace::BLOB_DATA, "Feature Type Description": "Parse JSON into a variable", "Feature Name Template": "JSON Var #varname" }
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
  const detectUnits = ifNil(definition.detectUnits, true);
  var jsondata = undefined;
  // debug(context, [varname, rawjson, description, detectUnits, definition]);
  try {
    jsondata = detectUnits ? parseJsonWithUnits(rawjson) : parseJson(rawjson);
    debug(context, jsondata);
  } catch (error) {
    debug(context, ["Error parsing JSON", varname, error]);
    debug(context, rawjson);
    throw regenError("Could not parse JSON for variable #" ~ varname ~ ": " ~ error ~ " -- " ~ strTake(rawjson, 10000));
  }
  setVariable(context, varname, jsondata, description);
});

// == [Bag Keys] ==

/**
 * Part feature: extrudes `text` sized and aligned within a bounding plate at each selected plane.
 * Delegates all geometry to @see `textChip`.
 * @param definition {{
 *   @field json {string} : stringified JSON to produce
 * }}
 */
annotation { "Feature Type Name": "Key List", "Icon": IconNamespace::BLOB_DATA, "Feature Type Description": "Ordered Keys of a Map", "Feature Name Template": "Keys of #bagname" }
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
  const varname     = ifBlank(definition.varname, bagname ~ '_keys');
  const description = ifBlank(definition.description, strTake("Keys of #" ~ bagname, 50));

  const bag         = strBlank(bagname) ? {} : getVariable(context, bagname);
  var keysarr;
  if        (bag is array) {
    keysarr = range(0, size(bag));
  } else if (bag is map) {
    keysarr = keys(bag);
  } else {
    throw "Map Keys feature needs a bag or an array but variable " ~ varname ~ " was " ~ bag;
  }
  setVariable(context, varname, keysarr, description);
});

// == [Pick Values from a Bag] ==

/**
 * Part feature: extrudes `text` sized and aligned within a bounding plate at each selected plane.
 * Delegates all geometry to @see `textChip`.
 * @param definition {{
 *   @field json {string} : stringified JSON to produce
 * }}
 */
annotation { "Feature Type Name": "Values At Keylist", "Icon": IconNamespace::BLOB_DATA, "Feature Type Description": "Array of selected map values", "Feature Name Template": "Keys of #bagname -> #varname" }
export const valuesAtF = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Source Map Variable Name" }
  definition.bagname is string;
  annotation { "Name": "Result Keys Variable Name" }
  definition.varname is string;
  annotation { "Name": "JSON Ordered List of Keys", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  definition.keylistJSON is string;
  annotation { "Name": "Variable Description",       "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
  definition.description is string;
}
{
  const bagname     = ifBlank(definition.bagname, '');
  const varname     = ifBlank(definition.varname, bagname ~ '_keys');
  const description = ifBlank(definition.description, strTake("Keys of #" ~ bagname, 50));
  const bag         = strBlank(bagname) ? {} : getVariable(context, bagname);
  //
  const keylist     = parseJson(ifBlank(definition.keylistJSON, "[]"));
  const vals        = valuesAt(bag, keylist);
  //
  setVariable(context, varname, vals, description);
});


// == [Turn data container elements into eponymous variables] ==

/**
 * Part feature: extrudes `text` sized and aligned within a bounding plate at each selected plane.
 * Delegates all geometry to @see `textChip`.
 * @param definition {{
 *   @field json {string} : stringified JSON to produce
 * }}
 */
annotation {
    "Feature Type Name": "Splat Variables from Data Container", "Icon": IconNamespace::BLOB_DATA,
    "Feature Type Description": "Variables for elements of map/array",
    "Feature Name Template": "Values of #objname",
    "Editing Logic Function" : "splatEditLogic"
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
    forEach(obj, MissingPolicy.SKIP, (val, seq, _) => {
      const keyname = objname ~ "_" ~ padLeft(seq, 2, '0');
      setVariable(context, keyname, val, objname ~ "[" ~ keyname ~ "]");
    });
  } else {
    const prefixVarnames = ifNil(definition.prefixVarnames, true);
    forEach(obj, MissingPolicy.SKIP, (val, keyname, _) => {
      const fieldvar = (prefixVarnames ? field_varname(objname, keyname) : sanitize_varname(keyname));
      setVariable(context, fieldvar, val, objname ~ "." ~ keyname);
    });
  }
});

function sanitize_varname(varname is string) returns string {
    return replace(replace(varname, '\\.', "__"), '\\W', '_');
}
function field_varname(varname is string, fieldname is string) returns string {
    return varname ~ '__' ~ sanitize_varname(fieldname);
}

export function splatEditLogic(context is Context, id is Id, oldDefinition is map, definition is map, isCreating is boolean, specifiedParameters is map) returns map {
    debug(context, oldDefinition);
    debug(context, definition);
    debug(context, specifiedParameters);
    const objname = definition.objname;
    const obj = strBlank(objname) ? {} : getVariable(context, objname);
    definition.objIsArray = (obj is array);
    return definition;
}

// // == [Merge Maps] ==

// /**
//  * Utility Feature: merge a map with
//  * @param definition {{
//  *   @field varname {string} : name of the variable to set
//  *   @field rawjson {string} : stringified JSON to produce
//  *   @field detectUnits {boolean} : Applicable strings are parsed into a ValueWithUnits. For instance, "3 inch" will map to a ValueWithUnits with length units that repreresents 3 inches.
//  *   @field [description=default] {string} : description field for the variable; if blank, will show a snippet of the raw JSON
//  * }}
//  */
// annotation { "Feature Type Name": "JSON Var", "Icon": IconNamespace::BLOB_DATA, "Feature Type Description": "Parse JSON into a variable", "Feature Name Template": "JSON Var #varname" }
// export const mergeMapsVarF = defineFeature(function(context is Context, id is Id, definition is map)
// precondition {
//   annotation { "Name": "Variable Name" }
//   definition.varname is string;
//   annotation { "Name": "JSON String" }
//   definition.rawjson is string;
//   annotation { "Name": "Detect Units",               "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
//   definition.detectUnits is boolean;
//   annotation { "Name": "Variable Description",       "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
//   definition.description is string;
// }
// {
//   const varname     = ifBlank(definition.varname, 'jsondata');
//   const rawjson     = ifBlank(definition.rawjson, '{"oops":"empty input"}');
//   const description = ifBlank(definition.description, strTake("Parsed from JSON: " ~ rawjson, 200));
//   const detectUnits = ifNil(definition.detectUnits, true);
//   var jsondata = undefined;
//   // debug(context, [varname, rawjson, description, detectUnits, definition]);
//   try {
//     jsondata = detectUnits ? parseJsonWithUnits(rawjson) : parseJson(rawjson);
//   } catch (error) {
//     debug(context, ["Error parsing JSON", varname, error]);
//     debug(context, rawjson);
//     throw regenError("Could not parse JSON for variable #" ~ varname ~ ": " ~ error ~ " -- " ~ strTake(rawjson, 10000));
//   }
//   setVariable(context, varname, jsondata, description);
// });
