import      _                                   /**/ from 'lodash'
import      { expect }                            from 'chai'
import      * as Flipshop                         from '@flipshop/flipshop'
import      { Sockets }                          from '@flipshop/flipshop'

describe('@flipshop/flipshop Sockets', () => {
  it('should export SocketWrench instances', async () => {
    await Sockets.loadSocketWrenches()
    // console.log(_.map(Sockets.SocketWrenchList, 'title').join('\n'))
    console.log(_.sortBy(_.uniq(_.map(Sockets.SocketWrenchList, 'size_nom')), [(val) => /mm/i.test(val) ? 0 : 1, 'size_nom']).join('\n'))
  })
})