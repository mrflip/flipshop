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
where noted. Evaluators (`evaluate.fs`'s `evX` functions) are covered further down.

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

---

The sections below came out of a function-call census across `community/`, `cadsharp/`, and
`flipshop/` — every `std` file with real usage that wasn't already covered above. Notation
follows the conventions in the Legend: `x[]` for arrays, `[a, b, c?]` for tuples, `fooV:VU3` for
3D unit vectors, `qy:1` for a query required to resolve to exactly one entity, and `!` for "must
be nonempty" on an array or query.

### vector.fs

The file behind the `Vector` typedef — by far the single most-called thing in the census
(`vector()` alone: 532 hits across the three directories).

| Function | Does | Args |
|---|---|---|
| `vector(x, y)` / `vector(x, y, z)` | construct | 2 or 3 numbers → V |
| `vector(arr)` | construct | wraps an existing `arr!` as V |
| `zeroVector(size)` | construct | size:num → V of zeros |
| `squaredNorm(v)` | math | v:V → num\|VWU *(faster than `norm`, skips the `sqrt`)* |
| `norm(v)` | math | v:V → num\|VWU |
| `dot(v1, v2)` | math | v1, v2:V → num\|VWU |
| `cross(v1, v2)` | math | v1, v2:V3 → V3 |
| `angleBetween(v1, v2, refV?)` | math | v1, v2:V3 → angle ∈ [0,π]; with `refV`, a signed ccw angle as seen from `refV`'s tip, ∈ (-π,π] |
| `normalize(v)` | math | v:V → VU *(throws if zero-length)* |
| `project(targetV, sourceV)` | math | sourceV projected onto targetV → V |
| `perpendicularVector(v)` | construct | v:V3 → VU3, arbitrary but consistent for the same input |
| `rotationMatrix3d(fromV, toV)` | construct | fromV, toV:V3 → M, minimum rotation taking fromV to toV |
| `rotationMatrix3d(axisV, angle)` | construct | axisV:V3, angle:VWU → M, ccw about axisV |
| `scalarTripleProduct(v1, v2, v3)` | math | v1,v2,v3:V3 → num\|VWU *(`v1 · (v2 × v3)`)* |
| `tolerantEquals(v1, v2)` | predicate | same point, or same direction, within tolerance |
| `parallelVectors(v1, v2)` / `perpendicularVectors(v1, v2)` | predicate | angle check within tolerance |
| `clusterPoints(points[], tolerance)` | util | points:V3[] → number[][], indices grouped by proximity |

Predicates for the `Vector` shape itself, not listed as rows: `canBeVector`, `isLengthVector`,
`isUnitlessVector`, `is2dPoint`, `is2dPointVector`, `is2dDirection`, `is3dLengthVector`,
`is3dDirection`.

### feature.fs

The scaffolding every custom feature is built on. `defineFeature` itself is called once per
feature file, so its 40 census hits means 40 distinct features across the three directories.

| Function | Does | Args |
|---|---|---|
| `defineFeature(feature, defaults?)` | wrap | feature:`function(ctx, id, definition)` → function; `defaults` is merged into `definition` before `feature` runs. This wrapper *is* what makes a function a feature — it handles `startFeature`/`endFeature`/`abortFeature` and error status for you. |
| `forEachEntity(ctx, id, qy, operationToPerform)` | iterate | operationToPerform:`function(entityQ:1, innerId)`, called once per entity matched by qy with a disambiguated `innerId` |
| `isAnything(value)` | predicate | always true — typecheck for a feature parameter that accepts any expression |
| `verifyNonemptyQuery(ctx, definition, paramName, error)` | validate | throws+faults `paramName` if `definition[paramName]` resolves to nothing → entityQ[] *(the evaluated result, so you can use it immediately)* |
| `verifyNonemptyArray(ctx, definition, paramName, error)` | validate | throws+faults `paramName` if `definition[paramName]` isn't a nonempty array |
| `setFeatureComputedParameter(ctx, id, {name, value})` | util | makes `value` available in a Feature Name Template as `#name` |
| `getFullPatternTransform(ctx)` | pattern-aware | → T, the full composed transform of the active feature pattern (identity outside one) |
| `getRemainderPatternTransform(ctx, {references})` | pattern-aware | → T, the portion of the pattern transform not already applied to `references` |
| `transformResultIfNecessary(ctx, id, transform)` | pattern-aware | applies `transform` to entities created by `id`, skipped entirely if it's the identity |
| `makeRobustQuery(ctx, qy)` | util | qy ∪ an identity-tracking query, so the result survives identity-preserving upstream edits |
| `adjustAngle(ctx, angle)` | util | wraps `angle` into [0, 2π) *(range-checks instead, on old library versions)* |

Lifecycle internals `defineFeature` calls for you — rarely called directly: `startFeature`,
`endFeature`, `abortFeature`, `callSubfeatureAndProcessStatus`.

### error.fs

How a feature raises and reports problems to the user.

| Function | Does | Args |
|---|---|---|
| `regenError(message, faultyParameters[]?, entitiesQ?)` | construct | message:str\|ErrorStringEnum → map, meant to be `throw`n; `faultyParameters` highlight fields red in the dialog, `entitiesQ` highlights geometry in the viewport |
| `reportFeatureError(ctx, id, message, faultyParameters[]?)` | attach | message:str\|ErrorStringEnum — attaches an error status to `id` *without* throwing (doesn't abort the feature) |
| `reportFeatureWarning(ctx, id, message, faultyParameters[]?)` / `reportFeatureInfo(ctx, id, message, faultyParameters[]?)` | attach | same shape, warning/info severity |
| `getFeatureStatus(ctx, id)` | inspect | → FeatureStatus{statusType, statusEnum, statusMsg?, faultyParameters?} |
| `clearFeatureStatus(ctx, id, definition?)` | inspect | resets `id`'s status to OK |
| `featureHasError(ctx, id)` / `featureHasNonTrivialStatus(ctx, id)` | predicate | → bool |
| `verify(condition, error, regenErrorOptions?)` | util | throws `regenError(error, regenErrorOptions)` if `condition` is false |

### debug.fs

Visible only while the calling feature's edit dialog is open; never affects real geometry or queries.

| Function | Does | Args |
|---|---|---|
| `debug(ctx, value, color?)` | inspect | prints `value`; for Query/V3/direction-V3/Line/CoordSystem/Plane/Box3d, also highlights it in the viewport. Default color red. |
| `addDebugEntities(ctx, qy, color?)` | highlight | highlights qy with no printing |
| `addDebugPoint(ctx, pointV3, color?)` | highlight | draws a single point |
| `addDebugLine(ctx, p1V3, p2V3, color?)` | highlight | draws a line between two points, prints the distance |
| `addDebugArrow(ctx, fromV3, toV3, radius, color?)` | highlight | draws an arrow; `radius` sets the arrowhead width |
| `startTimer(name?)` / `printTimer(name?)` | profile | crude millisecond stopwatch pair for basic profiling |

`addAuxiliaryEntities`/`addAuxiliaryPoint`/`addAuxiliaryLine` are `id`-scoped siblings of the
`addDebug*` family — visible while a *given* feature id's dialog is open, not just the caller's.

### properties.fs

| Function | Does | Args |
|---|---|---|
| `setProperty(ctx, {entitiesQ!, propertyType, customPropertyId?, value})` | write | sets name/appearance/material/etc. on bodies or faces; `value`'s type (Color, Material, bool, VWU, str) depends on `propertyType` |
| `getProperty(ctx, {entityQ:1, propertyType, customPropertyId?})` | read | **not** callable inside a feature's own regeneration — only from tables, editing-logic, and manipulator-change functions, or on a different context |
| `color(red, green, blue, alpha?)` | construct | 4 (or 3, alpha=1) num ∈ [0,1] → Color |
| `material(name, density)` | construct | density:VWU\<density\> → Material |

### attributes.fs

Arbitrary data attached to entities by name, readable across features — see also `qHasAttribute*`
in the Queries section above.

| Function | Does | Args |
|---|---|---|
| `setAttribute(ctx, {entitiesQ!, name?, attribute})` | write | `attribute` can be any type; setting it to `undefined` clears the attribute |
| `getAttribute(ctx, {entityQ:1, name})` | read | → the single named attribute's value, or undefined |
| `getAttributes(ctx, {entitiesQ, name?, attributePattern?})` | read | → value[], one per matched entity |
| `getAllAttributes(ctx, {entityQ:1})` | read | → map, every attribute name on that one entity |
| `removeAttributes(ctx, {entitiesQ?, attributePattern?})` | write | legacy unnamed-attribute removal only — for named attributes, `setAttribute` with `attribute:undefined` instead |

### surfaceGeometry.fs (Plane)

Just the `Plane` half of this file — `cone`/`cylinder`/`sphere`/`torus`/`BSplineSurface` are the
"fancy surfaces" this doc otherwise skips.

| Function | Does | Args |
|---|---|---|
| `plane(origin, normal, x?)` | construct | origin, normal, x:V3; `x` defaults to an arbitrary vector ⟂ `normal` |
| `plane(cSys)` | construct | cSys:CoordSystem → Plane on its XY plane |
| `coordSystem(plane)` / `planeToCSys(plane)` | convert | Plane → CoordSystem at the same origin *(aliases of each other)* |
| `yAxis(plane)` | query | → V3 *(`normal × x`)* |
| `planeToWorld(plane, pointV2)` / `worldToPlane(plane, pointV3)` | convert | 2D plane-local ↔ 3D world point |
| `planeToWorld3D(plane)` / `worldToPlane3D(plane)` | convert | → T, the same conversion as a full transform |
| `project(plane, pointV3)` / `project(plane, line)` | query | → V3, or a Line with its origin moved onto the plane |
| `intersection(plane1, plane2)` | query | → Line, or undefined if parallel/coincident |
| `intersection(plane, line)` | query | → LinePlaneIntersection{dim, intersection} *(dim -1/0/1 = none/point/line-is-in-plane)* |
| `isPointOnPlane(point, plane)` | predicate | |
| `flip(plane)` | construct | → Plane with `normal` reversed |
| `mirrorAcross(plane)` | construct | → T, a non-rigid mirroring transform |
| `transform(fromPlane, toPlane)` | construct | → T mapping one plane onto the other |
| `tolerantEquals(plane1, plane2)` / `coplanarPlanes(plane1, plane2)` | predicate | equal (same local axes too) vs. merely coplanar |

### curveGeometry.fs (Line)

Just the `Line`/`Circle`/`Ellipse` half — `BSplineCurve`/`KnotArray` construction is skipped,
same rule as everywhere else in this doc.

| Function | Does | Args |
|---|---|---|
| `line(origin, direction)` | construct | direction:V3 gets normalized for you → Line |
| `collinearLines(line1, line2)` | predicate | |
| `transform(fromLine, toLine)` | construct | → T, minimum-rotation + translation mapping one line onto the other |
| `project(line, point)` | query | point's projection onto the line → V3 |
| `rotationAround(line, angle)` | construct | → T, ccw rotation about `line` by `angle` |
| `intersection(line1, line2)` | query | → LineLineIntersection{dim, intersection} *(dim -1/0/1 = none/point/collinear)* |
| `isPointOnLine(point, line)` | predicate | |
| `circle(cSys, radius)` / `circle(center, xDirection, normal, radius)` | construct | → Circle{coordSystem, radius} |
| `ellipse(cSys, majorRadius, minorRadius)` / `ellipse(center, xDirection, normal, majorRadius, minorRadius)` | construct | → Ellipse{coordSystem, majorRadius, minorRadius} |

`tolerantEquals` is overloaded for Line, Circle, and Ellipse too — same pattern as everywhere
else: compare each field within tolerance.

### coordSystem.fs

| Function | Does | Args |
|---|---|---|
| `coordSystem(origin, xAxis, zAxis)` | construct | xAxis, zAxis:V3 need not be unit length, but must be ⟂ |
| `toWorld(cSys, pointV3)` / `toWorld(cSys)` | convert | a point measured in cSys → world, or → T doing the same to any point |
| `fromWorld(cSys, pointV3)` / `fromWorld(cSys)` | convert | world point → measured in cSys, or → T doing the same |
| `yAxis(cSys)` | query | → V3 *(`zAxis × xAxis`)* |
| `scaleNonuniformly(xScale, yScale, zScale, cSys)` | construct | → T, 3-axis scaling centered on `cSys.origin` |
| `tolerantEquals(cSys1, cSys2)` | predicate | |

Constants: `WORLD_ORIGIN`, `X_DIRECTION`, `Y_DIRECTION`, `Z_DIRECTION`, `WORLD_COORD_SYSTEM`.

### context.fs (variables & ids)

The `Context`/`Id` *functions* — the types themselves are already in the typedefs table above.

| Function | Does | Args |
|---|---|---|
| `setVariable(ctx, name, value, description?)` | write | attaches any value to the context by name, for later features; readable as `#name` in expressions |
| `getVariable(ctx, name, defaultValue?)` | read | throws if `name` isn't found, unless `defaultValue` is given |
| `getAllVariables(ctx)` | read | → map, every variable on the context |
| `newId()` / `makeId(str)` | construct | → empty Id / single-segment Id |
| `isTopLevelId(id)` | predicate | true for a top-level feature or default geometry (`id` has exactly one segment) |
| `isAtVersionOrLater(ctx, version)` | predicate | whether the active feature runs at ≥ `version` *(library-version compatibility check)* |

### manipulator.fs

All of the `*Manipulator` constructors take one `definition:map` (not `ctx`/`id`) and return a
`Manipulator`; `addManipulators` is the one function that actually attaches them to a feature.

| Function | Does | Fields |
|---|---|---|
| `addManipulators(ctx, id, manipulators)` | register | manipulators:map\<str, Manipulator\> — keys match the `newManipulators` a manipulator-change function receives |
| `linearManipulator({base, direction:VU3, offset, minValue?, maxValue?, style?, primaryParameterId?})` | construct | single draggable arrow along `direction` |
| `angularManipulator({axisOrigin, axisDirection:VU3, rotationOrigin, angle, minValue?, maxValue?, disableMinimumOffset?})` | construct | curved drag handle for an angle |
| `triadManipulator({base, offset})` | construct | axis-aligned 3D position handle |
| `fullTriadManipulator({base:CoordSystem, transform, displayEditView?, dragType?})` | construct | full 3D transform handle (rotate + translate) |
| `pointsManipulator({points[]!, index})` | construct | one selectable point out of a set |
| `togglePointsManipulator({points[]!, selectedIndices[], suppressedIndices[]})` | construct | several independently selectable points |
| `flipManipulator({base, direction:VU3, flipped, otherDirection?})` | construct | click-to-flip arrow |

### evaluate.fs

The biggest gap the census turned up — 22+ distinct `evX` functions in real use, more than any
other uncovered file. Every `evX` shares the shape `evX(context is Context, arg is map)`; `arg`
fields are shown keyword-style below, `context` dropped as usual. Several come in singular/plural
pairs (one point vs. `ptFracs[]`) where the singular form is just `plural(context, {..., ptFracs: [ptFrac]})[0]`
under the hood — cheap to call either way.

| Function | Does | Returns |
|---|---|---|
| `evOwnerSketchPlane({ entity: sketchQuery:1!, checkAllEntities? = false })` | The plane of the sketch creating `entity`. By default, checks only the first entity; otherwise, throws if any entity is not on the same plane. | Plane |
| `evPlane({ faceQ:1! })` | The Plane a planar face or mate connector represents. | Plane |
| `evPlanarEdge({ edgeQ:1! })` | The Plane a planar edge lies in. | Plane |
| `evPlanarEdges({ edgesQ! })` | The common Plane all of `edgesQ` lie in, if they share one. | Plane |
| `evAxis({ axisQ:1! })` | The axis of a line, circle, cylinder, cone, sphere, torus, mate connector, or revolved surface. | Line |
| `evLine({ edgeQ:1! })` | `edgeQ` as a Line, if it's straight. | Line |
| `evMateConnector({ mateConnectorQ:1! })` | The coordinate system of a mate connector. | CoordSystem |
| `evVertexPoint({ vertexQ:1! })` | The location of a point, or the origin of a mate connector. | V3 |
| `evBox3d({ topologyQ!, cSys?, tight? = true })` | Bounding box around `topologyQ`, in `cSys` if given. `tight = false` trades precision for speed. | Box3d |
| `evDistance({ side0, side1, extendSide0?, extendSide1?, maximum? = false, precise? = true })` | Minimum (or, if `maximum`, maximum) distance between `side0` and `side1` — each a Q\|V3\|Line\|Plane or an array of those. | DistanceResult |
| `evCollision({ toolsQ!, targetsQ!, passOwners? = false })` | Collisions between `toolsQ` and `targetsQ`. | { type: ClashType, target: Q, targetBody: Q, tool: Q, toolBody: Q }[] |
| `evRaycast({ entitiesQ!, ray: Line, closest? = true, includeIntersectionsBehind? = false })` | Where `ray` hits `entitiesQ`, closest first. | RaycastResult[] |
| `evEdgeConvexity({ edgeQ:1! })` | Convexity of `edgeQ`: CONVEX, CONCAVE, SMOOTH, or VARIABLE. | EdgeConvexityType |
| `evEdgeCurvature({ edgeQ:1!, ptFrac, precise?, faceQ?:1 })` | Frenet frame along an edge, with curvature, at one point. | EdgeCurvatureResult: { frame: CoordSystem\<Z: tangent, X: normal, Y: binormal\>, curvature } |
| `evEdgeCurvatures({ edgeQ:1!, ptFracs[]!, precise?, faceQ?:1 })` | Frenet frames along an edge, with curvature | EdgeCurvatureResult[]: { frame: CoordSystem\<Z: tangent, X: normal, Y: binormal\>, curvature }[] |
| `evEdgeTangentLine({ edgeQ:1!, ptFrac, precise?, faceQ?:1 })` | Tangent Line to `edgeQ` at one point. | Line |
| `evEdgeTangentLines({ edgeQ:1!, ptFracs[]!, precise?, faceQ?:1 })` | Tangent Lines to `edgeQ` at several points. | Line[] |
| `evFaceNormalAtEdge({ edgeQ:1!, faceQ:1!, ptFrac, precise?, faceOriented? = false })` | Surface normal of `faceQ` at a point along one of its edges. | VU3 |
| `evFaceTangentPlaneAtEdge({ edgeQ:1!, faceQ:1!, ptFrac, precise?, faceOriented? = false })` | Plane tangent to `faceQ` at a point along one of its edges. | Plane |
| `evFaceTangentPlanesAtEdge({ edgeQ:1!, faceQ:1!, ptFracs[]!, precise?, faceOriented? = false })` | Same, at several points along the edge. | Plane[] |
| `evFaceTangentPlane({ faceQ:1!, ptFracV:VF2 })` | Plane tangent to `faceQ` at one point, given in fractional face-bbox coordinates. | Plane |
| `evFaceTangentPlanes({ faceQ:1!, ptFracs:VF2[]!, returnUndefinedOutsideFace? = false })` | Same, at several points. | Plane[] |
| `evFaceCurvature({ faceQ:1!, ptFracV:VF2 })` | Principal curvatures at a point on the non-mesh face, specified by the fractional coordinates within its bounding box. | FaceCurvatureResult: { minCurvature, maxCurvature, minDirection: VU3!, maxDirection: VU3! } |
| `evFaceCurvatures({ faceQ:1!, ptFracs:VF2[]! })` | Same, at several points. | FaceCurvatureResult[] |
| `evFilletRadius({ faceQ:1! })` | Radius of a constant-radius fillet face. | VWU\<length\> |
| `evLength({ entitiesQ! })` | Total length of `entitiesQ`'s edges (own edges, or edges owned by any bodies in it). | VWU\<length\> |
| `evArea({ entitiesQ! })` | Total area of `entitiesQ`'s faces. | VWU\<area\> |
| `evVolume({ entitiesQ!, accuracy?:VolumeAccuracy })` | Total volume of `entitiesQ`'s solid bodies. | VWU\<volume\> |
| `evApproximateCentroid({ entitiesQ! })` | Approximate center of mass of `entitiesQ`, assuming uniform density. Prefer a bounding-box center for modeling purposes. | V3 |
| `evApproximateMassProperties({ entitiesQ!, density, cSys? })` | Approximate mass, centroid, inertia tensor, and volume/area/length/count for a given `density`. | MassProperties |
| `evCurveDefinition({ edgeQ:1!, returnBSplinesAsOther? = false })` | `edgeQ` as a Circle, Ellipse, Line, or BSplineCurve (an unspecified map if none of those). | Circle\|Ellipse\|Line\|BSplineCurve\|map |

Left out: B-spline/surface approximators (`evApproximateBSplineCurve/Surface`, `evSurfaceDefinition`,
`evFaceCurvatureDerivative`, `evEdgeCurvatureDerivative`, `evFacePeriodicity`, `evTessellatedLoftMatches`),
mesh/sheet-metal (`evMeshPoints`, `evSheetMetalHoleToolBodies`, `evSheetMetalFormToolBodies`, `evCornerType`),
QA/diagnostic tools (`evMaxPathDeviation`, `evPointsDeviation`, `evOffsetDetection`, `evTolerances`,
`evMaxTolerance`, `evFaults`), and `evMateConnectorCoordSystem` (`@internal`) — same exclusion rules as
the rest of this doc.

---

The two tables below came out of a second census, this time over `cadsharp/` (an expert FeatureScript
author's published features, copy-pasted out of his workspaces — spans enough of FeatureScript's
history that some of it may predate functions std has since grown) and over `flipshop/`. This isn't
a census of *his* collection for its own sake — it's what his usage patterns say about which `std`
tools are worth reaching for, plus the few places he's genuinely plugged a gap `std` still has.

### Cadsharp

Two real gaps in `std`, both from `cadsharp/alignedBoundingBox.fs` and `cadsharp/utils/queryFinder.fs`
(duplicated verbatim in `utils/functionsPascoe.fs`). Everything else he leans on turned out to already
be `std` — see the import/usage notes below.

| Function | Does | Args |
|---|---|---|
| `smallestBox(ctx, id, definition, csys, axis, maxLoopCount)` | construct | Iteratively rotates `csys` about `axis` to shrink the bounding box of `definition.entities`; call once per axis to converge on a near-minimum-volume oriented box. `std`'s `evBox3d` only *measures* a box in a `cSys` you already picked — it never searches for a better one. |
| `buildBoundingCylinder(ctx, id, definition, baseCsys, bboxSize)` | construct | Best-fit cylinder body around `definition.entities`, already oriented in world space → Q |
| `buildBoundingSphere(ctx, id, definition, boxMap, bboxSize)` | construct | Best-fit sphere body → Q |
| `buildBoundingSplinePrism(ctx, id, definition, baseCsys, bboxSize)` | construct | Best-fit smooth prism, extruded along the shortest box dimension → Q |
| `QFinderPredicate(definition, suffix)` + `QFinderFunction(ctx, definition, suffix)` + `QFinderSetDefaultsAndVisibility(ctx, definition, oldDefinition, qFinderVisibleInputs, qFinderDefaultInputs, suffix)` | UI pattern | A drop-in "search by attribute / feature / property / identity / transient ID / everything" toggle that turns one plain `Query` parameter into a rich finder UI, with caching. `std` has no equivalent — you'd otherwise hand-roll this per feature. |

**Imports.** Nearly every top-level file imports `onshape/std/common.fs` (the "get almost everything"
bundle: context, feature, query, evaluate, units, properties, string, sketch, debug, attributes,
coordSystem, curveGeometry, surfaceGeometry, box, and more — but *not* the op-wrapping modules like
`extrude.fs`/`fillet.fs`); a few of his files layer `onshape/std/geometry.fs` on top, which re-exports
`common.fs` *plus* every remaining op/feature-definition module (sheet metal, welds, the lot). In
practice: `import common.fs` gets you almost everything in this doc; add `geometry.fs` if you're
missing something op-specific. No individually-imported `std` file in his code pointed at anything
not already covered above or in the main tables.

**External documents — need your help.** A handful of import paths are opaque `documentId/workspaceId/elementId`
triples pointing at other Onshape documents, not `onshape/std`. One dominates: the same document+element
(`cbeb3dcf671e00785597bd76` / element `a75ab01def146a42f55baa7f`, pinned at a couple of different
workspace snapshots) is imported by **17 of his 20 top-level files** — almost certainly his personal
shared utility library, and the most likely place to find more of this caliber of tooling. A second
document (`c7c08274a0d273b9a5f5b47d`, a few different elements/workspaces) shows up in 6 files and
likely backs `cadsharpUrlPredicate`/`cadsharpUrlFunctionForPreExistingEditLogic` (18 and 10 calls —
reads like a standard attribution/help-link widget he drops into every feature). A third, single-segment
path (`905d9c769056ba52d974e529`, no workspace/element split) appears in 7 files including
`utils/nodeTransform.fs`. If you can share any of those three, it's worth a second pass. Two remaining
single-use paths (`12312312345abcabcabcdeff/...`) look like placeholder/test IDs, not worth chasing.

**Nothing clearly deprecated.** I went looking for signs his code predates functions `std` later added
(the kind of thing that'd now be a one-liner) and came up empty — the one candidate, a `tolerantEq`
helper, turned out to be dead commented-out code in `approximateFace.fs`, not a live call. Everything
else he reaches for either matches this doc already or is one of the two gaps above.

### flipshop/coreUtils — essential patches

Per your steer: everything here is treated as already-vetted, not code needing scrutiny — it's the
project's own `math.fs`/`string.fs`/`containers.fs`. One file name is worth fixing: `clxnGetset.f` is
missing its `s` (`.f`, not `.fs`) — harmless to Onshape, but it'll silently skip any tool that globs
for `*.fs`, including every census in this document.

| File | Fills the gap of | Catalog |
|---|---|---|
| `typeUtils.fs` | no nil/blank/zero coalescing — `std` has no `||`-style default-if-undefined | ifNil, ifBlank, ifZero, ifNilOrZero, isNil, isPresent, isEmpty, strBlank, truthy, vector2, mm, zero |
| `clxnWalking.fs` | no map/array iteration beyond raw `for` loops | forEach, mapValues, mapValues3, valuesAt, sizeof, hasKey, hasPresentKey, rebag, objectify, pick, pickDefined, boxarrPush, boxarrUnshift, arrLast, arrayIncludes |
| `clxnGetset.f` *(sic — see above)* | no path-based nested get/set in one call | getAt, setAt, deepMerge, pathForKey |
| `clxnReshape.fs` | no flatten/unflatten between nested maps and dot-path keys | dotMap, undotMap, buildNestedChoices |
| `stringUtils.fs` | no case conversion or padding in `string.fs` | downcase, upcase, downcaseChar, upcaseChar, padLeft, padRight, paddingFor, strTake, strTakeRight, strSlice, strRepeat, titleCase, hasMatch, starbanner |
| `colorUtils.fs` | `Color` is RGBA-only — no hex/tuple string conversions | toColor, toHexcolor, hexcolorToColor, toUnitcolor, unitcolorToColor, toTuplecolor, tuplestrToColor, hexpairToInt, intToHexpair, sameColor, setColor, isHexcolor |
| `metadataUtils.fs` | naming/attribute idioms layered over `properties.fs`/`attributes.fs` | getName, setName, setReadableName, getNameProp, getNameProps, getNameOfBody, getAttrs, getAllAttrs, getBestAttr, setPropAndAttribute, defaultMaybe, sanitize_varname |
| `miscUtils.fs` | currying for the function-valued params `mapArray`/`filter`/`sort` expect | curry2to0, curry2to1, curry2to2, curry3to0, curry3to1, curry3to2, noop, idsFor, parseJsonSafely |
| `debugUtils.fs` | one `debug.fs`-style helper | highlightQuery |
| `jsonVarF.fs` | a small JSON-backed list/table parameter framework | jsonVarF, keylistF, keylistEditLogic, valuesAtF, valuesAtEditLogic, sizeofF, sizeofEditLogic, splatF |

Ranked by real usage across `flipshop/`, the ones you reach for constantly: `ifNil` (47), `forEach` (20),
`vector2` (16), `ifBlank` (16), `mapValues` (15), `toColor`/`padLeft`/`curry3to0`/`curry2to0` (12 each),
`getAt` (8) — worth knowing `getAt`/`setAt`/`deepMerge`/`pathForKey` live in the misnamed `clxnGetset.f`.
