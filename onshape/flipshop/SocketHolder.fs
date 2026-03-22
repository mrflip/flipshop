FeatureScript 2909;
import(path : "onshape/std/geometry.fs", version : "2909.0");
export import(path : "daa2f7d60ba23b30cdfc9d62", version : "146bbe88c26a4a3a41774202");

// SocketWrenches and SocketWrenchesByFamily are defined in SocketWrenches.fs
// (same Feature Studio document — no import needed)

// Sentinel values for LengthBoundSpec min/max when no practical limit applies.
const hugeSizeVal = 1000000;
const tinySizeVal = 0.001;
// Shorthand aliases used throughout for readability.
const mm          = millimeter;
const zero        = 0 * mm;
var   idUniquer   = 0;

// Cuts a cylindrical socket cell into a body on the selected reference plane.
// The cutout is a circle sized to the chosen socket's wrench-end diameter plus
// the insertion gap, extruded two layer-heights deep.
annotation { "Feature Type Name": "Socket Cell Cutter" }
export const socketCellCutter = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name":  "Reference plane", "Filter":  QueryFilterCompound.ALLOWS_PLANE, "MaxNumberOfPicks":  1 }
  definition.referencePlaneQ is Query;

  annotation { "Name" : "Socket Selection", "Lookup Table" : SocketWrenches3 }
  definition.socketPath is LookupTablePath;

  annotation { "Name":  "Layer height" }
  isLength(definition.layerHeight,   {(millimeter) : [tinySizeVal, 4.55, hugeSizeVal]} as LengthBoundSpec);

  annotation { "Name":  "Insertion gap" }
  isLength(definition.insertionGap, {(millimeter) : [0, 0.3, hugeSizeVal]} as LengthBoundSpec);

  // Angular position of the callout label's closest point to the socket center.
  annotation { "Name":  "Callout angle" }
  isAngle(definition.calloutAngle, {(degree) : [0, 0, 360]} as AngleBoundSpec);
}
{
  const socketParams = socketCellParams(context, definition);
  const ids = {
    bboxesSk:         id + "bboxesSk",
    shapesSk:         id + "shapesSk",
    labelsSk:         id + "labelsSk",
    calloutsSk:       id + "calloutsSk",
    extrudedCutout:   id + "extrudedCutout",
  };

  const sketches = {
   bboxes:    newSketchOnPlane(context, ids.bboxesSk,    { "sketchPlane":  socketParams.sketchPlane }),
   shapes:    newSketchOnPlane(context, ids.shapesSk,    { "sketchPlane":  socketParams.sketchPlane }),
   labels:    newSketchOnPlane(context, ids.labelsSk,    { "sketchPlane":  socketParams.sketchPlane }),
   // Callout sketch X axis is the CW tangent at calloutAngle (= radial rotated −90°),
   // so text reads perpendicular to the ray and descends in Y when calloutAngle is 0.
   callouts:  rotatedSketch(context, ids.calloutsSk, socketParams, socketParams.calloutAngle - 90 * degree),
  };

  drawBoundingBoxes(context,   ids.bboxesSk, sketches.bboxes, socketParams);
  drawSocketBaseShape(context, ids.shapesSk, sketches.shapes, socketParams.socket, socketParams);
  skSolve(sketches.shapes);

  radialText(sketches.callouts, socketParams, "Hi, Mom", 4 * mm);
  skSolve(sketches.callouts);

//   drawAndMeasureText(context, ids.labelsSk, sketches.labels, "Hi, Mom", 5*mm, vector(1*mm, 3*mm));

  const cutoutFaces = qCreatedBy(ids.shapesSk, EntityType.FACE);
  opExtrude(context, ids.extrudedCutout, {
    "entities":  cutoutFaces,
    "direction": socketParams.sketchPlane.normal,
    "endBound":  BoundingType.BLIND,
    "endDepth":  socketParams.cutoutDepth,
  });

  setProperty(context, {
    "entities":     qCreatedBy(ids.extrudedCutout, EntityType.BODY),
    "propertyType": PropertyType.NAME,
    "value":        "Socket Cell Cutter",
  });
});

// Draws text on the callout sketch starting at params.cutoutRadius in the +V
// direction (radially outward) and centered on the H axis (tangentially).
// The callout sketch must already be oriented so that +V is the radial direction
// and +H is the CW tangent (i.e., created with rotatedSketch at calloutAngle − 90°).
// textHeight controls font size; character width is estimated at 0.65 × textHeight.
function radialText(sketch is Sketch, params is map, text is string, textHeight is ValueWithUnits) {
  const halfW       = textHeight * 0.65 * length(text);
  const innerRadius = params.cutoutRadius;
  skText(sketch, nextLabelId("callout"), {
    "fontName":     "OpenSans-Regular.ttf",
    "firstCorner":  vector(-halfW,  innerRadius),
    "secondCorner": vector( halfW,  innerRadius + textHeight),
    "text":         text,
  });
}

// Returns the lookup-table entry for a socket family (all sizes) identified by keypath.
// Traverses SocketWrenches2 down to the socket_variant level, stopping before sizing.
function getSocketFamilyRef(context is Context, keypath is LookupTablePath) {
  return SocketWrenches2.entries[keypath.socket_kind].entries[keypath.drive_kind].entries[keypath.unit_system].entries[keypath.sqdrive_size].entries[keypath.reach_kind].entries[keypath.socket_variant];
}

// Returns the lookup-table entry for a specific socket size identified by keypath.
// Traverses SocketWrenches3 all the way to the sizing level, returning the full
// dimension record (wrench_end_diam, wx_overall, etc.).
function getSocketRef(context is Context, keypath is LookupTablePath) {
  return SocketWrenches3.entries[keypath.socket_kind].entries[keypath.drive_kind].entries[keypath.unit_system].entries[keypath.sqdrive_size].entries[keypath.reach_kind].entries[keypath.socket_variant].entries[keypath.sizing];
}

// Returns the 2D coordinate in a rotated sketch that locates the same 3D point
// as coord in the original sketch.  Both sketches share the same origin and normal;
// only the X axis differs by angle.  Expressing the same point in the rotated frame
// is equivalent to rotating the 2D coordinate vector by -angle.
function toRotatedSketchCoord(coord is Vector, angle is ValueWithUnits) returns Vector {
  const h = coord[0];
  const v = coord[1];
  return vector(
     h * cos(angle) + v * sin(angle),
    -h * sin(angle) + v * cos(angle)
  );
}

// Returns a new sketch whose axes share the origin and normal of params.sketchPlane
// but are rotated by angle around that normal.
// Used to angle label text independently of the main geometry sketches.
function rotatedSketch(context is Context, id is Id, params is map, angle is ValueWithUnits) returns Sketch {
//   const basePlane = evPlane(context, { "face": params.referencePlaneQ });
  const basePlane = params.sketchPlane;
  const rotatedX = cos(angle) * basePlane.x + sin(angle) * cross(basePlane.normal, basePlane.x);
  const rotatedPlane = plane(basePlane.origin, basePlane.normal, rotatedX);
  return newSketchOnPlane(context, id, { "sketchPlane": rotatedPlane });
}

// Draws reference and cutout bounding-box rectangles onto sketch, plus midpoint
// dots on each edge.  The nominal box (construction) matches the wrench-end diameter;
// the cutout box (solid) adds the insertion gap on all sides.
function drawBoundingBoxes(context is Context, id is Id, sketch is Sketch, params is map) returns builtin {
  const nb = params.nomBounds;
  const cb = params.cutBounds;

  // Nominal body bounding box (construction, matches wrench-end diameter)
  skRectangle(sketch, "nominalBounds", {
    "firstCorner":  vector(nb.minH, nb.minV),
    "secondCorner": vector(nb.maxH, nb.maxV),
    "construction": true,
  });
  // Cutout bounding box (solid, includes insertion gap)
  skRectangle(sketch, "cutoutBounds", {
    "firstCorner":  vector(cb.minH, cb.minV),
    "secondCorner": vector(cb.maxH, cb.maxV),
    "construction": false,
  });
  // Midpoint dots on nominal bounds
  skPoint(sketch, "nomTopMidDot",   { "position":  vector(nb.ctrH, nb.maxV) });
  skPoint(sketch, "nomRightMidDot", { "position":  vector(nb.maxH, nb.ctrV) });
  // Midpoint dots on cutout bounds
  skPoint(sketch, "cutTopMidDot",   { "position":  vector(cb.ctrH, cb.maxV) });
  skPoint(sketch, "cutRightMidDot", { "position":  vector(cb.maxH, cb.ctrV) });

  skSolve(sketch);
  return sketch;
}

// // Returns a 2-vector from center along rayAngle to the given distance.
// function radialPoint(center is Vector, rayAngle, dist) returns Vector {
//   return center + dist * vector(cos(rayAngle), sin(rayAngle));
// }

// // Draws a text label on sketch whose baseline is perpendicular to rayAngle,
// // centered on the ray at (radius + offset) from center.
// function radialText(context is Context, params, sketch is Sketch, entityId is string,
//   text is string, center is Vector, rayAngle is ValueWithUnits, radius is ValueWithUnits, offset is ValueWithUnits, textHeight is ValueWithUnits) {
//   const pt      = radialPoint(center, rayAngle, radius + offset);
//   const perpDir = vector(-sin(rayAngle), cos(rayAngle));
//   const rayDir  = vector( cos(rayAngle), sin(rayAngle));
//   const halfW   = textHeight * 0.65 * length(text);
//   skText(sketch, nextLabelId(entityId), {
//     "fontName":      "OpenSans-Regular.ttf",
//     "firstCorner":   pt - halfW * perpDir,
//     "secondCorner":  pt + halfW * perpDir + textHeight * rayDir,
//     "text":          text,
//   });
// }

// Draws the two circles that define the socket cell profile on sketch.
// The nominal wrench-end circle is construction-only; the cutout circle is the
// solid region that will be extruded to produce the actual pocket.
function drawSocketBaseShape(context is Context, id is Id, sketch is Sketch, socket is map, params is map) {
  const center = vector(params.nomBounds.ctrH, params.nomBounds.ctrV);
  // Nominal wrench-end circle (construction)
  skCircle(sketch, "wrenchEndCircle", { "center": center, "radius": params.bodyDiam / 2, "construction": true,  });
  // Cutout circle = wrench-end + insertion gap all around (real region)
  skCircle(sketch, "cutoutCircle",    { "center": center, "radius": params.cutoutRadius });
}

// Resolves the socket family entry from the lookup table and builds the base
// parameter map shared by all sizes in that family.  Returns a map containing
// the socket path, family record, gap/height inputs, and evaluated sketch plane.
function socketFamilyCellParams(context, definition is map) returns map {
  // Look up the specific socket by sizing
  const socketPath   = definition.socketPath;
  const socketFamily = getSocketFamilyRef(context, socketPath);
  debug(context, [definition.socketPath]);
  if (socketFamily == undefined) { throw regenError("Unknown socket family '" ~ socketPath); }
  const sketchPlane = evPlane(context, { "face":  definition.referencePlaneQ });

  return mergeMaps(socketPath, {
    "socketPath":      socketPath,
    "socketFamily":    socketFamily,
    "insertionGap":    definition.insertionGap,
    "layerHeight":     definition.layerHeight,
    "calloutAngle":    definition.calloutAngle,
    "sketchPlane":     sketchPlane,
  });
}

// Extends socketFamilyCellParams with the geometry derived from the specific
// socket size: body diameter, cutout radius (diameter/2 + gap), cutout depth
// (2 × layer height), and pre-computed nominal and cutout bounding boxes centered
// at the sketch origin.
function socketCellParams(context, definition is map) returns map {
  const socketParams = socketFamilyCellParams(context, definition);

  const socket       = getSocketRef(context, socketParams.socketPath);
  const bodyDiam     = (socket.wrench_end_diam != undefined) ? socket.wrench_end_diam : socket.wx_overall;
  const cutoutRadius = bodyDiam / 2 + socketParams.insertionGap;
  const cutoutDepth  = 2 * socketParams.layerHeight;

  // Bounding boxes centered at sketch origin
  const nomR      = bodyDiam / 2;
  const nomBounds = {
    "minH":  -nomR,        "ctrH":  zero,  "maxH":  nomR,        "sizeH": bodyDiam,
    "minV":  -nomR,        "ctrV":  zero,  "maxV":  nomR,        "sizeV": bodyDiam,
  };
  const cutBounds = {
    "minH":  -cutoutRadius, "ctrH":  zero,  "maxH":  cutoutRadius, "sizeH": 2 * cutoutRadius,
    "minV":  -cutoutRadius, "ctrV":  zero,  "maxV":  cutoutRadius, "sizeV": 2 * cutoutRadius,
  };

  return mergeMaps(socketParams, {
    "socket":        socket,
    "bodyDiam":      bodyDiam,
    "cutoutRadius":  cutoutRadius,
    "cutoutDepth":   cutoutDepth,
    "nomBounds":     nomBounds,
    "cutBounds":     cutBounds,
  });
}

function nextLabelId(label is string) returns string { return label ~ "_" ~ (idUniquer++); }