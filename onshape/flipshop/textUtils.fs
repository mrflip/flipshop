FeatureScript 3029;
import(path : "onshape/std/geometry.fs", version : "3029.0");
export import(path : "19a276cbe441b4dcf19aaca1", version : "f65e96bb9819bc0fe6817767");
import(path : "c50e2363725f9cf513e36928", version : "d2b15246df075c7e1d8bf2f5");
import(path : "075be6354063579d5fedb3b7", version : "6fb770875f7cb63a9103cbe2");
import(path : "e313a0b67ecb3be0415d2186", version : "f6e848b7a870ea151275f4b7");
IconNamespace::import(path : "476f292746334f9ccc9aa08c", version : "ed7e2d8b30026af72161db6f");

export const TEXT_STABILIZER = "∑qÚÅ|(Á)?;";

// == [Text Chip] ==

/**
 * Part feature: extrudes `text` sized and aligned within a bounding plate at each selected plane.
 * Delegates all geometry to @see `textChip`.
 * @param definition {{
 *      @field sketchPlaneQ {Query} : One or more sketch planes; a separate text body is created at each.
 *      @field text {string} : Text to render.
 *      @field fontName {FontName} : Font filename.
 *      @field baselineHeight {ValueWithUnits} : Nominal cap height (before resizing).
 *      @field boundsWidth {ValueWithUnits} : Carrier plate width.
 *      @field boundsHeight {ValueWithUnits} : Carrier plate height.
 *      @field horizontalResizing {ResizingPolicy} : Resizing policy for X.
 *      @field verticalResizing {ResizingPolicy} : Resizing policy for Y.
 *      @field horizontalAlign {HorizontalAlignment} : X anchor within the plate.
 *      @field verticalAlign {VerticalAlignment} : Y anchor within the plate.
 *      @field textAngle {ValueWithUnits} : In-plane rotation angle.
 *      @field textDepth {ValueWithUnits} : Extrusion depth of the text lettering.
 *      @field plateDepth {ValueWithUnits} : Extrusion depth of the carrier plate.
 *      @field [cleanupSketches=false] {boolean} : When true, deletes sketch bodies after generation.
 * }}
 */
annotation { "Feature Type Name": "Text Chip", "Icon": IconNamespace::BLOB_DATA }
export const textChipF = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Sketch Plane", "Filter": QueryFilterCompound.ALLOWS_PLANE, "MaxNumberOfPicks": 20 }
  definition.sketchPlaneQ is Query;
  annotation { "Name" : "Opposite direction",        "UIHint" : UIHint.OPPOSITE_DIRECTION }
  definition.oppositeDirection is boolean;
  annotation { "Name": "Text",                       "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
  definition.text is string;
  annotation { "Name": "Font Name",                  "UIHint": [UIHint.SHOW_LABEL, UIHint.REMEMBER_PREVIOUS_VALUE] }
  definition.fontName is FontName;
  annotation { "Name": "Baseline Height",             "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.baselineHeight, { (millimeter): [0.001, 18, 1000000] } as LengthBoundSpec);
  annotation { "Name": "Bounds Width",              "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.boundsWidth, { (millimeter):    [0.001, 24, 1000000] } as LengthBoundSpec);
  annotation { "Name": "Bounds Height", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.boundsHeight, { (millimeter):   [0.001, 16, 1000000] } as LengthBoundSpec);
  annotation { "Name": "Text Angle" }
  isAngle(definition.textAngle, { (degree): [0, 0, 360] } as AngleBoundSpec);
  annotation { "Name": "Text Depth" }
  isLength(definition.textDepth, { (millimeter): [0.001, 2, 1000000] } as LengthBoundSpec);
  annotation { "Name": "Plate Depth" }
  isLength(definition.plateDepth, { (millimeter): [0.001, 0.02, 1000000] } as LengthBoundSpec);
  annotation { "Name": "Horizontal Alignment", "UIHint": [UIHint.SHOW_LABEL, UIHint.REMEMBER_PREVIOUS_VALUE] }
  definition.horizontalAlign is HorizontalAlignment;
  annotation { "Name": "Vertical Alignment",    "UIHint": [UIHint.SHOW_LABEL, UIHint.REMEMBER_PREVIOUS_VALUE] }
  definition.verticalAlign is VerticalAlignment;
  annotation { "Name": "Horizontal Resizing",   "UIHint": [UIHint.SHOW_LABEL, UIHint.REMEMBER_PREVIOUS_VALUE] }
  definition.horizontalResizing is ResizingPolicy;
  if ( (definition.horizontalResizing == ResizingPolicy.FREE)   ||  (definition.horizontalResizing == ResizingPolicy.FORCE)
    || (definition.horizontalResizing == ResizingPolicy.SHRINK) ||  (definition.horizontalResizing == ResizingPolicy.GROW)
    || (definition.horizontalResizing == ResizingPolicy.FOLLOW)) {
    annotation { "Name": "Vertical Resizing",     "UIHint": [UIHint.SHOW_LABEL, UIHint.REMEMBER_PREVIOUS_VALUE] }
    definition.verticalResizing is StretchingPolicy;
  }
  annotation { "Name": "Cleanup sketches", "Default" : true, "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE] }
  definition.cleanupSketches is boolean;
}
{
  definition.verticalResizing = definition.verticalResizing as ResizingPolicy;
  textChip(context, id, definition);
});

/**
 * Extrudes `definition.text` at each plane in `definition.sketchPlaneQ`.
 * Calls @see `textChipAt` for each evaluated plane.
 * @param context {Context} : Model context.
 * @param id {Id} : Base feature id.
 * @param definition {map} : Options for @see `renderTextAt`, plus `sketchPlaneQ`.
 */
export function textChip(context is Context, id is Id, definition is map) {
  var ptIdx = 0;
  for (var planeEnt in evaluateQuery(context, definition.sketchPlaneQ)) {
    textChipAt(context, id + ("p" ~ toString(ptIdx)), planeEnt, definition);
    ptIdx += 1;
  }
}

/**
 * Extrudes `definition.text` at each plane in `definition.sketchPlaneQ`.
 * Calls @see `textChipAt` for each evaluated plane.
 * @param context {Context} : Model context.
 * @param id {Id} : Base feature id.
 * @param definition {map} : Options for @see `renderTextAt`, plus `sketchPlaneQ`.
 */
export function embossTextFaces(context is Context, id is Id, definition is map) {
  var ptIdx = 0;
  for (var planeEnt in evaluateQuery(context, definition.sketchPlaneQ)) {
    embossText(context, id + ("p" ~ toString(ptIdx)), planeEnt, definition.partQ, definition);
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
 *   - @field horizontalResizing {ResizingPolicy} : Resizing policy for X.
 *   - @field verticalResizing {ResizingPolicy} : Resizing policy for Y.
 *   - @field horizontalAlign {HorizontalAlignment} : X anchor within the plate.
 *   - @field verticalAlign {VerticalAlignment} : Y anchor within the plate.
 *   - @field textAngle {ValueWithUnits} : In-plane rotation angle.
 *   - @field textDepth {ValueWithUnits} : Extrusion depth of the text lettering.
 *   - @field plateDepth {ValueWithUnits} : Extrusion depth of the carrier plate.
 *   - @field [cleanupSketches=false] {boolean} : When true, deletes sketch bodies after generation.
 */
export function textChipAt(context is Context, id is Id, planeEnt is Query, opts is map) {
  const basePlane = evPlane(context, { "face": planeEnt });
  const polarity = ifNil(opts.oppositeDirection, false) ? -1 : 1;
  const ids = { plateSk: id + "plateSk", extrudePlate:  id + "extrudePlate",  emboss: id + "embossText",  cleanup: id + "cleanup" };
  const plateSk = rotatedSketch(context, ids.plateSk,  { "basePlane": basePlane }, opts.textAngle);
  // Anchor offset: how far into the bounds box the text anchor sits (in sketch coords)
  var anchorX = opts.boundsWidth  * horizAlignmentShift(opts.horizontalAlign);
  var anchorY = opts.boundsHeight * vertAlignmentShift(opts.verticalAlign);

  // Carrier plate: bounds rectangle with the anchor at the sketch origin
  skRectangle(plateSk, "plate", {
    "firstCorner":  vector(-anchorX,                         -anchorY),
    "secondCorner": vector(opts.boundsWidth - anchorX, opts.boundsHeight - anchorY),
  });
  skSolve(plateSk);

  // Extrude target plate
  opExtrude(context, ids.extrudePlate, {
    "entities":    qCreatedBy(ids.plateSk, EntityType.FACE),
    "direction":   -polarity * basePlane.normal,
    "endBound":    BoundingType.BLIND,
    "endDepth":    opts.plateDepth,
  });
  const plateBodiesQ = qCreatedBy(ids.extrudePlate, EntityType.BODY);

  const scaledParams = embossText(context, ids.emboss, planeEnt, plateBodiesQ, opts);

  // const cleanupSketches = definition.cleanupSketches == undefined ? false : definition.cleanupSketches;
  if (ifNil(opts.cleanupSketches, true)) {
    opDeleteBodies(context, ids.cleanup, { "entities": qCreatedBy(ids.plateSk, EntityType.BODY) });
  }

  return scaledParams;
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
 *   - @field horizontalResizing {ResizingPolicy} : Resizing policy for X.
 *   - @field verticalResizing {ResizingPolicy} : Resizing policy for Y.
 *   - @field horizontalAlign {HorizontalAlignment} : X anchor within the plate.
 *   - @field verticalAlign {VerticalAlignment} : Y anchor within the plate.
 *   - @field textAngle {ValueWithUnits} : In-plane rotation angle.
 *   - @field textDepth {ValueWithUnits} : Extrusion depth of the text lettering.
 *   - @field plateDepth {ValueWithUnits} : Extrusion depth of the carrier plate.
 *   - @field oppositeDirection {boolean} : Text extrudes opposite face normal
 *   - @field [cleanupSketches=false] {boolean} : When true, deletes sketch bodies after generation.
 */
export function embossText(context is Context, id is Id, planeEnt is Query, partQ is Query, opts is map) {
  const basePlane = evPlane(context, { "face": planeEnt });
  const ids = {
    textSk:        id + "textSk",
    extentSk:      id + "extentSk",
    extrudeText:   id + "extrudeText",
    extrudePlate:  id + "extrudePlate",
    extrudeExtent: id + "extrudeExtent",
    scale:         id + "Scale",
    cleanup:       id + "cleanup",
  };
  const sketches = {
    text:       rotatedSketch(context, ids.textSk,   { "basePlane": basePlane }, opts.textAngle),
    extent:     rotatedSketch(context, ids.extentSk, { "basePlane": basePlane }, opts.textAngle),
  };
  const polarity = ifNil(opts.oppositeDirection, false) ? -1 : 1;

  // Text: sized and aligned within the bounds at the sketch origin -- but no non-uniform scaling applied
  //
  const scaledParams = skTextAt(context, id, "text", sketches.text, opts.text, vector(0 * mm, 0 * mm), opts.baselineHeight, {
    "fontName":           opts.fontName,
    "bounds":             vector(opts.boundsWidth, opts.boundsHeight),
    "horizontalResizing": opts.horizontalResizing,
    "verticalResizing":   opts.verticalResizing,
    "horizontalAlign":    opts.horizontalAlign,
    "verticalAlign":      opts.verticalAlign,
    "cleanupSketches":    false,
  });
  skSolve(sketches.text);
  const textSkRegionQ = qSketchRegion(ids.textSk, true);

//   // Anchor offset: how far into the bounds box the text anchor sits (in sketch coords)
  var anchorX = opts.boundsWidth  * horizAlignmentShift(opts.horizontalAlign);
  var anchorY = opts.boundsHeight * vertAlignmentShift(opts.verticalAlign);

  // Extrude carrier plate for extruded text to be a single part
  // skTextAt actually scales uniformly; we can't perform horizontal scaling until we've extruded and re-scaled
  const rawScaling = mergeMaps(scaledParams.scaling, { "scale0": scaledParams.scaling.scale1 });
  const actualBox = boxmRescale(scaledParams.textCoords.actualBox, rawScaling);
  skBoxmRectangle(context, sketches.extent, "actualBox",     actualBox);
  skBoxmMidline0(context,  sketches.extent, "aboxHzMidline", actualBox, true);
  skBoxmMidline1(context,  sketches.extent, "aboxVtMidline", actualBox, true);
  skBoxmRectangle(context, sketches.extent, "paddedBox",     boxmRescale(scaledParams.textCoords.paddedBox, rawScaling), true);
  skBoxmRectangle(context, sketches.extent, "stableBox",     boxmRescale(scaledParams.textCoords.stableBox, rawScaling), true);
  skSimpleCircle(context,  sketches.extent, "anchor", zero, zero, boxmMinSize01(actualBox) / 20, true);
  skSolve(sketches.extent);
  const extentSkFacesQ = qSketchRegion(ids.extentSk, true);
  const extentPlateQ   = simpleExtrude(context, ids.extrudeExtent, -polarity * basePlane.normal, extentSkFacesQ, { depth: opts.plateDepth });

  debug(context, [
    "anchoring",    anchorX/mm, anchorY/mm,
    "scaling",      scaledParams.scaling.shift0/mm, scaledParams.scaling.shift1/mm, scaledParams.scaling.scale0, scaledParams.scaling.scale1,
    "actualBox",    boxmPretty(actualBox),
    "actualBoxMinSize", boxmMinSize01(actualBox)/mm,
  ]);

  // Extrude actual text, merging onto the carrier
  extrude(context, ids.extrudeText, mergeMaps(opts, {
    "entities":          textSkRegionQ,
    "direction":         polarity * basePlane.normal,
    "endBound":          BoundingType.BLIND,
    "depth":             opts.textDepth,
    "operationType":     NewBodyOperationType.ADD,
    "bodyType":          ExtendedToolBodyType.SOLID,
    "defaultScope":      false,
    "oppositeDirection": ifNil(opts.oppositeDirection, false),
    "booleanScope":      extentPlateQ,
  }));
  const textBodiesQ   = qCreatedBy(ids.extrudeText, EntityType.BODY);
  const textChipPartQ = qUnion([textBodiesQ, extentPlateQ]);
  setReadableName(context, textChipPartQ, "T:" ~ opts.text);

  opTransform(context, ids.scale, {
    "bodies" :  textChipPartQ,
    "transform" : scaleNonuniformly(scaledParams.scaling.scale0 / scaledParams.scaling.scale1, 1.0, 1.0, coordSystem(basePlane)),
  });

  if (opts.operationType == EmbossOpType.DEBOSS) {
    opBoolean(context, id + "boolean1", {
      "tools" :         textChipPartQ,
      "targets":        partQ,                             // qEntityFilter(partQ, EntityType.BODY),
      "operationType" : BooleanOperationType.SUBTRACTION,
      "keepTools":      true,
    });
  } else if (opts.operationType == EmbossOpType.EMBED) {
    const copyOfExtentPlate = simpleExtrude(context, id + "copyOfExtentPlate", -polarity * basePlane.normal, extentSkFacesQ, { depth: opts.plateDepth });
    const embedToolsQ = qUnion([partQ, copyOfExtentPlate]);
    opBoolean(context, id + "boolean1", {
      "tools" :         embedToolsQ,
      "targets":        textChipPartQ,
      "operationType" : BooleanOperationType.SUBTRACT_COMPLEMENT,
      "keepTools":      true,
    });
    // opDeleteBodies(context, id + "deleteCopyOfExtentPlate", { "entities" : copyOfExtentPlate });
    opBoolean(context, id + "debossText", {
      "tools" :         textChipPartQ,
      "targets":        embedToolsQ,
      "operationType" : BooleanOperationType.SUBTRACTION,
      "keepTools":      true,
    });
  } else {
    opBoolean(context, id + "boolean1", {
      "tools" :         qUnion([textChipPartQ, partQ]),
      "operationType" : ifNil(opts.operationType, BooleanOperationType.UNION),
    });
  }

  // const cleanupSketches = definition.cleanupSketches == undefined ? false : definition.cleanupSketches;
  if (ifNil(opts.cleanupSketches, true)) {
    opDeleteBodies(context, ids.cleanup, { "entities": qUnion([qCreatedBy(ids.textSk, EntityType.BODY), qCreatedBy(ids.extentSk, EntityType.BODY)]) });
  }
  return scaledParams;
}

// function simpleBoolean(context is Context, id is Id, tools is Query, targets is Query, options is map) returns Query {
// }

// --

// == [Sketch Text] ==

/**
 * `skText` entity on `sketch` anchored at `position`, auto-sized and aligned per the given policies.
 * Measures natural text geometry via `textBounds`, then derives the scaled `baselineHeight` and
 * `firstCorner` satisfying the resizing and alignment constraints before delegating to `skBasicTextAt`.
 * Returns `firstCorner`, `baselineHeight` (after scaling), `scaleFactor`, and `textCoords`.
 *
 * Because of limitations in OnShape, this cannot control the horizontal scaling;
 * see embossText for one way to do this
 *
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
 *   - @field [horizontalResizing=ResizingPolicy.NONE] {ResizingPolicy} : Resizing policy for X.
 *   - @field [verticalResizing=ResizingPolicy.NONE] {ResizingPolicy} : Resizing policy for Y.
 *   - @field [horizontalAlign=HorizontalAlignment.LEFT] {HorizontalAlignment} : X anchor interpretation.
 *   - @field [verticalAlign=VerticalAlignment.TOP_BASELINE] {VerticalAlignment} : Y anchor interpretation.
 *   - @field [keepTools=false] {boolean} : Retain temporary sketch bodies from textBounds.
 */
export function skTextAt(context is Context, id is Id, entityId is string, sketch is Sketch, text is string, position is Vector, baselineHeight is ValueWithUnits, options is map) returns map {
  const opts = mergeMaps({
    "fontName":            FontName.OPEN_SANS_REGULAR,
    "baselineHeight":      baselineHeight,
    "keepTools":           false,
    "horizontalAlign":     HorizontalAlignment.PADDED_CENTER,
    "verticalAlign":       VerticalAlignment.ACTUAL_BTM,
    "horizontalResizing":  ResizingPolicy.DOWNSCALE,
    "verticalResizing":    ResizingPolicy.SHRINK,
  }, options);
  // Measure natural text geometry at the nominal baselineHeight
  const textCoords = textBounds(context, id + "textBounds", text, opts);

  const vSizingDriver  = vSizingDriverFor(opts.verticalAlign);
  const rawYSize = (
      (vSizingDriver  == VSizingDriver.ACTUAL)    ? textCoords.actualBox.size1
    : ((vSizingDriver == VSizingDriver.STABLE)    ? textCoords.stableBox.size1
    : ((vSizingDriver == VSizingDriver.CAPHEIGHT) ? textCoords.layoutBox.size1
    : textCoords.paddedBox.size1))
  );
  const rawXSize   = (hSizingDriverFor(opts.horizontalAlign) == HSizingDriver.PADDED) ? textCoords.paddedBox.size0 : textCoords.actualBox.size0;
  const rawSize    = vector(rawXSize, rawYSize);
  const targetSize = ifNil(opts.bounds, rawSize);
  const factors    = resizingFactors(rawSize, targetSize, { "horizontalResizing": opts.horizontalResizing, "verticalResizing": opts.verticalResizing });
  const scale0     = factors.scale0;
  const scale1     = factors.scale1;

  const inputHeight  = opts.baselineHeight * scale1;

  debug(context, [opts.horizontalResizing, opts.verticalResizing,
  opts.verticalResizing == ResizingPolicy.FOLLOW,
  opts.verticalResizing == StretchingPolicy.FOLLOW,
  (opts.verticalResizing as StretchingPolicy) == StretchingPolicy.FOLLOW,
  (opts.verticalResizing as ResizingPolicy)   == ResizingPolicy.FOLLOW
  ]);

  // Horizontal offset: position the named x-anchor of the scaled text at position[0]
  var shift0 = 0 * mm;
  if        (opts.horizontalAlign == HorizontalAlignment.PADDED_LEFT) {
    shift0 = -textCoords.paddedBox.min0;
  } else if (opts.horizontalAlign == HorizontalAlignment.PADDED_CENTER) {
    shift0 = -textCoords.paddedBox.midvec[0];
  } else if (opts.horizontalAlign == HorizontalAlignment.PADDED_RIGHT) {
    shift0 = -textCoords.paddedBox.max0;
  } else if (opts.horizontalAlign == HorizontalAlignment.ACTUAL_LEFT) {
    shift0 = -textCoords.actualBox.min0;
  } else if (opts.horizontalAlign == HorizontalAlignment.ACTUAL_CENTER) {
    shift0 = -textCoords.actualBox.midvec[0];
  } else if (opts.horizontalAlign == HorizontalAlignment.ACTUAL_RIGHT) {
    shift0 = -textCoords.actualBox.max0;
  }
  shift0 = shift0 * scale1;

  // Vertical offset: position the named y-anchor of the scaled text at position[1]
  var shift1 = 0 * mm;
  if (opts.verticalAlign == VerticalAlignment.STABLE_TOP) {
    shift1 = -textCoords.stableBox.max1;
  } else if (opts.verticalAlign == VerticalAlignment.STABLE_MID) {
    shift1 = -textCoords.stableBox.midvec[1];
  } else if (opts.verticalAlign == VerticalAlignment.STABLE_BTM) {
    shift1 = -textCoords.stableBox.min1;
  } else if (opts.verticalAlign == VerticalAlignment.ACTUAL_TOP) {
    shift1 = -textCoords.actualBox.max1;
  } else if (opts.verticalAlign == VerticalAlignment.ACTUAL_MID) {
    shift1 = -textCoords.actualBox.midvec[1];
  } else if (opts.verticalAlign == VerticalAlignment.ACTUAL_BTM) {
    shift1 = -textCoords.actualBox.min1;
  } else if (opts.verticalAlign == VerticalAlignment.CAPHEIGHT) {
    shift1 = -textCoords.layoutBox.max1;
  } else if (opts.verticalAlign == VerticalAlignment.BASELINE) {
    shift1 = -textCoords.layoutBox.min1;
  }
  shift1 = shift1 * scale1;

  const firstCorner = position + vector(shift0, shift1);
  skBasicTextAt(context, entityId, sketch, text, firstCorner, inputHeight, opts);

  // debug(context, ['skTextAt', boxmSimply(targetSize), boxmSimply(rawSize), opts.horizontalResizing, opts.verticalResizing, shift0/mm, shift1/mm, scale0, scale1, boxmSimply(firstCorner), inputHeight/mm, opts]);

  return {
    "firstCorner":    firstCorner,
    "baselineHeight": inputHeight,
    "scaling":        { "shift0": shift0, "shift1": shift1, "scale0": scale0, "scale1": scale1 },
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
export function skBasicTextAt(context is Context, entityId is string, sketch is Sketch, text is string, firstCorner is Vector, baselineHeight is ValueWithUnits, opts is map) {
  const fontNameKey = ifNil(opts.fontName, FontName.OPEN_SANS_REGULAR);
  skText(sketch, entityId, {
    "text": text, fontName: FontNameString[fontNameKey], "firstCorner": firstCorner, secondCorner: firstCorner + vector(1*mm, baselineHeight),
  });
}

// --

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
  const ids = { actualSk: prefix + "Actual", stableSk: prefix + "Stable", deleteText: id + "deleteSketch", deleteMax: id + "deleteMax", deleteMin: id + "deleteMin", cleanup: prefix + "cleanup" };
  const sketches = {
    "actual": newSketchOnPlane(context, ids.actualSk,  { "sketchPlane": PL_TOP }),
    "stable": newSketchOnPlane(context, ids.stableSk,  { "sketchPlane": PL_TOP }),
  };
  // Draw the text
  const stabilizedText = TEXT_STABILIZER ~ text ~ TEXT_STABILIZER; // sandwich text to handle multiline -- text must have equivalent line count to have equivalent stable extent
  skBasicTextAt(context, "actualBounds", sketches.actual,  text,           opts.position, opts.baselineHeight, opts);
  skBasicTextAt(context, "stableBounds", sketches.stable,  stabilizedText, opts.position, opts.baselineHeight, opts);
  skSolve(sketches.actual);
  skSolve(sketches.stable);
  const actualSkBodiesQ  = qCreatedBy(ids.actualSk, EntityType.BODY);
  const actualSkRegionQ  = qSketchRegion(ids.actualSk, true);
  const stableSkRegionQ  = qSketchRegion(ids.stableSk, true);
  //
  // Max box (tight against the actual text region rendering both tall letters (l,|,`, etc) and low letters (j,y,;,Q,etc))
  const stableMeasurer   = evBox3d(context, { "topology": stableSkRegionQ,   "tight": true });

  // Padded Box (width includes horizontal padding, height spans actual extent of text)
  const paddedBox        = boxmForBbox(evBox3d(context, { "topology": actualSkBodiesQ,  "tight": true }));
  // Original Sketch Text Box (includes horizontal padding; extends from baseline to cap height)
  const layoutBox          = boxmForXYZ(zero, paddedBox.max0, zero, opts.baselineHeight);
  // Actual Bounding box (tight against the actual text region as rendered)
  const actualBox        = boxmForBbox(evBox3d(context, { "topology": actualSkRegionQ,   "tight": true }));
  // Stable box (actual width of text, and height of tallest/lowest letters in font (l,|,Å, etc // {,},j,y, etc)
  const stableBox       = boxmForXYZ(actualBox.min0, actualBox.max0, stableMeasurer.minCorner[1], stableMeasurer.maxCorner[1]);

  const result = {
    "layoutBox": layoutBox,
    "paddedBox": paddedBox,
    "actualBox": actualBox,
    "stableBox": stableBox,
  };
  opDeleteBodies(context, ids.cleanup, { "entities": qSketchFilter(qCreatedBy(id), SketchObject.YES) });
  return result;
}

// == [Resizing Text] ==

/**
 * Per-dimension ratios of `rawSize` to `targetSize`, plus the extremes.
 * A `ratio` > 1 means the bounds is larger than the original in that dimension.
 * @param rawSize    {Vector} : Original 2-D size.
 * @param targetSize {Vector} : Target 2-D size.
 */
export function resizingRatios(rawSize is Vector, targetSize is Vector) returns map {
  const ratio0 = targetSize[0] / rawSize[0];
  const ratio1 = targetSize[1] / rawSize[1];
  return {
    "ratio0":        ratio0,
    "ratio1":        ratio1,
    "largestRatio":  max(ratio0, ratio1),
    "smallestRatio": min(ratio0, ratio1),
  };
}

/* Per-axis scale factor for a single independent ResizingPolicy. FOLLOW resolved by caller. */
function scaleForPolicy(policy is ResizingPolicy, ratio is number) returns number {
  if (policy == ResizingPolicy.FREE)      { return 1.0; }
  if (policy == ResizingPolicy.FORCE)     { return ratio; }
  if (policy == ResizingPolicy.GROW)      { return max(1.0, ratio); }
  if (policy == ResizingPolicy.SHRINK)    { return min(1.0, ratio); }
  return 1.0;
}

/**
 * Scale factor vector for the given per-axis `ResizingPolicy` pair, derived from `baseFactors`.
 * Dimension-coupled policies (CONTAIN, COVER, DOWNSCALE, MAXIMIZE) apply the same uniform factor
 * to both axes; per-axis policies are resolved independently, with FOLLOW inheriting the other axis.
 * @param baseFactors {map} : Output of @see `resizingRatios`.
 * @param horizontalResizing {ResizingPolicy} : Policy for dimension 0 (X).
 * @param verticalResizing   {ResizingPolicy} : Policy for dimension 1 (Y).
 */
export function resizingFactorsFor(baseFactors is map, horizontalPolicy is ResizingPolicy, verticalPolicy is ResizingPolicy) returns Vector {
  const r0 = baseFactors.ratio0;
  const r1 = baseFactors.ratio1;
  const lr = baseFactors.largestRatio;
  const sr = baseFactors.smallestRatio;
  // Dimension-coupled policies — both axes share the same uniform factor
  if (horizontalPolicy == ResizingPolicy.CONTAINED) { return vector(    sr,         sr);     }
  if (horizontalPolicy == ResizingPolicy.COVERS)    { return vector(    lr,         lr);     }
  if (horizontalPolicy == ResizingPolicy.DOWNSCALE) { return vector(min(sr, 1), min(sr, 1)); }
  if (horizontalPolicy == ResizingPolicy.UPSCALE)   { return vector(max(lr, 1), max(lr, 1)); }
  // Per-axis independent policies; resolve FOLLOW after computing the other axis
  var sf0 = scaleForPolicy(horizontalPolicy, r0);
  var sf1 = scaleForPolicy(verticalPolicy, r1);
  if (horizontalPolicy == ResizingPolicy.FOLLOW) { sf0 = sf1; }
  if (verticalPolicy   == ResizingPolicy.FOLLOW) { sf1 = sf0; }
  return vector(sf0, sf1);
}

/**
 * Resizing result for `rawSize` scaled into `targetSize` under the given per-axis policies.
 * Returns `rawSize`, `targetSize`, and `scale0`/`scale` (per-axis proportion to apply to rawSize).
 * @param rawSize {Vector} : Original 2-D size.
 * @param bounds {Vector} : Target 2-D bounds.
 * @param policies {map} : Resizing policies.
 *   - @field horizontalResizing {ResizingPolicy} : Policy for dimension 0 (X).
 *   - @field verticalResizing {ResizingPolicy} : Policy for dimension 1 (Y).
 */
export function resizingFactors(rawSize is Vector, targetSize is Vector, policies is map) returns map {
  const baseFactors = resizingRatios(rawSize, targetSize);
  const scaleFactor = resizingFactorsFor(baseFactors, policies.horizontalResizing as ResizingPolicy, policies.verticalResizing as ResizingPolicy);
  return {
    "rawSize":     rawSize,
    "targetSize":  targetSize,
    "scale0":      scaleFactor[0],
    "scale1":      scaleFactor[1],
  };
}

function vSizingDriverFor(verticalAlignment is VerticalAlignment) returns VSizingDriver {
    if        ((verticalAlignment == VerticalAlignment.ACTUAL_TOP) || (verticalAlignment == VerticalAlignment.ACTUAL_MID)  || (verticalAlignment == VerticalAlignment.ACTUAL_BTM)) {
        return VSizingDriver.ACTUAL;
    } else if ((verticalAlignment == VerticalAlignment.STABLE_TOP) || (verticalAlignment == VerticalAlignment.STABLE_MID)  || (verticalAlignment == VerticalAlignment.STABLE_BTM)) {
        return VSizingDriver.STABLE;
    } else if ((VerticalAlignment == VerticalAlignment.CAPHEIGHT)) {
        return VSizingDriver.STABLE;
    }
    return VSizingDriver.STABLE;
}

function hSizingDriverFor(horizontalAlignment is HorizontalAlignment) returns HSizingDriver {
    if        ((horizontalAlignment == HorizontalAlignment.PADDED_LEFT) || (horizontalAlignment == HorizontalAlignment.PADDED_CENTER)  || (horizontalAlignment == HorizontalAlignment.PADDED_RIGHT)) {
        return HSizingDriver.PADDED;
    }
    return HSizingDriver.RENDERED;
}

function horizAlignmentShift(horizontalAlign is HorizontalAlignment) returns number {
  if        (horizontalAlign == HorizontalAlignment.ACTUAL_CENTER || horizontalAlign == HorizontalAlignment.PADDED_CENTER) {
    return (1/2);
  } else if (horizontalAlign == HorizontalAlignment.ACTUAL_RIGHT || horizontalAlign == HorizontalAlignment.PADDED_RIGHT) {
    return 1.0;
  }
  return 0;
}

function vertAlignmentShift(verticalAlign is VerticalAlignment) returns number {
  if ((verticalAlign == VerticalAlignment.ACTUAL_MID) || (verticalAlign == VerticalAlignment.STABLE_MID)) {
    return (1/2);
  } else if (verticalAlign == VerticalAlignment.ACTUAL_TOP || verticalAlign == VerticalAlignment.CAPHEIGHT || verticalAlign == VerticalAlignment.STABLE_TOP) {
    return 1.0;
  }
  return 0;
}

// --

// == [Utility Functions] ==

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
 * Unique entity id from `label`, with uniqueness scoped to `params`.
 * @param params {map} : Caller's params map; holds the per-caller counter.
 * @param label {string} : Base prefix.
 */
export function nextLabelId(params is map, label is string) returns string {
    params.idUniquer = params.idUniqer == undefined ? 0 : params.idUniqer + 1;
    return replace(label ~ "_" ~ params.idUniquer, "[^\\w]", '-');
}

// --
