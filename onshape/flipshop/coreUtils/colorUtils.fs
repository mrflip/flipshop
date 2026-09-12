FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
// import(path : "4ebdc64943b566160ea5cc28", version : "ab211723d9126cd674ed5829");
import(path : "66e287bede293cb227dfb89c", version : "e6c9aacc05cf0a6f15c7d4c7");
import(path : "dd812faf6ff4099cda4aa0eb", version : "ccf1aec9c08b9ad59691b4d4");


/** sets the APPEARANCE property on the query objects to the given shade; color can be a Color ({ red: 0-1, blue: ... }) or a hexcolor ("#f00ba7") */
export function setColor(context is Context, qq is Query, cmap is Color) {
    //set color of hole
    setProperty(context, {
        "entities" :     qq,
        "propertyType" : PropertyType.APPEARANCE,
        "value" :        cmap,
    });
}
export function setColor(context is Context, qq is Query, colorstr is string) {
    return setColor(context, qq, toColor(colorstr));
}
export function setColor(context is Context, qq is Query, carr is array) {
    return setColor(context, qq, toColor(carr));
}

/**
 * Converts hexadecimal values to RGB. Based on @Pascoe -- https://cad.onshape.com/documents/c7c08274a0d273b9a5f5b47d/v/3bc427217b140370d222e3c6/e/f5c0f1a094e248af44b52910
 *
 * @param hexadecimal {string} : A hexadecimal value.
 *
 *@returns {{
 *      @field red {number} : Red.
 *      @field green {number} : Green.
 *      @field blue {number} : Blue.
 *      @field alpha {number} : Alpha.
 * }}
 */
export function hexcolorToColor(hexcolor is string) returns Color
{
    const mm = match(hexcolor, HexcolorRE);
    if (! mm.hasMatch) {  return OopsColor; }

    var mapcolor = { alpha: 1.0 };

    // Convert each character to its corresponding decimal value
    mapcolor.red   = hexpairToInt(mm.captures[1]) / 255;
    mapcolor.green = hexpairToInt(mm.captures[2]) / 255;
    mapcolor.blue  = hexpairToInt(mm.captures[3]) / 255;
    mapcolor.alpha = hexpairToInt(mm.captures[4], 255) / 255;
    return mapcolor as Color;
}

/**
 * Converts a decimal triplet or quartet to RGB.
 *
 * @param tuplestr {string} : A string like `[0, 1, 255]` or `200,99,100,33`. Spaces and brackets are optional; the values must be integers and there must be three or four of them.
 *
 *@returns {{
 *      @field red {number} : Red.
 *      @field green {number} : Green.
 *      @field blue {number} : Blue.
 *      @field alpha {number} : Alpha.
 * }}
 */
export function tuplestrToColor(tuplestr is string) returns Color
{
    const mm = match(tuplestr, TuplecolorRE);
    if (! mm.hasMatch) {  return OopsColor; }

    var mapcolor = { };

    // Convert each character to its corresponding decimal value
    mapcolor.red   = stringToNumber(mm.captures[1]) / 255;
    mapcolor.green = stringToNumber(mm.captures[2]) / 255;
    mapcolor.blue  = stringToNumber(mm.captures[3]) / 255;
    mapcolor.alpha = strBlank(mm.captures[4]) ? 1.0 : (stringToNumber(mm.captures[4]) / 255);
    // println('tuplestrToColor' ~ mm ~ ' -> ' ~ mapcolor);
    return mapcolor as Color;
}

/**
 * Converts triplet or quartet of 0.0-1.0 values to RGB
 *
 * @param unitcolor {string} : A string like `[0.5, 0.8675309, 1.0]` or `[0.0, 0.1, 1.0, 1.0]`.
 *
 *@returns {{
 *      @field red {number} : Red.
 *      @field green {number} : Green.
 *      @field blue {number} : Blue.
 *      @field alpha {number} : Alpha.
 * }}
 */
export function unitcolorToColor(tuplestr is string) returns Color
{
    const mm = match(tuplestr, UnitcolorRE);
    if (! mm.hasMatch) {  return OopsColor; }

    var mapcolor = { };

    // Convert each character to its corresponding decimal value
    mapcolor.red   = stringToNumber(mm.captures[1]);
    mapcolor.green = stringToNumber(mm.captures[2]);
    mapcolor.blue  = stringToNumber(mm.captures[3]);
    mapcolor.alpha = strBlank(mm.captures[4]) ? 1.0 : (stringToNumber(mm.captures[4]));
    // println('unitcolorToColor' ~ mm ~ ' -> ' ~ mapcolor);
    return mapcolor as Color;
}

export function toColor(cmap is Color) returns Color {
    return cmap;
}

export function toColor(carr is array) returns Color {
    if (size(carr) == 3) { return color(carr[0], carr[1], carr[2]); }
    if (size(carr) == 4) { return color(carr[0], carr[1], carr[2], carr[3]); }
    return OopsColor;
}

export function toColor(raw is string) returns Color {
    const str = replace(raw, "\\s+", "");
    try {
        if (hasMatch(str, HexcolorRE))   { return hexcolorToColor(str); }
        if (hasMatch(str, UnitcolorRE))  { return unitcolorToColor(str); }
        if (hasMatch(str, TuplecolorRE)) { return tuplestrToColor(str); }
        println('toColor found no matching strategy for "' ~ str ~ '"');
    } catch (err) { println('toColor had an error ' ~ err ~ ' for "' ~ str ~ '"'); }
    return OopsColor;
}

export function toHexcolor(color is Color) returns string {
    const rgb = toTuplecolor(color);
    return '#' ~ intToHexpair(rgb[0]) ~ intToHexpair(rgb[1]) ~ intToHexpair(rgb[2]) ~ intToHexpair(rgb[3]);
}

export function toUnitcolor(color is Color) returns array {
    const alpha = ifNil(color.alpha, 1.0);
    return [color.red, color.green, color.blue, alpha];
}

export function toTuplecolor(color is Color) returns array {
    const alpha = ifNil(color.alpha, 1.0);
    return [
        roundToPrecision(color.red    * 255, 0),
        roundToPrecision(color.green  * 255, 0),
        roundToPrecision(color.blue   * 255, 0),
        roundToPrecision(alpha        * 255, 0),
    ];
}

export function hexpairToInt(hexpair is string, fallback) {
    const num = HexpairToInt[downcase(hexpair)];
    if (num == undefined) { return fallback; }
    return num;
}
export function hexpairToInt(hexpair is string) { return hexpairToInt(hexpair, undefined); }

export function intToHexpair(num is number) {
    return IntToHexpair[num];
}

export function sameColor(c1 is Color, c2 is Color, tol is number) {
    const alpha1 = ifNil(c1.alpha, 1.0);
    const alpha2 = ifNil(c2.alpha, 1.0);
    return tolerantEquals(c1.red, c2.red, tol) && tolerantEquals(c1.green, c2.green, tol) && tolerantEquals(c1.blue, c2.blue, tol) && tolerantEquals(alpha1, alpha2, tol);
}
export function sameColor(c1 is Color, c2 is Color) {
    return sameColor(c1, c2, 0.999999999 / 255);
}

export function isHexcolor(str) {
    if (! (str is string)) { return false; }
    return hasMatch(str, HexcolorRE);
}

//  it won't import from his libs, why? Copied instead

const OopsColor = { red: 1, blue: 0, green: 0, alpha: 1.0 } as Color;
export const HexcolorRE = "^#?([0-9a-f][0-9a-f])([0-9a-f][0-9a-f])([0-9a-f][0-9a-f])([0-9a-f][0-9a-f])?$";

// export const TuplecolorRE = "^ *\\[? *([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5]) *, *([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5]) *, *([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(?: *, *([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5]))? *\\]?$";

export const TuplecolorRE = "^ *\\[? *(\\d+) *, *(\\d+) *, *(\\d+)(?: *, *(\\d+))? *\\]?$";
export const UnitcolorRE = "^ *\\[? *([01](?:\\.\\d+)?) *, *([01](?:\\.\\d+)?) *, *([01](?:\\.\\d+)?)(?: *, *([01](?:\\.\\d+)?))? *\\]? *$";

// Define a map to convert hex characters to decimal values
export const HexCharToInt = {
    '0' : 0, '1' : 1, '2' : 2, '3' : 3, '4' : 4, '5' : 5, '6' : 6, '7' : 7,
    '8' : 8, '9' : 9, 'A' : 10, 'B' : 11, 'C' : 12, 'D' : 13, 'E' : 14, 'F' : 15,
    'a' : 10, 'b' : 11, 'c' : 12, 'd' : 13, 'e' : 14, 'f' : 15
};

// Define a map to convert hex characters to decimal values
const HexpairToInt = {
    '00' :  0,  '01' :  1,  '02' :  2,  '03' :  3,  '04' :  4,  '05' :  5,  '06' :  6,  '07' :  7,   '08' :  8,  '09' :  9,  '0a' : 10,  '0b' : 11,  '0c' : 12,  '0d' : 13,  '0e' : 14,  '0f' : 15,
    '10' : 16,  '11' : 17,  '12' : 18,  '13' : 19,  '14' : 20,  '15' : 21,  '16' : 22,  '17' : 23,   '18' : 24,  '19' : 25,  '1a' : 26,  '1b' : 27,  '1c' : 28,  '1d' : 29,  '1e' : 30,  '1f' : 31,
    '20' : 32,  '21' : 33,  '22' : 34,  '23' : 35,  '24' : 36,  '25' : 37,  '26' : 38,  '27' : 39,   '28' : 40,  '29' : 41,  '2a' : 42,  '2b' : 43,  '2c' : 44,  '2d' : 45,  '2e' : 46,  '2f' : 47,
    '30' : 48,  '31' : 49,  '32' : 50,  '33' : 51,  '34' : 52,  '35' : 53,  '36' : 54,  '37' : 55,   '38' : 56,  '39' : 57,  '3a' : 58,  '3b' : 59,  '3c' : 60,  '3d' : 61,  '3e' : 62,  '3f' : 63,
    '40' : 64,  '41' : 65,  '42' : 66,  '43' : 67,  '44' : 68,  '45' : 69,  '46' : 70,  '47' : 71,   '48' : 72,  '49' : 73,  '4a' : 74,  '4b' : 75,  '4c' : 76,  '4d' : 77,  '4e' : 78,  '4f' : 79,
    '50' : 80,  '51' : 81,  '52' : 82,  '53' : 83,  '54' : 84,  '55' : 85,  '56' : 86,  '57' : 87,   '58' : 88,  '59' : 89,  '5a' : 90,  '5b' : 91,  '5c' : 92,  '5d' : 93,  '5e' : 94,  '5f' : 95,
    '60' : 96,  '61' : 97,  '62' : 98,  '63' : 99,  '64' : 100, '65' : 101, '66' : 102, '67' : 103,  '68' : 104, '69' : 105, '6a' : 106, '6b' : 107, '6c' : 108, '6d' : 109, '6e' : 110, '6f' : 111,
    '70' : 112, '71' : 113, '72' : 114, '73' : 115, '74' : 116, '75' : 117, '76' : 118, '77' : 119,  '78' : 120, '79' : 121, '7a' : 122, '7b' : 123, '7c' : 124, '7d' : 125, '7e' : 126, '7f' : 127,
    '80' : 128, '81' : 129, '82' : 130, '83' : 131, '84' : 132, '85' : 133, '86' : 134, '87' : 135,  '88' : 136, '89' : 137, '8a' : 138, '8b' : 139, '8c' : 140, '8d' : 141, '8e' : 142, '8f' : 143,
    '90' : 144, '91' : 145, '92' : 146, '93' : 147, '94' : 148, '95' : 149, '96' : 150, '97' : 151,  '98' : 152, '99' : 153, '9a' : 154, '9b' : 155, '9c' : 156, '9d' : 157, '9e' : 158, '9f' : 159,
    'a0' : 160, 'a1' : 161, 'a2' : 162, 'a3' : 163, 'a4' : 164, 'a5' : 165, 'a6' : 166, 'a7' : 167,  'a8' : 168, 'a9' : 169, 'aa' : 170, 'ab' : 171, 'ac' : 172, 'ad' : 173, 'ae' : 174, 'af' : 175,
    'b0' : 176, 'b1' : 177, 'b2' : 178, 'b3' : 179, 'b4' : 180, 'b5' : 181, 'b6' : 182, 'b7' : 183,  'b8' : 184, 'b9' : 185, 'ba' : 186, 'bb' : 187, 'bc' : 188, 'bd' : 189, 'be' : 190, 'bf' : 191,
    'c0' : 192, 'c1' : 193, 'c2' : 194, 'c3' : 195, 'c4' : 196, 'c5' : 197, 'c6' : 198, 'c7' : 199,  'c8' : 200, 'c9' : 201, 'ca' : 202, 'cb' : 203, 'cc' : 204, 'cd' : 205, 'ce' : 206, 'cf' : 207,
    'd0' : 208, 'd1' : 209, 'd2' : 210, 'd3' : 211, 'd4' : 212, 'd5' : 213, 'd6' : 214, 'd7' : 215,  'd8' : 216, 'd9' : 217, 'da' : 218, 'db' : 219, 'dc' : 220, 'dd' : 221, 'de' : 222, 'df' : 223,
    'e0' : 224, 'e1' : 225, 'e2' : 226, 'e3' : 227, 'e4' : 228, 'e5' : 229, 'e6' : 230, 'e7' : 231,  'e8' : 232, 'e9' : 233, 'ea' : 234, 'eb' : 235, 'ec' : 236, 'ed' : 237, 'ee' : 238, 'ef' : 239,
    'f0' : 240, 'f1' : 241, 'f2' : 242, 'f3' : 243, 'f4' : 244, 'f5' : 245, 'f6' : 246, 'f7' : 247,  'f8' : 248, 'f9' : 249, 'fa' : 250, 'fb' : 251, 'fc' : 252, 'fd' : 253, 'fe' : 254, 'ff' : 255,
};

const IntToHexpair = [
    '00', '01', '02', '03', '04', '05', '06', '07', '08', '09', '0a', '0b', '0c', '0d', '0e', '0f',
    '10', '11', '12', '13', '14', '15', '16', '17', '18', '19', '1a', '1b', '1c', '1d', '1e', '1f',
    '20', '21', '22', '23', '24', '25', '26', '27', '28', '29', '2a', '2b', '2c', '2d', '2e', '2f',
    '30', '31', '32', '33', '34', '35', '36', '37', '38', '39', '3a', '3b', '3c', '3d', '3e', '3f',
    '40', '41', '42', '43', '44', '45', '46', '47', '48', '49', '4a', '4b', '4c', '4d', '4e', '4f',
    '50', '51', '52', '53', '54', '55', '56', '57', '58', '59', '5a', '5b', '5c', '5d', '5e', '5f',
    '60', '61', '62', '63', '64', '65', '66', '67', '68', '69', '6a', '6b', '6c', '6d', '6e', '6f',
    '70', '71', '72', '73', '74', '75', '76', '77', '78', '79', '7a', '7b', '7c', '7d', '7e', '7f',
    '80', '81', '82', '83', '84', '85', '86', '87', '88', '89', '8a', '8b', '8c', '8d', '8e', '8f',
    '90', '91', '92', '93', '94', '95', '96', '97', '98', '99', '9a', '9b', '9c', '9d', '9e', '9f',
    'a0', 'a1', 'a2', 'a3', 'a4', 'a5', 'a6', 'a7', 'a8', 'a9', 'aa', 'ab', 'ac', 'ad', 'ae', 'af',
    'b0', 'b1', 'b2', 'b3', 'b4', 'b5', 'b6', 'b7', 'b8', 'b9', 'ba', 'bb', 'bc', 'bd', 'be', 'bf',
    'c0', 'c1', 'c2', 'c3', 'c4', 'c5', 'c6', 'c7', 'c8', 'c9', 'ca', 'cb', 'cc', 'cd', 'ce', 'cf',
    'd0', 'd1', 'd2', 'd3', 'd4', 'd5', 'd6', 'd7', 'd8', 'd9', 'da', 'db', 'dc', 'dd', 'de', 'df',
    'e0', 'e1', 'e2', 'e3', 'e4', 'e5', 'e6', 'e7', 'e8', 'e9', 'ea', 'eb', 'ec', 'ed', 'ee', 'ef',
    'f0', 'f1', 'f2', 'f3', 'f4', 'f5', 'f6', 'f7', 'f8', 'f9', 'fa', 'fb', 'fc', 'fd', 'fe', 'ff',
];