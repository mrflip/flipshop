import      _                                   /**/ from 'lodash'
import      { CK }                                   from '@freeword/meta'

export type DrillTitle = string
export const drill_title = CK.extkeyish

/** Drive type for square (1/4", 3/8", 1/2") and hex (1/4") drives */
export const ToolDriveVals   = ['isq_0250in', 'isq_0375in', 'isq_0500in', 'hex_0250in', 'isq_0750in', 'isq_1000in'] as const
/** Socket kind (socket_sparkplug, socket_exthex, socket_extstar, socket_bit, socket_flex, socket_extension, socket_ujoint, socket_adapter) */
export const SocketKindVals  = [
  'socket_sparkplug', 'socket_exthex', 'socket_extstar', 'socket_bit', 'socket_flex',
  'socket_extension', 'socket_ujoint', 'socket_adapter',
] as const
/** Socket variant (standard, impact, ball) */
export const SocketVariantVals = ['std', 'impact', 'ball'] as const
/** Socket reach (standard, midlen, deep, long, xlong, other) */
export const SocketReachVals = ['std', 'midlen', 'deep', 'long', 'xlong', 'other', 'uj_std', 'uj_deep', 'uj_ext'] as const
/** Unit system (metric, us) */
export const UnitSystemVals = ['metric', 'us'] as const

/** Tool drive (isq_0250in, isq_0375in, isq_0500in, hex_0250in, isq_0750in, isq_1000in) */
export const tool_drive      = CK.oneof(ToolDriveVals)
/** Socket kind (socket_sparkplug, socket_exthex, socket_extstar, socket_bit, socket_flex, socket_extension, socket_ujoint, socket_adapter) */
export const socket_kind     = CK.oneof(SocketKindVals)
/** Socket reach (standard, midlen, deep, long, xlong, other) */
export const socket_reach    = CK.oneof(SocketReachVals)
/** Socket variant (standard, impact, ball) */
export const socket_variant  = CK.oneof(SocketVariantVals)
/** Unit system (metric, us) */
export const unit_system   = CK.oneof(UnitSystemVals)

export type  ToolDrive       = typeof ToolDriveVals[number]
export type SocketKind       = typeof SocketKindVals[number]
export type SocketReach      = typeof SocketReachVals[number]
export type SocketVariant    = typeof SocketVariantVals[number]
export type UnitSystem       = typeof UnitSystemVals[number]

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
  /* slotted */                                   'slotted',
  /* square */                                    'square',
  /* phillips */                                  'phillips',
  /* pozidriv */                                  'pozidriv',
  /* phillips-profile cross drive */              'phillips',
  /* combo phillips/slotted */                    'phslot',
  /* "Flat" screwdriver */                        'slot',
] as const
export const internal_drive = CK.oneof(InternalDriveVals)
export const OtherFastenerDriveVals = [
  /* Knurled Thumb drive */                       'knurled',
  /* Deforming Square Keying */                   'carriage',
] as const
export const other_fastener_drive = CK.oneof(OtherFastenerDriveVals)
export const FastenerDriveVals = [...ExternalDriveVals, ...InternalDriveVals, ...OtherFastenerDriveVals] as const
export const fastener_drive = CK.oneof(FastenerDriveVals)

export const MetricWrenchSizingVals = ['Wr2.5mm',  'Wr3mm',    'Wr3.2mm',   'Wr4mm',    'Wr5mm',    'Wr5.5mm',  'Wr7mm',    'Wr8mm',    'Wr10mm',    'Wr13mm',    'Wr17mm',     'Wr19mm', 'Wr24mm',  'Wr30mm',  'Wr36mm']                           as const
export const USWrenchSizingVals     = ['Wr5/32in', 'Wr1/4in',  'Wr5/16in', 'Wr11/32in', 'Wr3/8in',  'Wr7/16in', 'Wr1/2in',  'Wr9/16in', 'Wr3/4in',  'Wr15/16in', 'Wr1in', 'Wr1+1/8in', 'Wr1+5/16in', 'Wr1+1/2in']                                             as const
export const MetricHexkeySizingVals = ['H0.7mm',   'H0.9mm',   'H1.25mm',   'H1.3mm',   'H1.5mm',   'H2mm',     'H2.5mm',   'H3mm',     'H4mm',      'H5mm',      'H6mm',       'H7mm',   'H8mm',   'H10mm',   'H12mm',   'H14mm', 'H17mm', 'H19mm'] as const
export const USHexkeySizingVals     = ['H0.028in', 'H0.035in', 'H0.050in',  'H1/16in',   'H5/64in',  'H3/32in',  'H7/64in',  'H1/8in',  'H9/64in',   'H5/32in',   'H3/16in',   'H1/4in',    'H5/16in',    'H3/8in', 'H1/2in', 'H5/8in',  'H3/4in']   as const
export const TorxkeySizingVals      = ['T5',       'T6',       'T7',        'T8',       'T9',       'T10',      'T15',      'T20',      'T25',       'T27',       'T30',        'T40',    'T45',     'T50',     'T55',  'T60']                       as const
export const DriverSizingVals       = [...MetricWrenchSizingVals, ...USWrenchSizingVals, ...MetricHexkeySizingVals, ...USHexkeySizingVals, ...TorxkeySizingVals] as const
/** Wrench or hexkey driver name */
export type DriverSizing     = typeof DriverSizingVals[number]
export type WrenchSizing    = typeof MetricWrenchSizingVals[number] | typeof USWrenchSizingVals[number]
export type HexkeySizing    = typeof MetricHexkeySizingVals[number] | typeof USHexkeySizingVals[number]
export type TorxkeySizing   = typeof TorxkeySizingVals[number]
export type KeydriveSizing  = HexkeySizing | TorxkeySizing

export const driver_title = CK.oneof(DriverSizingVals)
export const wrench_title = CK.oneof([...MetricWrenchSizingVals, ...USWrenchSizingVals])
export const hexkey_title = CK.oneof([...MetricHexkeySizingVals, ...USHexkeySizingVals])
export const torxkey_title = CK.oneof(TorxkeySizingVals)
export const keydrive_title = CK.oneof([...MetricHexkeySizingVals, ...USHexkeySizingVals, ...TorxkeySizingVals])

export type FastenerSizingPref = typeof FastenerSizingPrefVals[number]; const FastenerSizingPrefVals = ['A', 'B']                           as const; export const fastener_size_pref = CK.oneof(FastenerSizingPrefVals)
export type ThreadingPref      = typeof ThreadingPrefVals[number];      const ThreadingPrefVals      = ['a', 'b', 'c']                      as const; export const thread_pref       = CK.oneof(ThreadingPrefVals)
export type FastenerPref       = typeof FastenerPrefVals[number];       const FastenerPrefVals       = ['Aa', 'Ab', 'Ac', 'Ba', 'Bb', 'Bc'] as const; export const fastener_pref        = CK.oneof(FastenerPrefVals)
