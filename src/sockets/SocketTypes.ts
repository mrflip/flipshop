import      _                                   /**/ from 'lodash'
import      { CK }                                   from '@freeword/meta'
import      * as FE                                  from '../fastener/FastenerEnums.ts'
import      { mm_lte_100, mm_lte_2000, mm_lte_360 }  from '../fastener/FastenerTypes.ts'

export const socketWrench = CK.obj({
  title:                CK.titleish,
  sku:                  CK.extkeyish,
  upc:                  CK.unumstr,
  url:                  CK.urlstr,
  img_url:              CK.urlstr,
  size_nom:             CK.str.regex(/^(?:((?:\d+-)?[\/\d]+ in|[\d\.]+ ?mm)|T\d+|E\d+|#[123]|#0+)$/),
  //
  socket_kind:          CK.oneof(FE.SocketKindVals),
  sqdrive_size:         CK.oneof(FE.ToolDriveVals),
  drive_kind:           CK.oneof(FE.FastenerDriveVals),
  bit_kind:             CK.oneof(FE.InternalDriveVals),
  reach_kind:           CK.oneof(FE.SocketReachVals),
  //
  male_drive_size:      CK.oneof(FE.ToolDriveVals),
  female_drive_size:    CK.oneof(FE.ToolDriveVals),
  //
  ln_overall:           mm_lte_2000,
  wd_overall:           mm_lte_100,
  ht_overall:           mm_lte_100,
  bit_ln:               mm_lte_360,
  bit_ln_exposed:       mm_lte_360,
  nose_diam:            mm_lte_100,
  drive_end_ln:         mm_lte_100,
  wt:                   mm_lte_100,
  wt_lb:                mm_lte_100,
  shoulder_ln:          mm_lte_360,
  wrench_end_ln:        mm_lte_2000,
  wrench_dp:            mm_lte_100,
  drive_end_hex_af:     mm_lte_100,
  bolt_clr:             mm_lte_360,
  bolt_depth:           mm_lte_100,
}).partial().required({ title: true, sku: true, url: true, img_url: true, size_nom: true }).strict()
export interface SocketWrenchT   extends CK.Zcasted<typeof socketWrench> {}
export interface SocketWrenchSk  extends CK.Zsketch<typeof socketWrench> {}
