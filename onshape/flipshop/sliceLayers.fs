FeatureScript 2909;
import(path : "onshape/std/geometry.fs", version : "2909.0");
import(path : "e0ff2cae11eb84dfd2b7b6b3", version : "0af8fc719b47a95c5023defd");

/**
 * Feature: slices target bodies into even layers along a chosen plane/face,
 * advancing in the plane's normal direction (or its opposite).
 * @param definition {{
 *      @field splittingEntity {Query} : Planar face or mate connector defining the base cut plane.
 *      @field flipDirection {boolean} : When true, layers advance in the –normal direction.
 *      @field targets {Query} : Bodies to slice.
 *      @field layerDepth {ValueWithUnits} : Thickness of each layer.
 *      @field maxCount {integer} : Maximum number of cuts (upper safety guard).
 * }}
 */
annotation { "Feature Type Name" : "Slice Layers" }
export const sliceLayersFeature = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Splitting plane or face", "Filter": GeometryType.PLANE, "MaxNumberOfPicks": 1 }
  definition.splittingEntity is Query;

  annotation { "Name": "Flip direction" }
  definition.flipDirection is boolean;

  annotation { "Name": "Target bodies", "Filter": EntityType.BODY && BodyType.SOLID, "MaxNumberOfPicks": 100 }
  definition.targets is Query;

  annotation { "Name": "Layer depth" }
  isLength(definition.layerDepth, { (millimeter): [tinySizeVal, 10, hugeSizeVal] } as LengthBoundSpec);

  annotation { "Name": "Max cut count" }
  isInteger(definition.maxCount, { (unitless): [1, 10, 200] } as IntegerBoundSpec);
}
{
  const basePlane = evPlane(context, { "face": definition.splittingEntity });
  sliceLayers(context, id, basePlane, definition.targets, {
    "layerDepth":        definition.layerDepth,
    "maxCount":          definition.maxCount,
    "oppositeDirection": definition.flipDirection,
  });
});


/**
 * Core slicing logic: advances a cut plane in steps of `layerDepth` along
 * (or against) the plane normal, cutting only bodies that still extend
 * beyond the current cut position.
 * @param context {Context} : Model context.
 * @param id {Id} : Feature id; sub-ids are derived from this.
 * @param basePlane {Plane} : Base plane; its normal defines the advance axis.
 * @param bodiesQ {Query} : Initial query for the bodies to slice.
 * @param options {map} : Keyword options.
 *   - @field layerDepth {ValueWithUnits} : Cut spacing.
 *   - @field maxCount {integer} : Loop guard.
 *   - @field oppositeDirection {boolean} : Flip normal.
 */
export function sliceLayers(context is Context, id is Id, basePlane is Plane,
    bodiesQ is Query, options is map)
{
  const layerDepth is ValueWithUnits = options.layerDepth;
  const maxCount   is number         = options.maxCount;

  // Normal that the cut advances along (flip when requested).
  const advanceNormal is Vector = options.oppositeDirection ? -basePlane.normal : basePlane.normal;

  var currentTargetsQ is Query = bodiesQ;

  for (var ii = 0; ii <= maxCount; ii += 1) {
    // The cut plane origin moves by ii * layerDepth along the advance direction.
    const offset   is ValueWithUnits = ii * layerDepth;
    const cutPlane is Plane          = plane(basePlane.origin + advanceNormal * offset, basePlane.normal);

    // Find bodies that still have geometry in front of the cut; break if nothing's left.
    const bodiesAheadQ is Query = qInFrontOfOrIntersectingPlane(currentTargetsQ, cutPlane);
    debug(context, ["sliceLayers", "query", ii, bodiesAheadQ, currentTargetsQ, evaluateQuery(context, currentTargetsQ)], DebugColor.RED);
    if (isQueryEmpty(context, bodiesAheadQ)) { break; }

    // Split bodies that straddle this cut plane.
    try {
      opSplitPart(context, (id + "cut" + toString(ii)), {
        "targets":   currentTargetsQ->qIntersectsPlane(cutPlane),
        "tool":      cutPlane,
        "keepTools": false,
      });
    } catch {} // Tolerate cuts that don't intersect any body (edge case at extremes).

    // Label every surviving piece that already has a name attribute.
    const depthStr is string = toString(round(offset / mm, 0.1));
    // ── Rebuild the "current targets" for the next iteration ───────────.
    // After the split, newly created bodies are tracked automatically via
    // the original query because Onshape's query tracking follows splits.
    // We have to leave the original bodiesQ in here in case they weren't sliced this time.
    for (var body in evaluateQuery(context, currentTargetsQ->qIntersectsPlane(cutPlane))) {
      const nameAttr = getNameOfBody(context, body, '');
      debug(context, ["nameAttr", nameAttr]);
      if ((nameAttr == undefined) || (nameAttr == '')) { continue; }
      setName(context, body, stripLayerLabel(nameAttr) ~ " L:" ~ toString(ii) ~ " H:" ~ depthStr);
    }
  }
}

// --

/**
 * Strips any existing layer label suffix before re-labeling, so repeated
 * previews or re-runs don't accumulate suffixes.
 * @param name {string} : Body name, possibly with a trailing layer label.
 */
function stripLayerLabel(name is string) returns string {
  return replace(name, " L:\\d+ H:.+$", "");
}

/**
 * Bodies from `q` that are either strictly in front of `cutPlane`
 * or straddle it — i.e. the bodies we need to cut into or advance past.
 * @param q {Query} : Bodies to filter.
 * @param cutPlane {Plane} : Dividing plane.
 */
function qInFrontOfOrIntersectingPlane(q is Query, cutPlane is Plane) returns Query {
  return qUnion([
    qInFrontOfPlane(q, cutPlane),
    qIntersectsPlane(q, cutPlane),
  ]);
}
