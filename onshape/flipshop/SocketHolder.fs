FeatureScript 2909;
import(path : "onshape/std/geometry.fs", version : "2909.0");

// SocketWrenches and SocketWrenchesByFamily are defined in SocketWrenches.fs
// (same Feature Studio document — no import needed)

const hugeSizeVal = 1000000;
const tinySizeVal = 0.001;

annotation { "Feature Type Name": "Socket Cell Cutter" }
export const socketCellCutter = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name":  "Reference plane", "Filter":  QueryFilterCompound.ALLOWS_PLANE, "MaxNumberOfPicks":  1 }
  definition.referencePlane is Query;

  // Pull-down of family titles from SocketWrenchesByFamily keys
  annotation { "Name":  "Socket family", "Default":  "Bolt Socket, 6-Point Metric, 3/8 Sq.Dr, Regular" }
  definition.socketFamily is string;

  // Pull-down of sizings within the selected family
  annotation { "Name":  "Socket sizing", "Default":  "10mm" }
  definition.socketSizing is string;

  annotation { "Name":  "Layer height" }
  isLength(definition.layerHeight,   {(millimeter) : [tinySizeVal, 4.55, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name":  "Insertion gap" }
  isLength(definition.insertionGap, {(millimeter) : [0, 0.3, hugeSizeVal]} as LengthBoundSpec);
}
{
  const params = socketHolderCellParams(definition);
  const ids = {
    "boundingBoxes":  id + "boundingBoxes",
    "cutoutShapes":   id + "cutoutShapes",
    "extrudedCutout": id + "extrudedCutout",
  };

  const sketchPlane = evPlane(context, { "face":  definition.referencePlane });

  drawBoundingBoxes(context, ids.boundingBoxes, sketchPlane, params);
  drawCutoutShapes(context,  ids.cutoutShapes,  sketchPlane, params);

  const cutoutFaces = qCreatedBy(ids.cutoutShapes, EntityType.FACE);
  opExtrude(context, ids.extrudedCutout, {
    "entities":  cutoutFaces,
    "direction": sketchPlane.normal,
    "endBound":  BoundingType.BLIND,
    "endDepth":  params.cutout_depth,
  });

  setProperty(context, {
    "entities":     qCreatedBy(ids.extrudedCutout, EntityType.BODY),
    "propertyType": PropertyType.NAME,
    "value":        "Socket Cell Cutter",
  });
});

function socketHolderCellParams(definition is map) returns map {
  // Look up the sizing map for this family
  const familySockets = SocketWrenchesByFamily[definition.socketFamily];
  if (familySockets == undefined) { throw regenError("Unknown socket family: " ~ definition.socketFamily); }

  // Look up the specific socket by sizing
  const socket = familySockets[definition.socketSizing];
  if (socket == undefined) { throw regenError("Unknown sizing '" ~ definition.socketSizing ~ "' for family '" ~ definition.socketFamily ~ "'"); }

  // wx_overall is always present and equals the outer body diameter
  const bodyDiam = (socket.wrench_end_diam != undefined) ? socket.wrench_end_diam : socket.wx_overall;

  const insertionGap = definition.insertionGap;
  const layerHeight  = definition.layerHeight;
  const cutoutRadius = bodyDiam / 2 + insertionGap;
  const cutoutDepth  = 2 * layerHeight;

  // Bounding boxes centered at sketch origin
  const zero     = 0 * millimeter;
  const nomR     = bodyDiam / 2;
  const nomBounds = {
    "minH":  -nomR,        "ctrH":  zero,  "maxH":  nomR,        "sizeH": bodyDiam,
    "minV":  -nomR,        "ctrV":  zero,  "maxV":  nomR,        "sizeV": bodyDiam,
  };
  const cutBounds = {
    "minH":  -cutoutRadius, "ctrH":  zero,  "maxH":  cutoutRadius, "sizeH": 2 * cutoutRadius,
    "minV":  -cutoutRadius, "ctrV":  zero,  "maxV":  cutoutRadius, "sizeV": 2 * cutoutRadius,
  };

  return {
    "socket":        socket,
    "body_diam":     bodyDiam,
    "insertion_gap": insertionGap,
    "layer_height":  layerHeight,
    "cutout_radius": cutoutRadius,
    "cutout_depth":  cutoutDepth,
    "nom_bounds":    nomBounds,
    "cut_bounds":    cutBounds,
  };
}

function drawBoundingBoxes(context is Context, id is Id, sketchPlane is Plane, params is map) returns builtin {
  const sketch = newSketchOnPlane(context, id, { "sketchPlane":  sketchPlane });
  const nb = params.nom_bounds;
  const cb = params.cut_bounds;

  // Nominal body bounding box (construction, matches wrench-end diameter)
  skRectangle(sketch, "nominalBounds", {
    "firstCorner":  vector(nb.minH, nb.minV),
    "secondCorner": vector(nb.maxH, nb.maxV),
    "construction": true,
  });
  // Cutout bounding box (solid, includes insertion gap)
  skRectangle(sketch, "cutoutBounds", {
    "firstCorner":  vector(cb.minH, cb.minV),
    "secondCorner": vector(cb.maxH, cb.maxV),
    "construction": false,
  });
  // Midpoint dots on nominal bounds
  skPoint(sketch, "nomTopMidDot",   { "position":  vector(nb.ctrH, nb.maxV) });
  skPoint(sketch, "nomRightMidDot", { "position":  vector(nb.maxH, nb.ctrV) });
  // Midpoint dots on cutout bounds
  skPoint(sketch, "cutTopMidDot",   { "position":  vector(cb.ctrH, cb.maxV) });
  skPoint(sketch, "cutRightMidDot", { "position":  vector(cb.maxH, cb.ctrV) });

  skSolve(sketch);
  return sketch;
}

function drawCutoutShapes(context is Context, id is Id, sketchPlane is Plane, params is map) {
  const sketch = newSketchOnPlane(context, id, { "sketchPlane":  sketchPlane });
  const center = vector(params.nom_bounds.ctrH, params.nom_bounds.ctrV);

  // Nominal wrench-end circle (construction)
  skCircle(sketch, "wrenchEndCircle", {
    "center":       center,
    "radius":       params.body_diam / 2,
    "construction": true,
  });
  // Cutout circle = wrench-end + insertion gap all around (real region)
  skCircle(sketch, "cutoutCircle", {
    "center": center,
    "radius": params.cutout_radius,
  });

  skSolve(sketch);
}
