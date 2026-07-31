FeatureScript 3029;
import(path : "onshape/std/geometry.fs", version : "3029.0");
import(path : "b496c26424acaeef92a4fd5f", version : "52f21cd35a6702085706f15a");
export import(path : "19a276cbe441b4dcf19aaca1", version : "f65e96bb9819bc0fe6817767");
IconNamespace::import(path : "476f292746334f9ccc9aa08c", version : "ed7e2d8b30026af72161db6f");

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
annotation { "Feature Type Name": "Text Chip", "Icon": IconNamespace::BLOB_DATA, "Feature Type Description": "A thin plate with scaled and aligned text", "Feature Name Template" : "Text Chip #text" }
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
