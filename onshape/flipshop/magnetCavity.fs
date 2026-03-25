FeatureScript 2909;
import(path : "onshape/std/geometry.fs", version : "2909.0");

const hugeSizeVal = 1000000;
const tinySizeVal = 0.001;

// == [Magnet Cavity Part Feature] ==

/**
 * Part feature: rectangular magnet cavity array cut into target bodies.
 * Delegates all geometry to @see `magnetCavity`.
 * @param definition {{
 *   @field basePoint {Query} : Reference face/plane; origin becomes the base point of the cavity grid.
 *   @field targetBodies {Query} : Solid bodies to cut cavities into.
 *   @field length {ValueWithUnits} : Magnet nominal length (H dimension). Default 60 mm.
 *   @field width {ValueWithUnits} : Magnet nominal width (V dimension). Default 10 mm.
 *   @field depth {ValueWithUnits} : Cavity insertion depth. Default 3 mm.
 *   @field insertion_gap {ValueWithUnits} : Per-dimension clearance added to each cavity. Default +0.15 mm.
 *   @field horizontal_reps {number} : Columns of cavities.
 *   @field vertical_reps {number} : Rows of cavities.
 *   @field horizontal_spacing {ValueWithUnits} : Edge-to-edge gap between adjacent columns. Default 1 mm.
 *   @field vertical_spacing {ValueWithUnits} : Edge-to-edge gap between adjacent rows. Default 1 mm.
 *   @field horizontal_shift {ValueWithUnits} : Shifts the base point rightward (+H) from the face origin. Default 0 mm.
 *   @field vertical_shift {ValueWithUnits} : Shifts the base point upward (+V) from the face origin. Default 0 mm.
 * }}
 */
annotation { "Feature Type Name": "Magnet Cavity" }
export const magnetCavityPart = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Base point", "Filter": QueryFilterCompound.ALLOWS_PLANE, "MaxNumberOfPicks": 1 }
  definition.basePoint is Query;

  annotation { "Name": "Target bodies", "Filter": EntityType.BODY, "MaxNumberOfPicks": -1 }
  definition.targetBodies is Query;

  annotation { "Name": "Length" }
  isLength(definition.length, {(millimeter) : [tinySizeVal, 60, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name": "Width" }
  isLength(definition.width, {(millimeter) : [tinySizeVal, 10, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name": "Depth" }
  isLength(definition.depth, {(millimeter) : [tinySizeVal, 3, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name": "Insertion gap" }
  isLength(definition.insertion_gap, {(millimeter) : [-hugeSizeVal, 0.15, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name": "Horizontal reps" }
  isInteger(definition.horizontal_reps, {(unitless) : [1, 1, 100]} as IntegerBoundSpec);

  annotation { "Name": "Vertical reps" }
  isInteger(definition.vertical_reps, {(unitless) : [1, 1, 100]} as IntegerBoundSpec);

  annotation { "Name": "Horizontal spacing" }
  isLength(definition.horizontal_spacing, {(millimeter) : [-hugeSizeVal, 1, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name": "Vertical spacing" }
  isLength(definition.vertical_spacing, {(millimeter) : [-hugeSizeVal, 1, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name": "Horizontal shift" }
  isLength(definition.horizontal_shift, {(millimeter) : [-hugeSizeVal, 0, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name": "Vertical shift" }
  isLength(definition.vertical_shift, {(millimeter) : [-hugeSizeVal, 0, hugeSizeVal]} as LengthBoundSpec);
}
{
  magnetCavity(context, id, definition.basePoint, {
    "target_bodies":      definition.targetBodies,
    "length":             definition.length,
    "width":              definition.width,
    "depth":             definition.depth,
    "insertion_gap":      definition.insertion_gap,
    "horizontal_reps":    definition.horizontal_reps,
    "vertical_reps":      definition.vertical_reps,
    "horizontal_spacing": definition.horizontal_spacing,
    "vertical_spacing":   definition.vertical_spacing,
    "horizontal_shift":   definition.horizontal_shift,
    "vertical_shift":     definition.vertical_shift,
  });
});
// --

// == [Magnet Cavity Geometry] ==

/**
 * Cuts a grid of rectangular magnet cavities into `options.target_bodies`.
 * The base point lies at the center of the left edge of the cavity grid bounding box:
 * H = 0 is the left face of the leftmost column; V = 0 is vertically centered on the grid.
 * Cavities tile rightward; for odd `vertical_reps` the center row aligns with V = 0,
 * for even `vertical_reps` the gap between the two center rows straddles V = 0.
 * @param context {Context} : Model context.
 * @param id {Id} : Feature id prefix.
 * @param basePoint {Query} : Reference face/plane; its origin and axes define the cavity coordinate frame.
 * @param options {map} :
 *   - @field target_bodies {Query} : Bodies to subtract cavities from.
 *   - @field length {ValueWithUnits} : Magnet nominal length (H).
 *   - @field width {ValueWithUnits} : Magnet nominal width (V).
 *   - @field depth {ValueWithUnits} : Cavity insertion depth (into the material).
 *   - @field insertion_gap {ValueWithUnits} : Clearance added once to each cavity dimension.
 *   - @field horizontal_reps {number} : Columns of cavities.
 *   - @field vertical_reps {number} : Rows of cavities.
 *   - @field horizontal_spacing {ValueWithUnits} : Edge-to-edge gap between columns.
 *   - @field vertical_spacing {ValueWithUnits} : Edge-to-edge gap between rows.
 *   - @field horizontal_shift {ValueWithUnits} : Shifts the base point right (+H) from the face origin.
 *   - @field vertical_shift {ValueWithUnits} : Shifts the base point up (+V) from the face origin.
 */
function magnetCavity(context is Context, id is Id, basePoint is Query, options is map) {
  const basePlane    = evPlane(context, { "face": basePoint });
  const vAxis        = cross(basePlane.normal, basePlane.x);
  const workOrigin   = basePlane.origin
                     + options.horizontal_shift * basePlane.x
                     + options.vertical_shift   * vAxis;
  const workPlane    = plane(workOrigin, basePlane.normal, basePlane.x);

  // Cavity dimensions include the per-dimension insertion gap.
  const cavityLength = options.length + options.insertion_gap;
  const cavityWidth  = options.width  + options.insertion_gap;
  const strideH      = cavityLength + options.horizontal_spacing;
  const strideV      = cavityWidth  + options.vertical_spacing;

  // V center of the first (bottom) row so that the grid is vertically centered on V = 0.
  // Derivation: firstCtrV = -((vertical_reps - 1) * strideV) / 2
  // For 1 rep → 0; for 2 reps → ±strideV/2; for 3 reps → -strideV, 0, +strideV.
  const firstCtrV = -((options.vertical_reps - 1) * strideV) / 2;

  // Draw all cavity rectangles in a single sketch.
  const sketchId = id + "cavitySk";
  const sketch   = newSketchOnPlane(context, sketchId, { "sketchPlane": workPlane });
  var cavIdx = 0;
  for (var iy = 0; iy < options.vertical_reps; iy += 1) {
    for (var ix = 0; ix < options.horizontal_reps; ix += 1) {
      const minH = ix * strideH;
      const maxH = minH + cavityLength;
      const ctrV = firstCtrV + iy * strideV;
      const minV = ctrV - cavityWidth / 2;
      const maxV = ctrV + cavityWidth / 2;
      skRectangle(sketch, "cav_" ~ cavIdx, {
        "firstCorner":  vector(minH, minV),
        "secondCorner": vector(maxH, maxV),
      });
      cavIdx += 1;
    }
  }
  skSolve(sketch);

  // Extrude all cavities into the material (opposite the face normal).
  const extrudeId = id + "cavityExtrude";
  opExtrude(context, extrudeId, {
    "entities":  qCreatedBy(sketchId, EntityType.FACE),
    "direction": -workPlane.normal,
    "endBound":  BoundingType.BLIND,
    "endDepth":  options.depth,
  });

  // Subtract all cutter bodies from the target bodies.
  opBoolean(context, id + "boolSubtract", {
    "targets":       options.target_bodies,
    "tools":         qCreatedBy(extrudeId, EntityType.BODY),
    "operationType": BooleanOperationType.SUBTRACTION,
  });
}
// --
