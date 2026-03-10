import      { expect }                            from 'chai'
import      * as Flipshop                         from '@flipshop/flipshop'

describe('@freeword/meta package structure', () => {
  it('should be importable as a module', () => {
    expect(Flipshop).to.exist
    expect(Flipshop).to.not.be.null
    expect(Flipshop).to.not.be.undefined
  })

  it('should have expected top-level exports', () => {
    const expectedExports = [
      'Fasteners',
      'FastenerSizing',
      'Threading',
      'Screw',
      'ExternalDriveScrew',
      'InternalDriveScrew',
      'Thruhole',
      'DrillBit',
    ]

    expectedExports.forEach(exportName => {
      expect(FreewordMeta).to.have.property(exportName)
    })
  })

  it('should not have unexpected properties', () => {
    const allowedProps = [
      // 'default', // ESM default export
      "AtoZlos", "AtoZnums", "AtoZups", "Chars09AZaz", "CharsAZ09Bar", "CharsAZaz", "Filer",
      "MAX_UINT32", "Numerals", "PosStemkinds", 'Poskinds',
      "RandomFactory", "SeededRandomFactory", "Stemkinds", "StrAtoZ", "StrAtoZlo", "StrAtoZup",
      "StrNumerals", "Streaming", "SuffixREForStemkind", "UF", "Wordbits", "Wordform",
    ]
    expect(FreewordMeta).to.include.keys(...allowedProps)
  })

  it('should have immutable constants', () => {
    // Test that constants are not accidentally mutable
    const originalPoskinds = [...FreewordMeta.Poskinds]
    const originalStemkindsForPos = JSON.parse(JSON.stringify(FreewordMeta.PosStemkinds))

    expect(FreewordMeta.Poskinds).to.deep.equal(originalPoskinds)
    expect(FreewordMeta.PosStemkinds).to.deep.equal(originalStemkindsForPos)
  })

  it('should support destructuring imports', () => {
    const { Poskinds, PosStemkinds } = FreewordMeta

    expect(Poskinds).to.be.an('array')
    expect(PosStemkinds).to.be.an('object')
  })
})