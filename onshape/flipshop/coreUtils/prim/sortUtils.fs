FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
//
import(path : "54590bc1c9cee0141b968fbb", version : "4bd6c845a2f67b3868ae7632"); // clxn for flatMap, mapValues
import(path : "66e287bede293cb227dfb89c", version : "f5db6953645e7ce84f7591c0"); // ifNil &c
import(path : "dd812faf6ff4099cda4aa0eb", version : "4e9280895db387eb34594ef9"); // strUtils for hasNatch
import(path : "08b6ba15b8255611bafe7520", version : "f10d0d385cd8835dbe66a5dc"); // helperFuncs, for iteratee & identity
import(path : "607f97fc690581579d1d4a08", version : "edd393c457dca9e8660ae895"); // clxnGetset, for iteratee

export function strOrderBy(vals is array) returns array {
  var bucket = {};
  for (var ii = 0; ii < size(vals); ii += 1) {
    const str = "" ~ vals[ii];
    bucket[str] = ifNil(bucket[str], 0) + 1;
  }
  return flatMap(bucket, (reps, val) => makeArray(reps, val));
}

export function strOrderByUniq(vals is array) returns array {
  var bucket = {};
  for (var ii = 0; ii < size(vals); ii += 1) {
    const str = vals[ii];
    bucket["" ~ str] = ii;
  }
  return keys(bucket);
}

/** Compares two values.
 * Follows the philosophy of `std`'s `cmpTo` -- it does not invent orderings, but if there is an obvious one, it uses it.
 * Errors caused by incompatible types -- e.g. comparing a number with a string -- will in general manifest as a method-not-found error
 *
 * * Array: compares pairwise; the array that compares as smaller or that ends first is smaller
 * * String: compares lexicographically, according to the rules FS uses for sorting map keys
 * * Number: compares numerically
 * * ValueWithUnits: compares numerically; units must be compatible
 * * Boolean: true is greater than false, false is equal to false, true is equal to true
 * * Undefined: equal to itself and nothing else; comparing with anything else is an error
 * * Box: compares the value inside the box; if the box is empty, or contains undefined, it compares as undefined
 * * Map: compares the keys lexicographically; if the keylists are identical, compares the lists of values pairwise
 */
export const cmp = (function(aa, bb) returns number { return cmpTo(aa, bb); });

/** Compares two values to get a total ordering, even where types are incompatible.
 * - If the values are equal, returns 0.
 * - Boxes are compared by their contents.
 * - Values of the same type are compared by `cmpTo`.
 * - Values with compatible units are compared by their value
 * - Values with incompatible units are compared by their units lexicographically, which may not be perfectly stable.
 * - Values of different types are compared by their fstypenum number.
 */
export const cmpAny = (function(aa, bb) returns number {
  if (aa == bb) { return 0; }
  if (aa is box) { return cmpAny(aa[], bb is box ? bb[] : bb); }
  const atype = fstypename(aa);
  if (atype == fstypename(bb)) {
    if (atype == FSTypename.VALUEWITHUNITS) {
      if (aa.unit == bb.unit) { return cmpTo(aa.value, bb.value); }
      return cmpTo(aa.unit, bb.unit);
    }
    return cmpTo(aa, bb, cmpAny);
  }
  return fstypenum(aa) < fstypenum(bb) ? -1 : 1;
});

export function cmpTo(aa is array, bb is array, comparator is function) returns number {
  const len = min(size(aa), size(bb));
  for (var ii = 0; ii < len; ii += 1) {
    const result = comparator(aa[ii], bb[ii]);
    if (result != 0) { return result; }
  }
  return size(aa) - size(bb);
}
export function cmpTo(aa is array, bb is array) returns number { return cmpTo(aa, bb, cmp); }

export function cmpMapByKeysOnly(aa is map, bb is map, comparator is function) returns number {
  return cmpTo(keys(aa), keys(bb), comparator);
}
export function cmpMapByKeysOnly(aa is map, bb is map) returns number { return cmpMapByKeysOnly(aa, bb, cmp); }

export function cmpTo(aa is map, bb is map, comparator is function) returns number {
  const bykeys = cmpMapByKeysOnly(aa, bb, comparator);
  if (bykeys != 0) { return bykeys; }
  return cmpTo(values(aa), values(bb), comparator);
}
export function cmpTo(aa is map, bb is map) returns number { return cmpTo(aa, bb, cmp); }

/** Undefined is equal to itself and nothing else -- comparing with anything else is an error */
export function cmpTo(aa is undefined, bb is undefined) returns number {
  return 0;
}
export function cmpTo(aa is undefined,   bb is undefined, cf is function) returns number { return 0; }

/** true is greater than false; booleans cannot compare with anything else */
export function cmpTo(aa is boolean, bb is boolean) returns number {
  if (aa == bb) { return 0; } return aa ? 1 : -1;
}
export function cmpTo(aa is boolean, bb is boolean, cf is function) returns number { return cmpTo(aa, bb); }

/** Compares numbers numerically using `tolerantEquals` */
export function cmpTo(aa is number, bb is number) returns number {
  if (tolerantEquals(aa, bb)) { return 0; }
  return aa < bb ? -1 : 1;
}
export function cmpTo(aa is number, bb is number, cf is function) returns number { return cmpTo(aa, bb); }

/** Compares ValueWithUnits numerically using `tolerantEquals`; the units must be compatible */
export function cmpTo(aa is ValueWithUnits, bb is ValueWithUnits) returns number {
  if (tolerantEquals(aa, bb)) { return 0; }
  return aa < bb ? -1 : 1;
}
export function cmpTo(aa is ValueWithUnits, bb is ValueWithUnits, cf is function) returns number { return cmpTo(aa, bb); }

/** Compares strings lexicographically; the strings must be compatible. String comparison is slow and stupid */
export function cmpTo(aa is string, bb is string) returns number {
  if (aa == bb) { return 0; }
  const sorted = strOrderBy([aa, bb]);
  if (sorted[0] == ('' ~ aa)) { return -1; }
  if (sorted[0] == ('' ~ bb)) { return  1; }
  throw "The sort did not work: " ~ sorted ~ " for comparing " ~ [aa, bb];
}
export function cmpTo(aa is string, bb is string, cf is function) returns number { return cmpTo(aa, bb); }

export function cmpTo(aa is box, bb is box, cf is function) returns number {
  return cmpTo(aa[], bb[], cf);
}
export function cmpTo(aa is box, bb is box) returns number { return cmpTo(aa, bb, cmp); }

/**
 * `[val, ckey]` for each entry of `collection` -- an array's elements paired with their index, a
 * map's values with their key. The `ckey` rides along only far enough to reach each `funcOrPath`;
 * the sorted result is `val`s alone.
 *
 * @param collection {array|map}: Collection to walk.
 *
 * @returns {array}: `[val, ckey]` pairs, in `collection`'s own order.
 */
function valCkeysFor(collection is array) returns array {
  return mapValues(collection, (val, idx is number) => [val, idx]);
}
function valCkeysFor(collection is map) returns array {
  return mapValues(keys(collection), (key is string, _seq) => [collection[key], key]);
}

/**
 * One sort axis per element of `funcOrPaths`, each coerced through `iteratee` -- a function is used
 * as-is, a dotkey string reads that property, a map becomes a `matches` rule. A lone `funcOrPath`
 * is a single axis; `undefined` or an empty array name no axis at all, which is `identity`.
 *
 * @seeAlso [iteratee]
 *
 * @param funcOrPaths {function|string|map|array}: One `funcOrPath` per axis, or a lone `funcOrPath`.
 *
 * @returns {array}: Functions, each invoked as `(val, ckey)`.
 */
function axisFuncsFor(funcOrPaths) returns array {
  if (! (funcOrPaths is array)) { return [iteratee(funcOrPaths)]; }
  if (size(funcOrPaths) == 0)   { return [identity]; }
  return mapValues(funcOrPaths, (funcOrPath, _idx) => iteratee(funcOrPath));
}

/**
 * `spec` spread across `axisCount` sort axes: an array fills axes positionally, padding whatever
 * it runs short of with `fallback`; any other value broadcasts itself to every axis; `undefined`
 * leaves every axis at `fallback`. This is how `orders` and `comparators` each accept a bare
 * value, a per-axis array, or nothing at all.
 *
 * @param spec: Per-axis array, a bare value to broadcast, or `undefined`.
 * @param axisCount {number}: Number of sort axes to fill.
 * @param fallback: Stands in for axes `spec` leaves unspoken for.
 *
 * @returns {array}: Exactly `axisCount` values.
 */
function axisValsFor(spec, axisCount is number, fallback) returns array {
  if (spec is undefined)  { return makeArray(axisCount, fallback); }
  if (! (spec is array))  { return makeArray(axisCount, spec); }
  const spokenFor = min(axisCount, size(spec));
  var axisVals = makeArray(axisCount, fallback);
  for (var ii = 0; ii < spokenFor; ii += 1) {
    if (! (spec[ii] is undefined)) { axisVals[ii] = spec[ii]; }
  }
  return axisVals;
}

/**
 * `critsA` against `critsB` axis by axis, stopping at the first axis that separates them.
 * Each axis's `comparator` result is multiplied by that axis's `order`, so a negative `order`
 * flips its direction; an `order` of `0` skips the axis outright, so its `comparator` never runs
 * and its criteria never have to be comparable.
 *
 * @param critsA {array}: One `funcOrPath` result per axis.
 * @param critsB {array}: Likewise, for the element being compared against.
 * @param axisOrders {array}: One `order` number per axis.
 * @param axisComparators {array}: One `comparator` function per axis.
 *
 * @returns {number}: Negative if `critsA` sorts first, positive if `critsB` does, zero if the axes cannot separate them.
 */
function cmpCrits(critsA is array, critsB is array, axisOrders is array, axisComparators is array) returns number {
  for (var ii = 0; ii < size(critsA); ii += 1) {
    if (axisOrders[ii] == 0) { continue; }
    const comparator = axisComparators[ii];
    const result = axisOrders[ii] * comparator(critsA[ii], critsB[ii]);
    if (result != 0) { return result; }
  }
  return 0;
}

/**
 * Shared body of `orderBy` and `orderAnyBy`, once `fallbackComparator` says which comparator the
 * axes that named none of their own should use.
 *
 * @param collection {array|map}: Collection to iterate over.
 * @param funcOrPaths: One `funcOrPath` per sort axis @see `axisFuncsFor`.
 * @param orders: One `order` per sort axis, or a bare number to broadcast @see `axisValsFor`.
 * @param comparators: One `comparator` per sort axis, or a bare function to broadcast @see `axisValsFor`.
 * @param fallbackComparator {function}: Comparator for any axis `comparators` leaves unspoken for.
 *
 * @returns {array}: The new sorted array.
 */
function orderedValsFor(collection, funcOrPaths, orders, comparators, fallbackComparator is function) returns array {
  const axisFuncs       = axisFuncsFor(funcOrPaths);
  const axisOrders      = axisValsFor(orders, size(axisFuncs), 1);
  const axisComparators = axisValsFor(comparators, size(axisFuncs), fallbackComparator);
  const critsVals = mapValues(valCkeysFor(collection), function(valCkey is array, _seq) returns array {
    const val  = valCkey[0];
    const ckey = valCkey[1];
    return [mapValues(axisFuncs, (axisFunc, _idx) => axisFunc(val, ckey)), val];
  });
  const sortedCritsVals = sort(critsVals, function(critsValA is array, critsValB is array) returns number {
    return cmpCrits(critsValA[0], critsValB[0], axisOrders, axisComparators);
  });
  return mapValues(sortedCritsVals, (critsVal is array, _idx) => critsVal[1]);
}

/** Creates an array of elements, sorted in ascending order by the results of
 * running each element in a collection thru each `funcOrPath`.
 * If the `order` for any axis is unspecified, all values are sorted in the ascending order given by the comparator.
 * Otherwise, specify the sort order of corresponding values
 * as -1 for descending, 1 for ascending, and 0 to ignore that axis.
 * This method performs a stable sort, that is, it preserves the original sort order of
 * equal elements.
 * Each `funcOrPath` is invoked as `(val, ckey)`.
 *
 * @seeAlso [orderAnyBy]
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param funcOrPaths {function|string|array}: One `funcOrPath` per sort axis; defaults to `[identity]`.
 *   An array is always read as a list of axes, so a deep path wants its dotkey spelling -- `"a.b"`, not `["a", "b"]`.
 *   @optional
 * @param orders {number|number[]}: The sort orders of `funcOrPaths`. If a bare number is given, it is used for all axes. If there are more funcOrPaths than `orders`, the remaining ones will sort ascending (the default)
 *   @optional
 * @param comparators {function|function[]}: `(criteriaA, criteriaB) => number`, applied to pairs of `funcOrPath`'s results. If a bare function is given, it is used for all axes. If there are more funcOrPaths than `comparators`, the remaining ones will use cmp (or cmpAny if called as orderAnyBy);
 *   Defaults to `cmp` -- incompatible types throw. Pass `cmpAny` to allow mixed types, or use the orderAnyBy convenience function.
 *   @optional
 *
 * @returns {array}: the new sorted array.
 *
 * @example `var users = [ { 'user': 'fred', 'age': 48 }, { 'user': 'barney', 'age': 34 }, { 'user': 'fred', 'age': 40 }, { 'user': 'barney', 'age': 36 } ]; orderBy(users, ['user', 'age'], [1, -1]); // => objects for [['barney', 36], ['barney', 34], ['fred', 48], ['fred', 40]]`
 * @example `orderBy([3, 1, 2]); // => [1, 2, 3]`
 * @example `orderBy([{ "n": 3 }, { "n": 1 }], "n"); // => [{ "n": 1 }, { "n": 3 }]`
 * @example `orderBy([1, 2, 3], identity, -1); // => [3, 2, 1]`
 * @example `orderBy({ "a": 3, "b": 1 }, identity); // => [1, 3]`
 */
export function orderBy(collection, funcOrPaths, orders, comparators) returns array {
  return orderedValsFor(collection, funcOrPaths, orders, comparators, cmp);
}
export function orderBy(collection, funcOrPaths, orders) returns array { return orderBy(collection, funcOrPaths, orders, undefined); }
export function orderBy(collection, funcOrPaths) returns array         { return orderBy(collection, funcOrPaths, undefined, undefined); }
export function orderBy(collection) returns array                      { return orderBy(collection, undefined, undefined, undefined); }

/**
 * `orderBy`, with `cmpAny` standing in wherever a `comparator` goes unnamed -- a total ordering
 * even across mixed or otherwise incompatible types among a `funcOrPath`'s results, so this never
 * throws where `orderBy` might.
 *
 * @seeAlso [orderBy]
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param funcOrPaths {function|string|array}: Options for @see `orderBy`.
 *   @optional
 * @param orders {number|number[]}: Options for @see `orderBy`.
 *   @optional
 * @param comparators {function|function[]}: Options for @see `orderBy`; unnamed axes get `cmpAny` rather than `cmp`.
 *   @optional
 *
 * @returns {array}: the new sorted array.
 *
 * @example `orderAnyBy([1, "2", 0]); // => [0, 1, "2"] -- number sorts before string, per cmpAny's fstypenum fallback`
 */
export function orderAnyBy(collection, funcOrPaths, orders, comparators) returns array {
  return orderedValsFor(collection, funcOrPaths, orders, comparators, cmpAny);
}
export function orderAnyBy(collection, funcOrPaths, orders) returns array { return orderAnyBy(collection, funcOrPaths, orders, undefined); }
export function orderAnyBy(collection, funcOrPaths) returns array         { return orderAnyBy(collection, funcOrPaths, undefined, undefined); }
export function orderAnyBy(collection) returns array                      { return orderAnyBy(collection, undefined, undefined, undefined); }

// • undefined: Represents an unassigned or empty value, with only one possible value ().
// • boolean: A logical truth value, limited to  and .
// • number: An IEEE 64-bit floating-point value, which includes standard numeric values as well as , , and .
// • string: A standard text sequence stored as Unicode.
// • array: A fixed-size, heterogeneous collection of items indexed using small integers.
// • map: A heterogeneous collection of key-value pairs indexed by values.
// • box: A mutable container that holds a single value which can be changed over time.
// • builtin: An opaque internal type that is handled and understood directly by the runtime system.
// • function: A closure or lambda expression created using a function expression. [1]

enum FSTypename {
  UNDEFINED,
  BOOLEAN,
  NUMBER,
  VALUEWITHUNITS,
  VECTOR,
  STRING,
  QUERY,
  ARRAY,
  MAP,
  BOX,
  FUNCTION,
  BUILTIN,
}

function fstypename(val is undefined) returns FSTypename { return FSTypename.UNDEFINED; }
function fstypename(val is boolean)   returns FSTypename { return FSTypename.BOOLEAN; }
function fstypename(val is number)    returns FSTypename { return FSTypename.NUMBER; }
function fstypename(val is ValueWithUnits) returns FSTypename { return FSTypename.VALUEWITHUNITS; }
function fstypename(val is Vector)    returns FSTypename { return FSTypename.VECTOR; }
function fstypename(val is string)    returns FSTypename { return FSTypename.STRING; }
function fstypename(val is Query)     returns FSTypename { return FSTypename.QUERY; }
function fstypename(val is array)     returns FSTypename { return FSTypename.ARRAY; }
function fstypename(val is map)       returns FSTypename { return FSTypename.MAP; }
function fstypename(val is box)       returns FSTypename { return FSTypename.BOX; }
function fstypename(val is function)  returns FSTypename { return FSTypename.FUNCTION; }
function fstypename(val is builtin)   returns FSTypename { return FSTypename.BUILTIN; }

function fstypenum(val is undefined)  returns number     { return  0; }
function fstypenum(val is boolean)    returns number     { return  1; }
function fstypenum(val is number)     returns number     { return 10; }
function fstypenum(val is ValueWithUnits) returns number { return 11; }
function fstypenum(val is Vector)    returns number      { return 12; }
function fstypenum(val is string)     returns number     { return 30; }
function fstypenum(val is Query)     returns number      { return 40; }
function fstypenum(val is array)      returns number     { return 81; }
function fstypenum(val is map)        returns number     { return 82; }
function fstypenum(val is box)        returns number     { return 83; }
function fstypenum(val is function)   returns number     { return 80; }
function fstypenum(val is builtin)    returns number     { return 99; }

export const BaseUnitsRE = "^(?!^,)(?:,?(ampere|kelvin|kilogram|meter|radian|second)\\b)+$";

export function isReallyValueWithUnits(val is ValueWithUnits) returns boolean {
  if (size(val) != 2) { return false; }
  return hasMatch(val.unit, BaseUnitsRE);
}
