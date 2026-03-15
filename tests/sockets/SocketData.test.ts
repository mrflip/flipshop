import      _                                /**/ from 'lodash'
import      { expect }                            from 'chai'
import type * as TY                               from '@freeword/meta'
import      { UF }                                from '@freeword/meta'
import      * as Flipshop                         from '@flipshop/flipshop'
import      { Sockets }                           from '@flipshop/flipshop'
import      * as TH                               from '../TestHelpers.ts'

const ExemplarKeys = {
  hex_socket_10mm: `3/8\" Drive Long Ball End Hex Bit Metric Socket 10mm`,
} as const satisfies Record<string, string>
const Exemplars = { hex_socket_10mm: undefined! } as Record<keyof typeof ExemplarKeys, Flipshop.Sockets.SocketWrench>

describe('@flipshop/flipshop Sockets', () => {
  beforeAll(async () => {
    await Sockets.loadSocketWrenches()
    _.each(ExemplarKeys, (socketTitle, handle) => { Exemplars[handle] = Sockets.SocketWrenchByTitle[socketTitle] })
  })

  it('has expected contents', () => {
    // console.log(UF.inspectify(Sockets.socketwrenches.socket_bit?.inthex))
    expect(TH.checkSnapshot(Exemplars.hex_socket_10mm)).to.be.true
  })

  describe('Exporting to Featurescript', () => {
    it.todo('a SocketWrench model can emit a Featurescript string')
  })
})