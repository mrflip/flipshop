import _                                    /**/ from 'lodash'
import type * as TY                               from './internal.ts'
import { UF } from '@freeword/meta'
import { MM_IN } from '../fastener/FastenerTypes.ts'

/** Replace all values equal to the string `'null'` with the javascript value `null`.
 * The dangers of this should be apparent for general use
 */
export function nullify(clxn: TY.AnyBag): TY.AnyBag {
  if (UF.arrayish(clxn)) { return _.map(clxn, (val) => { val === 'null' ? null : val }) }
  return _.mapValues(clxn, (val) => (val === 'null' ? null : val))
}

/** Convert all values with keys ending in `'_in'` to the equivalent millimeter value.
 *  Tries not to overwrite existing values; pretty stupid, otherwise
 *  @param    data - The data to convert
 *  @returns the input, with new fields added
 */
export function mmize<BT extends TY.AnyBag>(data: BT): BT & TY.AnyBag {
  const mmized = {} as TY.AnyBag
  _.each(data, (val, key) => {
    if (/_in$/i.test(key) && UF.isBlank(val)) {
      mmized[key.replace(/_in$/i, '')] = _.round(val * MM_IN, 4)
    }
  })
  return { ...data, ...mmized } as BT & TY.AnyBag
}

export function canhasbucket<BT extends TY.AnyBag>(bag: BT, keys: string[]): BT {
  let bucket = bag as TY.AnyBag
  _.each(keys, (key) => {
    bucket[key] ??= {}
    bucket = bucket[key] as TY.AnyBag
  })
  return bucket as BT
}