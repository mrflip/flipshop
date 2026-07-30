
// == [Measure Text] ==

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


// export function boxMag(bb is Box3d, unit is ValueWithUnits) {
//   const min  = vecmapM(vec2Map(bb.minCorner), unit);
//   const max  = vecmapM(vec2Map(bb.maxCorner), unit);
//   const size = vecmapSubtract(max, min);
//   return { "min": min, "max": max, "size": size };
// }

// export function vec2Map(vec is Vector) {
//   return { a: vec[0], b: vec[1], c: vec[2] };
// }

// export function vecmapSubtract(vec1 is map, vec2 is map) {
//     return { a: vec1.a - vec2.a, b: vec1.b - vec2.b, c: (vec1.c == undefined || vec2.c == undefined) ? 0 : vec1.c - vec2.c };
// }

// export function vecmapM(vecmap is map, unit is ValueWithUnits) {
//   const vm2 = { a: vecmap.a / unit, b: vecmap.b / unit };
//   if (vecmap.c == undefined) { return vm2; }
//   return mergeMaps(vm2, { c: vecmap.c / unit });
// }