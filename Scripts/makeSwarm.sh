#!/bin/bash
docker-machine ssh default "docker swarm leave -f"
docker-machine ssh default "docker swarm init --advertise-addr 10.0.0.109"
command=$(docker-machine ssh default "docker swarm join-token worker |grep docker")


myIp=$(ifconfig en1 inet | cut -d' ' -f2 | grep '10.0')
echo Myip is $myIp
for ip in $(cat hosts);do
 if [ $ip != $myIp ]; then
	 echo $ip
	 ssh -t -q "$USER@$ip" "PATH=$PATH;eval \$(docker-machine env);docker-machine ssh default \"docker swarm leave -f\""
	 ssh -t -q "$USER@$ip" "PATH=$PATH;eval \$(docker-machine env);docker-machine ssh default \"$command\""
 fi
done
