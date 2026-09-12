FeatureScript 3044;
import(path : "onshape/std/common.fs", version : "3044.0");
// Utils
import(path : "14a20c5c0c7e0354a621f347/2598ef4fc8f1edc5a0e5674f/8c588debec029dab0d734198", version : "5a1a12cc17eebf141db54c9f");
import(path : "14a20c5c0c7e0354a621f347/2598ef4fc8f1edc5a0e5674f/4ebdc64943b566160ea5cc28", version : "d0ff83fd50d99e237a69f68a");
import(path : "14a20c5c0c7e0354a621f347/2598ef4fc8f1edc5a0e5674f/9935c9eba0658e8e5d6b672b", version : "3f735e5f0e03a21a2b73182f");

// Import Hole Tools
BasicHole::import(path : "42452d0d1f5d09a3406f73ac", version : "35bdde628945bd5114060e99");
SelfTapping::import(path : "ca0f63acd016b867f9c2aa8b", version : "b4e38e23c37449b507c62026");
TearHole::import(path : "a76776f84ec6cffe1563a34a", version : "32323d1ce06ad2ed580fbf34");
Wings::import(path : "526e2eb84c79796c0543822d", version : "b33866b6811b0b073b114b58");
IconNamespace::import(path : "bc5e3a00dc2e900fd9de64f9", version : "3aed6b7999f0466af80794c5");


export enum FDMHoleEndStyle {
  annotation { "Name": 'Blind' }
  BLIND,
  annotation { "Name": 'Up to next' }
  UP_TO_NEXT,
  annotation { "Name": 'Up to entity' }
  UP_TO_ENTITY,
  annotation { "Name": 'Through all' }
  THROUGH
}
const FDMHoleEndStyleTitles = {
    FDMHoleEndStyle.BLIND:        'blind',
    FDMHoleEndStyle.UP_TO_NEXT:   '⇒X',
    FDMHoleEndStyle.UP_TO_ENTITY: '⇒Y',
    FDMHoleEndStyle.THROUGH:      'thru',
  };

/**
 * Defines the shape of the cut hole.
 * @value SPLIT : This adds a vertical cut through the hole.
 * @value THREAD : This adds pillars to the hole where the thread of
 * the screw can bind into.
 * @value TEAR : The Hole has a tear shape with an angle on that side.
 */
export enum HoleType {
  annotation { "Name": 'Simple Hole' }
  SIMPLE,
  annotation { "Name": 'Splited Hole' }
  SPLIT,
  annotation { "Name": 'Self Tapping' }
  THREAD,
  annotation { "Name": 'Tear Shaped' }
  TEAR
}
const HoleTypeTitles = {
    HoleType.SIMPLE: 'Hole',
    HoleType.TEAR:   'Tear',
    HoleType.THREAD: 'Thread',
    HoleType.SPLIT:  'Split',
  };

/**
 * Defines thread sizes available for the THREAD HoleType
 */
export enum ThreadType {
  annotation { "Name": 'M2' }
  M2,
  annotation { "Name": 'M2.5' }
  M2_5,
  annotation { "Name": 'M3' }
  M3,
  annotation { "Name": 'M4' }
  M4,
  annotation { "Name": 'M5' }
  M5
}

// Tool parts reject a depth at or below 0.01mm, so a degenerate cut collapses to a hole that is
// visibly nothing rather than to a hard parameter error out of the instantiated part studio
const MIN_CUT_DEPTH = 0.02 * millimeter;
// A raycast hit this close to the ray origin is the face the mate connector sits on
const SAME_FACE_TOL = 0.001 * millimeter;
// Slack for hits against a selected end entity, which may be coincident with the start face
const END_ENTITY_TOL = 0.05 * millimeter;
// How far past the last exit face a THROUGH cut runs
const THROUGH_OVERSHOOT = 1 * millimeter;
// Used when a raycast finds nothing to terminate against
const UP_TO_NEXT_FALLBACK = 5 * millimeter;
const THROUGH_FALLBACK = 50 * millimeter;
// How far along the axis to probe for "the cut starts inside this body"
const AXIS_PROBE_INSET = 0.01 * millimeter;
// Below this, the hole axis is too near parallel with an end plane to intersect it
const PLANE_PARALLEL_TOL = 1e-6;
// A boolean that refuses a tangency or a coincident face will usually accept the same solid
// moved off it by an amount well under print resolution
const PLUG_NUDGE = 0.01 * millimeter;


/**
 * Cuts one hole at one mate connector, into every target body the axis actually runs into.
 *
 * Target bodies are handled one at a time only where an operation would otherwise weld them
 * together. That is just the prefill: a UNION takes every body in its `tools` and returns one
 * body, so unioning the plug against the whole target set merges overlapping targets. The wing
 * and cutter booleans are SUBTRACTIONs, which remove the tool from each target independently
 * and leave the targets separate, so those stay as single calls against the full selection.
 */
function cutHoleAtMate(context is Context, id is Id, definition is map, mateQ is Query) {
  const holeLoc = holeLocFor(context, definition, mateQ);

  if (definition.prefill) {
    prefillDrilledBodies(context, id + "prefill", definition, holeLoc);
  }

  if (definition.hasWings) {
    // The wings sit inside the hole, so their end is pinned to the cut depth
    var wingEndDepth = ifNilOrZero(definition.wingEndDepth, holeLoc.depth - definition.wingFaceInset);
    wingEndDepth = min(wingEndDepth, holeLoc.depth);
    var wingFaceInset = max(definition.wingFaceInset, definition.wingFaceInset * 0.5 + definition.chamferDist * 0.6);
    wingFaceInset = clamp(wingFaceInset, 0 * millimeter, wingEndDepth - 0.01 * millimeter);

    const wingsQ = instancedQFor(context, id + "wings", Wings::build, {
          "diameter":          definition.diameter,
          "wingMidDiamOutset": definition.wingMidDiamOutset,
          "wingGapThk":        definition.wingGapThk,
          "wingEndDepth":      wingEndDepth,
          "wingFaceInset":     wingFaceInset,
          "wingSpread":        definition.wingSpread,
        }, holeLoc.transform);
    highlightQuery(context, wingsQ, DebugColor.ORANGE, definition.debugMe);

    try {
      opBoolean(context, id + "wingsBool", {
            "tools":         wingsQ,
            "targets":       definition.targetBody,
            "operationType": BooleanOperationType.SUBTRACTION,
          });
    } catch (err) {
      debug(context, err);
    }
  }

  const cutter = cutterFor(definition, holeLoc.depth);
  const cutterQ = instancedQFor(context, id + "cutter", cutter.build, cutter.config, holeLoc.transform);
  highlightQuery(context, cutterQ, DebugColor.CYAN, definition.debugMe);

  try {
    opBoolean(context, id + "boolean1", {
          "tools":         cutterQ,
          "targets":       definition.targetBody,
          "operationType": BooleanOperationType.SUBTRACTION,
        });
    if (definition.hasColor) {
      try {
        setColor(context, qCreatedBy(id + "boolean1", EntityType.FACE), definition.holeColor);
      } catch (err) {
        debug(context, [err, 'bad hole', definition.holeColor]);
      }
    }
  } catch (err) {
    debug(context, err);
  }
}

/**
 * Adds the prefill plug to each drilled body separately.
 *
 * One plug per body, because a boolean consumes its tools and because a single union across
 * several targets would return them as one merged body.
 *
 * Three guards against a union that cannot produce a manifold solid. The axis test picks the
 * candidate bodies, which is cheap but only tells you the hole line crosses them. Each candidate
 * is then checked for shared volume, so contact along a face, an edge or a point is skipped
 * rather than handed to a boolean that would have to resolve zero-thickness geometry. What is
 * left is retried once with the plug nudged off whatever coincidence the boolean refused, and
 * anything still unconsumed is deleted instead of being left loose in the part studio.
 */
function prefillDrilledBodies(context is Context, id is Id, definition is map, holeLoc is map) {
  const plugDiam = ifNilOrZero(definition.prefillDiam, definition.diameter + 1 * millimeter);
  const plugDepth = ifNilOrZero(definition.prefillDepth, holeLoc.depth);
  const plugConfig = {
      "diameter":    plugDiam,
      "depth":       plugDepth,
      "split":       false,
      "height":      0 * millimeter,
      // A chamfer that eats the plug leaves a knife edge where the cone meets the body face
      "chamferDist": min(definition.chamferDist, plugDepth * 0.5),
    };

  const bodyQs = drilledBodyQsFor(context, definition.targetBody, holeLoc, plugDepth);
  if (size(bodyQs) == 0) { return; }

  const plugQs = instancedQsFor(context, id + "plugs", BasicHole::build, plugConfig, holeLoc.transform, size(bodyQs));
  var madeQs = plugQs;

  for (var ii = 0; ii < size(bodyQs); ii += 1) {
    highlightQuery(context, plugQs[ii], DebugColor.YELLOW, definition.debugMe);
    if (! bodiesInterfere(context, plugQs[ii], bodyQs[ii])) { continue; }
    if (tryUnion(context, id + "union" + unstableIdComponent(ii), bodyQs[ii], plugQs[ii])) { continue; }

    const nudgedQ = instancedQFor(context, id + "nudged" + unstableIdComponent(ii), BasicHole::build, mergeMaps(plugConfig, {
          "diameter": plugDiam + PLUG_NUDGE,
          "depth":    plugDepth + PLUG_NUDGE,
        }), holeLoc.transform);
    madeQs = append(madeQs, nudgedQ);
    tryUnion(context, id + "retry" + unstableIdComponent(ii), bodyQs[ii], nudgedQ);
  }

  // A consumed plug no longer resolves, so whatever is left is from a skip or a failed union
  const strayQs = qUnion(madeQs);
  if (! isQueryEmpty(context, strayQs)) {
    opDeleteBodies(context, id + "strays", { "entities": strayQs });
  }
}


annotation {
    "Feature Type Name": '3D Printing Hole',
    "Feature Type Description": 'Create Custom Holes designed for 3D Printing, located and oriented by mate connectors',
    "Icon": IconNamespace::BLOB_DATA,
    "Feature Name Template": '#displayTitle',
    "Editing Logic Function": 'fdmHoleFeatureEditLogic'
  }
export const FDMHoleFeature = defineFeature(function(context is Context, id is Id, definition is map)
  precondition
  {
    annotation { "Name": 'Mate connectors', "Filter": BodyType.MATE_CONNECTOR }
    definition.mateConnectors is Query;

    annotation { "Name": 'Opposite direction', "UIHint": [UIHint.OPPOSITE_DIRECTION] }
    definition.flipDir is boolean;

    annotation { "Name": 'Target Bodies', "Filter": EntityType.BODY && BodyType.SOLID }
    definition.targetBody is Query;

    annotation { "Name": 'Hole Method' }
    definition.method is HoleType;

    annotation { "Name": 'Feature Display Title', "UIHint": [UIHint.ALWAYS_HIDDEN] }
    definition.displayTitle is string;

    if (definition.method == HoleType.SIMPLE || definition.method == HoleType.SPLIT || definition.method == HoleType.TEAR) {
      annotation { "Name": 'diameter' }
      isLength(definition.diameter, { (millimeter): [0.001, 5, 1000] } as LengthBoundSpec);

      annotation { "Name": 'Termination', "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE, UIHint.SHOW_LABEL] }
      definition.endStyle is FDMHoleEndStyle;

      if (definition.endStyle == FDMHoleEndStyle.UP_TO_ENTITY || definition.endStyle == FDMHoleEndStyle.UP_TO_NEXT) {
        if (definition.endStyle == FDMHoleEndStyle.UP_TO_ENTITY) {
          annotation { "Name": 'Up to entity or mate connector',
                       "Filter": (EntityType.FACE && SketchObject.NO && AllowMeshGeometry.YES) || QueryFilterCompound.ALLOWS_VERTEX,
                       "MaxNumberOfPicks": 1 }
          definition.endBoundEntity is Query;
        }

        annotation { "Name": 'Offset from tip', "Column Name": 'Has offset', "UIHint": [UIHint.DISPLAY_SHORT, UIHint.FIRST_IN_ROW] }
        definition.offset is boolean;

        if (definition.offset) {
          annotation { "Name": 'Offset from tip', "UIHint": [UIHint.DISPLAY_SHORT] }
          isLength(definition.offsetDistance, ZERO_INCLUSIVE_OFFSET_BOUNDS);

          annotation { "Name": 'Opposite direction', "Column Name": 'Offset opposite direction', "UIHint": [UIHint.OPPOSITE_DIRECTION] }
          definition.oppositeOffsetDirection is boolean;
        }
      }

      if (definition.endStyle == FDMHoleEndStyle.BLIND) {
        annotation { "Name": 'Distance' }
        isLength(definition.depth, ZERO_INCLUSIVE_OFFSET_BOUNDS);
      }

      if (definition.method == HoleType.SPLIT) {
        annotation { "Name": 'Split Height' }
        isLength(definition.split_height, { (millimeter): [0.001, 0.5, 1000] } as LengthBoundSpec);
      } else if (definition.method == HoleType.TEAR) {
        annotation { "Name": 'layerHeight' }
        isLength(definition.layerHeight, { (millimeter): [0.001, 0.2, 1000] } as LengthBoundSpec);

        annotation { "Name": 'Angle' }
        isAngle(definition.angle, { (degree): [60, 120, 179] } as AngleBoundSpec);

        annotation { "Name": 'Bottom Teardrop' }
        definition.bottomTear is boolean;
      }
    } else if (definition.method == HoleType.THREAD) {
      annotation { "Name": 'Thread' }
      definition.thread is ThreadType;

      annotation { "Name": 'Blind' }
      isLength(definition.depthT, { (millimeter): [0.001, 25, 1000] } as LengthBoundSpec);

      annotation { "Name": 'Count' }
      isInteger(definition.thread_count, { (unitless): [2, 3, 6] } as IntegerBoundSpec);
    }

    annotation { "Name": 'Chamfer Distance' }
    isLength(definition.chamferDist, { (millimeter): [0, 0, 1000] } as LengthBoundSpec);

    annotation { "Name": 'Prefill', "Default": false }
    definition.prefill is boolean;

    if (definition.prefill) {
      annotation { "Name": 'Prefill Diameter' }
      isLength(definition.prefillDiam, { (millimeter): [0, 0, 1000] } as LengthBoundSpec);

      annotation { "Name": 'Prefill Depth' }
      isLength(definition.prefillDepth, { (millimeter): [0, 0, 1000] } as LengthBoundSpec);
    }

    annotation { "Name": 'Add Wings', "Default": false }
    definition.hasWings is boolean;

    if (definition.hasWings) {
      annotation { "Name": 'Wing Diameter Outset' }
      isLength(definition.wingMidDiamOutset, { (millimeter): [0.01, 1.6, 1000] } as LengthBoundSpec);

      annotation { "Name": 'Wing Gap Thickness' }
      isLength(definition.wingGapThk, { (millimeter): [0.01, 0.1, 1000] } as LengthBoundSpec);

      annotation { "Name": 'Wings Face Inset' }
      isLength(definition.wingFaceInset, { (millimeter): [0, 0.4, 1000] } as LengthBoundSpec);

      annotation { "Name": 'Wings End Depth' }
      isLength(definition.wingEndDepth, { (millimeter): [0, 0, 1000] } as LengthBoundSpec);

      annotation { "Name": 'Wing Spread Angle' }
      isAngle(definition.wingSpread, { (degree): [2, 120, 179] } as AngleBoundSpec);
    }

    annotation { "Name": 'Color Hole', "Default": false, "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE] }
    definition.hasColor is boolean;

    if (definition.hasColor) {
      annotation { "Name": 'Hole Color Spec', "Default": '#fcd899', "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE] }
      definition.holeColor is string;
    }

    annotation { "Name": 'Highlight While Editing', "Default": false, "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE] }
    definition.debugMe is boolean;
  }
  {
    forEachEntity(context, id + "operation", definition.mateConnectors, function(mateQ is Query, id is Id) {
      cutHoleAtMate(context, id, definition, mateQ);
    });
    holeFeatureName(context, id, definition);
  });

export function fdmHoleFeatureEditLogic(context is Context, id is Id, oldDefinition is map, definition is map, isCreating is boolean, specifiedParameters is map) returns map {
  return holeFeatureName(context, id, definition);
}


/**
 * Everything a cut needs from one mate connector: where it starts, which way it runs, the
 * transform that lands a tool part there, and how deep it reaches.
 */
function holeLocFor(context is Context, definition is map, mateQ is Query) returns map {
  // The mate connector supplies origin, hole axis, and clocking in one shot
  var mateCsys = evMateConnector(context, { "mateConnector": mateQ });
  if (definition.flipDir) {
    // Reverse Z while keeping the user's X direction, so the CSYS stays right handed
    mateCsys = coordSystem(mateCsys.origin, mateCsys.xAxis, -mateCsys.zAxis);
  }
  const holeLoc = {
      "pos": mateCsys.origin,
      // The tool parts are built cutting along their own -Z, so once `toWorld` places them the
      // cut runs against the mate connector's Z. Terminations have to be measured on the axis
      // the cut actually runs along, so the negation lives here rather than at each raycast.
      // The transform is deliberately not negated: that still has to match how the parts build.
      "dir": -mateCsys.zAxis,
      "transform": toWorld(mateCsys),
    };
  return mergeMaps(holeLoc, { "depth": holeDepthFor(context, definition, holeLoc) });
}

/**
 * Cut depth along the hole axis, never degenerate.
 *
 * Measured against the whole target selection on purpose: overlapping bodies share one hole
 * geometry rather than each terminating at its own next face.
 */
function holeDepthFor(context is Context, definition is map, holeLoc is map) returns ValueWithUnits {
  return max(MIN_CUT_DEPTH, uncheckedHoleDepthFor(context, definition, holeLoc));
}

function uncheckedHoleDepthFor(context is Context, definition is map, holeLoc is map) returns ValueWithUnits {
  if (definition.method == HoleType.THREAD) {
    return definition.depthT;
  }

  if (definition.endStyle == FDMHoleEndStyle.BLIND) {
    return definition.depth;
  }

  if (definition.endStyle == FDMHoleEndStyle.UP_TO_NEXT) {
    const hits = evRaycast(context, {
          "ray":      line(holeLoc.pos + holeLoc.dir * SAME_FACE_TOL, holeLoc.dir),
          "entities": definition.targetBody,
        });
    for (var hit in hits) {
      // Ignore the starting face intersection
      if (hit.distance > SAME_FACE_TOL) { return applyTipOffset(definition, hit.distance); }
    }
    println('Can not find next entity!');
    return applyTipOffset(definition, UP_TO_NEXT_FALLBACK);
  }

  if (definition.endStyle == FDMHoleEndStyle.UP_TO_ENTITY) {
    return applyTipOffset(definition, endBoundDepthFor(context, definition, holeLoc, definition.endBoundEntity));
  }

  // THROUGH: scan backwards for the last valid exit boundary hit
  const hits = evRaycast(context, {
        "ray":      line(holeLoc.pos, holeLoc.dir),
        "entities": definition.targetBody,
      });
  for (var ii = size(hits) - 1; ii >= 0; ii -= 1) {
    if (hits[ii].distance > END_ENTITY_TOL) { return hits[ii].distance + THROUGH_OVERSHOOT; }
  }
  return THROUGH_FALLBACK;
}

/**
 * Applies the optional "offset from tip" to a raycast distance.
 */
function applyTipOffset(definition is map, distance is ValueWithUnits) returns ValueWithUnits {
  if (definition.offset != true) {
    return distance;
  }
  if (definition.oppositeOffsetDirection) {
    return distance - definition.offsetDistance;
  }
  return distance + definition.offsetDistance;
}

/**
 * How far the selected end bound sits from the hole start, measured along the hole axis.
 *
 * Axial is the only thing that means anything here, because the number is handed to the tool
 * part as its depth. A raycast hit is already a distance along the ray, so that case is direct.
 * Past that:
 *   - a planar face is intersected with the axis, so a face tilted by theta terminates the cut
 *     at 1/cos(theta), not at cos(theta) as a nearest-point distance would
 *   - a vertex or mate connector is projected onto the axis, which is where Onshape's own
 *     "up to vertex" terminates: the plane through that point, however far off to one side it is
 *   - anything else keeps the nearest-point behaviour, but projected onto the axis and flagged,
 *     since the cut will not actually land on the entity
 * A bound that resolves behind the start now comes back negative and gets clamped to the
 * minimum cut depth, rather than silently cutting forwards by the distance backwards.
 */
function endBoundDepthFor(context is Context, definition is map, holeLoc is map, endQ is Query) returns ValueWithUnits {
  const found = endBoundHitFor(context, holeLoc, endQ);
  if (definition.debugMe) {
    // Draws the measurement axis. If this arrow points opposite to where the hole cuts, every
    // termination in this feature is reading backwards, not just this one.
    debug(context, line(holeLoc.pos, holeLoc.dir));
    println('up to entity: ' ~ found.via ~ ' at ' ~ simpleNumber(found.depth) ~ 'mm along the axis');
  }
  if (found.depth <= 0 * millimeter) {
    println('up to entity: ' ~ found.via ~ ' resolves behind the hole direction, nothing to cut to');
  }
  return found.depth;
}

function endBoundHitFor(context is Context, holeLoc is map, endQ is Query) returns map {
  const hits = evRaycast(context, {
        "ray":      line(holeLoc.pos, holeLoc.dir),
        "entities": endQ,
      });
  for (var hit in hits) {
    if (hit.distance > END_ENTITY_TOL) { return { "depth": hit.distance, "via": 'raycast' }; }
  }

  // Bounded face the ray fell outside of, or an angled end plane: intersect the axis with it
  const planeQ = qGeometry(endQ, GeometryType.PLANE);
  if (! isQueryEmpty(context, planeQ)) {
    const endPlane = evPlane(context, { "face": planeQ });
    const axisAlongNormal = dot(holeLoc.dir, endPlane.normal);
    if (abs(axisAlongNormal) > PLANE_PARALLEL_TOL) {
      return {
          "depth": dot(endPlane.origin - holeLoc.pos, endPlane.normal) / axisAlongNormal,
          "via":   'end plane',
        };
    }
  }

  const endPos = pointEndPosFor(context, endQ);
  if (isPresent(endPos)) {
    return { "depth": dot(endPos - holeLoc.pos, holeLoc.dir), "via": 'point projection' };
  }

  highlightQuery(context, endQ, DebugColor.RED);
  const nearest = evDistance(context, {
        "side0":       holeLoc.pos,
        "side1":       endQ,
        "extendSide1": true,
      });
  return { "depth": dot(nearest.sides[1].point - holeLoc.pos, holeLoc.dir), "via": 'nearest point' };
}

/**
 * Position of a point-like end bound, or undefined if the selection is neither a vertex nor a
 * mate connector.
 */
function pointEndPosFor(context is Context, endQ is Query) {
  const vertexQ = qEntityFilter(endQ, EntityType.VERTEX);
  if (! isQueryEmpty(context, vertexQ)) {
    return evVertexPoint(context, { "vertex": vertexQ });
  }
  const mateQ = qBodyType(endQ, BodyType.MATE_CONNECTOR);
  if (! isQueryEmpty(context, mateQ)) {
    return evMateConnector(context, { "mateConnector": mateQ }).origin;
  }
  return undefined;
}

/**
 * The target bodies this hole actually runs into, in target order.
 *
 * A body counts if the axis starts inside it, or crosses one of its faces within `reach`.
 * Cheap enough to run per mate connector, and it keeps a per-body union from being attempted
 * against a body the tool never touches.
 */
function drilledBodyQsFor(context is Context, targetQ is Query, holeLoc is map, reach is ValueWithUnits) returns array {
  const probePos = holeLoc.pos + holeLoc.dir * AXIS_PROBE_INSET;
  var bodyQs = [];
  for (var bodyQ in evaluateQuery(context, targetQ)) {
    if (! isQueryEmpty(context, qContainsPoint(bodyQ, probePos))) {
      bodyQs = append(bodyQs, bodyQ);
      continue;
    }
    const hits = evRaycast(context, {
          "ray":      line(holeLoc.pos, holeLoc.dir),
          "entities": bodyQ,
        });
    for (var hit in hits) {
      if (hit.distance <= reach) {
        bodyQs = append(bodyQs, bodyQ);
        break;
      }
    }
  }
  return bodyQs;
}

/**
 * Which tool part builds this cut, and how it is configured.
 */
function cutterFor(definition is map, depth is ValueWithUnits) returns map {
  if (definition.method == HoleType.THREAD) {
    return {
        "build": SelfTapping::build,
        "config": {
            "config":      definition.thread,
            "depth":       depth,
            "threads":     definition.thread_count,
            "chamferDist": definition.chamferDist,
          },
      };
  }

  if (definition.method == HoleType.TEAR) {
    return {
        "build": TearHole::build,
        "config": {
            "diameter":    definition.diameter,
            "depth":       depth,
            "angle":       definition.angle,
            "layerHeight": definition.layerHeight,
            "chamferDist": definition.chamferDist,
            "bottomTear":  definition.bottomTear,
          },
      };
  }

  return {
      "build": BasicHole::build,
      "config": {
          "diameter":    definition.diameter,
          "depth":       depth,
          "split":       definition.method == HoleType.SPLIT,
          "height":      ifNil(definition.split_height, 0 * millimeter),
          "chamferDist": definition.chamferDist,
        },
    };
}

/**
 * Whether two solids share volume, as opposed to merely touching or missing each other.
 *
 * Only the count of collision records is read, so this does not depend on their shape. If a
 * pure touch ever shows up here as a collision, the record's clash type is the knob for it.
 * An evaluation that refuses outright falls through to attempting the union, since skipping
 * material is worse than a boolean that might still have worked.
 */
function bodiesInterfere(context is Context, toolQ is Query, targetQ is Query) returns boolean {
  try {
    return size(evCollision(context, { "tools": toolQ, "targets": targetQ })) > 0;
  } catch (err) {
    return true;
  }
}

/**
 * Unions one plug into one body, reporting whether the boolean accepted it rather than throwing,
 * so a refusal can be retried and the leftover swept up.
 */
function tryUnion(context is Context, id is Id, bodyQ is Query, plugQ is Query) returns boolean {
  try {
    opBoolean(context, id, {
          "tools":         qUnion([bodyQ, plugQ]),
          "operationType": BooleanOperationType.UNION,
        });
  } catch (err) {
    debug(context, err);
    return false;
  }
  return true;
}

/**
 * `count` copies of one tool part at one transform, as separate bodies.
 * A boolean consumes its tools, so a tool that has to meet several bodies needs several copies.
 */
function instancedQsFor(context is Context, id is Id, build, config is map, transform is Transform, count is number) returns array {
  const instantiator = newInstantiator(id);
  var thingQs = [];
  for (var ii = 0; ii < count; ii += 1) {
    thingQs = append(thingQs, addInstance(instantiator, build, {
          "configuration": config,
          "transform":     transform,
        }));
  }
  instantiate(context, instantiator);
  return thingQs;
}

function instancedQFor(context is Context, id is Id, build, config is map, transform is Transform) returns Query {
  return instancedQsFor(context, id, build, config, transform, 1)[0];
}

function holeFeatureName(context is Context, id is Id, definition is map) returns map {
  var parts = [HoleTypeTitles[definition.method]];
  const titleDiam = simpleNumber(definition.diameter);
  const titleLen = (definition.endStyle == FDMHoleEndStyle.BLIND) ? ('•' ~ simpleNumber(definition.depth)) : (FDMHoleEndStyleTitles[definition.endStyle]);
  parts = append(parts, titleDiam ~ titleLen);
  if (ifNil(definition.chamferDist, -1) > 0) { parts = append(parts, 'C' ~ simpleNumber(definition.chamferDist)); }
  if (ifNil(definition.hasWings, false))     { parts = append(parts, 'W'); }
  if (ifNil(definition.prefill, false))      { parts = append(parts, 'P' ~ simpleNumber(ifNilOrZero(definition.prefillDiam, definition.diameter))); }
  const displayTitle = join(parts, ' ');
  setFeatureComputedParameter(context, id, { name: 'displayTitle', value: displayTitle });
  definition.displayTitle = displayTitle;
  return definition;
}

/**
 * A missing or zero length parameter means "unset" for the optional sizing fields.
 *
 * Self-contained on purpose: lengths in, lengths out, compared against `0 * millimeter` rather
 * than a bare `0`, so nothing on either path can cross between lengths and plain numbers. A
 * value that somehow arrives unitless is treated as unset rather than fed into length maths.
 */
function ifNilOrZero(val, fallback) {
  if (! isPresent(val)) { return fallback; }
  if (! (val is ValueWithUnits)) { return fallback; }
  return (val > 0 * millimeter) ? val : fallback;
}

export function simpleNumber(num is ValueWithUnits) returns string { return simpleNumber(num / millimeter); }
export function simpleNumber(num is number) returns string {
  if (tolerantEquals(num, floor(num))) { return "" ~ floor(num); }
  return "" ~ roundToPrecision(num, 1);
}