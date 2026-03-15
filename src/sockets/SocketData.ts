import       _                               /**/ from 'lodash'
import type * as TY                               from './internal.ts'
import      { UF }                                from '@freeword/meta'
import      { SocketWrench }                      from './SocketModel.ts'
import type { SocketWrenchT }                     from './SocketTypes.ts'
import type { FastenerDrive, SocketKind, SocketReach, ToolDrive } from '../fastener/FastenerEnums.ts'
import { canhasbucket } from '../utils/DatafileHelpers.ts'

// export const SocketWrenchList: SocketWrench[] = []
export const SocketWrenchByTitle: Record<string, SocketWrench> = {}
export const SocketWrenches: TY.PartialBag<SocketKind, TY.PartialBag<FastenerDrive, TY.PartialBag<ToolDrive, TY.PartialBag<SocketReach, SocketWrench>>>> = {}

export async function loadSocketWrenches(): Promise<TY.Bag<SocketWrench>> {
  const { default: RawSocketWrenches } = await import('../../data/sockets/sockets.json', { with: { type: 'json' } })
  _.each(RawSocketWrenches as SocketWrenchT[], (raw) => {
    const socket = SocketWrench.live(raw)
    SocketWrenchByTitle[socket.title] = socket
    const bucket = canhasbucket(SocketWrenches, [socket.socket_kind, socket.drive_kind, socket.unit_system, socket.sqdrive_size, socket.socket_variant, socket.reach_kind])
    if (bucket[socket.sizing] && (! /^(socket_(sparkplug|ujoint|extension))$/.test(socket.socket_kind))) { console.warn('Duplicate sizing:', socket.sizing, bucket[socket.sizing], socket) }
    bucket[socket.sizing] = socket
  })
  // console.log(UF.inspectify(SocketWrenches))
  return SocketWrenchByTitle
}