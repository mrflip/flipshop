import      { expect }                            from 'chai'
import      * as Flipshop                         from '@flipshop/flipshop'
import      { Fastener }                          from '@flipshop/flipshop'

describe('@flipshop/flipshop package structure', () => {
  it('should be importable as a module', () => {
    expect(Flipshop).to.exist
    expect(Flipshop).to.not.be.null
    expect(Flipshop).to.not.be.undefined
  })

  it('should have expected top-level exports', () => {
    const MainPackageKeys = ['Fastener', 'Sockets', 'Utils']
    expect(Flipshop).to.have.keys(...MainPackageKeys)
  })
  const PackageFastenerKeys = [
    'FastenerSizing',
    'Threading',
    'Screw',
    'ExternalDriveScrew',
    'InternalDriveScrew',
    'Thruhole',
    'DrillBit',
  ]

  it('should have expected Fastener exports', () => {
    expect(Flipshop.Fastener).to.include.keys(...PackageFastenerKeys)
  })

  it('should support destructuring imports', () => {
    expect(Fastener).to.include.keys(...PackageFastenerKeys)
  })
})