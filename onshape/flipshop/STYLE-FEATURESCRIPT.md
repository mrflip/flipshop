# FeatureScript Style Guide

This guide documents conventions for `.fs` files in this project.

---

## Indentation & Braces

- Use **2 spaces** per indent level. No tabs.
- Opening braces go **at the end of the line** that introduces the block — never on their own line.
- Always use braces for `if`/`else` bodies, even single-line ones.

```featurescript
if (x > 0) {
  doSomething();
} else {
  doOther();
}
```

---

## Maps

- Put the colon **immediately after** the closing quote: `"key":  value` (no space before `:`).
- Use **two spaces** after the colon as a minimum, then pad further to **vertically align** all values in the same map literal.
- Always put a **trailing comma** on the last element of a multi-line map or array.

```featurescript
return {
  "minH":     minH,
  "ctrH":     (minH + maxH) / 2,
  "maxH":     maxH,
  "sizeH":    maxH - minH,
};
```

- Short maps with one or two entries may be written inline: `{ "start":  p, "end":  q }`.

---

## Naming Conventions

| Construct                | Convention          | Example                  |
|--------------------------|---------------------|--------------------------|
| Functions                | `lowerCamelCase`    | `boxForBounds`           |
| Local variables          | `lowerCamelCase`    | `xStride`, `item0CtrH`   |
| Map keys (string)        | `underscore_case`   | `"n_x_items"`, `"body_bounds"` |
| Enum type names          | `UpperCamelCase`    | `ShapeType`              |
| Enum values              | `UPPER_SNAKE_CASE`  | `CIRCLE`, `ROUND_RECT`   |
| Exported feature consts  | `lowerCamelCase`    | `patternedShapes`        |
| Module-level constants   | `lowerCamelCase`    | `hugeSizeVal`            |

---

## Variables

- Use `const` for anything that is not reassigned.
- Use `var` only when the value changes (loop counters, accumulator variables).

---

## For Loops

Use `i += 1`, not `i++`:

```featurescript
for (var i = 0; i < n; i += 1) {
  ...
}
```

---

## String Concatenation

Use the `~` operator:

```featurescript
const name = "item_" ~ itemIndex;
```

---

## Type Annotations

* Use `is Type` syntax in preconditions and function signatures.
* Declare return types explicitly with `returns map`, `returns array`, `returns builtin`, etc.

```featurescript
function my_func(definition is map) returns map {
  ...
}
```

---

## Comments

Bulleted lists should use `*` at the top level and `-` (indented by two spaces per level) for sub-lists:

```
/** drawShape
 * * circle: draws a circle
 * * polygon: based on number of sids:
 *   - will error if sides is 2 or less.
 */
```

### Function preambles (`/** */` docblocks)

All functions get a `/** */` docblock immediately above the definition. Follow the Onshape stdlib style (`onshape/std/`).

**Tone and content:**
* Lead with what the function *is* or *returns*, not with "Creates/Builds/Draws" when you can say it more directly. "Bounding-box map from min/max extents" beats "Builds a bounding-box map from min/max extents."
* Omit implementation details: delegation chains, internal mechanics, unused parameters, regex patterns, etc.
* If repetitive tokens can be elided without cost to clarity, do so: for example, `@params context {Context} : Model context` improves on `@params context {Context} : The Model context structure.` by omitting "The", "structure" and the ".".

With all that said, it's easier for the next programmer to remove excess prose than to wonder at missing details; be informative, not prolix, but never mum.

**Positional parameters** — use `@param`:

```featurescript
/**
 * Arc on `sketch` from center, start, and end — adapts to `skArc`'s three-point API.
 * @param context {Context} : Model context.
 * @param id {Id} : Sketch feature id.
 * @param sketch {Sketch} : Target sketch.
 * @param angle {ValueWithUnits} : In-plane rotation angle.
 */
```

**Keyword-options maps** — name the parameter `options`, document fields as `- @field` bullets, defaults inline with `[name=default]` (using two spaces to indent):

```featurescript
/**
 * Text metrics for `text`: tight bounding boxes, width/height, aspect ratio, etc.
 * @param context {Context} : Model context.
 * @param text {string} : Text to measure.
 * @param options {map} : keyword options
 *   - @field [fontName="OpenSans-Regular.ttf"] {string} : Font filename.
 *   - @field [baselineHeight=10mm] {ValueWithUnits} : Nominal cap height.
 *   - @field [keepTools=false] {boolean} : Retain the temporary sketch body.
 */
```

**Pass-through options** — use `@see` instead of re-listing fields:

```featurescript
/**
 * Rendered width of `text` on the global XY plane.
 * @param opts {map} : Options for @see `textBounds`, plus
 *   @field poem {string} : a poem to sing while we measure
 */
```

**Definition maps** (feature `definition` or named struct-like maps) — use `@field` inside `{{ }}`:

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

**Other tags:**

| Tag                                          | Usage                      |
| -------------------------------------------- | -------------------------- |
| `@param id : @autocomplete \`id + "name1"\`` | Standard id parameter hint |
| `@optional`                                  | Field is optional          |
| `@requiredif { condition }`                  | Conditionally required     |
| `@eg \`expression\``                         | Inline example value       |
| `@seealso [functionName]`                    | Cross-reference            |
| `@internal`                                  | Not part of the public API |

**Inline notes** — use `/* */` for TODOs or import-group labels; `//` for same-line clarifications and commented-out code:

```featurescript
/* TODO: describe this in fuller detail */
/* enumerations used by opBodyDraft */
```

---

## Precondition Annotations

Each precondition field gets an `annotation` on the line immediately before it:

```featurescript
annotation { "Name":  "Item X size" }
isLength(definition.item_x_size, {(millimeter) : [tinySizeVal, 10, hugeSizeVal]} as LengthBoundSpec);
```

---

## Enums

Annotate each enum value with its display name:

```featurescript
export enum ShapeType {
  annotation { "Name": "Circle" }
  CIRCLE,
  annotation { "Name": "Round Rectangle" }
  ROUNDRECT,
  annotation { "Name": "Rectangle" }
  RECTANGLE
}
```

---

## Multi-line Function Signatures

When parameters don't fit on one line, indent continuation lines by 2 spaces:

```featurescript
function box_for_bounds(minH is ValueWithUnits, maxH is ValueWithUnits,
  minV is ValueWithUnits, maxV is ValueWithUnits,
  minD is ValueWithUnits, maxD is ValueWithUnits) returns map {
    ...
  }
```
