FeatureScript 2909;
import(path : "onshape/std/geometry.fs", version : "2909.0");

// Shorthand aliases used throughout for readability.
export const mm          = millimeter;
export const zero        = 0 * mm;
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
export function setPropAndAttribute(context is Context, entities is Query, propType, attrName is string, value is string) {
  setProperty(context,  { "entities": entities, "propertyType": propType, "value":     value });
  setAttribute(context, { "entities": entities, "name":         attrName, "attribute": value });
}

/**
 * Sets the NAME property and a "name" attribute on entities.
 * @param context {Context}
 * @param entities {Query}
 * @param name {string}
 */
export function setName(context is Context, entities is Query, nameText is string) {
  setPropAndAttribute(context, entities, PropertyType.NAME, "name", nameText);
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
export function getBestAttr(context is Context, query is Query, attrName is string, defaultVal) {
  const entries = getAttrs(context, query, attrName, defaultVal);
  if (size(entries) == 0) { return undefined; }
  for (var i = 0; i < size(entries); i += 1) {
    if (entries[i].val != defaultVal) { return mergeMaps(entries[i], { "thingIndex": i }); }
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
export function getNames(context is Context, query is Query, defaultVal is string) returns array {
  return getAttrs(context, query, "name", defaultVal);
}

/**
 * `getBestAttr` specialised for the "name" attribute.
 * @param context {Context}
 * @param query {Query}
 * @param defaultVal {string}
 * @returns map `{ thing, attrName, thingIndex, val }`, or undefined
 */
export function getName(context is Context, query is Query, defaultVal is string) {
  return getBestAttr(context, query, "name", defaultVal);
}

export function getNameOfBody(context is Context, body is Query, defaultVal is string) {
  const nameAttr = getAttributes(context, { "entities" : body, "name": "name" });
  if ((size(nameAttr) == 0) || (nameAttr[0] == undefined)) { return defaultVal; }
  return nameAttr[0];
}
