import      _                                   /**/ from 'lodash'
//
import type { MM, Inch, SKU, Title }                 from './FastenerTypes.ts'
//
import      { Thing }                                from '../utils/Thing.ts'
import       * as FT                                 from './FastenerTypes.ts'
import       * as FE                                 from './FastenerEnums.ts'

export class FastenerSizing extends Thing implements FT.FastenerSizingT {
  declare title:        Title
  declare size_pref:    FE.FastenerSizingPref
  get main():           Threading { return (this.title === '#0') ? this.fine! : this.coarse }
  declare coarse:       Threading
  declare fine:         Threading | null
  declare xfine:        Threading | null
  declare diam_major:   MM
  get diam_major_in():  Inch { return this.diam_major / FT.MM_IN }

  declare thruhole:     FT.ThruholeT

  declare hexnut:       FT.NutT
  declare sqnut:        FT.NutT
  declare hhcs?:        ExternalDriveScrew<'exthex', 'bolt'>
  declare shcs?:        InternalDriveScrew<'inthex', 'socket'>
  declare bhcs?:        InternalDriveScrew<'inthex', 'button'>
  declare fhcs?:        InternalDriveScrew<'inthex', 'flathead'>
  declare losock?:      InternalDriveScrew<'inthex', 'losock'>
  declare torx?:        InternalDriveScrew<'torx', 'socket'>
  declare sss?:         InternalDriveScrew<'inthex', 'setscrew'>

  declare fw_sm:        Washer
  declare fw_reg:       Washer | undefined
  declare fw_lg:        Washer | undefined

  static get checker() { return FT.fastener_sizing }
  get Factory(): typeof FastenerSizing { return this.constructor as typeof FastenerSizing }
  static fill(raw: FT.FastenerSizingSk): FT.FastenerSizingT { return this.checker.cast(raw) }
  static live(raw: FT.FastenerSizingSk): FastenerSizing     { return super.live(raw) as FastenerSizing }
}
export class Threading extends Thing implements FT.ThreadingT {
  //
  declare title:        Title
  declare stdz:         FE.ThreadingStandardization
  declare thread_pref:  FE.ThreadingPref
  /** Fastener preference: 'Aa' for UNC / ISOC (and for UNF-only #0); 'Ab' for UNF and the most common finer-pitch ISO; 'Ac' for UNEF and any other ISO threadings. */
  get pref():           FE.FastenerPref { return this.thread_pref + this.sizing.size_pref as FE.FastenerPref }
  declare sizing:       FastenerSizing
  declare pitch:        MM
  declare diam_minor:   MM; get diam_minor_in(): Inch { return this.diam_minor / FT.MM_IN }

  declare taphole:      FT.TapholeT

  get diam_major():     Inch   { return this.sizing.diam_major }
  get diam_major_in():  Inch   { return this.sizing.diam_major_in }
  get tpi():            number { return FT.MM_IN / this.pitch }
  //
  static get checker() { return FT.threading }
  get Factory(): typeof Threading { return this.constructor as typeof Threading }
}
export class Screw<TDK extends FE.FastenerDrive, THF extends FE.HeadForm> extends Thing implements FT.ScrewT<TDK, THF> {
  //
  declare drive_kind:   TDK
  declare head_form:    THF
  declare driver_title: FE.DriverTitle
  declare head_ht:      MM
  declare refsku?:      SKU | undefined
}

export class Washer extends Thing implements FT.WasherT {
  //
  declare diam_od:      MM
  declare diam_id:      MM
  declare ht:           MM
  declare stdz?:        FE.WasherStandardization | undefined
  declare refsku?:      SKU | undefined
  //
  static get checker() { return FT.washer }
  get Factory(): typeof Washer { return this.constructor as typeof Washer }
}

// (Screw as OmitStatics<typeof Screw<FE.ExternalDrive, 'bolt'>, { checker: any, drive_kind: FE.ExternalDrive, head_form: FE.HeadForm }>)
export class ExternalDriveScrew<TDK extends FE.ExternalDrive = FE.ExternalDrive, THF extends 'bolt' = 'bolt'> extends Screw<TDK, THF> implements FT.ExternalDriveScrewT<TDK, THF> {
  //
  declare driver_title:     FE.WrenchTitle
  declare drive_kind:       TDK
  declare head_form:        THF
  declare head_diam_af:     MM
  declare head_ht:          MM
  //
  static get checker(): typeof FT.external_drive_screw { return FT.external_drive_screw }
  get Factory(): typeof ExternalDriveScrew { return this.constructor as typeof ExternalDriveScrew }
}
export class InternalDriveScrew<TDK extends FE.InternalDrive, THF extends FE.HeadForm> extends Screw<TDK, THF> implements FT.InternalDriveScrewT<TDK, THF> {
  //
  declare driver_title:     FE.KeydriveTitle
  declare drive_kind:       TDK
  declare head_form:        THF
  declare head_diam_od:     MM
  declare key_diam_af:      MM
  declare key_dp?:          MM | undefined
  //
  static get checker(): typeof FT.internal_drive_screw { return FT.internal_drive_screw }
  get Factory(): typeof InternalDriveScrew { return this.constructor as typeof InternalDriveScrew }
}
export class Thruhole extends Thing implements FT.ThruholeT {
  //
  declare loose_diam:      MM
  declare reg_diam:        MM
  declare close_diam:      MM
  declare loose_drill?:    FE.DrillTitle | undefined
  declare reg_drill?:      FE.DrillTitle | undefined
  declare close_drill?:    FE.DrillTitle | undefined
  //
  static get checker() { return FT.thruhole }
  get Factory(): typeof Thruhole { return this.constructor as typeof Thruhole }
}

export class DrillBit  extends Thing {
  declare title:        Title
  declare diam_od:      MM
  get diam_od_in():     Inch { return this.diam_od / FT.MM_IN }
}
