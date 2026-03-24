FeatureScript 2909;
import(path : "onshape/std/geometry.fs", version : "2909.0");
export import(path : "daa2f7d60ba23b30cdfc9d62", version : "146bbe88c26a4a3a41774202");
export import(path : "4989999bb256f6d486ab7381", version : "75b239924e285228035fc1a4");

// SocketWrenches and SocketWrenchesByFamily are defined in SocketWrenches.fs
// (same Feature Studio document — no import needed)

// Sentinel values for LengthBoundSpec min/max when no practical limit applies.
const hugeSizeVal = 1000000;
const tinySizeVal = 0.001;
// Shorthand aliases used throughout for readability.
const mm          = millimeter;
const zero        = 0 * mm;

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

  radialText(sketches.callouts, socketParams, "Hi, Mom", 4 * mm);
  skSolve(sketches.callouts);

//   drawAndMeasureText(context, ids.labelsSk, sketches.labels, "Hi, Mom", 5*mm, vector(1*mm, 3*mm));

  const cutoutFaces = qCreatedBy(ids.shapesSk, EntityType.FACE);
  opExtrude(context, ids.extrudedCutout, {
    "entities":  cutoutFaces,
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

// == [Socket Cell Size] ==

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
  const bodyDiam     = (socket.wrench_end_diam != undefined) ? socket.wrench_end_diam : socket.wx_overall;
  const cutoutRadius = bodyDiam / 2 + opts.insertionGap;
  const paddedRadius = cutoutRadius + opts.cutoutPadding;

  const calloutText = socket.sizing_mm_text;
  const labelText   = socket.sizing;
  const calloutTc   = textBounds(context, id + "calloutTC", calloutText, { "baselineHeight":  opts.calloutHeight });
  const labelTc     = textBounds(context, id + "labelTC",   labelText,   { "baselineHeight":  opts.labelHeight  });

  // Callout chip: BOTTOM_EXTENT at y=0, right edge tangent to padded circle at 9 o'clock (left)
  const calloutMinH = -paddedRadius - calloutTc.actualWidth;
  const calloutMaxH = -paddedRadius;
  const calloutMinV = zero;
  const calloutMaxV = calloutTc.bbox.maxCorner[1];

  // Label chip: TOP_EXTENT at y=-paddedRadius (6 o'clock), horizontally centered at x=0
  const labelHalfW = labelTc.actualWidth / 2;
  const labelMinH  = -labelHalfW;
  const labelMaxH  =  labelHalfW;
  const labelMinV  = -paddedRadius - labelTc.actualHeight;
  const labelMaxV  = -paddedRadius;

  // Tight bounding box over padded circle + callout chip + label chip
  const bboxMinH = min(min(-paddedRadius, calloutMinH), labelMinH);
  const bboxMaxH = max(max( paddedRadius, calloutMaxH), labelMaxH);
  const bboxMinV = min(min(-paddedRadius, calloutMinV), labelMinV);
  const bboxMaxV = max(max( paddedRadius, calloutMaxV), labelMaxV);

  // Cell border = tight bounding box expanded by borderPadding on all sides
  const borderMinH = bboxMinH - opts.borderPadding;
  const borderMaxH = bboxMaxH + opts.borderPadding;
  const borderMinV = bboxMinV - opts.borderPadding;
  const borderMaxV = bboxMaxV + opts.borderPadding;

  // Snap cell width and height up to nearest gridSize multiple
  const rawWidth   = borderMaxH - borderMinH;
  const rawHeight  = borderMaxV - borderMinV;
  const cellWidth  = ceil(rawWidth  / opts.gridSize) * opts.gridSize;
  const cellHeight = ceil(rawHeight / opts.gridSize) * opts.gridSize;

  return {
    "bodyDiam":      bodyDiam,
    "cutoutRadius":  cutoutRadius,
    "paddedRadius":  paddedRadius,
    "calloutText":   calloutText,
    "labelText":     labelText,
    "bboxMinH":      bboxMinH,   "bboxMaxH":   bboxMaxH,
    "bboxMinV":      bboxMinV,   "bboxMaxV":   bboxMaxV,
    "borderMinH":    borderMinH, "borderMaxH": borderMaxH,
    "borderMinV":    borderMinV, "borderMaxV": borderMaxV,
    "cellWidth":     cellWidth,  "cellHeight": cellHeight,
  };
}
// --

// == [Socket Cell] ==

/**
 * Single holder cell for `socket`, centered at `center` on `opts.basePlane`.
 * Creates four sketches (cutout, decoration, callout, label), extrudes a solid cell body,
 * and cuts a pocket for the socket. Returns the geometry from @see `socketCellSize`.
 * @param context {Context} : Model context.
 * @param id {Id} : Unique id prefix for all sketch and extrude operations.
 * @param socket {map} : Socket dimension record.
 * @param opts {map} : Options for @see `socketCellSize`, plus:
 *   - @field basePlane {Plane} : Drawing plane; sketch origin is the coordinate origin.
 *   - @field layerHeight {ValueWithUnits} : Pocket depth = 2 × this.
 *   - @field holderDepth {ValueWithUnits} : Full cell body thickness.
 * @param center {Vector} : 2-D sketch-plane coordinate of the socket circle center.
 */
function socketCell(context is Context, id is Id, socket is map, opts is map, center is Vector) returns map {
  const cs = socketCellSize(context, id + "sz", socket, opts);
  const cx = center[0];
  const cy = center[1];

  // Cutout sketch — solid circle only; face is extruded into the pocket tool
  const cutoutSkId = id + "cutoutSk";
  const cutoutSk   = newSketchOnPlane(context, cutoutSkId, { "sketchPlane":  opts.basePlane });
  skCircle(cutoutSk, "cutout", { "center": vector(cx, cy), "radius":  cs.cutoutRadius });
  skSolve(cutoutSk);

  // Decoration sketch — padded circle (construction), bbox (construction), border (solid)
  const decoSkId = id + "decoSk";
  const decoSk   = newSketchOnPlane(context, decoSkId, { "sketchPlane":  opts.basePlane });
  skCircle(decoSk, "padded", {
    "center":       vector(cx, cy),
    "radius":       cs.paddedRadius,
    "construction": true,
  });
  skRectangle(decoSk, "bbox", {
    "firstCorner":  vector(cx + cs.bboxMinH, cy + cs.bboxMinV),
    "secondCorner": vector(cx + cs.bboxMaxH, cy + cs.bboxMaxV),
    "construction": true,
  });
  skRectangle(decoSk, "border", {
    "firstCorner":  vector(cx + cs.borderMinH, cy + cs.borderMinV),
    "secondCorner": vector(cx + cs.borderMaxH, cy + cs.borderMaxV),
  });
  skSolve(decoSk);

  // Callout sketch — sizing_mm_text; BOTTOM_EXTENT tangent to padded circle at 9 o'clock
  const calloutSkId = id + "calloutSk";
  const calloutSk   = newSketchOnPlane(context, calloutSkId, { "sketchPlane":  opts.basePlane });
  skTextAt(context, id + "calloutTxt", "callout", calloutSk, cs.calloutText,
    vector(cx - cs.paddedRadius, cy),
    opts.calloutHeight, {
      "horizontalAlign":  HorizontalAlignment.RIGHT,
      "verticalAlign":    VerticalAlignment.BOTTOM_EXTENT,
    });
  skSolve(calloutSk);

  // Label sketch — sizing text; TOP_EXTENT tangent to padded circle at 6 o'clock, centered
  const labelSkId = id + "labelSk";
  const labelSk   = newSketchOnPlane(context, labelSkId, { "sketchPlane":  opts.basePlane });
  skTextAt(context, id + "labelTxt", "label", labelSk, cs.labelText,
    vector(cx, cy - cs.paddedRadius),
    opts.labelHeight, {
      "horizontalAlign":  HorizontalAlignment.CENTER,
      "verticalAlign":    VerticalAlignment.TOP_EXTENT,
    });
  skSolve(labelSk);

  // Extrude cell body from border rectangle face
  opExtrude(context, id + "plate", {
    "entities":  qCreatedBy(decoSkId, EntityType.FACE),
    "direction": opts.basePlane.normal,
    "endBound":  BoundingType.BLIND,
    "endDepth":  opts.holderDepth,
  });

  // Extrude pocket tool from cutout circle, then subtract from cell body
  opExtrude(context, id + "pocketTool", {
    "entities":  qCreatedBy(cutoutSkId, EntityType.FACE),
    "direction": opts.basePlane.normal,
    "endBound":  BoundingType.BLIND,
    "endDepth":  2 * opts.layerHeight,
  });
  opBoolean(context, id + "pocketCut", {
    "tools":          qCreatedBy(id + "pocketTool", EntityType.BODY),
    "targets":        qCreatedBy(id + "plate",      EntityType.BODY),
    "operationType":  BooleanOperationType.SUBTRACTION,
  });

  setProperty(context, {
    "entities":     qCreatedBy(id + "plate", EntityType.BODY),
    "propertyType": PropertyType.NAME,
    "value":        "Socket Cell " ~ socket.sizing,
  });

  return cs;
}
// --

// == [Socket Holder] ==

/**
 * Holder cells for every socket in `familyRef`, arranged left-to-right on `opts.basePlane`.
 * @param context {Context} : Model context.
 * @param id {Id} : Unique id prefix.
 * @param familyRef {map} : Socket variant map from @see `getSocketFamilyRef` (contains `.entries`).
 * @param opts {map} : Options for @see `socketCell`.
 */
function socketHolder(context is Context, id is Id, familyRef is map, opts is map) {
  var cursorX = zero;
//   var idx     = 0;

  for (var sizingKey, socketRecord in familyRef.entries) {
    // Pre-measure so we can position the circle center: left border edge is at cursorX,
    // and the circle is at -cs.borderMinH to the right of the left edge.
    const cs     = socketCellSize(context, id + ("ps" ~ sizingKey), socketRecord, opts);
    const center = vector(cursorX - cs.borderMinH, zero);
    socketCell(context, id + ("c" ~ sizingKey), socketRecord, opts, center);
    cursorX  = cursorX + cs.cellWidth;
    // idx     += 1;
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
  isLength(definition.cutoutPadding, { (millimeter): [0, 1, hugeSizeVal] } as LengthBoundSpec);

  annotation { "Name": "Border padding", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.borderPadding, { (millimeter): [0, 1, hugeSizeVal] } as LengthBoundSpec);

  annotation { "Name": "Grid size", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.gridSize, { (millimeter): [tinySizeVal, 5, hugeSizeVal] } as LengthBoundSpec);

  annotation { "Name": "Callout height", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.calloutHeight, { (millimeter): [tinySizeVal, 4, hugeSizeVal] } as LengthBoundSpec);

  annotation { "Name": "Label height", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.labelHeight, { (millimeter): [tinySizeVal, 3, hugeSizeVal] } as LengthBoundSpec);
}
{
  const basePlane = evPlane(context, { "face":  definition.referencePlaneQ });
  const familyRef   = getSocketFamilyRef(context, definition.familyPath);
  debug(context, [familyRef], DebugColor.MAGENTA);
  if (familyRef == undefined) {
    throw regenError("Unknown socket family");
  }

  socketHolder(context, id, familyRef, {
    "basePlane":     basePlane,
    "holderDepth":   definition.holderDepth,
    "layerHeight":   definition.layerHeight,
    "insertionGap":  definition.insertionGap,
    "cutoutPadding": definition.cutoutPadding,
    "borderPadding": definition.borderPadding,
    "gridSize":      definition.gridSize,
    "calloutHeight": definition.calloutHeight,
    "labelHeight":   definition.labelHeight,
  });
});
// --
