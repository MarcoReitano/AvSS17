#!/usr/bin/env bash

function cmd() {
bash -c "$1"
    if [[ $? != 0 ]]; then
        echo "failed to run: $1"
        exit 0
    fi
}

if [[ $# < 1 ]]; then
    echo 'Ip address is missing please give the IP address as the argument'
    echo './autodeploy.sh [IP Address]'
    exit 0
fi


test -d "./.git" # check if runned in git root
if [[ $? != 0 ]]; then
    echo "Please run this Script from the Git Root Directory"
    exit 0
fi


if  $(uname | grep 'darwin' -i -q) ; then
    which brew
    if [[ $? != 0 ]]; then
        /usr/bin/ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)"
        cmd 'brew install docker'
        wich docker
        if [[ $? != 0 ]]; then
            echo "docker coudn't be installed"
            exit -1
        fi
        cmd "open /Applications/Docker.app"
    fi
fi


cmd 'docker-compose -f ./Build/Docker/docker-compose.yml build'
echo "+-----------------------------------+"
echo "| Creatring a Swarm  as the Manager |"
echo "+-----------------------------------+"
docker swarm leave --force
cmd "docker swarm init --advertise-addr=$1"
cmd 'docker swarm join-token worker  | grep docker  | sed s/\ *// | tee tokenfile.sh'
cmd 'sudo chmod +x ./tokenfile.sh'

echo "+---------------------------------------------+"
echo "| Press any Key To Deploy the Stack           |"
echo "| Press after Every worker Joined the Swarm   |"
echo "+---------------------------------------------+"
read -n1 -s tmp
cmd 'docker stack deploy -c ./Build/Docker/docker-compose-swarm.yml avsStack'


