# coreUtils

General-purpose FeatureScript helpers shared across this project's features — not
geometry-specific, just the plumbing every feature ends up needing. Style follows
[`STYLE-Featurescript.md`](../STYLE-Featurescript.md); the standard-library map/array/string
functions these build on are catalogued in [`onshape/README.md`](../../README.md).

Each module lives next to its own tests, grouped by role:

* `clxn/` — collection walking and reshaping: `clxnUtils.fs`, `clxnGetset.fs`, `clxnReshape.fs`.
* `prim/` — primitive-value helpers: `stringUtils.fs`, `typeUtils.fs`, `helperFuncs.fs`, `sortUtils.fs`.
* `fancy/` — Onshape-specific, needs a live `Query`/`Context`: `colorUtils.fs`, `metadataUtils.fs`, `debugUtils.fs`.
* `features/` — variable-producing Features: `jsonVarF.fs`.
* `lib/` — the `runTests` test harness itself.

`coreUtils.fs` itself has no functions of its own — it's meant as a single-file re-export of
everything else in this directory, so other features can `import` just this one file for all of
it. **It's currently out of date**: it only re-exports `clxnUtils.fs`, `clxnReshape.fs`,
`clxnGetset.fs`, `stringUtils.fs`, and `typeUtils.fs` — `helperFuncs.fs`, `sortUtils.fs`,
`colorUtils.fs`, `metadataUtils.fs`, `debugUtils.fs`, and `jsonVarF.fs` are missing from its
export list and need to be added. (There's also an older, differently-scoped aggregator,
`mrflipUtils.fs`, left over from before this directory's reorg — it re-exports a different subset
still and predates several files entirely; it should probably be retired once `coreUtils.fs` is
complete, rather than kept as a second, inconsistent entry point.)

## Tests

One `run<Thing>Tests` function per case list, sitting in a `test<File>.fs` right next to the file
it tests. Large modules may have their tests broken into separate files, as you'll see with the collection (clxn) utilties.

---

## Menu

Much of this directory is an ongoing port of [lodash](https://lodash.com/docs)'s conveniences into
FeatureScript, pulled in as needed from the reference copy at `lodash.js`. The tables below group
everything by what it's *for*, not by lodash's own category boundaries or which file it happens to
live in — `union`/`intersection`/`difference` are lodash "Array" functions, for instance, but read
better here under [Set Operations](#set-operations).

**Notation used in the `Function Name` column:**

| Notation | Means |
| --- | --- |
| a plain name (`getAt`, `pick`, …) | Implemented in this project's coreUtils. |
| `name` *(std)* | Covered by a FeatureScript standard-library builtin (`math.fs`/`string.fs`/`containers.fs`) — no port needed. |
| `` `is array` `` etc. *(lang)* | Built into the FeatureScript language itself (a type-check expression or operator), not a function call. |
| ~~name~~ | Nothing implements it yet — a real, if low-priority, porting candidate, listed in a "Possible, not yet built" table under the section it belongs to. |
| a struck name in [Not Planned](#not-planned) | Consciously not planned — that table has the reason. |

A function can have both a coreUtils port *and* a std builtin that inspired it, and the two can
differ in how much each actually does — `deepMerge` vs. `mergeMaps` *(std)* is the clearest case,
noted where it comes up.

---

## Collection

### Collection Walking

These functions accept an iterator, `func(val, key is string)` for maps, `func(val, idx is number)` for arrays.
The ones with a `3` suffix (eg mapValues3) will call `func(val, seq, iteration is number)` -- where seq is the map key or array iterator and iteration is the index of the given element.

| Function Name                              | Lodash  | Description                                                                           | Caveats                                                                                                                                                                                                                                                   |
| ------------------------------------------ | ------- | ------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `forEach` / `forEach3`                     | ≡       | Call the function for each element, in array or (alphabetic) key order                | Stops early only when `func` returns `NextStepAction.BREAK`, not `false` like lodash. A `keylist` overload walks exactly those keys, in order; `NilPolicy` (`NIL` default / `SKIP`) decides whether a set-but-`undefined` entry is visited. |
| `forEachRight`                             | ≡       | `forEach`, back-to-front.                                                             | Map form walks `keys(bag)` in reverse.                                                                                                                                                                                                                    |
| `mapValues` / `mapValues3`                 | `map`   | Map a collection to a same-shaped result -- array->array, map->map.                   | Same `keylist`/`missingPolicy` overloads as `forEach`.                                                                                                                                                                                                    |
| `find` / `findLast`                        | ≡       | First/last element matching a rule.                                                   | Array form dereferences `findIndex`/`findLastIndex`; map form walks `keys(bag)`.                                                                                                                                                                          |
| `flatMap` / `flatMapDeep` / `flatMapDepth` | ≡       | Map then flatten one/all/`n` levels.                                                  | Rides on the `flatten` family (Array, below).                                                                                                                                                                                                             |
| `countBy`                                  | ≡       | Counts of elements, grouped by a computed key.                                        |                                                                                                                                                                                                                                                           |
| `groupBy`                                  | ≡       | Group elements by a computed key.                                                     | Built on std's `insertIntoMapOfArrays`.                                                                                                                                                                                                                   |
| `partition`                                | ≡       | Split into two groups by a rule.                                                      |                                                                                                                                                                                                                                                           |
| `reduceRight`                              | ≡       | `reduce`, back-to-front.                                                              |                                                                                                                                                                                                                                                           |
| `reject`                                   | ≡       | Elements *failing* a rule — inverse of `filter` *(std)*.                              |                                                                                                                                                                                                                                                           |
| `objectify`                                | `keyBy` | Key a collection by a computed key.                                                   | Key/value roles are swapped from lodash's `keyBy`: `objectify` keys by the *value* and computes the result at each key, where `keyBy` keys by a computed key and keeps the value.                                                                         |
| `rebag`                                    | —       | Rebuild a map by computing a `[newKey, newVal]` pair for every entry.                 | The general machine `objectify` is built from; a callback returning `undefined` drops that entry.                                                                                                                                                         |
| `boxarrPush` / `boxarrUnshift`             | —       | Append/prepend into the array inside a `box`, in place.                               | For accumulating into an outer array from inside a `forEach`/`mapValues` callback.                                                                                                                                                                        |
| `NilPolicy`                            | —       | `SKIP` / `NIL` — how the functions above treat a set-but-`undefined` entry. |                                                                                                                                                                                                                                                           |
| `NextStepAction.BREAK`                     | —       | Sentinel a callback returns to stop a walk early.                                     |                                                                                                                                                                                                                                                           |

### Set Operations

| Function Name                                          | Lodash        | Description                                                              | Caveats                                                                                                             |
| ------------------------------------------------------ | ------------- | ------------------------------------------------------------------------ | ------------------------------------------------------------------------------------------------------------------- |
| `union` / `unionBy` / `unionWith`                      | ≡             | Deduplicated concatenation of several arrays.                            | Takes an array of arrays — no variadic arguments.                                                                   |
| `intersection` / `intersectionBy` / `intersectionWith` | ≡             | Values present in every given array.                                     | Same array-of-arrays shape as `union`.                                                                              |
| `difference` / `differenceBy` / `differenceWith`       | ≡             | Values in one array not present in another.                              | Exclusion values are one array, not trailing arguments — concatenate first to exclude from several sources at once. |
| `without(arr, removeables)`                            | ≡             | Alias for difference                                                     | lodash splats each removeable into its own argument; this allows only one list                                      |
| `xor` / `xorBy` / `xorWith`                            | ≡             | Symmetric difference of several arrays.                                  | Array-of-arrays shape, like `union`/`intersection`.                                                                 |
| `uniqBy` / `uniqWith`                                  | `uniq` family | Duplicate-free version of an array, by computed key / custom comparator. | Plain `uniq` is std's `deduplicate`.                                                                                |

### Array

| Function Name                              | Lodash     | Description                                                                   | Caveats                                                                          |
| ------------------------------------------ | ---------- | ----------------------------------------------------------------------------- | -------------------------------------------------------------------------------- |
| `chunk`                                    | ≡          | Split an array into groups of `chunkSize` elements.                           | Last group holds the remainder; `chunkSize < 1` → `[]`.                          |
| `compact`                                  | ≡          | Remove falsey values from an array.                                           | Only `undefined`/`false` are falsey in FeatureScript, so `0` and `""` survive.   |
| `drop` / `dropRight`                       | ≡          | Drop `n` elements from the front/back.                                        | `n` defaults to `1`.                                                            |
| `dropWhile` / `dropRightWhile`             | ≡          | Drop elements from the front/back while a rule holds.                         | Rule defaults to `truthy`.                                                      |
| `take` / `takeRight`                       | ≡          | Take `n` elements from the front/back.                                        | `n` defaults to `1`, matching `drop`/`dropRight`.                               |
| `takeWhile` / `takeRightWhile`             | ≡          | Take elements from the front/back while a rule holds.                         | Rule defaults to `truthy`, matching `dropWhile`/`dropRightWhile`.               |
| `findIndex` / `findLastIndex`              | ≡          | Index of the first/last element matching a rule, or `-1`.                     | No `fromIndex` argument.                                                         |
| `lastIndexOf`                              | ≡          | Index of the last occurrence of a value, or `-1`.                             | No `fromIndex` argument.                                                         |
| `flatten` / `flattenDeep` / `flattenDepth` | ≡          | Flatten an array one level / fully / `n` levels deep.                         |                                                                                  |
| `fromPairs`                                | ≡          | `[[k, v], …]` → map.                                                          | A repeated key keeps its last pair.                                              |
| `initial`                                  | ≡          | All but the last element.                                                     |                                                                                  |
| `tail`                                     | ≡          | All but the first element.                                                    |                                                                                  |
| `nth`                                      | ≡          | Nth element; negative counts from the end.                                    |                                                                                  |
| `arrFirst` / `first` *(std)*               | `head`     | First element of an array.                                                    |                                                                                  |
| `arrLast` / `last` *(std)*                 | ≡          | Last element of an array.                                                     |                                                                                  |
| `concatenateArrays` *(std)*                | `concat`   | Concatenate array(s)/value(s) into a new array.                               | std is arrays-only — no flattening of bare scalar arguments the way lodash does. |
| `indexOf` *(std)*                          | ≡          | Index of the first occurrence of a value.                                     |                                                                                  |
| `join` *(std)*                             | ≡          | Join array elements into a string.                                            |                                                                                  |
| `reverse` *(std)*                          | ≡          | Reverse an array.                                                             |                                                                                  |
| `subArray` *(std)*                         | `slice`    | Slice of an array between two indices.                                        |                                                                                  |
| `removeElementAt` *(std)*                  | ~ `pullAt` | Remove one element by index.                                                  | Takes one index at a time and doesn't hand back what it removed.                 |
| ~~`pullAt`~~                               | ≡          | Remove elements by index, mutating the original and returning what was pulled | Not implemented yet                                                              |
| `zipObject`                                | ≡          | Build a map pairing `keylist`/`valuelist` by position.                        | A `keylist` entry past the end of `valuelist` (or vice versa) is simply dropped. |
| `zip` *(std)* / `zipWith`                  | ≡          | Groups N lists into a list of N-tuples.                                       |                                                                                  |
| `unzip` / `unzipWith`                      | ≡          | Splits a list of N-tuples into N lists                                        |                                                                                  |
| ~~`zipObjectDeep`~~                        | ≡          | ??                                                                            | Not implemented yet                                                              |

### Map/Record

| Function Name                      | Lodash           | Description                                                                                 | Caveats                                                                                                                                                                                                              |
| ---------------------------------- | ---------------- | ------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `pick`                             | ≡                | Map of just the given keys.                                                                 | Each key/`keylist` entry is resolved via `getAt`/`setAt`, so a dotted string or key-path array reaches into nested structure and rebuilds the same nesting in the result.                                          |
| `pickBy`                           | ≡                | `pick`, keyed by a rule instead of a key list.                                              |                                                                                                                                                                                                                      |
| `pickDefined`                      | `pick` family    | `pick`, keeping only defined values.                                                        | Fixed "is defined" rule; @see `pickBy` for a customizable one. Only a literal top-level key, not a dotted path.                                                                                                      |
| `omit`                             | ≡                | Inverse of `pick` — all keys *except* the given ones.                                       | Same dotted-path/key-path support as `pick`; a path with nothing currently at it is skipped rather than autovivifying empty maps along the way.                                                                     |
| `omitBy`                           | `omit` family    | `omit`, keyed by a rule instead of a key list.                                              |                                                                                                                                                                                                                      |
| `mapKeys`                          | ≡                | Map a map's keys, values unchanged.                                                         |                                                                                                                                                                                                                      |
| `invert` / `invertBy`              | `invert` family  | Swap a map's keys and values.                                                               | A repeated value keeps only its last key for plain `invert`; `invertBy` groups every key under its computed value instead.                                                                                           |
| `findKey` / `findLastKey`          | ≡                | First/last key whose value matches a rule.                                                  |                                                                                                                                                                                                                      |
| `toPairs`                          | ≡                | Map → `[[k, v], …]`.                                                                        |                                                                                                                                                                                                                      |
| `hasKey` / `hasPresentKey`         | `has`            | Whether a key/index resolves to a value (hasKey) or a non-undefined value (hasPresentKey)   | Does not accept a path of keys, a dotted path (unlike `getAt`) or negative array index.                                                                                                                              |
| ~~`hasKeyAt` / `hasPresentKeyAt`~~ | `has`            | Whether a key/index/[path of keys]/dotpath string resolves to something.                    |                                                                                                                                                                                                                      |
| `valuesAt` (may rename to getMany) | `at`             | Values at several keys/indices, in order.                                                   | No path traversal — each entry of the key list is a literal key or index.                                                                                                                                            |
| ~~`getManyAt`~~                    | `at`             | like `valuesAt` but may supply a key/keypath/dotpath/index/-index                           | Not implemented                                                                                                                                                                                                      |
| `keys` *(std)* / `values` *(std)*  | ≡                | A map's keys / values. Note that FS returns keys always in alphabetic, not insertion, order |                                                                                                                                                                                                                      |
| `getAt`                            | `get`            | Path-based read from a map/array.                                                           | Dotted string, literal key-list array, or a single array index (negative counts from the end).                                                                                                                       |
| `setAt`                            | `set`            | Path-based write.                                                                           | Returns a new value rather than mutating; missing steps autovivify as a map, unless the *next* segment looks like a non-negative integer, in which case an array — lodash's own `set` heuristic; `onCollision(existing, incoming)` resolves an already-occupied leaf. `setAtWith` overrides the autoviv heuristic outright. |
| `update` / `updateWith`            | —                | Read-modify-write at a path.                                                                | `setAt(bag, path, updater(getAt(bag, path)))`; `updateWith`'s `onCollision` resolves the freshly-updated value against what was just read at that same leaf — a different hook from `setAtWith`'s segment customizer.       |
| `setAtWith`                        | `setWith`        | `setAt`, with a customizer for what an autovivified segment becomes.                        | `segmentFor(existingChildOrUndefined, nextSegment) => newChildContainer`; consulted only where a segment isn't already a map or array.                                                                               |
| `deepMerge` / `mergeMaps` *(std)*  | `merge`          | Recursively combine two maps.                                                               | The "weird/less-functional builtin" case: std's `mergeMaps` is a shallow, one-level combine; `deepMerge` is the fuller recursive port lodash's `merge` actually does, including merging two arrays index by index.   |
| `assignWith`                       | —                | `mergeMaps`, with a custom per-key combiner.                                                | `undefined` from the combiner falls back to `incoming` winning.                                                                                                                                                      |
| `mergeWith`                        | —                | `deepMerge`, with a custom per-key combiner.                                                | Combiner is consulted at every level of the recursion, not just the leaves.                                                                                                                                          |
| `pathForKey`                       | `toPath`         | Split a dotted-key string into path segments.                                               | No `a[0].b` bracket syntax — an array index is just another dot-separated segment.                                                                                                                                   |
| `lastInWins`                       | —                | The default `onCollision`: whatever's incoming replaces whatever's there.                   |                                                                                                                                                                                                                      |
| `undotMap`                         | —                | Expand dotted top-level keys into nested maps.                                              | Applies shallowest-key-first so the result doesn't depend on map iteration order; `onCollision` (default `deepMerge`) resolves a leaf two keys both reach.                                                           |
| `dotMap`                           | ≈ `flattenDepth` | Depth-first flatten of a nested map into dotted keys.                                       | `options.maxDepth` caps how many levels collapse; arrays and empty maps are leaves; lossy against a key that already contains a dot.                                                                                 |
| `buildNestedChoices`               | —                | Build the `{name, displayName, entries}` for custom Part Features from a uniform nested map |                                                                                                                                                                                                                      |
| ~~transform~~                      | ≡                | `reduce`-like, but building up a map/array accumulator.                                     | `foldArray` *(std)* already reduces to any accumulator shape — a thin rename, not new capability.                                                                                                                    |
| ~~functions~~                      | ≡                | Names of a map's function-valued entries.                                                   | `keys(omitBy(bag, (val) => !(val is function)))` covers it today.                                                                                                                                                    |
| ~~result~~                         | ≡                | Like `getAt`, but invokes a function value found at the path.                               | Marginal over `getAt` plus a manual call at the call site.                                                                                                                                                           |
| ~~invoke~~                         | ≡                | Call a method found at a path, with arguments.                                              | Same marginal-over-`getAt` judgment as `result`.                                                                                                                                                                     |

Not implemented: `assign`, `assignWith`, `defaults`, `defaultsDeep`, `unset`, `update`, `updateWith`, `setWith`, and
`mergeWith` all mutate their first argument in place in lodash — see
[Mutating (in-place) Functions](#mutating-in-place-functions). `assignIn`, `assignInWith`, `extend`,
`extendWith`, `hasIn`, `valuesIn`, `forIn`/`forInRight`/`forOwn`/`forOwnRight`, `functionsIn`, and
`entriesIn`/`toPairsIn` are [Not Planned](#not-planned) — FeatureScript maps have no
own-vs-inherited-property distinction for those to draw.

### Collection

General collection predicates that don't fit Walking, Set Operations, or the more specific
buckets above.

| Function Name             | Lodash     | Description                              | Caveats                                        |
| ------------------------- | ---------- | ---------------------------------------- | ---------------------------------------------- |
| `sizeof` / `size` *(std)* | ≡          | Element/key/character count.             | `0` for `undefined`.                           |
| `arrayIncludes`           | `includes` | Whether a value appears in a collection. | Map or array; no substring search on a string. |
| `all` *(std)*             | `every`    | Whether every element passes a rule.     |                                                |
| `any` *(std)*             | `some`     | Whether any element passes a rule.       |                                                |
| `filter` *(std)*          | ≡          | Elements passing a rule.                 |                                                |
| `foldArray` *(std)*       | `reduce`   | Reduce a collection to a single value.   |                                                |

### Random

| Function Name             | Lodash     | Description                              | Caveats                                        |
| ------------------------- | ---------- | ---------------------------------------- | ---------------------------------------------- |
| ~~sample~~ / ~~sampleSize~~ / ~~shuffle~~ | `sample` family | Random element(s) / shuffled order. | Needs a seeded pseudo-random generator — FeatureScript has no entropy source, but a linear-congruential generator seeded by a caller-supplied number is just arithmetic. Same workaround `random` ([Simple Types](#simple-types), below) is waiting on. |

### Stats

| Function Name      | Lodash       | Description                                                       | Caveats                                                                                                           |
| ------------------ | ------------ | ----------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------- |
| `min` *(std)*      | ≡            | Smaller of two values, or of an array.                            | Value must be comparable (number, `ValueWithUnits`, etc).                                                         |
| `max` *(std)*      | ≡            | Larger of two values, or of an array.                             | Same comparability rule as `min`.                                                                                 |
| `average` *(std)*  | `mean`       | Average of an array.                                              |                                                                                                                   |
| `sum` *(std)*      | ≡            | Sum of an array.                                                  |                                                                                                                   |
| `maxBy` / `minBy`  | `*By` family | Element of an array for which a computed value is greatest/least. | `undefined` for an empty array — std's array `max`/`min` pick the value itself, these pick the element behind it. |
| `meanBy` / `sumBy` | `*By` family | `average`/`sum`, func-mapped.                                     |                                                                                                                   |

### Sort

`cmp`/`cmpTo` give every FeatureScript type a total-ish ordering (comparing incompatible types
throws, except through `cmpAny`), and `orderBy`/`orderAnyBy` sort a collection by one or more
computed keys on top of that — lodash's `sortBy` is the single-axis case of `orderBy` with no
`orders` or `comparators` override, so it isn't ported separately.

| Function Name    | Lodash         | Description                                                                                                 | Caveats                                                                                                       |
| ---------------- | -------------- | ----------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------- |
| `strOrderBy`     | —              | Sorts an array of values by their stringified form, via a bucket-count trick rather than a real comparator. | Values with equal string forms all sort together but keep no defined relative order.                          |
| `strOrderByUniq` | —              | `strOrderBy`, deduplicated.                                                                                 |                                                                                                               |
| `cmp` / `cmpTo`  | ≈              | Three-way comparison (`-1`/`0`/`1`) between two values of the same type. | Comparing incompatible types throws; comparing strings is nutty in FeatureScript -- it's not built in so we have a hack, via `strOrderBy`. |
| `cmpAny`         | ≈              | Three-way comparison (`-1`/`0`/`1`), returns a total order on any combination of values — falls back to comparing by type when the types themselves differ. | Never throws, unlike `cmp`. |
| `cmpMapByKeysOnly` | —            | Compares two maps by their key lists alone, ignoring values. | `cmpTo` on two maps uses this first, then breaks ties by comparing values. |
| `orderBy` / `sortBy` | ≡          | `collection` (array or map, keys discarded) stably sorted by one `funcOrPath` per sort axis, each with its own `order` and `comparator`. | An array `funcOrPaths` is always a list of axes, so a deep path needs its dotkey spelling (`"a.b"`, not `["a", "b"]`). The single-axis case is what lodash calls `sortBy`, so there's no separate port of that name. |
| `orderAnyBy`     | —              | `orderBy`, with `cmpAny` standing in wherever a `comparator` goes unnamed, so mixed/incompatible types across results never throw. | |
| ~~sortedUniq~~ / ~~sortedUniqBy~~ | ≡ | `uniq`/`uniqBy`, optimized for sorted input. | Redundant with `uniqBy`/std `deduplicate` at this project's scale. |
| ~~sortedIndex~~ / ~~sortedIndexBy~~ / ~~sortedIndexOf~~ / ~~sortedLastIndex~~ / ~~sortedLastIndexBy~~ / ~~sortedLastIndexOf~~ | `sortedIndex*` family | Binary-search insertion points into an already-sorted array. | std's `sort(arr, compareFunction)` makes this possible for numbers, but the 6-function family is a lot of surface for an optimization this codebase has no hot path for. |

---

## Basics

### String

| Function Name | Lodash | Description | Caveats |
| --- | --- | --- | --- |
| `strSlice` | `slice` | JS-style slice: negative indexes count from the end, either index clamps into range. | `endseq` accepts `SequencePosition.END` in place of a literal length, for a call site that must supply all three arguments (e.g. inside a fixed-arity callback). |
| `strTake` / `strTakeRight` | — | First/last `len` characters of a string. | `len <= 0` is `""`. |
| `padLeft` | `padStart` | Pad a string (or number) on the left. | Truncates the padding if it overshoots `minlen`; a `number` overload stringifies first. |
| `padRight` | `padEnd` | Pad a string on the right. | No numeric overload — a `number` must be stringified by the caller first. |
| `pad` | ≡ | Pad both sides, splitting as evenly as possible. | Right side gets the extra character when the padding is odd. |
| `strRepeat` / `repeatString` *(std)* | `repeat` | Repeat a string `n` times. | |
| `starbanner` | — | Wraps a string in a `***`-bordered banner, for a `debug()` call that wants to stand out. | |
| `hasMatch` | — | Whether a regex matches anywhere in a string. | `false` (not a throw) on `undefined` input or a bad pattern. |
| `upcase` | `toUpper` | ASCII-only uppercase. | Explicit character lookup; a non-letter passes through unchanged. |
| `downcase` | `toLower` | ASCII-only lowercase. | Same lookup-table approach as `upcase`. |
| `titleCase` | `startCase` | Word-boundary capitalization. | Splits on a configurable delimiter set (default `-_`) rather than Unicode word-boundary detection; an optional per-character translation can run ahead of capitalization. |
| `words` | ≡ | Split a string into words. | Simplified: splits only on non-alphanumeric delimiter runs, not on camelCase boundaries or digit runs the way lodash's own `words` does. |
| `camelCase` | ≡ | Words joined with no separator; first word lower, the rest capitalized. | Built on `words` — inherits its simplified word-boundary rule. |
| `kebabCase` | ≡ | Words joined with `-`, all lowercase. | Built on `words`. |
| `snakeCase` | ≡ | Words joined with `_`, all lowercase. | Built on `words`. |
| `lowerCase` | ≡ | Words joined with a space, all lowercase. | Built on `words`. |
| `upperCase` | ≡ | Words joined with a space, all uppercase. | Built on `words`. |
| `capitalize` | ≡ | First character uppercased, the rest lowercased. | |
| `upperFirst` / `lowerFirst` | ≡ | Only the first character's case changed; everything else left as-is. | |
| `escapeRegExp` | ≡ | Backslash-escape a string's regex metacharacters. | So it matches literally when dropped into `match`/`replace`/`splitByRegexp`; `trimStart`/`trimEnd` use it to build a safe `[...]` character class from a caller-supplied `chars` argument. |
| `trim` / `trimStart` / `trimEnd` | ≡ | Strip whitespace (or a given character set) from either/both ends. | |
| `truncate` | ≡ | Cut a string to a length, with an omission marker. | No `separator` option to break at a word/regex boundary instead of an exact count. |
| `startsWith` *(std)* / `endsWith` *(std)* | ≡ | Whether a string starts/ends with a substring. | |
| `replace` *(std)* | ≡ | Replace matches in a string. | |
| `stringToNumber` *(std)* | `parseInt` | Parse a string as a number. | |
| `splitByRegexp` *(std)* / `splitIntoCharacters` *(std)* | `split` | Split a string. | std splits by regexp or into characters — no plain-substring/limit split like lodash's. |

Possible, not yet built:

| Function Name | Lodash | Description | Caveats |
| --- | --- | --- | --- |
| ~~deburr~~ | ≡ | Strip Latin-1 diacritics (`café` → `cafe`). | Same technique as `upcaseChar`/`downcaseChar`'s explicit lookup table, just a bigger one mapping every accented Latin-1 character to its bare-ASCII form. |

### Simple Types

| Function Name | Lodash | Description | Caveats |
| --- | --- | --- | --- |
| `isNil` / `isPresent` | `isNil`, `isNull`, `isUndefined` | Is the value absent / present? | FeatureScript has no `null`, so all three JS distinctions collapse into one `undefined` check. |
| `ifNil` | `defaultTo` | Value, or a fallback if nil. | |
| `ifBlank` | — | Value, or a fallback if nil or an empty string. | |
| `ifZero` | — | Value, or a fallback if nil or (within tolerance) zero. | Overloaded for `number` and `ValueWithUnits`. |
| `truthy` | — | Neither `undefined` nor `false`. | `0` and `""` are truthy, unlike JS's wider falsey set. |
| `strBlank` | — | Whether a value is `undefined` or `""`. | |
| `isEmpty` | ≡ | Is the value empty? | Restricted to the map/string/array/`undefined` cases FeatureScript actually has; unlike lodash, a number or boolean is never empty. |
| `castArray` | ≡ | Wrap a non-array value in a 1-element array. | Value passes through unchanged if it's already an array. |
| `sizeof` | `size` | Key count (map) / character count (string) / element count (array) / `0` (`undefined`). | |
| `vector2` | — | Drops a `Vector`'s z component. | |
| `mm`, `zero` | — | `millimeter` shorthand, and `0 * mm`. | |
| `==` *(lang)* | `isEqual`, `isEqualWith` | Deep structural equality. | FeatureScript's `==` already does deep value comparison on maps/arrays — the default equality operator, not a function. |
| `>` / `>=` / `<` / `<=` *(lang)* | `gt`, `gte`, `lt`, `lte` | Numeric comparison. | Plain comparison operators, numbers only — no lodash-style mixed-type coercion. |
| `toString` *(std)* | ≡ | Converts a value to its string form. | |
| `isInteger` *(std)* | ≡ | Whether a number has no fractional part. | |
| `stringToNumber` *(std)* | `toNumber`/`parseInt` | Coerce a value to a number. | Only the one real case (parsing a numeric string) comes up in this codebase. |
| `` `is array`/`is map`/`is string`/`is number`/`is boolean`/`is function` `` *(lang)* | `isArray`, `isBoolean`, `isFunction`, `isMap`, `isObject`, `isPlainObject`, `isNumber`, `isString` | Type-check a value. | A language expression, not a function call. |
| `clamp` *(std)* | ≡ | Constrain a number to `[lower, upper]`. | |
| `inRange` | ≡ | Whether a number falls within `[start, end)`. | Bounds auto-swap if `start > end`, matching lodash; `start` defaults to `0`. |

Possible, not yet built:

| Function Name | Lodash | Description | Caveats |
| --- | --- | --- | --- |
| ~~random~~ | ≡ | Random number in a range. | FeatureScript has no entropy source, but a seeded linear-congruential generator is just arithmetic — needs a caller-supplied seed rather than true randomness. Same workaround `sample`/`sampleSize`/`shuffle` ([Collection](#collection-1), above) are waiting on. |
| ~~isMatch~~ / ~~isMatchWith~~ | ≡ | Whether a map has the same values as a partial "source" map, at the source's keys. | `pick(obj, keys(source)) == source` covers `isMatch` today, since `==` is already deep equality; `isMatchWith`'s per-key customizer is the one open piece. |
| ~~toInteger~~ | ≡ | Coerce a value to an integer. | A one-line `floor`/`round` over `stringToNumber`, above. |

### Helper Functions

FeatureScript functions are already first-class values with real closures, so most of lodash's
Function and Util categories turn out to be a short closure away — both are folded into this one
section. The genuine gaps need *variadic* calling or `this`/method binding, neither of which
FeatureScript has; those are [Not Planned](#not-planned) rather than repeated per-row here.

| Function Name | Lodash | Description | Caveats |
| --- | --- | --- | --- |
| `curry2to0`…`curry3to2` | `unary`, `ary` | Wraps a function to accept `N` arguments but call it with only the first `M` — for sitting a fixed-arity callback in a `forEach`/`mapValues`-shaped slot. | Covers `unary` (`curryXto1`) and the common `ary` shapes; a new arity just needs one more `curryNtoM` sibling, not a general variadic `ary`. |
| `idsFor` | — | Map of `tags` to a same-named child of an `Id`. | The `ids` map a multi-sketch/multi-op feature declares up front, built in one call. |
| `parseJsonSafely` | — | Parses JSON, wrapping a parse failure in a `regenError` labeled with a caller-supplied story instead of surfacing the raw throw. | `opts.detectUnits` parses unit-bearing strings (e.g. `"3 inch"`) into a `ValueWithUnits`. |
| `attempt` | ≡ | Call a function, returning its result or the error it throws instead of propagating. | No argument-forwarding: FeatureScript has no variadic call syntax. |
| `attemptLoudly` | — | `attempt`, but logs the caught error instead of swallowing it silently. | |
| `constant` | ≡ | Always return a fixed value, ignoring arguments. | The single-argument form only fits a one-argument slot; an `arity` argument (1/2/3) builds the matching shape. |
| `identity` / `identity1` / `identity2` / `identity3` | ≡ | Return the argument unchanged. | Arity variants for sitting in a 1/2/3-argument callback slot. |
| `noop` / `noop0` / `noop1` / `noop2` / `noop3` | ≡ | Does nothing, returns `undefined`. | Arity variants, same reasoning as `identity`. |
| `times` | ≡ | Call a function `n` times, collecting results. | `n < 1` returns `[]`; defaults to `identity` with no `func` given. |
| `rangeRight` / `range` *(std)* | ≡ | Array of numbers from `start` to `end`, descending / ascending. | `rangeRight` inherits std `range`'s inclusive-of-`end` convention, not lodash's exclusive one. |
| `property` / `propertyOf` | ≡ | Build a function that reads one path off whatever it's given / off a fixed object. | Via `getAt`. |
| `matches` / `matchesProperty` | ≡ | Build a rule checking a map against a partial shape / one path against a value. | Rides on the same `pick(obj, keys(source)) == source` trick as `isMatch`. |
| `cond` | ≡ | Build a function trying `[predicate, handler]` pairs in order, running the first match. | Single-argument version — lodash's `cond` forwards every argument it receives, which needs variadic call syntax FeatureScript doesn't have. |
| `conforms` / `conformsTo` | ≡ | Build a rule / test a map against a map of per-key predicates. | |
| `over` / `overEvery` / `overSome` | `overXXXX` | Run several functions against the same argument; collect results / require all / require any. | |
| `iteratee` | ≡ | Coerce a string/map/function "shorthand" into a real function. | `property` / `matches` / pass-through, falling back to `identity`. |

Possible, not yet built:

| Function Name | Lodash | Description | Caveats |
| --- | --- | --- | --- |
| `memoizeFunction` *(std)* | `memoize` | Cache a function's results by argument. | Already a std builtin. |
| ~~flip~~ | ≡ | Swap a 2-argument function's argument order. | `(func) => (a, b) => func(b, a)`. |
| ~~flow~~ / ~~flowRight~~ | ≡ | Compose an array of unary functions, left-to-right / right-to-left. | `foldArray` *(std)* over the function array, seeded with the input. |
| ~~negate~~ | ≡ | Boolean-invert a rule's result. | One per arity already in use. |
| ~~once~~ | ≡ | Call a function at most once; return the first result on every later call. | Needs a `box` to remember "already called" plus the cached result, the same closure-over-`box` idiom `boxarrPush` already uses. |
| ~~before~~ / ~~after~~ | ≡ | Call a function only until / starting at the `n`th call. | Same `box`-counter idiom as `once`. |
| ~~partial~~ / ~~partialRight~~ | ≡ | Fix some leading/trailing arguments, return a function awaiting the rest. | The value-fixing sibling of `curryNtoM`. Also covers `wrap` (`wrap(value, fn)` ≡ `partial(fn, value)`). |
| ~~curry~~ / ~~curryRight~~ | ≡ | General auto-curry of any arity. | `curry2to0`…`curry3to2` already solve the one shape this project needed; a fully generic curry needs variadic arity, which FeatureScript doesn't have. |
| ~~uniqueId~~ | ≡ | Generate a monotonically-increasing id. | Needs a counter living in the `Context` (`setVariable`/`getVariable`) rather than a plain module-global variable, which FeatureScript has no equivalent of. |

### Metadata

No lodash correspondence — Onshape-specific entity/attribute plumbing.

| Function Name | Lodash | Description | Caveats |
| --- | --- | --- | --- |
| `setPropAndAttribute` | — | Sets a property and mirrors it into a same-named attribute. | Onshape won't let a feature read its own properties back mid-regeneration — only the attribute survives that round trip. |
| `setName` | — | `setPropAndAttribute` for the Name property/attribute. | |
| `setReadableName` | — | `setName`, after collapsing whitespace runs to one space and truncating to a max length (default `20`). | |
| `getAttrs` | — | `{ thing, attrName, val }` for every entity in a query, falling back to a default where the attribute is unset. | |
| `getAllAttrs` | — | Every attribute on one entity, as `{name: value}`. | `{"ok": false, "err": err}` if the read throws. |
| `getBestAttr` | — | The first `getAttrs` entry whose value isn't the ignored sentinel, or the first entry if every one of them is. | `undefined` if the query is empty. |
| `getNameProps` / `getNames` / `getName` / `getNameProp` | — | `getAttrs`/`getBestAttr`, specialized to `"Name"`. | `getName` falls back to `ignoredVal` (default `"Part"`) when the query resolves to nothing, rather than dereferencing `undefined`. |
| `getNameOfBody` | — | `"Name"` attribute directly on a body, not its best/first entity. | |
| `defaultMaybe` | — | Value for a derived field: kept auto-derived from its base field for as long as it hasn't been hand-edited away from what the derivation would have produced. | The pattern behind every `*EditLogic` function that keeps a variable name in sync with what it names. |
| `sanitize_varname` / `field_varname` | — | Turn a string into a legal-ish identifier. | Each `.` becomes `_`; every other non-word character becomes `__`, independently — no run-collapsing. |
| `PL_TOP`, `hugeSizeVal` / `tinySizeVal` | — | Shorthand top-plane constant; sentinel min/max for a `LengthBoundSpec` with no practical limit. | |

### Color

No lodash correspondence — color parsing isn't something lodash does.

| Function Name | Lodash | Description | Caveats |
| --- | --- | --- | --- |
| `setColor` | — | Sets the `APPEARANCE` property on a query. | Accepts a `Color`, or anything `toColor` accepts (hexcolor string, tuple string, or array). |
| `hexcolorToColor` | — | Parses `"#rrggbb"`/`"#rrggbbaa"` into a `Color`. | Leading `#` optional, either case; `OopsColor` (bright red) on no match. |
| `tuplestrToColor` | — | Parses a 0–255 RGB(A) tuple string (`"200,99,100,33"` or `"[0, 1, 255]"`) into a `Color`. | |
| `unitcolorToColor` | — | Parses a 0.0–1.0 RGB(A) tuple string into a `Color`. | |
| `toColor` | — | Dispatches to whichever of the above fits. | A `Color` passes through; an array goes straight to `color(...)` (no 0–255 detection); a string is sniffed hex → unit → tuple, first match wins. `OopsColor` if nothing matches. |
| `toHexcolor` / `toUnitcolor` / `toTuplecolor` | — | The reverse conversions, `Color` → string/array. | |
| `hexpairToInt` / `intToHexpair` | — | 2-character hex string ↔ 0–255 decimal. | Case-insensitive going in. |
| `sameColor` | — | Channel-by-channel tolerant equality. | Default tolerance is just under `1/255`; a missing `alpha` on either side is treated as `1.0`. |
| `isHexcolor` | — | Whether a string is a hexcolor string `hexcolorToColor` would accept. | |

### JSON

A collection of Features, not a library — each reads/writes variables on a live `Context`, so none
of them have plain-data tests.

| Function Name | Lodash | Description | Caveats |
| --- | --- | --- | --- |
| `jsonVarF` | — | Parses a JSON string and sets it as a variable. | |
| `keylistF` / `keylistEditLogic` | — | Sets a variable to the ordered keys of another variable. | Edit logic keeps the result variable's name tracking the source's, until hand-edited. |
| `sizeofF` / `sizeofEditLogic` | — | Sets a variable to `sizeof` of another variable. | Same tracking-until-hand-edited edit logic. |
| `valuesAtF` / `valuesAtEditLogic` | — | Sets a variable to `valuesAt(bag, keylist)` for a source variable and a JSON-array key list. | Edit logic tracks both the result variable's name and its description. |
| `splatF` | — | Explodes a variable's value into one variable per entry. | `<objname>_<00-padded index>` for an array; `<objname>_<key>` (or bare `<key>` if `prefixVarnames` is turned off) for a map. |

### Debug

No lodash correspondence — viewport highlighting.

| Function Name | Lodash | Description | Caveats |
| --- | --- | --- | --- |
| `highlightQuery` | — | `addDebugEntities` on a query, plus the edges of its owning bodies (otherwise invisible through an occluding body's faces). | A no-op unless `debugMe` is `true` (the default), so a call site can leave the call in place and flip one flag. |

---

## Bugs found and fixed along the way

* **`clxn/clxnUtils.fs`:** `NextStepAction` (the `forEach`/`mapValues`-family early-exit sentinel)
  was never `export`ed, so no caller outside the file could ever actually trigger `BREAK` — every
  walk anywhere in the codebase always ran to completion. Exported now, and covered by
  `runForEachBreakTests`/`runForEachKeylistBreakTests` in `clxn/testClxnUtils.fs`.
* **`prim/testStringUtils.fs`:** `runPadTests` had four `return runTests(...)` statements
  stacked in a row — only the first ever ran, so `padRight` was never tested at all, and the more
  interesting padding-behavior cases (`LeftPadTestCases`/`RightPadTestCases`) silently never
  executed either.
* **`prim/typeUtils.fs`:** `ifZero` had two dead, unreachable private overloads after the real
  (exported) ones — one untyped, one with the exact same `(val is ValueWithUnits, fallback)`
  signature as the exported version above it, which is either a silent duplicate-definition or a
  compile error depending on how FeatureScript resolves it. Deleted both.
* **`fancy/metadataUtils.fs`:** `getName` called `.val` on whatever `getNameProp` returned without
  checking for `undefined` first — and `getNameProp`/`getBestAttr` return `undefined` exactly
  when `query` resolves to no entities, which is not a rare case. Fixed to fall back to
  `ignoredVal` in that case, matching every sibling function's documented contract.
* **`fancy/colorUtils.fs`:** `HexcolorRE` only matched lowercase hex digits (`[0-9a-f]`), so
  `hexcolorToColor`, `toColor`, and `isHexcolor` all silently failed on a perfectly standard
  uppercase or mixed-case hex string like `"#FF0000"` — falling back to `OopsColor` (or `false`
  for `isHexcolor`) instead of parsing it. Widened to `[0-9a-fA-F]`; `hexpairToInt` already
  downcased its input, so nothing else needed to change.
* **`features/jsonVarF.fs`:** `keylistF`'s throw message named `definition.varname` (the
  *destination* variable being written) when reporting that the *source* variable wasn't a bag or
  array — a debugging-time red herring, since the name in the error never matched the variable
  actually at fault. Now names `bagname`, the variable that was actually checked.
* **`clxn/clxnGetset.fs`:** the string-keyed overload of the private write-path helper
  `seqForPlacement(arr, seq is string)` called `seqForSegment` — the *read*-path helper, which
  returns `Sentinel.ABSENT` for an out-of-range index — instead of recursing into its own
  numeric-argument overload, the *write*-path helper that pads past-the-end indices instead of
  rejecting them. `setAt(arr, "5", val)` on a shorter array would have received `Sentinel.ABSENT`
  as the placement index instead of `5`, rather than padding the array as documented. Fixed to
  call `seqForPlacement(arr, stringToNumber(seq))`.
* **`clxn/clxnUtils.fs`:** `dropWhile`/`dropRightWhile`'s 1-arg overloads defaulted their rule to
  bare `truthy` — a 1-arg function — but their own bodies call `rule(val, seq)` with two
  arguments, an arity mismatch that would throw the moment either 1-arg overload actually ran.
  Surfaced while adding the matching `take`/`takeRight`/`takeWhile`/`takeRightWhile` default
  overloads. Fixed to default to `curry2to1(truthy)` instead, applied to the new `takeWhile`/
  `takeRightWhile` 1-arg overloads too.
* **`fancy/testColorUtils.fs`:** the whole file was replaced — `runColorRoundtripTests` wrapped
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

| Lodash | Function Name | Description |
| --- | --- | --- |
| `pull`, `pullAll`, `pullAllBy` | `difference`, `differenceBy` | Remove given values from an array. |
| `pullAllWith` | `differenceWith` | Remove given values from an array, by custom comparator. |
| `remove` | `partition` (keep the removed half) | Split an array by a rule. |
| `fill` | *(not built — `subArray` + `makeArray` + `concatenateArrays`)* | Overwrite a sub-range of an array with a fixed value. |
| `assign` | `mergeMaps` *(std)* | Shallow-copy one map's keys onto another. |
| `assignWith` | `assignWith` | Shallow-copy, with a custom per-key combiner; `undefined` falls back to `incoming` winning. |
| `defaults`, `defaultsDeep` | `deepMerge(source, dest)` *(args flipped)* | Fill in missing keys from a fallback object. |
| `unset` | `setAt(bag, path, undefined)` | Delete whatever's at a path. |
| `update`, `updateWith` | `update`, `updateWith` | Read-modify-write a value at a path — `setAt(bag, path, updater(getAt(bag, path)))`. |
| `setWith` | `setAtWith` | Set a path, with a customizer for autovivified segments. |
| `mergeWith` | `mergeWith` | Deep-merge two maps, with a per-key combiner called at every level; `undefined` falls back to `deepMerge`'s own rule. |

## Lodash aliases

Lodash sometimes offers two names for the same function; the tables above show only the one this
project settled on: `first` (not `head`), `forEach` (not `each`), `forEachRight` (not `eachRight`),
and `toPairs` (not `entries`).

## Not Planned

Every function that showed up in a "not yet"/"not implemented" bucket anywhere above has been
resolved one way or the other: either it's a real (if sometimes struck-through, sometimes
workaround-needing) row in a section above, a row in [Mutating (in-place)
Functions](#mutating-in-place-functions), or it's not planned, for one of a handful of recurring
reasons tabulated here — no more scattered per-row cross-references.

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
