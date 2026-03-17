#!/bin/bash
SCRIPTDIR=$(realpath "$(dirname "$0")")
MAINDIR=$(realpath "$SCRIPTDIR/../..")

cd $MAINDIR
mkdir -p ripd/fastener/{standards,specifications}
FASTENER_SITE=${FASTENER_SITE:-$1}
if [ -z "$FASTENER_SITE" ]; then echo "Usage: $0 <fastener-site>"; exit 1; fi

RIPD_DIR=$MAINDIR/tmp/${FASTENER_SITE}.com
FASTENER_DIR=$MAINDIR/ripd/fastener
cd $RIPD_DIR

mv  standards/bs-4933/90-deg-countersunk-sqa*-bolts*/index.html  standards/bs-4933/90-deg-countersunk-square-bolts.html   || true
mv standards/bs-4933/120-deg-countersunk-sqa*-bolts*/index.html  standards/bs-4933/120-deg-countersunk-square-bolts.html || true

shopt -s nullglob
for subdir in {standards,specifications}; do
  cd $RIPD_DIR/$subdir
  cp {asme,as,bs,csn,din,eu,ifi,is,IS-,jis,pn,uni}*[0-9]*.html $FASTENER_DIR/$subdir/
  for foo in `find * -type f -iname '*.html'` ; do
    if ! echo $foo | egrep -i -q '(asme|as|bs|csn|din|en|eu|ifi|is-|IS-|iso|jis|pn|sae|uni).*[0-9]' ; then continue; fi
    cp "$foo" "$FASTENER_DIR/$subdir/$(echo $foo | sd '/index.html' '.html' | sd '/' -- '--')"
  done
done
shopt -u nullglob

echo "Success!"