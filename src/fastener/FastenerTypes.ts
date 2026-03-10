import type { Optionalize } from '../utils/TSTools.ts'
import * as CK from '@freeword/meta/checks.js'
import * as FE from './FastenerEnums.ts'

export type MM         = number
export type Inch       = number
export type Title      = string
export type SKU        = string
export const MM2Inch = 25.4

export const FastenerMasterDataCols = [
  'title', 'nom_size', 'common', 'pitch', 'tpi', 'maj_diam', 'maj_diam_in', 'spec',
  'thruhole_close', 'thruhole_reg', 'thruhole_loose', 'selftap_pla', 'selftap_petg', 'taphole_nonfe', 'taphole_fe', 'thruhole_close_drill', 'thruhole_normal_drill', 'thruhole_loose_drill', 'taphole_nonfe_drill', 'taphole_fe_drill',
  'hn_sku', 'hn_af', 'hn_thk',
  'sqn_af', 'sqn_thk',
  'fw_id_sm', 'fw_od_sm', 'fw_thk_sm', 'fw_std_sm',
  'fw_id_reg', 'fw_od_reg', 'fw_thk_reg', 'fw_std_reg',
  'fw_id_lg', 'fw_od_lg', 'fw_thk_lg', 'fw_std_lg',
  'shcs_key', 'bhcs_key', 'fhcs_key', 'shcslo_key', 'setscrew_key', 'shcs_key_af', 'shcs_key_dp', 'nom_size',
  'hn_af_nom', 'hn_thk_nom', 'hn_sstd',
  'hhcs_wrench', 'hhcs_head_af', 'shcs_head_diam', 'shcslo_head_diam', 'bhcs_head_diam', 'fhcs_head_diam', 'shcs_head_thk', 'shcslo_head_thk', 'hhcs_head_thk', 'bhcs_head_thk', 'fhcs_head_thk', 'fhcs_head_angle', 'shcs_bore_diam', 'shcslo_bore_diam', 'bhcs_bore_diam', 'shcs_bore_dp', 'shcslo_bore_dp', 'boss_wall_thk_min', 'boss_wall_thk', 'engagement_len_min',
] as const;

export const FastenerProps = {
  '1/4': {
    title: '1/4-20', sizing_pref: 'A',
    diam_major: 0.250,
    coarse: {
      title: '1/4-20', stdz: 'UNC', pitch: MM2Inch / 20, diam_minor: 0.1905 * MM2Inch, thread_pref: 'a', pref: 'Aa',
      taphole:     { nonfe_diam: 11.11, fe_diam: 11.11, pla_diam: 11.11, petg_diam: 11.11,  },
    },
    fine: {
      title: '1/4-28', stdz: 'UNF', pitch: MM2Inch / 28, diam_minor: 0.2074 * MM2Inch, thread_pref: 'b', pref: 'Ab',
      taphole:     { nonfe_diam: 11.11, fe_diam: 11.11, pla_diam: 11.11, petg_diam: 11.11,  },
    },
    xfine: {
      title: '1/4-32', stdz: 'UNEF', pitch: MM2Inch / 32, diam_minor: 0.2128 * MM2Inch, thread_pref: 'c', pref: 'Ac',
      taphole:     { nonfe_diam: 11.11, fe_diam: 11.11, pla_diam: 11.11, petg_diam: 11.11,  },
    },
    hhcs:     { driver_title: 'Wr7/16in', head_diam_af: 11.11, head_ht: 4.14 },
    shcs:     { driver_title: 'H3/16in',  head_diam_od: 11.11, head_ht: 4.14, key_diam_af: 11.11, key_dp: 11.11 },
    hn:       { diam_af: 11.11,      ht: 5.56,      refsku:       'mcmc_91845A029' },
    sqn:      { diam_af: 11.11,      ht: 4.14,      refsku:       'mcmc_94855A247' },
    // fw_reg:   {},
    // fw_lg:    {},
    // shcs:     {},
    // bhcs:     {},
    // fhcs:     {},
    // shlo:     {},
    // setscrew: {},
    // Washer
    fw_sm:    { diam_od: 11.11, diam_id: 11.11, ht: 11.11, stdz: 'USS' },
    //
    thruhole:    { close_diam: 11.11, reg_diam: 11.11, loose_diam: 11.11 },
  },
} satisfies Record<Title, FastenerSizingT>

export const nut = CK.obj({
  diam_af: CK.num.gt(0).max(100),
  ht:      CK.num.gt(0).max(100),
  refsku:  CK.keyish.optional(),
})
export type NutSk = CK.Zsketch<typeof nut>
export type NutT  = CK.Zcasted<typeof nut>

export const washer = CK.obj({
  diam_od: CK.num.gt(0).max(100),
  diam_id: CK.num.gt(0).max(100),
  ht:      CK.num.gt(0).max(100),
  stdz:    FE.washer_stdz.optional(),
  refsku:  CK.keyish.optional(),
})
export type WasherSk = CK.Zsketch<typeof washer>
export type WasherT  = CK.Zcasted<typeof washer>

export interface ScrewT<TDK extends FE.FastenerDrive, THF extends FE.HeadForm> {
  drive_kind:   TDK
  head_form:    THF
  driver_title: FE.DriverTitle
  head_ht:     MM
  refsku?:      SKU | undefined
}
export interface ExternalDriveScrewT<TDK extends FE.ExternalDrive = FE.ExternalDrive, THF extends 'bolt' = 'bolt'> extends ScrewT<TDK, THF> {
  driver_title: FE.WrenchTitle
  head_diam_af: MM
}
export interface InternalDriveScrewT<TDK extends FE.InternalDrive = FE.InternalDrive, THF extends FE.HeadForm = 'socket'> extends ScrewT<TDK, THF> {
  driver_title: FE.KeydriveTitle
  head_diam_od: MM
  key_diam_af:  MM
  key_dp?:      MM | undefined
}

export interface FastenerSizingT {
  title:        Title
  /** How commonly found this sizing is. For metric, uses ISO 261; For US, refers to common practice */
  sizing_pref:  FastenerSizingPref
  /** Coarse thread details: UNC or the primary ISO thread */
  coarse:       ThreadingT
  /** Fine thread details: UNF or the most-common secondary ISO thread */
  fine:         ThreadingT | null
  /** Extra fine thread details: UNEF or the next-most-common ISO thread */
  xfine:        ThreadingT | null
  /** Major diameter of male thread (outside diameter) */
  diam_major:   MM
  //
  /** Bore hole size to pass through: close, regular, loose; For metric, follows ISO 273 (metric) or ASME B18.2.8 (US) */
  thruhole: { close_diam: MM, reg_diam: MM, loose_diam: MM, close_drill?: FE.DrillTitle | undefined, reg_drill?: FE.DrillTitle | undefined, loose_drill?: FE.DrillTitle | undefined }
  //
  /** Hex nut details. For US sizes, uses the Finished Hex Nut or Machine Nut according to common practice */
  hn:       NutT
  /** Square nut details. These are not perfectly standardized */
  sqn:      NutT
  /** Hex bolt (Hex-Head Cap Screw) details, according to ASME B18.2.1 (US) and ISO 262 (metric) */
  hhcs:     Optionalize<ExternalDriveScrewT<'exthex', 'bolt'>,     'drive_kind' | 'head_form'>
  /** Socket head cap screw (SHCS, aka "Allen Head") details, according to ASME B18.3 / ASTM F835 (US) and ISO 4753 (metric) */
  shcs:     Optionalize<InternalDriveScrewT<'allen',  'socket'>,   'drive_kind' | 'head_form'>
  bhcs?:    Optionalize<InternalDriveScrewT<'allen',  'button'>,   'drive_kind' | 'head_form'> | undefined
  fhcs?:    Optionalize<InternalDriveScrewT<'allen',  'flathead'>, 'drive_kind' | 'head_form'> | undefined
  losock?:  Optionalize<InternalDriveScrewT<'allen',  'losock'>,   'drive_kind' | 'head_form'> | undefined
  torx?:    Optionalize<InternalDriveScrewT<'torx',   'socket'>,   'drive_kind' | 'head_form'> | undefined
  sss?:     Optionalize<InternalDriveScrewT<'allen',  'setscrew'>, 'drive_kind' | 'head_form'> | undefined
  fw_sm?:   WasherT | undefined
  fw_reg?:  WasherT | undefined
  fw_lg?:   WasherT | undefined

}
export type FastenerSizingPref = typeof FastenerSizingPrefVals[number]; const FastenerSizingPrefVals = ['A', 'B'] as const
export type ThreadingPref      = typeof ThreadingPrefVals[number];      const ThreadingPrefVals      = ['a', 'b', 'c'] as const
export type FastenerPref       = typeof FastenerPrefVals[number];       const FastenerPrefVals       = ['Aa', 'Ab', 'Ac', 'Ba', 'Bb', 'Bc'] as const

export interface ThreadingT {
  title:        Title
  stdz:         FE.ThreadingStandardization
  /** How commonly found this thread is: 'a' for UNC / ISOC (and for UNF-only #0); 'b' for UNF and the most common finer-pitch ISO; 'c' for UNEF and any other ISO threadings. */
  thread_pref:  ThreadingPref
  pref:         FastenerPref
  /** Pitch of thread (spacing between threads)*/
  pitch:        MM
  /** Minor diameter of male thread */
  diam_minor:   MM
  /** Bore Hole Diameter for tapping (ferrous/non-ferrous) or self-tapping (pla/petg/etc). Follows ISO 261/965 (metric) or ASME B1.1 (US) */
  taphole:  { pla_diam: MM,  petg_diam: MM, nonfe_diam: MM, fe_diam: MM, fe_drill?: FE.DrillTitle | undefined, nonfe_drill?: FE.DrillTitle | undefined }
}
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

export interface TapholeT {
  nonfe_diam: MM
  fe_diam:    MM
  pla_diam:   MM
  petg_diam:  MM
}

export interface ThruholeT {
  close_diam: MM
  reg_diam: MM
  loose_diam: MM
  close_drill?: FE.DrillTitle | undefined
  reg_drill?: FE.DrillTitle | undefined
  loose_drill?: FE.DrillTitle | undefined
}