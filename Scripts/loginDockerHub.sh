#!/bin/bash

for ip in $(cat ./hosts); do
    # -p and -u password and user
    echo current $ip
    ssh -t -q "$USER@$ip" "PATH=$PATH;eval \$(docker-machine env);docker-machine ssh default \"docker login -u avsss17 -p avsss17\""
done
