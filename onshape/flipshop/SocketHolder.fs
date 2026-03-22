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

/**
 * Feature: cuts a cylindrical socket pocket into a body on the selected reference plane.
 * The pocket is a circle sized to the chosen socket's wrench-end diameter plus the
 * insertion gap, extruded two layer-heights deep.
 * @param definition {{
 *      @field referencePlaneQ {Query} : The plane on which the socket cell is sketched.
 *      @field socketPath {LookupTablePath} : Socket selection from the `SocketWrenches3` lookup table.
 *      @field layerHeight {ValueWithUnits} : FDM layer height; pocket depth is 2 × this value.
 *      @field insertionGap {ValueWithUnits} : Clearance added to the socket body radius to size the cutout.
 *      @field calloutAngle {ValueWithUnits} : Angular position (0–360°) of the callout label relative to the socket center.
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

/**
 * Draws a text label on the callout sketch anchored at `params.cutoutRadius` in the +V
 * direction (radially outward) and centered on the H axis (tangentially).
 * The callout sketch must already be oriented so +V is radially outward and +H is the
 * CW tangent — i.e., created with `rotatedSketch` at `calloutAngle − 90°`.
 * Character width is estimated at 0.65 × `textHeight`.
 * @param sketch {Sketch} : The callout sketch to draw into.
 * @param params {map} : Socket cell params; `cutoutRadius` sets the inner radius of the label.
 * @param text {string} : The label text to draw.
 * @param textHeight {ValueWithUnits} : Font size (baseline-to-cap height).
 */
function radialText(sketch is Sketch, params is map, text is string, textHeight is ValueWithUnits) {
  const halfW       = textHeight * 0.65 * length(text);
  const innerRadius = params.cutoutRadius;
  skText(sketch, nextLabelId("callout"), {
    "fontName":     "OpenSans-Regular.ttf",
    "firstCorner":  vector(-halfW,  innerRadius),
    "secondCorner": vector( halfW,  innerRadius + textHeight),
    "text":         text,
  });
}

/**
 * Returns the lookup-table entry for a socket family (all sizes) identified by `keypath`.
 * Traverses `SocketWrenches2` to the `socket_variant` tier, stopping before the sizing level.
 * @param context {Context} : The model context (unused, reserved for future use).
 * @param keypath {LookupTablePath} : Lookup table path identifying the socket family.
 */
function getSocketFamilyRef(context is Context, keypath is LookupTablePath) {
  return SocketWrenches2.entries[keypath.socket_kind].entries[keypath.drive_kind].entries[keypath.unit_system].entries[keypath.sqdrive_size].entries[keypath.reach_kind].entries[keypath.socket_variant];
}

/**
 * Returns the lookup-table entry for a specific socket size identified by `keypath`.
 * Traverses `SocketWrenches3` to the `sizing` tier, returning the full dimension record
 * with fields such as `wrench_end_diam` and `wx_overall`.
 * @param context {Context} : The model context (unused, reserved for future use).
 * @param keypath {LookupTablePath} : Lookup table path identifying the socket and size.
 */
function getSocketRef(context is Context, keypath is LookupTablePath) {
  return SocketWrenches3.entries[keypath.socket_kind].entries[keypath.drive_kind].entries[keypath.unit_system].entries[keypath.sqdrive_size].entries[keypath.reach_kind].entries[keypath.socket_variant].entries[keypath.sizing];
}

/**
 * Converts a 2D sketch coordinate to the equivalent coordinate in a sketch whose X axis
 * is rotated by `angle` relative to the original.  Both sketches share the same origin
 * and normal; the conversion is a 2D rotation of `coord` by `-angle`.
 * @param coord {Vector} : 2D coordinate in the original sketch frame.
 * @param angle {ValueWithUnits} : Rotation angle of the target sketch's X axis.
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
 * Creates a new sketch on the same plane as `params.sketchPlane` but with its X axis
 * rotated by `angle` around the plane normal.  Used to draw label text at an independent
 * angle from the main geometry sketches.
 * @param context {Context} : The model context.
 * @param id {Id} : Sketch feature id.
 * @param params {map} : Must contain `sketchPlane` (Plane).
 * @param angle {ValueWithUnits} : In-plane rotation angle for the sketch X axis.
 */
function rotatedSketch(context is Context, id is Id, params is map, angle is ValueWithUnits) returns Sketch {
//   const basePlane = evPlane(context, { "face": params.referencePlaneQ });
  const basePlane = params.sketchPlane;
  const rotatedX = cos(angle) * basePlane.x + sin(angle) * cross(basePlane.normal, basePlane.x);
  const rotatedPlane = plane(basePlane.origin, basePlane.normal, rotatedX);
  return newSketchOnPlane(context, id, { "sketchPlane": rotatedPlane });
}

/**
 * Draws two bounding rectangles onto `sketch`: the nominal box (construction, sized to
 * the wrench-end diameter) and the cutout box (solid, extended by the insertion gap on
 * all sides).  Adds midpoint dots on the top and right edges of each rectangle.  Solves
 * and returns the sketch.
 * @param context {Context} : The model context (unused, reserved for future use).
 * @param id {Id} : Sketch feature id (unused directly; sketch was created by the caller).
 * @param sketch {Sketch} : The sketch to draw into.
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
//   skText(sketch, nextLabelId(entityId), {
//     "fontName":      "OpenSans-Regular.ttf",
//     "firstCorner":   pt - halfW * perpDir,
//     "secondCorner":  pt + halfW * perpDir + textHeight * rayDir,
//     "text":          text,
//   });
// }

/**
 * Draws two concentric circles onto `sketch` defining the socket cell profile.
 * The nominal wrench-end circle (construction) shows the socket body diameter; the cutout
 * circle (solid, larger by `insertionGap`) is the region extruded to form the pocket.
 * @param context {Context} : The model context (unused, reserved for future use).
 * @param id {Id} : Sketch feature id (unused directly).
 * @param sketch {Sketch} : The sketch to draw into.
 * @param socket {map} : Socket dimension record (unused; sizing comes from `params`).
 * @param params {map} : Socket cell params; uses `nomBounds.ctrH/ctrV`, `bodyDiam`, `cutoutRadius`.
 */
function drawSocketBaseShape(context is Context, id is Id, sketch is Sketch, socket is map, params is map) {
  const center = vector(params.nomBounds.ctrH, params.nomBounds.ctrV);
  // Nominal wrench-end circle (construction)
  skCircle(sketch, "wrenchEndCircle", { "center": center, "radius": params.bodyDiam / 2, "construction": true,  });
  // Cutout circle = wrench-end + insertion gap all around (real region)
  skCircle(sketch, "cutoutCircle",    { "center": center, "radius": params.cutoutRadius });
}

/**
 * Looks up the socket family in `SocketWrenches2` and builds the base parameter map
 * shared by all sizes in that family.  Throws a regen error if the family is not found.
 * Returns a map merging the lookup-table key fields with `socketPath`, `socketFamily`,
 * `insertionGap`, `layerHeight`, `calloutAngle`, and `sketchPlane`.
 * @param context : The model context (used for `evPlane`).
 * @param definition {map} : The raw feature definition map.
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
 * Extends `socketFamilyCellParams` with geometry for the specific socket size: body
 * diameter (from `wrench_end_diam` if present, else `wx_overall`), cutout radius
 * (body radius + `insertionGap`), cutout depth (2 × `layerHeight`), and pre-computed
 * nominal and cutout bounding boxes centered at the sketch origin.
 * @param context : The model context (passed through to `socketFamilyCellParams`).
 * @param definition {map} : The raw feature definition map.
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

/**
 * Returns a unique sketch entity id string by appending a monotonically increasing
 * counter to `label`, using the module-level `idUniquer` variable.
 * @param label {string} : Base label prefix.
 */
function nextLabelId(label is string) returns string { return label ~ "_" ~ (idUniquer++); }