/**
 * Hex cell painter
 *
 * Paints a cell's background with the hexcolor it contains.
 * Batched reads/writes, clears stale colors, keeps text readable.
 */

const CONFIG = {
  // Only run on sheets whose name matches this
  sheetNamePattern: /colors/i,

  // Which cell gets painted, relative to the cell holding the code.
  // { rows: 0, cols: 1 } paints the cell to the right instead.
  targetOffset: { rows: 0, cols: 0 },

  // Reset the background when a cell stops being a valid hexcolor
  clearNonMatches: true,

  // Flip font color to black/white so the text stays legible
  autoContrastText: true,

  // Accept #FFF and #AABBCC as well as strict lowercase
  allowShorthand: false,
  requireLowercase: true
}

// Colour maths — normalizeHexcolor, rgbForHexcolor, textColorForHexcolor and
// the conversion functions — lives in Color.gs. Apps Script shares one global
// scope across .gs files, so declaring any of it twice is a SyntaxError.

const DEFAULT_BACKGROUND = '#ffffff'   // what Sheets reports for an unstyled cell
const DEFAULT_FONT_COLOR = '#000000'


/**
 * Simple trigger. Fires on manual edits only.
 * @param {Object} event Apps Script edit event.
 */
function onEdit(event) {
  if (! (event && event.range))                        { return }
  const sheet = event.range.getSheet()
  if (! CONFIG.sheetNamePattern.test(sheet.getName())) { return }

  paintRange(event.range)
}

/**
 * Adds a manual escape hatch, since onEdit never sees formula
 * recalcs, IMPORTRANGE refreshes, or writes made by other scripts.
 */
function onOpen() {
  SpreadsheetApp.getUi()
    .createMenu('Hex tools')
    .addItem('Repaint this sheet', 'repaintActiveSheet')
    .addToUi()
}

function repaintActiveSheet() {
  const sheet = SpreadsheetApp.getActiveSheet()
  paintRange(sheet.getDataRange())
}

/**
 * Reads the range once, writes back at most twice.
 * @param {GoogleAppsScript.Spreadsheet.Range} range Source cells.
 */
function paintRange(range) {
  const vals        = range.getValues()
  const target      = range.offset(CONFIG.targetOffset.rows, CONFIG.targetOffset.cols)
  const backgrounds = target.getBackgrounds()
  const fontColors  = target.getFontColors()

  let backgroundsDirty = false
  let fontColorsDirty  = false

  for (let ii = 0; ii < vals.length; ii++) {
    for (let jj = 0; jj < vals[ii].length; jj++) {
      const hexcolor = normalizeHexcolor(vals[ii][jj], CONFIG)

      let nextBackground = backgrounds[ii][jj]
      let nextFontColor  = fontColors[ii][jj]

      if (hexcolor) {
        nextBackground = hexcolor
        if (CONFIG.autoContrastText) { nextFontColor = textColorForHexcolor(hexcolor) }
      } else if (CONFIG.clearNonMatches) {
        // Only reset what isn't already default — an unstyled cell reads back
        // as #ffffff/#000000, and clearing it would be a write for nothing
        if (backgrounds[ii][jj] != DEFAULT_BACKGROUND)                          { nextBackground = null }
        if (CONFIG.autoContrastText && fontColors[ii][jj] != DEFAULT_FONT_COLOR) { nextFontColor = null }
      }

      if (nextBackground !== backgrounds[ii][jj]) {
        backgrounds[ii][jj] = nextBackground
        backgroundsDirty    = true
      }
      if (nextFontColor !== fontColors[ii][jj]) {
        fontColors[ii][jj] = nextFontColor
        fontColorsDirty    = true
      }
    }
  }

  if (backgroundsDirty) { target.setBackgrounds(backgrounds) }
  if (fontColorsDirty)  { target.setFontColors(fontColors) }
}