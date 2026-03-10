#!/bin/bash
MAINDIR=$(realpath $(dirname $0)/..)
shopt -s nullglob
chmod -v a+x $MAINDIR/{scripts,scripts/*}/*-*.{sh,ts,js,py,rb}
shopt -u nullglob
