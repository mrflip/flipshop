import      _                                   /**/ from 'lodash'
import      { CK }                                   from '@freeword/meta'
import type { OmitStatics }                          from '@freeword/meta'

import type { MM, Inch, SKU, Title } from './FastenerTypes.ts'
import * as FT from './FastenerTypes.ts'
import * as FE from './FastenerEnums.ts'

export class Thing {
  constructor(raw: object) {
    const props = this.Factory.checker.cast(raw)
    Object.assign(this, _.omitBy(props, _.isNil))
  }
  get Factory(): typeof Thing { return this.constructor as typeof Thing }
  static get checker(): { cast: (raw: any) => any } { return CK.obj({ }) }
  static fill(raw: object) { return this.checker.cast(raw) }
  static live(raw: object) { return new this(this.fill(raw)) }
}

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
  declare shcs?:        InternalDriveScrew<'allen', 'socket'>
  declare bhcs?:        InternalDriveScrew<'allen', 'button'>
  declare fhcs?:        InternalDriveScrew<'allen', 'flathead'>
  declare losock?:      InternalDriveScrew<'allen', 'losock'>
  declare torx?:        InternalDriveScrew<'torx', 'socket'>
  declare sss?:         InternalDriveScrew<'allen', 'setscrew'>

  declare fw_sm:        Washer
  declare fw_reg:       Washer | undefined
  declare fw_lg:        Washer | undefined

  static get checker() { return FT.fastener_sizing }
  get Factory(): typeof FastenerSizing { return this.constructor as typeof FastenerSizing }
  static fill(raw: FT.FastenerSizingSk): FT.FastenerSizingT { return this.checker.cast(raw) }
  static live(raw: FT.FastenerSizingSk): FastenerSizing     { return super.live(raw) as FastenerSizing }
}
export class Threading extends Thing implements FT.ThreadingT {
  static get checker() { return FT.threading }
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
}
export class Screw<TDK extends FE.FastenerDrive, THF extends FE.HeadForm> extends Thing implements FT.ScrewT<TDK, THF> {
  static get checker() { return FT.screw as CK.Zchecker<FT.ScrewT<FE.FastenerDrive, FE.HeadForm>, any, FT.ScrewSk> }
  //
  declare drive_kind:   TDK
  declare head_form:    THF
  declare driver_title: FE.DriverTitle
  declare head_ht:      MM
  declare refsku?:      SKU | undefined
}

export class Washer extends Thing implements FT.WasherT {
  static get checker() { return FT.washer }
  //
  declare diam_od:      MM
  declare diam_id:      MM
  declare ht:           MM
  declare stdz?:        FE.WasherStandardization | undefined
  declare refsku?:      SKU | undefined
}

// type ScrewTX = OmitStatics<Screw<FE.FastenerDrive, FE.HeadForm>, 'checker'>
export class ExternalDriveScrew<TDK extends FE.ExternalDrive = FE.ExternalDrive, THF extends 'bolt' = 'bolt'> extends
  (Screw as OmitStatics<typeof Screw<FE.ExternalDrive, 'bolt'>, { checker: CK.Zchecker, drive_kind: FE.ExternalDrive, head_form: FE.HeadForm }>)
  implements FT.ExternalDriveScrewT<TDK, THF>
  {
  static get checker() { return FT.external_drive_screw }
  //
  declare driver_title:     FE.WrenchTitle
  declare drive_kind:       TDK
  declare head_form:        THF
  declare head_diam_af:     MM
  declare head_ht:          MM
}
export class InternalDriveScrew<TDK extends FE.InternalDrive, THF extends FE.HeadForm> extends Screw<TDK, THF> implements FT.InternalDriveScrewT<TDK, THF> {
  static get checker() { return FT.internal_drive_screw }
  //
  declare driver_title:     FE.KeydriveTitle
  declare drive_kind:       TDK
  declare head_form:        THF
  declare head_diam_od:     MM
  declare key_diam_af:      MM
  declare key_dp?:          MM | undefined
}
export class Thruhole extends Thing implements FT.ThruholeT {
  static get checker() { return FT.thruhole }
  //
  declare loose_diam:      MM
  declare reg_diam:        MM
  declare close_diam:      MM
  declare loose_drill?:    FE.DrillTitle | undefined
  declare reg_drill?:      FE.DrillTitle | undefined
  declare close_drill?:    FE.DrillTitle | undefined
}

export class DrillBit  extends Thing {
  declare title:        Title
  declare diam_od:      MM
  get diam_od_in():     Inch { return this.diam_od / FT.MM_IN }
  // static get checker() { return drill_bit }
}