FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
import(path : "66e287bede293cb227dfb89c", version : "7ad52f18be252f35a5ea9f6d"); // typeUtils

/**
 * Sentinel for "nothing at that step", so a fallback that is itself a map or an array can
 * never be mistaken for something found in the bag and descended into.
 */
export enum GetsetAtStep { MISSING }

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
 * contain a negative array index, which counts from the end.
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
 * If a portion of the path doesn't exist, it's created — unlike lodash's `set`, always as a map,
 * even for an integer segment; there's no way to grow an array mid-path the way assigning past
 * the end of a whole array already does.
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
 *   setAt({}, "a.0.b", 1);   // => { "a": { "0": { "b": 1 } } }
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
 * Read-modify-write: sets `bag`/`arr` at `keyStr`/`keyPath` to `updater(currentVal)`, via
 * `getAt`/`setAt`. `updateWith` additionally takes `setAt`'s `onCollision` — really a customizer
 * for how a missing intermediate segment gets created, same as lodash's `updateWith` — since the
 * updated value only ever collides with the (about to be replaced) value it was read from.
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
 * Recursively merges `incoming` into `existing`: two maps combine key by key all the way down;
 * an `undefined` source leaves the existing value alone; anything else, `incoming` wins.
 *
 * Unlike lodash's `merge`, two arrays replace rather than merge index by index — an array here
 * is a value, and half-overwriting one is worse than replacing it outright.
 *
 * @param existing: Base value.
 * @param incoming: Value to merge in; wins any conflict that isn't two maps.
 * @example
 *   deepMerge({ "a": { "x": 1 } }, { "a": { "y": 2 } }); // => { "a": { "x": 1, "y": 2 } }
 *   deepMerge({ "a": [1, 2] }, { "a": [3] });            // => { "a": [3] }
 *   deepMerge({ "a": 1 }, undefined);                    // => { "a": 1 }
 */
export function deepMerge(existing, incoming) {
  if (! isPresent(incoming)) { return existing; }
  if (! isPresent(existing)) { return incoming; }
  if (! ((existing is map) && (incoming is map))) { return incoming; }
  var out = existing;
  for (var keyStr in keys(incoming)) {
    out[keyStr] = deepMerge(existing[keyStr], incoming[keyStr]);
  }
  return out;
}

/**
 * `mergeMaps` *(std)*, with `combine(existingVal, incomingVal, key)` deciding what lands at a key
 * present in `incoming`, instead of `incoming` unconditionally winning — returning `undefined`
 * from `combine` falls back to that default.
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
 * `deepMerge`, with `combine(existingVal, incomingVal, key)` deciding what lands at a key present
 * in both — called at every level of the recursion, not just the leaves, so returning `undefined`
 * falls back to `deepMerge`'s own rule for that pair.
 * @example
 *   mergeWith({ "a": 1 }, { "a": 2 }, (existingVal, incomingVal) => existingVal + incomingVal);
 *   // => { "a": 3 }
 */
export function mergeWith(existing, incoming, combine is function) {
  if (! isPresent(incoming)) { return existing; }
  if (! isPresent(existing)) { return incoming; }
  const combined = combine(existing, incoming);
  if (isPresent(combined)) { return combined; }
  if (! ((existing is map) && (incoming is map))) { return incoming; }
  var out = existing;
  for (var keyStr in keys(incoming)) {
    out[keyStr] = mergeWith(existing[keyStr], incoming[keyStr], combine);
  }
  return out;
}

/**
 * Converts a dotted string into a path array — @see `getAt`/`setAt`'s second argument. Unlike
 * lodash's `toPath`, there's no `a[0].b` bracket syntax; an array index is just another
 * dot-separated segment (`"a.0.b"`).
 *
 * An empty segment names an empty key, which a map is perfectly willing to hold, so `"a..b"` is
 * three steps and `".foo"` is two. A key with a terminal dot is unhandled: `splitByRegexp`
 * discards exactly one trailing empty segment, so `"foo."` reads as `"foo"` rather than as
 * `"foo"` plus a trailing empty key.
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
    if (current == GetsetAtStep.MISSING) { return fallback; }
  }
  return current;
}

function setAtPath(container, keyPath is array, atIndex is number, val, onCollision is function) {
  if (size(keyPath) == 0) { return container; }
  const segment = keyPath[atIndex];
  if (atIndex == (size(keyPath) - 1)) { return placedAt(container, segment, val, onCollision); }
  const existing = steppedInto(container, segment);
  const child = ((existing is map) || (existing is array)) ? existing : {};
  // lastInWins on the way back out: the child already carries whatever was under it, so running
  // the resolver here would merge that subtree with itself
  return placedAt(container, segment, setAtPath(child, keyPath, atIndex + 1, val, onCollision), lastInWins);
}

function placedAt(container, segment, val, onCollision is function) {
  const existing = steppedInto(container, segment);
  const placed = (existing == GetsetAtStep.MISSING) ? val : onCollision(existing, val);
  if (container is array) { return placedInArray(container, segment, placed); }
  var out = (container is map) ? container : {};
  out[segment] = placed;
  return out;
}

/**
 * One step down. MISSING covers both a step that is not there and a step off the end of a
 * scalar; an array element that is there and undefined comes back as undefined.
 * (maps cannot hold undefined values, so there is no possible distinction)
 */
function steppedInto(container, segment) {
  if (container is map) {
    const val = container[segment];
    return isPresent(val) ? val : GetsetAtStep.MISSING;
  } else if (container is array) {
    const realSeq = seqForSegment(container, segment);
    return (realSeq == GetsetAtStep.MISSING) ? GetsetAtStep.MISSING : container[realSeq];
  }
  return GetsetAtStep.MISSING;
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

/** An index for reading: in range, or MISSING. Never complains, since a miss is an answer. */
function seqForSegment(arr is array, seq is number)  {
  const realSeq = (seq < 0) ? (size(arr) + seq) : seq;
  if (realSeq >= size(arr)) { return GetsetAtStep.MISSING; }
  if (realSeq < 0)          { return GetsetAtStep.MISSING; }
  return realSeq;
}

/** An index for reading: in range, or MISSING. Never complains, since a miss is an answer. */
function seqForSegment(arr is array, seq is string) {
  if (! match(seq, "^-?\\d+$").hasMatch) { throw nonkeyIndexMessage(seq, arr); }
  return seqForSegment(arr, stringToNumber(seq));
}

/**
 * An index for writing: past the end is fine, since setAt grows the array, but a segment that
 * is not an index at all has nowhere to go in an array and one before the start has no meaning.
 */
function seqForPlacement(arr is array, seq is number) {
  const realSeq = (seq < 0) ? (size(arr) + seq) : seq;
  if (realSeq >= size(arr)) { return realSeq; }                     // past the end, we handle by padding
  if (realSeq < 0)          { throw beforeStartMessage(seq, arr); } // negative index pointing before the start, no way to interpret that as meaningful
  return realSeq;
}

/**
 * An index for writing: past the end is fine, since setAt grows the array, but a segment that
 * is not an index at all has nowhere to go in an array and one before the start has no meaning.
 */
function seqForPlacement(arr is array, seq is string) {
  if (! match(seq, "^-?\\d+$").hasMatch) { throw nonkeyIndexMessage(seq, arr); }
  return seqForSegment(arr, stringToNumber(seq));
}

function beforeStartMessage(seq, arr is array) returns string {
  return 'Index ' ~ seq ~ ' is before the start of a ' ~ size(arr) ~ '-element array';
}

function nonkeyIndexMessage(seq, subj) returns string {
  return 'Index ' ~ seq ~ ' is not a valid key for ' ~ subj;
}
