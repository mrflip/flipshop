
//_______________________________________________________________________________________________________________________________________________
//
// This FeatureScript is owned by Michael Pascoe and is distributed by CADSharp LLC.
// You may not redistribute it for commercial purposes without the permission of said owner and CADSharp LLC. Copyright (c) 2023 Michael Pascoe.
//_______________________________________________________________________________________________________________________________________________


FeatureScript 2296;
import(path : "onshape/std/common.fs", version : "2296.0");


// CADSharp
export import(path : "cbeb3dcf671e00785597bd76/409d65a3744fe434f32bdffc/a75ab01def146a42f55baa7f", version : "381046010d5aea697e433948");

IconNamespace::import(path : "4004a1f7980bc0268da6f3e3", version : "52a33f12f70fc943e4eee7a3");

annotation {
        "Feature Type Name" : "Offset Adjacent Faces",
        "Icon" : IconNamespace::BLOB_DATA,
        "Feature Type Description" : "<br> <b>Summary</b> <br> Offsets the adjacent faces of a selection.",
        "Description Image" : cadsharpLogo::BLOB_DATA,
        "Editing Logic Function" : "cadsharpUrlEditLogic" }

export const offsetAdjacentFaces = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Faces", "Filter" : EntityType.FACE }
        definition.Faces is Query;

        annotation { "Name" : "Hint edges", "Filter" : EntityType.EDGE }
        definition.hintEdges is Query;


        annotation { "Name" : "Offset" }
        isLength(definition.Offset, { (inch) : [0, 0.125, 1e5] } as LengthBoundSpec);

        annotation { "Name" : "Opposite direction", "UIHint" : UIHint.OPPOSITE_DIRECTION }
        definition.oppositeDirection is boolean;

        cadsharpUrlPredicate(definition);
    }
    {
        var adjacentFaces = qAdjacent(definition.Faces, AdjacencyType.EDGE, EntityType.FACE);

        if (!isQueryEmpty(context, definition.hintEdges))
        {

            const loopedEdges = qLoopBoundedFaces(qUnion([definition.Faces, definition.hintEdges]));

            debug(context, loopedEdges, DebugColor.RED);
            const hintFaces = qAdjacent(loopedEdges, AdjacencyType.EDGE, EntityType.FACE);
            debug(context, hintFaces, DebugColor.RED);
            adjacentFaces = qIntersection([adjacentFaces, hintFaces]);
        }

        debug(context, adjacentFaces, DebugColor.CYAN);

        opOffsetFace(context, id + "offsetFace1", {
                    "moveFaces" : adjacentFaces,
                    "offsetDistance" : (definition.oppositeDirection ? 1 : -1) * definition.Offset

                });
    }, {});
