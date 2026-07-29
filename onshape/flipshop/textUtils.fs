FeatureScript 2909;
import(path : "onshape/std/geometry.fs", version : "2909.0");
export import(path : "19a276cbe441b4dcf19aaca1", version : "f2b74f49a1aac39b8fbf1834");
import(path : "c50e2363725f9cf513e36928", version : "906b813fb990c112d679b90b");
import(path : "075be6354063579d5fedb3b7", version : "b59a457165ed53b2ec7d71d6");

// == [Render Text] ==

/**
 * Part feature: extrudes `text` sized and aligned within a bounding plate at each selected plane.
 * Delegates all geometry to @see `renderText`.
 * @param definition {{
 *      @field sketchPlaneQ {Query} : One or more sketch planes; a separate text body is created at each.
 *      @field text {string} : Text to render.
 *      @field fontName {FontName} : Font filename.
 *      @field baselineHeight {ValueWithUnits} : Nominal cap height (before resizing).
 *      @field boundsWidth {ValueWithUnits} : Carrier plate width.
 *      @field boundsHeight {ValueWithUnits} : Carrier plate height.
 *      @field resizing0 {ResizingPolicy} : Resizing policy for X.
 *      @field resizing1 {ResizingPolicy} : Resizing policy for Y.
 *      @field horizontalAlign {HorizontalAlignment} : X anchor within the plate.
 *      @field verticalAlign {VerticalAlignment} : Y anchor within the plate.
 *      @field textAngle {ValueWithUnits} : In-plane rotation angle.
 *      @field textDepth {ValueWithUnits} : Extrusion depth of the text lettering.
 *      @field plateDepth {ValueWithUnits} : Extrusion depth of the carrier plate.
 *      @field [cleanupSketches=false] {boolean} : When true, deletes sketch bodies after generation.
 * }}
 */
annotation { "Feature Type Name": "Render Text" }
export const renderTextF = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Sketch Plane", "Filter": QueryFilterCompound.ALLOWS_PLANE, "MaxNumberOfPicks": 10 }
  definition.sketchPlaneQ is Query;
  annotation { "Name": "Text", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
  definition.text is string;
  annotation { "Name": "Font Name", "UIHint": [UIHint.SHOW_LABEL, UIHint.REMEMBER_PREVIOUS_VALUE] }
  definition.fontName is FontName;
  annotation { "Name": "Baseline Height", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.baselineHeight, { (millimeter): [0.001, 10, 1000000] } as LengthBoundSpec);
  annotation { "Name": "Bounds Width", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.boundsWidth, { (millimeter): [0.001, 50, 1000000] } as LengthBoundSpec);
  annotation { "Name": "Bounds Height", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.boundsHeight, { (millimeter): [0.001, 10, 1000000] } as LengthBoundSpec);
  annotation { "Name": "Horizontal Alignment", "UIHint": [UIHint.SHOW_LABEL, UIHint.REMEMBER_PREVIOUS_VALUE] }
  definition.horizontalAlign is HorizontalAlignment;
  annotation { "Name": "Vertical Alignment", "UIHint": [UIHint.SHOW_LABEL, UIHint.REMEMBER_PREVIOUS_VALUE] }
  definition.verticalAlign is VerticalAlignment;
  annotation { "Name": "Horizontal Resizing", "UIHint": [UIHint.SHOW_LABEL, UIHint.REMEMBER_PREVIOUS_VALUE] }
  definition.resizing0 is ResizingPolicy;
  annotation { "Name": "Vertical Resizing", "UIHint": [UIHint.SHOW_LABEL, UIHint.REMEMBER_PREVIOUS_VALUE] }
  definition.resizing1 is ResizingPolicy;
  annotation { "Name": "Text Angle" }
  isAngle(definition.textAngle, { (degree): [0, 0, 360] } as AngleBoundSpec);
  annotation { "Name": "Text Depth" }
  isLength(definition.textDepth, { (millimeter): [0.001, 1, 1000000] } as LengthBoundSpec);
  annotation { "Name": "Plate Depth" }
  isLength(definition.plateDepth, { (millimeter): [0.001, 0.5, 1000000] } as LengthBoundSpec);
  annotation { "Name": "Cleanup sketches" }
  definition.cleanupSketches is boolean;
}
{
  renderText(context, id, definition);
});

// --

/**
 * Extrudes `definition.text` at each plane in `definition.sketchPlaneQ`.
 * Calls @see `renderTextAt` for each evaluated plane.
 * @param context {Context} : Model context.
 * @param id {Id} : Base feature id.
 * @param definition {map} : Options for @see `renderTextAt`, plus `sketchPlaneQ`.
 */
export function renderText(context is Context, id is Id, definition is map) {
  var ptIdx = 0;
  for (var planeEnt in evaluateQuery(context, definition.sketchPlaneQ)) {
    renderTextAt(context, id + ("p" ~ toString(ptIdx)), planeEnt, definition);
    ptIdx += 1;
  }
}

/**
 * Extrudes `definition.text` sized and aligned within a bounding plate at a single plane.
 * @param context {Context} : Model context.
 * @param id {Id} : Base feature id.
 * @param planeEnt {Query} : Single sketch plane entity.
 * @param definition {map} : Keyword options.
 *   - @field text {string} : Text to render.
 *   - @field fontName {FontName} : Font filename.
 *   - @field baselineHeight {ValueWithUnits} : Nominal cap height (before resizing).
 *   - @field boundsWidth {ValueWithUnits} : Carrier plate width.
 *   - @field boundsHeight {ValueWithUnits} : Carrier plate height.
 *   - @field resizing0 {ResizingPolicy} : Resizing policy for X.
 *   - @field resizing1 {ResizingPolicy} : Resizing policy for Y.
 *   - @field horizontalAlign {HorizontalAlignment} : X anchor within the plate.
 *   - @field verticalAlign {VerticalAlignment} : Y anchor within the plate.
 *   - @field textAngle {ValueWithUnits} : In-plane rotation angle.
 *   - @field textDepth {ValueWithUnits} : Extrusion depth of the text lettering.
 *   - @field plateDepth {ValueWithUnits} : Extrusion depth of the carrier plate.
 *   - @field [cleanupSketches=false] {boolean} : When true, deletes sketch bodies after generation.
 */
function renderTextAt(context is Context, id is Id, planeEnt is Query, definition is map) {
  const basePlane = evPlane(context, { "face": planeEnt });
  const ids = {
    textSk:       id + "textSk",
    plateSk:      id + "plateSk",
    extentSk:     id + "extentSk",
    extrudeText:  id + "extrudeText",
    extrudePlate: id + "extrudePlate",
    cleanup:      id + "cleanup",
  };
  const sketches = {
    text:       rotatedSketch(context, ids.textSk,   { "basePlane": basePlane }, definition.textAngle),
    plate:      rotatedSketch(context, ids.plateSk,  { "basePlane": basePlane }, definition.textAngle),
    extent:     rotatedSketch(context, ids.extentSk, { "basePlane": basePlane }, definition.textAngle),
  };
  // Anchor offset: how far into the bounds box the text anchor sits (in sketch coords)
  var anchorX = 0 * mm;
  if (definition.horizontalAlign == HorizontalAlignment.CENTER || definition.horizontalAlign == HorizontalAlignment.CENTER_NOMINAL) {
    anchorX = definition.boundsWidth / 2;
  } else if (definition.horizontalAlign == HorizontalAlignment.RIGHT || definition.horizontalAlign == HorizontalAlignment.MAX) {
    anchorX = definition.boundsWidth;
  }
  var anchorY = 0 * mm;
  if (definition.verticalAlign == VerticalAlignment.MIDDLE) {
    anchorY = definition.boundsHeight / 2;
  } else if (definition.verticalAlign == VerticalAlignment.TOP_EXTENT || definition.verticalAlign == VerticalAlignment.TOP_BASELINE || definition.verticalAlign == VerticalAlignment.MAX) {
    anchorY = definition.boundsHeight;
  }

  // Carrier plate: bounds rectangle with the anchor at the sketch origin
  skRectangle(sketches.plate, "plate", {
    "firstCorner":  vector(-anchorX,                         -anchorY),
    "secondCorner": vector(definition.boundsWidth - anchorX, definition.boundsHeight - anchorY),
  });
  skSolve(sketches.plate);
  const plateSkFacesQ = qCreatedBy(ids.plateSk, EntityType.FACE);

  // Text: sized and aligned within the bounds at the sketch origin
  const scaledParams = skTextAt(context, id, "text", sketches.text, definition.text, vector(0 * mm, 0 * mm), definition.baselineHeight, {
    "fontName":        definition.fontName,
    "bounds":          vector(definition.boundsWidth, definition.boundsHeight),
    "resizing0":       definition.resizing0,
    "resizing1":       definition.resizing1,
    "horizontalAlign": definition.horizontalAlign,
    "verticalAlign":   definition.verticalAlign,
    "cleanupSketches": false,
  });
  skSolve(sketches.text);
  const textSkRegionQ = qSketchRegion(ids.textSk, true);

  // Text Carrier: actual extent of text glyphs
  skRectangle(sketches.extent, "extent", {
    "firstCorner":  vector(-anchorX,                         -anchorY),
    "secondCorner": vector(-anchorX + scaledParams.textCoords.left + scaledParams.textCoords.actualWidth, - anchorY + scaledParams.textCoords.actualHeight),
  });
  boxmRectangle(context, sketches.extent, "paddedBox", scaledParams.textCoords.paddedBox);
  boxmRectangle(context, sketches.extent, "actualBox", scaledParams.textCoords.actualBox);
//   boxmRectangle(context, sketches.extent, "actualBox", scaledParams.textCoords.actualBox);
  skSolve(sketches.extent);
//   const extentSkFacesQ = qCreatedBy(ids.extentSk, EntityType.FACE);

  // Extrude text lettering
  opExtrude(context, ids.extrudeText, {
    "entities":  textSkRegionQ,
    "direction": basePlane.normal,
    "endBound":  BoundingType.BLIND,
    "endDepth":  definition.textDepth,
  });
  const textBodiesQ = qCreatedBy(ids.extrudeText, EntityType.BODY);

  // Extrude carrier plate, merging with the text bodies
  extrude(context, ids.extrudePlate, {
    "entities":          plateSkFacesQ,
    "direction":         basePlane.normal,
    "endBound":          BoundingType.BLIND,
    "depth":             definition.plateDepth,
    "operationType":     NewBodyOperationType.ADD,
    "bodyType":          ExtendedToolBodyType.SOLID,
    "defaultScope":      false,
    "oppositeDirection": true,
    "booleanScope":      textBodiesQ,
  });
  const scrubbed = replace(definition.text, "[\\s]+", " ");
  const partName = "T:" ~ substring(scrubbed, 0, min(20, length(scrubbed)));
  setName(context, qUnion([textBodiesQ, qCreatedBy(ids.extrudePlate, EntityType.BODY)]), partName);

  // const cleanupSketches = definition.cleanupSketches == undefined ? false : definition.cleanupSketches;
  if ((definition.cleanupSketches != undefined) && (definition.cleanupSketches == true)) {
    opDeleteBodies(context, ids.cleanup, { "entities": qCreatedBy(ids.textSk, EntityType.BODY) });
  }
}

// --

// == [Sketch Text] ==

/**
 * `skText` entity on `sketch` anchored at `position`, auto-sized and aligned per the given policies.
 * Measures natural text geometry via `textBounds`, then derives the scaled `baselineHeight` and
 * `firstCorner` satisfying the resizing and alignment constraints before delegating to `skBasicTextAt`.
 * Returns `firstCorner`, `baselineHeight` (after scaling), `scaleFactor`, and `textCoords`.
 * @param context {Context} : Model context.
 * @param id {Id} : Base feature id for @see `textBounds` temporary geometry.
 * @param entityId {string} : Sketch entity id.
 * @param sketch {Sketch} : Target sketch.
 * @param text {string} : Text to draw.
 * @param position {Vector} : Anchor point; meaning determined by `horizontalAlign`/`verticalAlign`.
 * @param baselineHeight {ValueWithUnits} : Nominal cap height before any resizing is applied.
 * @param options {map} : keyword options
 *   - @field [fontName=FontName.OPEN_SANS_REGULAR] {FontName} : Font filename.
 *   - @field [bounds] {Vector} : Target 2-D bounds for resizing; required for non-NONE policies.
 *   - @field [resizing0=ResizingPolicy.NONE] {ResizingPolicy} : Resizing policy for X.
 *   - @field [resizing1=ResizingPolicy.NONE] {ResizingPolicy} : Resizing policy for Y.
 *   - @field [horizontalAlign=HorizontalAlignment.LEFT] {HorizontalAlignment} : X anchor interpretation.
 *   - @field [verticalAlign=VerticalAlignment.TOP_BASELINE] {VerticalAlignment} : Y anchor interpretation.
 *   - @field [keepTools=false] {boolean} : Retain temporary sketch bodies from textBounds.
 */
export function skTextAt(context is Context, id is Id, entityId is string, sketch is Sketch, text is string, position is Vector, baselineHeight is ValueWithUnits, options is map) returns map {
  const opts = mergeMaps({
    "fontName":        FontName.OPEN_SANS_REGULAR,
    "baselineHeight":  baselineHeight,
    "keepTools":       false,
    "resizing0":       ResizingPolicy.DOWNSCALE,
    "resizing1":       ResizingPolicy.DOWNSCALE,
    "horizontalAlign": HorizontalAlignment.CENTER,
    "verticalAlign":   VerticalAlignment.BOTTOM_EXTENT,
  }, options);
  // Measure natural text geometry at the nominal baselineHeight
  const textCoords = textBounds(context, id + "textBounds", text, opts);
  //
//   const origSize  = vector(textCoords.actualWidth, textCoords.capHeight);
  // text renders uniformly: all metrics (x and y) scale with baselineHeight, i.e. sf[1]
  const bounds    = opts.bounds == undefined ? textCoords.origBox.sizevec : opts.bounds;
  const factors   = resizingFactors(textCoords.origBox.sizevec, bounds, { "resizing0": opts.resizing0, "resizing1": opts.resizing1 });
  const sf        = factors.scaleFactor;
  const newHeight = opts.baselineHeight * sf[1];
  const xScale = sf[0];
  const yScale = sf[1];
  // Horizontal offset: position the named x-anchor of the scaled text at position[0]
  var xOffset = 0 * mm;
  if (opts.horizontalAlign == HorizontalAlignment.MIN) {
    xOffset = -textCoords.minLeft;
  } else if (opts.horizontalAlign == HorizontalAlignment.LEFT) {
    xOffset = -textCoords.left;
  } else if (opts.horizontalAlign == HorizontalAlignment.CENTER) {
    xOffset = -(textCoords.left + textCoords.right) / 2;
  } else if (opts.horizontalAlign == HorizontalAlignment.CENTER_NOMINAL) {
    xOffset = -(textCoords.minLeft + textCoords.maxRight) / 2;
  } else if (opts.horizontalAlign == HorizontalAlignment.RIGHT) {
    xOffset = -textCoords.right;
  } else if (opts.horizontalAlign == HorizontalAlignment.MAX) {
    xOffset = -textCoords.maxRight;
  }
  xOffset = xOffset * xScale;

  // Vertical offset: position the named y-anchor of the scaled text at position[1]
  var yOffset = 0 * mm;
  if (opts.verticalAlign == VerticalAlignment.MAX) {
    yOffset = -textCoords.maxHeight;
  } else if (opts.verticalAlign == VerticalAlignment.TOP_EXTENT) {
    yOffset = -textCoords.bbox.maxCorner[1];
  } else if (opts.verticalAlign == VerticalAlignment.TOP_BASELINE) {
    yOffset = -textCoords.capHeight;
  } else if (opts.verticalAlign == VerticalAlignment.MIDDLE) {
    yOffset = -(textCoords.bbox.maxCorner[1] + textCoords.bbox.minCorner[1]) / 2;
  } else if (opts.verticalAlign == VerticalAlignment.BOTTOM_BASELINE) {
    yOffset = -textCoords.baselineHeight;
  } else if (opts.verticalAlign == VerticalAlignment.BOTTOM_EXTENT) {
    yOffset = -textCoords.bbox.minCorner[1];
  } else if (opts.verticalAlign == VerticalAlignment.MIN) {
    yOffset = -textCoords.minHeight;
  }
  yOffset = yOffset * yScale;

  const firstCorner = position + vector(xOffset, yOffset);
  skBasicTextAt(context, entityId, sketch, text, firstCorner, newHeight, opts);
  return {
    "firstCorner":    firstCorner,
    "baselineHeight": newHeight,
    "xOffset":        xOffset,
    "yOffset":        yOffset,
    "scaleFactor":    sf,
    "textCoords":     textCoords,
  };
}

/**
 * `skText` entity on `sketch` at `firstCorner` with the given cap height.
 * @param context {Context} : Model context.
 * @param entityId {string} : Sketch entity id.
 * @param sketch {Sketch} : Target sketch.
 * @param text {string} : Text to draw.
 * @param firstCorner {Vector} : Bottom-left anchor.
 * @param baselineHeight {ValueWithUnits} : Cap height.
 * @param options {map} : keyword options
 *   - @field [fontName="OpenSans-Regular.ttf"] {FontName} : Font filename.
 */
export function skBasicTextAt(context is Context, entityId is string, sketch is Sketch, text is string, firstCorner is Vector, baselineHeight is ValueWithUnits, options is map) {
  const opts = mergeMaps({ "fontName": FontName.OPEN_SANS_REGULAR }, options);
  //   debug(context, [opts.fontName, FontName.ARIMO, FontNameString[opts.fontName]]);
  skText(sketch, entityId, {
    "text": text, fontName: FontNameString[opts.fontName], "firstCorner": firstCorner, secondCorner: firstCorner + vector(1*mm, baselineHeight),
  });
}

// --

// == [Measure Text] ==

export function boxmRectangle(context is Context, sketch is Sketch, sketchLabel is string, boxm is map) returns map {
  skRectangle(sketch, sketchLabel, {
    "firstCorner":  vector2(boxm.minvec),
    "secondCorner": vector2(boxm.maxvec),
  });
  return boxm;
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
    const shift2 = (opts.shift1 == undefined) ? zero : opts.shift2;
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

/**
 * Text metrics for `text`: tight bounding boxes (`tbox`, `bbox`, `wbox`), padded and
 * actual width/height, aspect ratio, overflow fraction, and descender fraction.
 * @param context {Context} : Model context.
 * @param id {Id} : Base feature id.
 * @param text {string} : Text to measure.
 * @param options {map} : keyword options
 *      - @field [fontName=FontName.OPEN_SANS_REGULAR] {FontName} : Font filename.
 *      - @field [baselineHeight=10mm] {ValueWithUnits} : Nominal cap height.
 *      - @field [keepTools=false] {boolean} : Retain the temporary sketch body.
 *      - @field [position=vector(0 * mm, 0 * mm)] { Vector } : origin point
 *      - @field [cleanupSketches=false] {boolean} : When true, deletes sketch bodies after generation.
 */
export function textBounds(context is Context, id is Id, text is string, options is map) returns map {
  const opts = mergeMaps({
      "fontName":       FontName.OPEN_SANS_REGULAR,
      "baselineHeight": 10*mm,
      "keepTools":      false,
      "position":       vector(0 * mm, 0 * mm),
  }, options);
  const prefix = id + nextLabelId(opts, "tempSketch" ~ text);
  const ids = { textSk: prefix + "text", maxSk: prefix + "max", minSk: prefix + "min", deleteText: id + "deleteSketch", deleteMax: id + "deleteMax", deleteMin: id + "deleteMin", cleanup: prefix + "cleanup" };
  const sketches = {
    "text": newSketchOnPlane(context, ids.textSk, { "sketchPlane": PL_TOP }),
    "max":  newSketchOnPlane(context, ids.maxSk,  { "sketchPlane": PL_TOP }),
    "min":  newSketchOnPlane(context, ids.minSk,  { "sketchPlane": PL_TOP }),
  };
  // Draw the text
  skBasicTextAt(context, "textBounds", sketches.text, text,             opts.position, opts.baselineHeight, opts);
  skBasicTextAt(context, "maxBounds",  sketches.max,  text ~ "(ÂÅ|q);", opts.position, opts.baselineHeight, opts);
  skBasicTextAt(context, "minBounds",  sketches.min,  "x",              opts.position, opts.baselineHeight, opts);
  skSolve(sketches.text);
  const textSkBodiesQ  = qCreatedBy(ids.textSk, EntityType.BODY);
  const textSkRegionQ  = qSketchRegion(ids.textSk, true);
  skSolve(sketches.max);
  const maxSkRegionQ   = qSketchRegion(ids.maxSk, true);
  skSolve(sketches.min);
  const minSkRegionQ   = qSketchRegion(ids.minSk, true);
  //
  // Wide box (includes horizontal padding and actual extent of text)
  const wbox             = evBox3d(context, { "topology": textSkBodiesQ,  "tight": true });
  // Text box (includes horizontal padding; extends from baseline to cap height)
  const tbox             = box3d(vector(0*mm, 0*mm, 0*mm), vector(wbox.maxCorner[0], opts.baselineHeight, 0*mm));
  // Bounding box (tight against the actual text region as rendered)
  const bbox             = evBox3d(context, { "topology": textSkRegionQ,  "tight": true });
  // Max box (tight against the actual text region rendering both tall letters (l,|,`, etc) and low letters (j,y,;,Q,etc))
  const ylMeasurer       = evBox3d(context, { "topology": maxSkRegionQ,   "tight": true });
  // Min box (tight against the actual text region using only "x")
  const xMeasurer        = evBox3d(context, { "topology": minSkRegionQ,   "tight": true });

  // Padded Box (width includes horizontal padding, height spans actual extent of text)
  const paddedBox        = boxmForBbox(evBox3d(context, { "topology": textSkBodiesQ,  "tight": true }));
  // Original Sketch Text Box (includes horizontal padding; extends from baseline to cap height)
  const origBox          = boxmForXYZ(zero, paddedBox.max0, zero, opts.baselineHeight);
  // Actual Bounding box (tight against the actual text region as rendered)
  const actualBox        = boxmForBbox(evBox3d(context, { "topology": textSkRegionQ,   "tight": true }));
  // Max box (actual width of text, and height of tallest/lowest letters in font (l,|,Å, etc // {,},j,y, etc)
  const minVertBox       = boxmForXYZ(actualBox.min0, actualBox.max0, ylMeasurer.minCorner[1], ylMeasurer.maxCorner[1]);
  // Min box (actual width of text, and baseline-to-x-height of a rendered "x")
  const maxVertBox       = boxmForXYZ(actualBox.min0, actualBox.max0, xMeasurer.minCorner[1], xMeasurer.maxCorner[1]);

  debug(context, boxmPretty(paddedBox));
  debug(context, boxmPretty(origBox));
  debug(context, ["actualBox",  boxmPretty(actualBox)]);
  debug(context, ["maxVertBox", boxmPretty(maxVertBox)]);
  debug(context, ["minVertBox", boxmPretty(minVertBox)]);
  // Calculate the text metrics
  const minLeft          = wbox.minCorner[0];
  const left             = bbox.minCorner[0];
  const right            = bbox.maxCorner[0];
  const maxRight         = wbox.maxCorner[0];
  const maxHeight        = ylMeasurer.maxCorner[1];
  const capHeight        = opts.baselineHeight;
  const xHeight          = xMeasurer.maxCorner[1];
  const baselineHeight   = xMeasurer.minCorner[1];
  const minHeight        = ylMeasurer.minCorner[1];
  const minBbox          = box3d(vector(minLeft, baselineHeight, 0*mm), vector(maxRight, xHeight,   0*mm));
  const maxBbox          = box3d(vector(left,    minHeight,      0*mm), vector(right,    maxHeight, 0*mm));
  const padBbox          = box3d(vector(minLeft, minHeight,      0*mm), vector(maxRight, maxHeight, 0*mm));
  const paddedWidth      = tbox.maxCorner[0] - tbox.minCorner[0];
  const paddedHeight     = tbox.maxCorner[1] - tbox.minCorner[1];
  const actualWidth      = bbox.maxCorner[0] - bbox.minCorner[0];
  const actualHeight     = bbox.maxCorner[1] - bbox.minCorner[1];
  const overflowHeight   = tbox.maxCorner[1] - opts.baselineHeight;
  const leftPaddingWidth  = tbox.minCorner[0] - bbox.minCorner[0];
  const rightPaddingWidth = bbox.maxCorner[0] - tbox.maxCorner[0];
  const result = {
    "tbox":             tbox,
    "bbox":             bbox,
    "wbox":             wbox,
    "minBbox":          minBbox,
    "padBbox":          padBbox,
    "maxBbox":          maxBbox,
    "minLeft":          minLeft, "left": left, "right": right, "maxRight": maxRight,
    "maxHeight":        maxHeight, "capHeight": capHeight, "xHeight": xHeight, "baselineHeight": baselineHeight, "minHeight": minHeight,
    "minCenter":        vector((right    + left)    / 2, (xHeight   + baselineHeight) / 2),
    "maxCenter":        vector((right    + left)    / 2, (maxHeight + minHeight)      / 2),
    "padCenter":        vector((maxRight + minLeft) / 2, (maxHeight + minHeight)      / 2),
    "leftPaddingWidth": leftPaddingWidth, "rightPaddingWidth": rightPaddingWidth,
    "paddedWidth":      paddedWidth,     "paddedHeight":     paddedHeight,
    "actualWidth":      actualWidth,     "actualHeight":     actualHeight,
    "overflowHeight":   overflowHeight,
    "aspectRatio":      actualWidth / actualHeight,
    "descenderFrac":    (actualHeight - overflowHeight - opts.baselineHeight) / actualHeight,
    "overflowFrac":     overflowHeight / actualHeight,
    "origBox": origBox, "actualBox": actualBox, "paddedBox": paddedBox, "maxVertBox": maxVertBox, "minVertBox": minVertBox,
  };
  // if ((opts.cleanupSketches != undefined) && (opts.cleanupSketches != false)) {
    opDeleteBodies(context, ids.cleanup, { "entities": qSketchFilter(qCreatedBy(id), SketchObject.YES) });
  // }
  return result;
}

/**
 * Rendered width of `text` on the global XY plane.
 * @param context {Context} : Model context.
 * @param id {Id} : Base feature id.
 * @param text {string} : Text to measure.
 * @param opts {map} : Options for @see `textBounds`.
 */
export function measureTextWidth(context is Context, id is Id, text is string, opts is map) returns ValueWithUnits {
  const  textCoords = textBounds(context, id, text, opts);
  return textCoords.maxCorner[0] - textCoords.minCorner[0];
}

/**
 * Rendered baseline metrics for `text` on the global XY plane.
 * @param context {Context} : Model context.
 * @param id {Id} : Base feature id.
 * @param text {string} : Text to measure.
 * @param opts {map} : Options for @see `textBounds`.
 */
export function measureTextBaseline(context is Context, id is Id, text is string, opts is map) returns ValueWithUnits {
  const  textCoords = textBounds(context, id, text, opts);
  return textCoords.maxCorner[0] - textCoords.minCorner[0];
}

/**
 * Sketch on `params.basePlane` with X axis rotated `angle` around the plane normal.
 * @param context {Context} : Model context.
 * @param id {Id} : Sketch feature id.
 * @param params {map} : Must contain `basePlane` (Plane).
 * @param angle {ValueWithUnits} : In-plane rotation angle.
 */
export function rotatedSketch(context is Context, id is Id, params is map, angle is ValueWithUnits) returns Sketch {
  const  basePlane    = params.basePlane;
  const  rotatedX     = cos(angle) * basePlane.x + sin(angle) * cross(basePlane.normal, basePlane.x);
  const  rotatedPlane = plane(basePlane.origin, basePlane.normal, rotatedX);
  return newSketchOnPlane(context, id, { "sketchPlane": rotatedPlane });
}

/**
 * Feature: extrudes `text` and visualizes its bounding boxes for debugging text metrics.
 * Produces an extruded text body plus a thin carrier plate covering
 * the tight text area, named with the measured aspect ratio and descender fraction.
 * @param context {Context} : Model context.
 * @param id {Id} : Base feature id.
 * @param definition {{
 *      @field text {string} : Text to render and measure.
 *      @field fontName {FontName} : Font filename.
 *      @field baselineHeight {ValueWithUnits} : Cap height.
 *      @field sketchPlaneQ {Query} : Sketch plane.
 *      @field textAngle {ValueWithUnits} : In-plane rotation angle.
 * }}
 */
annotation { "Feature Type Name": "Measure Text 3D" }
export const measureText3d = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Text" }
  definition.text is string;

  annotation { "Name": "Font Name", "UIHint" : UIHint.SHOW_LABEL }
  definition.fontName is FontName;

  annotation { "Name": "Baseline Height" }
  isLength(definition.baselineHeight, { (millimeter): [0.001, 10, 1000000] } as LengthBoundSpec);

  annotation { "Name": "Sketch Plane", "Filter": QueryFilterCompound.ALLOWS_PLANE, "MaxNumberOfPicks": 1 }
  definition.sketchPlaneQ is Query;

  annotation { "Name": "Text Angle" }
  isAngle(definition.textAngle, { (degree): [0, 0, 360] } as AngleBoundSpec);
}
{
  const ids           = { "extrudedText": id + "extrudedText", "textFacesSk": id + "textFacesSk", "carrierSk": id + "carrierSk", "carrierPlate": id + "carrierPlate" };
  const basePlane     = evPlane(context, { "face": definition.sketchPlaneQ });
  const params        = {
     fontName: definition.fontName,
     baselineHeight: definition.baselineHeight,
     "basePlane": basePlane,
     textAngle: definition.textAngle,
     text: definition.text,
  };
  //
  const sketches   = {
    textFaces: rotatedSketch(context, ids.textFacesSk, params, definition.textAngle),
    carrierSk: rotatedSketch(context, ids.carrierSk,   params, definition.textAngle),
  };
  const textCoords = textBounds(context, id, params.text, params);

  // Extrude the text
  skBasicTextAt(context, "measureText3d", sketches.textFaces, params.text, vector(0*mm, 0*mm), params.baselineHeight, params);
  skSolve(sketches.textFaces);
  opExtrude(context, ids.extrudedText, {
    "entities":  qSketchRegion(ids.textFacesSk, true),
    "direction": basePlane.normal,
    "endBound":  BoundingType.BLIND,
    "endDepth":  definition.baselineHeight / 20,
  });
  const extrudedBodies = qCreatedBy(ids.extrudedText, EntityType.BODY);

  // Draw the text extents
  // debug(context, ["measureText3d", boxMag(textCoords.tbox, 1*mm), boxMag(textCoords.bbox, 1*mm)], DebugColor.CYAN);
  skRectangle(sketches.carrierSk,    "bbox",  { firstCorner: vector2(textCoords.bbox.minCorner),                secondCorner: vector2(textCoords.bbox.maxCorner)  });
  skRectangle(sketches.carrierSk,    "tbox",  { firstCorner: vector2(textCoords.tbox.minCorner),                secondCorner: vector2(textCoords.tbox.maxCorner), construction: true });
  skLineSegment(sketches.carrierSk, "xheight", { start: vector(textCoords.left, textCoords.xHeight), end: vector(textCoords.right, textCoords.xHeight),    construction: true });
  skLineSegment(sketches.carrierSk, "padCorners", { start: vector2(textCoords.padBbox.minCorner),              end: vector2(textCoords.padBbox.maxCorner), construction: true });
  skLineSegment(sketches.carrierSk, "maxCorners", { start: vector2(textCoords.maxBbox.minCorner),              end: vector2(textCoords.maxBbox.maxCorner), construction: true });
  skPoint(sketches.carrierSk, "minCorner", { "position":    vector2(textCoords.wbox.minCorner) });
  skPoint(sketches.carrierSk, "maxCorner", { "position":    vector2(textCoords.wbox.maxCorner) });
  skPoint(sketches.carrierSk, "padCenter", { "position":    vector2(textCoords.padCenter) });
  skPoint(sketches.carrierSk, "maxCenter", { "position":    vector2(textCoords.maxCenter) });
  skSolve(sketches.carrierSk);

  // Extrude a carrier plate covering the whole text area
  const plateFace  = qCreatedBy(ids.carrierSk, EntityType.FACE);
  extrude(context, ids.carrierPlate, {
    "entities":          plateFace,
    "direction":         basePlane.normal,
    "endBound":          BoundingType.BLIND,
    "depth":             abs(definition.baselineHeight) / 100,
    "operationType":     NewBodyOperationType.ADD,
    "bodyType":          ExtendedToolBodyType.SOLID,
    "defaultScope":      false,
    "oppositeDirection": true,
    "booleanScope":      extrudedBodies,
  });

  setName(context, extrudedBodies, "AR: " ~ replace(substring(toString(round(textCoords.aspectRatio, 0.1)), 0, 3), "(\\.?0+$|0+$)", "") ~ " df: " ~ substring(toString(round(textCoords.descenderFrac, 0.1)), 0, 3));
});

// --

// == [Emboss Text] ==

export enum EmbossType {
  annotation { "Name": "Emboss" }
  EMBOSS,
  annotation { "Name": "Deboss" }
  DEBOSS
}

/**
 * Extrudes `text` and booleans it into or out of `targets`.
 * EMBOSS raises letters above `sketchPlane` (extrudes along normal, unions with targets).
 * DEBOSS carves letters into `sketchPlane` (extrudes against normal, subtracts from targets).
 * @param context {Context} : Model context.
 * @param id {Id} : Base feature id.
 * @param sketchPlane {Plane} : Plane on which text is sketched; normal points outward from material surface.
 * @param position {Vector} : Anchor point in sketch-plane local coords.
 * @param targets {Query} : Solid bodies to boolean with.
 * @param text {string} : Text to emboss/deboss.
 * @param embossType {EmbossType} : EMBOSS or DEBOSS.
 * @param textHeight {ValueWithUnits} : Cap height.
 * @param options {map} : keyword options for @see `skTextAt`, plus:
 *   - @field endDepth {ValueWithUnits} : Extrusion depth (positive, required).
 *   - @field [fontName=FontName.OPEN_SANS_REGULAR] {FontName} : Font filename.
 *   - @field [horizontalAlign=HorizontalAlignment.CENTER] {HorizontalAlignment}
 *   - @field [verticalAlign=VerticalAlignment.BOTTOM_BASELINE] {VerticalAlignment}
 *   - @field [cleanupSketches=false] {boolean} : When true, deletes sketch bodies after generation.
 */
export function embossText(context is Context, id is Id, sketchPlane is Plane,
  position is Vector, targets is Query, text is string, embossType is EmbossType,
  textHeight is ValueWithUnits, options is map) {
  const ids = { "textSk":  id + "textSk",  "extrude": id + "extrude", "bool": id + "bool" };
  const sketch = newSketchOnPlane(context, ids.textSk, { "sketchPlane":  sketchPlane });
  skTextAt(context, id + "meas", "text", sketch, text, position, textHeight, options);
  skSolve(sketch);
  const textFacesQ = qSketchRegion(ids.textSk, true);
  // EMBOSS: letters protrude along normal; DEBOSS: letters cut against normal (into material)
  const extrudeDir = (embossType == EmbossType.EMBOSS) ? sketchPlane.normal : sketchPlane.normal * -1;
  opExtrude(context, ids.extrude, mergeMaps({
    "entities":  textFacesQ,
    "direction": extrudeDir,
    "endBound":  BoundingType.BLIND,
  }, options));
  const textBodiesQ  = qCreatedBy(ids.extrude, EntityType.BODY);
  const targetSolids = qBodyType(targets, BodyType.SOLID);
  if (embossType == EmbossType.EMBOSS) {
    opBoolean(context, ids.bool, {
      "tools":          qUnion([targetSolids, textBodiesQ]),
      "operationType":  BooleanOperationType.UNION,
      "keepTools":      false,
    });
  } else {
    opBoolean(context, ids.bool, {
      "tools":          textBodiesQ,
      "targets":        targetSolids,
      "operationType":  BooleanOperationType.SUBTRACTION,
      "keepTools":      false,
    });
  }
  if (options.cleanupSketches == true) { opDeleteBodies(context, id + "cleanup",  { "entities": qCreatedBy(ids.textSk,  EntityType.BODY) }); }
}
// --
// --

// == [Resizing Text] ==

/**
 * Per-dimension ratios of `origSize` to `bounds`, plus the extremes.
 * A `ratio` > 1 means the original is larger than bounds in that dimension.
 * @param origSize {Vector} : Original 2-D size.
 * @param bounds {Vector} : Target 2-D bounds.
 */
export function resizingRatios(origSize is Vector, bounds is Vector) returns map {
  const ratio0 = origSize[0] / bounds[0];
  const ratio1 = origSize[1] / bounds[1];
  return {
    "ratio0":        ratio0,
    "ratio1":        ratio1,
    "largestRatio":  max(ratio0, ratio1),
    "smallestRatio": min(ratio0, ratio1),
  };
}

/* Per-axis scale factor for a single independent ResizingPolicy. FOLLOW resolved by caller. */
function scaleForPolicy(policy is ResizingPolicy, ratio is number) returns number {
  if (policy == ResizingPolicy.NONE)     { return 1.0; }
  if (policy == ResizingPolicy.STRETCH)  { return 1.0 / ratio; }
  if (policy == ResizingPolicy.LIMIT)    { return min(1.0, 1.0 / ratio); }
  if (policy == ResizingPolicy.EMBIGGEN) { return max(1.0, 1.0 / ratio); }
  return 1.0;
}

/**
 * Scale factor vector for the given per-axis `ResizingPolicy` pair, derived from `baseFactors`.
 * Dimension-coupled policies (CONTAIN, COVER, DOWNSCALE, MAXIMIZE) apply the same uniform factor
 * to both axes; per-axis policies are resolved independently, with FOLLOW inheriting the other axis.
 * @param baseFactors {map} : Output of @see `resizingRatios`.
 * @param resizing0 {ResizingPolicy} : Policy for dimension 0 (X).
 * @param resizing1 {ResizingPolicy} : Policy for dimension 1 (Y).
 */
export function resizingFactorsFor(baseFactors is map, resizing0 is ResizingPolicy, resizing1 is ResizingPolicy) returns Vector {
  const r0 = baseFactors.ratio0;
  const r1 = baseFactors.ratio1;
  const lr = baseFactors.largestRatio;
  const sr = baseFactors.smallestRatio;
  // Dimension-coupled policies — both axes share the same uniform factor
  if (resizing0 == ResizingPolicy.CONTAIN)   { return vector(    1.0 / lr,         1.0 / lr);     }
  if (resizing0 == ResizingPolicy.COVER)     { return vector(    1.0 / sr,         1.0 / sr);     }
  if (resizing0 == ResizingPolicy.DOWNSCALE) { return vector(min(1.0 / lr, 1), min(1.0 / lr, 1)); }
  if (resizing0 == ResizingPolicy.MAXIMIZE)  { return vector(max(1.0 / sr, 1), max(1.0 / sr, 1)); }
  // Per-axis independent policies; resolve FOLLOW after computing the other axis
  var sf0 = scaleForPolicy(resizing0, r0);
  var sf1 = scaleForPolicy(resizing1, r1);
  if (resizing0 == ResizingPolicy.FOLLOW) { sf0 = sf1; }
  if (resizing1 == ResizingPolicy.FOLLOW) { sf1 = sf0; }
  return vector(sf0, sf1);
}

/**
 * Resizing result for `origSize` scaled into `bounds` under the given per-axis policies.
 * Returns `origSize`, `bounds`, and `scaleFactor` (per-axis proportion to apply to origSize).
 * @param origSize {Vector} : Original 2-D size.
 * @param bounds {Vector} : Target 2-D bounds.
 * @param policies {map} : Resizing policies.
 *   - @field resizing0 {ResizingPolicy} : Policy for dimension 0 (X).
 *   - @field resizing1 {ResizingPolicy} : Policy for dimension 1 (Y).
 */
export function resizingFactors(origSize is Vector, bounds is Vector, policies is map) returns map {
  const baseFactors = resizingRatios(origSize, bounds);
  const scaleFactor = resizingFactorsFor(baseFactors, policies.resizing0, policies.resizing1);
  return {
    "origSize":    origSize,
    "bounds":      bounds,
    "scaleFactor": scaleFactor,
  };
}

// --

// == [Utility Functions] ==

/**
 * Unique entity id from `label`, with uniqueness scoped to `params`.
 * @param params {map} : Caller's params map; holds the per-caller counter.
 * @param label {string} : Base prefix.
 */
export function nextLabelId(params is map, label is string) returns string {
    params.idUniquer = params.idUniqer == undefined ? 0 : params.idUniqer + 1;
    return replace(label ~ "_" ~ params.idUniquer, "[^\\w]", '-');
}

// --
