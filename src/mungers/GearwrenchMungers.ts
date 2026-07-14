#! /usr/bin/env yarn node
import      _                                /**/ from 'lodash'
import      { load as cheerioLoad }               from 'cheerio'
//
import type * as TY                               from '@freeword/meta'
import      { CK }                                from '@freeword/meta'
import      * as Fastener                         from '../fastener/index.ts'
import      * as Sockets                          from '../sockets/index.ts'
import       { DistanceLookup }                   from './DistanceLookup.ts'
import       { DriveToDriven }                    from '../sockets/DriverTargets.ts'
import type * as FE                               from '../fastener/FastenerEnums.ts'

const { MM_IN, KG_LB } = Fastener

// == [Types] ==

export const gearwrenchSocket = CK.obj({
  ...Sockets.socketWrench.shape,
  gwtitle:              CK.titleish,
  is_knurled:           CK.bool.optional(),
  is_magnetic:          CK.bool.optional(),
  is_wobble:            CK.bool.optional(),
  is_locking:           CK.bool.optional(),
  is_quickrel:          CK.bool.optional(),
  is_prop65:            CK.bool.optional(),
  is_hiviz:             CK.bool.optional(),
  material:             CK.oneof([ 'Alloy Steel', 'Alloy Steel with S2 Steel Bit', 'Chrome-Molybdenum (Cr-Mo)']),
  surf_finish:          CK.oneof([ 'Full Polish Chrome', 'Full Polish Chrome Holder with Black Oxide Bit', 'Black Oxide', 'Manganese Phosphate', 'Industrial Black Finish']),
  ansi_stdz:            CK.oneof([ 'Meets or Exceeds' ]).optional(),
  asme_stdz:            CK.oneof([ 'B107.1', 'B107.5M', 'Meets or Exceeds', 'B107.34', 'B107.1 B107.5M', 'B107.2', 'B107.33M', 'B107.110-2012', 'B107.33']).optional(),
  usfed_stdz:           CK.oneof([ 'GGG-W-641E' ]).optional(),
}).strict()
export interface GearwrenchSocketT  extends CK.Zcasted<typeof gearwrenchSocket> {}
export interface GearwrenchSocketSk extends CK.Zsketch<typeof gearwrenchSocket> {}
// --

// == [Remaps] ==

export const sqdrive_size_remap: TY.Bag<Fastener.FastenerEnums.ToolDrive> = { '1/4 in': 'isq_0250in', '3/8 in': 'isq_0375in', '1/2 in': 'isq_0500in', '3/4 in': 'isq_0750in', '1 in': 'isq_1000in' } as const
export const drive_kind_remap: TY.Bag<Fastener.FastenerEnums.FastenerDrive> = {
  'Hex': 'inthex', 'Torx®': 'torx', 'Tamper Proof Torx®': 'torxtp', 'External Torx®': 'extstar', 'Ballpoint Hex': 'inthex', 'Triple Square': 'triple_square', 'Slotted': 'slotted', 'Phillips®': 'phillips', 'Pozidriv®': 'pozidriv',
  '6 Point': 'exthex',  '6 Point 6 Point': 'exthex', '12 Point': 'extstar12',
  'Ball Hex': 'inthex', 'Slotted Phillips®/Slotted/Pozidriv®': 'phillips', 'Phillips® Phillips®/Slotted/Pozidriv®': 'phillips', 'Pozidriv® Phillips®/Slotted/Pozidriv®': 'pozidriv',
  'Square Square': 'square', 'Square': 'square',
} as const
export const socket_kind_remap: TY.Bag<Fastener.FastenerEnums.SocketKind> = {
  'Spark Plug Socket Spark Plug Service Tools': 'socket_sparkplug', 'Socket': 'socket_exthex', 'Bit Socket': 'socket_bit', 'Flex Socket': 'socket_exthex', 'Socket Spark Plug Service Tools': 'socket_sparkplug', 'Extension': 'socket_extension',
  'Universal Joint': 'socket_ujoint', 'Universal Joint Socket': 'socket_exthex', 'Socket Extension': 'socket_exthex', 'Adapter': 'socket_adapter',
  // 'Extension Socket': 'socket_exthex',
} as const
export const reach_kind_remap: TY.Bag<Fastener.FastenerEnums.SocketReach> = {
  'Standard': 'reg', 'Mid Length': 'midlen', 'Deep': 'deep', 'Long': 'long', 'Extra Long': 'xlong',
} as const
export const unit_system_remap: TY.Bag<Fastener.FastenerEnums.UnitSystem> = {
  'SAE': 'us',
  'Metric': 'metric', 'Metric Metric': 'metric',
  'Phillips®': 'metric', 'Pozidriv®': 'metric',
  'Torx': 'metric', 'SAE SAE/Metric': 'us', 'Metric SAE/Metric': 'metric', 'Torx®': 'metric', 'Tamper Proof Torx®': 'metric', 'External Torx®': 'metric',
  'SAE SAE': 'us',
} as const

export const fieldname_remap = {
  "Size":               'sizing',
  "UPC":                'upc',
  //
  "Type":               'socket_kind',
  "Drive Tang Size":    'sqdrive_size',
  "Drive Type":         'drive_kind',
  "Bit Type":           'bit_kind',
  "Length Format":      'reach_kind',
  "SAE/Metric/Torx":    'unit_system',
  //
  "Overall Length":     'ln_overall',
  "Overall Width":      'wx_overall',
  "Overall Height":     'wy_overall',
  //
  "Wrench Depth":       'target_dp',
  "Bolt Depth":         'target_dp',
  "Bit Length":         'bit_ln',
  "Exposed Bit Length": 'bit_ln_exposed',
  "Nose Diameter":      'nose_diam',
  "Drive End":          'ratchet_end_diam',
  "Bolt Clearance":     'bolt_clr_diam',
  "Length to Shoulder": 'shoulder_ln',
  "Wrench End":         'target_end_diam',
  "Drive End Hex Across Flats": 'drive_end_hex_af', // data is inconsistent
  //
  "Male Drive Size":    'male_drive_size',
  "Female Drive Size":  'female_drive_size',
  //
  "Weight (Catalog)":   'wt_lb',
  "Hi-Viz®":            'is_hiviz',
  "Material":           'material',
  "Finish":             'surf_finish',
  "Knurled":            'is_knurled',
  "Magnetic":           'is_magnetic',
  "Wobble":             'is_wobble',
  "Locking":            'is_locking',
  "Quick Release":      'is_quickrel',
  "Prop 65":            'is_prop65',
  //
  "Packaging":           null,
  "Warranty":            null,
  "Size Range (Metric)": null,
  "Size Range (SAE)":    null,
  "Family Name":         null,
  "ANSI Specification": 'ansi_stdz',
  "ASME Specification": 'asme_stdz',
  "US Federal Specification": 'usfed_stdz',
}
// --

const SizeOverrides = {
  '7/16in 6-Point US 3/8Dr Regular': { "ratchet_end_diam": 18.623 },
  '7/16in 6-Point US 3/8Dr Deep':    { "ratchet_end_diam": 18.623 },
  '1/2in 6-Point US 3/8Dr Regular':  { "ratchet_end_diam": 18.923 },
  '1/2in 6-Point US 3/8Dr Deep':     { "ratchet_end_diam": 18.923 },
  '3/4in 6-Point US 3/8Dr Regular':  { "ratchet_end_diam": 25.708 },
  '3/4in 6-Point US 3/8Dr Deep':     { "ratchet_end_diam": 25.708 },
}

export const AdditonalSizes = {
  "4mm 6-Point MM 1/4Dr U-Joint Regular":       { from:     "4mm 6-Point MM 1/4Dr Regular",          dir: /chrome-sockets/,   title:      "4mm 6-Point MM 1/4Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:        "4mm", sizing_mm:  4.0,     sizing_in: 0.1574803, */ /* ln_overall: 24.511,  wx_overall: 11.811,  wy_overall: 11.811,  target_end_diam: 6.7056,   ratchet_end_diam: 11.811,  shoulder_ln: 11.5062, target_dp: 8.001,     bolt_clr_diam: 3.4036, targets: { drives:"M2",      exthex_sz:"M2",      hhcs_sz:"M2",    hn_sz:"NM2",    hhn_sz: "HNM2"     } */ },
  '4.5mm 6-Point MM 1/4Dr Deep':                { from:     "4mm 6-Point MM 1/4Dr Deep",             dir: /impact-products/,  title:    "4.5mm 6-Point MM 1/4Dr Deep",             reach_kind: "deep",   socket_variant: "std",       sizing:      "4.5mm", sizing_mm: 4.5,      sizing_in: 0.1771654,    ln_overall: 48.9966, wx_overall: 11.9126, wy_overall: 11.9126, target_end_diam:  7.4,     ratchet_end_diam: 11.9126, shoulder_ln: 21.9964, target_dp: 3.5,       bolt_clr_diam: 4.0,    targets: {                                                                                            } },
  '4.5mm 6-Point MM 1/4Dr Regular':             { from:     "4mm 6-Point MM 1/4Dr Regular",          dir: /impact-products/,  title:    "4.5mm 6-Point MM 1/4Dr Regular",          reach_kind: "reg",    socket_variant: "std",       sizing:      "4.5mm", sizing_mm: 4.5,      sizing_in: 0.1771654,    ln_overall: 24.511,  wx_overall: 11.811,  wy_overall: 11.811,  target_end_diam:  7.4,     ratchet_end_diam: 11.811,  shoulder_ln: 11.5062, target_dp: 8.001,     bolt_clr_diam: 3.9,    targets: {                                                                                            } },
  // '4.5mm 6-Point MM 1/4Dr Regular Impact':   { from:     "4mm 6-Point MM 1/4Dr Regular",          dir: /impact-products/,  title:    "4.5mm 6-Point MM 1/4Dr Regular Impact",   reach_kind: "reg",    socket_variant: "impact",    sizing:      "4.5mm", sizing_mm: 4.5,      sizing_in: 0.1771654,    ln_overall: 24.511,  wx_overall: 11.811,  wy_overall: 11.811,  target_end_diam:  7.4,     ratchet_end_diam: 11.811,  shoulder_ln: 11.5062, target_dp: 8.001,     bolt_clr_diam: 3.9,    targets: {                                                                                            } },
  "4.5mm 6-Point MM 1/4Dr U-Joint Regular":     { from:     "4mm 6-Point MM 1/4Dr Regular",          dir: /chrome-sockets/,   title:    "4.5mm 6-Point MM 1/4Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",       sizing:      "4.5mm", sizing_mm: 4.5,      sizing_in: 0.1771654,    ln_overall: 24.511,  wx_overall: 11.811,  wy_overall: 11.811,  target_end_diam: 7.4,      ratchet_end_diam: 11.811,  shoulder_ln: 11.5062, target_dp: 8.001,     bolt_clr_diam: 3.9878, targets: {                                                                                            } },
  "6mm 6-Point MM 3/8Dr U-Joint Regular":       { from:     "6mm 6-Point MM 3/8Dr Regular",          dir: /chrome-sockets/,   title:      "6mm 6-Point MM 3/8Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:        "6mm", sizing_mm:  6.0,     sizing_in: 0.2362205, */ /* ln_overall: 45.0088, wx_overall: 16.9926, wy_overall: 16.9926, target_end_diam: 9.2964,   ratchet_end_diam: 16.9926, shoulder_ln: 11.5062, target_dp: 7.493,     bolt_clr_diam: 5.0038, targets: {                                                                                            } */ },
  "6mm 6-Point MM 3/8Dr Regular Impact":        { from:     "6mm 6-Point MM 3/8Dr Regular",          dir: /chrome-sockets/,   title:      "6mm 6-Point MM 3/8Dr Regular Impact",   reach_kind: "reg",    socket_variant: "impact" },
  "7mm 6-Point MM 3/8Dr Regular Impact":        { from:     "7mm 6-Point MM 3/8Dr Regular",          dir: /chrome-sockets/,   title:      "7mm 6-Point MM 3/8Dr Regular Impact",   reach_kind: "reg",    socket_variant: "impact" },
  "7mm 6-Point MM 3/8Dr U-Joint Regular":       { from:     "7mm 6-Point MM 3/8Dr Regular",          dir: /chrome-sockets/,   title:      "7mm 6-Point MM 3/8Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:        "7mm", sizing_mm:  7.0,     sizing_in: 0.2755906, */ /* ln_overall: 45.0088, wx_overall: 16.9926, wy_overall: 16.9926, target_end_diam: 10.795,   ratchet_end_diam: 16.9926, shoulder_ln: 11.5062, target_dp: 7.493,     bolt_clr_diam: 5.0038, targets: { drives:"M4",      exthex_sz:"M4",      hhcs_sz:"M4",    hn_sz:"NM4",    hhn_sz: "HNM4"     } */ },
  "8mm 6-Point MM 3/8Dr U-Joint Regular":       { from:     "8mm 6-Point MM 3/8Dr Regular",          dir: /chrome-sockets/,   title:      "8mm 6-Point MM 3/8Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:        "8mm", sizing_mm:  8.0,     sizing_in: 0.3149606, */ /* ln_overall: 45.0088, wx_overall: 16.9926, wy_overall: 16.9926, target_end_diam: 11.9888,  ratchet_end_diam: 16.9926, shoulder_ln: 11.5062, target_dp: 7.493,     bolt_clr_diam: 7.1882, targets: { drives:"M5",      exthex_sz:"M5",      hhcs_sz:"M5",    hn_sz:"NM5",    hhn_sz: "HNM5"     } */ },
  "9mm 6-Point MM 3/8Dr U-Joint Regular":       { from:     "9mm 6-Point MM 3/8Dr Regular",          dir: /chrome-sockets/,   title:      "9mm 6-Point MM 3/8Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:        "9mm", sizing_mm:  9.0,     sizing_in: 0.3543307, */ /* ln_overall: 45.0088, wx_overall: 16.9926, wy_overall: 16.9926, target_end_diam: 12.9032,  ratchet_end_diam: 16.9926, shoulder_ln: 11.5062, target_dp: 7.493,     bolt_clr_diam: 7.1882, targets: {                                                                                            } */ },
  "20mm 6-Point MM 3/8Dr U-Joint Regular":      { from:    "20mm 6-Point MM 3/8Dr Regular",          dir: /chrome-sockets/,   title:     "20mm 6-Point MM 3/8Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:       "20mm", sizing_mm: 20.0,     sizing_in: 0.7874016, */ /* ln_overall: 27.9908, wx_overall: 27.0002, wy_overall: 27.0002, target_end_diam: 27.0002,  ratchet_end_diam: 25.8064, shoulder_ln: 11.5062, target_dp: 15.0114,   bolt_clr_diam: 8.9916, targets: {                                                                                            } */ },
  "21mm 6-Point MM 3/8Dr U-Joint Regular":      { from:    "21mm 6-Point MM 3/8Dr Regular",          dir: /chrome-sockets/,   title:     "21mm 6-Point MM 3/8Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:       "21mm", sizing_mm: 21.0,     sizing_in: 0.8267717, */ /* ln_overall: 27.9908, wx_overall: 27.8892, wy_overall: 27.8892, target_end_diam: 27.8892,  ratchet_end_diam: 26.6954, shoulder_ln: 11.5062, target_dp: 15.0114,   bolt_clr_diam: 8.9916, targets: { drives:"D14",     exthex_sz:"D14",     hhcs_sz:"D14",   hn_sz:"ND14",   hhn_sz: "HND14"    } */ },
  "22mm 6-Point MM 3/8Dr U-Joint Regular":      { from:    "22mm 6-Point MM 3/8Dr Regular",          dir: /chrome-sockets/,   title:     "22mm 6-Point MM 3/8Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:       "22mm", sizing_mm: 22.0,     sizing_in: 0.8661417, */ /* ln_overall: 27.9908, wx_overall: 29.4894, wy_overall: 29.4894, target_end_diam: 29.4894,  ratchet_end_diam: 28.2956, shoulder_ln: 11.5062, target_dp: 15.0114,   bolt_clr_diam: 8.9916, targets: { drives:"M14",     exthex_sz:"M14",     hhcs_sz:"M14",   hn_sz:"NM14",   hhn_sz: "HNM14"    } */ },
  "24mm 6-Point MM 3/8Dr Regular":              { from:    "24mm 6-Point MM 3/8Dr Deep",             dir: /chrome-sockets/,   title:     "24mm 6-Point MM 3/8Dr Regular",          reach_kind: 'reg',    socket_variant: "std",    /* sizing:       "24mm", sizing_mm: 24.0,     sizing_in: 0.9448819, */ ln_overall: 27.9908, wx_overall: 31.8008, wy_overall: 31.8008, target_end_diam: 31.8008,  ratchet_end_diam: 30.607,  shoulder_ln: 11.5062, target_dp: 15.0114,   bolt_clr_diam: 8.509,  targets: { drives:  "M16",   exthex_sz:"M16",     hhcs_sz:"M16",   hn_sz:"NM16",   hhn_sz:"HNM16"     } },
  "24mm 6-Point MM 3/8Dr Regular Impact":       { from:    "24mm 6-Point MM 3/8Dr Deep",             dir: /chrome-sockets/,   title:     "24mm 6-Point MM 3/8Dr Regular Impact",   reach_kind: 'reg',    socket_variant: "impact", /* sizing:       "24mm", sizing_mm: 24.0,     sizing_in: 0.9448819, */ ln_overall: 27.9908, wx_overall: 31.8008, wy_overall: 31.8008, target_end_diam: 31.8008,  ratchet_end_diam: 30.607,  shoulder_ln: 11.5062, target_dp: 15.0114,   bolt_clr_diam: 8.509,  targets: { drives:  "M16",   exthex_sz:"M16",     hhcs_sz:"M16",   hn_sz:"NM16",   hhn_sz:"HNM16"     } },
  "24mm 6-Point MM 3/8Dr U-Joint Regular":      { from:    "24mm 6-Point MM 3/8Dr Deep",             dir: /chrome-sockets/,   title:     "24mm 6-Point MM 3/8Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:       "24mm", sizing_mm: 24.0,     sizing_in: 0.9448819, */ ln_overall: 27.9908, wx_overall: 31.8008, wy_overall: 31.8008, target_end_diam: 31.8008,  ratchet_end_diam: 30.607,  shoulder_ln: 11.5062, target_dp: 15.0114,   bolt_clr_diam: 8.509,  targets: { drives:  "M16",   exthex_sz:"M16",     hhcs_sz:"M16",   hn_sz:"NM16",   hhn_sz:"HNM16"     } },

  "5/32in 6-Point US 1/4Dr U-Joint Regular":    { from:  "5/32in 6-Point US 1/4Dr Regular",          dir: /chrome-sockets/,   title:   "5/32in 6-Point US 1/4Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:     "5/32in", sizing_mm: 3.9688,   sizing_in: 0.156252,  */ /* ln_overall: 24.511,  wx_overall: 11.811,  wy_overall: 11.811,  target_end_diam: 6.9088,   ratchet_end_diam: 11.811,  shoulder_ln: 11.5062, target_dp: 7.493,     bolt_clr_diam: 3.4036, targets: {                                                                                            } */ },
  "7/32in 6-Point US 1/4Dr U-Joint Regular":    { from:  "7/32in 6-Point US 1/4Dr Regular",          dir: /chrome-sockets/,   title:   "7/32in 6-Point US 1/4Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:     "7/32in", sizing_mm: 5.5562,   sizing_in: 0.218748,  */ /* ln_overall: 24.511,  wx_overall: 11.811,  wy_overall: 11.811,  target_end_diam: 8.509,    ratchet_end_diam: 11.811,  shoulder_ln: 11.5062, target_dp: 8.001,     bolt_clr_diam: 3.9878, targets: {                                                                                            } */ },
  "9/32in 6-Point US 1/4Dr U-Joint Regular":    { from:  "9/32in 6-Point US 1/4Dr Regular",          dir: /chrome-sockets/,   title:   "9/32in 6-Point US 1/4Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:     "9/32in", sizing_mm: 7.1438,   sizing_in: 0.281252,  */ /* ln_overall: 24.511,  wx_overall: 11.811,  wy_overall: 11.811,  target_end_diam: 10.6934,  ratchet_end_diam: 11.811,  shoulder_ln: 11.5062, target_dp: 10.0076,   bolt_clr_diam: 5.588,  targets: {                                                                                            } */ },
  "11/32in 6-Point US 1/4Dr U-Joint Regular":   { from: "11/32in 6-Point US 1/4Dr Regular",          dir: /chrome-sockets/,   title:  "11/32in 6-Point US 1/4Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:    "11/32in", sizing_mm: 8.7313,   sizing_in: 0.343752,  */ /* ln_overall: 24.511,  wx_overall: 12.4968, wy_overall: 12.4968, target_end_diam: 12.4968,  ratchet_end_diam: 12.4968, shoulder_ln: 11.5062, target_dp: 10.4902,   bolt_clr_diam: 5.588,  targets: { drives:"#8",      exthex_sz:"#8",      hhcs_sz:"#8",    hn_sz:"N#8"                        } */ },

  "9/32in 6-Point US 3/8Dr Regular":            { from:   "1/4in 6-Point US 3/8Dr Regular",          dir: /chrome-sockets/,   title:   "9/32in 6-Point US 3/8Dr Regular",          reach_kind: "reg",    socket_variant: "std",       sizing:     "9/32in", sizing_mm: 7.14375,  sizing_in: 0.28125,      ln_overall: 24.9936, wx_overall: 17.1958, wy_overall: 17.1958, target_end_diam: 10.85,    ratchet_end_diam: 17.1958, shoulder_ln: 11.5062, target_dp: 7.493,     bolt_clr_diam: 5.7,    targets: {                                                                                            } },
  "11/32in 6-Point US 3/8Dr Regular":           { from:  "5/16in 6-Point US 3/8Dr Regular",          dir: /chrome-sockets/,   title:  "11/32in 6-Point US 3/8Dr Regular",          reach_kind: "reg",    socket_variant: "std",       sizing:    "11/32in", sizing_mm: 8.73125,  sizing_in: 0.34375,      ln_overall: 24.9936, wx_overall: 17.1958, wy_overall: 17.1958, target_end_diam: 12.75,    ratchet_end_diam: 17.1958, shoulder_ln: 11.5062, target_dp: 7.493,     bolt_clr_diam: 8.2,    targets: { drives:  "#8",    exthex_sz: "#8",     hhcs_sz:  "#8",  hn_sz: "N#8"                       } },
  "9/32in 6-Point US 3/8Dr Deep":               { from:   "1/4in 6-Point US 3/8Dr Deep",             dir: /chrome-sockets/,   title:   "9/32in 6-Point US 3/8Dr Deep",             reach_kind: "deep",   socket_variant: "std",       sizing:     "9/32in", sizing_mm: 7.14375,  sizing_in: 0.28125,      ln_overall: 63.5,    wx_overall: 16.7894, wy_overall: 16.7894, target_end_diam: 10.85,    ratchet_end_diam: 16.7894, shoulder_ln: 29.0068, target_dp: 3.9878,    bolt_clr_diam: 6.3,    targets: {                                                                                            } },
  "11/32in 6-Point US 3/8Dr Deep":              { from:  "5/16in 6-Point US 3/8Dr Deep",             dir: /chrome-sockets/,   title:  "11/32in 6-Point US 3/8Dr Deep",             reach_kind: "deep",   socket_variant: "std",       sizing:    "11/32in", sizing_mm: 8.73125,  sizing_in: 0.34375,      ln_overall: 63.5,    wx_overall: 16.7894, wy_overall: 16.7894, target_end_diam: 12.75,    ratchet_end_diam: 16.7894, shoulder_ln: 29.0068, target_dp: 5.0038,    bolt_clr_diam: 8.0,    targets: { drives:  "#8",    exthex_sz: "#8",     hhcs_sz:  "#8",  hn_sz: "N#8"                       } },

  "1/4in 6-Point US 3/8Dr Regular Impact":      { from:   "1/4in 6-Point US 3/8Dr Regular",          dir: /chrome-sockets/,   title:    "1/4in 6-Point US 3/8Dr Regular Impact",   reach_kind: "reg",    socket_variant: "impact", /* sizing:      "1/4in", sizing_mm: 6.35,     sizing_in: 0.25,      */ /* ln_overall: 24.9936, wx_overall: 17.1958, wy_overall: 17.1958, target_end_diam: 10.0076,  ratchet_end_diam: 17.1958, shoulder_ln: 11.5062, target_dp: 7.493,     bolt_clr_diam: 5.0038, targets: { drives:"#4",      exthex_sz:"#4",      hhcs_sz:"#4",    hn_sz:"N#4"                        } */ },
  "9/32in 6-Point US 3/8Dr Regular Impact":     { from:   "1/4in 6-Point US 3/8Dr Regular",          dir: /chrome-sockets/,   title:   "9/32in 6-Point US 3/8Dr Regular Impact",   reach_kind: "reg",    socket_variant: "impact",    sizing:     "9/32in", sizing_mm: 7.14375,  sizing_in: 0.28125,      ln_overall: 24.9936, wx_overall: 17.1958, wy_overall: 17.1958, target_end_diam: 10.85,    ratchet_end_diam: 17.1958, shoulder_ln: 11.5062, target_dp: 7.493,     bolt_clr_diam: 5.7,    targets: {                                                                                            } },
  "11/32in 6-Point US 3/8Dr Regular Impact":    { from:  "5/16in 6-Point US 3/8Dr Regular",          dir: /chrome-sockets/,   title:  "11/32in 6-Point US 3/8Dr Regular Impact",   reach_kind: "reg",    socket_variant: "impact",    sizing:    "11/32in", sizing_mm: 8.73125,  sizing_in: 0.34375,      ln_overall: 24.9936, wx_overall: 17.1958, wy_overall: 17.1958, target_end_diam: 12.75,    ratchet_end_diam: 17.1958, shoulder_ln: 11.5062, target_dp: 7.493,     bolt_clr_diam: 8.2,    targets: { drives:  "#8",    exthex_sz: "#8",     hhcs_sz:  "#8",  hn_sz: "N#8"                       } },

  "1/4in 6-Point US 3/8Dr U-Joint Regular":     { from:   "1/4in 6-Point US 3/8Dr Regular",          dir: /chrome-sockets/,   title:    "1/4in 6-Point US 3/8Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:      "1/4in", sizing_mm: 6.35,     sizing_in: 0.25,      */ /* ln_overall: 24.9936, wx_overall: 17.1958, wy_overall: 17.1958, target_end_diam: 10.0076,  ratchet_end_diam: 17.1958, shoulder_ln: 11.5062, target_dp: 7.493,     bolt_clr_diam: 5.0038, targets: { drives:"#4",      exthex_sz:"#4",      hhcs_sz:"#4",    hn_sz:"N#4"                        } */ },
  "9/32in 6-Point US 3/8Dr U-Joint Regular":    { from:   "1/4in 6-Point US 3/8Dr Regular",          dir: /chrome-sockets/,   title:   "9/32in 6-Point US 3/8Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",       sizing:     "9/32in", sizing_mm: 7.14375,  sizing_in: 0.28125,      ln_overall: 24.9936, wx_overall: 17.1958, wy_overall: 17.1958, target_end_diam: 10.85,    ratchet_end_diam: 17.1958, shoulder_ln: 11.5062, target_dp: 7.493,     bolt_clr_diam: 5.7,    targets: {                                                                                            } },
  "11/32in 6-Point US 3/8Dr U-Joint Regular":   { from:  "5/16in 6-Point US 3/8Dr Regular",          dir: /chrome-sockets/,   title:  "11/32in 6-Point US 3/8Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",       sizing:    "11/32in", sizing_mm: 8.73125,  sizing_in: 0.34375,      ln_overall: 24.9936, wx_overall: 17.1958, wy_overall: 17.1958, target_end_diam: 12.75,    ratchet_end_diam: 17.1958, shoulder_ln: 11.5062, target_dp: 7.493,     bolt_clr_diam: 8.2,    targets: { drives:  "#8",    exthex_sz: "#8",     hhcs_sz:  "#8",  hn_sz: "N#8"                       } },
  "5/16in 6-Point US 3/8Dr U-Joint Regular":    { from:  "5/16in 6-Point US 3/8Dr Regular",          dir: /chrome-sockets/,   title:   "5/16in 6-Point US 3/8Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:     "5/16in", sizing_mm: 7.9375,   sizing_in: 0.3125,    */ /* ln_overall: 24.9936, wx_overall: 17.1958, wy_overall: 17.1958, target_end_diam: 11.7094,  ratchet_end_diam: 17.1958, shoulder_ln: 11.5062, target_dp: 7.493,     bolt_clr_diam: 7.1882, targets: { drives:"#6",      exthex_sz:"#6",      hhcs_sz:"#6",    hn_sz:"N#6"                        } */ },
  "13/16in 6-Point US 3/8Dr U-Joint Regular":   { from: "13/16in 6-Point US 3/8Dr Regular",          dir: /chrome-sockets/,   title:  "13/16in 6-Point US 3/8Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:    "13/16in", sizing_mm: 20.6375,  sizing_in: 0.8125,    */ /* ln_overall: 27.9908, wx_overall: 27.8892, wy_overall: 27.8892, target_end_diam: 27.8892,  ratchet_end_diam: 27.8892, shoulder_ln: 11.5062, target_dp: 15.0114,   bolt_clr_diam: 8.9916, targets: { drives:"HN1/2in", exthex_sz:"HN1/2in",                                 hhn_sz: "HN1/2in"   } */ },
  "7/8in 6-Point US 3/8Dr U-Joint Regular":     { from:   "7/8in 6-Point US 3/8Dr Regular",          dir: /chrome-sockets/,   title:    "7/8in 6-Point US 3/8Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:      "7/8in", sizing_mm: 22.225,   sizing_in: 0.875,     */ /* ln_overall: 27.9908, wx_overall: 29.6926, wy_overall: 29.6926, target_end_diam: 29.6926,  ratchet_end_diam: 29.6926, shoulder_ln: 11.5062, target_dp: 15.0114,   bolt_clr_diam: 8.9916, targets: { drives:"9/16in",  exthex_sz:"9/16in",  hhcs_sz:"9/16in",hn_sz:"N9/16in"                    } */ },
  "15/16in 6-Point US 3/8Dr U-Joint Regular":   { from: "15/16in 6-Point US 3/8Dr Regular",          dir: /chrome-sockets/,   title:  "15/16in 6-Point US 3/8Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:    "15/16in", sizing_mm: 23.8125,  sizing_in: 0.9375,    */ /* ln_overall: 29.4894, wx_overall: 31.9024, wy_overall: 31.9024, target_end_diam: 31.9024,  ratchet_end_diam: 31.9024, shoulder_ln: 11.5062, target_dp: 15.0114,   bolt_clr_diam: 8.9916, targets: { drives:"5/8in",   exthex_sz:"5/8in",   hhcs_sz:"5/8in", hn_sz:"N5/8in"                     } */ },
  "1in 6-Point US 3/8Dr U-Joint Regular":       { from:     "1in 6-Point US 3/8Dr Regular",          dir: /chrome-sockets/,   title:      "1in 6-Point US 3/8Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:        "1in", sizing_mm: 25.4,     sizing_in: 1.0,       */ /* ln_overall: 33.5026, wx_overall: 33.909,  wy_overall: 33.909,  target_end_diam: 33.909,   ratchet_end_diam: 33.909,  shoulder_ln: 11.5062, target_dp: 18.0086,   bolt_clr_diam: 8.9916, targets: {                                                                                            } */ },

  // "5mm 6-Point MM 1/4Dr Regular":            { from:     "5mm 6-Point MM 1/4Dr Regular",          dir: /chrome-sockets/,   title:      "5mm 6-Point MM 1/4Dr Regular",          reach_kind: "reg",    socket_variant: "std",    /* sizing:        "5mm", sizing_mm:  5.0,     sizing_in: 0.1968504, */ ln_overall: 24.511,  wx_overall: 11.811,  wy_overall: 11.811,  target_end_diam: 8.001,    ratchet_end_diam: 11.811,  shoulder_ln: 11.5062, target_dp: 8.001,     bolt_clr_diam: 3.9878, targets: { drives:  "M2.5",  exthex_sz:"M2.5",    hhcs_sz:"M2.5",  hn_sz: "NM2.5", hhn_sz:"HNM2.5"    } },
  // "5mm 6-Point MM 1/4Dr U-Joint Regular":    { from:     "5mm 6-Point MM 1/4Dr U-Joint Regular",  dir: /chrome-sockets/,   title:      "5mm 6-Point MM 1/4Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:        "5mm", sizing_mm:  5.0,     sizing_in: 0.1968504, */ ln_overall: 35.0012, wx_overall: 13.9954, wy_overall: 13.9954, target_end_diam: 8.5598,   ratchet_end_diam: 13.9954,                       target_dp: 6.858,                            targets: { drives:  "M2.5",  exthex_sz:"M2.5",    hhcs_sz:"M2.5",  hn_sz: "NM2.5", hhn_sz:"HNM2.5"    } },
  // "5.5mm 6-Point MM 1/4Dr U-Joint Regular":  { from:   "5.5mm 6-Point MM 1/4Dr U-Joint Regular",  dir: /chrome-sockets/,   title:    "5.5mm 6-Point MM 1/4Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:      "5.5mm", sizing_mm:  5.5,     sizing_in: 0.2165354, */ ln_overall: 35.0012, wx_overall: 13.9954, wy_overall: 13.9954, target_end_diam: 9.6012,   ratchet_end_diam: 13.9954,                       target_dp: 7.366,                            targets: { drives:  "M3",    exthex_sz:"M3",      hhcs_sz:"M3",    hn_sz:"NM3",    hhn_sz:"HNM3"      } },
  // "10mm 6-Point MM 3/8Dr Regular":           { from:    "10mm 6-Point MM 3/8Dr Regular",          dir: /chrome-sockets/,   title:     "10mm 6-Point MM 3/8Dr Regular",          reach_kind: "reg",    socket_variant: "std",    /* sizing:       "10mm", sizing_mm: 10.0,     sizing_in: 0.3937008, */ ln_overall: 45.0088, wx_overall: 16.9926, wy_overall: 16.9926, target_end_diam: 14.5034,  ratchet_end_diam: 16.9926, shoulder_ln: 11.5062, target_dp: 7.493,     bolt_clr_diam: 7.1882, targets: { drives:  "M6",    exthex_sz:"M6",      hhcs_sz:"M6",    hn_sz:"NM6",    hhn_sz:"HNM6"      } },
  // "10mm 6-Point MM 3/8Dr U-Joint Regular":   { from:    "10mm 6-Point MM 3/8Dr U-Joint Regular",  dir: /chrome-sockets/,   title:     "10mm 6-Point MM 3/8Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:       "10mm", sizing_mm: 10.0,     sizing_in: 0.3937008, */ ln_overall: 45.0088, wx_overall: 18.9992, wy_overall: 18.9992, target_end_diam: 8.4074,   ratchet_end_diam: 18.9992,                       target_dp:   7.112,                          targets: { drives:  "M6",    exthex_sz:"M6",      hhcs_sz:"M6",    hn_sz:"NM6",    hhn_sz:"HNM6"      } },
  // "19mm 6-Point MM 3/8Dr Regular":           { from:    "19mm 6-Point MM 3/8Dr Regular",          dir: /chrome-sockets/,   title:     "19mm 6-Point MM 3/8Dr Regular",          reach_kind: "reg",    socket_variant: "std",    /* sizing:       "19mm", sizing_mm: 19.0,     sizing_in: 0.7480315, */ ln_overall: 24.9936, wx_overall: 23.7998, wy_overall: 23.7998, target_end_diam: 23.7998,  ratchet_end_diam: 23.7998, shoulder_ln: 11.5062, target_dp: 12.4968,   bolt_clr_diam: 8.9916, targets: { drives:  "M12",   exthex_sz:"M12",     hhcs_sz:"M12",   hn_sz:"NM12",   hhn_sz:"HNM12"     } },
  // "19mm 6-Point MM 3/8Dr U-Joint Regular":   { from:    "19mm 6-Point MM 3/8Dr U-Joint Regular",  dir: /chrome-sockets/,   title:     "19mm 6-Point MM 3/8Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:       "19mm", sizing_mm: 19.0,     sizing_in: 0.7480315, */ ln_overall: 50.0126, wx_overall: 18.9992, wy_overall: 18.9992, target_end_diam: 17.1958,  ratchet_end_diam: 18.9992,                       target_dp:   11.43,                          targets: { drives:  "M12",   exthex_sz:"M12",     hhcs_sz:"M12",   hn_sz:"NM12",   hhn_sz:"HNM12"     } },
  // "3/16in 6-Point US 1/4Dr Regular":         { from:  "3/16in 6-Point US 1/4Dr Regular",          dir: /chrome-sockets/,   title:   "3/16in 6-Point US 1/4Dr Regular",          reach_kind: "reg",    socket_variant: "std",    /* sizing:     "3/16in", sizing_mm: 4.7625,   sizing_in: 0.1875,    */ ln_overall: 24.511,  wx_overall: 11.811,  wy_overall: 11.811,  target_end_diam: 7.7978,   ratchet_end_diam: 11.811,  shoulder_ln: 11.5062, target_dp: 8.001,     bolt_clr_diam: 3.9878, targets: { drives:"#2",      exthex_sz:"#2",      hhcs_sz:"#2",    hn_sz:"N#2"                        } },
  // "3/16in 6-Point US 1/4Dr U-Joint Regular": { from:  "3/16in 6-Point US 1/4Dr U-Joint Regular",  dir: /chrome-sockets/,   title:   "3/16in 6-Point US 1/4Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:     "3/16in", sizing_mm: 4.7625,   sizing_in: 0.1875,    */ ln_overall: 35.0012, wx_overall: 13.9954, wy_overall: 13.9954, target_end_diam: 9.6012,   ratchet_end_diam: 13.9954,                       target_dp:   6.096,                          targets: { drives:"#2",      exthex_sz:"#2",      hhcs_sz:"#2",    hn_sz:"N#2"                        } },
  // "1/4in 6-Point US 1/4Dr Regular":          { from:   "1/4in 6-Point US 1/4Dr Regular",          dir: /chrome-sockets/,   title:    "1/4in 6-Point US 1/4Dr Regular",          reach_kind: "reg",    socket_variant: "std",    /* sizing:      "1/4in", sizing_mm: 6.35,     sizing_in: 0.25,      */ ln_overall: 24.511,  wx_overall: 11.811,  wy_overall: 11.811,  target_end_diam: 9.4996,   ratchet_end_diam: 11.811,  shoulder_ln: 11.5062, target_dp: 8.9916,    bolt_clr_diam: 3.9878, targets: { drives:"#4",      exthex_sz:"#4",      hhcs_sz:"#4",    hn_sz:"N#4"                        } },
  // "1/4in 6-Point US 1/4Dr U-Joint Regular":  { from:   "1/4in 6-Point US 1/4Dr U-Joint Regular",  dir: /chrome-sockets/,   title:    "1/4in 6-Point US 1/4Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:      "1/4in", sizing_mm: 6.35,     sizing_in: 0.25,      */ ln_overall: 35.0012, wx_overall: 13.9954, wy_overall: 13.9954, target_end_diam: 9.6012,   ratchet_end_diam: 13.9954,                       target_dp:   6.858,                          targets: { drives:"#4",      exthex_sz:"#4",      hhcs_sz:"#4",    hn_sz:"N#4"                        } },
  // "5/16in 6-Point US 1/4Dr Regular":         { from:  "5/16in 6-Point US 1/4Dr Regular",          dir: /chrome-sockets/,   title:   "5/16in 6-Point US 1/4Dr Regular",          reach_kind: "reg",    socket_variant: "std",    /* sizing:     "5/16in", sizing_mm: 7.9375,   sizing_in: 0.3125,    */ ln_overall: 24.511,  wx_overall: 11.811,  wy_overall: 11.811,  target_end_diam: 11.5062,  ratchet_end_diam: 11.811,  shoulder_ln: 11.5062, target_dp: 10.4902,   bolt_clr_diam: 5.588,  targets: { drives:"#6",      exthex_sz:"#6",      hhcs_sz:"#6",    hn_sz:"N#6"                        } },
  // "5/16in 6-Point US 1/4Dr U-Joint Regular": { from:  "5/16in 6-Point US 1/4Dr U-Joint Regular",  dir: /chrome-sockets/,   title:   "5/16in 6-Point US 1/4Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:     "5/16in", sizing_mm: 7.9375,   sizing_in: 0.3125,    */ ln_overall: 35.0012, wx_overall: 13.9954, wy_overall: 13.9954, target_end_diam: 11.811,   ratchet_end_diam: 13.9954,                       target_dp:   6.096,                          targets: { drives:"#6",      exthex_sz:"#6",      hhcs_sz:"#6",    hn_sz:"N#6"                        } },
  // "3/8in 6-Point US 3/8Dr Regular":          { from:   "3/8in 6-Point US 3/8Dr Regular",          dir: /chrome-sockets/,   title:    "3/8in 6-Point US 3/8Dr Regular",          reach_kind: "reg",    socket_variant: "std",    /* sizing:      "3/8in", sizing_mm: 9.525,    sizing_in: 0.375,     */ ln_overall: 24.9936, wx_overall: 17.1958, wy_overall: 17.1958, target_end_diam: 13.7922,  ratchet_end_diam: 17.1958, shoulder_ln: 11.5062, target_dp: 7.493,     bolt_clr_diam: 7.1882, targets: { drives:"#10",     exthex_sz:"#10",     hhcs_sz:"#10",   hn_sz:"N#10"                       } },
  // "3/8in 6-Point US 3/8Dr U-Joint Regular":  { from:   "3/8in 6-Point US 3/8Dr U-Joint Regular",  dir: /chrome-sockets/,   title:    "3/8in 6-Point US 3/8Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:      "3/8in", sizing_mm: 9.525,    sizing_in: 0.375,     */ ln_overall: 43.9928, wx_overall: 18.9992, wy_overall: 18.9992, target_end_diam: 14.5034,  ratchet_end_diam: 18.9992,                       target_dp: 5.588,                            targets: { drives:"#10",     exthex_sz:"#10",     hhcs_sz:"#10",   hn_sz:"N#10"                       } },
  // "3/4in 6-Point US 3/8Dr Regular":          { from:   "3/4in 6-Point US 3/8Dr Regular",          dir: /chrome-sockets/,   title:    "3/4in 6-Point US 3/8Dr Regular",          reach_kind: "reg",    socket_variant: "std",    /* sizing:      "3/4in", sizing_mm: 19.05,    sizing_in: 0.75,      */ ln_overall: 24.9936, wx_overall: 25.908,  wy_overall: 25.908,  target_end_diam: 25.908,   ratchet_end_diam: 25.708,  shoulder_ln: 11.5062, target_dp: 12.4968,   bolt_clr_diam: 8.9916, targets: { drives:"1/2in",   exthex_sz:"1/2in",   hhcs_sz:"1/2in", hn_sz:"N1/2in", hhn_sz: "HN7/16in" } },
  // "3/4in 6-Point US 3/8Dr U-Joint Regular":  { from:   "3/4in 6-Point US 3/8Dr U-Joint Regular",  dir: /chrome-sockets/,   title:    "3/4in 6-Point US 3/8Dr U-Joint Regular",  reach_kind: "uj_reg", socket_variant: "std",    /* sizing:      "3/4in", sizing_mm: 19.05,    sizing_in: 0.75,      */ ln_overall: 51.0032, wx_overall: 25.8064, wy_overall: 25.8064, target_end_diam: 25.8064,  ratchet_end_diam: 18.9992,                       target_dp:   11.43,                          targets: { drives:"1/2in",   exthex_sz:"1/2in",   hhcs_sz:"1/2in", hn_sz:"N1/2in", hhn_sz: "HN7/16in" } },

} satisfies TY.Bag<Partial<GearwrenchSocketSk> & { from: string, dir: RegExp, copied?: boolean }>

// == [Helpers] ==

/** Tracks enum values for each field */
export const Enumish = {
  socket_kind: [], sqdrive_size: [], unit_system: [], drive_kind: [], bit_kind: [], reach_kind: [], socket_variant: [],
} satisfies TY.PartialBag<keyof Sockets.SocketWrenchT, string[]>

/** Extracts dimensions from the raw compount specification text (`Dim. A : Overall Length : 10 in`) */
function extract_dim(rawkey: string, raw: string): [TY.Fieldname, number] {
  // the weird two-specs-in-one are only for spark plug sockets, discarding the second one
  const match = /^(.+?) : (\d+(?:\.\d+)?) (in|mm)(?: (Wrench End|Wrench Depth) : (\d+(\.\d+)?) (in|mm))?$/.exec(raw)
  if (! match) { throw new Error(`Invalid dimension: ${raw}`) }
  const [_s, raw_fn, valstr, units] = match
  //
  const fn = fieldname_remap[raw_fn!]; if (! fn) { console.warn(`Unknown dimension: ${rawkey} => ${raw}`) }
  const num = Number(valstr)
  const val = units === 'in' ? _.round(num * MM_IN, 7) : _.round(num, 7)
  // console.warn(`${rawkey} => ${raw} => ${fn} = ${val} ${units} ${num}`)
  return [fn ?? 'oops', val]
}
/** Extracts distances from simple specification text */
function extract_dist(raw: string): number {
  const match = /^(\d+(?:\.\d+)?) ?(in|mm)$/.exec(raw)
  if (! match) { throw new Error(`Invalid distance: ${raw}`) }
  const [_s,  valstr, units] = match
  //
  const num = Number(valstr)
  const val = units === 'in' ? _.round(num * MM_IN, 7) : _.round(num, 7)
  return val
}
// --

// == [Parsing] ==

const DriverKindToPrefix = {
  exthex:          'Wr',   extstar:         'E',   extstar12:       'Wr',   inthex:          'H',
  intsq:           'Dr',   torx:            'T',   torxtp:          'TP',   triple_square:   'Sq3',
  slotted:         'Sl',   square:          'Sq',  phillips:        'Ph',   pozidriv:        'Pz',
  phslot:          'Ph',   knurled:         'Kn',  carriage:        'Cr',
} as const satisfies { [key in Fastener.FastenerEnums.FastenerDrive]: string }

/** Parses a gearwrench socket product page and returns a SocketWrenchProductT */
export function parseProductPage(filepath: TY.Anypath, textblob: string): GearwrenchSocketT {
  const $ = cheerioLoad(textblob)
  // Title: prefer og:title, fall back to <title>
  const ogTitle  = $('meta[property="og:title"]').attr('content') ?? ''
  const rawTitle    = ogTitle.trim() || $('title').text()
  // URL from canonical or og:url
  const url      = $('link[rel="canonical"]').attr('href')
                ?? $('meta[property="og:url"]').attr('content')
                ?? ''
  // Main image from og:image or image_src link
  const img_url = $('meta[property="og:image"]').attr('content')
                ?? $('link[rel="image_src"]').attr('href')
                ?? ''

  // SKU from JSON-LD Product schema
  let sku = ''
  $('script[type="application/ld+json"]').each((_, el) => {
    try {
      const data = JSON.parse($(el).html() ?? '{}')
      const graph: any[] = data['@graph'] ?? [data]
      for (const node of graph) {
        if (node['@type'] === 'Product' && node.sku) {
          sku = String(node.sku)
          break
        }
      }
    } catch { /* malformed JSON-LD, skip */ }
  })
  const gwtitle = rawTitle
    .replace(/\s*-\s*Gearwrench\s*$/i, '')
    .replace(sku + ' ', '')
    .replaceAll(/(\d) *"/g, '$1in')
    .replaceAll(/(\d) +(mm|in)\b/g, '$1$2')
    .trim()

  // Specifications from <li id="specifications">
  const specifications: Record<string, string> = {}
  $('#specifications li.field__item').each((_, el) => {
    const spans = $(el).find('span')
    const label = spans.eq(0).text().replace(/\s*:\s*$/, '').trim()
    const value = spans.eq(1).text().replace(/\s+/g, ' ').trim()
    if (label) specifications[label] = value
  })
  const result = { sku, gwtitle, url, img_url } as GearwrenchSocketSk & { drive_end_hex_af: any, bit_ln_exposed: any, wt_lb: any }

  _.each(specifications, (raw, key) => {
    if (/^Dim\./.test(key)) { const [fn, val] = extract_dim(key, raw); result[fn] = val; return }
    const fn = fieldname_remap[key]
    if (_.isNull(fn)) { return }
    if (fn === 'bit_kind' && /External/.test(raw)) { return } // a couple pages have bit types where it's not a bit kind
    if (fn === 'wt_lb')                       { result[fn] = Number(raw.replace(/ lb$/, '')); result.wt = _.round(result.wt_lb * KG_LB, 6); return }
    if (/drive_size$/.test(fn))               { result[fn] = sqdrive_size_remap[raw]; if (! result[fn]) { console.warn(`Unknown drive size: ${raw}`)   } return }
    if (/(bit|drive)_kind$/.test(fn))         { result[fn] = drive_kind_remap[raw];   if (! result[fn]) { console.warn(`Unknown drive kind: ${raw}`)   } return }
    if (fn === 'socket_kind')                 { result[fn] = socket_kind_remap[raw];  if (! result[fn]) { console.warn(`Unknown socket kind: ${raw}`)  } return }
    if (fn === 'reach_kind')                  { result[fn] = reach_kind_remap[raw];   if (! result[fn]) { console.warn(`Unknown reach kind: ${raw}`)   } return }
    if (fn === 'unit_system')                 { result[fn] = unit_system_remap[raw];  if (! result[fn]) { console.warn(`Unknown unit system: ${raw}`) } return }
    if (/(overall|bit_ln_exposed)$/.test(fn)) { result[fn] = extract_dist(raw); return }
    if (fn === 'is_prop65' && /WARNING/.test(raw))    { result[fn] = true;  return }
    if (fn === 'is_prop65' && /No Warning/.test(raw)) { result[fn] = false; return }
    if (/is_/.test(fn)) { result[fn] = CK.boolish.cast(raw, { filepath, specifications, raw, key }); return }
    if (fn) { result[fn] = raw; return }
    console.warn(`Unknown specification: ${key} = ${raw}`)
  })
  if (result.socket_kind === 'socket_exthex' && /\bFlex Socket\b/i.test(gwtitle)) { result.reach_kind = 'uj_' + result.reach_kind as FE.SocketReach}
  if (result.socket_kind === 'socket_exthex' && /\bUniversal\b/i.test(gwtitle))   { result.reach_kind = 'uj_' + result.reach_kind as FE.SocketReach }
  if (specifications['Type'] === 'Socket Extension') { result.reach_kind = 'uj_ext' }
  result.socket_variant = 'std'
  if (/impact/i.test(gwtitle))                            { result.socket_variant = 'impact' }
  if (/ball/i.test(specifications['Drive Type'] ?? 'xx')) { result.socket_variant = 'ball' }
  delete result.drive_end_hex_af
  if (result.bit_ln_exposed) {
    result.bit_ln_total = _.max([result.bit_ln, result.bit_ln_exposed])
    result.bit_ln       = _.min([result.bit_ln, result.bit_ln_exposed])
    delete result.bit_ln_exposed
  }
  if (/slotted/.test(result.bit_kind!)) { result.drive_kind = 'slotted'; result.sizing = result.sizing.replace(/^(?:Sl)?#?(\d+)(mm)?/, 'Sl$1') }
  if (result.bit_kind && (result.bit_kind !== result.drive_kind)) { console.warn(`Bit kind mismatch: ${result.bit_kind} !== ${result.drive_kind}`, result) }
  if (result.target_end_diam === 1024.89) { result.target_end_diam = 102.489 } // assuming this is a typo for 4.035 in (102.489 mm)
  if (result.drive_kind === 'extstar')  { result.socket_kind = 'socket_extstar'; result.unit_system = 'metric' }
  if (result.drive_kind === 'phillips') { result.sizing = result.sizing.replace(/^(Ph)?#?/, 'Ph') }
  if (result.drive_kind === 'pozidriv') { result.sizing = result.sizing.replace(/^(Pz)?#?/, 'Pz') }
  if (/^(socket_(extension|adapter|ujoint))$/.test(result.socket_kind)) { result.reach_kind = 'other'; result.drive_kind = 'intsq' }
  if (/^(socket_(extension))$/.test(result.socket_kind)) {
    result.unit_system ??= 'us'
    result.sizing ??= specifications['Overall Length'] + ' - ' + (specifications['Male Drive Size'] ?? specifications['Drive Tang Size'] ?? '')
  }
  if (/^(socket_(adapter|ujoint))$/.test(result.socket_kind)) {
    result.unit_system ??= 'us'
    result.sizing ??= specifications['Male Drive Size'] ?? specifications['Drive Tang Size'] as string
  }
  if (specifications['Size Range (SAE)']) { result.unit_system = 'us' } if (specifications['Size Range (Metric)']) { result.unit_system = 'metric' }
  result.sizing = result.sizing?.replace(/ +(mm|in)\b/g, '$1').replaceAll(/(\d+)-(\d+\/\d+)in/g, '$1+$2in').replaceAll(/\.0+in/g, 'in')
  result.sizing_mm = DistanceLookup[result.sizing]
  result.sizing_in = _.round(result.sizing_mm / MM_IN, 7)
  result.img_url = img_url.replace(/\?itok=.*$/, '')

  // Track enum values for each field
  _.each(_.pick(result, _.keys(Enumish)), (val, key) => { const seen = Enumish[key]; if (! seen.includes(val)) { seen.push(val) }  })

  // if (! (result.ln_overall && result.wy_overall && result.wx_overall)) { console.warn(`No overall length`, UF.prettify(result)); result.ln_overall ??= 1; result.wy_overall ??= 1; result.wx_overall ??= 1; }
  const overall_wx = result.wx_overall ?? _.max([result.target_end_diam, result.ratchet_end_diam])
  const overall_wy = result.wy_overall ?? _.max([result.target_end_diam, result.ratchet_end_diam])
  if (overall_wx) { result.wx_overall = overall_wx } if (overall_wy) { result.wy_overall = overall_wy }
  // if (specifications['Type'] === 'Socket Extension') { console.warn('\nSocket Extension\n', gwtitle, specifications, result) }

  result.title    = result.gwtitle
  const driverSizingPrefix = DriverKindToPrefix[result.drive_kind]
  const driver_sz =  (driverSizingPrefix + result.sizing.replace(/^([ET]+|P[hz]|Sl)/, '')) as FE.ToolDrive
  result.targets = DriveToDriven[driver_sz] ?? {}
  if (/socket_(exthex|extstar|bit)$/.test(result.socket_kind)) {
    if (! Fastener.FastenerEnums.DriverSizingVals.includes(driver_sz)) { console.warn(driver_sz) }
    // drives.exthex_sz = result.sizing
  }
  const socket = gearwrenchSocket.cast(result as GearwrenchSocketSk, { filepath, specifications })
  socket.title = socket.sizing + " " + Sockets.SocketWrench.familyTitleFor(socket)
  if (SizeOverrides[socket.title]) { _.merge(socket, SizeOverrides[socket.title]) }
  return socket
}
// --
