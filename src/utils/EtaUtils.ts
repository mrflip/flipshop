import      lodash                           /**/ from 'lodash'
import      { Eta, type EtaConfig }               from 'eta'
//
import      * as FM                               from '@freeword/meta'
import      { UF, Filer }                         from '@freeword/meta'
//
import type { AnyBag }                            from './internal.ts'

const BaseConfig = {
  /** Automatically XML-escape?  */ autoEscape:    false,
  /** Apply a filter function?   */ autoFilter:    false,
  /** automatic trim whitespace  */ autoTrim:      [false, false], // "nl" | "slurp" | false
  /** Pretty errors (slower?     */ debug:         true,
  /** Remove whitespace?         */ rmWhitespace:  false,
  /** Delimiters                 */ tags:          ['{%', '%}'],
  /** Cache templates?           */ cache:         true,
  parse: {
    /** Evaluation Prefix        */ exec:          '!',
    /** Interpolation Prefix     */ interpolate:   '',
    /** Raw Interpolation Prefix */ raw:           '~',
  },
} as const satisfies Partial<EtaConfig>

const Helpers = {
  lodash,
  _: lodash,
  ...UF,
  ...FM,
  ...lodash.pick(UF, ['kfy', 'qtc', 'qt', 'wd', 'okish', 'inspectify', 'inCols', 'toSentence', 'inspectify', 'indent', 'dedent']),
  pad: lodash.padEnd,
}

export function load<ParamsT, ExtrasT = AnyBag>(pathname: string, extras: ExtrasT = {} as ExtrasT) {
  const   { dirpath, basename } = Filer.pathinfoFor(pathname) as FM.PathinfoT
  const   eta = new Eta({ ...BaseConfig, views: dirpath })
  return  (async (params: ParamsT) => eta.render(basename, {
    ...Helpers,
    ...extras,
    ...params,
  }))
}