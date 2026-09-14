FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
//
export import(path : "dd812faf6ff4099cda4aa0eb", version : "4f51cee26e9dac742d41aef0");
export import(path : "66e287bede293cb227dfb89c", version : "92efbb7ccaa5d62b7bde80f1");

export function runTests(context is Context, testname is string, verbose is boolean, testCases is array, testfunc is function) returns map {
    var results = []; var counts = { "OK": 0, "MISMATCH": 0, "ERROR": 0, total: 0 };
    for (var testCase in testCases) {
      const args        = testCase[0];
      const wanted      = testCase[1];
      const description = (size(testCase) >= 3 ? testCase[2] : "");
      var   grade       = undefined;
      var   actual      = undefined;
      try {
        actual = testfunc(args);
        grade  = (actual == wanted) ? "OK" : "MISMATCH";
      } catch (error) {
        actual = error;
        grade  = "ERROR";
      }
      results  = append(results, concatenateArrays([[grade, actual, wanted], args]));
      counts[grade] += 1;
      counts.total  += 1;
      if (grade != "OK") {
        debug(context, "Test " ~ grade ~ " for " ~ testname ~ replace(replace("" ~ args, "^\\[", "("), "\\]$", ")"));
        if (description != "") { debug(context, "Test of " ~ description); }
        debug(context, "Actual: " ~ [actual]);
        debug(context, "Wanted: " ~ [wanted]);
      }
    }
    const ok = (counts.total == counts.OK);
    if (verbose || (! ok)) {
      debug(context, "Tests for " ~ testname ~ ": " ~ counts);
      debug(context, results);
    }
    return { "counts": counts, "results": results, "OK": ok };
}

/**
 * Wraps a test function so a suite of throw cases runs through the same harness: the wrapped
 * function hands back whatever was thrown, and throws when nothing was. Belongs next to
 * runTests rather than here, once you have somewhere to put it.
 */
export function assertThrows(testfunc is function) returns function {
  return (args is array) => errorFromThrow(testfunc, args);
}
enum ReturnValue { VOID }

export function errorFromThrow(testfunc is function, args is array) {
  var actual = ReturnValue.VOID;
  try {
    actual = testfunc(args);
  } catch (err) {
    // println("caught error" ~ err);
    return err;
  }
  throw 'Expected a throw, got ' ~ [actual];
}
