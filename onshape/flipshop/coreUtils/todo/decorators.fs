
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
/** Creates an array of function property names from own enumerable properties
 * of `object`.
 *
 * @seeAlso [functionsIn]
 *
 * @param object {map}: The object to inspect.
 *
 * @returns {array}: the function names.
 *
 * @example `function Foo() { this.a = constant('a'); this.b = constant('b'); } Foo.prototype.c = constant('c'); functions(new Foo); // => ['a', 'b']`
 */
export function functions(object) {
  if (false) { functions(object); }
  throw 'TODO: implement functions';
}

// lodash
/** This method invokes `interceptor` and returns `value`. The interceptor
 * is invoked with one argument; (value). The purpose of this method is to
 * "tap into" a method chain sequence in order to modify intermediate results.
 *
 * @param value: The value to provide to `interceptor`.
 * @param interceptor {function}: Function to invoke.
 *
 * @returns: `value`.
 *
 * @example `_([1, 2, 3]) .tap(function(array) { array.pop(); }) .reverse() .value(); // => [2, 1]`
 */
export function tap(value, interceptor) {
  if (false) { tap(value, interceptor); }
  throw 'TODO: implement tap';
}


// lodash
/** Like `tap` except that it returns the result of `interceptor`.
 * The purpose of this method is to "pass thru" values replacing intermediate
 * results in a method chain sequence.
 *
 * @param value: The value to provide to `interceptor`.
 * @param interceptor {function}: Function to invoke.
 *
 * @returns: the result of `interceptor`.
 *
 * @example `_(' abc ') .chain() .trim() .thru(function(value) { return [value]; }) .value(); // => ['abc']`
 */
export function thru(value, interceptor) {
  if (false) { thru(value, interceptor); }
  throw 'TODO: implement thru';
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
