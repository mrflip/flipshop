import      _                                /**/ from 'lodash'
import      { expect }                            from 'chai'
import type * as TY                               from '@freeword/meta'
import      * as Flipshop                         from '@flipshop/flipshop'
import      { Sockets }                           from '@flipshop/flipshop'
import      * as TH                               from '../TestHelpers.ts'
import { SocketWrenches, socketWrenchesToFeaturescript } from '../../src/sockets/SocketData.ts'
import      { SocketWrench }                      from '../../src/sockets/SocketModel.ts'
import type { SocketWrenchT }                     from '../../src/sockets/SocketTypes.ts'

const ExemplarKeys = {
  hex_socket_10mm: `3/8in Drive Long Ball End Hex Bit Metric Socket 10mm`,
} as const satisfies Record<string, string>
const Exemplars = { hex_socket_10mm: undefined! } as Record<keyof typeof ExemplarKeys, Flipshop.Sockets.SocketWrench>
const SomeSocketWrenches = {} as typeof SocketWrenches

function makeSocket(partial: Partial<SocketWrenchT>): SocketWrench {
  return SocketWrench.live({
    title: 'Test Socket', sizing: '10mm', sizing_mm: 10, sizing_in: 0.394,
    sku: 'TEST123', upc: '000000000000', url: 'https://example.com', img_url: 'https://example.com/img.jpg',
    socket_kind: 'socket_exthex', drive_kind: 'exthex', unit_system: 'metric',
    sqdrive_size: 'isq_0375in', socket_variant: 'std', reach_kind: 'reg',
    ...partial,
  })
}

describe('SocketWrench.groupingTitleRoot', () => {
  it('bit socket: includes unit system when drive is inthex', () => {
    const s = makeSocket({ socket_kind: 'socket_bit', drive_kind: 'inthex', unit_system: 'us', sqdrive_size: 'isq_0375in', socket_variant: 'ball', reach_kind: 'long' })
    expect(s.groupingTitleRoot()).to.equal('Bit Socket, Int Hex, US, 3/8 Sq.Dr, Ball End, Long')
  })
  it('bolt socket: includes Metric, omits Standard variant', () => {
    const s = makeSocket({ socket_kind: 'socket_exthex', drive_kind: 'exthex', unit_system: 'metric', sqdrive_size: 'isq_0750in', socket_variant: 'std', reach_kind: 'reg' })
    expect(s.groupingTitleRoot()).to.equal('Bolt Socket, 6-Point, Metric, 3/4 Sq.Dr, Regular')
  })
  it('bolt socket: includes Impact variant', () => {
    const s = makeSocket({ socket_kind: 'socket_exthex', drive_kind: 'exthex', unit_system: 'metric', sqdrive_size: 'isq_0750in', socket_variant: 'impact', reach_kind: 'reg' })
    expect(s.groupingTitleRoot()).to.equal('Bolt Socket, 6-Point, Metric, 3/4 Sq.Dr, Impact, Regular')
  })
  it('star socket: omits Metric for extstar drive', () => {
    const s = makeSocket({ socket_kind: 'socket_extstar', drive_kind: 'extstar', unit_system: 'metric', sqdrive_size: 'isq_0375in', socket_variant: 'std', reach_kind: 'reg' })
    expect(s.groupingTitleRoot()).to.equal('Star Socket, Ext Torx, 3/8 Sq.Dr, Regular')
  })
  it('omits Metric for torx drive', () => {
    const s = makeSocket({ drive_kind: 'torx', unit_system: 'metric', socket_variant: 'std', reach_kind: 'reg' })
    expect(s.groupingTitleRoot()).to.not.include('Metric')
  })
  it('still shows US for torx when unit system is us', () => {
    const s = makeSocket({ drive_kind: 'torx', unit_system: 'us', socket_variant: 'std', reach_kind: 'reg' })
    expect(s.groupingTitleRoot()).to.include('US')
  })
})

describe('@flipshop/flipshop Sockets', () => {
  beforeAll(async () => {
    await Sockets.loadSocketWrenches()
    _.each(ExemplarKeys, (socketTitle, handle) => { Exemplars[handle] = Sockets.SocketWrenchByTitle[socketTitle] })
    _.merge(SomeSocketWrenches, _.pick(SocketWrenches, ['socket_bit.inthex.isq_0250in', 'socket_exthex.exthex.us.isq_0250in.std.reg', 'socket_exthex.exthex.metric.isq_0375in.impact.deep']))
  })

  it('has expected contents', () => {
    // console.log(UF.inspectify(Sockets.socketwrenches.socket_bit?.inthex))
    expect(TH.checkSnapshot(Exemplars.hex_socket_10mm)).to.be.true
  })

  describe('Exporting to Featurescript', () => {
    describe('a SocketWrench model can emit a Featurescript string', () => {
      it('without failing', () => {
        const { hex_socket_10mm } = Exemplars
        const fs = hex_socket_10mm.toFeaturescript()
        expect(fs).to.be.a('string')
        expect(fs).to.match(/^\{.*\}$/)
      })
      it('formats the title as a quoted string', () => {
        const fs = Exemplars.hex_socket_10mm.toFeaturescript()
        expect(fs).to.include(`"title": "3/8in Drive Long Ball End Hex Bit Metric Socket 10mm"`)
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
    describe('we can dump the SocketWrench data as a Featurescript text blob', () => {
      it('for subsampled SocketWrench data', () => {
        const blob = socketWrenchesToFeaturescript(SomeSocketWrenches)
        expect(TH.checkSnapshot(blob)).to.be.true
        expect(blob.length).to.be.greaterThan(18_000)
      })
      it('starts with the const declaration', () => {
        const blob = socketWrenchesToFeaturescript(SomeSocketWrenches)
        expect(blob).to.match(/^const SocketWrenches =/)
      })
      it('nests enum keys as quoted strings', () => {
        const blob = socketWrenchesToFeaturescript(SomeSocketWrenches)
        expect(blob).to.include('"socket_exthex"')
        expect(blob).to.include('"exthex"')
        expect(blob).to.include('"isq_0250in"')
      })
      it('leaf sockets render as inline featurescript maps', () => {
        const blob = socketWrenchesToFeaturescript(SomeSocketWrenches)
        expect(blob).to.include('"sizing_mm":')
        expect(blob).to.include('* mm')
      })
    })
  })
})