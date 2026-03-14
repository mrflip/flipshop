#! /usr/bin/env yarn node
import      _                                /**/ from 'lodash'
import      { load as cheerioLoad }               from 'cheerio'
//
import type * as TY                               from '@freeword/meta'
import      { CK }                                from '@freeword/meta'
import      * as Fastener                         from '../fastener/index.ts'
import      * as Sockets                         from '../sockets/index.ts'

const { MM_IN, KG_LB } = Fastener

// == [Types] ==

export const gearwrenchSocket = CK.obj({
  ...Sockets.socketWrench.shape,
  is_knurled:           CK.bool,
  is_magnetic:          CK.bool,
  is_wobble:            CK.bool,
  is_locking:           CK.bool,
  is_quickrel:          CK.bool,
  is_prop65:            CK.bool,
  is_hiviz:             CK.bool,
  material:             CK.oneof([ 'Alloy Steel', 'Alloy Steel with S2 Steel Bit', 'Chrome-Molybdenum (Cr-Mo)']),
  surf_finish:          CK.oneof([ 'Full Polish Chrome', 'Full Polish Chrome Holder with Black Oxide Bit', 'Black Oxide', 'Manganese Phosphate', 'Industrial Black Finish']),
  ansi_stdz:            CK.oneof([ 'Meets or Exceeds' ]),
  asme_stdz:            CK.oneof([ 'B107.1', 'B107.5M', 'Meets or Exceeds', 'B107.34', 'B107.1 B107.5M', 'B107.2', 'B107.33M', 'B107.110-2012', 'B107.33']),
  usfed_stdz:           CK.oneof([ 'GGG-W-641E' ]),
}).partial().required({ title: true, sku: true, url: true, img_url: true }).strict()
export interface GearwrenchSocketT  extends CK.Zcasted<typeof gearwrenchSocket> {}
export interface GearwrenchSocketSk extends CK.Zsketch<typeof gearwrenchSocket> {}
// --

// == [Remaps] ==

export const sqdrive_size_remap: TY.Bag<Fastener.FastenerEnums.ToolDrive> = { "1/4 in": 'isq_1_4', "3/8 in": 'isq_3_8', "1/2 in": 'isq_1_2', '3/4 in': 'isq_3_4', '1 in': 'isq_1_in' } as const
export const drive_kind_remap: TY.Bag<Fastener.FastenerEnums.FastenerDrive> = {
  'Hex': 'inthex', 'Torx®': 'torx', 'Tamper Proof Torx®': 'torxtp', 'External Torx®': 'extstar', 'Ballpoint Hex': 'inthex', 'Triple Square': 'triple_square', 'Slotted': 'slotted', 'Phillips®': 'phillips', 'Pozidriv®': 'pozidriv',
  '6 Point': 'exthex',  '6 Point 6 Point': 'exthex', 'Ball Hex': 'inthex', 'Slotted Phillips®/Slotted/Pozidriv®': 'phillips', 'Phillips® Phillips®/Slotted/Pozidriv®': 'phillips', 'Pozidriv® Phillips®/Slotted/Pozidriv®': 'pozidriv',
  'Square Square': 'square', 'Square': 'square',
} as const
export const socket_kind_remap: TY.Bag<Fastener.FastenerEnums.SocketKind> = {
  'Spark Plug Socket Spark Plug Service Tools': "socket_sparkplug", 'Socket': 'socket_exthex', 'Bit Socket': 'socket_bit', 'Flex Socket': 'socket_flex', 'Socket Spark Plug Service Tools': "socket_sparkplug", 'Extension': "socket_extension",
  "Universal Joint": "socket_ujoint", "Universal Joint Socket": "socket_ujoint", "Socket Extension": 'socket_extension', 'Adapter': 'socket_adapter',
} as const
export const reach_kind_remap: TY.Bag<Fastener.FastenerEnums.SocketReach> = {
  'Standard': 'standard', 'Mid Length': 'midlen', 'Deep': 'deep', 'Long': 'long', 'Extra Long': 'xlong',
} as const

export const fieldname_remap = {
  "Size":               'size_nom',
  "UPC":                'upc',
  //
  "Type":               'socket_kind',
  "Drive Tang Size":    'sqdrive_size',
  "Drive Type":         'drive_kind',
  "Bit Type":           'bit_kind',
  "Length Format":      'reach_kind',
  //
  "Overall Length":     'ln_overall',
  "Overall Width":      'wd_overall',
  "Overall Height":     'ht_overall',
  "Exposed Bit Length": 'bit_ln_exposed',
  //
  "Bit Length":         'bit_ln',
  "Nose Diameter":      'nose_diam',
  "Drive End":          'drive_end_ln',
  "Bolt Clearance":     'bolt_clr',
  "Bolt Depth":         'bolt_depth',
  "Length to Shoulder": 'shoulder_ln',
  "Wrench Depth":       'wrench_dp',
  "Wrench End":         'wrench_end_ln',
  "Drive End Hex Across Flats": 'drive_end_hex_af',
  //
  "Male Drive Size":    'male_drive_size',
  "Female Drive Size":  'female_drive_size',
  //
  "Weight (Catalog)":   'wt_lb',
  "Hi-Viz®":            'is_hiviz',
  "Material":           'material',
  "Finish":             'surf_finish',
  "Knurled":            'is_knurled',
  "Magnetic":           'is_magnetic',
  "Wobble":             'is_wobble',
  "Locking":            'is_locking',
  "Quick Release":      'is_quickrel',
  "Prop 65":            'is_prop65',
  //
  "Packaging":           null,
  "Warranty":            null,
  "Size Range (Metric)": null,
  "Size Range (SAE)":    null,
  "SAE/Metric/Torx":     null,
  "Family Name":         null,
  "ANSI Specification": 'ansi_stdz',
  "ASME Specification": 'asme_stdz',
  "US Federal Specification": 'usfed_stdz',
}
// --

// == [Helpers] ==

/** Tracks enum values for each field */
const Enumish = { socket_kind: [], sqdrive_size: [], drive_kind: [], bit_kind: [], reach_kind: [], material: [], surf_finish: [], ansi_stdz: [], asme_stdz: [], usfed_stdz: [] }

/** Extracts dimensions from the raw compount specification text (`Dim. A : Overall Length : 10 in`) */
function extract_dim(rawkey: string, raw: string): [TY.Fieldname, number] {
  // the weird two-specs-in-one are only for spark plug sockets, discarding the second one
  const match = /^(.+?) : (\d+(?:\.\d+)?) (in|mm)(?: (Wrench End|Wrench Depth) : (\d+(\.\d+)?) (in|mm))?$/.exec(raw)
  if (! match) { throw new Error(`Invalid dimension: ${raw}`) }
  const [_s, raw_fn, valstr, units] = match
  //
  const fn = fieldname_remap[raw_fn!]; if (! fn) { console.warn(`Unknown dimension: ${rawkey} => ${raw}`) }
  const num = Number(valstr)
  const val = units === 'in' ? _.round(num * MM_IN, 4) : num
  // console.warn(`${rawkey} => ${raw} => ${fn} = ${val} ${units} ${num}`)
  return [fn ?? 'oops', val]
}
/** Extracts distances from simple specification text */
function extract_dist(raw: string): number {
  const match = /^(\d+(?:\.\d+)?) ?(in|mm)$/.exec(raw)
  if (! match) { throw new Error(`Invalid distance: ${raw}`) }
  const [_s,  valstr, units] = match
  //
  const num = Number(valstr)
  const val = units === 'in' ? _.round(num * MM_IN, 4) : num
  return val
}
// --

// == [Parsing] ==

/** Parses a gearwrench socket product page and returns a SocketWrenchProductT */
export function parseProductPage(filepath: TY.Anypath, textblob: string): GearwrenchSocketT {
  const $ = cheerioLoad(textblob)
  // Title: prefer og:title, fall back to <title>
  const ogTitle  = $('meta[property="og:title"]').attr('content') ?? ''
  const rawTitle    = ogTitle.trim() || $('title').text()
  const title = rawTitle.replace(/\s*-\s*Gearwrench\s*$/i, '').replace(/^\d+ /, "").trim()
  // URL from canonical or og:url
  const url      = $('link[rel="canonical"]').attr('href')
                ?? $('meta[property="og:url"]').attr('content')
                ?? ''
  // Main image from og:image or image_src link
  const img_url = $('meta[property="og:image"]').attr('content')
                ?? $('link[rel="image_src"]').attr('href')
                ?? ''

  // SKU from JSON-LD Product schema
  let sku = ''
  $('script[type="application/ld+json"]').each((_, el) => {
    try {
      const data = JSON.parse($(el).html() ?? '{}')
      const graph: any[] = data['@graph'] ?? [data]
      for (const node of graph) {
        if (node['@type'] === 'Product' && node.sku) {
          sku = String(node.sku)
          break
        }
      }
    } catch { /* malformed JSON-LD, skip */ }
  })

  // Specifications from <li id="specifications">
  const specifications: Record<string, string> = {}
  $('#specifications li.field__item').each((_, el) => {
    const spans = $(el).find('span')
    const label = spans.eq(0).text().replace(/\s*:\s*$/, '').trim()
    const value = spans.eq(1).text().replace(/\s+/g, ' ').trim()
    if (label) specifications[label] = value
  })
  const result: TY.AnyBag = { sku, title, url, img_url }

  _.each(specifications, (raw, key) => {
    if (/^Dim\./.test(key)) { const [fn, val] = extract_dim(key, raw); result[fn] = val; return }
    const fn = fieldname_remap[key]
    if (_.isNull(fn)) { return }
    if (fn === 'bit_kind' && /External/.test(raw)) { return } // a couple pages have bit types where it's not a bit kind
    if (fn === 'wt_lb')                       { result[fn] = Number(raw.replace(/ lb$/, '')); result.wt = _.round(result.wt_lb * KG_LB, 3); return }
    if (/drive_size$/.test(fn))               { result[fn] = sqdrive_size_remap[raw]; if (! result[fn]) { console.warn(`Unknown drive size: ${raw}`)   } return }
    if (/(bit|drive)_kind$/.test(fn))         { result[fn] = drive_kind_remap[raw];   if (! result[fn]) { console.warn(`Unknown drive kind: ${raw}`)   } return }
    if (fn === 'socket_kind')                 { result[fn] = socket_kind_remap[raw];  if (! result[fn]) { console.warn(`Unknown socket kind: ${raw}`)  } return }
    if (fn === 'reach_kind')                  { result[fn] = reach_kind_remap[raw];   if (! result[fn]) { console.warn(`Unknown reach kind: ${raw}`)   } return }
    if (/(overall|bit_ln_exposed)$/.test(fn)) { result[fn] = extract_dist(raw); return }
    if (fn === 'is_prop65' && /WARNING/.test(raw))    { result[fn] = true;  return }
    if (fn === 'is_prop65' && /No Warning/.test(raw)) { result[fn] = false; return }
    if (/is_/.test(fn)) { result[fn] = CK.boolish.cast(raw, { filepath, specifications, raw, key }); return }
    if (fn) { result[fn] = raw; return }
    console.warn(`Unknown specification: ${key} = ${raw}`)
  })
  if (result.drive_kind === 'extstar') { result.socket_kind = 'socket_extstar' }
  _.each(_.pick(result, _.keys(Enumish)), (val, key) => { const seen = Enumish[key]; if (! seen.includes(val)) { seen.push(val) }  })
  return gearwrenchSocket.cast(result as GearwrenchSocketSk, { filepath, specifications })
}
// --
