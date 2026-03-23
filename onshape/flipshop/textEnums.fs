
export enum VerticalAlignment {
  annotation { "Name": "Top of the tallest letter" }
  MAX,
  annotation { "Name": "Top of the text, as rendered" }
  TOP_EXTENT,
  annotation { "Name": "Nominal cap height" }
  TOP_BASELINE,
  annotation { "Name": "Midline of the text, as rendered" }
  MIDDLE,
  annotation { "Name": "Baseline of the text" }
  BOTTOM_BASELINE,
  annotation { "Name": "Bottom of the text, as rendered" }
  BOTTOM_EXTENT,
  annotation { "Name": "Bottom of the lowest-hanging letter" }
  MIN,
}

export enum HorizontalAlignment {
  annotation { "Name": "Left of the text, including padding" }
  MIN,
  annotation { "Name": "Left of the text, as rendered" }
  LEFT,
  annotation { "Name": "Center of the text, as rendered" }
  CENTER,
  annotation { "Name": "Center of the text, including padding" }
  CENTER_NOMINAL,
  annotation { "Name": "Right of the text, as rendered" }
  RIGHT,
  annotation { "Name": "Right of the text, including padding" }
  MAX,
  // JUSTIFY is not supported yet
}

export enum ResizingPolicy {
  // Dimension-independent policies: can mix and match these

  /** Preserve original dimensions; no resizing. */
  annotation { "Name": "No resizing (preserve original)" }
  NONE,
  /** Shrink to fit the available space if larger, but never expand. Affects only this dimension (not proportional). */
  annotation { "Name": "Constrain maximum size (shrink only)" }
  LIMIT,
  /** Expand to fill the available space if smaller, but never shrink. Affects only this dimension (not proportional). */
  annotation { "Name": "Constrain minimum size (expand only)" }
  EMBIGGEN,
  /** Stretch to fill the bounds exactly, ignoring aspect ratio. */
  annotation { "Name": "Stretch to fill (ignore aspect ratio)" }
  FILL,
  /** Scale proportionally to match the scaling factor of the other dimension. Maintains aspect ratio (e.g., height follows width changes). */
  annotation { "Name": "Follow other dimension (maintain aspect ratio)" }
  FOLLOW,

  // Dimension-dependent policies: resizing1 must be undefined or equal to resizing0

  /** Resize proportionally to fit entirely within bounds, with at least one dimension fitting exactly. May shrink or expand. */
  annotation { "Name": "Grow/Shrink proportionally to fit inside bounds" }
  CONTAIN,
  /** Resize proportionally to fill bounds, with at least one dimension fitting exactly. May shrink or expand. */
  annotation { "Name": "Grow/Shrink proportionally to cover bounds completely" }
  COVER,
  /** Shrink proportionally to fit within bounds, but never expand. */
  annotation { "Name": "Shrink until both fit (but never enlarge)" }
  DOWNSCALE,
  /** Grow proportionally to fill bounds, but never shrink */
  annotation { "Name": "Grow until both fit (but never shrink)" }
  MAXIMIZE,

}

// // Pangram character array for use in LUT building (buildTextWidthTableFor, coming later).
// const pangram      = "how quickly daft jumping zebras vex";
// const pangramChars = [
//   "h", "o", "w", " ", "q", "u", "i", "c", "k", "l", "y", " ", "d", "a", "f", "t",
//   " ", "j", "u", "m", "p", "i", "n", "g", " ", "z", "e", "b", "r", "a", "s",
//   " ", "v", "e", "x",
//   "H", "O", "W", " ", "Q", "U", "I", "C", "K", "L", "Y", " ", "D", "A", "F", "T",
//   " ", "J", "U", "M", "P", "I", "N", "G", " ", "Z", "E", "B", "R", "A", "S",
//   " ", "V", "E", "X"
// ];

// // Printable ASCII from 0x20 (space) to 0x7e (tilde); 0x7f (DEL) has no glyph and is omitted.
// const asciiChars = [" ", "!", "\"", "#", "$", "%", "&", "'", "(", ")", "*", "+", ",", "-", ".", "/",
//                     "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ":", ";", "<", "=", ">", "?",
//                     "@", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O",
//                     "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "[", "\\", "]", "^", "_",
//                     "`", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o",
//                     "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "{", "|", "}", "~"];

export enum FontName {
  annotation { "Name": "Open Sans Regular (a good default)" }
  OPEN_SANS_REGULAR,
  annotation {"Name": "Open Sans Bold" }
  OPEN_SANS_BOLD,
  annotation {"Name": "Open Sans Italic" }
  OPEN_SANS_ITALIC,
  annotation { "Name": "Allerta (no bold/italic options)" }
  ALLERTA,
  annotation { "Name": "Allerta Stencil (no bold/italic options)" }
  ALLERTA_STENCIL,
  annotation { "Name": "Arimo (sans-serif font)" }
  ARIMO,
  annotation { "Name": "Arimo Bold" }
  ARIMO_BOLD,
  annotation { "Name": "Arimo Italic" }
  ARIMO_ITALIC,
  annotation { "Name": "Balthazar (no bold/italic options)" }
  BALTHAZAR,
  annotation { "Name": "Baumans (no bold/italic options)" }
  BAUMANS,
  annotation { "Name": "Bebas Neue (no bold/italic options)" }
  BEBAS_NEUE,
  annotation { "Name": "Comic Neue" }
  COMIC_NEUE,
  annotation { "Name": "Comic Neue Bold" }
  COMIC_NEUE_BOLD,
  annotation { "Name": "Comic Neue Italic" }
  COMIC_NEUE_ITALIC,
  annotation { "Name": "Courier Prime (monospaced)" }
  COURIER_PRIME,
  annotation { "Name": "Courier Prime Bold" }
  COURIER_PRIME_BOLD,
  annotation { "Name": "Courier Prime Italic" }
  COURIER_PRIME_ITALIC,
  annotation { "Name": "Didact Gothic (no bold/italic options)" }
  DIDACT_GOTHIC,
  annotation { "Name": "Droid Sans Mono (monospaced sans-serif font, no bold/italic options)" }
  DROID_SANS_MONO,
  annotation { "Name": "Inconsolata (monospaced sans-serif font)" }
  INCONSOLATA,
  annotation { "Name": "Inconsolata Bold" }
  INCONSOLATA_BOLD,
  annotation { "Name": "Inter (sans-serif font, no italic options)" }
  INTER,
  annotation { "Name": "Inter Bold" }
  INTER_BOLD,
  annotation { "Name": "Michroma (sans-serif font, no italic options)" }
  MICHROMA,
  annotation { "Name": "MPLUSRounded1c (sans-serif font, no italic options)" }
  MPLUSRounded1c,
  annotation { "Name": "MPLUSRounded1c Bold" }
  MPLUSRounded1c_BOLD,
  annotation { "Name": "Noto Sans (sans-serif font)" }
  NOTO_SANS,
  annotation { "Name": "Noto Sans Bold" }
  NOTO_SANS_BOLD,
  annotation { "Name": "Noto Sans Italic" }
  NOTO_SANS_ITALIC,
  annotation { "Name": "Noto Sans CJK JP (Japanese font, no italic options)" }
  NOTO_SANS_CJK_JP,
  annotation { "Name": "Noto Sans CJK JP Bold" }
  NOTO_SANS_CJK_JP_BOLD,
  annotation { "Name": "Noto Sans CJK KR (Korean font, no italic options)" }
  NOTO_SANS_CJK_KR,
  annotation { "Name": "Noto Sans CJK KR Bold" }
  NOTO_SANS_CJK_KR_BOLD,
  annotation { "Name": "Noto Sans CJK SC (Chinese (simplified) font, no italic options)" }
  NOTO_SANS_CJK_SC,
  annotation { "Name": "Noto Sans CJK SC Bold" }
  NOTO_SANS_CJK_SC_BOLD,
  annotation { "Name": "Noto Sans CJK TC (Chinese (traditional) font, no italic options)" }
  NOTO_SANS_CJK_TC,
  annotation { "Name": "Noto Sans CJK TC Bold" }
  NOTO_SANS_CJK_TC_BOLD,
  annotation { "Name": "Noto Serif (serif font)" }
  NOTO_SERIF,
  annotation { "Name": "Noto Serif Bold" }
  NOTO_SERIF_BOLD,
  annotation { "Name": "Noto Serif Italic" }
  NOTO_SERIF_ITALIC,
  annotation { "Name": "Orbitron (sans-serif font, no italic options)" }
  ORBITRON,
  annotation { "Name": "Orbitron Bold" }
  ORBITRON_BOLD,
  annotation { "Name": "Oswald (sans-serif font, no italic options)" }
  OSWALD,
  annotation { "Name": "Oswald Bold" }
  OSWALD_BOLD,
  annotation { "Name": "Poppins (sans-serif font)" }
  POPPINS,
  annotation { "Name": "Poppins Bold" }
  POPPINS_BOLD,
  annotation { "Name": "Poppins Italic" }
  POPPINS_ITALIC,
  annotation { "Name": "PTSans (sans-serif font)" }
  PTSANS,
  annotation { "Name": "PTSans Bold" }
  PTSANS_BOLD,
  annotation { "Name": "PTSans Italic" }
  PTSANS_ITALIC,
  annotation { "Name": "Rajdhani (sans-serif font, no italic options)" }
  RAJDHANI,
  annotation { "Name": "Rajdhani Bold" }
  RAJDHANI_BOLD,
  annotation { "Name": "Roboto (sans-serif font)" }
  ROBOTO,
  annotation { "Name": "Roboto Bold" }
  ROBOTO_BOLD,
  annotation { "Name": "Roboto Italic" }
  ROBOTO_ITALIC,
  annotation { "Name": "Roboto Slab (sans-serif font, no italic options)" }
  ROBOTO_SLAB,
  annotation { "Name": "Roboto Slab Bold" }
  ROBOTO_SLAB_BOLD,
  annotation { "Name": "Ropa Sans (sans-serif font, no bold options)" }
  ROPA_SANS,
  annotation { "Name": "Ropa Sans Italic" }
  ROPA_SANS_ITALIC,
  annotation { "Name": "Sofia Sans (sans-serif font)" }
  SOFIA_SANS,
  annotation { "Name": "Sofia Sans Bold" }
  SOFIA_SANS_BOLD,
  annotation { "Name": "Sofia Sans Italic" }
  SOFIA_SANS_ITALIC,
  annotation { "Name": "Source Sans Pro (sans-serif font)" }
  SOURCE_SANS_PRO,
  annotation { "Name": "Source Sans Pro Bold" }
  SOURCE_SANS_PRO_BOLD,
  annotation { "Name": "Source Sans Pro Italic" }
  SOURCE_SANS_PRO_ITALIC,
  annotation { "Name": "Tinos (serif font, metrically compatible with Times New Roman)" }
  TINOS,
  annotation { "Name": "Tinos Bold" }
  TINOS_BOLD,
  annotation { "Name": "Tinos Italic" }
  TINOS_ITALIC,
 }

 export const FontNameString = {
  FontName.OPEN_SANS_REGULAR:     "OpenSans-Regular.ttf",
  FontName.OPEN_SANS_BOLD:        "OpenSans-Bold.ttf",
  FontName.OPEN_SANS_ITALIC:      "OpenSans-Italic.ttf",
  FontName.ALLERTA:               "Allerta-Regular.ttf",
  FontName.ALLERTA_STENCIL:       "AllertaStencil-Regular.ttf",
  FontName.ARIMO:                 "Arimo-Regular.ttf",
  FontName.ARIMO_BOLD:            "Arimo-Bold.ttf",
  FontName.ARIMO_ITALIC:          "Arimo-Italic.ttf",
  FontName.BALTHAZAR:             "Balthazar-Regular.ttf",
  FontName.BAUMANS:               "Baumans-Regular.ttf",
  FontName.BEBAS_NEUE:            "BebasNeue-Regular.ttf",
  FontName.COMIC_NEUE:            "ComicNeue-Regular.ttf",
  FontName.COMIC_NEUE_BOLD:       "ComicNeue-Bold.ttf",
  FontName.COMIC_NEUE_ITALIC:     "ComicNeue-Italic.ttf",
  FontName.COURIER_PRIME:         "CourierPrime-Regular.ttf",
  FontName.COURIER_PRIME_BOLD:    "CourierPrime-Bold.ttf",
  FontName.COURIER_PRIME_ITALIC:  "CourierPrime-Italic.ttf",
  FontName.DIDACT_GOTHIC:         "DidactGothic-Regular.ttf",
  FontName.DROID_SANS_MONO:       "DroidSansMono.ttf",
  FontName.INCONSOLATA:           "Inconsolata-Regular.ttf",
  FontName.INCONSOLATA_BOLD:      "Inconsolata-Bold.ttf",
  FontName.INTER:                 "Inter-Regular.ttf",
  FontName.INTER_BOLD:            "Inter-Bold.ttf",
  FontName.MICHROMA:              "Michroma-Regular.ttf",
  FontName.MPLUSRounded1c:        "MPLUSRounded1c-Regular.ttf",
  FontName.MPLUSRounded1c_BOLD:   "MPLUSRounded1c-Bold.ttf",
  FontName.NOTO_SANS:             "NotoSans-Regular.ttf",
  FontName.NOTO_SANS_BOLD:        "NotoSans-Bold.ttf",
  FontName.NOTO_SANS_ITALIC:      "NotoSans-Italic.ttf",
  FontName.NOTO_SANS_CJK_JP:      "NotoSansCJKjp-Regular.otf",
  FontName.NOTO_SANS_CJK_JP_BOLD: "NotoSansCJKjp-Bold.otf",
  FontName.NOTO_SANS_CJK_KR:      "NotoSansCJKkr-Regular.otf",
  FontName.NOTO_SANS_CJK_KR_BOLD: "NotoSansCJKkr-Bold.otf",
  FontName.NOTO_SANS_CJK_SC:      "NotoSansCJKsc-Regular.otf",
  FontName.NOTO_SANS_CJK_SC_BOLD: "NotoSansCJKsc-Bold.otf",
  FontName.NOTO_SANS_CJK_TC:      "NotoSansCJKtc-Regular.otf",
  FontName.NOTO_SANS_CJK_TC_BOLD: "NotoSansCJKtc-Bold.otf",
  FontName.NOTO_SERIF:            "NotoSerif-Regular.ttf",
  FontName.NOTO_SERIF_BOLD:       "NotoSerif-Bold.ttf",
  FontName.NOTO_SERIF_ITALIC:     "NotoSerif-Italic.ttf",
  FontName.ORBITRON:              "Orbitron-Regular.ttf",
  FontName.ORBITRON_BOLD:         "Orbitron-Bold.ttf",
  FontName.OSWALD:                "Oswald-Regular.ttf",
  FontName.OSWALD_BOLD:           "Oswald-Bold.ttf",
  FontName.POPPINS:               "Poppins-Regular.ttf",
  FontName.POPPINS_BOLD:          "Poppins-Bold.ttf",
  FontName.POPPINS_ITALIC:        "Poppins-Italic.ttf",
  FontName.PTSANS:                "PTSans-Regular.ttf",
  FontName.PTSANS_BOLD:           "PTSans-Bold.ttf",
  FontName.PTSANS_ITALIC:         "PTSans-Italic.ttf",
  FontName.RAJDHANI:              "Rajdhani-Regular.ttf",
  FontName.RAJDHANI_BOLD:         "Rajdhani-Bold.ttf",
  FontName.ROBOTO:                "Roboto-Regular.ttf",
  FontName.ROBOTO_BOLD:           "Roboto-Bold.ttf",
  FontName.ROBOTO_ITALIC:         "Roboto-Italic.ttf",
  FontName.ROBOTO_SLAB:           "RobotoSlab-Regular.ttf",
  FontName.ROBOTO_SLAB_BOLD:      "RobotoSlab-Bold.ttf",
  FontName.ROPA_SANS:             "RopaSans-Regular.ttf",
  FontName.ROPA_SANS_ITALIC:      "RopaSans-Italic.ttf",
  FontName.SOFIA_SANS:            "SofiaSans-Regular.ttf",
  FontName.SOFIA_SANS_BOLD:       "SofiaSans-Bold.ttf",
  FontName.SOFIA_SANS_ITALIC:     "SofiaSans-Italic.ttf",
  FontName.SOURCE_SANS_PRO:       "SourceSansPro-Regular.ttf",
  FontName.SOURCE_SANS_PRO_BOLD:   "SourceSansPro-Bold.ttf",
  FontName.SOURCE_SANS_PRO_ITALIC: "SourceSansPro-Italic.ttf",
  FontName.TINOS:                 "Tinos-Regular.otf",
  FontName.TINOS_BOLD:            "Tinos-Bold.ttf",
  FontName.TINOS_ITALIC:          "Tinos-Italic.ttf",
 };
