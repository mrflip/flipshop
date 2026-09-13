# coreUtils

General-purpose FeatureScript helpers shared across this project's features — not
geometry-specific, just the plumbing every feature ends up needing. Style follows
[`STYLE-Featurescript.md`](../STYLE-Featurescript.md); the standard-library map/array/string
functions these build on are catalogued in [`onshape/README.md`](../../README.md).

This is a starting summary, one line per exported function/constant. It does not (yet) cover
every file in this directory in equal depth — the `clxn*` collection utilities got the deepest
pass, since they're the most heavily used and the least self-explanatory from the call site.
`coreUtils.fs` itself has no functions of its own — it just re-exports the documents the other
files in this directory compile into, so other features can `import` this one file for all of it.

## Tests

One `run<Thing>Tests` function per case list, in `tests/`. All `clxn*` suites — everything from
this file and from `clxnGetset.fs`/`clxnReshape.fs` — run out of a single Feature,
`runClxnTestsFS` in `tests/testClxnWalking.fs`. Adding a new `clxn*` function's tests means
adding its `run*Tests` call to that Feature, not creating a new one. (`sizeof`'s tests live
under `runCoreUtilsTestsFS` in `tests/testCoreUtils.fs` instead, since `sizeof` predates the
`clxn*` split.)

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

### Quirks worth knowing

* `pick` and `pickDefined` produce the same result on a map — kept both because "defined" reads
  clearer at some call sites (`embedBodiesF.fs` uses `pickDefined`).
* `hasKey`/`hasPresentKey`/`valuesAt` on an array never accept a negative index; `getAt`/`setAt`
  in `clxnGetset.fs` do. Don't assume the negative-index convention is uniform across this
  directory.
* `NextStepAction` used to be un-exported, so nothing outside `clxnWalking.fs` could ever
  actually trigger `BREAK` — every `forEach`/`mapValues` walk anywhere in the codebase always ran
  to completion. It's exported now (see `runForEachBreakTests` / `runForEachKeylistBreakTests`
  in `tests/testClxnWalking.fs`), so early-exit is available and verified going forward.

---

## Other files (brief)

### `colorUtils.fs` — Color ↔ hex/tuple string conversion

`setColor` (Color, hexcolor, or `[r,g,b,a?]` array), `hexcolorToColor`/`tuplestrToColor`/
`unitcolorToColor` (parse `"#rrggbb[aa]"` / `"r,g,b[,a]"` 0–255 / `"r,g,b[,a]"` 0–1 strings),
`toColor` (dispatches to whichever of the above matches), `toHexcolor`/`toUnitcolor`/
`toTuplecolor` (the reverse conversions), `hexpairToInt`/`intToHexpair`, `sameColor` (tolerant
equality), `isHexcolor`.

### `stringUtils.fs` — string slicing, padding, and case conversion

`strSlice` (Python-style slice, negative indexes, `SequencePosition.END`), `strTake`/
`strTakeRight` (first/last N chars), `padLeft`/`padRight` (string or number), `strRepeat`,
`starbanner` (wraps a message in a `***` banner for `debug`), `hasMatch` (regex test that
doesn't throw), `upcase`/`downcase` (ASCII-only case flip), `titleCase` (configurable word-break
characters and per-character translation map).

### `metadataUtils.fs` — entity names and attributes

`setPropAndAttribute`/`setName`/`setReadableName` (mirror a property into a same-keyed attribute,
since properties can't be read back mid-regeneration), `getAttrs`/`getBestAttr` (per-entity
attribute values across a query, with a "best" one picked over a sentinel default),
`getNameProps`/`getNames`/`getName`/`getNameProp`/`getNameOfBody` (the above specialized to the
`"Name"` attribute), `getAllAttrs`, `defaultMaybe` (editing-logic helper: keep a derived field at
its default only while it hasn't been hand-edited), `sanitize_varname`/`field_varname`.
Also: `PL_TOP`, `hugeSizeVal`/`tinySizeVal` (bound-spec sentinels).

### `jsonVarF.fs` — variable-producing utility Features

`jsonVarF` (parse a JSON string into a named variable), `keylistF` (a map/array's keys as a
variable), `sizeofF` (`sizeof` as a variable), `valuesAtF` (`valuesAt` as a variable), `splatF`
(explodes a map/array into one variable per entry) — plus each one's `*EditLogic` function.

### `miscUtils.fs` — small combinators

`idsFor` (an `ids`-map from a base `Id` and a tag list), `noop`, `curry2to0`/`curry2to1`/
`curry2to2`/`curry3to0`/`curry3to1`/`curry3to2` (drop trailing arguments so a fixed-arity
callback fits a `forEach`/`mapValues` slot), `parseJsonSafely` (wraps `parseJson`/
`parseJsonWithUnits` with a `regenError` on failure instead of an opaque throw).

### `typeUtils.fs` — presence checks and small numeric/string guards

`ifNil`/`ifBlank`/`ifZero` (fallback when `undefined`/blank-string/zero), `isPresent`/`isNil`/
`truthy`/`strBlank`/`isEmpty`, `vector2` (drops a `Vector`'s z component), `mm`/`zero`
(shorthand constants).

### `debugUtils.fs` — viewport highlighting

`highlightQuery` — `addDebugEntities` a query plus its owning bodies' edges (so it's visible
through occluding faces), gated behind a `debugMe` flag so call sites can leave it in and toggle
it off.
