const LO_HEXCOLOR_RE    = /^(#([0-9a-f])([0-9a-f])([0-9a-f])(?:[0-9a-f]{3})?)$/
const HEXCOLOR_RE       = new RegExp(LO_HEXCOLOR_RE, 'i')
const FULL_HEXCOLOR_LEN = 7

const SRGB_KNEE            = 0.04045    // sRGB decode knee, in 0-1 units
const SRGB_ENCODE_KNEE     = 0.0031308  // the same knee, coming back the other way
const SRGB_KNEE_DIVISOR    = 12.92
const GAMUT_TOLERANCE      = 1e-6       // slack for float error at the gamut wall
const ACHROMATIC_C         = 1e-7       // below this chroma, hue is meaningless

const WCAG_CHANNEL_WEIGHTS = [0.2126, 0.7152, 0.0722]
const DARK_TEXT_MIN_LUM    = 0.179   // above this, black text beats white text
//
const CSPACE_NAME_RE = /^(rgb|hsv|oklch|oklab)$/

/**
 * Returns a canonical hexcolor ('#' + a 6-digit lowercase hex value), or null if it's not a strict match
 * @param {string | null} str Raw cell value.
 * @return {string | null} Normalized color
 */
function normalizeHexcolor(str, { requireLowercase = false, allowShorthand = false } = {}) {
  if (! (str && isString(str)))  { return null }                     // null: empty or not a string
  const matcher = requireLowercase ? LO_HEXCOLOR_RE : HEXCOLOR_RE
  const [_str, hexcolor, rr, gg, bb] = matcher.exec(str.trim()) ?? ['', null]
  if (! hexcolor)                           { return null }                     // null: doesn't work as a hex code
  if (hexcolor.length == FULL_HEXCOLOR_LEN) { return hexcolor.toLowerCase() }   // color: full hexcolor
  if (! allowShorthand)                     { return null }                     // null: shorthand but we don't care
  return ('#' + rr + rr + gg + gg + bb + bb).toLowerCase()                      // color: shorthand and we do care
}

/**
 * Relative luminance per WCAG 2.x: linearize each channel, then weight by
 * the eye's sensitivity to it
 * @param {[number, number, number]} rgb Red, green, blue, each 0-255.
 * @return {number} Luminance, 0 (black) through 1 (white)
 */
function wcagLuminanceForRGB(rgb) {
  const linear = rgb.map(function(channel) {
    const scaled = channel / 255
    if (scaled <= SRGB_KNEE) { return scaled / SRGB_KNEE_DIVISOR }
    return Math.pow((scaled + 0.055) / 1.055, 2.4)
  })

  return linear.reduce(function(total, channel, ii) {
    return total + (channel * WCAG_CHANNEL_WEIGHTS[ii])
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
 * Splits a canonical hexcolor into its three 8-bit channel values
 * @param {string} hexcolor Canonical '#rrggbb' string.
 * @return {[number, number, number]} Red, green, blue, each 0-255
 */
function rgbForHexcolor(raw) {
  const hexcolor = normalizeHexcolor(raw)
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
   - (str) a string giving the hexcode
   - args (t0, t1, t2, cspace = 'rgb')
   - a triple and optional colorspace ([t0, t1, t2], cspace = 'rgb')
   - see CSPACE_NAME_RE for allowed colorspaces; default is 'rgb'
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

function getTripleFromArgs(args) {
  if (isArray(args[0]) && isArray(args[0][0])) { return getTripleFromArgs([...args[0][0], args[1] ?? 'rgb']) }
  if (isTriple(args[0])) { return getTripleFromArgs([...args[0], args[1] ?? 'rgb']) }
  const [t0, t1, t2, cspace = 'rgb'] = args
  if (! (isString(cspace)))          { return null }
  if (! CSPACE_NAME_RE.test(cspace)) { return null }
  if (! isTriple([t0, t1, t2]))      { return null }
  return [t0, t1, t2, cspace.toLowerCase()]
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
 * Linear-light LMS cone responses. Note these are raw LMS — oklabFor takes
 * the cube roots itself. Keep the split in mind if you ever write an inverse.
 * @param {...*} args As rgbFor.
 * @return {[number, number, number] | null} Long, medium, short
 */
function lmsFor(...args) {
  var [red255, grn255, blu255] = rgbFor(...args) ?? []
  if (! Number.isFinite(red255)) { return null }
  //
  const [sred, sgrn, sblu] = [linearForRGB(red255), linearForRGB(grn255), linearForRGB(blu255)]
  // Linear sRGB to LMS
  const lms_l = (0.4122214708 * sred + 0.5363325363 * sgrn + 0.0514459929 * sblu)
  const lms_m = (0.2119034982 * sred + 0.6806995451 * sgrn + 0.1073969566 * sblu)
  const lms_s = (0.0883024619 * sred + 0.2817188376 * sgrn + 0.6299787005 * sblu)
  //
  return [lms_l, lms_m, lms_s]
}

/**
 * Oklab in cylindrical form. Lightness is perceptual, not luminance;
 * chroma is absolute colorfulness, not saturation.
 * @param {...*} args As rgbFor.
 * @return {[number, number, number] | null} Lightness 0-1, chroma 0-0.4, hue 0-360
 */
function oklchFor(...args) {
  const [okl, oka, okb] = oklabFor(...args) ?? []
  if (! Number.isFinite(okl)) { return null }

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

  const wrapped   = ((hue % 360) + 360) % 360
  const satClamped = Math.min(Math.max(sat, 0), 1)
  const valClamped = Math.min(Math.max(val, 0), 1)

  const chroma  = valClamped * satClamped
  const sector  = wrapped / 60
  const second  = chroma * (1 - Math.abs((sector % 2) - 1))
  const lift    = valClamped - chroma

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
 * clamping instead would silently shift hue and lightness.
 * @param {number} okl Lightness 0-1.
 * @param {number} oka Green-red axis.
 * @param {number} okb Blue-yellow axis.
 * @return {[number, number, number] | null} Red, green, blue, each 0-255
 */
function rgbForOKLAB(okl, oka, okb) {
  const linear = linearForOklab(okl, oka, okb)
  if (! linear) { return null }
  //
  return linear.map((channel) => (clamp8bit(srgbForLinear(channel) * 255)))
}

function forceGamut(...args) {

  // const inGamut = linear.every((channel) => (
  //   (channel >= -GAMUT_TOLERANCE) && (channel <= 1 + GAMUT_TOLERANCE)
  // ))
  // if (! inGamut) { return null }
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


function linearForRGB(val) {
  return linearForSRGB(val / 255)
}
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

function hexcolorFor(...args) {
  if (args.length === 1 && isString(args[0])) { return normalizeHexcolor(args[0]) }
  const [red, grn, blu] = rgbFor(...args) ?? []
  if (! Number.isFinite(red)) { return null }
  //
  return '#' + hexpad(red) + hexpad(grn) + hexpad(blu)
}

function clamp8bit(val) {
  if (! Number.isFinite(val)) { return 0 }
  return Math.min(Math.max(Math.round(val), 0), 255)
}

function isTriple(arr) { return Array.isArray(arr) && (arr.length === 3) && arr.every(Number.isFinite) }
function isString(val) { return (typeof val === 'string') }
function isFinite(val) { return Number.isFinite(val) }
function isArray(val)  { return Array.isArray(val) }

function first(val)  { return (Array.isArray(val) && val.length >= 1) ? val[0] : null }
function second(val) { return (Array.isArray(val) && val.length >= 2) ? val[1] : null }
function third(val)  { return (Array.isArray(val) && val.length >= 3) ? val[2] : null }

function hexpad(val) { return Number.isFinite(val) ? clamp8bit(val).toString(16).padStart(2, '0') : '' }

const SAMPLEVALS = `#000000 #d93669 #da6bca #da8ac9 #dbc02d #dbc35a #46cedb #dc4833 #99dcb0 #dfa475 #e06f64`.split(/[^#0-9a-f]+/i)
function bobotest() {
  return SAMPLEVALS.map((hexcolor) => {
    const oklab = oklabFor(hexcolor)
    const oklch = oklchFor(hexcolor)
    const rgb   = rgbFor(hexcolor)
    const hsv   = hsvFor(hexcolor)
    const rgbOklab = rgbForOKLAB(oklab[0], oklab[1], oklab[2], 'oklab')
    return [
      inspectTriple(oklab), inspectTriple(oklch), inspectTriple(rgb), inspectTriple(hsv),
      inspectTriple(rgbOklab),
      inspectTriple(rgbForHSV(hsv[0], hsv[1], hsv[2])),
      inspectTriple(rgbForOKLAB(oklab[0], oklab[1], oklab[2])),
      inspectTriple(rgbForOKLCH(oklch[0], oklch[1], oklch[2])),
    ]
  })
}
function inspectTriple(triple) {
  return triple?.map((el) => roundTo(el, 0.01)).join(', ')
}
function roundTo(val, interval = 1) {
  if (! isFinite(val)) { return null }
  return Math.round(val / interval) * interval
}
function tryit(func) {
  try {  return func() } catch (err) { return String(err) }
}

// == [sheet-facing wrappers]

function RGB(...args)      { const triple = rgbFor(...args);   return triple ? [triple] : null }
function OKLAB(...args)    { const triple = oklabFor(...args); return triple ? [triple] : null }
function OKLCH(...args)    { const triple = oklchFor(...args); return triple ? [triple] : null }
function HSV(...args)      { const triple = hsvFor(...args);   return triple ? [triple] : null }
function HEXCOLOR(...args) { return hexcolorFor(...args) }
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

function MAX_CHROMA(okl, okh) { return maxChromaForOKLCH(okl, okh) }

function TOSTRING(val) { return String(val) + (isArray(val) ? 'arr' : (typeof val)) + (isFinite(val?.length) ? val.length : '') }
