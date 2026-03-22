FeatureScript 2909;
import(path : "onshape/std/geometry.fs", version : "2909.0");
export import(path : "e814a17c4e5c208c3325bba8", version : "31bdc2a06c1e490fdcc264b5");

const mm = millimeter;
const PL_TOP  = plane(WORLD_ORIGIN, Z_AXIS.direction);

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
 * Text metrics for `text`: tight bounding boxes (`tbox`, `bbox`, `wbox`), padded and
 * actual width/height, aspect ratio, overflow fraction, and descender fraction.
 * @param context {Context} : Model context.
 * @param id {Id} : Base feature id.
 * @param text {string} : Text to measure.
 * @param options {map} : keyword options
 *      - @field [fontName=FontName.OPEN_SANS_REGULAR] {FontName} : Font filename.
 *      - @field [baselineHeight=10mm] {ValueWithUnits} : Nominal cap height.
 *      - @field [keepTools=false] {boolean} : Retain the temporary sketch body.
 */
export function textBounds(context is Context, id is Id, text is string, options is map) returns map {
  const opts = mergeMaps({
      "fontName":       FontName.OPEN_SANS_REGULAR,
      "baselineHeight": 10*mm,
      "keepTools":      false,
  }, options);
  const tempSkId  = id + nextLabelId(opts, "tempSketch" ~ text);
  const sketch    = newSketchOnPlane(context, tempSkId + "text", { "sketchPlane": PL_TOP });
  const maxSketch = newSketchOnPlane(context, tempSkId + "max",  { "sketchPlane": PL_TOP });
  const minSketch = newSketchOnPlane(context, tempSkId + "min",  { "sketchPlane": PL_TOP });
  // Draw the text
  skBasicTextAt(context, "textBounds", sketch,    text,                     vector(0 * mm, 0 * mm), opts.baselineHeight, opts);
  skBasicTextAt(context, "maxBounds",  maxSketch, "'[}lLTQZ96|^$§`" ~ text ~ "gjpqyQ;,", vector(0 * mm, 0 * mm), opts.baselineHeight, opts);
  skBasicTextAt(context, "minBounds",  minSketch, "x",                      vector(0 * mm, 0 * mm), opts.baselineHeight, opts);
  skSolve(sketch); skSolve(maxSketch); skSolve(minSketch);
  const sketchBodies       = qCreatedBy(tempSkId + "text", EntityType.BODY);
  const maxBodies          = qCreatedBy(tempSkId + "max",  EntityType.BODY);
  const minBodies          = qCreatedBy(tempSkId + "min",  EntityType.BODY);
  //
  // Wide box (includes horizontal padding and actual extent of text)
  const wbox             = evBox3d(context, { "topology": sketchBodies,                    "tight": true });
  // Text box (includes horizontal padding; extends from baseline to cap height)
  const tbox             = box3d(vector(0*mm, 0*mm, 0*mm), vector(wbox.maxCorner[0], opts.baselineHeight, 0*mm));
  // Bounding box (tight against the actual text region as rendered)
  const bbox             = evBox3d(context, { "topology": qSketchRegion(tempSkId + "text", true), "tight": true });
  // Max box (tight against the actual text region rendering both tall letters (l,|,`, etc) and low letters (j,y,;,Q,etc))
  const ylMeasurer       = evBox3d(context, { "topology": qSketchRegion(tempSkId + "max", true), "tight": true });
  // Min box (tight against the actual text region using only "x")
  const xMeasurer        = evBox3d(context, { "topology": qSketchRegion(tempSkId + "min", true), "tight": true });
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
  };
  opDeleteBodies(context, id + "deleteSketch",  { "entities": sketchBodies });
  opDeleteBodies(context, id + "deleteMax",     { "entities": maxBodies });
  opDeleteBodies(context, id + "deleteMin",     { "entities": minBodies });
  debug(context, ["textBounds tbox", text, boxMag(tbox, 1*mm)]);
  debug(context, ["textBounds wbox", text, boxMag(wbox, 1*mm)]);
  debug(context, ["textBounds bbox", text, boxMag(bbox, 1*mm)]);
  debug(context, ["textBounds  min",  text, boxMag(minBbox, 1*mm)]);
  debug(context, ["textBounds  max",  text, boxMag(maxBbox, 1*mm)]);
  return result;
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
  debug(context, [opts.fontName, FontName.ARIMO, FontNameString[opts.fontName]]);
  skText(sketch, entityId, {
    "text": text, fontName: FontNameString[opts.fontName], "firstCorner": firstCorner, secondCorner: firstCorner + vector(1*mm, baselineHeight),
  });
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
  debug(context, ["measureText3d", boxMag(textCoords.tbox, 1*mm), boxMag(textCoords.bbox, 1*mm)], DebugColor.CYAN);
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

  setProperty(context, {
    "entities":     extrudedBodies,
    "propertyType": PropertyType.NAME,
    // "value":        definition.text,
    "value":        "AR: " ~ replace(substring(toString(round(textCoords.aspectRatio, 0.1)), 0, 3), "(\\.?0+$|0+$)", "") ~ " df: " ~ substring(toString(round(textCoords.descenderFrac, 0.1)), 0, 3),
  });
});

/**
 * Unique entity id from `label`, with uniqueness scoped to `params`.
 * @param params {map} : Caller's params map; holds the per-caller counter.
 * @param label {string} : Base prefix.
 */
export function nextLabelId(params is map, label is string) returns string {
    params.idUniquer = params.idUniqer == undefined ? 0 : params.idUniqer + 1;
    return replace(label ~ "_" ~ params.idUniquer, "[^\\w]", '-');
}


// // Pangram character array for use in LUT building (buildTextWidthTableFor, coming later).
// const pangram      = "How quickly daft jumping zebras vex";
// const pangramChars = ["H", "o", "w", " ", "q", "u", "i", "c", "k", "l", "y", " ", "d", "a", "f", "t",
//                       " ", "j", "u", "m", "p", "i", "n", "g", " ", "z", "e", "b", "r", "a", "s",
//                       " ", "v", "e", "x"];

// // Printable ASCII from 0x20 (space) to 0x7e (tilde); 0x7f (DEL) has no glyph and is omitted.
// const asciiChars = [" ", "!", "\"", "#", "$", "%", "&", "'", "(", ")", "*", "+", ",", "-", ".", "/",
//                     "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ":", ";", "<", "=", ">", "?",
//                     "@", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O",
//                     "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "[", "\\", "]", "^", "_",
//                     "`", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o",
//                     "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "{", "|", "}", "~"];

export enum VerticalAlignment {
  annotation { "Name": "Top of the tallest letter" }
  MAX,
  annotation { "Name": "Top of the text, as rendered" }
  TOP,
  annotation { "Name": "Nominal cap height" }
  NOMINAL_CAP,
  annotation { "Name": "Center of the text, as rendered" }
  CENTER,
  annotation { "Name": "Baseline of the text" }
  BASELINE,
  annotation { "Name": "Bottom of the text, as rendered" }
  BOTTOM,
  annotation { "Name": "Bottom of the lowest-hanging letter" }
  MIN,
}

export enum HorizontalAlignment {
  annotation { "Name": "Left of the text, including padding" }
  MIN,
  annotation { "Name": "Left of the text, as rendered" }
  LEFT,
  annotation { "Name": "Center of the text, as rendered" }
  CENTER,
  annotation { "Name": "Center of the text, including padding" }
  CENTER_NOMINAL,
  annotation { "Name": "Right of the text, as rendered" }
  RIGHT,
  annotation { "Name": "Right of the text, including padding" }
  MAX,
}

export enum ResizingPolicy {
  // Dimension-independent policies: can mix and match these

  /** Preserve original dimensions; no resizing. */
  annotation { "Name": "No resizing (preserve original)" }
  NONE,
  /** Shrink to fit the available space if larger, but never expand. Affects only this dimension (not proportional). */
  annotation { "Name": "Constrain maximum size (shrink only)" }
  LIMIT,
  /** Expand to fill the available space if smaller, but never shrink. Affects only this dimension (not proportional). */
  annotation { "Name": "Constrain minimum size (expand only)" }
  EMBIGGEN,
  /** Stretch to fill the bounds exactly, ignoring aspect ratio. */
  annotation { "Name": "Stretch to fill (ignore aspect ratio)" }
  FILL,
  /** Scale proportionally to match the scaling factor of the other dimension. Maintains aspect ratio (e.g., height follows width changes). */
  annotation { "Name": "Follow other dimension (maintain aspect ratio)" }
  FOLLOW,

  // Dimension-dependent policies: resizing1 must be undefined or equal to resizing0

  /** Resize proportionally to fit entirely within bounds, with at least one dimension fitting exactly. May shrink or expand. */
  annotation { "Name": "Grow/Shrink proportionally to fit inside bounds" }
  CONTAIN,
  /** Resize proportionally to fill bounds, with at least one dimension fitting exactly. May shrink or expand. */
  annotation { "Name": "Grow/Shrink proportionally to cover bounds completely" }
  COVER,
  /** Shrink proportionally to fit within bounds, but never expand. */
  annotation { "Name": "Shrink until both fit (but never enlarge)" }
  DOWNSCALE,
  /** Grow proportionally to fill bounds, but never shrink */
  annotation { "Name": "Grow until both fit (but never shrink)" }
  MAXIMIZE,

}

export enum FontName {
  annotation { "Name": "Open Sans Regular (a good default)" }
  OPEN_SANS_REGULAR,
  annotation {"Name": "Open Sans Bold" }
  OPEN_SANS_BOLD,
  annotation {"Name": "Open Sans Italic" }
  OPEN_SANS_ITALIC,
  annotation { "Name": "Allerta (no bold/italic options)" }
  ALLERTA,
  annotation { "Name": "Allerta Stencil (no bold/italic options)" }
  ALLERTA_STENCIL,
  annotation { "Name": "Arimo (sans-serif font)" }
  ARIMO,
  annotation { "Name": "Arimo Bold" }
  ARIMO_BOLD,
  annotation { "Name": "Arimo Italic" }
  ARIMO_ITALIC,
  annotation { "Name": "Balthazar (no bold/italic options)" }
  BALTHAZAR,
  annotation { "Name": "Baumans (no bold/italic options)" }
  BAUMANS,
  annotation { "Name": "Bebas Neue (no bold/italic options)" }
  BEBAS_NEUE,
  annotation { "Name": "Comic Neue" }
  COMIC_NEUE,
  annotation { "Name": "Comic Neue Bold" }
  COMIC_NEUE_BOLD,
  annotation { "Name": "Comic Neue Italic" }
  COMIC_NEUE_ITALIC,
  annotation { "Name": "Courier Prime (monospaced)" }
  COURIER_PRIME,
  annotation { "Name": "Courier Prime Bold" }
  COURIER_PRIME_BOLD,
  annotation { "Name": "Courier Prime Italic" }
  COURIER_PRIME_ITALIC,
  annotation { "Name": "Didact Gothic (no bold/italic options)" }
  DIDACT_GOTHIC,
  annotation { "Name": "Droid Sans Mono (monospaced sans-serif font, no bold/italic options)" }
  DROID_SANS_MONO,
  annotation { "Name": "Inconsolata (monospaced sans-serif font)" }
  INCONSOLATA,
  annotation { "Name": "Inconsolata Bold" }
  INCONSOLATA_BOLD,
  annotation { "Name": "Inter (sans-serif font, no italic options)" }
  INTER,
  annotation { "Name": "Inter Bold" }
  INTER_BOLD,
  annotation { "Name": "Michroma (sans-serif font, no italic options)" }
  MICHROMA,
  annotation { "Name": "MPLUSRounded1c (sans-serif font, no italic options)" }
  MPLUSRounded1c,
  annotation { "Name": "MPLUSRounded1c Bold" }
  MPLUSRounded1c_BOLD,
  annotation { "Name": "Noto Sans (sans-serif font)" }
  NOTO_SANS,
  annotation { "Name": "Noto Sans Bold" }
  NOTO_SANS_BOLD,
  annotation { "Name": "Noto Sans Italic" }
  NOTO_SANS_ITALIC,
  annotation { "Name": "Noto Sans CJK JP (Japanese font, no italic options)" }
  NOTO_SANS_CJK_JP,
  annotation { "Name": "Noto Sans CJK JP Bold" }
  NOTO_SANS_CJK_JP_BOLD,
  annotation { "Name": "Noto Sans CJK KR (Korean font, no italic options)" }
  NOTO_SANS_CJK_KR,
  annotation { "Name": "Noto Sans CJK KR Bold" }
  NOTO_SANS_CJK_KR_BOLD,
  annotation { "Name": "Noto Sans CJK SC (Chinese (simplified) font, no italic options)" }
  NOTO_SANS_CJK_SC,
  annotation { "Name": "Noto Sans CJK SC Bold" }
  NOTO_SANS_CJK_SC_BOLD,
  annotation { "Name": "Noto Sans CJK TC (Chinese (traditional) font, no italic options)" }
  NOTO_SANS_CJK_TC,
  annotation { "Name": "Noto Sans CJK TC Bold" }
  NOTO_SANS_CJK_TC_BOLD,
  annotation { "Name": "Noto Serif (serif font)" }
  NOTO_SERIF,
  annotation { "Name": "Noto Serif Bold" }
  NOTO_SERIF_BOLD,
  annotation { "Name": "Noto Serif Italic" }
  NOTO_SERIF_ITALIC,
  annotation { "Name": "Orbitron (sans-serif font, no italic options)" }
  ORBITRON,
  annotation { "Name": "Orbitron Bold" }
  ORBITRON_BOLD,
  annotation { "Name": "Oswald (sans-serif font, no italic options)" }
  OSWALD,
  annotation { "Name": "Oswald Bold" }
  OSWALD_BOLD,
  annotation { "Name": "Poppins (sans-serif font)" }
  POPPINS,
  annotation { "Name": "Poppins Bold" }
  POPPINS_BOLD,
  annotation { "Name": "Poppins Italic" }
  POPPINS_ITALIC,
  annotation { "Name": "PTSans (sans-serif font)" }
  PTSANS,
  annotation { "Name": "PTSans Bold" }
  PTSANS_BOLD,
  annotation { "Name": "PTSans Italic" }
  PTSANS_ITALIC,
  annotation { "Name": "Rajdhani (sans-serif font, no italic options)" }
  RAJDHANI,
  annotation { "Name": "Rajdhani Bold" }
  RAJDHANI_BOLD,
  annotation { "Name": "Roboto (sans-serif font)" }
  ROBOTO,
  annotation { "Name": "Roboto Bold" }
  ROBOTO_BOLD,
  annotation { "Name": "Roboto Italic" }
  ROBOTO_ITALIC,
  annotation { "Name": "Roboto Slab (sans-serif font, no italic options)" }
  ROBOTO_SLAB,
  annotation { "Name": "Roboto Slab Bold" }
  ROBOTO_SLAB_BOLD,
  annotation { "Name": "Ropa Sans (sans-serif font, no bold options)" }
  ROPA_SANS,
  annotation { "Name": "Ropa Sans Italic" }
  ROPA_SANS_ITALIC,
  annotation { "Name": "Sofia Sans (sans-serif font)" }
  SOFIA_SANS,
  annotation { "Name": "Sofia Sans Bold" }
  SOFIA_SANS_BOLD,
  annotation { "Name": "Sofia Sans Italic" }
  SOFIA_SANS_ITALIC,
  annotation { "Name": "Source Sans Pro (sans-serif font)" }
  SOURCE_SANS_PRO,
  annotation { "Name": "Source Sans Pro Bold" }
  SOURCE_SANS_PRO_BOLD,
  annotation { "Name": "Source Sans Pro Italic" }
  SOURCE_SANS_PRO_ITALIC,
  annotation { "Name": "Tinos (serif font, metrically compatible with Times New Roman)" }
  TINOS,
  annotation { "Name": "Tinos Bold" }
  TINOS_BOLD,
  annotation { "Name": "Tinos Italic" }
  TINOS_ITALIC,
 }

 export const FontNameString = {
  FontName.OPEN_SANS_REGULAR:     "OpenSans-Regular.ttf",
  FontName.OPEN_SANS_BOLD:        "OpenSans-Bold.ttf",
  FontName.OPEN_SANS_ITALIC:      "OpenSans-Italic.ttf",
  FontName.ALLERTA:               "Allerta-Regular.ttf",
  FontName.ALLERTA_STENCIL:       "AllertaStencil-Regular.ttf",
  FontName.ARIMO:                 "Arimo-Regular.ttf",
  FontName.ARIMO_BOLD:            "Arimo-Bold.ttf",
  FontName.ARIMO_ITALIC:          "Arimo-Italic.ttf",
  FontName.BALTHAZAR:             "Balthazar-Regular.ttf",
  FontName.BAUMANS:               "Baumans-Regular.ttf",
  FontName.BEBAS_NEUE:            "BebasNeue-Regular.ttf",
  FontName.COMIC_NEUE:            "ComicNeue-Regular.ttf",
  FontName.COMIC_NEUE_BOLD:       "ComicNeue-Bold.ttf",
  FontName.COMIC_NEUE_ITALIC:     "ComicNeue-Italic.ttf",
  FontName.COURIER_PRIME:         "CourierPrime-Regular.ttf",
  FontName.COURIER_PRIME_BOLD:    "CourierPrime-Bold.ttf",
  FontName.COURIER_PRIME_ITALIC:  "CourierPrime-Italic.ttf",
  FontName.DIDACT_GOTHIC:         "DidactGothic-Regular.ttf",
  FontName.DROID_SANS_MONO:       "DroidSansMono.ttf",
  FontName.INCONSOLATA:           "Inconsolata-Regular.ttf",
  FontName.INCONSOLATA_BOLD:      "Inconsolata-Bold.ttf",
  FontName.INTER:                 "Inter-Regular.ttf",
  FontName.INTER_BOLD:            "Inter-Bold.ttf",
  FontName.MICHROMA:              "Michroma-Regular.ttf",
  FontName.MPLUSRounded1c:        "MPLUSRounded1c-Regular.ttf",
  FontName.MPLUSRounded1c_BOLD:   "MPLUSRounded1c-Bold.ttf",
  FontName.NOTO_SANS:             "NotoSans-Regular.ttf",
  FontName.NOTO_SANS_BOLD:        "NotoSans-Bold.ttf",
  FontName.NOTO_SANS_ITALIC:      "NotoSans-Italic.ttf",
  FontName.NOTO_SANS_CJK_JP:      "NotoSansCJKjp-Regular.otf",
  FontName.NOTO_SANS_CJK_JP_BOLD: "NotoSansCJKjp-Bold.otf",
  FontName.NOTO_SANS_CJK_KR:      "NotoSansCJKkr-Regular.otf",
  FontName.NOTO_SANS_CJK_KR_BOLD: "NotoSansCJKkr-Bold.otf",
  FontName.NOTO_SANS_CJK_SC:      "NotoSansCJKsc-Regular.otf",
  FontName.NOTO_SANS_CJK_SC_BOLD: "NotoSansCJKsc-Bold.otf",
  FontName.NOTO_SANS_CJK_TC:      "NotoSansCJKtc-Regular.otf",
  FontName.NOTO_SANS_CJK_TC_BOLD: "NotoSansCJKtc-Bold.otf",
  FontName.NOTO_SERIF:            "NotoSerif-Regular.ttf",
  FontName.NOTO_SERIF_BOLD:       "NotoSerif-Bold.ttf",
  FontName.NOTO_SERIF_ITALIC:     "NotoSerif-Italic.ttf",
  FontName.ORBITRON:              "Orbitron-Regular.ttf",
  FontName.ORBITRON_BOLD:         "Orbitron-Bold.ttf",
  FontName.OSWALD:                "Oswald-Regular.ttf",
  FontName.OSWALD_BOLD:           "Oswald-Bold.ttf",
  FontName.POPPINS:               "Poppins-Regular.ttf",
  FontName.POPPINS_BOLD:          "Poppins-Bold.ttf",
  FontName.POPPINS_ITALIC:        "Poppins-Italic.ttf",
  FontName.PTSANS:                "PTSans-Regular.ttf",
  FontName.PTSANS_BOLD:           "PTSans-Bold.ttf",
  FontName.PTSANS_ITALIC:         "PTSans-Italic.ttf",
  FontName.RAJDHANI:              "Rajdhani-Regular.ttf",
  FontName.RAJDHANI_BOLD:         "Rajdhani-Bold.ttf",
  FontName.ROBOTO:                "Roboto-Regular.ttf",
  FontName.ROBOTO_BOLD:           "Roboto-Bold.ttf",
  FontName.ROBOTO_ITALIC:         "Roboto-Italic.ttf",
  FontName.ROBOTO_SLAB:           "RobotoSlab-Regular.ttf",
  FontName.ROBOTO_SLAB_BOLD:      "RobotoSlab-Bold.ttf",
  FontName.ROPA_SANS:             "RopaSans-Regular.ttf",
  FontName.ROPA_SANS_ITALIC:      "RopaSans-Italic.ttf",
  FontName.SOFIA_SANS:            "SofiaSans-Regular.ttf",
  FontName.SOFIA_SANS_BOLD:       "SofiaSans-Bold.ttf",
  FontName.SOFIA_SANS_ITALIC:     "SofiaSans-Italic.ttf",
  FontName.SOURCE_SANS_PRO:       "SourceSansPro-Regular.ttf",
  FontName.SOURCE_SANS_PRO_BOLD:   "SourceSansPro-Bold.ttf",
  FontName.SOURCE_SANS_PRO_ITALIC: "SourceSansPro-Italic.ttf",
  FontName.TINOS:                 "Tinos-Regular.otf",
  FontName.TINOS_BOLD:            "Tinos-Bold.ttf",
  FontName.TINOS_ITALIC:          "Tinos-Italic.ttf",
 };
