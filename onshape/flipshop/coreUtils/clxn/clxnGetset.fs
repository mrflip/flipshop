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

// == [Collection merging] -- deepMerge, assignWith, mergeWith

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
// --

// == [Collection Inspection] -- hasKey

/**
 * Whether `key` is present in `obj`. Unlike lodash's `has`, `key` is a single literal key or
 * index — never a dotted path — and a map only ever contains `key` when its value isn't
 * `undefined`, since FeatureScript elides one on the way in.
 *
 * For an array, `missingPolicy` decides whether an in-bounds slot holding `undefined` counts:
 * `MissingPolicy.USE_UNDEFINED` (the default) says yes; `MissingPolicy.SKIP` says no, the same as
 * `hasPresentKey`. Neither array overload accepts a negative index, unlike @see `getAt`.
 *
 * `hasPresentKey` is `hasKey` pinned to the stricter policy.
 *
 * @example
 *   hasKey({ "a": 1 }, "a");         // => true
 *   hasKey({ "a": undefined }, "a"); // => false
 *   hasKey([1, 2, 3], 2);            // => true
 *   hasKey([1, 2, 3], -1);           // => false
 *   hasKey([1, undefined, 3], 1, MissingPolicy.SKIP); // => false
 */
export function hasKey(obj is map, key is string) returns boolean {
    return (obj[key] != undefined);
}
export function hasKey(obj is map, key is string, missingPolicy is MissingPolicy) returns boolean {
    return (obj[key] != undefined); // offered for symmetry with the array case
}
export function hasPresentKey(obj is map, key is string) returns boolean {
  return hasKey(obj, key); // FS does not retain keys with undefined values
}

export function hasKey(obj is array, key is number, missingPolicy is MissingPolicy) returns boolean {
  if (missingPolicy == MissingPolicy.SKIP) { return hasPresentKey(obj, key); }
  return (key >= 0) && (key < size(obj));
}
export function hasKey(obj is array, key is number) returns boolean {
  return (key >= 0) && (key < size(obj));
}
export function hasPresentKey(obj is array, key is number) returns boolean {
  return (key >= 0) && (key < size(obj)) && (obj[key] != undefined);
}

/**
 * Coerces `spec` into a callable iteratee: a function passes through unchanged, a map becomes a
 * `matches` rule, a string becomes a `property` accessor, a `[path, srcValue]` array becomes a
 * `matchesProperty` rule, and anything else falls back to `identity`.
 * @example
 *   iteratee("a")({ "a": 1 });                    // => 1
 *   iteratee({ "a": 1 })({ "a": 1, "b": 2 });      // => true
 *   iteratee(["a", 1])({ "a": 1, "b": 2 });        // => true
 */
export const iteratee = (function(spec) returns function {
  if (spec is function) { return spec; }
  if (spec is map)      { return matches(spec); }
  if (spec is string)   { return property(spec); }
  if (spec is array)    { return matchesProperty(spec[0], spec[1]); }
  return (val, _seq) => val;
});

/**
 * Builds a rule that's `true` for any map holding `source`'s entries — a partial deep match.
 * `(obj, seq)` callback shape, discarding `seq` @see `conforms`.
 * @example
 *   matches({ "a": 1 })({ "a": 1, "b": 2 }, 0); // => true
 *   matches({ "a": 1 })({ "a": 2, "b": 2 }, 0); // => false
 */
export function matches(source is map) returns function {
  return (obj is map, _seq) => (pick(obj, keys(source)) == source);
}
/**
 * Builds a rule that's `true` when `path` of a given object equals `srcValue`; `path` can be a
 * string/dotpath/pathlist @see `getAt`. `(obj, seq)` callback shape, discarding `seq` @see `conforms`.
 * @example
 *   matchesProperty("a.b", 1)({ "a": { "b": 1 } }, 0); // => true
 */
export function matchesProperty(path, srcValue) returns function {
  return (obj, _seq) => (getAt(obj, path) == srcValue);
}

/**
 * Builds a function that reads `path` off whatever it's given; `path` can be a
 * string/dotpath/pathlist @see `getAt`. `(obj, seq)` callback shape, discarding `seq` @see `conforms`.
 * @example
 *   property("a.b")({ "a": { "b": 1 } }, 0); // => 1
 */
export const property = (function(path) returns function {
  return (obj, _seq) => getAt(obj, path);
});

/**
 * The reverse of `property`: fixes the object up front and builds a function that reads whatever
 * path it's given off of it.
 * @example
 *   propertyOf({ "a": { "b": 1 } })("a.b", 0); // => 1
 */
export const propertyOf = (function(obj) returns function {
  return (path, _seq) => getAt(obj, path);
});

// --

// == [Collection retrieve many] -- pick, pickDefined, arrLast, omit, omitBy, pickBy

/**
 * A map with just `bag`'s entries at `keylist` — like lodash's `pick`, an absent key is simply
 * missing from the result rather than present with an `undefined` value. Each entry of `keylist`
 * can be a dotted string or key-path array @see `getAt`, reaching into a nested structure and
 * rebuilding the same nesting in the result — `pick({ "a": { "b": 1 } }, ["a.b"])` is
 * `{ "a": { "b": 1 } }`, not a flat `{ "a.b": 1 }`. This is lossy against a key that already
 * contains a literal dot, same caveat as `dotMap`/`undotMap`.
 *
 * `pickDefined` additionally drops a key whose value is `undefined` — for a map this is the same
 * result as `pick`, since a map can never hold an `undefined` value to differ over. Unlike
 * `pickBy`, the rule isn't customizable and the keys considered are exactly `keylist`, not every
 * key of `bag`. `pickDefined` only accepts a literal top-level key, not a dotted path.
 *
 * @example
 *   pick({ "a": 1, "b": 2, "c": 3 }, ["a", "c"]); // => { "a": 1, "c": 3 }
 *   pick({ "a": { "b": 1, "c": 2 } }, ["a.b"]); // => { "a": { "b": 1 } }
 *   pickDefined({ "a": 1, "b": undefined, "c": 3 }, ["a", "b", "c"]); // => { "a": 1, "c": 3 }
 */
export function pick(bag is map, keylist is array) returns map {
    var result = {};
    for (var pathSpec in keylist) {
      const val = getAt(bag, pathSpec, Sentinel.ABSENT);
      if (val != Sentinel.ABSENT) { result = setAt(result, pathSpec, val); }
    }
    return result;
}

export function pickDefined(bag is map, keylist is array) returns map {
    var result = {};
    for (var k in keylist) {
      if (bag[k] != undefined) { result[k] = bag[k]; }
    }
    return result;
}

/**
 * Last element of `arr`, or `undefined` if it's empty.
 * @example
 *   arrLast([1, 2, 3]); // => 3
 *   arrLast([]);        // => undefined
 */
export const arrLast = (function(arr is array) {
    if (size(arr) <= 0) { return undefined; }
    return arr[size(arr) - 1];
});

/**
 * First element of `arr`, or `undefined` if it's empty.
 * @example
 *   arrFirst([1, 2, 3]); // => 1
 *   arrFirst([]);        // => undefined
 */
export const arrFirst = (function(arr is array) {
    if (size(arr) <= 0) { return undefined; }
    return arr[0];
});

/**
 * `bag` without the entries at `keylist` — the inverse of `pick`. Each entry of `keylist` can be
 * a dotted string or key-path array @see `getAt`, deleting a nested leaf without disturbing its
 * siblings; a path with nothing currently at it is skipped rather than autovivifying empty maps
 * along the way. `omitBy` instead drops any entry for which `rule(val, key)` holds, the inverse
 * of `pickDefined`'s spirit but with a caller-supplied rule rather than a fixed "is defined"
 * check. `rule` is coerced through `iteratee` @see `iteratee`.
 * @example
 *   omit({ "a": 1, "b": 2, "c": 3 }, ["b"]); // => { "a": 1, "c": 3 }
 *   omit({ "a": { "b": 1, "c": 2 } }, ["a.b"]); // => { "a": { "c": 2 } }
 */
export function omit(bag is map, keylist is array) returns map {
  var result = bag;
  for (var pathSpec in keylist) {
    if (getAt(result, pathSpec, Sentinel.ABSENT) != Sentinel.ABSENT) {
      result = setAt(result, pathSpec, undefined);
    }
  }
  return result;
}
export function omitBy(bag is map, rule) returns map {
  const fn = iteratee(rule);
  var result = {};
  for (var key in keys(bag)) {
    if (! fn(bag[key], key)) { result[key] = bag[key]; }
  }
  return result;
}

/**
 * `bag`'s entries for which `rule(val, key)` holds — the inverse of `omitBy`, and the
 * generic-rule sibling of `pickDefined`'s fixed "is defined" check, matching lodash's
 * `pickBy(object, [predicate=_.identity])`. `rule` is coerced through `iteratee` @see `iteratee`.
 * @example
 *   pickBy({ "a": 1, "b": 2, "c": 3 }, (val, _key) => val > 1); // => { "b": 2, "c": 3 }
 */
export function pickBy(bag is map, rule) returns map {
  const fn = iteratee(rule);
  var result = {};
  for (var key in keys(bag)) {
    if (fn(bag[key], key)) { result[key] = bag[key]; }
  }
  return result;
}

// --

// == [Function values] --

export const ClxnGetsetFuncs = {
  "lastInWins": lastInWins,
  "getAt":             (bag, path, fallback)                     => getAt(bag, path, fallback),
  "setAt":             (bag, path, val, onCollision)             => setAt(bag, path, val, onCollision),
  "update":            (bag, keyStrOrPath, updater)              => update(bag, keyStrOrPath, updater),
  "updateWith":        (bag, keyStrOrPath, updater, onCollision) => updateWith(bag, keyStrOrPath, updater, onCollision),
  "setAtWith":         (bag, keyStrOrPath, val, segmentFor)      => setAtWith(bag, keyStrOrPath, val, segmentFor),
  "deepMerge":         (existing, incoming)                      => deepMerge(existing, incoming),
  "assignWith":        (existing, incoming, combine)             => assignWith(existing, incoming, combine),
  "mergeWith":         (existing, incoming, combine)             => mergeWith(existing, incoming, combine),
  "pathForKey":        (keyStr)                                  => pathForKey(keyStr),
  "arrLast":           (arr)                                     => arrLast(arr),
  "arrFirst":          (arr)                                     => arrFirst(arr),
  "hasKey":            (obj, key)                                => hasKey(obj, key),
  "hasPresentKey":     (obj, key)                                => hasPresentKey(obj, key),
  "iteratee":          (spec)                                    => iteratee(spec),
  "matches":           (source)                                  => matches(source),
  "matchesProperty":   (path, srcValue)                          => matchesProperty(path, srcValue),
  "property":          (path)                                    => property(path),
  "propertyOf":        (obj)                                     => propertyOf(obj),
  "pick":              (bag, keylist)                            => pick(bag, keylist),
  "pickDefined":       (bag, keylist)                            => pickDefined(bag, keylist),
  "omit":              (bag, keylist)                            => omit(bag, keylist),
  "omitBy":            (bag, rule)                               => omitBy(bag, rule),
  "pickBy":            (bag, rule)                               => pickBy(bag, rule),
};
