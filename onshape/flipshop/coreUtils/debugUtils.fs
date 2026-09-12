FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");

export function highlightQuery(context is Context, qq is Query, debugColor is DebugColor, debugMe is boolean) {
  if (! debugMe) { return; }
  var debugEdges = qOwnedByBody(qq, EntityType.EDGE); //add edges because bodies can't be seen through other bodes
  addDebugEntities(context, qUnion([qq, debugEdges]), debugColor);
}
export function highlightQuery(context is Context, qq is Query, debugColor is DebugColor) { return highlightQuery(context, qq, debugColor, true); }
