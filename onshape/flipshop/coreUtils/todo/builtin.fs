
// lodash
/** Clamps `number` within the inclusive `lower` and `upper` bounds.
 *
 * @param number {number}: The number to clamp.
 * @param lower {number}: The lower bound.
 *   @optional
 * @param upper {number}: The upper bound.
 *
 * @returns {number}: the clamped number.
 *
 * @example `clamp(-10, -5, 5); // => -5`
 * @example `clamp(10, -5, 5); // => 5`
 */
export function clamp(number, lower, upper) {}


// lodash
/** Creates a new array concatenating `array` with any additional arrays
 * and/or values.
 *
 * @param array {array}: The array to concatenate.
 * @param values: The values to concatenate.
 *   @optional
 *
 * @returns {array}: the new concatenated array.
 *
 * @example `var array = [1]; var other = concat(array, 2, [3], [[4]]); println(other); // => [1, 2, 3, [4]]`
 * @example `println(array); // => [1]`
 */
export function concat(array, values) {
  if (false) { concat(array, values); }
  throw 'TODO: implement concat';
}

// lodash
/** Fills in `object`'s keys that resolve to `undefined` from `sources`, applied left to right;
 * once a key is filled, later sources are ignored for that key.
 *
 * @seeAlso [defaultsDeep]
 *
 * @param object {map}: The destination map.
 * @param sources {map}: The source maps.
 *   @optional
 *
 * @returns {map}: `object`.
 *
 * @example `defaults({ 'a': 1 }, { 'b': 2 }, { 'a': 3 }); // => { 'a': 1, 'b': 2 }`
 */
export function defaults(object, sources) {
  if (false) { defaults(object, sources); }
  throw 'TODO: implement defaults';
}
// lodash
/** Like `defaults` except that it recursively fills in default keys.
 *
 * @seeAlso [defaults]
 *
 * @param object {map}: The destination map.
 * @param sources {map}: The source maps.
 *   @optional
 *
 * @returns {map}: `object`.
 *
 * @example `defaultsDeep({ 'a': { 'b': 2 } }, { 'a': { 'b': 1, 'c': 3 } }); // => { 'a': { 'b': 2, 'c': 3 } }`
 */
export function defaultsDeep(object, sources) {
  if (false) { defaultsDeep(object, sources); }
  throw 'TODO: implement defaultsDeep';
}

// lodash
/** Checks `value` to determine whether a default value should be returned in
 * its place. The `defaultValue` is returned if `value` is `NaN`, `null`,
 * or `undefined`.
 *
 * @param value: The value to check.
 * @param defaultValue: The default value.
 *
 * @returns: the resolved value.
 *
 * @example `defaultTo(1, 10); // => 1`
 * @example `defaultTo(undefined, 10); // => 10`
 */
export function defaultTo(value, defaultValue) {
  if (false) { defaultTo(value, defaultValue); }
  throw 'TODO: implement defaultTo';
}

// lodash
/** Checks if `string` ends with the given target string.
 *
 * @param string {string}: The string to inspect; defaults to `''`.
 *   @optional
 * @param target {string}: The string to search for.
 *   @optional
 * @param position {number}: The position to search up to; defaults to `string.length`.
 *   @optional
 *
 * @returns {boolean}: `true` if `string` ends with `target`, else `false`.
 *
 * @example `endsWith('abc', 'c'); // => true`
 * @example `endsWith('abc', 'b'); // => false`
 * @example `endsWith('abc', 'b', 2); // => true`
 */
export function endsWith(string, target, position) {
  if (false) { endsWith(string, target, position); }
  throw 'TODO: implement endsWith';
}

// lodash
/** Checks if `rule` returns truthy for **all** elements of `collection`.
 * Iteration is stopped once `rule` returns falsey. `rule` is invoked as `(val, ckey)`.
 *
 * **Note:** This method returns `true` for
 * [empty collections](https://en.wikipedia.org/wiki/Empty_set) because
 * [everything is true](https://en.wikipedia.org/wiki/Vacuous_truth) of
 * elements of empty collections.
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param rule {ruleOrKey}: Coerced to a rule; defaults to `identity`.
 *   @optional
 *
 * @returns {boolean}: `true` if all elements pass the rule check, else `false`.
 *
 * @example `every([true, 1, null, 'yes'], Boolean); // => false`
 * @example `var users = [ { 'user': 'barney', 'age': 36, 'active': false }, { 'user': 'fred', 'age': 40, 'active': false } ]; every(users, { 'user': 'barney', 'active': false }); // => false`
 * @example `every(users, ['active', false]); // => true`
 * @example `every(users, 'active'); // => false`
 */
export function every(collection, rule) {
  if (false) { every(collection, rule); }
  throw 'TODO: implement every';
}

// lodash
/** Computes `number` rounded down to `precision`.
 *
 * @param number {number}: The number to round down.
 * @param precision {number}: The precision to round down to; defaults to `0`.
 *   @optional
 *
 * @returns {number}: the rounded down number.
 *
 * @example `floor(4.006); // => 4`
 * @example `floor(0.046, 2); // => 0.04`
 * @example `floor(4060, -2); // => 4000`
 */
export function floor(number, precision) {
  if (false) { floor(number, precision); }
  throw 'TODO: implement floor';
}

// lodash
/** Creates a function that returns the result of invoking the given functions
 * with the `this` binding of the created function, where each successive
 * invocation is supplied the return value of the previous.
 *
 * @seeAlso [flowRight]
 *
 * @param funcs {(Function|Function[])}: The functions to invoke.
 *   @optional
 *
 * @returns {function}: the new composite function.
 *
 * @example `function square(n) { return n * n; } var addSquare = flow([add, square]); addSquare(1, 2); // => 9`
 */
export function flow(funcs) {
  if (false) { flow(funcs); }
  throw 'TODO: implement flow';
}