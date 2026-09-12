FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");

// boolean, number, string, array, map, box, function, builtin, and undefined
enum NextStepAction {
    annotation { "Name": "Break out of the loop early" }
    BREAK
}
export enum MissingPolicy {
    annotation { "Name": "Treat set-but-undefined values the same as absent values: they should not appear in the result" }
    SKIP,
    annotation { "Name": "Treat set-but-undefined values as present (include them in results)" }
    USE_UNDEFINED
}

// == [Collection Inspection] -- sizeof, hasKey

export function sizeof(val is map) returns number {
    return size(keys(val));
}
export function sizeof(val is string) returns number {
    return length(val);
}
export function sizeof(val is array) returns number {
    return size(val);
}
export function sizeof(val is undefined) returns number {
    return 0;
}

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

export function arrayIncludes(arr is array, target) returns boolean {
  for (var item in arr) {
    if (item == target) { return true; }
  }
  return false;
}
// --

// == [Collection retrieve many] -- pick, pickDefined, arrLast
export function pick(bag is map, keylist is array) returns map {
    var result = {};
    for (var kk in keylist) {
      result[kk] = bag[kk];
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

export function arrLast(arr is array) {
    if (size(arr) <= 0) { return undefined; }
    return arr[size(arr) - 1];
}
// --

// == [valuesAt]

export function valuesAt(arr is array, keylist is array) returns array {
  return mapValues(keylist, (seq is number, _) => hasKey(arr, seq) ? arr[seq] : undefined);
}

export function valuesAt(arr is array, keylist is array, missingPolicy is MissingPolicy) returns array {
  if (missingPolicy == MissingPolicy.USE_UNDEFINED) {
    return mapValues(keylist, (seq is number, _) => hasKey(arr, seq) ? arr[seq] : undefined);
  }
  const result = new box([]);
  forEach(keylist, (seq is number, _) => { if (hasPresentKey(arr, seq)) { boxarrPush(result, arr[seq]); } });
  return result[];
}

export function valuesAt(bag is map, keylist is array, missingPolicy is MissingPolicy) returns array {
  if (missingPolicy == MissingPolicy.USE_UNDEFINED) {
    return mapValues(keylist, (key is string, _) => bag[key]);
  }
  const result = new box([]);
  forEach(keylist, (key is string, _) => { if (bag[key] != undefined) { boxarrPush(result, bag[key]); } });
  return result[];
}

export function valuesAt(bag is map, keylist is array) returns array {
  return mapValues(keylist, (key is string, _) => bag[key]);
}
//--

// == [Collection Walking] -- mapValues, objectify, rebag

export function forEach(bag is map, keylist is array, missingPolicy is MissingPolicy, func is function) {
  var seq  = 0;
  if (missingPolicy == MissingPolicy.SKIP) {
      for (var key in keylist) {
        const val = bag[key];
        if (val == undefined) { continue; }
        const result = func(val, key, seq);
        if (result == NextStepAction.BREAK) { break; }
        seq += 1;
      }
  } else {
      for (var key in keylist) {
        const val = bag[key];
        const result = func(val, key, seq);
        if (result == NextStepAction.BREAK) { break; }
        seq += 1;
      }
  }
}

export function forEach(bag is map, missingPolicy is MissingPolicy, func is function) {
    return forEach(bag, keys(bag), MissingPolicy.USE_UNDEFINED, func);
}

export function forEach(bag is map, keylist is array, func is function) {
  var seq  = 0;
  for (var key in keylist) {
    const val = bag[key];
    const result = func(val, key, seq);
    if (result == NextStepAction.BREAK) { break; }
    seq += 1;
  }
}

export function forEach(bag is map, func is function) {
  return forEach(bag, keys(bag), func);
}

export function forEach(arr is array, missingPolicy is MissingPolicy, func is function) {
  if (missingPolicy == MissingPolicy.SKIP) {
      for (var seq = 0; seq < size(arr); seq += 1) {
        if (arr[seq] == undefined) { continue; }
        const result = func(arr[seq], seq, seq);
        if (result == NextStepAction.BREAK) { break; }
      }
  } else {
      for (var seq = 0; seq < size(arr); seq += 1) {
        const result = func(arr[seq], seq, seq);
        if (result == NextStepAction.BREAK) { break; }
      }
  }
}

export function forEach(arr is array, func is function) {
  for (var seq = 0; seq < size(arr); seq += 1) {
    const result = func(arr[seq], seq);
    if (result == NextStepAction.BREAK) { break; }
  }
}

export function mapValues(bag is map, keylist is array, missingPolicy is MissingPolicy, func is function) returns map {
  const result = new box({});
  forEach(bag, keylist, missingPolicy, (val, key, seq is number) => { result[][key] = func(val, key); });
  return result[];
}
export function mapValues3(bag is map, keylist is array, missingPolicy is MissingPolicy, func is function) returns map {
  const result = new box({});
  forEach(bag, keylist, missingPolicy, (val, key, seq is number) => { result[][key] = func(val, key, seq); });
  return result[];
}

export function mapValues(bag is map, keylist is array, func is function) returns map {
  const result = new box({});
  forEach(bag, keylist, (val, key, seq is number) => { result[][key] = func(val, key); });
  return result[];
}
export function mapValues3(bag is map, keylist is array, func is function) returns map {
  const result = new box({});
  forEach(bag, keylist, (val, key, seq is number) => { result[][key] = func(val, key, seq); });
  return result[];
}
export function mapValues(bag is map, missingPolicy is MissingPolicy, func is function) returns map {
  return mapValues(bag, keys(bag), missingPolicy, func);
}
export function mapValues3(bag is map, missingPolicy is MissingPolicy, func is function) returns map {
  return mapValues3(bag, keys(bag), missingPolicy, func);
}
export function mapValues(bag, func is function) returns map {
  return mapValues(bag, keys(bag), func);
}
export function mapValues3(bag is map, func is function) returns map {
  return mapValues3(bag, keys(bag), func);
}

export function mapValues(arr is array, missingPolicy is MissingPolicy, func is function) returns array {
  var result = new box(makeArray(size(arr)));
  forEach(arr, missingPolicy, (val, seq, _) => {
      result[][seq] = func(val, seq);
  });
  return result[];
}

export function mapValues3(arr is array, missingPolicy is MissingPolicy, func is function) returns array {
  var result = new box(makeArray(size(arr)));
  forEach(arr, missingPolicy, (val, seq, _) => {
      result[][seq] = func(val, seq, seq);
  });
  return result[];
}

export function mapValues(arr is array, func is function) returns array {
  var result = makeArray(size(arr));
  for (var seq = 0; seq < size(arr); seq += 1) {
    result[seq] = func(arr[seq], seq);
  }
  return result;
}
export function mapValues3(arr is array, func is function) returns array {
  var result = makeArray(size(arr));
  for (var seq = 0; seq < size(arr); seq += 1) {
    result[seq] = func(arr[seq], seq, seq);
  }
  return result;
}

export function objectify(arr is array, func is function) returns map {
    return rebag(arr, (val, seq is number) => [val, func(val, seq)]);
}
export function rebag(arr is array, func is function) returns map {
    var result = new box({});
    forEach(arr, (val, seq is number)                => { const kv = func(val, seq);      if (kv != undefined) { result[][kv[0]] = kv[1]; } });
    return result[];
}
export function rebag(bag is map, func is function) returns map {
    var result = new box({});
    forEach(bag, (val, key is string, seq is number) => { const kv = func(val, key, seq); if (kv != undefined) { result[][kv[0]] = kv[1]; } });
    return result[];
}
// --

// == [Box Array Utils]

export function boxarrPush(arrRef is box, val) {
  arrRef[] = append(arrRef[], val);
  return val;
}
export function boxarrUnshift(arrRef is box, val) {
  arrRef[] = concatenateArrays([val], arrRef[]);
  return val;
}
//--
