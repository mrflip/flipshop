import       _                               /**/ from 'lodash'
import      { SocketWrench }                      from './SocketModel.ts'
import type { SocketWrenchT }                     from './SocketTypes.ts'

export const SocketWrenches: SocketWrench[] = []

export async function loadSocketWrenches(): Promise<SocketWrench[]> {
  const { default: RawSocketWrenches } = await import('../../data/sockets/sockets.json', { with: { type: 'json' } })
  _.each(RawSocketWrenches as SocketWrenchT[], (raw) => SocketWrenches.push(SocketWrench.live(raw)))
  return SocketWrenches
}