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

# check if inside a mac Darwin is Linux for mac
# Only This machine
if  $(uname | grep 'darwin' -i -q) ; then
    which docker
    if [[ $? != 0 ]]; then
        which brew
        if [[ $? != 0 ]]; then
            /usr/bin/ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)"
        fi
        cmd 'brew cask install docker'
        which docker
        if [[ $? != 0 ]]; then
            echo "docker coudn't be installed"
            exit -1
        fi
    fi
    cmd "open /Applications/Docker.app"
fi




# All other Machines
for ip in $(cat ./hosts) ; do
    ssh -t "$USER@$ip" "ls /Applications/Docker.app"
    if [[ $? != 0 ]]; then
        ssh -t "$USER@$ip" "ls /usr/local/bin/brew"
        if [[ $? != 0 ]]; then
            ssh -t "$USER@$ip" '/usr/bin/ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)"'
        fi
        ssh -t "$USER@$ip" 'brew cask install docker'

        ssh -t "$USER@$ip" "ls /Applications/Docker.app"
        if [[ $? != 0 ]]; then
            echo "docker coudn't be installed on machine $ip"
        fi
    fi
    cmd "open /Applications/Docker.app"
done


cmd 'docker-compose -f ./Build/Docker/docker-compose.yml build'
echo "+-----------------------------------+"
echo "| Creatring a Swarm  as the Manager |"
echo "+-----------------------------------+"
docker swarm leave --force
cmd "docker swarm init --advertise-addr=$1"
cmd 'docker swarm join-token worker  | grep docker  | sed s/\ *// | tee tokenfile.sh'
# cmd 'sudo chmod +x ./tokenfile.sh'


# Using Hosts file for the list of Ip / Hostnames
test -f ./hosts
if [[ $? != 0 ]]; then
    echo "no host file in ./host found please create one"
fi
# run the token Command for each  Host in the hosts File
for ip in $(cat ./hosts) ; do
    ssh -t "$USER@$ip" "open /Applications/Docker.app"
done


echo "+-----------------------------------------------------------------+"
echo "|Press any Key when on every Worker the docker Service is Running |"
echo "|                     on Every Node                               |"
echo "+-----------------------------------------------------------------+"
read -n1 -s tmp

tokenCommand=$(docker swarm join-token worker  | grep docker  | sed s/\ *//)
for ip in $(cat ./hosts) ; do
    ssh "$USER@$ip"  "$tokenCommand"
done
cmd 'docker node ls' # print all Connected Nodes
echo "+---------------------------------------------+"
echo "| Press any Key To Deploy the Stack           |"
echo "| Press after Every worker Joined the Swarm   |"
echo "+---------------------------------------------+"
read -n1 -s tmp
cmd 'docker stack deploy -c ./Build/Docker/docker-compose-swarm.yml avsStack'
