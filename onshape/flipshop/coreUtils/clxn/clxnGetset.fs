FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
//
import(path : "66e287bede293cb227dfb89c", version : "7ad52f18be252f35a5ea9f6d"); // typeUtils

// lodash flattens to Infinity by default; FeatureScript has no Infinity literal, so this stands in
export const UnboundedDotDepth = 1000000000;

/** The default collision rule: whatever arrives replaces whatever is there, as lodash's set does. */
export const lastInWins = ((existing, incoming) => incoming);

/**
 * Gets the value at `keyStr`/`keyPath` of `bag`/`arr`. If the resolved value is `undefined`,
 * `fallback` is returned in its place — except an array element that is present but genuinely
 * `undefined`, which comes back as itself; a map can't hold that case, since FeatureScript elides
 * an `undefined` value on the way in.
 *
 * A string path is dotted (`"a.b.c"` is three steps); an array path is a list of literal keys, so
 * `["a.b"]` is one step, reaching a key with a dot in its own name. Either kind of path may
 * contain a negative array index, which counts from the end — lodash's `get` has no such support.
 *
 * @param bag {map|array}: Container to read from.
 * @param keyStr {string}: Dotted path.
 * @param keyPath {array}: Path as literal keys/indexes.
 * @param fallback: Returned in place of an `undefined` result. Defaults to `undefined`.
 * @example
 *   getAt({ "a": { "b": 1 } }, "a.b");                            // => 1
 *   getAt({ "a": { "b": 1 } }, ["a", "b"]);                       // => 1
 *   getAt({ "a": { "b": 1 } }, "a.c", { "not": "met" });          // => { "not": "met" }
 *   getAt({ "rows": [{ "cells": [7, 8] }] }, "rows.-1.cells.-1"); // => 8
 *   getAt([1, 2, 3], -1);                                         // => 3
 */
export function getAt(bag is map, keyStr is string, fallback) {
  return valAtPath(bag, pathForKey(keyStr), fallback);
}

export function getAt(bag is map, keyPath is array, fallback) {
  return valAtPath(bag, keyPath, fallback);
}

export function getAt(arr is array, seq is number, fallback) {
  return valAtPath(arr, [seq], fallback);
}

export function getAt(arr is array, seqStr is string, fallback) {
  return valAtPath(arr, pathForKey(seqStr), fallback);
}

export function getAt(arr is array, keyPath is array, fallback) {
  return valAtPath(arr, keyPath, fallback);
}

export function getAt(bag is map, keyStr is string) {
  return valAtPath(bag, pathForKey(keyStr), undefined);
}

export function getAt(bag is map, keyPath is array) {
  return valAtPath(bag, keyPath, undefined);
}

export function getAt(arr is array, seq is number) {
  return valAtPath(arr, [seq], undefined);
}

export function getAt(arr is array, seqStr is string) {
  return valAtPath(arr, pathForKey(seqStr), undefined);
}

export function getAt(arr is array, keyPath is array) {
  return valAtPath(arr, keyPath, undefined);
}

/**
 * Sets the value at `keyStr`/`keyPath` of `bag`/`arr`, returning the (possibly new) container.
 * If a portion of the path doesn't exist, it's created as a map, unless the *next* segment looks
 * like a non-negative integer (a bare number, or a string of digits), in which case it's created
 * as an array instead — matching lodash's own `set`/`baseSet` heuristic. @see `setAtWith` to
 * override that heuristic outright, the way lodash's `setWith` customizer does.
 *
 * A scalar in the way of a deeper path is replaced outright, same as lodash. Where the path's own
 * leaf is already occupied, `onCollision(existing, incoming)` decides what lands there instead of
 * just overwriting it — default is `lastInWins`; @see `deepMerge` for the deep-merge rule.
 *
 * Two more consequences of FeatureScript having no `null`: setting a value of `undefined` deletes
 * a map key rather than storing one, and writing past the end of an array pads it with
 * `undefined` rather than throwing.
 *
 * @param bag {map|array}: Container to write into.
 * @param keyStr {string}: Dotted path.
 * @param keyPath {array}: Path as literal keys/indexes.
 * @param val: Value to place at the path's leaf.
 * @param onCollision {function}: `(existing, incoming) => merged`, called only when the leaf is already occupied. Defaults to `lastInWins`.
 * @example
 *   setAt({}, "a.b.c", 1);   // => { "a": { "b": { "c": 1 } } }
 *   setAt({}, "a.0.b", 1);   // => { "a": [{ "b": 1 }] }
 *   setAt([1, 2], 4, 9);     // => [1, 2, undefined, undefined, 9]
 *   setAt({ "a": { "x": 1 } }, "a", { "y": 2 }, ((existing, incoming) => deepMerge(existing, incoming)));
 *                            // => { "a": { "x": 1, "y": 2 } }
 */
export function setAt(bag is map, keyStr is string, val) returns map {
  return setAtPath(bag, pathForKey(keyStr), 0, val, lastInWins);
}

export function setAt(bag is map, keyStr is string, val, onCollision is function) returns map {
  return setAtPath(bag, pathForKey(keyStr), 0, val, onCollision);
}

export function setAt(bag is map, keyPath is array, val) returns map {
  return setAtPath(bag, keyPath, 0, val, lastInWins);
}

export function setAt(bag is map, keyPath is array, val, onCollision is function) returns map {
  return setAtPath(bag, keyPath, 0, val, onCollision);
}

export function setAt(arr is array, seq is number, val) returns array {
  return setAtPath(arr, [seq], 0, val, lastInWins);
}

export function setAt(arr is array, seq is number, val, onCollision is function) returns array {
  return setAtPath(arr, [seq], 0, val, onCollision);
}

export function setAt(arr is array, seqStr is string, val) returns array {
  return setAtPath(arr, pathForKey(seqStr), 0, val, lastInWins);
}

export function setAt(arr is array, seqStr is string, val, onCollision is function) returns array {
  return setAtPath(arr, pathForKey(seqStr), 0, val, onCollision);
}

export function setAt(arr is array, keyPath is array, val) returns array {
  return setAtPath(arr, keyPath, 0, val, lastInWins);
}

export function setAt(arr is array, keyPath is array, val, onCollision is function) returns array {
  return setAtPath(arr, keyPath, 0, val, onCollision);
}

/**
 * Read-modify-write: `keyStr`/`keyPath` of `bag`/`arr` becomes `updater(currentVal)`.
 * `updateWith` additionally takes an `onCollision`, called only when the leaf `keyStrOrPath`
 * already resolves to something — it customizes how the freshly-`updater`'d value combines with
 * what was just read, not how a missing intermediate segment gets created (@see `setAtWith` for
 * that — the actual counterpart to lodash's `setWith`/`updateWith` customizer). Since the value
 * being placed only ever collides with the value it was itself derived from, most of the time
 * `onCollision` can just take the incoming side and ignore `existing`. @see `getAt`/`setAt`.
 * @example
 *   update({ "a": 1 }, "a", (val) => val + 1); // => { "a": 2 }
 */
export function update(bag, keyStrOrPath, updater is function) {
  return setAt(bag, keyStrOrPath, updater(getAt(bag, keyStrOrPath)));
}
export function updateWith(bag, keyStrOrPath, updater is function, onCollision is function) {
  return setAt(bag, keyStrOrPath, updater(getAt(bag, keyStrOrPath)), onCollision);
}

/**
 * `setAt`, with `segmentFor(existingChildOrUndefined, nextSegment) => newChildContainer`
 * overriding what an autovivified intermediate segment becomes, in place of `setAt`'s own
 * map-unless-the-next-segment-looks-like-an-index heuristic — the actual hook lodash's `setWith`
 * customizer provides. `segmentFor` is consulted only where a segment doesn't already resolve to
 * a map or array; returning `undefined` falls back to `setAt`'s own default for that segment.
 * @param bag {map}: Container to write into.
 * @param keyStrOrPath {string|array}: Dotted path, or path as literal keys/indexes.
 * @param val: Value to place at the path's leaf.
 * @param segmentFor {function}: `(existingChildOrUndefined, nextSegment) => newChildContainer`.
 * @example
 *   setAtWith({}, "a.0.b", 1, (_existing, _nextSegment) => ({})); // => { "a": { "0": { "b": 1 } } }
 */
export function setAtWith(bag is map, keyStrOrPath, val, segmentFor is function) returns map {
  return setAtPath(bag, pathArrayFor(keyStrOrPath), 0, val, lastInWins, segmentFor);
}

function pathArrayFor(keyStrOrPath) returns array {
  return (keyStrOrPath is string) ? pathForKey(keyStrOrPath) : keyStrOrPath;
}

/**
 * Recursively merges `incoming` into `existing`: two maps combine key by key all the way down;
 * two arrays combine index by index, `existing`'s tail past `size(incoming)` surviving untouched,
 * matching lodash's `merge`; an `undefined` source leaves the existing value alone; anything
 * else, `incoming` wins.
 *
 * @param existing: Base value.
 * @param incoming: Value to merge in; wins any conflict that isn't two maps or two arrays.
 * @example
 *   deepMerge({ "a": { "x": 1 } }, { "a": { "y": 2 } }); // => { "a": { "x": 1, "y": 2 } }
 *   deepMerge({ "a": [1, 2] }, { "a": [3] });            // => { "a": [3, 2] }
 *   deepMerge({ "a": 1 }, undefined);                    // => { "a": 1 }
 */
export function deepMerge(existing, incoming) {
  if (! isPresent(incoming)) { return existing; }
  if (! isPresent(existing)) { return incoming; }
  if ((existing is array) && (incoming is array)) { return deepMergeArrays(existing, incoming); }
  if (! ((existing is map) && (incoming is map))) { return incoming; }
  var out = existing;
  for (var keyStr in keys(incoming)) {
    out[keyStr] = deepMerge(existing[keyStr], incoming[keyStr]);
  }
  return out;
}

/** `deepMerge`'s array-array case: `incoming[seq]` merges onto `existing[seq]` for every `seq` in `incoming`; `existing`'s tail past that survives untouched. */
function deepMergeArrays(existing is array, incoming is array) returns array {
  var out = existing;
  for (var seq = 0; seq < size(incoming); seq += 1) {
    const existingVal = (seq < size(existing)) ? existing[seq] : undefined;
    const mergedVal = deepMerge(existingVal, incoming[seq]);
    if (seq < size(out)) { out[seq] = mergedVal; } else { out = append(out, mergedVal); }
  }
  return out;
}

/**
 * `mergeMaps` *(std)*, with `combine(existingVal, incomingVal, key)` deciding what lands at a key
 * present in `incoming`, instead of `incoming` unconditionally winning — returning `undefined`
 * from `combine` falls back to that default. Unlike lodash's `assignWith` customizer, `combine`
 * doesn't also receive the whole source/destination objects.
 * @example
 *   assignWith({ "a": 1 }, { "a": 2 }, (existingVal, incomingVal) => existingVal + incomingVal);
 *   // => { "a": 3 }
 */
export function assignWith(existing is map, incoming is map, combine is function) returns map {
  var out = existing;
  for (var key in keys(incoming)) {
    const combined = combine(existing[key], incoming[key], key);
    out[key] = isPresent(combined) ? combined : incoming[key];
  }
  return out;
}

/**
 * `deepMerge`, with `combine(existingVal, incomingVal)` deciding what lands at a pair present in
 * both — called at every level of the recursion, not just the leaves (including once per
 * index-pair when both sides are arrays), so returning `undefined` falls back to `deepMerge`'s
 * own rule for that pair. Unlike lodash's `mergeWith` customizer, `combine` gets neither a `key`
 * nor the whole source/destination objects.
 * @example
 *   mergeWith({ "a": 1 }, { "a": 2 }, (existingVal, incomingVal) => existingVal + incomingVal);
 *   // => { "a": 3 }
 */
export function mergeWith(existing, incoming, combine is function) {
  if (! isPresent(incoming)) { return existing; }
  if (! isPresent(existing)) { return incoming; }
  const combined = combine(existing, incoming);
  if (isPresent(combined)) { return combined; }
  if ((existing is array) && (incoming is array)) { return mergeWithArrays(existing, incoming, combine); }
  if (! ((existing is map) && (incoming is map))) { return incoming; }
  var out = existing;
  for (var keyStr in keys(incoming)) {
    out[keyStr] = mergeWith(existing[keyStr], incoming[keyStr], combine);
  }
  return out;
}

/** `mergeWith`'s array-array case, same shape as `deepMergeArrays` but threading `combine` through each pair. */
function mergeWithArrays(existing is array, incoming is array, combine is function) returns array {
  var out = existing;
  for (var seq = 0; seq < size(incoming); seq += 1) {
    const existingVal = (seq < size(existing)) ? existing[seq] : undefined;
    const mergedVal = mergeWith(existingVal, incoming[seq], combine);
    if (seq < size(out)) { out[seq] = mergedVal; } else { out = append(out, mergedVal); }
  }
  return out;
}

/**
 * Converts a dotted string into a path array — @see `getAt`/`setAt`'s second argument. Unlike
 * lodash's `toPath`, there's no `a[0].b` bracket syntax; an array index is just another
 * dot-separated segment (`"a.0.b"`).
 *
 * An empty segment names an empty key, which a map is perfectly willing to hold, so `"a..b"` is
 * three steps and `".foo"` is two. A key with a terminal dot is unhandled: `"foo."` reads as
 * `"foo"`, dropping the trailing empty segment rather than keeping it as a trailing empty key.
 *
 * @param keyStr {string}: Dotted path.
 * @example
 *   pathForKey("a.b.c"); // => ["a", "b", "c"]
 *   pathForKey("a..b");  // => ["a", "", "b"]
 *   pathForKey("foo.");  // => ["foo"]
 */
export function pathForKey(keyStr is string) returns array {
  if (keyStr == "") { return [""]; }
  return splitByRegexp(keyStr, "\\.");
}

function valAtPath(container, keyPath is array, fallback) {
  var current = container;
  for (var segment in keyPath) {
    current = steppedInto(current, segment);
    if (current == Sentinel.ABSENT) { return fallback; }
  }
  return current;
}

function setAtPath(container, keyPath is array, atIndex is number, val, onCollision is function) {
  return setAtPath(container, keyPath, atIndex, val, onCollision, noSegmentCustomizer);
}

function setAtPath(container, keyPath is array, atIndex is number, val, onCollision is function, segmentFor is function) {
  if (size(keyPath) == 0) { return container; }
  const segment = keyPath[atIndex];
  if (atIndex == (size(keyPath) - 1)) { return placedAt(container, segment, val, onCollision); }
  const existing = steppedInto(container, segment);
  const nextSegment = keyPath[atIndex + 1];
  const customChild = segmentFor((existing == Sentinel.ABSENT) ? undefined : existing, nextSegment);
  const child = ((existing is map) || (existing is array)) ? existing
    : (isPresent(customChild) ? customChild : emptyContainerFor(nextSegment));
  // lastInWins on the way back out: the child already carries whatever was under it, so running
  // the resolver here would merge that subtree with itself
  return placedAt(container, segment, setAtPath(child, keyPath, atIndex + 1, val, onCollision, segmentFor), lastInWins);
}

const noSegmentCustomizer = ((_existingChildOrUndefined, _nextSegment) => undefined);

/** `[]` if `segment` looks like a non-negative integer (bare number or digit string) — lodash's own `set`/`isIndex` heuristic — `{}` otherwise. */
function emptyContainerFor(segment) {
  return looksLikeIndex(segment) ? [] : {};
}

function looksLikeIndex(segment) returns boolean {
  if (segment is number) { return (segment >= 0) && isInteger(segment); }
  if (segment is string) { return match(segment, "^\\d+$").hasMatch; }
  return false;
}

function placedAt(container, segment, val, onCollision is function) {
  const existing = steppedInto(container, segment);
  const placed = (existing == Sentinel.ABSENT) ? val : onCollision(existing, val);
  if (container is array) { return placedInArray(container, segment, placed); }
  var out = (container is map) ? container : {};
  out[segment] = placed;
  return out;
}

/**
 * One step down. ABSENT covers both a step that is not there and a step off the end of a
 * scalar; an array element that is there and undefined comes back as undefined.
 * (maps cannot hold undefined values, so there is no possible distinction)
 */
function steppedInto(container, segment) {
  if (container is map) {
    const val = container[segment];
    return isPresent(val) ? val : Sentinel.ABSENT;
  } else if (container is array) {
    const realSeq = seqForSegment(container, segment);
    return (realSeq == Sentinel.ABSENT) ? Sentinel.ABSENT : container[realSeq];
  }
  return Sentinel.ABSENT;
}

function placedInArray(arr is array, segment, val) returns array {
  const seq = seqForPlacement(arr, segment);
  var out = arr;
  for (var ii = size(out); ii < seq; ii += 1) {
    out = append(out, undefined);
  }
  if (seq < size(out)) {
    out[seq] = val;
  } else {
    out = append(out, val);
  }
  return out;
}

/** An index for reading: in range, or ABSENT. Never complains, since a miss is an answer. */
function seqForSegment(arr is array, seq is number)  {
  const realSeq = (seq < 0) ? (size(arr) + seq) : seq;
  if (realSeq >= size(arr)) { return Sentinel.ABSENT; }
  if (realSeq < 0)          { return Sentinel.ABSENT; }
  return realSeq;
}

/** An index for reading: in range, or ABSENT. Never complains, since a miss is an answer. */
function seqForSegment(arr is array, seq is string) {
  if (! match(seq, "^-?\\d+$").hasMatch) { throw nonkeyIndexMessage(seq, arr); }
  return seqForSegment(arr, stringToNumber(seq));
}

/**
 * An index for writing: past the end is fine, since setAt grows the array, but a segment that
 * is not an index at all has nowhere to go in an array and one before the start has no meaning.
 */
function seqForPlacement(arr is array, seq is number) returns number {
  const realSeq = (seq < 0) ? (size(arr) + seq) : seq;
  if (realSeq >= size(arr)) { return realSeq; }                     // past the end, we handle by padding
  if (realSeq < 0)          { throw beforeStartMessage(seq, arr); } // negative index pointing before the start, no way to interpret that as meaningful
  return realSeq;
}

/** String form of the above: parses `seq` first, then applies the same rule. */
function seqForPlacement(arr is array, seq is string) {
  if (! match(seq, "^-?\\d+$").hasMatch) { throw nonkeyIndexMessage(seq, arr); }
  return seqForPlacement(arr, stringToNumber(seq));
}

function beforeStartMessage(seq, arr is array) returns string {
  return 'Index ' ~ seq ~ ' is before the start of a ' ~ size(arr) ~ '-element array';
}

function nonkeyIndexMessage(seq, subj) returns string {
  return 'Index ' ~ seq ~ ' is not a valid key for ' ~ subj;
}

// == [Function values] --

export const ClxnGetsetFuncs = {
  "lastInWins": lastInWins,
  "getAt":      (bag, path, fallback) => getAt(bag, path, fallback),
  "setAt":      (bag, path, val, onCollision) => setAt(bag, path, val, onCollision),
  "update":     (bag, keyStrOrPath, updater) => update(bag, keyStrOrPath, updater),
  "updateWith": (bag, keyStrOrPath, updater, onCollision) => updateWith(bag, keyStrOrPath, updater, onCollision),
  "setAtWith":  (bag, keyStrOrPath, val, segmentFor) => setAtWith(bag, keyStrOrPath, val, segmentFor),
  "deepMerge":  (existing, incoming) => deepMerge(existing, incoming),
  "assignWith": (existing, incoming, combine) => assignWith(existing, incoming, combine),
  "mergeWith":  (existing, incoming, combine) => mergeWith(existing, incoming, combine),
  "pathForKey": (keyStr) => pathForKey(keyStr),
};
