FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
import(path : "66e287bede293cb227dfb89c", version : "7ad52f18be252f35a5ea9f6d");

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
 * lodash's get and set over maps and arrays, with your negative indexes kept.
 *
 * One rule decides how the second argument is read, everywhere:
 *   a string  is a dotted path, so "a.b" is two steps and "rows.-1.x" is three
 *   an array  is a list of literal keys, so ["a.b"] is one step and reaches a key with a dot in it
 *   a number  is an array index
 *
 * A path descends whatever it meets. A string segment indexes an array when it is an integer
 * string, a map by its own key otherwise. An empty path is the identity, so these fold.
 *
 * getAt returns the fallback (undefined when none is given) for any step that misses, including
 * stepping into a scalar. An array element that is present but undefined is returned as
 * undefined rather than as the fallback; a map cannot hold that case, since FeatureScript
 * elides an undefined value on the way in.
 *
 * setAt autovivifies missing steps as maps and returns the container with the type it was
 * handed. Where the leaf is already occupied it calls `onCollision(existing, incoming)`, which
 * defaults to lastInWins; @see `deepMerge` for the deep-merge rule. The resolver sees leaf
 * collisions only: a scalar sitting in the way of a deeper path is replaced outright, the way
 * lodash's set replaces one, while an existing map or array is descended and written through.
 *
 * Two more consequences of the language: setting an undefined value deletes a map key rather
 * than storing one, and writing past the end of an array pads it with undefined.
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
 * lodash's merge, which is the deep one Onshape's mergeMaps is not: two maps combine key by
 * key all the way down, anything else is replaced by what arrives, and an undefined source
 * leaves what is there alone. As a collision rule under a shallowest-first loop, "what
 * arrives wins" reads as "the deeper key wins".
 *
 * Unlike lodash, two arrays are replaced rather than merged index by index. An array here is
 * a value, and half-overwriting one is worse than replacing it. If you want lodash's rule it
 * is one more branch below.
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
 * Splits a dotted key into segments. An empty segment names an empty key, which a map is
 * perfectly willing to hold, so "a..b" is three steps and ".foo" is two.
 *
 * A key with a terminal dot is unhandled behavior: splitByRegexp discards exactly one trailing
 * empty segment, so "foo." reads as "foo" and ".." as two empty keys rather than three. The
 * first person who wants otherwise can decide what it means and patch it here.
 *
 * A key with no dot at all never reaches the splitter, which is what keeps "" a literal empty
 * key rather than whatever the trailing-empty rule would make of it.
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
