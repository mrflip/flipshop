export function fsStringField(kk: string, vv: string):  string { return `"${kk}": ${JSON.stringify(vv)}` }
export function fsBoolField  (kk: string, vv: boolean): string { return `"${kk}": ${vv}` }
export function fsMmField    (kk: string, vv: number):  string { return `"${kk}": ${vv} * mm` }
export function fsInchField  (kk: string, vv: number):  string { return `"${kk}": ${vv} * inch` }
export function fsNumField   (kk: string, vv: number):  string { return `"${kk}": ${vv}` }
