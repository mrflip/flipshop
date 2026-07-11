
export const Adapters = {

  exthex_0250in_pow: { // from 1/4 Hex Power --to-> 1/4insert m, 1/4 sqdr s/m, 3/8 sqdr s/m
    exthex_0250in_ins: {
      sm:   { title: "¼in Power Hex to ¼in Hex Insert 40mm",  abbr: "Bit",   driver_handle: "exthex_0250in_pow", driver_title: "¼in Power Hex", target_handle: "exthex_0250in_ins", driver_end_diam: 7.332, target_end_diam: 10.5, ln_overall:  40 },
      lg:   { title: "¼in Power Hex to ¼in Hex Insert 85mm",  abbr: "BitLg", driver_handle: "exthex_0250in_pow", driver_title: "¼in Power Hex", target_handle: "exthex_0250in_ins", driver_end_diam: 7.332, target_end_diam: 10.5, ln_overall:  85 },
      xl:   { title: "¼in Power Hex to ¼in Hex Insert 115mm", abbr: "BitXL", driver_handle: "exthex_0250in_pow", driver_title: "¼in Power Hex", target_handle: "exthex_0250in_ins", driver_end_diam: 7.332, target_end_diam: 10.5, ln_overall: 115 },
    },
    isq_0250in: {
      sm:  { title: "¼in Power Hex to ¼in Sq Dr 50mm",      abbr: "¼Sq S", driver_handle: "exthex_0250in_pow", driver_title: "¼in Power Hex", target_handle: "isq_0250in",       driver_end_diam: 7.332, target_end_diam: 10.5, ln_overall: 50 },
      med: { title: "¼in Power Hex to ¼in Sq Dr 65mm",      abbr: "¼Sq M", driver_handle: "exthex_0250in_pow", driver_title: "¼in Power Hex", target_handle: "isq_0250in",       driver_end_diam: 7.332, target_end_diam: 10.5, ln_overall: 65 },
    },
    isq_0375in: {
      sm:  { title: "¼in Power Hex to ⅜in Sq Dr 50mm",      abbr: "⅜Sq S", driver_handle: "exthex_0250in_pow", driver_title: "¼in Power Hex", target_handle: "isq_0375in",       driver_end_diam: 7.332, target_end_diam: 12.5, ln_overall: 52 },
      med: { title: "¼in Power Hex to ⅜in Sq Dr 65mm",      abbr: "⅜Sq M", driver_handle: "exthex_0250in_pow", driver_title: "¼in Power Hex", target_handle: "isq_0375in",       driver_end_diam: 7.332, target_end_diam: 12.5, ln_overall: 65 },
    },
    isq_0500in: {
      sm:  { title: "¼in Power Hex to ½in Sq Dr 51mm",      abbr: "½Sq S", driver_handle: "exthex_0250in_pow", driver_title: "¼in Power Hex", target_handle: "isq_0500in",       driver_end_diam: 7.332, target_end_diam: 15.4, ln_overall: 51 },
      med: { title: "¼in Power Hex to ½in Sq Dr 65mm",      abbr: "½Sq M", driver_handle: "exthex_0250in_pow", driver_title: "¼in Power Hex", target_handle: "isq_0500in",       driver_end_diam: 7.332, target_end_diam: 15.4, ln_overall: 65 },
    },
  },
  exthex_0250in_ins: { // from 1/4 Hex Insert --to-> 1/4 sqdr s, 3/8 sqdr s
    isq_0250in: {
      sm:  { title: "¼in Sq Dr 50mm for ¼in Hex Insert",      abbr: "¼Sq S", driver_handle: "exthex_0250in_ins", driver_title: "¼in Hex Insert", target_handle: "isq_0250in",     driver_end_diam: 7.332, target_end_diam: 8.2,  ln_overall: 31 },
    },
    isq_0375in: {
      sm:  { title: "⅜in Sq Dr 50mm for ¼in Hex Insert",      abbr: "⅜Sq S", driver_handle: "exthex_0250in_ins", driver_title: "¼in Hex Insert", target_handle: "isq_0375in",     driver_end_diam: 7.332, target_end_diam: 12.5, ln_overall: 39 },
    },
  },
  isq_0250in: { // from 1/4 Sq Dr --to-> 1/4insert, 1/4 sqdr s/m/l, 3/8 sqdr s/m
    exthex_0250in_ins: {
      med: { title: "¼in Hex Insert 33mm for ¼in Sq Dr", abbr: "Bit", driver_handle: "isq_0250in", driver_title: "¼in Sq Dr", target_handle: "exthex_0250in_ins", driver_end_diam: 12, target_end_diam: 14, ln_overall: 33 },
    },
    isq_0250in: {
      "2in - 1/4in ext":  { title:  "2in - ¼in Impact Extension", abbr: "¼Xt2",    driver_handle: "isq_0250in", driver_title: "¼SqDr Impact", target_handle: "isq_0250in", socket_variant: "impact", driver_end_diam: 15.9,   target_end_diam: 14.7, ln_overall: 50.8,  wt: 0.045359, wt_lb: 0.1, sku: "84172", upc: "099575841729", url: "https://www.gearwrench.com/all-tools/ratchets-sockets/impact-products/84172-14-drive-impact-extension-2", img_url: "https://www.gearwrench.com/sites/gearwrench/files/styles/large/public/pim_images/GW_84172_IMG-MAIN.jpg" },
      "4in - 1/4in ext":  { title:  "4in - ¼in Impact Extension", abbr: "¼Xt4",    driver_handle: "isq_0250in", driver_title: "¼SqDr Impact", target_handle: "isq_0250in", socket_variant: "impact", driver_end_diam: 15.9,   target_end_diam: 14.7, ln_overall: 101.6, wt: 0.090718, wt_lb: 0.2, sku: "84173", upc: "099575841736", url: "https://www.gearwrench.com/all-tools/ratchets-sockets/impact-products/84173-14-drive-impact-extension-4", img_url: "https://www.gearwrench.com/sites/gearwrench/files/styles/large/public/pim_images/GW_84173_IMG-MAIN.jpg" },
      "6in - 1/4in ext":  { title:  "6in - ¼in Impact Extension", abbr: "¼Xt6",    driver_handle: "isq_0250in", driver_title: "¼SqDr Impact", target_handle: "isq_0250in", socket_variant: "impact", driver_end_diam: 15.9,   target_end_diam: 14.7, ln_overall: 152.4, wt: 0.136078, wt_lb: 0.3, sku: "84174", upc: "099575841743", url: "https://www.gearwrench.com/all-tools/ratchets-sockets/impact-products/84174-14-drive-impact-extension-6", img_url: "https://www.gearwrench.com/sites/gearwrench/files/styles/large/public/pim_images/GW_84174_IMG-MAIN.jpg" }
    },
    isq_0375in: {
      med: { title: "¼in Socket Adapter for ⅜Sq Dr Impact",       abbr: "⅜Sq",     driver_handle: "isq_0250in", driver_title: "¼SqDr Impact", target_handle: "isq_0375in", socket_variant: "impact", driver_end_diam: 22.098, target_end_diam: 14.7, ln_overall: 38.1, wt: 0.058967, wt_lb: 0.13, sku: "84408", upc: "099575844089", url: "https://www.gearwrench.com/all-tools/ratchets-sockets/impact-products/84408-38-drive-38-f-x-12-m-impact-adapter", img_url: "https://www.gearwrench.com/sites/gearwrench/files/styles/large/public/pim_images/GW_84408_IMG-MAIN.jpg" }
    }
  },
  isq_0375in: { // from 3/8 Sq Dr --to-> 1/4insert, 1/4 sqdr, 3/8 sqdr s/m/l, 1/2 sqdr
    exthex_0250in_ins: {
      med: { title: "¼in Hex Insert 40mmfor ⅜in Sq Dr", abbr: "Bit", driver_handle: "isq_0375in", driver_title: "⅜in Sq Dr", target_handle: "exthex_0250in_ins", driver_end_diam: 17.8, target_end_diam: 14, ln_overall: 40 },
    },
    isq_0250in: {
      med: { title: "¼in Socket Adapter for ⅜Sq Dr Impact",       abbr: "⅜Sq",     driver_handle: "isq_0250in", driver_title: "¼SqDr Impact", target_handle: "isq_0375in", socket_variant: "impact", driver_end_diam: 22.098, target_end_diam: 8.2, ln_overall: 22 }
    },
    isq_0375in: {
      "3in - 3/8in ext":  { title:  "3in - ⅜in Impact Extension", abbr: "⅜Xt3",  driver_handle: "isq_0375in", driver_title: "⅜SqDr Impact", target_handle: "isq_0375in", socket_variant: "impact", driver_end_diam: 17.8, target_end_diam: 12.4, ln_overall: 76.2,  wt: 0.077111, wt_lb: 0.17,  sku: "84432N", upc: "099575834264", url: "https://www.gearwrench.com/all-tools/ratchets-sockets/impact-products/84432n-38-drive-locking-impact-extension-3",  img_url: "https://www.gearwrench.com/sites/gearwrench/files/styles/large/public/pim_images/GW_84432N_IMG-MAIN.jpg" },
      "6in - 3/8in ext":  { title:  "6in - ⅜in Impact Extension", abbr: "⅜Xt6",  driver_handle: "isq_0375in", driver_title: "⅜SqDr Impact", target_handle: "isq_0375in", socket_variant: "impact", driver_end_diam: 17.8, target_end_diam: 12.4, ln_overall: 152.4, wt: 0.149685, wt_lb: 0.33,  sku: "84433N", upc: "099575863349", url: "https://www.gearwrench.com/all-tools/ratchets-sockets/impact-products/84433n-38-drive-locking-impact-extension-6",  img_url: "https://www.gearwrench.com/sites/gearwrench/files/styles/large/public/pim_images/GW_84433N_IMG-MAIN.jpg" },
      "10in - 3/8in ext": { title: "10in - ⅜in Impact Extension", abbr: "⅜Xt10", driver_handle: "isq_0375in", driver_title: "⅜SqDr Impact", target_handle: "isq_0375in", socket_variant: "impact", driver_end_diam: 17.8, target_end_diam: 12.4, ln_overall: 254,   wt: 0.240404, wt_lb: 0.53,  sku: "84405",  upc: "099575844058", url: "https://www.gearwrench.com/all-tools/ratchets-sockets/impact-products/84405-38-drive-impact-extension-10",          img_url: "https://www.gearwrench.com/sites/gearwrench/files/styles/large/public/pim_images/GW_84405_MAIN.jpg" },
      "12in - 3/8in ext": { title: "12in - ⅜in Impact Extension", abbr: "⅜Xt12", driver_handle: "isq_0375in", driver_title: "⅜SqDr Impact", target_handle: "isq_0375in", socket_variant: "impact", driver_end_diam: 17.8, target_end_diam: 12.4, ln_overall: 304.8, wt: 0.281227, wt_lb: 0.62,  sku: "84434N", upc: "099575834349", url: "https://www.gearwrench.com/all-tools/ratchets-sockets/impact-products/84434n-38-drive-locking-impact-extension-12", img_url: "https://www.gearwrench.com/sites/gearwrench/files/styles/large/public/pim_images/GW_84434N_IMG-MAIN.jpg" },
      "15in - 3/8in ext": { title: "15in - ⅜in Impact Extension", abbr: "⅜Xt15", driver_handle: "isq_0375in", driver_title: "⅜SqDr Impact", target_handle: "isq_0375in", socket_variant: "impact", driver_end_diam: 17.8, target_end_diam: 12.4, ln_overall: 381,   wt: 0.349266, wt_lb: 0.77,  sku: "84409N", upc: "099575844096", url: "https://www.gearwrench.com/all-tools/ratchets-sockets/impact-products/84409n-38-drive-impact-extension-15",         img_url: "https://www.gearwrench.com/sites/gearwrench/files/styles/large/public/pim_images/GW_84409N_MAIN.jpg" },
      "18in - 3/8in ext": { title: "18in - ⅜in Impact Extension", abbr: "⅜Xt18", driver_handle: "isq_0375in", driver_title: "⅜SqDr Impact", target_handle: "isq_0375in", socket_variant: "impact", driver_end_diam: 17.8, target_end_diam: 12.4, ln_overall: 457.2, wt: 0.439077, wt_lb: 0.968, sku: "84436N", upc: "099575844362", url: "https://www.gearwrench.com/all-tools/ratchets-sockets/impact-products/84436n-38-drive-locking-impact-extension-18", img_url: "https://www.gearwrench.com/sites/gearwrench/files/styles/large/public/pim_images/GW_84436N_IMG-MAIN.jpg" },
      "24in - 3/8in ext": { title: "24in - ⅜in Impact Extension", abbr: "⅜Xt24", driver_handle: "isq_0375in", driver_title: "⅜SqDr Impact", target_handle: "isq_0375in", socket_variant: "impact", driver_end_diam: 17.8, target_end_diam: 12.4, ln_overall: 609.6, wt: 0.578784, wt_lb: 1.276, sku: "84437N", upc: "099575844379", url: "https://www.gearwrench.com/all-tools/ratchets-sockets/impact-products/84437n-38-drive-locking-impact-extension-24", img_url: "https://www.gearwrench.com/sites/gearwrench/files/styles/large/public/pim_images/GW_84437N_IMG-MAIN.jpg" },
      "33in - 3/8in ext": { title: "33in - ⅜in Impact Extension", abbr: "⅜Xt33", driver_handle: "isq_0375in", driver_title: "⅜SqDr Impact", target_handle: "isq_0375in", socket_variant: "impact", driver_end_diam: 17.8, target_end_diam: 12.4, ln_overall: 838.2, wt: 0.788344, wt_lb: 1.738, sku: "84438N", upc: "099575844386", url: "https://www.gearwrench.com/all-tools/ratchets-sockets/impact-products/84438n-38-drive-locking-impact-extension-33", img_url: "https://www.gearwrench.com/sites/gearwrench/files/styles/large/public/pim_images/GW_84438N_IMG-MAIN.jpg" }
    },
    isq_0500in: {
      med:       { title: "½in Socket Adapter for ⅜Sq Dr Impact", abbr: "⅜Sq",   driver_handle: "isq_0500in", driver_title: "⅜SqDr Impact", target_handle: "isq_0500in", socket_variant: "impact", driver_end_diam: 17.8, target_end_diam: 16.4, ln_overall: 38.1, wt: 0.058967, wt_lb: 0.13, sku: "84408", upc: "099575844089", url: "https://www.gearwrench.com/all-tools/ratchets-sockets/impact-products/84408-38-drive-38-f-x-12-m-impact-adapter", img_url: "https://www.gearwrench.com/sites/gearwrench/files/styles/large/public/pim_images/GW_84408_IMG-MAIN.jpg" }
    },
  },
  isq_0500in: { // from 1/2 Sq Dr --to-> 3/8 sqdr, 3/4 sqdr
    isq_0375in: {
      med: { title: "⅜in Socket Adapter for ½Dr Impact",    drive_kind: "intsq", driver_handle: "isq_0500in", target_handle: "isq_0375in", driver_end_diam: 25.4, target_end_diam: 14.7, ln_overall: 38.1, wt: 0.072575, wt_lb: 0.16, sku: "84643", upc: "099575846434", url: "https://www.gearwrench.com/all-tools/ratchets-sockets/impact-products/84643-12-drive-12-f-x-38-m-impact-adapter", img_url: "https://www.gearwrench.com/sites/gearwrench/files/styles/large/public/pim_images/GW_84643_IMG-MAIN.jpg" },
      sm:  { title: "⅜in Socket Adapter for ½Dr Impact Sm", drive_kind: "intsq", driver_handle: "isq_0500in", target_handle: "isq_0375in", driver_end_diam: 21.8, target_end_diam: 14.7, ln_overall: 38.1, wt: 0.072575, wt_lb: 0.16, sku: "84643", upc: "099575846434", url: "https://www.gearwrench.com/all-tools/ratchets-sockets/impact-products/84643-12-drive-12-f-x-38-m-impact-adapter", img_url: "https://www.gearwrench.com/sites/gearwrench/files/styles/large/public/pim_images/GW_84643_IMG-MAIN.jpg" },
    },
  },
  // { title: "3/8in Socket Adapter US 1/2Dr Impact", socket_kind: "socket_adapter", drive_kind: "intsq", unit_system: "us", sqdrive_size: "isq_0500in", reach_kind: "other", socket_variant: "impact", sizing: "3/8in", sizing_mm: 9.525 * mm, sizing_mm_text: "9.5", sizing_in: 0.375 * inch, ln_overall: 38.1 * mm, wx_overall: 25.4 * mm, wy_overall: 25.4 * mm, ratchet_end_diam: 25.4 * mm, male_drive_size: "isq_0375in", female_drive_size: "isq_0500in", targets: {}, wt: 0.072575, wt_lb: 0.16, sku: "84643", upc: "099575846434", url: "https://www.gearwrench.com/all-tools/ratchets-sockets/impact-products/84643-12-drive-12-f-x-38-m-impact-adapter", img_url: "https://www.gearwrench.com/sites/gearwrench/files/styles/large/public/pim_images/GW_84643_IMG-MAIN.jpg" },

}