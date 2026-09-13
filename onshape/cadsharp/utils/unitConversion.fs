FeatureScript 2368;
import(path : "onshape/std/common.fs", version : "2368.0");

export enum Units
{
    annotation { "Name" : "mm" }
    MM,
    annotation { "Name" : "cm" }
    CM,
    annotation { "Name" : "m" }
    M,
    annotation { "Name" : "in" }
    IN,
    annotation { "Name" : "ft" }
    FT,
    annotation { "Name" : "yd" }
    YD,
}

const unitCalculations = {
        Units.MM : meter / millimeter,
        Units.CM : meter / centimeter,
        Units.M : meter / meter,
        Units.IN : meter / inch,
        Units.FT : meter / foot,
        Units.YD : meter / yard
    };

const unitStrings = {
        Units.MM : "mm",
        Units.CM : "cm",
        Units.M : "m",
        Units.IN : "in",
        Units.FT : "ft",
        Units.YD : "yd"
    };


/**
 * Converts a to a string to the desired units.
 *
 * @param definition {{
 *      @field valueWithUnits {Length} : Value with units to convert to string..
 *      @field units {Units} : Units to convert to.
 *      @field precision {Length} : Precision to roud to. For example, 3.
 * }}
 *
 * @returns {string} : Returns a string with the converted units.
 */
export function ConvertToUnitsString(definition is map)
{
    var value = definition.valueWithUnits.value;
    value = value * unitCalculations[definition.units];
    value = toString(roundToPrecision(value, 3));
    value = value ~ unitStrings[definition.units];

    return value;
}

