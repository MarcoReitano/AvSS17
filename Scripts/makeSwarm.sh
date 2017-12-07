#!/bin/bash
docker-machine ssh default "docker swarm leave -f"
dockerIp=$(docker-machine ssh default "ifconfig eth2 |grep 'inet addr' | cut -d: -f2| cut -d' ' -f1")
echo Docker ip is $dockerIp
docker-machine ssh default "docker swarm init --advertise-addr $dockerIp"
command=$(docker-machine ssh default 'docker swarm join-token worker | grep docker')
PATH=$PATH:/Applications/Docker.app/Contents/Resources/bin/


myIp=$(ifconfig en1 inet | cut -d' ' -f2 | grep '10.0')
echo Myip is $myIp

for ip in $(cat hosts);do
 if [ $ip != $myIp ]; then
	 echo $ip
	 ssh -t -q "$USER@$ip" "PATH=$PATH; docker-machine ssh default \"docker swarm leave -f\"" >/dev/null
	 ssh -t -q "$USER@$ip" "PATH=$PATH; docker-machine ssh default \"$command\""
 fi
done
