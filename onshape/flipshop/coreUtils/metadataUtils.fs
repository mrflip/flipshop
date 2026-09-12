FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
// import(path : "4ebdc64943b566160ea5cc28", version : "030f1c3158058ab3c122092a");
import(path : "dd812faf6ff4099cda4aa0eb", version : "ccf1aec9c08b9ad59691b4d4");
import(path : "66e287bede293cb227dfb89c", version : "e6c9aacc05cf0a6f15c7d4c7");


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
export function setPropAndAttribute(context is Context, entities is Query, propType, attrName is string, value is string) returns string {
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
 * Sets the NAME property and a "Name" attribute on entities.
 * @param context {Context}
 * @param entities {Query}
 * @param name {string}
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

export function getNames(context is Context, query is Query, ignoredVal is string) returns array {
  const nameProps = getNameProps(context, query, ignoredVal);
  return mapArray(nameProps, (result) => result.val);
}

export function getName(context is Context, query is Query, ignoredVal is string) returns string {
  const result = getNameProp(context, query, ignoredVal);
  return result.val;
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

export function getNameOfBody(context is Context, body is Query, defaultVal is string) {
  const nameAttr = getAttributes(context, { "entities" : body, "name": "Name" });
  if ((size(nameAttr) == 0) || (nameAttr[0] == undefined)) { return defaultVal; }
  return nameAttr[0];
}

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

export function sanitize_varname(varname is string) returns string {
    return replace(replace(varname, '\\.', "_"), '\\W', '__');
}
export function field_varname(varname is string, fieldname is string) returns string {
    return varname ~ '_' ~ sanitize_varname(fieldname);
}