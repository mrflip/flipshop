
## Lodash no-go list

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

## Resolved, no longer excluded

An earlier pass filed `assign`, `assignIn`, `extend`, `defaults`, `defaultsDeep`, `fill`, `pull`,
`pullAll`, `pullAllBy`, `pullAllWith`, `remove`, `unset`, `update`, `updateWith`, `setWith` under
a "modify the subject in-place" exclusion. That turned out not to be a real blocker.

Lodash mutates its first argument and returns it; this project already returns a new value from
everything instead (`setAt` vs. lodash's mutating `set` is the pattern the rest of `clxnGetset.fs`
follows), and FeatureScript's copy-on-write map/array semantics make that substitution free — there's
no shared mutable reference to lose by not mutating in place. Once you stop requiring the *same*
object identity back out, every function in that bucket turns out to be either already covered by
an existing pure function (`mergeMaps`/`deepMerge` cover `assign`/`defaults` once you flip an
argument order) or a short, unbuilt, pure one (`update`, `fill`, …). See the Object and Array
sections of README.md for the specifics, function by function.

---

## Missing lodash conveniences on existing ports

A 2026-09 pass through every non-test coreUtils file (excluding `sortUtils.fs`) checked each
function that has a real lodash counterpart against `lodash.js` for a *convenience* gap — not a
missing function (those are the struck-through/"Possible, not yet built" rows in README.md's
Menu), but a flexibility lodash's own version offers that this port's implementation doesn't.
Nothing below has been implemented; this is a punch list for a future pass, roughly in order of
how many call sites it would touch.

### Iteratee-shorthand coercion — the systemic one

`prim/helperFuncs.fs` already built the coercion this needs: `iteratee(spec)` turns a function
through unchanged, a map into a `matches` partial-match rule, and a string into a `property`
accessor — lodash calls this same mechanism internally (`getIteratee`) on every predicate/iteratee
argument it accepts. Almost none of the predicate-taking functions in this codebase actually call
`iteratee()` on what they're handed; they all require a literal function value instead. Two gaps
inside `iteratee()` itself, plus every affected caller:

* ~~`iteratee(spec)` (`prim/helperFuncs.fs`) doesn't handle an **array** shorthand~~ — resolved:
  a `[path, srcValue]` array now builds a `matchesProperty` rule, matching lodash's
  `_.iteratee(['user', 'fred'])`.
* ~~`cond(pairs)` (`prim/helperFuncs.fs`)~~ — resolved: each pair's predicate is now coerced
  through `iteratee`, for both the array-of-pairs and map-keyed-by-rule overloads.
* ~~`over`/`overEvery`/`overSome` (`prim/helperFuncs.fs`)~~ — resolved: every element of
  `funcs`/`predicates` is now coerced through `iteratee`, once per call rather than per
  `(val, seq)` invocation.
* ~~`clxn/clxnUtils.fs` — every one of these called `func`/`rule`/`iteratee` directly rather than
  through `iteratee()`~~ — resolved for all of them: `find`, `findLast`, `findKey`, `findLastKey`,
  `countBy`, `groupBy`, `partition`, `reject`, `flatMap`/`flatMapDeep`/`flatMapDepth`, `unionBy`,
  `intersectionBy`, `differenceBy`, `xorBy`, `uniqBy`, `maxBy`, `minBy`, `meanBy`, `sumBy`,
  `mapKeys`, `invertBy`, `mapValues`/`mapValues3`, `omitBy`. Params literally named `iteratee`
  (which shadowed the `iteratee()` function itself) were renamed to `iterateeSpec`; `meanBy`/
  `sumBy` were rerouted from std's single-arg `mapArray` to this file's own `mapValues`, which
  picks up both the coercion and the `(val, seq)` shape `iteratee()`'s output functions expect;
  `maxBy`/`minBy` switched from a `for (var val in arr)` walk to an index-based one so they could
  supply `seq`. `pick`/`omit`'s deep-path support (below) and `pickBy` (new, alongside `omitBy`)
  were built in the same pass.

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

* ~~`take`/`takeRight`/`takeWhile`/`takeRightWhile` have no default-argument overload~~ —
  resolved: all four now default the same way their `drop*` siblings already did (count to `1`,
  rule to `truthy`). Fixing this also surfaced a real arity bug in `dropWhile`/`dropRightWhile`'s
  own 1-arg overloads — they called their default `truthy` with `(val, seq)`, but `truthy` is a
  1-arg function; fixed by defaulting to `curry2to1(truthy)` instead, same fix applied to the new
  `takeWhile`/`takeRightWhile` 1-arg overloads.
* `chunk(arr, chunkSize)` requires `chunkSize`; lodash's `chunk(array, [size=1])` defaults it.
* ~~`pick`/`omit` only accept a literal top-level key list~~ — resolved: each `keylist` entry now
  resolves through `getAt`/`setAt`, so a dotted string or key-path array reaches into nested
  structure the way lodash's `_.pick(obj, ['a.b.c'])` does — `pick` rebuilds the same nesting in
  its result, `omit` skips a path with nothing currently at it rather than autovivifying empty
  maps along the way.
* `arrayIncludes(collection, value)` has no `fromIndex` parameter; lodash's
  `includes(collection, value, [fromIndex=0])` does.
* `find`/`findLast` have no `fromIndex` parameter either — the README already tracks this gap for
  `findIndex`/`findLastIndex`/`lastIndexOf`, but not for `find`/`findLast` themselves.
* ~~`pickDefined` hardcodes "keep defined values" with no way to supply a different rule~~ —
  resolved: `pickBy(bag, rule)` now exists alongside `omitBy`.

### `clxn/clxnGetset.fs`

* `getAt`/`setAt` don't degrade gracefully on a nullish root — lodash's `_.get`/`_.set` no-op
  (`object == null ? undefined/object : …`) rather than requiring `bag is map`/`is array` at the
  type level.
* `setAt`'s intermediate path segments always autovivify as a **map**, even when the next segment
  looks like an array index; lodash's `baseSet` creates `[]` there instead. Already noted in
  `setAt`'s own docstring as a caveat — restated here since it's a real missing convenience, not
  just documentation.
* `updateWith`'s `onCollision` customizes the already-occupied leaf (trivially true, since the
  leaf was just read) rather than customizing how *missing intermediate segments* get created —
  which is what lodash's real `updateWith`/`setWith` customizer does, and would be the actual hook
  to work around the map-vs-array autovivification gap above.
* `pathForKey` has no `a[0].b` bracket-syntax parsing like lodash's `toPath` — already documented
  in its own docstring.
* `deepMerge`/`mergeWith` replace arrays wholesale rather than merging index-by-index like
  lodash's `merge` — a deliberate, already-documented design choice, not an oversight; noted here
  only for completeness.
