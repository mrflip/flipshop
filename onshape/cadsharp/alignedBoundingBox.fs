FeatureScript 2345;
import(path : "onshape/std/common.fs", version : "2345.0");

export enum InputMethod
{
    annotation { "Name" : "Body entities" }
    ENTIRE_PART,
    annotation { "Name" : "All entities" }
    EVERYTHING
}

export enum BoxType
{
    annotation { "Name" : "Rectangular" }
    RECTANGULAR,
    annotation { "Name" : "Cylindrical" }
    CIRCULAR,
    annotation { "Name" : "Spherical" }
    SPHERE,
    annotation { "Name" : "Spline" }
    SPLINE
}

export enum Rotation
{
    annotation { "Name" : "Rotate all" }
    ROTATE_ALL,
    annotation { "Name" : "None" }
    NONE,
    annotation { "Name" : "Rotate around X" }
    ROTATE_X,
    annotation { "Name" : "Rotate around Y" }
    ROTATE_Y,
    annotation { "Name" : "Rotate around Z" }
    ROTATE_Z,
}

export enum Quality
{
    annotation { "Name" : "Fast" }
    FAST,
    annotation { "Name" : "High quality" }
    HIGH_QUALITY
}

export enum CalculateFrom
{
    annotation { "Name" : "First face" }
    FIRST_FACE,
    annotation { "Name" : "Mate connector" }
    MATE_CONNECTOR,
    annotation { "Name" : "World cSys" }
    WORLD_CSYS
}

export enum AXIS
{
    xAxis,
    yAxis,
    zAxis
}

/**
 * Returns an aligned bounding box or the measurements of that box.
 *
 * @param definition {{
 *      @field xyzOnly {boolean} : If true, returns only the xyz values.
 *      @field inputMethod {InputMethod} : Only a solid body, or any type of entity.
 *      @field entities {Query} : Entities to measure.
 *      @field calculateFrom {CalculateFrom} : The method used to calculate the bounding box.
 *      @field mate {Query} : The mate connector used as the reference coordinate system (if calculateFrom is MATE_CONNECTOR).
 *      @field rotation {Rotation} : The rotation type applied to the bounding box.
 *      @field quality {Quality} : The quality of the rotation (if rotation is ROTATE_ALL).
 *      @field tolerance {Length} : The tolerance for box change (if rotation is ROTATE_ALL).
 *      @field showMyWork {boolean} : Flag to indicate whether to show calculation boxes.
 *      @field offsets {boolean} : Flag to indicate whether offsets are applied.
 *      @field offset {Length} : The offset value (if offsets is true).
 *      @field intersectCurves {boolean} : Flag to indicate whether intersection curves are included.
 *      @field keepBox {boolean} : Flag to indicate whether to keep the box after calculation.
 *      @field showResultBox {boolean} : Shows the resulting bounding box.
 *      @field boxType {BoxType} : Choose if the box type is rectangular or best fit.
 * }}
 *
 * @return {{
 *      @field boxQuery {query} : Bounding box as a solid body.
 *      @field x {length} : x length of the box.
 *      @field y {length} : y length of the box.
 *      @field z {length} : z length of the box.
 *      @field origin : csys placed at the bottom left front corner of the box.
 * }}
 */
export function AlignedBoundingBoxFunction(context is Context, id is Id, definition is map)
{
    var toReturn = {};
    toReturn.boxQuery = qNothing();
    toReturn.x = undefined;
    toReturn.y = undefined;
    toReturn.z = undefined;

    if (definition.boxType == undefined)
        definition.boxType = BoxType.RECTANGULAR;

    var toColor = qNothing();
    var toDelete = qNothing();

    const maxLoopCount = 10;
    var referenceEntities = [];
    var sizeEntities = 0;
    var isVertex = 0;
    var isSketch = 0;
    var referenceFace = qNothing();

    if (definition.calculateFrom == CalculateFrom.FIRST_FACE)
    {
        referenceFace = getFirstReferenceFace(context, definition.entities);
    }

    if (definition.inputMethod != InputMethod.ENTIRE_PART)
    {
        referenceEntities = evaluateQuery(context, definition.entities);
        sizeEntities = size(referenceEntities);
    }

    if (definition.inputMethod != InputMethod.ENTIRE_PART && sizeEntities <= 2)
    {
        isVertex = size(evaluateQuery(context, qEntityFilter(definition.entities, EntityType.VERTEX)));
    }

    if (definition.inputMethod != InputMethod.ENTIRE_PART && definition.calculateFrom == CalculateFrom.FIRST_FACE)
    {
        isSketch = size(evaluateQuery(context, qSketchFilter(definition.entities, SketchObject.YES)));
    }

    if (definition.inputMethod == InputMethod.ENTIRE_PART)
    {
        try silent
        {
            const ownerBodies = qOwnerBody(definition.entities);
            if (!isQueryEmpty(context, ownerBodies))
                definition.entities = ownerBodies;
        }
    }

    // --- Feature-pattern support (mirrors CADSharp Laser Measure) -----------------------
    // Compute the remainder pattern transform from the SAME query evBox3d measures
    // (definition.entities, now resolved to owner bodies above) plus the reference frame
    // (the mate connector, when used).  This MUST be computed here -- in the same scope and
    // from the same query as the measurement, with nothing rewriting the query in between.
    // That consistency is what was missing before: when the input is patterned, the query
    // resolves to this instance's copy and evBox3d measures it, so the remainder is identity;
    // when only this feature is patterned, the remainder is the full instance transform.
    // Either way transformResultIfNecessary at the end puts the box on the right instance.
    var patternReferences = definition.entities;
    if (definition.calculateFrom == CalculateFrom.MATE_CONNECTOR && definition.mate is Query)
        patternReferences = qUnion([definition.entities, definition.mate]);
    const remainingTransform = getRemainderPatternTransform(context, { "references" : patternReferences });

    if (definition.showMyWork)
    {
        try silent
        {
            if (!isQueryEmpty(context, definition.entities))
            {
                addDebugEntities(context, definition.entities, DebugColor.BLACK);
            }
        }
    }

    var boxMap = {};

    if (!(sizeEntities == 1 && isVertex == 1) && !(isVertex == 2 && sizeEntities == 2))
    {
        boxMap.csys = WORLD_COORD_SYSTEM;

        if (definition.calculateFrom == CalculateFrom.FIRST_FACE)
        {
            try silent
            {
                const facePlane = evFaceTangentPlane(context, {
                            "face" : referenceFace,
                            "parameter" : vector(0.5, 0.5)
                        });

                boxMap.csys = coordSystem(facePlane);
            }

            if (definition.inputMethod != InputMethod.ENTIRE_PART && sizeEntities == isSketch)
            {
                definition.rotation = Rotation.ROTATE_Z;
            }
        }
        else if (definition.calculateFrom == CalculateFrom.MATE_CONNECTOR)
        {
            boxMap.csys = evMateConnector(context, { "mateConnector" : definition.mate });
        }

        // Apply rotation optimizations based on user settings
        if (definition.rotation == Rotation.ROTATE_ALL)
        {
            var p = 1;
            boxMap = smallestBox(context, id + p, definition, boxMap.csys, AXIS.zAxis, maxLoopCount);
            boxMap = smallestBox(context, id + p, definition, boxMap.csys, AXIS.xAxis, maxLoopCount);
            boxMap = smallestBox(context, id + p, definition, boxMap.csys, AXIS.yAxis, maxLoopCount);

            if (definition.quality == Quality.HIGH_QUALITY)
            {
                // Multi-pass rotation for precision
                boxMap = smallestBox(context, id + p, definition, boxMap.csys, AXIS.xAxis, maxLoopCount);
                boxMap = smallestBox(context, id + p, definition, boxMap.csys, AXIS.yAxis, maxLoopCount);
                boxMap = smallestBox(context, id + p, definition, boxMap.csys, AXIS.zAxis, maxLoopCount);
            }
        }
        else if (definition.rotation == Rotation.ROTATE_X)
        {
            boxMap = smallestBox(context, id, definition, boxMap.csys, AXIS.xAxis, maxLoopCount);
        }
        else if (definition.rotation == Rotation.ROTATE_Y)
        {
            boxMap = smallestBox(context, id, definition, boxMap.csys, AXIS.yAxis, maxLoopCount);
        }
        else if (definition.rotation == Rotation.ROTATE_Z)
        {
            boxMap = smallestBox(context, id, definition, boxMap.csys, AXIS.zAxis, maxLoopCount);
        }
        else if (definition.rotation == Rotation.NONE)
        {
            boxMap = bboxRotation(context, id, definition, boxMap.csys, AXIS.zAxis, 0 * degree);
        }

        const bboxSize = boxMap.bbox.maxCorner - boxMap.bbox.minCorner;

        toReturn.x = bboxSize[0];
        toReturn.y = bboxSize[1];
        toReturn.z = bboxSize[2];

        // Show result box debug if requested (works even with xyzOnly)
        if (definition.showResultBox)
        {
            try silent
            {
                debug(context, boxMap.bbox, boxMap.csys, DebugColor.CYAN);
            }
        }

        // Create and keep the bounding box if requested (works even with xyzOnly)
        if (definition.keepBox || !definition.xyzOnly)
        {
            var surfaceCheck = 0;
            if (bboxSize[0] < 0.001 * inch)
                surfaceCheck += 1;
            if (bboxSize[1] < 0.001 * inch)
                surfaceCheck += 1;
            if (bboxSize[2] < 0.001 * inch)
                surfaceCheck += 1;

            // Non-rectangular best-fit shapes (cylinder / sphere / smooth spline) are
            // built for genuinely 3D or planar inputs.  Highly degenerate inputs
            // (line- or point-like, surfaceCheck >= 2) always fall back to the box.
            const useFitShape = definition.boxType != BoxType.RECTANGULAR && surfaceCheck < 2;

            if (!useFitShape)
            {
                const z = 0.01 * inch;
                if (surfaceCheck == 1)
                    boxMap.bbox = extendBox3d(boxMap.bbox, z, 0);
                if (definition.offsets)
                    boxMap.bbox = extendBox3d(boxMap.bbox, definition.offset, 0);

                // Create the cuboid
                fCuboid(context, id + "cuboid1", {
                            "corner1" : boxMap.bbox.minCorner,
                            "corner2" : boxMap.bbox.maxCorner
                        });

                const cube = qCreatedBy(id + "cuboid1", EntityType.BODY);
                toColor = cube;
                toReturn.boxQuery = cube;

                // Orient cuboid to the calculated coordinate system
                opTransform(context, id + "transform1", {
                            "bodies" : cube,
                            "transform" : transform(XY_PLANE, plane(boxMap.csys))
                        });

                // Delete/Offset/Surface logic
                if (surfaceCheck == 1)
                {
                    const cubeFaces = qCreatedBy(id + "cuboid1", EntityType.FACE);
                    var faceToMove = evaluateQuery(context, qLargest(cubeFaces))[0];
                    faceToMove = makeRobustQuery(context, faceToMove);
                    opExtractSurface(context, id + "extractFace", { "faces" : faceToMove });
                    toColor = qCreatedBy(id + "extractFace", EntityType.BODY);
                    toDelete = cube;
                }
                else if (definition.offsets)
                {
                    opOffsetFace(context, id + "offsetFace1", {
                                "moveFaces" : qCreatedBy(id + "cuboid1", EntityType.FACE),
                                "offsetDistance" : definition.offset
                            });
                }

                if (!definition.keepBox)
                    toDelete = qUnion([toDelete, toColor]);

                if (surfaceCheck < 2)
                {
                    // Only set properties if we're not in xyzOnly mode
                    if (!definition.xyzOnly)
                    {
                        setProperty(context, { "entities" : toColor, "propertyType" : PropertyType.APPEARANCE, "value" : color(.5, .8, 1, .3) });
                        setProperty(context, { "entities" : toColor, "propertyType" : PropertyType.NAME, "value" : "Bounding box" });
                    }
                }
            }
            else
            {
                // --- BEST-FIT CYLINDER / SPHERE / SMOOTH-SPLINE SHAPES ---
                var fitBody = qNothing();
                if (definition.boxType == BoxType.CIRCULAR)
                    fitBody = buildBoundingCylinder(context, id, definition, boxMap.csys, bboxSize);
                else if (definition.boxType == BoxType.SPHERE)
                    fitBody = buildBoundingSphere(context, id, definition, boxMap, bboxSize);
                else // SPLINE
                    fitBody = buildBoundingSplinePrism(context, id, definition, boxMap.csys, bboxSize);

                toColor = fitBody;
                toReturn.boxQuery = fitBody;

                // Grow the fitted shape outward by the requested offset.
                if (definition.offsets)
                {
                    opOffsetFace(context, id + "offsetFitShape", {
                                "moveFaces" : qOwnedByBody(fitBody, EntityType.FACE),
                                "offsetDistance" : definition.offset
                            });
                }

                if (!definition.keepBox)
                    toDelete = qUnion([toDelete, toColor]);

                if (!definition.xyzOnly)
                {
                    setProperty(context, { "entities" : toColor, "propertyType" : PropertyType.APPEARANCE, "value" : color(.5, .8, 1, .3) });
                    setProperty(context, { "entities" : toColor, "propertyType" : PropertyType.NAME, "value" : boundingShapeName(definition.boxType) });
                }
            }

            // --- ORIGIN CALCULATION: BOTTOM LEFT (MIN CORNER) ---
            const bottomLeftWorld = toWorld(boxMap.csys, boxMap.bbox.minCorner);
            toReturn.origin = coordSystem(bottomLeftWorld, boxMap.csys.xAxis, boxMap.csys.zAxis);
        }

        if (definition.xyzOnly)
            return toReturn;

        // Variable creation logic (skipped when xyzOnly)
        var vCount = 0;
        var loop = true;
        while (loop)
        {
            vCount += 1;
            try silent
            {
                getVariable(context, "box" ~ vCount ~ "_X");
            }
            catch
            {
                loop = false;
            }
            setVariable(context, "box" ~ vCount ~ "_X", bboxSize[0]);
            setVariable(context, "box" ~ vCount ~ "_Y", bboxSize[1]);
            setVariable(context, "box" ~ vCount ~ "_Z", bboxSize[2]);
        }
    }
    else // Two vertices logic
    {
        const point1 = evVertexPoint(context, { "vertex" : referenceEntities[0] });
        const point2 = evVertexPoint(context, { "vertex" : referenceEntities[1] });
        opFitSpline(context, id + "fitSpline1", { "points" : [point1, point2] });
    }

    // Place the created bounding box / fit shape onto this pattern instance.  Outside a
    // feature pattern (and for all the non-patterned internal callers) the remainder is
    // the identity transform, so this is a no-op.
    transformResultIfNecessary(context, id, remainingTransform);

    return toReturn;
}

function getFirstReferenceFace(context is Context, entities is Query) returns Query
{
    var face = qNthElement(qEntityFilter(entities, EntityType.FACE), 0);
    if (!isQueryEmpty(context, face))
        return face;

    try silent
    {
        const ownerBodies = qOwnerBody(entities);
        face = qNthElement(qOwnedByBody(ownerBodies, EntityType.FACE), 0);
        if (!isQueryEmpty(context, face))
            return face;
    }

    return qNthElement(qOwnedByBody(entities, EntityType.FACE), 0);
}

/**
 * Smart hybrid optimization strategy:
 * Phase 1: Very coarse sampling (4 points) with evBox3d to find minimum region
 * Phase 2: Two-stage refinement with just 2-3 more evBox3d calls
 * Total: ~7 evBox3d calls per axis instead of ~15
 */
export function smallestBox(context, id, definition, csys, axis is AXIS, maxLoopCount is number)
{
    var toReturn = {};
    toReturn.csys = csys;
    toReturn.bbox;
    toReturn.allBoxes = [];

    const tolInch = definition.tolerance / inch;

    // PHASE 1: Very coarse 3-point sampling (0, 60, 120 degrees)
    // This gives us a good starting point with only 3 evBox3d calls
    // Covers 0-180° in 60° segments which is sufficient for finding minimum region
    var smallestVolume = 1e15;
    var smallestCsys = csys;
    var smallestBbox;
    var bestAngle = 0 * degree;

    const coarseAngles = [0 * degree, 60 * degree, 120 * degree];
    var bestIndex = 0;

    // Cache for coarse volumes to avoid recomputation
    var coarseVolumes = [];

    for (var i = 0; i < size(coarseAngles); i += 1)
    {
        const thisAngle = coarseAngles[i];
        const thisBox = bboxRotation(context, id, definition, csys, axis, thisAngle);
        const thisVol = getBoxVolume(thisBox.bbox);

        coarseVolumes = append(coarseVolumes, thisVol);

        if (thisVol < smallestVolume)
        {
            smallestVolume = thisVol;
            smallestCsys = thisBox.csys;
            smallestBbox = thisBox.bbox;
            bestAngle = thisAngle;
            bestIndex = i;
        }

        if (definition.showMyWork)
        {
            try silent
            {
                debug(context, thisBox.bbox, thisBox.csys, DebugColor.RED);
            }
        }
        if (definition.showMyWork)
            toReturn.allBoxes = append(toReturn.allBoxes, thisBox.bbox);
    }

    // Define search range based on best coarse point
    // Narrow to 60° region: 0° best → 0-60°, 60° best → 0-120°, 120° best → 60-180°
    var searchStart;
    var searchEnd;
    var startIndex;
    var endIndex;

    if (bestIndex == 0)
    {
        searchStart = 0 * degree;
        searchEnd = 60 * degree;
        startIndex = 0;
        endIndex = 1;
    }
    else if (bestIndex == 2)
    {
        searchStart = 60 * degree;
        searchEnd = 180 * degree;
        startIndex = 1;
        endIndex = 2;
    }
    else
    {
        // 60° is best, use full 0-120° range
        searchStart = 0 * degree;
        searchEnd = 120 * degree;
        startIndex = 0;
        endIndex = 2;
    }

    // PHASE 2: Single refinement point at midpoint
    // Just 1 evBox3d call instead of 2
    const midPoint = (searchStart + searchEnd) / 2;
    const midBox = bboxRotation(context, id, definition, csys, axis, midPoint);
    const midVol = getBoxVolume(midBox.bbox);

    if (definition.showMyWork)
    {
        try silent
        {
            debug(context, midBox.bbox, midBox.csys, DebugColor.RED);
        }
    }
    if (definition.showMyWork)
        toReturn.allBoxes = append(toReturn.allBoxes, midBox.bbox);

    // Update best if midpoint is better
    if (midVol < smallestVolume)
    {
        smallestVolume = midVol;
        smallestCsys = midBox.csys;
        smallestBbox = midBox.bbox;
        bestAngle = midPoint;
    }

    // PHASE 3: Parabolic interpolation using cached values
    // Use 3 points: best from coarse, midpoint, and neighbor
    // Zero additional evBox3d calls - all data is cached
    var refinedAngle = bestAngle;

    // Set up points for interpolation
    var a1;
    var a2;
    var a3;
    var v1;
    var v2;
    var v3;

    if (bestAngle == midPoint)
    {
        // Midpoint is best, use it as center with boundary neighbors
        a2 = midPoint;
        v2 = midVol;

        // Use the coarse boundary points
        a1 = searchStart;
        v1 = coarseVolumes[startIndex];
        a3 = searchEnd;
        v3 = coarseVolumes[endIndex];
    }
    else
    {
        // One of coarse points is best
        a2 = bestAngle;
        v2 = smallestVolume;

        // Use midpoint as one neighbor
        a1 = midPoint;
        v1 = midVol;

        // Use the other coarse boundary as second neighbor
        if (bestIndex == 0 || (bestIndex == 1 && midPoint > bestAngle))
        {
            a3 = searchEnd;
            v3 = coarseVolumes[endIndex];
        }
        else
        {
            a3 = searchStart;
            v3 = coarseVolumes[startIndex];
        }
    }

    // Parabolic interpolation formula
    const h1 = (a2 - a1) / degree;
    const h2 = (a3 - a2) / degree;
    const vDiff1 = v2 - v1;
    const vDiff2 = v3 - v2;

    const denom = h1 * vDiff2 + h2 * vDiff1;

    if (abs(denom) > 0.0001)
    {
        const offset = 0.5 * (h1 * h1 * vDiff2 - h2 * h2 * vDiff1) / denom;
        const interpolatedAngle = a2 - offset * degree;

        // Clamp to valid range
        if (interpolatedAngle > searchStart && interpolatedAngle < searchEnd)
        {
            refinedAngle = interpolatedAngle;
        }
    }

    // PHASE 4: Final precise evaluation
    // 1 evBox3d call at interpolated angle
    const finalBox = bboxRotation(context, id, definition, csys, axis, refinedAngle);
    const finalVol = getBoxVolume(finalBox.bbox);

    if (finalVol < smallestVolume)
    {
        smallestCsys = finalBox.csys;
        smallestBbox = finalBox.bbox;
    }

    if (definition.showMyWork)
    {
        try silent
        {
            debug(context, finalBox.bbox, finalBox.csys, DebugColor.GREEN);
        }
    }

    toReturn.csys = smallestCsys;
    toReturn.bbox = smallestBbox;

    return toReturn;
}

/**
 * Compares two evBox by volume (more accurate than sum of dimensions).
 *
 * @param boxA {box} : A box created from evBox;
 * @param boxB {box} : A box created from evBox;
 *
 * @return {{
 *          @field differenceSum {ValueWithUnits} : Volume difference (positive if boxB is larger).
 *          @field largerBox {Box} : The larger of the two boxes.
 *          @field smallerBox {Box} : The smaller of the two boxes.
 *
 * }}
 **/
export function compareBox(boxA, boxB)
{
    const boxSizeA = boxA.maxCorner - boxA.minCorner;
    const boxSizeB = boxB.maxCorner - boxB.minCorner;

    // Use volume for comparison (more accurate for 3D boxes)
    const minDim = 0.001 * inch;
    const ax = max(boxSizeA[0], minDim);
    const ay = max(boxSizeA[1], minDim);
    const az = max(boxSizeA[2], minDim);
    const bx = max(boxSizeB[0], minDim);
    const by = max(boxSizeB[1], minDim);
    const bz = max(boxSizeB[2], minDim);

    const volumeA = ax * ay * az;
    const volumeB = bx * by * bz;

    return {
        "differenceSum": volumeB - volumeA,
        "smallerBox": (volumeB < volumeA) ? boxB : boxA,
        "largerBox": (volumeB < volumeA) ? boxA : boxB
    };
}

/**
 * Calculate box volume for comparison.
 */
function getBoxVolume(bbox)
{
    const boxSize = bbox.maxCorner - bbox.minCorner;
    const minDim = 0.001 * inch;
    const x = max(boxSize[0], minDim);
    const y = max(boxSize[1], minDim);
    const z = max(boxSize[2], minDim);
    return x * y * z / (inch^3);
}

export function bboxRotation(context, id, definition, csys, axis is AXIS, degrees)
{
    var rotationAxis;
    if (axis == AXIS.yAxis)
    {
        // Y axis is not directly available on csys, compute from cross product of Z and X
        rotationAxis = cross(csys.zAxis, csys.xAxis);
    }
    else if (axis == AXIS.xAxis)
    {
        rotationAxis = csys.xAxis;
    }
    else
    {
        rotationAxis = csys.zAxis;
    }

    const localCsys = rotationAround(line(csys.origin, rotationAxis), degrees) * csys;

    const bbox = evBox3d(context, {
                "topology" : definition.entities,
                "cSys" : localCsys,
                "tight" : true
            });

    return {
        "size": bbox.maxCorner - bbox.minCorner,
        "csys": localCsys,
        "bbox": bbox
    };
}

// =====================================================================================
//  BEST-FIT SHAPE BUILDERS (cylinder / sphere / smooth spline)
//
//  These all reuse the rotation-optimised coordinate system computed above.  For the
//  cylinder and spline shapes the part's silhouette is recovered from its support
//  function (sampled with evBox3d in rotated frames), so no tessellation is needed and
//  the result is valid for solids, surfaces and curves.  The reconstructed silhouette
//  polygon circumscribes the true silhouette: the cylinder (minimum enclosing circle)
//  and sphere (half the box diagonal) strictly enclose the part, while the smooth
//  spline wraps the silhouette with a small outward margin so it stays clear of flat
//  faces (an interpolating spline otherwise bows inward and would clip them).
// =====================================================================================

/**
 * Builds a best-fit bounding cylinder around `definition.entities`.
 *
 * The axis is chosen from the three axes of `baseCsys` to minimise the cylinder
 * volume; the radius is the minimum enclosing circle of the part's silhouette in the
 * plane perpendicular to that axis.
 *
 * @return {Query} : the created cylinder body, already oriented in world space.
 */
export function buildBoundingCylinder(context is Context, id is Id, definition is map, baseCsys is CoordSystem, bboxSize is Vector) returns Query
{
    const dirCalls = (definition.quality == Quality.HIGH_QUALITY) ? 24 : 12;

    var best = undefined;
    for (var axisIndex = 0; axisIndex < 3; axisIndex += 1)
    {
        const section = sampleSilhouette(context, definition, baseCsys, axisIndex, dirCalls);
        const circle = minimumEnclosingCircle(section.polygon);
        const radius = max(circle.radius, 0.005 * inch);
        const height = max(section.zMax - section.zMin, 0.01 * inch);
        const volume = PI * radius * radius * height;
        if (best == undefined || volume < best.volume)
            best = { "volume" : volume, "section" : section, "circle" : circle, "radius" : radius };
    }

    const center = best.circle.center;
    var zLo = best.section.zMin;
    var zHi = best.section.zMax;
    if (zHi - zLo < 0.01 * inch)
    {
        const mid = (zHi + zLo) / 2;
        zLo = mid - 0.005 * inch;
        zHi = mid + 0.005 * inch;
    }

    // Build in the section's local frame, then orient to world.
    fCylinder(context, id + "fitCylinder", {
                "bottomCenter" : vector(center[0], center[1], zLo),
                "topCenter" : vector(center[0], center[1], zHi),
                "radius" : best.radius
            });

    const body = qCreatedBy(id + "fitCylinder", EntityType.BODY);
    opTransform(context, id + "fitCylinderXform", {
                "bodies" : body,
                "transform" : transform(XY_PLANE, plane(best.section.buildCsys))
            });

    return body;
}

/**
 * Builds a best-fit bounding sphere, centred at the centre of the rotation-optimised
 * bounding box with a radius of half the box's space diagonal.  This is guaranteed to
 * enclose the part for any input.
 *
 * @return {Query} : the created sphere body.
 */
export function buildBoundingSphere(context is Context, id is Id, definition is map, boxMap is map, bboxSize is Vector) returns Query
{
    const centerLocal = (boxMap.bbox.minCorner + boxMap.bbox.maxCorner) / 2;
    const centerWorld = toWorld(boxMap.csys, centerLocal);
    const radius = max(0.5 * norm(bboxSize), 0.005 * inch);

    opSphere(context, id + "fitSphere", {
                "center" : centerWorld,
                "radius" : radius
            });

    return qCreatedBy(id + "fitSphere", EntityType.BODY);
}

/**
 * Builds a smooth best-fit prism.  The part's silhouette in the plane perpendicular to
 * its shortest box axis is recovered from the support function, nudged outward by the
 * local chord sagitta (so the smooth curve stays outside flat faces), wrapped with a
 * closed periodic fit spline, and extruded the length of that axis.
 *
 * @return {Query} : the created prism body, already oriented in world space.
 */
export function buildBoundingSplinePrism(context is Context, id is Id, definition is map, baseCsys is CoordSystem, bboxSize is Vector) returns Query
{
    const dirCalls = (definition.quality == Quality.HIGH_QUALITY) ? 24 : 12;

    // Extrude along the shortest box dimension so the spline wraps the largest face.
    var axisIndex = 0;
    if (bboxSize[1] < bboxSize[axisIndex])
        axisIndex = 1;
    if (bboxSize[2] < bboxSize[axisIndex])
        axisIndex = 2;

    const section = sampleSilhouette(context, definition, baseCsys, axisIndex, dirCalls);

    var zLo = section.zMin;
    var zHi = section.zMax;
    if (zHi - zLo < 0.01 * inch)
    {
        const mid = (zHi + zLo) / 2;
        zLo = mid - 0.005 * inch;
        zHi = mid + 0.005 * inch;
    }

    // The circumscribing polygon's vertices lie on the support (tangent) lines, so a
    // spline interpolated straight through them bows inward across flat faces and would
    // clip the part.  Collapse the coincident vertices that flat faces produce, then
    // nudge each vertex outward by the local chord sagitta so the smooth profile stays
    // outside the silhouette.
    const step = (180 * degree) / dirCalls;
    const profile = inflatePolygonForSpline(dedupePolygon(section.polygon), step);

    // Closed fit spline through the profile points (first point repeated to close).
    var profilePoints = profile;
    profilePoints = append(profilePoints, profile[0]);

    const sketchId = id + "fitSplineSketch";
    var sketchPlane = XY_PLANE;
    sketchPlane.origin[2] = zLo;
    const sketch = newSketchOnPlane(context, sketchId, { "sketchPlane" : sketchPlane });
    skFitSpline(sketch, "silhouette", { "points" : profilePoints });
    skSolve(sketch);

    opExtrude(context, id + "fitSplineExtrude", {
                "entities" : makeQuery(sketchId + "imprint", "IMPRINT", EntityType.FACE, {}),
                "startBound" : BoundingType.BLIND,
                "endBound" : BoundingType.BLIND,
                "startDepth" : 0 * inch,
                "direction" : [0, 0, 1],
                "endDepth" : zHi - zLo
            });

    opDeleteBodies(context, id + "fitSplineDeleteSketch", {
                "entities" : qCreatedBy(sketchId, EntityType.BODY)
            });

    const body = qCreatedBy(id + "fitSplineExtrude", EntityType.BODY);
    opTransform(context, id + "fitSplineXform", {
                "bodies" : body,
                "transform" : transform(XY_PLANE, plane(section.buildCsys))
            });

    return body;
}

/**
 * Samples the convex silhouette of `definition.entities`, projected onto the plane
 * perpendicular to local axis `axisIndex` of `baseCsys`.
 *
 * The part's support distance is measured in `2 * dirCalls` evenly spaced directions
 * using evBox3d in rotated frames (each call yields two opposite directions), and the
 * silhouette polygon is reconstructed as the intersection of consecutive support
 * lines.  Coordinates are expressed in `buildCsys`, a frame whose z-axis is the chosen
 * axis and whose xy-plane holds the polygon.
 *
 * @return {{
 *      @field polygon {array} : convex silhouette points, as 2D length vectors in the
 *              `buildCsys` xy-plane, ordered by angle.
 *      @field zMin {ValueWithUnits} : minimum part extent along the chosen axis.
 *      @field zMax {ValueWithUnits} : maximum part extent along the chosen axis.
 *      @field buildCsys {CoordSystem} : frame in which the polygon and z-range live.
 * }}
 */
function sampleSilhouette(context is Context, definition is map, baseCsys is CoordSystem, axisIndex is number, dirCalls is number)
{
    const xA = baseCsys.xAxis;
    const zA = baseCsys.zAxis;
    const yA = cross(zA, xA); // y-axis is not stored on a CoordSystem

    var axisDir;
    var uDir;
    if (axisIndex == 0)
    {
        axisDir = xA;
        uDir = yA;
    }
    else if (axisIndex == 1)
    {
        axisDir = yA;
        uDir = xA;
    }
    else
    {
        axisDir = zA;
        uDir = xA;
    }

    const vDir = cross(axisDir, uDir);
    const buildCsys = coordSystem(baseCsys.origin, uDir, axisDir);

    const step = (180 * degree) / dirCalls;

    var supportMax = [];
    var supportMin = [];
    var zMin;
    var zMax;

    for (var s = 0; s < dirCalls; s += 1)
    {
        const phi = s * step;
        const dir = cos(phi) * uDir + sin(phi) * vDir;
        const dirCsys = coordSystem(baseCsys.origin, dir, axisDir);
        const bb = evBox3d(context, {
                    "topology" : definition.entities,
                    "cSys" : dirCsys,
                    "tight" : true
                });

        // Support distance from the origin along +dir and -dir.
        supportMax = append(supportMax, bb.maxCorner[0]);
        supportMin = append(supportMin, -bb.minCorner[0]);

        if (s == 0)
        {
            zMin = bb.minCorner[2];
            zMax = bb.maxCorner[2];
        }
    }

    // Support value for each of the 2 * dirCalls evenly spaced directions (k * step).
    const total = 2 * dirCalls;
    var polygon = [];
    for (var k = 0; k < total; k += 1)
    {
        const angle1 = k * step;
        const support1 = (k < dirCalls) ? supportMax[k] : supportMin[k - dirCalls];

        const kNext = (k + 1) % total;
        const angle2 = (k + 1) * step; // kept increasing so sin(angle2 - angle1) > 0
        const support2 = (kNext < dirCalls) ? supportMax[kNext] : supportMin[kNext - dirCalls];

        polygon = append(polygon, intersectSupportLines(angle1, support1, angle2, support2));
    }

    return {
        "polygon" : polygon,
        "zMin" : zMin,
        "zMax" : zMax,
        "buildCsys" : buildCsys
    };
}

/**
 * Intersection point of two support lines  p . (cos a, sin a) = h.
 */
function intersectSupportLines(angle1, support1, angle2, support2) returns Vector
{
    const c1 = cos(angle1);
    const s1 = sin(angle1);
    const c2 = cos(angle2);
    const s2 = sin(angle2);
    const det = c1 * s2 - s1 * c2; // = sin(angle2 - angle1), > 0 for our spacing

    const x = (support1 * s2 - support2 * s1) / det;
    const y = (support2 * c1 - support1 * c2) / det;
    return vector(x, y);
}

/**
 * Minimum (near-optimal) enclosing circle of a set of 2D length points, using the
 * Badoiu-Clarkson iterative scheme.  The final radius is the exact maximum distance
 * from the converged centre, so every input point is enclosed.
 *
 * @return {{ @field center {Vector} : the 2D centre. @field radius {ValueWithUnits} }}
 */
function minimumEnclosingCircle(points is array)
{
    var center = vector(0 * inch, 0 * inch);
    for (var p in points)
        center = center + p;
    center = center / size(points);

    const iterations = 300;
    for (var i = 1; i <= iterations; i += 1)
    {
        var farPoint = points[0];
        var farDist = norm(points[0] - center);
        for (var p in points)
        {
            const d = norm(p - center);
            if (d > farDist)
            {
                farDist = d;
                farPoint = p;
            }
        }
        center = center + (farPoint - center) * (1.0 / (i + 1));
    }

    var radius = 0 * inch;
    for (var p in points)
        radius = max(radius, norm(p - center));

    return { "center" : center, "radius" : radius };
}

/**
 * Collapses consecutive (and wrap-around) coincident points.  Flat faces of the part
 * produce several support-line intersections at the same corner, which must be merged
 * before a spline is fitted through them.
 */
function dedupePolygon(polygon is array) returns array
{
    const n = size(polygon);
    if (n < 2)
        return polygon;

    var centroid = vector(0 * inch, 0 * inch);
    for (var p in polygon)
        centroid = centroid + p;
    centroid = centroid / n;

    var maxR = 0 * inch;
    for (var p in polygon)
        maxR = max(maxR, norm(p - centroid));
    const mergeTol = max(1e-5 * maxR, 1e-7 * meter);

    var result = [];
    for (var p in polygon)
    {
        if (size(result) == 0 || norm(p - result[size(result) - 1]) > mergeTol)
            result = append(result, p);
    }
    // Merge the wrap-around seam (last vs first) too.
    if (size(result) > 2 && norm(result[size(result) - 1] - result[0]) <= mergeTol)
        result = resize(result, size(result) - 1);
    return result;
}

/**
 * Nudges each convex-polygon vertex outward (along the outward bisector of its two
 * edges) by the local chord sagitta, so a spline interpolated through the result stays
 * outside the original circumscribing polygon's edges and therefore outside the part.
 */
function inflatePolygonForSpline(polygon is array, step) returns array
{
    const n = size(polygon);
    if (n < 3)
        return polygon;

    var result = [];
    for (var k = 0; k < n; k += 1)
    {
        const prev = polygon[(k - 1 + n) % n];
        const here = polygon[k];
        const next = polygon[(k + 1) % n];

        const toPrev = prev - here;
        const toNext = next - here;
        const edgeLen = max(norm(toPrev), norm(toNext));
        const margin = 0.5 * edgeLen * tan(step / 2);

        // (toPrev + toNext) points toward the polygon interior at a convex vertex.
        const inward = toPrev + toNext;
        const inwardLen = norm(inward);
        if (inwardLen > 1e-9 * meter)
            result = append(result, here - inward / inwardLen * margin);
        else
            result = append(result, here);
    }
    return result;
}

/**
 * Display name for a fitted bounding shape.
 */
function boundingShapeName(boxType is BoxType) returns string
{
    if (boxType == BoxType.CIRCULAR)
        return "Bounding cylinder";
    else if (boxType == BoxType.SPHERE)
        return "Bounding sphere";
    else if (boxType == BoxType.SPLINE)
        return "Bounding spline";
    return "Bounding box";
}

