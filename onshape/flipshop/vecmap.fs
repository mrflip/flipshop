FeatureScript 2909;
import(path : "onshape/std/common.fs", version : "2909.0");

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

export function boxmForBbox(bbox is Box3d) returns map {
  return mergeMaps(bbox, {
    // minvec: bbox.minCorner, maxvec: bbox.maxCorner,
    min0:  bbox.minCorner[0], min1: bbox.minCorner[1],
    max0:  bbox.maxCorner[0], max1: bbox.maxCorner[1],
    size0: bbox.maxCorner[0] - bbox.minCorner[0],
    size1: bbox.maxCorner[1] - bbox.minCorner[1],
  });
}

export function boxmCoords(boxm is map) returns array {
    return [boxm.min0, boxm.min1, boxm.max0, boxm.max1];
}

export function boxmSimply(boxm is map, units is ValueWithUnits) returns array {
    const coords = boxmCoords(boxm);
    return [coords[0] / units, coords[1] / units, coords[2] / units, coords[3] / units];
}
export function boxmPretty(boxm is map, units is ValueWithUnits) returns map {
  return {
    // minvec: bbox.minCorner, maxvec: bbox.maxCorner,
    min0:  boxm.min0 / units, min1: boxm.min1 / units,
    max0:  boxm.max0 / units, max1: boxm.max1 / units,
    size0: boxm.size0 / units,
    size1: boxm.size1 / units,
  };
}
export function placeBoxm(boxm is map, opts is map) returns map {
    const shift0 = (opts.shift0 == undefined) ? zero : opts.shift0;
    const shift1 = (opts.shift1 == undefined) ? zero : opts.shift1;
    const scale0 = (opts.scale0 == undefined) ? 1    : opts.scale0;
    const scale1 = (opts.scale1 == undefined) ? 1    : opts.scale1;
    const min0 = (boxm.min0 + shift0);       const min1 = boxm.min1 + shift1;
    const max0 = min0 + boxm.size0 * scale0; const max1 = min1 + boxm.size1 * scale1;
    return { "min0": min0, "min1": min1, "max0": max0, "max1": max1, "size0": max0 - min0, "size1": max1 - min1 };
}