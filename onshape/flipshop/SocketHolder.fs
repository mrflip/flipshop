FeatureScript 2909;
import(path : "onshape/std/geometry.fs", version : "2909.0");
export import(path : "daa2f7d60ba23b30cdfc9d62", version : "5434ebe0d73f93454827045d");
export import(path : "4989999bb256f6d486ab7381", version : "ffdb0ffe5f74177ae1ad0ca3");

// SocketWrenches and SocketWrenchesByFamily are defined in SocketWrenches.fs
// (same Feature Studio document — no import needed)

// Sentinel values for LengthBoundSpec min/max when no practical limit applies.
const hugeSizeVal = 1000000;
const tinySizeVal = 0.001;
// Shorthand aliases used throughout for readability.
const mm          = millimeter;
const zero        = 0 * mm;
const callout1Angle = 140 * degree;
const callout2Angle =  40 * degree;
const socketCellPadding = { left: 4*mm, top: 3*mm };

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
  const ids = { bboxesSk: id + "bboxesSk", shapesSk: id + "shapesSk", labelsSk: id + "labelsSk", calloutsSk: id + "calloutsSk", extrudedCutout: id + "extrudedCutout" };

  const sketches = {
   bboxes:    newSketchOnPlane(context, ids.bboxesSk,    { "sketchPlane":  socketParams.basePlane }),
   shapes:    newSketchOnPlane(context, ids.shapesSk,    { "sketchPlane":  socketParams.basePlane }),
   labels:    newSketchOnPlane(context, ids.labelsSk,    { "sketchPlane":  socketParams.basePlane }),
   // Callout sketch X axis is the CW tangent at calloutAngle (= radial rotated −90°),
   // so text reads perpendicular to the ray and descends in Y when calloutAngle is 0.
   callouts:  rotatedSketch(context, ids.calloutsSk, socketParams, socketParams.calloutAngle - 90 * degree),
  };

  drawBoundingBoxes(context,   ids.bboxesSk, sketches.bboxes, socketParams);
  drawSocketBaseShape(context, ids.shapesSk, sketches.shapes, socketParams.socket, socketParams);
  skSolve(sketches.shapes);
  const shapesSkFacesQ = qCreatedBy(ids.shapesSk, EntityType.FACE);

  radialText(sketches.callouts, socketParams, "Hi, Mom", 4 * mm);
  skSolve(sketches.callouts);

//   drawAndMeasureText(context, ids.labelsSk, sketches.labels, "Hi, Mom", 5*mm, vector(1*mm, 3*mm));

  opExtrude(context, ids.extrudedCutout, {
    "entities":  shapesSkFacesQ,
    "direction": socketParams.basePlane.normal,
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

// /**
//  * 2D coordinate in a sketch frame whose X axis is rotated `angle` from the original.
//  * @param coord {Vector} : 2D coordinate in the original frame.
//  * @param angle {ValueWithUnits} : X-axis rotation of the target frame.
//  */
// function toRotatedSketchCoord(coord is Vector, angle is ValueWithUnits) returns Vector {
//   const h = coord[0];
//   const v = coord[1];
//   return vector(
//      h * cos(angle) + v * sin(angle),
//     -h * sin(angle) + v * cos(angle)
//   );
// }

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
  skCircle(sketch, "ratchetEndCircle", { "center": center, "radius": params.bodyDiam / 2, "construction": true,  });
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
  // debug(context, [definition.socketPath]);
  if (socketFamily == undefined) { throw regenError("Unknown socket family '" ~ socketPath); }
  const basePlane = evPlane(context, { "face":  definition.referencePlaneQ });

  return mergeMaps(socketPath, {
    "socketPath":      socketPath,
    "socketFamily":    socketFamily,
    "insertionGap":    definition.insertionGap,
    "layerHeight":     definition.layerHeight,
    "calloutAngle":    definition.calloutAngle,
    "basePlane":       basePlane,
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
  const bodyDiam     = (socket.ratchet_end_diam != undefined) ? socket.ratchet_end_diam : socket.wx_overall;
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

// == [Socket Cell Size] ==

/**
 * Axis-aligned bounding box (centered at origin) of a text chip placed tangentially at `angle`
 * on the padded circle.
 * @param tc {map} : Text bounds from @see `textBounds`; uses `actualWidth` and `actualHeight`.
 * @param paddedRadius {ValueWithUnits} : Distance from center to inner (baseline) edge of chip.
 * @param angle {ValueWithUnits} : Position angle (0 = 3 o'clock, 90 = 12 o'clock, CCW).
 */
function calloutChipBounds(tc is map, paddedRadius is ValueWithUnits, angle is ValueWithUnits) returns map {
  const hw   = tc.actualWidth / 2;
  const v0   = paddedRadius;
  const v1   = paddedRadius + tc.actualHeight;
  const sinA = sin(angle);
  const cosA = cos(angle);
  // Rotated-sketch frame: H = CW tangent = (sinA, -cosA), V = radial = (cosA, sinA)
  // World coord of (h, v): x = h*sinA + v*cosA,  y = -h*cosA + v*sinA
  const x00 = (-hw) * sinA + v0 * cosA;
  const x10 = ( hw) * sinA + v0 * cosA;
  const x01 = (-hw) * sinA + v1 * cosA;
  const x11 = ( hw) * sinA + v1 * cosA;
  const y00 =   hw  * cosA + v0 * sinA;
  const y10 = (-hw) * cosA + v0 * sinA;
  const y01 =   hw  * cosA + v1 * sinA;
  const y11 = (-hw) * cosA + v1 * sinA;
  return {
    "minH":  min(min(x00, x10), min(x01, x11)),
    "maxH":  max(max(x00, x10), max(x01, x11)),
    "minV":  min(min(y00, y10), min(y01, y11)),
    "maxV":  max(max(y00, y10), max(y01, y11)),
  };
}

/**
 * Sketch on `basePlane` with origin translated to `center` (local 2-D coords) and X axis
 * rotated by `angle`.
 * @param context {Context} : Model context.
 * @param id {Id} : Sketch feature id.
 * @param basePlane {Plane} : Reference plane.
 * @param center {Vector} : 2-D offset from `basePlane.origin` in the plane's local (H, V) frame.
 * @param angle {ValueWithUnits} : In-plane rotation of the new sketch's X axis.
 */
function rotatedSketchAt(context is Context, id is Id, basePlane is Plane, center is Vector, angle is ValueWithUnits) returns Sketch {
  const yAxis    = cross(basePlane.normal, basePlane.x);
  const newPlane = plane(basePlane.origin + center[0] * basePlane.x + center[1] * yAxis, basePlane.normal, basePlane.x);
  return rotatedSketch(context, id, { "basePlane":  newPlane }, angle);
}

/**
 * Shared text options for callout chips: font and baseline height.
 * Pass directly to @see `textBounds`; merge with alignment options for @see `skTextAt`.
 * @param opts {map} : Socket cell opts; uses `calloutHeight`.
 */
function calloutChipOpts(opts is map) returns map {
  return {
    "baselineHeight":  opts.calloutHeight,
    "fontName":        FontName.BEBAS_NEUE,
  };
}

/**
 * Geometry record for a single socket cell: radii, text extents, bounding boxes, grid-snapped dimensions.
 * @param context {Context} : Model context.
 * @param id {Id} : Id prefix for temporary text-measurement operations.
 * @param socket {map} : Socket dimension record from SocketWrenches3.
 * @param opts {map} :
 *   - @field insertionGap {ValueWithUnits} : Radial clearance between socket and cutout edge.
 *   - @field cutoutPadding {ValueWithUnits} : Gap from cutout edge to padded circle.
 *   - @field borderPadding {ValueWithUnits} : Gap from tight bounding box to cell border rectangle.
 *   - @field gridSize {ValueWithUnits} : Cell width/height snapped up to nearest multiple.
 *   - @field calloutHeight {ValueWithUnits} : Cap height of the callout chip text.
 *   - @field labelHeight {ValueWithUnits} : Cap height of the label chip text.
 */
function socketCellSize(context is Context, id is Id, socket is map, opts is map) returns map {
  const bodyDiam     = (socket.ratchet_end_diam != undefined) ? socket.ratchet_end_diam : socket.wx_overall;
  const bodyRadius   = bodyDiam / 2;
  const cutoutRadius = bodyRadius + opts.insertionGap;
  const paddedRadius = cutoutRadius + opts.cutoutPadding;

  // Nose and target-end diameters, falling back to body diameter when absent
  const noseDiam      = (socket.nose_diam      != undefined) ? socket.nose_diam      : bodyDiam;
  const targetDiam    = (socket.target_end_diam != undefined) ? socket.target_end_diam : bodyDiam;

  // Callout chips use targets.drives (chip 1 at 120°) and targets.fhcs_sz (chip 2 at 210°)
  const targets     = socket.targets;
  const calloutText = ((targets != undefined) && (targets.drives  != undefined)) ? replace(targets.drives,  '(in|mm)$', '')  : "-";
  const fhcsText    = ((targets != undefined) && (targets.fhcs_sz != undefined)) ? replace(targets.fhcs_sz, '(in|mm)$', '')  : "-";
  const labelText   = replace(socket.sizing, '(in|mm)$', '');
  //   debug(context, ["socketCellSize", calloutText, fhcsText, labelText, socket, targets]);


  // Callout chip 1 at 120° (60° above left horizon)
  var calloutCbMinH = zero; var calloutCbMaxH = zero; var calloutCbMinV = zero; var calloutCbMaxV = zero;
  if (calloutText != "") {
    const calloutTc = textBounds(context, id + "calloutTC", calloutText, calloutChipOpts(opts));
    const calloutCb = calloutChipBounds(calloutTc, paddedRadius, callout1Angle);
    calloutCbMinH = calloutCb.minH;
    calloutCbMaxH = calloutCb.maxH;
    calloutCbMinV = calloutCb.minV;
    calloutCbMaxV = calloutCb.maxV;
  }
  // Callout chip 2 at 210° (30° below left horizon)
  var fhcsCbMinH = zero; var fhcsCbMaxH = zero; var fhcsCbMinV = zero; var fhcsCbMaxV = zero;
  if (fhcsText != "") {
    const fhcsTc = textBounds(context, id + "fhcsTC", fhcsText, calloutChipOpts(opts));
    const fhcsCb = calloutChipBounds(fhcsTc, paddedRadius, callout2Angle);
    fhcsCbMinH = fhcsCb.minH;
    fhcsCbMaxH = fhcsCb.maxH;
    fhcsCbMinV = fhcsCb.minV;
    fhcsCbMaxV = fhcsCb.maxV;
  }

  const labelTc    = textBounds(context, id + "labelTC", labelText, { "baselineHeight":  opts.labelHeight });
  // Label chip: TOP_EXTENT at y=-paddedRadius (6 o'clock), horizontally centered at x=0
  const labelHalfW = labelTc.actualWidth / 2;
  const labelMinH  = -labelHalfW;
  const labelMaxH  =  labelHalfW;
  const labelMinV  = -paddedRadius - labelTc.actualHeight;
  const labelMaxV  = -paddedRadius;

  // Tight bounding box over padded circle + callout chips + label chip +
  //   socketCellPadding sentinel extents (body circle + its padding lines)
  const bboxMinH = min(min(min(min(-paddedRadius, calloutCbMinH), fhcsCbMinH), labelMinH),
                       -(bodyRadius + socketCellPadding.left));
  const bboxMaxH = max(max(max( paddedRadius, calloutCbMaxH), fhcsCbMaxH), labelMaxH);
  const bboxMinV = min(min(min(-paddedRadius, calloutCbMinV), fhcsCbMinV), labelMinV);
  const bboxMaxV = max(max(max(max( paddedRadius, calloutCbMaxV), fhcsCbMaxV), labelMaxV),
                       bodyRadius + socketCellPadding.top);

  // Border = tight bounding box expanded by borderPadding on all sides
  const borderMinH = bboxMinH - opts.borderPadding;
  const borderMaxH = bboxMaxH + opts.borderPadding;
  const borderMinV = bboxMinV - opts.borderPadding;
  const borderMaxV = bboxMaxV + opts.borderPadding;

  // Snap up to nearest gridSize multiple.
  // Horizontal: center the extra space (split evenly left/right).
  // Vertical: extra space grows upward only — bottom edge stays at borderMinV for a stable baseline.
  const rawWidth   = borderMaxH - borderMinH;
  const rawHeight  = borderMaxV - borderMinV;
  const cellWidth  = ceil(rawWidth  / opts.gridSize) * opts.gridSize;
  const cellHeight = ceil(rawHeight / opts.gridSize) * opts.gridSize;
  const cellMinH   = borderMinH - (cellWidth  - rawWidth)  / 2;
  const cellMaxH   = borderMaxH + (cellWidth  - rawWidth)  / 2;
  const cellMinV   = borderMinV;
  const cellMaxV   = borderMaxV + (cellHeight - rawHeight);

  return {
    "bodyDiam":      bodyDiam,
    "cutoutRadius":  cutoutRadius,
    "paddedRadius":  paddedRadius,
    "noseDiam":      noseDiam,
    "targetDiam":    targetDiam,
    "calloutText":   calloutText,
    "fhcsText":      fhcsText,
    "labelText":     labelText,
    "bboxMinH":      bboxMinH,   "bboxMaxH":   bboxMaxH,
    "bboxMinV":      bboxMinV,   "bboxMaxV":   bboxMaxV,
    "cellMinH":      cellMinH,   "cellMaxH":   cellMaxH,
    "cellMinV":      cellMinV,   "cellMaxV":   cellMaxV,
    "cellWidth":     cellWidth,  "cellHeight": cellHeight,
  };
}
// --

// == [Socket Cell] ==

/**
 * Single holder cell for `socket` on `opts.basePlane`, bottom-aligned to `basePoint`.
 * Creates five sketches (cutout, decoration, callout×2, label), extrudes a solid cell body,
 * and cuts a pocket for the socket. Returns the geometry from @see `socketCellSize`.
 * @param context {Context} : Model context.
 * @param id {Id} : Unique id prefix for all sketch and extrude operations.
 * @param socket {map} : Socket dimension record.
 * @param opts {map} : Options for @see `socketCellSize`, plus:
 *   - @field basePlane {Plane} : Drawing plane; sketch origin is the coordinate origin.
 *   - @field layerHeight {ValueWithUnits} : Pocket depth = 2 × this.
 *   - @field holderDepth {ValueWithUnits} : Full cell body thickness.
 * @param basePoint {Vector} : Bottom-left corner of the cell rectangle in `basePlane` local coords.
 *   The socket center is at `(-cs.cellMinH, -cs.cellMinV)` relative to this point.
 */
function socketCell(context is Context, id is Id, socket is map, opts is map, basePoint is Vector) returns map {
  const cs = socketCellSize(context, id + "sz", socket, opts);
  const cx = basePoint[0] - cs.cellMinH;
  const cy = basePoint[1] - cs.cellMinV;

  const ids = { cutoutSk: id + "cutoutSk", decoSk: id + "decoSk", calloutSk1: id + "calloutSk1", calloutSk2: id + "calloutSk2", labelSk: id + "labelSk", plate: id + "plate", pocketTool: id + "pocketTool", pocketCut: id + "pocketCut" };
  const sketches = {
    cutout:   newSketchOnPlane(context, ids.cutoutSk,  { "sketchPlane":  opts.basePlane }),
    deco:     newSketchOnPlane(context, ids.decoSk,    { "sketchPlane":  opts.basePlane }),
    // Rotated callout sketches: V axis points radially outward, so text at (0, paddedRadius) is tangent to padded circle
    callout1: rotatedSketchAt(context, ids.calloutSk1, opts.basePlane, vector(cx, cy), callout1Angle - 90 * degree),
    callout2: rotatedSketchAt(context, ids.calloutSk2, opts.basePlane, vector(cx, cy), callout2Angle - 90 * degree),
    label:    newSketchOnPlane(context, ids.labelSk,   { "sketchPlane":  opts.basePlane }),
  };

  // Cutout sketch — solid circle only; face is extruded into the pocket tool
  skCircle(sketches.cutout, "cutout", { "center": vector(cx, cy), "radius":  cs.cutoutRadius });
  skSolve(sketches.cutout);
  const cutoutSkFacesQ = qCreatedBy(ids.cutoutSk, EntityType.FACE);

  // Decoration sketch — padded circle (construction), ratchet-end and nose circles (construction),
  //   bbox (construction), cell rectangle (solid — extruded into the holder body)
  skCircle(sketches.deco, "padded", {
    "center":       vector(cx, cy),
    "radius":       cs.paddedRadius,
    "construction": true,
  });
  skCircle(sketches.deco, "targetEndCircle", {
    "center":       vector(cx, cy),
    "radius":       cs.targetDiam / 2,
    "construction": true,
  });
  skPoint(sketches.deco, "targetEndDot", { "position":  vector(cx, cy + cs.targetDiam / 2) });
  skCircle(sketches.deco, "noseCircle", {
    "center":       vector(cx, cy),
    "radius":       cs.noseDiam / 2,
    "construction": true,
  });
  skPoint(sketches.deco, "noseDot", { "position":  vector(cx, cy - cs.noseDiam / 2) });
  // socketCellPadding indicators: construction lines at 9 o'clock and 12 o'clock on the body circle
  const bodyRadius = cs.bodyDiam / 2;
  skLineSegment(sketches.deco, "padLeft", {
    "start":        vector(cx - bodyRadius,                              cy),
    "end":          vector(cx - bodyRadius - socketCellPadding.left,    cy),
    "construction": true,
  });
  skLineSegment(sketches.deco, "padTop", {
    "start":        vector(cx, cy + bodyRadius),
    "end":          vector(cx, cy + bodyRadius + socketCellPadding.top),
    "construction": true,
  });
  skRectangle(sketches.deco, "bbox", {
    "firstCorner":  vector(cx + cs.bboxMinH, cy + cs.bboxMinV),
    "secondCorner": vector(cx + cs.bboxMaxH, cy + cs.bboxMaxV),
    "construction": true,
  });
  skRectangle(sketches.deco, "cell", {
    "firstCorner":  vector(cx + cs.cellMinH, cy + cs.cellMinV),
    "secondCorner": vector(cx + cs.cellMaxH, cy + cs.cellMaxV),
  });
  skSolve(sketches.deco);
  const decoSkFacesQ = qCreatedBy(ids.decoSk, EntityType.FACE);

  // Callout sketch 1 — targets.drives; baseline tangent to padded circle at 120° (60° above left horizon)
  if (cs.calloutText != "") {
    skTextAt(context, id + "calloutTxt1", "callout1", sketches.callout1, cs.calloutText,
      vector(0 * mm, cs.paddedRadius),
      opts.calloutHeight, mergeMaps(calloutChipOpts(opts), {
        "horizontalAlign":  HorizontalAlignment.CENTER,
        "verticalAlign":    VerticalAlignment.BOTTOM_BASELINE,
      }));
  }
  skSolve(sketches.callout1);

  // Callout sketch 2 — targets.fhcs_sz; baseline tangent to padded circle at 210° (30° below left horizon)
  if (cs.fhcsText != "") {
    skTextAt(context, id + "calloutTxt2", "callout2", sketches.callout2, cs.fhcsText,
      vector(0 * mm, cs.paddedRadius),
      opts.calloutHeight, mergeMaps(calloutChipOpts(opts), {
        "horizontalAlign":  HorizontalAlignment.CENTER,
        "verticalAlign":    VerticalAlignment.BOTTOM_BASELINE,
      }));
  }
  skSolve(sketches.callout2);

  // Label sketch — sizing text; TOP_EXTENT tangent to padded circle at 6 o'clock, centered
  skTextAt(context, id + "labelTxt", "label", sketches.label, cs.labelText,
    vector(cx, cy - cs.paddedRadius),
    opts.labelHeight, {
      "horizontalAlign":    HorizontalAlignment.CENTER,
      "verticalAlign":      VerticalAlignment.TOP_EXTENT,
    });
  skSolve(sketches.label);

  // Extrude cell body from border rectangle face
  opExtrude(context, ids.plate, {
    "entities":  decoSkFacesQ,
    "direction": opts.basePlane.normal,
    "endBound":  BoundingType.BLIND,
    "endDepth":  opts.holderDepth,
  });

  // Extrude pocket tool from cutout circle, then subtract from cell body
  opExtrude(context, ids.pocketTool, {
    "entities":  cutoutSkFacesQ,
    "direction": opts.basePlane.normal,
    "endBound":  BoundingType.BLIND,
    "endDepth":  2 * opts.layerHeight,
  });
  opBoolean(context, ids.pocketCut, {
    "tools":          qCreatedBy(ids.pocketTool, EntityType.BODY),
    "targets":        qCreatedBy(ids.plate,      EntityType.BODY),
    "operationType":  BooleanOperationType.SUBTRACTION,
  });

  setProperty(context, {
    "entities":     qCreatedBy(ids.plate, EntityType.BODY),
    "propertyType": PropertyType.NAME,
    "value":        socket.title,
  });

  return cs;
}
// --

// == [Socket Holder] ==

/**
 * Holder cells for every socket in `familyRef`, arranged left-to-right on `opts.basePlane`.
 * @param context {Context} : Model context.
 * @param id {Id} : Unique id prefix.
 * @param familyRef {array} : Socket records from @see `getSocketFamilyRef`.
 * @param opts {map} : Options for @see `socketCell`, plus:
 *   - @field omitSockets {array} : Sizing keys or loop-index strings to skip.
 *   - @field maxTotalWidth {ValueWithUnits} : Stop before starting a new cell once cursor reaches this.
 */
function socketHolder(context is Context, id is Id, familyRef is array, opts is map) {
  var cursorX = zero;

  for (var i = 0; i < size(familyRef); i += 1) {
    if (cursorX >= opts.maxTotalWidth) { break; }
    const socketRecord = familyRef[i];
    if (isIn(toString(i), opts.omitSockets) || isIn(socketRecord.sizing, opts.omitSockets)) { continue; }
    // Id is index-based so part identities survive family changes.
    const basePoint = vector(cursorX, zero);
    const cs = socketCell(context, id + ("c" ~ toString(i)), socketRecord, opts, basePoint);
    cursorX += cs.cellWidth;
  }
}
// --

// == [Socket Holder Part] ==

/**
 * Part feature: holder cells for every socket in a selected family.
 * @param definition {{
 *      @field referencePlaneQ {Query} : Top face of the holder; sketches are drawn here.
 *      @field familyPath {LookupTablePath} : Socket family from SocketWrenches2.
 *      @field holderDepth {ValueWithUnits} : Holder plate thickness.
 *      @field layerHeight {ValueWithUnits} : FDM layer height; pocket depth = 2 × this.
 *      @field insertionGap {ValueWithUnits} : Radial clearance between socket and cutout.
 *      @field cutoutPadding {ValueWithUnits} : Gap from cutout edge to padded circle.
 *      @field borderPadding {ValueWithUnits} : Gap from bounding box to cell border rectangle.
 *      @field gridSize {ValueWithUnits} : Cell width/height snapped to this multiple.
 *      @field calloutHeight {ValueWithUnits} : Cap height of callout text chip.
 *      @field labelHeight {ValueWithUnits} : Cap height of label text chip.
 *      @field omitSockets {string} : Comma-separated sizing keys or loop indices to skip.
 *      @field maxTotalWidth {ValueWithUnits} : Stop adding cells once cursor reaches this (0 = no limit).
 *      @field mergeCells {boolean} : When true, union all cell bodies into one and name it for the family.
 * }}
 */
annotation { "Feature Type Name": "Socket Holder" }
export const socketHolderPart = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Reference plane", "Filter": QueryFilterCompound.ALLOWS_PLANE, "MaxNumberOfPicks": 1 }
  definition.referencePlaneQ is Query;

  annotation { "Name": "Socket Family", "Lookup Table": SocketWrenches2 }
  definition.familyPath is LookupTablePath;

  annotation { "Name": "Holder depth", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.holderDepth, { (millimeter): [tinySizeVal, 10, hugeSizeVal] } as LengthBoundSpec);

  annotation { "Name": "Layer height", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.layerHeight, { (millimeter): [tinySizeVal, 0.2, hugeSizeVal] } as LengthBoundSpec);

  annotation { "Name": "Insertion gap", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.insertionGap, { (millimeter): [0, 0.3, hugeSizeVal] } as LengthBoundSpec);

  annotation { "Name": "Cutout padding", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.cutoutPadding, { (millimeter): [0, 0.5, hugeSizeVal] } as LengthBoundSpec);

  annotation { "Name": "Border padding", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.borderPadding, { (millimeter): [0, 1, hugeSizeVal] } as LengthBoundSpec);

  annotation { "Name": "Grid size", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.gridSize, { (millimeter): [tinySizeVal, 5, hugeSizeVal] } as LengthBoundSpec);

  annotation { "Name": "Callout height", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.calloutHeight, { (millimeter): [tinySizeVal, 4, hugeSizeVal] } as LengthBoundSpec);

  annotation { "Name": "Label height", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.labelHeight, { (millimeter): [tinySizeVal, 3, hugeSizeVal] } as LengthBoundSpec);

  annotation { "Name": "Omit sockets", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  definition.omitSockets is string;

  annotation { "Name": "Max total width", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.maxTotalWidth, { (millimeter): [0, 0, hugeSizeVal] } as LengthBoundSpec);

  annotation { "Name": "Merge cells", "Default": true }
  definition.mergeCells is boolean;
}
{
  const basePlane = evPlane(context, { "face":  definition.referencePlaneQ });
  // a map sizing key to socket -- eg `{ "7/16": {...}, "1/2": { ... } ... }`
  const familyRef   = getSocketFamilyRef(context, definition.familyPath);
  // debug(context, ["socketHolderPart", familyRef], DebugColor.MAGENTA);
  if (familyRef == undefined) { throw regenError("Unknown socket family"); }

  const omitSockets   = (definition.omitSockets == "") ? [] : splitByRegexp(definition.omitSockets, ",\\s*");
  const maxTotalWidth = (definition.maxTotalWidth < tinySizeVal * mm) ? hugeSizeVal * mm : definition.maxTotalWidth;
  // Family title: strip the sizing prefix from the first socket's title ("2mm Int Hex…" → "Int Hex…")
  const familyTitle   = replace(familyRef[0].title, "^" ~ familyRef[0].sizing ~ "\\s+", "");

  socketHolder(context, id, familyRef, {
    "basePlane":      basePlane,
    "holderDepth":    definition.holderDepth,
    "layerHeight":    definition.layerHeight,
    "insertionGap":   definition.insertionGap,
    "cutoutPadding":  definition.cutoutPadding,
    "borderPadding":  definition.borderPadding,
    "gridSize":       definition.gridSize,
    "calloutHeight":  definition.calloutHeight,
    "labelHeight":    definition.labelHeight,
    "omitSockets":    omitSockets,
    "maxTotalWidth":  maxTotalWidth,
  });

  if (definition.mergeCells) {
    const allCellsQ = qCreatedBy(id, EntityType.BODY);
    opBoolean(context, id + "merge", {
      "tools":          allCellsQ,
      "operationType":  BooleanOperationType.UNION,
    });
    setProperty(context, {
      "entities":     qCreatedBy(id + "merge", EntityType.BODY),
      "propertyType": PropertyType.NAME,
      "value":        familyTitle,
    });
  }
});
// --
