FeatureScript 2433;
import(path : "onshape/std/common.fs", version : "2433.0");

icon::import(path : "7fff4d05dd2e2b0727b3ccd4", version : "aa76da6f9525c8d447e247f0");


export import(path : "c7c08274a0d273b9a5f5b47d/086a62a637585e5a5d42d2c8/0f412781b1b19b04fbb0b9aa", version : "fdb8c450034239844d28efcb");

annotation { "Feature Type Name" : "Decimal to fraction", "Feature Type Description" : "", "Icon" : icon::BLOB_DATA }
export const decimalToFraction = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Variable name" }
        definition.variableName is string;

        annotation { "Name" : "Length" }
        isLength(definition.length, LENGTH_BOUNDS);

        annotation { "Name" : "Rounding increments" }
        isLength(definition.rounding, { (inch) : [0, /* min (inclusive) */
                            0.0625, /* default */
                            10000 /* max (inclusive) */] } as LengthBoundSpec);

        annotation { "Name" : "Round direction", "UIHint" : UIHint.SHOW_LABEL, "Default" : RoundingDirection.Closest }
        definition.roundDirection is RoundingDirection;
    }
    {
        const s = decimalToFractionalInches(definition.length, definition.rounding, definition.roundDirection);

        setVariable(context, definition.variableName, s);
    });
