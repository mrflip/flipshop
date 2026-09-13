FeatureScript 1948;
import(path : "onshape/std/common.fs", version : "1948.0");

// annotation { "Feature Type Name" : "My Feature" }
// export const myFeature = defineFeature(function(context is Context, id is Id, definition is map)
//     precondition
//     {
//         annotation { "Name" : "Boolean enum", "UIHint" : UIHint.HORIZONTAL_ENUM }
//         definition.booleanEnum is BooleonScopeEnumPascoe;

//     }
//     {
//         // Define the function's action
//     });


export predicate BooleanEnumPredicatePascoe(definition)
{
    annotation { "Name" : "Boolean enum", "UIHint" : UIHint.HORIZONTAL_ENUM }
    definition.booleanEnum is BooleonScopeEnumPascoe;
}

