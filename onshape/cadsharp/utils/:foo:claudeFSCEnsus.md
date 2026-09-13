wow... that's amazing. Can you put a markdown version of that in flipshop/onshape/README.md ? Leave the few existing lines of that file at the top.

Would you next use your good taste and memory to add other functions to the document (keep the README.md as its canonical home, I'll have you update the artifact at the end).

Add a section called Utilities -- I know string.fs, container.fs, math.fs have many; make a different type of table that just catalogs them: `| Math | tolerantEquals, ... abs, ..., isPositiveInteger, PI, TOLERANCE |`

Lastly for this step, make a section "Queries" (don't do Evaluators yet) using just the contents of `onshape/std/query.fs`. Do not respect the variable names they use for positional args: adopt the following conventions.

```
| qEdgeVertex(edgesQ, atStart?) | Start (or end) vertices of edgesQ |
| qConstructionFilter(qy, ConstructionObject.x) |  Construction (or non-construction) entities |
| qBodyType(qy, [BodyType]) | Entities matching the [BodyTypes] |
| qOwnedByBody(qy, bodyQ) | Entities owned by the specified bod(ies) |
| qParallelPlanes(qy, refPlane, includeAntiparallel?) | Entities strictly parallel to refPlane, optionally including antiparallel ones |
| qTangentConnectedFaces(seedQ, angleTolerance) | Entities connected to seedQ via tangent edges, flood-filling across any number of tangent edges. A tangent edge is an edge joining two faces such that the surface direction is continuous across the edge, up to the given `angleTolerance`, at every point along the full length of the edge. |
```

compare with the original, below this line -- I shorten docstrings following my pattern where it's simple, but in the case of qTangentConnectedFaces that actually has interesting things to say, keep it verbatim; I'm careful about singular/plural nouns; every totally-generic query is called 'qy', otherwise it's signified; transparent pseudocode boolVar?, [arrVar], EnumType.X, [EnumType] [number], etc

```

/**
 * A query for the start or end vertices of edges.
 */
export function qEdgeVertex(edgeQuery is Query, atStart is boolean)

/**
 * A query for all construction entities or all non-construction entities in `queryToFilter`.
 * @seealso [ConstructionObject]
 */
export function qConstructionFilter(queryToFilter is Query, constructionFilter is ConstructionObject) returns Query

/**
 * A query for all of the entities which match a `queryToFilter`, and belong to the
 * specified body or bodies.
 */
export function qOwnedByBody(queryToFilter is Query, body is Query) returns Query

/**
 * A query for all planar face entities that are parallel to the `referencePlane`.
 * @param referencePlane : The plane to reference when checking for parallelism.
 * @param allowAntiparallel : Whether to also return entities that are antiparallel.
 */
export function qParallelPlanes(queryToFilter is Query, referencePlane is Plane, allowAntiparallel is boolean) returns Query

/**
 * A query for a set of faces connected to `seed` via tangent edges, flood-filling
 * across any number of tangent edges.
 *
 * A tangent edge is an edge joining two faces such that the surface direction
 * is continuous across the edge, up to the given `angleTolerance`, at every
 * point along the full length of the edge.
 */
export function qTangentConnectedFaces(seed is Query, angleTolerance is ValueWithUnits) returns Query
```

Do what you can to make the cardinality of queries clear: I can't tell if there are cases where eg qEdgeVertex expects its query to resolve to a single edge, or if everything is written to be plural-safe. If it's the former, write `edgeQ:1!`, not `edgesQ`.


## Evaluators

/**
 * Return the convexity type of the given edge,
 * `CONVEX`, `CONCAVE`, `SMOOTH`, or `VARIABLE`.
 * If the edge is part of a body with inside and outside
 * convex and concave have the obvious meanings.
 * @param context
 * @param arg {{
 *      @field edge{Query}
 * }}
 * @throws {GBTErrorStringEnum.TOO_MANY_ENTITIES_SELECTED} : The query evaluates to more than one entity
 * @throws {GBTErrorStringEnum.BAD_GEOMETRY} : The query does not evaluate to a single edge.
 */
export function evEdgeConvexity(context is Context, arg is map) returns EdgeConvexityType

Do evaluate.fs next. As before, here's an example of the table format:

```
| evOwnerSketchPlane({ entity: sketchQuery:1!, checkAllEntities? = false }) | The plane of the sketch creating `entity`. By default, checks only the first entity; otherwise, throws if any entity is not on the same plane. | Plane |
| evEdgeCurvatures({ edgeQ:1!, ptFracs[]!, precise?, faceQ:1! }) | Frenet frames along an edge, with curvature | EdgeCurvatureResult[]: { frame: CoordSystem<Z: tangent, X: normal, Y: binormal>, curvature }[] |
| evFaceCurvature({ faceQ:1!, ptFracs:VF2[]! }) | Principal curvatures at a point on the non-mesh face, specified by the fractional coordinates within its bounding box | FaceCurvatureResult: { minCurvature, maxCurvature, minDirection: VU3!, maxDirection: VU3! }
```

compare with the original, below this line -- drop the `context` arg or naming the keyword arg; indicate a default value `foo = defaultVal`. You'll have to often reference the original return type -- don't try to summarize it completely if it's complex.

```

/**
 * Return the plane of the sketch that created the given entity.
 * @param context
 * @param arg {{
 *      @field entity{Query} : The sketch entity. May be a vertex, edge, face, or body.
 *      @field checkAllEntities{boolean} : If true, the function will only return a plane if all entities queried under 'entity' share coplanar sketch planes.
 *          Otherwise, the plane will only be evaluated for the first entity in the query. Default is false. @optional
 * }}
 * @throws {GBTErrorStringEnum.CANNOT_RESOLVE_PLANE} : Entities were not created by a sketch or do not share the same sketch plane.
 */
export function evOwnerSketchPlane(context is Context, arg is map) returns Plane
/**
 * Return Frenet frames along an edge, with curvature.
 * If the curve has zero curvature at an evaluated point then the returned normal and binormal are arbitrary
 * and only the tangent is significant.
 *
 * @param arg {{
 *      @field edge {Query}: The curve to use @eg `qNthElement(qEverything(EntityType.EDGE), 1)`
 *      @field parameters {array}:
 *             An array of numbers in the range 0..1 indicating points along
 *             the curve to evaluate frames at.
 *      @field arcLengthParameterization {boolean} :
 *             If true (default), the parameter measures distance
 *             along the edge, so `0.5` is the midpoint.
 *             If false, use an arbitrary but faster-to-evaluate parameterization.
 *             The parameterization is identical to that used by [evEdgeTangentLines].
 *             Results obtained with arcLengthParameterization will have lower accuracy due to approximation.
 *          @optional
 *      @field face {Query} :
 *             If present, the edge orientation used is such that walking along the edge
 *             with "up" being the `face` normal will keep `face` to the left.
 *             Must be adjacent to `edge`.
 *          @optional
 * }}
 * @returns {array} : An array of [EdgeCurvatureResult]s.
 * @throws {GBTErrorStringEnum.NO_TANGENT_LINE} : A frame could not be calculated for the specified input.
 */
export function evEdgeCurvatures(context is Context, arg is map) returns array

/**
 * Given a face, calculate and return principal curvatures at a point on that face,
 * specified by its parameter-space coordinates.
 *
 * @example ```
 * ...
 * ```
 *
 * @param context {Context}
 * @param arg {{
 *      @field face {Query}: The face on which to evaluate the curvature. The face cannot be a mesh.
 *          @eg `qNthElement(qEverything(EntityType.FACE), 1)`
 *      @field parameter {Vector}: a 2d unitless parameter-space vector specifying the location on the face.
 *          The coordinates are relative to the parameter-space bounding box of the face.
 *          @eg `vector(0.5, 0.5)`
 * }}
 */
export function evFaceCurvature(context is Context, arg is map) returns FaceCurvatureResult
// (FaceCurvatureResult:)
 // *      @field minCurvature {ValueWithUnits} : The smaller of the two principal curvatures (inverse length units).
 // *      @field maxCurvature {ValueWithUnits} : The larger of the two principal curvatures (inverse length units).
 // *      @field minDirection {Vector} : A 3D unit vector corresponding to `minCurvature`.
 // *      @field maxDirection {Vector} : A 3D unit vector corresponding to `maxCurvature`.

 ```

CADSharp Utils/
functionsPascoe.fs          https://cad.onshape.com/documents/c7c08274a0d273b9a5f5b47d/v/428044aea156e30f43a38edf/e/0abb9be049d15f1839c40841
queryFinder.fs              https://cad.onshape.com/documents/c7c08274a0d273b9a5f5b47d/v/428044aea156e30f43a38edf/e/95f7817ff5864d9d171be609
