#!/bin/bash

SCRIPTDIR=$(realpath "$(dirname "$0")")
ROOTDIR=$(realpath "$SCRIPTDIR/..")

cd "$ROOTDIR"

shopt -s nullglob
rm -rf .*tsbuildinfo* built */built
shopt -u nullglob