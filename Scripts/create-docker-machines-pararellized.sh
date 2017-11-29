#!/bin/bash
cat ./hosts | xargs -P10  -I{} ./create-docker-machine.sh {}

# Give the machines some time to Boot Up
wait
sleep 5
say "Pinge Maschinen in 15 Sekunden an"
for i in {15..1}; do
	say "$i"
	sleep 1
done
clear
# ping every Host to check if its Running

./ping-docker-machines.sh
