/**
 * Summary of essential US (Inch Series) Fastener Standards.
 * Covers the triad of specification: Geometry (ASME), Mechanical/Materials (ASTM/SAE), and Threading (ASME).
 */
export const FastenerStandardizations = {
  // --- GEOMETRY & DIMENSIONS (ASME) ---
  'ASME B1.1': {
    org: 'ASME',
    spec: 'B1.1',
    covers: ['threads'],
    title: 'Unified Inch Screw Threads (UN and UNR Thread Form)',
    notes: 'Defines thread profiles, pitches (UNC/UNF), and fit classes (2A/2B).'
  },
  'ASME B18.2.1': {
    org: 'ASME',
    spec: 'B18.2.1',
    covers: ['geometry'],
    title: 'Square, Hex, Heavy Hex, and Askew Head Bolts and Hex, Heavy Hex, Hex Flange, Lobed Head, and Lag Screws',
    notes: 'The primary dimensional standard for hex-head industrial bolts.'
  },
  'ASME B18.2.2': {
    org: 'ASME',
    spec: 'B18.2.2',
    covers: ['geometry'],
    title: 'Nuts for General Applications: Machine Screw Nuts, Hex, Square, Hex Flange, and Coupling Nuts',
    notes: 'Companion to B18.2.1 for the female-threaded side.'
  },
  'ASME B18.3': {
    org: 'ASME',
    spec: 'B18.3',
    covers: ['geometry'],
    title: 'Socket Cap, Shoulder, Set Screws, and Hex Keys',
    notes: 'Dimensional specs for Allen-drive style fasteners.'
  },

  // --- MECHANICAL & MATERIAL PROPERTIES (ASTM) ---
  'ASTM A193': {
    org: 'ASTM',
    spec: 'A193',
    covers: ['mechanical properties', 'material'],
    title: 'Alloy-Steel and Stainless Steel Bolting for High Temperature or High Pressure Service',
    notes: 'Commonly references Grade B7 (Chromoly) or B8 (Stainless).'
  },
  'ASTM F3125': {
    org: 'ASTM',
    spec: 'F3125',
    covers: ['mechanical properties', 'structural'],
    title: 'Standard Specification for High Strength Structural Bolts, Steel and Alloy Steel, Heat Treated',
    notes: `
      Standard for heavy structural steel-to-steel connections.
      Consolidation and replacement of six ASTM standards: A325, A325M, A490, A490M, F1852 and F2280.
    `,
  },
  // ASTM F912: Alloy Steel socket-set screws (SSS)
  'ASTM F835': {
    org: 'ASTM',
    spec: 'F835',
    covers: ['mechanical properties'],
    title: 'Alloy Steel Socket Button and Countersunk Flat Head Cap Screws',
    urls: ['https://www.astm.org'],
    notes: 'High-strength requirements for socket-drive screws.'
  },
  'ASTM F593': {
    org: 'ASTM',
    spec: 'F593',
    covers: ['mechanical properties', 'material'],
    title: 'Stainless Steel Bolts, Hex Cap Screws, and Studs',
    notes: 'The go-to for 304 and 316 stainless mechanical requirements.'
  },

  // --- MECHANICAL & MATERIAL PROPERTIES (SAE) ---
  'SAE J429': {
    org: 'SAE',
    spec: 'J429',
    covers: ['mechanical properties'],
    title: 'Mechanical and Material Requirements for Externally Threaded Fasteners',
    notes: 'Defines the ubiquitous Grade 2, 5, and 8 bolt strengths.'
  }
} as const
export type FastenerStandardizationKey = keyof typeof FastenerStandardizations;