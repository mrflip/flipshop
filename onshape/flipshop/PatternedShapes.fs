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
    "boundingBoxSk":     id + "boundingBoxSk", "shapesSk": id + "shapesSk", "extrudedShapes": id + "extrudedShapes",
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

  debug(context, [
    qCreatedBy(ids.boundingBoxSk, EntityType.BODY), qCreatedBy(ids.patternedShapes, EntityType.BODY)
  ]);


  setProperty(context, {
    "entities":  extrudedBodies,
    "propertyType":  PropertyType.NAME,
    "value":  "Patterned Shapes"
  });
});

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

function boxForCenterSize(ctrH is ValueWithUnits, ctrV is ValueWithUnits,
  sizeH is ValueWithUnits, sizeV is ValueWithUnits,
  minD is ValueWithUnits, maxD is ValueWithUnits) returns map {
    const halfH = sizeH / 2;
    const halfV = sizeV / 2;
    return boxForBounds(ctrH - halfH, ctrH + halfH, ctrV - halfV, ctrV + halfV, minD, maxD
    );
  }

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