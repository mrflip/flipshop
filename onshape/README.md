### Onshape Projects

Hello robots! See ../AGENTS.md for other important guidelines.

## Subdirectory Style

The onshape/flipshop directory has the actual Featurescript artifacts for this repo. The other directories are mirrors of the standard library and important community contributors:

Mirrors of official Onshape Reference Libraries

* `std/`: git@github.com:javawizard/onshape-std-library-mirror.git -- the onshape standard libraries
* `community/`: various community contributed onshape extensions. Agentic coders should look here for examples
* `cadsharp/`: utility scripts written by a top onshape user -- these demonstrate many expert aspects and best practices of Onshape Featurescript programming
* `flipshop`: this project's contributions

## onshape/std Field Atlas

A compact map of the FeatureScript standard library's core geometry types and the functions
that build and mutate them, distilled from the doc comments in `onshape/std`. Source files:
`geomOperations.fs`, `transform.fs`, `box.fs`, `sketch.fs`, `query.fs`, `math.fs`, `string.fs`,
`containers.fs`, plus the typedefs behind `Context`, `Query`, `Transform`, `CoordSystem`,
`Box3d`, `Path`, `Sketch`, `Line`, `Plane`, and a few evaluate-result types.
Sheet metal, threads, splines, surfaces and release/validation plumbing are left out except
where noted. Evaluators (`evaluate.fs`'s `evX` functions) are not yet covered.

### Legend

|               |                                                                                                                                                                     |
| ------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Q`           | Query                                                                                                                                                               |
| `V`           | Vector, dims unspecified                                                                                                                                            |
| `V2`          | Vector, 2D -- eg sketch plane; elements are ValueWithUnits                                                                                                          |
| `V3`          | Vector, 3D; elements are ValueWithUnits                                                                                                                             |
| `             |
| `M`           | Matrix                                                                                                                                                              |
| `VWU`         | ValueWithUnits                                                                                                                                                      |
| `MWU`         | MatrixWithUnits                                                                                                                                                     |
| `T`           | Transform                                                                                                                                                           |
| `Ctx`         | Context                                                                                                                                                             |
| `id`          | Id (string path)                                                                                                                                                    |
| `?`           | optional field/arg                                                                                                                                                  |
| `→`           | returns                                                                                                                                                             |
| `curvature`   | always has type `ValueWithUnits<inverse length>`; eg `maxCurvature` or `curvature`                                                                                  |
| `xxFrac`      | fractional (i.e. 0 <= unitless <= 1) number giving the proportional location along a curve/line/edge: 0.0 is the start, 1.0 is the end                                        |
| `xxFracV:VF2` | fractional (i.e. 0 <= unitless <= 1) vector giving the proportional location along the bounding box of a face/plane/etc: 0.0,0.0 is the bottom left, 1.0,1.0 is the top right |
| `fooV:VU3`    | 3D unit vector: magnitude is 1.0.                                                                                                                                   |
| `qy:1`        | query must resolve to exactly one entity                                                                                                                            |
| `!`           | required; on an array, also has at least one element; on a query, also resolves to one or more entities                                                             |
| `?`           | optional                                                                                                                                                            |
| `[thisQ, thatV, theOther?]`            | a tuple of typed elements (in this case, a query, a vector and an optional value)                                                                                   |
| `str` / `num` / `bool` / `arr` / `map` | string / number / boolean / array / generic record                                                                                                                  |


### Same concept, different names

The actual source of the disorientation: five recurring ideas, and the field names the
library reaches for each time it needs one. These names were picked op-by-op over a decade,
not from one schema. The one bright spot: `origin` is genuinely consistent everywhere a local
frame needs an anchor.

| Concept                  | Names in the wild                                                                                                                                                        | Seen on                                                                                                                                                    |
| ------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ---------------------------------------------------------------------------------------------------------------------------------------------------------- |
| stuff this op acts on    | `entities` / `tools`+`targets` / `bodies` / `moveFaces` / `deleteFaces` / `faceTargets`+`edgeTools`+`faceTools`+`bodyTools`+`planeTools` / `replaceFaces`+`templateFace` | opChamfer, opFillet, opDraft, opRevolve, opShell, opThicken / opBoolean / opTransform, opPattern / opMoveFace / opDeleteFace / opSplitFace / opReplaceFace |
| "how far" to move/resize | `endDepth`/`startDepth` / `offsetDistance` / `thickness1`/`thickness2` / `width`/`width1`/`width2` / `radius` | opExtrude / opOffsetFace / opThicken / opChamfer / opFillet, opShell                                                                                       |
| first point of a pair    | `minCorner` / `firstCorner` / `start`         | Box3d / skRectangle, skText / skLineSegment                                                                                                                |
| second point of a pair   | `maxCorner` / `secondCorner` / `end`          | Box3d / skRectangle, skText / skLineSegment                                                                                                                |
| local frame anchor point | `origin` *(consistent!)*                      | CoordSystem, Line, Plane, PersistentCoordSystem                                                                                                            |
| a transform to apply     | `transform` (singular) / `transforms` (array) | opMoveFace, opTransform / opPattern                                                                                                                        |

### Core typedefs — the nouns

Every `export type X typecheck canBeX` worth knowing, grouped by what kind of thing it is.
Field types are dropped where the name already says it (e.g. `angle`, `radius`).

| Type                      | Kind     | Fields                                                                                      |
| ------------------------- | -------- | ------------------------------------------------------------------------------------------- |
| `Context`                 | core     | opaque builtin — `@isContext(value)`                                                        |
| `Id`                      | core     | `arr<str>` — path segments, each `[a-zA-Z0-9_.+/-]*` or `ANY_ID`                            |
| `Query`                   | core     | `queryType:QueryType \| historyType:str`, `entityType?:EntityType`                          |
| `Vector`                  | math     | `arr` (size > 0) — 2D or 3D, unit-bearing or unitless by convention                         |
| `Matrix`                  | math     | opaque builtin `@isMatrix` — rows × cols nested array                                       |
| `MatrixWithUnits`         | math     | `value:M`, `unit:UnitSpec`                                                                  |
| `ValueWithUnits`          | math     | `value:num`, `unit:UnitSpec`                                                                |
| `UnitSpec`                | math     | `map<dimension:str, exponent:num>`, e.g. `{"meter":1}`                                      |
| `Transform`               | math     | `linear:M` (3×3), `translation:V3`                                                          |
| `Line`                    | geometry | `origin:V3`, `direction:V3`                                                                 |
| `Plane`                   | geometry | `origin:V3`, `x:V3`, `normal:V3` (⟂ to `x`)                                                 |
| `CoordSystem`             | geometry | `origin:V3`, `xAxis:V3`, `zAxis:V3` (⟂ required)                                            |
| `PersistentCoordSystem`   | geometry | `coordSystem?:CoordSystem`, `coordSystemId:str`, `forceRightHanded?:bool`                   |
| `Box3d`                   | geometry | `minCorner:V3`, `maxCorner:V3`                                                              |
| `Path`                    | geometry | `edges:arr<Q>`, `flipped:arr<bool>`, `closed:bool`, `adjacentFaces?:Q`                      |
| `PathDistanceInformation` | geometry | `distance:VWU`, `withinBoundingBox:bool`                                                    |
| `Sketch`                  | sketch   | opaque builtin — `@isSketch(value)`                                                         |
| `Color`                   | props    | `red,green,blue,alpha:num` ∈ [0,1]                                                          |
| `Material`                | props    | `name:str`, `density:VWU`                                                                   |
| `MassProperties`          | eval     | `mass:VWU`, `volume?/area?/length?:VWU`, `count?:num`, `centroid:V`, `inertia:MWU`          |
| `DistanceResult`          | eval     | `distance:VWU`, `sides:arr[2]` of `{index:num, point:V3, parameter}`                        |
| `RaycastResult`           | eval     | `entity:Q`, `entityType:EntityType`, `parameter:num\|V2`, `intersection:V3`, `distance:VWU` |

### geomOperations.fs — the verbs

Every `opX` shares the same outer shape: `opX(context is Context, id is Id, definition is map)`.
Everything below is the keyword-argument field list inside that `definition` map — the only
part that differs from op to op.

| Op                | Does       | `definition { … }` fields                                                                                                                                                                                                                                         |
| ----------------- | ---------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `opExtrude`       | primitive  | entities, direction, endBound, endDepth, endBoundEntity, endTranslationalOffset?, startBound?, isStartBoundOpposite, startDepth, startBoundEntity, startTranslationalOffset?                                                                                      |
| `opLoft`          | primitive  | profileSubqueries, guideSubqueries?, connections?, connectionsArcLengthParameterization, makePeriodic?, bodyType?, trimGuidesByProfiles, trimProfiles, derivativeInfo?, showIsocurves?, curveCount?, loftTopology?, addSections?, spine?, sectionCount            |
| `opFillet`        | edge-treat | entities, radius, tangentPropagation?, crossSection?, rho, magnitude, partialFilletBounds?, isVariable?, vertexSettings?, pointOnEdgeSettings?, smoothTransition, allowEdgeOverflow?, keepEdges?, smoothCorners?, smoothCornerExceptions?, createDetachedSurface? |
| `opChamfer`       | edge-treat | entities, chamferType, width, width1, width2, angle, oppositeDirection, tangentPropagation?                                                                                                                                                                       |
| `opDraft`         | edge-treat | draftType, draftFaces, referenceSurface, referenceEntityDraftOptions, pullVec:V3, angle, tangentPropagation?, referenceEntityPropagation?, reFillet?                                                                                                              |
| `opBodyDraft`     | edge-treat | selectionType?, topEdges, bottomEdges, faces, bodies, excludeFaces?, angle, bothSides?, pullDirection, draftOnSelf?, partingObject, matchFacesAtParting?, matchFaceType?, cornerType?, concaveRepair?, concaveRepairRadius, keepMaterial?, showRefs?              |
| `opSplitFace`     | split      | faceTargets, edgeTools?, projectionType?, direction?, faceTools?, bodyTools?, keepToolSurfaces?, planeTools?, extendToCompletion?, mutualImprint?                                                                                                                 |
| `opHole`          | feature    | holeDefinition, axes:arr\<Line\>, identities?, targets, subtractFromTargets?, targetsToExcludeFromSubtraction?, keepTools?                                                                                                                                        |
| `opRevolve`       | primitive  | entities, axis:Line, angleForward, angleBack                                          |
| `opSweep`         | primitive  | profiles, path, keepProfileOrientation, lockFaces?, lockDirection?, profileControl?   |
| `opPlane`         | primitive  | plane:Plane, width?, height?, defaultType? *(internal)*                               |
| `opPoint`         | primitive  | point:V3, origin? *(internal)*                                                        |
| `opPolyline`      | primitive  | points:arr\<V3\>, bendRadii:arr\<VWU\>                                                |
| `opBoolean`       | boolean    | tools, targets, operationType, targetsAndToolsNeedGrouping?, keepTools?, makeSolid?   |
| `opTransform`     | transform  | bodies, transform:T                                                                   |
| `opPattern`       | pattern    | entities, transforms:arr\<T\>, instanceNames:arr\<str\>, copyPropertiesAndAttributes? |
| `opShell`         | solidify   | entities, thickness                                                                   |
| `opThicken`       | solidify   | entities, thickness1, thickness2, keepTools?                                          |
| `opEnclose`       | solidify   | entities                                                                              |
| `opMoveFace`      | face-edit  | moveFaces, transform:T, reFillet?, mergeFaces?                                        |
| `opOffsetFace`    | face-edit  | moveFaces, offsetDistance, reFillet?, mergeFaces?                                     |
| `opReplaceFace`   | face-edit  | replaceFaces, templateFace, offset?, oppositeSense?                                   |
| `opDeleteFace`    | face-edit  | deleteFaces, includeFillet, capVoid, leaveOpen?                                       |
| `opSplitPart`     | split      | targets, tool *(Query \| Plane)*, keepTools?, keepType?, useTrimmed?                  |
| `opDeleteBodies`  | delete     | entities                                                                              |
| `opMateConnector` | assembly   | coordSystem:CoordSystem, owner                                                        |
| `opMergeContexts` | context    | *context = target;* contextFrom:Ctx, trackThroughMerge?                               |

Left out on purpose: surface/spline-only ops (opCreateBSplineCurve/Surface, opConstrainedSurface,
opFitSpline, opRuledSurface, opExtractSurface/Wires), sheet-metal (opSMFlatOperation), and the
shadow/isocline split variants — narrower tools, same `(ctx, id, definition)` shape.

### transform.fs

Unlike the ops above, these take real positional arguments — no `definition` map to unpack.

| Function                                    | Does      | Args                                             |
| ------------------------------------------- | --------- | ------------------------------------------------ |
| `transform(linear, translation)`            | construct | linear:M, translation:V3                         |
| `transform(translation)`                    | construct | translation:V3 *(no rotation/scale)*             |
| `transform(value)`                          | cast      | value:map → T *(raw reinterpret)*                |
| `transformFromBuiltin(definition)`          | internal  | definition:map{linear, translation}              |
| `identityTransform()`                       | construct | — → T                                            |
| `inverse(t)`                                | invert    | t:T                                              |
| `scaleUniformly(scale)`                     | construct | scale:num *(about origin)*                       |
| `scaleUniformly(scale, pointToScaleAbout)`  | construct | scale:num, pointToScaleAbout:V3                  |
| `scaleNonuniformly(xScale, yScale, zScale)` | construct | xScale, yScale, zScale:num *(about origin)*      |
| `scaleNonuniformly(…, pointToScaleAbout)`   | construct | xScale, yScale, zScale:num, pointToScaleAbout:V3 |
| `tolerantEquals(transform1, transform2)`    | predicate | transform1, transform2:T                         |
| `operator * (t1, t2)`                       | compose   | t1, t2:T → T *(t1 applied after t2)*             |
| `operator * (t, v)`                         | apply     | t:T, v:V3 → V3                                   |

### box.fs — Box3d

The whole file, seven functions plus a predicate — short enough to cover completely.

| Function                                   | Does      | Args                                                                  |
| ------------------------------------------ | --------- | --------------------------------------------------------------------- |
| `box3d(minCorner, maxCorner)`              | construct | minCorner, maxCorner:V3 *(auto-sorted per axis)*                      |
| `box3d(pointArray)`                        | construct | pointArray:arr\<V3\> *(bounding box of all points)*                   |
| `transformBox3d(boxIn, transformation)`    | transform | boxIn:Box3d, transformation:T *(re-bounds the 8 transformed corners)* |
| `extendBox3d(bBox, absoluteValue, factor)` | resize    | bBox:Box3d, absoluteValue:VWU, factor:num                             |
| `box3dCenter(bBox)`                        | query     | bBox:Box3d → V3                                                       |
| `box3dDiagonalLength(bBox)`                | query     | bBox:Box3d → VWU                                                      |
| `box3dAllCorners(bBox)`                    | query     | bBox:Box3d → arr\<V3\> [8]                                            |
| `insideBox3d(point, bBox)`                 | predicate | point:V3, bBox:Box3d                                                  |

### sketch.fs

Sketch entity functions all share the shape `skX(sketch is Sketch, xId is string, value is map)` —
again, the field list inside `value` is what varies. Note `firstCorner`/`secondCorner`
reappearing here instead of `start`/`end`, same idea as row 2–3 of the Rosetta table above.

| Function            | Entity     | `value { … }` fields                                                                                           |
| ------------------- | ---------- | -------------------------------------------------------------------------------------------------------------- |
| `newSketch`         | create     | (ctx, id, value) — sketchPlane:Q, disableImprinting?                                                           |
| `newSketchOnPlane`  | create     | (ctx, id, value) — sketchPlane:Plane                                                                           |
| `skSolve`           | solve      | (sketch)                                                                                                       |
| `skSetInitialGuess` | solve      | (sketch, initialGuess:map)                                                                                     |
| `skPoint`           | entity     | position:V2                                                                                                    |
| `skLineSegment`     | entity     | start:V2, end:V2, construction?                                                                                |
| `skCircle`          | entity     | center:V2, radius:VWU, construction?                                                                           |
| `skEllipse`         | entity     | center:V2, majorRadius:VWU, minorRadius:VWU, majorAxis?:V2, construction?                                      |
| `skArc`             | entity     | start:V2, mid:V2, end:V2, construction?                                                                        |
| `skEllipticalArc`   | entity     | center:V2, majorAxis:V2, minorRadius:VWU, majorRadius:VWU, startParameter:num, endParameter:num, construction? |
| `skRectangle`       | entity     | firstCorner:V2, secondCorner:V2, construction?                                                                 |
| `skRegularPolygon`  | entity     | center:V2, firstVertex:V2, sides:num, construction?                                                            |
| `skPolyline`        | entity     | points:arr\<V2\>, construction?, constrained?                                                                  |
| `skFitSpline`       | entity     | points:arr, parameters?:arr\<num\>, construction?, startDerivative?:V2, endDerivative?:V2                      |
| `skText`            | entity     | text:str, fontName:str, construction?, firstCorner?:V2, secondCorner?:V2, mirrorHorizontal?, mirrorVertical?   |
| `skConstraint`      | constraint | constraintType:ConstraintType, length?:VWU, angle?:VWU                                                         |

Left out: skSpline / skSplineSegment / skInterpolatedSpline(Segment) / skBezier / skImage /
skConicSegment — same map-shaped calls, spline- and import-specific fields.

### Utilities

Not full signatures — just a catalog of what's exported, so you know where to look.
`math.fs` and `containers.fs` are generic; `string.fs` is, well, strings.

| File            | Catalog                                                                                                                                                                                                                                                                                                                                                                                       |
| --------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `math.fs`       | tolerantEquals, abs, sqrt, log, log10, sinh, cosh, tanh, asinh, acosh, atanh, exp, exp2, hypot, floor, ceil, round, roundToPrecision, roundWithinTolerance, min, max, argMin, argMax, range, clamp, isInteger, isNonNegativeInteger, isPositiveInteger, PI, TOLERANCE                                                                                                                         |
| `string.fs`     | toString, print, println, splitIntoCharacters, parseJson, addCustomNumberMatching, match, replace, stringToNumber, length, substring, startsWith, endsWith, splitByRegexp, indexOf, indexOfRegexp, repeatString, isUndefinedOrEmptyString, join, REGEX_NUMBER, REGEX_NUMBER_CAPTURE                                                                                                           |
| `containers.fs` | makeArray, size, isIn, indexOf, isValueIn, mapArray, resize, append, concatenateArrays, mergeMaps, intersectMaps, reverse, mapLookup, sort, tolerantSort, filter, first, keys, values, subArray, insertIntoMapOfArrays, last, rotateArray, insertElementAt, removeElementAt, all, allCombinations, any, average, deduplicate, foldArray, mapArrayIndices, mapValue, memoizeFunction, sum, zip |

### Queries

Everything in `onshape/std/query.fs` — the `qX(...)` constructors that build a `Query`, plus
the handful of functions that consume one. Positional arg names below are **not** the
library's own parameter names; they're renamed to a consistent convention so you can tell at a
glance what's generic, what's specific, and what's plural-safe:

| Notation                    | Means                                                         |
| ----------------------------| ------------------------------------------------------------- |
| `qy`                        | a fully generic Query argument — no further meaning           |
| `xQ`                        | a Query argument with a specific role (e.g. `seedQ`, `bodyQ`) |
| `xQ:1!`                     | this argument must resolve to **exactly one** entity          |
| `name?`                     | optional argument, of whatever type fits                      |
| `x[]`                       | an array of `x`                                               |
| `[thisQ, thatV, theOther?]  | a tuple of typed elements (in this case, a query, a vector and an optional value) |
| `EnumType.X`                | a specific enum value is expected (`X` stands for "pick one") |
| `EnumType[]`                | an array of enum values                                       |
| plain names (`featureId`, `plane`, `line`, `point`, `direction`, `radius`, …) | a non-Query argument — see each row                           |

Almost everything here is written to be plural-safe: pass a query that matches five faces and
you get results for all five. The one explicit exception is `qHoleFaces`, whose `seed` the
docs require to be a single face.

**Special**

| Function                                                 . | Does                                                                                          |
| -----------------------------------------------------      | --------------------------------------------------------------------------------------------- |
| `qNothing()`                                               | Empty query; matches nothing.                                                                 |
| `qEverything(EntityType.X?)`                               | Every entity in the context, optionally filtered to one EntityType.                           |
| `qAllSolidBodies()`                                        | All solid bod(ies) (`BodyType.SOLID`).                                                        |
| `qAllNonMeshSolidBodies()`                                 | All solid bod(ies) with no mesh geometry.                                                     |
| `qAllModifiableSolidBodies()`                              | All modifiable solid bod(ies), mesh included.                                                 |
| `qAllModifiableSolidBodiesNoMesh()`                        | All modifiable, non-mesh solid bod(ies) — everything in the Part Studio's Parts list.         |
| `qNthElement(qy, n)`                                       | The nth entity of qy (0-based; `-1` = last). qy must resolve to at least `n+1` entities.      |
| `qEntityFilter(qy, EntityType.X)`                          | Entit(ies) of qy matching EntityType.X.                                                       |
| `qHasAttribute([qy], name)`                                | Entit(ies) (in qy, or in the whole context) carrying an attribute named `name`.               |
| `qHasAttributeWithValue([qy], name, value)`                | …carrying attribute `name` equal to `value`.                                                  |
| `qHasAttributeWithValueMatching([qy], name, pattern)`      | …carrying attribute `name` whose map value matches every key in `pattern`.                    |
| `qAttributeFilter([qy], pattern)`                          | Legacy unnamed-attribute match against `pattern` (type-tag aware).                            |
| `qCreatedBy(featureId, EntityType.X?)`                     | Entit(ies) created by `featureId` — accepts a single Id or a FeatureList (unions across all). |
| `qCapEntity(featureId, CapType.X, EntityType.X?)`          | Start/end cap entit(ies) of `featureId` (extrude, revolve, sweep, loft, thicken).             |
| `qNonCapEntity(featureId, EntityType.X?)`                  | Entit(ies) created by `featureId`, excluding cap entities.                                    |
| `qOpHoleProfile(featureId, {name?, identityQ?})`           | Profile edges/vertices of the `opHole` at `featureId`.                                        |
| `qOpHoleFace(featureId, {name?, identityQ?})`              | Hole faces of the `opHole` at `featureId`.                                                    |
| `qToleranceFilter(qy, threshold?)`                         | Edges/vertices of qy shorter than `threshold` (default zero-length tolerance).                |

**Boolean combinators**

| Function                                                 . | Does                                                                         |
| --------------------------------                           | ---------------------------------------------------------------------------- |
| `qUnion([qy])`                                             | Entit(ies) matching any of the listed querie(s); preserves input order.      |
| `qIntersection([qy])`                                      | Entit(ies) matching all of the listed queries; preserves order of the first. |
| `qSubtraction(qy1, qy2)`                                   | Entit(ies) in qy1 but not qy2; preserves order of qy1.                       |
| `qSymmetricDifference(qy1, qy2)`                           | Entit(ies) in exactly one of qy1 or qy2.                                     |

**Topological**

| Function                                                   | Does                                                                                           |
| ---------------------------------------------------------- | ---------------------------------------------------------------------------------------------- |
| `qOwnedByBody(qy, bodyQ)`                                  | Entit(ies) owned by bodyQ. *(or `qOwnedByBody(bodyQ, EntityType.X)` with no pre-filter query)* |
| `qOwnerBody(qy)`                                           | The owning body/bodies of qy (includes the body itself if one was passed in).                  |
| `qContainedInCompositeParts(compositePartsQ)`              | Part(s) contained inside compositePartsQ.                                                      |
| `qCompositePartsContaining(bodiesQ, CompositePartType.X?)` | Composite part(s) that contain bodiesQ.                                                        |
| `qFlattenedCompositeParts(qy)`                             | qy's non-composite entities, plus the constituents of any composite parts in qy.               |
| `qConsumed(qy, Consumed.X)`                                | Entit(ies) of qy that are (or aren't) consumed by a closed composite part.                     |
| `qCompositePartTypeFilter(qy, CompositePartType.X)`        | Bodies of qy that are open or closed composite parts.                                          |
| `qAdjacent(seedQ, AdjacencyType.X, EntityType.X?)`         | Entities sharing a vertex or edge with seedQ (seedQ itself excluded).                          |
| `qEdgeTopologyFilter(qy, EdgeTopology.X)`                  | Edges of qy matching an `EdgeTopology`.                                                        |
| `qEdgeVertex(edgesQ, atStart?)`                            | Start (or end) vertices of edgesQ.                                                             |

**Geometry type**

| Function                                                 . | Does                                                                      |
| -----------------------------------------------            | ------------------------------------------------------------------------- |
| `qGeometry(qy, GeometryType.X)`                            | Entit(ies) of qy with a given `GeometryType` (LINE, CIRCLE, CYLINDER, …). |
| `qBodyType(qy, [BodyType])`                                | Entit(ies) of qy belonging to body/bodies of the given `BodyType`(s).     |
| `qConstructionFilter(qy, ConstructionObject.X)`            | Construction (or non-construction) entities of qy.                        |

**Geometry matching**

| Function                                                 . | Does                                                                                                                 |
| -----------------------------------------------------      | -------------------------------------------------------------------------------------------------------------------- |
| `qParallelPlanes(qy, refPlane, includeAntiparallel?)`      | Entities strictly parallel to refPlane, optionally including antiparallel ones.                                      |
| `qPlanesParallelToDirection(qy, direction)`                | Planar faces of qy whose normal is ⟂ to `direction`.                                                                 |
| `qFacesParallelToDirection(qy, direction)`                 | Faces of qy parallel to `direction` (normal ⟂ for planar, axis ∥ for cylindrical, extrude direction ∥ for extruded). |

**Face & edge**

| Function                                         | Does                                                                                     |
| ------------------------------------------------ | ---------------------------------------------------------------------------------------- |
| `qConvexConnectedFaces(seedQ)`                   | Faces flood-filled from seedQ across convex edges.                                       |
| `qConcaveConnectedFaces(seedQ)`                  | Faces flood-filled from seedQ across concave edges.                                      |
| `qTangentConnectedFaces(seedQ, angleTolerance?)` | Entities connected to seedQ via tangent edges, flood-filling across any number of tangent edges. A tangent edge is an edge joining two faces such that the surface direction is continuous across the edge, up to the given `angleTolerance`, at every point along the full length of the edge. |
| `qTangentConnectedEdges(seedQ)`                  | Chain of tangent edges connected to seedQ via tangent vertices.                          |
| `qLoopEdges(seedQ)`                              | The edge loop(s) containing seedQ's laminar edges, or bounding seedQ's faces.            |
| `qParallelEdges(qy, direction)`                  | Linear edges of qy (anti)parallel to `direction`. *(or pass `edgesQ` in place of `direction` to match against any linear edge in edgesQ)*                                                                                                                                                       |
| `qLoopBoundedFaces(seedQ)`                       | Faces bounded by the face in seedQ, on the side of the edge in seedQ — e.g. select an entire pocket via `qUnion([pocketFace, touchingEdge])`. *(seedQ = `qUnion([faceQ:1!, edgeQ:1!])`; extras beyond the first of each are ignored)*                                                           |
| `qFaceOrEdgeBoundedFaces(seedQ)`                 | Faces adjacent to the seed face in seedQ, flood-filling outward until blocked by the other entities in seedQ. *(seedQ = `qUnion([seedFaceQ:1!, [boundaryQ]])` — the seed face must be ordered first)*                                                                                           |
| `qHoleFaces(seedQ:1!)`                           | All faces of the hole containing seedQ.                                                  |
| `qSketchRegion(featureId, filterInnerLoops?)`    | Closed 2D region(s) from the sketch at `featureId`.                                      |
| `qUniqueVertices(qy)`                            | qy's vertices, deduplicated (keeps the lowest deterministic ID of each duplicate set).   |
| `qMateConnectorsOfParts(partsQ)`                 | Mate connectors owned by partsQ.                                                         |
| `qFilletFaces(facesQ, CompareType.X)`            | Fillet faces on the same body as facesQ, compared by radius.                             |
| `qMatching(qy)`                                  | Faces/edges geometrically identical (same size & shape) to qy, within qy's owner bodies. |
| `qDependency(qy)`                                | The true dependency of qy (e.g. an extrude's profile edges).                             |
| `qLaminarDependency(qy)`                         | Like `qDependency`, but follows back to the nearest laminar edge.                        |
| `qPatternInstances(featureId, [instanceName], EntityType.X)` | Entities created by named instance(s) of the `opPattern` at `featureId`.     |

**Containment & intersection**

| Function                           | Does                                                                             |
| ---------------------------------- | -------------------------------------------------------------------------------- |
| `qContainsPoint(qy, point)`        | Entit(ies) of qy containing `point`.                                             |
| `qIntersectsLine(qy, line)`        | Entit(ies) of qy touching the infinite `line`.                                   |
| `qIntersectsPlane(qy, plane)`      | Entit(ies) of qy touching the infinite `plane`.                                  |
| `qInFrontOfPlane(qy, plane)`       | Entit(ies) of qy entirely on the normal side of `plane` (nothing straddling it). |
| `qCoincidesWithPlane(qy, plane)`   | Entit(ies) of qy coincident with `plane`.                                        |
| `qWithinRadius(qy, point, radius)` | Entit(ies) of qy within `radius` of `point`.                                     |

**Optimization**

| Function                                            | Does                                                                                                                                                |
| --------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------- |
| `qClosestTo(qy, point)`                             | Entity/ies of qy closest to `point` (ties within `TOLERANCE.zeroLength` all returned).                                                              |
| `qFarthestAlong(qy, direction)`                     | Entity/ies of qy farthest along `direction`.                                                                                                        |
| `qLargest(qy)`                                      | The largest entity/ies of qy, by length, area, or volume (highest dimension wins across ties).                                                      |
| `qSmallest(qy)`                                     | The smallest entity/ies of qy, by length, area, or volume.                                                                                          |
| `qEdgeConvexityTypeFilter(qy, EdgeConvexityType.X)` | Edges of qy matching a convexity type.                                                                                                              |
| `qAxis(qy, axis)`                                   | Axis-symmetric entities of qy (lines, circles, cylinders, cones, tori, revolved surfaces, mate connectors) sharing `axis` (direction sign ignored). |

**Mesh & sheet metal** *(outside this doc's usual scope, but they live in query.fs too)*

| Function                                                          | Does                                                                          |
| ----------------------------------------------------------------- | ----------------------------------------------------------------------------- |
| `qSourceMesh(meshVerticesQ, EntityType.X)`                        | Mesh element(s) owning the selected meshVerticesQ (BODY, FACE, or EDGE only). |
| `qMeshGeometryFilter(qy, MeshGeometry.X)`                         | Mesh (or non-mesh) entities of qy.                                            |
| `qModifiableEntityFilter(qy)`                                     | Entities of qy that aren't in-context (i.e. are modifiable).                  |
| `qSketchFilter(qy, SketchObject.X)`                               | Sketch (or non-sketch) entities of qy.                                        |
| `qActiveSheetMetalFilter(qy, ActiveSheetMetal.X)`                 | Entities of qy belonging (or not) to an active sheet metal model.             |
| `qSheetMetalFlatFilter(qy, SMFlatType.X)`                         | Entities of qy belonging (or not) to a flattened sheet metal part.            |
| `qSheetMetalFormFilter(qy, SMFormType.X)`                         | Entities of qy that are (or aren't) artifacts of a sheet-metal form feature.  |
| `qPartsAttachedTo(smEntitiesQ)`                                   | Part(s) that smEntitiesQ (e.g. bend lines) are attached to.                   |
| `qCorrespondingInFlat(foldedQ)`                                   | The flat-pattern equivalents of foldedQ's 3D sheet metal entities.            |
| `qSMDefinitionEntityFilter(qy, SheetMetalDefinitionEntityType.X)` | Entities of qy defined by a given sheet-metal master-body topology.           |

**Historical / codegen**

| Function                                                      | Does                                                                               |
| ------------------------------------------------------------- | ---------------------------------------------------------------------------------- |
| `qSplitBy(featureId, EntityType.X?, backBody?)`               | Entities on the front (`backBody`=false) or back side of the split at `featureId`. |
| `sketchEntityQuery(featureId, EntityType.X?, sketchEntityId)` | Wire-body entit(ies) created for one sketch entity.                                |

**Query evaluation** *(not constructors — these consume a `Context` to resolve a query)*

| Function                              | Does                                                                                  |
| ------------------------------------- | ------------------------------------------------------------------------------------- |
| `evaluateQuery(ctx, qy)`              | One transient query per entity matching qy, in `qUnion` order where applicable.       |
| `areQueriesEquivalent(ctx, qy1, qy2)` | Whether qy1 and qy2 resolve to the same entities (order-independent).                 |
| `isQueryEmpty(ctx, qy)`               | Whether qy resolves to nothing. Faster than checking `size(evaluateQuery(...)) == 0`. |
| `evaluateQueryCount(ctx, qy)`         | Count of entities matching qy. Faster than `size(evaluateQuery(...))`.                |

### Left out:

* **deprecated aliases**: `qSMFlatFilter`, `qVertexAdjacent`/`qEdgeAdjacent`, `qMatchingFaces`,
`qSourceMesh` 1-arg form, `qCapEntity` boolean form
* **pure codegen/internal plumbing**: `makeQuery`, `dummyQuery`, `qCompressed`, `qCoincidentFilter`, the `*Disambiguation` helpers,
`stripUnits`, `transientQueriesToStrings`
