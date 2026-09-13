# Human FIXME

Open questions and inconsistencies found while giving `onshape/flipshop/coreUtils/` (and
whatever else gets this treatment later) a docs/tests/style pass — things that need a human
decision rather than a mechanical fix. Nothing here is broken or blocking; picked up whenever
convenient. Completed work doesn't belong here — see each directory's own README for what's
already fixed.

## coreUtils/jsonVarF.fs

- [ ] `sizeofF` has no `description` field, unlike `keylistF`/`valuesAtF` — its precondition
      never declares `definition.description`, so the `ifBlank(definition.description, ...)` in
      its body always takes the fallback and the description can never be hand-edited. Add the
      field (mirroring `keylistF`'s precondition + UI annotation) if that's wanted here too, or
      leave it as an intentionally simpler feature.
- [ ] `keylistF`/`sizeofF` never update their `description` once set, where `valuesAtF` does (via
      `defaultMaybe` in its edit logic). The first two use a plain `ifBlank` at the body level,
      which fills in a description once and never revisits it even if the source variable is
      renamed. Worth deciding whether all three should track renames the same way.
- [ ] `keylistEditLogic`/`sizeofEditLogic`/`valuesAtEditLogic` are pure (never touch `context`)
      and would be cheap to unit-test the same way `defaultMaybe` is tested in
      `testMetadataUtils.fs`. Left out of scope for now (per explicit direction) — worth doing
      once `jsonVarF.fs`'s own compiled-document reference is known, so a new test file can be
      wired up with confidence instead of guessing an import path.

## coreUtils/colorUtils.fs

- [ ] `HexCharToInt` (exported) appears to be dead code — nothing in this file, or anywhere else
      in the mirrored repo, calls it. `HexpairToInt` (private, two-character keys) is what
      `hexpairToInt`/`intToHexpair` actually use. Kept rather than deleted since it's exported
      and something outside this repo (not mirrored locally) could still depend on it — delete if
      you can confirm nothing does.

## coreUtils/stringUtils.fs

- [ ] `rangedSequencePosition(pos, beg, end)`'s `beg` parameter is never read in the body — only
      `end` is used, for the one defined case (`SequencePosition.END`). Possibly a leftover from
      a `SequencePosition.START` that never got added. Either add that case or drop the unused
      parameter.
- [ ] `padRight` has no numeric overload, where `padLeft` does (`padLeft(num is number, ...)`
      stringifies and delegates). Intentional minimalism, or just never ported over? Worth adding
      for symmetry if `padRight` is ever called with a number the way `padLeft` already is.
