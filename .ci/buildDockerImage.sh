#!/bin/bash
set -e
set -u
set -x

docker build --file ./Messages/Dockerfile --tag ghcr.io/nmshd/bkb-messages:${TAG-temp} .
