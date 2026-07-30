FeatureScript 3029;
import(path : "onshape/std/common.fs", version : "3029.0");

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