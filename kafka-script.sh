#!/bin/bash

if [ $# -lt 1 ];
then
  echo "USAGE:"
  exit 1
fi
if command -v podman &> /dev/null
then
  podman run -it --rm --network hotel-booking-backend_app-tier bitnami/kafka:latest $@
  exit 0
elif command -v docker &> /dev/null
then
  docker run -it --rm --network hotel-booking-backend_app-tier bitnami/kafka:latest $@
  exit 0
fi