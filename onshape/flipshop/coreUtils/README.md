# coreUtils

General-purpose FeatureScript helpers shared across this project's features — not
geometry-specific, just the plumbing every feature ends up needing. Style follows
[`STYLE-Featurescript.md`](../STYLE-Featurescript.md); the standard-library map/array/string
functions these build on are catalogued in [`onshape/README.md`](../../README.md).

This is a starting summary, one line per exported function/constant, and every file in the
directory has now had a full pass: tests wherever plain-data testing is possible, docblocks
throughout, and a lodash correspondence table where one applies. `coreUtils.fs` itself has no
functions of its own — it just re-exports the documents the other files in this directory compile
into, so other features can `import` this one file for all of it.

## Tests

One `run<Thing>Tests` function per case list, in `tests/`. All `clxn*` suites — everything from
`clxnWalking.fs` and from `clxnGetset.fs`/`clxnReshape.fs` — run out of a single Feature,
`runClxnTestsFS` in `tests/testClxnWalking.fs`. Adding a new `clxn*` function's tests means
adding its `run*Tests` call to that Feature, not creating a new one. Every other file gets its
own `test<File>.fs` and `run<File>TestsFS` Feature (`sizeof`'s tests are the one exception,
living under `runCoreUtilsTestsFS` in `tests/testCoreUtils.fs`, since `sizeof` predates the
`clxn*` split).

Three files' worth of functions have no test suite at all, and for the same underlying reason:
`debugUtils.fs`'s `highlightQuery`, most of `metadataUtils.fs` (`setPropAndAttribute`, `setName`,
`setReadableName`, `getAttrs`, `getAllAttrs`, `getBestAttr`, `getNameProps`, `getNames`,
`getName`, `getNameProp`, `getNameOfBody`), and every Feature in `jsonVarF.fs`
(`jsonVarF`/`keylistF`/`sizeofF`/`valuesAtF`/`splatF`, plus `setColor` in `colorUtils.fs`) read or
write properties/attributes/variables on a live `Query`/`Context`, which the plain-data-table
`runTests` harness this whole test suite is built on has no way to exercise without an actual
part studio to run against. `metadataUtils.fs`'s three pure functions (`defaultMaybe`,
`sanitize_varname`, `field_varname`) are tested in `testMetadataUtils.fs` despite that, and every
other function in `colorUtils.fs` is pure and tested in `testColorUtils.fs`.

---

## Lodash correspondence

Much of this directory is an ongoing port of [lodash](https://lodash.com/docs)'s conveniences
into FeatureScript, pulled in as needed from the reference copy at `lodash.js`. This section
catalogs all ~300 of lodash's top-level public functions, organized the way lodash's own docs
group them (Array, Collection, Function, Lang, Math, Number, Object, Seq, String, Util, plus a
one-function Date category), and says what — if anything — covers each one on the FeatureScript
side.

**Reading the FeatureScript column:**

| Notation | Means |
|---|---|
| a plain name (`getAt`, `pick`, …) | implemented in this project's coreUtils — see the file-by-file catalogs further down |
| `name` *(std)* | a FeatureScript standard-library builtin (`math.fs`/`string.fs`/`containers.fs`) covers it, no port needed |
| `` `is array` `` etc. *(lang)* | built into the FeatureScript language itself (a type-check expression or operator), not a function call |
| ~~name~~ | nothing implements it, anywhere — a real (if low-priority) porting candidate |
| a struck basket + "→ HUMAN-FIXME.md" | consciously not planned; the reason and the full member list live in [`HUMAN-FIXME.md`](HUMAN-FIXME.md) instead of being repeated here |

A function can have both a coreUtils port *and* a std builtin that inspired it, and the two can
differ in how much they actually do — `merge` is the poster child: coreUtils' `deepMerge`
recurses through nested maps the way lodash's `merge` does, while std's own `mergeMaps` is a
shallower, "less functional" one-level combine. Both are noted on that row.

### Array

| FeatureScript | Lodash | Does | Notes |
|---|---|---|---|
| `concatenateArrays` *(std)* | `concat` | Concatenates array(s)/value(s) into a new array. | std is arrays-only — no flattening of bare scalar arguments the way lodash does. |
| `first` *(std)* | `head` / `first` | First element of an array. | std spells it `first`; lodash treats `head`/`first` as aliases of one function. |
| `indexOf` *(std)* | `indexOf` | Index of the first occurrence of a value. | Direct match. |
| `join` *(std)* | `join` | Joins array elements into a string. | Direct match. |
| `last` *(std)* / `arrLast` | `last` | Last element of an array. | Both std's `last` and coreUtils' `arrLast` cover this directly. |
| `removeElementAt` *(std)* | `pullAt` | Removes element(s) by index. | std takes one index at a time and doesn't hand back what it removed, unlike lodash's variadic, removed-values-returning form. |
| `reverse` *(std)* | `reverse` | Reverses an array. | Direct match. |
| `subArray` *(std)* | `slice` | Slice of an array between two indices. | Direct match. |
| `deduplicate` *(std)* | `uniq` | Duplicate-free version of an array. | Direct match. |
| `zip` *(std)* | `zip` | Groups the nth elements of several arrays together. | Direct match. |
| `chunk` | `chunk` | Splits an array into groups of `chunkSize`. | |
| `compact` | `compact` | Removes falsey values from an array. | Narrower than lodash: only `undefined`/`false` are falsey in FeatureScript, so `0` and `""` survive — a thin wrapper around std `filter` and typeUtils' `truthy`. |
| `difference` / `differenceBy` | `difference`, `differenceBy` | Values in one array not present in another. | No varargs, so the exclusion values are one array, not trailing arguments — concatenate first to exclude from several sources. ~~differenceWith~~ not ported (marginal over `differenceBy`). |
| `drop` / `dropRight` / `dropRightWhile` / `dropWhile` | `drop` family | Drop `n` elements (or while a predicate holds) from either end. | |
| `findIndex` | `findIndex` | Index of the first element matching a predicate. | No `fromIndex` argument. |
| `findLastIndex` | `findLastIndex` | Index of the last element matching a predicate. | No `fromIndex` argument. |
| `flatten` / `flattenDeep` / `flattenDepth` | `flatten` family | Flatten an array one level / fully / `n` levels deep. | `dotMap`'s `maxDepth` (coreUtils) is the map-shaped cousin, not array-shaped. |
| `fromPairs` | `fromPairs` | `[[k,v], …]` → map. | |
| `initial` | `initial` | All but the last element. | |
| `intersection` / `intersectionBy` | `intersection`, `intersectionBy` | Values present in every given array. | Takes an array of arrays (no varargs). ~~intersectionWith~~ not ported. |
| `lastIndexOf` | `lastIndexOf` | Index of the last occurrence of a value, searching from the end. | No `fromIndex` argument. |
| `nth` | `nth` | Nth element (negative counts from the end). | |
| ~~sortedIndex~~ / ~~sortedIndexBy~~ / ~~sortedIndexOf~~ / ~~sortedLastIndex~~ / ~~sortedLastIndexBy~~ / ~~sortedLastIndexOf~~ | `sortedIndex*` family | Binary-search insertion points into an already-sorted array. | std's `sort(arr, compareFunction)` makes this possible for numbers, but the 6-function family is a lot of surface for an optimization this codebase has no hot path for. |
| ~~sortedUniq~~ / ~~sortedUniqBy~~ | `sortedUniq`, `sortedUniqBy` | `uniq`/`uniqBy`, optimized for sorted input. | Redundant with `uniqBy`/std `deduplicate` at this project's scale — the "sorted" optimization buys nothing here. |
| `tail` | `tail` | All but the first element. | |
| `take` / `takeRight` / `takeRightWhile` / `takeWhile` | `take` family | Take `n` elements (or while a predicate holds) from either end. | `strTake`/`strTakeRight` (coreUtils) cover the string analogue, not arrays. |
| `union` / `unionBy` | `union`, `unionBy` | Deduplicated concatenation of several arrays. | Takes an array of arrays (no varargs). ~~unionWith~~ not ported. |
| `uniqBy` / `uniqWith` | `uniqBy`, `uniqWith` | `uniq`, iteratee-mapped / custom comparator. | Plain `uniq` is std's `deduplicate`. |
| `unzip` / `unzipWith` | `unzip`, `unzipWith` | Inverse of `zip`. | `zip`'s grouping is its own inverse, so `unzip` is a bare alias for std `zip`. |
| `without` | `without` | Values from an array excluding the given ones. | Identical to `difference` once variadic exclusion values collapse to one array — kept as its own name for lodash parity. |
| `xor` | `xor` | Symmetric difference of several arrays. | Takes an array of arrays (no varargs). ~~xorBy~~ / ~~xorWith~~ not ported. |
| `zipObject` / `zipWith` | `zipObject`, `zipWith` | Build an object, or combine grouped elements with a function. | `zipWith` and `unzipWith` are the same shape of operation, both riding on `zip`'s self-symmetry. ~~zipObjectDeep~~ not ported (needs path-based nested writes, low value here). |

Not Yet (perhaps never):

| ~~fill~~ / ~~pull~~ / ~~pullAll~~ / ~~pullAllBy~~ / ~~pullAllWith~~ / ~~remove~~ | `fill`, `pull*`, `remove` | Fill/remove elements, in place. | Mutate-in-place family — see HUMAN-FIXME.md. |


### Collection

| FeatureScript | Lodash | Does | Notes |
|---|---|---|---|
| `forEach` | `forEach` / `each` | Iterate a collection. | |
| `mapValues3` (array form) | `map` | Map a collection to a same-shaped result. | |
| `arrayIncludes` | `includes` | Whether a value appears in a collection. | |
| `objectify` | `keyBy` | Key a collection by a computed key. | Key/value roles swapped from lodash — see the file-by-file table below. |
| `all` *(std)* | `every` | Whether every element passes a predicate. | |
| `any` *(std)* | `some` | Whether any element passes a predicate. | |
| `filter` *(std)* | `filter` | Elements passing a predicate. | |
| `foldArray` *(std)* | `reduce` | Reduce a collection to a single value. | |
| `sizeof` / `size` *(std)* | `size` | Element/key/character count. | |
| `countBy` | `countBy` | Counts of elements, grouped by a computed key. | |
| `find` / `findLast` | `find`, `findLast` | First/last element matching a predicate. | Arrays only — dereferences `arrayUtils`' `findIndex`/`findLastIndex`. |
| `flatMap` / `flatMapDeep` / `flatMapDepth` | `flatMap` family | Map then flatten one/all/`n` levels. | Rides on `arrayUtils`' `flatten` family. |
| `forEachRight` | `forEachRight` / `eachRight` | Iterate a collection back-to-front. | Arrays only. |
| `groupBy` | `groupBy` | Group elements by a computed key. | Built on std's `insertIntoMapOfArrays`. |
| `partition` | `partition` | Split into two groups by a predicate. | |
| `reduceRight` | `reduceRight` | `reduce`, back-to-front. | `foldArray` *(std)* over a `reverse`d array. |
| `reject` | `reject` | Elements *failing* a predicate — inverse of `filter`. | |

Not yet:
| ~~sample~~ / ~~sampleSize~~ / ~~shuffle~~ | `sample` family | Random element(s) / shuffled order. | Randomness isn't a FeatureScript thing by design. |
| ~~sortBy~~ / ~~orderBy~~ | `sortBy`, `orderBy` | Sort by one or more iteratees, each with its own direction. | Iteratee results are often strings, and Onshape can't compare strings with `< > <= >=` — leaving this alone until that workaround is worth the heroics. |
| ~~invokeMap~~ | `invokeMap` | Invoke a named method on each element. | No dynamic method dispatch by name in FeatureScript. |

### Function

| FeatureScript | Lodash | Does | Notes |
|---|---|---|---|
| `memoizeFunction` *(std)* | `memoize` | Cache a function's results by argument. | |

Evaluate to see what's both useful and possible:

| FeatureScript | Lodash | Does | Notes |
|---|---|---|---|
| ~~after~~ / ~~ary~~ / ~~before~~ / ~~flip~~ / ~~flow~~ / ~~flowRight~~ / ~~negate~~ / ~~once~~ / ~~overArgs~~ / ~~rearg~~ / ~~rest~~ / ~~spread~~ / ~~unary~~ / ~~wrap~~ | arity/composition family | Reshape a function's arity, argument order, or call count. | → HUMAN-FIXME.md ("related to Date or functions"). |

Not Yet/ever:

| FeatureScript | Lodash | Does | Notes |
|---|---|---|---|
| ~~debounce~~ / ~~defer~~ / ~~delay~~ / ~~throttle~~ | async family | Delay, batch, or rate-limit a function call. | → HUMAN-FIXME.md ("async"). |
| ~~bind~~ / ~~bindKey~~ / ~~curry~~ / ~~curryRight~~ | binding/currying family | Partially apply or fix a function's `this`/arity. | → HUMAN-FIXME.md ("metaprogramming"). coreUtils has its own fixed-arity `curry2to0`…`curry3to2` (`miscUtils.fs`) for the one shape this project actually needed, not a general variadic curry. |

### Lang

| FeatureScript                           | Lodash                                  | Does                                 | Notes                                                                                                                                                                    |
| ---------------------------------------- | ---------------------------------------- | ------------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `isNil`                                 | `isNil`, `isNull`, `isUndefined`        | Is the value absent?                 | FeatureScript has no `null`, so all three JS distinctions collapse into one `undefined` check — also expressible as the language operator `== undefined`.                |
| `isEmpty`                               | `isEmpty`                               | Is the value empty?                  | Restricted to the map/string/array/`undefined` cases FeatureScript actually has.                                                                                         |
| `ifNil`                                 | `defaultTo`                             | Value, or a fallback if nil.         |                                                                                                                                                                          |
| `` `is array`/`is map`/`is string`/`is number`/`is boolean`/`is function` `` *(lang)* | `isArray`, `isBoolean`, `isFunction`, `isMap`, `isObject`, `isPlainObject`, `isNumber`, `isString` | Type-check a value.                  | FeatureScript's `is <Type>` is a language expression, not a function call — this is the basket the styling of `isArray … isUndefined` in this doc's own intro refers to. |
| `==` *(lang)*                           | `isEqual`, `isEqualWith`                | Deep structural equality.            | FeatureScript's `==` already does deep value comparison on maps/arrays — this is the default equality operator, not a function.                                          |
| `>` / `>=` / `<` / `<=` *(lang)*        | `gt`, `gte`, `lt`, `lte`                | Numeric comparison.                  | Plain comparison operators, numbers only — no lodash-style mixed-type coercion.                                                                                          |
| `toString` *(std)*                      | `toString`                              | Converts a value to its string form. |                                                                                                                                                                          |

Not yet/ever:

| FeatureScript                           | Lodash                                  | Does                                 | Notes                                                                                                                                                                    |
| ---------------------------------------- | ---------------------------------------- | ------------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| ~~isArguments~~ / ~~isArrayBuffer~~ / ~~isArrayLike~~ / ~~isArrayLikeObject~~ / ~~isBuffer~~ / ~~isDate~~ / ~~isElement~~ / ~~isError~~ / ~~isFinite~~ / ~~isInteger~~ / ~~isLength~~ / ~~isMatch~~ / ~~isMatchWith~~ / ~~isNaN~~ / ~~isNative~~ / ~~isObjectLike~~ / ~~isRegExp~~ / ~~isSafeInteger~~ / ~~isSet~~ / ~~isSymbol~~ / ~~isTypedArray~~ / ~~isWeakMap~~ / ~~isWeakSet~~ | remaining `is*` predicates | Type-check JS/DOM-runtime-specific types, or match against a pattern. | FeatureScript has no Symbol/Buffer/WeakMap/DOM-element/etc. types for most of these to even ask about. |
| `castArray` | `castArray` | Wrap a non-array value in a 1-element array. | |
| ~~clone~~ / ~~cloneDeep~~ / ~~cloneDeepWith~~ / ~~cloneWith~~ | `clone` family | Shallow/deep copy a value. | FeatureScript maps and arrays already have value (copy-on-write) semantics, so "clone" is largely a non-issue here — there's no shared mutable reference to defend against in the first place. |
| ~~toArray~~ / ~~toFinite~~ / ~~toInteger~~ / ~~toLength~~ / ~~toNumber~~ / ~~toPlainObject~~ / ~~toSafeInteger~~ | `to*` coercion family | Coerce a value toward a target type. | |
| ~~conformsTo~~ | `conformsTo` | Whether an object satisfies a predicate-shaped spec. | → HUMAN-FIXME.md ("metaprogramming"). |

### Math

| FeatureScript | Lodash | Does | Notes |
|---|---|---|---|
| `ceil` *(std)* | `ceil` | Round up. | |
| `floor` *(std)* | `floor` | Round down. | |
| `round` / `roundToPrecision` *(std)* | `round` | Round to the nearest integer, or `n` decimal places. | lodash's optional precision argument becomes a second, separate std function. |
| `min` *(std)* | `min` | Smaller of two values. | std takes two values; lodash's `min` reduces a whole array. |
| `max` *(std)* | `max` | Larger of two values. | Same array-vs-pair difference as `min`. |
| `average` *(std)* | `mean` | Average of an array. | |
| `sum` *(std)* | `sum` | Sum of an array. | |
| ~~add~~ / ~~divide~~ / ~~multiply~~ / ~~subtract~~ | arithmetic-as-functions | `+ - * /` as callables. | FeatureScript just uses the operators directly. |
| `maxBy` / `meanBy` / `minBy` / `sumBy` | `*By` family | Iteratee-mapped versions of the row above. | In `numberUtils.fs`. |

### Number

| FeatureScript | Lodash | Does | Notes |
|---|---|---|---|
| `clamp` *(std)* | `clamp` | Constrain a number to `[lower, upper]`. | |
| `inRange` | `inRange` | Whether a number falls within a range. | Bounds auto-swap if `start > end`, matching lodash. |
| ~~random~~ | `random` | Random number in a range. | Randomness isn't a FeatureScript thing by design. |

### Object

| FeatureScript | Lodash | Does | Notes |
|---|---|---|---|
| `getAt` | `get` | Path-based read from a map/array. | |
| `setAt` | `set` | Path-based write. | lodash's `set` mutates; `setAt` returns a new value. |
| `deepMerge` / `mergeMaps` *(std)* | `merge` | Recursively combine two maps. | The headline "weird/less-functional builtin" case: std's `mergeMaps` is a shallow, one-level combine; coreUtils' `deepMerge` is the fuller recursive port lodash's `merge` actually does. |
| `pick` | `pick` | Map of just the given keys. | |
| `pickDefined` | `pickBy` | `pick`, keeping only defined values. | |
| `valuesAt` | `at` | Values at several keys/indices, in order. | |
| `hasKey` | `has` | Whether a key/path resolves to something. | |
| `mapValues` | `mapValues` | Map a map's values, keys unchanged. | |
| `keys` *(std)* | `keys` | A map's keys. | |
| `values` *(std)* | `values` | A map's values. | |
| `toPairs` | `toPairs` | Map → `[[k,v], …]`. | FeatureScript maps have no own/inherited distinction, so this one function also covers `entries`, `entriesIn`, and `toPairsIn`. In `objectUtils.fs`. |
| `findKey` / `findLastKey` | `findKey`, `findLastKey` | First/last key whose value matches a predicate. | In `objectUtils.fs`. |
| `invert` / `invertBy` | `invert` family | Swap an object's keys and values. | In `objectUtils.fs`. |
| `mapKeys` | `mapKeys` | Map a map's keys, values unchanged. | In `objectUtils.fs`. |
| `omit` / `omitBy` | `omit` family | Inverse of `pick`/`pickBy` above — all keys *except* the given ones. | In `objectUtils.fs`. |
| ~~result~~ | `result` | Like `get`, but invokes a function value found at the path. | Marginal over `getAt` plus a manual call at the call site. |

Not yet/ever:

| FeatureScript | Lodash | Does | Notes |
|---|---|---|---|
| ~~mergeWith~~ | `mergeWith` | `merge` with a custom per-key combiner. | Customizer variant of the row above; not itself ported. |
| ~~functions~~ / ~~functionsIn~~ | `functions` family | Names of an object's function-valued properties. | |
| ~~hasIn~~ | `hasIn` | `has`, including inherited properties. | |
| ~~transform~~ | `transform` | `reduce`-like, but building up a map/array accumulator. | |
| ~~valuesIn~~ | `valuesIn` | `values`, including inherited properties. | |
| ~~invoke~~ | `invoke` | Call a method found at a path. | |
| ~~forIn~~ / ~~forInRight~~ / ~~forOwn~~ / ~~forOwnRight~~ | `forIn`/`forOwn` family | Iterate an object's own vs. inherited keys. | FeatureScript maps have no prototype chain, so the own/inherited split is moot — coreUtils' `forEach` already covers the (only) own-keys case. |
| ~~assign~~ / ~~assignIn~~ / ~~assignInWith~~ / ~~assignWith~~ / ~~extend~~ / ~~extendWith~~ | `assign` family | Copy own (+ inherited) properties onto a destination object, in place. | → HUMAN-FIXME.md ("modify the subject in-place"). |
| ~~defaults~~ / ~~defaultsDeep~~ | `defaults` family | Fill in missing keys from source object(s), in place. | → HUMAN-FIXME.md. |
| ~~unset~~ / ~~update~~ / ~~updateWith~~ / ~~setWith~~ | in-place path-write family | Path-based delete/update, mutating. | → HUMAN-FIXME.md. |
| ~~create~~ | `create` | New object with a given prototype. | FeatureScript maps have no prototype chain. |


### String

| FeatureScript | Lodash | Does | Notes |
|---|---|---|---|
| `padLeft` | `padStart` | Pad a string/number on the left. | |
| `padRight` | `padEnd` | Pad on the right. | |
| `strRepeat` / `repeatString` *(std)* | `repeat` | Repeat a string `n` times. | |
| `upcase` | `toUpper` | ASCII-only uppercase. | |
| `downcase` | `toLower` | ASCII-only lowercase. | |
| `titleCase` | `startCase` | Word-boundary capitalization. | |
| `startsWith` *(std)* | `startsWith` | Whether a string starts with a substring. | |
| `endsWith` *(std)* | `endsWith` | Whether a string ends with a substring. | |
| `splitByRegexp` / `splitIntoCharacters` *(std)* | `split` | Split a string. | std splits by regexp or into characters — no plain-substring/limit split like lodash's. |
| `replace` *(std)* | `replace` | Replace matches in a string. | |
| `stringToNumber` *(std)* | `parseInt` | Parse a string as a number. | |
| `camelCase` / `kebabCase` / `lowerCase` / `snakeCase` / `upperCase` | case-convention family | Sibling case-converters to `upcase`/`downcase`/`titleCase`, for other word-casing conventions. | Built on `words` — see its note on the simplified word-boundary rule. |
| `capitalize` / `lowerFirst` / `upperFirst` | single-character case tweaks | Change just the first character's case. | |
| `pad` | `pad` | Pad on both sides. | `padLeft`/`padRight` remain the one-sided originals. |
| `trim` / `trimEnd` / `trimStart` | `trim` family | Strip whitespace (or a given character set) from either end. | |
| `truncate` | `truncate` | Truncate a string to a length, with an omission marker. | No `separator` option to break at a word/regex boundary instead of an exact count. |
| `words` | `words` | Split a string into words. | Simplified: splits only on non-alphanumeric delimiter runs, not on camelCase boundaries or digit runs the way lodash's own `words` does. |

Not yet/ever:

| FeatureScript | Lodash | Does | Notes |
|---|---|---|---|
| ~~deburr~~ | `deburr` | Strip Latin-1 diacritics. | |
| ~~template~~ / ~~escape~~ / ~~unescape~~ / ~~escapeRegExp~~ | templating family | Compile/interpolate templates; HTML/regex escaping. | → HUMAN-FIXME.md ("templating"). |


### Util

| FeatureScript | Lodash | Does | Notes |
|---|---|---|---|
| `noop` | `noop` | Does nothing, returns `undefined`. | |
| `pathForKey` | `toPath` | Split a dotted-key string into path segments. | |
| `range` *(std)* | `range` | Array of numbers from `start` to `end`. | |
| `rangeRight` | `rangeRight` | `range`, descending. | `reverse` *(std)* over std's own `range` — inherits its inclusive-of-`end` convention. |

Not Yet:


| FeatureScript | Lodash | Does | Notes |
|---|---|---|---|
| ~~attempt~~ / ~~bindAll~~ / ~~cond~~ / ~~conforms~~ / ~~constant~~ / ~~identity~~ / ~~iteratee~~ / ~~matches~~ / ~~matchesProperty~~ / ~~method~~ / ~~methodOf~~ / ~~mixin~~ / ~~noConflict~~ / ~~nthArg~~ / ~~over~~ / ~~overEvery~~ / ~~overSome~~ / ~~property~~ / ~~propertyOf~~ / ~~runInContext~~ / ~~stubArray~~ / ~~stubFalse~~ / ~~stubObject~~ / ~~stubString~~ / ~~stubTrue~~ / ~~times~~ / ~~uniqueId~~ | metaprogramming family | Build predicate/iteratee functions dynamically, generate ids, run code defensively. | → HUMAN-FIXME.md ("metaprogramming"). |
| ~~VERSION~~ | `VERSION` | lodash's own version string. | Nothing to port. |

### Date, Seq

Date functions, and the Seq API, are not contemplated to be written at the moment.

### Function / Metaprogramming

Many of these may not be possible, hold off unless needed; the `->` operator in FS may cover some of these

* ary, unary, once, flip, flow, flowRight, rest, spread, negate, overArgs, rearg, wrap
* attempt, bind, bindAll, bindKey, cond, conforms, conformsTo, curry, curryRight, iteratee, matches, matchesProperty, method, methodOf, mixin, noConflict, nthArg, over, overEvery, overSome, partial, partialRight, property, propertyOf, runInContext, stubArray, stubFalse, stubObject, stubString, stubTrue, times, uniqueId


### Other
`debugUtils.fs` and `metadataUtils.fs` have no entries in the tables above — neither has a lodash
counterpart for anything they export (Onshape-specific viewport/attribute plumbing).

---

## `arrayUtils.fs` — lodash-style array helpers

Straightforward ports from lodash's Array category; anything needing varargs takes an array of
arrays/values instead (`difference(arr, excludeArr)`, `union(arrList)`, …). Alphabetical order.

| Export | Does |
|---|---|
| `chunk(arr, chunkSize)` | Splits `arr` into `chunkSize`-length groups; the last group holds the remainder. `chunkSize < 1` → `[]`. |
| `compact(arr)` | `arr` filtered to `typeUtils.truthy` — only `undefined`/`false` drop, unlike lodash's wider falsey set. |
| `difference(arr, excludeArr)` / `differenceBy(arr, excludeArr, iteratee)` | `arr` minus anything (or anything whose `iteratee` value) appears in `excludeArr`. |
| `drop(arr, dropCount)` / `dropRight(arr, dropCount)` | `arr` with `dropCount` elements removed from the front/back. |
| `dropWhile(arr, predicate)` / `dropRightWhile(arr, predicate)` | `arr` with a leading/trailing run satisfying `predicate` removed. |
| `findIndex(arr, predicate)` / `findLastIndex(arr, predicate)` | Index of the first/last element satisfying `predicate`, or `-1`. |
| `flatten(arr)` / `flattenDeep(arr)` / `flattenDepth(arr, depth)` | Array nesting removed one level / fully / up to `depth` levels. |
| `fromPairs(pairs)` | `[[key, val], …]` → map; a repeated key keeps its last pair. |
| `initial(arr)` | `arr` without its last element. |
| `intersection(arrList)` / `intersectionBy(arrList, iteratee)` | Values (or `iteratee` results) present in every array of `arrList`, deduplicated. |
| `lastIndexOf(arr, val)` | Index of the last occurrence of `val`, or `-1`. |
| `nth(arr, seq)` | Element at `seq`; negative counts from the end; out of bounds is `undefined`. |
| `tail(arr)` | `arr` without its first element. |
| `take(arr, takeCount)` / `takeRight(arr, takeCount)` | First/last `takeCount` elements of `arr`. |
| `takeWhile(arr, predicate)` / `takeRightWhile(arr, predicate)` | Leading/trailing run of `arr` satisfying `predicate`. |
| `union(arrList)` / `unionBy(arrList, iteratee)` | Deduplicated concatenation of every array in `arrList`. |
| `uniqBy(arr, iteratee)` / `uniqWith(arr, comparator)` | `arr` deduplicated by `iteratee(val)`, or by a custom equality `comparator`. |
| `unzip(arr)` / `unzipWith(arr, iteratee)` | Ungroups `arr`'s rows into columns (an alias for std `zip`, its own inverse), optionally mapped through `iteratee`. |
| `without(arr, excludeArr)` | `arr` minus anything in `excludeArr` — identical to `difference`, under lodash's other name. |
| `xor(arrList)` | Values appearing in exactly one array of `arrList`. |
| `zipObject(keylist, valuelist)` | Map pairing `keylist`/`valuelist` by position. |
| `zipWith(arrList, iteratee)` | Std `zip` on `arrList`, each grouped row passed through `iteratee`. |

## `clxnGetset.fs` — path-based get/set on maps and arrays

Lodash's `get`/`set`/`merge`, adapted to FeatureScript's value semantics and negative-index
convention.

| Export | Does |
|---|---|
| `getAt(bag\|arr, keyStr\|keyPath\|seq, fallback?)` | Reads a dotted path (`"a.b.c"`), a literal key list (`["a.b"]`), or a single array index (negative counts from the end). Missing at any step returns `fallback` (default `undefined`); a present-but-`undefined` array element is *not* the fallback. |
| `setAt(bag\|arr, keyStr\|keyPath\|seq, val, onCollision?)` | Same path reading as `getAt`, but writes: autovivifies missing steps as maps, replaces a scalar in the way, and pads an array written past its end with `undefined`. `onCollision(existing, incoming)` resolves a leaf that's already occupied — default is `lastInWins`. |
| `deepMerge(existing, incoming)` | Lodash's deep merge: two maps combine key by key all the way down; anything else, `incoming` wins; an `undefined` `incoming` leaves `existing` alone. Arrays replace whole (no index-by-index merge). The `onCollision` to pass `setAt` for merge-on-write. |
| `pathForKey(keyStr)` | Splits a dotted key into segments (`"a..b"` → `["a", "", "b"]`); a key with a terminal dot is unhandled and drops that last empty segment. |
| `lastInWins` | The default `onCollision`: `(existing, incoming) => incoming`. |
| `GetsetAtStep.MISSING` | Internal sentinel for "nothing at this step" — never returned to a caller. |

## `clxnReshape.fs` — dotted keys ↔ nested maps, and choice-tree building

| Export | Does |
|---|---|
| `undotMap(obj, onCollision?)` | Expands `obj`'s dotted top-level keys into nested maps (`{"a.b": 1, "a": {"c": 2}}` → `{"a": {"b": 1, "c": 2}}`). Applies shallowest-key-first so the result doesn't depend on map iteration order. `onCollision` resolves a leaf two keys both reach — default `deepMerge`, so two maps landing on the same spot combine rather than one replacing the other. Only `obj`'s own keys are split; dots already inside a value's keys are left alone. |
| `dotMap(obj, options?)` | The inverse: depth-first flattens a nested map into dotted keys. `options.maxDepth` caps how many levels collapse (lodash's `flattenDepth` semantics); arrays and empty maps are leaves. Lossy against a key that already contains a dot — `undotMap(dotMap(x))` isn't always `x`. |
| `buildNestedChoices(levels, tree)` | Turns a raw uniformly-nested map into the `{name, displayName, entries}` structure a custom-feature enum dialog wants, given a `levels` list like `['socket_kind', ['drive_kind', {inthex: 'Int Hex'}]]` (name, optional display name, optional key-translation map per level). |

## `clxnWalking.fs` — inspecting, iterating, and reshaping maps and arrays

The workhorse file: `forEach` and `mapValues`/`mapValues3` are what most other collection code
in this project is built from.

| Export | Does |
|---|---|
| `sizeof(val)` | Key count (map) / character count (string) / element count (array) / `0` (`undefined`). |
| `hasKey(obj, key, missingPolicy?)` | Whether `key` resolves to something in `obj`. For a map this always means present-and-not-`undefined` (the `missingPolicy` overload exists only for symmetry and never changes the answer). For an array, `MissingPolicy.USE_UNDEFINED` (the default) checks only that the index is in bounds; `SKIP` also requires the slot not be `undefined`. Neither array overload accepts a negative index, unlike `getAt`. |
| `hasPresentKey(obj, key)` | `hasKey` pinned to the strict policy: in bounds (or present) *and* not `undefined`. |
| `arrayIncludes(arr, target)` | Whether `target` appears anywhere in `arr` (`==`, so a deep match on maps/arrays too). |
| `pick(bag, keylist)` | A map of just `bag`'s entries at `keylist`; an absent key is elided, not set to `undefined`. |
| `pickDefined(bag, keylist)` | Same, skipping a key whose value is `undefined` — behaviorally identical to `pick` on a map, since FeatureScript elides `undefined` on write either way. |
| `arrLast(arr)` | Last element of `arr`, or `undefined` if empty. |
| `valuesAt(bag\|arr, keylist, missingPolicy?)` | Array of values at `keylist`, in that order — the plural cousin of a single lookup, with no path traversal. `USE_UNDEFINED` (default) keeps the result the same length as `keylist`; `SKIP` drops missing slots. Array indexes must be non-negative and in bounds. |
| `forEach(bag\|arr, keylist?, missingPolicy?, func)` | Visits every entry, calling `func(val, key, seq)` (map) or `func(val, seq)` (plain array) / `func(val, seq, seq)` (array + `missingPolicy`) — `seq` is always a 0-based visit count. Returning `NextStepAction.BREAK` from `func` stops the walk immediately. `missingPolicy` (`USE_UNDEFINED` default / `SKIP`) decides whether an `undefined`-valued entry is visited at all. A `keylist` walks exactly those keys, in that order, instead of `keys(bag)`. |
| `mapValues(bag\|arr, ...)` / `mapValues3(...)` | `forEach`, collecting `func`'s return value back into a same-shaped map/array. `mapValues3` adds the trailing `seq` to the callback; both take the same `keylist`/`missingPolicy` overloads as `forEach`. |
| `rebag(bag\|arr, func)` | Rebuilds a map by asking `func(val, key?, seq)` for the `[newKey, newVal]` pair each entry becomes; `func` returning `undefined` drops that entry. Later entries win a `newKey` collision. |
| `objectify(arr, func)` | `rebag` specialized so an array's own values become the result's keys: `objectify(arr, func)` ≡ `rebag(arr, (val, seq) => [val, func(val, seq)])`. |
| `boxarrPush(arrRef, val)` / `boxarrUnshift(arrRef, val)` | Append / prepend `val` into the array inside a `box`, in place, returning `val` — for accumulating into an outer array from inside a `forEach`/`mapValues` callback. |
| `MissingPolicy` | `SKIP` / `USE_UNDEFINED` — how the functions above treat an entry whose value is `undefined`, whether that's because it's missing outright or genuinely set to `undefined`. |
| `NextStepAction.BREAK` | Sentinel a `forEach`-family callback returns to stop the walk early. |
| `countBy(arr, iteratee)` | Map of `iteratee(val)` results to how many elements produced each one. |
| `find(arr, predicate)` / `findLast(arr, predicate)` | First/last element for which `predicate` holds, or `undefined` — `arrayUtils`' `findIndex`/`findLastIndex`, dereferenced. |
| `flatMap(arr, iteratee)` / `flatMapDeep(arr, iteratee)` / `flatMapDepth(arr, iteratee, depth)` | `arr` mapped through `iteratee`, then flattened one level / fully / `depth` levels, via `arrayUtils`' `flatten` family. |
| `forEachRight(arr, func)` | `forEach`, back-to-front — same `NextStepAction.BREAK` early exit. |
| `groupBy(arr, iteratee)` | Map of `iteratee(val)` results to the elements that produced each one, via std's `insertIntoMapOfArrays`. |
| `partition(arr, predicate)` | `[passed, failed]` — `arr` split by whether `predicate` holds, each half keeping `arr`'s order. |
| `reduceRight(arr, seed?, foldFunction)` | `foldArray` *(std)*, back-to-front, over a `reverse`d copy of `arr`. |
| `reject(arr, predicate)` | Elements of `arr` for which `predicate` does *not* hold — the inverse of `filter` *(std)*. |

## `stringUtils.fs` — string slicing, padding, and case conversion

| Export | Does |
|---|---|
| `strSlice(str, begseq, endseq?)` | JS-style slice: negative indexes count from the end, either index clamps into range. `endseq` accepts `SequencePosition.END` in place of a literal length. |
| `strTake(str, len)` / `strTakeRight(str, len)` | First/last `len` characters; `len <= 0` is `""`. |
| `padLeft(str\|num, minlen, padstr?)` | Pads to `minlen` with `padstr` (default `" "`), truncating the padding if it overshoots; a `number` overload stringifies first. |
| `padRight(str, minlen, padstr?)` | Same as `padLeft`, right-padded — but no numeric overload; a `number` must be stringified by the caller. |
| `strRepeat(str, reps)` | `str` repeated `reps` times. |
| `starbanner(str)` | Wraps `str` in a `***`-bordered banner, for a `debug()` call that wants to stand out. |
| `hasMatch(str, regex)` | Whether `regex` matches anywhere in `str`; `false` (not a throw) on `undefined` input or a bad pattern. |
| `upcase(str)` / `downcase(str)` | ASCII-only case flip via an explicit character lookup; a non-letter passes through unchanged. |
| `titleCase(str, opts?)` | Splits on `opts.spaces` (default `-_`), capitalizes each word's first letter, lower-cases the rest; `opts.tr` translates individual characters before capitalization. |
| `words(str)` | `str` split into words on runs of non-alphanumeric characters — the shared primitive behind the case-convention functions below. |
| `camelCase(str)` / `kebabCase(str)` / `lowerCase(str)` / `snakeCase(str)` / `upperCase(str)` | `words`, rejoined without separators / with `-` / with a space / with `_` / with a space (case-converted per convention). |
| `capitalize(str)` | First character uppercased, the rest lowercased. |
| `upperFirst(str)` / `lowerFirst(str)` | Only the first character's case changed; everything else left as-is. |
| `pad(str, minlen, padstr?)` | Pads both sides if shorter than `minlen`, splitting as evenly as possible (right side gets the extra character when odd). |
| `trimStart(str, chars?)` / `trimEnd(str, chars?)` / `trim(str, chars?)` | Strips `chars` (default whitespace) from the front / back / both ends. |
| `truncate(str, opts?)` | `str` cut to at most `opts.length` (default `30`) characters, omission marker included, replacing the cut tail with `opts.omission` (default `"..."`). |

## `metadataUtils.fs` — entity names and attributes

Everything here but the last three rows needs a live `Query` against real geometry — there's no
plain-data test for them; @see the Tests section above.

| Export | Does |
|---|---|
| `setPropAndAttribute(context, entities, propType, attrName, value)` | Sets a property and mirrors it into a same-named attribute, since Onshape won't let a feature read its own properties back mid-regeneration — only the attribute survives that round trip. |
| `setName(context, entities, nameText)` | `setPropAndAttribute` for `PropertyType.NAME` / `"Name"`. |
| `setReadableName(context, entities, nameText, maxLength?)` | `setName`, after collapsing whitespace runs to one space and truncating to `maxLength` (default `20`). |
| `getAttrs(context, query, attrName, defaultVal)` | `{ thing, attrName, val }` for every entity in `query`, `val` falling back to `defaultVal` where the attribute is unset. |
| `getAllAttrs(context, entity)` | Every attribute on one entity, as `{name: value}`; `{"ok": false, "err": err}` if the read throws. |
| `getBestAttr(context, query, attrName, ignoredVal)` | The first `getAttrs` entry whose value isn't `ignoredVal`, or the first entry if every one of them is; `undefined` if `query` is empty. |
| `getNameProps` / `getNames` / `getName` / `getNameProp` | `getAttrs`/`getBestAttr`, specialized to `"Name"`. `getName` falls back to `ignoredVal` (default `"Part"`) when `query` resolves to nothing, rather than dereferencing `undefined`. |
| `getNameOfBody(context, body, defaultVal)` | `"Name"` attribute directly on `body`, not its best/first entity. |
| `defaultMaybe(oldDefinition, newDefinition, basekey, destkey, valfunc)` | Value for `newDefinition[destkey]`: kept auto-derived from `newDefinition[basekey]` via `valfunc` for as long as it hasn't been hand-edited away from what `valfunc` would have produced from the old base. The pattern behind every `*EditLogic` function that keeps a variable name in sync with what it names. |
| `sanitize_varname(varname)` / `field_varname(varname, fieldname)` | Turns `varname` into a legal-ish identifier (each `.` → `_`, every other non-word character → `__`, independently — no run-collapsing); `field_varname` joins two of them with `_`. |
| `PL_TOP`, `hugeSizeVal` / `tinySizeVal` | Shorthand top-plane constant; sentinel min/max for a `LengthBoundSpec` with no practical limit. |

## `miscUtils.fs` — small combinators

| Export | Does |
|---|---|
| `idsFor(id, tags)` | `{tag: id + tag}` for each of `tags` — the `ids` map a multi-sketch/multi-op feature declares up front, built in one call. |
| `noop` | Returns `undefined`, regardless of arguments. |
| `curry3to0` / `curry3to1` / `curry3to2` / `curry2to0` / `curry2to1` / `curry2to2` | Wraps `func` to accept `N` arguments but call it with only the first `M`, so a fixed-arity callback can sit in a `forEach`/`mapValues`-shaped slot. |
| `parseJsonSafely(rawjson, opts?)` | `parseJson`/`parseJsonWithUnits` (per `opts.detectUnits`), wrapping a parse failure in a `regenError` labeled with `opts.story` instead of surfacing the raw throw. |
| `rangeRight(from, to)` | `range` *(std)*, descending, via a plain `reverse`. |

## `numberUtils.fs` — range checks and iteratee-mapped math reductions

| Export | Does |
|---|---|
| `inRange(num, start?, end)` | Whether `num` falls in `[start, end)`; `start` defaults to `0`; bounds auto-swap if `start > end`. |
| `maxBy(arr, iteratee)` / `minBy(arr, iteratee)` | Element of `arr` for which `iteratee(val)` is greatest/least, or `undefined` for an empty `arr` — std's array `max`/`min` pick the value itself, these pick the element behind it. |
| `meanBy(arr, iteratee)` / `sumBy(arr, iteratee)` | `average`/`sum` *(std)*, iteratee-mapped. |

## `objectUtils.fs` — lodash-style map helpers

Straightforward ports from lodash's Object category, beyond what `clxnGetset.fs`/`clxnWalking.fs`
already cover (`getAt`/`setAt`/`pick`/`mapValues`/`hasKey`/…).

| Export | Does |
|---|---|
| `findKey(bag, predicate)` / `findLastKey(bag, predicate)` | First/last key of `bag` (in `keys(bag)` order) whose value satisfies `predicate(val, key)`. |
| `invert(bag)` | `bag` with keys and values swapped; a repeated value keeps only its last key. A non-string value is stringified into its new key. |
| `invertBy(bag, iteratee)` | `invert`, grouping every key (not just the last) under `iteratee(val)`, via std's `insertIntoMapOfArrays`. |
| `mapKeys(bag, iteratee)` | `bag`'s values, keyed by `iteratee(val, key)` instead of `key`. |
| `omit(bag, keylist)` | `bag` without the entries at `keylist` — the inverse of `pick`. |
| `omitBy(bag, predicate)` | `bag` without any entry for which `predicate(val, key)` holds. |
| `toPairs(bag)` | `bag` flattened into `[[key, val], …]` pairs — the inverse of `fromPairs` *(arrayUtils)*. |

## `typeUtils.fs` — presence checks and small guards

| Export | Does |
|---|---|
| `ifNil(val, fallback)` | `val`, or `fallback` if `val` is `undefined`. |
| `ifBlank(val, fallback)` | `val`, or `fallback` if `val` is `undefined` or `""`. |
| `ifZero(val, fallback)` | `val`, or `fallback` if `val` is `undefined` or (within tolerance) zero; overloaded for `number` and `ValueWithUnits`. |
| `truthy(val)` | Neither `undefined` nor `false` — `0` and `""` are truthy, unlike JS. |
| `isPresent(val)` / `isNil(val)` | Not-`undefined` / is-`undefined`. |
| `strBlank(val)` | `undefined` or `""`. |
| `isEmpty(val)` | `undefined`, or a map/string/array with nothing in it. |
| `castArray(val)` | `val` unchanged if it's already an array, otherwise `[val]`. |
| `vector2(vec)` | Drops `vec`'s z component. |
| `mm`, `zero` | `millimeter`, and `0 * mm`. |

## `debugUtils.fs` — viewport highlighting

| Export | Does |
|---|---|
| `highlightQuery(context, qq, debugColor, debugMe?)` | `addDebugEntities` on `qq` plus the edges of its owning bodies (otherwise invisible through an occluding body's faces); a no-op unless `debugMe` is `true` (the default), so a call site can leave the call in place and flip one flag. |

## `colorUtils.fs` — Color ↔ hex/tuple string conversion

No lodash correspondence — color parsing isn't something lodash does.

| Export | Does |
|---|---|
| `setColor(context, qq, cmap)` | Sets the `APPEARANCE` property on `qq` to `cmap` — a `Color`, or anything `toColor` accepts. |
| `hexcolorToColor(hexcolor)` | Parses `"#rrggbb"`/`"#rrggbbaa"` (leading `#` optional, either case) into a `Color`. |
| `tuplestrToColor(tuplestr)` | Parses a 0–255 RGB(A) tuple string (`"200,99,100,33"` or `"[0, 1, 255]"`) into a `Color`. |
| `unitcolorToColor(unitcolor)` | Parses a 0.0–1.0 RGB(A) tuple string (`"0.5, 0.1, 1.0"`) into a `Color`. |
| `toColor(cmap\|carr\|raw)` | Dispatches to whichever of the above fits: a `Color` passes through, an array goes straight to `color(...)` (no 0–255 detection), a string is sniffed hex → unit → tuple, first match wins. `OopsColor` (bright red) if nothing matches. |
| `toHexcolor` / `toUnitcolor` / `toTuplecolor` | The reverse conversions, `Color` → string/array. |
| `hexpairToInt(hexpair, fallback?)` / `intToHexpair(num)` | 2-character hex string ↔ 0–255 decimal, case-insensitive going in. |
| `sameColor(c1, c2, tol?)` | Channel-by-channel tolerant equality; default `tol` is just under `1/255`. |
| `isHexcolor(str)` | Whether `str` is a hexcolor string `hexcolorToColor` would accept. |

## `jsonVarF.fs` — variable-producing utility Features

A collection of Features, not a library — each `defineFeature` body reads/writes variables on a
live `Context`, so (per this pass's scope) none of them have plain-data tests; only docblocks and
style got the treatment here.

| Export | Does |
|---|---|
| `jsonVarF` | Parses `definition.rawjson` and sets it as variable `varname`. |
| `keylistF` / `keylistEditLogic` | Sets `varname` to the ordered keys of variable `bagname`. Edit logic keeps `varname` at `bagname ~ "_keys"` until hand-edited. |
| `sizeofF` / `sizeofEditLogic` | Sets `varname` to `sizeof` of variable `objname`. Edit logic keeps `varname` at `objname ~ "_size"` until hand-edited. |
| `valuesAtF` / `valuesAtEditLogic` | Sets `varname` to `valuesAt(bag, keylist)` for variable `bagname` and a JSON-array `keylistJSON`. Edit logic tracks both `varname` and `description`. |
| `splatF` | Explodes variable `objname`'s value into one variable per entry — `<objname>_<00-padded index>` for an array, `<objname>_<key>` (or bare `<key>`) for a map. |

Open design questions found while documenting this file are tracked in
[`../HUMAN-FIXME.md`](../HUMAN-FIXME.md), not here.

---

## Bugs found and fixed along the way

* **`clxnWalking.fs`:** `NextStepAction` (the `forEach`/`mapValues`-family early-exit sentinel)
  was never `export`ed, so no caller outside the file could ever actually trigger `BREAK` — every
  walk anywhere in the codebase always ran to completion. Exported now, and covered by
  `runForEachBreakTests`/`runForEachKeylistBreakTests` in `tests/testClxnWalking.fs`.
* **`tests/testStringUtils.fs`:** `runPadTests` had four `return runTests(...)` statements
  stacked in a row — only the first ever ran, so `padRight` was never tested at all, and the more
  interesting padding-behavior cases (`LeftPadTestCases`/`RightPadTestCases`) silently never
  executed either.
* **`typeUtils.fs`:** `ifZero` had two dead, unreachable private overloads after the real
  (exported) ones — one untyped, one with the exact same `(val is ValueWithUnits, fallback)`
  signature as the exported version above it, which is either a silent duplicate-definition or a
  compile error depending on how FeatureScript resolves it. Deleted both.
* **`metadataUtils.fs`:** `getName` called `.val` on whatever `getNameProp` returned without
  checking for `undefined` first — and `getNameProp`/`getBestAttr` return `undefined` exactly
  when `query` resolves to no entities, which is not a rare case. Fixed to fall back to
  `ignoredVal` in that case, matching every sibling function's documented contract.
* **`colorUtils.fs`:** `HexcolorRE` only matched lowercase hex digits (`[0-9a-f]`), so
  `hexcolorToColor`, `toColor`, and `isHexcolor` all silently failed on a perfectly standard
  uppercase or mixed-case hex string like `"#FF0000"` — falling back to `OopsColor` (or `false`
  for `isHexcolor`) instead of parsing it. Widened to `[0-9a-fA-F]`; `hexpairToInt` already
  downcased its input, so nothing else needed to change.
* **`jsonVarF.fs`:** `keylistF`'s throw message named `definition.varname` (the *destination*
  variable being written) when reporting that the *source* variable wasn't a bag or array — a
  debugging-time red herring, since the name in the error never matched the variable actually at
  fault. Now names `bagname`, the variable that was actually checked.
* **`tests/testColorUtils.fs`:** the whole file was replaced — `runColorRoundtripTests` wrapped
  its body in a bare `try { }` with no `catch`, which isn't valid FeatureScript (only `try silent
  { }` and `try { } catch (error) { }` are), and even if it had compiled, the mismatches it
  collected into `oops` were never reported or asserted on — the function always returned `{}`
  regardless of what it found. On top of that, the Feature itself was exported as
  `runStringUtilsTestsFS`, a copy-paste of `testStringUtils.fs`'s Feature name rather than its own
  — a name collision waiting to happen the day both files' symbols land in the same scope.
  Replaced with table-driven `runTests`-style coverage for every pure function in the file.
