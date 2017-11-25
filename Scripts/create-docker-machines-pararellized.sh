#!/bin/bash
cat ./hosts | xargs -P10  -I{} ./create-docker-machine.sh {}

# Give the machines some time to Boot Up
sleep 1
# ping every Host to check if its Running

for ip in $(cat ./hosts) ; do
    dockerIp=$(echo $(( $(echo $ip | cut -d'.' -f4 ) + 100)))
    ping -t 2 $dockerIp

done

