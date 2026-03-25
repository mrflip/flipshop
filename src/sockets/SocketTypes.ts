import      _                                   /**/ from 'lodash'
import      { CK }                                   from '@freeword/meta'
import      * as FE                                  from '../fastener/FastenerEnums.ts'
import      { driverTargets }                        from './DriverTargets.ts'
import      { in_lte_36, mm_lte_100, mm_lte_2000, mm_lte_360 }  from '../fastener/FastenerTypes.ts'

export const socketWrench = CK.obj({
  /** Name of the socket (e.g. `1/4" Drive 6 Point Standard SAE Socket 5/16"`) */
  title:                CK.titleish,
  /** type of socket: socket_exthex, socket_bit, socket_flex, socket_extension, socket_ujoint, socket_adapter, socket_sparkplug, socket_extstar, other */
  socket_kind:          CK.oneof(FE.SocketKindVals),
  /** type of drive: intsq, inthex, extsq, exthex, extstar, other */
  drive_kind:           CK.oneof(FE.FastenerDriveVals),
  /** unit system: metric, us */
  unit_system:          CK.oneof(FE.UnitSystemVals),
  /** size of the square drive: for Gearwrench, one of the standard US ratchet drive sizes (1/4 in, 3/8 in, 1/2 in, 3/4 in, 1 in) */
  sqdrive_size:         CK.oneof(FE.ToolDriveVals),
  /** type of reach: standard, midlen, deep, long, xlong, other */
  reach_kind:           CK.oneof(FE.SocketReachVals),
  /** type of variant: standard, impact, ball */
  socket_variant:       CK.oneof(FE.SocketVariantVals),
  /** type of bit (internal drive sockets); should equal drive_kind */
  bit_kind:             CK.oneof(FE.InternalDriveVals).optional(),
  /** text giving the nominal size of the socket (e.g. "1/4 in", "2 mm", "T10", "E14", "Ph1", "Ph00") */
  sizing:               CK.str.regex(/^(?:((?:\d+\+)?[\/\d]+in|[\d\.]+mm)|T\d+|E\d+|Sl\d+|P[hz](?:[01234]|00|000)|.+in - .+in)$/),
  /** drive size of the tool in millimeters */
  sizing_mm:            mm_lte_2000,
  /** sizing_mm rounded to one decimal place with trailing zeros removed (e.g. "9.5", "12.7") */
  sizing_mm_text:       CK.str.optional(),
  /** drive size of the tool in inches */
  sizing_in:            in_lte_36,
  //
  /** overall length of the socket along the central axis */
  ln_overall:           mm_lte_2000.optional(),
  /** overall size of the socket in the X direction; equals the larger of the ratchet end diameter and the target end diameter */
  wx_overall:           mm_lte_360.optional(),
  /** overall size of the socket in the Y direction; equals the larger of the ratchet end diameter and the target end diameter */
  wy_overall:           mm_lte_360.optional(),
  /** diameter at the base (ratchet end) */
  target_end_diam:      mm_lte_2000.optional(),
  /** diameter at the tip (target end) */
  ratchet_end_diam:       mm_lte_100.optional(),
  /** length of bit (internal drive sockets) between the tip and the nose. Note: the specs sometimes list only bit length, and it's not clear which is the total length. */
  bit_ln:               mm_lte_360.optional(),
  /** length of bit (internal drive sockets) between the tip and where it ends inside the socket, assumedly. Note: the specs sometimes list only bit length, and it's not clear which is the total length. */
  bit_ln_total:         mm_lte_360.optional(),
  /** length from drive tip to shoulder */
  shoulder_ln:          mm_lte_360.optional(),
  /** diameter at the nose for bit sockets (where the bit is inserted) */
  nose_diam:            mm_lte_100.optional(),
  /** depth of fastener (nut/head) accomodated (for external drive sockets) */
  target_dp:            mm_lte_100.optional(),
  /** diameter of bore to clear bolt protruding past nut (for external drive sockets) */
  bolt_clr_diam:        mm_lte_360.optional(),
  /** for adapters, extensions and universal joints, the size of the male (driver) end */
  male_drive_size:      CK.oneof(FE.ToolDriveVals).optional(),
  /** for adapters, extensions and universal joints, the size of the female (wrench) end */
  female_drive_size:    CK.oneof(FE.ToolDriveVals).optional(),
  /** drive to driven mapping */
  targets:              driverTargets.default({}),
  /** weight of the socket in kilograms */
  wt:                   mm_lte_100.optional(),
  /** weight of the socket in pounds */
  wt_lb:                mm_lte_100.optional(),
  /** alphanumeric SKU of an exemplar socket */
  sku:                  CK.extkeyish,
  /** alphanumeric UPC of an exemplar socket */
  upc:                  CK.unumstr,
  /** URL of an exemplar product page */
  url:                  CK.urlstr,
  /** URL of an exemplar image */
  img_url:              CK.urlstr,
})
export interface SocketWrenchT   extends CK.Zcasted<typeof socketWrench> {}
export interface SocketWrenchSk  extends CK.Zsketch<typeof socketWrench> {}

export const bitSocket = CK.obj({
  ...socketWrench.shape,
  socket_kind:          CK.oneof(['socket_bit']),
  bit_ln_exposed:       mm_lte_360.optional(),
})