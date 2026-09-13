FeatureScript 2411;
import(path : "onshape/std/common.fs", version : "2411.0");

export enum RoundingDirection
{
    Up,
    Down,
    Closest,
}

export function decimalToFractionalInches(x is ValueWithUnits, roundingIncrements is ValueWithUnits, roundingDirection is RoundingDirection) returns string
{
    // --- Apply rounding direction ---
    if (roundingDirection == RoundingDirection.Up)
    {
        x = ceil(x / roundingIncrements) * roundingIncrements;
    }
    else if (roundingDirection == RoundingDirection.Down)
    {
        x = floor(x / roundingIncrements) * roundingIncrements;
    }
    else // Closest
    {
        x = round(x / roundingIncrements) * roundingIncrements;
    }

    var wholeNumber = floor((x + 1e-11 * meter) / inch);
    var remainder = x / inch - wholeNumber;
    var fraction = 1;

    for (var i = 0; i < 8; i += 1)
    {
        if (abs(remainder - round(remainder)) < 1e-11)
        {
            break;
        }
        remainder *= 2;
        fraction *= 2;
    }

    if (fraction == 1)
    {
        return wholeNumber->toString();
    }

    var fractionalPart = round(remainder) ~ "/" ~ fraction;

    if (wholeNumber == 0)
    {
        return fractionalPart->toString();
    }

    return wholeNumber ~ " " ~ fractionalPart;
}
