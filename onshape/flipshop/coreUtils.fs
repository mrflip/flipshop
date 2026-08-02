FeatureScript 3029;
import(path : "onshape/std/common.fs", version : "3029.0");

export function ifNil(val, fallback) {
  if (val == undefined) { return fallback; }
  return val;
}

export function ifBlank(val, fallback) {
  if (strBlank(val)) { return fallback; }
  return val;
}

export function truthy(val)    { return (val != undefined) && (val != false); }

export function isPresent(val) { return (val != undefined); }

export function strBlank(val)  { return isUndefinedOrEmptyString(val); }

export function isEmpty(val) returns boolean {
    if (val is undefined) { return true; }
    if (val is map || val is string || val is array) { return sizeof(val) <= 0; }
    return false;
}

export function arrayIncludes(arr is array, target) returns boolean {
  for (var item in arr) {
    if (item == target) { return true; }
  }
  return false;
}

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

export function strSlice(str is string, begseq is number, endseq is number) returns string {
    const len = length(str);
    // Negative indices count back from the end; positive indices clamp to len
    var beg = (begseq < 0) ? max(len + begseq, 0) : min(begseq, len);
    var end = (endseq < 0) ? max(len + endseq, 0) : min(endseq, len);

    if (beg >= end) { return ""; }

    return substring(str, beg, end);
}
export enum SequencePosition { annotation { "Name": "End of String" } END }

export function rangedSequencePosition(pos is SequencePosition, beg is number, end is number) {
    if (pos == SequencePosition.END) { return end; }
    return undefined;
}

// See tests in utilsTests.fs

export function strSlice(str is string, begseq is number, endseq is SequencePosition) returns string {
  return strSlice(str, begseq, rangedSequencePosition(endseq, 0, length(str)));
}
export function strSlice(str is string, begseq is number) returns string {
    return strSlice(str, begseq, length(str));
}

export function strTake(str is string, len is number) returns string {
    if (len <= 0) { return ""; }
    return substring(str, 0, min(len, length(str)));
}
export function strTakeRight(str is string, len is number) returns string {
    if (len <= 0) { return ""; }
    const beg = max(0, length(str) - len);
    return substring(str, beg, length(str));
}

export function arrLast(arr is array) {
    if (size(arr) <= 0) { return undefined; }
    return arr[size(arr) - 1];
}

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

// boolean, number, string, array, map, box, function, builtin, and undefined
enum NextStepAction {
    annotation { "Name": "Break out of the loop early" }
    BREAK
}
export enum MissingPolicy {
    annotation { "Name": "Missing values do not appear in the result" }
    SKIP,
    annotation { "Name": "Missing values will be treated as undefined" }
    USE_UNDEFINED
}

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

export function hasKey(obj is map, key is string) returns boolean {
    return (obj[key] != undefined);
}
export function hasPresentKey(obj is array, key is number) returns boolean {
  return (key >= 0) && (key < size(obj)) && (obj[key] != undefined);
}
export function hasKey(obj is array, key is number, missingPolicy is MissingPolicy) returns boolean {
  if (missingPolicy == MissingPolicy.SKIP) { return hasPresentKey(obj, key); }
  return (key >= 0) && (key < size(obj));
}
export function hasKey(obj is array, key is number) returns boolean {
  return (key >= 0) && (key < size(obj));
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

export function boxarrPush(arrRef is box, val) {
  arrRef[] = append(arrRef[], val);
  return val;
}
export function boxarrUnshift(arrRef is box, val) {
  arrRef[] = concatenateArrays([val], arrRef[]);
  return val;
}

export const noop = function() {};

export function curry3to0(func is function) { return (_a, _b, _c) => func();     }
export function curry3to1(func is function) { return (a,  _b, _c) => func(a);    }
export function curry3to2(func is function) { return (a,   b, _c) => func(a, b); }

export function curry2to0(func is function) { return (_a, _b) => func();     }
export function curry2to1(func is function) { return (a,  _b) => func(a);    }
export function curry2to2(func is function) { return (a,   b) => func(a, b); }

export function paddingFor(padstr is string, neededLen is number) returns string {
    const reps = ceil(neededLen / length(padstr));
    return repeatString(padstr, reps);
}
export function padLeft(str is string, minlen is number, padstr is string) returns string {
    if (padstr == '') { throw "Padchar must not be blank (trying to pad " ~ str ~ ")"; }
    const minlen0 = max(0, minlen);
    const neededLen = minlen0 - length(str);
    if (neededLen <= 0) { return str; }
    const padding = strTake(paddingFor(padstr, neededLen), neededLen);
    return padding ~ str;
}
export function padLeft(str is string, minlen is number) returns string {
  return padLeft(str, minlen, ' ');
}
export function padLeft(num is number, minlen is number, padstr is string) returns string {
  return padLeft('' ~ num, minlen, padstr);
}
export function padLeft(num is number, minlen is number) returns string {
  return padLeft(num, minlen, ' ');
}
export function padRight(str is string, minlen is number, padstr is string) returns string {
    if (padstr == '') { throw "Padchar must not be blank (trying to pad " ~ str ~ ")"; }
    const minlen0 = max(0, minlen);
    const neededLen = minlen0 - length(str);
    if (neededLen <= 0) { return str; }
    const padding = strTakeRight(paddingFor(padstr, neededLen), neededLen);
    return str ~ padding;
}
export function padRight(str is string, minlen is number) returns string {
  return padRight(str, minlen, ' ');
}