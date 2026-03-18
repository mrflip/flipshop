FeatureScript 2909;
import(path : "onshape/std/geometry.fs", version : "2909.0");
export import(path : "daa2f7d60ba23b30cdfc9d62", version : "146bbe88c26a4a3a41774202");

// SocketWrenches and SocketWrenchesByFamily are defined in SocketWrenches.fs
// (same Feature Studio document — no import needed)

const hugeSizeVal = 1000000;
const tinySizeVal = 0.001;
const zero        = 0 * millimeter;

annotation { "Feature Type Name": "Socket Cell Cutter" }
export const socketCellCutter = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name":  "Reference plane", "Filter":  QueryFilterCompound.ALLOWS_PLANE, "MaxNumberOfPicks":  1 }
  definition.referencePlane is Query;

  annotation { "Name" : "Socket Selection", "Lookup Table" : SocketWrenches3 }
  definition.socketPath is LookupTablePath;

  annotation { "Name":  "Layer height" }
  isLength(definition.layerHeight,   {(millimeter) : [tinySizeVal, 4.55, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name":  "Insertion gap" }
  isLength(definition.insertionGap, {(millimeter) : [0, 0.3, hugeSizeVal]} as LengthBoundSpec);
}
{
  const socketParams = socketCellParams(context, definition);
  const ids = {
    bboxesSk:         id + "bboxesSk",
    shapesSk:         id + "shapesSk",
    labelsSk:         id + "labelsSk",
    calloutsSk:       id + "calloutsSk",
    extrudedCutout:   id + "extrudedCutout",
  };

  const sketchPlane = evPlane(context, { "face":  definition.referencePlane });
  const sketches = {
   bboxes:    newSketchOnPlane(context, ids.bboxesSk,    { "sketchPlane":  sketchPlane }),
   shapes:    newSketchOnPlane(context, ids.shapesSk,    { "sketchPlane":  sketchPlane }),
   labels:    newSketchOnPlane(context, ids.labelsSk,    { "sketchPlane":  sketchPlane }),
   callouts:  newSketchOnPlane(context, ids.calloutsSk,  { "sketchPlane":  sketchPlane }),
  };

  drawBoundingBoxes(context,   ids.bboxesSk, sketches.bboxes, socketParams);
  drawSocketBaseShape(context, ids.shapesSk, sketches.shapes, socketParams.socket, socketParams);
  skSolve(sketches.shapes);

  const center = vector(socketParams.nomBounds.ctrH, socketParams.nomBounds.ctrV);
  radialText(context, ids.calloutsSk, socketParams, sketches.callouts,
    socketParams.socket.sizing_mm_text, center,
    145 * degree, socketParams.cutoutRadius, 1.5 * millimeter, 4 * millimeter);
  skSolve(sketches.callouts);

  const cutoutFaces = qCreatedBy(ids.shapesSk, EntityType.FACE);
  opExtrude(context, ids.extrudedCutout, {
    "entities":  cutoutFaces,
    "direction": sketchPlane.normal,
    "endBound":  BoundingType.BLIND,
    "endDepth":  socketParams.cutoutDepth,
  });

  setProperty(context, {
    "entities":     qCreatedBy(ids.extrudedCutout, EntityType.BODY),
    "propertyType": PropertyType.NAME,
    "value":        "Socket Cell Cutter",
  });
});

function getSocketFamilyRef(context is Context, keypath is LookupTablePath) {
  return SocketWrenches2.entries[keypath.socket_kind].entries[keypath.drive_kind].entries[keypath.unit_system].entries[keypath.sqdrive_size].entries[keypath.reach_kind].entries[keypath.socket_variant];
}

function getSocketRef(context is Context, keypath is LookupTablePath) {
  return SocketWrenches3.entries[keypath.socket_kind].entries[keypath.drive_kind].entries[keypath.unit_system].entries[keypath.sqdrive_size].entries[keypath.reach_kind].entries[keypath.socket_variant].entries[keypath.sizing];
}

function drawBoundingBoxes(context is Context, id is Id, sketch is Sketch, params is map) returns builtin {
  const nb = params.nomBounds;
  const cb = params.cutBounds;

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

// Returns a 2-vector from center along rayAngle to the given distance.
function radialPoint(center is Vector, rayAngle, dist) returns Vector {
  return center + dist * vector(cos(rayAngle), sin(rayAngle));
}

// Draws a text label on sketch whose baseline is perpendicular to rayAngle,
// centered on the ray at (radius + offset) from center.
function radialText(context is Context, id is Id, params, sketch is Sketch,
  text is string, center is Vector, rayAngle, radius, offset, textHeight) {
  const pt      = radialPoint(center, rayAngle, radius + offset);
  const perpDir = vector(-sin(rayAngle), cos(rayAngle));
  const rayDir  = vector( cos(rayAngle), sin(rayAngle));
  const halfW   = textHeight * 0.65 * length(text);
  skText(sketch, "sizeLabel", {
    "fontName":      "OpenSans-Regular.ttf",
    "firstCorner":   pt - halfW * perpDir,
    "secondCorner":  pt + halfW * perpDir + textHeight * rayDir,
    "text":          text,
  });
}

// Draws text on sketch, solves it, queries the resulting edges, and debugs their bounding box.
// id must be the feature Id used to create sketch (used to query created entities).
// textHeight sets the font size; firstCorner is the bottom-left anchor of the text box.
function drawAndMeasureText(context is Context, id is Id, sketch is Sketch,
  text is string, textHeight, firstCorner is Vector) {
  const estimatedWidth = textHeight * 0.65 * length(text);
  skText(sketch, "text", {
    "fontName":      "OpenSans-Regular.ttf",
    "firstCorner":   firstCorner,
    "secondCorner":  firstCorner + vector(estimatedWidth, textHeight),
    "text":          text,
  });
  skSolve(sketch);
  const textEnts   = sketchEntityQuery(id, EntityType.EDGE, "text");
  const textBounds = evBox3d(context, { "topology": textEnts, "tight": true });
  debug(context, textBounds);
}

function drawSocketBaseShape(context is Context, id is Id, sketch is Sketch, socket is map, params is map) {
  const center = vector(params.nomBounds.ctrH, params.nomBounds.ctrV);
  // Nominal wrench-end circle (construction)
  skCircle(sketch, "wrenchEndCircle", { "center": center, "radius": params.bodyDiam / 2, "construction": true,  });
  // Cutout circle = wrench-end + insertion gap all around (real region)
  skCircle(sketch, "cutoutCircle",    { "center": center, "radius": params.cutoutRadius });
}

function socketFamilyCellParams(context, definition is map) returns map {
  // Look up the specific socket by sizing
  const socketPath   = definition.socketPath;
  const socketFamily = getSocketFamilyRef(context, socketPath);
  debug(context, [definition.socketPath]);
  if (socketFamily == undefined) { throw regenError("Unknown socket family '" ~ socketPath); }

  return mergeMaps(socketPath, {
    "socketPath":    socketPath,
    "socketFamily":  socketFamily,
    "insertionGap":  definition.insertionGap,
    "layerHeight":   definition.layerHeight,
  });
}

function socketCellParams(context, definition is map) returns map {
  const socketParams = socketFamilyCellParams(context, definition);

  const socket       = getSocketRef(context, socketParams.socketPath);
  const bodyDiam     = (socket.wrench_end_diam != undefined) ? socket.wrench_end_diam : socket.wx_overall;
  const cutoutRadius = bodyDiam / 2 + socketParams.insertionGap;
  const cutoutDepth  = 2 * socketParams.layerHeight;

  // Bounding boxes centered at sketch origin
  const nomR      = bodyDiam / 2;
  const nomBounds = {
    "minH":  -nomR,        "ctrH":  zero,  "maxH":  nomR,        "sizeH": bodyDiam,
    "minV":  -nomR,        "ctrV":  zero,  "maxV":  nomR,        "sizeV": bodyDiam,
  };
  const cutBounds = {
    "minH":  -cutoutRadius, "ctrH":  zero,  "maxH":  cutoutRadius, "sizeH": 2 * cutoutRadius,
    "minV":  -cutoutRadius, "ctrV":  zero,  "maxV":  cutoutRadius, "sizeV": 2 * cutoutRadius,
  };

  return mergeMaps(socketParams, {
    "socket":        socket,
    "bodyDiam":      bodyDiam,
    "cutoutRadius":  cutoutRadius,
    "cutoutDepth":   cutoutDepth,
    "nomBounds":     nomBounds,
    "cutBounds":    cutBounds,
  });
}
