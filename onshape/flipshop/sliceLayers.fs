FeatureScript 2909;
import(path : "onshape/std/geometry.fs", version : "2909.0");

const hugeSizeVal = 1000000;
const tinySizeVal = 0.001;

/**
 * Feature: slices target bodies into parallel layers spaced `layer_depth` apart from a reference plane.
 * Produces up to `split_count + 1` named fragments per body.
 * Fragment names follow the pattern `<original> L<idx> H<dist>` where `idx` is the 0-based layer
 * index (closest-to-plane first) and `dist` is the fragment centroid's signed distance from the
 * reference plane along the cut direction, rounded to 0.1 mm.
 * @param definition {{
 *   @field splitPlane {Query} : Reference plane, face, or mate connector; defines cut origin and normal.
 *   @field layer_depth {ValueWithUnits} : Distance between consecutive cut planes.
 *   @field split_count {number} : Number of cuts; produces at most split_count + 1 fragments per body.
 *   @field targetBodies {Query} : Solid bodies to slice.
 *   @field flip_direction {boolean} : When true, cuts proceed against the plane normal.
 * }}
 */
annotation { "Feature Type Name": "Slice Layers" }
export const sliceLayers = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Split plane", "Filter": QueryFilterCompound.ALLOWS_PLANE, "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  definition.splitPlane is Query;

  annotation { "Name": "Layer depth", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  isLength(definition.layer_depth, {(millimeter) : [tinySizeVal, 10, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name": "Split count", "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  isInteger(definition.split_count, {(unitless) : [1, 5, 100]} as IntegerBoundSpec);

  annotation { "Name": "Target bodies", "Filter": EntityType.BODY, "UIHint": UIHint.REMEMBER_PREVIOUS_VALUE }
  definition.targetBodies is Query;

  annotation { "Name": "Other direction", "UIHint": UIHint.OPPOSITE_DIRECTION }
  definition.flip_direction is boolean;
}
{
  const basePlane = evPlane(context, { "face": definition.splitPlane });
  const cutDir    = definition.flip_direction ? -basePlane.normal : basePlane.normal;

  const targetBodiesArr = evaluateQuery(context, definition.targetBodies);

  for (var bi = 0; bi < size(targetBodiesArr); bi += 1) {
    const bodyQ    = targetBodiesArr[bi];
    const origNameResult = getName(context, bodyQ, "Part " ~ bi);
    const origName = (origNameResult != undefined) ? origNameResult.val : ("Part " ~ bi);

    // Successively split the topmost fragment (in cutDir) with each cut plane,
    // accumulating lower fragments in order from the reference plane outward.
    var allFrags   = [];
    var currentTop = bodyQ;

    for (var ci = 0; ci < definition.split_count; ci += 1) {
      const cutOffset = (ci + 1) * definition.layer_depth;
      const cutOrigin = basePlane.origin + cutOffset * cutDir;
      const cutPlane  = plane(cutOrigin, cutDir);

      // Only proceed if currentTop extends into the positive-cutDir half-space of this cut plane.
      const topBox  = evBox3d(context, { "topology": currentTop, "tight": false });
      const maxProj = dot(topBox.maxCorner - basePlane.origin, cutDir);
      if (maxProj <= cutOffset) {
        break;
      }

      const planeId   = id + ("plane_b" ~ bi ~ "_c" ~ ci);
      const splitId   = id + ("split_b" ~ bi ~ "_c" ~ ci);

      opPlane(context, planeId, {
        "plane":  cutPlane,
        "width":  1000 * millimeter,
        "height": 1000 * millimeter,
      });
      const cutPlaneQ = qCreatedBy(planeId, EntityType.BODY);

      opSplitPart(context, splitId, {
        "targets":   currentTop,
        "tool":      cutPlaneQ,
        "keepTools": false,
      });

      // Determine which resulting piece is lower (accumulate) and which is the new top (continue cutting).
      const newBodyQ  = qCreatedBy(splitId, EntityType.BODY);
      const curBox    = evBox3d(context, { "topology": currentTop, "tight": false });
      const newBox    = evBox3d(context, { "topology": newBodyQ,   "tight": false });
      const curCtr    = (curBox.minCorner + curBox.maxCorner) / 2;
      const newCtr    = (newBox.minCorner + newBox.maxCorner) / 2;
      const curDist   = dot(curCtr - basePlane.origin, cutDir);
      const newDist   = dot(newCtr - basePlane.origin, cutDir);

      if (curDist <= newDist) {
        // currentTop is the lower fragment; newBodyQ becomes the new top.
        allFrags   = append(allFrags, currentTop);
        currentTop = newBodyQ;
      } else {
        // newBodyQ is the lower fragment; currentTop remains the top.
        allFrags = append(allFrags, newBodyQ);
      }
    }

    // Final top fragment (above all cut planes).
    allFrags = append(allFrags, currentTop);

    // Name each fragment: "<original> L<idx> H<dist_mm>"
    for (var fi = 0; fi < size(allFrags); fi += 1) {
      const fragQ     = allFrags[fi];
      const fragBox   = evBox3d(context, { "topology": fragQ, "tight": false });
      const fragCtr   = (fragBox.minCorner + fragBox.maxCorner) / 2;
      const distMm    = dot(fragCtr - basePlane.origin, cutDir) / millimeter;
      const roundedMm = round(distMm * 10) / 10;
      setName(context, fragQ, origName ~ " L" ~ fi ~ " H" ~ roundedMm);
    }
  }
});
