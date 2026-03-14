#! /usr/bin/env yarn node
import      _                                /**/ from 'lodash'
import type * as _ZMP                             from './ZodMonkeypunch.ts'
import      { readdirSync }                       from 'fs'
import      { join }                              from 'path'
import type * as TY                               from '@freeword/meta'
import      { Filer }                             from '@freeword/meta'
import      { UF }                                from '@freeword/meta'
import      * as Flipshop                         from '@flipshop/flipshop'

// == [Main] ==

const freals = true
const dirs = freals ? [
  Filer.__relname(import.meta.url, '..', '..', 'tmp', 'www.gearwrench.com', 'all-tools', 'ratchets-sockets', 'chrome-sockets'),
  Filer.__relname(import.meta.url, '..', '..', 'tmp', 'www.gearwrench.com', 'all-tools', 'ratchets-sockets', 'impact-products'),
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
    .filter((filename) => (! /(-set-|-set\.|^80148s-06-14)/.test(filename)))
    .map((filename) => join(ripdDir, filename))

  const products = _.compact(await Promise.all(files.map(parseProductFile)))
  SocketWrenchProducts.push(...products)
  console.warn(`Parsed ${products.length} / ${files.length} products from ${ripdDir}.`)
}
const sorted = _.orderBy(SocketWrenchProducts, ['socket_kind', 'drive_kind', 'unit_system', 'sqdrive_size', 'socket_variant', 'reach_kind', 'wrench_end_diam', 'ln_overall'])
// console.warn(UF.prettify(Enumish))
console.log(JSON.stringify(sorted, null, 2))

// --