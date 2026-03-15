import      _                                   /**/ from 'lodash'
import      { CK }                                   from '@freeword/meta'
// import type { OmitStatics }                          from '@freeword/meta'
//
import type * as TY                                  from './internal.ts'
//
import      { Thing }                                from '../utils/Thing.ts'
import      { fsStringField, fsBoolField, fsMmField, fsInchField, fsNumField } from '../utils/FeaturescriptHelpers.ts'
import       * as FE                                 from '../fastener/FastenerEnums.ts'
import       { socketWrench, type SocketWrenchT }    from './SocketTypes.ts'

export class SocketWrench extends Thing implements SocketWrenchT {
  //
  declare title:                TY.Title
  declare sizing:               string
  declare sizing_mm:            TY.MM
  declare sizing_in:            TY.Inch
  declare socket_kind:          FE.SocketKind
  declare sqdrive_size:         FE.ToolDrive
  declare drive_kind:           FE.FastenerDrive
  declare unit_system:          FE.UnitSystem
  declare socket_variant:       FE.SocketVariant
  declare reach_kind:           FE.SocketReach
  declare bit_kind?:            FE.InternalDrive     | undefined
  //
  declare ln_overall?:          TY.MM                | undefined
  declare wx_overall?:          TY.MM                | undefined
  declare wy_overall?:          TY.MM                | undefined
  declare drive_end_diam?:      TY.MM                | undefined
  declare wrench_end_diam?:     TY.MM                | undefined
  //
  declare bit_ln?:              TY.MM                | undefined
  declare bit_ln_total?:        TY.MM                | undefined
  declare nose_diam?:           TY.MM                | undefined
  declare shoulder_ln?:         TY.MM                | undefined
  declare wrench_dp?:           TY.MM                | undefined
  declare bolt_clr_diam?:       TY.MM                | undefined
  declare male_drive_size?:     FE.ToolDrive         | undefined
  declare female_drive_size?:   FE.ToolDrive         | undefined
  //
  declare wt?:                  TY.MM                | undefined
  declare wt_lb?:               TY.MM                | undefined
  declare sku:                  TY.SKU
  declare upc:                  TY.SKU
  declare url:                  TY.URLStr
  declare img_url:              TY.URLStr
  //
  static get checker()    { return socketWrench }
  get Factory():            typeof SocketWrench  { return this.constructor as typeof SocketWrench }
  static fill(raw: SocketWrenchT): SocketWrenchT { return super.fill(raw)  as        SocketWrenchT }
  static live(raw: SocketWrenchT): SocketWrench  { return super.live(raw)  as        SocketWrench  }

  private static readonly INCH_FIELDS  = new Set<string>(['sizing_in'])
  private static readonly PLAIN_FIELDS = new Set<string>(['wt', 'wt_lb'])

  toFeaturescript(): string {
    const data = this.flatten() as Record<string, unknown>
    const parts = Object.entries(data)
      .filter(([_k, vv]) => vv !== undefined && vv !== null)
      .map(([kk, vv]) => {
        if (typeof vv === 'string')  { return fsStringField(kk, vv) }
        if (typeof vv === 'boolean') { return fsBoolField(kk, vv) }
        if (typeof vv === 'number') {
          if (SocketWrench.INCH_FIELDS.has(kk))  { return fsInchField(kk, vv) }
          if (SocketWrench.PLAIN_FIELDS.has(kk)) { return fsNumField(kk, vv) }
          return fsMmField(kk, vv)
        }
        return fsStringField(kk, JSON.stringify(vv))
      })
    return `{ ${parts.join(', ')} }`
  }
}
