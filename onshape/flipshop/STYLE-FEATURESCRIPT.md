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

- Use `is Type` syntax in preconditions and function signatures.
- Declare return types explicitly with `returns map`, `returns array`, `returns builtin`, etc.

```featurescript
function my_func(definition is map) returns map {
  ...
}
```

---

## Comments

### Function preambles (`/** */` docblocks)

All functions get a JSDoc-style `/** */` block comment immediately above the definition, following the Onshape stdlib convention (`onshape/std/`):

```featurescript
/**
 * Brief one-line description of what the function does.
 * Additional context if needed.
 * @param id : @autocomplete `id + "operationName1"`
 * @param definition {{
 *      @field fieldName {Type} : Description of the field.
 *      @field optionalField {boolean} : @optional Description. Default is `false`.
 *      @field conditionalField {ValueWithUnits} : @requiredif {`someFlag` is `true`}
 *          @eg `0.2 * inch`
 * }}
 */
```

**Tag reference:**

| Tag | Usage |
|-----|-------|
| `@param id : @autocomplete \`id + "name1"\`` | Standard id parameter hint |
| `@param definition {{ ... }}` | Double-braces wraps the field list |
| `@field name {Type} : desc` | One field per line, indented 6 spaces inside definition |
| `@optional` | Field is optional (append to field description) |
| `@requiredif { condition }` | Conditionally required (note: stdlib uses both `@requiredif` and `@requiredIf` inconsistently — prefer lowercase) |
| `@eg \`expression\`` | Inline example value |
| `@ex \`expression\`` | Longer example (multiline context) |
| `@seealso [functionName]` | Cross-reference to another function |
| `@internal` | Marks the function as not part of the public API |

Use `/* brief note */` inline block comments for TODOs or import-group labels:

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
