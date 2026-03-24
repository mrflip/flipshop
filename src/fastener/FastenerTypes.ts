import type { Optionalize } from '../utils/TSTools.ts'
import       { CK }         from '@freeword/meta'
import       * as FE        from './FastenerEnums.ts'

export type MM         = number
export type Inch       = number
export type Title      = string
export type SKU        = string
export const MM_IN     = 25.4
export const KG_LB     = 0.45359237
export const mm_lte_100 = CK.ustrnum.pipe(CK.num.gt(0).max(100))    // 3.94 in
export const mm_lte_360 = CK.ustrnum.pipe(CK.num.gt(0).max(360))    // 14.17 in
export const mm_lte_1000 = CK.ustrnum.pipe(CK.num.gt(0).max(1000))  // 39.37 in
export const mm_lte_1200 = CK.ustrnum.pipe(CK.num.gt(0).max(1200))  // 47.24 in
export const mm_lte_2000 = CK.ustrnum.pipe(CK.num.gt(0).max(2000))  // 78.74 in
export const in_lte_4   = CK.ustrnum.pipe(CK.num.gt(0).max(4))      // 102 mm
export const in_lte_14  = CK.ustrnum.pipe(CK.num.gt(0).max(14))     // 356 mm
export const in_lte_36  = CK.ustrnum.pipe(CK.num.gt(0).max(36))     // 914 mm
export const in_lte_48  = CK.ustrnum.pipe(CK.num.gt(0).max(48))     // 1219 mm
export const in_lte_80  = CK.ustrnum.pipe(CK.num.gt(0).max(80))     // 2032 mm

export const taphole = CK.obj({
  nonfe_diam: mm_lte_100,
  fe_diam:    mm_lte_100,
  pla_diam:   mm_lte_100,
  petg_diam:  mm_lte_100,
  fe_drill:   FE.drill_title.optional(),
  nonfe_drill: FE.drill_title.optional(),
})
export interface TapholeSk extends CK.Zsketch<typeof taphole> {}
export interface TapholeT extends CK.Zcasted<typeof taphole> {}

export const thruhole = CK.obj({
  close_diam: mm_lte_100,
  reg_diam: mm_lte_100,
  loose_diam: mm_lte_100,
  close_drill: FE.drill_title.optional(),
  reg_drill:   FE.drill_title.optional(),
  loose_drill: FE.drill_title.optional(),
})
export interface ThruholeSk extends CK.Zsketch<typeof thruhole> {}
export interface ThruholeT extends CK.Zcasted<typeof thruhole> {}

export const nut = CK.obj({
  driver_sz: FE.wrench_sz,
  diam_af:      mm_lte_100,
  ht:           mm_lte_100,
  refsku:       CK.keyish.optional(),
})
export type NutSk = CK.Zsketch<typeof nut>
export type NutT  = CK.Zcasted<typeof nut>

export const washer = CK.obj({
  diam_od: mm_lte_100,
  diam_id: mm_lte_100,
  ht:      mm_lte_100,
  stdz:    FE.washer_stdz.optional(),
  refsku:  CK.keyish.optional(),
})
export type WasherSk = CK.Zsketch<typeof washer>
export type WasherT  = CK.Zcasted<typeof washer>

export const screw = CK.obj({
  drive_kind:   FE.fastener_drive,
  head_form:    FE.head_form,
  driver_sz: FE.driver_sz,
  head_ht:      mm_lte_100,
  refsku:       CK.keyish.optional(),
})
export const external_drive_screw = CK.obj({
  ...screw.shape,
  drive_kind:   FE.external_drive,
  head_form:    CK.oneof(['bolt']).default('bolt'),
  driver_sz: FE.wrench_sz,
  head_diam_af: mm_lte_100,
})
export const internal_drive_screw = CK.obj({
  ...screw.shape,
  drive_kind:   FE.internal_drive,
  driver_sz: FE.keydrive_sz,
  head_diam_od: mm_lte_100,
  key_diam_af:  mm_lte_100,
  key_dp:       mm_lte_100.optional(),
})
export const hhcs   = CK.obj({ ...external_drive_screw.shape, head_form: CK.literal('bolt').default('bolt'),         drive_kind: CK.literal('exthex').default('exthex') })
export const shcs   = CK.obj({ ...internal_drive_screw.shape, head_form: CK.literal('socket').default('socket'),     drive_kind: CK.literal('inthex').default('inthex') })
export const bhcs   = CK.obj({ ...internal_drive_screw.shape, head_form: CK.literal('button').default('button'),     drive_kind: CK.literal('inthex').default('inthex') })
export const fhcs   = CK.obj({ ...internal_drive_screw.shape, head_form: CK.literal('flathead').default('flathead'), drive_kind: CK.literal('inthex').default('inthex') })
export const losock = CK.obj({ ...internal_drive_screw.shape, head_form: CK.literal('losock').default('losock'),     drive_kind: CK.literal('inthex').default('inthex') })
export const torx   = CK.obj({ ...internal_drive_screw.shape, head_form: CK.literal('socket').default('socket'),     drive_kind: CK.literal('torx').default('torx') })
export const sss    = CK.obj({ ...internal_drive_screw.shape, head_form: CK.literal('setscrew').default('setscrew'), drive_kind: CK.literal('inthex').default('inthex') })

export interface ScrewSk extends CK.Zsketch<typeof screw> {
  drive_kind:   FE.FastenerDrive
  head_form:    FE.HeadForm
}
export interface ScrewT<TDK extends FE.FastenerDrive, THF extends FE.HeadForm> extends CK.Zcasted<typeof screw> {
  drive_kind:   TDK
  head_form:    THF
}
export interface ExternalDriveScrewT<TDK extends FE.ExternalDrive = FE.ExternalDrive, THF extends 'bolt' = 'bolt'> extends ScrewT<TDK, THF>, CK.Zcasted<typeof external_drive_screw> {
  drive_kind:   TDK
  head_form:    THF
  driver_sz: FE.WrenchSizing
}

export interface ExternalDriveScrewSk<TDK extends FE.ExternalDrive = FE.ExternalDrive, THF extends 'bolt' = 'bolt'> extends Optionalize<ScrewSk, 'head_form'>, CK.Zsketch<typeof external_drive_screw> {
  drive_kind:    TDK
  head_form?:    THF | undefined
  driver_sz: FE.WrenchSizing
}
export interface InternalDriveScrewT<TDK extends FE.InternalDrive = FE.InternalDrive, THF extends FE.HeadForm = 'socket'> extends ScrewT<TDK, THF>, CK.Zcasted<typeof internal_drive_screw> {
  drive_kind:   TDK
  head_form:    THF
  driver_sz: FE.KeydriveSizing
}

export const threading = CK.obj({
  title:        CK.titleish,
  stdz:         FE.thread_stdz,
  /** How commonly found this thread is: 'a' for UNC / ISOC (and for UNF-only #0); 'b' for UNF and the most common finer-pitch ISO; 'c' for UNEF and any other ISO threadings. */
  thread_pref:  FE.thread_pref,
  /** Pitch of thread (spacing between threads)*/
  pitch:        mm_lte_100,
  /** Minor diameter of male thread */
  diam_minor:   mm_lte_100.optional(),
  /** Bore Hole Diameter for tapping (ferrous/non-ferrous) or self-tapping (pla/petg/etc). Follows ISO 261/965 (metric) or ASME B1.1 (US) */
  taphole,
})
export interface ThreadingSk extends CK.Zsketch<typeof threading> {}
export interface ThreadingT extends CK.Zcasted<typeof threading> {}

export interface BoltSocketT {
  title:        Title
  pts:          6 | 8 | 12
  diam_af:      MM
  diam_od:      MM
  drive:        FE.ToolDrive
}
export interface SurfaceT {
  title:        Title
  diam_od:      MM
  diam_af:      MM
  profile:      'circle' | 'rectangle' | 'hexagon'
}
export interface ExtrudedT extends SurfaceT {
  ht:          MM
}
export interface IntrudedT extends ExtrudedT {
  int_dp:      MM
}

export const   fastener_sizing = CK.obj({
  title:        CK.titleish,
  /** How commonly found this sizing is. For metric, uses ISO 261; For US, refers to common practice */
  size_pref:  FE.fastener_size_pref,
  /** Coarse thread details: UNC or the primary ISO thread */
  coarse:       threading,
  /** Fine thread details: UNF or the most-common secondary ISO thread */
  fine:         threading.nullable().optional(),
  /** Extra fine thread details: UNEF or the next-most-common ISO thread */
  xfine:        threading.nullable().optional(),
  diam_major:   mm_lte_100,
  /** Bore hole size to pass through: close, regular, loose; For metric, follows ISO 273 (metric) or ASME B18.2.8 (US) */
  thruhole,
  /** Hex nut details. For US sizes, uses the Finished Hex Nut or Machine Nut according to common practice */
  hexnut:       nut.optional(),
  /** Square nut details. These are not perfectly standardized */
  sqnut:        nut.optional(),
  /** Hex bolt (Hex-Head Cap Screw) details, according to ASME B18.2.1 (US) and ISO 262 (metric) */
  hhcs:        hhcs.optional(),
  /** Socket head cap screw (SHCS, aka "inthex Head") details, according to ASME B18.3 / ASTM F835 (US) and ISO 4753 (metric) */
  shcs:        shcs.optional(),
  /** Button head cap screw (BHCS, aka "Button Head") details */
  bhcs:         bhcs.optional(),
  /** Flat head cap screw (FHCS, aka "Flat Head") details */
  fhcs:         fhcs.optional(),
  /** Low-profile socket head cap screw details */
  losock:       losock.optional(),
  /** Torx head cap screw details */
  torx:         torx.optional(),
  /** Socket-set screw (SSS, aka "Set Screw" or "Grub Screw") details */
  sss:          sss.partial().optional(),
  /** Small washer details */
  fw_sm:        washer.optional(),
  /** Regular washer details */
  fw_reg:       washer.optional(),
  /** Large (aka "Fender") washer details */
  fw_lg:        washer.optional(),
})

export interface FastenerSizingSk extends CK.Zsketch<typeof fastener_sizing> {}
export interface FastenerSizingT extends CK.Zcasted<typeof fastener_sizing> {
  // hhcs:     Optionalize<ExternalDriveScrewT<'exthex', 'bolt'>,     'drive_kind' | 'head_form'>
  // shcs:     Optionalize<InternalDriveScrewT<'inthex',  'socket'>,   'drive_kind' | 'head_form'>
  // bhcs?:    Optionalize<InternalDriveScrewT<'inthex',  'button'>,   'drive_kind' | 'head_form'> | undefined
  // fhcs?:    Optionalize<InternalDriveScrewT<'inthex',  'flathead'>, 'drive_kind' | 'head_form'> | undefined
  // losock?:  Optionalize<InternalDriveScrewT<'inthex',  'losock'>,   'drive_kind' | 'head_form'> | undefined
  // torx?:    Optionalize<InternalDriveScrewT<'torx',   'socket'>,   'drive_kind' | 'head_form'> | undefined
  // sss?:     Optionalize<InternalDriveScrewT<'inthex',  'setscrew'>, 'drive_kind' | 'head_form'> | undefined
}

export interface FastenerFlatPropsT extends Omit<FastenerSizingSk, 'coarse' | 'fine' | 'xfine'>, ThreadingSk { threading_kind: 'coarse' | 'fine' | 'xfine' }