import      _                                /**/ from 'lodash'
import      { expect }                            from 'chai'
import type * as TY                               from '@freeword/meta'
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
    it('a SocketWrench model can emit a Featurescript string', () => {
      const { hex_socket_10mm } = Exemplars
      const fs = hex_socket_10mm.toFeaturescript()
      expect(fs).to.be.a('string')
      expect(fs).to.match(/^\{.*\}$/)
    })
    it('formats the title as a quoted string', () => {
      const fs = Exemplars.hex_socket_10mm.toFeaturescript()
      expect(fs).to.include(`"title": "3/8\\" Drive Long Ball End Hex Bit Metric Socket 10mm"`)
    })
    it('formats mm fields with * mm', () => {
      const fs = Exemplars.hex_socket_10mm.toFeaturescript()
      expect(fs).to.include(`"sizing_mm": 10 * mm`)
    })
    it('formats inch fields with * inch', () => {
      const { hex_socket_10mm } = Exemplars
      const fs = hex_socket_10mm.toFeaturescript()
      const expectedIn = `"sizing_in": ${hex_socket_10mm.sizing_in} * inch`
      expect(fs).to.include(expectedIn)
    })
    it('formats enum fields as quoted strings', () => {
      const fs = Exemplars.hex_socket_10mm.toFeaturescript()
      expect(fs).to.include(`"socket_kind": "socket_bit"`)
      expect(fs).to.include(`"drive_kind": "inthex"`)
      expect(fs).to.include(`"unit_system": "metric"`)
      expect(fs).to.include(`"sqdrive_size": "isq_0375in"`)
    })
    it('omits undefined optional fields', () => {
      const fs = Exemplars.hex_socket_10mm.toFeaturescript()
      // wt is not always present; if absent it should not appear
      if (Exemplars.hex_socket_10mm.wt === undefined) {
        expect(fs).to.not.include('"wt":')
      }
    })
    it('only includes SocketWrenchT fields, not Gearwrench-specific fields', () => {
      const fs = Exemplars.hex_socket_10mm.toFeaturescript()
      expect(fs).to.not.include('"material":')
      expect(fs).to.not.include('"surf_finish":')
      expect(fs).to.not.include('"is_knurled":')
    })
  })
})