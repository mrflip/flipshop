#!/bin/bash
SCRIPTDIR=$(realpath "$(dirname "$0")")
MAINDIR=$(realpath "$SCRIPTDIR/../..")

mkdir -p $MAINDIR/tmp ;
cd       $MAINDIR/tmp
FASTENER_SITE=${FASTENER_SITE:-$1}
if [ -z "$FASTENER_SITE" ]; then echo "Usage: $0 <fastener-site>"; exit 1; fi

LOGFILE=./log-${FASTENER_SITE}.com-`date +%Y%m%d`-a.log
echo "  Downloading ${FASTENER_SITE}.com standards and specifications -- background this and run"
echo "tail -f $LOGFILE"
echo "  to follow progress"

shopt -s nullglob
rm ${FASTENER_SITE}.com/{standards,specifications}/index* || true
wget -E -r -l18 -nc -np -nv -a $LOGFILE  https://${FASTENER_SITE}.com/standards/
wget -E -r -l18 -nc -np -nv -a $LOGFILE  https://${FASTENER_SITE}.com/specifications/
shopt -u nullglob
echo
echo "Success!"