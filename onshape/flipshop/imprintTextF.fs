FeatureScript 3029;
import(path : "onshape/std/common.fs", version : "3029.0");
// export import(path : "onshape/std/booleanoperationtype.gen.fs", version: "2909.0");
export import(path : "dff50ab3ba4ca6e98b25b414", version : "6797c064b64f6f7eb539a857");
IconNamespace::import(path : "6f86e45900dd425ab5907742", version : "6af6fe9704e3d2b0184a4da4");

// == [Imprint Text] ==

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
annotation { "Feature Type Name": "Imprint Text", "Icon": IconNamespace::BLOB_DATA, "Feature Type Description": "Emboss, Embed or Carve text from a part", "Feature Name Template" : "E #operationType #text" }
export const imprintTextF = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Sketch Planes", "Filter": QueryFilterCompound.ALLOWS_PLANE, "MaxNumberOfPicks": 20 }
  definition.sketchPlaneQ is Query;
  annotation { "Name": "Parts", "UIHint" : [UIHint.SHOW_LABEL, UIHint.REMEMBER_PREVIOUS_VALUE], "Filter": EntityType.BODY && BodyType.SOLID, "MaxNumberOfPicks": 20 }
  definition.partQ is Query;
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
  annotation { "Name": "Text Depth", "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE] }
  isLength(definition.textDepth, { (millimeter): [0.001, 2, 1000000] } as LengthBoundSpec);
  annotation { "Name": "Plate Depth", "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE] }
  isLength(definition.plateDepth, { (millimeter): [0.001, 0.02, 1000000] } as LengthBoundSpec);
  annotation { "Name": "Horizontal Alignment", "UIHint": [UIHint.SHOW_LABEL, UIHint.REMEMBER_PREVIOUS_VALUE] }
  definition.horizontalAlign is HorizontalAlignment;
  annotation { "Name": "Vertical Alignment",    "UIHint": [UIHint.SHOW_LABEL, UIHint.REMEMBER_PREVIOUS_VALUE] }
  definition.verticalAlign is VerticalAlignment;
  annotation { "Name": "Horizontal Resizing",   "UIHint": [UIHint.SHOW_LABEL, UIHint.REMEMBER_PREVIOUS_VALUE] }
  definition.horizontalResizing is ResizingPolicy;
  annotation { "Name": "Vertical Resizing",     "UIHint": [UIHint.SHOW_LABEL, UIHint.REMEMBER_PREVIOUS_VALUE] }
  if ( (definition.horizontalResizing == ResizingPolicy.FREE)   ||  (definition.horizontalResizing == ResizingPolicy.FORCE)
    || (definition.horizontalResizing == ResizingPolicy.SHRINK) ||  (definition.horizontalResizing == ResizingPolicy.GROW)
    || (definition.horizontalResizing == ResizingPolicy.FOLLOW)) {
    annotation { "Name": "Vertical Resizing",     "UIHint": [UIHint.SHOW_LABEL, UIHint.REMEMBER_PREVIOUS_VALUE] }
    definition.verticalResizing is StretchingPolicy;
  }
  definition.operationType is EmbossOpType;
  if (definition.operationType == EmbossOpType.DEBOSS) {
    annotation { "Name": "Keep Text", "Default" : true, "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE] }
    definition.keepText is boolean;
  }
  annotation { "Name": "Cleanup sketches", "Default" : true, "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE] }
  definition.cleanupSketches is boolean;
}
{
  definition.verticalResizing = definition.verticalResizing as ResizingPolicy;
  embossTextFaces(context, id, definition);
});

// --