FeatureScript 2909;
import(path : "onshape/std/geometry.fs", version : "2909.0");
export import(path : "daa2f7d60ba23b30cdfc9d62", version : "146bbe88c26a4a3a41774202");

// SocketWrenches and SocketWrenchesByFamily are defined in SocketWrenches.fs
// (same Feature Studio document — no import needed)

// Sentinel values for LengthBoundSpec min/max when no practical limit applies.
const hugeSizeVal = 1000000;
const tinySizeVal = 0.001;
// Shorthand aliases used throughout for readability.
const mm          = millimeter;
const zero        = 0 * mm;
var   idUniquer   = 0;

// == [Socket Cell Cutter] ==

/**
 * Feature: cylindrical socket pocket cut into a body on the selected plane.
 * Pocket diameter = socket wrench-end diameter + 2 × insertion gap; depth = 2 × layer height.
 * @param definition {{
 *      @field referencePlaneQ {Query} : Sketch plane.
 *      @field socketPath {LookupTablePath} : Socket selection from `SocketWrenches3`.
 *      @field layerHeight {ValueWithUnits} : FDM layer height; pocket depth is 2 × this.
 *      @field insertionGap {ValueWithUnits} : Radial clearance around the socket body.
 *      @field calloutAngle {ValueWithUnits} : Callout label angle (0–360°) from socket center.
 * }}
 */
annotation { "Feature Type Name": "Socket Cell Cutter" }
export const socketCellCutter = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name":  "Reference plane", "Filter":  QueryFilterCompound.ALLOWS_PLANE, "MaxNumberOfPicks":  1 }
  definition.referencePlaneQ is Query;

  annotation { "Name" : "Socket Selection", "Lookup Table" : SocketWrenches3 }
  definition.socketPath is LookupTablePath;

  annotation { "Name":  "Layer height" }
  isLength(definition.layerHeight,   {(millimeter) : [tinySizeVal, 4.55, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name":  "Insertion gap" }
  isLength(definition.insertionGap, {(millimeter) : [0, 0.3, hugeSizeVal]} as LengthBoundSpec);

  // Angular position of the callout label's closest point to the socket center.
  annotation { "Name":  "Callout angle" }
  isAngle(definition.calloutAngle, {(degree) : [0, 0, 360]} as AngleBoundSpec);
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

  const sketches = {
   bboxes:    newSketchOnPlane(context, ids.bboxesSk,    { "sketchPlane":  socketParams.sketchPlane }),
   shapes:    newSketchOnPlane(context, ids.shapesSk,    { "sketchPlane":  socketParams.sketchPlane }),
   labels:    newSketchOnPlane(context, ids.labelsSk,    { "sketchPlane":  socketParams.sketchPlane }),
   // Callout sketch X axis is the CW tangent at calloutAngle (= radial rotated −90°),
   // so text reads perpendicular to the ray and descends in Y when calloutAngle is 0.
   callouts:  rotatedSketch(context, ids.calloutsSk, socketParams, socketParams.calloutAngle - 90 * degree),
  };

  drawBoundingBoxes(context,   ids.bboxesSk, sketches.bboxes, socketParams);
  drawSocketBaseShape(context, ids.shapesSk, sketches.shapes, socketParams.socket, socketParams);
  skSolve(sketches.shapes);

  radialText(sketches.callouts, socketParams, "Hi, Mom", 4 * mm);
  skSolve(sketches.callouts);

//   drawAndMeasureText(context, ids.labelsSk, sketches.labels, "Hi, Mom", 5*mm, vector(1*mm, 3*mm));

  const cutoutFaces = qCreatedBy(ids.shapesSk, EntityType.FACE);
  opExtrude(context, ids.extrudedCutout, {
    "entities":  cutoutFaces,
    "direction": socketParams.sketchPlane.normal,
    "endBound":  BoundingType.BLIND,
    "endDepth":  socketParams.cutoutDepth,
  });

  setProperty(context, {
    "entities":     qCreatedBy(ids.extrudedCutout, EntityType.BODY),
    "propertyType": PropertyType.NAME,
    "value":        "Socket Cell Cutter",
  });
});
// --

// == [Radial Text] ==

/**
 * Callout label on `sketch` at `params.cutoutRadius`, centered on the H axis.
 * Sketch must be oriented with +V radially outward and +H as the CW tangent.
 * @param sketch {Sketch} : Target callout sketch.
 * @param params {map} : Socket cell params; uses `cutoutRadius`.
 * @param text {string} : Label text.
 * @param textHeight {ValueWithUnits} : Cap height.
 */
function radialText(sketch is Sketch, params is map, text is string, textHeight is ValueWithUnits) {
  const halfW       = textHeight * 0.65 * length(text);
  const innerRadius = params.cutoutRadius;
  skText(sketch, nextLabelId(params, "callout"), {
    "fontName":     "OpenSans-Regular.ttf",
    "firstCorner":  vector(-halfW,  innerRadius),
    "secondCorner": vector( halfW,  innerRadius + textHeight),
    "text":         text,
  });
}

// --

// == [Socket Data Helpers] ==

/**
 * Family entry from `SocketWrenches2` for `keypath`, stopping before the sizing tier.
 * @param context {Context} : Model context.
 * @param keypath {LookupTablePath} : Socket family path.
 */
function getSocketFamilyRef(context is Context, keypath is LookupTablePath) {
  return SocketWrenches2.entries[keypath.socket_kind].entries[keypath.drive_kind].entries[keypath.unit_system].entries[keypath.sqdrive_size].entries[keypath.reach_kind].entries[keypath.socket_variant];
}

/**
 * Dimension record from `SocketWrenches3` for the specific size at `keypath`.
 * @param context {Context} : Model context.
 * @param keypath {LookupTablePath} : Full socket path including sizing tier.
 */
function getSocketRef(context is Context, keypath is LookupTablePath) {
  return SocketWrenches3.entries[keypath.socket_kind].entries[keypath.drive_kind].entries[keypath.unit_system].entries[keypath.sqdrive_size].entries[keypath.reach_kind].entries[keypath.socket_variant].entries[keypath.sizing];
}
// --

/**
 * 2D coordinate in a sketch frame whose X axis is rotated `angle` from the original.
 * @param coord {Vector} : 2D coordinate in the original frame.
 * @param angle {ValueWithUnits} : X-axis rotation of the target frame.
 */
function toRotatedSketchCoord(coord is Vector, angle is ValueWithUnits) returns Vector {
  const h = coord[0];
  const v = coord[1];
  return vector(
     h * cos(angle) + v * sin(angle),
    -h * sin(angle) + v * cos(angle)
  );
}

/**
 * Sketch on `params.sketchPlane` with X axis rotated `angle` around the plane normal.
 * @param context {Context} : Model context.
 * @param id {Id} : Sketch feature id.
 * @param params {map} : Must contain `sketchPlane` (Plane).
 * @param angle {ValueWithUnits} : In-plane rotation angle.
 */
function rotatedSketch(context is Context, id is Id, params is map, angle is ValueWithUnits) returns Sketch {
//   const basePlane = evPlane(context, { "face": params.referencePlaneQ });
  const basePlane = params.sketchPlane;
  const rotatedX = cos(angle) * basePlane.x + sin(angle) * cross(basePlane.normal, basePlane.x);
  const rotatedPlane = plane(basePlane.origin, basePlane.normal, rotatedX);
  return newSketchOnPlane(context, id, { "sketchPlane": rotatedPlane });
}

/**
 * Nominal (construction) and cutout (solid) bounding rectangles on `sketch`,
 * with midpoint dots on the top and right edges of each.
 * @param context {Context} : Model context.
 * @param id {Id} : Sketch feature id.
 * @param sketch {Sketch} : Target sketch.
 * @param params {map} : Socket cell params; uses `nomBounds` and `cutBounds`.
 */
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

// // Returns a 2-vector from center along rayAngle to the given distance.
// function radialPoint(center is Vector, rayAngle, dist) returns Vector {
//   return center + dist * vector(cos(rayAngle), sin(rayAngle));
// }

// // Draws a text label on sketch whose baseline is perpendicular to rayAngle,
// // centered on the ray at (radius + offset) from center.
// function radialText(context is Context, params, sketch is Sketch, entityId is string,
//   text is string, center is Vector, rayAngle is ValueWithUnits, radius is ValueWithUnits, offset is ValueWithUnits, textHeight is ValueWithUnits) {
//   const pt      = radialPoint(center, rayAngle, radius + offset);
//   const perpDir = vector(-sin(rayAngle), cos(rayAngle));
//   const rayDir  = vector( cos(rayAngle), sin(rayAngle));
//   const halfW   = textHeight * 0.65 * length(text);
//   skText(sketch, nextLabelId(entityId, "radialText"), {
//     "fontName":      "OpenSans-Regular.ttf",
//     "firstCorner":   pt - halfW * perpDir,
//     "secondCorner":  pt + halfW * perpDir + textHeight * rayDir,
//     "text":          text,
//   });
// }

/**
 * Nominal wrench-end circle (construction) and cutout circle (solid) on `sketch`.
 * @param context {Context} : Model context.
 * @param id {Id} : Sketch feature id.
 * @param sketch {Sketch} : Target sketch.
 * @param socket {map} : Socket dimension record.
 * @param params {map} : Socket cell params; uses `nomBounds`, `bodyDiam`, `cutoutRadius`.
 */
function drawSocketBaseShape(context is Context, id is Id, sketch is Sketch, socket is map, params is map) {
  const center = vector(params.nomBounds.ctrH, params.nomBounds.ctrV);
  // Nominal wrench-end circle (construction)
  skCircle(sketch, "wrenchEndCircle", { "center": center, "radius": params.bodyDiam / 2, "construction": true,  });
  // Cutout circle = wrench-end + insertion gap all around (real region)
  skCircle(sketch, "cutoutCircle",    { "center": center, "radius": params.cutoutRadius });
}

/**
 * Base param map for a socket family: path, family record, gap, height, callout angle, and sketch plane.
 * @param context : Model context.
 * @param definition {map} : Raw feature definition.
 */
function socketFamilyCellParams(context, definition is map) returns map {
  // Look up the specific socket by sizing
  const socketPath   = definition.socketPath;
  const socketFamily = getSocketFamilyRef(context, socketPath);
  debug(context, [definition.socketPath]);
  if (socketFamily == undefined) { throw regenError("Unknown socket family '" ~ socketPath); }
  const sketchPlane = evPlane(context, { "face":  definition.referencePlaneQ });

  return mergeMaps(socketPath, {
    "socketPath":      socketPath,
    "socketFamily":    socketFamily,
    "insertionGap":    definition.insertionGap,
    "layerHeight":     definition.layerHeight,
    "calloutAngle":    definition.calloutAngle,
    "sketchPlane":     sketchPlane,
  });
}

/**
 * Full socket cell params, adding body diameter, cutout radius, cutout depth,
 * and bounding boxes to the family params.
 * @param context : Model context.
 * @param definition {map} : Raw feature definition.
 */
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
    "cutBounds":     cutBounds,
  });
}
