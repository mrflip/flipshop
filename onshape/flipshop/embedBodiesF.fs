FeatureScript 3029;
import(path : "onshape/std/geometry.fs", version : "3029.0");
export import(path : "19a276cbe441b4dcf19aaca1", version : "f65e96bb9819bc0fe6817767");
import(path : "c50e2363725f9cf513e36928", version : "8d87b15bcfd55baf6b749c19");
import(path : "075be6354063579d5fedb3b7", version : "6fb770875f7cb63a9103cbe2");
import(path : "e313a0b67ecb3be0415d2186", version : "b7ba666508e3e18f4b234ebd");
import(path : "58963520be3fe612d10b6d2e", version : "367c6e05048da3074180d270");
//
IconNamespace::import(path : "476f292746334f9ccc9aa08c", version : "ed7e2d8b30026af72161db6f");

// == [Embed Shape] ==

/**
 * Part feature: Given a set of tools, a set of targets, and an optional boundary plane
 *   - reduce the tools to only occupy the volume of the target bodies (if a boundary plane is given, preserve the volume above it)
 *   - carve the tool volume from the targets
 * This is useful for 3D-printing -- you may establish a part's geometry and then separate it into distinct bodies that occupy the same volume
 *
 * @param definition {{
 *      @field tools {Query}   : the tools to intersect with the targets
 *      @field targets {Query} : the targets to subtract the tool volume from
 *      @field carrierPlane {Query} : Single sketch plane entity
 *      @field oppositeDirection {boolean} : orientation of the carrier plane
 * }}
 */
annotation { "Feature Type Name": "Embed Bodies", "Icon": IconNamespace::BLOB_DATA, "Feature Type Description": "Intersect targets and tools into disjoint parts" }
export const embedBodiesF = defineFeature(function(context is Context, id is Id, definition is map)
precondition {
  annotation { "Name": "Tools", "UIHint" : [UIHint.SHOW_LABEL, UIHint.REMEMBER_PREVIOUS_VALUE], "Filter": EntityType.BODY && BodyType.SOLID, "MaxNumberOfPicks": 20 }
  definition.tools is Query;
  annotation { "Name": "Targets", "UIHint" : [UIHint.SHOW_LABEL, UIHint.REMEMBER_PREVIOUS_VALUE], "Filter": EntityType.BODY && BodyType.SOLID, "MaxNumberOfPicks": 20 }
  definition.targets is Query;
  annotation { "Name": "Preservation Plane", "Default" : true, "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE] }
  definition.hasPreservingPlane is boolean;
  if (definition.hasPreservingPlane) {
    annotation { "Name": "Sketch Plane", "Filter": QueryFilterCompound.ALLOWS_PLANE, "MaxNumberOfPicks": 20 }
    definition.preservingPlaneQ is Query;
    annotation { "Name" : "Opposite direction", "UIHint" : UIHint.OPPOSITE_DIRECTION }
    definition.oppositeDirection is boolean;
  }
}
{
  const tools   = definition.tools;
  const targets = definition.targets;
  const opts    = pickDefined(definition, ["preservingPlaneQ", "oppositeDirection"]);
  embedBodies(context, id, tools, targets, opts);
});
