FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
import(path : "4ebdc64943b566160ea5cc28", version : "12537da8aac0b4ff47ae5cb3");

export const SocketWrenchChoiceStructure = [
  ['socket_kind', { socket_bit: 'Bit Socket', socket_exthex: 'Bolt Socket' }],
  ['drive_kind', { inthex: 'Int Hex', exthex: '6-Point' }],
  ['unit_system', { us: 'US' }],
  ['sqdrive_size', 'Square Drive Size', { isq_0250in: '1/4Dr', isq_0375in: '3/8Dr', isq_0500in: '1/2Dr'}],
  'sizing',
];

export const SocketWrenches = {
  socket_bit: {
    inthex: {
      metric: {
        isq_0250in: {
          "H2mm": { "title": "H2mm Int Hex MM 1/4Dr Regular", sizing_mm: 2 * mm, ln_overall: 38.862 * mm, "wx_overall": 12.9032 * mm, "wy_overall": 12.9032 * mm, targets: { "drives": "M2.5", "drives_alt": "M3", "shcs_sz": "M2.5", "fhcs_sz": "M3", "bhcs_sz": "M3", "sss_sz": "M4", "losock_sz": "M3" }, "wt": 0.02268 },
          "H2.5mm": { "title": "H2.5mm Int Hex MM 1/4Dr Regular", sizing_mm: 2.5 * mm, ln_overall: 38.862 * mm, "wx_overall": 12.9032 * mm, "wy_overall": 12.9032 * mm, targets: { "drives": "M3", "drives_alt": "M4", "shcs_sz": "M3", "fhcs_sz": "M4", "bhcs_sz": "M4", "sss_sz": "M5", "losock_sz": "M4" }, "wt": 0.02268 },
          "H3mm": { "title": "H3mm Int Hex MM 1/4Dr Regular", sizing_mm: 3 * mm, ln_overall: 38.862 * mm, "wx_overall": 12.9032 * mm, "wy_overall": 12.9032 * mm, targets: { "drives": "M4", "drives_alt": "M5", "shcs_sz": "M4", "fhcs_sz": "M5", "bhcs_sz": "M5", "sss_sz": "M6", "losock_sz": "M5" }, "wt": 0.02268 },
        },
        isq_0375in: {
          "H3mm": { "title": "H3mm Int Hex MM 3/8Dr Regular", sizing_mm: 3 * mm, ln_overall: 41.9862 * mm, "wx_overall": 17.4752 * mm, "wy_overall": 17.4752 * mm, targets: { "drives": "M4", "drives_alt": "M5", "shcs_sz": "M4", "fhcs_sz": "M5", "bhcs_sz": "M5", "sss_sz": "M6", "losock_sz": "M5" }, "wt": 0.049895 },
          "H7mm": { "title": "H7mm Int Hex MM 3/8Dr Regular", sizing_mm: 7 * mm, ln_overall: 46.99 * mm, "wx_overall": 17.4752 * mm, "wy_overall": 17.4752 * mm, targets: {}, "wt": 0.063503 },
          "H10mm": { "title": "H10mm Int Hex MM 3/8Dr Regular", sizing_mm: 10 * mm, ln_overall: 49.9872 * mm, "wx_overall": 19.05 * mm, "wy_overall": 19.05 * mm, targets: { "drives": "M12", "drives_alt": "M16", "shcs_sz": "M12", "fhcs_sz": "M16", "bhcs_sz": "M16", "sss_sz": "M20" }, "wt": 0.063503 },
        },
      },
      us: {
        isq_0375in: {
          "H1/4in": { "title": "H1/4in Int Hex US 3/8Dr Regular", sizing_mm: 6.35 * mm, ln_overall: 41.9862 * mm, "wx_overall": 17.4752 * mm, "wy_overall": 17.4752 * mm, targets: { "drives": "5/16in", "drives_alt": "7/16in", "shcs_sz": "5/16in", "fhcs_sz": "7/16in", "bhcs_sz": "7/16in", "sss_sz": "1/2in" }, "wt": 0.063503 },
          "H5/16in": { "title": "H5/16in Int Hex US 3/8Dr Regular", sizing_mm: 7.9375 * mm, ln_overall: 46.99 * mm, "wx_overall": 17.4752 * mm, "wy_overall": 17.4752 * mm, targets: { "drives": "3/8in", "drives_alt": "1/2in", "shcs_sz": "3/8in", "fhcs_sz": "1/2in", "bhcs_sz": "1/2in", "sss_sz": "5/8in" }, "wt": 0.063503 },
          "H3/8in": { "title": "H3/8in Int Hex US 3/8Dr Regular", sizing_mm: 9.525 * mm, ln_overall: 46.99 * mm, "wx_overall": 19.05 * mm, "wy_overall": 19.05 * mm, targets: { "drives": "1/2in", "drives_alt": "5/8in", "shcs_sz": "1/2in", "fhcs_sz": "5/8in", "bhcs_sz": "5/8in", "sss_sz": "3/4in" }, "wt": 0.063503, "wt_lb": 0.14, "sku": "80421", "upc": "099575804212", "url": "https://www.gearwrench.com/all-tools/ratchets-sockets/chrome-sockets/80421-38-drive-hex-bit-sae-socket-38", "img_url": "https://www.gearwrench.com/sites/gearwrench/files/styles/large/public/pim_images/GW_80421_FRNT_MAIN.jpg" }
        },
        isq_0500in: {
          "H5/16in": { "title": "H5/16in Int Hex US 1/2Dr Regular", sizing_mm: 7.9375 * mm, ln_overall: 62.992 * mm, "wx_overall": 23.9776 * mm, "wy_overall": 23.9776 * mm, targets: { "drives": "3/8in", "drives_alt": "1/2in", "shcs_sz": "3/8in", "fhcs_sz": "1/2in", "bhcs_sz": "1/2in", "sss_sz": "5/8in" }, "wt": 0.095254 },
          "H3/8in": { "title": "H3/8in Int Hex US 1/2Dr Regular", sizing_mm: 9.525 * mm, ln_overall: 62.992 * mm, "wx_overall": 23.9776 * mm, "wy_overall": 23.9776 * mm, targets: { "drives": "1/2in", "drives_alt": "5/8in", "shcs_sz": "1/2in", "fhcs_sz": "5/8in", "bhcs_sz": "5/8in", "sss_sz": "3/4in" }, "wt": 0.104326 },
        },
      },
    },
    phillips: {
      metric: {
        isq_0375in: {
          "Ph1": { "title": "Ph1 Phillips 3/8Dr Regular", sizing_mm: 3 * mm, ln_overall: 41.9862 * mm, "wx_overall": 17.4752 * mm, "wy_overall": 17.4752 * mm, targets: {}, "wt": 0.040823 },
          "Ph2": { "title": "Ph2 Phillips 3/8Dr Regular", sizing_mm: 5 * mm, ln_overall: 41.9862 * mm, "wx_overall": 17.4752 * mm, "wy_overall": 17.4752 * mm, targets: {}, "wt": 0.040823 },
          "Ph3": { "title": "Ph3 Phillips 3/8Dr Regular", sizing_mm: 8 * mm, ln_overall: 41.9862 * mm, "wx_overall": 17.4752 * mm, "wy_overall": 17.4752 * mm, targets: {}, "wt": 0.045359, "wt_lb": 0.1, "sku": "80469", "upc": "099575804694", "url": "https://www.gearwrench.com/all-tools/ratchets-sockets/chrome-sockets/80469-38-drive-phillipsr-bit-socket-3", "img_url": "https://www.gearwrench.com/sites/gearwrench/files/styles/large/public/pim_images/GW_80469_FRNT_MAIN.jpg" }
        },
      },
    },
  },
  socket_exthex: {
    exthex: {
      metric: {
        isq_0250in: {
          "4mm": { "title": "4mm 6-Point MM 1/4Dr Regular", sizing_mm: 4 * mm, ln_overall: 24.511 * mm, "wx_overall": 11.811 * mm, "wy_overall": 11.811 * mm, "target_end_diam": 6.7056 * mm, targets: { "drives": "M2", "exthex_sz": "M2", "hhcs_sz": "M2", "hn_sz": "NM2", "hhn_sz": "HNM2" }, "wt": 0.01134 },
          "7mm": { "title": "7mm 6-Point MM 1/4Dr Regular", sizing_mm: 7 * mm, ln_overall: 24.511 * mm, "wx_overall": 11.811 * mm, "wy_overall": 11.811 * mm, "target_end_diam": 10.6934 * mm, targets: { "drives": "M4", "exthex_sz": "M4", "hhcs_sz": "M4", "hn_sz": "NM4", "hhn_sz": "HNM4" }, "wt": 0.009072 },
          "8mm": { "title": "8mm 6-Point MM 1/4Dr Regular", sizing_mm: 8 * mm, ln_overall: 24.511 * mm, "wx_overall": 11.811 * mm, "wy_overall": 11.811 * mm, "target_end_diam": 11.5062 * mm, targets: { "drives": "M5", "exthex_sz": "M5", "hhcs_sz": "M5", "hn_sz": "NM5", "hhn_sz": "HNM5" }, "wt": 0.009072 },
          "10mm": { "title": "10mm 6-Point MM 1/4Dr Regular", sizing_mm: 10 * mm, ln_overall: 24.511 * mm, "wx_overall": 14.3002 * mm, "wy_overall": 14.3002 * mm, "target_end_diam": 14.3002 * mm, targets: { "drives": "M6", "exthex_sz": "M6", "hhcs_sz": "M6", "hn_sz": "NM6", "hhn_sz": "HNM6" }, "wt": 0.018144 },
          "13mm": { "title": "13mm 6-Point MM 1/4Dr Regular", sizing_mm: 13 * mm, ln_overall: 24.511 * mm, "wx_overall": 17.907 * mm, "wy_overall": 17.907 * mm, "target_end_diam": 17.907 * mm, targets: { "drives": "M8", "exthex_sz": "M8", "hhcs_sz": "M8", "hn_sz": "NM8", "hhn_sz": "HNM8" }, "wt": 0.018144 },
        },
        isq_0375in: {
          "6mm": { "title": "6mm 6-Point MM 3/8Dr Regular", sizing_mm: 6 * mm, ln_overall: 24.9936 * mm, "wx_overall": 16.9926 * mm, "wy_overall": 16.9926 * mm, "target_end_diam": 9.2964 * mm, targets: {}, "wt": 0.018144 },
          "7mm": { "title": "7mm 6-Point MM 3/8Dr Regular", sizing_mm: 7 * mm, ln_overall: 24.9936 * mm, "wx_overall": 16.9926 * mm, "wy_overall": 16.9926 * mm, "target_end_diam": 10.795 * mm, targets: { "drives": "M4", "exthex_sz": "M4", "hhcs_sz": "M4", "hn_sz": "NM4", "hhn_sz": "HNM4" }, "wt": 0.013608 },
          "8mm": { "title": "8mm 6-Point MM 3/8Dr Regular", sizing_mm: 8 * mm, ln_overall: 24.9936 * mm, "wx_overall": 16.9926 * mm, "wy_overall": 16.9926 * mm, "target_end_diam": 11.9888 * mm, targets: { "drives": "M5", "exthex_sz": "M5", "hhcs_sz": "M5", "hn_sz": "NM5", "hhn_sz": "HNM5" }, "wt": 0.013608 },
          "9mm": { "title": "9mm 6-Point MM 3/8Dr Regular", sizing_mm: 9 * mm, ln_overall: 24.9936 * mm, "wx_overall": 16.9926 * mm, "wy_overall": 16.9926 * mm, "target_end_diam": 12.9032 * mm, targets: {}, "wt": 0.018144 },
          "10mm": { "title": "10mm 6-Point MM 3/8Dr Regular", sizing_mm: 10 * mm, ln_overall: 24.9936 * mm, "wx_overall": 16.9926 * mm, "wy_overall": 16.9926 * mm, "target_end_diam": 14.5034 * mm, targets: { "drives": "M6", "exthex_sz": "M6", "hhcs_sz": "M6", "hn_sz": "NM6", "hhn_sz": "HNM6" }, "wt": 0.018144 },
          "11mm": { "title": "11mm 6-Point MM 3/8Dr Regular", sizing_mm: 11 * mm, ln_overall: 24.9936 * mm, "wx_overall": 16.9926 * mm, "wy_overall": 16.9926 * mm, "target_end_diam": 15.7988 * mm, targets: {}, "wt": 0.013608 },
          "12mm": { "title": "12mm 6-Point MM 3/8Dr Regular", sizing_mm: 12 * mm, ln_overall: 24.9936 * mm, "wx_overall": 16.9926 * mm, "wy_overall": 16.9926 * mm, "target_end_diam": 16.6116 * mm, targets: { "drives": "D8", "exthex_sz": "D8", "hhcs_sz": "D8", "hn_sz": "ND8", "hhn_sz": "HND8" }, "wt": 0.013608 },
        },
      },
      us: {
        isq_0250in: {
          "5/32in": { "title": "5/32in 6-Point US 1/4Dr Regular", sizing_mm: 3.9688 * mm, ln_overall: 24.511 * mm, "wx_overall": 11.811 * mm, "wy_overall": 11.811 * mm, "target_end_diam": 6.9088 * mm, targets: {}, "wt": 0.009072 },
          "3/16in": { "title": "3/16in 6-Point US 1/4Dr Regular", sizing_mm: 4.7625 * mm, ln_overall: 24.511 * mm, "wx_overall": 11.811 * mm, "wy_overall": 11.811 * mm, "target_end_diam": 7.7978 * mm, targets: { "drives": "#2", "exthex_sz": "#2", "hhcs_sz": "#2", "hn_sz": "N#2" }, "wt": 0.009072 },
          "7/32in": { "title": "7/32in 6-Point US 1/4Dr Regular", sizing_mm: 5.5562 * mm, ln_overall: 24.511 * mm, "wx_overall": 11.811 * mm, "wy_overall": 11.811 * mm, "target_end_diam": 8.509 * mm, targets: {}, "wt": 0.009072 },
          "1/4in": { "title": "1/4in 6-Point US 1/4Dr Regular", sizing_mm: 6.35 * mm, ln_overall: 24.511 * mm, "wx_overall": 11.811 * mm, "wy_overall": 11.811 * mm, "target_end_diam": 9.4996 * mm, targets: { "drives": "#4", "exthex_sz": "#4", "hhcs_sz": "#4", "hn_sz": "N#4" }, "wt": 0.009072 },
          "9/32in": { "title": "9/32in 6-Point US 1/4Dr Regular", sizing_mm: 7.1438 * mm, ln_overall: 24.511 * mm, "wx_overall": 11.811 * mm, "wy_overall": 11.811 * mm, "target_end_diam": 10.6934 * mm, targets: {}, "wt": 0.009072 },
          "5/16in": { "title": "5/16in 6-Point US 1/4Dr Regular", sizing_mm: 7.9375 * mm, ln_overall: 24.511 * mm, "wx_overall": 11.811 * mm, "wy_overall": 11.811 * mm, "target_end_diam": 11.5062 * mm, targets: { "drives": "#6", "exthex_sz": "#6", "hhcs_sz": "#6", "hn_sz": "N#6" }, "wt": 0.009072 },
          "11/32in": { "title": "11/32in 6-Point US 1/4Dr Regular", sizing_mm: 8.7313 * mm, ln_overall: 24.511 * mm, "wx_overall": 12.4968 * mm, "wy_overall": 12.4968 * mm, "target_end_diam": 12.4968 * mm, targets: { "drives": "#8", "exthex_sz": "#8", "hhcs_sz": "#8", "hn_sz": "N#8" }, "wt": 0.018144 },
          "3/8in": { "title": "3/8in 6-Point US 1/4Dr Regular", sizing_mm: 9.525 * mm, ln_overall: 24.511 * mm, "wx_overall": 13.7922 * mm, "wy_overall": 13.7922 * mm, "target_end_diam": 13.7922 * mm, targets: { "drives": "#10", "exthex_sz": "#10", "hhcs_sz": "#10", "hn_sz": "N#10" }, "wt": 0.009072 },
          "7/16in": { "title": "7/16in 6-Point US 1/4Dr Regular", sizing_mm: 11.1125 * mm, ln_overall: 24.511 * mm, "wx_overall": 15.6972 * mm, "wy_overall": 15.6972 * mm, "target_end_diam": 15.6972 * mm, targets: { "drives": "1/4in", "exthex_sz": "1/4in", "hhcs_sz": "1/4in", "hn_sz": "N1/4in" }, "wt": 0.013608 },
          "1/2in": { "title": "1/2in 6-Point US 1/4Dr Regular", sizing_mm: 12.7 * mm, ln_overall: 24.511 * mm, "wx_overall": 17.2974 * mm, "wy_overall": 17.2974 * mm, "target_end_diam": 17.2974 * mm, targets: { "drives": "5/16in", "exthex_sz": "5/16in", "hhcs_sz": "5/16in", "hn_sz": "N5/16in", "hhn_sz": "HN1/4in" }, "wt": 0.013608 },
          "9/16in": { "title": "9/16in 6-Point US 1/4Dr Regular", sizing_mm: 14.2875 * mm, ln_overall: 24.511 * mm, "wx_overall": 19.5072 * mm, "wy_overall": 19.5072 * mm, "target_end_diam": 19.5072 * mm, targets: { "drives": "3/8in", "exthex_sz": "3/8in", "hhcs_sz": "3/8in", "hn_sz": "N3/8in", "hhn_sz": "HN5/16in" }, "wt": 0.018144, "wt_lb": 0.04, "sku": "80114D", "upc": "099575801143", "url": "https://www.gearwrench.com/all-tools/ratchets-sockets/chrome-sockets/80114d-14-drive-6-point-standard-sae-socket-916", "img_url": "https://www.gearwrench.com/sites/gearwrench/files/styles/large/public/pim_images/GW_80114D_IMG-MAIN.jpg" }
        },
      },
    },
  },
};

export const SocketWrenchChoices = {
  "Bit Socket": { "name": "drive_kind", "displayName": "Drive Kind", "entries": {
    "Int Hex": { "name": "unit_system", "displayName": "Unit System", "entries": {
      "Metric": { "name": "sqdrive_size", "displayName": "Square Drive Size", "entries": {
        "1/4Dr": { "name": "sizing", "displayName": "Sizing", "entries": {
          "H2mm": SocketWrenches["socket_bit"]["inthex"]["metric"]["isq_0250in"]["H2mm"],
          "H2.5mm": SocketWrenches["socket_bit"]["inthex"]["metric"]["isq_0250in"]["H2.5mm"],
          "H3mm": SocketWrenches["socket_bit"]["inthex"]["metric"]["isq_0250in"]["H3mm"],
        } },
        "3/8Dr": { "name": "sizing", "displayName": "Sizing", "entries": {
          "H3mm": SocketWrenches["socket_bit"]["inthex"]["metric"]["isq_0375in"]["H3mm"],
          "H7mm": SocketWrenches["socket_bit"]["inthex"]["metric"]["isq_0375in"]["H7mm"],
          "H10mm": SocketWrenches["socket_bit"]["inthex"]["metric"]["isq_0375in"]["H10mm"]
        } },
      } },
      "US": { "name": "sqdrive_size", "displayName": "Square Drive Size", "entries": {
        "3/8Dr": { "name": "sizing", "displayName": "Sizing", "entries": {
          "H1/4in": SocketWrenches["socket_bit"]["inthex"]["us"]["isq_0375in"]["H1/4in"],
          "H5/16in": SocketWrenches["socket_bit"]["inthex"]["us"]["isq_0375in"]["H5/16in"],
          "H3/8in": SocketWrenches["socket_bit"]["inthex"]["us"]["isq_0375in"]["H3/8in"]
        } },
        "1/2Dr": { "name": "sizing", "displayName": "Sizing", "entries": {
          "H5/16in": SocketWrenches["socket_bit"]["inthex"]["us"]["isq_0500in"]["H5/16in"],
          "H3/8in": SocketWrenches["socket_bit"]["inthex"]["us"]["isq_0500in"]["H3/8in"],
        } },
      } },
    } },
    "Phillips": { "name": "unit_system", "displayName": "Unit System", "entries": {
      "Metric": { "name": "sqdrive_size", "displayName": "Square Drive Size", "entries": {
        "3/8Dr": { "name": "sizing", "displayName": "Sizing", "entries": {
          "Ph1": SocketWrenches["socket_bit"]["phillips"]["metric"]["isq_0375in"]["Ph1"],
          "Ph2": SocketWrenches["socket_bit"]["phillips"]["metric"]["isq_0375in"]["Ph2"],
          "Ph3": SocketWrenches["socket_bit"]["phillips"]["metric"]["isq_0375in"]["Ph3"]
        } },
      } },
    } },
  } },
  "Bolt Socket": { "name": "drive_kind", "displayName": "Drive Kind", "entries": {
      "6-Point": { "name": "unit_system", "displayName": "Unit System", "entries": {
        "Metric": { "name": "sqdrive_size", "displayName": "Square Drive Size", "entries": {
          "1/4Dr": { "name": "sizing", "displayName": "Sizing", "entries": {
            "4mm": SocketWrenches["socket_exthex"]["exthex"]["metric"]["isq_0250in"]["4mm"],
            "7mm": SocketWrenches["socket_exthex"]["exthex"]["metric"]["isq_0250in"]["7mm"],
            "8mm": SocketWrenches["socket_exthex"]["exthex"]["metric"]["isq_0250in"]["8mm"],
            "10mm": SocketWrenches["socket_exthex"]["exthex"]["metric"]["isq_0250in"]["10mm"],
            "13mm": SocketWrenches["socket_exthex"]["exthex"]["metric"]["isq_0250in"]["13mm"],
          } },
          "3/8Dr": { "name": "sizing", "displayName": "Sizing", "entries": {
            "6mm": SocketWrenches["socket_exthex"]["exthex"]["metric"]["isq_0375in"]["6mm"],
            "7mm": SocketWrenches["socket_exthex"]["exthex"]["metric"]["isq_0375in"]["7mm"],
            "8mm": SocketWrenches["socket_exthex"]["exthex"]["metric"]["isq_0375in"]["8mm"],
            "9mm": SocketWrenches["socket_exthex"]["exthex"]["metric"]["isq_0375in"]["9mm"],
            "10mm": SocketWrenches["socket_exthex"]["exthex"]["metric"]["isq_0375in"]["10mm"],
            "11mm": SocketWrenches["socket_exthex"]["exthex"]["metric"]["isq_0375in"]["11mm"],
            "12mm": SocketWrenches["socket_exthex"]["exthex"]["metric"]["isq_0375in"]["12mm"],
          } },
        } },
        "US": { "name": "sqdrive_size", "displayName": "Square Drive Size", "entries": {
          "1/4Dr": { "name": "sizing", "displayName": "Sizing", "entries": {
            "1/4in": SocketWrenches["socket_exthex"]["exthex"]["us"]["isq_0250in"]["1/4in"],
            "9/32in": SocketWrenches["socket_exthex"]["exthex"]["us"]["isq_0250in"]["9/32in"],
            "5/16in": SocketWrenches["socket_exthex"]["exthex"]["us"]["isq_0250in"]["5/16in"],
          } },
        } },
      } },
  } },
};

export const SocketWrenchesDotted = {
  "socket_bit.inthex.metric.isq_0250in.H2mm": SocketWrenches["socket_bit"]["inthex"]["metric"]["isq_0250in"]["H2mm"],
  "socket_bit.inthex.metric.isq_0250in": {
    // this key should NOT be un-dotted: only act at level 1, do not descend
    "H2.5mm": SocketWrenches["socket_bit"]["inthex"]["metric"]["isq_0250in"]["H2.5mm"],
    "H3mm": SocketWrenches["socket_bit"]["inthex"]["metric"]["isq_0250in"]["H3mm"],
  },
  "socket_bit.inthex.metric.isq_0250in.H3mm": SocketWrenches["socket_bit"]["inthex"]["metric"]["isq_0250in"]["H3mm"],
  // dotted keys -- ocurring before or after a partial safe collision -- will still naturally merge
  "socket_bit.inthex.metric.isq_0375in.H3mm": SocketWrenches["socket_bit"]["inthex"]["metric"]["isq_0375in"]["H3mm"],
  "socket_bit.inthex": {
    "metric": {
      "isq_0375in": {
        "H7mm": SocketWrenches["socket_bit"]["inthex"]["metric"]["isq_0375in"]["H7mm"],
      },
    },
  },
  "socket_bit.inthex.metric.isq_0375in.H10mm": SocketWrenches["socket_bit"]["inthex"]["metric"]["isq_0375in"]["H10mm"],
  "socket_bit.inthex.us": {
    "isq_0375in": {
      "H1/4in": SocketWrenches["socket_bit"]["inthex"]["us"]["isq_0375in"]["H1/4in"],
      "H5/16in": SocketWrenches["socket_bit"]["inthex"]["us"]["isq_0375in"]["H5/16in"],
      "H3/8in": SocketWrenches["socket_bit"]["inthex"]["us"]["isq_0375in"]["H3/8in"],
    },
    "isq_0500in": {
      "H5/16in": SocketWrenches["socket_bit"]["inthex"]["us"]["isq_0500in"]["H5/16in"],
      "H3/8in": SocketWrenches["socket_bit"]["inthex"]["us"]["isq_0500in"]["H3/8in"],
    },
  },
  "socket_bit.phillips.metric.isq_0375in.Ph1": SocketWrenches["socket_bit"]["phillips"]["metric"]["isq_0375in"]["Ph1"],
  "socket_bit.phillips.metric.isq_0375in.Ph2": SocketWrenches["socket_bit"]["phillips"]["metric"]["isq_0375in"]["Ph2"],
  "socket_bit.phillips.metric.isq_0375in.Ph3": SocketWrenches["socket_bit"]["phillips"]["metric"]["isq_0375in"]["Ph3"],
};
