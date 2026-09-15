

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
