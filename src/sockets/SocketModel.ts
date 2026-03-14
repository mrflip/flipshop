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
  static get checker() { return socketWrench }
  declare title:                TY.Title
  declare sku:                  TY.SKU
  declare upc:                  TY.SKU
  declare url:                  TY.URLStr
  declare img_url:              TY.URLStr
  declare size_nom:             string
  declare socket_kind?:         FE.SocketKind        | undefined
  declare sqdrive_size?:        FE.ToolDrive         | undefined
  declare drive_kind?:          FE.FastenerDrive     | undefined
  declare bit_kind?:            FE.InternalDrive     | undefined
  declare reach_kind?:          FE.SocketReach       | undefined
  declare male_drive_size?:     FE.ToolDrive         | undefined
  declare female_drive_size?:   FE.ToolDrive         | undefined
  declare ln_overall?:          TY.MM                | undefined
  declare wd_overall?:          TY.MM                | undefined
  declare ht_overall?:          TY.MM                | undefined
  declare bit_ln?:              TY.MM                | undefined
  declare bit_ln_exposed?:      TY.MM                | undefined
  declare nose_diam?:           TY.MM                | undefined
  declare drive_end_ln?:        TY.MM                | undefined
  declare wt?:                  TY.MM                | undefined
  declare wt_lb?:               TY.MM                | undefined
  declare shoulder_ln?:         TY.MM                | undefined
  declare wrench_end_ln?:       TY.MM                | undefined
  declare wrench_dp?:           TY.MM                | undefined
  declare drive_end_hex_af?:    TY.MM                | undefined
  declare bolt_clr?:            TY.MM                | undefined
  declare bolt_depth?:          TY.MM                | undefined
}
