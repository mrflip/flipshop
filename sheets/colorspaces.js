/**
 * Color.gs — conversions between hexcolor, RGB, HSV, Oklab and Oklch.
 *
 * Unit conventions, since they differ per space:
 *   rgb    red/grn/blu   0-255 integers
 *   hsv    hue 0-360     sat 0-1        val 0-1
 *   oklab  okl 0-1       oka/okb approx -0.4..0.4
 *   oklch  okl 0-1       okc 0-0.4      okh 0-360
 *
 * Every *For() function returns null rather than throwing, so a bad cell
 * value propagates as an empty result instead of a #ERROR across the sheet.
 *
 * Depends on CONFIG (Code.gs) for allowShorthand / requireLowercase. Those
 * reads sit inside function bodies on purpose: Apps Script shares one global
 * scope across .gs files but evaluates them in project order, so touching
 * another file's const at load time throws.
 */

const LO_HEXCOLOR_RE    = /^(#([0-9a-f])([0-9a-f])([0-9a-f])(?:[0-9a-f]{3})?)$/
const HEXCOLOR_RE       = new RegExp(LO_HEXCOLOR_RE, 'i')
const FULL_HEXCOLOR_LEN = 7

const CSPACE_NAME_RE = /^(rgb|hsv|oklch|oklab)$/

const SRGB_KNEE         = 0.04045    // sRGB decode knee, in 0-1 units
const SRGB_ENCODE_KNEE  = 0.0031308  // the same knee, coming back the other way
const SRGB_KNEE_DIVISOR = 12.92
const GAMUT_TOLERANCE   = 1e-6       // slack for float error at the gamut wall
const ACHROMATIC_C      = 1e-7       // below this chroma, hue is meaningless

const WCAG_CHANNEL_WEIGHTS = [0.2126, 0.7152, 0.0722]
const DARK_TEXT_MIN_LUM    = 0.179   // above this, black text beats white text

/**
 * Returns a canonical hexcolor ('#' + a 6-digit lowercase hex value), or null if it's not a strict match
 * @param {string | null} str Raw cell value.
 * @param {Object} [opts] Defaults to CONFIG so every caller agrees on what counts.
 * @return {string | null} Normalized color
 */
function normalizeHexcolor(str, opts = CONFIG) {
  const { requireLowercase = false, allowShorthand = false } = opts ?? {}
  if (! (str && isString(str)))             { return null }                     // null: empty or not a string
  const matcher = requireLowercase ? LO_HEXCOLOR_RE : HEXCOLOR_RE
  const [_str, hexcolor, rr, gg, bb] = matcher.exec(str.trim()) ?? ['', null]
  if (! hexcolor)                           { return null }                     // null: doesn't work as a hex code
  if (hexcolor.length == FULL_HEXCOLOR_LEN) { return hexcolor.toLowerCase() }   // color: full hexcolor
  if (! allowShorthand)                     { return null }                     // null: shorthand but we don't care
  return ('#' + rr + rr + gg + gg + bb + bb).toLowerCase()                      // color: shorthand and we do care
}

/**
 * Splits a hexcolor into its three 8-bit channel values
 * @param {string} str Raw cell value; normalized before use.
 * @return {[number, number, number] | null} Red, green, blue, each 0-255
 */
function rgbForHexcolor(str) {
  const hexcolor = normalizeHexcolor(str)
  if (! hexcolor) { return null }
  return [
    parseInt(hexcolor.slice(1, 3), 16),
    parseInt(hexcolor.slice(3, 5), 16),
    parseInt(hexcolor.slice(5, 7), 16)
  ]
}

/**
 * Returns [red, grn, blu] given a single hexcolor, or three components plus a space name (default 'rgb')
 * @param {...*} args Either:
 *   - (str) a string giving the hexcode
 *   - (t0, t1, t2, cspace = 'rgb')
 *   - a triple and optional colorspace ([t0, t1, t2], cspace = 'rgb')
 *   - a 1x3 sheet range or another colorfunction's output ([[t0, t1, t2]], cspace = 'rgb')
 *   - see CSPACE_NAME_RE for allowed colorspaces
 * @return {[number, number, number] | null} Red, green, blue, each 0-255
 */
function rgbFor(...args) {
  if (args.length === 1 && isString(args[0])) { return rgbForHexcolor(args[0]) }

  const [t0, t1, t2, cspace] = getTripleFromArgs(args) ?? []
  if (! isFinite(t0)) { return null }

  if (cspace === 'rgb')   { return [clamp8bit(t0), clamp8bit(t1), clamp8bit(t2)] }
  if (cspace === 'hsv')   { return rgbForHSV(t0, t1, t2)   }
  if (cspace === 'oklab') { return rgbForOKLAB(t0, t1, t2) }
  if (cspace === 'oklch') { return rgbForOKLCH(t0, t1, t2) }
  return null
}

/**
 * Unwraps the several shapes a triple can arrive in, and settles the space name.
 * @param {Array} args The raw arguments array.
 * @return {[number, number, number, string] | null} Components plus lowercased space name
 */
function getTripleFromArgs(args) {
  if (isArray(args[0]) && isArray(args[0][0])) { return getTripleFromArgs([...args[0][0], args[1] ?? 'rgb']) }
  if (isTriple(args[0]))                       { return getTripleFromArgs([...args[0], args[1] ?? 'rgb']) }

  const [t0, t1, t2, cspace = 'rgb'] = args
  if (! isString(cspace))                      { return null }
  if (! CSPACE_NAME_RE.test(cspace))           { return null }
  if (! isTriple([t0, t1, t2]))                { return null }
  return [t0, t1, t2, cspace.toLowerCase()]
}

/**
 * Linear-light LMS cone responses.
 * @param {...*} args As rgbFor.
 * @return {[number, number, number] | null} Long, medium, short
 */
function lmsFor(...args) {
  const [red255, grn255, blu255] = rgbFor(...args) ?? []
  if (! isFinite(red255)) { return null }

  const [sred, sgrn, sblu] = [linearForRGB(red255), linearForRGB(grn255), linearForRGB(blu255)]

  return [
    (0.4122214708 * sred + 0.5363325363 * sgrn + 0.0514459929 * sblu),
    (0.2119034982 * sred + 0.6806995451 * sgrn + 0.1073969566 * sblu),
    (0.0883024619 * sred + 0.2817188376 * sgrn + 0.6299787005 * sblu)
  ]
}

/**
 * @param {...*} args As rgbFor.
 * @return {[number, number, number] | null} Lightness 0-1, and the two opponent axes
 */
function oklabFor(...args) {
  const [lms_l, lms_m, lms_s] = lmsFor(...args) ?? []
  if (! isFinite(lms_l)) { return null }

  const [l3, m3, s3] = [Math.cbrt(lms_l), Math.cbrt(lms_m), Math.cbrt(lms_s)]

  return [
    0.2104542553 * l3 + 0.7936177850 * m3 - 0.0040720468 * s3,
    1.9779984951 * l3 - 2.4285922050 * m3 + 0.4505937099 * s3,
    0.0259040371 * l3 + 0.7827717662 * m3 - 0.8086757660 * s3
  ]
}

/**
 * Oklab in cylindrical form. Lightness is perceptual, not luminance;
 * chroma is absolute colorfulness, not saturation.
 * @param {...*} args As rgbFor.
 * @return {[number, number, number] | null} Lightness 0-1, chroma 0-0.4, hue 0-360
 */
function oklchFor(...args) {
  const [okl, oka, okb] = oklabFor(...args) ?? []
  if (! isFinite(okl)) { return null }

  const okc = Math.hypot(oka, okb)
  if (okc < ACHROMATIC_C) { return [okl, 0, 0] }                                 // grey: hue is undefined, report 0

  return [okl, okc, ((Math.atan2(okb, oka) * 180 / Math.PI) % 360 + 360) % 360]
}

/**
 * @param {...*} args As rgbFor.
 * @return {[number, number, number] | null} Hue 0-360, saturation 0-1, value 0-1
 */
function hsvFor(...args) {
  const [red, grn, blu] = rgbFor(...args) ?? []
  if (! isFinite(red)) { return null }

  const scaled = [red / 255, grn / 255, blu / 255]
  const val    = Math.max(...scaled)
  const low    = Math.min(...scaled)
  const range  = val - low

  if (range === 0)          { return [0, 0, val] }                              // grey: hue and sat both vanish

  const [sRed, sGrn, sBlu] = scaled
  let hue
  if (val === sRed)         { hue = ((sGrn - sBlu) / range) % 6 }
  else if (val === sGrn)    { hue = ((sBlu - sRed) / range) + 2 }
  else                      { hue = ((sRed - sGrn) / range) + 4 }

  return [((hue * 60) % 360 + 360) % 360, range / val, val]
}

/**
 * @param {number} hue Hue 0-360; wraps.
 * @param {number} sat Saturation 0-1.
 * @param {number} val Value 0-1.
 * @return {[number, number, number] | null} Red, green, blue, each 0-255
 */
function rgbForHSV(hue, sat, val) {
  if (! isTriple([hue, sat, val])) { return null }

  const wrapped    = ((hue % 360) + 360) % 360
  const satClamped = Math.min(Math.max(sat, 0), 1)
  const valClamped = Math.min(Math.max(val, 0), 1)

  const chroma = valClamped * satClamped
  const sector = wrapped / 60
  const second = chroma * (1 - Math.abs((sector % 2) - 1))
  const lift   = valClamped - chroma

  let triple
  if (sector < 1)      { triple = [chroma, second, 0] }
  else if (sector < 2) { triple = [second, chroma, 0] }
  else if (sector < 3) { triple = [0, chroma, second] }
  else if (sector < 4) { triple = [0, second, chroma] }
  else if (sector < 5) { triple = [second, 0, chroma] }
  else                 { triple = [chroma, 0, second] }

  return triple.map((channel) => clamp8bit((channel + lift) * 255))
}

/**
 * Inverse of oklabFor. Returns null when the color falls outside sRGB —
 * clamping instead would silently shift hue and lightness. Use forceGamut
 * first if you want the nearest reachable color rather than a rejection.
 * @param {number} okl Lightness 0-1.
 * @param {number} oka Green-red axis.
 * @param {number} okb Blue-yellow axis.
 * @return {[number, number, number] | null} Red, green, blue, each 0-255
 */
function rgbForOKLAB(okl, oka, okb) {
  const linear = linearForOklab(okl, oka, okb)
  if (! linear)          { return null }
  if (! inGamut(linear)) { return null }

  return linear.map((channel) => clamp8bit(srgbForLinear(channel) * 255))
}

/**
 * Inverse of oklchFor.
 * @param {number} okl Lightness 0-1.
 * @param {number} okc Chroma 0-0.4.
 * @param {number} okh Hue 0-360; wraps.
 * @return {[number, number, number] | null} Red, green, blue, each 0-255, or null if out of gamut
 */
function rgbForOKLCH(okl, okc, okh) {
  if (! isTriple([okl, okc, okh])) { return null }

  const radians = ((okh % 360) + 360) % 360 * Math.PI / 180
  return rgbForOKLAB(okl, okc * Math.cos(radians), okc * Math.sin(radians))
}

/**
 * Pulls an out-of-gamut color back to the gamut wall by reducing chroma only,
 * holding lightness and hue fixed. That is the trade you want: dropping
 * saturation is far less visible than the hue and lightness shifts you get
 * from clamping channels.
 * @param {number} okl Lightness 0-1.
 * @param {number} okc Chroma 0-0.4.
 * @param {number} okh Hue 0-360.
 * @return {[number, number, number] | null} An in-gamut [okl, okc, okh]
 */
function forceGamut(okl, okc, okh) {
  if (! isTriple([okl, okc, okh]))  { return null }
  if (rgbForOKLCH(okl, okc, okh))   { return [okl, okc, okh] }                   // already fine, leave it alone
  return [okl, maxChromaForOKLCH(okl, okh), okh]
}

/**
 * The most chroma sRGB can hold at a given lightness and hue. This is the
 * function behind a constant-lightness palette row: take the minimum across
 * every hue you intend to use, and every column stays in gamut.
 * @param {number} okl Lightness 0-1.
 * @param {number} okh Hue 0-360.
 * @return {number} Chroma, 0 if the lightness itself is unreachable
 */
function maxChromaForOKLCH(okl, okh) {
  if (! isTriple([okl, 0, okh])) { return 0 }

  let lo = 0
  let hi = 0.45
  for (let ii = 0; ii < 40; ii++) {
    const mid = (lo + hi) / 2
    if (rgbForOKLCH(okl, mid, okh)) { lo = mid } else { hi = mid }
  }
  return lo
}

/**
 * Relative luminance per WCAG 2.x. Distinct from Oklab lightness: this is
 * photometric (proportional to light energy), where lightness is perceptual.
 * @param {[number, number, number]} rgb Red, green, blue, each 0-255.
 * @return {number} Luminance, 0 (black) through 1 (white)
 */
function wcagLuminanceForRGB(rgb) {
  if (! isTriple(rgb)) { return 0 }
  return rgb.reduce(function(total, channel, ii) {
    return total + (linearForRGB(channel) * WCAG_CHANNEL_WEIGHTS[ii])
  }, 0)
}

/**
 * Picks whichever of black or white stays readable on a given swatch
 * @param {string} hexcolor Canonical '#rrggbb' string.
 * @return {string} Font color as a hexcolor
 */
function textColorForHexcolor(hexcolor) {
  const luminance = wcagLuminanceForRGB(rgbForHexcolor(hexcolor))
  return (luminance > DARK_TEXT_MIN_LUM) ? '#000000' : '#ffffff'
}

/**
 * @param {...*} args As rgbFor.
 * @return {string | null} Canonical '#rrggbb'
 */
function hexcolorFor(...args) {
  if (args.length === 1 && isString(args[0])) { return normalizeHexcolor(args[0]) }
  const [red, grn, blu] = rgbFor(...args) ?? []
  if (! isFinite(red)) { return null }

  return '#' + hexpad(red) + hexpad(grn) + hexpad(blu)
}

// --- helpers ---------------------------------------------------------------

function linearForRGB(val)  { return linearForSRGB(val / 255) }

function linearForSRGB(val) {
  return (val > SRGB_KNEE) ? Math.pow((val + 0.055) / 1.055, 2.4) : (val / SRGB_KNEE_DIVISOR)
}

function srgbForLinear(val) {
  if (val <= SRGB_ENCODE_KNEE) { return val * SRGB_KNEE_DIVISOR }
  return 1.055 * Math.pow(Math.max(val, 0), 1 / 2.4) - 0.055
}

function linearForOklab(okl, oka, okb) {
  if (! isTriple([okl, oka, okb])) { return null }

  const lmsLong  = okl + 0.3963377774 * oka + 0.2158037573 * okb
  const lmsMed   = okl - 0.1055613458 * oka - 0.0638541728 * okb
  const lmsShort = okl - 0.0894841775 * oka - 1.2914855480 * okb

  const lmsLongCubed  = lmsLong ** 3
  const lmsMedCubed   = lmsMed ** 3
  const lmsShortCubed = lmsShort ** 3

  return [
     4.0767416621 * lmsLongCubed - 3.3077115913 * lmsMedCubed + 0.2309699292 * lmsShortCubed,
    -1.2684380046 * lmsLongCubed + 2.6097574011 * lmsMedCubed - 0.3413193965 * lmsShortCubed,
    -0.0041960863 * lmsLongCubed - 0.7034186147 * lmsMedCubed + 1.7076147010 * lmsShortCubed
  ]
}

function inGamut(linear) {
  return linear.every((channel) => ((channel >= -GAMUT_TOLERANCE) && (channel <= 1 + GAMUT_TOLERANCE)))
}

function clamp8bit(val) {
  if (! isFinite(val)) { return 0 }
  return Math.min(Math.max(Math.round(val), 0), 255)
}

function isTriple(arr) { return Array.isArray(arr) && (arr.length === 3) && arr.every(Number.isFinite) }
function isString(val) { return (typeof val === 'string') }
function isFinite(val) { return Number.isFinite(val) }
function isArray(val)  { return Array.isArray(val) }

function first(val)  { return (Array.isArray(val) && val.length >= 1) ? val[0] : null }
function second(val) { return (Array.isArray(val) && val.length >= 2) ? val[1] : null }
function third(val)  { return (Array.isArray(val) && val.length >= 3) ? val[2] : null }

function hexpad(val) { return isFinite(val) ? clamp8bit(val).toString(16).padStart(2, '0') : '' }

// --- sheet-facing wrappers -------------------------------------------------

function HEXCOLOR(...args) { return hexcolorFor(...args) }

function RGB(...args)   { const triple = rgbFor(...args);   return triple ? [triple] : null }
function OKLAB(...args) { const triple = oklabFor(...args); return triple ? [triple] : null }
function OKLCH(...args) { const triple = oklchFor(...args); return triple ? [triple] : null }
function HSV(...args)   { const triple = hsvFor(...args);   return triple ? [triple] : null }

function RGB_R(...args) { return  first(rgbFor(...args)) }
function RGB_G(...args) { return second(rgbFor(...args)) }
function RGB_B(...args) { return  third(rgbFor(...args)) }

function OKLAB_L(...args) { return  first(oklabFor(...args)) }
function OKLAB_A(...args) { return second(oklabFor(...args)) }
function OKLAB_B(...args) { return  third(oklabFor(...args)) }

function OKLCH_L(...args) { return  first(oklchFor(...args)) }
function OKLCH_C(...args) { return second(oklchFor(...args)) }
function OKLCH_H(...args) { return  third(oklchFor(...args)) }

function HSV_H(...args) { return  first(hsvFor(...args)) }
function HSV_S(...args) { return second(hsvFor(...args)) }
function HSV_V(...args) { return  third(hsvFor(...args)) }

function MAX_CHROMA(okl, okh)  { const triple = forceGamut(okl, 0.45, okh); return second(triple) }
function FORCE_GAMUT(...args)  { const triple = forceGamut(...args); return triple ? [triple] : null }