import      _                                   /**/ from 'lodash'
import      { CK }                                   from '@freeword/meta'
// import type { OmitStatics }                          from '@freeword/meta'
//
import type * as TY                                  from './internal.ts'
//
import      { Thing }                                from '../utils/Thing.ts'
import       * as FE                                 from '../fastener/FastenerEnums.ts'
import       { socketWrench, type SocketWrenchT }    from './SocketTypes.ts'

export class SocketWrench extends Thing implements SocketWrenchT {
  //
  declare title:                TY.Title
  declare sku:                  TY.SKU
  declare upc:                  TY.SKU
  declare url:                  TY.URLStr
  declare img_url:              TY.URLStr
  declare size_nom:             string
  declare socket_kind:          FE.SocketKind
  declare sqdrive_size:         FE.ToolDrive
  declare drive_kind:           FE.FastenerDrive
  declare unit_system:          FE.UnitSystem
  declare socket_variant:       FE.SocketVariant
  declare reach_kind:           FE.SocketReach
  declare ln_overall?:          TY.MM                | undefined
  declare wx_overall?:          TY.MM                | undefined
  declare wy_overall?:          TY.MM                | undefined
  declare wt?:                  TY.MM                | undefined
  declare wt_lb?:               TY.MM                | undefined
  declare bit_kind?:            FE.InternalDrive     | undefined
  declare male_drive_size?:     FE.ToolDrive         | undefined
  declare female_drive_size?:   FE.ToolDrive         | undefined
  declare bit_ln?:              TY.MM                | undefined
  declare bit_ln_exposed?:      TY.MM                | undefined
  declare nose_diam?:           TY.MM                | undefined
  declare drive_end_diam?:      TY.MM                | undefined
  declare shoulder_ln?:         TY.MM                | undefined
  declare wrench_end_diam?:     TY.MM                | undefined
  declare wrench_dp?:           TY.MM                | undefined
  declare bolt_clr_diam?:       TY.MM                | undefined
  //
  static get checker()    { return socketWrench }
  get Factory():            typeof SocketWrench  { return this.constructor as typeof SocketWrench }
  static fill(raw: SocketWrenchT): SocketWrenchT { return super.fill(raw)  as        SocketWrenchT }
  static live(raw: SocketWrenchT): SocketWrench  { return super.live(raw)  as        SocketWrench  }
}
