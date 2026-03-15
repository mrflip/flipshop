import      _                                /**/ from 'lodash'
import      { expect }                            from 'chai'
import      { UF }                                from '@freeword/meta'
import      * as Flipshop                         from '@flipshop/flipshop'
import      { Sockets }                           from '@flipshop/flipshop'

describe('@flipshop/flipshop Sockets', () => {
  it('should export SocketWrench instances', async () => {
    await Sockets.loadSocketWrenches()
    console.log(UF.inspectify(Sockets.SocketWrenches.socket_bit?.inthex))
    // console.log(_.sortBy(_.uniq(_.map(Sockets.SocketWrenchList, 'sizing')), [(val) => /mm/i.test(val) ? 0 : 1, 'sizing']).join('\n'))
  })
})