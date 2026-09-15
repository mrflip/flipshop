FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");


// TODO-helperFuncs
/** Adds two numbers.
 *
 * @param augend {number}: The first number in an addition.
 * @param addend {number}: The second number in an addition.
 *
 * @returns {number}: the total.
 *
 * @example `add(6, 4); // => 10`
 */
export function add(augend, addend) {
  if (false) { add(augend, addend); }
  throw 'TODO: implement add';
}

// TODO-helperFuncs
/** The opposite of `before`; this method creates a function that invokes
 * `func` only after it's called `minCalls` or more times.
 *
 * @param minCalls {number}: The number of calls before `func` is invoked.
 * @param func {function}: Function to restrict.
 *
 * @returns {function}: the new restricted function.
 *
 * @example `var saves = ['profile', 'settings']; var done = after(saves.length, function() { println('done saving!'); }); forEach(saves, function(type) { asyncSave({ 'type': type, 'complete': done }); }); // => Logs 'done saving!' after the two async saves have completed.`
 */
export function after(n, func) {
  if (false) { after(n, func); }
  throw 'TODO: implement after';
}

// lodash
/** Copies values of `sources` onto the destination map `bag`, left to right; a later source's key
 * overwrites an earlier one.
 *
 * @seeAlso [assignIn]
 *
 * @param bag {map}: The destination map.
 * @param sources {map}: The source maps.
 *   @optional
 *
 * @returns {map}: `object`.
 *
 * @example `assign({ 'a': 0, 'b': 1 }, { 'a': 1, 'c': 3 }); // => { 'a': 1, 'b': 1, 'c': 3 }`
 */
export function assign(bag, sources) {
  if (false) { assign(bag, sources); }
  throw 'TODO: implement assign';
}

// flipshop
/** `mergeMaps` *(std)*, with `combine(existingVal, incomingVal, key)` deciding what lands at a key
 * present in `incoming`, instead of `incoming` unconditionally winning -- returning `undefined`
 * from `combine` falls back to that default. Unlike lodash's `assignWith` customizer, `combine`
 * doesn't also receive the whole source/destination objects.
 *
 * @example `assignWith({ "a": 1 }, { "a": 2 }, (existingVal, incomingVal) => existingVal + incomingVal);`
 * @example `// => { "a": 3 }`
 */
// export function assignWith(existing, incoming, combine) {}

// lodash
/** Like `assign` except that it accepts `customizer`
 * which is invoked to produce the assigned values. If `customizer` returns
 * `undefined`, assignment is handled by the method instead. The `customizer`
 * is invoked with five arguments: (objValue, srcValue, key, object, source).
 *
 * **Note:** This method mutates `object`.
 *
 * @seeAlso [assignInWith]
 *
 * @param object {map}: The destination object.
 * @param sources {map}: The source objects.
 * @param customizer {function}: Function to customize assigned values.
 *   @optional
 *
 * @returns {map}: `object`.
 *
 * @example `function customizer(objValue, srcValue) { return isUndefined(objValue) ? srcValue : objValue; } var defaults = partialRight(assignWith, customizer); defaults({ 'a': 1 }, { 'b': 2 }, { 'a': 3 }); // => { 'a': 1, 'b': 2 }`
 */
export function assignWith(object, sources, customizer) {
  if (false) { assignWith(object, sources, customizer); }
  throw 'TODO: implement assignWith';
}

/**
 * Array of `bag`'s (or `arr`'s) values at `keylist`, in that order. Unlike lodash's `at`, there's
 * no path traversal — each entry of `keylist` is a literal key or index, not a dotted path — and
 * an array index must be non-negative and in bounds.
 *
 * `nilPolicy` decides what happens at a key/index with nothing there: `NIL` (the
 * default) fills the slot with `undefined`, so the result stays the same length as `keylist`;
 * `SKIP` drops the slot instead, so the result can come back shorter.
 *
 * @example
 *   valuesAt({ "a": 11, "b": 22 }, ["b", "a"]);                      // => [22, 11]
 *   valuesAt({ "a": 11, "b": 22 }, ["c"]);                           // => [undefined]
 *   valuesAt({ "a": 11, "b": 22 }, ["c", "b"], NilPolicy.SKIP);  // => [22]
 */
// lodash
/** Creates an array of values corresponding to `keylist` of `object`.
 *
 * @param object {map}: The map to iterate over.
 * @param keylist {array}: Each entry an `anypath` @see `getAt` -- a dotkey string or a keypath array.
 *   @optional
 *
 * @returns {array}: the picked values.
 *
 * @example `var object = { 'a': [{ 'b': { 'c': 3 } }, 4] }; at(object, ['a.0.b.c', 'a.1']); // => [3, 4]`
 */
export function valuesAt(object, keylist) {}


/**
 * Calls `func` with no arguments, returning its result — or the error it throws, caught instead
 * of propagated.
 * @seeAlso [attemptLoudly]
 *
 * @param func {function}: Function to attempt.
 * @param args: The arguments to invoke `func` with.
 *   @optional
 *
 * @returns: the `func` result or error object.
 *
 * @example
 *   attempt(function() { return 42; });    // => 42
 *   attempt(function() { throw "boom"; }); // => "boom"
 */
export function attempt(func, args) {}

// flipshop
/** Converts `str` to [camel case](https://en.wikipedia.org/wiki/CamelCase):
 * split into words and rejoined with he first word lowercased,
 * and every other word capitalized, no separators.
 *
 * @param string {str}: The string to convert; defaults to `''`.
 *   @optional
 *
 * @returns {string}: the camel cased string.
 *
 * @example `camelCase('Foo Bar'); // => 'fooBar'`
 * @example `camelCase('--foo-bar--'); // => 'fooBar'`
 * @example `camelCase('__FOO_BAR__'); // => 'fooBar'`
 */
export function camelCase(str) {}

/** Converts the first character of `str` to upper case and the remaining
 * to lower case.
 *
 * @param str {string}: The string to capitalize; defaults to `''`.
 *   @optional
 *
 * @returns {string}: the capitalized string.
 *
 * @example `capitalize('FRED'); // => 'Fred'`
 */
export function capitalize(string) {}

// lodash
/** Casts `val` as an array if it's not one.
 *
 * @param val: The value to inspect.
 *
 * @returns {array}: the cast array.
 *
 * @example `castArray(1); // => [1]`
 * @example `castArray({ 'a': 1 }); // => [{ 'a': 1 }]`
 * @example `castArray('abc'); // => ['abc']`
 * @example `castArray(null); // => [null]`
 * @example `castArray(undefined); // => [undefined]`
 * @example `castArray(); // => []`
 * @example `var array = [1, 2, 3]; println(castArray(array) === array); // => true`
 */
export function castArray(val) {}
export function manyFor(val) {}

// flipshop
/** Creates an array of elements split into groups the length of `size`.
 * If `array` can't be split evenly, the final chunk will be the remaining
 * elements.
 * chunkSize < 1` returns an empty array.
 *
 * @param array {array}: The array to process.
 * @param chunkSize {number}: The length of each chunk; defaults to `1`.
 *   @optional
 *
 * @returns {array}: the new array of chunks.
 *
 * @example `chunk(['a', 'b', 'c', 'd'], 2); // => [['a', 'b'], ['c', 'd']]`
 * @example `chunk(['a', 'b', 'c', 'd'], 3); // => [['a', 'b', 'c'], ['d']]`
 */
export function chunk(array, size) {
}
// flipshop
/** `arr` with every falsey element removed.
 * The values `false` and `undefined` are falsey; everything else is truthy.
 *
 * @param arr {array}: The array to compact.
 *
 * @returns {array}: the new array of filtered values.
 *
 * @example `compact([0, 1, false, 2, "", 3]); // => [0, 1, 2, "", 3]`
 * @example `compact([0, false, undefined]); // => [0]`
 */
export function compact(array) {
  if (false) { compact(array); }
  throw 'TODO: implement compact';
}

// flipshop
/** Builds a function that tries `pairs` in order, calling and returning the first `handler` whose
 * `rule` holds `val`, or `undefined` if none does. Each `rule` is a `ruleOrKey`, so a
 * dotpath string, `[path, srcValue]` array, or partial-match map works in place of a
 * literal `rule(val, ckey) => boolean` function -- matching lodash's own `cond`, which coerces
 * every rule the same way.
 *
 * @example `const grade = cond([ [(score, _ckey) => score >= 90, constant("A")],`
 * @example `[(score, _ckey) => score >= 80, constant("B")],`
 * @example `[constant(true), constant("F")] ]); grade(95, 0); // => "A"`
 * @example `grade(70, 0); // => "F"`
 */
// export function cond(pairs) {}

// flipshop
/** `cond`, keyed by rule instead of ordered by array position -- each map key doubles as its own
 * `ruleOrKey` (naturally a dotpath string), paired with its value as the `handler`.
 *
 * @example `const speak = cond({ "isDog": constant("Woof"), "isCat": constant("Meow") }); speak({ "isDog": true, "isCat": false }, 0); // => "Woof"`
 */
// export function cond(pairs) {}

// lodash
/** Creates a two-argument function that iterates over `ruleHandlePairs` --
 * each a ruleOrKey rule iteratees and a handler function --
 * and invokes the handler of the first rule that returns truthy.
 *
 * @param ruleHandlePairs {array}: The rule-handler pairs.
 *
 * @returns {function}: the new composite function.
 *
 * @example `var func = cond([ [matches({ 'a': 1 }), constant('matches A')], [conforms({ 'b': isNumber }), constant('matches B')], [stubTrue, constant('no match')] ]); func({ 'a': 1, 'b': 2 }); // => 'matches A'`
 * @example `func({ 'a': 0, 'b': 1 }, 0); // => 'matches B'`
 * @example `func({ 'a': '1', 'b': '2' }, 0); // => 'no match'`
 */
export function cond(ruleHandlePairs) {}

// lodash
/** Creates a function that invokes each of `rules`'s ruleOrKey iteratees
 * against the correspondingly-keyed val of a given map `bag`,
 * returning `true` if every rule returns truthy, else `false`.
 *
 * @param rules {map}: Map of key to rule to conform to.
 *
 * @returns {function}: the new spec function.
 *
 * @example `var objects = [ { 'a': 2, 'b': 1 }, { 'a': 1, 'b': 2 } ]; filter(objects, conforms({ 'b': (n, _key) => n > 1 })); // => [{ 'a': 1, 'b': 2 }]`
 */
export function conforms(source) {}

// lodash
/** Checks if `bag` conforms to `rules` by invoking
 * each of `rules`'s ruleOrKey iteratees against the
 * correspondingly-keyed val of a given map `bag`,
 * returning `true` if every rule returns truthy, else `false`.
 *
 * @param bag {map}: The map to inspect.
 * @param rules {map}: Map of key to rule to conform to.
 *
 * @returns {boolean}: `true` if `bag` conforms, else `false`.
 *
 * @example `var bag = { 'a': 1, 'b': 2 }; conformsTo(bag, { 'b': (n, _key) => n > 1 }); // => true`
 * @example `conformsTo(bag, { 'b': (n, _key) => n > 2 }); // => false`
 */
export function conformsTo(bag, rules) {}

// flipshop
/** Builds a two-argument function that always returns `val`,
 * ignoring any arguments it's called with.
 *
 * @param val: The value to return.
 *
 * @param value: The value to return from the new function.
 *
 * @returns {function}: the new constant function.
 *
 * @example `var objects = times(2, constant({ 'a': 1 })); println(objects); // => [{ 'a': 1 }, { 'a': 1 }]`
 * @example `println(objects[0] === objects[1]); // => true`
 */
export function constant(value) {}

// flipshop
/** Creates a map of keys generated from the results of running each element of `collection`
 * thru `funcOrKey`. The corresponding value of each key is the number of times the key was
 * returned by `funcOrKey`.
 *
 * @param collection {array|map}: The array or map to iterate over.
 * @param funcOrKey {function|string|array}: `(val, ckey) => key`; defaults to `identity`.
 *   @optional
 *
 * @returns {map}: the composed aggregate map.
 *
 * @example `countBy([6.1, 4.2, 6.3], Math.floor); // => { '4': 1, '6': 2 }`
 * @example `countBy(['one', 'two', 'three'], 'length'); // => { '3': 2, '5': 1 }`
 * @example `countBy([1, 2, 3, 4], (val, _ckey) => (val % 2 == 0) ? "even" : "odd"); // => { "odd": 2, "even": 2 }`
 * @example `countBy({ a: 1, b: 2 }, (val, _ckey) => (val % 2 == 0) ? "even" : "odd"); // => { "odd": 1, "even": 1 }`
 */
export function countBy(collection, funcOrKey) {}

// lodash
/** Deburrs `string` by converting
 * [Latin-1 Supplement](https://en.wikipedia.org/wiki/Latin-1_Supplement_(Unicode_block)#Character_table)
 * and [Latin Extended-A](https://en.wikipedia.org/wiki/Latin_Extended-A)
 * letters to basic Latin letters and removing
 * [combining diacritical marks](https://en.wikipedia.org/wiki/Combining_Diacritical_Marks).
 *
 * @param string {string}: The string to deburr; defaults to `''`.
 *   @optional
 *
 * @returns {string}: the deburred string.
 *
 * @example `deburr('déjà vu'); // => 'deja vu'`
 */
export function deburr(string) {
  if (false) { deburr(string); }
  throw 'TODO: implement deburr';
}

// flipshop
/** `arr`'s values that don't appear in `excludeArr`.
 * The order and references of result values are determined by the first array.
 *
 * @param arr {array}: The array to inspect.
 * @param excludeArr {array}: The values to exclude.
 *
 * @returns {array}: the new array of filtered values.
 *
 * @example `difference([2, 1], [2, 3]); // => [1]`
 */
// export function difference(arr, excludeArr) {}

// flipshop
/** Like `difference` except that it compares `arr` and `excludeArr` by `funcOrKey(val, ckey)` instead of `val`
 * itself.
 *
 * @param arr {array}: The array to inspect.
 * @param excludeArr {array}: The values to exclude.
 *   @optional
 * @param funcOrKey {function}: `(val) => criteria`; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the new array of filtered values.
 *
 * @example `differenceBy([2.1, 1.2], [2.3, 3.4], (val, _ckey) => floor(val)); // => [1.2]`
 */
// export function differenceBy(arr, excludeArr, funcOrKey) {}

// lodash
/** Like `difference` except that it accepts `funcOrKey`, invoked for each element of `array`
 * and `values` to generate the criterion by which they're compared. The order and references
 * of result values are determined by the first array.
 *
 * **Note:** Unlike `pullAllBy`, this method returns a new array.
 *
 * @param array {array}: The array to inspect.
 * @param values {array}: The values to exclude.
 *   @optional
 * @param funcOrKey {function}: `(val) => criteria`; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the new array of filtered values.
 *
 * @example `differenceBy([2.1, 1.2], [2.3, 3.4], Math.floor); // => [1.2]`
 * @example `differenceBy([{ 'x': 2 }, { 'x': 1 }], [{ 'x': 1 }], 'x'); // => [{ 'x': 2 }]`
 */
export function differenceBy(array, values, funcOrKey) {
  if (false) { differenceBy(array, values, funcOrKey); }
  throw 'TODO: implement differenceBy';
}

// flipshop
/** Like `difference` except that it compares `arr` and `excludeArr`
 * with `comparator(val, other)` instead of `==`.
 * The order and references of result values are determined by the first array.
 *
 * @param arr {array}: The array to inspect.
 * @param excludeArr {array}: The values to exclude.
 *   @optional
 * @param comparator {function}: The comparator invoked per element.
 *   @optional
 *
 * @returns {array}: the new array of filtered values.
 *
 * @example `var objects = [{ 'x': 1, 'y': 2 }, { 'x': 2, 'y': 1 }]; differenceWith(objects, [{ 'x': 1, 'y': 2 }], isEqual); // => [{ 'x': 2, 'y': 1 }]`
 */
export function differenceWith(array, values, comparator) {}

// lodash
/** Divide two numbers.
 *
 * @param dividend {number}: The first number in a division.
 * @param divisor {number}: The second number in a division.
 *
 * @returns {number}: the quotient.
 *
 * @example `divide(6, 4); // => 1.5`
 */
export function divide(dividend, divisor) {
  throw 'TODO: implement divide';
}

// flipshop
/** `arr` with the first `dropCount` elements dropped from the beginning.
 * `dropCount <= 0` returns `arr` unchanged.
 *
 * @param array {arr}: The array to query.
 * @param dropCount {number}: The number of elements to drop; defaults to `1`.
 *   @optional
 *
 * @returns {array}: the slice of `array`.
 *
 * @example `drop([1, 2, 3]); // => [2, 3]`
 * @example `drop([1, 2, 3], 2); // => [3]`
 * @example `drop([1, 2, 3], 5); // => []`
 * @example `drop([1, 2, 3], 0); // => [1, 2, 3]`
 */
export function drop(arr, dropCount) {}

// flipshop
/** `arr` with the last `dropCount` elements dropped from the end.
 *
 * @param arr {array}: The array to query.
 * @param dropCount {number}: The number of elements to drop; defaults to `1`.
 *   @optional
 *
 * @returns {array}: the slice of `array`.
 *
 * @example `dropRight([1, 2, 3]); // => [1, 2]`
 * @example `dropRight([1, 2, 3], 2); // => [1]`
 * @example `dropRight([1, 2, 3], 5); // => []`
 * @example `dropRight([1, 2, 3], 0); // => [1, 2, 3]`
 */
export function dropRight(array, n) {
  if (false) { dropRight(array, n); }
  throw 'TODO: implement dropRight';
}

// flipshop
/** `arr` with elements dropped from the end for as long as `rule` returns truthy.
 * The result holds elements from the start of the array up to and
 * including the first (i.e rightmost) element that `rule` rejects
 * `rule` is invoked as `(val, ckey)`.
 *
 * @param arr {array}: The array to query.
 * @param rule {ruleOrKey}: Coerced to a rule; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the slice of `array`.
 *
 * @example `var users = [ { 'user': 'barney', 'active': true }, { 'user': 'fred', 'active': false }, { 'user': 'pebbles', 'active': false } ]; dropRightWhile(users, function(o) { return !o.active; }); // => objects for ['barney']`
 * @example `dropRightWhile([1, 2, 3, 4], (val) => val >= 3); // => [1, 2]`
 * @example `dropRightWhile(users, { 'user': 'pebbles', 'active': false }); // => objects for ['barney', 'fred']`
 * @example `dropRightWhile(users, ['active', false]); // => objects for ['barney']`
 * @example `dropRightWhile(users, 'active'); // => objects for ['barney', 'fred', 'pebbles']`
 */
export function dropRightWhile(array, rule) {}

// flipshop
/** `arr` with elements dropped from the beginning for as long as `rule` holds.
 * The result holds elements starting from the first (i.e leftmost) element
 * that `rule` rejects and everything after it.
 *
 * @param arr {array}: The array to query.
 * @param rule {ruleOrKey}: Coerced to a rule; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the slice of `array`.
 *
 * @example `dropWhile([1, 2, 3, 4], (val) => val < 3); // => [3, 4]`
 * @example `var users = [ { 'user': 'barney', 'active': false }, { 'user': 'fred', 'active': false }, { 'user': 'pebbles', 'active': true } ]; dropWhile(users, function(o) { return !o.active; }); // => objects for ['pebbles']`
 * @example `dropWhile(users, { 'user': 'barney', 'active': false }); // => objects for ['fred', 'pebbles']`
 * @example `dropWhile(users, ['active', false]); // => objects for ['pebbles']`
 * @example `dropWhile(users, 'active'); // => objects for ['barney', 'fred', 'pebbles']`
 */
export function dropWhile(array, rule) {}

// flipshop
/** `str` with every regex metacharacter (`\ ^ $ . * + ? ( ) [ ] { } |`) preceded by a backslash,
 * so it can be matched literally.
 *
 *
 * @param str {string}: The string to escape; defaults to `''`.
 *   @optional
 *
 * @returns {string}: the escaped string.
 *
 * @example `escapeRegExp("[lodash](https://lodash.com/)"); // => "\\[lodash\\]\\(https://lodash\\.com/\\)"`
 */
export function escapeRegExp(str) {}

// flipshop
/** /** `varname ~ "_" ~ fieldname`, with `fieldname` run through `sanitize_varname` first. */
 */
// export function field_varname(varname, fieldname) {}

// lodash
/** Fills elements of `array` with `value` from `start` up to, but not
 * including, `end`.
 *
 * **Note:** This method mutates `array`.
 *
 * @param array {array}: The array to fill.
 * @param value: The value to fill `array` with.
 * @param start {number}: The start position; defaults to `0`.
 *   @optional
 * @param end {number}: The end position; defaults to `array.length`.
 *   @optional
 *
 * @returns {array}: `array`.
 *
 * @example `var array = [1, 2, 3]; fill(array, 'a'); println(array); // => ['a', 'a', 'a']`
 * @example `fill(Array(3), 2); // => [2, 2, 2]`
 * @example `fill([4, 6, 8, 10], '*', 1, 3); // => [4, '*', '*', 10]`
 */
export function fill(array, value, start, end) {
  if (false) { fill(array, value, start, end); }
  throw 'TODO: implement fill';
}

// lodash
/** Iterates over elements of `collection`, returning an array of all elements
 * `rule` returns truthy for. `rule` is invoked as `(val, ckey)`.
 *
 * **Note:** Unlike `remove`, this method returns a new array.
 *
 * @seeAlso [reject]
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param rule {ruleOrKey}: Coerced to a rule; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the new filtered array.
 *
 * @example `var users = [ { 'user': 'barney', 'age': 36, 'active': true }, { 'user': 'fred', 'age': 40, 'active': false } ]; filter(users, function(o) { return !o.active; }); // => objects for ['fred']`
 * @example `filter(users, { 'age': 36, 'active': true }); // => objects for ['barney']`
 * @example `filter(users, ['active', false]); // => objects for ['fred']`
 * @example `filter(users, 'active'); // => objects for ['barney']`
 * @example `filter(users, overSome([{ 'age': 36 }, ['age', 40]])); // => objects for ['fred', 'barney']`
 */
export function filter(collection, rule) {
  if (false) { filter(collection, rule); }
  throw 'TODO: implement filter';
}

// flipshop
/** Returns the first element of collection for which `rule` holds (returns truthy)
 * Returns fallback value if no element is found.
 * `rule` is invoked as `(val, ckey)`.
 *
 * @param collection {array|map}: The collection to inspect.
 * @param ruleOrKey {function|string|array}: `(val, ckey) => key`; defaults to `identity`.
 *   @optional
 * @param fromIndex {number}: The index to search from; defaults to `0`.
 *   @optional
 * @param fallback {any}: The value to return if no element is found; defaults to `undefined`.
 *
 * @returns: the matched element, else `undefined`.
 *
 * @example `find([1, 2, 3], (val) => val > 1); // => 2`
 * @example `find({ a: 1, b: 2 }, (val, key) => key == "b"); // => 2`
 * @example `var users = [ { 'user': 'barney', 'age': 36, 'active': true }, { 'user': 'fred', 'age': 40, 'active': false }, { 'user': 'pebbles', 'age': 1, 'active': true } ]; find(users, function(o) { return o.age < 40; }); // => object for 'barney'`
 * @example `find(users, { 'age': 1, 'active': true }); // => object for 'pebbles'`
 * @example `find(users, ['active', false]); // => object for 'fred'`
 * @example `find(users, 'active'); // => object for 'barney'`
 */
export function find(collection, rule, fromIndex, fallback) {}

// flipshop
/** Index of the first element of `arr` for which `rule` holds (returns truthy),
 * or `fallbackIndex` if none does.
 *
 * @param arr {array}: The array to inspect.
 * @param ruleOrKey {function|string|array}: `(val, ckey) => key`; defaults to `identity`.
 *   @optional
 * @param fromIndex {number}: The index to search from; defaults to `0`.
 *   @optional
 *
 * @returns {number}: the index of the found element, else `fallbackIndex`.
 *
 * @example `findIndex([1, 2, 3], (val) => val > 1); // => 1`
 * @example `findIndex([1, 2, 3], (val) => val > 9); // => -1`
 * @example `var users = [ { 'user': 'barney', 'active': false }, { 'user': 'fred', 'active': false }, { 'user': 'pebbles', 'active': true } ]; findIndex(users, function(o) { return o.user == 'barney'; }); // => 0`
 * @example `findIndex(users, { 'user': 'fred', 'active': false }); // => 1`
 * @example `findIndex(users, ['active', false]); // => 0`
 * @example `findIndex(users, 'active'); // => 2`
 */
export function findIndex(arr, rule, fromIndex, fallbackIndex) {}

// lodash
/** Key of the first element of `collection` for which `rule` holds (returns truthy),
 * or `fallbackKey` if none does.
 * `rule` is invoked as `(val, ckey)`.
 *
 * @param collection {map|array}: The collection to inspect.
 * @param rule {ruleOrKey}: Coerced to a rule; defaults to `identity`.
 *   @optional
 * @param fallbackKey {string|number|undefined}: The key to return if no element is found; defaults to `undefined`.
 *
 * @returns {string|number|undefined}: the key of the matched element, else `fallbackKey`.
 *
 * @example `var users = { 'barney': { 'age': 36, 'active': true }, 'fred': { 'age': 40, 'active': false }, 'pebbles': { 'age': 1, 'active': true } }; findKey(users, function(o) { return o.age < 40; }); // => 'barney' (iteration order is not guaranteed)`
 * @example `findKey(users, { 'age': 1, 'active': true }); // => 'pebbles'`
 * @example `findKey(users, ['active', false]); // => 'fred'`
 * @example `findKey(users, 'active'); // => 'barney'`
 */
export function findKey(bag is map, rule) {}
export function findKey(bag is map, rule, fallbackKey) {}
export function findKey(bag is array, rule) {}
export function findKey(bag is array, rule, fallbackKey) {}
// lodash
/** Value of the last element of `collection` for which `rule`
 * holds (returns truthy), or `fallback` if none does.
 * Like `find` except that it iterates over elements of
 * `collection` from right to left.
 *
 * @param collection {array|map}: The collection to inspect.
 * @param rule {ruleOrKey}: Coerced to a rule; defaults to `identity`.
 *   @optional
 * @param fromIndex {number}: The index to search from; defaults to `collection.length-1`.
 *   @optional
 * @param fallback {any}: The value to return if no element is found; defaults to `undefined`.
 *
 * @returns: the matched element, else `undefined`.
 *
 * @example `findLast([1, 2, 3, 4], function(n) { return n % 2 == 1; }); // => 3`
 */
export function findLast(collection, rule, fromIndex) {}

// flipshop
/** Index of the last element of `arr` for which `rule` holds (returns truthy),
 * or `fallbackIndex` if none does.
 * Like `findIndex` except that it iterates over elements of
 * `collection` from right to left.
 *
 * @param arr {array}: The array to inspect.
 * @param rule {ruleOrKey}: Coerced to a rule; defaults to `identity`.
 *   @optional
 * @param fromIndex {number}: The index to search from; defaults to `array.length-1`.
 *   @optional
 *
 * @returns {number}: the index of the found element, else `-1`.
 *
 * @example `var users = [ { 'user': 'barney', 'active': true }, { 'user': 'fred', 'active': false }, { 'user': 'pebbles', 'active': false } ]; findLastIndex(users, function(o) { return o.user == 'pebbles'; }); // => 2`
 * @example `findLastIndex(users, { 'user': 'barney', 'active': true }); // => 0`
 * @example `findLastIndex(users, ['active', false]); // => 2`
 * @example `findLastIndex(users, 'active'); // => 0`
 */
export function findLastIndex(array, rule, fromIndex) {
  if (false) { findLastIndex(array, rule, fromIndex); }
  throw 'TODO: implement findLastIndex';
}

// flipshop
/** Key of the last element of `collection` for which `rule` holds (returns truthy),
 * Like `findKey` except that it iterates over elements of
 * a collection in the opposite order.
 *
 * @param collection {map|array}: The collection to inspect.
 * @param rule {ruleOrKey} Defaults to `identity`.
 *   @optional
 * @param fallbackKey {string|number|undefined}: The key to return if no element is found; defaults to `undefined`.
 *
 * @returns {string|number|undefined}: the key of the matched element, else `fallbackKey`.
 *
 * @example `var users = { 'barney': { 'age': 36, 'active': true }, 'fred': { 'age': 40, 'active': false }, 'pebbles': { 'age': 1, 'active': true } }; findLastKey(users, function(o) { return o.age < 40; }); // => returns 'pebbles' assuming 'findKey' returns 'barney'`
 * @example `findLastKey(users, { 'age': 36, 'active': true }); // => 'barney'`
 * @example `findLastKey(users, ['active', false]); // => 'fred'`
 * @example `findLastKey(users, 'active'); // => 'pebbles'`
 */
export function findLastKey(bag is map, rule) {}
export function findLastKey(bag is map, rule, fallbackKey) {}
export function findLastKey(bag is array, rule) {}
export function findLastKey(bag is array, rule, fallbackKey) {}

// flipshop
/** Creates a flattened array of values by running each element in `collection`
 * thru `funcOrKey` and flattening the mapped results. `funcOrKey` is invoked as `(val, ckey)`.
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param funcOrKey {function|string|array}: iteratee invoked per iteration; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the new flattened array.
 *
 * @example `function duplicate(n) { return [n, n]; } flatMap([1, 2], duplicate); // => [1, 1, 2, 2]`
 */
export function flatMap(collection, funcOrKey) {
  if (false) { flatMap(collection, funcOrKey); }
  throw 'TODO: implement flatMap';
}

// lodash
/** Like `flatMap` except that it recursively flattens the
 * mapped results.
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param funcOrKey {function|string|array}: iteratee invoked per iteration; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the new flattened array.
 *
 * @example `function duplicate(n) { return [[[n, n]]]; } flatMapDeep([1, 2], duplicate); // => [1, 1, 2, 2]`
 */
export function flatMapDeep(collection, funcOrKey) {
  if (false) { flatMapDeep(collection, funcOrKey); }
  throw 'TODO: implement flatMapDeep';
}

// lodash
/** Like `flatMap` except that it recursively flattens the
 * mapped results up to `depth` times.
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param funcOrKey {function|string|array}: iteratee invoked per iteration; defaults to `identity`.
 *   @optional
 * @param depth {number}: The maximum recursion depth; defaults to `1`.
 *   @optional
 *
 * @returns {array}: the new flattened array.
 *
 * @example `function duplicate(n) { return [[[n, n]]]; } flatMapDepth([1, 2], duplicate, 2); // => [[1, 1], [2, 2]]`
 */
export function flatMapDepth(collection, funcOrKey, depth) {
  if (false) { flatMapDepth(collection, funcOrKey, depth); }
  throw 'TODO: implement flatMapDepth';
}

// lodash
/** Flattens `array` a single level deep.
 * @seeAlso `flattenDeep`, `flattenDepth`.
 *
 * @param array {array}: The array to flatten.
 *
 * @returns {array}: the new flattened array.
 *
 * @example `flatten([1, [2, [3, [4]], 5]]); // => [1, 2, [3, [4]], 5]`
 */
export function flatten(array) {
  if (false) { flatten(array); }
  throw 'TODO: implement flatten';
}

// flipshop
/** Recursively flattens `arr`.
 *
 * @param arr {array}: The array to flatten.
 *
 * @returns {array}: the new flattened array.
 *
 * @example `flattenDeep([1, [2, [3, [4]], 5]]); // => [1, 2, 3, 4, 5]`
 */
export function flattenDeep(arr) {}

// flipshop
/** `arr` with up to `depth` levels of array nesting removed; `depth <= 0` returns `arr` unchanged.
 *
 * @example `flattenDepth([1, [2, [3, [4]], 5]], 2); // => [1, 2, 3, [4], 5]`
 */
// export function flattenDepth(arr, depth) {}

// lodash
/** Recursively flatten `arr` up to `depth` times.
 * `depth <= 0` returns `arr` unchanged.
 * @seeAlso `flatten`, `flattenDeep`.
 *
 * @param arr {array}: The array to flatten.
 * @param depth {number}: The maximum recursion depth; defaults to `1`.
 *   @optional
 *
 * @returns {array}: the new flattened array.
 *
 * @example `var arr = [1, [2, [3, [4]], 5]]; flattenDepth(arr, 1); // => [1, 2, [3, [4]], 5]`
 * @example `flattenDepth(arr, 2); // => [1, 2, 3, [4], 5]`
 */
export function flattenDepth(arr, depth) {}

// lodash
/** Iterates over elements of `collection` and invokes `funcOrKey` for each element, as
 * `funcOrKey(val, ckey)`. Return `NextStepAction.BREAK` from `func` to stop iteration early.
 *
 * @seeAlso [forEach]
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param funcOrKey {function|string|array}: iteratee invoked per iteration; defaults to `identity`.
 *   @optional
 * @param nilPolicy {NilPolicy}:
 *   with `NilPolicy.NIL` (the default), existing-as-undefined entries are visited;
 *   with `NilPolicy.SKIP`, they are skipped; there will be a gap in the sequence of indexes for arrays.
 *   defaults to `NilPolicy.NIL`.
 *   @optional
 * @param keylist {array}: The keys, in order, to iterate over;
 *   keys are allowed to be repeated, absent, or out-of-bounds for the collection.
 *   @optional
 *
 * @returns {array|map}: `collection`.
 *
 * @example `forEach3(["a", "b", "c"], (val, ckey, iter) => { println([val, ckey, iter]); }); // => Logs '[c, 0, 0]' then '[b, 1, 1]' then '[a, 2, 2]'.`
 * @example `forEach3({ a: 1, b: 2 }, (val, ckey, iter) => { println([val, ckey, iter]); }); // => Logs '[1, "a", 0]' then '[2, "b", 1]'.`
 */
// export function forEach(bag, keylist, nilPolicy, func) {}

// flipshop
/** `forEach`, but it passes the iteration count as a third argument.
 * `iter` is a 0-based visit-count that's distinct from `ckey` when a `keylist`
 * or `nilPolicy` makes visit order diverge from key order.
 * Return `NextStepAction.BREAK` from `func` to stop iteration early.
 *
 * @seeAlso [forEach]
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param funcOrKey {function|string|array}: iteratee invoked per iteration; defaults to `identity`.
 *   @optional
 * @param nilPolicy {NilPolicy}: The policy to use for existing-as-undefined values;
 *   defaults to `NilPolicy.NIL` (no elements are skipped).
 *   @optional
 * @param keylist {array}: The keys, in order, to iterate over;
 *   keys are allowed to be repeated, absent, or out-of-bounds for the collection.
 *   @optional
 *
 * @returns {array|map}: `collection`.
 *
 * @example `forEach3(["a", "b", "c"], (val, ckey, iter) => { println([val, ckey, iter]); }); // => Logs '[c, 0, 0]' then '[b, 1, 1]' then '[a, 2, 2]'.`
 * @example `forEach3({ a: 1, b: 2 }, (val, ckey, iter) => { println([val, ckey, iter]); }); // => Logs '[1, "a", 0]' then '[2, "b", 1]'.`
 */
// export function forEach3(bag, keylist, nilPolicy, func) {}

// flipshop
/** Like `forEach` except that it iterates over elements of
 * `collection` from right to left.
 * Return `NextStepAction.BREAK` from `func` to stop iteration early.
 *
 * @seeAlso [forEach]
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param funcOrKey {function|string|array}: iteratee invoked per iteration; defaults to `identity`.
 *   @optional
 * @param nilPolicy {NilPolicy}: The policy to use for missing values; defaults to `NilPolicy.NIL`.
 *   @optional
 * @param keylist {array}: The keys, in order, to iterate over;
 *   keys are allowed to be repeated, absent, or out-of-bounds for the collection.
 *   @optional
 *
 * @returns {array|map}: `collection`.
 *
 * @example `forEachRight(["a", "b", "c"], (val, ckey) => { println([val, ckey]); }); // => Logs '[c, 0]' then '[b, 1]' then '[a, 2]'.`
 * @example `forEachRight({ a: 1, b: 2 }, (val, ckey) => { println([val, ckey]); }); // => Logs '[2, "b"]' then '[1, "a"]'.`
 */
// export function forEachRight(bag, keylist, nilPolicy, func) {}

/** Map built from `pairs` -- `[[key, val], ...]` -- the inverse of `entries`
 * (i.e. `toPairs`). A repeated key keeps the value given by the last pair.
 *
 * @param pairs {array}: The key-value pairs.
 *
 * @returns {map}: the new object.
 *
 * @example `fromPairs([['a', 1], ['b', 2]]); // => { 'a': 1, 'b': 2 }`
 */

/** Gets the value at `keyname`/`keypath` of `bag`/`arr`. If the resolved value is `undefined`,
 * -- because it's out of bounds, or missing, or exists with the value `undefined` --
 * `fallback` is returned in its place
 *
 * A string path is dotted (`"a.b.c"` is three steps).
 * An array path is a list of literal keys: `["a.b"]` is one step, reaching a key with a dot in its own name.
 * Either kind of path may contain a negative array index, which counts from the end.
 * This does not support the array-bracket style (`a.b[3]`); use eg `a.b.3` directly.
 * Strings presented as array indexes are converted to numbers, or an error is thrown.
 *
 * @param bag {map|array}: Container to read from.
 * @param path {anypath}: Path to get.
 *   as a dotted string (`a.3.5`),
 *   or as a list of literal keys/indexes (`["a", 3, "5"]`).
 * @param fallback: Returned in place of an `undefined` result. Defaults to `undefined`.
 *
 * @example `getAt({ "a": { "b": 1 } }, "a.b"); // => 1`
 * @example `getAt({ "a": { "b": 1 } }, ["a", "b"]); // => 1`
 * @example `getAt({ "a": { "b": 1 } }, "a.c", { "not": "met" }); // => { "not": "met" }`
 * @example `getAt({ "rows": [{ "cells": [7, 8] }] }, "rows.-1.cells.-1"); // => 8`
 * @example `getAt([1, 2, 3], -1); // => 3`
 * @example `getAt({ "2.5": "literal", "2": { "5": "pathed" } }, "2.5");      // => "pathed"`
 * @example `getAt({ "2.5": "literal", "2": { "5": "pathed" } }, ["2.5"]);    // => "literal"`
 * @example `getAt({ "2.5": "literal", "2": { "5": "pathed" } }, ["2", "5"]); // => "pathed"`
 * @example `getAt([0, "one", ['a', 'b', 'c', 'd', 'e', 'f']], "1"); // => "one"`
 * @example `getAt([0, "one", ['a', 'b', 'c', 'd', 'e', 'f']], "2.5"); // => "f"`
 * @example `getAt([0, "one", ['a', 'b', 'c', 'd', 'e', 'f']], ["2.5"]); // error`
 * @example `getAt([0, "one", ['a', 'b', 'c', 'd', 'e', 'f']], ["2", "5"]); // => "f"`
 */
// export function getAt(bag, keyname, fallback) {}


// flipshop
/** Creates a map of keys generated from the results of running each element of `collection`
 * thru `funcOrKey`. The order of grouped values is determined by the order they occur in
 * `collection`. The corresponding value of each key is an array of elements responsible for
 * generating the key.
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param funcOrKey {function}: `(val, ckey) => key`; defaults to `identity`.
 *   @optional
 *
 * @returns {map}: the composed aggregate object.
 *
 * @example `groupBy([1, 2, 3, 4], (val, _seq) => (val % 2 == 0) ? "even" : "odd"); // => { "odd": [1, 3], "even": [2, 4] }`
 * @example `groupBy([6.1, 4.2, 6.3], Math.floor); // => { '4': [4.2], '6': [6.1, 6.3] }`
 * @example `groupBy(['one', 'two', 'three'], 'length'); // => { '3': ['one', 'two'], '5': ['three'] }`
 */
// export function groupBy(arr, funcOrKey) {}

// TODO-helperFuncs
/** Checks if `value` is greater than `other`.
 *
 * @seeAlso [lt]
 *
 * @param value: The value to compare.
 * @param other: The other value to compare.
 *
 * @returns {boolean}: `true` if `value` is greater than `other`, else `false`.
 *
 * @example `gt(3, 1); // => true`
 * @example `gt(3, 3); // => false`
 * @example `gt(1, 3); // => false`
 */
export function gt(value, other) {
  if (false) { gt(value, other); }
  throw 'TODO: implement gt';
}

// TODO-helperFuncs
/** Checks if `value` is greater than or equal to `other`.
 *
 * @seeAlso [lte]
 *
 * @param value: The value to compare.
 * @param other: The other value to compare.
 *
 * @returns {boolean}: `true` if `value` is greater than or equal to `other`, else `false`.
 *
 * @example `gte(3, 1); // => true`
 * @example `gte(3, 3); // => true`
 * @example `gte(1, 3); // => false`
 */
export function gte(value, other) {
  if (false) { gte(value, other); }
  throw 'TODO: implement gte';
}

// flipshop
/** Checks if `keypath` exists in `collection`. For an array, negative indices are counted from the end.
 *
 * For an array, `nilPolicy` decides whether an in-bounds slot holding `undefined` counts:
 * `NilPolicy.NIL` (the default) says yes; `NilPolicy.SKIP` says no,
 * `hasPresentKey` is `hasKey` with NilPolicy.SKIP.
 * @seeAlso [hasPresentKey]
 *
 * @param collection {map|array}: The collection to query.
 * @param keypath {anypath}: The path to check.
 * @param nilPolicy {NilPolicy}: The policy to use for existing-as-undefined values;
 *   defaults to `NilPolicy.NIL` (no elements are skipped).
 *   @optional
 *
 * @example `hasKey({ "a": 1 }, "a"); // => true`
 * @example `hasKey({ "a": undefined }, "a"); // => false`
 * @example `hasKey([1, 2, 3], 2); // => true`
 * @example `hasKey([1, 2, 3], -1); // => false`
 * @example `hasKey([1, undefined, 3], 1, NilPolicy.SKIP); // => false`
 */
// export function hasKey(obj, key) {}

// flipshop
/** Whether `str` matches `regex` anywhere -- a `match` wrapper that returns `false` instead of
 * throwing, on either a malformed `regex` or an `undefined` `str`.
 *
 * @example `hasMatch("hello", "ell"); // => true`
 * @example `hasMatch("hello", "^e"); // => false`
 */
// export function hasMatch(str, regex) {}

// lodash
/** Gets the first element of `arr`.
 *
 * @param arr {array}: The array to query.
 *
 * @returns: the first element of `array`.
 *
 * @example `head([1, 2, 3]); // => 1`
 * @example `head([]); // => undefined`
 */
export function head(arr) {}

// lodash
/** Returns the first argument it receives.
 * @seeAlso [identity0]
 * @seeAlso [identity1]
 * @seeAlso [identity3]
 * @seeAlso [identity4]
 * @seeAlso [identity5]
 *
 * @param value: Any value.
 *
 * @returns: `value`.
 *
 * @example `var obj = { 'a': 1 }; println(identity(object) === object); // => true`
 */
export function identity(val, _seq) {
  if (false) { identity(val, _seq); }
  throw 'TODO: implement identity';
}

// flipshop
/** returns `fallback` if `val` is either equal (within tolerance) to zero, or is undefined;
 * otherwise, returns `val`.
 *
 * @example `ifZero(0, "default"); // => "default"`
 * @example `ifZero(5, "default"); // => 5`
 * @example `ifZero(0 * millimeter, "default"); // => "default"`
 */
// export function ifZero(val, fallback) {}

// flipshop
/** Whether `target` appears anywhere in the `bag`'s values or `arr`'s elements, outside of the first (or last) `numToSkip`.
 * Comparison is plain `==`, which is deep structural equality on maps and arrays.
 * If `fromIndex` is negative, it's used as the offset from the end of `collection`.
 * @seeAlso [substring]
 *
 * TODO:
 * @param numToSkip {number}: The index to search from; defaults to `0`.
 *   @optional
 *
 * @example `arrayIncludes([1, 2, 3], 2); // => true`
 * @example `arrayIncludes([{ "a": 1 }], { "a": 1 }); // => true`
 * @example `arrayIncludes({ "x": 1, "y": 2 }, 2); // => true`
 */
// export function arrayIncludes(arr, target) {}

// lodash
/** Gets the index at which the first occurrence of `value` is found in `array`
 * using the given `comparator`.
 * If `fromIndex` is negative, it's used as the
 * offset from the end of `array`.
 *
 * @param arr {array}: The array to inspect.
 * @param val: The value to search for.
 * @param fromIndex {number}: The index to search from; defaults to `0`.
 *   @optional
 * @param fallbackIndex {number}: The index to return if no element is found; defaults to `-1`.
 * @param comparator {function}: The comparator invoked per element.
 *   @optional
 *
 * @returns {number}: the index of the matched value, else `fallbackIndex`.
 *
 * @example `indexOf([1, 2, 1, 2], 2); // => 1`
 * @example `indexOf([1, 2, 1, 2], 2, 2); // => 3`
 */
export function indexOf(arr, val, fromIndex, fallbackIndex, comparator) {}

/** Gets all but the last element of `array`.
 *
 * @param array {array}: The array to query.
 *
 * @returns {array}: the slice of `array`.
 *
 * @example `initial([1, 2, 3]); // => [1, 2]`
 */
export function initial(array) {}

// flipshop
/** Whether `num` falls in `[start, end)` (or `[end, start)` if `end < start`). If
 * `end` is not specified, it's set to `start` with `start` then set to `0`.
 * If `start` is greater than `end` the params are swapped to support
 * negative ranges.
 *
 * @seeAlso [range]
 * @seeAlso [rangeRight]
 *
 * @param num {number}: The number to check.
 * @param start {number}: The start of the range; defaults to `0`.
 *   @optional
 * @param end {number}: The end of the range.
 *
 * @returns {boolean}: `true` if `number` is in the range, else `false`.
 *
 * @example `'inRange(3, 5); // => true (implicit start of 0)`
 * @example `'inRange(3, 1, 5); // => true`
 * @example `'inRange(5, 1, 5); // => false, upper bound is exclusive`
 * @example `'inRange(3, 2, 4); // => true`
 * @example `'inRange(4, 8); // => true`
 * @example `'inRange(4, 2); // => false`
 * @example `'inRange(2, 2); // => false`
 * @example `'inRange(1.2, 2); // => true`
 * @example `'inRange(5.2, 4); // => false`
 * @example `'inRange(-3, -2, -6); // => true`
 */
// export function inRange(num, start, end) {}

// flipshop
/** Values present in every array of `arrays`, deduplicated, ordered as they occur in
 * `arrays[0]`.
 *
 * @param arrays {array}: The arrays to inspect.
 *   @optional
 *
 * @returns {array}: the new array of intersecting values.
 *
 * @example `intersection([2, 1], [2, 3]); // => [2]`
* @example `intersection([[2, 1], [2, 3], [1, 2]]); // => [2]`
*/
export function intersection(arrays) {
  if (false) { intersection(arrays); }
  throw 'TODO: implement intersection';
}

// flipshop
/** `intersection`, comparing elements by `funcOrKey(val, idx)` instead of `val` itself.
 * `funcOrKey` is coerced through `iteratee` @see `iteratee`.
 * The order and references of result values are
 * determined by the first array.
 *
 * @param arrays {array}: The arrays to inspect.
 *   @optional
 * @param funcOrKey {function}: `(val, seq) => criteria`; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the new array of intersecting values.
 *
 * @example `intersectionBy([2.1, 1.2], [2.3, 3.4], Math.floor); // => [2.1]`
 * @example `intersectionBy([{ 'x': 1 }], [{ 'x': 2 }, { 'x': 1 }], 'x'); // => [{ 'x': 1 }]`
 * @example `intersectionBy([[2.1, 1.2], [2.3, 3.4]], (val, _seq) => floor(val)); // => [2.1]`
 */
export function intersectionBy(arrays, funcOrKey) {
  if (false) { intersectionBy(arrays, funcOrKey); }
  throw 'TODO: implement intersectionBy';
}

// flipshop
/** `intersection`, comparing elements with `comparator(val, other)` instead of `==`.
 * The order and references of result values are determined by the first array.
 *
 * @param arrays {array}: The arrays to inspect.
 *   @optional
 * @param comparator {function}: The comparator invoked per element.
 *   @optional
 *
 * @returns {array}: the new array of intersecting values.
 *
 * @example `var objects = [{ 'x': 1, 'y': 2 }, { 'x': 2, 'y': 1 }]; var others = [{ 'x': 1, 'y': 1 }, { 'x': 1, 'y': 2 }]; intersectionWith(objects, others, isEqual); // => [{ 'x': 1, 'y': 2 }]`
 */
export function intersectionWith(arrays, comparator) {
  if (false) { intersectionWith(arrays, comparator); }
  throw 'TODO: implement intersectionWith';
}

// flipshop
/** `bag` with its keys and values swapped: `{"a": "x", "b": "x"}` → `{"x": "b"}`
 * A value that occurs more than once keeps only its last key.
 * A non-string value is stringified into its new key
 * @seeAlso [invertBy]
 *
 * @example `invert({ "a": 1, "b": 2, "c": 1 }); // => { "1": "c", "2": "b" }`
 */
// export function invert(bag) {}

// lodash
/** Like `invert` except that the inverted map is generated from the results of running each
 * element of `object` thru `funcOrKey`. The corresponding inverted value of each inverted key
 * is an array of keys responsible for generating the inverted value.
 *
 * @param bag {map}: The object to invert.
 * @param funcOrKey {function}: `(val, key) => criteria`; defaults to `identity`.
 *   @optional
 *
 * @returns {map}: the new inverted bag.
 *
 * @example `invertBy({ "a": 1, "b": 2, "c": 1 }); // => { "1": ["a", "c"], "2": ["b"] }`
 * @example `invertBy({ "a": 1, "b": 2, "c": 1 }, (val, key) => "group" + val); // => { "group1": ["a", "c"], "group2": ["b"] }`
 */
export function invertBy(bag, funcOrKey) {}

// lodash
/** Invokes the method at `path` of `object`.
 *
 * @param object {map}: The object to query.
 * @param path {anypath}: Path to the method to invoke.
 * @param args: The arguments to invoke the method with.
 *   @optional
 *
 * @returns: the result of the invoked method.
 *
 * @example `var object = { 'a': [{ 'b': { 'c': [1, 2, 3, 4] } }] }; invoke(object, 'a[0].b.c.slice', 1, 3); // => [2, 3]`
 */
export function invoke(object, path, args) {
  if (false) { invoke(object, path, args); }
  throw 'TODO: implement invoke';
}

// lodash
/** Invokes the method at `path` of each element in `collection`, returning
 * an array of the results of each invoked method. Any additional arguments
 * are provided to each invoked method. If `path` is a function, it's invoked
 * for, and `this` bound to, each element in `collection`.
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param path {array|function|string}: The path of the method to invoke or the function invoked per iteration.
 * @param args: The arguments to invoke each method with.
 *   @optional
 *
 * @returns {array}: the array of results.
 *
 * @example `invokeMap([[5, 1, 7], [3, 2, 1]], 'sort'); // => [[1, 5, 7], [1, 2, 3]]`
 * @example `invokeMap([123, 456], String.prototype.split, ''); // => [['1', '2', '3'], ['4', '5', '6']]`
 */
export function invokeMap(collection, path, args) {
  if (false) { invokeMap(collection, path, args); }
  throw 'TODO: implement invokeMap';
}

// lodash
/** Checks if `value` is classified as an `Array` object.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is an array, else `false`.
 *
 * @example `isArray([1, 2, 3]); // => true`
 * @example `isArray(document.body.children); // => false`
 * @example `isArray('abc'); // => false`
 * @example `isArray(noop); // => false`
 */
export function isArray(value) {
  if (false) { isArray(value); }
  throw 'TODO: implement isArray';
}

// lodash
/** Checks if `value` is classified as a boolean primitive or object.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is a boolean, else `false`.
 *
 * @example `isBoolean(false); // => true`
 * @example `isBoolean(null); // => false`
 */
export function isBoolean(value) {
  if (false) { isBoolean(value); }
  throw 'TODO: implement isBoolean';
}

// lodash
/** Checks if `value` is an empty map, array, box, or string --
 * one with no keys, no elements, or no characters, respectively.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is empty, else `false`.
 *
 * @example `isEmpty(null); // => true`
 * @example `isEmpty(true); // => true`
 * @example `isEmpty(1); // => true`
 * @example `isEmpty([1, 2, 3]); // => false`
 * @example `isEmpty({ 'a': 1 }); // => false`
 * // empty box example
 */
export function isEmpty(value) {}

// lodash
/** Performs a deep comparison between two values to determine if they are
 * equivalent -- keys and vals for two maps, elements for two arrays, recursively.
 *
 * @param value: The value to compare.
 * @param other: The other value to compare.
 *
 * @returns {boolean}: `true` if the values are equivalent, else `false`.
 *
 * @example `var object = { 'a': 1 }; var other = { 'a': 1 }; isEqual(object, other); // => true`
 * @example `object === other; // => false`
 */
export function isEqual(value, other) {
  if (false) { isEqual(value, other); }
  throw 'TODO: implement isEqual';
}

// lodash
/** Like `isEqual` except that it accepts `customizer` which
 * is invoked to compare values. If `customizer` returns `undefined`, comparisons
 * are handled by the method instead. The `customizer` is invoked with up to
 * six arguments: (objValue, othValue [, ckey, object, other, stack]).
 *
 * @param value: The value to compare.
 * @param other: The other value to compare.
 * @param customizer {function}: Function to customize comparisons.
 *   @optional
 *
 * @returns {boolean}: `true` if the values are equivalent, else `false`.
 *
 * @example `function isGreeting(value) { return /^h(?:i|ello)$/.test(value); } function customizer(objValue, othValue) { if (isGreeting(objValue) && isGreeting(othValue)) { return true; } } var array = ['hello', 'goodbye']; var other = ['hi', 'goodbye']; isEqualWith(array, other, customizer); // => true`
 */
export function isEqualWith(value, other, customizer) {
  if (false) { isEqualWith(value, other, customizer); }
  throw 'TODO: implement isEqualWith';
}

// lodash
/** Checks if `value` is a finite primitive number.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is a finite number, else `false`.
 *
 * @example `isFinite(3); // => true`
 * @example `isFinite(Number.MIN_VALUE); // => true`
 * @example `isFinite(Infinity); // => false`
 * @example `isFinite('3'); // => false`
 */
export function isFinite(value) {
  if (false) { isFinite(value); }
  throw 'TODO: implement isFinite';
}

// lodash
/** Checks if `value` is classified as a `Function` object.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is a function, else `false`.
 *
 * @example `isFunction(_); // => true`
 * @example `isFunction(/abc/); // => false`
 */
export function isFunction(value) {
  if (false) { isFunction(value); }
  throw 'TODO: implement isFunction';
}

// lodash
/** Checks if `value` is an integer.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is an integer, else `false`.
 *
 * @example `isInteger(3); // => true`
 * @example `isInteger(Number.MIN_VALUE); // => false`
 * @example `isInteger(Infinity); // => false`
 * @example `isInteger('3'); // => false`
 */
export function isInteger(value) {
  if (false) { isInteger(value); }
  throw 'TODO: implement isInteger';
}

// lodash
/** Checks if `value` is classified as a `Map` object.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is a map, else `false`.
 *
 * @example `isMap(new Map); // => true`
 * @example `isMap(new WeakMap); // => false`
 */
export function isMap(value) {
  if (false) { isMap(value); }
  throw 'TODO: implement isMap';
}

// lodash
/** Performs a partial deep comparison between `object` and `source` to
 * determine if `object` contains `source`'s vals at `source`'s keys.
 *
 * **Note:** This method is equivalent to `matches` when `source` is
 * partially applied.
 *
 * Partial comparisons will match empty array and empty object `source`
 * values against any array or object value, respectively. See `isEqual`
 * for a list of supported value comparisons.
 *
 * @param object {map}: The object to inspect.
 * @param source {map}: Map of vals to match.
 *
 * @returns {boolean}: `true` if `object` is a match, else `false`.
 *
 * @example `var object = { 'a': 1, 'b': 2 }; isMatch(object, { 'b': 2 }); // => true`
 * @example `isMatch(object, { 'b': 1 }); // => false`
 */
export function isMatch(object, source) {
  if (false) { isMatch(object, source); }
  throw 'TODO: implement isMatch';
}

// lodash
/** Like `isMatch` except that it accepts `customizer` which
 * is invoked to compare values. If `customizer` returns `undefined`, comparisons
 * are handled by the method instead. The `customizer` is invoked with five
 * arguments: (objValue, srcValue, ckey, object, source).
 *
 * @param object {map}: The object to inspect.
 * @param source {map}: Map of vals to match.
 * @param customizer {function}: Function to customize comparisons.
 *   @optional
 *
 * @returns {boolean}: `true` if `object` is a match, else `false`.
 *
 * @example `function isGreeting(value) { return /^h(?:i|ello)$/.test(value); } function customizer(objValue, srcValue) { if (isGreeting(objValue) && isGreeting(srcValue)) { return true; } } var object = { 'greeting': 'hello' }; var source = { 'greeting': 'hi' }; isMatchWith(object, source, customizer); // => true`
 */
export function isMatchWith(object, source, customizer) {
  if (false) { isMatchWith(object, source, customizer); }
  throw 'TODO: implement isMatchWith';
}

// lodash
/** Checks if `value` is `NaN`.
 *
 * **Note:** This method is based on
 * [`Number.isNaN`](https://mdn.io/Number/isNaN) and is not the same as
 * global [`isNaN`](https://mdn.io/isNaN) which returns `true` for
 * `undefined` and other non-number values.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is `NaN`, else `false`.
 *
 * @example `isNaN(NaN); // => true`
 * @example `isNaN(new Number(NaN)); // => true`
 * @example `isNaN(undefined); // => true`
 * @example `isNaN(undefined); // => false`
 */
export function isNaN(value) {
  if (false) { isNaN(value); }
  throw 'TODO: implement isNaN';
}

// lodash
/** Checks if `value` is `null` or `undefined`.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is nullish, else `false`.
 *
 * @example `isNil(null); // => true`
 * @example `isNil(void 0); // => true`
 * @example `isNil(NaN); // => false`
 */
export function isNil(value) {
  if (false) { isNil(value); }
  throw 'TODO: implement isNil';
}

// lodash
/** Checks if `value` is classified as a `Number` primitive or object.
 *
 * **Note:** To exclude `Infinity`, `-Infinity`, and `NaN`, which are
 * classified as numbers, use the `isFinite` method.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is a number, else `false`.
 *
 * @example `isNumber(3); // => true`
 * @example `isNumber(Number.MIN_VALUE); // => true`
 * @example `isNumber(Infinity); // => true`
 * @example `isNumber('3'); // => false`
 */
export function isNumber(value) {
  if (false) { isNumber(value); }
  throw 'TODO: implement isNumber';
}

// lodash
/** Checks if `value` is classified as a `String` primitive or object.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is a string, else `false`.
 *
 * @example `isString('abc'); // => true`
 * @example `isString(1); // => false`
 */
export function isString(value) {
  if (false) { isString(value); }
  throw 'TODO: implement isString';
}

// lodash
/** Checks if `value` is `undefined`.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is `undefined`, else `false`.
 *
 * @example `isUndefined(void 0); // => true`
 * @example `isUndefined(null); // => false`
 */
export function isUndefined(value) {
  if (false) { isUndefined(value); }
  throw 'TODO: implement isUndefined';
}

// lodash
/** Coerces `spec`, a `funcOrKey` or `ruleOrKey`, into a callable: a function passes through
 * unchanged, a key/dotkey/keypath becomes a `property` accessor, a `[path, srcValue]` array
 * becomes a `matchesProperty` rule, a map becomes a `matches` rule, and anything else falls
 * back to `identity`.
 *
 * @param spec: The value to convert to a callback; defaults to `identity`.
 *   @optional
 *
 * @returns {function}: the callback.
 *
 * @example `var users = [ { 'user': 'barney', 'age': 36, 'active': true }, { 'user': 'fred', 'age': 40, 'active': false } ]; filter(users, iteratee({ 'user': 'barney', 'active': true })); // => [{ 'user': 'barney', 'age': 36, 'active': true }]`
 * @example `filter(users, iteratee(['user', 'fred'])); // => [{ 'user': 'fred', 'age': 40 }]`
 * @example `map(users, iteratee('user')); // => ['barney', 'fred']`
 */
export function iteratee(spec) {
  if (false) { iteratee(spec); }
  throw 'TODO: implement iteratee';
}

// lodash
/** Converts all elements in `array` into a string separated by `separator`.
 *
 * @param array {array}: The array to convert.
 * @param separator {string}: The element separator; defaults to `','`.
 *   @optional
 *
 * @returns {string}: the joined string.
 *
 * @example `join(['a', 'b', 'c'], '~'); // => 'a~b~c'`
 */
export function join(array, separator) {
  if (false) { join(array, separator); }
  throw 'TODO: implement join';
}

// flipshop
/** /** `str` split into words, lowercased, and joined with `-`. */
 */
// export function kebabCase(str) {}

// lodash
/** Converts `string` to
 * [kebab case](https://en.wikipedia.org/wiki/Letter_case#Special_case_styles).
 *
 * @param string {string}: The string to convert; defaults to `''`.
 *   @optional
 *
 * @returns {string}: the kebab cased string.
 *
 * @example `kebabCase('Foo Bar'); // => 'foo-bar'`
 * @example `kebabCase('fooBar'); // => 'foo-bar'`
 * @example `kebabCase('__FOO_BAR__'); // => 'foo-bar'`
 */
export function kebabCase(string) {
  if (false) { kebabCase(string); }
  throw 'TODO: implement kebabCase';
}

// lodash
/** Creates a map of keys generated from the results of running each element of `collection`
 * thru `funcOrKey`. The corresponding value of each key is the last element responsible for
 * generating the key.
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param funcOrKey {function}: `(val, ckey) => key`; defaults to `identity`.
 *   @optional
 *
 * @returns {map}: the composed aggregate object.
 *
 * @example `var array = [ { 'dir': 'left', 'code': 97 }, { 'dir': 'right', 'code': 100 } ]; keyBy(array, function(o) { return String.fromCharCode(o.code); }); // => { 'a': { 'dir': 'left', 'code': 97 }, 'd': { 'dir': 'right', 'code': 100 } }`
 * @example `keyBy(array, 'dir'); // => { 'left': { 'dir': 'left', 'code': 97 }, 'right': { 'dir': 'right', 'code': 100 } }`
 */
export function keyBy(collection, funcOrKey) {
  if (false) { keyBy(collection, funcOrKey); }
  throw 'TODO: implement keyBy';
}

// flipshop
/** /** Keeps `varname` at `bagname ~ "_keys"` for as long as it hasn't been hand-edited; @see `defaultMaybe`. */
 */
// export function keylistEditLogic(context, id, oldDefinition, newDefinition, isCreating, specifiedParameters) {}

// lodash
/** Creates an array of the keys of `object`.
 *
 * @param object {map}: The map to query.
 *
 * @returns {array}: the array of keys.
 *
 * @example `keys({ 'a': 1, 'b': 2 }); // => ['a', 'b'] (iteration order is not guaranteed)`
 */
export function keys(object) {
  if (false) { keys(object); }
  throw 'TODO: implement keys';
}

// lodash
/** Same as `keys` -- FeatureScript maps have no own/inherited distinction, so there's no extra
 * reach for this to have over `keys`.
 *
 * @param object {map}: The map to query.
 *
 * @returns {array}: the array of keys.
 *
 * @example `keysIn({ 'a': 1, 'b': 2 }); // => ['a', 'b'] (iteration order is not guaranteed)`
 */
export function keysIn(object) {
  if (false) { keysIn(object); }
  throw 'TODO: implement keysIn';
}

// lodash
/** Gets the last element of `array`.
 *
 * @param array {array}: The array to query.
 *
 * @returns: the last element of `array`.
 *
 * @example `last([1, 2, 3]); // => 3`
 */
export function last(array) {
  if (false) { last(array); }
  throw 'TODO: implement last';
}

// flipshop
/** Index of the last occurrence of `val` in `arr`, searching from the end, or `-1` if absent.
 *
 * @example `lastIndexOf([1, 2, 1], 1); // => 2`
 */
// export function lastIndexOf(arr, val) {}

// lodash
/** Like `indexOf` except that it iterates over elements of
 * `array` from right to left.
 *
 * @param array {array}: The array to inspect.
 * @param value: The value to search for.
 * @param fromIndex {number}: The index to search from; defaults to `array.length-1`.
 *   @optional
 *
 * @returns {number}: the index of the matched value, else `-1`.
 *
 * @example `lastIndexOf([1, 2, 1, 2], 2); // => 3`
 * @example `lastIndexOf([1, 2, 1, 2], 2, 2); // => 1`
 */
export function lastIndexOf(array, value, fromIndex) {
  if (false) { lastIndexOf(array, value, fromIndex); }
  throw 'TODO: implement lastIndexOf';
}

// flipshop
/** /** `str` split into words, lowercased, and joined with a space. */
 */
// export function lowerCase(str) {}

// lodash
/** Converts `string`, as space separated words, to lower case.
 *
 * @param string {string}: The string to convert; defaults to `''`.
 *   @optional
 *
 * @returns {string}: the lower cased string.
 *
 * @example `lowerCase('--Foo-Bar--'); // => 'foo bar'`
 * @example `lowerCase('fooBar'); // => 'foo bar'`
 * @example `lowerCase('__FOO_BAR__'); // => 'foo bar'`
 */
export function lowerCase(string) {
  if (false) { lowerCase(string); }
  throw 'TODO: implement lowerCase';
}

// flipshop
/** `str` with only its first character lowercased, the rest left untouched -- `upperFirst`'s
 * counterpart.
 *
 * @example `lowerFirst("Fred"); // => "fred"`
 */
// export function lowerFirst(str) {}

// lodash
/** Converts the first character of `string` to lower case.
 *
 * @param string {string}: The string to convert; defaults to `''`.
 *   @optional
 *
 * @returns {string}: the converted string.
 *
 * @example `lowerFirst('Fred'); // => 'fred'`
 * @example `lowerFirst('FRED'); // => 'fRED'`
 */
export function lowerFirst(string) {
  if (false) { lowerFirst(string); }
  throw 'TODO: implement lowerFirst';
}

// lodash
/** Checks if `value` is less than `other`.
 *
 * @seeAlso [gt]
 *
 * @param value: The value to compare.
 * @param other: The other value to compare.
 *
 * @returns {boolean}: `true` if `value` is less than `other`, else `false`.
 *
 * @example `lt(1, 3); // => true`
 * @example `lt(3, 3); // => false`
 * @example `lt(3, 1); // => false`
 */
export function lt(value, other) {
  if (false) { lt(value, other); }
  throw 'TODO: implement lt';
}

// lodash
/** Checks if `value` is less than or equal to `other`.
 *
 * @seeAlso [gte]
 *
 * @param value: The value to compare.
 * @param other: The other value to compare.
 *
 * @returns {boolean}: `true` if `value` is less than or equal to `other`, else `false`.
 *
 * @example `lte(1, 3); // => true`
 * @example `lte(3, 3); // => true`
 * @example `lte(3, 1); // => false`
 */
export function lte(value, other) {
  if (false) { lte(value, other); }
  throw 'TODO: implement lte';
}

// lodash
/** Creates an array of values by running each element in `collection` thru `funcOrKey`.
 * `funcOrKey` is invoked as `(val, ckey)`.
 *
 * Many lodash methods are guarded to work as iteratees for methods like
 * `every`, `filter`, `map`, `mapValues`, `reject`, and `some`.
 *
 * The guarded methods are:
 * `ary`, `chunk`, `curry`, `curryRight`, `drop`, `dropRight`, `every`,
 * `fill`, `invert`, `parseInt`, `random`, `range`, `rangeRight`, `repeat`,
 * `sampleSize`, `slice`, `some`, `sortBy`, `split`, `take`, `takeRight`,
 * `template`, `trim`, `trimEnd`, `trimStart`, and `words`
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param funcOrKey {function|string|array}: iteratee invoked per iteration; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the new mapped array.
 *
 * @example `function square(n) { return n * n; } map([4, 8], square); // => [16, 64]`
 * @example `map({ 'a': 4, 'b': 8 }, square); // => [16, 64] (iteration order is not guaranteed)`
 * @example `var users = [ { 'user': 'barney' }, { 'user': 'fred' } ]; map(users, 'user'); // => ['barney', 'fred']`
 */
export function map(collection, funcOrKey) {
  if (false) { map(collection, funcOrKey); }
  throw 'TODO: implement map';
}

// flipshop
/** `bag`'s values, replacing each key with `funcOrKey(val, key)` -- `mapValues`' sibling for
 * keys instead of values. A collision on the computed key keeps the last entry that produced it.
 * `funcOrKey` is coerced through `iteratee` @see `iteratee`.
 *
 * @example `mapKeys({ "a": 1, "b": 2 }, function(val, key) { return key ~ val; }); // => { "a1": 1, "b2": 2 }`
 */
// export function mapKeys(bag, funcOrKey) {}

// lodash
/** The opposite of `mapValues`; this method creates a map with the same values as `object` and
 * keys generated by running each of `object`'s keys thru `funcOrKey`. `funcOrKey` is invoked
 * as `(val, key)`.
 *
 * @seeAlso [mapValues]
 *
 * @param object {map}: The object to iterate over.
 * @param funcOrKey {function|string|array}: iteratee invoked per iteration; defaults to `identity`.
 *   @optional
 *
 * @returns {map}: the new mapped object.
 *
 * @example `mapKeys({ 'a': 1, 'b': 2 }, function(value, key) { return key + value; }); // => { 'a1': 1, 'b2': 2 }`
 */
export function mapKeys(object, funcOrKey) {
  if (false) { mapKeys(object, funcOrKey); }
  throw 'TODO: implement mapKeys';
}

// flipshop
/** `bag`/`arr` with every value replaced by `func`'s result. Lodash splits this into two
 * functions -- `mapValues` keeps an object's keys, `map` returns a new array -- unified here under
 * one dispatch: `func` is `func(val, ckey)`.
 * `mapValues3` instead hands `func` all three of `val`, `ckey`, and a 0-based visit-count `iter`
 * when a map form needs to distinguish visit order from key order. The `keylist` and
 * `nilPolicy` overloads follow `forEach`'s rules: a `keylist` walks exactly those keys, and
 * `nilPolicy` decides whether an `undefined` value is mapped (`NIL`, the default) or its key
 * dropped from the result entirely (`SKIP`). `func` is coerced through `iteratee` @see `iteratee`
 * -- `mapValues(users, 'name')` extracts a `name` field from each.
 *
 * @example `mapValues({ "fred": 40, "pebbles": 1 }, function(age) { return age * 2; }); // => { "fred": 80, "pebbles": 2 }`
 * @example `mapValues([4, 8], function(n) { return n * n; }); // => [16, 64]`
 */
// export function mapValues(bag, keylist, nilPolicy, func) {}

// lodash
/** Creates a map with the same keys as `object` and values generated by running each of
 * `object`'s keys thru `funcOrKey`. `funcOrKey` is invoked as `(val, key)`.
 *
 * @seeAlso [mapKeys]
 *
 * @param object {map}: The object to iterate over.
 * @param funcOrKey {function|string|array}: iteratee invoked per iteration; defaults to `identity`.
 *   @optional
 *
 * @returns {map}: the new mapped object.
 *
 * @example `var users = { 'fred': { 'user': 'fred', 'age': 40 }, 'pebbles': { 'user': 'pebbles', 'age': 1 } }; mapValues(users, function(o) { return o.age; }); // => { 'fred': 40, 'pebbles': 1 } (iteration order is not guaranteed)`
 * @example `mapValues(users, 'age'); // => { 'fred': 40, 'pebbles': 1 } (iteration order is not guaranteed)`
 */
export function mapValues(object, funcOrKey) {
  if (false) { mapValues(object, funcOrKey); }
  throw 'TODO: implement mapValues';
}

// flipshop
/** Builds a rule that's `true` for any map holding `source`'s entries -- a partial deep match.
 * `(val, ckey)` callback shape, discarding `ckey` @see `conforms`.
 *
 * @example `matches({ "a": 1 })({ "a": 1, "b": 2 }, 0); // => true`
 * @example `matches({ "a": 1 })({ "a": 2, "b": 2 }, 0); // => false`
 */
// export function matches(source) {}

// lodash
/** Creates a function that performs a partial deep comparison between a given
 * map and `source`, returning `true` if the given map has `source`'s vals at
 * `source`'s keys, else `false`.
 *
 * **Note:** The created function is equivalent to `isMatch` with `source`
 * partially applied.
 *
 * Partial comparisons will match empty array and empty object `source`
 * values against any array or object value, respectively. See `isEqual`
 * for a list of supported value comparisons.
 *
 * **Note:** Multiple values can be checked by combining several matchers
 * using `overSome`
 *
 * @param source {map}: Map of vals to match.
 *
 * @returns {function}: the new spec function.
 *
 * @example `var objects = [ { 'a': 1, 'b': 2, 'c': 3 }, { 'a': 4, 'b': 5, 'c': 6 } ]; filter(objects, matches({ 'a': 4, 'c': 6 })); // => [{ 'a': 4, 'b': 5, 'c': 6 }]`
 * @example `filter(objects, overSome([matches({ 'a': 1 }), matches({ 'a': 4 })])); // => [{ 'a': 1, 'b': 2, 'c': 3 }, { 'a': 4, 'b': 5, 'c': 6 }]`
 */
export function matches(source) {
  if (false) { matches(source); }
  throw 'TODO: implement matches';
}

// flipshop
/** Builds a rule that's `true` when `path` of a given map equals `srcValue`; `path` is an
 * `anypath` @see `getAt`. `(val, ckey)` callback shape, discarding `ckey` @see `conforms`.
 *
 * @example `matchesProperty("a.b", 1)({ "a": { "b": 1 } }, 0); // => true`
 */
// export function matchesProperty(path, srcValue) {}

// lodash
/** Creates a function that performs a partial deep comparison between the
 * value at `path` of a given object to `srcValue`, returning `true` if the
 * object value is equivalent, else `false`.
 *
 * **Note:** Partial comparisons will match empty array and empty object
 * `srcValue` values against any array or object value, respectively. See
 * `isEqual` for a list of supported value comparisons.
 *
 * **Note:** Multiple values can be checked by combining several matchers
 * using `overSome`
 *
 * @param path {anypath}: Path to get.
 * @param srcValue: The value to match.
 *
 * @returns {function}: the new spec function.
 *
 * @example `var objects = [ { 'a': 1, 'b': 2, 'c': 3 }, { 'a': 4, 'b': 5, 'c': 6 } ]; find(objects, matchesProperty('a', 4)); // => { 'a': 4, 'b': 5, 'c': 6 }`
 * @example `filter(objects, overSome([matchesProperty('a', 1), matchesProperty('a', 4)])); // => [{ 'a': 1, 'b': 2, 'c': 3 }, { 'a': 4, 'b': 5, 'c': 6 }]`
 */
export function matchesProperty(path, srcValue) {
  if (false) { matchesProperty(path, srcValue); }
  throw 'TODO: implement matchesProperty';
}

// lodash
/** Computes the maximum value of `array`. If `array` is empty or falsey,
 * `undefined` is returned.
 *
 * @param array {array}: The array to iterate over.
 *
 * @returns: the maximum value.
 *
 * @example `max([4, 2, 8, 6]); // => 8`
 * @example `max([]); // => undefined`
 */
export function max(array) {
  if (false) { max(array); }
  throw 'TODO: implement max';
}

// flipshop
/** Element of `arr` for which `funcOrKey(val, idx)` is greatest, or `undefined` for an empty
 * `arr` -- std's array `max` picks the greatest value itself; this picks the element behind the
 * greatest *computed* value. `funcOrKey` is coerced through `iteratee` @see `iteratee`.
 *
 * @example `maxBy([{ "n": 1 }, { "n": 3 }, { "n": 2 }], (val) => val.n); // => { "n": 3 }`
 */
// export function maxBy(arr, funcOrKey) {}

// lodash
/** Like `max` except that it accepts `funcOrKey` which is
 * invoked for each element in `array` to generate the criterion by which
 * the value is ranked. `funcOrKey` is invoked as `(val, idx)`.
 *
 * @param array {array}: The array to iterate over.
 * @param funcOrKey {function}: `(val) => criteria`; defaults to `identity`.
 *   @optional
 *
 * @returns: the maximum value.
 *
 * @example `var objects = [{ 'n': 1 }, { 'n': 2 }]; maxBy(objects, function(o) { return o.n; }); // => { 'n': 2 }`
 * @example `maxBy(objects, 'n'); // => { 'n': 2 }`
 */
export function maxBy(array, funcOrKey) {
  if (false) { maxBy(array, funcOrKey); }
  throw 'TODO: implement maxBy';
}

// lodash
/** Computes the mean of the values in `array`.
 *
 * @param array {array}: The array to iterate over.
 *
 * @returns {number}: the mean.
 *
 * @example `mean([4, 2, 8, 6]); // => 5`
 */
export function mean(array) {
  if (false) { mean(array); }
  throw 'TODO: implement mean';
}

// flipshop
/** Average of `funcOrKey(val, idx)` across `arr` -- `average` *(std)*, mapped via `mapValues`.
 * `funcOrKey` is coerced through `iteratee` @see `iteratee`.
 *
 * @example `meanBy([{ "n": 2 }, { "n": 4 }], (val) => val.n); // => 3`
 */
// export function meanBy(arr, funcOrKey) {}

// lodash
/** Like `mean` except that it accepts `funcOrKey` which is
 * invoked for each element in `array` to generate the value to be averaged.
 * `funcOrKey` is invoked as `(val, idx)`.
 *
 * @param array {array}: The array to iterate over.
 * @param funcOrKey {function}: `(val) => criteria`; defaults to `identity`.
 *   @optional
 *
 * @returns {number}: the mean.
 *
 * @example `var objects = [{ 'n': 4 }, { 'n': 2 }, { 'n': 8 }, { 'n': 6 }]; meanBy(objects, function(o) { return o.n; }); // => 5`
 * @example `meanBy(objects, 'n'); // => 5`
 */
export function meanBy(array, funcOrKey) {
  if (false) { meanBy(array, funcOrKey); }
  throw 'TODO: implement meanBy';
}

// lodash
/** Creates a function that memoizes the result of `func`. If `resolver` is
 * provided, it determines the cache key for storing the result based on the
 * arguments provided to the memoized function. By default, the first argument
 * provided to the memoized function is used as the map cache key. The `func`
 * is invoked with the `this` binding of the memoized function.
 *
 * **Note:** The cache is exposed as the `cache` property on the memoized
 * function. Its creation may be customized by replacing the `memoize.Cache`
 * constructor with one whose instances implement the
 * [`Map`](http://ecma-international.org/ecma-262/7.0/#sec-properties-of-the-map-prototype-object)
 * method interface of `clear`, `delete`, `get`, `has`, and `set`.
 *
 * @param func {function}: Function to have its output memoized.
 * @param resolver {function}: Function to resolve the cache key.
 *   @optional
 *
 * @returns {function}: the new memoized function.
 *
 * @example `var object = { 'a': 1, 'b': 2 }; var other = { 'c': 3, 'd': 4 }; var values = memoize(values); values(object); // => [1, 2]`
 * @example `values(other); // => [3, 4]`
 * @example `object.a = 2; values(object); // => [1, 2]`
 * @example `values.cache.set(object, ['a', 'b']); values(object); // => ['a', 'b']`
 * @example `memoize.Cache = WeakMap;`
 */
export function memoize(func, resolver) {
  if (false) { memoize(func, resolver); }
  throw 'TODO: implement memoize';
}

// lodash
/** Like `assign` except that it recursively merges keys of source maps into the destination
 * map: a source key resolving to `undefined` is skipped if a destination value exists; array
 * and map values are merged recursively; anything else is overridden by assignment. Sources are
 * applied left to right. @see `deepMerge`, which does this for a single pair.
 *
 * @param object {map}: The destination map.
 * @param sources {map}: The source maps.
 *   @optional
 *
 * @returns {map}: `object`.
 *
 * @example `var object = { 'a': [{ 'b': 2 }, { 'd': 4 }] }; var other = { 'a': [{ 'c': 3 }, { 'e': 5 }] }; merge(object, other); // => { 'a': [{ 'b': 2, 'c': 3 }, { 'd': 4, 'e': 5 }] }`
 */
export function merge(object, sources) {
  if (false) { merge(object, sources); }
  throw 'TODO: implement merge';
}

// flipshop
/** Recursively merges `incoming` into `existing`: two maps combine key by key all the way down;
 * two arrays combine index by index, `existing`'s tail past `size(incoming)` surviving untouched,
 * matching lodash's `merge`; an `undefined` source leaves the existing value alone; anything
 * else, `incoming` wins.
 *
 * @param existing: Base value.
 * @param incoming: Value to merge in; wins any conflict that isn't two maps or two arrays.
 *
 * @example `deepMerge({ "a": { "x": 1 } }, { "a": { "y": 2 } }); // => { "a": { "x": 1, "y": 2 } }`
 * @example `deepMerge({ "a": [1, 2] }, { "a": [3] }); // => { "a": [3, 2] }`
 * @example `deepMerge({ "a": 1 }, undefined); // => { "a": 1 }`
 */
// export function deepMerge(existing, incoming) {}


// flipshop
/** `deepMerge`, with `combine(existingVal, incomingVal)` deciding what lands at a pair present in
 * both -- called at every level of the recursion, not just the leaves (including once per
 * index-pair when both sides are arrays), so returning `undefined` falls back to `deepMerge`'s
 * own rule for that pair. Unlike lodash's `mergeWith` customizer, `combine` gets neither a `key`
 * nor the whole source/destination objects.
 *
 * @example `mergeWith({ "a": 1 }, { "a": 2 }, (existingVal, incomingVal) => existingVal + incomingVal);`
 * @example `// => { "a": 3 }`
 */
// export function mergeWith(existing, incoming, combine) {}

// lodash
/** Like `merge` except that it accepts `customizer` which
 * is invoked to produce the merged values of the destination and source
 * properties. If `customizer` returns `undefined`, merging is handled by the
 * method instead. The `customizer` is invoked with six arguments:
 * (objValue, srcValue, key, object, source, stack).
 *
 * **Note:** This method mutates `object`.
 *
 * @param object {map}: The destination object.
 * @param sources {map}: The source objects.
 * @param customizer {function}: Function to customize assigned values.
 *
 * @returns {map}: `object`.
 *
 * @example `function customizer(objValue, srcValue) { if (isArray(objValue)) { return objValue.concat(srcValue); } } var object = { 'a': [1], 'b': [2] }; var other = { 'a': [3], 'b': [4] }; mergeWith(object, other, customizer); // => { 'a': [1, 3], 'b': [2, 4] }`
 */
export function mergeWith(object, sources, customizer) {
  if (false) { mergeWith(object, sources, customizer); }
  throw 'TODO: implement mergeWith';
}

// lodash
/** Creates a function that invokes the method at `path` of a given object.
 * Any additional arguments are provided to the invoked method.
 *
 * @param path {anypath}: Path to the method to invoke.
 * @param args: The arguments to invoke the method with.
 *   @optional
 *
 * @returns {function}: the new invoker function.
 *
 * @example `var objects = [ { 'a': { 'b': constant(2) } }, { 'a': { 'b': constant(1) } } ]; map(objects, method('a.b')); // => [2, 1]`
 * @example `map(objects, method(['a', 'b'])); // => [2, 1]`
 */
export function method(path, args) {
  if (false) { method(path, args); }
  throw 'TODO: implement method';
}

// lodash
/** The opposite of `method`; this method creates a function that invokes
 * the method at a given path of `object`. Any additional arguments are
 * provided to the invoked method.
 *
 * @param object {map}: The object to query.
 * @param args: The arguments to invoke the method with.
 *   @optional
 *
 * @returns {function}: the new invoker function.
 *
 * @example `var array = times(3, constant), object = { 'a': array, 'b': array, 'c': array }; map(['a[2]', 'c[0]'], methodOf(object)); // => [2, 0]`
 * @example `map([['a', '2'], ['c', '0']], methodOf(object)); // => [2, 0]`
 */
export function methodOf(object, args) {
  if (false) { methodOf(object, args); }
  throw 'TODO: implement methodOf';
}

// lodash
/** Computes the minimum value of `array`. If `array` is empty or falsey,
 * `undefined` is returned.
 *
 * @param array {array}: The array to iterate over.
 *
 * @returns: the minimum value.
 *
 * @example `min([4, 2, 8, 6]); // => 2`
 * @example `min([]); // => undefined`
 */
export function min(array) {
  if (false) { min(array); }
  throw 'TODO: implement min';
}

// flipshop
/** `maxBy`'s counterpart: element of `arr` for which `funcOrKey(val, idx)` is least, or
 * `undefined` for an empty `arr`. `funcOrKey` is coerced through `iteratee` @see `iteratee`.
 *
 * @example `minBy([{ "n": 1 }, { "n": 3 }, { "n": 2 }], (val) => val.n); // => { "n": 1 }`
 */
// export function minBy(arr, funcOrKey) {}

// lodash
/** Like `min` except that it accepts `funcOrKey` which is
 * invoked for each element in `array` to generate the criterion by which
 * the value is ranked. `funcOrKey` is invoked as `(val, idx)`.
 *
 * @param array {array}: The array to iterate over.
 * @param funcOrKey {function}: `(val) => criteria`; defaults to `identity`.
 *   @optional
 *
 * @returns: the minimum value.
 *
 * @example `var objects = [{ 'n': 1 }, { 'n': 2 }]; minBy(objects, function(o) { return o.n; }); // => { 'n': 1 }`
 * @example `minBy(objects, 'n'); // => { 'n': 1 }`
 */
export function minBy(array, funcOrKey) {
  if (false) { minBy(array, funcOrKey); }
  throw 'TODO: implement minBy';
}

// lodash
/** Adds all of `source`'s function-valued keys to the destination map.
 *
 * @param object {function|map}: The destination map; defaults to `lodash`.
 *   @optional
 * @param source {map}: Map of functions to add.
 * @param options {{
 *    @field chain {boolean}: Specify whether mixins are chainable; defaults to `true`.
 *     @optional
 * }}
 *
 * @returns {function|map}: `object`.
 *
 * @example `function vowels(string) { return filter(string, function(v) { return /[aeiou]/i.test(v); }); } mixin({ 'vowels': vowels }); vowels('fred'); // => ['e']`
 * @example `_('fred').vowels().value(); // => ['e']`
 * @example `mixin({ 'vowels': vowels }, { 'chain': false }); _('fred').vowels(); // => ['e']`
 */
export function mixin(object, source, options) {
  if (false) { mixin(object, source, options); }
  throw 'TODO: implement mixin';
}

// lodash
/** Multiply two numbers.
 *
 * @param multiplier {number}: The first number in a multiplication.
 * @param multiplicand {number}: The second number in a multiplication.
 *
 * @returns {number}: the product.
 *
 * @example `multiply(6, 4); // => 24`
 */
export function multiply(multiplier, multiplicand) {
  if (false) { multiply(multiplier, multiplicand); }
  throw 'TODO: implement multiply';
}

// lodash
/** Returns `undefined`.
 * @seeAlso [noop0] [noop1] [noop2]
 *
 * @example `times(2, noop); // => [undefined, undefined]`
 */
export function noop() {
  if (false) { noop(); }
  throw 'TODO: implement noop';
}

// flipshop
/** Element of `arr` at `idx`; a negative `idx` counts back from the end.
 * `undefined` if `idx` is out of bounds in either direction.
 *
 * @param array {array}: The array to examine.
 * @param idx {number}: The index of the element to return; defaults to `0`.
 *   @optional
 * @param fallback {any}: The value to return if `idx` is absent according to `nilPolicy`.
 *   @optional
 * @param nilPolicy {NilPolicy}: The policy to use for existing-but-undefined values;
 *   NIL means return `undefined`, SKIP means return `fallback`
 *   defaults to `NilPolicy.NIL`
 *
 * @returns: the nth element of `array`.
 *
 * @example `var array = ['a', 'b', 'c', 'd']; nth(array, 1); // => 'b'`
 * @example `nth(array, -2); // => 'c';`
 */
export function nth(array, idx, fallback, nilPolicy) {}

// lodash
/** Creates a function that gets the argument at index `n`. If `n` is negative,
 * the nth argument from the end is returned.
 *
 * @param n {number}: The index of the argument to return; defaults to `0`.
 *   @optional
 *
 * @returns {function}: the new pass-thru function.
 *
 * @example `var func = nthArg(1); func('a', 'b', 'c', 'd'); // => 'b'`
 * @example `var func = nthArg(-2); func('a', 'b', 'c', 'd'); // => 'c'`
 */
export function nthArg(n) {
  if (false) { nthArg(n); }
  throw 'TODO: implement nthArg';
}

// flipshop
/** Creates a map whose keys are values of `arr`, mapped to the value given by `funcOrKey`
 * On a collision, the last element responsible for a given key wins.
 *
 * @example `objectify(["a", "b", "c"], function(val, idx) { return idx; }); // => { "a": 0, "b": 1, "c": 2 }`
 * @example `objectify(["x", "x"], function(val, idx) { return idx; }); // => { "x": 1 }`
 */
// export function objectify(arr, func) {}

/** Creates a map of items in `bag` that are not in `pathlist`;
 * The opposite of `pick`
 *
 * @param bag {map}: The source map.
 * @param pathlist {array}: Each entry a key or keypath to omit.
 *   @optional
 *
 * @returns {map}: the new map.
 *
 * @example `var object = { 'a': 1, 'b': '2', 'c': 3 }; omit(object, ['a', 'c']); // => { 'b': '2' }`
 */
export function omit(object, pathlist) {
  if (false) { omit(object, pathlist); }
  throw 'TODO: implement omit';
}

// lodash
/** Creates a map of `object`'s keys that `rule` doesn't return truthy for.
 * `rule` is invoked as `(val, key)`. The opposite of `pickBy`
 *
 * @param object {map}: The source map.
 * @param rule {ruleOrKey}: Coerced to a rule; defaults to `identity`.
 *   @optional
 *
 * @returns {map}: the new map.
 *
 * @example `var object = { 'a': 1, 'b': '2', 'c': 3 }; omitBy(object, isNumber); // => { 'b': '2' }`
 */
export function omitBy(object, rule) {
  if (false) { omitBy(object, rule); }
  throw 'TODO: implement omitBy';
}

// lodash
/** Creates a function that is restricted to invoking `func` once. Repeat calls
 * to the function return the value of the first invocation.
 * @seeAlso [before] [after]
 *
 * @param func {function}: Function to restrict.
 * @param arity {number}: The number of arguments (0 <= arity <= 5) to invoke `func` with. Defaults to 2.
 *
 * @returns {function}: the new restricted function.
 *
 * @example `var initialize = once(createApplication); initialize(); initialize(); // => 'createApplication' is invoked once`
 */
export function once(func) {
  if (false) { once(func); }
  throw 'TODO: implement once';
}

// flipshop

/** Creates an array of elements, sorted in ascending order by the results of
 * running each element in a collection thru each `funcOrKey`.
 * If the `order` for any axis is unspecified, all values are sorted in the ascending order given by the comparator.
 * Otherwise, specify the sort order of corresponding values
 * as -1 for descending, 1 for ascending, and 0 to ignore that axis.
 * This method performs a stable sort, that is, it preserves the original sort order of
 * equal elements.
 * Each `funcOrKey` is invoked as `(val, ckey)`.
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param funcOrKeys {function|string|array}: One `funcOrKey` per sort axis; defaults to `[identity]`.
 *   @optional
 * @param orders {number|number[]}: The sort orders of `funcOrKeys`. If a bare number is given, it is used for all axes. if there are more funcOrKeys than `orders`, the remaining ones will sort ascending (the default)
 *   @optional
 * @param comparators {function|function[]}: `(criteriaA, criteriaB) => number`, applied to pairs of `funcOrKey`'s results.
 *   If a bare function is given, it is used for all axes.
 *   If there are more funcOrKeys than `comparators`, the remaining ones will use cmpAny.
 *   Defaults to `cmpAny` -- incompatible types nonetheless have a consistent total ordering.
 *
 * @returns {array}: the new sorted array.
 *
 * @example `var users = [ { 'user': 'fred', 'age': 48 }, { 'user': 'barney', 'age': 34 }, { 'user': 'fred', 'age': 40 }, { 'user': 'barney', 'age': 36 } ]; orderBy(users, ['user', 'age'], [1, -1]); // => objects for [['barney', 36], ['barney', 34], ['fred', 48], ['fred', 40]]`
 * @example `orderBy([3, 1, 2]); // => [1, 2, 3]`
 * @example `orderBy([{ "n": 3 }, { "n": 1 }], "n"); // => [{ "n": 1 }, { "n": 3 }]`
 * @example `orderBy([1, 2, 3], identity, -1); // => [3, 2, 1]`
 * @example `orderBy({ "a": 3, "b": 1 }, identity); // => [1, 3]`
 */
// export function orderAnyBy(vals, funcOrKeys, orders, comparators) {}


// flipshop

/** Creates an array of elements, sorted in ascending order by the results of
 * running each element in a collection thru each `funcOrKey`.
 * If the `order` for any axis is unspecified, all values are sorted in the ascending order given by the comparator.
 * Otherwise, specify the sort order of corresponding values
 * as -1 for descending, 1 for ascending, and 0 to ignore that axis.
 * This method performs a stable sort, that is, it preserves the original sort order of
 * equal elements.
 * Each `funcOrKey` is invoked as `(val, ckey)`.
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param funcOrKeys {function|string|array}: One `funcOrKey` per sort axis; defaults to `[identity]`.
 *   @optional
 * @param orders {number|number[]}: The sort orders of `funcOrKeys`. If a bare number is given, it is used for all axes. if there are more funcOrKeys than `orders`, the remaining ones will sort ascending (the default)
 *   @optional
 * @param comparators {function|function[]}: `(criteriaA, criteriaB) => number`, applied to pairs of `funcOrKey`'s results. If a bare function is given, it is used for all axes. if there are more funcOrKeys than `comparators `, the remaining ones will use cmp (or cmpAny if called as orderByAny);
 *   Defaults to `cmp` -- incompatible types throw. Pass `cmpAny` to allow mixed types, or use the orderByAny convenience function.
 *
 * @returns {array}: the new sorted array.
 *
 * @example `var users = [ { 'user': 'fred', 'age': 48 }, { 'user': 'barney', 'age': 34 }, { 'user': 'fred', 'age': 40 }, { 'user': 'barney', 'age': 36 } ]; orderBy(users, ['user', 'age'], [1, -1]); // => objects for [['barney', 36], ['barney', 34], ['fred', 48], ['fred', 40]]`
 * @example `orderBy([3, 1, 2]); // => [1, 2, 3]`
 * @example `orderBy([{ "n": 3 }, { "n": 1 }], "n"); // => [{ "n": 1 }, { "n": 3 }]`
 * @example `orderBy([1, 2, 3], identity, -1); // => [3, 2, 1]`
 * @example `orderBy({ "a": 3, "b": 1 }, identity); // => [1, 3]`
 */
export function orderBy(collection, funcOrKeys, orders, comparators) {}

// flipshop
/** Creates a function`overfunc(vals is array)` invoking `funcs` on its argument
 * and returning their results as an array of results.
 *
 * @param funcs {array}: Each a `funcOrKey`; defaults to `[identity]`.
 *   @optional
 *
 * @returns {function}: the new function.
 *
 * @example `var overfunc = over([Math.max, Math.min]); overfunc([1, 2, 3, 4]); // => [4, 1]`
 */
// export function over(funcOrKeys) {}


// lodash
/** Creates a function that invokes `func` with its arguments transformed.
 *
 * @param func {function}: Function to wrap.
 * @param transforms {(Function|Function[])}: ] The argument transforms; defaults to `[identity`.
 *   @optional
 *
 * @returns {function}: the new function.
 *
 * @example `function doubled(n) { return n * 2; } function square(n) { return n * n; } var func = overArgs(function(x, y) { return [x, y]; }, [square, doubled]); func(9, 3); // => [81, 6]`
 * @example `func(10, 5); // => [100, 10]`
 */
export function overArgs(func, transforms) {
  if (false) { overArgs(func, transforms); }
  throw 'TODO: implement overArgs';
}

// flipshop
/** Builds a rule that's `true` only when every function in `funcs` returns truthy for
 * `(val, ckey)`. Each element of `funcs` is a `ruleOrKey`; @see `over`.
 *
 * @example `overEvery([(val, _ckey) => val > 0, (val, _ckey) => val < 10])(5, 0); // => true`
 */
// export function overEvery(funcs) {}

// lodash
/** Creates a function that checks if **all** of the `rules` return
 * truthy when invoked with the arguments it receives. Each of `rules` is a `ruleOrKey`, so a
 * partial-match map or a `[path, srcValue]` array can stand in for a literal rule function.
 *
 * @param rules {array}: Each a `ruleOrKey`; defaults to `[identity]`.
 *   @optional
 *
 * @returns {function}: the new function.
 *
 * @example `var func = overEvery([Boolean, isFinite]); func('1'); // => true`
 * @example `func(null); // => false`
 * @example `func(NaN); // => false`
 */
export function overEvery(rules) {
  if (false) { overEvery(rules); }
  throw 'TODO: implement overEvery';
}

// flipshop
/** Builds a rule that's `true` when any function in `funcs` returns truthy for `(val, ckey)`. Each
 * element of `funcs` is a `ruleOrKey`; @see `over`.
 *
 * @example `overSome([(val, _ckey) => val < 0, (val, _ckey) => val > 10])(5, 0); // => false`
 */
// export function overSome(funcs) {}

// lodash
/** Creates a function that checks if **any** of the `rules` return
 * truthy when invoked with the arguments it receives. Each of `rules` is a `ruleOrKey`, so a
 * partial-match map or a `[path, srcValue]` array can stand in for a literal rule function.
 *
 * @param rules {array}: Each a `ruleOrKey`; defaults to `[identity]`.
 *   @optional
 *
 * @returns {function}: the new function.
 *
 * @example `var func = overSome([Boolean, isFinite]); func('1'); // => true`
 * @example `func(null); // => true`
 * @example `func(NaN); // => false`
 * @example `var matchesFunc = overSome([{ 'a': 1 }, { 'a': 2 }]) var matchesPropertyFunc = overSome([['a', 1], ['a', 2]])`
 */
export function overSome(rules) {
  if (false) { overSome(rules); }
  throw 'TODO: implement overSome';
}

// flipshop
/** Pads `str` on both sides if it's shorter than `minlen`, splitting the padding as evenly as
 * possible and favoring the right side when it's odd -- `padLeft`/`padRight`'s two-sided sibling.
 *
 * @example `pad("hi", 6); // => " hi "`
 * @example `pad("hi", 5); // => " hi "`
 */
// export function pad(str, minlen, padstr) {}

// lodash
/** Pads `string` on the left and right sides if it's shorter than `length`.
 * Padding characters are truncated if they can't be evenly divided by `length`.
 *
 * @param string {string}: The string to pad; defaults to `''`.
 *   @optional
 * @param length {number}: The padding length; defaults to `0`.
 *   @optional
 * @param chars {string}: The string used as padding; defaults to `' '`.
 *   @optional
 *
 * @returns {string}: the padded string.
 *
 * @example `pad('abc', 8); // => ' abc '`
 * @example `pad('abc', 8, '_-'); // => '_-abc_-_'`
 * @example `pad('abc', 3); // => 'abc'`
 */
export function pad(string, length, chars) {
  if (false) { pad(string, length, chars); }
  throw 'TODO: implement pad';
}

// flipshop
/** /** `padstr` repeated enough times to reach at least `neededLen` characters -- not truncated to it; that's the caller's job. */
 */
// export function paddingFor(padstr, neededLen) {}

// lodash
/** Pads `string` on the right side if it's shorter than `length`. Padding
 * characters are truncated if they exceed `length`.
 *
 * @param string {string}: The string to pad; defaults to `''`.
 *   @optional
 * @param length {number}: The padding length; defaults to `0`.
 *   @optional
 * @param chars {string}: The string used as padding; defaults to `' '`.
 *   @optional
 *
 * @returns {string}: the padded string.
 *
 * @example `padEnd('abc', 6); // => 'abc '`
 * @example `padEnd('abc', 6, '_-'); // => 'abc_-_'`
 * @example `padEnd('abc', 3); // => 'abc'`
 */
export function padEnd(string, length, chars) {
  if (false) { padEnd(string, length, chars); }
  throw 'TODO: implement padEnd';
}

// flipshop
/** Pads `str` on the left side if it's shorter than `minlen`. Padding characters are truncated if
 * they exceed `minlen`. Overloads: `padLeft(num is number, minlen is number, padstr is string)`
 * and `padLeft(num is number, minlen is number)` call `padLeft` on the stringified value.
 *
 * @example `padLeft("hello world", 12); // => " hello world"`
 * @example `padLeft("hello world", 12, "!"); // => "!hello world"`
 * @example `padLeft("hello world", 11); // => "hello world"`
 */
// export function padLeft(str, minlen, padstr) {}

// flipshop
/** Pads `str` on the right side if it's shorter than `minlen`. Padding characters are truncated
 * if they exceed `minlen`. Unlike `padLeft`, there's no numeric overload -- a `number` has to be
 * stringified by the caller first.
 *
 * @example `padRight("hello world", 12); // => "hello world "`
 * @example `padRight("hello world", 12, "!"); // => "hello world!"`
 */
// export function padRight(str, minlen, padstr) {}

// lodash
/** Pads `string` on the left side if it's shorter than `length`. Padding
 * characters are truncated if they exceed `length`.
 *
 * @param string {string}: The string to pad; defaults to `''`.
 *   @optional
 * @param length {number}: The padding length; defaults to `0`.
 *   @optional
 * @param chars {string}: The string used as padding; defaults to `' '`.
 *   @optional
 *
 * @returns {string}: the padded string.
 *
 * @example `padStart('abc', 6); // => ' abc'`
 * @example `padStart('abc', 6, '_-'); // => '_-_abc'`
 * @example `padStart('abc', 3); // => 'abc'`
 */
export function padStart(string, length, chars) {
  if (false) { padStart(string, length, chars); }
  throw 'TODO: implement padStart';
}

// lodash
/** Converts `string` to an integer of the specified radix. If `radix` is
 * `undefined` or `0`, a `radix` of `10` is used unless `value` is a
 * hexadecimal, in which case a `radix` of `16` is used.
 *
 * **Note:** This method aligns with the
 * [ES5 implementation](https://es5.github.io/#x15.1.2.2) of `parseInt`.
 *
 * @param string {string}: The string to convert.
 * @param radix {number}: The radix to interpret `value` by; defaults to `10`.
 *   @optional
 *
 * @returns {number}: the converted integer.
 *
 * @example `parseInt('08'); // => 8`
 * @example `map(['6', '08', '10'], parseInt); // => [6, 8, 10]`
 */
export function parseInt(string, radix) {
  if (false) { parseInt(string, radix); }
  throw 'TODO: implement parseInt';
}

// flipshop
/** Parses `rawjson` into a map/array, wrapping a parse failure in a `regenError` (naming `story`,
 * a caller-supplied label) instead of surfacing `parseJson`'s own opaque throw.
 *
 * @param opts {map}: keyword options - @field [detectUnits=false] {boolean}: Parse unit-bearing strings (e.g. `"3 inch"`) into a `ValueWithUnits`, via `parseJsonWithUnits`. - @field [story="because"] {string}: Label for the error message, so a caller can say what it was trying to do.
 *
 * @example `parseJsonSafely('{"a": 1}'); // => { "a": 1 }`
 */
// export function parseJsonSafely(rawjson, opts) {}

// lodash
/** Creates a function that invokes `func` with `partials` prepended to the
 * arguments it receives. Like `bind` except it does **not**
 * alter the `this` binding.
 *
 * The `partial.placeholder` value, which defaults to `_` in monolithic
 * builds, may be used as a placeholder for partially applied arguments.
 *
 * @param func {function}: Function to partially apply arguments to.
 * @param partials: The arguments to be partially applied.
 *   @optional
 *
 * @returns {function}: the new partially applied function.
 *
 * @example `function greet(greeting, name) { return greeting + ' ' + name; } var sayHelloTo = partial(greet, 'hello'); sayHelloTo('fred'); // => 'hello fred'`
 * @example `var greetFred = partial(greet, _, 'fred'); greetFred('hi'); // => 'hi fred'`
 */
export function partial(func, partials) {
  if (false) { partial(func, partials); }
  throw 'TODO: implement partial';
}

// lodash
/** Like `partial` except that partially applied arguments
 * are appended to the arguments it receives.
 *
 * The `partialRight.placeholder` value, which defaults to `_` in monolithic
 * builds, may be used as a placeholder for partially applied arguments.
 *
 * **Note:** This method doesn't set the "length" property of partially
 * applied functions.
 *
 * @param func {function}: Function to partially apply arguments to.
 * @param partials: The arguments to be partially applied.
 *   @optional
 *
 * @returns {function}: the new partially applied function.
 *
 * @example `function greet(greeting, name) { return greeting + ' ' + name; } var greetFred = partialRight(greet, 'fred'); greetFred('hi'); // => 'hi fred'`
 * @example `var sayHelloTo = partialRight(greet, 'hello', _); sayHelloTo('fred'); // => 'hello fred'`
 */
export function partialRight(func, partials) {
  if (false) { partialRight(func, partials); }
  throw 'TODO: implement partialRight';
}

// flipshop
/** `[passed, failed]` -- `bag`/`arr` split into the elements for which `rule` holds and the ones
 * for which it doesn't, keeping visiting order. `rule` gets `(val, ckey)`; the map form returns
 * values only, same as lodash's collection form. `rule` is a `ruleOrKey`.
 *
 * @example `partition([1, 2, 3, 4], (val, _seq) => val % 2 == 0); // => [[2, 4], [1, 3]]`
 */
// export function partition(arr, rule) {}

// lodash
/** Creates an array of elements split into two groups, the first of which
 * contains elements `rule` returns truthy for, the second of which
 * contains elements `rule` returns falsey for. `rule` is invoked as `(val, ckey)`.
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param rule {ruleOrKey}: Coerced to a rule; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the array of grouped elements.
 *
 * @example `var users = [ { 'user': 'barney', 'age': 36, 'active': false }, { 'user': 'fred', 'age': 40, 'active': true }, { 'user': 'pebbles', 'age': 1, 'active': false } ]; partition(users, function(o) { return o.active; }); // => objects for [['fred'], ['barney', 'pebbles']]`
 * @example `partition(users, { 'age': 1, 'active': false }); // => objects for [['pebbles'], ['barney', 'fred']]`
 * @example `partition(users, ['active', false]); // => objects for [['barney', 'pebbles'], ['fred']]`
 * @example `partition(users, 'active'); // => objects for [['fred'], ['barney', 'pebbles']]`
 */
export function partition(collection, rule) {
  if (false) { partition(collection, rule); }
  throw 'TODO: implement partition';
}

// flipshop
/** Converts a dotted string into a path array -- @see `getAt`/`setAt`'s second argument. Unlike
 * lodash's `toPath`, there's no `a[0].b` bracket syntax; an array index is just another
 * dot-separated segment (`"a.0.b"`).
 *
 * An empty segment names an empty key, which a map is perfectly willing to hold, so `"a..b"` is
 * three steps and `".foo"` is two. A key with a terminal dot is unhandled: `"foo."` reads as
 * `"foo"`, dropping the trailing empty segment rather than keeping it as a trailing empty key.
 *
 * @param keyname {string}: Dotted path.
 *
 * @example `pathForKey("a.b.c"); // => ["a", "b", "c"]`
 * @example `pathForKey("a..b"); // => ["a", "", "b"]`
 * @example `pathForKey("foo."); // => ["foo"]`
 */
// export function pathForKey(keyname) {}

// flipshop
/** A map with just `bag`'s entries at `keylist` -- like lodash's `pick`, an absent key is simply
 * missing from the result rather than present with an `undefined` value. Each entry of `keylist`
 * can be a dotted string or key-path array @see `getAt`, reaching into a nested structure and
 * rebuilding the same nesting in the result -- `pick({ "a": { "b": 1 } }, ["a.b"])` is
 * `{ "a": { "b": 1 } }`, not a flat `{ "a.b": 1 }`. This is lossy against a key that already
 * contains a literal dot, same caveat as `dotMap`/`undotMap`.
 *
 * `pickDefined` additionally drops a key whose value is `undefined` -- for a map this is the same
 * result as `pick`, since a map can never hold an `undefined` value to differ over. Unlike
 * `pickBy`, the rule isn't customizable and the keys considered are exactly `keylist`, not every
 * key of `bag`. `pickDefined` only accepts a literal top-level key, not a dotted path.
 *
 * @example `pick({ "a": 1, "b": 2, "c": 3 }, ["a", "c"]); // => { "a": 1, "c": 3 }`
 * @example `pick({ "a": { "b": 1, "c": 2 } }, ["a.b"]); // => { "a": { "b": 1 } }`
 * @example `pickDefined({ "a": 1, "b": undefined, "c": 3 }, ["a", "b", "c"]); // => { "a": 1, "c": 3 }`
 */
// export function pick(bag, keylist) {}

// lodash
/** Creates a map of the picked keys of `object`.
 *
 * @param object {map}: The source map.
 * @param keylist {array}: Each entry an `anypath` @see `getAt`, to pick.
 *   @optional
 *
 * @returns {map}: the new map.
 *
 * @example `var object = { 'a': 1, 'b': '2', 'c': 3 }; pick(object, ['a', 'c']); // => { 'a': 1, 'c': 3 }`
 */
export function pick(object, keylist) {
  if (false) { pick(object, keylist); }
  throw 'TODO: implement pick';
}

// flipshop
/** `bag`'s entries for which `rule(val, key)` holds -- the inverse of `omitBy`, and the
 * generic-rule sibling of `pickDefined`'s fixed "is defined" check, matching lodash's
 * `pickBy(object, [rule=identity])`.
 *
 * @example `pickBy({ "a": 1, "b": 2, "c": 3 }, (val, _key) => val > 1); // => { "b": 2, "c": 3 }`
 */
// export function pickBy(bag, rule) {}

// lodash
/** Creates a map of `object`'s keys that `rule` returns truthy for. `rule` is invoked as
 * `(val, key)`.
 *
 * @param object {map}: The source map.
 * @param rule {ruleOrKey}: Coerced to a rule; defaults to `identity`.
 *   @optional
 *
 * @returns {map}: the new map.
 *
 * @example `var object = { 'a': 1, 'b': '2', 'c': 3 }; pickBy(object, isNumber); // => { 'a': 1, 'c': 3 }`
 */
export function pickBy(object, rule) {
  if (false) { pickBy(object, rule); }
  throw 'TODO: implement pickBy';
}

// lodash
/** Creates a function that returns the value at `path` of a given object.
 *
 * @param path {anypath}: Path to get.
 *
 * @returns {function}: the new accessor function.
 *
 * @example `var objects = [ { 'a': { 'b': 2 } }, { 'a': { 'b': 1 } } ]; map(objects, property('a.b')); // => [2, 1]`
 * @example `map(sortBy(objects, property(['a', 'b'])), 'a.b'); // => [1, 2]`
 */
export function property(path) {
  if (false) { property(path); }
  throw 'TODO: implement property';
}

// lodash
/** The opposite of `property`; this method creates a function that returns the value at a given
 * `anypath` of `object`.
 *
 * @param object {map}: The map to query.
 *
 * @returns {function}: the new accessor function.
 *
 * @example `var array = [0, 1, 2], object = { 'a': array, 'b': array, 'c': array }; map(['a.2', 'c.0'], propertyOf(object)); // => [2, 0]`
 * @example `map([['a', '2'], ['c', '0']], propertyOf(object)); // => [2, 0]`
 */
export function propertyOf(object) {
  if (false) { propertyOf(object); }
  throw 'TODO: implement propertyOf';
}

// lodash
/** Removes all given values from `array` using
 * [`SameValueZero`](http://ecma-international.org/ecma-262/7.0/#sec-samevaluezero)
 * for equality comparisons.
 *
 * **Note:** Unlike `without`, this method mutates `array`. Use `remove`
 * to remove elements from an array by rule.
 *
 * @param array {array}: The array to modify.
 * @param values: The values to remove.
 *   @optional
 *
 * @returns {array}: `array`.
 *
 * @example `var array = ['a', 'b', 'c', 'a', 'b', 'c']; pull(array, 'a', 'c'); println(array); // => ['b', 'b']`
 */
export function pull(array, values) {
  if (false) { pull(array, values); }
  throw 'TODO: implement pull';
}

// lodash
/** Like `pull` except that it accepts an array of values to remove.
 *
 * **Note:** Unlike `difference`, this method mutates `array`.
 *
 * @param array {array}: The array to modify.
 * @param values {array}: The values to remove.
 *
 * @returns {array}: `array`.
 *
 * @example `var array = ['a', 'b', 'c', 'a', 'b', 'c']; pullAll(array, ['a', 'c']); println(array); // => ['b', 'b']`
 */
export function pullAll(array, values) {
  if (false) { pullAll(array, values); }
  throw 'TODO: implement pullAll';
}

// lodash
/** Like `pullAll` except that it accepts `funcOrKey` which is
 * invoked for each element of `array` and `values` to generate the criterion
 * by which they're compared. `funcOrKey` is invoked as `(val, idx)`.
 *
 * **Note:** Unlike `differenceBy`, this method mutates `array`.
 *
 * @param array {array}: The array to modify.
 * @param values {array}: The values to remove.
 * @param funcOrKey {function}: `(val) => criteria`; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: `array`.
 *
 * @example `var array = [{ 'x': 1 }, { 'x': 2 }, { 'x': 3 }, { 'x': 1 }]; pullAllBy(array, [{ 'x': 1 }, { 'x': 3 }], 'x'); println(array); // => [{ 'x': 2 }]`
 */
export function pullAllBy(array, values, funcOrKey) {
  if (false) { pullAllBy(array, values, funcOrKey); }
  throw 'TODO: implement pullAllBy';
}

// lodash
/** Like `pullAll` except that it accepts `comparator` which
 * is invoked to compare elements of `array` to `values`. The comparator is
 * invoked with two arguments: (arrVal, othVal).
 *
 * **Note:** Unlike `differenceWith`, this method mutates `array`.
 *
 * @param array {array}: The array to modify.
 * @param values {array}: The values to remove.
 * @param comparator {function}: The comparator invoked per element.
 *   @optional
 *
 * @returns {array}: `array`.
 *
 * @example `var array = [{ 'x': 1, 'y': 2 }, { 'x': 3, 'y': 4 }, { 'x': 5, 'y': 6 }]; pullAllWith(array, [{ 'x': 3, 'y': 4 }], isEqual); println(array); // => [{ 'x': 1, 'y': 2 }, { 'x': 5, 'y': 6 }]`
 */
export function pullAllWith(array, values, comparator) {
  if (false) { pullAllWith(array, values, comparator); }
  throw 'TODO: implement pullAllWith';
}

// lodash
/** Removes elements from `array` corresponding to `indexes` and returns an
 * array of removed elements.
 *
 * **Note:** Unlike `at`, this method mutates `array`.
 *
 * @param array {array}: The array to modify.
 * @param indexes {(number|number[])}: The indexes of elements to remove.
 *   @optional
 *
 * @returns {array}: the new array of removed elements.
 *
 * @example `var array = ['a', 'b', 'c', 'd']; var pulled = pullAt(array, [1, 3]); println(array); // => ['a', 'c']`
 * @example `println(pulled); // => ['b', 'd']`
 */
export function pullAt(array, indexes) {
  if (false) { pullAt(array, indexes); }
  throw 'TODO: implement pullAt';
}

// lodash
/** Produces a random number between the inclusive `lower` and `upper` bounds.
 * If only one argument is provided a number between `0` and the given number
 * is returned. If `floating` is `true`, or either `lower` or `upper` are
 * floats, a floating-point number is returned instead of an integer.
 *
 * **Note:** JavaScript follows the IEEE-754 standard for resolving
 * floating-point values which can produce unexpected results.
 *
 * **Note:** If `lower` is greater than `upper`, the values are swapped.
 *
 * @param lower {number}: The lower bound; defaults to `0`.
 *   @optional
 * @param upper {number}: The upper bound; defaults to `1`.
 *   @optional
 * @param floating {boolean}: Specify returning a floating-point number.
 *   @optional
 *
 * @returns {number}: the random number.
 *
 * @example `random(0, 5); // => an integer between 0 and 5`
 * @example `random(5, 0); // => an integer between 0 and 5`
 * @example `random(5); // => also an integer between 0 and 5`
 * @example `random(-5); // => an integer between -5 and 0`
 * @example `random(5, true); // => a floating-point number between 0 and 5`
 * @example `random(1.2, 5.2); // => a floating-point number between 1.2 and 5.2`
 */
export function random(lower, upper, floating) {
  if (false) { random(lower, upper, floating); }
  throw 'TODO: implement random';
}

// lodash
/** Creates an array of numbers (positive and/or negative) progressing from
 * `start` up to, but not including, `end`. A step of `-1` is used if a negative
 * `start` is specified without an `end` or `step`. If `end` is not specified,
 * it's set to `start` with `start` then set to `0`.
 *
 * **Note:** JavaScript follows the IEEE-754 standard for resolving
 * floating-point values which can produce unexpected results.
 *
 * @seeAlso [inRange]
 * @seeAlso [rangeRight]
 *
 * @param start {number}: The start of the range; defaults to `0`.
 *   @optional
 * @param end {number}: The end of the range.
 * @param step {number}: The value to increment or decrement by; defaults to `1`.
 *   @optional
 *
 * @returns {array}: the range of numbers.
 *
 * @example `range(4); // => [0, 1, 2, 3]`
 * @example `range(-4); // => [0, -1, -2, -3]`
 * @example `range(1, 5); // => [1, 2, 3, 4]`
 * @example `range(0, 20, 5); // => [0, 5, 10, 15]`
 * @example `range(0, -4, -1); // => [0, -1, -2, -3]`
 * @example `range(1, 4, 0); // => [1, 1, 1]`
 * @example `range(0); // => []`
 */
export function range(start, end, step) {
  if (false) { range(start, end, step); }
  throw 'TODO: implement range';
}

// flipshop
/** /** Resolves `pos` (only `SequencePosition.END` is defined) against `beg`/`end`; `undefined` otherwise. */
 */
// export function rangedSequencePosition(pos, beg, end) {}

// lodash
/** Like `range` except that it populates values in
 * descending order.
 *
 * @seeAlso [inRange]
 * @seeAlso [range]
 *
 * @param start {number}: The start of the range; defaults to `0`.
 *   @optional
 * @param end {number}: The end of the range.
 * @param step {number}: The value to increment or decrement by; defaults to `1`.
 *   @optional
 *
 * @returns {array}: the range of numbers.
 *
 * @example `rangeRight(4); // => [3, 2, 1, 0]`
 * @example `rangeRight(-4); // => [-3, -2, -1, 0]`
 * @example `rangeRight(1, 5); // => [4, 3, 2, 1]`
 * @example `rangeRight(0, 20, 5); // => [15, 10, 5, 0]`
 * @example `rangeRight(0, -4, -1); // => [-3, -2, -1, 0]`
 * @example `rangeRight(1, 4, 0); // => [1, 1, 1]`
 * @example `rangeRight(0); // => []`
 */
export function rangeRight(start, end, step) {
  if (false) { rangeRight(start, end, step); }
  throw 'TODO: implement rangeRight';
}

// lodash
/** Creates a function that invokes `func` with arguments arranged according
 * to the specified `indexes` where the argument value at the first index is
 * provided as the first argument, the argument value at the second index is
 * provided as the second argument, and so on.
 *
 * @param func {function}: Function to rearrange arguments for.
 * @param indexes {(number|number[])}: The arranged argument indexes.
 *
 * @returns {function}: the new function.
 *
 * @example `var rearged = rearg(function(a, b, c) { return [a, b, c]; }, [2, 0, 1]); rearged('b', 'c', 'a') // => ['a', 'b', 'c']`
 */
export function rearg(func, indexes) {
  if (false) { rearg(func, indexes); }
  throw 'TODO: implement rearg';
}

// flipshop
/** Rebuilds a map (from a map or an array) by asking `func(val, ckey)` for the `[newKey, newVal]`
 * pair each entry becomes; an entry where `func` returns `undefined` (rather than a pair) is
 * dropped rather than written under an `undefined` key. Where two entries land on the same
 * `newKey`, the later one wins -- `keys(bag)` order for a map, index order for an array.
 */
// export function rebag(arr, func) {}

// lodash
/** Used to match [combining diacritical marks](https://en.wikipedia.org/wiki/Combining_Diacritical_Marks) and
 * [combining diacritical marks for symbols](https://en.wikipedia.org/wiki/Combining_Diacritical_Marks_for_Symbols).
 */
export function reComboMark(rsCombo, 'g') {
  if (false) { reComboMark(rsCombo, 'g'); }
  throw 'TODO: implement reComboMark';
}

// lodash
/** Reduces `collection` to a value which is the accumulated result of running
 * each element in `collection` thru `reducer`, where each successive
 * invocation is supplied the return value of the previous. If `accumulator`
 * is not given, the first element of `collection` is used as the initial
 * value. `reducer` is invoked as `(acc, val, ckey)`.
 *
 * Many lodash methods are guarded to work as iteratees for methods like
 * `reduce`, `reduceRight`, and `transform`.
 *
 * The guarded methods are:
 * `assign`, `defaults`, `defaultsDeep`, `includes`, `merge`, `orderBy`,
 * and `sortBy`
 *
 * @seeAlso [reduceRight]
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param reducer {function}: `(acc, val, ckey) => acc`; defaults to `identity`.
 *   @optional
 * @param accumulator: The initial value.
 *   @optional
 *
 * @returns: the accumulated value.
 *
 * @example `reduce([1, 2], function(sum, n) { return sum + n; }, 0); // => 3`
 * @example `reduce({ 'a': 1, 'b': 2, 'c': 1 }, function(result, value, key) { (result[value] || (result[value] = [])).push(key); return result; }, {}); // => { '1': ['a', 'c'], '2': ['b'] } (iteration order is not guaranteed)`
 */
export function reduce(collection, reducer, accumulator) {
  if (false) { reduce(collection, reducer, accumulator); }
  throw 'TODO: implement reduce';
}

// flipshop
/** `bag`/`arr` reduced right-to-left through `reducer(acc, val, ckey)` -- unlike std's
 * `foldArray`, `reducer` also gets `ckey` as a third argument.
 *
 * @example `reduceRight([1, 2, 3], "", function(acc, val, _ckey) { return acc ~ val; }); // => "321"`
 */
// export function reduceRight(arr, seed, reducer) {}

// flipshop
/** /** `reduceRight`, seeded from the last-visited element -- `undefined` for an empty `arr`/`bag`. */
 */
// export function reduceRight(arr, reducer) {}

// lodash
/** Like `reduce` except that it iterates over elements of
 * `collection` from right to left.
 *
 * @seeAlso [reduce]
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param reducer {function}: `(acc, val, ckey) => acc`; defaults to `identity`.
 *   @optional
 * @param accumulator: The initial value.
 *   @optional
 *
 * @returns: the accumulated value.
 *
 * @example `var array = [[0, 1], [2, 3], [4, 5]]; reduceRight(array, function(flattened, other) { return flattened.concat(other); }, []); // => [4, 5, 2, 3, 0, 1]`
 */
export function reduceRight(collection, reducer, accumulator) {
  if (false) { reduceRight(collection, reducer, accumulator); }
  throw 'TODO: implement reduceRight';
}

// flipshop
/** Elements of `bag`/`arr` for which `rule` does *not* hold -- the inverse of `filter` *(std)*.
 * `rule` gets `(val, ckey)`; the map form returns values only.
 * `rule` is a `ruleOrKey`.
 *
 * @example `reject([1, 2, 3, 4], (val, _ckey) => val % 2 == 0); // => [1, 3]`
 */
// export function reject(arr, rule) {}

// lodash
/** The opposite of `filter`; this method returns the elements of `collection`
 * that `rule` does **not** return truthy for.
 *
 * @seeAlso [filter]
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param rule {ruleOrKey}: Coerced to a rule; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the new filtered array.
 *
 * @example `var users = [ { 'user': 'barney', 'age': 36, 'active': false }, { 'user': 'fred', 'age': 40, 'active': true } ]; reject(users, function(o) { return !o.active; }); // => objects for ['fred']`
 * @example `reject(users, { 'age': 40, 'active': true }); // => objects for ['barney']`
 * @example `reject(users, ['active', false]); // => objects for ['fred']`
 * @example `reject(users, 'active'); // => objects for ['barney']`
 */
export function reject(collection, rule) {
  if (false) { reject(collection, rule); }
  throw 'TODO: implement reject';
}

// lodash
/** Removes all elements from `array` that `rule` returns truthy for
 * and returns an array of the removed elements. `rule` is invoked as `(val, idx)`.
 *
 * **Note:** Unlike `filter`, this method mutates `array`. Use `pull`
 * to pull elements from an array by value.
 *
 * @param array {array}: The array to modify.
 * @param rule {ruleOrKey}: Coerced to a rule; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the new array of removed elements.
 *
 * @example `var array = [1, 2, 3, 4]; var evens = remove(array, function(n) { return n % 2 == 0; }); println(array); // => [1, 3]`
 * @example `println(evens); // => [2, 4]`
 */
export function remove(array, rule) {
  if (false) { remove(array, rule); }
  throw 'TODO: implement remove';
}

// lodash
/** Repeats the given string `n` times.
 *
 * @param string {string}: The string to repeat; defaults to `''`.
 *   @optional
 * @param n {number}: The number of times to repeat the string; defaults to `1`.
 *   @optional
 *
 * @returns {string}: the repeated string.
 *
 * @example `repeat('*', 3); // => '***'`
 * @example `repeat('abc', 2); // => 'abcabc'`
 * @example `repeat('abc', 0); // => ''`
 */
export function repeat(string, n) {
  if (false) { repeat(string, n); }
  throw 'TODO: implement repeat';
}

// lodash
/** Replaces matches for `pattern` in `string` with `replacement`.
 *
 * **Note:** This method is based on
 * [`String#replace`](https://mdn.io/String/replace).
 *
 * @param string {string}: The string to modify; defaults to `''`.
 *   @optional
 * @param pattern {RegExp|string}: The pattern to replace.
 * @param replacement {function|string}: The match replacement.
 *
 * @returns {string}: the modified string.
 *
 * @example `replace('Hi Fred', 'Fred', 'Barney'); // => 'Hi Barney'`
 */
export function replace(string, pattern, replacement) {
  if (false) { replace(string, pattern, replacement); }
  throw 'TODO: implement replace';
}

// lodash
/** Used to match `RegExp`
 * [syntax characters](http://ecma-international.org/ecma-262/7.0/#sec-patterns).
 */
export function reRegExpChar() {
  if (false) { reRegExpChar(); }
  throw 'TODO: implement reRegExpChar';
}

// lodash
/** Creates a function that invokes `func` with the `this` binding of the
 * created function and arguments from `start` and beyond provided as
 * an array.
 *
 * **Note:** This method is based on the
 * [rest parameter](https://mdn.io/rest_parameters).
 *
 * @param func {function}: Function to apply a rest parameter to.
 * @param start {number}: The start position of the rest parameter; defaults to `func.length-1`.
 *   @optional
 *
 * @returns {function}: the new function.
 *
 * @example `var say = rest(function(what, names) { return what + ' ' + initial(names).join(', ') + (size(names) > 1 ? ', & ' : '') + last(names); }); say('hello', 'fred', 'barney', 'pebbles'); // => 'hello fred, barney, & pebbles'`
 */
export function rest(func, start) {
  if (false) { rest(func, start); }
  throw 'TODO: implement rest';
}

// lodash
/** Like `get` except that if the resolved value is a
 * function it's invoked with the `this` binding of its parent object and
 * its result is returned.
 *
 * @param object {map}: The object to query.
 * @param path {anypath}: Path to resolve.
 * @param defaultValue: The value returned for `undefined` resolved values.
 *   @optional
 *
 * @returns: the resolved value.
 *
 * @example `var object = { 'a': [{ 'b': { 'c1': 3, 'c2': constant(4) } }] }; result(object, 'a[0].b.c1'); // => 3`
 * @example `result(object, 'a[0].b.c2'); // => 4`
 * @example `result(object, 'a[0].b.c3', 'default'); // => 'default'`
 * @example `result(object, 'a[0].b.c3', constant('default')); // => 'default'`
 */
export function result(object, path, defaultValue) {
  if (false) { result(object, path, defaultValue); }
  throw 'TODO: implement result';
}

// lodash
/** Reverses `array` so that the first element becomes the last, the second
 * element becomes the second to last, and so on.
 *
 * **Note:** This method mutates `array` and is based on
 * [`Array#reverse`](https://mdn.io/Array/reverse).
 *
 * @param array {array}: The array to modify.
 *
 * @returns {array}: `array`.
 *
 * @example `var array = [1, 2, 3]; reverse(array); // => [3, 2, 1]`
 * @example `println(array); // => [3, 2, 1]`
 */
export function reverse(array) {
  if (false) { reverse(array); }
  throw 'TODO: implement reverse';
}

// lodash
/** Computes `number` rounded to `precision`.
 *
 * @param number {number}: The number to round.
 * @param precision {number}: The precision to round to; defaults to `0`.
 *   @optional
 *
 * @returns {number}: the rounded number.
 *
 * @example `round(4.006); // => 4`
 * @example `round(4.006, 2); // => 4.01`
 * @example `round(4060, -2); // => 4100`
 */
export function round(number, precision) {
  if (false) { round(number, precision); }
  throw 'TODO: implement round';
}

// lodash
/** Create a new pristine `lodash` function using the `context` object.
 *
 * @param context {map}: The context object; defaults to `root`.
 *   @optional
 *
 * @returns {function}: a new `lodash` function.
 *
 * @example `mixin({ 'foo': constant('foo') }); var lodash = runInContext(); lodash.mixin({ 'bar': lodash.constant('bar') }); isFunction(foo); // => true`
 * @example `isFunction(bar); // => false`
 * @example `lodash.isFunction(lodash.foo); // => false`
 * @example `lodash.isFunction(lodash.bar); // => true`
 * @example `var defer = runInContext({ 'setTimeout': setImmediate }).defer;`
 */
export function runInContext(context) {
  if (false) { runInContext(context); }
  throw 'TODO: implement runInContext';
}

// flipshop
/** Whether `c1` and `c2` match channel-by-channel within `tol` (default just under `1/255`, so a
 * color that round-tripped through an 8-bit tuple still compares equal). A missing `alpha` on
 * either side is treated as `1.0`.
 */
// export function sameColor(c1, c2, tol) {}

// lodash
/** Gets a random element from `collection`.
 *
 * @param collection {array|map}: The collection to sample.
 *
 * @returns: the random element.
 *
 * @example `sample([1, 2, 3, 4]); // => 2`
 */
export function sample(collection) {
  if (false) { sample(collection); }
  throw 'TODO: implement sample';
}

// lodash
/** Gets `n` random elements at unique keys from `collection` up to the
 * size of `collection`.
 *
 * @param collection {array|map}: The collection to sample.
 * @param n {number}: The number of elements to sample; defaults to `1`.
 *   @optional
 *
 * @returns {array}: the random elements.
 *
 * @example `sampleSize([1, 2, 3], 2); // => [3, 1]`
 * @example `sampleSize([1, 2, 3], 4); // => [2, 3, 1]`
 */
export function sampleSize(collection, n) {
  if (false) { sampleSize(collection, n); }
  throw 'TODO: implement sampleSize';
}

// flipshop
/** `varname` as a legal-ish variable name: each `.` becomes `_`, and every other non-word
 * character becomes `__` -- independently, so two special characters in a row don't collapse
 * into one replacement.
 *
 * @example `sanitize_varname("foo.bar"); // => "foo_bar"`
 * @example `sanitize_varname("foo bar!"); // => "foo__bar__"`
 */
// export function sanitize_varname(varname) {}

// lodash
/** Sets the value at `path` of `object`. If a portion of `path` doesn't exist, it's created: an
 * array for a missing segment that looks like an index, a map for every other missing segment.
 * Use `setWith` to customize `path` creation.
 *
 * @param object {map}: The map to modify.
 * @param path {anypath}: Path to set.
 * @param value: The value to set.
 *
 * @returns {map}: `object`.
 *
 * @example `var object = { 'a': [{ 'b': { 'c': 3 } }] }; set(object, 'a[0].b.c', 4); println(object.a[0].b.c); // => 4`
 * @example `set(object, ['x', '0', 'y', 'z'], 5); println(object.x[0].y.z); // => 5`
 */
export function set(object, path, value) {
  if (false) { set(object, path, value); }
  throw 'TODO: implement set';
}

// flipshop
/** Sets the value at `keyname`/`keypath` of `bag`/`arr`, returning the (possibly new) container.
 * If a portion of the path doesn't exist, it's created as a map, unless the *next* segment looks
 * like a non-negative integer (a bare number, or a string of digits), in which case it's created
 * as an array instead -- matching lodash's own `set`/`baseSet` heuristic. @see `setAtWith` to
 * override that heuristic outright, the way lodash's `setWith` customizer does.
 *
 * A scalar in the way of a deeper path is replaced outright, same as lodash. Where the path's own
 * leaf is already occupied, `onCollision(existing, incoming)` decides what lands there instead of
 * just overwriting it -- default is `lastInWins`; @see `deepMerge` for the deep-merge rule.
 *
 * Two more consequences of FeatureScript having no `null`: setting a value of `undefined` deletes
 * a map key rather than storing one, and writing past the end of an array pads it with
 * `undefined` rather than throwing.
 *
 * @param bag {map|array}: Container to write into.
 * @param keyname {string}: Dotted path.
 * @param keypath {array}: Path as literal keys/indexes.
 * @param val: Value to place at the path's leaf.
 * @param onCollision {function}: `(existing, incoming) => merged`, called only when the leaf is already occupied. Defaults to `lastInWins`.
 *
 * @example `setAt({}, "a.b.c", 1); // => { "a": { "b": { "c": 1 } } }`
 * @example `setAt({}, "a.0.b", 1); // => { "a": [{ "b": 1 }] }`
 * @example `setAt([1, 2], 4, 9); // => [1, 2, undefined, undefined, 9]`
 * @example `setAt({ "a": { "x": 1 } }, "a", { "y": 2 }, ((existing, incoming) => deepMerge(existing, incoming)));`
 * @example `// => { "a": { "x": 1, "y": 2 } }`
 */
// export function setAt(bag, keyname, val) {}

// flipshop
/** `setAt`, with `segmentFor(existingChildOrUndefined, nextSegment) => newChildContainer`
 * overriding what an autovivified intermediate segment becomes, in place of `setAt`'s own
 * map-unless-the-next-segment-looks-like-an-index heuristic -- the actual hook lodash's `setWith`
 * customizer provides. `segmentFor` is consulted only where a segment doesn't already resolve to
 * a map or array; returning `undefined` falls back to `setAt`'s own default for that segment.
 *
 * @param bag {map}: Container to write into.
 * @param keynameOrPath {string|array}: Dotted path, or path as literal keys/indexes.
 * @param val: Value to place at the path's leaf.
 * @param segmentFor {function}: `(existingChildOrUndefined, nextSegment) => newChildContainer`.
 *
 * @example `setAtWith({}, "a.0.b", 1, (_existing, _nextSegment) => ({})); // => { "a": { "0": { "b": 1 } } }`
 */
// export function setAtWith(bag, keynameOrPath, val, segmentFor) {}

// flipshop
/** Sets the `APPEARANCE` property on `qq` to `cmap` -- a `Color`, or anything `toColor` accepts
 * (a hexcolor string, a `"r,g,b[,a]"` tuple string, or an `[r,g,b,a?]` array).
 */
// export function setColor(context, qq, cmap) {}

// flipshop
/** Sets the NAME property and a "Name" attribute on entities.
 *
 * @param context {Context}
 * @param entities {Query}
 * @param name {string}
 */
// export function setName(context, entities, nameText) {}

// flipshop
/** Sets both a FeatureScript property and a same-keyed attribute on entities.
 * Use attributes (not properties) to read names back during regeneration, since
 * Onshape does not allow reading properties until the full part studio has rendered.
 *
 * @param context {Context}
 * @param entities {Query}
 * @param propType {PropertyType}: e.g. PropertyType.NAME
 * @param attrName {string}: attribute key to mirror the value under
 * @param value {string}: the value to set
 */
// export function setPropAndAttribute(context, entities, propType, attrName, value) {}

// flipshop
/** `setName`, after collapsing `nameText`'s whitespace runs to single spaces and truncating to
 * `maxLength` -- for a name that might be multi-line or arbitrarily long (e.g. copied from a
 * sketch's text) but needs to read as one short line in the part tree.
 *
 * @param context {Context}
 * @param entities {Query}
 * @param nameText {string}
 * @param maxLength {number}: Defaults to `20`.
 */
// export function setReadableName(context, entities, nameText, maxLength) {}

// lodash
/** Like `set` except that it accepts `customizer` which is
 * invoked to produce the objects of `path`.  If `customizer` returns `undefined`
 * path creation is handled by the method instead. The `customizer` is invoked
 * with three arguments: (nsValue, key, nsObject).
 *
 * **Note:** This method mutates `object`.
 *
 * @param object {map}: The object to modify.
 * @param path {anypath}: Path to set.
 * @param value: The value to set.
 * @param customizer {function}: Function to customize assigned values.
 *   @optional
 *
 * @returns {map}: `object`.
 *
 * @example `var object = {}; setWith(object, '[0][1]', 'a', Object); // => { '0': { '1': 'a' } }`
 */
export function setWith(object, path, value, customizer) {
  if (false) { setWith(object, path, value, customizer); }
  throw 'TODO: implement setWith';
}

// lodash
/** Creates an array of shuffled values, using a version of the
 * [Fisher-Yates shuffle](https://en.wikipedia.org/wiki/Fisher-Yates_shuffle).
 *
 * @param collection {array|map}: The collection to shuffle.
 *
 * @returns {array}: the new shuffled array.
 *
 * @example `shuffle([1, 2, 3, 4]); // => [4, 1, 3, 2]`
 */
export function shuffle(collection) {
  if (false) { shuffle(collection); }
  throw 'TODO: implement shuffle';
}

// lodash
/** Gets the size of `collection`: its length for an array or string, or its key count for a map.
 *
 * @param collection {array|map|string}: The collection to inspect.
 *
 * @returns {number}: the collection size.
 *
 * @example `size([1, 2, 3]); // => 3`
 * @example `size({ 'a': 1, 'b': 2 }); // => 2`
 * @example `size('pebbles'); // => 7`
 */
export function size(collection) {
  if (false) { size(collection); }
  throw 'TODO: implement size';
}

// flipshop
/** Size of `val`: length for a string, element count for an array, key count for a map, `0` for
 * `undefined`.
 *
 * @example `sizeof([0, 1, 2]); // => 3`
 * @example `sizeof({ "a": 1 }); // => 1`
 * @example `sizeof("12345"); // => 5`
 * @example `sizeof(undefined); // => 0`
 */
// export function sizeof(val) {}

// flipshop
/** /** Keeps `varname` at `objname ~ "_size"` for as long as it hasn't been hand-edited; @see `defaultMaybe`. */
 */
// export function sizeofEditLogic(context, id, oldDefinition, newDefinition, isCreating, specifiedParameters) {}

// lodash
/** Creates a slice of `array` from `start` up to, but not including, `end`.
 *
 * **Note:** This method is used instead of
 * [`Array#slice`](https://mdn.io/Array/slice) to ensure dense arrays are
 * returned.
 *
 * @param array {array}: The array to slice.
 * @param start {number}: The start position; defaults to `0`.
 *   @optional
 * @param end {number}: The end position; defaults to `array.length`.
 *   @optional
 *
 * @returns {array}: the slice of `array`.
 */
export function slice(array, start, end) {
  if (false) { slice(array, start, end); }
  throw 'TODO: implement slice';
}

// flipshop
/** /** `str` split into words, lowercased, and joined with `_`. */
 */
// export function snakeCase(str) {}

// lodash
/** Converts `string` to
 * [snake case](https://en.wikipedia.org/wiki/Snake_case).
 *
 * @param string {string}: The string to convert; defaults to `''`.
 *   @optional
 *
 * @returns {string}: the snake cased string.
 *
 * @example `snakeCase('Foo Bar'); // => 'foo_bar'`
 * @example `snakeCase('fooBar'); // => 'foo_bar'`
 * @example `snakeCase('--FOO-BAR--'); // => 'foo_bar'`
 */
export function snakeCase(string) {
  if (false) { snakeCase(string); }
  throw 'TODO: implement snakeCase';
}

// lodash
/** Checks if `rule` returns truthy for **any** element of `collection`.
 * Iteration is stopped once `rule` returns truthy. `rule` is invoked as `(val, ckey)`.
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param rule {ruleOrKey}: Coerced to a rule; defaults to `identity`.
 *   @optional
 *
 * @returns {boolean}: `true` if any element passes the rule check, else `false`.
 *
 * @example `some([null, 0, 'yes', false], Boolean); // => true`
 * @example `var users = [ { 'user': 'barney', 'active': true }, { 'user': 'fred', 'active': false } ]; some(users, { 'user': 'barney', 'active': false }); // => false`
 * @example `some(users, ['active', false]); // => true`
 * @example `some(users, 'active'); // => true`
 */
export function some(collection, rule) {
  if (false) { some(collection, rule); }
  throw 'TODO: implement some';
}

// lodash
/** Uses a binary search to determine the lowest index at which `value`
 * should be inserted into `array` in order to maintain its sort order.
 *
 * @param array {array}: The sorted array to inspect.
 * @param value: The value to evaluate.
 *
 * @returns {number}: the index at which `value` should be inserted into `array`.
 *
 * @example `sortedIndex([30, 50], 40); // => 1`
 */
export function sortedIndex(array, value) {
  if (false) { sortedIndex(array, value); }
  throw 'TODO: implement sortedIndex';
}

// lodash
/** Like `sortedIndex` except that it accepts `funcOrKey`
 * which is invoked for `value` and each element of `array` to compute their
 * sort ranking. `funcOrKey` is invoked as `(val, idx)`.
 *
 * @param array {array}: The sorted array to inspect.
 * @param value: The value to evaluate.
 * @param funcOrKey {function}: `(val) => criteria`; defaults to `identity`.
 *   @optional
 *
 * @returns {number}: the index at which `value` should be inserted into `array`.
 *
 * @example `var objects = [{ 'x': 4 }, { 'x': 5 }]; sortedIndexBy(objects, { 'x': 4 }, function(o) { return o.x; }); // => 0`
 * @example `sortedIndexBy(objects, { 'x': 4 }, 'x'); // => 0`
 */
export function sortedIndexBy(array, value, funcOrKey) {
  if (false) { sortedIndexBy(array, value, funcOrKey); }
  throw 'TODO: implement sortedIndexBy';
}

// lodash
/** Like `indexOf` except that it performs a binary
 * search on a sorted `array`.
 *
 * @param array {array}: The array to inspect.
 * @param value: The value to search for.
 *
 * @returns {number}: the index of the matched value, else `-1`.
 *
 * @example `sortedIndexOf([4, 5, 5, 5, 6], 5); // => 1`
 */
export function sortedIndexOf(array, value) {
  if (false) { sortedIndexOf(array, value); }
  throw 'TODO: implement sortedIndexOf';
}

// lodash
/** Like `sortedIndex` except that it returns the highest
 * index at which `value` should be inserted into `array` in order to
 * maintain its sort order.
 *
 * @param array {array}: The sorted array to inspect.
 * @param value: The value to evaluate.
 *
 * @returns {number}: the index at which `value` should be inserted into `array`.
 *
 * @example `sortedLastIndex([4, 5, 5, 5, 6], 5); // => 4`
 */
export function sortedLastIndex(array, value) {
  if (false) { sortedLastIndex(array, value); }
  throw 'TODO: implement sortedLastIndex';
}

// lodash
/** Like `sortedLastIndex` except that it accepts `funcOrKey`
 * which is invoked for `value` and each element of `array` to compute their
 * sort ranking. `funcOrKey` is invoked as `(val, idx)`.
 *
 * @param array {array}: The sorted array to inspect.
 * @param value: The value to evaluate.
 * @param funcOrKey {function}: `(val) => criteria`; defaults to `identity`.
 *   @optional
 *
 * @returns {number}: the index at which `value` should be inserted into `array`.
 *
 * @example `var objects = [{ 'x': 4 }, { 'x': 5 }]; sortedLastIndexBy(objects, { 'x': 4 }, function(o) { return o.x; }); // => 1`
 * @example `sortedLastIndexBy(objects, { 'x': 4 }, 'x'); // => 1`
 */
export function sortedLastIndexBy(array, value, funcOrKey) {
  if (false) { sortedLastIndexBy(array, value, funcOrKey); }
  throw 'TODO: implement sortedLastIndexBy';
}

// lodash
/** Like `lastIndexOf` except that it performs a binary
 * search on a sorted `array`.
 *
 * @param array {array}: The array to inspect.
 * @param value: The value to search for.
 *
 * @returns {number}: the index of the matched value, else `-1`.
 *
 * @example `sortedLastIndexOf([4, 5, 5, 5, 6], 5); // => 3`
 */
export function sortedLastIndexOf(array, value) {
  if (false) { sortedLastIndexOf(array, value); }
  throw 'TODO: implement sortedLastIndexOf';
}

// lodash
/** Like `uniq` except that it's designed and optimized
 * for sorted arrays.
 *
 * @param array {array}: The array to inspect.
 *
 * @returns {array}: the new duplicate free array.
 *
 * @example `sortedUniq([1, 1, 2]); // => [1, 2]`
 */
export function sortedUniq(array) {
  if (false) { sortedUniq(array); }
  throw 'TODO: implement sortedUniq';
}

// lodash
/** Like `uniqBy` except that it's designed and optimized
 * for sorted arrays.
 *
 * @param array {array}: The array to inspect.
 * @param funcOrKey {function}: `(val) => criteria`.
 *   @optional
 *
 * @returns {array}: the new duplicate free array.
 *
 * @example `sortedUniqBy([1.1, 1.2, 2.3, 2.4], Math.floor); // => [1.1, 2.3]`
 */
export function sortedUniqBy(array, funcOrKey) {
  if (false) { sortedUniqBy(array, funcOrKey); }
  throw 'TODO: implement sortedUniqBy';
}

// lodash
/** Splits `string` by `separator`.
 *
 * **Note:** This method is based on
 * [`String#split`](https://mdn.io/String/split).
 *
 * @param string {string}: The string to split; defaults to `''`.
 *   @optional
 * @param separator {RegExp|string}: The separator pattern to split by.
 * @param limit {number}: The length to truncate results to.
 *   @optional
 *
 * @returns {array}: the string segments.
 *
 * @example `split('a-b-c', '-', 2); // => ['a', 'b']`
 */
export function split(string, separator, limit) {
  if (false) { split(string, separator, limit); }
  throw 'TODO: implement split';
}

// lodash
/** Creates a function that invokes `func` with the `this` binding of the
 * create function and an array of arguments much like
 * [`Function#apply`](http://www.ecma-international.org/ecma-262/7.0/#sec-function.prototype.apply).
 *
 * **Note:** This method is based on the
 * [spread operator](https://mdn.io/spread_operator).
 *
 * @param func {function}: Function to spread arguments over.
 * @param start {number}: The start position of the spread; defaults to `0`.
 *   @optional
 *
 * @returns {function}: the new function.
 *
 * @example `var say = spread(function(who, what) { return who + ' says ' + what; }); say(['fred', 'hello']); // => 'fred says hello'`
 * @example `var numbers = Promise.all([ Promise.resolve(40), Promise.resolve(36) ]); numbers.then(spread(function(x, y) { return x + y; })); // => a Promise of 76`
 */
export function spread(func, start) {
  if (false) { spread(func, start); }
  throw 'TODO: implement spread';
}

// flipshop
/** Wraps `str` in a border of `*` characters matching its own length, for a `debug()` call that
 * wants to stand out.
 *
 * @example `starbanner("hi"); // => "\n**\nhi\n**\n\n"`
 */
// export function starbanner(str) {}

// lodash
/** Converts `string` to
 * [start case](https://en.wikipedia.org/wiki/Letter_case#Stylistic_or_specialised_usage).
 *
 * @param string {string}: The string to convert; defaults to `''`.
 *   @optional
 *
 * @returns {string}: the start cased string.
 *
 * @example `startCase('--foo-bar--'); // => 'Foo Bar'`
 * @example `startCase('fooBar'); // => 'Foo Bar'`
 * @example `startCase('__FOO_BAR__'); // => 'FOO BAR'`
 */
export function startCase(string) {
  if (false) { startCase(string); }
  throw 'TODO: implement startCase';
}

// lodash
/** Checks if `string` starts with the given target string.
 *
 * @param string {string}: The string to inspect; defaults to `''`.
 *   @optional
 * @param target {string}: The string to search for.
 *   @optional
 * @param position {number}: The position to search from; defaults to `0`.
 *   @optional
 *
 * @returns {boolean}: `true` if `string` starts with `target`, else `false`.
 *
 * @example `startsWith('abc', 'a'); // => true`
 * @example `startsWith('abc', 'b'); // => false`
 * @example `startsWith('abc', 'b', 1); // => true`
 */
export function startsWith(string, target, position) {
  if (false) { startsWith(string, target, position); }
  throw 'TODO: implement startsWith';
}

// flipshop
/** Repeats `str` `reps` times.
 *
 * @example `strRepeat("*", 3); // => "***"`
 * @example `strRepeat("abc", 2); // => "abcabc"`
 * @example `strRepeat("abc", 0); // => ""`
 */
// export function strRepeat(str, reps) {}

// flipshop
/** Text of `str` from `begseq` up to, but not including, `endseq` -- the semantics of JS's
 * `String.prototype.slice`.
 *
 * A negative `begseq`/`endseq` counts back from the end of the string (`max(length(str) +
 * begseq, 0)`); an omitted or too-large `endseq` extracts to the end of the string; if, after
 * normalizing, `endseq <= begseq`, the result is `""`.
 *
 * `SequencePosition.END` is an explicit stand-in for "to the end of the string" at a call site
 * that must supply all three arguments (e.g. inside a fixed-arity callback), since FeatureScript
 * has no omitted-argument default.
 *
 * @example `strSlice("hello world", 0, 3); // => "hel"`
 * @example `strSlice("hello world", -5); // => "world"`
 * @example `strSlice("hello world", 0, -1); // => "hello worl"`
 * @example `strSlice("hello world", -3, -1); // => "rl"`
 * @example `strSlice("hello world", 100, 200); // => ""`
 */
// export function strSlice(str, begseq, endseq) {}

// flipshop
/** First `len` characters of `str`; `""` if `len <= 0`.
 *
 * @example `strTake("hello world", 2); // => "he"`
 */
// export function strTake(str, len) {}

// flipshop
/** Last `len` characters of `str`; `""` if `len <= 0`.
 *
 * @example `strTakeRight("hello world", 2); // => "ld"`
 */
// export function strTakeRight(str, len) {}

// lodash
/** This method returns a new empty array.
 *
 * @returns {array}: the new empty array.
 *
 * @example `var arrays = times(2, stubArray); println(arrays); // => [[], []]`
 * @example `println(arrays[0] === arrays[1]); // => false`
 */
export function stubArray() {
  if (false) { stubArray(); }
  throw 'TODO: implement stubArray';
}

// lodash
/** This method returns `false`.
 *
 * @returns {boolean}: `false`.
 *
 * @example `times(2, stubFalse); // => [false, false]`
 */
export function stubFalse() {
  if (false) { stubFalse(); }
  throw 'TODO: implement stubFalse';
}

// lodash
/** This method returns a new empty object.
 *
 * @returns {map}: the new empty object.
 *
 * @example `var objects = times(2, stubObject); println(objects); // => [{}, {}]`
 * @example `println(objects[0] === objects[1]); // => false`
 */
export function stubObject() {
  if (false) { stubObject(); }
  throw 'TODO: implement stubObject';
}

// lodash
/** This method returns an empty string.
 *
 * @returns {string}: the empty string.
 *
 * @example `times(2, stubString); // => ['', '']`
 */
export function stubString() {
  if (false) { stubString(); }
  throw 'TODO: implement stubString';
}

// lodash
/** This method returns `true`.
 *
 * @returns {boolean}: `true`.
 *
 * @example `times(2, stubTrue); // => [true, true]`
 */
export function stubTrue() {
  if (false) { stubTrue(); }
  throw 'TODO: implement stubTrue';
}

// lodash
/** Subtract two numbers.
 *
 * @param minuend {number}: The first number in a subtraction.
 * @param subtrahend {number}: The second number in a subtraction.
 *
 * @returns {number}: the difference.
 *
 * @example `subtract(6, 4); // => 2`
 */
export function subtract(minuend, subtrahend) {
  if (false) { subtract(minuend, subtrahend); }
  throw 'TODO: implement subtract';
}

// lodash
/** Computes the sum of the values in `array`.
 *
 * @param array {array}: The array to iterate over.
 *
 * @returns {number}: the sum.
 *
 * @example `sum([4, 2, 8, 6]); // => 20`
 */
export function sum(array) {
  if (false) { sum(array); }
  throw 'TODO: implement sum';
}

// flipshop
/** Sum of `funcOrKey(val, idx)` across `arr` -- `sum` *(std)*, mapped via `mapValues`.
 * `funcOrKey` is coerced through `iteratee` @see `iteratee`.
 *
 * @example `sumBy([{ "n": 2 }, { "n": 4 }], (val) => val.n); // => 6`
 */
// export function sumBy(arr, funcOrKey) {}

// lodash
/** Like `sum` except that it accepts `funcOrKey` which is
 * invoked for each element in `array` to generate the value to be summed.
 * `funcOrKey` is invoked as `(val, idx)`.
 *
 * @param array {array}: The array to iterate over.
 * @param funcOrKey {function}: `(val) => criteria`; defaults to `identity`.
 *   @optional
 *
 * @returns {number}: the sum.
 *
 * @example `var objects = [{ 'n': 4 }, { 'n': 2 }, { 'n': 8 }, { 'n': 6 }]; sumBy(objects, function(o) { return o.n; }); // => 20`
 * @example `sumBy(objects, 'n'); // => 20`
 */
export function sumBy(array, funcOrKey) {
  if (false) { sumBy(array, funcOrKey); }
  throw 'TODO: implement sumBy';
}

// flipshop
/** `arr` without its first element; `[]` for an empty or single-element `arr`.
 *
 * @example `tail([1, 2, 3]); // => [2, 3]`
 */
// export function tail(arr) {}

// lodash
/** Gets all but the first element of `array`.
 *
 * @param array {array}: The array to query.
 *
 * @returns {array}: the slice of `array`.
 *
 * @example `tail([1, 2, 3]); // => [2, 3]`
 */
export function tail(array) {
  if (false) { tail(array); }
  throw 'TODO: implement tail';
}

// flipshop
/** First `takeCount` elements of `arr`; `takeCount <= 0` returns `[]`. With no `takeCount` given,
 * defaults to `1`, matching `drop`'s own default.
 *
 * @example `take([1, 2, 3], 2); // => [1, 2]`
 * @example `take([1, 2, 3]); // => [1]`
 */
// export function take(arr, takeCount) {}

// lodash
/** Creates a slice of `array` with `n` elements taken from the beginning.
 *
 * @param array {array}: The array to query.
 * @param n {number}: The number of elements to take; defaults to `1`.
 *   @optional
 *
 * @returns {array}: the slice of `array`.
 *
 * @example `take([1, 2, 3]); // => [1]`
 * @example `take([1, 2, 3], 2); // => [1, 2]`
 * @example `take([1, 2, 3], 5); // => [1, 2, 3]`
 * @example `take([1, 2, 3], 0); // => []`
 */
export function take(array, n) {
  if (false) { take(array, n); }
  throw 'TODO: implement take';
}

// flipshop
/** Last `takeCount` elements of `arr`; `takeCount <= 0` returns `[]`. With no `takeCount` given,
 * defaults to `1`, matching `dropRight`'s own default.
 *
 * @example `takeRight([1, 2, 3], 2); // => [2, 3]`
 * @example `takeRight([1, 2, 3]); // => [3]`
 */
// export function takeRight(arr, takeCount) {}

// lodash
/** Creates a slice of `array` with `n` elements taken from the end.
 *
 * @param array {array}: The array to query.
 * @param n {number}: The number of elements to take; defaults to `1`.
 *   @optional
 *
 * @returns {array}: the slice of `array`.
 *
 * @example `takeRight([1, 2, 3]); // => [3]`
 * @example `takeRight([1, 2, 3], 2); // => [2, 3]`
 * @example `takeRight([1, 2, 3], 5); // => [1, 2, 3]`
 * @example `takeRight([1, 2, 3], 0); // => []`
 */
export function takeRight(array, n) {
  if (false) { takeRight(array, n); }
  throw 'TODO: implement takeRight';
}

// flipshop
/** Elements taken from the end of `arr` for as long as `rule` holds. With no `rule` given,
 * defaults to `truthy`, matching `dropRightWhile`'s own default.
 *
 * @example `takeRightWhile([1, 2, 3, 4], (val) => val > 2); // => [3, 4]`
 */
// export function takeRightWhile(arr, rule) {}

// lodash
/** Creates a slice of `array` with elements taken from the end. Elements are
 * taken until `rule` returns falsey. `rule` is invoked as `(val, idx)`.
 *
 * @param array {array}: The array to query.
 * @param rule {ruleOrKey}: Coerced to a rule; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the slice of `array`.
 *
 * @example `var users = [ { 'user': 'barney', 'active': true }, { 'user': 'fred', 'active': false }, { 'user': 'pebbles', 'active': false } ]; takeRightWhile(users, function(o) { return !o.active; }); // => objects for ['fred', 'pebbles']`
 * @example `takeRightWhile(users, { 'user': 'pebbles', 'active': false }); // => objects for ['pebbles']`
 * @example `takeRightWhile(users, ['active', false]); // => objects for ['fred', 'pebbles']`
 * @example `takeRightWhile(users, 'active'); // => []`
 */
export function takeRightWhile(array, rule) {
  if (false) { takeRightWhile(array, rule); }
  throw 'TODO: implement takeRightWhile';
}

// flipshop
/** Elements taken from the beginning of `arr` for as long as `rule` holds. With no `rule` given,
 * defaults to `truthy`, matching `dropWhile`'s own default.
 *
 * @example `takeWhile([1, 2, 3, 4], (val) => val < 3); // => [1, 2]`
 */
// export function takeWhile(arr, rule) {}

// lodash
/** Creates a slice of `array` with elements taken from the beginning. Elements
 * are taken until `rule` returns falsey. `rule` is invoked as `(val, idx)`.
 *
 * @param array {array}: The array to query.
 * @param rule {ruleOrKey}: Coerced to a rule; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the slice of `array`.
 *
 * @example `var users = [ { 'user': 'barney', 'active': false }, { 'user': 'fred', 'active': false }, { 'user': 'pebbles', 'active': true } ]; takeWhile(users, function(o) { return !o.active; }); // => objects for ['barney', 'fred']`
 * @example `takeWhile(users, { 'user': 'barney', 'active': false }); // => objects for ['barney']`
 * @example `takeWhile(users, ['active', false]); // => objects for ['barney', 'fred']`
 * @example `takeWhile(users, 'active'); // => []`
 */
export function takeWhile(array, rule) {
  if (false) { takeWhile(array, rule); }
  throw 'TODO: implement takeWhile';
}

// flipshop
/** Invokes `func(iter)` `count` times (i.e. for `iter` = 0 to `count` - 1), returning an array
 * of the results of each invocation.
 *
 * @param count {number}: The number of times to invoke `func`.
 * @param func {function}: `(iter) => val`; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the array of results.
 *
 * @example `times(3, String); // => ['0', '1', '2']`
 * @example `times(3, (iter) => iter * iter); // => [0, 1, 4]`
 * @example `times(4, constant(0)); // => [0, 0, 0, 0]`
 */

// flipshop
/** /** `titleCase` with default options -- @see the two-argument overload. */
 */
// export function titleCase(str) {}

// flipshop
/** Converts `str` to start case, in the spirit of lodash's `startCase`: splits into words,
 * capitalizes each word's first letter, and lower-cases the rest. Unlike `startCase`, word
 * breaks come from a configurable set of delimiter characters rather than Unicode word-boundary
 * detection, and a single-character translation can run ahead of capitalization.
 *
 * @param opts {map}: keyword options - @field [spaces="-_"] {string}: Characters that denote a word break. - @field [tr={}] {map}: Single-character to single-character translation, applied before capitalization.
 *
 * @example `titleCase("socket_kind"); // => "Socket Kind"`
 * @example `titleCase("hello.world", { "spaces": "." }); // => "Hello World"`
 * @example `titleCase("a_b_c", { "tr": { "a": "X", "b": "Y" } }); // => "X Y C"`
 */
// export function titleCase(str, opts) {}

// lodash
/** Converts `value` to an array.
 *
 * @param value: The value to convert.
 *
 * @returns {array}: the converted array.
 *
 * @example `toArray({ 'a': 1, 'b': 2 }); // => [1, 2]`
 * @example `toArray('abc'); // => ['a', 'b', 'c']`
 * @example `toArray(1); // => []`
 * @example `toArray(null); // => []`
 */
export function toArray(value) {
  if (false) { toArray(value); }
  throw 'TODO: implement toArray';
}

// flipshop
/** Best-effort `Color` from whatever's given:
 * - a `Color` is passed through unchanged.
 * - a 3- or 4-number array is passed straight to `color(...)` -- values are assumed already
 *   0.0–1.0; there's no 0–255 detection here, unlike the string form below.
 * - a string is sniffed against `hexcolorToColor`/`unitcolorToColor`/`tuplestrToColor`, in that
 *   order, first match wins. An all-`0`/`1` integer tuple like `"1,0,0"` is genuinely ambiguous
 *   between the two string scales and reads as 0.0–1.0 (pure red), not 0–255 (near-black).
 *
 * `OopsColor` (bright red) comes back if nothing matches, or if parsing throws.
 */
// export function toColor(cmap) {}

// lodash
/** Converts `value` to a finite number.
 *
 * @param value: The value to convert.
 *
 * @returns {number}: the converted number.
 *
 * @example `toFinite(3.2); // => 3.2`
 * @example `toFinite(Number.MIN_VALUE); // => 5e-324`
 * @example `toFinite(Infinity); // => 1.7976931348623157e+308`
 * @example `toFinite('3.2'); // => 3.2`
 */
export function toFinite(value) {
  if (false) { toFinite(value); }
  throw 'TODO: implement toFinite';
}

// flipshop
/** /** `color` as `"#rrggbbaa"`. */
 */
// export function toHexcolor(color) {}

// lodash
/** Converts `value` to an integer.
 *
 * **Note:** This method is loosely based on
 * [`ToInteger`](http://www.ecma-international.org/ecma-262/7.0/#sec-tointeger).
 *
 * @param value: The value to convert.
 *
 * @returns {number}: the converted integer.
 *
 * @example `toInteger(3.2); // => 3`
 * @example `toInteger(Number.MIN_VALUE); // => 0`
 * @example `toInteger(Infinity); // => 1.7976931348623157e+308`
 * @example `toInteger('3.2'); // => 3`
 */
export function toInteger(value) {
  if (false) { toInteger(value); }
  throw 'TODO: implement toInteger';
}

// lodash
/** Converts `value` to an integer suitable for use as the length of an
 * array-like object.
 *
 * **Note:** This method is based on
 * [`ToLength`](http://ecma-international.org/ecma-262/7.0/#sec-tolength).
 *
 * @param value: The value to convert.
 *
 * @returns {number}: the converted integer.
 *
 * @example `toLength(3.2); // => 3`
 * @example `toLength(Number.MIN_VALUE); // => 0`
 * @example `toLength(Infinity); // => 4294967295`
 * @example `toLength('3.2'); // => 3`
 */
export function toLength(value) {
  if (false) { toLength(value); }
  throw 'TODO: implement toLength';
}

// lodash
/** Converts `string`, as a whole, to lower case just like
 * [String#toLowerCase](https://mdn.io/toLowerCase).
 *
 * @param string {string}: The string to convert; defaults to `''`.
 *   @optional
 *
 * @returns {string}: the lower cased string.
 *
 * @example `toLower('--Foo-Bar--'); // => '--foo-bar--'`
 * @example `toLower('fooBar'); // => 'foobar'`
 * @example `toLower('__FOO_BAR__'); // => '__foo_bar__'`
 */
export function toLower(string) {
  if (false) { toLower(string); }
  throw 'TODO: implement toLower';
}

// lodash
/** Converts `value` to a number.
 *
 * @param value: The value to process.
 *
 * @returns {number}: the number.
 *
 * @example `toNumber(3.2); // => 3.2`
 * @example `toNumber(Number.MIN_VALUE); // => 5e-324`
 * @example `toNumber(Infinity); // => Infinity`
 * @example `toNumber('3.2'); // => 3.2`
 */
export function toNumber(value) {
  if (false) { toNumber(value); }
  throw 'TODO: implement toNumber';
}

// flipshop
/** `bag` flattened into `[[key, val], ...]` pairs, in `keys(bag)` order -- the inverse of
 * `fromPairs` *(arrayUtils)*. FeatureScript maps have no own/inherited distinction, so this
 * covers lodash's `entries`, `entriesIn`, and `toPairsIn` as well as `toPairs`.
 *
 * @example `toPairs({ "a": 1, "b": 2 }); // => [["a", 1], ["b", 2]]`
 */
// export function toPairs(bag) {}

// lodash
/** Same as `toPairs` -- FeatureScript maps have no own/inherited distinction, so there's no
 * extra reach for this to have over `toPairs`.
 *
 * @param object {map}: The map to query.
 *
 * @returns {array}: the key-value pairs.
 *
 * @example `toPairs({ 'a': 1, 'b': 2 }); // => [['a', 1], ['b', 2]] (iteration order is not guaranteed)`
 */
export function toPairs(object) {
  if (false) { toPairs(object); }
  throw 'TODO: implement toPairs';
}

// lodash
/** Same as `toPairs` -- FeatureScript maps have no own/inherited distinction, so there's no
 * extra reach for this to have over `toPairs`.
 *
 * @param object {map}: The map to query.
 *
 * @returns {array}: the key-value pairs.
 *
 * @example `toPairsIn({ 'a': 1, 'b': 2 }); // => [['a', 1], ['b', 2]] (iteration order is not guaranteed)`
 */
export function toPairsIn(object) {
  if (false) { toPairsIn(object); }
  throw 'TODO: implement toPairsIn';
}

// lodash
/** Converts `value` to a `keypath` array.
 *
 * @param value: The value to convert.
 *
 * @returns {array}: the new keypath array.
 *
 * @example `toPath('a.b.c'); // => ['a', 'b', 'c']`
 */
export function toPath(value) {
  if (false) { toPath(value); }
  throw 'TODO: implement toPath';
}

// lodash
/** Same as `identity` -- FeatureScript maps have no prototype chain to flatten, so there's
 * nothing for this to have over `value` itself.
 *
 * @param value: The value to convert.
 *
 * @returns {map}: `value`.
 */
export function toPlainObject(value) {
  if (false) { toPlainObject(value); }
  throw 'TODO: implement toPlainObject';
}

// lodash
/** Converts `value` to a safe integer. A safe integer can be compared and
 * represented correctly.
 *
 * @param value: The value to convert.
 *
 * @returns {number}: the converted integer.
 *
 * @example `toSafeInteger(3.2); // => 3`
 * @example `toSafeInteger(Number.MIN_VALUE); // => 0`
 * @example `toSafeInteger(Infinity); // => 9007199254740991`
 * @example `toSafeInteger('3.2'); // => 3`
 */
export function toSafeInteger(value) {
  if (false) { toSafeInteger(value); }
  throw 'TODO: implement toSafeInteger';
}

// lodash
/** Converts `value` to a string. An empty string is returned for `null`
 * and `undefined` values. The sign of `-0` is preserved.
 *
 * @param value: The value to convert.
 *
 * @returns {string}: the converted string.
 *
 * @example `toString(null); // => ''`
 * @example `toString(-0); // => '-0'`
 * @example `toString([1, 2, 3]); // => '1,2,3'`
 */
export function toString(value) {
  if (false) { toString(value); }
  throw 'TODO: implement toString';
}

// flipshop
/** /** `color` as `[red, green, blue, alpha]`, each rounded to a 0–255 integer; `alpha` defaults to `1.0` (255) if unset. */
 */
// export function toTuplecolor(color) {}

// flipshop
/** /** `color` as `[red, green, blue, alpha]`, each 0.0–1.0; `alpha` defaults to `1.0` if unset. */
 */
// export function toUnitcolor(color) {}

// lodash
/** Converts `string`, as a whole, to upper case just like
 * [String#toUpperCase](https://mdn.io/toUpperCase).
 *
 * @param string {string}: The string to convert; defaults to `''`.
 *   @optional
 *
 * @returns {string}: the upper cased string.
 *
 * @example `toUpper('--foo-bar--'); // => '--FOO-BAR--'`
 * @example `toUpper('fooBar'); // => 'FOOBAR'`
 * @example `toUpper('__foo_bar__'); // => '__FOO_BAR__'`
 */
export function toUpper(string) {
  if (false) { toUpper(string); }
  throw 'TODO: implement toUpper';
}

// lodash
/** An alternative to `reduce`; this method transforms `object` into a new `accumulator` map,
 * which is the result of running each of `object`'s keys thru `reducer`, invoked as
 * `(acc, val, ckey)`. If `accumulator` is not provided, an empty map is used. `reducer` may
 * exit iteration early by explicitly returning `false`.
 *
 * @param object {map}: The object to iterate over.
 * @param reducer {function}: `(acc, val, ckey) => acc`; defaults to `identity`.
 *   @optional
 * @param accumulator: The custom accumulator value.
 *   @optional
 *
 * @returns: the accumulated value.
 *
 * @example `transform([2, 3, 4], function(result, n) { result.push(n *= n); return n % 2 == 0; }, []); // => [4, 9]`
 * @example `transform({ 'a': 1, 'b': 2, 'c': 1 }, function(result, value, key) { (result[value] || (result[value] = [])).push(key); }, {}); // => { '1': ['a', 'c'], '2': ['b'] }`
 */
export function transform(object, reducer, accumulator) {
  if (false) { transform(object, reducer, accumulator); }
  throw 'TODO: implement transform';
}

// flipshop
/** /** `trimStart` and `trimEnd` together: strips from both ends. */
 */
// export function trim(str, chars) {}

// lodash
/** Removes leading and trailing whitespace or specified characters from `string`.
 *
 * @param string {string}: The string to trim; defaults to `''`.
 *   @optional
 * @param chars {string}: The characters to trim; defaults to `whitespace`.
 *   @optional
 *
 * @returns {string}: the trimmed string.
 *
 * @example `trim(' abc '); // => 'abc'`
 * @example `trim('-_-abc-_-', '_-'); // => 'abc'`
 * @example `map([' foo ', ' bar '], trim); // => ['foo', 'bar']`
 */
export function trim(string, chars) {
  if (false) { trim(string, chars); }
  throw 'TODO: implement trim';
}

// flipshop
/** /** `trimStart`'s counterpart: strips from the back instead of the front. */
 */
// export function trimEnd(str, chars) {}

// lodash
/** Removes trailing whitespace or specified characters from `string`.
 *
 * @param string {string}: The string to trim; defaults to `''`.
 *   @optional
 * @param chars {string}: The characters to trim; defaults to `whitespace`.
 *   @optional
 *
 * @returns {string}: the trimmed string.
 *
 * @example `trimEnd(' abc '); // => ' abc'`
 * @example `trimEnd('-_-abc-_-', '_-'); // => '-_-abc'`
 */
export function trimEnd(string, chars) {
  if (false) { trimEnd(string, chars); }
  throw 'TODO: implement trimEnd';
}

// flipshop
/** `str` with any character in `chars` (default whitespace) removed from the front.
 *
 * @example `trimStart(" hi "); // => "hi "`
 */
// export function trimStart(str, chars) {}

// lodash
/** Removes leading whitespace or specified characters from `string`.
 *
 * @param string {string}: The string to trim; defaults to `''`.
 *   @optional
 * @param chars {string}: The characters to trim; defaults to `whitespace`.
 *   @optional
 *
 * @returns {string}: the trimmed string.
 *
 * @example `trimStart(' abc '); // => 'abc '`
 * @example `trimStart('-_-abc-_-', '_-'); // => 'abc-_-'`
 */
export function trimStart(string, chars) {
  if (false) { trimStart(string, chars); }
  throw 'TODO: implement trimStart';
}

// flipshop
/** `str` shortened to at most `opts.length` characters (the omission marker included), replacing
 * whatever got cut with `opts.omission`. Unlike lodash, there's no `separator` option to break at
 * a word/regex boundary instead of an exact character count.
 *
 * @param opts {map}: keyword options - @field [length=30] {number}: Maximum result length, omission marker included. - @field [omission="..."] {string}: Marker appended when `str` is cut.
 *
 * @example `truncate("hello world", { "length": 8 }); // => "hello..."`
 */
// export function truncate(str, opts) {}

// lodash
/** Truncates `string` if it's longer than the given maximum string length.
 * The last characters of the truncated string are replaced with the omission
 * string which defaults to "...".
 *
 * @param string {string}: The string to truncate; defaults to `''`.
 *   @optional
 * @param options {{
 *    @field length {number}: The maximum string length; defaults to `30`.
 *     @optional
 *    @field omission {string}: The string to indicate text is omitted; defaults to `'...'`.
 *     @optional
 *    @field separator {RegExp|string}: The separator pattern to truncate to.
 *     @optional
 * }}
 *
 * @returns {string}: the truncated string.
 *
 * @example `truncate('hi-diddly-ho there, neighborino'); // => 'hi-diddly-ho there, neighbo...'`
 * @example `truncate('hi-diddly-ho there, neighborino', { 'length': 24, 'separator': ' ' }); // => 'hi-diddly-ho there,...'`
 * @example `truncate('hi-diddly-ho there, neighborino', { 'length': 24, 'separator': /,? +/ }); // => 'hi-diddly-ho there...'`
 * @example `truncate('hi-diddly-ho there, neighborino', { 'omission': ' [...]' }); // => 'hi-diddly-ho there, neig [...]'`
 */
export function truncate(string, options) {
  if (false) { truncate(string, options); }
  throw 'TODO: implement truncate';
}

// flipshop
/** Parses a 0–255 RGB(A) tuple string (`"200,99,100,33"` or `"[0, 1, 255]"` -- brackets and spaces
 * optional, values must be plain integers) into a `Color`. Missing alpha defaults to fully
 * opaque. Returns `OopsColor` (bright red) on anything that doesn't match, rather than throwing.
 *
 * @param tuplestr {string}
 *
 * @returns {{
 *    @field red {number}
 *    @field green {number}
 *    @field blue {number}
 *    @field alpha {number}
 * }}
 */
// export function tuplestrToColor(tuplestr) {}

// lodash
/** Creates a function that accepts up to one argument, ignoring any
 * additional arguments.
 *
 * @param func {function}: Function to cap arguments for.
 *
 * @returns {function}: the new capped function.
 *
 * @example `map(['6', '8', '10'], unary(parseInt)); // => [6, 8, 10]`
 */
export function unary(func) {
  if (false) { unary(func); }
  throw 'TODO: implement unary';
}

// flipshop
/** Expands the dotted top-level keys of `obj` into nested maps: `{ "a.b": 1, "a": { "c": 2 } }`
 * becomes `{ "a": { "b": 1, "c": 2 } }`.
 *
 * Keys are applied shallowest first, so the result is independent of map iteration order (which
 * FeatureScript does not promise): two keys of equal depth can never have one path be a prefix of
 * the other, so their writes cannot interact, and across depths the deeper key always lands
 * later. Shallow keys lay down structure, deeper keys refine it.
 *
 * Where two keys land on the same path, `onCollision(existing, incoming)` decides, defaulting to
 *
 * @seeAlso [deepMerge]
 * @seeAlso [partition]
 */
// export function undotMap(obj, onCollision) {}

// lodash
/** The inverse of `escape`; this method converts the HTML entities
 * `&amp;`, `&lt;`, `&gt;`, `&quot;`, and `&#39;` in `string` to
 * their corresponding characters.
 *
 * **Note:** No other HTML entities are unescaped. To unescape additional
 * HTML entities use a third-party library like [_he_](https://mths.be/he).
 *
 * @param string {string}: The string to unescape; defaults to `''`.
 *   @optional
 *
 * @returns {string}: the unescaped string.
 *
 * @example `unescape('fred, barney, &amp; pebbles'); // => 'fred, barney, & pebbles'`
 */
export function unescape(string) {
  if (false) { unescape(string); }
  throw 'TODO: implement unescape';
}

// flipshop
/** Deduplicated concatenation of every array in `arrList`, ordered by first occurrence.
 *
 * @example `union([[2], [1, 2], [2, 3]]); // => [2, 1, 3]`
 */
// export function union(arrList) {}

// lodash
/** Creates an array of unique values, in order, from all given arrays using
 * [`SameValueZero`](http://ecma-international.org/ecma-262/7.0/#sec-samevaluezero)
 * for equality comparisons.
 *
 * @param arrays {array}: The arrays to inspect.
 *   @optional
 *
 * @returns {array}: the new array of combined values.
 *
 * @example `union([2], [1, 2]); // => [2, 1]`
 */
export function union(arrays) {
  if (false) { union(arrays); }
  throw 'TODO: implement union';
}

// flipshop
/** `union`, deduplicating by `funcOrKey(val)` instead of `val` itself. `funcOrKey` is coerced
 * through `iteratee` @see `iteratee`.
 *
 * @example `unionBy([[2.1], [1.2, 2.3]], (val) => floor(val)); // => [2.1, 1.2]`
 */
// export function unionBy(arrList, funcOrKey) {}

// lodash
/** Like `union` except that it accepts `funcOrKey` which is
 * invoked for each element of each `arrays` to generate the criterion by
 * which uniqueness is computed. Result values are chosen from the first
 * array in which the value occurs. The function/propname is invoked with one argument:
 * (value).
 *
 * @param arrays {array}: The arrays to inspect.
 *   @optional
 * @param funcOrKey {function}: `(val) => criteria`; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the new array of combined values.
 *
 * @example `unionBy([2.1], [1.2, 2.3], Math.floor); // => [2.1, 1.2]`
 * @example `unionBy([{ 'x': 1 }], [{ 'x': 2 }, { 'x': 1 }], 'x'); // => [{ 'x': 1 }, { 'x': 2 }]`
 */
export function unionBy(arrays, funcOrKey) {
  if (false) { unionBy(arrays, funcOrKey); }
  throw 'TODO: implement unionBy';
}

// flipshop
/** `union`, deduplicating with `comparator(val, kept)` instead of `==`.
 *
 * @example `unionWith([[{ "x": 1 }], [{ "x": 1 }, { "x": 2 }]], (aa, bb) => aa.x == bb.x);`
 * @example `// => [{ "x": 1 }, { "x": 2 }]`
 */
// export function unionWith(arrList, comparator) {}

// lodash
/** Like `union` except that it accepts `comparator` which
 * is invoked to compare elements of `arrays`. Result values are chosen from
 * the first array in which the value occurs. The comparator is invoked
 * with two arguments: (arrVal, othVal).
 *
 * @param arrays {array}: The arrays to inspect.
 *   @optional
 * @param comparator {function}: The comparator invoked per element.
 *   @optional
 *
 * @returns {array}: the new array of combined values.
 *
 * @example `var objects = [{ 'x': 1, 'y': 2 }, { 'x': 2, 'y': 1 }]; var others = [{ 'x': 1, 'y': 1 }, { 'x': 1, 'y': 2 }]; unionWith(objects, others, isEqual); // => [{ 'x': 1, 'y': 2 }, { 'x': 2, 'y': 1 }, { 'x': 1, 'y': 1 }]`
 */
export function unionWith(arrays, comparator) {
  if (false) { unionWith(arrays, comparator); }
  throw 'TODO: implement unionWith';
}

// lodash
/** Creates a duplicate-free version of an array, using
 * [`SameValueZero`](http://ecma-international.org/ecma-262/7.0/#sec-samevaluezero)
 * for equality comparisons, in which only the first occurrence of each element
 * is kept. The order of result values is determined by the order they occur
 * in the array.
 *
 * @param array {array}: The array to inspect.
 *
 * @returns {array}: the new duplicate free array.
 *
 * @example `uniq([2, 1, 2]); // => [2, 1]`
 */
export function uniq(array) {
  if (false) { uniq(array); }
  throw 'TODO: implement uniq';
}

// flipshop
/** `arr` with duplicate elements removed, keeping the first occurrence -- like std's
 * `deduplicate`, but comparing `funcOrKey(val, idx)` instead of `val` itself. `funcOrKey` is
 * coerced through `iteratee` @see `iteratee`.
 *
 * @example `uniqBy([2.1, 1.2, 2.3], (val, _seq) => floor(val)); // => [2.1, 1.2]`
 */
// export function uniqBy(arr, funcOrKey) {}

// lodash
/** Like `uniq` except that it accepts `funcOrKey` which is
 * invoked for each element in `array` to generate the criterion by which
 * uniqueness is computed. The order of result values is determined by the
 * order they occur in the array. The function/propname is invoked with one argument:
 * (value).
 *
 * @param array {array}: The array to inspect.
 * @param funcOrKey {function}: `(val) => criteria`; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the new duplicate free array.
 *
 * @example `uniqBy([2.1, 1.2, 2.3], Math.floor); // => [2.1, 1.2]`
 * @example `uniqBy([{ 'x': 1 }, { 'x': 2 }, { 'x': 1 }], 'x'); // => [{ 'x': 1 }, { 'x': 2 }]`
 */
export function uniqBy(array, funcOrKey) {
  if (false) { uniqBy(array, funcOrKey); }
  throw 'TODO: implement uniqBy';
}

// lodash
/** Generates a unique ID. If `prefix` is given, the ID is appended to it.
 *
 * @param prefix {string}: The value to prefix the ID with; defaults to `''`.
 *   @optional
 *
 * @returns {string}: the unique ID.
 *
 * @example `uniqueId('contact_'); // => 'contact_104'`
 * @example `uniqueId(); // => '105'`
 */
export function uniqueId(prefix) {
  if (false) { uniqueId(prefix); }
  throw 'TODO: implement uniqueId';
}

// flipshop
/** `arr` with duplicate elements removed, keeping the first occurrence, where two elements count
 * as duplicates when `comparator(val, kept)` is `true`.
 *
 * @example `uniqWith([{ "x": 1 }, { "x": 1 }, { "x": 2 }], (aa, bb) => aa.x == bb.x);`
 * @example `// => [{ "x": 1 }, { "x": 2 }]`
 */
// export function uniqWith(arr, comparator) {}

// lodash
/** Like `uniq` except that it accepts `comparator` which
 * is invoked to compare elements of `array`. The order of result values is
 * determined by the order they occur in the array.The comparator is invoked
 * with two arguments: (arrVal, othVal).
 *
 * @param array {array}: The array to inspect.
 * @param comparator {function}: The comparator invoked per element.
 *   @optional
 *
 * @returns {array}: the new duplicate free array.
 *
 * @example `var objects = [{ 'x': 1, 'y': 2 }, { 'x': 2, 'y': 1 }, { 'x': 1, 'y': 2 }]; uniqWith(objects, isEqual); // => [{ 'x': 1, 'y': 2 }, { 'x': 2, 'y': 1 }]`
 */
export function uniqWith(array, comparator) {
  if (false) { uniqWith(array, comparator); }
  throw 'TODO: implement uniqWith';
}

// flipshop
/** Parses a 0.0–1.0 RGB(A) tuple string (`"0.5, 0.8675309, 1.0"` or `"[0.0, 0.1, 1.0, 1.0]"`)
 * into a `Color`. Missing alpha defaults to fully opaque. Returns `OopsColor` (bright red) on
 * anything that doesn't match, rather than throwing.
 *
 * @param unitcolor {string}
 *
 * @returns {{
 *    @field red {number}
 *    @field green {number}
 *    @field blue {number}
 *    @field alpha {number}
 * }}
 */
// export function unitcolorToColor(unitcolor) {}

// lodash
/** Removes the val at `path` of `object`.
 *
 * @param object {map}: The map to modify.
 * @param path {anypath}: Path to unset.
 *
 * @returns {boolean}: `true` if a val was deleted, else `false`.
 *
 * @example `var object = { 'a': [{ 'b': { 'c': 7 } }] }; unset(object, 'a.0.b.c'); // => true`
 * @example `println(object); // => { 'a': [{ 'b': {} }] };`
 * @example `unset(object, ['a', '0', 'b', 'c']); // => true`
 * @example `println(object); // => { 'a': [{ 'b': {} }] };`
 */
export function unset(object, path) {
  if (false) { unset(object, path); }
  throw 'TODO: implement unset';
}

// flipshop
/** Inverse of `zip` *(std)* -- ungroups `arr`'s rows back into columns. `zip`'s grouping is its own
 * inverse (transposing rows and columns twice returns the original shape), so `unzip` is just
 * `zip` under lodash's name for the reverse direction.
 *
 * @example `unzip([["a", 1, true], ["b", 2, false]]); // => [["a", "b"], [1, 2], [true, false]]`
 */
// export function unzip(arr) {}

// lodash
/** Like `zip` except that it accepts an array of grouped
 * elements and creates an array regrouping the elements to their pre-zip
 * configuration.
 *
 * @param array {array}: The array of grouped elements to process.
 *
 * @returns {array}: the new array of regrouped elements.
 *
 * @example `var zipped = zip(['a', 'b'], [1, 2], [true, false]); // => [['a', 1, true], ['b', 2, false]]`
 * @example `unzip(zipped); // => [['a', 'b'], [1, 2], [true, false]]`
 */
export function unzip(array) {
  if (false) { unzip(array); }
  throw 'TODO: implement unzip';
}

// flipshop
/** `unzip`, passing each ungrouped column through `combiner` before collecting it.
 *
 * @example `unzipWith([[1, 10], [2, 20]], (col) => sum(col)); // => [3, 30]`
 */
// export function unzipWith(arr, combiner) {}

// lodash
/** Like `unzip` except that it accepts `combiner` to specify how regrouped values should be
 * combined; invoked with the elements of each group: (...group).
 *
 * @param array {array}: The array of grouped elements to process.
 * @param combiner {function}: Invoked with the array of values collected at each position; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the new array of regrouped elements.
 *
 * @example `var zipped = zip([1, 2], [10, 20], [100, 200]); // => [[1, 10, 100], [2, 20, 200]]`
 * @example `unzipWith(zipped, add); // => [3, 30, 300]`
 */
export function unzipWith(array, combiner) {
  if (false) { unzipWith(array, combiner); }
  throw 'TODO: implement unzipWith';
}

// flipshop
/** Converts `str`, as a whole, to upper case. Same ASCII-only limitation as `downcase`.
 *
 * @example `upcase("fooBar"); // => "FOOBAR"`
 * @example `upcase("--foo-bar--"); // => "--FOO-BAR--"`
 */
// export function upcase(str) {}

// flipshop
/** Read-modify-write: `keyname`/`keypath` of `bag`/`arr` becomes `updater(currentVal)`.
 * `updateWith` additionally takes an `onCollision`, called only when the leaf `keynameOrPath`
 * already resolves to something -- it customizes how the freshly-`updater`'d value combines with
 * what was just read, not how a missing intermediate segment gets created (@see `setAtWith` for
 * that -- the actual counterpart to lodash's `setWith`/`updateWith` customizer). Since the value
 * being placed only ever collides with the value it was itself derived from, most of the time
 * `onCollision` can just take the incoming side and ignore `existing`. @see `getAt`/`setAt`.
 *
 * @example `update({ "a": 1 }, "a", (val) => val + 1); // => { "a": 2 }`
 */
// export function update(bag, keynameOrPath, updater) {}

// lodash
/** Like `set` except that accepts `updater` to produce the
 * value to set. Use `updateWith` to customize `path` creation. The `updater`
 * is invoked with one argument: (value).
 *
 * **Note:** This method mutates `object`.
 *
 * @param object {map}: The object to modify.
 * @param path {anypath}: Path to set.
 * @param updater {function}: Function to produce the updated value.
 *
 * @returns {map}: `object`.
 *
 * @example `var object = { 'a': [{ 'b': { 'c': 3 } }] }; update(object, 'a[0].b.c', function(n) { return n * n; }); println(object.a[0].b.c); // => 9`
 * @example `update(object, 'x[0].y.z', function(n) { return n ? n + 1 : 0; }); println(object.x[0].y.z); // => 0`
 */
export function update(object, path, updater) {
  if (false) { update(object, path, updater); }
  throw 'TODO: implement update';
}

// lodash
/** Like `update` except that it accepts `customizer` which is
 * invoked to produce the objects of `path`.  If `customizer` returns `undefined`
 * path creation is handled by the method instead. The `customizer` is invoked
 * with three arguments: (nsValue, key, nsObject).
 *
 * **Note:** This method mutates `object`.
 *
 * @param object {map}: The object to modify.
 * @param path {anypath}: Path to set.
 * @param updater {function}: Function to produce the updated value.
 * @param customizer {function}: Function to customize assigned values.
 *   @optional
 *
 * @returns {map}: `object`.
 *
 * @example `var object = {}; updateWith(object, '[0][1]', constant('a'), Object); // => { '0': { '1': 'a' } }`
 */
export function updateWith(object, path, updater, customizer) {
  if (false) { updateWith(object, path, updater, customizer); }
  throw 'TODO: implement updateWith';
}

// flipshop
/** /** `str` split into words, uppercased, and joined with a space. */
 */
// export function upperCase(str) {}

// lodash
/** Converts `string`, as space separated words, to upper case.
 *
 * @param string {string}: The string to convert; defaults to `''`.
 *   @optional
 *
 * @returns {string}: the upper cased string.
 *
 * @example `upperCase('--foo-bar'); // => 'FOO BAR'`
 * @example `upperCase('fooBar'); // => 'FOO BAR'`
 * @example `upperCase('__foo_bar__'); // => 'FOO BAR'`
 */
export function upperCase(string) {
  if (false) { upperCase(string); }
  throw 'TODO: implement upperCase';
}

// flipshop
/** `str` with only its first character uppercased, the rest left untouched -- unlike `capitalize`,
 * everything after the first character is left as-is rather than lowercased.
 *
 * @example `upperFirst("fred"); // => "Fred"`
 */
// export function upperFirst(str) {}

// lodash
/** Converts the first character of `string` to upper case.
 *
 * @param string {string}: The string to convert; defaults to `''`.
 *   @optional
 *
 * @returns {string}: the converted string.
 *
 * @example `upperFirst('fred'); // => 'Fred'`
 * @example `upperFirst('FRED'); // => 'FRED'`
 */
export function upperFirst(string) {
  if (false) { upperFirst(string); }
  throw 'TODO: implement upperFirst';
}

// lodash
/** Creates an array of the vals of `object`.
 *
 * @param object {map}: The map to query.
 *
 * @returns {array}: the array of vals.
 *
 * @example `values({ 'a': 1, 'b': 2 }); // => [1, 2] (iteration order is not guaranteed)`
 * @example `values('hi'); // => ['h', 'i']`
 */
export function values(object) {
  if (false) { values(object); }
  throw 'TODO: implement values';
}

// flipshop
/** Array of `bag`'s (or `arr`'s) values at `keylist`, in that order. Unlike lodash's `at`, there's
 * no path traversal -- each entry of `keylist` is a literal key or index, not a dotted path -- and
 * an array index must be non-negative and in bounds.
 *
 * `nilPolicy` decides what happens at a key/index with nothing there: `NIL` (the
 * default) fills the slot with `undefined`, so the result stays the same length as `keylist`;
 * `SKIP` drops the slot instead, so the result can come back shorter.
 *
 * @example `valuesAt({ "a": 11, "b": 22 }, ["b", "a"]); // => [22, 11]`
 * @example `valuesAt({ "a": 11, "b": 22 }, ["c"]); // => [undefined]`
 * @example `valuesAt({ "a": 11, "b": 22 }, ["c", "b"], NilPolicy.SKIP); // => [22]`
 */
// export function valuesAt(arr, keylist) {}

// flipshop
/** Keeps `varname` at `bagname ~ "_vals"` and `description` at a summary of the selection, each
 * for as long as it hasn't been hand-edited; @see `defaultMaybe`.
 */
// export function valuesAtEditLogic(context, id, oldDefinition, newDefinition, isCreating, specifiedParameters) {}

// lodash
/** Same as `values` -- FeatureScript maps have no own/inherited distinction, so there's no
 * extra reach for this to have over `values`.
 *
 * @param object {map}: The map to query.
 *
 * @returns {array}: the array of vals.
 *
 * @example `valuesIn({ 'a': 1, 'b': 2 }); // => [1, 2] (iteration order is not guaranteed)`
 */
export function valuesIn(object) {
  if (false) { valuesIn(object); }
  throw 'TODO: implement valuesIn';
}

// flipshop
/** `arr` without any element equal to one in `excludeArr`. Lodash's `without` takes the exclusion
 * values as trailing variadic arguments; here they're a single array, which makes this identical
 * to @see `difference` -- kept under its own name to match lodash's vocabulary.
 *
 * @example `without([2, 1, 2, 3], [1, 2]); // => [3]`
 */
// export function without(arr, excludeArr) {}

// lodash
/** Creates an array excluding all given values using
 * [`SameValueZero`](http://ecma-international.org/ecma-262/7.0/#sec-samevaluezero)
 * for equality comparisons.
 *
 * **Note:** Unlike `pull`, this method returns a new array.
 *
 * @seeAlso [difference]
 * @seeAlso [xor]
 *
 * @param array {array}: The array to inspect.
 * @param values: The values to exclude.
 *   @optional
 *
 * @returns {array}: the new array of filtered values.
 *
 * @example `without([2, 1, 2, 3], 1, 2); // => [3]`
 */
export function without(array, values) {
  if (false) { without(array, values); }
  throw 'TODO: implement without';
}

// flipshop
/** `str` split into words on runs of non-alphanumeric characters -- the shared primitive behind
 * `camelCase`/`kebabCase`/`snakeCase`/`upperCase`/`lowerCase` below. Simpler than lodash's own
 * `words`: this splits only on delimiter characters, not on camelCase boundaries or digit runs.
 *
 * @example `words("foo-bar_baz qux"); // => ["foo", "bar", "baz", "qux"]`
 */
// export function words(str) {}

// lodash
/** Splits `string` into an array of its words.
 *
 * @param string {string}: The string to inspect; defaults to `''`.
 *   @optional
 * @param pattern {RegExp|string}: The pattern to match words.
 *   @optional
 *
 * @returns {array}: the words of `string`.
 *
 * @example `words('fred, barney, & pebbles'); // => ['fred', 'barney', 'pebbles']`
 * @example `words('fred, barney, & pebbles', /[^, ]+/g); // => ['fred', 'barney', '&', 'pebbles']`
 */
export function words(string, pattern) {
  if (false) { words(string, pattern); }
  throw 'TODO: implement words';
}

// lodash
/** Creates a function that provides `value` to `wrapper` as its first
 * argument. Any additional arguments provided to the function are appended
 * to those provided to the `wrapper`. The wrapper is invoked with the `this`
 * binding of the created function.
 *
 * @param value: The value to wrap.
 * @param wrapper {function}: The wrapper function; defaults to `identity`.
 *   @optional
 *
 * @returns {function}: the new function.
 *
 * @example `var p = wrap(escape, function(func, text) { return '<p>' + func(text) + '</p>'; }); p('fred, barney, & pebbles'); // => '<p>fred, barney, &amp; pebbles</p>'`
 */
export function wrap(value, wrapper) {
  if (false) { wrap(value, wrapper); }
  throw 'TODO: implement wrap';
}

// lodash
/** This method is the wrapper version of `at`.
 *
 * @param keylist {array}: Each entry an `anypath`, to pick.
 *   @optional
 *
 * @returns {map}: the new `lodash` wrapper instance.
 *
 * @example `var object = { 'a': [{ 'b': { 'c': 3 } }, 4] }; _(object).at(['a[0].b.c', 'a[1]']).value(); // => [3, 4]`
 */
export function wrapperAt(paths) {
  if (false) { wrapperAt(paths); }
  throw 'TODO: implement wrapperAt';
}

// lodash
/** Creates a `lodash` wrapper instance with explicit method chain sequences enabled.
 *
 * @returns {map}: the new `lodash` wrapper instance.
 *
 * @example `var users = [ { 'user': 'barney', 'age': 36 }, { 'user': 'fred', 'age': 40 } ]; _(users).head(); // => { 'user': 'barney', 'age': 36 }`
 * @example `_(users) .chain() .head() .pick('user') .value(); // => { 'user': 'barney' }`
 */
export function wrapperChain() {
  if (false) { wrapperChain(); }
  throw 'TODO: implement wrapperChain';
}

// lodash
/** Executes the chain sequence and returns the wrapped result.
 *
 * @returns {map}: the new `lodash` wrapper instance.
 *
 * @example `var array = [1, 2]; var wrapped = _(array).push(3); println(array); // => [1, 2]`
 * @example `wrapped = wrapped.commit(); println(array); // => [1, 2, 3]`
 * @example `wrapped.last(); // => 3`
 * @example `println(array); // => [1, 2, 3]`
 */
export function wrapperCommit() {
  if (false) { wrapperCommit(); }
  throw 'TODO: implement wrapperCommit';
}

// lodash
/** Gets the next value on a wrapped object following the
 * [iterator protocol](https://mdn.io/iteration_protocols#iterator).
 *
 * @returns {map}: the next iterator value.
 *
 * @example `var wrapped = _([1, 2]); wrapped.next(); // => { 'done': false, 'value': 1 }`
 * @example `wrapped.next(); // => { 'done': false, 'value': 2 }`
 * @example `wrapped.next(); // => { 'done': true, 'value': undefined }`
 */
export function wrapperNext() {
  if (false) { wrapperNext(); }
  throw 'TODO: implement wrapperNext';
}

// lodash
/** Creates a clone of the chain sequence planting `value` as the wrapped value.
 *
 * @param value: The value to plant.
 *
 * @returns {map}: the new `lodash` wrapper instance.
 *
 * @example `function square(n) { return n * n; } var wrapped = _([1, 2]).map(square); var other = wrapped.plant([3, 4]); other.value(); // => [9, 16]`
 * @example `wrapped.value(); // => [1, 4]`
 */
export function wrapperPlant(value) {
  if (false) { wrapperPlant(value); }
  throw 'TODO: implement wrapperPlant';
}

// lodash
/** This method is the wrapper version of `reverse`.
 *
 * **Note:** This method mutates the wrapped array.
 *
 * @returns {map}: the new `lodash` wrapper instance.
 *
 * @example `var array = [1, 2, 3]; _(array).reverse().value() // => [3, 2, 1]`
 * @example `println(array); // => [3, 2, 1]`
 */
export function wrapperReverse() {
  if (false) { wrapperReverse(); }
  throw 'TODO: implement wrapperReverse';
}

// lodash
/** Enables the wrapper to be iterable.
 *
 * @returns {map}: the wrapper object.
 *
 * @example `var wrapped = _([1, 2]); wrapped[Symbol.iterator]() === wrapped; // => true`
 * @example `Array.from(wrapped); // => [1, 2]`
 */
export function wrapperToIterator() {
  if (false) { wrapperToIterator(); }
  throw 'TODO: implement wrapperToIterator';
}

// lodash
/** Executes the chain sequence to resolve the unwrapped value.
 *
 * @returns: the resolved unwrapped value.
 *
 * @example `_([1, 2, 3]).value(); // => [1, 2, 3]`
 */
export function wrapperValue() {
  if (false) { wrapperValue(); }
  throw 'TODO: implement wrapperValue';
}

// flipshop
/** Symmetric difference: values that appear in exactly one array of `arrList`, deduplicated, in
 * first-occurrence order. Lodash's `xor` takes the arrays as trailing variadic arguments; here
 * they're a single array of arrays.
 *
 * @example `xor([[2, 1], [2, 3]]); // => [1, 3]`
 */
// export function xor(arrList) {}

// lodash
/** Creates an array of unique values that is the
 * [symmetric difference](https://en.wikipedia.org/wiki/Symmetric_difference)
 * of the given arrays. The order of result values is determined by the order
 * they occur in the arrays.
 *
 * @seeAlso [difference]
 * @seeAlso [without]
 *
 * @param arrays {array}: The arrays to inspect.
 *   @optional
 *
 * @returns {array}: the new array of filtered values.
 *
 * @example `xor([2, 1], [2, 3]); // => [1, 3]`
 */
export function xor(arrays) {
  if (false) { xor(arrays); }
  throw 'TODO: implement xor';
}

// flipshop
/** `xor`, comparing by `funcOrKey(val, idx)` instead of `val` itself. `funcOrKey` is coerced
 * through `iteratee` @see `iteratee`.
 *
 * @example `xorBy([[2.1, 1.2], [2.3, 3.4]], (val, _seq) => floor(val)); // => [1.2, 3.4]`
 */
// export function xorBy(arrList, funcOrKey) {}

// lodash
/** Like `xor` except that it accepts `funcOrKey` which is
 * invoked for each element of each `arrays` to generate the criterion by
 * which by which they're compared. The order of result values is determined
 * by the order they occur in the arrays. The function/propname is invoked with one
 * argument: (value).
 *
 * @param arrays {array}: The arrays to inspect.
 *   @optional
 * @param funcOrKey {function}: `(val) => criteria`; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the new array of filtered values.
 *
 * @example `xorBy([2.1, 1.2], [2.3, 3.4], Math.floor); // => [1.2, 3.4]`
 * @example `xorBy([{ 'x': 1 }], [{ 'x': 2 }, { 'x': 1 }], 'x'); // => [{ 'x': 2 }]`
 */
export function xorBy(arrays, funcOrKey) {
  if (false) { xorBy(arrays, funcOrKey); }
  throw 'TODO: implement xorBy';
}

// flipshop
/** `xor`, comparing with `comparator(val, otherVal)` instead of `==`.
 *
 * @example `xorWith([[{ "x": 1 }, { "x": 2 }], [{ "x": 2 }]], (aa, bb) => aa.x == bb.x);`
 * @example `// => [{ "x": 1 }]`
 */
// export function xorWith(arrList, comparator) {}

// lodash
/** Like `xor` except that it accepts `comparator` which is
 * invoked to compare elements of `arrays`. The order of result values is
 * determined by the order they occur in the arrays. The comparator is invoked
 * with two arguments: (arrVal, othVal).
 *
 * @param arrays {array}: The arrays to inspect.
 *   @optional
 * @param comparator {function}: The comparator invoked per element.
 *   @optional
 *
 * @returns {array}: the new array of filtered values.
 *
 * @example `var objects = [{ 'x': 1, 'y': 2 }, { 'x': 2, 'y': 1 }]; var others = [{ 'x': 1, 'y': 1 }, { 'x': 1, 'y': 2 }]; xorWith(objects, others, isEqual); // => [{ 'x': 2, 'y': 1 }, { 'x': 1, 'y': 1 }]`
 */
export function xorWith(arrays, comparator) {
  if (false) { xorWith(arrays, comparator); }
  throw 'TODO: implement xorWith';
}

// lodash
/** Creates an array of grouped elements, the first of which contains the
 * first elements of the given arrays, the second of which contains the
 * second elements of the given arrays, and so on.
 *
 * @param arrays {array}: The arrays to process.
 *   @optional
 *
 * @returns {array}: the new array of grouped elements.
 *
 * @example `zip(['a', 'b'], [1, 2], [true, false]); // => [['a', 1, true], ['b', 2, false]]`
 */
export function zip(arrays) {
  if (false) { zip(arrays); }
  throw 'TODO: implement zip';
}

// flipshop
/** Map pairing up `keylist` and `valuelist` by position: `zipObject(["a","b"], [1,2])` is
 * `{"a": 1, "b": 2}`. A `keylist` entry past the end of `valuelist` is simply absent from the
 * result -- FeatureScript maps drop a key written to `undefined`; a `valuelist` entry past the end
 * of `keylist` is dropped.
 *
 * @example `zipObject(["a", "b"], [1, 2]); // => { "a": 1, "b": 2 }`
 * @example `zipObject(["a", "b"], [1]); // => { "a": 1 }`
 */
// export function zipObject(keylist, valuelist) {}

// lodash
/** Like `fromPairs` except that it accepts two arrays, one of keys and one of corresponding
 * values.
 *
 * @param keylist {array}: Defaults to `[]`.
 *   @optional
 * @param valuelist {array}: Defaults to `[]`.
 *   @optional
 *
 * @returns {map}: the new map.
 *
 * @example `zipObject(['a', 'b'], [1, 2]); // => { 'a': 1, 'b': 2 }`
 */
export function zipObject(keylist, valuelist) {
  if (false) { zipObject(keylist, valuelist); }
  throw 'TODO: implement zipObject';
}

// lodash
/** Like `zipObject` except that each entry of `keylist` is an `anypath` @see `getAt`, not a
 * literal key.
 *
 * @param keylist {array}: Each entry an `anypath`; defaults to `[]`.
 *   @optional
 * @param valuelist {array}: Defaults to `[]`.
 *   @optional
 *
 * @returns {map}: the new map.
 *
 * @example `zipObjectDeep(['a.b.0.c', 'a.b.1.d'], [1, 2]); // => { 'a': { 'b': [{ 'c': 1 }, { 'd': 2 }] } }`
 */
export function zipObjectDeep(keylist, valuelist) {
  if (false) { zipObjectDeep(keylist, valuelist); }
  throw 'TODO: implement zipObjectDeep';
}

// TODO-clxnUtils
/** `zip` *(std)* on `arrList`, passing each grouped row through `combiner` before collecting it --
 * the same shape as `unzipWith`, under lodash's name for the zipping direction.
 *
 * @example `zipWith([[1, 2], [10, 20]], (row) => sum(row)); // => [11, 22]`
 */
export function zipWith(arrList, combiner) {
  if (false) { zipWith(arrList, combiner); }
  throw 'TODO: implement zipWith';
}

// TODO-clxnUtils
/** Like `zip` except that it accepts `combiner` to specify how grouped values should be
 * combined; invoked with the elements of each group: (...group).
 *
 * @param arrays {array}: The arrays to process.
 *   @optional
 * @param combiner {function}: Invoked with the array of values collected at each position; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the new array of grouped elements.
 *
 * @example `zipWith([1, 2], [10, 20], [100, 200], function(a, b, c) { return a + b + c; }); // => [111, 222]`
 */
export function zipWith2(arrays, combiner) {
  if (false) { zipWith2(arrays, combiner); }
  throw 'TODO: implement zipWith2';
}
