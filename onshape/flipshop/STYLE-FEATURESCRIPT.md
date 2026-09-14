# FeatureScript Style Guide

Conventions for `.fs` files in this project. The audience is a human or an AI coding session
working in this codebase; the sections are ordered by how expensive it is to get them wrong.

> **Read this first.** Rules that cause the most damage when ignored:
> 1. **Do not invent stdlib functions.** See [Language Limitations](#language-limitations).
> 2. **Apart from a few language requirements, follow modern TypeScript style conventions and approach.**
> 3. **Quote map keys where scope demands.** A bare key collides with in-scope variable names.
> 4. **Do not build Contraptions, do not bang rocks together**. If you run up against one of featurescript's weird limitations, do not start implementing complicated workarounds, rethinking your whole approach, or banging out ad-hoc MacGuyvers. Either carry on, documenting the edge case; stop, and ask for help; or write a function fooFacade(...) that has no essential complications past calling foo, but gives us a hook to address whatever friction you've encountered.
>

The files in `coreUtils` are good exemplars

---

## Language Limitations

These are the things that produce confidently wrong code rather than syntax errors.

**Do not invent stdlib functions.**  the onshape/README.md file has a great overview of the essential operations. See especially the coreUtils/ directory, where we've implemented most of lodash. Use those freely. If it's not in either it might be hallucination: `onshape/std` has everything up to a year ago.

**Value semantics.** Maps and arrays are values, not references. Assignment copies; there is no
aliasing. Function parameters are immutable, so mutating one starts with `var result = obj;`.

**No null, and `undefined` is indistinguishable from absence from a map.** Reading a missing key returns
`undefined`; storing `undefined` under a key of a map gives you the same reading back. There is no
key-existence test beyond comparing against `undefined` (`isPresent`).

**Random number generation is weird**; it's generally discouraged as a practice in CAD, but it can be done with a workaround not yet implemented.

**`==` is deep structural equality** on maps and arrays. This is what makes table-driven tests
viable — compare whole result maps, don't walk them field by field.

**Units are part of the type.** `2 * millimeter` is not `2`. Mixing unitless and dimensioned
values errors; go unitless by dividing (`length / millimeter`). Numeric literals in a
dimensioned expression are almost always a bug.

**Overloading by arity works.** `dotMap(obj)` delegating to `dotMap(obj, options)` is the way to
give a parameter a default — there are no default argument values. What Onshape rejects is two
*definition keys* sharing a name inside a `defineFeature` precondition block.

**Strings.** `~` concatenates. `splitByRegexp(str, pattern)` splits; note it drops trailing empty
segments. There is no string interpolation, but everything pretty-prints nicely. Don't start banging rocks together if you hit an edge case with regex: work around it or prompt for help.

**No circular imports**, and import strings are opaque. Don't try to fix that or work around it, but do be aware. Add an import if it's obvious what the path is fom a neighboring file, but don't bother with guessing or stubbing a new or unknown one, it's easy to add by me.

**Regular Expressions**: are passed as strings, so carry extra backslashes in your toolbag. The engine is the C++ ECMAScript Engine https://en.cppreference.com/cpp/regex/ecmascript -- which
  - No Lookbehind Assertions: You cannot use lookbehinds (like (?<=a)b or (?<!a)b). Only lookaheads are supported.
  - No Named Capture Groups: You cannot name your groups (like (?<name>\w+)). You must use numbers like \1 or $1.
  - No Inline Modifiers: You cannot change settings inside the text pattern (like typing (?i) to ignore capital letters). Instead, you must set these options in your C++ code when you make the regex object.
  - No Possessive Quantifiers: It lacks advanced control features like ++ or *+ to stop the engine from trying too hard to find matches
  - No Unicode character groups

**Cannot order two strings natively** using the builtin `<`, `>`, `<=`, `>=` operators. We have a more expensive `cmp`/`cmpTo` workaround, built on `strOrderBy`. It will partially order anything comparable (`true > false`, `undefined == undefined`, `"hell" < "hello"`, `[1, 2] < [1, 2, 3] == [1, 2, 3] < [1, 3, 0]`, etc), handles ValueWithUnits, and compares with tolerance.

**Sorting** Use one of these, as appropriate:
* `orderBy`    -- accepts a custom iteratee, direction, comparator. sorts array or map values, comparing recursively. Preferred in most cases
* `orderByAny` -- accepts a custom iteratee, direction; will not fail even if the data is heterogenous. Use this only if that laxity is desireable
* `sort` (*std*) -- a builtin, but requires you to make your own comparator
* `tolerantSort` (*std*) -- a builtin that (like our default) uses tolerant ordering by default.

**Errors.** `throw regenError(...)` is what Onshape renders properly in the feature tree; a
thrown bare string works but surfaces differently, so use it only where a caller is catching the
value (tests). `try silent { }` suppresses; `try { } catch (error) { }` binds the thrown value.

**Output.** `debug(context, ...)` is the only channel. No print, no console.

**Versions.** The `FeatureScript NNNN;` header and every import version must agree. Bumping means
re-checking behavior, not just the number.

---

## Indentation & Braces

- **2 spaces** per indent level. No tabs.
- Opening braces go **at the end of the line** that introduces the block — never on their own
  line. The sole exception is the `precondition` block, whose brace goes on its own line.
- Always brace `if`/`else` bodies, even single-statement ones.
- Cuddle `} else if (...) {` and `} catch (error) {`.
- Short guards stay on one line, still braced.
- Always parenthesize and space a negation.

```featurescript
if (isQueryBlank(context, thingQ)) { continue; }

if (! isPresent(thing)) {
  doSomething();
} else if (thing is map) {
  doOther();
}

const isSandwich = (hasBread && (! isHotdog));
```

---

## Maps

**Generally, quote map keys** with double quotes within functions: `{ "key":  val }`, not `{ key:  val }`. This is not
cosmetic: a bare key resolves against in-scope variables, so `{ fooName:  1 }` where `fooName` is
a local silently produces a map keyed by that local's *value*. The only unquoted form is
dot-access on the right-hand side of an expression (`ids.fooSk`, `sketches.foo`).

However, unquoted keys are easier to read: move long literals to end of file at the upper scope.. Constants are hoisted: the order of a function implementation and a constant definition don't matter. However to the however: only unquote keys at a level if all/nearly all keys are identifierish

- Colon **immediately after** the key — no space before `:`. Apply this in doc blocks too.
- **One space** after the colon as a minimum, then pad to **vertically align** the values in the
  literal. However, don't let one long key push the rest out to a silly distance; break the column instead.
- **Trailing comma** on the last element of every multi-line map or array.
- Accord a literal the **number of lines** according to length but also its didactic importance: `{ "start":  fromPos, "end":  ontoPos }`, and so might a long bag of cookie-cutter options; in a wrapper around a certain other function, one might well spell out each key on its own line so it can be compared easily to the docs.
**Subscripting.** Use brackets with double-quotes `foo["this.that"]` for keys not reachable with a dot.

```featurescript
// the similar args are grouped; we use whitespace communicatively; we're forced to explicitly add keys, FS does not allow JS-style shorthand:
return {
  "minH":   minH,
  "maxH":   maxH,
  "ctrH":  (maxH + minH) / 2,
  "sizeH":  maxH - minH,
};
```

**Key spelling.**

* Keys in bags of data are `underscore_case`. Keys consumed by std APIs keep
std's own spelling — `sketchPlane`, `keepTools`, `defaultValue` — because those are the names the
API reads. Never translate a std key into our house casing.
* Keep variable names and keys consistent. If we choose to add `fooMateQ` to a feature definition, for as long as it's "foo-ness" is most salient, you'll access it as definition.fooMateQ, dereference it (if needed) as const fooMateQ, and (if needed) evaluate it to `fooMate`. At whatever call boundary its contextual purpose becomes most salient, the name should reflect that: `drillHappyHoles(..., holeMateQ is Query, ...)`

---

## Strings

`'` by default (no shift key). `"` for map keys and subscripts that must be quoted (due to map-key-scoping rules or special characters). In cases where there's a parallel construction (eg a map literal, tuples) with multiple strings, putting the second slot in single quotes means the IDE can help align them:

```featurescript
// Our IDE can vertically align the second slot on `  "` and the third on ` '`
const PadTestCases = [
  [["", 0],             "",             'empty string, length: 0, default padding: returns input'],
  [["", 0, " "],        "",             'empty string, length 0, space padding explicit: returns input'],
  [["hello world", 0],  "hello world",  'string, length: 0, default padding: returns input'],
];
```

```featurescript
const title = 'Airplane!';
const entry = { "title":  'Airplane!' };
const val   = obj["this.that"];
```

Bare keys would be preferable but Onshape's scoping rules make them challenging — see Maps above.

---

## Naming Conventions

| Construct               | Convention         | Example                        |
|-------------------------|--------------------|--------------------------------|
| Functions               | `lowerCamelCase`   | `boxForBounds`                 |
| Local variables         | `lowerCamelCase`   | `xStride`, `item0CtrH`         |
| Map keys (ours)         | `underscore_case`  | `"n_x_items"`, `"body_bounds"` |
| Map keys (std APIs)     | as std spells them | `"sketchPlane"`, `"keepTools"` |
| Enum type names         | `UpperCamelCase`   | `ShapeType`                    |
| Enum values             | `UPPER_SNAKE_CASE` | `CIRCLE`, `ROUND_RECT`         |
| Exported features       | `lowerCamelCaseFS` | `patternedShapesFS`            |
| Module-level constants  | `UpperCamelCase`   | `HexcodeRE`                    |

**No single-letter names.** Use `row`, `col`, `thing`. The only sanctioned short names are `ii`,
`jj` for traditional index iterators.

**Never `name`, `value`, or `query` as variable names.** Onshape's scoping rules interact badly
with them when they are also map keys — same hazard as bare map keys. For `name`, there's almost always some salient prefix to supply `fooName`.

If genericity is exactly the salient feature:
* `val` for an any-typed value. vv/kk are secondary choices in a lambda when key or val is in-scope
* `key` for a generic map key, `keylist` for a list of keys. Use `propnames` and `props` when metaprogramming a structured object rather than a bag
* `seq` for a generic array index
* `bag` for a generic key-value map (Record<string, any>); `fooBag` for a key-value map of foo's
* `arr`, for a generic array; `foos` or `foolist` as taste informs
* `ii`/`jj`/`kk` for array iterators, or for element indexes in a grid pattern (`row` and `col`, or `horiz` and `vert` may be more evocative but only in their precise meaning). One may use `seq`, `fooSeq`, `fooIdx`, `fooIdx0 / fooIdx1 / fooIdx2`, but ii/jj/kk are never foo'ed.
* `xx` / `yy` / `zz` for **position** values within the contextually natural coordinate system. If it's important to
* `wd` / `dp` / `ht` / `diam` (`fooWd`, `fooDiam`) for the width (x-ish), depth (y-ish), height (z-ish), or diameter dimensions of an object.
  - `od`, `fooOD` for an outer or circumscribed or bounding-circle diameter; `fooID` for an inscribed or inner diameter of a shell/torus/disc.
  - `len` for generically a width/height/depth or other length dimension of an object. Use `dist` for a scalar distance, `span` for a a vector distance
  - `thk` for thickness, which is often more evocative: I'd describe a chamfer's axial direction with `thk` but a cylindrical prism's with `ht`.
  - `offset` or `shift` for the delta from some reference; `gap` or `sep` for the exterior separation of objects; `spc` for spacing interval along a regular tempo. Define the positioning of opposite edges as a `margin` and `shift` rather than start/end padding.
  - `fooCount` for the reported quantity. One may use `nFoos` for an input quantity.
* `qy` for a truly unsalient query, but `fooQ` if there's any context to attach to it
* `mate`
* `from` and `onto` for initial and final state, `beg` and `end` for a directed boundary, `ante` and `post` for before/after, `curr` and `prev` for a chain
* `loc` would be a variable describing a location; used locally it can have any convenient type, but it's often useful to follow the ducktyped-intent-map pattern described elsewhere. Use `pt` or `vertex` or `center` exactly when you want to specifically note that a location is a point, or a single-point junction, or a unique unambiguous and dominant central location.

**Agnostic parameters** — where the function genuinely does not care what it got — are named
`val`, `str`, `obj`, `num`, `err`.

**No space between function name and args:** `paintRange(range)`.

### Domain vocabulary

| Name                  | Means                                                                       |
|-----------------------|-----------------------------------------------------------------------------|
| `thing`               | The thing itself, where typing it body/solid/query isn't essential           |
| `fooQ`                | A Query-typed value; `qq` is the generic query iterator                      |
| `qy`                  | A query where genericity matters more than specificity                       |
| `pos`                 | Position coordinates of a point, against a tacit contextual origin           |
| `span`                | A from-here-onto-there vector                                                |
| `dir`                 | A direction, unit or not                                                     |
| `vec` / `coords`      | Only once reified: `vec` for a Vector, `coords` for a map                    |
| `origin`              | Only an actual coordinate system origin                                      |
| `center`              | Only an unambiguously special central position                               |
| `pair`                | Only where cardinality could never be other than two — `[from, onto]` does   |
| `mate`                | Never `connector`. Same `fooQ` pattern applies: `mateQ`                      |
| `transform(s)`        | The data structure `opTransform`'s transform arg accepts                     |

Never `pt` for an object. A circle radius is a `span` or a `dir` depending on use; the point where
it meets the circle might be an `edgePos` but would not be a `radius`.

In FeatureScript, only specific items have strong data types (like Query). Others are general-purpose types with special internal structures, or they are just terms used to describe parts of a 3D model.
## Which items have strong types?

* Strong Types: Query, Vector, Line, Plane, CoordSystem, and Ray are built-in, strong data types. You can check them using the is keyword (for example: if (myVar is Vector)).
* Not Strong Types: Items like Body, Solid, Face, and Edge are not data types in FeatureScript. Instead, they are just geometric objects inside your 3D model. In your code, you always refer to them using a Query.
* Basic Types: Distance is not a special type; it is just a regular number (ValueWithUnits) that holds a length unit like inches or millimeters. Point is just a Vector that holds three numbers [x, y, z].

------------------------------
## Recap Table with typename
Here is the updated guide. It includes the exact FeatureScript typename or internal structure used to handle each item in code.

| Concept           | What it is                               | Role in FeatureScript                                | typename / Data Type                                |
| ----------------- | ---------------------------------------- | ---------------------------------------------------- | --------------------------------------------------- |
| Query             | A selection rule or criteria to dynamically find geometry | Used to select and pass around geometry references throughout a feature. | Query                                               |
| Entity            | A specific geometric component (can be many of the below) | Selected and operated on via a Query; never held as a bare value. | Handled via Query                                   |
| Body              | An independent geometric container                 | Selected and grouped via a Query, same as any other entity. | Handled via Query                                   |
| Solid             | A 3D volumetric single continuous mass         | Selected via a Query filtered to `BodyType.SOLID`. | Handled via Query (with BodyType.SOLID)             |
| Part              | A user-facing 3D solid body.                      | Selected via a Query; the top-level entity shown in the Parts list. | Handled via Query                                   |
| Point             | A 0D coordinate location.                | A single spot in 3D space [x, y, z].                 | Vector (array of 3 numbers)                         |
| Vector            | Coordinate tuple; specify if it's a `dir`ection, `offset`, `pos`ition, `gap`, repetition `spc`ing, etc           | Vector                                              |
| Distance          | Length measurement; specify instead eg `thk`ness, `offset`, `size`, etc              | ValueWithUnits (a number with units)                |
| Line              | A mathematical line, perhaps an axis or boundary           | Line (map with origin and direction)                |
| Plane             | A mathematical infinite flat surface.    | Used to position sketches or features.               | Plane (map with origin, normal, and x-direction)    |
| Sketch Plane      | A plane specifically used for sketching. | The flat canvas where 2D geometry is drawn.          | Plane (same type as a regular plane)                |
| Coordinate System | A full 3D reference frame.               | Defines a local local origin and 3 directions.       | CoordSystem (map with origin and 3 coordinate axes) |
| Surface           | A 2D sheet body with zero thickness.     | Used for open boundaries or quilts.                  | Handled via Query (with BodyType.SHEET)             |
| Ray               | A starting point and a direction.        | Used for mathematical calculations like ray-casting. | Ray (map with origin and direction)                 |
| Face              | A bounded region of a body.              | A specific selectable flat or curved wall.           | Handled via Query (with EntityType.FACE)            |


---

## Ducktyping and Late Resolution

Where there are several ways to say the same thing, use a generic term carrying a ducktyped map
until the concrete representation is genuinely needed. Only then tag a variable with its type,
and extract via an idempotent function.

- `foo` is a field; `foo'ed` is a map carrying that field. A `thingLoc` is thing'ed and loc'ed;
  a mated map is mated; a Qed map is Qed.
- For a parameter that is "something I can unambiguously resolve to what I need", use a struct
  with the best generic non-reified name: `Loc` for location information,
  `MultiPlacement { thing, fromLoc, ontoLocs }` for where-to-place-a-thing instructions.

**Idempotent extractors resolve the ducks:**

- `fooedFor(x)` returns the input with `foo` filled in — *plus every intermediate it had to
  compute on the way*, so downstream callers don't recompute. Returns its input unchanged when
  `foo` is already set.
- `fooFor(x)` returns just `foo`.
- `fooForBar(bar)` is the primitive: one concrete representation to another.
- `bestFooedFor(x)` dispatches on which duck it received and complains if it isn't one.

```featurescript
bestThingQedForMated(thingLoc)  // fills in thing.query, unchanged if already set
bestThingQedFor(thingLoc)       // dispatches on the duck, complains if it isn't one
bestThingQFor(thingLoc)         // just the query
```

**Late resolution is the point.** Don't pre-convert before calling. Pass whatever representation
is on hand and let the callee run the extractor itself. A call site that converts is a call site
that will convert again next time.

The colour library is the worked example: a Color duck of
`{ rgb, hexcolor, hsv, yuv, srgb, rgb255tuple }` with primitives like `srgbForRGB`,
`rgbForHexcode`, where `yuvedFor(color)` may need srgb, gets it from rgb, and returns
`{ yuv, srgb, rgb }`.

---

## Variables

- `const` by default.
- `var` only where the value is actually reassigned (loop counters, accumulators).
- Parameters are immutable; copy into a `var` before mutating.

---

## For Loops

Use `ii += 1`, not `ii++`:

```featurescript
for (var ii = 0; ii < size(segments); ii += 1) {
  ...
}
```

Iterate maps by key rather than by entry, which keeps `entry.value` out of scope:

```featurescript
for (var keyName in keys(obj)) {
  const val = obj[keyName];
  ...
}
```

---

## Anonymous / Lambda / Dagger Functions

Params are always parenthesized, even a single one.
Use dagger functions for inline-lambdas.
Moderately prefer dagger functions for very brief function variables, `function` functions if not.
Embrace a whole const dagger function in parens; that reminds us to use a `;`.
You cannot refer to a function by name unless it was declared variable style (`const foo = (() => ...);`) -- BUT you also can't overload a constant. FML.
Our solution in coreUtils is to export a bag of functions.

```featurescript


mapArray(rows, (row) => row.width);

const fieldOf = ((val) => val[fieldName]);

const downcaseChar = (function(char is string) returns string {
  const lochar = UpperToLowercase[char];
  if (lochar == undefined) { return char; }
  return lochar;
});

```

---

## File Order

1. The big does-the-thing function at the top.
2. The feature function below most things.
3. Purely utility functions at the end.

---

## Type Annotations

Use `is Type` in preconditions and signatures. Declare return types explicitly with `returns map`,
`returns array`, `returns builtin`.

```featurescript
function boxForBounds(definition is map) returns map {
  ...
}
```

Leave a ducktyped or genuinely agnostic parameter unannotated rather than lying about its type.

---

## Multi-line Function Signatures

Continuation lines indent by 2 spaces; the body indents from the signature's first line.

```featurescript
function boxForBounds(minH is ValueWithUnits, maxH is ValueWithUnits,
  minV is ValueWithUnits, maxV is ValueWithUnits,
  minD is ValueWithUnits, maxD is ValueWithUnits) returns map {
  ...
}
```

---

## Comments

Bulleted lists use `*` at the top level and `-` (indented two spaces per level) for sub-lists.

```
/** drawShape
 * * circle: draws a circle
 * * polygon: based on number of sides:
 *   - will error if sides is 2 or less.
 */
```

Use `/** */` for doc blocks,  `/* */` for TODOs and import-group labels; `//` for same-line clarifications and commented-out
code.

### Function preambles (`/** */` docblocks)

All functions get a docblock immediately above the definition, in Onshape stdlib style
(`onshape/std/`).

**Tone and content:**

* Lead with what the function *is* or *returns*, not "Creates/Builds/Draws" when you can say it
  more directly. "Bounding-box map from min/max extents" beats "Builds a bounding-box map from
  min/max extents."
* Omit implementation details: delegation chains, internal mechanics, unused parameters, regex
  patterns.
* Elide repetitive tokens where clarity doesn't suffer: `@param context {Context} : Model context`
  improves on `@param context {Context} : The Model context structure.`

With that said, it is easier for the next programmer to remove excess prose than to wonder at missing details; be
informative, not prolix, but never mum.

**Positional parameters** — `@param`:

```featurescript
/**
 * Arc on `sketch` from center, start, and end — adapts to `skArc`'s three-point API.
 * @param context {Context}:        Model context.
 * @param id      {Id}:             Sketch feature id.
 * @param sketch  {Sketch}:         Target sketch.
 * @param angle   {ValueWithUnits}: In-plane rotation angle.
 */
```

**Keyword-options maps** — name the parameter `options`, document fields as `- @field` bullets,
defaults inline as `[name=default]`:

```featurescript
/**
 * Text metrics for `text`:  tight bounding boxes, width/height, aspect ratio, etc.
 * @param context {Context}: Model context.
 * @param text {string}:     Text to measure.
 * @param options {map}:     keyword options
 *   - @field [fontName="OpenSans-Regular.ttf"] {string}: Font filename.
 *   - @field [baselineHeight=10mm] {ValueWithUnits}:     Nominal cap height.
 *   - @field [keepTools=false] {boolean}:                Retain the temporary sketch body.
 */
```

**Pass-through options** — `@see` instead of re-listing fields:

```featurescript
/**
 * Rendered width of `text` on the global XY plane.
 * @param opts {map} : Options for @see `textBounds`, plus
 *   @field poem {string} : a poem to sing while we measure
 */
```

**Definition maps** — `@field` inside `{{ }}`:

```featurescript
/**
 * Feature: rectangular grid of extruded shapes on a reference plane.
 * @param definition {{
 *      @field referencePlane {Query} : Sketch plane.
 *      @field shape_type {ShapeType} : Shape per grid cell.
 *      @field [corner_radius=0] {ValueWithUnits} : Fillet radius; applies only when `shape_type` is `ROUNDRECT`.
 * }}
 */
```

**Ducktyped parameters** — `@param thingLoc {map}` tells the reader nothing. List the accepted
ducks and name the extractor that resolves them:

```featurescript
/**
 * Placement transforms for `thing` onto each of `ontoLocs`.
 * @param thingLoc {map} : thing'ed and loc'ed; any duck `bestThingQedFor` accepts —
 *   - @field thing {map} : Qed, bodied, or parted.
 *   - @field fromLoc {map} : mated, or pos'ed and dir'ed.
 */
```

**Throwing** — there is no `@throws` tag. Put the conditions in the closing prose of the
description, naming the thrown condition in the same words the error message uses.

**Other tags:**

| Tag                                          | Usage                      |
|----------------------------------------------|----------------------------|
| `@param id : @autocomplete \`id + "name1"\`` | Standard id parameter hint |
| `@optional`                                  | Field is optional          |
| `@requiredif { condition }`                  | Conditionally required     |
| `@eg \`expression\``                         | Inline example value       |
| `@seealso [functionName]`                    | Cross-reference            |
| `@internal`                                  | Not part of the public API |

---

## Precondition Annotations

Each precondition field gets an `annotation` on the line immediately before it. The `precondition`
block is the one place an opening brace goes on its own line.

```featurescript
annotation { "Name":  "Item X size" }
isLength(definition.item_x_size, {(millimeter) : [tinySizeVal, 10, hugeSizeVal]} as LengthBoundSpec);
```

`UIHint` values always go in an array, even a single one:

```featurescript
annotation { "Name":  "Reference plane", "UIHint":  [UIHint.INITIAL_FOCUS] }
```

Two definition keys may not share a name inside one precondition block.

---

## Enums

Annotate each value with its display name:

```featurescript
export enum ShapeType {
  annotation { "Name":  "Circle" }
  CIRCLE,
  annotation { "Name":  "Round Rectangle" }
  ROUNDRECT,
  annotation { "Name":  "Rectangle" }
  RECTANGLE
}
```

---

## Feature Parts with Multiple Sketches or Ids

Any feature or helper creating more than one sketch or operation id uses a unified `ids` map and a
`sketches` map, following `socketCellCutter` as the reference.

**`ids` map** — all id strings in one `const`, `fooSk` keys for sketches, plain camel-case
otherwise:

```featurescript
const ids = { bboxesSk: id + "bboxesSk", shapesSk: id + "shapesSk", extrudedCutout: id + "extrudedCutout" };
```

**`sketches` map** — base name to `Sketch` object, aligned:

```featurescript
const sketches = {
  bboxes:  newSketchOnPlane(context, ids.bboxesSk, { "sketchPlane":  socketParams.basePlane }),
  shapes:  newSketchOnPlane(context, ids.shapesSk, { "sketchPlane":  socketParams.basePlane }),
};
```

**Queries** — declare a sketch's query immediately after its `skSolve`, not at the call site that
first consumes it:

```featurescript
skSolve(sketches.shapes);
const shapesSkFacesQ = qCreatedBy(ids.shapesSk, EntityType.FACE);
```

Query variable names follow `<baseName>Sk<EntityType>Q` — `shapesSkFacesQ`, `cutoutSkBodiesQ`.

## Visual Weight == Didactic Weight

Make the visual tempo of the code match what you're trying to communicate:

```
  // Compare: passing check, nothing to see here, move along
  if (isNil(opts.something)) { throw 'Please supply the "something" options; received keys were ' ~ keys(opts); }
  // Compare: pay attention to what is happening here
  if (isNil(opts.nuclearLaunchAuthorizationCode)) {
    throw "The opts.nuclearLaunchAuthorizationCode is absent, please audit the call chain. Received keys were ' ~ keys(opts);
  }

  // a default-args overload is another good example of nothing interesting to see
  happyFunc(arg1, arg2, opts is map) {
    // ...
  }
  happyFunc(arg1, arg2) { return happyFunc(arg1, arg2, {}) }

  // using whitespace to make the parallel construction clear, and easy to browse -- worth the cost of a scrollbar
  const PadTestCases = [
    [["", 0],                       "",                    'empty string, length: 0, default padding: returns input'],
    [["", 0, " "],                  "",                    'empty string, length 0, space padding explicit: returns input'],
    [["hello world", 0],            "hello world",         'string, length: 0, default padding: returns input'],
    [["hello world", 12],           " hello world",        'string, length one more than its own, default padding: adds one space'],
    [["hello world", 12, "!"],      "!hello world",        'string, length one more than its own, with one pad character: adds one character of padding'],
    [["hello world", 12, "!!!"],    "!hello world",        'string, length one more than its own, with three pad characters: adds one character of padding'],
  ]
```
