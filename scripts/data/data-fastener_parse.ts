#! /usr/bin/env yarn node
import _                                    /**/ from 'lodash';
import { readFileSync, writeFileSync }           from 'fs';
import * as FM                                   from '@freeword/meta';
import * as Filer                                from '@freeword/meta/Filer.js';
import * as Flipshop                             from '@flipshop/flipshop';
import type * as FT                              from '@flipshop/flipshop';

const FastenerMasterDataLines = readFileSync(Filer.__relname(import.meta.url, '..', '..', 'data', 'fastener', 'FastenerMasterData.tsv'), 'utf8').split(/\r?\n/g).map(line => line.split('\t'))
const Fieldnames = FastenerMasterDataLines.shift()!
const FastenerMasterData = FastenerMasterDataLines.map((vals) => {
  const obj = {} as Flipshop.Fastener.FastenerFlatPropsT & { [key in 'coarse' | 'fine' | 'xfine']: Flipshop.Fastener.ThreadingT }
  _.each(_.zipObject(Fieldnames, vals), (val, key) => { if (val === 'null') { _.set(obj, key, null) } else if (! FM.UF.isVoid(val)) { _.set(obj, key, val) } })
  obj[obj.threading_kind] = obj
  if (obj.size_pref !== 'A' || obj.thread_pref !== 'a' || FM.UF.isVoid(obj.hexnut?.ht) || obj.threading_kind !== 'coarse') { return null }
  return Flipshop.Fastener.FastenerSizing.fill(obj)
})

console.log(_.take(FastenerMasterData, 4))