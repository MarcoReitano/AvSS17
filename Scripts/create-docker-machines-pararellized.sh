#!/usr/bin/bash
# cat ./hosts | xargs -P10  -I{} ./create-docker-machine.sh {}

# Give the machines some time to Boot Up
sleep 1
# ping every Host to check if its Running

for ip in $(cat ./hosts) ; do
    dockerIp=$(echo $ip | cut -d'.' -f4 | sed -e 's/.*/\0 +100/' | bc | sed -e 's/.*/10.0.0.\0/' )
    ping -w 2 $dockerIp
done

