import      _                                   /**/ from 'lodash'

import type {
  FastenerSizingT, Title, ThreadingT, TapholeT, ThruholeT,
  ThreadingPref, FastenerPref, MM, Inch, SKU,
  ScrewT,  ExternalDriveScrewT, InternalDriveScrewT, NutT,
} from './FastenerTypes.ts'
import { MM2Inch } from './FastenerTypes.ts'
import * as FE from './FastenerEnums.ts'

export class Thing {
  constructor(props: object) { Object.assign(this, _.omitBy(props, _.isNil)) }
}

export class FastenerSizing implements FastenerSizingT {
  declare title:        Title
  declare sizing_pref:  'A' | 'B'
  get main():           Threading { return (this.title === '#0') ? this.fine! : this.coarse }
  declare coarse:       Threading
  declare fine:         Threading | null
  declare xfine:        Threading | null
  declare diam_major:   MM
  get diam_major_in():  Inch { return this.diam_major / MM2Inch }

  declare thruhole:     ThruholeT

  declare hn:           NutT
  declare sqn:          NutT
  declare hhcs:         ExternalDriveScrew<'exthex', 'bolt'>
  declare shcs:         InternalDriveScrew<'allen', 'socket'>
  declare bhcs?:        InternalDriveScrew<'allen', 'button'>
  declare fhcs?:        InternalDriveScrew<'allen', 'flathead'>
  declare losock?:      InternalDriveScrew<'allen', 'losock'>
  declare torx?:        InternalDriveScrew<'torx', 'socket'>
  declare sss?:         InternalDriveScrew<'allen', 'setscrew'>
}
export class Threading extends Thing implements ThreadingT {
  declare title:        Title
  declare stdz:         FE.ThreadingStandardization
  declare thread_pref:  ThreadingPref
  get pref():           FastenerPref { return this.thread_pref + this.sizing.sizing_pref as FastenerPref }
  declare sizing:       FastenerSizing
  declare pitch:        MM
  declare diam_minor:   MM; get diam_minor_in(): Inch { return this.diam_minor / MM2Inch }

  declare taphole:      TapholeT

  get diam_major():     Inch   { return this.sizing.diam_major }
  get diam_major_in():  Inch   { return this.sizing.diam_major_in }
  get tpi():            number { return MM2Inch / this.pitch }
}
export class Screw<TDK extends FE.FastenerDrive, THF extends FE.HeadForm> extends Thing implements ScrewT<TDK, THF> {
  declare drive_kind:   TDK
  declare head_form:    THF
  declare driver_title: FE.DriverTitle
  declare head_ht:      MM
  declare refsku?:      SKU | undefined
}

export class ExternalDriveScrew<TDK extends FE.ExternalDrive = FE.ExternalDrive, THF extends 'bolt' = 'bolt'> extends Screw<TDK, THF> implements ExternalDriveScrewT<TDK, THF> {
  declare driver_title: FE.WrenchTitle
  declare head_diam_af: MM
  declare head_ht: MM
}
export class InternalDriveScrew<TDK extends FE.InternalDrive, THF extends FE.HeadForm> extends Screw<TDK, THF> implements InternalDriveScrewT<TDK, THF> {
  declare driver_title: FE.KeydriveTitle
  declare head_diam_od: MM
  declare key_diam_af:  MM
  declare key_dp?:      MM | undefined
}
export class Thruhole extends Thing implements ThruholeT {
  declare loose_diam: MM
  declare reg_diam: MM
  declare close_diam: MM
  declare loose_drill?: FE.DrillTitle | undefined
  declare reg_drill?:   FE.DrillTitle | undefined
  declare close_drill?: FE.DrillTitle | undefined
}

export class DrillBit  extends Thing {
  declare title:        Title
  declare diam_od:      MM
  get diam_od_in():     Inch { return this.diam_od / MM2Inch }
}