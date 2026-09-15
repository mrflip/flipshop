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
/** Creates a function that invokes `func`, with up to `n` arguments,
 * ignoring any additional arguments.
 *
 * @param func {function}: Function to cap arguments for.
 * @param n {number}: The arity cap; defaults to `func.length`.
 *   @optional
 *
 * @returns {function}: the new capped function.
 *
 * @example `map(['6', '8', '10'], ary(parseInt, 1)); // => [6, 8, 10]`
 */
export function ary(func, n) {
  if (false) { ary(func, n); }
  throw 'TODO: implement ary';
}

// flipshop
/** Wraps a test function so a suite of throw cases runs through the same harness: the wrapped
 * function hands back whatever was thrown, and throws when nothing was. Belongs next to
 * runTests rather than here, once you have somewhere to put it.
 */
// export function assertThrows(testfunc) {}

// lodash
/** Copies keys of source maps onto the destination map, left to right; a later source's key
 * overwrites an earlier one.
 *
 * @seeAlso [assignIn]
 *
 * @param object {map}: The destination map.
 * @param sources {map}: The source maps.
 *   @optional
 *
 * @returns {map}: `object`.
 *
 * @example `assign({ 'a': 0, 'b': 1 }, { 'a': 1, 'c': 3 }); // => { 'a': 1, 'b': 1, 'c': 3 }`
 */
export function assign(object, sources) {
  if (false) { assign(object, sources); }
  throw 'TODO: implement assign';
}

// lodash
/** Same as `assign` -- FeatureScript maps have no own/inherited distinction, so there's no
 * extra reach for this to have over `assign`.
 *
 * @seeAlso [assign]
 *
 * @param object {map}: The destination map.
 * @param sources {map}: The source maps.
 *   @optional
 *
 * @returns {map}: `object`.
 *
 * @example `assignIn({ 'a': 0, 'b': 1 }, { 'a': 1, 'c': 3 }); // => { 'a': 1, 'b': 1, 'c': 3 }`
 */
export function assignIn(object, sources) {
  if (false) { assignIn(object, sources); }
  throw 'TODO: implement assignIn';
}

// lodash
/** Like `assignIn` except that it accepts `customizer`
 * which is invoked to produce the assigned values. If `customizer` returns
 * `undefined`, assignment is handled by the method instead. The `customizer`
 * is invoked with five arguments: (objValue, srcValue, key, object, source).
 *
 * **Note:** This method mutates `object`.
 *
 * @seeAlso [assignWith]
 *
 * @param object {map}: The destination object.
 * @param sources {map}: The source objects.
 * @param customizer {function}: Function to customize assigned values.
 *   @optional
 *
 * @returns {map}: `object`.
 *
 * @example `function customizer(objValue, srcValue) { return isUndefined(objValue) ? srcValue : objValue; } var defaults = partialRight(assignInWith, customizer); defaults({ 'a': 1 }, { 'b': 2 }, { 'a': 3 }); // => { 'a': 1, 'b': 2 }`
 */
export function assignInWith(object, sources, customizer) {
  if (false) { assignInWith(object, sources, customizer); }
  throw 'TODO: implement assignInWith';
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
export function at(object, keylist) {
  if (false) { at(object, keylist); }
  throw 'TODO: implement at';
}

// lodash
/** Attempts to invoke `func`, returning either the result or the caught error
 * object. Any additional arguments are provided to `func` when it's invoked.
 *
 * @param func {function}: Function to attempt.
 * @param args: The arguments to invoke `func` with.
 *   @optional
 *
 * @returns: the `func` result or error object.
 *
 * @example `var elements = attempt(function(selector) { return document.querySelectorAll(selector); }, '>_>'); if (isError(elements)) { elements = []; }`
 */
export function attempt(func, args) {
  if (false) { attempt(func, args); }
  throw 'TODO: implement attempt';
}

// TODO-helperFuncs
/** Creates a function that, for the first `maxCalls`, invokes `func` with the arguments it receives;
 * Subsequent calls to the created function return the result of the final `maxCalls`'th `func` invocation.
 *
 * @param maxCalls {number}: The number of calls at which `func` is no longer invoked.
 * @param func {function}: Function to restrict.
 *
 * @returns {function}: the new restricted function.
 *
 * @example `handler.on('click', before(5, addContactToList)); // => Allows adding up to 4 contacts to the list.`
 */
export function before(maxCalls is number, func is function) {
  if (false) { before(maxCalls is number, func is function); }
  throw 'TODO: implement before';
}

// lodash
/** Creates a function that invokes `func` with the `this` binding of `thisArg`
 * and `partials` prepended to the arguments it receives.
 *
 * The `bind.placeholder` value, which defaults to `_` in monolithic builds,
 * may be used as a placeholder for partially applied arguments.
 *
 * **Note:** Unlike native `Function#bind`, this method doesn't set the "length"
 * property of bound functions.
 *
 * @param func {function}: Function to bind.
 * @param thisArg: The `this` binding of `func`.
 * @param partials: The arguments to be partially applied.
 *   @optional
 *
 * @returns {function}: the new bound function.
 *
 * @example `function greet(greeting, punctuation) { return greeting + ' ' + this.user + punctuation; } var object = { 'user': 'fred' }; var bound = bind(greet, object, 'hi'); bound('!'); // => 'hi fred!'`
 * @example `var bound = bind(greet, object, _, '!'); bound('hi'); // => 'hi fred!'`
 */
export function bind(func, thisArg, partials) {
  if (false) { bind(func, thisArg, partials); }
  throw 'TODO: implement bind';
}

// lodash
/** Binds methods of an object to the object itself, overwriting the existing
 * method.
 *
 * **Note:** This method doesn't set the "length" property of bound functions.
 *
 * @param object {map}: The object to bind and assign the bound methods to.
 * @param methodNames {(string|string[])}: The object method names to bind.
 *
 * @returns {map}: `object`.
 *
 * @example `var view = { 'label': 'docs', 'click': function() { println('clicked ' + this.label); } }; bindAll(view, ['click']); jQuery(element).on('click', view.click); // => Logs 'clicked docs' when clicked.`
 */
export function bindAll(object, methodNames) {
  if (false) { bindAll(object, methodNames); }
  throw 'TODO: implement bindAll';
}

// lodash
/** Creates a function that invokes the method at `object[key]` with `partials`
 * prepended to the arguments it receives.
 *
 * This method differs from `bind` by allowing bound functions to reference
 * methods that may be redefined or don't yet exist. See
 * [Peter Michaux's article](http://peter.michaux.ca/articles/lazy-function-definition-pattern)
 * for more details.
 *
 * The `bindKey.placeholder` value, which defaults to `_` in monolithic
 * builds, may be used as a placeholder for partially applied arguments.
 *
 * @param object {map}: The object to invoke the method on.
 * @param key {string}: The key of the method.
 * @param partials: The arguments to be partially applied.
 *   @optional
 *
 * @returns {function}: the new bound function.
 *
 * @example `var object = { 'user': 'fred', 'greet': function(greeting, punctuation) { return greeting + ' ' + this.user + punctuation; } }; var bound = bindKey(object, 'greet', 'hi'); bound('!'); // => 'hi fred!'`
 * @example `object.greet = function(greeting, punctuation) { return greeting + 'ya ' + this.user + punctuation; }; bound('!'); // => 'hiya fred!'`
 * @example `var bound = bindKey(object, 'greet', _, '!'); bound('hi'); // => 'hiya fred!'`
 */
export function bindKey(object, key, partials) {
  if (false) { bindKey(object, key, partials); }
  throw 'TODO: implement bindKey';
}

// flipshop
/** Mutates the array inside `arrRef` in place and returns `val`, for accumulating into an outer
 * array from within a `forEach`/`mapValues` callback. `boxarrPush` appends; `boxarrUnshift`
 * prepends.
 */
// export function boxarrPush(arrRef, val) {}

// flipshop
/** Nested choice-list structure (`{ name, displayName, entries }` per level) built by walking
 * `tree` according to `levels`. Returns `tree` unchanged if `levels` has fewer than 2 entries.
 *
 * @param levels {array}: One entry per level -- @see `parseLevelDef` for the accepted shapes.
 * @param tree {map}: Raw deeply-nested map, keyed the same way at each level as `levels` describes.
 *
 * @example `buildNestedChoices(['socket_kind', ['drive_kind', { "inthex": 'Int Hex' }]], tree);`
 */
// export function buildNestedChoices(levels, tree) {}

// flipshop
/** `str` split into words and rejoined in camelCase: the first word lowercased, every other word
 * capitalized, no separators.
 *
 * @example `camelCase("Foo Bar"); // => "fooBar"`
 * @example `camelCase("foo-bar"); // => "fooBar"`
 */
// export function camelCase(str) {}

// lodash
/** Converts `string` to [camel case](https://en.wikipedia.org/wiki/CamelCase).
 *
 * @param string {string}: The string to convert; defaults to `''`.
 *   @optional
 *
 * @returns {string}: the camel cased string.
 *
 * @example `camelCase('Foo Bar'); // => 'fooBar'`
 * @example `camelCase('--foo-bar--'); // => 'fooBar'`
 * @example `camelCase('__FOO_BAR__'); // => 'fooBar'`
 */
export function camelCase(string) {
  if (false) { camelCase(string); }
  throw 'TODO: implement camelCase';
}

// flipshop
/** `str` with its first character uppercased and the rest lowercased.
 *
 * @example `capitalize("FRED"); // => "Fred"`
 */
// export function capitalize(str) {}

// lodash
/** Converts the first character of `string` to upper case and the remaining
 * to lower case.
 *
 * @param string {string}: The string to capitalize; defaults to `''`.
 *   @optional
 *
 * @returns {string}: the capitalized string.
 *
 * @example `capitalize('FRED'); // => 'Fred'`
 */
export function capitalize(string) {
  if (false) { capitalize(string); }
  throw 'TODO: implement capitalize';
}

// lodash
/** Casts `value` as an array if it's not one.
 *
 * @param value: The value to inspect.
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
export function castArray(value) {
  if (false) { castArray(value); }
  throw 'TODO: implement castArray';
}

// lodash
/** Computes `number` rounded up to `precision`.
 *
 * @param number {number}: The number to round up.
 * @param precision {number}: The precision to round up to; defaults to `0`.
 *   @optional
 *
 * @returns {number}: the rounded up number.
 *
 * @example `ceil(4.006); // => 5`
 * @example `ceil(6.004, 2); // => 6.01`
 * @example `ceil(6040, -2); // => 6100`
 */
export function ceil(number, precision) {
  if (false) { ceil(number, precision); }
  throw 'TODO: implement ceil';
}

// lodash
/** Creates a `lodash` wrapper instance that wraps `value` with explicit method
 * chain sequences enabled. The result of such sequences must be unwrapped
 * with `_#value`.
 *
 * @param value: The value to wrap.
 *
 * @returns {map}: the new `lodash` wrapper instance.
 *
 * @example `var users = [ { 'user': 'barney', 'age': 36 }, { 'user': 'fred', 'age': 40 }, { 'user': 'pebbles', 'age': 1 } ]; var youngest = _ .chain(users) .sortBy('age') .map(function(o) { return o.user + ' is ' + o.age; }) .head() .value(); // => 'pebbles is 1'`
 */
export function chain(value) {
  if (false) { chain(value); }
  throw 'TODO: implement chain';
}

// flipshop
/** Splits `arr` into groups of `chunkSize` elements each; the last group holds whatever's left
 * over. `chunkSize < 1` returns an empty array. Unlike lodash, `chunkSize` has no default of `1`
 * -- it's always required here.
 *
 * @example `chunk(["a", "b", "c", "d"], 2); // => [["a", "b"], ["c", "d"]]`
 * @example `chunk(["a", "b", "c", "d"], 3); // => [["a", "b", "c"], ["d"]]`
 */
// export function chunk(arr, chunkSize) {}

// lodash
/** Creates an array of elements split into groups the length of `size`.
 * If `array` can't be split evenly, the final chunk will be the remaining
 * elements.
 *
 * @param array {array}: The array to process.
 * @param size {number}: The length of each chunk; defaults to `1`.
 *   @optional
 *
 * @returns {array}: the new array of chunks.
 *
 * @example `chunk(['a', 'b', 'c', 'd'], 2); // => [['a', 'b'], ['c', 'd']]`
 * @example `chunk(['a', 'b', 'c', 'd'], 3); // => [['a', 'b', 'c'], ['d']]`
 */
export function chunk(array, size) {
  if (false) { chunk(array, size); }
  throw 'TODO: implement chunk';
}

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
export function clamp(number, lower, upper) {
  if (false) { clamp(number, lower, upper); }
  throw 'TODO: implement clamp';
}

// lodash
/** Creates a shallow clone of `value`.
 *
 * **Note:** This method is loosely based on the
 * [structured clone algorithm](https://mdn.io/Structured_clone_algorithm)
 * and supports cloning arrays, array buffers, booleans, date objects, maps,
 * numbers, `Object` objects, regexes, sets, strings, symbols, and typed
 * arrays. The own enumerable properties of `arguments` objects are cloned
 * as plain objects. An empty object is returned for uncloneable values such
 * as error objects, functions, DOM nodes, and WeakMaps.
 *
 * @seeAlso [cloneDeep]
 *
 * @param value: The value to clone.
 *
 * @returns: the cloned value.
 *
 * @example `var objects = [{ 'a': 1 }, { 'b': 2 }]; var shallow = clone(objects); println(shallow[0] === objects[0]); // => true`
 */
export function clone(value) {
  if (false) { clone(value); }
  throw 'TODO: implement clone';
}

// lodash
/** Like `clone` except that it recursively clones `value`.
 *
 * @seeAlso [clone]
 *
 * @param value: The value to recursively clone.
 *
 * @returns: the deep cloned value.
 *
 * @example `var objects = [{ 'a': 1 }, { 'b': 2 }]; var deep = cloneDeep(objects); println(deep[0] === objects[0]); // => false`
 */
export function cloneDeep(value) {
  if (false) { cloneDeep(value); }
  throw 'TODO: implement cloneDeep';
}

// lodash
/** Like `cloneWith` except that it recursively clones `value`.
 *
 * @seeAlso [cloneWith]
 *
 * @param value: The value to recursively clone.
 * @param customizer {function}: Function to customize cloning.
 *   @optional
 *
 * @returns: the deep cloned value.
 *
 * @example `function customizer(value) { if (isElement(value)) { return value.cloneNode(true); } } var el = cloneDeepWith(document.body, customizer); println(el === document.body); // => false`
 * @example `println(el.nodeName); // => 'BODY'`
 * @example `println(el.childNodes.length); // => 20`
 */
export function cloneDeepWith(value, customizer) {
  if (false) { cloneDeepWith(value, customizer); }
  throw 'TODO: implement cloneDeepWith';
}

// lodash
/** Like `clone` except that it accepts `customizer` which
 * is invoked to produce the cloned value. If `customizer` returns `undefined`,
 * cloning is handled by the method instead. The `customizer` is invoked with
 * up to four arguments; (value [, index|key, object, stack]).
 *
 * @seeAlso [cloneDeepWith]
 *
 * @param value: The value to clone.
 * @param customizer {function}: Function to customize cloning.
 *   @optional
 *
 * @returns: the cloned value.
 *
 * @example `function customizer(value) { if (isElement(value)) { return value.cloneNode(false); } } var el = cloneWith(document.body, customizer); println(el === document.body); // => false`
 * @example `println(el.nodeName); // => 'BODY'`
 * @example `println(el.childNodes.length); // => 0`
 */
export function cloneWith(value, customizer) {
  if (false) { cloneWith(value, customizer); }
  throw 'TODO: implement cloneWith';
}

// flipshop
/** /** Undefined is equal to itself and nothing else -- comparing with anything else is an error */
 */
// export function cmpTo(aa, bb) {}

// flipshop
/** /** true is greater than false; booleans cannot compare with anything else */
 */
// export function cmpTo(aa, bb) {}

// flipshop
/** /** Compares numbers numerically using `tolerantEquals` */
 */
// export function cmpTo(aa, bb) {}

// flipshop
/** /** Compares ValueWithUnits numerically using `tolerantEquals`; the units must be compatible */
 */
// export function cmpTo(aa, bb) {}

// flipshop
/** /** Compares strings lexicographically; the strings must be compatible. String comparison is slow and stupid */
 */
// export function cmpTo(aa, bb) {}

// flipshop
/** `arr` with every falsey element removed, per typeUtils' `truthy` -- narrower than lodash's own
 * falsey set, since FeatureScript treats only `undefined` and `false` as falsey (`0` and `""`
 * stay truthy).
 *
 * @example `compact([0, 1, false, 2, "", 3]); // => [0, 1, 2, "", 3]`
 * @example `compact([0, false, undefined]); // => [0]`
 */
// export function compact(arr) {}

// lodash
/** Creates an array with all falsey values removed. The values `false`, `null`,
 * `0`, `-0`, `0n`, `""`, `undefined`, and `NaN` are falsy.
 *
 * @param array {array}: The array to compact.
 *
 * @returns {array}: the new array of filtered values.
 *
 * @example `compact([0, 1, false, 2, '', 3]); // => [1, 2, 3]`
 */
export function compact(array) {
  if (false) { compact(array); }
  throw 'TODO: implement compact';
}

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

// flipshop
/** Builds a function that tries `pairs` in order, calling and returning the first `handler` whose
 * `rule` holds `val`, or `undefined` if none does. Each `rule` is coerced through `funcOrProp`, so a
 * dotpath string, `[path, srcValue]` array, or partial-match map works in place of a
 * literal `rule(val, seq) => boolean` function -- matching lodash's own `cond`, which runs
 * `funcOrProp` on every rule.
 *
 * @example `const grade = cond([ [(score, _seq) => score >= 90, constant("A")],`
 * @example `[(score, _seq) => score >= 80, constant("B")],`
 * @example `[constant(true), constant("F")] ]); grade(95, 0); // => "A"`
 * @example `grade(70, 0); // => "F"`
 */
// export function cond(pairs) {}

// flipshop
/** `cond`, keyed by rule instead of ordered by array position -- each map key doubles as its own
 * `rule` (coerced through `funcOrProp`, so a key is naturally a dotpath string), paired with
 * its value as the `handler`.
 *
 * @example `const speak = cond({ "isDog": constant("Woof"), "isCat": constant("Meow") }); speak({ "isDog": true, "isCat": false }, 0); // => "Woof"`
 */
// export function cond(pairs) {}

// lodash
/** Creates a function that iterates over `pairs` and invokes the corresponding
 * function of the first rule to return truthy. The rule-function
 * pairs are invoked with the `this` binding and arguments of the created
 * function.
 *
 * @param pairs {array}: The rule-function pairs.
 *
 * @returns {function}: the new composite function.
 *
 * @example `var func = cond([ [matches({ 'a': 1 }), constant('matches A')], [conforms({ 'b': isNumber }), constant('matches B')], [stubTrue, constant('no match')] ]); func({ 'a': 1, 'b': 2 }); // => 'matches A'`
 * @example `func({ 'a': 0, 'b': 1 }); // => 'matches B'`
 * @example `func({ 'a': '1', 'b': '2' }); // => 'no match'`
 */
export function cond(pairs) {
  if (false) { cond(pairs); }
  throw 'TODO: implement cond';
}

// lodash
/** Creates a function that invokes the rule properties of `source` with
 * the corresponding property values of a given object, returning `true` if
 * all rules return truthy, else `false`.
 *
 * **Note:** The created function is equivalent to `conformsTo` with
 * `source` partially applied.
 *
 * @param source {map}: The object of property rules to conform to.
 *
 * @returns {function}: the new spec function.
 *
 * @example `var objects = [ { 'a': 2, 'b': 1 }, { 'a': 1, 'b': 2 } ]; filter(objects, conforms({ 'b': function(n) { return n > 1; } })); // => [{ 'a': 1, 'b': 2 }]`
 */
export function conforms(source) {
  if (false) { conforms(source); }
  throw 'TODO: implement conforms';
}

// lodash
/** Checks if `object` conforms to `source` by invoking the rule
 * properties of `source` with the corresponding property values of `object`.
 *
 * **Note:** This method is equivalent to `conforms` when `source` is
 * partially applied.
 *
 * @param object {map}: The object to inspect.
 * @param source {map}: The object of property rules to conform to.
 *
 * @returns {boolean}: `true` if `object` conforms, else `false`.
 *
 * @example `var object = { 'a': 1, 'b': 2 }; conformsTo(object, { 'b': function(n) { return n > 1; } }); // => true`
 * @example `conformsTo(object, { 'b': function(n) { return n > 2; } }); // => false`
 */
export function conformsTo(object, source) {
  if (false) { conformsTo(object, source); }
  throw 'TODO: implement conformsTo';
}

// flipshop
/** Builds a single-argument function that always returns `val`, ignoring the argument it's called
 * with -- FeatureScript calls a function with exactly its declared arity, so unlike lodash's
 * `constant` this only fits a one-argument slot (e.g. `mapArray`/`filter`/a `cond` handler); lift
 * it into a `(val, seq)` slot with `curry2to1(constant(val))`.
 *
 * @example `times(3, constant(0)); // => [0, 0, 0]`
 */
// export function constant(val, arity) {}

// lodash
/** Creates a function that returns `value`.
 *
 * @param value: The value to return from the new function.
 *
 * @returns {function}: the new constant function.
 *
 * @example `var objects = times(2, constant({ 'a': 1 })); println(objects); // => [{ 'a': 1 }, { 'a': 1 }]`
 * @example `println(objects[0] === objects[1]); // => true`
 */
export function constant(value) {
  if (false) { constant(value); }
  throw 'TODO: implement constant';
}

// flipshop
/** Map of `iterateeSpec(val, seq)` (array) / `iterateeSpec(val, key)` (map) results to how many
 * elements of `arr`/`bag` produced that result. `iterateeSpec` is coerced through `funcOrProp`
 *
 * @example `countBy([1, 2, 3, 4], (val, _seq) => (val % 2 == 0) ? "even" : "odd"); // => { "odd": 2, "even": 2 }`
 * @example `countBy({ a: 1, b: 2 }, (val, _key) => (val % 2 == 0) ? "even" : "odd"); // => { "odd": 1, "even": 1 }`
 */
// export function countBy(arr, iterateeSpec) {}

// lodash
/** Creates an object composed of keys generated from the results of running
 * each element of `collection` thru `funcOrProp`. The corresponding value of
 * each key is the number of times the key was returned by `funcOrProp`. The
 * iteratee is invoked (value).
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param funcOrProp {function}: Function/propname to transform keys; defaults to `identity`.
 *   @optional
 *
 * @returns {map}: the composed aggregate object.
 *
 * @example `countBy([6.1, 4.2, 6.3], Math.floor); // => { '4': 1, '6': 2 }`
 * @example `countBy(['one', 'two', 'three'], 'length'); // => { '3': 2, '5': 1 }`
 */
export function countBy(collection, iteratee) {
  if (false) { countBy(collection, iteratee); }
  throw 'TODO: implement countBy';
}

// TODO-metaprogramming
/** Creates a function that accepts arguments of `func` and either invokes
 * `func` returning its result, if at least `arity` number of arguments have
 * been provided, or returns a function that accepts the remaining `func`
 * arguments, and so on. The arity of `func` may be specified if `func.length`
 * is not sufficient.
 *
 * The `curry.placeholder` value, which defaults to `_` in monolithic builds,
 * may be used as a placeholder for provided arguments.
 *
 * **Note:** This method doesn't set the "length" property of curried functions.
 *
 * @param func {function}: Function to curry.
 * @param arity {number}: The arity of `func`; defaults to `func.length`.
 *   @optional
 *
 * @returns {function}: the new curried function.
 *
 * @example `var abc = function(a, b, c) { return [a, b, c]; }; var curried = curry(abc); curried(1)(2)(3); // => [1, 2, 3]`
 * @example `curried(1, 2)(3); // => [1, 2, 3]`
 * @example `curried(1, 2, 3); // => [1, 2, 3]`
 * @example `curried(1)(_, 3)(2); // => [1, 2, 3]`
 */
export function curry(func, arity) {
  if (false) { curry(func, arity); }
  throw 'TODO: implement curry';
}

// flipshop
/** `curryNtoM` wraps `func` to accept `N` arguments but call `func` with only the first `M` of
 * them -- dropping trailing arguments so a fixed-arity callback (`func()`, `func(val)`, …) can sit
 * in a slot that always calls with `N` arguments, like a `forEach`/`mapValues` iteratee.
 *
 * @example `curry2to0(function() { return "called"; })("ignored1", "ignored2"); // => "called"`
 * @example `curry3to1(function(val) { return val; })(1, 2, 3); // => 1`
 */
// export function curry3to0(func) {}

// TODO-metaprogramming
/** Like `curry` except that arguments are applied to `func`
 * in the manner of `partialRight` instead of `partial`.
 *
 * The `curryRight.placeholder` value, which defaults to `_` in monolithic
 * builds, may be used as a placeholder for provided arguments.
 *
 * **Note:** This method doesn't set the "length" property of curried functions.
 *
 * @param func {function}: Function to curry.
 * @param arity {number}: The arity of `func`; defaults to `func.length`.
 *   @optional
 *
 * @returns {function}: the new curried function.
 *
 * @example `var abc = function(a, b, c) { return [a, b, c]; }; var curried = curryRight(abc); curried(3)(2)(1); // => [1, 2, 3]`
 * @example `curried(2, 3)(1); // => [1, 2, 3]`
 * @example `curried(1, 2, 3); // => [1, 2, 3]`
 * @example `curried(3)(1, _)(2); // => [1, 2, 3]`
 */
export function curryRight(func, arity) {
  if (false) { curryRight(func, arity); }
  throw 'TODO: implement curryRight';
}

// lodash
/** Creates a debounced function that delays invoking `func` until after `wait`
 * milliseconds have elapsed since the last time the debounced function was
 * invoked. The debounced function comes with a `cancel` method to cancel
 * delayed `func` invocations and a `flush` method to immediately invoke them.
 * Provide `options` to indicate whether `func` should be invoked on the
 * leading and/or trailing edge of the `wait` timeout. The `func` is invoked
 * with the last arguments provided to the debounced function. Subsequent
 * calls to the debounced function return the result of the last `func`
 * invocation.
 *
 * **Note:** If `leading` and `trailing` options are `true`, `func` is
 * invoked on the trailing edge of the timeout only if the debounced function
 * is invoked more than once during the `wait` timeout.
 *
 * If `wait` is `0` and `leading` is `false`, `func` invocation is deferred
 * until to the next tick, similar to `setTimeout` with a timeout of `0`.
 *
 * See [David Corbacho's article](https://css-tricks.com/debouncing-throttling-explained-examples/)
 * for details over the differences between `debounce` and `throttle`.
 *
 * @param func {function}: Function to debounce.
 * @param wait {number}: The number of milliseconds to delay; defaults to `0`.
 *   @optional
 * @param options {{
 *    @field leading {boolean}: Specify invoking on the leading edge of the timeout; defaults to `false`.
 *     @optional
 *    @field maxWait {number}: The maximum time `func` is allowed to be delayed before it's invoked.
 *     @optional
 *    @field trailing {boolean}: Specify invoking on the trailing edge of the timeout; defaults to `true`.
 *     @optional
 * }}
 *
 * @returns {function}: the new debounced function.
 *
 * @example `jQuery(window).on('resize', debounce(calculateLayout, 150)); jQuery(element).on('click', debounce(sendMail, 300, { 'leading': true, 'trailing': false })); var debounced = debounce(batchLog, 250, { 'maxWait': 1000 }); var source = new EventSource('/stream'); jQuery(source).on('message', debounced); jQuery(window).on('popstate', debounced.cancel);`
 */
export function debounce(func, wait, options) {
  if (false) { debounce(func, wait, options); }
  throw 'TODO: implement debounce';
}

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
/** Value for `newDefinition[destkey]`, keeping it auto-derived from `newDefinition[basekey]` via
 * `valfunc(baseVal, definition)` for as long as the user hasn't overridden it -- the pattern
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
 *
 * @example `defaultMaybe({ "bagname": "foo", "varname": "foo_keys" }, { "bagname": "bar", "varname": "foo_keys" }, "bagname", "varname", (name, _) => (name ~ "_keys"));`
 * @example `// => "bar_keys" -- varname was tracking its default, so it follows bagname's rename`
 * @example `defaultMaybe({ "bagname": "foo", "varname": "myKeys" }, { "bagname": "bar", "varname": "myKeys" }, "bagname", "varname", (name, _) => (name ~ "_keys"));`
 * @example `// => "myKeys" -- varname was hand-edited away from its default, so it's left alone`
 */
// export function defaultMaybe(oldDefinition, newDefinition, basekey, destkey, valfunc) {}

// lodash
/** Assigns own and inherited enumerable string keyed properties of source
 * objects to the destination object for all destination properties that
 * resolve to `undefined`. Source objects are applied from left to right.
 * Once a property is set, additional values of the same property are ignored.
 *
 * **Note:** This method mutates `object`.
 *
 * @seeAlso [defaultsDeep]
 *
 * @param object {map}: The destination object.
 * @param sources {map}: The source objects.
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
/** Like `defaults` except that it recursively assigns
 * default properties.
 *
 * **Note:** This method mutates `object`.
 *
 * @seeAlso [defaults]
 *
 * @param object {map}: The destination object.
 * @param sources {map}: The source objects.
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
/** Defers invoking the `func` until the current call stack has cleared. Any
 * additional arguments are provided to `func` when it's invoked.
 *
 * @param func {function}: Function to defer.
 * @param args: The arguments to invoke `func` with.
 *   @optional
 *
 * @returns {number}: the timer id.
 *
 * @example `defer(function(text) { println(text); }, 'deferred'); // => Logs 'deferred' after one millisecond.`
 */
export function defer(func, args) {
  if (false) { defer(func, args); }
  throw 'TODO: implement defer';
}

// lodash
/** Invokes `func` after `wait` milliseconds. Any additional arguments are
 * provided to `func` when it's invoked.
 *
 * @param func {function}: Function to delay.
 * @param wait {number}: The number of milliseconds to delay invocation.
 * @param args: The arguments to invoke `func` with.
 *   @optional
 *
 * @returns {number}: the timer id.
 *
 * @example `delay(function(text) { println(text); }, 1000, 'later'); // => Logs 'later' after one second.`
 */
export function delay(func, wait, args) {
  if (false) { delay(func, wait, args); }
  throw 'TODO: implement delay';
}

// flipshop
/** `arr`'s values that don't appear in `excludeArr`, order taken from `arr`. Lodash's `difference`
 * takes the exclusion values as trailing variadic arrays; FeatureScript has no varargs, so they're
 * a single array here -- call with `concatenateArrays([...])` to exclude from several sources at
 * once.
 *
 * @example `difference([2, 1], [2, 3]); // => [1]`
 */
// export function difference(arr, excludeArr) {}

// lodash
/** Creates an array of `array` values not included in the other given arrays
 * using [`SameValueZero`](http://ecma-international.org/ecma-262/7.0/#sec-samevaluezero)
 * for equality comparisons. The order and references of result values are
 * determined by the first array.
 *
 * **Note:** Unlike `pullAll`, this method returns a new array.
 *
 * @seeAlso [without]
 * @seeAlso [xor]
 *
 * @param array {array}: The array to inspect.
 * @param values {array}: The values to exclude.
 *   @optional
 *
 * @returns {array}: the new array of filtered values.
 *
 * @example `difference([2, 1], [2, 3]); // => [1]`
 */
export function difference(array, values) {
  if (false) { difference(array, values); }
  throw 'TODO: implement difference';
}

// flipshop
/** `difference`, comparing `arr` and `excludeArr` by `iterateeSpec(val, seq)` instead of `val`
 * itself. `iterateeSpec` is coerced through `funcOrProp` @see `funcOrProp`.
 *
 * @example `differenceBy([2.1, 1.2], [2.3, 3.4], (val, _seq) => floor(val)); // => [1.2]`
 */
// export function differenceBy(arr, excludeArr, iterateeSpec) {}

// lodash
/** Like `difference` except that it accepts `funcOrProp` which
 * is invoked for each element of `array` and `values` to generate the criterion
 * by which they're compared. The order and references of result values are
 * determined by the first array. The function/propname is invoked with one argument:
 * (value).
 *
 * **Note:** Unlike `pullAllBy`, this method returns a new array.
 *
 * @param array {array}: The array to inspect.
 * @param values {array}: The values to exclude.
 *   @optional
 * @param funcOrProp {function}: Function/propname invoked per element; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the new array of filtered values.
 *
 * @example `differenceBy([2.1, 1.2], [2.3, 3.4], Math.floor); // => [1.2]`
 * @example `differenceBy([{ 'x': 2 }, { 'x': 1 }], [{ 'x': 1 }], 'x'); // => [{ 'x': 2 }]`
 */
export function differenceBy(array, values, iteratee) {
  if (false) { differenceBy(array, values, iteratee); }
  throw 'TODO: implement differenceBy';
}

// flipshop
/** `difference`, comparing `arr` and `excludeArr` with `comparator(val, other)` instead of `==`.
 *
 * @example `differenceWith([{ "x": 1 }, { "x": 2 }], [{ "x": 1 }], (aa, bb) => aa.x == bb.x);`
 * @example `// => [{ "x": 2 }]`
 */
// export function differenceWith(arr, excludeArr, comparator) {}

// lodash
/** Like `difference` except that it accepts `comparator`
 * which is invoked to compare elements of `array` to `values`. The order and
 * references of result values are determined by the first array. The comparator
 * is invoked with two arguments: (arrVal, othVal).
 *
 * **Note:** Unlike `pullAllWith`, this method returns a new array.
 *
 * @param array {array}: The array to inspect.
 * @param values {array}: The values to exclude.
 *   @optional
 * @param comparator {function}: The comparator invoked per element.
 *   @optional
 *
 * @returns {array}: the new array of filtered values.
 *
 * @example `var objects = [{ 'x': 1, 'y': 2 }, { 'x': 2, 'y': 1 }]; differenceWith(objects, [{ 'x': 1, 'y': 2 }], isEqual); // => [{ 'x': 2, 'y': 1 }]`
 */
export function differenceWith(array, values, comparator) {
  if (false) { differenceWith(array, values, comparator); }
  throw 'TODO: implement differenceWith';
}

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
  if (false) { divide(dividend, divisor); }
  throw 'TODO: implement divide';
}

// flipshop
/** Depth-first flatten of a nested map into dotted keys: `{ "a": { "b": 1 } }` becomes
 * `{ "a.b": 1 }`.
 *
 * `options.maxDepth` is how many levels to collapse, i.e. how many dots a key can gain, the
 * way lodash's flattenDepth counts. Absent means all the way down; 0 or less is a no-op, and
 * a depth deeper or shallower than the map is fine. Unknown option keys are ignored.
 *
 * Arrays are leaves, and so are empty maps: there is no inner key to dot with, and dropping
 * the key would lose it.
 *
 * Keys that already contain a dot are dotted anyway. undotMap cannot tell those dots from the
 * ones added here, so a key like `"H2.5mm"` does not survive the round trip. See the
 * RoundTripCases in the test module.
 */
// export function dotMap(obj, options) {}

// flipshop
/** Converts `str`, as a whole, to lower case. ASCII-only, via an explicit character lookup --
 * unlike lodash's `toLower`, there's no Unicode case folding, but a non-letter character is left
 * untouched rather than causing an error.
 *
 * @example `downcase("fooBar"); // => "foobar"`
 * @example `downcase("--FOO-BAR--"); // => "--foo-bar--"`
 */
// export function downcase(str) {}

// flipshop
/** `arr` with the first `dropCount` elements removed; `dropCount <= 0` returns `arr` unchanged.
 *
 * @example `drop([1, 2, 3], 2); // => [3]`
 * @example `drop([1, 2, 3], 5); // => []`
 */
// export function drop(arr, dropCount) {}

// lodash
/** Creates a slice of `array` with `n` elements dropped from the beginning.
 *
 * @param array {array}: The array to query.
 * @param n {number}: The number of elements to drop; defaults to `1`.
 *   @optional
 *
 * @returns {array}: the slice of `array`.
 *
 * @example `drop([1, 2, 3]); // => [2, 3]`
 * @example `drop([1, 2, 3], 2); // => [3]`
 * @example `drop([1, 2, 3], 5); // => []`
 * @example `drop([1, 2, 3], 0); // => [1, 2, 3]`
 */
export function drop(array, n) {
  if (false) { drop(array, n); }
  throw 'TODO: implement drop';
}

// flipshop
/** `arr` with the last `dropCount` elements removed; `dropCount <= 0` returns `arr` unchanged.
 *
 * @example `dropRight([1, 2, 3], 2); // => [1]`
 * @example `dropRight([1, 2, 3], 5); // => []`
 */
// export function dropRight(arr, dropCount) {}

// lodash
/** Creates a slice of `array` with `n` elements dropped from the end.
 *
 * @param array {array}: The array to query.
 * @param n {number}: The number of elements to drop; defaults to `1`.
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
/** `arr` with elements dropped from the end for as long as `rule` holds; the first
 * (rightmost-scanned) element `rule` rejects, and everything before it, is kept.
 *
 * @example `dropRightWhile([1, 2, 3, 4], (val) => val > 2); // => [1, 2]`
 */
// export function dropRightWhile(arr, rule) {}

// lodash
/** Creates a slice of `array` excluding elements dropped from the end.
 * Elements are dropped until `rule` returns falsey. The rule is
 * invoked with three arguments: (value, index, array).
 *
 * @param array {array}: The array to query.
 * @param rule {function}: Function invoked per iteration; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the slice of `array`.
 *
 * @example `var users = [ { 'user': 'barney', 'active': true }, { 'user': 'fred', 'active': false }, { 'user': 'pebbles', 'active': false } ]; dropRightWhile(users, function(o) { return !o.active; }); // => objects for ['barney']`
 * @example `dropRightWhile(users, { 'user': 'pebbles', 'active': false }); // => objects for ['barney', 'fred']`
 * @example `dropRightWhile(users, ['active', false]); // => objects for ['barney']`
 * @example `dropRightWhile(users, 'active'); // => objects for ['barney', 'fred', 'pebbles']`
 */
export function dropRightWhile(array, rule) {
  if (false) { dropRightWhile(array, rule); }
  throw 'TODO: implement dropRightWhile';
}

// flipshop
/** `arr` with elements dropped from the beginning for as long as `rule` holds.
 *
 * @example `dropWhile([1, 2, 3, 4], (val) => val < 3); // => [3, 4]`
 */
// export function dropWhile(arr, rule) {}

// lodash
/** Creates a slice of `array` excluding elements dropped from the beginning.
 * Elements are dropped until `rule` returns falsey. The rule is
 * invoked with three arguments: (value, index, array).
 *
 * @param array {array}: The array to query.
 * @param rule {function}: Function invoked per iteration; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the slice of `array`.
 *
 * @example `var users = [ { 'user': 'barney', 'active': false }, { 'user': 'fred', 'active': false }, { 'user': 'pebbles', 'active': true } ]; dropWhile(users, function(o) { return !o.active; }); // => objects for ['pebbles']`
 * @example `dropWhile(users, { 'user': 'barney', 'active': false }); // => objects for ['fred', 'pebbles']`
 * @example `dropWhile(users, ['active', false]); // => objects for ['pebbles']`
 * @example `dropWhile(users, 'active'); // => objects for ['barney', 'fred', 'pebbles']`
 */
export function dropWhile(array, rule) {
  if (false) { dropWhile(array, rule); }
  throw 'TODO: implement dropWhile';
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
/** Performs a
 * [`SameValueZero`](http://ecma-international.org/ecma-262/7.0/#sec-samevaluezero)
 * comparison between two values to determine if they are equivalent.
 *
 * @param value: The value to compare.
 * @param other: The other value to compare.
 *
 * @returns {boolean}: `true` if the values are equivalent, else `false`.
 *
 * @example `var object = { 'a': 1 }; var other = { 'a': 1 }; eq(object, object); // => true`
 * @example `eq(object, other); // => false`
 * @example `eq('a', 'a'); // => true`
 * @example `eq('a', Object('a')); // => false`
 * @example `eq(NaN, NaN); // => true`
 */
export function eq(value, other) {
  if (false) { eq(value, other); }
  throw 'TODO: implement eq';
}

// lodash
/** Converts the characters "&", "<", ">", '"', and "'" in `string` to their
 * corresponding HTML entities.
 *
 * **Note:** No other characters are escaped. To escape additional
 * characters use a third-party library like [_he_](https://mths.be/he).
 *
 * Though the ">" character is escaped for symmetry, characters like
 * ">" and "/" don't need escaping in HTML and have no special meaning
 * unless they're part of a tag or unquoted attribute value. See
 * [Mathias Bynens's article](https://mathiasbynens.be/notes/ambiguous-ampersands)
 * (under "semi-related fun fact") for more details.
 *
 * When working with HTML you should always
 * [quote attribute values](http://wonko.com/post/html-escaping) to reduce
 * XSS vectors.
 *
 * @param string {string}: The string to escape; defaults to `''`.
 *   @optional
 *
 * @returns {string}: the escaped string.
 *
 * @example `escape('fred, barney, & pebbles'); // => 'fred, barney, &amp; pebbles'`
 */
export function escape(string) {
  if (false) { escape(string); }
  throw 'TODO: implement escape';
}

// flipshop
/** `str` with every regex metacharacter (`\ ^ $ . * + ? ( ) [ ] { } |`) preceded by a backslash,
 * so it can be dropped into `match`/`replace`/`splitByRegexp`'s `regExp` argument and matched
 * literally instead of interpreted.
 *
 * @example `escapeRegExp("[lodash](https://lodash.com/)"); // => "\\[lodash\\]\\(https://lodash\\.com/\\)"`
 */
// export function escapeRegExp(str) {}

// lodash
/** Escapes the `RegExp` special characters "^", "$", "\", ".", "*", "+",
 * "?", "(", ")", "[", "]", "{", "}", and "|" in `string`.
 *
 * @param string {string}: The string to escape; defaults to `''`.
 *   @optional
 *
 * @returns {string}: the escaped string.
 *
 * @example `escapeRegExp('[lodash](https://lodash.com/)'); // => '\[lodash\]\(https://lodash\.com/\)'`
 */
export function escapeRegExp(string) {
  if (false) { escapeRegExp(string); }
  throw 'TODO: implement escapeRegExp';
}

// lodash
/** Checks if `rule` returns truthy for **all** elements of `collection`.
 * Iteration is stopped once `rule` returns falsey. The rule is
 * invoked with three arguments: (value, index|key, collection).
 *
 * **Note:** This method returns `true` for
 * [empty collections](https://en.wikipedia.org/wiki/Empty_set) because
 * [everything is true](https://en.wikipedia.org/wiki/Vacuous_truth) of
 * elements of empty collections.
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param rule {function}: Function invoked per iteration; defaults to `identity`.
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
 * `rule` returns truthy for. The rule is invoked with three
 * arguments: (value, index|key, collection).
 *
 * **Note:** Unlike `remove`, this method returns a new array.
 *
 * @seeAlso [reject]
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param rule {function}: Function invoked per iteration; defaults to `identity`.
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
/** First element of `bag`/`arr` for which `rule` holds, or `undefined` if none does. The map form
 * hands `rule` the key as a second argument. `findLast` scans from the end instead. `rule` is
 * coerced through `funcOrProp` @see `funcOrProp`.
 *
 * @example `find([1, 2, 3], (val) => val > 1); // => 2`
 * @example `find({ a: 1, b: 2 }, (val, key) => key == "b"); // => 2`
 */
// export function find(arr, rule) {}

// lodash
/** Iterates over elements of `collection`, returning the first element
 * `rule` returns truthy for. The rule is invoked with three
 * arguments: (value, index|key, collection).
 *
 * @param collection {array|map}: The collection to inspect.
 * @param rule {function}: Function invoked per iteration; defaults to `identity`.
 *   @optional
 * @param fromIndex {number}: The index to search from; defaults to `0`.
 *   @optional
 *
 * @returns: the matched element, else `undefined`.
 *
 * @example `var users = [ { 'user': 'barney', 'age': 36, 'active': true }, { 'user': 'fred', 'age': 40, 'active': false }, { 'user': 'pebbles', 'age': 1, 'active': true } ]; find(users, function(o) { return o.age < 40; }); // => object for 'barney'`
 * @example `find(users, { 'age': 1, 'active': true }); // => object for 'pebbles'`
 * @example `find(users, ['active', false]); // => object for 'fred'`
 * @example `find(users, 'active'); // => object for 'barney'`
 */
export function find(collection, rule, fromIndex) {
  if (false) { find(collection, rule, fromIndex); }
  throw 'TODO: implement find';
}

// flipshop
/** Index of the first element of `arr` for which `rule` holds, or `-1` if none does.
 *
 * @example `findIndex([1, 2, 3], (val) => val > 1); // => 1`
 * @example `findIndex([1, 2, 3], (val) => val > 9); // => -1`
 */
// export function findIndex(arr, rule) {}

// lodash
/** Like `find` except that it returns the index of the first
 * element `rule` returns truthy for instead of the element itself.
 *
 * @param array {array}: The array to inspect.
 * @param rule {function}: Function invoked per iteration; defaults to `identity`.
 *   @optional
 * @param fromIndex {number}: The index to search from; defaults to `0`.
 *   @optional
 *
 * @returns {number}: the index of the found element, else `-1`.
 *
 * @example `var users = [ { 'user': 'barney', 'active': false }, { 'user': 'fred', 'active': false }, { 'user': 'pebbles', 'active': true } ]; findIndex(users, function(o) { return o.user == 'barney'; }); // => 0`
 * @example `findIndex(users, { 'user': 'fred', 'active': false }); // => 1`
 * @example `findIndex(users, ['active', false]); // => 0`
 * @example `findIndex(users, 'active'); // => 2`
 */
export function findIndex(array, rule, fromIndex) {
  if (false) { findIndex(array, rule, fromIndex); }
  throw 'TODO: implement findIndex';
}

// flipshop
/** First key of `bag` whose value satisfies `rule(val, key)`, or `undefined` if none does.
 * `findLastKey` scans in the reverse of `keys(bag)` order. `rule` is coerced through `funcOrProp`
 *
 * @example `findKey({ "a": 1, "b": 2, "c": 3 }, function(val, key) { return val > 1; }); // => "b"`
 */
// export function findKey(bag, rule) {}

// lodash
/** Like `find` except that it returns the key of the first
 * element `rule` returns truthy for instead of the element itself.
 *
 * @param object {map}: The object to inspect.
 * @param rule {function}: Function invoked per iteration; defaults to `identity`.
 *   @optional
 *
 * @returns {string|undefined}: the key of the matched element, else `undefined`.
 *
 * @example `var users = { 'barney': { 'age': 36, 'active': true }, 'fred': { 'age': 40, 'active': false }, 'pebbles': { 'age': 1, 'active': true } }; findKey(users, function(o) { return o.age < 40; }); // => 'barney' (iteration order is not guaranteed)`
 * @example `findKey(users, { 'age': 1, 'active': true }); // => 'pebbles'`
 * @example `findKey(users, ['active', false]); // => 'fred'`
 * @example `findKey(users, 'active'); // => 'barney'`
 */
export function findKey(object, rule) {
  if (false) { findKey(object, rule); }
  throw 'TODO: implement findKey';
}

// flipshop
/** /** `find`, scanning from the end. */
 */
// export function findLast(arr, rule) {}

// lodash
/** Like `find` except that it iterates over elements of
 * `collection` from right to left.
 *
 * @param collection {array|map}: The collection to inspect.
 * @param rule {function}: Function invoked per iteration; defaults to `identity`.
 *   @optional
 * @param fromIndex {number}: The index to search from; defaults to `collection.length-1`.
 *   @optional
 *
 * @returns: the matched element, else `undefined`.
 *
 * @example `findLast([1, 2, 3, 4], function(n) { return n % 2 == 1; }); // => 3`
 */
export function findLast(collection, rule, fromIndex) {
  if (false) { findLast(collection, rule, fromIndex); }
  throw 'TODO: implement findLast';
}

// flipshop
/** `findIndex`, scanning from the end: index of the last element of `arr` for which `rule`
 * holds, or `-1` if none does.
 *
 * @example `findLastIndex([1, 2, 3], (val) => val < 3); // => 1`
 */
// export function findLastIndex(arr, rule) {}

// lodash
/** Like `findIndex` except that it iterates over elements
 * of `collection` from right to left.
 *
 * @param array {array}: The array to inspect.
 * @param rule {function}: Function invoked per iteration; defaults to `identity`.
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
/** Last key of `bag` whose value satisfies `rule(val, key)`, or `undefined` if none does.
 * `findKey` scans in the reverse of `keys(bag)` order. `rule` is coerced through `funcOrProp`
 *
 * @example `findLastKey({ "a": 1, "b": 2, "c": 3 }, function(val, key) { return val > 1; }); // => "b"`
 */
// export function findLastKey(bag, rule) {}

// lodash
/** Like `findKey` except that it iterates over elements of
 * a collection in the opposite order.
 *
 * @param object {map}: The object to inspect.
 * @param rule {function}: Function invoked per iteration; defaults to `identity`.
 *   @optional
 *
 * @returns {string|undefined}: the key of the matched element, else `undefined`.
 *
 * @example `var users = { 'barney': { 'age': 36, 'active': true }, 'fred': { 'age': 40, 'active': false }, 'pebbles': { 'age': 1, 'active': true } }; findLastKey(users, function(o) { return o.age < 40; }); // => returns 'pebbles' assuming 'findKey' returns 'barney'`
 * @example `findLastKey(users, { 'age': 36, 'active': true }); // => 'barney'`
 * @example `findLastKey(users, ['active', false]); // => 'fred'`
 * @example `findLastKey(users, 'active'); // => 'pebbles'`
 */
export function findLastKey(object, rule) {
  if (false) { findLastKey(object, rule); }
  throw 'TODO: implement findLastKey';
}

// flipshop
/** `bag`/`arr` mapped through `iterateeSpec`, then flattened one level @see `flatten`. The map form
 * hands `iterateeSpec` the key as a second argument, and always returns an array, same as lodash's
 * collection form. `flatMapDeep`/`flatMapDepth` flatten fully / to `depth` levels instead.
 * `iterateeSpec` is coerced through `funcOrProp` @see `funcOrProp`.
 *
 * @example `flatMap([1, 2], (val) => [val, val]); // => [1, 1, 2, 2]`
 * @example `flatMap({ a: 1, b: 2 }, (val) => [val, val]); // => [1, 1, 2, 2]`
 */
// export function flatMap(arr, iterateeSpec) {}

// lodash
/** Creates a flattened array of values by running each element in `collection`
 * thru `funcOrProp` and flattening the mapped results. The function/propname is invoked
 * with three arguments: (value, index|key, collection).
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param funcOrProp {function|string|array}: iteratee invoked per iteration; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the new flattened array.
 *
 * @example `function duplicate(n) { return [n, n]; } flatMap([1, 2], duplicate); // => [1, 1, 2, 2]`
 */
export function flatMap(collection, iteratee) {
  if (false) { flatMap(collection, iteratee); }
  throw 'TODO: implement flatMap';
}

// lodash
/** Like `flatMap` except that it recursively flattens the
 * mapped results.
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param funcOrProp {function|string|array}: iteratee invoked per iteration; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the new flattened array.
 *
 * @example `function duplicate(n) { return [[[n, n]]]; } flatMapDeep([1, 2], duplicate); // => [1, 1, 2, 2]`
 */
export function flatMapDeep(collection, iteratee) {
  if (false) { flatMapDeep(collection, iteratee); }
  throw 'TODO: implement flatMapDeep';
}

// lodash
/** Like `flatMap` except that it recursively flattens the
 * mapped results up to `depth` times.
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param funcOrProp {function|string|array}: iteratee invoked per iteration; defaults to `identity`.
 *   @optional
 * @param depth {number}: The maximum recursion depth; defaults to `1`.
 *   @optional
 *
 * @returns {array}: the new flattened array.
 *
 * @example `function duplicate(n) { return [[[n, n]]]; } flatMapDepth([1, 2], duplicate, 2); // => [[1, 1], [2, 2]]`
 */
export function flatMapDepth(collection, iteratee, depth) {
  if (false) { flatMapDepth(collection, iteratee, depth); }
  throw 'TODO: implement flatMapDepth';
}

// flipshop
/** `arr` with one level of array nesting removed. @see `flattenDeep`, `flattenDepth`.
 *
 * @example `flatten([1, [2, [3, [4]], 5]]); // => [1, 2, [3, [4]], 5]`
 */
// export function flatten(arr) {}

// lodash
/** Flattens `array` a single level deep.
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
/** `arr`, with every level of array nesting removed.
 *
 * @example `flattenDeep([1, [2, [3, [4]], 5]]); // => [1, 2, 3, 4, 5]`
 */
// export function flattenDeep(arr) {}

// lodash
/** Recursively flattens `array`.
 *
 * @param array {array}: The array to flatten.
 *
 * @returns {array}: the new flattened array.
 *
 * @example `flattenDeep([1, [2, [3, [4]], 5]]); // => [1, 2, 3, 4, 5]`
 */
export function flattenDeep(array) {
  if (false) { flattenDeep(array); }
  throw 'TODO: implement flattenDeep';
}

// flipshop
/** `arr` with up to `depth` levels of array nesting removed; `depth <= 0` returns `arr` unchanged.
 *
 * @example `flattenDepth([1, [2, [3, [4]], 5]], 2); // => [1, 2, 3, [4], 5]`
 */
// export function flattenDepth(arr, depth) {}

// lodash
/** Recursively flatten `array` up to `depth` times.
 *
 * @param array {array}: The array to flatten.
 * @param depth {number}: The maximum recursion depth; defaults to `1`.
 *   @optional
 *
 * @returns {array}: the new flattened array.
 *
 * @example `var array = [1, [2, [3, [4]], 5]]; flattenDepth(array, 1); // => [1, 2, [3, [4]], 5]`
 * @example `flattenDepth(array, 2); // => [1, 2, 3, [4], 5]`
 */
export function flattenDepth(array, depth) {
  if (false) { flattenDepth(array, depth); }
  throw 'TODO: implement flattenDepth';
}

// lodash
/** Creates a function that invokes `func` with arguments reversed.
 *
 * @param func {function}: Function to flip arguments for.
 *
 * @returns {function}: the new flipped function.
 *
 * @example `var flipped = flip(function() { return toArray(arguments); }); flipped('a', 'b', 'c', 'd'); // => ['d', 'c', 'b', 'a']`
 */
export function flip(func) {
  if (false) { flip(func); }
  throw 'TODO: implement flip';
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

// lodash
/** Like `flow` except that it creates a function that
 * invokes the given functions from right to left.
 *
 * @seeAlso [flow]
 *
 * @param funcs {(Function|Function[])}: The functions to invoke.
 *   @optional
 *
 * @returns {function}: the new composite function.
 *
 * @example `function square(n) { return n * n; } var addSquare = flowRight([square, add]); addSquare(1, 2); // => 9`
 */
export function flowRight(funcs) {
  if (false) { flowRight(funcs); }
  throw 'TODO: implement flowRight';
}

// flipshop
/** Iterates over `bag`/`arr`,
 *
 * @example `forEach({ "a": 1, "b": 2, "c": 3 }, function(val, key) { if (key == "b") { return NextStepAction.BREAK; } }); // visits "a" then "b"; "c" is never reached`
 */

// lodash
/** Iterates over elements of `collection` and invokes `funcOrProp` for each element.
 * Return `NextStepAction.BREAK` from `func` to stop iteration early.
 *
 * @seeAlso [forEach]
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param funcOrProp {function|string|array}: iteratee invoked per iteration; defaults to `identity`.
 *   A function iteratee is invoked as `func(val, key is string)` (map) or `func(val, seq is number)` (array).
 *   @optional
 * @param missingPolicy {NilPolicy}:
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
 * @example `forEach3(["a", "b", "c"], (val, key, seq) => { println([val, key, seq]); }); // => Logs '[c, 0, 0]' then '[b, 1, 1]' then '[a, 2, 2]'.`
 * @example `forEach3({ a: 1, b: 2 }, (val, key, seq) => { println([val, key, seq]); }); // => Logs '[1, "a", 0]' then '[2, "b", 1]'.`
 */
// export function forEach(bag, keylist, missingPolicy, func) {}

// flipshop
/** Like `forEach` except that it calls a function iteratee with three arguments:
 * (val, key, seq) for a map, or (val, seq, seq) for an array.
 * Return `NextStepAction.BREAK` from `func` to stop iteration early.
 *
 * @seeAlso [forEach]
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param funcOrProp {function|string|array}: iteratee invoked per iteration; defaults to `identity`.
 *   A function iteratee is invoked as `func(val, key is string, seq is number)` (map)
 *   or `func(val, seq is number, seq is number)` (array).
 *   @optional
 * @param missingPolicy {NilPolicy}: The policy to use for missing values; defaults to `NilPolicy.NIL`.
 *   @optional
 * @param keylist {array}: The keys, in order, to iterate over;
 *   keys are allowed to be repeated, absent, or out-of-bounds for the collection.
 *   @optional
 *
 * @returns {array|map}: `collection`.
 *
 * @example `forEach3(["a", "b", "c"], (val, key, seq) => { println([val, key, seq]); }); // => Logs '[c, 0, 0]' then '[b, 1, 1]' then '[a, 2, 2]'.`
 * @example `forEach3({ a: 1, b: 2 }, (val, key, seq) => { println([val, key, seq]); }); // => Logs '[1, "a", 0]' then '[2, "b", 1]'.`
 */
// export function forEach3(bag, keylist, missingPolicy, func) {}

// flipshop
/** Like `forEach` except that it iterates over elements of
 * `collection` from right to left.
 * Return `NextStepAction.BREAK` from `func` to stop iteration early.
 *
 * @seeAlso [forEach]
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param funcOrProp {function|string|array}: iteratee invoked per iteration; defaults to `identity`.
 *   A function iteratee is invoked as `func(val, key is string, seq is number)` (map)
 *   or `func(val, seq is number, seq is number)` (array).
 *   @optional
 * @param missingPolicy {NilPolicy}: The policy to use for missing values; defaults to `NilPolicy.NIL`.
 *   @optional
 * @param keylist {array}: The keys, in order, to iterate over;
 *   keys are allowed to be repeated, absent, or out-of-bounds for the collection.
 *   @optional
 *
 * @returns {array|map}: `collection`.
 *
 * @example `forEachRight(["a", "b", "c"], (val, seq) => { println([val, seq]); }); // => Logs '[c, 0]' then '[b, 1]' then '[a, 2]'.`
 * @example `forEachRight({ a: 1, b: 2 }, (val, key) => { println([val, key]); }); // => Logs '[2, "b"]' then '[1, "a"]'.`
 */
// export function forEachRight(collection, iteratee, missingPolicy, func) {}

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
 * @param path {array|string}: The path of the property to get:
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
/** Creates an object composed of keys generated from the results of running
 * each element of `collection` thru `funcOrProp`. The order of grouped values
 * is determined by the order they occur in `collection`. The corresponding
 * value of each key is an array of elements responsible for generating the
 * key. A function iteratee is invoked with (val, seq).
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param funcOrProp {function}: Function/propname to transform keys; defaults to `identity`.
 *   @optional
 *
 * @returns {map}: the composed aggregate object.
 *
 * @example `groupBy([1, 2, 3, 4], (val, _seq) => (val % 2 == 0) ? "even" : "odd"); // => { "odd": [1, 3], "even": [2, 4] }`
 * @example `groupBy([6.1, 4.2, 6.3], Math.floor); // => { '4': [4.2], '6': [6.1, 6.3] }`
 * @example `groupBy(['one', 'two', 'three'], 'length'); // => { '3': ['one', 'two'], '5': ['three'] }`
 */
// export function groupBy(arr, iterateeSpec) {}

// lodash
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

// lodash
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

// lodash
/** Checks if `path` is a direct property of `object`.
 *
 * @param object {map}: The object to query.
 * @param path {array|string}: The path to check.
 *
 * @returns {boolean}: `true` if `path` exists, else `false`.
 *
 * @example `var object = { 'a': { 'b': 2 } }; var other = create({ 'a': create({ 'b': 2 }) }); has(object, 'a'); // => true`
 * @example `has(object, 'a.b'); // => true`
 * @example `has(object, ['a', 'b']); // => true`
 * @example `has(other, 'a'); // => false`
 */
export function has(object, path) {
  if (false) { has(object, path); }
  throw 'TODO: implement has';
}

// lodash
/** Checks if `path` is a direct or inherited property of `object`.
 *
 * @param object {map}: The object to query.
 * @param path {array|string}: The path to check.
 *
 * @returns {boolean}: `true` if `path` exists, else `false`.
 *
 * @example `var object = create({ 'a': create({ 'b': 2 }) }); hasIn(object, 'a'); // => true`
 * @example `hasIn(object, 'a.b'); // => true`
 * @example `hasIn(object, ['a', 'b']); // => true`
 * @example `hasIn(object, 'b'); // => false`
 */
export function hasIn(object, path) {
  if (false) { hasIn(object, path); }
  throw 'TODO: implement hasIn';
}

// flipshop
/** Whether `key` is present in `obj`. Unlike lodash's `has`, `key` is a single literal key or
 * index -- never a dotted path -- and a map only ever contains `key` when its value isn't
 * `undefined`, since FeatureScript elides one on the way in.
 *
 * For an array, `missingPolicy` decides whether an in-bounds slot holding `undefined` counts:
 * `NilPolicy.NIL` (the default) says yes; `NilPolicy.SKIP` says no, the same as
 * `hasPresentKey`. Neither array overload accepts a negative index, unlike @see `getAt`.
 *
 * `hasPresentKey` is `hasKey` pinned to the stricter policy.
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
/** Gets the first element of `array`.
 *
 * @param array {array}: The array to query.
 *
 * @returns: the first element of `array`.
 *
 * @example `head([1, 2, 3]); // => 1`
 * @example `head([]); // => undefined`
 */
export function head(array) {
  if (false) { head(array); }
  throw 'TODO: implement head';
}

// flipshop
/** Parses a 6- or 8-digit hex color string (`"#rrggbb"` or `"#rrggbbaa"`, leading `#` optional,
 * either case) into a `Color`. Missing `aa` defaults to fully opaque. Returns `OopsColor`
 * (bright red) on anything that doesn't match, rather than throwing.
 *
 * @param hexcolor {string}
 *
 * @returns {{
 *    @field red {number}
 *    @field green {number}
 *    @field blue {number}
 *    @field alpha {number}
 * }}
 */
// export function hexcolorToColor(hexcolor) {}

// flipshop
/** Decimal value of a 2-character hex string (case-insensitive), or `fallback` (default
 * `undefined`) if `hexpair` isn't a valid 2-digit hex pair.
 *
 * @example `hexpairToInt("ff"); // => 255`
 * @example `hexpairToInt("FF"); // => 255`
 */
// export function hexpairToInt(hexpair, fallback) {}

// flipshop
/** Highlights `qq` in the viewport via `addDebugEntities`, plus the edges of `qq`'s owning
 * bodies -- a body is otherwise invisible through the faces of any body occluding it. A no-op
 * unless `debugMe` is true (default), so a call site can leave this in place and flip one flag
 * rather than comment the call out.
 *
 * @param context {Context}
 * @param qq {Query}: Entities to highlight.
 * @param debugColor {DebugColor}
 * @param debugMe {boolean}: Set false to silence this call without removing it. Defaults to `true`.
 */
// export function highlightQuery(context, qq, debugColor, debugMe) {}

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
/** Map of `tags` to a same-named child of `id`: `idsFor(id, ["a", "b"])` is
 * `{ "a": id + "a", "b": id + "b" }` -- the `ids` map every multi-sketch/multi-op feature
 * declares up front, built in one call instead of one line per key.
 *
 * @param id {Id}: Base id.
 * @param tags {array}: Id-suffix strings, one per key.
 */
// export function idsFor(id, tags) {}

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
 * using [`SameValueZero`](http://ecma-international.org/ecma-262/7.0/#sec-samevaluezero)
 * for equality comparisons. If `fromIndex` is negative, it's used as the
 * offset from the end of `array`.
 *
 * @param array {array}: The array to inspect.
 * @param value: The value to search for.
 * @param fromIndex {number}: The index to search from; defaults to `0`.
 *   @optional
 *
 * @returns {number}: the index of the matched value, else `-1`.
 *
 * @example `indexOf([1, 2, 1, 2], 2); // => 1`
 * @example `indexOf([1, 2, 1, 2], 2, 2); // => 3`
 */
export function indexOf(array, value, fromIndex) {
  if (false) { indexOf(array, value, fromIndex); }
  throw 'TODO: implement indexOf';
}

// flipshop
/** `arr` without its last element; `[]` for an empty or single-element `arr`.
 *
 * @example `initial([1, 2, 3]); // => [1, 2]`
 */
// export function initial(arr) {}

// lodash
/** Gets all but the last element of `array`.
 *
 * @param array {array}: The array to query.
 *
 * @returns {array}: the slice of `array`.
 *
 * @example `initial([1, 2, 3]); // => [1, 2]`
 */
export function initial(array) {
  if (false) { initial(array); }
  throw 'TODO: implement initial';
}

// flipshop
/** Whether `num` falls in `[start, end)` (or `[end, start)` if `end < start`). If
 * `end` is not specified, it's set to `start` with `start` then set to `0`.
 * If `start` is greater than `end` the params are swapped to support
 * negative ranges.
 *
 * @param number {number}: The number to check.
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

// lodash
/** Checks if `n` is between `start` and up to, but not including, `end`. If
 * `end` is not specified, it's set to `start` with `start` then set to `0`.
 * If `start` is greater than `end` the params are swapped to support
 * negative ranges.
 *
 * @seeAlso [range]
 * @seeAlso [rangeRight]
 *
 * @param number {number}: The number to check.
 * @param start {number}: The start of the range; defaults to `0`.
 *   @optional
 * @param end {number}: The end of the range.
 *
 * @returns {boolean}: `true` if `number` is in the range, else `false`.
 *
 * @example `inRange(3, 2, 4); // => true`
 * @example `inRange(4, 8); // => true`
 * @example `inRange(4, 2); // => false`
 * @example `inRange(2, 2); // => false`
 * @example `inRange(1.2, 2); // => true`
 * @example `inRange(5.2, 4); // => false`
 * @example `inRange(-3, -2, -6); // => true`
 */
export function inRange(number, start, end) {
  if (false) { inRange(number, start, end); }
  throw 'TODO: implement inRange';
}

// flipshop
/** Values present in every array of `arrList`, deduplicated, ordered as they occur in
 * `arrList[0]`.
 *
 * @example `intersection([[2, 1], [2, 3], [1, 2]]); // => [2]`
 */
// export function intersection(arrList) {}

// lodash
/** Creates an array of unique values that are included in all given arrays
 * using [`SameValueZero`](http://ecma-international.org/ecma-262/7.0/#sec-samevaluezero)
 * for equality comparisons. The order and references of result values are
 * determined by the first array.
 *
 * @param arrays {array}: The arrays to inspect.
 *   @optional
 *
 * @returns {array}: the new array of intersecting values.
 *
 * @example `intersection([2, 1], [2, 3]); // => [2]`
 */
export function intersection(arrays) {
  if (false) { intersection(arrays); }
  throw 'TODO: implement intersection';
}

// flipshop
/** `intersection`, comparing elements by `iterateeSpec(val, seq)` instead of `val` itself.
 * `iterateeSpec` is coerced through `funcOrProp` @see `funcOrProp`.
 *
 * @example `intersectionBy([[2.1, 1.2], [2.3, 3.4]], (val, _seq) => floor(val)); // => [2.1]`
 */
// export function intersectionBy(arrList, iterateeSpec) {}

// lodash
/** Like `intersection` except that it accepts `funcOrProp`
 * which is invoked for each element of each `arrays` to generate the criterion
 * by which they're compared. The order and references of result values are
 * determined by the first array. The function/propname is invoked with one argument:
 * (value).
 *
 * @param arrays {array}: The arrays to inspect.
 *   @optional
 * @param funcOrProp {function}: Function/propname invoked per element; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the new array of intersecting values.
 *
 * @example `intersectionBy([2.1, 1.2], [2.3, 3.4], Math.floor); // => [2.1]`
 * @example `intersectionBy([{ 'x': 1 }], [{ 'x': 2 }, { 'x': 1 }], 'x'); // => [{ 'x': 1 }]`
 */
export function intersectionBy(arrays, iteratee) {
  if (false) { intersectionBy(arrays, iteratee); }
  throw 'TODO: implement intersectionBy';
}

// flipshop
/** `intersection`, comparing elements with `comparator(val, other)` instead of `==`.
 *
 * @example `intersectionWith([[{ "x": 1 }, { "x": 2 }], [{ "x": 2 }]], (aa, bb) => aa.x == bb.x);`
 * @example `// => [{ "x": 2 }]`
 */
// export function intersectionWith(arrList, comparator) {}

// lodash
/** Like `intersection` except that it accepts `comparator`
 * which is invoked to compare elements of `arrays`. The order and references
 * of result values are determined by the first array. The comparator is
 * invoked with two arguments: (arrVal, othVal).
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
/** /** Lowercase 2-character hex string for `num`, a 0–255 integer. */
 */
// export function intToHexpair(num) {}

// flipshop
/** `bag` with its keys and values swapped: `{"a": "x", "b": "x"}` → `{"x": "b"}` -- a value that
 * occurs more than once keeps only its last key, same as lodash. A non-string value is
 * stringified into its new key, matching how lodash's own object keys coerce. `invertBy` collects
 * every key instead of just the last one, grouped under `iterateeSpec(val, key)` rather than
 * `val` itself. `iterateeSpec` is coerced through `funcOrProp` @see `funcOrProp`.
 *
 * @example `invert({ "a": 1, "b": 2, "c": 1 }); // => { "1": "c", "2": "b" }`
 */
// export function invert(bag) {}

// lodash
/** Creates an object composed of the inverted keys and values of `object`.
 * If `object` contains duplicate values, subsequent values overwrite
 * property assignments of previous values.
 *
 * @param object {map}: The object to invert.
 *
 * @returns {map}: the new inverted object.
 *
 * @example `var object = { 'a': 1, 'b': 2, 'c': 1 }; invert(object); // => { '1': 'c', '2': 'b' }`
 */
export function invert(object) {
  if (false) { invert(object); }
  throw 'TODO: implement invert';
}

// lodash
/** Like `invert` except that the inverted object is generated
 * from the results of running each element of `object` thru `funcOrProp`. The
 * corresponding inverted value of each inverted key is an array of keys
 * responsible for generating the inverted value. The function/propname is invoked
 * with one argument: (value).
 *
 * @param object {map}: The object to invert.
 * @param funcOrProp {function}: Function/propname invoked per element; defaults to `identity`.
 *   @optional
 *
 * @returns {map}: the new inverted object.
 *
 * @example `var object = { 'a': 1, 'b': 2, 'c': 1 }; invertBy(object); // => { '1': ['a', 'c'], '2': ['b'] }`
 * @example `invertBy(object, function(value) { return 'group' + value; }); // => { 'group1': ['a', 'c'], 'group2': ['b'] }`
 */
export function invertBy(object, iteratee) {
  if (false) { invertBy(object, iteratee); }
  throw 'TODO: implement invertBy';
}

// lodash
/** Invokes the method at `path` of `object`.
 *
 * @param object {map}: The object to query.
 * @param path {array|string}: The path of the method to invoke.
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
/** Checks if `value` is likely an `arguments` object.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is an `arguments` object, else `false`.
 *
 * @example `isArguments(function() { return arguments; }()); // => true`
 * @example `isArguments([1, 2, 3]); // => false`
 */
export function isArguments(value) {
  if (false) { isArguments(value); }
  throw 'TODO: implement isArguments';
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
/** Checks if `value` is classified as an `ArrayBuffer` object.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is an array buffer, else `false`.
 *
 * @example `isArrayBuffer(new ArrayBuffer(2)); // => true`
 * @example `isArrayBuffer(new Array(2)); // => false`
 */
export function isArrayBuffer(value) {
  if (false) { isArrayBuffer(value); }
  throw 'TODO: implement isArrayBuffer';
}

// lodash
/** Checks if `value` is array-like. A value is considered array-like if it's
 * not a function and has a `value.length` that's an integer greater than or
 * equal to `0` and less than or equal to `Number.MAX_SAFE_INTEGER`.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is array-like, else `false`.
 *
 * @example `isArrayLike([1, 2, 3]); // => true`
 * @example `isArrayLike(document.body.children); // => true`
 * @example `isArrayLike('abc'); // => true`
 * @example `isArrayLike(noop); // => false`
 */
export function isArrayLike(value) {
  if (false) { isArrayLike(value); }
  throw 'TODO: implement isArrayLike';
}

// lodash
/** Like `isArrayLike` except that it also checks if `value`
 * is an object.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is an array-like object, else `false`.
 *
 * @example `isArrayLikeObject([1, 2, 3]); // => true`
 * @example `isArrayLikeObject(document.body.children); // => true`
 * @example `isArrayLikeObject('abc'); // => false`
 * @example `isArrayLikeObject(noop); // => false`
 */
export function isArrayLikeObject(value) {
  if (false) { isArrayLikeObject(value); }
  throw 'TODO: implement isArrayLikeObject';
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
/** Checks if `value` is a buffer.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is a buffer, else `false`.
 *
 * @example `isBuffer(new Buffer(2)); // => true`
 * @example `isBuffer(new Uint8Array(2)); // => false`
 */
export function isBuffer(value) {
  if (false) { isBuffer(value); }
  throw 'TODO: implement isBuffer';
}

// lodash
/** Checks if `value` is classified as a `Date` object.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is a date object, else `false`.
 *
 * @example `isDate(new Date); // => true`
 * @example `isDate('Mon April 23 2012'); // => false`
 */
export function isDate(value) {
  if (false) { isDate(value); }
  throw 'TODO: implement isDate';
}

// lodash
/** Checks if `value` is likely a DOM element.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is a DOM element, else `false`.
 *
 * @example `isElement(document.body); // => true`
 * @example `isElement('<body>'); // => false`
 */
export function isElement(value) {
  if (false) { isElement(value); }
  throw 'TODO: implement isElement';
}

// lodash
/** Checks if `value` is an empty object, collection, map, or set.
 *
 * Objects are considered empty if they have no own enumerable string keyed
 * properties.
 *
 * Array-like values such as `arguments` objects, arrays, buffers, strings, or
 * jQuery-like collections are considered empty if they have a `length` of `0`.
 * Similarly, maps and sets are considered empty if they have a `size` of `0`.
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
 */
export function isEmpty(value) {
  if (false) { isEmpty(value); }
  throw 'TODO: implement isEmpty';
}

// lodash
/** Performs a deep comparison between two values to determine if they are
 * equivalent.
 *
 * **Note:** This method supports comparing arrays, array buffers, booleans,
 * date objects, error objects, maps, numbers, `Object` objects, regexes,
 * sets, strings, symbols, and typed arrays. `Object` objects are compared
 * by their own, not inherited, enumerable properties. Functions and DOM
 * nodes are compared by strict equality, i.e. `===`.
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
 * six arguments: (objValue, othValue [, index|key, object, other, stack]).
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
/** Checks if `value` is an `Error`, `EvalError`, `RangeError`, `ReferenceError`,
 * `SyntaxError`, `TypeError`, or `URIError` object.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is an error object, else `false`.
 *
 * @example `isError(new Error); // => true`
 * @example `isError(Error); // => false`
 */
export function isError(value) {
  if (false) { isError(value); }
  throw 'TODO: implement isError';
}

// lodash
/** Checks if `value` is a finite primitive number.
 *
 * **Note:** This method is based on
 * [`Number.isFinite`](https://mdn.io/Number/isFinite).
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

// flipshop
/** /** Whether `str` is a hexcolor string @see `hexcolorToColor` would accept. */
 */
// export function isHexcolor(str) {}

// lodash
/** Checks if `value` is an integer.
 *
 * **Note:** This method is based on
 * [`Number.isInteger`](https://mdn.io/Number/isInteger).
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
/** Checks if `value` is a valid array-like length.
 *
 * **Note:** This method is loosely based on
 * [`ToLength`](http://ecma-international.org/ecma-262/7.0/#sec-tolength).
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is a valid length, else `false`.
 *
 * @example `isLength(3); // => true`
 * @example `isLength(Number.MIN_VALUE); // => false`
 * @example `isLength(Infinity); // => false`
 * @example `isLength('3'); // => false`
 */
export function isLength(value) {
  if (false) { isLength(value); }
  throw 'TODO: implement isLength';
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
 * determine if `object` contains equivalent property values.
 *
 * **Note:** This method is equivalent to `matches` when `source` is
 * partially applied.
 *
 * Partial comparisons will match empty array and empty object `source`
 * values against any array or object value, respectively. See `isEqual`
 * for a list of supported value comparisons.
 *
 * @param object {map}: The object to inspect.
 * @param source {map}: The object of property values to match.
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
 * arguments: (objValue, srcValue, index|key, object, source).
 *
 * @param object {map}: The object to inspect.
 * @param source {map}: The object of property values to match.
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
/** Checks if `value` is a pristine native function.
 *
 * **Note:** This method can't reliably detect native functions in the presence
 * of the core-js package because core-js circumvents this kind of detection.
 * Despite multiple requests, the core-js maintainer has made it clear: any
 * attempt to fix the detection will be obstructed. As a result, we're left
 * with little choice but to throw an error. Unfortunately, this also affects
 * packages, like [babel-polyfill](https://www.npmjs.com/package/babel-polyfill),
 * which rely on core-js.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is a native function, else `false`.
 *
 * @example `isNative(Array.prototype.push); // => true`
 * @example `isNative(_); // => false`
 */
export function isNative(value) {
  if (false) { isNative(value); }
  throw 'TODO: implement isNative';
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
/** Checks if `value` is `null`.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is `null`, else `false`.
 *
 * @example `isNull(null); // => true`
 * @example `isNull(void 0); // => false`
 */
export function isNull(value) {
  if (false) { isNull(value); }
  throw 'TODO: implement isNull';
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
/** Checks if `value` is the
 * [language type](http://www.ecma-international.org/ecma-262/7.0/#sec-ecmascript-language-types)
 * of `Object`. (e.g. arrays, functions, objects, regexes, `new Number(0)`, and `new String('')`)
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is an object, else `false`.
 *
 * @example `isObject({}); // => true`
 * @example `isObject([1, 2, 3]); // => true`
 * @example `isObject(noop); // => true`
 * @example `isObject(null); // => false`
 */
export function isObject(value) {
  if (false) { isObject(value); }
  throw 'TODO: implement isObject';
}

// lodash
/** Checks if `value` is object-like. A value is object-like if it's not `null`
 * and has a `typeof` result of "object".
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is object-like, else `false`.
 *
 * @example `isObjectLike({}); // => true`
 * @example `isObjectLike([1, 2, 3]); // => true`
 * @example `isObjectLike(noop); // => false`
 * @example `isObjectLike(null); // => false`
 */
export function isObjectLike(value) {
  if (false) { isObjectLike(value); }
  throw 'TODO: implement isObjectLike';
}

// lodash
/** Checks if `value` is a plain object, that is, an object created by the
 * `Object` constructor or one with a `[[Prototype]]` of `null`.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is a plain object, else `false`.
 *
 * @example `function Foo() { this.a = 1; } isPlainObject(new Foo); // => false`
 * @example `isPlainObject([1, 2, 3]); // => false`
 * @example `isPlainObject({ 'x': 0, 'y': 0 }); // => true`
 * @example `isPlainObject(Object.create(null)); // => true`
 */
export function isPlainObject(value) {
  if (false) { isPlainObject(value); }
  throw 'TODO: implement isPlainObject';
}

// lodash
/** Checks if `value` is classified as a `RegExp` object.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is a regexp, else `false`.
 *
 * @example `isRegExp(/abc/); // => true`
 * @example `isRegExp('/abc/'); // => false`
 */
export function isRegExp(value) {
  if (false) { isRegExp(value); }
  throw 'TODO: implement isRegExp';
}

// lodash
/** Checks if `value` is a safe integer. An integer is safe if it's an IEEE-754
 * double precision number which isn't the result of a rounded unsafe integer.
 *
 * **Note:** This method is based on
 * [`Number.isSafeInteger`](https://mdn.io/Number/isSafeInteger).
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is a safe integer, else `false`.
 *
 * @example `isSafeInteger(3); // => true`
 * @example `isSafeInteger(Number.MIN_VALUE); // => false`
 * @example `isSafeInteger(Infinity); // => false`
 * @example `isSafeInteger('3'); // => false`
 */
export function isSafeInteger(value) {
  if (false) { isSafeInteger(value); }
  throw 'TODO: implement isSafeInteger';
}

// lodash
/** Checks if `value` is classified as a `Set` object.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is a set, else `false`.
 *
 * @example `isSet(new Set); // => true`
 * @example `isSet(new WeakSet); // => false`
 */
export function isSet(value) {
  if (false) { isSet(value); }
  throw 'TODO: implement isSet';
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
/** Checks if `value` is classified as a `Symbol` primitive or object.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is a symbol, else `false`.
 *
 * @example `isSymbol(Symbol.iterator); // => true`
 * @example `isSymbol('abc'); // => false`
 */
export function isSymbol(value) {
  if (false) { isSymbol(value); }
  throw 'TODO: implement isSymbol';
}

// lodash
/** Checks if `value` is classified as a typed array.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is a typed array, else `false`.
 *
 * @example `isTypedArray(new Uint8Array); // => true`
 * @example `isTypedArray([]); // => false`
 */
export function isTypedArray(value) {
  if (false) { isTypedArray(value); }
  throw 'TODO: implement isTypedArray';
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
/** Checks if `value` is classified as a `WeakMap` object.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is a weak map, else `false`.
 *
 * @example `isWeakMap(new WeakMap); // => true`
 * @example `isWeakMap(new Map); // => false`
 */
export function isWeakMap(value) {
  if (false) { isWeakMap(value); }
  throw 'TODO: implement isWeakMap';
}

// lodash
/** Checks if `value` is classified as a `WeakSet` object.
 *
 * @param value: The value to check.
 *
 * @returns {boolean}: `true` if `value` is a weak set, else `false`.
 *
 * @example `isWeakSet(new WeakSet); // => true`
 * @example `isWeakSet(new Set); // => false`
 */
export function isWeakSet(value) {
  if (false) { isWeakSet(value); }
  throw 'TODO: implement isWeakSet';
}

// lodash
/** Creates a function that invokes `func` with the arguments of the created
 * function. If `func` is a property name, the created function returns the
 * property value for a given element. If `func` is an array or object, the
 * created function returns `true` for elements that contain the equivalent
 * source properties, otherwise it returns `false`.
 *
 * @param func: The value to convert to a callback; defaults to `identity`.
 *   @optional
 *
 * @returns {function}: the callback.
 *
 * @example `var users = [ { 'user': 'barney', 'age': 36, 'active': true }, { 'user': 'fred', 'age': 40, 'active': false } ]; filter(users, iteratee({ 'user': 'barney', 'active': true })); // => [{ 'user': 'barney', 'age': 36, 'active': true }]`
 * @example `filter(users, iteratee(['user', 'fred'])); // => [{ 'user': 'fred', 'age': 40 }]`
 * @example `map(users, iteratee('user')); // => ['barney', 'fred']`
 * @example `iteratee = wrap(iteratee, function(iteratee, func) { return !isRegExp(func) ? iteratee(func) : function(string) { return func.test(string); }; }); filter(['abc', 'def'], /ef/); // => ['def']`
 */
export function iteratee(func) {
  if (false) { iteratee(func); }
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
/** Creates an object composed of keys generated from the results of running
 * each element of `collection` thru `funcOrProp`. The corresponding value of
 * each key is the last element responsible for generating the key. A function
 * iteratee is invoked as func(val, seq)
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param funcOrProp {function}: Function/propname to transform keys; defaults to `identity`.
 *   @optional
 *
 * @returns {map}: the composed aggregate object.
 *
 * @example `var array = [ { 'dir': 'left', 'code': 97 }, { 'dir': 'right', 'code': 100 } ]; keyBy(array, function(o) { return String.fromCharCode(o.code); }); // => { 'a': { 'dir': 'left', 'code': 97 }, 'd': { 'dir': 'right', 'code': 100 } }`
 * @example `keyBy(array, 'dir'); // => { 'left': { 'dir': 'left', 'code': 97 }, 'right': { 'dir': 'right', 'code': 100 } }`
 */
export function keyBy(collection, iteratee) {
  if (false) { keyBy(collection, iteratee); }
  throw 'TODO: implement keyBy';
}

// flipshop
/** /** Keeps `varname` at `bagname ~ "_keys"` for as long as it hasn't been hand-edited; @see `defaultMaybe`. */
 */
// export function keylistEditLogic(context, id, oldDefinition, newDefinition, isCreating, specifiedParameters) {}

// lodash
/** Creates an array of the own enumerable property names of `object`.
 *
 * **Note:** Non-object values are coerced to objects. See the
 * [ES spec](http://ecma-international.org/ecma-262/7.0/#sec-object.keys)
 * for more details.
 *
 * @param object {map}: The object to query.
 *
 * @returns {array}: the array of property names.
 *
 * @example `function Foo() { this.a = 1; this.b = 2; } Foo.prototype.c = 3; keys(new Foo); // => ['a', 'b'] (iteration order is not guaranteed)`
 * @example `keys('hi'); // => ['0', '1']`
 */
export function keys(object) {
  if (false) { keys(object); }
  throw 'TODO: implement keys';
}

// lodash
/** Creates an array of the own and inherited enumerable property names of `object`.
 *
 * **Note:** Non-object values are coerced to objects.
 *
 * @param object {map}: The object to query.
 *
 * @returns {array}: the array of property names.
 *
 * @example `function Foo() { this.a = 1; this.b = 2; } Foo.prototype.c = 3; keysIn(new Foo); // => ['a', 'b', 'c'] (iteration order is not guaranteed)`
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
/** Creates an array of values by running each element in `collection` thru
 * `funcOrProp`. The function/propname is invoked with three arguments:
 * (value, index|key, collection).
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
 * @param funcOrProp {function|string|array}: iteratee invoked per iteration; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the new mapped array.
 *
 * @example `function square(n) { return n * n; } map([4, 8], square); // => [16, 64]`
 * @example `map({ 'a': 4, 'b': 8 }, square); // => [16, 64] (iteration order is not guaranteed)`
 * @example `var users = [ { 'user': 'barney' }, { 'user': 'fred' } ]; map(users, 'user'); // => ['barney', 'fred']`
 */
export function map(collection, iteratee) {
  if (false) { map(collection, iteratee); }
  throw 'TODO: implement map';
}

// flipshop
/** `bag`'s values, replacing each key with `iterateeSpec(val, key)` -- `mapValues`' sibling for
 * keys instead of values. A collision on the computed key keeps the last entry that produced it.
 * `iterateeSpec` is coerced through `funcOrProp` @see `funcOrProp`.
 *
 * @example `mapKeys({ "a": 1, "b": 2 }, function(val, key) { return key ~ val; }); // => { "a1": 1, "b2": 2 }`
 */
// export function mapKeys(bag, iterateeSpec) {}

// lodash
/** The opposite of `mapValues`; this method creates an object with the
 * same values as `object` and keys generated by running each own enumerable
 * string keyed property of `object` thru `funcOrProp`. The function/propname is invoked
 * with three arguments: (value, key, object).
 *
 * @seeAlso [mapValues]
 *
 * @param object {map}: The object to iterate over.
 * @param funcOrProp {function|string|array}: iteratee invoked per iteration; defaults to `identity`.
 *   @optional
 *
 * @returns {map}: the new mapped object.
 *
 * @example `mapKeys({ 'a': 1, 'b': 2 }, function(value, key) { return key + value; }); // => { 'a1': 1, 'b2': 2 }`
 */
export function mapKeys(object, iteratee) {
  if (false) { mapKeys(object, iteratee); }
  throw 'TODO: implement mapKeys';
}

// flipshop
/** `bag`/`arr` with every value replaced by `func`'s result. Lodash splits this into two
 * functions -- `mapValues` keeps an object's keys, `map` returns a new array -- unified here under
 * one dispatch: `func` is `func(val, key)` for a map or `func(val, seq)` for an array.
 * `mapValues3` instead hands `func` all three of `val`, `key`, and a 0-based visit-count `seq`
 * (for an array, `seq` fills both slots) when a map form needs to distinguish visit order from
 * key order. The `keylist` and `missingPolicy` overloads follow `forEach`'s rules: a `keylist`
 * walks exactly those keys, and `missingPolicy` decides whether an `undefined` value is mapped
 * (`NIL`, the default) or its key dropped from the result entirely (`SKIP`). `func` is
 * coerced through `funcOrProp` @see `funcOrProp` -- `mapValues(users, 'name')` extracts a `name`
 * field from each.
 *
 * @example `mapValues({ "fred": 40, "pebbles": 1 }, function(age) { return age * 2; }); // => { "fred": 80, "pebbles": 2 }`
 * @example `mapValues([4, 8], function(n) { return n * n; }); // => [16, 64]`
 */
// export function mapValues(bag, keylist, missingPolicy, func) {}

// lodash
/** Creates an object with the same keys as `object` and values generated
 * by running each own enumerable string keyed property of `object` thru
 * `funcOrProp`. The function/propname is invoked with three arguments:
 * (value, key, object).
 *
 * @seeAlso [mapKeys]
 *
 * @param object {map}: The object to iterate over.
 * @param funcOrProp {function|string|array}: iteratee invoked per iteration; defaults to `identity`.
 *   @optional
 *
 * @returns {map}: the new mapped object.
 *
 * @example `var users = { 'fred': { 'user': 'fred', 'age': 40 }, 'pebbles': { 'user': 'pebbles', 'age': 1 } }; mapValues(users, function(o) { return o.age; }); // => { 'fred': 40, 'pebbles': 1 } (iteration order is not guaranteed)`
 * @example `mapValues(users, 'age'); // => { 'fred': 40, 'pebbles': 1 } (iteration order is not guaranteed)`
 */
export function mapValues(object, iteratee) {
  if (false) { mapValues(object, iteratee); }
  throw 'TODO: implement mapValues';
}

// flipshop
/** Builds a rule that's `true` for any map holding `source`'s entries -- a partial deep match.
 * `(obj, seq)` callback shape, discarding `seq` @see `conforms`.
 *
 * @example `matches({ "a": 1 })({ "a": 1, "b": 2 }, 0); // => true`
 * @example `matches({ "a": 1 })({ "a": 2, "b": 2 }, 0); // => false`
 */
// export function matches(source) {}

// lodash
/** Creates a function that performs a partial deep comparison between a given
 * object and `source`, returning `true` if the given object has equivalent
 * property values, else `false`.
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
 * @param source {map}: The object of property values to match.
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
/** Builds a rule that's `true` when `path` of a given object equals `srcValue`; `path` can be a
 * string/dotpath/pathlist @see `getAt`. `(obj, seq)` callback shape, discarding `seq` @see `conforms`.
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
 * @param path {array|string}: The path of the property to get.
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
/** Element of `arr` for which `iterateeSpec(val, seq)` is greatest, or `undefined` for an empty
 * `arr` -- std's array `max` picks the greatest value itself; this picks the element behind the
 * greatest *computed* value. `iterateeSpec` is coerced through `funcOrProp` @see `funcOrProp`.
 *
 * @example `maxBy([{ "n": 1 }, { "n": 3 }, { "n": 2 }], (val) => val.n); // => { "n": 3 }`
 */
// export function maxBy(arr, iterateeSpec) {}

// lodash
/** Like `max` except that it accepts `funcOrProp` which is
 * invoked for each element in `array` to generate the criterion by which
 * the value is ranked. A function iteratee is invoked with (val, seq).
 *
 * @param array {array}: The array to iterate over.
 * @param funcOrProp {function}: Function/propname invoked per element; defaults to `identity`.
 *   @optional
 *
 * @returns: the maximum value.
 *
 * @example `var objects = [{ 'n': 1 }, { 'n': 2 }]; maxBy(objects, function(o) { return o.n; }); // => { 'n': 2 }`
 * @example `maxBy(objects, 'n'); // => { 'n': 2 }`
 */
export function maxBy(array, iteratee) {
  if (false) { maxBy(array, iteratee); }
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
/** Average of `iterateeSpec(val, seq)` across `arr` -- `average` *(std)*, mapped via `mapValues`.
 * `iterateeSpec` is coerced through `funcOrProp` @see `funcOrProp`.
 *
 * @example `meanBy([{ "n": 2 }, { "n": 4 }], (val) => val.n); // => 3`
 */
// export function meanBy(arr, iterateeSpec) {}

// lodash
/** Like `mean` except that it accepts `funcOrProp` which is
 * invoked for each element in `array` to generate the value to be averaged.
 * A function iteratee is invoked with (val, seq).
 *
 * @param array {array}: The array to iterate over.
 * @param funcOrProp {function}: Function/propname invoked per element; defaults to `identity`.
 *   @optional
 *
 * @returns {number}: the mean.
 *
 * @example `var objects = [{ 'n': 4 }, { 'n': 2 }, { 'n': 8 }, { 'n': 6 }]; meanBy(objects, function(o) { return o.n; }); // => 5`
 * @example `meanBy(objects, 'n'); // => 5`
 */
export function meanBy(array, iteratee) {
  if (false) { meanBy(array, iteratee); }
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
/** Like `assign` except that it recursively merges own and
 * inherited enumerable string keyed properties of source objects into the
 * destination object. Source properties that resolve to `undefined` are
 * skipped if a destination value exists. Array and plain object properties
 * are merged recursively. Other objects and value types are overridden by
 * assignment. Source objects are applied from left to right. Subsequent
 * sources overwrite property assignments of previous sources.
 *
 * **Note:** This method mutates `object`.
 *
 * @param object {map}: The destination object.
 * @param sources {map}: The source objects.
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
 * @param path {array|string}: The path of the method to invoke.
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
/** `maxBy`'s counterpart: element of `arr` for which `iterateeSpec(val, seq)` is least, or
 * `undefined` for an empty `arr`. `iterateeSpec` is coerced through `funcOrProp` @see `funcOrProp`.
 *
 * @example `minBy([{ "n": 1 }, { "n": 3 }, { "n": 2 }], (val) => val.n); // => { "n": 1 }`
 */
// export function minBy(arr, iterateeSpec) {}

// lodash
/** Like `min` except that it accepts `funcOrProp` which is
 * invoked for each element in `array` to generate the criterion by which
 * the value is ranked. A function iteratee is invoked with (val, seq).
 *
 * @param array {array}: The array to iterate over.
 * @param funcOrProp {function}: Function/propname invoked per element; defaults to `identity`.
 *   @optional
 *
 * @returns: the minimum value.
 *
 * @example `var objects = [{ 'n': 1 }, { 'n': 2 }]; minBy(objects, function(o) { return o.n; }); // => { 'n': 1 }`
 * @example `minBy(objects, 'n'); // => { 'n': 1 }`
 */
export function minBy(array, iteratee) {
  if (false) { minBy(array, iteratee); }
  throw 'TODO: implement minBy';
}

// lodash
/** Adds all own enumerable string keyed function properties of a source
 * object to the destination object. If `object` is a function, then methods
 * are added to its prototype as well.
 *
 * **Note:** Use `runInContext` to create a pristine `lodash` function to
 * avoid conflicts caused by modifying the original.
 *
 * @param object {function|map}: The destination object; defaults to `lodash`.
 *   @optional
 * @param source {map}: The object of functions to add.
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
/** Used to resolve the
 * [`toStringTag`](http://ecma-international.org/ecma-262/7.0/#sec-object.prototype.tostring)
 * of values.
 */
export function nativeObjectToString() {
  if (false) { nativeObjectToString(); }
  throw 'TODO: implement nativeObjectToString';
}

// lodash
/** Creates a function that negates the result of the rule `func`. The
 * `func` rule is invoked with the `this` binding and arguments of the
 * created function.
 *
 * @param rule {function}: The rule to negate.
 *
 * @returns {function}: the new negated function.
 *
 * @example `function isEven(n) { return n % 2 == 0; } filter([1, 2, 3, 4, 5, 6], negate(isEven)); // => [1, 3, 5]`
 */
export function negate(rule) {
  if (false) { negate(rule); }
  throw 'TODO: implement negate';
}

// lodash
/** Reverts the `_` variable to its previous value and returns a reference to
 * the `lodash` function.
 *
 * @returns {function}: the `lodash` function.
 *
 * @example `var lodash = noConflict();`
 */
export function noConflict() {
  if (false) { noConflict(); }
  throw 'TODO: implement noConflict';
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
/** Element of `arr` at `seq`; a negative `seq` counts back from the end. `undefined` if `seq`,
 * after that adjustment, is out of bounds.
 *
 * @example `nth([1, 2, 3], 1); // => 2`
 * @example `nth([1, 2, 3], -1); // => 3`
 */
// export function nth(arr, seq) {}

// lodash
/** Gets the element at index `n` of `array`. If `n` is negative, the nth
 * element from the end is returned.
 *
 * @param array {array}: The array to query.
 * @param n {number}: The index of the element to return; defaults to `0`.
 *   @optional
 *
 * @returns: the nth element of `array`.
 *
 * @example `var array = ['a', 'b', 'c', 'd']; nth(array, 1); // => 'b'`
 * @example `nth(array, -2); // => 'c';`
 */
export function nth(array, n) {
  if (false) { nth(array, n); }
  throw 'TODO: implement nth';
}

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
/** Creates a map keyed by each element of `arr` itself, with the value at that key set to
 * `func(val, seq)` -- the last element responsible for a given key wins on a collision. This is
 * `keyBy` with the key and value roles swapped: lodash's `keyBy` keys by `iteratee(value)` and
 * keeps `value` itself, where `objectify` keys by `value` and lets `func` compute the result.
 *
 * @example `objectify(["a", "b", "c"], function(val, seq) { return seq; }); // => { "a": 0, "b": 1, "c": 2 }`
 * @example `objectify(["x", "x"], function(val, seq) { return seq; }); // => { "x": 1 }`
 */
// export function objectify(arr, func) {}

// flipshop
/** `bag` without the entries at `keylist` -- the inverse of `pick`. Each entry of `keylist` can be
 * a dotted string or key-path array @see `getAt`, deleting a nested leaf without disturbing its
 * siblings; a path with nothing currently at it is skipped rather than autovivifying empty maps
 * along the way. `omitBy` instead drops any entry for which `rule(val, key)` holds, the inverse
 * of `pickDefined`'s spirit but with a caller-supplied rule rather than a fixed "is defined"
 * check. `rule` is coerced through `funcOrProp` @see `funcOrProp`.
 *
 * @example `omit({ "a": 1, "b": 2, "c": 3 }, ["b"]); // => { "a": 1, "c": 3 }`
 * @example `omit({ "a": { "b": 1, "c": 2 } }, ["a.b"]); // => { "a": { "c": 2 } }`
 */
// export function omit(bag, keylist) {}

// lodash
/** The opposite of `pick`; this method creates an object composed of the
 * own and inherited enumerable property paths of `object` that are not omitted.
 *
 * **Note:** This method is considerably slower than `pick`.
 *
 * @param object {map}: The source object.
 * @param paths {(string|string[])}: The property paths to omit.
 *   @optional
 *
 * @returns {map}: the new object.
 *
 * @example `var object = { 'a': 1, 'b': '2', 'c': 3 }; omit(object, ['a', 'c']); // => { 'b': '2' }`
 */
export function omit(object, paths) {
  if (false) { omit(object, paths); }
  throw 'TODO: implement omit';
}

// lodash
/** The opposite of `pickBy`; this method creates an object composed of
 * the own and inherited enumerable string keyed properties of `object` that
 * `rule` doesn't return truthy for. The rule is invoked with two
 * arguments: (value, key).
 *
 * @param object {map}: The source object.
 * @param rule {function}: Function invoked per property; defaults to `identity`.
 *   @optional
 *
 * @returns {map}: the new object.
 *
 * @example `var object = { 'a': 1, 'b': '2', 'c': 3 }; omitBy(object, isNumber); // => { 'b': '2' }`
 */
export function omitBy(object, rule) {
  if (false) { omitBy(object, rule); }
  throw 'TODO: implement omitBy';
}

// lodash
/** Creates a function that is restricted to invoking `func` once. Repeat calls
 * to the function return the value of the first invocation. The `func` is
 * invoked with the `this` binding and arguments of the created function.
 *
 * @param func {function}: Function to restrict.
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
/** `orderBy`, with `cmpAny` as the comparator -- a total ordering even across mixed/incompatible
 * types in `iterateeSpec`'s results, so this never throws where `orderBy` might.
 *
 * @param vals {array|map}: Collection to sort.
 * @param funcOrPropSpec: @see `orderBy`. Defaults to `identity`.
 * @param order {number}: @see `orderBy`. Defaults to `1`.
 *
 * @example `orderAnyBy([1, "2", 0]); // => [0, 1, "2"] -- number sorts before string, per cmpAny's fstypenum fallback`
 */
// export function orderAnyBy(vals, iterateeSpec, order) {}

// flipshop
/** `vals`, ordered by `iterateeSpec`: `vals`'s elements (array) or values (map, keys discarded),
 * stably sorted by `iteratee(iterateeSpec)`'s result for each, compared with `comparator`.
 * Defaults to `cmp`, so an `iterateeSpec` producing incompatible types across elements -- e.g. a
 * mix of numbers and strings -- throws exactly as `cmp` does; @see `orderAnyBy` for a version that
 * never throws. Lodash's `orderBy` accepts one-or-many iteratees and orders; this accepts just one
 * of each.
 *
 * @param vals {array|map}: Collection to sort.
 * @param funcOrPropSpec: Ducktyped @see `funcOrProp` -- a function `(val, seq|key) => criteria`, a map (matches rule), or a string (property path). Defaults to `identity`.
 * @param order {number}: Positive sorts ascending, negative descending, zero leaves `vals` in its original order regardless of `iterateeSpec`. Defaults to `1`.
 * @param comparator {function}: `(criteriaA, criteriaB) => number`, applied to pairs of `iterateeSpec`'s results. Defaults to `cmp`.
 *
 * @example `orderBy([3, 1, 2]); // => [1, 2, 3]`
 * @example `orderBy([{ "n": 3 }, { "n": 1 }], "n"); // => [{ "n": 1 }, { "n": 3 }]`
 * @example `orderBy([1, 2, 3], identity, -1); // => [3, 2, 1]`
 * @example `orderBy({ "a": 3, "b": 1 }, identity); // => [1, 3]`
 */
// export function orderBy(vals, iterateeSpec, order, comparator) {}

// lodash
/** Like `sortBy` except that it allows specifying the sort
 * orders of The function/propnames to sort by. If `orders` is unspecified, all values
 * are sorted in ascending order. Otherwise, specify an order of "desc" for
 * descending or "asc" for ascending sort order of corresponding values.
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param funcOrProps {Array[]|Function[]|Object[]|string[]}: ] The function/propnames to sort by; defaults to `[identity`.
 *   @optional
 * @param orders {string[]}: The sort orders of `iteratees`.
 *   @optional
 *
 * @returns {array}: the new sorted array.
 *
 * @example `var users = [ { 'user': 'fred', 'age': 48 }, { 'user': 'barney', 'age': 34 }, { 'user': 'fred', 'age': 40 }, { 'user': 'barney', 'age': 36 } ]; orderBy(users, ['user', 'age'], ['asc', 'desc']); // => objects for [['barney', 36], ['barney', 34], ['fred', 48], ['fred', 40]]`
 */
export function orderBy(collection, iteratees, orders) {
  if (false) { orderBy(collection, iteratees, orders); }
  throw 'TODO: implement orderBy';
}

// flipshop
/** Builds a function that calls every function in `funcs` with `(val, seq)`, collecting results
 * into an array in `funcs`' order. Each element of `funcs` is coerced through `funcOrProp`, so a
 * dotpath string, `[path, srcValue]` array, or partial-match map can stand in for a literal
 * function, matching lodash's own `over`/`overEvery`/`overSome` docs.
 *
 * @example `over([(val, _seq) => val + 1, (val, _seq) => val - 1])(5, 0); // => [6, 4]`
 */
// export function over(funcs) {}

// lodash
/** Creates a function that invokes `iteratees` with the arguments it receives
 * and returns their results.
 *
 * @param funcOrProps {(Function|Function[])}: ] The function/propnames to invoke; defaults to `[identity`.
 *   @optional
 *
 * @returns {function}: the new function.
 *
 * @example `var func = over([Math.max, Math.min]); func(1, 2, 3, 4); // => [4, 1]`
 */
export function over(iteratees) {
  if (false) { over(iteratees); }
  throw 'TODO: implement over';
}

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
 * `(val, seq)`. Each element of `funcs` is coerced through `funcOrProp`; @see `over`.
 *
 * @example `overEvery([(val, _seq) => val > 0, (val, _seq) => val < 10])(5, 0); // => true`
 */
// export function overEvery(funcs) {}

// lodash
/** Creates a function that checks if **all** of the `rules` return
 * truthy when invoked with the arguments it receives.
 *
 * Following shorthands are possible for providing rules.
 * Pass an `Object` and it will be used as an parameter for `matches` to create the rule.
 * Pass an `Array` of parameters for `matchesProperty` and the rule will be created using them.
 *
 * @param rules {(Function|Function[])}: ] The rules to check; defaults to `[identity`.
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
/** Builds a rule that's `true` when any function in `funcs` returns truthy for `(val, seq)`. Each
 * element of `funcs` is coerced through `funcOrProp`; @see `over`.
 *
 * @example `overSome([(val, _seq) => val < 0, (val, _seq) => val > 10])(5, 0); // => false`
 */
// export function overSome(funcs) {}

// lodash
/** Creates a function that checks if **any** of the `rules` return
 * truthy when invoked with the arguments it receives.
 *
 * Following shorthands are possible for providing rules.
 * Pass an `Object` and it will be used as an parameter for `matches` to create the rule.
 * Pass an `Array` of parameters for `matchesProperty` and the rule will be created using them.
 *
 * @param rules {(Function|Function[])}: ] The rules to check; defaults to `[identity`.
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
 * for which it doesn't, keeping visiting order. `rule` gets `(val, seq)` (array) or `(val, key)`
 * (map); the map form returns values only, same as lodash's collection form. `rule` is coerced
 * through `funcOrProp` @see `funcOrProp`.
 *
 * @example `partition([1, 2, 3, 4], (val, _seq) => val % 2 == 0); // => [[2, 4], [1, 3]]`
 */
// export function partition(arr, rule) {}

// lodash
/** Creates an array of elements split into two groups, the first of which
 * contains elements `rule` returns truthy for, the second of which
 * contains elements `rule` returns falsey for. The rule is
 * invoked with one argument: (value).
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param funcOrProp {function|string|array}: iteratee; defaults to `identity`.
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
/** Creates an object composed of the picked `object` properties.
 *
 * @param object {map}: The source object.
 * @param paths {(string|string[])}: The property paths to pick.
 *   @optional
 *
 * @returns {map}: the new object.
 *
 * @example `var object = { 'a': 1, 'b': '2', 'c': 3 }; pick(object, ['a', 'c']); // => { 'a': 1, 'c': 3 }`
 */
export function pick(object, paths) {
  if (false) { pick(object, paths); }
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
/** Creates an object composed of the `object` properties `rule` returns
 * truthy for. The rule is invoked with two arguments: (value, key).
 *
 * @param object {map}: The source object.
 * @param rule {function}: Function invoked per property; defaults to `identity`.
 *   @optional
 *
 * @returns {map}: the new object.
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
 * @param path {array|string}: The path of the property to get.
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
/** The opposite of `property`; this method creates a function that returns
 * the value at a given path of `object`.
 *
 * @param object {map}: The object to query.
 *
 * @returns {function}: the new accessor function.
 *
 * @example `var array = [0, 1, 2], object = { 'a': array, 'b': array, 'c': array }; map(['a[2]', 'c[0]'], propertyOf(object)); // => [2, 0]`
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
/** Like `pullAll` except that it accepts `funcOrProp` which is
 * invoked for each element of `array` and `values` to generate the criterion
 * by which they're compared. A function iteratee is invoked with (val, seq).
 *
 * **Note:** Unlike `differenceBy`, this method mutates `array`.
 *
 * @param array {array}: The array to modify.
 * @param values {array}: The values to remove.
 * @param funcOrProp {function}: Function/propname invoked per element; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: `array`.
 *
 * @example `var array = [{ 'x': 1 }, { 'x': 2 }, { 'x': 3 }, { 'x': 1 }]; pullAllBy(array, [{ 'x': 1 }, { 'x': 3 }], 'x'); println(array); // => [{ 'x': 2 }]`
 */
export function pullAllBy(array, values, iteratee) {
  if (false) { pullAllBy(array, values, iteratee); }
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
/** Rebuilds a map (from a map or an array) by asking `func(val, key?)` -- `func(val, seq)` for an
 * array, `func(val, key)` for a map -- for the `[newKey, newVal]` pair each entry becomes; an
 * entry where `func` returns `undefined` (rather than a pair) is dropped rather than written
 * under an `undefined` key. Where two entries land on the same `newKey`, the later one wins --
 * `keys(bag)` order for a map, index order for an array.
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
 * each element in `collection` thru `funcOrProp`, where each successive
 * invocation is supplied the return value of the previous. If `accumulator`
 * is not given, the first element of `collection` is used as the initial
 * value. The function/propname is invoked with four arguments:
 * (accumulator, value, index|key, collection).
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
 * @param funcOrProp {function}: Reducer function invoked per iteration; defaults to `identity`.
 *   @optional
 * @param accumulator: The initial value.
 *   @optional
 *
 * @returns: the accumulated value.
 *
 * @example `reduce([1, 2], function(sum, n) { return sum + n; }, 0); // => 3`
 * @example `reduce({ 'a': 1, 'b': 2, 'c': 1 }, function(result, value, key) { (result[value] || (result[value] = [])).push(key); return result; }, {}); // => { '1': ['a', 'c'], '2': ['b'] } (iteration order is not guaranteed)`
 */
export function reduce(collection, iteratee, accumulator) {
  if (false) { reduce(collection, iteratee, accumulator); }
  throw 'TODO: implement reduce';
}

// flipshop
/** `bag`/`arr` reduced right-to-left through `foldFunction(accumulator, val, seq|key)` -- unlike
 * std's `foldArray`, `foldFunction` also gets the index/key as a third argument.
 *
 * @example `reduceRight([1, 2, 3], "", function(acc, val, _seq) { return acc ~ val; }); // => "321"`
 */
// export function reduceRight(arr, seed, foldFunction) {}

// flipshop
/** /** `reduceRight`, seeded from the last-visited element -- `undefined` for an empty `arr`/`bag`. */
 */
// export function reduceRight(arr, foldFunction) {}

// lodash
/** Like `reduce` except that it iterates over elements of
 * `collection` from right to left.
 *
 * @seeAlso [reduce]
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param funcOrProp {function}: Reducer function invoked per iteration; defaults to `identity`.
 *   @optional
 * @param accumulator: The initial value.
 *   @optional
 *
 * @returns: the accumulated value.
 *
 * @example `var array = [[0, 1], [2, 3], [4, 5]]; reduceRight(array, function(flattened, other) { return flattened.concat(other); }, []); // => [4, 5, 2, 3, 0, 1]`
 */
export function reduceRight(collection, iteratee, accumulator) {
  if (false) { reduceRight(collection, iteratee, accumulator); }
  throw 'TODO: implement reduceRight';
}

// flipshop
/** Elements of `bag`/`arr` for which `rule` does *not* hold -- the inverse of `filter` *(std)*.
 * `rule` gets `(val, seq)` (array) or `(val, key)` (map); the map form returns values only.
 * `rule` is coerced through `funcOrProp` @see `funcOrProp`.
 *
 * @example `reject([1, 2, 3, 4], (val, _seq) => val % 2 == 0); // => [1, 3]`
 */
// export function reject(arr, rule) {}

// lodash
/** The opposite of `filter`; this method returns the elements of `collection`
 * that `rule` does **not** return truthy for.
 *
 * @seeAlso [filter]
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param rule {function}: Function invoked per iteration; defaults to `identity`.
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
 * and returns an array of the removed elements. The rule is invoked
 * with three arguments: (value, index, array).
 *
 * **Note:** Unlike `filter`, this method mutates `array`. Use `pull`
 * to pull elements from an array by value.
 *
 * @param array {array}: The array to modify.
 * @param rule {function}: Function invoked per iteration; defaults to `identity`.
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
 * @param path {array|string}: The path of the property to resolve.
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
/** Sets the value at `path` of `object`. If a portion of `path` doesn't exist,
 * it's created. Arrays are created for missing index properties while objects
 * are created for all other missing properties. Use `setWith` to customize
 * `path` creation.
 *
 * **Note:** This method mutates `object`.
 *
 * @param object {map}: The object to modify.
 * @param path {array|string}: The path of the property to set.
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
 * @param path {array|string}: The path of the property to set.
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
/** Gets the size of `collection` by returning its length for array-like
 * values or the number of own enumerable string keyed properties for objects.
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
 * Iteration is stopped once `rule` returns truthy. The rule is
 * invoked with three arguments: (value, index|key, collection).
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param rule {function}: Function invoked per iteration; defaults to `identity`.
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
/** Creates an array of elements, sorted in ascending order by the results of
 * running each element in a collection thru each iteratee. This method
 * performs a stable sort, that is, it preserves the original sort order of
 * equal elements. The function/propnames are invoked with one argument: (value).
 *
 * @param collection {array|map}: The collection to iterate over.
 * @param funcOrProps {(Function|Function[])}: ] The function/propnames to sort by; defaults to `[identity`.
 *   @optional
 *
 * @returns {array}: the new sorted array.
 *
 * @example `var users = [ { 'user': 'fred', 'age': 48 }, { 'user': 'barney', 'age': 36 }, { 'user': 'fred', 'age': 30 }, { 'user': 'barney', 'age': 34 } ]; sortBy(users, [function(o) { return o.user; }]); // => objects for [['barney', 36], ['barney', 34], ['fred', 48], ['fred', 30]]`
 * @example `sortBy(users, ['user', 'age']); // => objects for [['barney', 34], ['barney', 36], ['fred', 30], ['fred', 48]]`
 */
export function sortBy(collection, iteratees) {
  if (false) { sortBy(collection, iteratees); }
  throw 'TODO: implement sortBy';
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
/** Like `sortedIndex` except that it accepts `funcOrProp`
 * which is invoked for `value` and each element of `array` to compute their
 * sort ranking. A function iteratee is invoked with (val, seq).
 *
 * @param array {array}: The sorted array to inspect.
 * @param value: The value to evaluate.
 * @param funcOrProp {function}: Function/propname invoked per element; defaults to `identity`.
 *   @optional
 *
 * @returns {number}: the index at which `value` should be inserted into `array`.
 *
 * @example `var objects = [{ 'x': 4 }, { 'x': 5 }]; sortedIndexBy(objects, { 'x': 4 }, function(o) { return o.x; }); // => 0`
 * @example `sortedIndexBy(objects, { 'x': 4 }, 'x'); // => 0`
 */
export function sortedIndexBy(array, value, iteratee) {
  if (false) { sortedIndexBy(array, value, iteratee); }
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
/** Like `sortedLastIndex` except that it accepts `funcOrProp`
 * which is invoked for `value` and each element of `array` to compute their
 * sort ranking. A function iteratee is invoked with (val, seq).
 *
 * @param array {array}: The sorted array to inspect.
 * @param value: The value to evaluate.
 * @param funcOrProp {function}: Function/propname invoked per element; defaults to `identity`.
 *   @optional
 *
 * @returns {number}: the index at which `value` should be inserted into `array`.
 *
 * @example `var objects = [{ 'x': 4 }, { 'x': 5 }]; sortedLastIndexBy(objects, { 'x': 4 }, function(o) { return o.x; }); // => 1`
 * @example `sortedLastIndexBy(objects, { 'x': 4 }, 'x'); // => 1`
 */
export function sortedLastIndexBy(array, value, iteratee) {
  if (false) { sortedLastIndexBy(array, value, iteratee); }
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
 * @param funcOrProp {function}: Function/propname invoked per element.
 *   @optional
 *
 * @returns {array}: the new duplicate free array.
 *
 * @example `sortedUniqBy([1.1, 1.2, 2.3, 2.4], Math.floor); // => [1.1, 2.3]`
 */
export function sortedUniqBy(array, iteratee) {
  if (false) { sortedUniqBy(array, iteratee); }
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
/** Sum of `iterateeSpec(val, seq)` across `arr` -- `sum` *(std)*, mapped via `mapValues`.
 * `iterateeSpec` is coerced through `funcOrProp` @see `funcOrProp`.
 *
 * @example `sumBy([{ "n": 2 }, { "n": 4 }], (val) => val.n); // => 6`
 */
// export function sumBy(arr, iterateeSpec) {}

// lodash
/** Like `sum` except that it accepts `funcOrProp` which is
 * invoked for each element in `array` to generate the value to be summed.
 * A function iteratee is invoked with (val, seq).
 *
 * @param array {array}: The array to iterate over.
 * @param funcOrProp {function}: Function/propname invoked per element; defaults to `identity`.
 *   @optional
 *
 * @returns {number}: the sum.
 *
 * @example `var objects = [{ 'n': 4 }, { 'n': 2 }, { 'n': 8 }, { 'n': 6 }]; sumBy(objects, function(o) { return o.n; }); // => 20`
 * @example `sumBy(objects, 'n'); // => 20`
 */
export function sumBy(array, iteratee) {
  if (false) { sumBy(array, iteratee); }
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
 * taken until `rule` returns falsey. The rule is invoked with
 * three arguments: (value, index, array).
 *
 * @param array {array}: The array to query.
 * @param rule {function}: Function invoked per iteration; defaults to `identity`.
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
 * are taken until `rule` returns falsey. The rule is invoked with
 * three arguments: (value, index, array).
 *
 * @param array {array}: The array to query.
 * @param rule {function}: Function invoked per iteration; defaults to `identity`.
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
/** Invokes `func(seq, seq)` `count` times (i.e. for seq = 0 to count - 1),
 * returning an array of the results of each invocation.
 *
 * @param count {number}: The number of times to invoke `func`.
 * @param func {function}: Function invoked per iteration; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the array of results.
 *
 * @example `times(3, String); // => ['0', '1', '2']`
 * @example `times(3, (seq, _seq2) => seq * seq); // => [0, 1, 4]`
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
/** Creates an array of own enumerable string keyed-value pairs for `object`
 * which can be consumed by `fromPairs`. If `object` is a map or set, its
 * entries are returned.
 *
 * @param object {map}: The object to query.
 *
 * @returns {array}: the key-value pairs.
 *
 * @example `function Foo() { this.a = 1; this.b = 2; } Foo.prototype.c = 3; toPairs(new Foo); // => [['a', 1], ['b', 2]] (iteration order is not guaranteed)`
 */
export function toPairs(object) {
  if (false) { toPairs(object); }
  throw 'TODO: implement toPairs';
}

// lodash
/** Creates an array of own and inherited enumerable string keyed-value pairs
 * for `object` which can be consumed by `fromPairs`. If `object` is a map
 * or set, its entries are returned.
 *
 * @param object {map}: The object to query.
 *
 * @returns {array}: the key-value pairs.
 *
 * @example `function Foo() { this.a = 1; this.b = 2; } Foo.prototype.c = 3; toPairsIn(new Foo); // => [['a', 1], ['b', 2], ['c', 3]] (iteration order is not guaranteed)`
 */
export function toPairsIn(object) {
  if (false) { toPairsIn(object); }
  throw 'TODO: implement toPairsIn';
}

// lodash
/** Converts `value` to a property path array.
 *
 * @param value: The value to convert.
 *
 * @returns {array}: the new property path array.
 *
 * @example `toPath('a.b.c'); // => ['a', 'b', 'c']`
 * @example `toPath('a[0].b.c'); // => ['a', '0', 'b', 'c']`
 */
export function toPath(value) {
  if (false) { toPath(value); }
  throw 'TODO: implement toPath';
}

// lodash
/** Converts `value` to a plain object flattening inherited enumerable string
 * keyed properties of `value` to own properties of the plain object.
 *
 * @param value: The value to convert.
 *
 * @returns {map}: the converted plain object.
 *
 * @example `function Foo() { this.b = 2; } Foo.prototype.c = 3; assign({ 'a': 1 }, new Foo); // => { 'a': 1, 'b': 2 }`
 * @example `assign({ 'a': 1 }, toPlainObject(new Foo)); // => { 'a': 1, 'b': 2, 'c': 3 }`
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
/** An alternative to `reduce`; this method transforms `object` to a new
 * `accumulator` object which is the result of running each of its own
 * enumerable string keyed properties thru `funcOrProp`, with each invocation
 * potentially mutating the `accumulator` object. If `accumulator` is not
 * provided, a new object with the same `[[Prototype]]` will be used. The
 * iteratee is invoked with four arguments: (accumulator, value, key, object).
 * Iteratee functions may exit iteration early by explicitly returning `false`.
 *
 * @param object {map}: The object to iterate over.
 * @param funcOrProp {function}: Reducer function invoked per iteration; defaults to `identity`.
 *   @optional
 * @param accumulator: The custom accumulator value.
 *   @optional
 *
 * @returns: the accumulated value.
 *
 * @example `transform([2, 3, 4], function(result, n) { result.push(n *= n); return n % 2 == 0; }, []); // => [4, 9]`
 * @example `transform({ 'a': 1, 'b': 2, 'c': 1 }, function(result, value, key) { (result[value] || (result[value] = [])).push(key); }, {}); // => { '1': ['a', 'c'], '2': ['b'] }`
 */
export function transform(object, iteratee, accumulator) {
  if (false) { transform(object, iteratee, accumulator); }
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
/** `union`, deduplicating by `iterateeSpec(val)` instead of `val` itself. `iterateeSpec` is coerced
 * through `funcOrProp` @see `funcOrProp`.
 *
 * @example `unionBy([[2.1], [1.2, 2.3]], (val) => floor(val)); // => [2.1, 1.2]`
 */
// export function unionBy(arrList, iterateeSpec) {}

// lodash
/** Like `union` except that it accepts `funcOrProp` which is
 * invoked for each element of each `arrays` to generate the criterion by
 * which uniqueness is computed. Result values are chosen from the first
 * array in which the value occurs. The function/propname is invoked with one argument:
 * (value).
 *
 * @param arrays {array}: The arrays to inspect.
 *   @optional
 * @param funcOrProp {function}: Function/propname invoked per element; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the new array of combined values.
 *
 * @example `unionBy([2.1], [1.2, 2.3], Math.floor); // => [2.1, 1.2]`
 * @example `unionBy([{ 'x': 1 }], [{ 'x': 2 }, { 'x': 1 }], 'x'); // => [{ 'x': 1 }, { 'x': 2 }]`
 */
export function unionBy(arrays, iteratee) {
  if (false) { unionBy(arrays, iteratee); }
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
 * `deduplicate`, but comparing `iterateeSpec(val, seq)` instead of `val` itself. `iterateeSpec` is
 * coerced through `funcOrProp` @see `funcOrProp`.
 *
 * @example `uniqBy([2.1, 1.2, 2.3], (val, _seq) => floor(val)); // => [2.1, 1.2]`
 */
// export function uniqBy(arr, iterateeSpec) {}

// lodash
/** Like `uniq` except that it accepts `funcOrProp` which is
 * invoked for each element in `array` to generate the criterion by which
 * uniqueness is computed. The order of result values is determined by the
 * order they occur in the array. The function/propname is invoked with one argument:
 * (value).
 *
 * @param array {array}: The array to inspect.
 * @param funcOrProp {function}: Function/propname invoked per element; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the new duplicate free array.
 *
 * @example `uniqBy([2.1, 1.2, 2.3], Math.floor); // => [2.1, 1.2]`
 * @example `uniqBy([{ 'x': 1 }, { 'x': 2 }, { 'x': 1 }], 'x'); // => [{ 'x': 1 }, { 'x': 2 }]`
 */
export function uniqBy(array, iteratee) {
  if (false) { uniqBy(array, iteratee); }
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
/** Removes the property at `path` of `object`.
 *
 * **Note:** This method mutates `object`.
 *
 * @param object {map}: The object to modify.
 * @param path {array|string}: The path of the property to unset.
 *
 * @returns {boolean}: `true` if the property is deleted, else `false`.
 *
 * @example `var object = { 'a': [{ 'b': { 'c': 7 } }] }; unset(object, 'a[0].b.c'); // => true`
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
/** `unzip`, passing each ungrouped column through `funcOrProp` before collecting it.
 *
 * @example `unzipWith([[1, 10], [2, 20]], (col) => sum(col)); // => [3, 30]`
 */
// export function unzipWith(arr, iteratee) {}

// lodash
/** Like `unzip` except that it accepts `funcOrProp` to specify
 * how regrouped values should be combined. The function/propname is invoked with the
 * elements of each group: (...group).
 *
 * @param array {array}: The array of grouped elements to process.
 * @param funcOrProp {function}: Function to combine regrouped values; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the new array of regrouped elements.
 *
 * @example `var zipped = zip([1, 2], [10, 20], [100, 200]); // => [[1, 10, 100], [2, 20, 200]]`
 * @example `unzipWith(zipped, add); // => [3, 30, 300]`
 */
export function unzipWith(array, iteratee) {
  if (false) { unzipWith(array, iteratee); }
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
 * @param path {array|string}: The path of the property to set.
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
 * @param path {array|string}: The path of the property to set.
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
/** Creates an array of the own enumerable string keyed property values of `object`.
 *
 * **Note:** Non-object values are coerced to objects.
 *
 * @param object {map}: The object to query.
 *
 * @returns {array}: the array of property values.
 *
 * @example `function Foo() { this.a = 1; this.b = 2; } Foo.prototype.c = 3; values(new Foo); // => [1, 2] (iteration order is not guaranteed)`
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
 * `missingPolicy` decides what happens at a key/index with nothing there: `NIL` (the
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
/** Creates an array of the own and inherited enumerable string keyed property
 * values of `object`.
 *
 * **Note:** Non-object values are coerced to objects.
 *
 * @param object {map}: The object to query.
 *
 * @returns {array}: the array of property values.
 *
 * @example `function Foo() { this.a = 1; this.b = 2; } Foo.prototype.c = 3; valuesIn(new Foo); // => [1, 2, 3] (iteration order is not guaranteed)`
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
 * @param paths {(string|string[])}: The property paths to pick.
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
/** `xor`, comparing by `iterateeSpec(val, seq)` instead of `val` itself. `iterateeSpec` is coerced
 * through `funcOrProp` @see `funcOrProp`.
 *
 * @example `xorBy([[2.1, 1.2], [2.3, 3.4]], (val, _seq) => floor(val)); // => [1.2, 3.4]`
 */
// export function xorBy(arrList, iterateeSpec) {}

// lodash
/** Like `xor` except that it accepts `funcOrProp` which is
 * invoked for each element of each `arrays` to generate the criterion by
 * which by which they're compared. The order of result values is determined
 * by the order they occur in the arrays. The function/propname is invoked with one
 * argument: (value).
 *
 * @param arrays {array}: The arrays to inspect.
 *   @optional
 * @param funcOrProp {function}: Function/propname invoked per element; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the new array of filtered values.
 *
 * @example `xorBy([2.1, 1.2], [2.3, 3.4], Math.floor); // => [1.2, 3.4]`
 * @example `xorBy([{ 'x': 1 }], [{ 'x': 2 }, { 'x': 1 }], 'x'); // => [{ 'x': 2 }]`
 */
export function xorBy(arrays, iteratee) {
  if (false) { xorBy(arrays, iteratee); }
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
/** Like `fromPairs` except that it accepts two arrays,
 * one of property identifiers and one of corresponding values.
 *
 * @param props {array}: ] The property identifiers; defaults to `[`.
 *   @optional
 * @param values {array}: ] The property values; defaults to `[`.
 *   @optional
 *
 * @returns {map}: the new object.
 *
 * @example `zipObject(['a', 'b'], [1, 2]); // => { 'a': 1, 'b': 2 }`
 */
export function zipObject(props, values) {
  if (false) { zipObject(props, values); }
  throw 'TODO: implement zipObject';
}

// lodash
/** Like `zipObject` except that it supports property paths.
 *
 * @param props {array}: ] The property identifiers; defaults to `[`.
 *   @optional
 * @param values {array}: ] The property values; defaults to `[`.
 *   @optional
 *
 * @returns {map}: the new object.
 *
 * @example `zipObjectDeep(['a.b[0].c', 'a.b[1].d'], [1, 2]); // => { 'a': { 'b': [{ 'c': 1 }, { 'd': 2 }] } }`
 */
export function zipObjectDeep(props, values) {
  if (false) { zipObjectDeep(props, values); }
  throw 'TODO: implement zipObjectDeep';
}

// TODO-clxnUtils
/** `zip` *(std)* on `arrList`, passing each grouped row through `funcOrProp` before collecting it --
 * the same shape as `unzipWith`, under lodash's name for the zipping direction.
 *
 * @example `zipWith([[1, 2], [10, 20]], (row) => sum(row)); // => [11, 22]`
 */
export function zipWith(arrList, funcOrProp) {
  if (false) { zipWith(arrList, funcOrProp); }
  throw 'TODO: implement zipWith';
}

// TODO-clxnUtils
/** Like `zip` except that it accepts `funcOrProp` to specify
 * how grouped values should be combined. The function/propname is invoked with the
 * elements of each group: (...group).
 *
 * @param arrays {array}: The arrays to process.
 *   @optional
 * @param funcOrProp {function}: Function to combine grouped values; defaults to `identity`.
 *   @optional
 *
 * @returns {array}: the new array of grouped elements.
 *
 * @example `zipWith([1, 2], [10, 20], [100, 200], function(a, b, c) { return a + b + c; }); // => [111, 222]`
 */
export function zipWith2(arrays, iteratee) {
  if (false) { zipWith2(arrays, iteratee); }
  throw 'TODO: implement zipWith2';
}
