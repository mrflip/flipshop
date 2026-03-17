import       _                               /**/ from 'lodash'
import type * as TY                               from './internal.ts'
import      { UF }                                from '@freeword/meta'
import      { SocketWrench }                      from './SocketModel.ts'
import type { SocketWrenchT }                     from './SocketTypes.ts'
import type { FastenerDrive, SocketKind, SocketReach, ToolDrive } from '../fastener/FastenerEnums.ts'
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
    const bucket = canhasbucket(SocketWrenches, [socket.socket_kind, socket.drive_kind, socket.unit_system, socket.sqdrive_size, socket.socket_variant, socket.reach_kind])
    if (bucket[socket.sizing] && (! /^(socket_(sparkplug|ujoint|extension))$/.test(socket.socket_kind))) { console.warn('Duplicate sizing:', socket.sizing, bucket[socket.sizing], socket) }
    bucket[socket.sizing] = socket
  })
  // console.log(UF.inspectify(SocketWrenches))
  return SocketWrenchByTitle
}

/** Converts a family display title to a valid FeatureScript enum identifier (UPPER_SNAKE_CASE). */
export function familyTitleToEnumKey(title: string): string {
  return title.toUpperCase().replace(/[^A-Z0-9]+/g, '_').replace(/^_+|_+$/g, '')
}

/** Walks the SocketWrenches tree depth-first; at depth 6 (the sizing map level) records
 *  familyTitle → "SocketWrenches.k1.k2.k3.k4.k5.k6" using a representative socket. */
function buildFamilyPathMap(tree: typeof SocketWrenches): Record<string, string> {
  const result: Record<string, string> = {}
  function walk(node: unknown, keys: string[]) {
    if (keys.length === 6) {
      const socket = Object.values(node as Record<string, unknown>).find(v => v instanceof SocketWrench) as SocketWrench | undefined
      if (socket) { result[socket.familyTitle] = 'SocketWrenches.' + keys.join('.') }
      return
    }
    for (const [key, child] of Object.entries(node as Record<string, unknown>)) {
      walk(child, [...keys, key])
    }
  }
  walk(tree, [])
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
  const familyPaths   = buildFamilyPathMap(tree)
  const sortedEntries = Object.entries(familyPaths).sort(([a], [b]) => a.localeCompare(b))
  const familyLines   = sortedEntries.map(([title, path]) => `  ${_.padEnd(JSON.stringify(title) + ':', 65)} ${path}`)
  const familyConst   = `const SocketWrenchesByFamily = {\n${familyLines.join(',\n')}\n};`
  const enumValues    = sortedEntries.map(([title]) =>
    `  annotation { "Name": ${JSON.stringify(title)} }\n  ${familyTitleToEnumKey(title)}`
  )
  const familyEnum    = `export enum SocketFamilyEnum {\n${enumValues.join(',\n')}\n}`
  return socketWrenchesFeaturescriptHeader + `\n\nconst SocketWrenches = ${renderNode(tree, 0)};\n\n${familyConst}\n\n${familyEnum}`
}