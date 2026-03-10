#! /usr/bin/env yarn node
import _                                    /**/ from 'lodash';
import { readFileSync, writeFileSync }           from 'fs';
import * as Filer                                from '@freeword/meta/Filer.js';
import * as Flipshop                             from '@flipshop/flipshop';
import type * as FT                              from '@flipshop/flipshop';

const FastenerMasterDataCols = Flipshop.Fastener.FastenerMasterDataCols as readonly string[]

const FastenerMasterData = readFileSync(Filer.__relname(import.meta.url, '..', '..', 'data', 'fastener', 'FastenerMasterData.tsv'), 'utf8').split(/\r?\n/g).map(line => line.split('\t'))
console.log(_.take(FastenerMasterData, 10))