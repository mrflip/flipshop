#! /usr/bin/env bash

SCRIPTDIR="$(realpath $(dirname $0))"
MAINDIR="$(realpath $(dirname $SCRIPTDIR))"

mkdir -p $MAINDIR/tmp
cd $MAINDIR/tmp

wget -E -r -l500 --no-clobber --no-parent https://www.gearwrench.com/all-tools/ratchets-sockets/chrome-sockets
wget -E -r -l500 --no-clobber --no-parent https://www.gearwrench.com/all-tools/ratchets-sockets/impact-products
wget -E -r -l500 --no-clobber --no-parent https://www.gearwrench.com/all-tools/wrenches/ratcheting-wrenches
wget -E -r -l500 --no-clobber --no-parent https://www.gearwrench.com/all-tools/wrenches/combination-wrenches
