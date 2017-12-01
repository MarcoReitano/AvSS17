#!/bin/bash

memory="32768" # 32Gb
cpu_count="10"
disk_size="20000"
#docker_machine="/usr/local/bin/docker-machine"
docker_machine="/Applications/Docker.app/Contents/Resources/bin/docker-machine"
PATH=$PATH:"/Applications/Docker.app/Contents/Resources/bin/docker-machine"

for ip in $(cat ./hosts); do
	i=$(echo $ip | cut -d'.' -f4 )
	echo "Current ip $ip"
    ./create-docker-machine.sh $ip
done
