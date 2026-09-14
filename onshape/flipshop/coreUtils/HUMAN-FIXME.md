# Process Updates from Agent and Human Go Here When Reasonable

## Pre-release TODO:

* pickBy and setAtWith (clxnGetset.fs) have no tests at all.
* doMany, attemptLoudly (helperFuncs.fs) — untested.
* features/jsonVarF.fs has no test file — all five exported Features (jsonVarF, keylistF, sizeofF, valuesAtF, splatF) are uncovered.
* setColor and the getAttrs/getName* family (metadataUtils.fs) are untested, but plausibly out of scope for a pure-function harness since they need a live Context/Query.
* runSetAtThrowsTests/runUndotMapThrowsTests only run if (verbose) — worth confirming that's deliberate, since it means the default run skips them.

## Thinking about what to exclude from the lodash translation:

Fuller reasoning for the categories in [README.md](README.md)'s "Decisions: not planned" table
that need more than a one-word reason.

* **templating**: `template`, `escape`, `unescape` compile/interpolate strings or escape HTML —
  real work for a feature this project has never needed. `escapeRegExp` doesn't belong in this
  bucket despite living next to these three in lodash's own docs — it's a plain character-escape
  function with nothing to do with templates or HTML, and is filed as a real, unbuilt candidate in
  README.md's String section instead.
* **async**: `debounce`, `throttle`, `defer`, `delay` all assume a timer/event-loop FeatureScript
  regeneration doesn't have.
* **date**: `now`. (An earlier pass filed `after`/`before` here too — that was a mistake; they're
  the Function-category call-count wrappers, not date predicates, and are real, unbuilt
  candidates now — see README.md's Function section.)
* **chaining**: `chain`, `tap`, `thru`, `VERSION`, and lodash's entire `Seq` category — the
  wrap-in-an-object-and-call-methods API, not a plumbing gap.

## Missing lodash conveniences on existing ports

A 2026-09 pass through every non-test coreUtils file (excluding `sortUtils.fs`) checked each
function that has a real lodash counterpart against `lodash.js` for a *convenience* gap — not a
missing function (those are the struck-through/"Possible, not yet built" rows in README.md's
Menu), but a flexibility lodash's own version offers that this port's implementation doesn't.
Nothing below has been implemented; this is a punch list for a future pass, roughly in order of
how many call sites it would touch.

### `prim/stringUtils.fs`

* `words(str)` has no second parameter — lodash's `words(string, [pattern])` accepts a custom
  RegExp/string overriding the default word-splitting rule; this port always uses the fixed
  `[^a-zA-Z0-9]+` delimiter with no override hook. (Distinct from the already-documented
  "no camelCase/digit-run boundary detection" caveat — that's about the *default* pattern's
  behavior, this is about there being no way to supply a different one.)
* `strRepeat(str, reps)` requires `reps` on every call; lodash's `repeat(string, [n=1])` defaults
  it to `1`.
* General pattern across most of this file: lodash's string functions coerce a non-string /
  `undefined` argument via an implicit `toString()` (so `_.trim(undefined)` is `''`, not a throw).
  This port's `is string`-typed functions (`trim`, `capitalize`, `words`, …) require an actual
  string and will misbehave/throw on `undefined` instead. Reads like a deliberate call consistent
  with the project's "no null" stance rather than an oversight — flagged since it's still a real
  behavioral divergence from lodash's contract, worth a conscious decision either way.

### `clxn/clxnUtils.fs` (beyond the iteratee-coercion item above)


* `chunk(arr, chunkSize)` requires `chunkSize`; lodash's `chunk(array, [size=1])` defaults it.
* `arrayIncludes(collection, value)` has no `fromIndex` parameter; lodash's
  `includes(collection, value, [fromIndex=0])` does.
* `find`/`findLast` have no `fromIndex` parameter either — the README already tracks this gap for
  `findIndex`/`findLastIndex`/`lastIndexOf`, but not for `find`/`findLast` themselves.

### `clxn/clxnGetset.fs`

* `getAt`/`setAt` don't degrade gracefully on a nullish root — lodash's `_.get`/`_.set` no-op
  (`object == null ? undefined/object : …`) rather than requiring `bag is map`/`is array` at the
  type level.
* ~~`setAt`'s intermediate path segments always autovivify as a **map**, even when the next
  segment looks like an array index~~ — resolved: `setAt` now creates `[]` there instead, matching
  lodash's `baseSet` heuristic, whenever the segment it's about to create is followed by a
  non-negative-integer-looking segment (bare number or digit string).
* ~~`updateWith`'s `onCollision` customizes the already-occupied leaf ... rather than customizing
  how *missing intermediate segments* get created~~ — resolved by addition rather than by
  repurposing `updateWith`: a new `setAtWith(bag, keyStrOrPath, val, segmentFor)` is the actual
  counterpart to lodash's `setWith` customizer, consulted only where a segment doesn't already
  resolve to a map or array. `updateWith`'s own `onCollision` keeps its existing, test-validated
  meaning (the leaf-combine resolver) — changing what it means would have broken
  `testClxnGetset.fs`'s `UpdateWithCases`, which documents that meaning as intentional.
* `pathForKey` has no `a[0].b` bracket-syntax parsing like lodash's `toPath` — already documented
  in its own docstring.
