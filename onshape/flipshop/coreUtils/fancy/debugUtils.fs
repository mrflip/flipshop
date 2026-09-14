FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");

/**
 * Highlights `qq` in the viewport via `addDebugEntities`, plus the edges of `qq`'s owning
 * bodies — a body is otherwise invisible through the faces of any body occluding it. A no-op
 * unless `debugMe` is true (default), so a call site can leave this in place and flip one flag
 * rather than comment the call out.
 * @param context {Context}
 * @param qq {Query}: Entities to highlight.
 * @param debugColor {DebugColor}
 * @param debugMe {boolean}: Set false to silence this call without removing it. Defaults to `true`.
 */
export function highlightQuery(context is Context, qq is Query, debugColor is DebugColor, debugMe is boolean) {
  if (! debugMe) { return; }
  var debugEdges = qOwnedByBody(qq, EntityType.EDGE); //add edges because bodies can't be seen through other bodes
  addDebugEntities(context, qUnion([qq, debugEdges]), debugColor);
}
export function highlightQuery(context is Context, qq is Query, debugColor is DebugColor) { return highlightQuery(context, qq, debugColor, true); }
