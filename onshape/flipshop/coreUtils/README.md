# coreUtils

General-purpose FeatureScript helpers shared across this project's features — not
geometry-specific, just the plumbing every feature ends up needing. Style follows
[`STYLE-Featurescript.md`](../STYLE-Featurescript.md); the standard-library map/array/string
functions these build on are catalogued in [`onshape/README.md`](../../README.md).

This is a starting summary, one line per exported function/constant. `colorUtils.fs` and
`jsonVarF.fs` are still at brief/prose treatment rather than a full table — everything else has
had a full pass (tests, docblocks, lodash correspondence where one exists). `coreUtils.fs` itself
has no functions of its own — it just re-exports the documents the other files in this directory
compile into, so other features can `import` this one file for all of it.

## Tests

One `run<Thing>Tests` function per case list, in `tests/`. All `clxn*` suites — everything from
`clxnWalking.fs` and from `clxnGetset.fs`/`clxnReshape.fs` — run out of a single Feature,
`runClxnTestsFS` in `tests/testClxnWalking.fs`. Adding a new `clxn*` function's tests means
adding its `run*Tests` call to that Feature, not creating a new one. Every other file gets its
own `test<File>.fs` and `run<File>TestsFS` Feature (`sizeof`'s tests are the one exception,
living under `runCoreUtilsTestsFS` in `tests/testCoreUtils.fs`, since `sizeof` predates the
`clxn*` split).

Two files' worth of functions have no test suite at all: `debugUtils.fs`'s `highlightQuery` and
most of `metadataUtils.fs` (`setPropAndAttribute`, `setName`, `setReadableName`, `getAttrs`,
`getAllAttrs`, `getBestAttr`, `getNameProps`, `getNames`, `getName`, `getNameProp`,
`getNameOfBody`) read or write properties/attributes on a live `Query`, which the
plain-data-table `runTests` harness this whole test suite is built on has no way to exercise
without an actual part studio to run against. `metadataUtils.fs`'s three pure functions
(`defaultMaybe`, `sanitize_varname`, `field_varname`) are tested in `testMetadataUtils.fs`
despite that.

---

## Lodash correspondence

Much of this directory is an ongoing port of [lodash](https://lodash.com/docs)'s conveniences
into FeatureScript, pulled in as needed from the reference copy at `lodash.js`. Where a function
here has a clear lodash counterpart, its docblock is written from that counterpart's own
description, and the mapping is recorded here. Not everything has one — see the file-by-file
catalogs below for the rest.

### `clxnGetset.fs`

| FeatureScript | lodash | Notes |
|---|---|---|
| `getAt` | `get` | Array segments accept a negative index, which lodash's `get` does not. |
| `setAt` | `set` | Autovivifies only maps, never arrays, even for an integer segment (lodash builds an array there). A leaf collision goes through a pluggable `onCollision` instead of always overwriting. Returns a new value rather than mutating in place. |
| `deepMerge` | `merge` | Two arrays replace each other instead of merging index-by-index. Takes exactly two arguments, not a variadic source list. Returns a new value rather than mutating in place. |
| `pathForKey` | `toPath` | No `a[0].b` bracket syntax — an index is just another dotted segment (`"a.0.b"`). |

### `clxnWalking.fs`

| FeatureScript | lodash | Notes |
|---|---|---|
| `sizeof` | `size` | Also accepts `undefined`, returning `0`. |
| `hasKey` | `has` | `key` is a single literal key/index, never a dotted path. The array overload adds a `missingPolicy` (present-but-`undefined` counts unless `SKIP`) and never accepts a negative index. |
| `arrayIncludes` | `includes` | Array-only — no substring search on a string, no value search over a map. |
| `pick` | `pick` | Takes one explicit key array rather than variadic paths; otherwise the same "absent key stays absent" behavior. |
| `pickDefined` | `pickBy` | Fixed predicate ("not `undefined`") over a given `keylist`, rather than an arbitrary predicate over every key of the object. |
| `arrLast` | `last` | Direct match. |
| `valuesAt` | `at` | No path traversal (literal keys/indexes only) and no negative array indices. Adds a `missingPolicy` to drop absent slots instead of always leaving an `undefined` gap. |
| `forEach` | `forEach` | Early exit is returning `NextStepAction.BREAK`, not any falsy value. Adds `missingPolicy` and an explicit `keylist` walk order. |
| `mapValues` / `mapValues3` | `mapValues` (map) / `map` (array) | Unifies lodash's two separate functions under one dispatch. `mapValues3` adds a 0-based `seq` neither lodash callback gets. Adds `keylist` and `missingPolicy`. |
| `objectify` | `keyBy` | Key/value roles are swapped from lodash: `objectify` keys by the element itself and lets `func` compute the value, where `keyBy` keys by `iteratee(value)` and keeps `value` as-is. |

### `stringUtils.fs`

| FeatureScript | lodash | Notes |
|---|---|---|
| `padLeft` | `padStart` | Also overloaded for a `number`, stringified first. |
| `padRight` | `padEnd` | Same numeric overload as `padLeft`. |
| `strRepeat` | `repeat` | Direct match. |
| `upcase` | `toUpper` | ASCII-only, via an explicit character lookup — no Unicode case folding. |
| `downcase` | `toLower` | Same ASCII-only limitation as `upcase`. |
| `titleCase` | `startCase` | Configurable word-break characters (default `-_`, not Unicode word-boundary detection) and an optional per-character translation map. |

### `typeUtils.fs`

| FeatureScript | lodash | Notes |
|---|---|---|
| `ifNil` | `defaultTo` | Only checks `undefined` — FeatureScript has no `null`/`NaN` to also catch. |
| `isNil` | `isNil` | Same `undefined`-only narrowing. |
| `isEmpty` | `isEmpty` | Restricted to the map/string/array/`undefined` cases FeatureScript actually has. |

### `miscUtils.fs`

| FeatureScript | lodash | Notes |
|---|---|---|
| `noop` | `noop` | Direct match. |

`debugUtils.fs` and `metadataUtils.fs` have no entries here — neither has a lodash counterpart
for anything they export (Onshape-specific viewport/attribute plumbing).

---

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

## `stringUtils.fs` — string slicing, padding, and case conversion

| Export | Does |
|---|---|
| `strSlice(str, begseq, endseq?)` | JS-style slice: negative indexes count from the end, either index clamps into range. `endseq` accepts `SequencePosition.END` in place of a literal length. |
| `strTake(str, len)` / `strTakeRight(str, len)` | First/last `len` characters; `len <= 0` is `""`. |
| `padLeft(str\|num, minlen, padstr?)` / `padRight(...)` | Pads to `minlen` with `padstr` (default `" "`), truncating the padding if it overshoots; a `number` is stringified first. |
| `strRepeat(str, reps)` | `str` repeated `reps` times. |
| `starbanner(str)` | Wraps `str` in a `***`-bordered banner, for a `debug()` call that wants to stand out. |
| `hasMatch(str, regex)` | Whether `regex` matches anywhere in `str`; `false` (not a throw) on `undefined` input or a bad pattern. |
| `upcase(str)` / `downcase(str)` | ASCII-only case flip via an explicit character lookup; a non-letter passes through unchanged. |
| `titleCase(str, opts?)` | Splits on `opts.spaces` (default `-_`), capitalizes each word's first letter, lower-cases the rest; `opts.tr` translates individual characters before capitalization. |

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
| `vector2(vec)` | Drops `vec`'s z component. |
| `mm`, `zero` | `millimeter`, and `0 * mm`. |

## `debugUtils.fs` — viewport highlighting

| Export | Does |
|---|---|
| `highlightQuery(context, qq, debugColor, debugMe?)` | `addDebugEntities` on `qq` plus the edges of its owning bodies (otherwise invisible through an occluding body's faces); a no-op unless `debugMe` is `true` (the default), so a call site can leave the call in place and flip one flag. |

---

## Other files (brief)

### `colorUtils.fs` — Color ↔ hex/tuple string conversion

`setColor` (Color, hexcolor, or `[r,g,b,a?]` array), `hexcolorToColor`/`tuplestrToColor`/
`unitcolorToColor` (parse `"#rrggbb[aa]"` / `"r,g,b[,a]"` 0–255 / `"r,g,b[,a]"` 0–1 strings),
`toColor` (dispatches to whichever of the above matches), `toHexcolor`/`toUnitcolor`/
`toTuplecolor` (the reverse conversions), `hexpairToInt`/`intToHexpair`, `sameColor` (tolerant
equality), `isHexcolor`.

### `jsonVarF.fs` — variable-producing utility Features

`jsonVarF` (parse a JSON string into a named variable), `keylistF` (a map/array's keys as a
variable), `sizeofF` (`sizeof` as a variable), `valuesAtF` (`valuesAt` as a variable), `splatF`
(explodes a map/array into one variable per entry) — plus each one's `*EditLogic` function.

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
