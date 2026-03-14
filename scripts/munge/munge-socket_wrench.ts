#! /usr/bin/env yarn node
import      _                                /**/ from 'lodash'
import type * as _ZMP                             from './ZodMonkeypunch.ts'
import      { load as cheerioLoad }               from 'cheerio'
import      { readdirSync }                       from 'fs'
import      { join }                              from 'path'
import type * as TY                               from '@freeword/meta'
import      { Filer, CK }                         from '@freeword/meta'
import      * as Flipshop                         from '@flipshop/flipshop'

// == [Main] ==

const freals = true
const dirs = freals ? [
  Filer.__relname(import.meta.url, '..', '..', 'ripd', 'ratchets-sockets', 'chrome-sockets'),
  Filer.__relname(import.meta.url, '..', '..', 'ripd', 'ratchets-sockets', 'impact-products'),
] : [
  Filer.__relname(import.meta.url, '..', '..', 'tests', 'fixtures', 'socket_wrenches_raw'),
]

async function parseProductFile(filepath: TY.Anypath): Promise<Flipshop.Mungers.GearwrenchMungers.GearwrenchSocketT | null> {
  const loadedText = await Filer.loadtext(filepath)
  if (! loadedText.ok) { console.error(`Failed to load ${filepath}: ${loadedText.gist}`); return null }
  return Flipshop.Mungers.GearwrenchMungers.parseProductPage(filepath, loadedText.val)
}

const SocketWrenchProducts: Flipshop.Mungers.GearwrenchMungers.GearwrenchSocketT[] = []
for (const ripdDir of dirs) {
  const files = readdirSync(ripdDir)
    .filter((filename) => filename.endsWith('.html'))
    .filter((filename) => (! filename.includes('-set-')))
    .map((filename) => join(ripdDir, filename))

  const products = await Promise.all(files.map(parseProductFile))
  const valid    = products.filter(Boolean) as Flipshop.Mungers.GearwrenchMungers.GearwrenchSocketT[]
  SocketWrenchProducts.push(...valid)
  console.warn(`Parsed ${valid.length} / ${files.length} products from ${ripdDir}.`)
}
// console.warn(UF.prettify(Enumish))
console.log(JSON.stringify(SocketWrenchProducts, null, 2))

// --