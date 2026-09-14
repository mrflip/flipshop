FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
//
import(path : "54590bc1c9cee0141b968fbb", version : "502cf2d68de5897e8d3b8586"); // clxn for flatMap
import(path : "66e287bede293cb227dfb89c", version : "69074ba3c174781892f72df6"); // ifNil &c


export function strOrderBy(vals is array) returns array {
    var bucket = {};
    for (var ii = 0; ii < size(vals); ii += 1) {
        const str = "" ~ vals[ii];
        bucket[str] = ifNil(bucket[str], 0) + 1;
    }
    return flatMap(bucket, (reps, val) => makeArray(reps, val));
}

export function strOrderByUniq(vals is array) returns array {
    var bucket = {};
    for (var ii = 0; ii < size(vals); ii += 1) {
        const str = vals[ii];
        bucket["" ~ str] = ii;
    }
    return keys(bucket);
}

export function cmp() {
}

export function strCmp(aa is string, bb is string) returns number {
    const sorted = strOrderBy([aa, bb]);
    if (size(sorted) == 1)      { return  0; }
    if (sorted[0] == ('' ~ aa)) { return -1; }
    if (sorted[0] == ('' ~ bb)) { return  1; }
    throw "The sort did not work: " ~ sorted ~ " for comparing " ~ [aa, bb];
}