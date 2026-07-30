FeatureScript 2909;
import(path : "onshape/std/common.fs", version : "2909.0");

const mm          = millimeter;
const zero        = 0 * mm;

export function vector2(vec is Vector) {
    return vector(vec[0], vec[1]);
}

export function boxmRectangle(context is Context, sketch is Sketch, sketchLabel is string, boxm is map, construction is boolean) returns map {
  skRectangle(sketch, sketchLabel, {
    "firstCorner":  vector2(boxm.minvec),
    "secondCorner": vector2(boxm.maxvec),
    "construction": ifNil(construction, false),
  });
  return boxm;
}
export function boxmRectangle(context is Context, sketch is Sketch, sketchLabel is string, boxm is map) returns map {
  return boxmRectangle(context, sketch, sketchLabel, boxm, false);
}

export function boxmCoords(boxm is map) returns array {
    return [boxm.min0, boxm.max0, boxm.min1, boxm.max1, boxm.min2, boxm.max2];
}

export function boxmSimply(boxm is Vector) returns array {
    const unit = 1*mm;
    return [boxm[0] / unit, boxm[1] / unit, (size(boxm) <= 2) ? 0 :  boxm[2] / unit];
}

export function boxmSimply(boxm is map, unit is ValueWithUnits) returns array {
    const coords = boxmCoords(boxm);
    return [coords[0] / unit, coords[1] / unit, coords[2] / unit, coords[3] / unit, coords[4] / unit, coords[5] / unit];
}
export function boxmSimply(boxm is map) returns map { return boxmSimply(boxm, 1 * mm); }

export function boxmPretty(boxm is map, unit is ValueWithUnits) returns map {
  return {
    min0:  boxm.min0 / unit,   max0: boxm.max0 / unit,
    min1:  boxm.min1 / unit,   max1: boxm.max1 / unit,
    min2:  boxm.min2 / unit,   max2: boxm.max2 / unit,
    size0: boxm.size0 / unit,
    size1: boxm.size1 / unit,
    size2: boxm.size2 / unit,
  };
}
export function boxmPretty(boxm is map) returns map { return boxmPretty(boxm, 1 * mm); }

export function placeBoxm(boxm is map, opts is map) returns map {
    const shift0 = ifNil(opts.shift0, zero);
    const shift1 = ifNil(opts.shift1, zero);
    const shift2 = ifNil(opts.shift2, zero);
    const scale0 = ifNil(opts.scale0, 1);
    const scale1 = ifNil(opts.scale1, 1);
    const scale2 = ifNil(opts.scale2, 1);
    const min0 = shift0 + boxm.min0;             const min1 = shift1 + boxm.min1;             const min2 = shift2 + boxm.min2;
    const max0 = min0   + (scale0 * boxm.size0); const max1 = min1   + (scale1 * boxm.size1); const max2 = min2   + (scale2 * boxm.size2);
    return boxmForXYZ(min0, max0, min1, max1, min2, max2);
}

export function boxmRescale(boxm is map, opts is map) returns map {
    const shift0 = ifNil(opts.shift0, zero);
    const shift1 = ifNil(opts.shift1, zero);
    const shift2 = ifNil(opts.shift2, zero);
    const scale0 = ifNil(opts.scale0, 1);
    const scale1 = ifNil(opts.scale1, 1);
    const scale2 = ifNil(opts.scale2, 1);
    const min0 = shift0 + (scale0 * boxm.min0);  const min1 = shift1 + (scale1 * boxm.min1);  const min2 = shift2 + (scale2 * boxm.min2);
    const max0 = min0   + (scale0 * boxm.size0); const max1 = min1   + (scale1 * boxm.size1); const max2 = min2   + (scale2 * boxm.size2);
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

export function ifNil(val, fallback) { if (val == undefined) { return fallback; } return val; }
