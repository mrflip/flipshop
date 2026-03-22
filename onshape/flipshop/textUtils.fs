FeatureScript 2909;
import(path : "onshape/std/geometry.fs", version : "2909.0");
export import(path : "e814a17c4e5c208c3325bba8", version : "31bdc2a06c1e490fdcc264b5");

const mm = millimeter;
// const xx = line(WORLD_ORIGIN, WORLD_ORIGIN);
const PL_TOP  = plane(WORLD_ORIGIN, Z_AXIS.direction);

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

// /** Draws text on sketch, solves it, queries the resulting edges, and debugs their bounding box.
//  * id must be the feature Id used to create sketch (used to query created entities).
//  * textHeight sets the font size; firstCorner is the bottom-left anchor of the text box.
//  */
// function drawAndMeasureText(context is Context, sketchId is Id, sketch is Sketch, text is string, textHeight, firstCorner is Vector) {
//   const estimatedWidth = textHeight * 0.65 * length(text);
//   skText(sketch, "text", {
//     "fontName":      "OpenSans-Regular.ttf",
//     "firstCorner":   firstCorner,
//     "secondCorner":  firstCorner + vector(estimatedWidth, textHeight),
//     "text":          text,
//   });
//   skSolve(sketch);
//   const textEnts = sketchEntityQuery(sketchId, EntityType.EDGE, "text");
//   const xx = qCreatedBy(sketchId, EntityType.BODY);
//   debug(context, [textEnts, xx, 1], DebugColor.RED);
//   const textBounds = evBox3d(context, { "topology": xx, "tight": true });
//   debug(context, [textBounds, 2]);
// }

/**
 * Creates a new sketch on the same plane as `params.basePlane` but with its X axis
 * rotated by `angle` around the plane normal.  Used to draw label text at an independent
 * angle from the main geometry sketches.
 * @param context {Context} : The model context.
 * @param id {Id} : Sketch feature id.
 * @param params {map} : Must contain `basePlane` (Plane).
 * @param angle {ValueWithUnits} : In-plane rotation angle for the sketch X axis.
 */
export function rotatedSketch(context is Context, id is Id, params is map, angle is ValueWithUnits) returns Sketch {
  const  basePlane    = params.basePlane;
  const  rotatedX     = cos(angle) * basePlane.x + sin(angle) * cross(basePlane.normal, basePlane.x);
  const  rotatedPlane = plane(basePlane.origin, basePlane.normal, rotatedX);
  return newSketchOnPlane(context, id, { "sketchPlane": rotatedPlane });
}

/**
 * Draws `text` in a temporary sketch on the global XY plane, solves it, and returns a
 * map of text metrics: tight bounding boxes (`tbox`, `bbox`, `wbox`), padded and actual
 * width/height, aspect ratio, overflow fraction, and descender fraction.  The temporary
 * sketch body is deleted before returning unless `overrides.keepTools` is `true`.
 * @param context {Context} : The model context.
 * @param id {Id} : Base feature id; temp sketch id is derived from this.
 * @param text {string} : The text string to measure.
 * @param overrides {map} : @optional Overrides merged over defaults:
 *      `fontName` (string, default `"OpenSans-Regular.ttf"`),
 *      `baselineHeight` (ValueWithUnits, default `10 * mm`),
 *      `keepTools` (boolean, default `false`).
 */
export function textBounds(context is Context, id is Id, text is string, overrides is map) returns map {
  const args = mergeMaps({
      "fontName":       "OpenSans-Regular.ttf",
      "baselineHeight": 10*mm,
      "keepTools":      false,
  }, overrides);
  const tempSkId  = id + nextLabelId(args, "tempSketch" ~ text);
  const sketch    = newSketchOnPlane(context, tempSkId, { "sketchPlane": PL_TOP });
  // Draw the text
  skTextAt(context, "textBounds", sketch, text, vector(0 * mm, 0 * mm), args.baselineHeight, args);
  skSolve(sketch);
  const sketchBody       = qCreatedBy(tempSkId, EntityType.BODY);
  //
  // Text box (includes the margin)
  const wbox             = evBox3d(context, { "topology": sketchBody,                    "tight": true });
  const tbox             = box3d(vector(0*mm, 0*mm, 0*mm), vector(wbox.maxCorner[0], args.baselineHeight, 0*mm));
  // Bounding box (tight against the actual text region as rendered)
  const bbox             = evBox3d(context, { "topology": qSketchRegion(tempSkId, true), "tight": true });
  // Calculate the text metrics
  const paddedWidth      = tbox.maxCorner[0] - tbox.minCorner[0];
  const paddedHeight     = tbox.maxCorner[1] - tbox.minCorner[1];
  const actualWidth      = bbox.maxCorner[0] - bbox.minCorner[0];
  const actualHeight     = bbox.maxCorner[1] - bbox.minCorner[1];
  const overflowHeight   = tbox.maxCorner[1] - args.baselineHeight;
  const leftMarginWidth  = tbox.minCorner[0] - bbox.minCorner[0];
  const rightMarginWidth = bbox.maxCorner[0] - tbox.maxCorner[0];
  const result = {
    "tbox":             tbox,
    "bbox":             bbox,
    "wbox":             wbox,
    "leftMarginWidth":  leftMarginWidth, "rightMarginWidth": rightMarginWidth,
    "paddedWidth":      paddedWidth,     "paddedHeight":     paddedHeight,
    "actualWidth":      actualWidth,     "actualHeight":     actualHeight,
    "overflowHeight":   overflowHeight,
    "aspectRatio":      actualWidth / actualHeight,
    "descenderFrac":    (actualHeight - overflowHeight - args.baselineHeight) / actualHeight,
    "overflowFrac":     overflowHeight / actualHeight,
  };
  opDeleteBodies(context, id + "deleteSketch",  { "entities": sketchBody });
  debug(context, [tbox, bbox]);
  return result;
}

/**
 * Draws a text entity onto `sketch` at `firstCorner` with the given `baselineHeight`.
 * The second corner is set to `firstCorner + vector(1*mm, baselineHeight)`, so actual
 * width is determined by the font renderer.  Font defaults to `"OpenSans-Regular.ttf"`.
 * @param context {Context} : The model context (unused, reserved for future use).
 * @param entityId {string} : Sketch entity id for the text element.
 * @param sketch {Sketch} : The sketch to draw into.
 * @param text {string} : The text string to draw.
 * @param firstCorner {Vector} : 2D bottom-left anchor of the text box.
 * @param baselineHeight {ValueWithUnits} : Font size (baseline-to-cap height).
 * @param overrides {map} : @optional Overrides; supports `fontName` (string).
 */
export function skTextAt(context is Context, entityId is string, sketch is Sketch, text is string, firstCorner is Vector, baselineHeight is ValueWithUnits, overrides is map) {
  const args = mergeMaps({ "fontName": "OpenSans-Regular.ttf" }, overrides);
  skText(sketch, entityId, {
    "text": text, fontName: args.fontName, "firstCorner": firstCorner, secondCorner: firstCorner + vector(1*mm, baselineHeight),
  });
}

/**
 * Returns the rendered width of `text` at the specified baseline height, measured on the
 * global XY plane by delegating to `textBounds` and reading the actual width from the bbox.
 * @param context {Context} : The model context.
 * @param id {Id} : Base feature id passed through to `textBounds`.
 * @param text {string} : The text string to measure.
 * @param args {map} : Options passed through to `textBounds` (e.g., `fontName`, `baselineHeight`).
 */
export function measureTextWidth(context is Context, id is Id, text is string, args is map) returns ValueWithUnits {
  const  textCoords = textBounds(context, id, text, args);
  return textCoords.maxCorner[0] - textCoords.minCorner[0];
}


/**
 * Intended to return the baseline height metric for `text`; currently delegates to
 * `textBounds` and returns the same value as `measureTextWidth` (work in progress).
 * @param context {Context} : The model context.
 * @param id {Id} : Base feature id passed through to `textBounds`.
 * @param text {string} : The text string to measure.
 * @param args {map} : Options passed through to `textBounds` (e.g., `fontName`, `baselineHeight`).
 */
export function measureTextBaseline(context is Context, id is Id, text is string, args is map) returns ValueWithUnits {
  const  textCoords = textBounds(context, id, text, args);
  return textCoords.maxCorner[0] - textCoords.minCorner[0];
}

/**
 * Feature: extrudes text on a selected plane and visualizes its bounding boxes.
 * Used for measuring and debugging text metrics (padded box, tight bbox, aspect ratio,
 * descender fraction).  Produces an extruded text body plus a thin carrier plate covering
 * the tight text area, named with the measured aspect ratio and descender fraction.
 * @param definition {{
 *      @field text {string} : The text string to render and measure.
 *      @field fontName {string} : Font filename (e.g., `"OpenSans-Regular.ttf"`).
 *      @field baselineHeight {ValueWithUnits} : Font size (baseline-to-cap height).
 *      @field sketchPlaneQ {Query} : The plane on which to draw the text.
 *      @field textAngle {ValueWithUnits} : In-plane rotation angle for the text sketch.
 * }}
 */
annotation { "Feature Type Name": "Measure Text 3D" }
export const measureText3d = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Text" }
  definition.text is string;

  annotation { "Name": "Font Name" }
  definition.fontName is string;

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
  skTextAt(context, "measureText3d", sketches.textFaces, params.text, vector(0*mm, 0*mm), params.baselineHeight, params);
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
  skRectangle(sketches.carrierSk, "bbox", { "firstCorner": vector2(textCoords.bbox.minCorner), "secondCorner": vector2(textCoords.bbox.maxCorner)  });
  skRectangle(sketches.carrierSk, "tbox", { "firstCorner": vector2(textCoords.tbox.minCorner), "secondCorner": vector2(textCoords.tbox.maxCorner), construction: true });
  skPoint(sketches.carrierSk, "minCorner", { "position" : vector2(textCoords.wbox.minCorner) });
  skPoint(sketches.carrierSk, "maxCorner", { "position" : vector2(textCoords.wbox.maxCorner) });
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
    "value":        "AR: " ~ round(textCoords.aspectRatio, 0.01) ~ " df: " ~ round(textCoords.descenderFrac, 0.1),
  });
});

/**
 * Returns a unique sketch entity id string derived from `label`, with non-word characters
 * replaced by `-` and a per-call counter appended.  The counter is stored in and read
 * from `params.idUniquer`, so uniqueness is scoped to the params map passed in.
 * @param params {map} : Mutable params map used to track the counter in `idUniquer`.
 * @param label {string} : Base label prefix.
 */
export function nextLabelId(params is map, label is string) returns string {
    params.idUniquer = params.idUniqer == undefined ? 0 : params.idUniqer + 1;
    return replace(label ~ "_" ~ params.idUniquer, "[^\\w]", '-');
}
