
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
