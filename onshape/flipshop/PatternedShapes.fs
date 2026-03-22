FeatureScript 2909;
// import(path : "onshape/std/common.fs", version : "2909.0");
import(path : "onshape/std/geometry.fs", version : "2909.0");

const hugeSizeVal = 1000000;
const tinySizeVal = 0.001;

export enum ShapeType {
  annotation { "Name": "Circle" }
  CIRCLE,
  annotation { "Name": "Round Rectangle" }
  ROUNDRECT,
  annotation { "Name": "Rectangle" }
  RECTANGLE
}

/**
 * Feature: rectangular grid of extruded shapes on a reference plane, backed by a thin base plate.
 * @param definition {{
 *      @field referencePlane {Query} : Sketch plane.
 *      @field item_x_size {ValueWithUnits} : Item width.
 *      @field item_y_size {ValueWithUnits} : Item height.
 *      @field shape_type {ShapeType} : Shape per grid cell.
 *      @field n_x_items {number} : Column count.
 *      @field n_y_items {number} : Row count.
 *      @field corner_radius {ValueWithUnits} : Fillet radius; applies only when `shape_type` is `ROUNDRECT`.
 *      @field x_gutter {ValueWithUnits} : Horizontal gap between items.
 *      @field y_gutter {ValueWithUnits} : Vertical gap between items.
 *      @field x_margin_left {ValueWithUnits} : Margin on both sides of the grid in X.
 *      @field y_margin_front {ValueWithUnits} : Margin on both sides of the grid in Y.
 *      @field thickness {ValueWithUnits} : Extrusion depth.
 * }}
 */
annotation { "Feature Type Name":  "Patterned Shapes" }
export const patternedShapes = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name":  "Reference plane", "Filter":  QueryFilterCompound.ALLOWS_PLANE, "MaxNumberOfPicks":  1 }
  definition.referencePlane is Query;

  annotation { "Name":  "Item X size" }
  isLength(definition.item_x_size, {(millimeter) : [tinySizeVal, 10, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name":  "Item Y size" }
  isLength(definition.item_y_size, {(millimeter) : [tinySizeVal, 10, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name":  "Shape type" }
  definition.shape_type is ShapeType;

  annotation { "Name":  "Number of items (X)" }
  isInteger(definition.n_x_items, {(unitless) : [1, 3, 100]} as IntegerBoundSpec);

  annotation { "Name":  "Number of items (Y)" }
  isInteger(definition.n_y_items, {(unitless) : [1, 1, 100]} as IntegerBoundSpec);

  annotation { "Name":  "Corner radius" }
  isLength(definition.corner_radius, {(millimeter) : [0, 0, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name":  "X gutter" }
  isLength(definition.x_gutter, {(millimeter) : [-hugeSizeVal, 5, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name":  "Y gutter" }
  isLength(definition.y_gutter, {(millimeter) : [-hugeSizeVal, 5, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name":  "X margin (left)" }
  isLength(definition.x_margin_left, {(millimeter) : [-hugeSizeVal, 5, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name":  "Y margin (front)" }
  isLength(definition.y_margin_front, {(millimeter) : [-hugeSizeVal, 5, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name":  "Thickness" }
  isLength(definition.thickness, {(millimeter) : [tinySizeVal, 5, hugeSizeVal]} as LengthBoundSpec);
}
{
  // Normalize and validate parameters
  const params = patternedShapesParams(definition);
  const ids = {
    "boundingBoxSk":     id + "boundingBoxSk",   "shapesSk": id + "shapesSk", "extrudedShapes": id + "extrudedShapes",
    "patternedShapes":   id + "patternedShapes",
  };

  // Get the sketch plane
  const sketchPlane = evPlane(context, { "face":  definition.referencePlane });

  // Bounding Box Sketch
  const boundsSketch = drawBoundingAndPaddingBoxes(context, ids.boundingBoxSk, sketchPlane, params);
  // Create items sketch
  drawPatternedShapes(context, ids.shapesSk, sketchPlane, params);

  // Extrude items forward
  const shapesQuery = qCreatedBy(ids.shapesSk, EntityType.FACE);
  opExtrude(context, ids.extrudedShapes, {
    "entities":     shapesQuery,
    "direction":    sketchPlane.normal,
    "endBound":     BoundingType.BLIND,
    "endDepth":     params.body_bounds.sizeD
  });

  const plateFace  = qCreatedBy(ids.boundingBoxSk, EntityType.FACE);
  const extrudedBodies = qCreatedBy(ids.extrudedShapes, EntityType.BODY);
  extrude(context, ids.patternedShapes, {
    "entities":          plateFace,
    "direction":         sketchPlane.normal,
    "endBound":          BoundingType.BLIND,
    "depth":             abs(params.body_bounds.maxD) / 100,
    "operationType":     NewBodyOperationType.ADD,
    "bodyType":          ExtendedToolBodyType.SOLID,
    "defaultScope":      false,
    "oppositeDirection": true,
    "booleanScope":      extrudedBodies,
  });

  // debug(context, [qCreatedBy(ids.boundingBoxSk, EntityType.BODY), qCreatedBy(ids.patternedShapes, EntityType.BODY)]);
  setProperty(context, { "entities": extrudedBodies, "propertyType":  PropertyType.NAME, "value": "Patterned Shapes"  });
});

/**
 * Bounding-box map from explicit min/max extents on H, V, and D axes;
 * includes min, ctr, max, and size fields for each.
 * @param minH {ValueWithUnits} : Min H.
 * @param maxH {ValueWithUnits} : Max H.
 * @param minV {ValueWithUnits} : Min V.
 * @param maxV {ValueWithUnits} : Max V.
 * @param minD {ValueWithUnits} : Min D.
 * @param maxD {ValueWithUnits} : Max D.
 */
function boxForBounds(minH is ValueWithUnits, maxH is ValueWithUnits,
  minV is ValueWithUnits, maxV is ValueWithUnits,
  minD is ValueWithUnits, maxD is ValueWithUnits) returns map {
    return {
      "minH":     minH,
      "ctrH":     (minH + maxH) / 2,
      "maxH":     maxH,
      "minV":     minV,
      "ctrV":     (minV + maxV) / 2,
      "maxV":     maxV,
      "sizeH":    maxH - minH,
      "sizeV":    maxV - minV,
      "minD":     minD,
      "ctrD":     (minD + maxD) / 2,
      "maxD":     maxD,
      "sizeD":    maxD - minD,
    };
  }

/**
 * Bounding-box map from center position and total size on H and V.
 * @param ctrH {ValueWithUnits} : Center H.
 * @param ctrV {ValueWithUnits} : Center V.
 * @param sizeH {ValueWithUnits} : Width.
 * @param sizeV {ValueWithUnits} : Height.
 * @param minD {ValueWithUnits} : Min D.
 * @param maxD {ValueWithUnits} : Max D.
 */
function boxForCenterSize(ctrH is ValueWithUnits, ctrV is ValueWithUnits,
  sizeH is ValueWithUnits, sizeV is ValueWithUnits,
  minD is ValueWithUnits, maxD is ValueWithUnits) returns map {
    const halfH = sizeH / 2;
    const halfV = sizeV / 2;
    return boxForBounds(ctrH - halfH, ctrH + halfH, ctrV - halfV, ctrV + halfV, minD, maxD
    );
  }

/**
 * Normalized params map from feature `definition`: strides, item bounding box,
 * items bounds, padded bounds (with margins), and body bounds.
 * @param definition {map} : Raw feature definition.
 */
function patternedShapesParams(definition is map) returns map {
  // Validate gutters
  if (definition.x_gutter <= - definition.item_x_size) { throw regenError("X gutter must be smaller than item X size");  }
  if (definition.y_gutter <= - definition.item_y_size) { throw regenError("Y gutter must be smaller than item Y size"); }

  var cornerRadius = definition.corner_radius;
  // Validate corner radius (only relevant for Roundrect)
  if (definition.shape_type == ShapeType.ROUNDRECT && definition.corner_radius > 0 * millimeter) {
    const maxRadius = min(definition.item_x_size, definition.item_y_size) / 2;
    if (definition.corner_radius > maxRadius) {
      // throw regenError("Corner radius cannot exceed half of the smaller item dimension");
      cornerRadius = maxRadius;
    }
  }
  // Calculate strides
  const x_stride = definition.item_x_size + definition.x_gutter;
  const y_stride = definition.item_y_size + definition.y_gutter;
  // Calculate first item center position
  const item0_ctrH = definition.item_x_size / 2;
  const item0_ctrV = definition.item_y_size / 2;
  // Create item0 box
  const item0 = boxForCenterSize(
    item0_ctrH, item0_ctrV,
    definition.item_x_size, definition.item_y_size,
    0 * meter, definition.thickness
  );
  // Calculate items bounds (no margins)
  const items_width = definition.n_x_items * definition.item_x_size +
  (definition.n_x_items - 1) * definition.x_gutter;
  const items_height = definition.n_y_items * definition.item_y_size +
  (definition.n_y_items - 1) * definition.y_gutter;
  const items_bounds = boxForBounds(
    0 * meter, items_width,
    0 * meter, items_height,
    0 * meter, definition.thickness
  );

  // Create margin map
  const margin = {
    "minH":  definition.x_margin_left,
    "maxH":  definition.x_margin_left,  // Same as left
    "minV":  definition.y_margin_front,
    "maxV":  definition.y_margin_front, // Same as front
  };

  // Calculate padded bounds (with margins)
  const padded_bounds = boxForBounds(
    -margin.minH, items_width + margin.maxH,
    -margin.minV, items_height + margin.maxV,
    0 * meter, definition.thickness
  );

  // Calculate body bounds (includes base plate)
  const base_plate_thickness = 0.05 * millimeter;
  const body_bounds = boxForBounds(
    items_bounds.minH,        items_bounds.maxH,
    items_bounds.minV,        items_bounds.maxV,
    (- base_plate_thickness), definition.thickness
  );

  return {
    "shape_type":  definition.shape_type,
    "n_x_items":  definition.n_x_items,
    "n_y_items":  definition.n_y_items,
    "corner_radius":  cornerRadius,
    "x_stride":  x_stride,
    "y_stride":  y_stride,
    "item0":     item0,
    "items_bounds":   items_bounds,
    "padded_bounds":  padded_bounds,
    "body_bounds":    body_bounds,
    "margin":         margin,
    "polygon_sides":  4,
    "do_fillet_polygons":  definition.shape_type == ShapeType.ROUNDRECT && definition.corner_radius > 0 * meter,
  };
}

/**
 * Sketch with items bounding rectangle (construction) and padded bounds (solid),
 * each with midpoint dots on the top and right edges.
 * @param context {Context} : Model context.
 * @param id {Id} : Sketch feature id.
 * @param sketchPlane {Plane} : Sketch plane.
 * @param params {map} : Params from `patternedShapesParams`.
 */
function drawBoundingAndPaddingBoxes(context is Context, id is Id, sketchPlane is Plane, params is map) returns builtin {
  const sketch = newSketchOnPlane(context, id, { "sketchPlane":  sketchPlane });
  // Draw items bounding box (solid)
  skRectangle(sketch, "itemsBounds", {
    "firstCorner":   vector(params.items_bounds.minH, params.items_bounds.minV),
    "secondCorner":  vector(params.items_bounds.maxH, params.items_bounds.maxV),
    "construction":  true,
  });
  // Draw items bounding box (solid)
  skRectangle(sketch, "paddedBounds", {
    "firstCorner":  vector(params.padded_bounds.minH, params.padded_bounds.minV),
    "secondCorner":  vector(params.padded_bounds.maxH, params.padded_bounds.maxV),
    "construction":  false,
  });
  // Add dots at midpoint of top and right lines
  skPoint(sketch, "itemsBoundsTopMidDot",    { "position":  vector(params.items_bounds.ctrH,  params.items_bounds.maxV)  });
  skPoint(sketch, "itemsBoundsRightMidDot",  { "position":  vector(params.items_bounds.maxH,  params.items_bounds.ctrV)  });
  skPoint(sketch, "paddedBoundsTopMidDot",   { "position":  vector(params.padded_bounds.ctrH, params.padded_bounds.maxV) });
  skPoint(sketch, "paddedBoundsRightMidDot", { "position":  vector(params.padded_bounds.maxH, params.padded_bounds.ctrV) });

  skSolve(sketch);
  return sketch;
}

/**
 * Sketch of the full item grid, with construction decoration for the first item.
 * @param context {Context} : Model context.
 * @param id {Id} : Sketch feature id.
 * @param sketchPlane {Plane} : Sketch plane.
 * @param params {map} : Params from `patternedShapesParams`.
 */
function drawPatternedShapes(context is Context, id is Id, sketchPlane is Plane, params is map) {
  const sketch = newSketchOnPlane(context, id, { "sketchPlane":  sketchPlane });
  var itemIndex = 0;
  for (var iy = 0; iy < params.n_y_items; iy += 1) {
    for (var ix = 0; ix < params.n_x_items; ix += 1) {
      const centerH = params.item0.ctrH + ix * params.x_stride;
      const centerV = params.item0.ctrV + iy * params.y_stride;
      const center = vector(centerH, centerV);
      addSimpleShape(sketch, "item_" ~ itemIndex, center, params, params.item0);
      itemIndex += 1;
    }
  }
  // Add decoration for first item
  decorateItem0(sketch, id, params);
  skSolve(sketch);
}

/**
 * Construction guide lines on `sketch` for the first item: center-to-perimeter
 * in H and V, and perimeter-to-bounding-box extensions.
 * @param sketch {Sketch} : Target sketch.
 * @param sketchId {Id} : Sketch feature id.
 * @param params {map} : Params from `patternedShapesParams`; uses `item0` and `items_bounds`.
 */
function decorateItem0(sketch is Sketch, sketchId is Id, params is map) {
  const center = vector(params.item0.ctrH, params.item0.ctrV);
  // Horizontal line from center to left perimeter
  skLineSegment(sketch, "guideLine_h", {
    "start":  center, "end":  vector(params.item0.minH, params.item0.ctrV), "construction":  true
  });
  // Vertical line from center to bottom perimeter
  skLineSegment(sketch, "guideLine_v", {
    "start":  center, "end":  vector(params.item0.ctrH, params.item0.minV), "construction":  true
  });
  // Horizontal line from left perimeter to bounding box
  skLineSegment(sketch, "toBBox_h", {
    "start":  vector(params.item0.minH, params.item0.ctrV), "end":  vector(params.items_bounds.minH, params.item0.ctrV), "construction":  true
  });
  // Vertical line from bottom perimeter to bounding box
  skLineSegment(sketch, "toBBox_v", {
    "start":  vector(params.item0.ctrH, params.item0.minV), "end":  vector(params.item0.ctrH, params.items_bounds.minV), "construction":  true
  });
}

/**
 * Single item shape on `sketch` at `center`, dispatched on `params.shape_type`.
 * @param sketch {Sketch} : Target sketch.
 * @param idStr {string} : Entity id prefix.
 * @param center {Vector} : 2D center.
 * @param params {map} : Params from `patternedShapesParams`; uses `shape_type` and `do_fillet_polygons`.
 * @param itemBox {map} : Item bounding box; provides `sizeH` and `sizeV`.
 */
function addSimpleShape(sketch is Sketch, idStr is string, center is Vector, params is map, itemBox is map) {
  if (params.shape_type == ShapeType.CIRCLE) {
    skCircle(sketch, idStr, { "center":  center, "radius":  itemBox.sizeH / 2 });
  } else if (params.do_fillet_polygons) {
    addRoundedPolygon(sketch, idStr, center, params, itemBox);
  } else {
    skRectangle(sketch, idStr, {
      "firstCorner":  vector(center[0] - itemBox.sizeH / 2, center[1] - itemBox.sizeV / 2),
      "secondCorner": vector(center[0] + itemBox.sizeH / 2, center[1] + itemBox.sizeV / 2),
    });
  }
}

/**
 * Arc on `sketch` from center, start, and end — adapts to `skArc`'s three-point API.
 * @param sketch {Sketch} : Target sketch.
 * @param id {string} : Entity id.
 * @param definition {{
 *      @field center {Vector} : Arc center.
 *      @field start {Vector} : Start point on the circle.
 *      @field end {Vector} : End point on the circle.
 * }}
 */
function skCenteredArc(sketch is Sketch, id is string, definition is map) {
  // Convert center-based arc to three-point arc
  // Given center, start, and end, calculate the midpoint on the arc
  const center = definition.center;
  const start = definition.start;
  const end = definition.end;
  // Calculate vectors from center to start and end
  const toStart = start - center;
  const toEnd = end - center;
  // Midpoint angle is average of start and end angles
  const midAngle = (toStart + toEnd) / 2;
  const midNormalized = midAngle / norm(midAngle);
  const radius = norm(toStart);
  const mid = center + midNormalized * radius;
  //
  skArc(sketch, id, { "start":  start, "mid":  mid, "end":  end });
}

/**
 * Fillet-ear geometry (arc start, end, and focus) for each vertex of a polygon.
 * @param definition {{
 *      @field vertices {array} : Ordered 2D corner positions (Vector).
 *      @field size {ValueWithUnits} : Setback distance (fillet radius).
 * }}
 */
function cornerEars(definition is map) returns array {
  const vertices = definition.vertices;
  const sz = definition.size;
  const n = size(vertices);
  var ears = [];
  for (var i = 0; i < n; i += 1)  {
    const prev = vertices[(i - 1 + n) % n];
    const apex = vertices[i];
    const next = vertices[(i + 1) % n];
    //
    const startToApex = apex - prev;
    const endToApex = apex - next;
    const startDir = startToApex / norm(startToApex);
    const endDir = endToApex / norm(endToApex);
    const earStart = apex - startDir * sz;
    const earEnd = apex - endDir * sz;
    //
    // The focus is the center of the arc, offset from apex by sz in both directions
    const focus = apex - startDir * sz - endDir * sz;
    //
    ears = append(ears, { "start":  earStart, "end":    earEnd, "apex":   apex, "size":   sz, "focus":  focus });
  }
  return ears;
}

/**
 * Four corners of a rectangle as CW-ordered 2D Vectors: BL, BR, TR, TL.
 * @param center {Vector} : 2D center.
 * @param itemBox {map} : Item box with `sizeH` and `sizeV`.
 */
function verticesFromBox(center is Vector, itemBox is map) returns array {
  const halfW = itemBox.sizeH / 2;
  const halfH = itemBox.sizeV / 2;
  const cx = center[0];
  const cy = center[1];
  return [
    vector(cx - halfW, cy - halfH),  // bottom-left
    vector(cx + halfW, cy - halfH),  // bottom-right
    vector(cx + halfW, cy + halfH),  // top-right
    vector(cx - halfW, cy + halfH),  // top-left
  ];
}

/**
 * Rounded rectangle on `sketch` as alternating fillet arcs and straight edges.
 * Arc ids: `idStr ~ "_arc_" ~ i`; edge ids: `idStr ~ "_edge_" ~ i`.
 * @param sketch {Sketch} : Target sketch.
 * @param idStr {string} : Entity id prefix.
 * @param center {Vector} : 2D center.
 * @param params {map} : Params from `patternedShapesParams`; uses `corner_radius`.
 * @param itemBox {map} : Item bounding box.
 */
function addRoundedPolygon(sketch is Sketch, idStr is string, center is Vector, params is map, itemBox is map) {
  const vertices = verticesFromBox(center, itemBox);
  const ears = cornerEars({
    "vertices":  vertices,
    "size":  params.corner_radius
  });
  const n = size(ears);
  for (var i = 0; i < n; i += 1) {
    const ear = ears[i];
    const nextEar = ears[(i + 1) % n];
    // Draw arc for this corner
    skCenteredArc(sketch, idStr ~ "_arc_"  ~ i, { "start":  ear.start, "center":  ear.focus, "end":  ear.end });
    // Draw line segment to next corner
    skLineSegment(sketch, idStr ~ "_edge_" ~ i, { "start":  ear.end,                         "end":  nextEar.start});
  }
}