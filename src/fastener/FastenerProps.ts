import type { Title, FastenerSizingSk } from './FastenerTypes.ts'
import { MM2Inch } from './FastenerTypes.ts'

export const FastenerProps = {
  '1/4': {
    title: '1/4-20', size_pref: 'A',
    diam_major: 0.250,
    coarse: {
      title: '1/4-20', stdz: 'UNC', pitch: MM2Inch / 20, diam_minor: 0.1905 * MM2Inch, thread_pref: 'a',
      taphole:     { nonfe_diam: 11.11, fe_diam: 11.11, pla_diam: 11.11, petg_diam: 11.11,  },
    },
    fine: {
      title: '1/4-28', stdz: 'UNF', pitch: MM2Inch / 28, diam_minor: 0.2074 * MM2Inch, thread_pref: 'b',
      taphole:     { nonfe_diam: 11.11, fe_diam: 11.11, pla_diam: 11.11, petg_diam: 11.11,  },
    },
    xfine: {
      title: '1/4-32', stdz: 'UNEF', pitch: MM2Inch / 32, diam_minor: 0.2128 * MM2Inch, thread_pref: 'c',
      taphole:     { nonfe_diam: 11.11, fe_diam: 11.11, pla_diam: 11.11, petg_diam: 11.11,  },
    },
    hhcs:     { driver_title: 'Wr7/16in', head_diam_af: 11.11, head_ht: 4.14 },
    shcs:     { driver_title: 'H3/16in',  head_diam_od: 11.11, head_ht: 4.14, key_diam_af: 11.11, key_dp: 11.11 },
    hexnut:   { driver_title: 'Wr7/16in', diam_af: 11.11,      ht: 5.56,      refsku:       'mcmc_91845A029' },
    sqnut:    { driver_title: 'Wr1/2in',  diam_af: 11.11,      ht: 4.14,      refsku:       'mcmc_94855A247' },
    // fw_reg:   {},
    // fw_lg:    {},
    // shcs:     {},
    // bhcs:     {},
    // fhcs:     {},
    // shlo:     {},
    // setscrew: {},
    // Washer
    fw_sm:    { diam_od: 11.11, diam_id: 11.11, ht: 11.11, stdz: 'USS' },
    //
    thruhole:    { close_diam: 11.11, reg_diam: 11.11, loose_diam: 11.11 },
  },
} satisfies Record<Title, FastenerSizingSk>
