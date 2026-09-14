FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
//
import(path : "54590bc1c9cee0141b968fbb", version : "c6288b0ee53a3fd79915562f"); // clxn for flatMap, mapValues
import(path : "66e287bede293cb227dfb89c", version : "fdb086499ae795a21af26725"); // ifNil &c
import(path : "dd812faf6ff4099cda4aa0eb", version : "fe205ef425e3a7b0e6f43229"); // strUtils for hasNatch
import(path : "08b6ba15b8255611bafe7520", version : "1e6e0baae135d1e3f89931d2"); // helperFuncs, for iteratee & identity

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
 * `sort` *(std)*, extended with `pairs`' criteria already computed: sorts `pairs` (each a
 * `[criteria, val]`) by their `criteria` slot and returns just the `val`s, in `order`'s direction.
 * The multiply-by-`order` trick works for any comparator, not just a `-1/0/1` one: `order`'s sign
 * flips or keeps `comparator`'s sign, and `order == 0` collapses every comparison to `0`, which
 * `sort`'s stable merge leaves in `pairs`' original order -- a "neuter" pass-through.
 * @param pairs {array}: `[criteria, val]` tuples.
 * @param order {number}: Positive sorts ascending by `criteria`, negative descending, zero leaves `pairs` in their original order.
 * @param comparator {function}: `(criteriaA, criteriaB) => number`.
 */
function sortPairsBy(pairs is array, order is number, comparator is function) returns array {
  const sortedPairs = sort(pairs, function(pairA is array, pairB is array) returns number {
    return order * comparator(pairA[0], pairB[0]);
  });
  return mapValues(sortedPairs, function(pair is array, _seq is number) { return pair[1]; });
}

/**
 * `vals`, ordered by `iterateeSpec`: `vals`'s elements (array) or values (map, keys discarded),
 * stably sorted by `iteratee(iterateeSpec)`'s result for each, compared with `comparator`.
 * Defaults to `cmp`, so an `iterateeSpec` producing incompatible types across elements -- e.g. a
 * mix of numbers and strings -- throws exactly as `cmp` does; @see `orderAnyBy` for a version that
 * never throws. Lodash's `orderBy` accepts one-or-many iteratees and orders; this accepts just one
 * of each.
 * @param vals {array|map}: Collection to sort.
 * @param iterateeSpec: Ducktyped @see `iteratee` -- a function `(val, seq|key) => criteria`, a map
 *   (matches predicate), or a string (property path). Defaults to `identity`.
 * @param order {number}: Positive sorts ascending, negative descending, zero leaves `vals` in its
 *   original order regardless of `iterateeSpec`. Defaults to `1`.
 * @param comparator {function}: `(criteriaA, criteriaB) => number`, applied to pairs of
 *   `iterateeSpec`'s results. Defaults to `cmp`.
 * @example
 *   orderBy([3, 1, 2]);                                // => [1, 2, 3]
 *   orderBy([{ "n": 3 }, { "n": 1 }], "n");             // => [{ "n": 1 }, { "n": 3 }]
 *   orderBy([1, 2, 3], identity, -1);                   // => [3, 2, 1]
 *   orderBy({ "a": 3, "b": 1 }, identity);              // => [1, 3]
 */
export function orderBy(vals is array, iterateeSpec, order is number, comparator is function) returns array {
  const keyFn = iteratee(iterateeSpec);
  const pairs = mapValues(vals, function(val, seq is number) { return [keyFn(val, seq), val]; });
  return sortPairsBy(pairs, order, comparator);
}
export function orderBy(vals is map, iterateeSpec, order is number, comparator is function) returns array {
  const keyFn = iteratee(iterateeSpec);
  const pairs = mapValues(keys(vals), function(key is string, _seq is number) { return [keyFn(vals[key], key), vals[key]]; });
  return sortPairsBy(pairs, order, comparator);
}
export function orderBy(vals, iterateeSpec, order is number) returns array { return orderBy(vals, iterateeSpec, order, cmp); }
export function orderBy(vals, iterateeSpec) returns array { return orderBy(vals, iterateeSpec, 1, cmp); }
export function orderBy(vals) returns array { return orderBy(vals, identity, 1, cmp); }

/**
 * `orderBy`, with `cmpAny` as the comparator -- a total ordering even across mixed/incompatible
 * types in `iterateeSpec`'s results, so this never throws where `orderBy` might.
 * @param vals {array|map}: Collection to sort.
 * @param iterateeSpec: @see `orderBy`. Defaults to `identity`.
 * @param order {number}: @see `orderBy`. Defaults to `1`.
 * @example
 *   orderAnyBy([1, "2", 0]); // => [0, 1, "2"] -- number sorts before string, per cmpAny's fstypenum fallback
 */
export function orderAnyBy(vals, iterateeSpec, order is number) returns array { return orderBy(vals, iterateeSpec, order, cmpAny); }
export function orderAnyBy(vals, iterateeSpec) returns array { return orderAnyBy(vals, iterateeSpec, 1); }
export function orderAnyBy(vals) returns array { return orderAnyBy(vals, identity, 1); }

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
