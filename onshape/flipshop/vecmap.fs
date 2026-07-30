FeatureScript 2909;
import(path : "onshape/std/common.fs", version : "2909.0");

const mm          = millimeter;
const zero        = 0 * mm;

export function boxMag(bb is Box3d, units is ValueWithUnits) {
  const min  = vecmapM(vec2Map(bb.minCorner), units);
  const max  = vecmapM(vec2Map(bb.maxCorner), units);
  const size = vecmapSubtract(max, min);
  return { "min": min, "max": max, "size": size };
}

export function vec2Map(vec is Vector) {
  return { a: vec[0], b: vec[1], c: vec[2] };
}

export function vecmapSubtract(vec1 is map, vec2 is map) {
    return { a: vec1.a - vec2.a, b: vec1.b - vec2.b, c: (vec1.c == undefined || vec2.c == undefined) ? 0 : vec1.c - vec2.c };
}

export function vecmapM(vecmap is map, units is ValueWithUnits) {
  const vm2 = { a: vecmap.a / units, b: vecmap.b / units };
  if (vecmap.c == undefined) { return vm2; }
  return mergeMaps(vm2, { c: vecmap.c / units });
}

export function vector2(vec is Vector) {
    return vector(vec[0], vec[1]);
}

export function boxmRectangle(context is Context, sketch is Sketch, sketchLabel is string, boxm is map, construction is boolean) returns map {
  skRectangle(sketch, sketchLabel, {
    "firstCorner":  vector2(boxm.minvec),
    "secondCorner": vector2(boxm.maxvec),
    "construction": (construction == undefined) ? false : construction,
  });
  return boxm;
}
export function boxmRectangle(context is Context, sketch is Sketch, sketchLabel is string, boxm is map) returns map {
  return boxmRectangle(context, sketch, sketchLabel, boxm, false);
}

export function boxmCoords(boxm is map) returns array {
    return [boxm.min0, boxm.max0, boxm.min1, boxm.max1, boxm.min2, boxm.max2];
}

export function boxmSimply(boxm is map, units is ValueWithUnits) returns array {
    const coords = boxmCoords(boxm);
    return [coords[0] / units, coords[1] / units, coords[2] / units, coords[3] / units, coords[4] / units, coords[5] / units];
}
export function boxmSimply(boxm is map) returns map { return boxmSimply(boxm, 1 * mm); }
export function boxmPretty(boxm is map, units is ValueWithUnits) returns map {
  return {
    // minvec: bbox.minCorner, maxvec: bbox.maxCorner,
    min0:  boxm.min0 / units,   max0: boxm.max0 / units,
    min1:  boxm.min1 / units,   max1: boxm.max1 / units,
    min2:  boxm.min2 / units,   max2: boxm.max2 / units,
    size0: boxm.size0 / units,
    size1: boxm.size1 / units,
    size2: boxm.size2 / units,
  };
}
export function boxmPretty(boxm is map) returns map { return boxmPretty(boxm, 1 * mm); }

export function placeBoxm(boxm is map, opts is map) returns map {
    const shift0 = (opts.shift0 == undefined) ? zero : opts.shift0;
    const shift1 = (opts.shift1 == undefined) ? zero : opts.shift1;
    const shift2 = (opts.shift2 == undefined) ? zero : opts.shift2;
    const scale0 = (opts.scale0 == undefined) ? 1    : opts.scale0;
    const scale1 = (opts.scale1 == undefined) ? 1    : opts.scale1;
    const scale2 = (opts.scale2 == undefined) ? 1    : opts.scale2;
    const min0 = boxm.min0 + shift0;           const min1 = boxm.min1 + shift1;           const min2 = boxm.min2 + shift2;
    const max0 = min0 + (boxm.size0 * scale0); const max1 = min1 + (boxm.size1 * scale1); const max2 = min2 + (boxm.size2 * scale2);
    return boxmForXYZ(min0, max0, min1, max1, min2, max2);
}

export function boxmForBbox(bbox is Box3d) returns map {
  return boxmForXYZ(
    bbox.minCorner[0], bbox.maxCorner[0],
    bbox.minCorner[1], bbox.maxCorner[1],
    bbox.minCorner[2], bbox.maxCorner[2]
  );
}

export function boxmForXYZ(min0 is ValueWithUnits, max0 is ValueWithUnits, min1 is ValueWithUnits, max1 is ValueWithUnits, min2 is ValueWithUnits, max2 is ValueWithUnits) returns map {
    return {
        "min0": min0, "max0": max0, "min1": min1, "max1": max1, "min2": min2, "max2": max2,
        "size0": max0 - min0,
        "size1": max1 - min1,
        "size2": max2 - min2,
        minvec:  vector(min0, min1, min2),
        maxvec:  vector(max0, max1, max2),
        midvec:  vector((min0 + max0) / 2, (min1 + max1) / 2, (min2 + max2) / 2),
        sizevec: vector(max0 - min0, max1 - min1, max2 - min2)
    };
}
export function boxmForXYZ(min0 is ValueWithUnits, max0 is ValueWithUnits, min1 is ValueWithUnits, max1 is ValueWithUnits) returns map {
    return boxmForXYZ(min0, max0, min1, max1, zero, zero);
}