#! /usr/bin/env yarn node
import _                                    /**/ from 'lodash';
import { load as cheerioLoad }                   from 'cheerio';
import { readdirSync }                           from 'fs';
import { join }                                  from 'path';
import * as Filer                                from '@freeword/meta/Filer.js';

// ─── Types ────────────────────────────────────────────────────────────────────

interface SocketWrenchProduct {
  sku:            string;
  title:          string;
  url:            string;
  imageUrl:       string;
  specifications: Record<string, string>;
}

// ─── Parsing ──────────────────────────────────────────────────────────────────

async function parseProductPage(filepath: string): Promise<SocketWrenchProduct | null> {
  const result = await Filer.loadtext(filepath);
  if (!result.ok) {
    console.error(`Failed to load ${filepath}: ${result.gist}`);
    return null;
  }

  const $ = cheerioLoad(result.val);

  // Title: prefer og:title, fall back to <title>
  const ogTitle  = $('meta[property="og:title"]').attr('content') ?? '';
  const title    = ogTitle.replace(/\s*-\s*Gearwrench\s*$/i, '').trim()
                || $('title').text().replace(/\s*-\s*Gearwrench\s*$/i, '').trim();

  // URL from canonical or og:url
  const url      = $('link[rel="canonical"]').attr('href')
                ?? $('meta[property="og:url"]').attr('content')
                ?? '';

  // Main image from og:image or image_src link
  const imageUrl = $('meta[property="og:image"]').attr('content')
                ?? $('link[rel="image_src"]').attr('href')
                ?? '';

  // SKU from JSON-LD Product schema
  let sku = '';
  $('script[type="application/ld+json"]').each((_, el) => {
    try {
      const data = JSON.parse($(el).html() ?? '{}');
      const graph: any[] = data['@graph'] ?? [data];
      for (const node of graph) {
        if (node['@type'] === 'Product' && node.sku) {
          sku = String(node.sku);
          break;
        }
      }
    } catch { /* malformed JSON-LD, skip */ }
  });

  // Specifications from <li id="specifications">
  const specifications: Record<string, string> = {};
  $('#specifications li.field__item').each((_, el) => {
    const spans = $(el).find('span');
    const label = spans.eq(0).text().replace(/\s*:\s*$/, '').trim();
    const value = spans.eq(1).text().replace(/\s+/g, ' ').trim();
    if (label) specifications[label] = value;
  });

  return { sku, title, url, imageUrl, specifications };
}

// ─── Main ─────────────────────────────────────────────────────────────────────

// const fixtureDir = Filer.__relname(import.meta.url, '..', '..', 'tests', 'fixtures', 'socket_wrenches_raw');
const ripdDir    = Filer.__relname(import.meta.url, '..', '..', 'ripd', 'www.gearwrench.com', 'all-tools', 'ratchets-sockets', 'chrome-sockets');

const files = readdirSync(ripdDir)
  .filter(f => f.endsWith('.html'))
  .map(f => join(ripdDir, f));

const products = await Promise.all(files.map(parseProductPage));
const valid    = products.filter(Boolean) as SocketWrenchProduct[];

console.log(JSON.stringify(valid, null, 2));
console.error(`\nParsed ${valid.length} / ${files.length} products.`);
