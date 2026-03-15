#! /usr/bin/env yarn node
import { Filer }                             from '@freeword/meta'
import { Sockets }                           from '@flipshop/flipshop'
import { socketWrenchesToFeaturescript }     from '../../src/sockets/SocketData.ts'

await Sockets.loadSocketWrenches()

const blob    = socketWrenchesToFeaturescript(Sockets.SocketWrenches)
const outpath = Filer.__relname(import.meta.url, '..', '..', 'onshape', 'flipshop', 'SocketWrenches.fs')
const result  = await Filer.dumptext(outpath, [blob])

if (! result.ok) { console.error(`Failed to write ${outpath}: ${result.gist}`); process.exit(1) }
console.log(`Wrote ${outpath}`)
