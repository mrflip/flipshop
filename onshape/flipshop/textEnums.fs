FeatureScript 2909;
import(path : "onshape/std/common.fs", version : "2909.0");

export enum VerticalAlignment {
  annotation { "Name": "Top of tallest renderable (l-height)" }
  MAX,
  annotation { "Name": "Top, as rendered" }
  TOP_EXTENT,
  annotation { "Name": "Nominal cap height" }
  TOP_BASELINE,
  annotation { "Name": "Midline, as rendered" }
  MIDDLE,
  annotation { "Name": "Baseline" }
  BOTTOM_BASELINE,
  annotation { "Name": "Bottom, as rendered" }
  BOTTOM_EXTENT,
  annotation { "Name": "Bottom of the lowest renderable (y-height)" }
  MIN,
}

export enum HorizontalAlignment {
  annotation { "Name": "Left, including padding" }
  MIN,
  annotation { "Name": "Left, as rendered" }
  LEFT,
  annotation { "Name": "Center, as rendered" }
  CENTER,
  annotation { "Name": "Center, including padding" }
  CENTER_NOMINAL,
  annotation { "Name": "Right, as rendered" }
  RIGHT,
  annotation { "Name": "Right, including padding" }
  MAX
  // JUSTIFY is not supported yet
}

export enum HSizingExtent {
  annotation { "Name": "Full width, including padding" }
  PADDED,
  annotation { "Name": "Real width (no padding)" }
  REAL
}

export enum VSizingExtent {
  /** Size will be the same regardless of the text content. */
  annotation { "Name": "Lowest to tallest renderable (y-height to l-height)" }
  MIN_MAX,
  /** Size will be the same regardless of the text content. */
  annotation { "Name": "Baseline to tallest renderable (l-height)" }
  BASELINE_MAX,
  /** Size will be the same regardless of the text content. */
  annotation { "Name": "Baseline-to-cap height" }
  NOMINAL,
  /** Size (and perhaps position) will vary on content: `axe` vs `Y0l0!` vs `eggy;` vs `jiggy` vs 'yolo!' */
  annotation { "Name": "Actual rendered height" }
  ACTUAL
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
  MAXIMIZE
}

export enum FontName {
  annotation { "Name": "Open Sans Regular" }
  OPEN_SANS_REGULAR,
  annotation {"Name": "Open Sans Bold" }
  OPEN_SANS_BOLD,
  annotation {"Name": "Open Sans Italic" }
  OPEN_SANS_ITALIC,
  annotation { "Name": "Allerta" }
  ALLERTA,
  annotation { "Name": "Allerta Stencil" }
  ALLERTA_STENCIL,
  annotation { "Name": "Arimo (sans-serif)" }
  ARIMO,
  annotation { "Name": "Arimo Bold" }
  ARIMO_BOLD,
  annotation { "Name": "Arimo Italic" }
  ARIMO_ITALIC,
  annotation { "Name": "Balthazar (classical serif)" }
  BALTHAZAR,
  annotation { "Name": "Baumans (cheeky sans)" }
  BAUMANS,
  annotation { "Name": "Bebas Neue (compact ss caps)" }
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
  annotation { "Name": "Didact Gothic" }
  DIDACT_GOTHIC,
  annotation { "Name": "Droid Sans Mono (monospaced sans-serif)" }
  DROID_SANS_MONO,
  annotation { "Name": "Inconsolata (monospaced sans-serif)" }
  INCONSOLATA,
  annotation { "Name": "Inconsolata Bold" }
  INCONSOLATA_BOLD,
  annotation { "Name": "Inter (sans-serif)" }
  INTER,
  annotation { "Name": "Inter Bold" }
  INTER_BOLD,
  annotation { "Name": "Michroma (sans-serif)" }
  MICHROMA,
  annotation { "Name": "MPLUSRounded1c (sans-serif)" }
  MPLUSRounded1c,
  annotation { "Name": "MPLUSRounded1c Bold" }
  MPLUSRounded1c_BOLD,
  annotation { "Name": "Noto Sans (sans-serif)" }
  NOTO_SANS,
  annotation { "Name": "Noto Sans Bold" }
  NOTO_SANS_BOLD,
  annotation { "Name": "Noto Sans Italic" }
  NOTO_SANS_ITALIC,
  annotation { "Name": "Noto Sans CJK JP (Japanese)" }
  NOTO_SANS_CJK_JP,
  annotation { "Name": "Noto Sans CJK JP (Japanese) Bold" }
  NOTO_SANS_CJK_JP_BOLD,
  annotation { "Name": "Noto Sans CJK KR (Korean)" }
  NOTO_SANS_CJK_KR,
  annotation { "Name": "Noto Sans CJK KR (Korean) Bold" }
  NOTO_SANS_CJK_KR_BOLD,
  annotation { "Name": "Noto Sans CJK SC (Chinese simplified)" }
  NOTO_SANS_CJK_SC,
  annotation { "Name": "Noto Sans CJK SC Bold" }
  NOTO_SANS_CJK_SC_BOLD,
  annotation { "Name": "Noto Sans CJK TC (Chinese traditional)" }
  NOTO_SANS_CJK_TC,
  annotation { "Name": "Noto Sans CJK TC Bold" }
  NOTO_SANS_CJK_TC_BOLD,
  annotation { "Name": "Noto Serif (serif)" }
  NOTO_SERIF,
  annotation { "Name": "Noto Serif Bold" }
  NOTO_SERIF_BOLD,
  annotation { "Name": "Noto Serif Italic" }
  NOTO_SERIF_ITALIC,
  annotation { "Name": "Orbitron (tron sans-serif)" }
  ORBITRON,
  annotation { "Name": "Orbitron Bold" }
  ORBITRON_BOLD,
  annotation { "Name": "Oswald (skinny sans-serif)" }
  OSWALD,
  annotation { "Name": "Oswald Bold" }
  OSWALD_BOLD,
  annotation { "Name": "Poppins (sans-serif)" }
  POPPINS,
  annotation { "Name": "Poppins Bold" }
  POPPINS_BOLD,
  annotation { "Name": "Poppins Italic" }
  POPPINS_ITALIC,
  annotation { "Name": "PTSans (sans-serif)" }
  PTSANS,
  annotation { "Name": "PTSans Bold" }
  PTSANS_BOLD,
  annotation { "Name": "PTSans Italic" }
  PTSANS_ITALIC,
  annotation { "Name": "Rajdhani (sans-serif)" }
  RAJDHANI,
  annotation { "Name": "Rajdhani Bold" }
  RAJDHANI_BOLD,
  annotation { "Name": "Roboto (sans-serif)" }
  ROBOTO,
  annotation { "Name": "Roboto Bold" }
  ROBOTO_BOLD,
  annotation { "Name": "Roboto Italic" }
  ROBOTO_ITALIC,
  annotation { "Name": "Roboto Slab (sans-serif)" }
  ROBOTO_SLAB,
  annotation { "Name": "Roboto Slab Bold" }
  ROBOTO_SLAB_BOLD,
  annotation { "Name": "Ropa Sans (sans-serif)" }
  ROPA_SANS,
  annotation { "Name": "Ropa Sans Italic" }
  ROPA_SANS_ITALIC,
  annotation { "Name": "Sofia Sans (sans-serif)" }
  SOFIA_SANS,
  annotation { "Name": "Sofia Sans Bold" }
  SOFIA_SANS_BOLD,
  annotation { "Name": "Sofia Sans Italic" }
  SOFIA_SANS_ITALIC,
  annotation { "Name": "Source Sans Pro (sans-serif)" }
  SOURCE_SANS_PRO,
  annotation { "Name": "Source Sans Pro Bold" }
  SOURCE_SANS_PRO_BOLD,
  annotation { "Name": "Source Sans Pro Italic" }
  SOURCE_SANS_PRO_ITALIC,
  annotation { "Name": "Tinos (serif)" }
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
  FontName.TINOS:                 "Tinos-Regular.ttf",
  FontName.TINOS_BOLD:            "Tinos-Bold.ttf",
  FontName.TINOS_ITALIC:          "Tinos-Italic.ttf",
 };

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

/** Printable ASCII from 0x20 (space) to 0x7e (tilde) */
export const asciiChars = [
  " ", "!", "\"", "#", "$", "%", "&", "'", "(", ")", "*", "+", ",", "-", ".", "/",
  "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ":", ";", "<", "=", ">", "?",
  "@", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O",
  "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "[", "\\", "]", "^", "_",
  "`", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o",
  "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "{", "|", "}", "~",        // 0x7f (DEL) has no glyph and is omitted.
];
