import      _                                   /**/ from 'lodash'
import      { CK }                                   from '@freeword/meta'

export type DrillTitle = string
export const drill_title = CK.extkeyish

/** Drive type for square (1/4", 3/8", 1/2") and hex (1/4") drives */
export const ToolDriveVals   = ['isq_0250in', 'isq_0375in', 'isq_0500in', 'hex_0250in', 'isq_0750in', 'isq_1000in'] as const
/** Unit system (metric, us) */
export const UnitSystemVals = ['metric', 'us'] as const
/** Socket kind (socket_sparkplug, socket_exthex, socket_extstar, socket_bit, socket_extension, socket_ujoint, socket_adapter) */
export const SocketKindVals  = [
  'socket_sparkplug', 'socket_exthex', 'socket_extstar', 'socket_bit',
  'socket_extension', 'socket_ujoint', 'socket_adapter',
] as const
/** Socket reach (standard, midlen, deep, long, xlong, other) */
export const SocketReachVals = ['reg', 'midlen', 'deep', 'long', 'xlong', 'other', 'uj_reg', 'uj_deep', 'uj_ext'] as const
/** Socket variant (standard, impact, ball) */
export const SocketVariantVals = ['std', 'impact', 'ball'] as const

export const SocketKindTitles = {
  'socket_sparkplug': 'Spark Plug Socket',
  'socket_exthex':    'Bolt Socket',
  'socket_extstar':   'Star Socket',
  'socket_bit':       'Bit Socket',
  'socket_extension': 'Socket Extension',
  'socket_ujoint':    'Socket U-Joint',
  'socket_adapter':   'Socket Adapter',
} as const
export const SocketReachTitles = {
  'reg': 'Regular', 'midlen': 'Medium', 'deep': 'Deep', 'long': 'Long', 'xlong': 'X-Long', 'other': '', 'uj_reg': 'U-Joint Regular', 'uj_deep': 'U-Joint Deep', 'uj_ext': 'U-Joint Extended',
} as const
export const SocketVariantTitles = {
  'std': '', 'impact': 'Impact', 'ball': 'Ball End',
} as const
export const ToolDriveTitles = {
  'isq_0250in': '1/4Dr', 'isq_0375in': '3/8Dr', 'isq_0500in': '1/2Dr',
  'hex_0250in': '1/4 Hex.Dr', 'isq_0750in': '3/4Dr', 'isq_1000in': '1Dr',
} as const
export const FastenerDriveTitles: Record<FastenerDrive, string> = {
  'exthex':        'Hex Bolt',    'extstar':  'Ext Torx',      'extstar12':    '12-Point',
  'inthex':        'Int Hex',     'intsq':    'Square Drive',
  'torx':          'Torx',        'torxtp':   'Security Torx', 'triple_square': 'Triple Sq.',
  'slotted':       'Slotted',     'square':   'Square',        'phillips':      'Phillips',
  'pozidriv':      'Pozidriv',
  'phslot':        'Ph/Slot',     'knurled':  'Knurled',        'carriage': 'Carriage',
}
export const SocketDriveTitles: Record<FastenerDrive, string> = {
  ...FastenerDriveTitles,
  'exthex':        '6-Point',
}
export const UnitSystemTitles = {
  'metric': 'MM', 'us': 'US',
} as const

/** Socket kind (socket_sparkplug, socket_exthex, socket_extstar, socket_bit, socket_extension, socket_ujoint, socket_adapter) */
export const socket_kind     = CK.oneof(SocketKindVals)
/** Tool drive (isq_0250in, isq_0375in, isq_0500in, hex_0250in, isq_0750in, isq_1000in) */
export const tool_drive      = CK.oneof(ToolDriveVals)
/** Unit system (mm, us) */
export const unit_system   = CK.oneof(UnitSystemVals)
/** Socket reach (standard, midlen, deep, long, xlong, other) */
export const socket_reach    = CK.oneof(SocketReachVals)
/** Socket variant (standard, impact, ball) */
export const socket_variant  = CK.oneof(SocketVariantVals)

export type SocketKind       = typeof SocketKindVals[number]
export type ToolDrive        = typeof ToolDriveVals[number]
export type UnitSystem       = typeof UnitSystemVals[number]
export type SocketReach      = typeof SocketReachVals[number]
export type SocketVariant    = typeof SocketVariantVals[number]

/** Threading standardization */
export type  ThreadingStandardization     = typeof ThreadingStandardizationVals[number]
export const ThreadingStandardizationVals = ['UNC', 'UNF', 'UNEF', 'ISOC', 'ISOF', 'ISOEF'] as const
export const thread_stdz = CK.oneof(ThreadingStandardizationVals)

export type WasherStandardization = typeof WasherStandardizationVals[number]
export const WasherStandardizationVals = ['USS', 'SAE', 'FDR', 'ISO-RW', 'ISO-LW'] as const
export const washer_stdz = CK.oneof(WasherStandardizationVals)

/** Fastener head form (bolt (external hex), socket (cylindrical socket), button, etc.) */
export type HeadForm = typeof HeadFormVals[number]
export const HeadFormVals = [
  'bolt', 'socket', /* low-profile socket */ 'losock', 'button', 'flathead', /* undercut flat */ 'underflat',
  'round', 'oval', 'pan', 'round', 'truss', 'fillister', 'cheese',
  'setscrew',
] as const
export const head_form = CK.oneof(HeadFormVals)

/** Fastener drive (external hex (exthex), internal hex (inthex), etc.) */
export type FastenerDrive = typeof FastenerDriveVals[number]
/** External drive (exthex, extstar, etc.) */
export type ExternalDrive = typeof ExternalDriveVals[number]
/** Internal drive (inthex, torx, etc.) */
export type InternalDrive = typeof InternalDriveVals[number]
/** Other drive (knurled, carriage, etc.) */
export type OtherFastenerDrive = typeof OtherFastenerDriveVals[number]

export const ExternalDriveVals = [
  /* 6-sided bolt head i.e. hhcs */                        'exthex',
  /* 6-sided external star (torx), less common */          'extstar',
  /* 12-sided external drive (combination wrench style) */ 'extstar12',
] as const
export const external_drive = CK.oneof(ExternalDriveVals)
export const InternalDriveVals = [
  /* 6-sided internal hex i.e. shcs */            'inthex',
  /* 4-sided internal square eg ratchets */       'intsq',
  /* 6-sided internal star i.e. torx */           'torx',
  /* tamper proof torx */                         'torxtp',
  /* triple square */                             'triple_square',
  /* slotted ('flat') */                          'slotted',
  /* square */                                    'square',
  /* phillips */                                  'phillips',
  /* pozidriv */                                  'pozidriv',
  /* phillips-profile cross drive */              'phillips',
] as const
export const internal_drive = CK.oneof(InternalDriveVals)
export const OtherFastenerDriveVals = [
  /* combo phillips/slotted */                    'phslot',
  /* Knurled Thumb drive */                       'knurled',
  /* Deforming Square Keying */                   'carriage',
] as const
export const other_fastener_drive = CK.oneof(OtherFastenerDriveVals)
export const FastenerDriveVals = [...ExternalDriveVals, ...InternalDriveVals, ...OtherFastenerDriveVals] as const
export const fastener_drive = CK.oneof(FastenerDriveVals)

// "A" sizes either match a standardized screw size OR come in a set as useless baggage (9, 11, 14, 17)
export const MetricWrenchSizingValsA = [
  "Wr4mm",       "Wr5mm",       "Wr5.5mm",     "Wr6mm",       "Wr7mm",       "Wr8mm",       "Wr9mm",       "Wr10mm",
  "Wr11mm",      "Wr12mm",      "Wr13mm",      "Wr14mm",      "Wr15mm",      "Wr16mm",      "Wr17mm",      "Wr18mm",
  "Wr19mm",      "Wr21mm",      "Wr22mm",      "Wr24mm",      "Wr27mm",      "Wr30mm",      "Wr34mm",      "Wr36mm",
  "Wr41mm",      "Wr46mm",      "Wr50mm",      "Wr55mm",      "Wr60mm",      "Wr65mm",
] as const
export const MetricWrenchSizingVals = [
  ...MetricWrenchSizingValsA,
  "Wr4.5mm",     "Wr20mm",      "Wr23mm",      "Wr25mm",      "Wr26mm",      "Wr28mm",      "Wr29mm",      "Wr31mm",
  "Wr32mm",      "Wr33mm",      "Wr35mm",      "Wr38mm",      "Wr37mm",      "Wr39mm",      "Wr40mm",      "Wr42mm",
  "Wr43mm",      "Wr44mm",      "Wr47mm",      "Wr45mm",      "Wr48mm",      "Wr49mm",      "Wr54mm",      "Wr58mm",
] as const
// "A" sizes either match a standardized screw size OR are micro (3/16in, 7/32in) or come in a set as useless baggage (9/32in, 1in)
export const USWrenchSizingValsA     = [
  "Wr3/16in",    "Wr7/32in",    "Wr1/4in",     "Wr9/32in",    "Wr5/16in",    "Wr11/32in",   "Wr3/8in",     "Wr7/16in",
  "Wr1/2in",     "Wr9/16in",    "Wr5/8in",     "Wr11/16in",   "Wr3/4in",     "Wr13/16in",   "Wr7/8in",     "Wr15/16in",
  "Wr1in",       "Wr1+1/16in",  "Wr1+1/8in",   "Wr1+1/4in",
  "Wr1+5/16in",  "Wr1+7/16in",  "Wr1+1/2in",   "Wr1+5/8in",   "Wr1+11/16in", "Wr1+13/16in", "Wr1+7/8in",   "Wr2in",
  "Wr2+1/16in",  "Wr2+3/16in",  "Wr2+1/4in",   "Wr2+3/8in",   "Wr2+5/8in",   "Wr2+3/4in",   "Wr3in",       "Wr3+1/8in",
  "Wr3+3/8in",   "Wr3+1/2in",   "Wr3+3/4in",   "Wr3+7/8in",   "Wr4+1/8in",   "Wr4+1/4in",   "Wr4+1/2in",
] as const
export const USWrenchSizingVals = [
  ...USWrenchSizingValsA,
  "Wr5/32in",
  "Wr1+3/16in",  "Wr1+3/8in",   "Wr1+9/16in",   "Wr1+3/4in",  "Wr1+15/16in", "Wr2+1/8in",   "Wr2+5/16in",
  "Wr2+7/16in",  "Wr2+1/2in",   "Wr2+9/16in",  "Wr2+13/16in", "Wr2+11/16in", "Wr2+7/8in",   "Wr2+15/16in",  "Wr3+1/4in",
  "Wr3+5/8in",    "Wr4in",
] as const
export const MetricHexkeySizingVals = [
  "H0.7mm",      "H0.9mm",      "H1.3mm",      "H1.5mm",      "H2mm",        "H2.5mm",      "H3mm",        "H4mm",
  "H5mm",        "H6mm",        "H7mm",        "H8mm",        "H9mm",        "H10mm",       "H11mm",       "H12mm",
  "H13mm",       "H14mm",       "H15mm",       "H17mm",       "H19mm",       "H22mm",       "H27mm",       "H32mm",
] as const
export const USHexkeySizingVals     = [
  "H0.028in",    "H0.035in",    "H0.050in",    "H1/16in",     "H5/64in",     "H3/32in",     "H7/64in",     "H1/8in",
  "H9/64in",     "H5/32in",     "H3/16in",     "H7/32in",     "H1/4in",      "H5/16in",     "H3/8in",      "H7/16in",
  "H1/2in",       "H9/16in",    "H5/8in",      "H11/16in",    "H3/4in",      "H7/8in",      "H1in",
] as const
export const TorxkeySizingVals      = [
  "T5",          "T6",          "T7",          "T8",         "T9",           "T10",         "T15",         "T20",
  "T25",         "T27",         "T30",         "T40",        "T45",          "T47",         "T50",         "T55",         "T60",
] as const
export const TorxpkeySizingVals      = [
  "TP5",         "TP6",         "TP7",         "TP8",         "TP9",         "TP10",        "TP15",        "TP20",
  "TP25",        "TP27",        "TP30",        "TP40",        "TP45",        "TP47",        "TP50",        "TP55",        "TP60",
] as const
export const ExtstarDriveSizingVals = [
  "E4",         "E5",         "E6",         "E7",         "E8",         "E10",        "E11",       "E12",
  "E14",        "E16",        "E18",        "E20",        "E22",        "E24",
] as const
export const RatchetDriveSizingVals = [
  "Dr1/4in",     "Dr3/8in",     "Dr1/2in",     "Dr3/4in",     "Dr1in",
]
export const PhillipsDriveSizingVals = ["Ph00", "Ph0", "Ph1", "Ph2", "Ph3", "Ph4"] as const
export const PozidrivDriveSizingVals = ["Pz2", "Pz3", "Pz4"] as const
export const SlottedDriveSizingVals  = ["Sl0", "Sl1", "Sl2", "Sl3", "Sl4", "Sl5", "Sl6"] as const
export const DriverSizingVals        = [
  ...MetricWrenchSizingVals, ...USWrenchSizingVals,
  ...MetricHexkeySizingVals, ...USHexkeySizingVals,
  ...TorxkeySizingVals,      ...TorxpkeySizingVals,
  ...RatchetDriveSizingVals,
  ...PhillipsDriveSizingVals,
  ...PozidrivDriveSizingVals,
  ...SlottedDriveSizingVals,
  ...ExtstarDriveSizingVals,
] as const
/** Wrench or hexkey driver name */
export type WrenchSizing    = typeof MetricWrenchSizingVals[number] | typeof USWrenchSizingVals[number]
export type HexkeySizing    = typeof MetricHexkeySizingVals[number] | typeof USHexkeySizingVals[number]
export type TorxkeySizing   = typeof TorxkeySizingVals[number]
export type KeydriveSizing  = HexkeySizing | TorxkeySizing
export type DriverSizing    = typeof DriverSizingVals[number]

export const wrench_sz = CK.oneof([...MetricWrenchSizingVals, ...USWrenchSizingVals])
export const hexkey_sz = CK.oneof([...MetricHexkeySizingVals, ...USHexkeySizingVals])
export const torxkey_sz = CK.oneof(TorxkeySizingVals)
export const keydrive_sz = CK.oneof([...MetricHexkeySizingVals, ...USHexkeySizingVals, ...TorxkeySizingVals])
export const driver_sz = CK.oneof(DriverSizingVals)

export type FastenerSizingPref = typeof FastenerSizingPrefVals[number]; const FastenerSizingPrefVals = ['A', 'B']                           as const; export const fastener_size_pref = CK.oneof(FastenerSizingPrefVals)
export type ThreadingPref      = typeof ThreadingPrefVals[number];      const ThreadingPrefVals      = ['a', 'b', 'c']                      as const; export const thread_pref       = CK.oneof(ThreadingPrefVals)
export type FastenerPref       = typeof FastenerPrefVals[number];       const FastenerPrefVals       = ['Aa', 'Ab', 'Ac', 'Ba', 'Bb', 'Bc'] as const; export const fastener_pref        = CK.oneof(FastenerPrefVals)

export const USFastenerSizingVals     = ["#0",  "#2",  "#3",  "#4",  "#6",  "#8",  "#10",  "#12",  "1/4in",  "5/16in",  "3/8in",  "7/16in",  "1/2in",  "9/16in",  "5/8in",  "3/4in",  "7/8in",  "1in",  "1+1/8in",  "1+1/4in",  "1+3/8in",  "1+1/2in",  "1+3/4in",  "2in",  "2+1/4in",  "2+1/2in",  "2+3/4in",  "3in"] as const
export const USNutSizingVals          = ["N#0",  "N#2",  "N#3",  "N#4",  "N#6",  "N#8",  "N#10",  "N#12",  "N1/4in",  "N5/16in",  "N3/8in",  "N7/16in",  "N1/2in",  "N9/16in",  "N5/8in",  "N3/4in",  "N7/8in",  "N1in",  "N1+1/8in",  "N1+1/4in",  "N1+3/8in",  "N1+1/2in",  "N1+3/4in",  "N2in",  "N2+1/4in",  "N2+1/2in",  "N2+3/4in",  "N3in"] as const
export const USHeavyNutSizingVals     = ["HN#0", "HN#2", "HN#3", "HN#4", "HN#6", "HN#8", "HN#10", "HN#12", "HN1/4in", "HN5/16in", "HN3/8in", "HN7/16in", "HN1/2in", "HN9/16in", "HN5/8in", "HN3/4in", "HN7/8in", "HN1in", "HN1+1/8in", "HN1+1/4in", "HN1+3/8in", "HN1+1/2in", "HN1+3/4in", "HN2in", "HN2+1/4in", "HN2+1/2in", "HN2+3/4in", "HN3in"] as const
export const MetricFastenerSizingVals = ["M1.4",   "M1.6",   "M2",   "M2.5",   "M3",   "M4",   "M5",   "M6",   "M8",   "M10",   "M12",   "M14",   "M16",   "M18",   "M20",   "M22",   "M24",   "M27",   "M30",   "M33",   "M36",   "M39",   "M42"] as const
export const MetricNutSizingVals      = ["NM1.4",  "NM1.6",  "NM2",  "NM2.5",  "NM3",  "NM4",  "NM5",  "NM6",  "NM8",  "NM10",  "NM12",  "NM14",  "NM16",  "NM18",  "NM20",  "NM22",  "NM24",  "NM27",  "NM30",  "NM33",  "NM36",  "NM39",  "NM42"] as const
export const MetricHeavyNutSizingVals = ["HNM1.4", "HNM1.6", "HNM2", "HNM2.5", "HNM3", "HNM4", "HNM5", "HNM6", "HNM8", "HNM10", "HNM12", "HNM14", "HNM16", "HNM18", "HNM20", "HNM22", "HNM24", "HNM27", "HNM30", "HNM33", "HNM36", "HNM39", "HNM42"] as const
export const WeirdFastenerSizingVals  = ["D8", "D10", "D12", "D14", "ND8", "ND10", "ND12", "ND14", "HND8", "HND10", "HND12", "HND14", "HN3/8in", "HN1/2in", "HN3/4in", "HN7/8in", "HN1in", "HN1+1/8in", "HN1+1/4in", "HN1+3/8in", "HN1+1/2in", "HN1+3/4in", "HN2in", "HN2+1/4in", "HN2+1/2in", "HN2+3/4in", "HN3in"] as const
export const FastenerSizingVals       = [...USFastenerSizingVals, ...MetricFastenerSizingVals] as const
export const usFastenerSizing         = CK.oneof(USFastenerSizingVals)
export const metricFastenerSizing     = CK.oneof(MetricFastenerSizingVals)
export const weirdFastenerSizing      = CK.oneof(WeirdFastenerSizingVals)
export const fastenerSizing           = CK.oneof(FastenerSizingVals)
export const anyFastenerSizing        = CK.oneof([...USFastenerSizingVals, ...MetricFastenerSizingVals, ...WeirdFastenerSizingVals, ...USNutSizingVals, ...USHeavyNutSizingVals, ...MetricNutSizingVals, ...MetricHeavyNutSizingVals])
export type  USFastenerSizing         = typeof USFastenerSizingVals[number]
export type  MetricFastenerSizing     = typeof MetricFastenerSizingVals[number]
export type  WeirdFastenerSizing      = typeof WeirdFastenerSizingVals[number]
export type  FastenerSizing           = typeof FastenerSizingVals[number]