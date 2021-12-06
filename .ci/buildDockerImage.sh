#!/bin/bash
set -e
set -u
set -x

docker build --file ./Messages.API/Dockerfile --tag ghcr.io/nmshd/bkb-messages:${TAG-temp} .
