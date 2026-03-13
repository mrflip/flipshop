import _                                     /**/ from 'lodash'
import      * as EtaUtils                         from './EtaUtils.ts'
//
import      * as FM                               from '@freeword/meta'
import      { UF, Filer, CK }                     from '@freeword/meta'
//
import type * as TY                               from './internal.ts'
//
import type {
  AnyBag, Barename, Handleish, ZodTypeAny,  Relpath, Abspath, Dirpath, AnyFext,
}                                       from './internal.js'

const ROOTPATH                 = Filer.__dirname(import.meta.url, '../../arda/gen')
export const TEMPLATE_SRC_PATH = Filer.__dirname(import.meta.url, '../templates')

export const BaseTemplaterCx = CK.Validator(({
  obj, anyfext, bareint, relpath, barename, logger, loazalnumbar, azalnumbar, label, anybag, bag, quantity,
}) => {
  const stepnum = bareint.min(1).max(999)
  const flowlabel = loazalnumbar
  const baseTemplaterProps = obj({
    ns:             label.default('tmp'),
    handle:         azalnumbar.optional(),
    pool:           label.optional(),
    poolnum:        stepnum.optional(),
    stepnum:        stepnum.optional(),
    rootpath:       relpath.default(ROOTPATH),
    templateRoot:   relpath.default(TEMPLATE_SRC_PATH),
    mainpath:       relpath.optional(),
    fext:           anyfext.optional(),
    logger:         logger,
  })
  const genParams = obj({
    relpath:       relpath.optional(),
    dirname:       relpath.optional(),
    barename:      barename.optional(),
    pool:          label.optional(),
    poolnum:       stepnum.optional(),
    step:          barename.optional(),
    stepnum:       stepnum.optional(),
    fext:          anyfext.optional(),
    dedent:        quantity.optional(),
    templatePath:  relpath.optional(),
  })
  return {
    stepManifest: bag(genParams.partial()),
    anybag,
    stepnum,
    flowlabel,
    genParams,
    // !! use passthrough so you don't wash away subclass props
    baseTemplaterProps: baseTemplaterProps,
    baseTemplaterDNA:   baseTemplaterProps.partial({ logger: true, mainpath: true }),
  }
})

export interface BaseTemplaterProps  extends CK.Zsketch<typeof BaseTemplaterCx.baseTemplaterProps> {}
export interface BaseTemplaterDNA    extends CK.Zsketch<typeof BaseTemplaterCx.baseTemplaterDNA>   {}
export interface BaseTemplaterThis<XRP extends AnyBag, BT extends BaseTemplater<XRP>> { name: string, new (dna: BaseTemplaterProps): BT, dnaDefaults: Partial<BaseTemplaterProps> &  AnyBag }

export interface DumpFrame         { ok: boolean, abspath: Relpath, step: string, handle: string, pool: string, content: string, gist?: 'empty' | 'present' | FM.FilerGist | undefined }
export interface GenParams         extends CK.Zsketch<typeof BaseTemplaterCx.genParams> {}

export interface TemplaterParamsish  extends GenParams { formatter?: TPLFormatterDNA | undefined, stepnum?: number | undefined }
export type SteppedManifest<ST extends string, BT extends TemplaterParamsish = TemplaterParamsish> = Record<ST, Partial<BT> & AnyBag>
export type SteppedContents<ST extends string> = Record<ST, string>
export type SteppedFrames<ST extends string> = Record<ST, DumpFrame>
export type SteppedParams<ST   extends string = string, RP extends GenParams = GenParams> = RP & { step: ST }
export interface TPLFormatterDNA  extends Partial<GenParams> { format?: (params: any) => string, templateFext?: string, extras?: AnyBag | undefined }
export interface TPLFormatter     extends TPLFormatterDNA    { format:  (params: any) => string, relpath: Relpath }

export class BaseTemplater<RP extends GenParams, XST extends string = string> implements BaseTemplaterProps {
  declare rootpath:     Abspath
  declare templateRoot: Abspath
  declare ns:           Handleish
  declare handle:       Handleish
  declare pool?:        Handleish     | undefined
  declare poolnum?:     number        | undefined
  declare stepnum?:     number        | undefined
  declare fext?:        TY.AnyFext    | undefined
  declare logger:       TY.LoggerT
  declare Formatters:   TY.Bag<TPLFormatterDNA>
  static steps?:        SteppedManifest<string, TemplaterParamsish> | undefined
  //
  static dnaDefaults: AnyBag = { }

  get steps()                 { return (this.constructor as typeof BaseTemplater).steps }
  get stepnames(): XST[]      { return _.keys(this.steps) as XST[] }
  get checker(): ZodTypeAny   { return BaseTemplaterCx.anybag }
  get mainpath(): Handleish   { return this.ns }

  constructor(dna: BaseTemplaterProps) {
    const props = BaseTemplaterCx.baseTemplaterDNA.passthrough().cast(dna)
    Object.assign(this, props)
    UF.decorate(this, { logger: dna.logger })
  }

  static make<XRP extends AnyBag, BT extends BaseTemplater<XRP>>(this: BaseTemplaterThis<XRP, BT>, dna: BaseTemplaterDNA) {
    const logger = UF.getLogger(dna)
    // !! use passthrough so you don't wash away subclass props
    const props = BaseTemplaterCx.baseTemplaterProps.passthrough().cast({ handle: this.name.replace(/TPL$/, ''), ...this.dnaDefaults, ...dna, logger })
    return new this(props)
  }

  get title() {
    return [this.ns, UF.smush('-', this.poolnum, this.pool), UF.smush('-', this.stepnum, this.handle)].join('/')
  }

  /**
   * @param params {RP} - bespoke gen params for templating
   *
   * @returns {string} - the rendered content of the file
   * */
  async render(params: RP): Promise<string> {
    return this.loadAndFormat(params)
  }

  // format(params: RP, template: string, othervals: AnyBag = {}) {
  //   const fullParams = this.fullGenvars(params, othervals)
  //   // const content = this.formatter(params).format(template, fullParams)
  //   // const template = EtaUtils.load(templatePath, { Lembas, ...params, ...formatter.extras, params, templater: this })
  //   const content = template
  //   if (params.dedent) { return UF.dedent(content, params.dedent) }
  //   return content
  // }
  fullGenvars(params: RP, othervals: AnyBag = {}): RP & AnyBag {
    return { ...params, ...othervals }
  }

  async loadAndFormat(params: RP, formatterHandle: string | undefined = undefined, othervals: AnyBag = {}) {
    const formatter = await this.formatter(params, formatterHandle)
    return formatter.format({ ...params, ...othervals })
  }

  /** all directly defined formatters, plus an auto-vivified formatter for each step */
  get _formatters(): TY.Bag<TPLFormatterDNA> {
    return { ..._.mapValues(this.steps, (stepBag) => (stepBag.formatter || {})), ...this.Formatters }
  }

  /** look up the formatter by handle (stepname, often);
   *  if memoized, return it, otherwise load the template file, compile it,
   *  and return the (now-memoized) formatter
   *
   * * to define a custom formatter, pre-arrange the `format` method property
   * * to re-use a formatter:
   *   `steps: { FakeAgentToks:   { formatterHandle: 'Sectoks' }, }`
   * * The template file is, by default, its poolpath/handle.fext.eta;
   *   - to specify an altername template pathname, override `.templateAbspath(params)`
   *     to return the full pathname incl. template fext:
   *     ```
   *      override templateAbspath(params) {
   *        if (params.step === 'JestConfig') { return this.templatePathjoin(this.ns, pool, 'jest.config.ts.eta') }` }
   *        return super.templateAbspath(params)
   *      }
   *     ```
   * */
  async formatter(params: RP, formatterHandle?: string | undefined): Promise<TPLFormatter> {
    // cascade into merged params and stepInfo, and a name for the formatter
    const stepInfo = this.stepInfo<{ formatterHandle?: string }>(params)
    const handle = formatterHandle || stepInfo.formatterHandle || params.step || this.handle
    // get the formatter -- every step gets a formatter automatically; you must define any custom-handle formatters in this.Formatters
    const formatter = this._formatters[handle] as TPLFormatter
    CK.anything.cast(formatter)
    // if the formatter.format -- runs the template -- is already defined, return it
    // you'd intervene to define a custom formatter by pre-arranging this method property
    if (_.isFunction(formatter.format)) { params.templatePath = formatter.relpath; return formatter } // eslint-disable-line no-param-reassign
    // otherwise, find the template, save it to be memoized, then load&return the formatter
    const { templateFext = 'eta' } = formatter
    const templatePath = this.templateAbspath({ ...params, handle, barename: handle, ...formatter }) + '.' + templateFext
    // hang the path into the params -- it's nice to know where you came from
    params.templatePath = templatePath.replace(this.templatePathjoin('../..'), '.')
    formatter.relpath   = templatePath
    // load the templater function, attach it to the formatter, and return that formatter
    const format         = EtaUtils.load(templatePath, { ...params, ...formatter.extras, params, templater: this })
    UF.adorn(formatter, 'format', format)
    return formatter
  }
  async ensureTemplates(params: RP) {
    const loaded = await UF.AwaitBag(_.mapValues(this.Formatters, async (_t: any, barename) => (
      this.formatter(params, barename)
    )))
    return loaded
  }

  // async renderTemplateFile(params: RP, othervals: AnyBag = {}): Promise<string> {
  //   const template = await this.loadTemplateFile(params)
  //   return this.format(params, template, othervals)
  // }
  async loadTemplateFile(params: RP): Promise<string> {
    const abspath = this.templateAbspath(params)
    const template = await Filer.loadtext(abspath)
    if (template.ok) { return template.val }
    const { err } = template
    this.logger.error(`Issue while loading template: ${err.message}`, { err, params })
    throw UF.throwable(`Issue while loading template: ${err.message}`, 'loadErr', { err, params })
  }

  // async loadAndRender(relpath: Relpath, params: AnyBag = {}, othervals: AnyBag = {}): Promise<string> {
  //   const template = await Filer.loadtext(Filer.abspathFor(relpath))
  //   return this.format(params as any, template.val, othervals)
  // }

  async dump(params: RP): Promise<DumpFrame | FM.BadFilerWriteResult<FM.CoreWriteGist>> {
    const content = await this.render(params)
    return this.dumptext(content, params)
  }
  /**
   * @param params {Partial<TplPathingOpts>} - options for path generation
   * @returns {object}
   *   - ok {boolean} - whether the file was written
   *   - abspath {Relpath} - the actual path written
   *   - content {string}  - the rendered content of the file
   * */
  async dumptext(content: string, params?: { barename: Barename }      | undefined): Promise<DumpFrame | FM.BadFilerWriteResult<FM.CoreWriteGist>>
  async dumptext(content: string, params?:                          RP | undefined): Promise<DumpFrame | FM.BadFilerWriteResult<FM.CoreWriteGist>>
  async dumptext(content: string, params?: { barename: Barename } | RP | undefined): Promise<DumpFrame | FM.BadFilerWriteResult<FM.CoreWriteGist>> {
    try {
      const { step, handle = this.handle, pool = this.pool } = params as any
      const filepath = this.dumpfileAbspath(params as RP)
      const present = (!! content.trim())
      const gist =  present ? 'present' : 'empty'
      if (! present) { this.logger.warn(`empty content dumping ${this.title} step ${(params as any)?.step} to ${filepath}`, { filepath, gist, params }) }
      const result = await Filer.dumptext(content, filepath)
      this.logger.debug(`Dumped template: ${this.title}`, { ...result, params })
      return { ...result, gist, step, handle, pool, content } as DumpFrame
    } catch (err) { this.logger.error(`Issue while dumping template: ${err.message}`, { err, params }); throw err }
  }

  async renderSteps<ST extends XST>(params: RP, steps?: SteppedManifest<ST, RP> | undefined, _also: AnyBag = {}): Promise<SteppedContents<string>> {
    const manifest = this.extractStepManifest(steps)
    const contents = await UF.AwaitBag(_.mapValues(manifest, (overrides, step) => (
      this.render({ ...params, step, ...overrides } as RP)
    )))
    return contents
  }
  async dumpSteps<ST extends XST>(params: RP, steps?: SteppedManifest<ST, RP> | undefined, also: AnyBag = {}): Promise<SteppedFrames<ST>> {
    const manifest = this.extractStepManifest(steps)
    const contents = await this.renderSteps(params, steps, also)
    const results = await UF.AwaitBag(_.mapValues(contents, (content, step) => (
      this.dumptext(content, { ...params, step, ...manifest[step as ST] })
    )))
    return results as SteppedFrames<ST>
  }
  async dumpStep<ST extends XST>(params: RP, step: ST | SteppedManifest<ST, RP>, also: AnyBag = {}): Promise<DumpFrame> {
    const manifest = this.stepManifestForSingleStep(params, step)
    const theStep = _.first(_.keys(manifest)) as ST
    const contents = await this.renderSteps(params, manifest, also)
    const results = await UF.AwaitBag(_.mapValues(contents, (content) => (
      this.dumptext(content, { ...params, step: theStep, ...manifest[theStep] })
    )))
    return results[theStep]! as DumpFrame
  }
  stepManifestForSingleStep<ST extends XST>(params: RP, step: ST | SteppedManifest<ST, RP>): SteppedManifest<ST, RP> {
    if (_.isString(step)) { return this.extractStepManifest({ [step]: {} } as SteppedManifest<ST, RP>) }
    const isSingleStep = (_.size(step) === 1) && (UF.objectish(step))
    if (! isSingleStep) { throw UF.throwable(`Inconsistent step manifest ${step}`, 'inconsistent', { want: 'single step', val: step, params }) }
    return this.extractStepManifest(step)
  }
  extractStepManifest<ST extends XST, MFT extends SteppedManifest<ST, RP>>(steps: MFT): MFT
  extractStepManifest<ST extends XST>(steps?: readonly ST[] | SteppedManifest<ST, RP> | undefined): SteppedManifest<ST, RP>
  extractStepManifest<ST extends XST>(steps?: readonly ST[] | SteppedManifest<ST, RP> | undefined): SteppedManifest<ST, RP> {
    const dna = (_.isArray(steps) ? UF.objectify(steps, () => ({})) : steps ?? this.steps!) as SteppedManifest<ST, RP>
    return BaseTemplaterCx.stepManifest.cast(dna) as SteppedManifest<ST, RP>
  }
  stepInfo<VT>(params: Partial<VT> & { step?: RP['step'] | undefined }): TemplaterParamsish & VT {
    const stepInfo = (params.step ? this.steps?.[params.step] : {}) as TemplaterParamsish
    return { ...params, ...(stepInfo || {}) } as TemplaterParamsish & VT
  }

  templatePathjoin(...relpaths: (Relpath | undefined)[]): Abspath {
    return Filer.abspathFor(this.templateRoot, ..._.compact(relpaths))
  }
  // templateDirpath(basename: TY.Barename, params: RP & AnyBag): Abspath {
  //   const { pool = this.pool } = BaseTemplaterCx.genParams.cast(params)
  //   return this.templatePathjoin(this.ns, pool, basename)
  // }
  templateAbspath(params: RP & AnyBag): Abspath {
    const { pool = this.pool } = BaseTemplaterCx.genParams.cast(params)
    const basename = UF.smush('.', this.barenameFor(params), this.fextFor(params))
    return this.templatePathjoin(this.ns, pool, basename)
  }

  static mearthdir(...pathsegs: (Relpath | undefined)[]): Abspath {
    return Filer.__dirname(import.meta.url, '../..', ...pathsegs)
  }
  mearthdir(...pathsegs:        (Relpath | undefined)[]): Abspath { return BaseTemplater.mearthdir(...pathsegs) }
  static unmearthPath(abspath: Abspath): Relpath { return abspath.replace(this.mearthdir(), '.') }
  unmearthPath(abspath:        Abspath): Relpath { return BaseTemplater.unmearthPath(abspath) }

  dumpfileAbspath(params:  Partial<RP> = {}): Relpath {
    return Filer.abspathFor(this.rootpath, this.relpathFor(params))
  }

  /**
   * @param params {Partial<TplPathingOpts>} - options for path generation
   *   - relpath  {Relpath}   - relative path to the file; if given, overrides all given pathopts
   *   - dirname  {Relpath}   - dirname of the file;  if given, overrides all given dirname-related pathopts
   *   - barename {Handleish} - basename of the file; if given, overrides all given filename-related pathopts
   *   - fext     {Fext}      - file extension; defaults to this.fext
   *   - pool     {Handleish} - pool name;      defaults to this.pool
   *   - poolnum  {number}    - pool number;    defaults to this.poolnum
   *   - step     {Handleish} - step name;      defaults to this.handle
   *   - stepnum  {number}    - step number;    defaults to this.stepnum
   *
   * If `relpath` is given, all given pathopts are ignored.
   * File dirname  taken from this.dirname()
   * File barename taken from this.basename()
   * Omitted parts and their separators will be omitted
   *
   * @returns {Relpath} - the path to the file (relative to gendir)
   * */
  relpathFor(params?: Partial<RP>): Relpath
  relpathFor(_params: Partial<RP> = {}): Relpath {
    const params = BaseTemplaterCx.genParams.passthrough().cast(_params)
    if (params.relpath) { return params.relpath }
    const dirname  =  this.dirnameFor(params as Partial<RP>)
    const barename = this.barenameFor(params as Partial<RP>)
    return Filer.abspathFor(this.mainpath, dirname, UF.smush('.', barename, this.fextFor(params)))
  }

  /**
   * @param params {Partial<TplPathingOpts>} - options for path generation
   *   - dirname  {Relpath}   - dirname of the file;
   *     if given, overrides all other dirname-related pathopts.
   *   - pool     {Handleish} - pool name;   defaults to this.pool
   *   - poolnum  {number}    - pool number; defaults to this.poolnum
   *
   * If `relpath` is given, all other pathopts are ignored.
   * File dirname  will be `${poolnum}-${pool}` e.g. `25-permfuncs`.
   * File basename will be `${pool}-${stepnum}-${step}.${fext}`, e.g. `permfuncs-25-foo.sql`
   * Omitted parts and their separators will be omitted
   *
   * @returns {Relpath} - the directory name (relative to gendir)
   * */
  dirnameFor(params?: Partial<RP>):      Dirpath
  dirnameFor(_params: Partial<RP> = {}): Dirpath {
    const params = BaseTemplaterCx.genParams.cast(_params)
    if (params.dirname) { return params.dirname }
    const { pool = this.pool, poolnum = this.poolnum, step = this.handle } = params
    if (this.steps?.[step]?.dirname) { return this.steps[step].dirname }
    return UF.smush('-', this.padStepnum(poolnum), pool)
  }

  /**
   * @param params {Partial<TplPathingOpts>} - options for path generation
   *   - barename {Handleish} - basename of the file;
   *     if given, overrides all other barename-related pathopts.
   *   - pool     {Handleish} - pool name;   defaults to this.pool
   *   - step     {Handleish} - step name;   defaults to this.handle
   *   - stepnum  {number}    - step number; defaults to this.stepnum
   *
   * File basename will be `${pool}-${stepnum}-${step}.${fext}`, e.g. `permfuncs-25-prd`
   * Omitted parts and their separators will be omitted
   *
   * @returns {Barename} - the filename part without extension
   * */
  barenameFor(params?: Partial<RP> & AnyBag):      Barename
  barenameFor(_params: Partial<RP> & AnyBag = {}): Barename {
    const params = BaseTemplaterCx.genParams.cast(_params)
    if (params.barename) { return params.barename }
    const { pool = this.pool, stepnum = this.stepnum, step = this.handle } = params
    if (this.steps?.[step]?.barename) { return this.steps[step].barename }
    return UF.smush('-', this.padStepnum(stepnum), pool, step)
  }

  fextFor(params: { fext?: AnyFext | undefined } = {}): AnyFext { return params.fext ?? this.fext ?? '' }

  padStepnum(stepnum: string | TY.NumberMaybe, padlen = 3): string {
    if (! (_.isNumber(stepnum) || _.isString(stepnum))) { return '' }
    return _.padStart(String(stepnum), padlen, '0')
  }

  announceWrittenFile(result: DumpFrame, ...stories: any[]): void {
    const { step, handle = 'missing step/handle', abspath = 'missing path' } = result || {}
    console.info(`wrote ${step || handle} to ${abspath}: ${result?.ok ? 'hooray' : 'failed'}`, ...stories) // eslint-disable-line no-console
  }

}
