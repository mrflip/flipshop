FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
//
import(path : "dd812faf6ff4099cda4aa0eb", version : "e85c3ea5781c0d24e61537eb"); // stringUtils, for strTake
import(path : "66e287bede293cb227dfb89c", version : "25bf5ea59817ea0aa1737abd"); // typeUtils, for strBlank &c

// Shorthand aliases used throughout for readability.
export const PL_TOP      = plane(WORLD_ORIGIN, Z_AXIS.direction);

// Sentinel values for LengthBoundSpec min/max when no practical limit applies.
export const hugeSizeVal = 1000000;
export const tinySizeVal = 0.001;


/**
 * Sets both a FeatureScript property and a same-keyed attribute on entities.
 * Use attributes (not properties) to read names back during regeneration, since
 * Onshape does not allow reading properties until the full part studio has rendered.
 * @param context {Context}
 * @param entities {Query}
 * @param propType {PropertyType} : e.g. PropertyType.NAME
 * @param attrName {string} : attribute key to mirror the value under
 * @param value {string} : the value to set
 */
export function setPropAndAttribute(context is Context, entities is Query, propType is PropertyType, attrName is string, value is string) returns string {
  setProperty(context,  { "entities": entities, "propertyType": propType, "value":     value });
  setAttribute(context, { "entities": entities, "name":         attrName, "attribute": value });
  return value;
}

/**
 * Sets the NAME property and a "Name" attribute on entities.
 * @param context {Context}
 * @param entities {Query}
 * @param name {string}
 */
export function setName(context is Context, entities is Query, nameText is string) returns string {
  return setPropAndAttribute(context, entities, PropertyType.NAME, "Name", nameText);
}

/**
 * `setName`, after collapsing `nameText`'s whitespace runs to single spaces and truncating to
 * `maxLength` — for a name that might be multi-line or arbitrarily long (e.g. copied from a
 * sketch's text) but needs to read as one short line in the part tree.
 * @param context {Context}
 * @param entities {Query}
 * @param nameText {string}
 * @param maxLength {number}: Defaults to `20`.
 */
export function setReadableName(context is Context, entities is Query, nameText is string, maxLength is number) returns string {
  const onelineName  = replace(nameText, "[\\s]+", " ");
  const readableName = strTake(onelineName, maxLength);
  return setName(context, entities, readableName);
}
export function setReadableName(context is Context, entities is Query, nameText is string) returns string {
  return setReadableName(context, entities, nameText, 20);
}

/**
 * Returns an array of `{ thing, attrName, val }` for each entity in `query`.
 * `val` is the attribute value if set, otherwise `defaultVal`.
 * @param context {Context}
 * @param query {Query}
 * @param attrName {string}
 * @param defaultVal : fallback value when the attribute is absent
 * @returns {array}
 */
export function getAttrs(context is Context, query is Query, attrName is string, defaultVal) returns array {
  const things = evaluateQuery(context, query);
  var result = [];
  for (var i = 0; i < size(things); i += 1) {
    const thing = things[i];
    const attrs = getAttributes(context, { "entities": thing, "name": attrName });
    const val = (size(attrs) > 0) ? attrs[0] : defaultVal;
    result = append(result, { "thing": thing, "attrName": attrName, "val": val });
  }
  return result;
}

/** Every attribute on `entity`, as `{name: value}`; `{"ok": false, "err": err}` if the read throws. */
export function getAllAttrs(context is Context, entity is Query) returns map {
    try {
        return getAllAttributes(context, {
                "entity" : entity,
        });
    } catch (err) {
        debug(context, "Error getting all attributes" ~ entity);
        return { "ok": false, "err": err };
    }
}

/**
 * Returns the best entry from `getAttrs`: the first entry whose value is defined
 * and not equal to `defaultVal`. Falls back to the first entry if all values equal
 * `defaultVal`. Returns `undefined` when the query resolves to no entities.
 * @param context {Context}
 * @param query {Query}
 * @param attrName {string}
 * @param defaultVal : sentinel value for "not set"
 * @returns map with fields `{ thing, attrName, thingIndex, val }`, or undefined
 */
export function getBestAttr(context is Context, query is Query, attrName is string, ignoredVal) {
  const entries = getAttrs(context, query, attrName, ignoredVal);
  if (size(entries) == 0) { return undefined; }
  for (var i = 0; i < size(entries); i += 1) {
    if (entries[i].val != ignoredVal) { return mergeMaps(entries[i], { "thingIndex": i }); }
  }
  return mergeMaps(entries[0], { "thingIndex": 0 });
}

/**
 * `getAttrs` specialised for the "name" attribute.
 * @param context {Context}
 * @param query {Query}
 * @param defaultVal {string}
 * @returns {array} of `{ thing, attrName, val }`
 */
export function getNameProps(context is Context, query is Query, ignoredVal is string) returns array {
  return getAttrs(context, query, "Name", ignoredVal);
}

/** `getNameProps`, keeping just each entry's `val`. */
export function getNames(context is Context, query is Query, ignoredVal is string) returns array {
  const nameProps = getNameProps(context, query, ignoredVal);
  return mapArray(nameProps, (result) => result.val);
}

/** `getNameProp`'s `val`, or `ignoredVal` when `query` resolves to no entities at all. */
export function getName(context is Context, query is Query, ignoredVal is string) returns string {
  const result = getNameProp(context, query, ignoredVal);
  return (result == undefined) ? ignoredVal : result.val;
}
export function getName(context is Context, query is Query) returns string {
  return getName(context, query, "Part");
}

/**
 * `getBestAttr` specialised for the "name" attribute.
 * @param context {Context}
 * @param query {Query}
 * @param defaultVal {string}
 * @returns map `{ thing, attrName, thingIndex, val }`, or undefined
 */
export function getNameProp(context is Context, query is Query, ignoredVal is string) {
  return getBestAttr(context, query, "Name", ignoredVal);
}
export function getNameProp(context is Context, query is Query) {
    return getNameProp(context, query, "Part");
}

/** `"Name"` attribute directly on `body` (not its best/first entity), or `defaultVal` if unset. */
export function getNameOfBody(context is Context, body is Query, defaultVal is string) returns string {
  const nameAttr = getAttributes(context, { "entities" : body, "name": "Name" });
  if ((size(nameAttr) == 0) || (nameAttr[0] == undefined)) { return defaultVal; }
  return nameAttr[0];
}

/**
 * Value for `newDefinition[destkey]`, keeping it auto-derived from `newDefinition[basekey]` via
 * `valfunc(baseVal, definition)` for as long as the user hasn't overridden it — the pattern
 * behind every `*EditLogic` function in this codebase that keeps a variable name in sync with
 * whatever it names (@see `jsonVarF.fs`'s `keylistEditLogic`).
 *
 * `destkey` is left alone as soon as it's set to anything other than what `valfunc` would have
 * derived: the "current default" is recomputed from `oldDefinition` and compared against
 * `oldDefinition[destkey]` to tell an untouched field from a hand-edited one.
 *
 * @param oldDefinition {map}: Definition before this edit.
 * @param newDefinition {map}: Definition being edited.
 * @param basekey {string}: Key of the field `destkey` derives from.
 * @param destkey {string}: Key of the derived field.
 * @param valfunc {function}: `(baseVal, definition) => derivedVal`.
 * @example
 *   defaultMaybe({ "bagname": "foo", "varname": "foo_keys" },
 *                { "bagname": "bar", "varname": "foo_keys" },
 *                "bagname", "varname", (name, _) => (name ~ "_keys"));
 *   // => "bar_keys" -- varname was tracking its default, so it follows bagname's rename
 *
 *   defaultMaybe({ "bagname": "foo", "varname": "myKeys" },
 *                { "bagname": "bar", "varname": "myKeys" },
 *                "bagname", "varname", (name, _) => (name ~ "_keys"));
 *   // => "myKeys" -- varname was hand-edited away from its default, so it's left alone
 */
export function defaultMaybe(oldDefinition is map, newDefinition is map, basekey is string, destkey is string, valfunc is function) {
  const baseNew = newDefinition[basekey];
  const destNew = newDefinition[destkey];
  const baseOld = oldDefinition[basekey];
  const destOld = oldDefinition[destkey];
  if (strBlank(baseNew))     { return destNew;    } // if no base, leave it alone
  const defaultNew = valfunc(baseNew, newDefinition);
  if (strBlank(destNew))     { return defaultNew; } // if dest is unset, set the default
  if (strBlank(baseOld))     { return destNew;    } // if dest couldn't have been defaulted, leave it alone
  if (baseOld == baseNew)    { return destNew;    } // if base is unchanged, leave it alone
  const defaultOld = valfunc(baseOld, oldDefinition);
  if (defaultOld == destOld) { return defaultNew; } // if old target is set to the old default, set it to the new default
  return destNew;
}

/**
 * `varname` as a legal-ish variable name: each `.` becomes `_`, and every other non-word
 * character becomes `__` — independently, so two special characters in a row don't collapse
 * into one replacement.
 * @example
 *   sanitize_varname("foo.bar");  // => "foo_bar"
 *   sanitize_varname("foo bar!"); // => "foo__bar__"
 */
export function sanitize_varname(varname is string) returns string {
    return replace(replace(varname, '\\.', "_"), '\\W', '__');
}

/** `varname ~ "_" ~ fieldname`, with `fieldname` run through `sanitize_varname` first. */
export function field_varname(varname is string, fieldname is string) returns string {
    return varname ~ '_' ~ sanitize_varname(fieldname);
}

// == [Function values] --

export const MetadataUtilsFuncs = {
  "setPropAndAttribute": (context, entities, propType, attrName, value) => setPropAndAttribute(context, entities, propType, attrName, value),
  "setName":             (context, entities, nameText)                  => setName(context, entities, nameText),
  "setReadableName":     (context, entities, nameText)                  => setReadableName(context, entities, nameText),
  "getAttrs":            (context, query, attrName, defaultVal)         => getAttrs(context, query, attrName, defaultVal),
  "getAllAttrs":         (context, entity)                              => getAllAttrs(context, entity),
  "getBestAttr":         (context, query, attrName, ignoredVal)         => getBestAttr(context, query, attrName, ignoredVal),
  "getNameProps":        (context, query, ignoredVal)                   => getNameProps(context, query, ignoredVal),
  "getNames":            (context, query, ignoredVal)                   => getNames(context, query, ignoredVal),
  "getName":             (context, query)                               => getName(context, query),
  "getNameProp":         (context, query)                               => getNameProp(context, query),
  "getNameOfBody":       (context, body, defaultVal)                    => getNameOfBody(context, body, defaultVal),
  "defaultMaybe":        (oldDefinition, newDefinition, basekey, destkey, valfunc) => defaultMaybe(oldDefinition, newDefinition, basekey, destkey, valfunc),
  "sanitize_varname":    (varname)                                      => sanitize_varname(varname),
  "field_varname":       (varname, fieldname)                           => field_varname(varname, fieldname),
};
