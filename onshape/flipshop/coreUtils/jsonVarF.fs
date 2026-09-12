FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
import(path : "4ebdc64943b566160ea5cc28", version : "3684960ff67d0db556614489");
import(path : "08b6ba15b8255611bafe7520", version : "2a3b23fb0cdd7a00cb549f43");
import(path : "6fcd20533bd2df7c4094a0bf", version : "5849e1654325cddeda6f7440");

IconNamespace::import(path : "ce7a4dfe88a5752b501f7baf", version : "2f0856d649310aa73a24518e");


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
  // const detectUnits = ifNil(definition.detectUnits, true);
  var jsondata = parseJsonSafely(rawjson, { detectUnits: ifNil(definition.detectUnits, true), story: "jsonVarF:" ~ varname });
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
  // const varname     = ifBlank(definition.varname, bagname ~ '_keys');
  const description = ifBlank(definition.description, strTake("Keys of #" ~ bagname, 50));

  const bag         = strBlank(bagname) ? {} : getVariable(context, bagname);
  var keysarr;
  if        (bag is array) {
    keysarr = range(0, size(bag));
  } else if (bag is map) {
    keysarr = keys(bag);
  } else {
    throw "Map Keys feature needs a bag or an array but variable " ~ definition.varname ~ " was " ~ bag;
  }
  setVariable(context, definition.varname, keysarr, description);
});

export function keylistEditLogic(context is Context, id is Id, oldDefinition is map, newDefinition is map, isCreating is boolean, specifiedParameters is map) returns map {
    // debug(context, ["keylistEditLogic", oldDefinition]);
    // debug(context, ["keylistEditLogic", newDefinition]);
    // debug(context, ["keylistEditLogic", specifiedParameters]);
    newDefinition.varname = defaultMaybe(oldDefinition, newDefinition, "bagname", "varname", (oname, _) => (oname ~ '_keys'));
    return newDefinition;
}

// == [SizeofF] ==

/**
 * Part feature: extrudes `text` sized and aligned within a bounding plate at each selected plane.
 * Delegates all geometry to @see `textChip`.
 * @param definition {{
 *   @field json {string} : stringified JSON to produce
 * }}
 */
annotation { "Feature Type Name": "Size of", "Icon": IconNamespace::BLOB_DATA, "Feature Type Description": "Size of a string/bag/list",
"Feature Name Template": "Size of #objname -> #varname", "Editing Logic Function" : "sizeofEditLogic"
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
  //   const varname     = ifBlank(definition.varname, objname ~ '_size');

  const obj         = strBlank(objname) ? {} : getVariable(context, objname);
  setVariable(context, definition.varname, sizeof(obj), description);
});

export function sizeofEditLogic(context is Context, id is Id, oldDefinition is map, newDefinition is map, isCreating is boolean, specifiedParameters is map) returns map {
    newDefinition.varname = defaultMaybe(oldDefinition, newDefinition, "objname", "varname", (oname, _) => (oname ~ '_size'));
    return newDefinition;
}

// == [Pick Values from a Bag] ==

/**
 * Values At Feature: ordered array of values from a map/array at given keys/indexes. Specify a variable name for the input source map/array and an output variable for the result array.
 * @param definition {{
 *   @field json {string} : stringified JSON to produce
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
  const varname     = ifBlank(definition.varname, bagname ~ '_vals');
  const bag         = strBlank(bagname) ? {} : getVariable(context, bagname);
  //
  const keylist     = parseJson(ifBlank(definition.keylistJSON, "[]"));
  const vals        = valuesAt(bag, keylist);
  const description = ifBlank(definition.description, strTake("Selected Values of #" ~ bagname ~ "[" ~ definition.keylistJSON ~ "]", 200));
  //
  setVariable(context, varname, vals, description);
});
export function valuesAtEditLogic(context is Context, id is Id, oldDefinition is map, newDefinition is map, isCreating is boolean, specifiedParameters is map) returns map {
    newDefinition.varname = defaultMaybe(oldDefinition, newDefinition, "bagname", "varname", (oname, _) => (oname ~ '_vals'));
    newDefinition.description = defaultMaybe(oldDefinition, newDefinition, "bagname", "description", (bagname, defn) => (
        strTake("Selected Values of #" ~ bagname ~ ifBlank(defn.keylistJSON, ""), 200))
    );
    return newDefinition;
}


// == [Turn data container elements into eponymous variables] ==

/**
 * Data feature: Extract named variables from a map/array.
 * If the`varname` variable is an array `${varname}_${index}`, eg `positionVector_00`, `positionVector_01`, etc
 * If `prefixVarnames` is true, variables will be named ${varname}_${keyname}`, eg `coords_x`, `coords_y`, etc
 * If `prefixVarnames` is false, variables will be named for the keyname`, eg `x`, `y`, etc
 *
 * @param definition {{
 *   @field json {string} : stringified JSON to produce
 * }}
 */
annotation {
    "Feature Type Name":        "Splat Variables", "Icon": IconNamespace::BLOB_DATA,
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