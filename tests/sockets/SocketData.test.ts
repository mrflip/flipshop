import      { expect }                            from 'chai'
import      * as Flipshop                         from '@flipshop/flipshop'
import      { Sockets }                          from '@flipshop/flipshop'

describe('@flipshop/flipshop Sockets', () => {
  it('should export SocketWrench instances', async () => {
    await Sockets.loadSocketWrenches()
    console.log(Sockets.SocketWrenches)
  })
})