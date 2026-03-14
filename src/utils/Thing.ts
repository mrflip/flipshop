import      _                                   /**/ from 'lodash'
import      { CK }                                   from '@freeword/meta'

export class Thing {
  constructor(raw: object) {
    const props = this.Factory.checker.cast(raw)
    Object.assign(this, _.omitBy(props, _.isNil))
  }
  get Factory(): typeof Thing { return this.constructor as typeof Thing }
  static get checker(): { cast: (raw: any) => any, shape: Record<string, CK.Zchecker> } { return CK.obj({ }) }
  // static fill(raw: object) { console.log(this.checker.report(raw)); return this.checker.cast(raw) }
  static fill(raw: object) {
    try { return this.checker.cast(raw) } catch (err) { console.error(err); throw err }
  }
  static live(raw: object) { return new this(this.fill(raw)) }

  static get fieldnames(): string[] { return _.keys(this.checker.shape) }

  static unflat(vals: any[]) { return _.zipObject(this.fieldnames, vals) }
  flatten() { return _.pick(this, this.Factory.fieldnames) }
}
