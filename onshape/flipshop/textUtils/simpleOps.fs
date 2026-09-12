FeatureScript 3029;
import(path : "onshape/std/common.fs", version : "3029.0");
import(path : "58963520be3fe612d10b6d2e", version : "a3fca0f70c41654575775503");

export function copyBodies(context is Context, id is Id, bodies is Query) returns Query {
    const copyId = id + "bodyCopy";
    opPattern(context, copyId, {
      "entities" : bodies,
      "transforms" : [identityTransform()],
      "instanceNames" : ["copy"]
    });
    return qCreatedBy(copyId, EntityType.BODY);
}

export function simpleExtrude(context is Context, id is Id, direction is Vector, entities is Query, options is map) returns Query {
  const opts = mergeMaps({
    oppositeDirection: false, endBound: BoundingType.BLIND,
  }, options);
  const polarity = opts.oppositeDirection ? -1 : 1;
  opExtrude(context, id, {
    "entities":  entities,
    "direction": polarity * direction,
    "endBound":  opts.endBound,
    "endDepth":  opts.depth,
  });
  return qCreatedBy(id, EntityType.BODY);
}

export function bboxBody(context is Context, id is Id, bodies is Query) returns Query {
  // 1. Bounding box of target, in world coordinates
  const bbox = evBox3d(context, { "topology" : bodies });
  const cuboidID = id + "bboxCuboid";
  fCuboid(context, cuboidID, { "corner1" : bbox.minCorner, "corner2" : bbox.maxCorner });
  return qCreatedBy(cuboidID, EntityType.BODY);
}

export function splitBboxBody(context is Context, id is Id, bodies is Query, splitPlane, keepType is SplitOperationKeepType) returns Query {
  // const opts = mergeMaps({ keepTools: false }, options);
  const bboxQ = bboxBody(context, id, bodies);
  const resultID = id + "splitBboxBody";
  opSplitPart(context, resultID, {
    "targets" :   bboxQ,
    "tool" :      splitPlane,
    "keepTools" : true,
    "keepType":   keepType,
  });
  return qUnion([bboxQ, qCreatedBy(resultID, EntityType.BODY)]);
}
export function splitBboxBody(context is Context, id is Id, bodies is Query, splitPlane is Query) returns Query {
  return splitBboxBody(context, id, bodies, splitPlane, SplitOperationKeepType.KEEP_FRONT);
}

export function isPlaneOrPlaneQ(context is Context, planeQ) returns boolean {
  if (planeQ == undefined) { return false; }
  if (planeQ is Plane) { return true; }
  return (! isQueryEmpty(context, planeQ));
}

export function embedBodies(context is Context, id is Id, tools is Query, targets is Query, opts is map) returns Query {
    var intersectionToolsQ = targets;
    var splitBboxQ = undefined;
    debug(context, tools);
    debug(context, targets);
    debug(context, opts);
    if (isPlaneOrPlaneQ(context, opts.preservingPlaneQ)) {
      const splitDirection = truthy(opts.oppositeDirection) ? SplitOperationKeepType.KEEP_BACK : SplitOperationKeepType.KEEP_FRONT;
      splitBboxQ = splitBboxBody(context, id, tools, opts.preservingPlaneQ, splitDirection);
      intersectionToolsQ = qUnion([splitBboxQ, intersectionToolsQ]);
    }
    opBoolean(context, id + "embedIntersect", {
      "tools" :         intersectionToolsQ,
      "targets":        tools,
      "operationType" : BooleanOperationType.SUBTRACT_COMPLEMENT,
      "keepTools":      true,
    });
    if (isPresent(splitBboxQ)) {
        opDeleteBodies(context, id + "deleteSplitBboxQ", { "entities" : splitBboxQ });
    }
    opBoolean(context, id + "embedSubtract", {
      "tools" :         tools,
      "targets":        targets,
      "operationType" : BooleanOperationType.SUBTRACTION,
      "keepTools":      true,
    });
  return qUnion([tools, targets]);
}
export function embedBodies(context is Context, id is Id, tools is Query, targets is Query) returns Query { return embedBodies(context, id, tools, targets, {}); }


export function skSimpleLine(context is Context, sketch is Sketch, sketchLabel is string, begvec is Vector, endvec is Vector, isConstruction is boolean) returns string {
  skLineSegment(sketch, sketchLabel, { start: begvec, end: endvec, construction: isConstruction });
  return sketchLabel;
}
export function skSimpleLine(context is Context, sketch is Sketch, sketchLabel is string, begvec is Vector, endvec is Vector) returns string {
  return skSimpleLine(context, sketch, sketchLabel, begvec, endvec, false);
}

export function skSimplePoint(context is Context, sketch is Sketch, sketchLabel is string, position is Vector) {
  skPoint(sketch, sketchLabel, { "position": position });
}
export function skSimplePoint(context is Context, sketch is Sketch, sketchLabel is string, xpos is ValueWithUnits, ypos is ValueWithUnits) {
  skSimplePoint(context, sketch, sketchLabel, vector(xpos, ypos));
}

export function skSimpleCircle(context is Context, sketch is Sketch, sketchLabel is string, center is Vector, radius is ValueWithUnits, isConstruction is boolean) returns string {
  skCircle(sketch, sketchLabel, { "center" : center, "radius" : radius, construction: isConstruction });
  return sketchLabel;
}
export function skSimpleCircle(context is Context, sketch is Sketch, sketchLabel is string, center is Vector, radius is ValueWithUnits) returns string {
  return skSimpleCircle(context, sketch, sketchLabel, center, radius, false);
}
export function skSimpleCircle(context is Context, sketch is Sketch, sketchLabel is string, xpos is ValueWithUnits, ypos is ValueWithUnits, radius is ValueWithUnits, isConstruction is boolean) returns string {
  return skSimpleCircle(context, sketch, sketchLabel, vector(xpos, ypos), radius, isConstruction);
}
export function skSimpleCircle(context is Context, sketch is Sketch, sketchLabel is string, xpos is ValueWithUnits, ypos is ValueWithUnits, radius is ValueWithUnits) returns string {
  return skSimpleCircle(context, sketch, sketchLabel, vector(xpos, ypos), radius, false);
}