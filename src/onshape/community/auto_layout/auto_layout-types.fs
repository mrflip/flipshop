FeatureScript 819;
import(path : "onshape/std/geometry.fs", version : "819.0");

export type AutoLayoutAttribute typecheck canBeAutoLayoutAttribute;

export predicate canBeAutoLayoutAttribute(value)
{
    value is string;
    value == "AutoLayout_PLACED";
}
