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

| Notation                          | Means                                                                                                                                                                                                                                        |
| --------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| a plain name (`getAt`, `pick`, …) | implemented in this project's coreUtils — see the file-by-file catalogs further down                                                                                                                                                         |
| `name` *(std)*                    | a FeatureScript standard-library builtin (`math.fs`/`string.fs`/`containers.fs`) covers it, no port needed                                                                                                                                   |
| `` `is array` `` etc. *(lang)*    | built into the FeatureScript language itself (a type-check expression or operator), not a function call                                                                                                                                      |
| ~~name~~                          | nothing implements it, anywhere — a real (if low-priority) porting candidate                                                                                                                                                                 |
| a struck basket + "→ Not Planned" | consciously not planned; the full list is tabulated in [Not Planned](#not-planned) at the very bottom of this document, with the harder-to-summarize reasons (templating, chaining, async) spelled out in [`HUMAN-FIXME.md`](HUMAN-FIXME.md) |

A function can have both a coreUtils port *and* a std builtin that inspired it, and the two can
differ in how much they actually do — `merge` is the poster child: coreUtils' `deepMerge`
recurses through nested maps the way lodash's `merge` does, while std's own `mergeMaps` is a
shallower, "less functional" one-level combine. Both are noted on that row.

### Array

| FeatureScript                                         | Lodash   | Does                                                             | Notes                                                                                                                                                                                        |
| ----------------------------------------------------- | -------- | ---------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `concatenateArrays` *(std)*                           | `concat` | Concatenates array(s)/value(s) into a new array.                 | std is arrays-only — no flattening of bare scalar arguments the way lodash does.                                                                                                             |
| `first` *(std)*                                       | ≡        | First element of an array.                                       | Lodash also spells this `head` — see [Lodash aliases](#lodash-aliases).                                                                                                                      |
| `indexOf` *(std)*                                     | ≡        | Index of the first occurrence of a value.                        |                                                                                                                                                                                 |
| `join` *(std)*                                        | ≡        | Joins array elements into a string.                              |                                                                                                                                                                                 |
| `last` *(std)* / `arrLast`                            | `last`   | Last element of an array.                                        | Both std's `last` and coreUtils' `arrLast` cover this directly.                                                                                                                              |
| `removeElementAt` *(std)*                             | `pullAt` | Removes element(s) by index.                                     | std takes one index at a time and doesn't hand back what it removed, unlike lodash's variadic, removed-values-returning form.                                                                |
| `reverse` *(std)*                                     | ≡        | Reverses an array.                                               |                                                                                                                                                                                 |
| `subArray` *(std)*                                    | `slice`  | Slice of an array between two indices.                           |                                                                                                                                                                                 |
| `deduplicate` *(std)*                                 | `uniq`   | Duplicate-free version of an array.                              |                                                                                                                                                                                 |
| `zip` *(std)*                                         | ≡        | Groups the nth elements of several arrays together.              |                                                                                                                                                                                 |
| `chunk`                                               | ≡        | Splits an array into groups of `chunkSize`.                      |                                                                                                                                                                                              |
| `compact`                                             | ≡        | Removes falsey values from an array.                             | Narrower than lodash: only `undefined`/`false` are falsey in FeatureScript, so `0` and `""` survive.                          |
| `difference` / `differenceBy`                         | ≡        | Values in one array not present in another.                      | No varargs, so the exclusion values are one array, not trailing arguments — concatenate first to exclude from several sources. ~~differenceWith~~ not ported (marginal over `differenceBy`). |
| `drop` / `dropRight` / `dropRightWhile` / `dropWhile` | ≡        | Drop `n` elements (or while a rule holds) from either end.  |                                                                                                                                                                                              |
| `findIndex`                                           | ≡        | Index of the first element matching a rule.                 | No `fromIndex` argument.                                                                                                                                                                     |
| `findLastIndex`                                       | ≡        | Index of the last element matching a rule.                  | No `fromIndex` argument.                                                                                                                                                                     |
| `flatten` / `flattenDeep` / `flattenDepth`            | ≡        | Flatten an array one level / fully / `n` levels deep.            | `dotMap`'s `maxDepth` (coreUtils) is the map-shaped cousin, not array-shaped.                                                                                                                |
| `fromPairs`                                           | ≡        | `[[k,v], …]` → map.                                              |                                                                                                                                                                                              |
| `initial`                                             | ≡        | All but the last element.                                        |                                                                                                                                                                                              |
| `intersection` / `intersectionBy`                     | ≡        | Values present in every given array.                             | Takes an array of arrays (no varargs). ~~intersectionWith~~ not ported.                                                                                                                      |
| `lastIndexOf`                                         | ≡        | Index of the last occurrence of a value, searching from the end. | No `fromIndex` argument.                                                                                                                                                                     |
| `nth`                                                 | ≡        | Nth element (negative counts from the end).                      |                                                                                                                                                                                              |
| ~~sortedUniq~~ / ~~sortedUniqBy~~                     | ≡        | `uniq`/`uniqBy`, optimized for sorted input.                     | Redundant with `uniqBy`/std `deduplicate` at this project's scale — the "sorted" optimization buys nothing here.                                                                             |
| `tail`                                                | ≡        | All but the first element.                                       |                                                                                                                                                                                              |
| `take` / `takeRight` / `takeRightWhile` / `takeWhile` | ≡        | Take `n` elements (or while a rule holds) from either end.  | `strTake`/`strTakeRight` (coreUtils) cover the string analogue, not arrays.                                                                                                                  |
| `union` / `unionBy`                                   | ≡        | Deduplicated concatenation of several arrays.                    | Takes an array of arrays (no varargs). ~~unionWith~~ not ported.                                                                                                                             |
| `uniqBy` / `uniqWith`                                 | ≡        | `uniq`, func-mapped / custom comparator.                     | Plain `uniq` is std's `deduplicate`.                                                                                                                                                         |
| `unzip` / `unzipWith`                                 | ≡        | Inverse of `zip`.                                                | `zip`'s grouping is its own inverse, so `unzip` is a bare alias for std `zip`.                                                                                                               |
| `without`                                             | ≡        | Values from an array excluding the given ones.                   | Identical to `difference` once variadic exclusion values collapse to one array — kept as its own name for lodash parity.                                                                     |
| `xor`                                                 | ≡        | Symmetric difference of several arrays.                          | Takes an array of arrays (no varargs). ~~xorBy~~ / ~~xorWith~~ not ported.                                                                                                                   |
| `zipObject` / `zipWith`                               | ≡        | Build an object, or combine grouped elements with a function.    | `zipWith` and `unzipWith` are the same shape of operation, both riding on `zip`'s self-symmetry. ~~zipObjectDeep~~ not ported (needs path-based nested writes, low value here).              |
| ~~sortedIndex~~ / ~~sortedIndexBy~~ / ~~sortedIndexOf~~ / ~~sortedLastIndex~~ / ~~sortedLastIndexBy~~ / ~~sortedLastIndexOf~~ | `sortedIndex*` family | Binary-search insertion points into an already-sorted array. | std's `sort(arr, compareFunction)` makes this possible for numbers, but the 6-function family is a lot of surface for an optimization this codebase has no hot path for. |

`pull`/`pullAll`/`pullAllBy`/`pullAllWith`, `remove`, and `fill` all mutate their array argument in
place in lodash — see [Mutating (in-place) Functions](#mutating-in-place-functions), below.

### Collection

| FeatureScript                              | Lodash                       | Does                                                  | Notes                                                                                          |
| ------------------------------------------ | ---------------------------- | ----------------------------------------------------- | ---------------------------------------------------------------------------------------------- |
| `forEach`                                  | ≡                             | Iterate a collection.                                 | Lodash also spells this `each` — see [Lodash aliases](#lodash-aliases). |
| `mapValues3` (array form)                  | `map`                        | Map a collection to a same-shaped result.             |                                                                                                |
| `arrayIncludes`                            | `includes`                   | Whether a value appears in a collection.              | Map or array; no substring search on a string.                                                 |
| `objectify`                                | `keyBy`                      | Key a collection by a computed key.                   | Key/value roles swapped from lodash — see the file-by-file table below.                        |
| `all` *(std)*                              | `every`                      | Whether every element passes a rule.             |                                                                                                |
| `any` *(std)*                              | `some`                       | Whether any element passes a rule.               |                                                                                                |
| `filter` *(std)*                           | ≡                            | Elements passing a rule.                         |                                                                                                |
| `foldArray` *(std)*                        | `reduce`                     | Reduce a collection to a single value.                |                                                                                                |
| `sizeof` / `size` *(std)*                  | `size`                       | Element/key/character count.                          |                                                                                                |
| `countBy`                                  | ≡                            | Counts of elements, grouped by a computed key.        | Map or array, like the rest of this section.                                                   |
| `find` / `findLast`                        | ≡                            | First/last element matching a rule.              | Array form dereferences `arrayUtils`' `findIndex`/`findLastIndex`; map form walks `keys(bag)`. |
| `flatMap` / `flatMapDeep` / `flatMapDepth` | ≡                            | Map then flatten one/all/`n` levels.                  | Rides on `arrayUtils`' `flatten` family.                                                       |
| `forEachRight`                             | ≡                            | Iterate a collection back-to-front.                   | Lodash also spells this `eachRight` — see [Lodash aliases](#lodash-aliases). Map form walks `keys(bag)` in reverse. |
| `groupBy`                                  | ≡                            | Group elements by a computed key.                     | Built on std's `insertIntoMapOfArrays`.                                                        |
| `partition`                                | ≡                            | Split into two groups by a rule.                 |                                                                                                |
| `reduceRight`                              | ≡                            | `reduce`, back-to-front.                              | `foldArray` *(std)* over a `reverse`d array.                                                   |
| `reject`                                   | ≡                            | Elements *failing* a rule — inverse of `filter`. |                                                                                                |

Possible, not yet built — both need a workaround rather than a missing capability:

| ~~sample~~ / ~~sampleSize~~ / ~~shuffle~~ | `sample` family | Random element(s) / shuffled order. | Needs a seeded pseudo-random generator — FeatureScript has no entropy source, but a plain linear-congruential generator seeded by a caller-supplied number is just arithmetic. Same workaround `random` (Number, below) is waiting on. |
| ~~sortBy~~ / ~~orderBy~~ | ≡ | Sort by one or more iteratees, each with its own direction. | std `sort(arr, compareFunction)` takes an arbitrary comparator, so a *numeric* func already sorts today (`sort(arr, (a, b) => func(a) - func(b))`); a *string* func needs a character-ordinal lookup table first — the same technique `upcaseChar`/`downcaseChar` (`stringUtils.fs`) already use — since FeatureScript has no `<`/`>` for strings. |

### Function

FeatureScript functions are already first-class values with real closures — `miscUtils.fs`'s
`curry2to0`…`curry3to2` close over `func`, and `boxarrPush` closes over a mutable `box` — so most
of lodash's Function category turns out to be a short closure away, once you stop assuming it
needs JS's variadic-argument or `this`-binding machinery. The two genuine gaps are things that
need *variadic* calling (no `arguments`-style rest args in FeatureScript) and things that need
*`this`/method* binding (no OOP here) — both filed under [Not Planned](#not-planned) at
the bottom of this document, instead of repeated per-row here.

| FeatureScript | Lodash | Does | Notes |
|---|---|---|---|
| `memoizeFunction` *(std)* | `memoize` | Cache a function's results by argument. | |
| `curry2to0`…`curry3to2` *(existing, `miscUtils.fs`)* | `unary`, `ary` | Call a function with only its first `N` of `M` arguments. | Already covers `unary` (`curryXto1`) and the common `ary` shapes; a new arity just needs one more `curryNtoM` sibling, not a general variadic `ary`. |

Possible, not yet built — a closure over a plain value or a `box` (for call-count/cache state)
covers all of these; none need anything FeatureScript doesn't already have:

| FeatureScript              | Lodash                    | Does                                                                      | Notes                                                                                                                                                                                          |
| -------------------------- | ------------------------- | ------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `flip`                     | ≡                         | Swap a 2-argument function's argument order.                              | `(func) => (a, b) => func(b, a)`.                                                                                                                                                              |
| `flow` / `flowRight`       | ≡                         | Compose an array of unary functions, left-to-right / right-to-left.       | `foldArray` *(std)* over the function array, seeded with the input.                                                                                                                            |
| `negate`                   | ≡                         | Boolean-invert a rule's result.                                      | `(pred) => (...) => !pred(...)`, one per arity already in use.                                                                                                                                 |
| `once`                     | ≡                         | Call `func` at most once; return the first result on every later call.    | Needs a `box` to remember "already called" plus the cached result — the same closure-over-`box` idiom `boxarrPush` already uses, just holding a flag instead of an array.                      |
| `before` / `after`         | ≡                         | Call `func` only until / starting at the `n`th call.                      | Same `box`-counter idiom as `once`. (An earlier pass filed these under "related to Date" in HUMAN-FIXME.md — that was a miscategorization; they're plain call-count wrappers.)                 |
| `partial` / `partialRight` | ≡                         | Fix some leading/trailing arguments, return a function awaiting the rest. | The value-fixing sibling of `curryNtoM` (which drops args instead of fixing them) — same fixed-arity-per-shape approach. Also covers `wrap` (`wrap(value, fn)` ≡ `partial(fn, value)`).        |
| ~~curry~~ / ~~curryRight~~ | ≡                         | General auto-curry of any arity.                                          | `curry2to0`…`curry3to2` already solve the one shape this project needed; a fully generic curry needs variadic arity, which FeatureScript doesn't have — low value beyond what's already there. |

### Lang

| FeatureScript                            | Lodash                           | Does                                         | Notes                                                                                                                                                                 |
| ---------------------------------------- | -------------------------------- | -------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `isNil`                                  | `isNil`, `isNull`, `isUndefined` | Is the value absent?                         | FeatureScript has no `null`, so all three JS distinctions collapse into one `undefined` check — also expressible as the language operator `== undefined`.             |
| `isEmpty`                                | ≡                                | Is the value empty?                          | Restricted to the map/string/array/`undefined` cases FeatureScript actually has.                                                                                      |
| `ifNil`                                  | `defaultTo`                      | Value, or a fallback if nil.                 |                                                                                                                                                                       |
| `==` *(lang)*                            | `isEqual`, `isEqualWith`         | Deep structural equality.                    | FeatureScript's `==` already does deep value comparison on maps/arrays — this is the default equality operator, not a function.                                       |
| `>` / `>=` / `<` / `<=` *(lang)*         | `gt`, `gte`, `lt`, `lte`         | Numeric comparison.                          | Plain comparison operators, numbers only — no lodash-style mixed-type coercion.                                                                                       |
| `toString` *(std)*                       | ≡                                | Converts a value to its string form.         |                                                                                                                                                                       |
| `isInteger` *(std, `math.fs`)*           | ≡                                | Whether a number has no fractional part.     | Already a `math.fs` builtin — just missing from this table until now.                                                                                                 |
| `castArray` *(existing, `typeUtils.fs`)* | ≡                                | Wrap a non-array value in a 1-element array. | Already implemented — earlier passes of this doc mis-filed it as unported below.                                                                                      |
| `toNumber`                               | ≡                                | Coerce a value to a number.                  | Mostly covered already: `stringToNumber` *(std)* handles the one real case (parsing a numeric string) — nothing else in this codebase needs coercing toward a number. |
| `` `is array`/`is map`/`is string`/`is number`/`is boolean`/`is function` `` *(lang)* | `isArray`, `isBoolean`, `isFunction`, `isMap`, `isObject`, `isPlainObject`, `isNumber`, `isString` | Type-check a value.                  | FeatureScript's `is <Type>` is a language expression, not a function call — this is the basket the styling of `isArray … isUndefined` in this doc's own intro refers to. |

Possible, not yet built:

| FeatureScript                           | Lodash                                  | Does                                 | Notes                                                                                                                                                                    |
| ---------------------------------------- | ---------------------------------------- | ------------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| ~~isMatch~~ / ~~isMatchWith~~ | ≡ | Whether an object has the same values as a partial "source" object, at the source's keys. | `pick(obj, keys(source)) == source` covers `isMatch` today, since `==` (row above) is already deep equality; `isMatchWith`'s per-key customizer is the one open piece. |
| ~~toInteger~~ | ≡ | Coerce a value to an integer. | A one-line `floor`/`round` over `toNumber` (above). |

### Math

| FeatureScript          | Lodash       | Does                    | Notes                                                                         |
| ---------------------- | ------------ | ----------------------- | ----------------------------------------------------------------------------- |
| `ceil` *(std)*         | ≡            | Round up.               |                                                                               |
| `floor` *(std)*        | ≡            | Round down.             |                                                                               |
| `round` / `roundToPrecision` *(std)*   | `round`      | Round to the nearest integer, or `n` decimal places. | lodash's optional precision argument becomes a second, separate std function. |
| `min` *(std)*          | ≡            | Smaller of two values.  | std offers `min(aa, bb)` and `min([value])`; value must be comparable (number, ValueWithUnits, etc). See cmp and minBy/maxBy |
| `max` *(std)*          | ≡            | Larger of two values.   | Same differences as `min`.                                       |
| `average` *(std)*      | `mean`       | Average of an array.    |                                                                               |
| `sum` *(std)*          | ≡            | Sum of an array.        |                                                                               |
| `maxBy` / `meanBy` / `minBy` / `sumBy` | `*By` family | func-mapped versions of the row above.           | In `numberUtils.fs`.                                                          |
| ~~add~~ / ~~divide~~ / ~~multiply~~ / ~~subtract~~ | arithmetic-as-functions | `+ - * /` as callables. | FeatureScript just uses the operators directly. |

### Number

| FeatureScript   | Lodash    | Does                                    | Notes                                                                                                                                                                                                                                                                         |
| --------------- | --------- | --------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `clamp` *(std)* | ≡         | Constrain a number to `[lower, upper]`. |                                                                                                                                                                                                                                                                               |
| `inRange`       | ≡         | Whether a number falls within a range.  | Bounds auto-swap if `start > end`, matching lodash.                                                                                                                                                                                                                           |
| ~~random~~      | ≡         | Random number in a range.               | Possible, not implemented: FeatureScript has no entropy source, but a seeded linear-congruential generator is just arithmetic — needs a caller-supplied seed rather than true randomness. Same workaround `sample`/`sampleSize`/`shuffle` (Collection, above) are waiting on. |

### Object

| FeatureScript                     | Lodash          | Does                                                                 | Notes                                                                                                                                                                                     |
| --------------------------------- | --------------- | -------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `getAt`                           | `get`           | Path-based read from a map/array.                                    |                                                                                                                                                                                           |
| `setAt`                           | `set`           | Path-based write.                                                    | lodash's `set` mutates; `setAt` returns a new value.                                                                                                                                      |
| `deepMerge` / `mergeMaps` *(std)* | `merge`         | Recursively combine two maps.                                        | The headline "weird/less-functional builtin" case: std's `mergeMaps` is a shallow, one-level combine; coreUtils' `deepMerge` is the fuller recursive port lodash's `merge` actually does. |
| `pick`                            | ≡               | Map of just the given keys.                                          |                                                                                                                                                                                           |
| `pickDefined`                     | `pickBy`        | `pick`, keeping only defined values.                                 |                                                                                                                                                                                           |
| `valuesAt`                        | `at`            | Values at several keys/indices, in order.                            |                                                                                                                                                                                           |
| `hasKey`                          | `has`           | Whether a key/path resolves to something.                            |                                                                                                                                                                                           |
| `mapValues`                       | ≡               | Map a map's values, keys unchanged.                                  |                                                                                                                                                                                           |
| `keys` *(std)*                    | ≡               | A map's keys.                                                        |                                                                                                                                                                                           |
| `values` *(std)*                  | ≡               | A map's values.                                                      |                                                                                                                                                                                           |
| ~~transform~~                     | ≡               | `reduce`-like, but building up a map/array accumulator.              | Not yet built, but `foldArray` *(std)* already reduces to any accumulator shape, map or array included — this is a thin rename, not new capability.                                       |
| ~~functions~~                     | ≡               | Names of an object's function-valued properties.                     | Not yet built, but plain: `keys(pickBy(bag, (val) => val is function))`.                                                                                                                  |
| `toPairs`                         | ≡               | Map → `[[k,v], …]`.                                                  | Lodash also spells this `entries` — see [Lodash aliases](#lodash-aliases). In `objectUtils.fs`.                                                                                           |
| `findKey` / `findLastKey`         | ≡               | First/last key whose value matches a rule.                      | In `objectUtils.fs`.                                                                                                                                                                      |
| `invert` / `invertBy`             | `invert` family | Swap an object's keys and values.                                    | In `objectUtils.fs`.                                                                                                                                                                      |
| `mapKeys`                         | ≡               | Map a map's keys, values unchanged.                                  | In `objectUtils.fs`.                                                                                                                                                                      |
| `omit` / `omitBy`                 | `omit` family   | Inverse of `pick`/`pickBy` above — all keys *except* the given ones. | In `objectUtils.fs`.                                                                                                                                                                      |
| ~~result~~                        | ≡               | Like `get`, but invokes a function value found at the path.          | Marginal over `getAt` plus a manual call at the call site.                                                                                                                                |
| ~~invoke~~                        | ≡               | Call a method found at a path, with arguments.                       | Same marginal-over-`getAt` judgment as `result`, just curried with call args.                                                                                                             |

`assign`, `assignWith`, `defaults`, `defaultsDeep`, `unset`, `update`, `updateWith`, `setWith`, and
`mergeWith` all mutate their first argument in place in lodash — see
[Mutating (in-place) Functions](#mutating-in-place-functions), below. `assignIn`, `assignInWith`,
`extend`, `extendWith`, `hasIn`, `valuesIn`, `forIn`/`forInRight`/`forOwn`/`forOwnRight`,
`functionsIn`, and `entriesIn`/`toPairsIn` are filed under [Not Planned](#not-planned) (reason: JS
Properties).

### String

## `stringUtils.fs` — string slicing, padding, and case conversion

| Export                                                                                       | Does                                                                                                                                                                  |
| -------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `strSlice(str, begseq, endseq?)`                                                             | JS-style slice: negative indexes count from the end, either index clamps into range. `endseq` accepts `SequencePosition.END` in place of a literal length.            |
| `strTake(str, len)` / `strTakeRight(str, len)`                                               | First/last `len` characters; `len <= 0` is `""`.                                                                                                                      |
| `padLeft(str\|num, minlen, padstr?)`                                                         | Pads to `minlen` with `padstr` (default `" "`), truncating the padding if it overshoots; a `number` overload stringifies first.                                       |
| `padRight(str, minlen, padstr?)`                                                             | Same as `padLeft`, right-padded — but no numeric overload; a `number` must be stringified by the caller.                                                              |
| `strRepeat(str, reps)`                                                                       | `str` repeated `reps` times.                                                                                                                                          |
| `starbanner(str)`                                                                            | Wraps `str` in a `***`-bordered banner, for a `debug()` call that wants to stand out.                                                                                 |
| `hasMatch(str, regex)`                                                                       | Whether `regex` matches anywhere in `str`; `false` (not a throw) on `undefined` input or a bad pattern.                                                               |
| `upcase(str)` / `downcase(str)`                                                              | ASCII-only case flip via an explicit character lookup; a non-letter passes through unchanged.                                                                         |
| `titleCase(str, opts?)`                                                                      | Splits on `opts.spaces` (default `-_`), capitalizes each word's first letter, lower-cases the rest; `opts.tr` translates individual characters before capitalization. |
| `words(str)`                                                                                 | `str` split into words on runs of non-alphanumeric characters — the shared primitive behind the case-convention functions below.                                      |
| `capitalize(str)`                                                                            | First character uppercased, the rest lowercased.                                                                                                                      |
| `upperFirst(str)` / `lowerFirst(str)`                                                        | Only the first character's case changed; everything else left as-is.                                                                                                  |
| `pad(str, minlen, padstr?)`                                                                  | Pads both sides if shorter than `minlen`, splitting as evenly as possible (right side gets the extra character when odd).                                             |
| `trimStart(str, chars?)` / `trimEnd(str, chars?)` / `trim(str, chars?)`                      | Strips `chars` (default whitespace) from the front / back / both ends.                                                                                                |
| `truncate(str, opts?)`                                                                       | `str` cut to at most `opts.length` (default `30`) characters, omission marker included, replacing the cut tail with `opts.omission` (default `"..."`).                |

| FeatureScript               | Lodash      | Does                                                         | Notes                                                                                                                                    |
| --------------------------- | ----------- | ------------------------------------------------------------ | ---------------------------------------------------------------------------------------------------------------------------------------- |
| `padLeft`                   | `padStart`  | Pad a string/number on the left.                             |                                                                                                                                          |
| `padRight`                  | `padEnd`    | Pad on the right.                                            |                                                                                                                                          |
| `upcase`                    | `toUpper`   | ASCII-only uppercase.                                        |                                                                                                                                          |
| `downcase`                  | `toLower`   | ASCII-only lowercase.                                        |                                                                                                                                          |
| `titleCase`                 | `startCase` | Word-boundary capitalization.                                |                                                                                                                                          |
| `startsWith` *(std)*        | ≡           | Whether a string starts with a substring.                    |                                                                                                                                          |
| `endsWith` *(std)*          | ≡           | Whether a string ends with a substring.                      |                                                                                                                                          |
| `replace` *(std)*           | ≡           | Replace matches in a string.                                 |                                                                                                                                          |
| `stringToNumber` *(std)*    | `parseInt`  | Parse a string as a number.                                  |                                                                                                                                          |
| `camelCase`                 | ≡           | Words joined with no separator; first word lower, the rest capitalized. | Built on `words` — see its note on the simplified word-boundary rule.                                                                    |
| `kebabCase`                 | ≡           | Words joined with `-`, all lowercase.                        | Built on `words`.                                                                                                                        |
| `snakeCase`                 | ≡           | Words joined with `_`, all lowercase.                        | Built on `words`.                                                                                                                        |
| `lowerCase`                 | ≡           | Words joined with a space, all lowercase.                    | Built on `words`.                                                                                                                        |
| `upperCase`                 | ≡           | Words joined with a space, all uppercase.                    | Built on `words`.                                                                                                                        |
| `capitalize`                | ≡           | First character uppercased, the rest lowercased.             |                                                                                                                                          |
| `lowerFirst` / `upperFirst` | ≡           | Only the first character's case changed; everything else left as-is.        |                                                                                                                                          |
| `pad`                       | ≡           | Pad on both sides.                                           | `padLeft`/`padRight` remain the one-sided originals.                                                                                     |
| `trim` / `trimEnd` / `trimStart` | ≡      | Strip whitespace (or a given character set) from either end. |                                                                                                                                          |
| `truncate`                  | ≡           | Truncate a string to a length, with an omission marker.      | No `separator` option to break at a word/regex boundary instead of an exact count.                                                       |
| `words`                     | ≡           | Split a string into words.                                   | Simplified: splits only on non-alphanumeric delimiter runs, not on camelCase boundaries or digit runs the way lodash's own `words` does. |
| `repeatString` *(std)*      | `repeat`    | Repeat a string `n` times.                                                  |                                                                                                                                          |
| `splitByRegexp` / `splitIntoCharacters` *(std)*                     | `split`                      | Split a string.                                                                                | std splits by regexp or into characters — no plain-substring/limit split like lodash's.                                                  |

Possible, not yet built:

| FeatureScript    | Lodash         | Does                                        | Notes                                                                                                                                                                                        |
| ---------------- | -------------- | ------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| ~~deburr~~       | ≡              | Strip Latin-1 diacritics (`café` → `cafe`). | Same technique as `upcaseChar`/`downcaseChar`'s explicit lookup table — just a bigger one, mapping every accented Latin-1 character to its bare-ASCII form.                                  |
| ~~escapeRegExp~~ | ≡              | Escape a string's regex-special characters. | A plain character-by-character `replace` — unlike its `template`/`escape`/`unescape` neighbors ([Not Planned](#not-planned), below), this has nothing to do with templating or HTML. |

### Util

Most of this category is "build a small function from a piece of data" — a string path, a
rule map, a value to always return. FeatureScript already has everything that needs:
functions are values, `getAt`/`==` already do path-reads and deep equality, and `all`/`any`
*(std)* already fold a list of predicates into one. This is composition, not metaprogramming —
the "metaprogramming" label an earlier pass used undersold it.

| FeatureScript   | Lodash       | Does                                          | Notes                                                                                  |
| --------------- | ------------ | --------------------------------------------- | -------------------------------------------------------------------------------------- |
| `noop`               | ≡            | Does nothing, returns `undefined`.            |                                                                                        |
| `pathForKey`         | `toPath`     | Split a dotted-key string into path segments. |                                                                                        |
| `range` / `rangeRight` *(std/existing)* | ≡ | Array of numbers from `start` to `end`, ascending / descending. | `rangeRight` is `reverse` *(std)* over std's own `range` — inherits its inclusive-of-`end` convention. |
| `constant`                        | ≡          | Always return a fixed value, ignoring any arguments.                                               | `(val) => (() => val)`. In `miscUtils.fs`. |
| `identity`                        | ≡          | Return the argument unchanged.                                                                     | `(val) => val`. In `miscUtils.fs`. |
| `times`                           | ≡          | Call a function `n` times, collecting results.                                                     | `mapArray` *(std)* over `range(0, n - 1)` — a loop, not metaprogramming. `n < 1` returns `[]`. In `miscUtils.fs`. |
| `attempt`                         | ≡          | Call a function, returning its result or the caught error instead of throwing.                     | `try { return func(); } catch (error) { return error; }` — the same shape `parseJsonSafely` already uses. No argument-forwarding: FeatureScript has no variadic call syntax. In `miscUtils.fs`. |
| `property` / `propertyOf`         | ≡          | Build a function that reads one path off whatever it's given.                                      | `(path) => (obj) => getAt(obj, path)`, and the argument-order flip for `propertyOf`. In `miscUtils.fs`. |
| `matches` / `matchesProperty`     | ≡          | Build a rule checking an object against a partial shape / one path against a value.           | Rides on the same `pick(obj, keys(source)) == source` trick as `isMatch` (Lang, above). In `miscUtils.fs`. |
| `cond`                            | ≡          | Build a function trying `[rule, handler]` pairs in order, running the first match.            | A single-argument version — lodash's `cond` forwards every argument it receives to both, which needs the variadic call syntax FeatureScript doesn't have. In `miscUtils.fs`. |
| `conforms` / `conformsTo`         | ≡          | Build a rule / test a map against a map of per-key predicates.                            | `all` *(std)* over `keys(source)`, each checked as `source[key](obj[key])`. In `miscUtils.fs`. |
| `over` / `overEvery` / `overSome` | `overXXXX` | Run several functions against the same argument; collect results / require all / require any.     | `mapValues`/`all`/`any` over the function array, each called with the same argument. In `miscUtils.fs`. |
| `iteratee`                        | ≡          | Coerce a string/map/function "shorthand" into a real function — `property`/`matches`/pass-through. | The dispatcher tying `property` and `matches` together; anything else falls back to `identity`. In `miscUtils.fs`. |

Possible, not yet built:

| FeatureScript | Lodash | Does                                      | Notes                                                                                                                                                                                                                                                                                        |
| ------------- | ------ | ----------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| ~~uniqueId~~  | ≡      | Generate a monotonically-increasing id.   | Needs a workaround: lodash keeps a module-global counter, which FeatureScript has no equivalent of — the counter would have to live in the `Context` (`setVariable`/`getVariable`, the way `jsonVarF.fs`'s Features already thread state through a context) rather than as a plain variable. |

### Date, Seq

Date functions, and the Seq API, are not contemplated to be written at the moment.

### Other
`debugUtils.fs` and `metadataUtils.fs` have no entries in the tables above — neither has a lodash
counterpart for anything they export (Onshape-specific viewport/attribute plumbing).

---

## `arrayUtils.fs` — lodash-style array helpers

Straightforward ports from lodash's Array category; anything needing varargs takes an array of
arrays/values instead (`difference(arr, excludeArr)`, `union(arrList)`, …). Alphabetical order.

| Export                                                | Does                                                                                                                |
| ----------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------- |
| `chunk(arr, chunkSize)`                               | Splits `arr` into `chunkSize`-length groups; the last group holds the remainder. `chunkSize < 1` → `[]`.            |
| `compact(arr)`                                        | `arr` filtered to `typeUtils.truthy` — only `undefined`/`false` drop, unlike lodash's wider falsey set.             |
| `difference(arr, excludeArr)` / `differenceBy`        | `arr` minus anything (or anything whose `func` value) appears in `excludeArr`.                                  |
| `drop(arr, dropCount)` / `dropRight`                  | `arr` with `dropCount` elements removed from the front/back.                                                        |
| `dropWhile(arr, rule)` / `dropRightWhile`             | `arr` with a leading/trailing run satisfying `rule` removed.                                                   |
| `findIndex(arr, rule)` / `findLastIndex`              | Index of the first/last element satisfying `rule`, or `-1`.                                                    |
| `flatten(arr)` / `flattenDeep` / `flattenDepth`       | Array nesting removed one level / fully / up to `depth` levels.                                                     |
| `fromPairs(pairs)`                                    | `[[key, val], …]` → map; a repeated key keeps its last pair.                                                        |
| `initial(arr)`                                        | `arr` without its last element.                                                                                     |
| `intersection(arrList)` / `intersectionBy`            | Values (or `func` results) present in every array of `arrList`, deduplicated.                                   |
| `lastIndexOf(arr, val)`                               | Index of the last occurrence of `val`, or `-1`.                                                                     |
| `nth(arr, seq)`                                       | Element at `seq`; negative counts from the end; out of bounds is `undefined`.                                       |
| `tail(arr)`                                           | `arr` without its first element.                                                                                    |
| `take(arr, takeCount)` / `takeRight`                  | First/last `takeCount` elements of `arr`.                                                                           |
| `takeWhile(arr, rule)` / `takeRightWhile`             | Leading/trailing run of `arr` satisfying `rule`.                                                               |
| `union(arrList)` / `unionBy`                          | Deduplicated concatenation of every array in `arrList`.                                                             |
| `uniqBy(arr, func)`                               | `arr` deduplicated by the result of `func(val)`
| `uniqWith(arr, comparator)`                           | `arr` deduplicated by a custom equality `comparator`.                                        |
| `unzip(arr)` / `unzipWith`                            | Ungroups `arr`'s rows into columns (an alias for std `zip`, its own inverse), optionally mapped through `func`. |
| `without(arr, excludeArr)`                            | `arr` minus anything in `excludeArr` — identical to `difference`, under lodash's other name.                        |
| `xor(arrList)`                                        | Values appearing in exactly one array of `arrList`.                                                                 |
| `zipObject(keylist, valuelist)`                       | Map pairing `keylist`/`valuelist` by position.                                                                      |
| `zipWith(arrList, func)`                          | Std `zip` on `arrList`, each grouped row passed through `func`.                                                 |

## `clxnGetset.fs` — path-based get/set on maps and arrays

Lodash's `get`/`set`/`merge`, adapted to FeatureScript's value semantics and negative-index
convention.

| Export                                                  | Does                                                                                                                                                                                                                                                                      |
| ------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `getAt(bag\|arr, keyStr\|path\|seq, fallback?)`         | Reads a dotted path (`"a.b.c"`), a literal key list (`["a.b"]`), or a single array index (negative counts from the end). Missing at any step returns `fallback` (default `undefined`); a present-but-`undefined` array element is *not* the fallback.                     |
| `setAt(bag\|arr, keyStr\|path\|seq, val, onCollision?)` | Same path reading as `getAt`, but writes: autovivifies missing steps as maps, replaces a scalar in the way, and pads an array written past its end with `undefined`. `onCollision(existing, incoming)` resolves a leaf that's already occupied — default is `lastInWins`. |
| `deepMerge(existing, incoming)`                         | Lodash's deep merge: two maps combine key by key all the way down; anything else, `incoming` wins; an `undefined` `incoming` leaves `existing` alone. Arrays replace whole (no index-by-index merge). The `onCollision` to pass `setAt` for merge-on-write.               |
| `pathForKey(keyStr)`                                    | Splits a dotted key into segments (`"a..b"` → `["a", "", "b"]`); a key with a terminal dot is unhandled and drops that last empty segment.                                                                                                                                |
| `lastInWins`                                            | The default `onCollision`: `(existing, incoming) => incoming`.                                                                                                                                                                                                            |
| `GetsetAtStep.MISSING`                                  | Internal sentinel for "nothing at this step" — never returned to a caller.                                                                                                                                                                                                |

## `clxnReshape.fs` — dotted keys ↔ nested maps, and choice-tree building

| Export                             | Does                                                                                                                                                                                                                                                                                                                                                                                                                                               |
| ---------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `undotMap(obj, onCollision?)`      | Expands `obj`'s dotted top-level keys into nested maps (`{"a.b": 1, "a": {"c": 2}}` → `{"a": {"b": 1, "c": 2}}`). Applies shallowest-key-first so the result doesn't depend on map iteration order. `onCollision` resolves a leaf two keys both reach — default `deepMerge`, so two maps landing on the same spot combine rather than one replacing the other. Only `obj`'s own keys are split; dots already inside a value's keys are left alone. |
| `dotMap(obj, options?)`            | The inverse: depth-first flattens a nested map into dotted keys. `options.maxDepth` caps how many levels collapse (lodash's `flattenDepth` semantics); arrays and empty maps are leaves. Lossy against a key that already contains a dot — `undotMap(dotMap(x))` isn't always `x`.                                                                                                                                                                 |
| `buildNestedChoices(levels, tree)` | Turns a raw uniformly-nested map into the `{name, displayName, entries}` structure a custom-feature enum dialog wants, given a `levels` list like `['socket_kind', ['drive_kind', {inthex: 'Int Hex'}]]` (name, optional display name, optional key-translation map per level).                                                                                                                                                                    |

## `clxnWalking.fs` — inspecting, iterating, and reshaping maps and arrays

The workhorse file: `forEach` and `mapValues`/`mapValues3` are what most other collection code
in this project is built from.

| Export                                              | Does                                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| --------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `sizeof(val)`                                       | Key count (map) / character count (string) / element count (array) / `0` (`undefined`).                                                                                                                                                                                                                                                                                                                                                                      |
| `hasKey(obj, key, missingPolicy?)`                  | Whether `key` resolves to something in `obj`. For a map this always means present-and-not-`undefined` (the `missingPolicy` overload exists only for symmetry and never changes the answer). For an array, `MissingPolicy.USE_UNDEFINED` (the default) checks only that the index is in bounds; `SKIP` also requires the slot not be `undefined`. Neither array overload accepts a negative index, unlike `getAt`.                                            |
| `hasPresentKey(obj, key)`                           | `hasKey` pinned to the strict policy: in bounds (or present) *and* not `undefined`.                                                                                                                                                                                                                                                                                                                                                                          |
| `arrayIncludes(bag\|arr, target)`                   | Whether `target` appears anywhere in `bag`'s values or `arr`'s elements (`==`, so a deep match on maps/arrays too).                                                                                                                                                                                                                                                                                                                                          |
| `pick(bag, keylist)`                                | A map of just `bag`'s entries at `keylist`; an absent key is elided, not set to `undefined`.                                                                                                                                                                                                                                                                                                                                                                 |
| `pickDefined(bag, keylist)`                         | Same, skipping a key whose value is `undefined` — behaviorally identical to `pick` on a map, since FeatureScript elides `undefined` on write either way.                                                                                                                                                                                                                                                                                                     |
| `arrLast(arr)`                                      | Last element of `arr`, or `undefined` if empty.                                                                                                                                                                                                                                                                                                                                                                                                              |
| `valuesAt(bag\|arr, keylist, missingPolicy?)`       | Array of values at `keylist`, in that order — the plural cousin of a single lookup, with no path traversal. `USE_UNDEFINED` (default) keeps the result the same length as `keylist`; `SKIP` drops missing slots. Array indexes must be non-negative and in bounds.                                                                                                                                                                                           |
| `forEach(bag\|arr, keylist?, missingPolicy?, func)` | Visits every entry, calling `func(val, key, seq)` (map) or `func(val, seq)` (plain array) / `func(val, seq, seq)` (array + `missingPolicy`) — `seq` is always a 0-based visit count. Returning `NextStepAction.BREAK` from `func` stops the walk immediately. `missingPolicy` (`USE_UNDEFINED` default / `SKIP`) decides whether an `undefined`-valued entry is visited at all. A `keylist` walks exactly those keys, in that order, instead of `keys(bag)`. |
| `mapValues(bag\|arr, ...)` / `mapValues3(...)`      | `forEach`, collecting `func`'s return value back into a same-shaped map/array. `mapValues3` adds the trailing `seq` to the callback; both take the same `keylist`/`missingPolicy` overloads as `forEach`.                                                                                                                                                                                                                                                    |
| `rebag(bag\|arr, func)`                             | Rebuilds a map by asking `func(val, key?, seq)` for the `[newKey, newVal]` pair each entry becomes; `func` returning `undefined` drops that entry. Later entries win a `newKey` collision.                                                                                                                                                                                                                                                                   |
| `objectify(arr, func)`                              | `rebag` specialized so an array's own values become the result's keys: `objectify(arr, func)` ≡ `rebag(arr, (val, seq) => [val, func(val, seq)])`.                                                                                                                                                                                                                                                                                                           |
| `boxarrPush(arrRef, val)` / `boxarrUnshift`         | Append / prepend `val` into the array inside a `box`, in place, returning `val` — for accumulating into an outer array from inside a `forEach`/`mapValues` callback.                                                                                                                                                                                                                                                                                         |
| `MissingPolicy`                                     | `SKIP` / `USE_UNDEFINED` — how the functions above treat an entry whose value is `undefined`, whether that's because it's missing outright or genuinely set to `undefined`.                                                                                                                                                                                                                                                                                  |
| `NextStepAction.BREAK`                              | Sentinel a `forEach`-family callback returns to stop the walk early.                                                                                                                                                                                                                                                                                                                                                                                         |
| `countBy(bag\|arr, func)`                           | Map of `func(val, key?)` results to how many elements produced each one — `key` only for the map form.                                                                                                                                                                                                                                                                                                                                                       |
| `find(bag\|arr, rule)` / `findLast(bag\|arr, rule)` | First/last element for which `rule` holds, or `undefined`. Array form dereferences `arrayUtils`' `findIndex`/`findLastIndex`; map form walks `keys(bag)` and hands `rule` the key.                                                                                                                                                                                                                                                                           |
| `flatMap(bag\|arr, func)`                           | Mapped through `func`, then flattened one level. Map form hands `func` the key and always returns an array.                                                                                                                                                                                                                                                                                                                                                  |
| `flatMapDeep(...)` / `flatMapDepth(..., depth)`     | Mapped through `func`, then flattened fully / `depth` levels. Map form hands `func` the key and always returns an array.                                                                                                                                                                                                                                                                                                                                     |
| `forEachRight(bag\|arr, func)`                      | `forEach`, back-to-front — same `NextStepAction.BREAK` early exit. Map form walks `keys(bag)` in reverse.                                                                                                                                                                                                                                                                                                                                                    |
| `groupBy(bag\|arr, func)`                           | Map of `func(val, key?)` results to the elements that produced each one, via std's `insertIntoMapOfArrays`.                                                                                                                                                                                                                                                                                                                                                  |
| `partition(bag\|arr, rule)`                         | `[passed, failed]` — split by whether `rule` holds, keeping visiting order. Map form hands `rule` the key and returns values only.                                                                                                                                                                                                                                                                                                                           |
| `reduceRight(bag\|arr, seed?, foldFunction)`        | `foldArray` *(std)*, back-to-front, over a `reverse`d copy of `arr`; map form walks `keys(bag)` in reverse and hands `foldFunction` the key.                                                                                                                                                                                                                                                                                                                 |
| `reject(bag\|arr, rule)`                            | Elements for which `rule` does *not* hold — the inverse of `filter` *(std)*. Map form hands `rule` the key and returns values only.                                                                                                                                                                                                                                                                                                                          |

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
| `attempt(func)` | `func()`'s result, or the error it throws, caught instead of propagated. No argument-forwarding. |
| `cond(pairs)` | Function trying `[rule, handler]` pairs in order; returns the first `handler(val)` whose `rule(val)` holds. |
| `conforms(source)` / `conformsTo(obj, source)` | Whether every predicate in `source` holds against the same-keyed value of `obj`; `conforms` is the curried, reusable form. |
| `constant(val)` | Function that always returns `val`, ignoring its arguments. |
| `identity(val)` | Returns `val` unchanged. |
| `iteratee(spec)` | Coerces a function/map/string `spec` into a callable iteratee — pass-through / `matches` / `property` — falling back to `identity`. |
| `matches(source)` / `matchesProperty(path, srcValue)` | Predicate for "has `source`'s entries" / "`path` equals `srcValue`", via `pick`/`getAt`. |
| `over(funcs)` / `overEvery(funcs)` / `overSome(funcs)` | Call every function in `funcs` on the same value, collecting results / requiring all truthy / requiring any truthy. |
| `property(path)` / `propertyOf(obj)` | Function reading `path` off whatever it's given / reading whatever path it's given off `obj`, via `getAt`. |
| `times(count, func?)` | `func(seq)` for `seq` from `0` to `count - 1`, collected into an array; `count < 1` is `[]`. Defaults to `identity`. |

## `numberUtils.fs` — range checks and func-mapped math reductions

| Export | Does |
|---|---|
| `inRange(num, start?, end)` | Whether `num` falls in `[start, end)`; `start` defaults to `0`; bounds auto-swap if `start > end`. |
| `maxBy(arr, func)` / `minBy(arr, func)` | Element of `arr` for which `func(val)` is greatest/least, or `undefined` for an empty `arr` — std's array `max`/`min` pick the value itself, these pick the element behind it. |
| `meanBy(arr, func)` / `sumBy(arr, func)` | `average`/`sum` *(std)*, func-mapped. |

## `objectUtils.fs` — lodash-style map helpers

Straightforward ports from lodash's Object category, beyond what `clxnGetset.fs`/`clxnWalking.fs`
already cover (`getAt`/`setAt`/`pick`/`mapValues`/`hasKey`/…).

| Export | Does |
|---|---|
| `findKey(bag, rule)` / `findLastKey(bag, rule)` | First/last key of `bag` (in `keys(bag)` order) whose value satisfies `rule(val, key)`. |
| `invert(bag)` | `bag` with keys and values swapped; a repeated value keeps only its last key. A non-string value is stringified into its new key. |
| `invertBy(bag, func)` | `invert`, grouping every key (not just the last) under `func(val)`, via std's `insertIntoMapOfArrays`. |
| `mapKeys(bag, func)` | `bag`'s values, keyed by `func(val, key)` instead of `key`. |
| `omit(bag, keylist)` | `bag` without the entries at `keylist` — the inverse of `pick`. |
| `omitBy(bag, rule)` | `bag` without any entry for which `rule(val, key)` holds. |
| `toPairs(bag)` | `bag` flattened into `[[key, val], …]` pairs — the inverse of `fromPairs` *(arrayUtils)*. |

## `typeUtils.fs` — presence checks and small guards

| Export                          | Does                                                                                                                   |
| ------------------------------- | ---------------------------------------------------------------------------------------------------------------------- |
| `ifNil(val, fallback)`          | `val`, or `fallback` if `val` is `undefined`.                                                                          |
| `ifBlank(val, fallback)`        | `val`, or `fallback` if `val` is `undefined` or `""`.                                                                  |
| `ifZero(val, fallback)`         | `val`, or `fallback` if `val` is `undefined` or (within tolerance) zero; overloaded for `number` and `ValueWithUnits`. |
| `truthy(val)`                   | Neither `undefined` nor `false` — `0` and `""` are truthy, unlike JS.                                                  |
| `isPresent(val)` / `isNil(val)` | Not-`undefined` / is-`undefined`.                                                                                      |
| `strBlank(val)`                 | `undefined` or `""`.                                                                                                   |
| `isEmpty(val)`                  | `undefined`, or a map/string/array with nothing in it.                                                                 |
| `castArray(val)`                | `val` unchanged if it's already an array, otherwise `[val]`.                                                           |
| `vector2(vec)`                  | Drops `vec`'s z component.                                                                                             |
| `mm`, `zero`                    | `millimeter`, and `0 * mm`.                                                                                            |

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

---

## Mutating (in-place) Functions

Lodash's own mutating functions — the ones that write into their first argument and hand it back,
rather than returning a new value. This project returns a new value from everything instead, so
each has a pure stand-in already (or a short, unbuilt, still-pure one); a true in-place version, if
ever wanted, would be its own realm of `box`-based mutable utilities, not a retrofit onto the pure
functions above.

| Mutating function | Implemented function | Brief description |
|---|---|---|
| `pull`, `pullAll`, `pullAllBy` | `difference`, `differenceBy` | Remove given values from an array. |
| `pullAllWith` | *(needs `differenceWith`, not yet ported)* | Remove given values from an array, by custom comparator. |
| `remove` | `partition` (keep the removed half) | Split an array by a rule. |
| `fill` | *(not built — `subArray` + `makeArray` + `concatenateArrays`)* | Overwrite a sub-range of an array with a fixed value. |
| `assign` | `mergeMaps` *(std)* | Shallow-copy one map's keys onto another. |
| `assignWith` | *(needs a customizer-aware `mergeMaps`)* | Shallow-copy, with a custom per-key combiner. |
| `defaults`, `defaultsDeep` | `deepMerge(source, dest)` *(args flipped)* | Fill in missing keys from a fallback object. |
| `unset` | `setAt(bag, path, undefined)` | Delete whatever's at a path. |
| `update`, `updateWith` | *(not built — `setAt(bag, path, updaterFunc(getAt(bag, path)))`)* | Read-modify-write a value at a path. |
| `setWith` | `setAt` (its existing `onCollision` param) | Set a path, with a customizer for autovivified segments. |
| `mergeWith` | *(needs a customizer-aware `deepMerge`)* | Deep-merge two maps, with a custom per-key combiner. |

## Lodash aliases

Lodash sometimes offers two names for the same function; the tables above show only the one this
project settled on: `first` (not `head`), `forEach` (not `each`), `forEachRight` (not `eachRight`),
and `toPairs` (not `entries`).

## Not Planned

Every function that showed up in a "not yet"/"not implemented" bucket anywhere above has been
resolved one way or the other: either it's a real (if sometimes struck-through, sometimes
workaround-needing) row in a category table above, a row in [Mutating (in-place)
Functions](#mutating-in-place-functions), or it's not planned, for one of a handful of recurring
reasons tabulated here — no more scattered "→ HUMAN-FIXME.md" cross-references per row.

| Realm | Reason | Function(s) |
|---|---|---|
| Collection | js-specific | `invokeMap` |
| Function | chaining | `chain`, `tap`, `thru` |
| Function | async | `debounce`, `defer`, `delay`, `throttle` |
| Function | js-specific | `bind`, `bindKey` |
| Function | js-specific | `rest`, `spread`, `overArgs`, `rearg` |
| Lang | js-specific | `isArguments`, `isArrayBuffer`, `isBuffer`, `isDate`, `isElement`, `isError`, `isNative`, `isRegExp`, `isSet`, `isSymbol`, `isTypedArray`, `isWeakMap`, `isWeakSet` |
| Lang | builtin | `isArrayLike`, `isArrayLikeObject`, `isObjectLike`, `isLength` |
| Lang | builtin | `isFinite`, `isNaN`, `isSafeInteger`, `toFinite`, `toLength`, `toSafeInteger` |
| Lang | builtin | `toPlainObject`, `toArray` |
| Lang | builtin | `clone`, `cloneDeep`, `cloneDeepWith`, `cloneWith` |
| Object | js-specific | `create` |
| Object | JS Properties | `hasIn`, `valuesIn`, `forIn`, `forInRight`, `forOwn`, `forOwnRight`, `assignIn`, `assignInWith`, `extend`, `extendWith`, `functionsIn`, `entriesIn`, `toPairsIn` |
| String | templating | `template` |
| String | web | `escape`, `unescape` |
| Util | js-specific | `bindAll`, `mixin`, `noConflict`, `runInContext` |
| Util | js-specific | `method`, `methodOf` |
| Util | builtin | `stubArray`, `stubFalse`, `stubObject`, `stubString`, `stubTrue` |
| Util | builtin | `VERSION` |
| Date | date | `now` |
| Seq | chaining | entire category (`chain`-wrapper methods) |

**Reasons:**

* **date** — depends on wall-clock time, which this project's permanent no-go list already covers.
* **web** — URL/HTML-specific (escaping markup, parsing URLs); out of scope by the same standing rule.
* **chaining** — lodash's wrap-in-an-object-and-call-methods API; an alternate calling convention, not a capability gap.
* **async** — assumes a timer/event-loop FeatureScript regeneration doesn't have.
* **templating** — string template compilation; see [`HUMAN-FIXME.md`](HUMAN-FIXME.md) for why this one's real work rather than a one-liner.
* **builtin** — already answered by a FeatureScript language construct, a std function, or a one-line closure not worth naming on its own.
* **JS Properties** — the own-vs-inherited-enumerable-property distinction JS's prototype chain draws (the `*In` function family, plus `forOwn`/`extend`); FeatureScript maps have no prototype chain, so there's nothing left for these to distinguish.
* **js-specific** — depends on some other JS-only runtime feature FeatureScript structurally doesn't have: prototype-based construction, `this`-binding, dynamic method-by-name dispatch, variadic/`apply`-style calling, or a type like Symbol/Buffer/WeakMap/a DOM element.
