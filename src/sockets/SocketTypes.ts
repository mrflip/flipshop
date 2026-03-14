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
  //
  socket_kind:          CK.oneof(FE.SocketKindVals),
  drive_kind:           CK.oneof(FE.FastenerDriveVals),
  sqdrive_size:         CK.oneof(FE.ToolDriveVals),
  reach_kind:           CK.oneof(FE.SocketReachVals),
  size_nom:             CK.str.regex(/^(?:((?:\d+-)?[\/\d]+ in|[\d\.]+ ?mm)|T\d+|E\d+|#[123]|#0+)$/),
  //
  bit_kind:             CK.oneof(FE.InternalDriveVals).optional(),
  male_drive_size:      CK.oneof(FE.ToolDriveVals).optional(),
  female_drive_size:    CK.oneof(FE.ToolDriveVals).optional(),
  //
  ln_overall:           mm_lte_2000.optional(),
  wx_overall:           mm_lte_360.optional(),
  wy_overall:           mm_lte_360.optional(),
  wt:                   mm_lte_100.optional(),
  wt_lb:                mm_lte_100.optional(),
  bit_ln:               mm_lte_360.optional(),
  bit_ln_total:         mm_lte_360.optional(),
  /** diameter at the base (wrench end) */
  wrench_end_diam:      mm_lte_2000.optional(),
  /** diameter at the tip (drive end) */
  drive_end_diam:       mm_lte_100.optional(),
  /** length from drive tip to shoulder */
  shoulder_ln:          mm_lte_360.optional(),
  /** diameter at the nose for bit sockets (where the bit is inserted) */
  nose_diam:            mm_lte_100.optional(),
  /** depth of fastener (nut/head) accomodated (for external drive sockets) */
  wrench_dp:            mm_lte_100.optional(),
  /** diameter of bore to clear bolt protruding past nut (for external drive sockets) */
  bolt_clr_diam:        mm_lte_360.optional(),
  bolt_clr_dp:           mm_lte_100.optional(),
})
export interface SocketWrenchT   extends CK.Zcasted<typeof socketWrench> {}
export interface SocketWrenchSk  extends CK.Zsketch<typeof socketWrench> {}

export const bitSocket = CK.obj({
  ...socketWrench.shape,
  socket_kind:          CK.oneof(['socket_bit']),
  bit_ln_exposed:       mm_lte_360.optional(),
})