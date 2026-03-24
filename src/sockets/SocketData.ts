import       _                               /**/ from 'lodash'
import type * as TY                               from './internal.ts'
import      { UF }                                from '@freeword/meta'
import      { SocketWrench }                      from './SocketModel.ts'
import type { SocketWrenchT }                     from './SocketTypes.ts'
import type { FastenerDrive, SocketKind, SocketReach, ToolDrive } from '../fastener/FastenerEnums.ts'
import      { SocketKindTitles, SocketDriveTitles, SocketReachTitles, ToolDriveTitles }  from '../fastener/FastenerEnums.ts'
import      { canhasbucket }                      from '../utils/DatafileHelpers.ts'

// export const SocketWrenchList: SocketWrench[] = []
export const SocketWrenchByTitle:   Record<string, SocketWrench>   = {}
export const SocketWrenchesByFamily: Record<string, SocketWrench[]> = {}
export const SocketWrenches: TY.PartialBag<SocketKind, TY.PartialBag<FastenerDrive, TY.PartialBag<ToolDrive, TY.PartialBag<SocketReach, SocketWrench>>>> = {}

export async function loadSocketWrenches(): Promise<TY.Bag<SocketWrench>> {
  const { default: RawSocketWrenches } = await import('../../data/sockets/sockets.json', { with: { type: 'json' } })
  _.each(RawSocketWrenches as SocketWrenchT[], (raw) => {
    const socket = SocketWrench.live(raw)
    SocketWrenchByTitle[socket.title] = socket
    ;(SocketWrenchesByFamily[socket.familyTitle] ??= []).push(socket)
    const bucket = canhasbucket(SocketWrenches, [socket.socket_kind, socket.drive_kind, socket.unit_system, socket.sqdrive_size, socket.reach_kind, socket.socket_variant])
    if (bucket[socket.sizing] && (! /^(socket_(sparkplug|ujoint|extension))$/.test(socket.socket_kind))) { console.warn('Duplicate sizing:', socket.sizing, bucket[socket.sizing], socket) }
    bucket[socket.sizing] = socket
  })
  // console.log(UF.inspectify(SocketWrenches))
  return SocketWrenchByTitle
}

/** Converts a family display title to a valid FeatureScript enum identifier (UPPER_SNAKE_CASE). */
export function familyTitleToEnumKey(title: string): string {
  return 'S_' + title.toUpperCase().replace(/[^A-Z0-9]+/g, '_').replace(/^_+|_+$/g, '')
}

// Axis metadata for the 6 levels of the SocketWrenches tree — used when emitting SocketWrenches2
const axisWrapperInfo = [
  { name: 'socket_kind',    displayName: 'Socket Kind',       titles: SocketKindTitles                                        as TY.AnyBag },
  { name: 'drive_kind',     displayName: 'Drive Kind',        titles: SocketDriveTitles                                       as TY.AnyBag },
  { name: 'unit_system',    displayName: 'Unit System',       titles: { metric: 'Metric', us: 'US' }                          as TY.AnyBag },
  { name: 'sqdrive_size',   displayName: 'Square Drive Size', titles: ToolDriveTitles                                         as TY.AnyBag },
  { name: 'reach_kind',     displayName: 'Reach',             titles: { ...SocketReachTitles, other: 'Other' }                as TY.AnyBag },
  { name: 'socket_variant', displayName: 'Socket Variant',    titles: { std: 'Standard', impact: 'Impact', ball: 'Ball End' } as TY.AnyBag },
  { name: 'sizing',         displayName: 'Sizing',            titles: {} as TY.AnyBag },
]

/** Walks the SocketWrenches tree depth-first; at depth 6 (the sizing map level) records
 *  familyTitle → "SocketWrenches2.entries[...].entries[...]..." using a representative socket. */
function buildFamilyPathMap(tree: typeof SocketWrenches): TY.AnyBag {
  const result: TY.AnyBag = {}
  function walk(node: unknown, rawKeys: string[], displayKeys: string[]) {
    if (rawKeys.length === 6) {
      const socket = Object.values(node as Record<string, unknown>).find(v => v instanceof SocketWrench) as SocketWrench | undefined
      if (socket) {
        const path = 'SocketWrenches2' + displayKeys.map(dk => `.entries[${JSON.stringify(dk)}]`).join('')
        result[socket.familyTitle] = path
      }
      return
    }
    const { titles } = axisWrapperInfo[rawKeys.length]!
    for (const [key, child] of Object.entries(node as Record<string, unknown>)) {
      const displayKey = titles[key] ?? key
      walk(child, [...rawKeys, key], [...displayKeys, displayKey])
    }
  }
  walk(tree, [], [])
  return result
}

const socketWrenchesFeaturescriptHeader = `
FeatureScript 2909;
import(path : "onshape/std/common.fs", version : "2909.0");

const mm = millimeter;
`.trim()

export function socketWrenchesToFeaturescript(tree: typeof SocketWrenches): string {
  function renderNode(node: unknown, depth: number): string {
    if (node instanceof SocketWrench) { return node.toFeaturescript() }
    const pad      = '  '.repeat(depth)
    const innerPad = '  '.repeat(depth + 1)
    const lines    = Object.entries(node as Record<string, unknown>)
      .map(([kk, vv]) => `${innerPad}${JSON.stringify(kk)}: ${renderNode(vv, depth + 1)}`)
    return `{\n${lines.join(',\n')}\n${pad}}`
  }
  function renderWrapped(node: unknown, level: number, indent: number, rawKeyPath: string[], stopDepth: number, arrayLeaf = false): string {
    if (level === stopDepth) {
      const prefix = 'SocketWrenches["' + rawKeyPath.join('"]["') + '"]'
      if (arrayLeaf) {
        const pad  = '  '.repeat(indent)
        const pad1 = '  '.repeat(indent + 1)
        const items = Object.keys(node as Record<string, unknown>).map(sizing => `${pad1}${prefix}[${JSON.stringify(sizing)}]`)
        return `[\n${items.join(',\n')}\n${pad}]`
      }
      return prefix
    }
    const { name, displayName, titles } = axisWrapperInfo[level]!
    const pad  = '  '.repeat(indent)
    const pad1 = '  '.repeat(indent + 1)
    const pad2 = '  '.repeat(indent + 2)
    const header = `{ "name": ${JSON.stringify(name)}, "displayName": ${JSON.stringify(displayName)}, "entries": {`
    if (level === stopDepth - 1) {
      // Leaf level: data entries at pad2; two separate closing braces
      const entryLines = Object.entries(node as Record<string, unknown>).map(([kk, vv]) => {
        const dk = titles[kk] ?? kk
        return `${pad2}${JSON.stringify(dk)}: ${renderWrapped(vv, stopDepth, indent + 2, [...rawKeyPath, kk], stopDepth, arrayLeaf)}`
      })
      return `${header}\n${entryLines.join(',\n')}\n${pad1}}\n${pad}}`
    } else {
      // Non-leaf: sub-wrapper entries at pad1; double-close at pad
      const entryLines = Object.entries(node as Record<string, unknown>).map(([kk, vv]) => (
        `${pad1}${JSON.stringify(titles[kk] ?? kk)}: ${renderWrapped(vv, level + 1, indent + 1, [...rawKeyPath, kk], stopDepth, arrayLeaf)}`
      ))
      return `${header}\n${entryLines.join(',\n')}\n${pad}}}`
    }
  }
  const familyPaths  = buildFamilyPathMap(tree)
  const familyEntries = Object.entries(familyPaths)
  const familyLines   = familyEntries.map(([title, path]) => `  SocketFamilyEnum.${_.padEnd(familyTitleToEnumKey(title) + ':', 65)} ${path}`)
  const familyConst   = `export const SocketWrenchesByFamily = {\n${familyLines.join(',\n')}\n};`
  const enumValues    = familyEntries.map(([title]) =>
    `  annotation { "Name": ${JSON.stringify(title)} }\n  ${familyTitleToEnumKey(title)}`
  )
  const familyEnum    = `export enum SocketFamilyEnum {\n${enumValues.join(',\n')}\n}`
  return socketWrenchesFeaturescriptHeader
    + `\n\nexport const SocketWrenches = ${renderNode(tree, 0)};`
    + `\n\nexport const SocketWrenches2 = ${renderWrapped(tree, 0, 1, [], 6, true)};`
    + `\n\nexport const SocketWrenches3 = ${renderWrapped(tree, 0, 1, [], 7)};`
    + `\n\n${familyEnum}\n\n${familyConst}`
}