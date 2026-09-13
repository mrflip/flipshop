FeatureScript 2522;
import(path : "onshape/std/common.fs", version : "2522.0");
ICON::import(path : "1b7978ec3c19690900ea486e", version : "43a81ab368820d05f36996ac");

export enum ConversionUnits
{
    annotation { "Name" : "mm" }
    MM,
    annotation { "Name" : "cm" }
    CM,
    annotation { "Name" : "m" }
    M,
    annotation { "Name" : "in" }
    IN,
    annotation { "Name" : "in (\")" }
    IN_SHORT,
    annotation { "Name" : "ft" }
    FT,
    annotation { "Name" : "ft (\')" }
    FT_SHORT,
    annotation { "Name" : "yd" }
    YD,
    annotation { "Name" : "deg" }
    DEGREE,
    annotation { "Name" : "deg (*)" }
    DEGREE_SHORT,
}

enum ConversionDataKeys
{
    UNIT_CONSTANT,
    STRING
}

export enum RoundingSystem
{
    Decimal,
    Fraction
}

export enum IncrementType
{
    annotation { "Name" : "Precision (Decimal places)" }
    Precision,
    annotation { "Name" : "Incremental (Step size)" }
    Incremental
}

export enum RoundingDirection
{
    Up,
    Down,
    Closest
}

const CONVERSION_DATA = {
        ConversionUnits.MM : { ConversionDataKeys.UNIT_CONSTANT : millimeter, ConversionDataKeys.STRING : "mm" },
        ConversionUnits.CM : { ConversionDataKeys.UNIT_CONSTANT : centimeter, ConversionDataKeys.STRING : "cm" },
        ConversionUnits.M : { ConversionDataKeys.UNIT_CONSTANT : meter, ConversionDataKeys.STRING : "m" },
        ConversionUnits.IN : { ConversionDataKeys.UNIT_CONSTANT : inch, ConversionDataKeys.STRING : "in" },
        ConversionUnits.IN_SHORT : { ConversionDataKeys.UNIT_CONSTANT : inch, ConversionDataKeys.STRING : "\"" },
        ConversionUnits.FT : { ConversionDataKeys.UNIT_CONSTANT : foot, ConversionDataKeys.STRING : "ft" },
        ConversionUnits.FT_SHORT : { ConversionDataKeys.UNIT_CONSTANT : foot, ConversionDataKeys.STRING : "\'" },
        ConversionUnits.YD : { ConversionDataKeys.UNIT_CONSTANT : yard, ConversionDataKeys.STRING : "yd" },
        ConversionUnits.DEGREE : { ConversionDataKeys.UNIT_CONSTANT : degree, ConversionDataKeys.STRING : "deg" },
        ConversionUnits.DEGREE_SHORT : { ConversionDataKeys.UNIT_CONSTANT : degree, ConversionDataKeys.STRING : "°" }
    };

annotation {
        "Feature Type Name" : "Variable to string",
        "Icon" : ICON::BLOB_DATA,
        "Feature Type Description" : "Converts measured variables to string format.",
        "Feature Name Template" : "###name #v"
    }
export const variableToString = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Rounding system", "UIHint" : UIHint.HORIZONTAL_ENUM }
        definition.roundingSystem is RoundingSystem;

        annotation { "Name" : "Name" }
        definition.name is string;

        annotation { "Name" : "Variable to convert" }
        isAnything(definition.variable);

        annotation { "Name" : "Convert to", "Default" : ConversionUnits.IN, "UIHint" : [UIHint.SHOW_LABEL, UIHint.REMEMBER_PREVIOUS_VALUE] }
        definition.conversionUnits is ConversionUnits;

        annotation { "Name" : "Round direction", "UIHint" : UIHint.SHOW_LABEL, "Default" : RoundingDirection.Closest }
        definition.roundDirection is RoundingDirection;

        annotation { "Group Name" : "Increment type", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Increment type" }
            definition.incrementType is IncrementType;

            if (definition.incrementType == IncrementType.Precision)
            {
                annotation { "Name" : "Round to places" }
                isInteger(definition.round, { (unitless) : [0, 3, 10000] } as IntegerBoundSpec);

                annotation { "Name" : "Enforce trailing zeros", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
                definition.enforceTrailingZeros is boolean;

                annotation { "Group Name" : "Trailing zeros", "Driving Parameter" : "enforceTrailingZeros", "Collapsed By Default" : false }
                {
                    if (definition.enforceTrailingZeros)
                    {
                        annotation { "Name" : "Trailing zeros" }
                        isInteger(definition.trailingZeros, { (unitless) : [0, 3, 10000] } as IntegerBoundSpec);
                    }
                }
            }
            else // Incremental
            {
                if (definition.conversionUnits == ConversionUnits.DEGREE || definition.conversionUnits == ConversionUnits.DEGREE_SHORT)
                {
                    annotation { "Name" : "Rounding increments" }
                    isAngle(definition.roundingAngle, { (degree) : [0, 1, 100000] } as AngleBoundSpec);
                }
                else
                {
                    annotation { "Name" : "Rounding increments" }
                    isLength(definition.roundingLength, { (inch) : [0, 0.0625, 10000] } as LengthBoundSpec);
                }
            }
        }

        annotation { "Name" : "Remove empty space", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
        definition.removeEmptySpace is boolean;
    }
    {
        var v;

        // === RoundingSystem drives behavior first ===
        if (definition.roundingSystem == RoundingSystem.Decimal)
        {
            if (definition.incrementType == IncrementType.Precision)
            {
                // --- FIXED: apply rounding direction manually for precision mode (FeatureScript-safe) ---
                var convUnit = CONVERSION_DATA[definition.conversionUnits][ConversionDataKeys.UNIT_CONSTANT];
                var baseValue = definition.variable / convUnit; // unitless number in target units

                // compute 10^round manually (since pow() doesn't exist)
                var factor = 1;
                for (var i = 0; i < definition.round; i += 1)
                    factor *= 10;

                // Apply rounding direction manually
                if (definition.roundDirection == RoundingDirection.Up)
                    baseValue = ceil(baseValue * factor) / factor;
                else if (definition.roundDirection == RoundingDirection.Down)
                    baseValue = floor(baseValue * factor) / factor;
                else
                    baseValue = round(baseValue * factor) / factor;

                // Convert back to ValueWithUnits
                var roundedVal = baseValue * convUnit;

                v = valueWithUnitsToStringDecimal(roundedVal, definition.conversionUnits,
                    definition.round, definition.enforceTrailingZeros,
                    definition.trailingZeros, !definition.removeEmptySpace);
            }
            else // Decimal + Incremental
            {
                // Round to step, then format as decimal (uses safe default precision = 3, no forced zeros)
                var inc = _pickIncrement(definition);
                var roundedVal = _roundByDirection(definition.variable, inc, definition.roundDirection);
                v = valueWithUnitsToStringDecimal(roundedVal, definition.conversionUnits,
                    3 /* default precision for step-rounded decimals */,
                    false /* enforceTrailingZeros */, 0 /* trailingZeros */,
                    !definition.removeEmptySpace);
            }
        }
        else // RoundingSystem.Fraction
        {
            if (definition.incrementType == IncrementType.Incremental)
            {
                // Fraction + Incremental (original behavior)
                var roundingIncrement = _pickIncrement(definition);
                v = valueWithUnitsToStringFractional(definition.variable,
                    definition.conversionUnits,
                    roundingIncrement,
                    definition.roundDirection,
                    !definition.removeEmptySpace);
            }
            else // Fraction + Precision
            {
                // No step provided in Precision branch; use sensible default per unit for fractional rounding
                var fallbackInc = _defaultFractionIncrement(definition.conversionUnits);
                v = valueWithUnitsToStringFractional(definition.variable,
                    definition.conversionUnits,
                    fallbackInc,
                    definition.roundDirection,
                    !definition.removeEmptySpace);
            }
        }

        setVariable(context, definition.name, v);

        setFeatureComputedParameter(context, id, {
                    "name" : "name",
                    "value" : definition.name
                });

        setFeatureComputedParameter(context, id, {
                    "name" : "v",
                    "value" : (" = " ~ v)
                });

        reportFeatureInfo(context, id, v);
    });




// --- DECIMAL HANDLER ---
export function valueWithUnitsToStringDecimal(value is ValueWithUnits, conversionUnits is ConversionUnits,
    roundingPrecision is number, enforceTrailingZeros is boolean,
    trailingZeros is number, addSpaceBetween is boolean) returns string
{
    var conversionMultiplier = meter / CONVERSION_DATA[conversionUnits][ConversionDataKeys.UNIT_CONSTANT];
    const space = addSpaceBetween ? " " : "";
    var unitLabel = space ~ CONVERSION_DATA[conversionUnits][ConversionDataKeys.STRING];

    if (!(value / meter is ValueWithUnits)) // Length
    {
    }
    else if (!(value / squareMeter is ValueWithUnits)) // Area
    {
        conversionMultiplier = conversionMultiplier ^ 2;
        unitLabel = unitLabel ~ "²";
    }
    else if (!(value / cubicMeter is ValueWithUnits)) // Volume
    {
        conversionMultiplier = conversionMultiplier ^ 3;
        unitLabel = unitLabel ~ "³";
    }
    else if (!(value / degree is ValueWithUnits)) // Angle
    {
        conversionMultiplier = radian / degree;
    }

    var roundedValue = roundToPrecision(value.value * conversionMultiplier, roundingPrecision);

    // --- Restored original trailing-zero logic ---
    if (enforceTrailingZeros)
    {
        var notEnoughDecimals = false;
        var qtyTrailing = 0;
        var s = toString(roundedValue);
        s = splitByRegexp(s, "[.]");

        try silent
        {
            notEnoughDecimals = size(s) < 2;
            qtyTrailing = 0;
        }

        try silent
        {
            qtyTrailing = size(splitIntoCharacters(s[1]));
            notEnoughDecimals = qtyTrailing < trailingZeros;
        }

        if (notEnoughDecimals)
        {
            const zerosToAdd = trailingZeros - qtyTrailing;
            const zeroArray = makeArray(zerosToAdd, "0");
            var toAdd = "";

            for (var i = 0; i < size(zeroArray); i += 1)
            {
                toAdd ~= zeroArray[i];
            }

            try silent
            {
                toAdd = s[1] ~ toAdd;
            }

            roundedValue = s[0] ~ "." ~ toAdd;
        }
    }

    return roundedValue ~ unitLabel;
}



// --- FRACTIONAL CONVERSION HANDLER ---
export function valueWithUnitsToStringFractional(value is ValueWithUnits,
    conversionUnits is ConversionUnits,
    roundingIncrement is ValueWithUnits,
    roundingDirection is RoundingDirection,
    addSpaceBetween is boolean) returns string
{
    var conversionUnit = CONVERSION_DATA[conversionUnits][ConversionDataKeys.UNIT_CONSTANT];
    const space = addSpaceBetween ? " " : "";
    var displayUnit = space ~ CONVERSION_DATA[conversionUnits][ConversionDataKeys.STRING];

    if (conversionUnit == inch || conversionUnit == foot)
    {
        var fractional = decimalToFractional(value, conversionUnit, roundingIncrement, roundingDirection);
        return fractional ~ displayUnit;
    }
    else if (conversionUnit == degree)
    {
        var roundingValue = _roundByDirection(value, roundingIncrement, roundingDirection);

        var whole = floor((roundingValue + 1e-11 * radian) / degree);
        var remainder = roundingValue / degree - whole;
        var denom = 1;

        for (var i = 0; i < 6; i += 1)
        {
            if (abs(remainder - round(remainder)) < 1e-11)
                break;
            remainder *= 2;
            denom *= 2;
        }

        if (denom == 1)
            return whole->toString() ~ displayUnit;

        var num = round(remainder);
        if (whole == 0)
            return num ~ "/" ~ denom ~ displayUnit;

        return whole ~ " " ~ num ~ "/" ~ denom ~ displayUnit;
    }

    return roundToPrecision(value / conversionUnit, 3)->toString() ~ displayUnit;
}

// --- FRACTIONAL Values ---
export function decimalToFractional(x is ValueWithUnits,
    baseUnit is ValueWithUnits,
    roundingIncrement is ValueWithUnits,
    roundingDirection is RoundingDirection) returns string
{
    var roundingValue = _roundByDirection(x, roundingIncrement, roundingDirection);

    var whole = floor((roundingValue + 1e-11 * meter) / baseUnit);
    var remainder = roundingValue / baseUnit - whole;
    var denom = 1;

    for (var i = 0; i < 6; i += 1)
    {
        if (abs(remainder - round(remainder)) < 1e-11)
            break;
        remainder *= 2;
        denom *= 2;
    }

    if (denom == 1)
        return whole->toString();

    var num = round(remainder);
    if (whole == 0)
        return num ~ "/" ~ denom;
    return whole ~ " " ~ num ~ "/" ~ denom;
}

// --- HELPERS (no new inputs/outputs) ---
function _roundByDirection(val is ValueWithUnits,
    increment is ValueWithUnits,
    dir is RoundingDirection) returns ValueWithUnits
{
    if (dir == RoundingDirection.Up)
        return ceil(val / increment) * increment;
    if (dir == RoundingDirection.Down)
        return floor(val / increment) * increment;
    return round(val / increment) * increment;
}

function _pickIncrement(definition is map) returns ValueWithUnits
{
    if (definition.conversionUnits == ConversionUnits.DEGREE || definition.conversionUnits == ConversionUnits.DEGREE_SHORT)
        return definition.roundingAngle;
    else
        return definition.roundingLength;
}

function _defaultFractionIncrement(u is ConversionUnits) returns ValueWithUnits
{
    if (u == ConversionUnits.IN || u == ConversionUnits.IN_SHORT)
        return inch / 16;
    if (u == ConversionUnits.FT || u == ConversionUnits.FT_SHORT)
        return foot / 16;
    if (u == ConversionUnits.DEGREE || u == ConversionUnits.DEGREE_SHORT)
        return 1 * degree;
    // For non-fraction-friendly units, fall back to a small length step
    return millimeter; // safe default, won’t change inputs/UI
}
