FeatureScript 2909;
import(path : "onshape/std/geometry.fs", version : "2909.0");
import(path : "e0ff2cae11eb84dfd2b7b6b3", version : "d88efbe00cb82e247edbd41c");

// == [Magnet Cavity Part Feature] ==

/**
 * Part feature: rectangular magnet cavity array cut into target bodies.
 * Delegates all geometry to @see `magnetCavity`.
 * @param definition {{
 *   @field basePoints {Query} : Reference faces/planes; each becomes a separate cavity grid origin.
 *   @field targetBodies {Query} : Solid bodies to cut cavities into.
 *   @field length {ValueWithUnits} : Magnet nominal length (H dimension). Default 60 mm.
 *   @field width {ValueWithUnits} : Magnet nominal width (V dimension). Default 10 mm.
 *   @field depth {ValueWithUnits} : Cavity insertion depth. Default 3 mm.
 *   @field [extrusion_offset=0] {ValueWithUnits} : Distance along the cut direction before the cavity begins.
 *   @field [insertion_gap=0.15mm] {ValueWithUnits} : Per-dimension clearance added to each cavity.
 *   @field [horizontal_reps=1] {number} : Columns of cavities.
 *   @field [vertical_reps=1] {number} : Rows of cavities.
 *   @field [horizontal_spacing=1mm] {ValueWithUnits} : Edge-to-edge gap between columns.
 *   @field [vertical_spacing=1mm] {ValueWithUnits} : Edge-to-edge gap between rows.
 *   @field [horizontal_shift=0] {ValueWithUnits} : Shifts the base point rightward (+H) from the face origin.
 *   @field [vertical_shift=0] {ValueWithUnits} : Shifts the base point upward (+V) from the face origin.
 *   @field [cleanupSketches=false] {boolean} : When true, deletes sketch bodies after generation.
 * }}
 */
annotation { "Feature Type Name": "Magnet Cavity" }
export const magnetCavityF = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Base points", "Filter": QueryFilterCompound.ALLOWS_PLANE, "MaxNumberOfPicks": 10, "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  definition.basePoints is Query;

  annotation { "Name": "Target bodies", "Filter": EntityType.BODY && BodyType.SOLID, "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  definition.targetBodies is Query;

  annotation { "Name": "Length", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.length, {(millimeter) : [tinySizeVal, 60, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name": "Width", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.width, {(millimeter) : [tinySizeVal, 10, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name": "Depth", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.depth, {(millimeter) : [tinySizeVal, 3, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name": "Extrusion offset", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.extrusion_offset, {(millimeter) : [-hugeSizeVal, 0, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name": "Insertion gap", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.insertion_gap, {(millimeter) : [-hugeSizeVal, 0.15, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name": "Horizontal reps", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  isInteger(definition.horizontal_reps, {(unitless) : [1, 1, 100]} as IntegerBoundSpec);

  annotation { "Name": "Vertical reps", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  isInteger(definition.vertical_reps, {(unitless) : [1, 1, 100]} as IntegerBoundSpec);

  annotation { "Name": "Horizontal spacing", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.horizontal_spacing, {(millimeter) : [-hugeSizeVal, 1, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name": "Vertical spacing", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.vertical_spacing, {(millimeter) : [-hugeSizeVal, 1, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name": "Horizontal shift", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.horizontal_shift, {(millimeter) : [-hugeSizeVal, 0, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name": "Vertical shift", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.vertical_shift, {(millimeter) : [-hugeSizeVal, 0, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name": "Cleanup sketches" }
  definition.cleanupSketches is boolean;
}
{
  magnetCavity(context, id, definition.basePoints, {
    "target_bodies":      definition.targetBodies,
    "length":             definition.length,
    "width":              definition.width,
    "depth":              definition.depth,
    "extrusion_offset":   definition.extrusion_offset,
    "insertion_gap":      definition.insertion_gap,
    "horizontal_reps":    definition.horizontal_reps,
    "vertical_reps":      definition.vertical_reps,
    "horizontal_spacing": definition.horizontal_spacing,
    "vertical_spacing":   definition.vertical_spacing,
    "horizontal_shift":   definition.horizontal_shift,
    "vertical_shift":     definition.vertical_shift,
    "cleanupSketches":    definition.cleanupSketches,
  });
});
// --

// == [Magnet Cavity Geometry] ==

/**
 * Grid of rectangular magnet cavities cut into `options.target_bodies` at each point in `basePoints`.
 * Calls @see `magnetCavityAt` for each evaluated point.
 * @param context {Context} : Model context.
 * @param id {Id} : Feature id prefix.
 * @param basePoints {Query} : Reference faces/planes; each defines a separate cavity coordinate frame.
 * @param options {map} : Keyword options — see @see `magnetCavityAt`.
 */
export function magnetCavity(context is Context, id is Id, basePoints is Query, options is map) {
  var ptIdx = 0;
  for (var bp in evaluateQuery(context, basePoints)) {
    magnetCavityAt(context, id + ("p" ~ toString(ptIdx)), bp, options);
    ptIdx += 1;
  }
}

/**
 * Grid of rectangular magnet cavities at a single base point.
 * The base point lies at the center of the left edge of the cavity grid bounding box:
 * H = 0 is the left face of the leftmost column; V = 0 is vertically centered on the grid.
 * @param context {Context} : Model context.
 * @param id {Id} : Feature id prefix.
 * @param basePoint {Query} : Single reference face/plane; its origin and axes define the cavity frame.
 * @param options {map} : Keyword options.
 *   - @field target_bodies {Query} : Bodies to subtract cavities from.
 *   - @field length {ValueWithUnits} : Magnet nominal length (H).
 *   - @field width {ValueWithUnits} : Magnet nominal width (V).
 *   - @field depth {ValueWithUnits} : Cavity insertion depth.
 *   - @field [extrusion_offset=0] {ValueWithUnits} : Distance along the cut direction before the cavity begins.
 *   - @field insertion_gap {ValueWithUnits} : Clearance added once to each cavity dimension.
 *   - @field horizontal_reps {number} : Columns of cavities.
 *   - @field vertical_reps {number} : Rows of cavities.
 *   - @field horizontal_spacing {ValueWithUnits} : Edge-to-edge gap between columns.
 *   - @field vertical_spacing {ValueWithUnits} : Edge-to-edge gap between rows.
 *   - @field horizontal_shift {ValueWithUnits} : Shifts base point right (+H) from the face origin.
 *   - @field vertical_shift {ValueWithUnits} : Shifts base point up (+V) from the face origin.
 *   - @field [cleanupSketches=false] {boolean} : When true, deletes sketch bodies after generation.
 */
function magnetCavityAt(context is Context, id is Id, basePoint is Query, options is map) {
  const ids = {
    cavitySk:      id + "cavitySk",
    cavityExtrude: id + "cavityExtrude",
    boolSubtract:  id + "boolSubtract",
    cleanup:       id + "cleanup",
  };

  const basePlane  = evPlane(context, { "face": basePoint });
  const vAxis      = cross(basePlane.normal, basePlane.x);
  const workOrigin = basePlane.origin
                   + options.horizontal_shift * basePlane.x
                   + options.vertical_shift   * vAxis;
  const workPlane  = plane(workOrigin, basePlane.normal, basePlane.x);

  // Cavity dimensions include the per-dimension insertion gap.
  const cavityLength = options.length + options.insertion_gap;
  const cavityWidth  = options.width  + options.insertion_gap;
  const strideH      = cavityLength + options.horizontal_spacing;
  const strideV      = cavityWidth  + options.vertical_spacing;

  // V center of the first (bottom) row, so the grid is vertically centered on V = 0.
  // For 1 rep → 0; for 2 → ±strideV/2; for 3 → −strideV, 0, +strideV.
  const firstCtrV = -((options.vertical_reps - 1) * strideV) / 2;

  // The sketch plane is offset along the cut direction by extrusion_offset,
  // so the cavity runs from extrusion_offset to extrusion_offset + depth.
  const sketchPlane = plane(
    workOrigin + options.extrusion_offset * workPlane.normal,
    workPlane.normal,
    workPlane.x
  );

  // Draw all cavity rectangles in a single sketch.
  const sketch = newSketchOnPlane(context, ids.cavitySk, { "sketchPlane": sketchPlane });
  var cavIdx = 0;
  for (var iy = 0; iy < options.vertical_reps; iy += 1) {
    for (var ix = 0; ix < options.horizontal_reps; ix += 1) {
      const minH = ix * strideH;
      const maxH = minH + cavityLength;
      const ctrV = firstCtrV + iy * strideV;
      skRectangle(sketch, "cav_" ~ cavIdx, {
        "firstCorner":  vector(minH, ctrV - cavityWidth / 2),
        "secondCorner": vector(maxH, ctrV + cavityWidth / 2),
      });
      cavIdx += 1;
    }
  }
  skSolve(sketch);
  const cavitySkFacesQ = qCreatedBy(ids.cavitySk, EntityType.FACE);

  // Extrude all cavities into the material (in the face normal direction).
  opExtrude(context, ids.cavityExtrude, {
    "entities":  cavitySkFacesQ,
    "direction": workPlane.normal,
    "endBound":  BoundingType.BLIND,
    "endDepth":  options.depth,
  });
  const cavityExtrudeBodiesQ = qCreatedBy(ids.cavityExtrude, EntityType.BODY);

  // Subtract all cutter bodies from the target bodies.
  opBoolean(context, ids.boolSubtract, {
    "targets":       options.target_bodies,
    "tools":         cavityExtrudeBodiesQ,
    "operationType": BooleanOperationType.SUBTRACTION,
  });

  if (options.cleanupSketches) {
    opDeleteBodies(context, ids.cleanup, {
      "entities": qBodyType(qCreatedBy(id, EntityType.BODY), BodyType.WIRE),
    });
  }
}
// --
