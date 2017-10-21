#!/usr/bin/env bash
docker='/usr/local/bin/docker'
dockermachine='/usr/local/bin/docker-machine'
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
        cmd 'sudo -u admin brew cask install docker'
        which docker
        if [[ $? != 0 ]]; then
            echo "docker coudn't be installed"
            exit -1
        fi
    fi
    cmd "open /Applications/Docker.app"
fi


function installDocker() {
# Install Stuff
# All other Machines
for ip in $(cat ./hosts) ; do
echo "ls1 $ip"
    ssh -t "$USER@$ip" "ls /Applications/Docker.app"
#    if [[ $? != 0 ]]; then
#        ssh -t "$USER@$ip" "ls /usr/local/bin/brew"
#        if [[ $? != 0 ]]; then
#           # ssh -t "$USER@$ip" '/usr/bin/ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)"'
#        fi
        #ssh -t "$USER@$ip" "sudo chown -R $(whoami) /usr/local/var/homebrew"
        #ssh -t "$USER@$ip" '/usr/local/bin/brew cask install docker'

        ssh -t "$USER@$ip" "ls /Applications/Docker.app/"
        if [[ $? != 0 ]]; then
            echo "docker coudn't be installed on machine $ip"
	    continue
        fi
##    fi
#    ssh -t "$USER@$ip" "open /Applications/Docker.app"
done
}

# cmd 'docker-compose -f ./Build/Docker/docker-compose.yml build' # later
echo "+-----------------------------------+"
echo "| Creatring a Swarm  as the Manager |"
echo "+-----------------------------------+"
cmd "$dockermachine ssh default 'docker swarm leave --force'"
cmd "$dockermachine ssh default 'docker swarm init --advertise-addr=$(docker-machine ip)'"
swarmToken=$(docker-machine ssh default 'docker swarm join-token worker -q')


# Using Hosts file for the list of Ip / Hostnames
test -f ./hosts
if [[ $? != 0 ]]; then
    echo "no host file in ./host found please create one"
fi

#create docker-machines and start them
for ip in $(cat ./hosts) ; do
    echo "Working on $ip"
    ssh -q -t "$USER@$ip" "ls /Applications/VirtualBox.app/ >/dev/null"
    if [[ $? != 0 ]]; then
	ssh -q -t "$USER@$ip" '/usr/local/bin/brew cask install virtualbox'
    fi
    vm=$(ssh -q -t "$USER@$ip" "$dockermachine ls") 
    echo $vm | grep default -i -q
    if [[ $? != 0 ]]; then
        ssh -q -t "$USER@$ip" "$dockermachine create default"
    fi
    echo $vm | grep default -i | grep running -i -q
    if [[ $? != 0 ]]; then
	ssh -q -t "$USER@$ip" "$dockermachine start default"
    fi
    ssh -q -t "$USER@$ip" "$dockermachine ssh default 'docker swarm leave --force'"
    ssh -q -t "$USER@$ip" "$dockermachine ssh default 'docker swarm join --token $swarmToken $1:2377'"
done



docker-machine ssh default 'docker node ls'

numberOfNodes=$(docker-machine ssh default 'docker node ls' | sed 1d | wc -l )
echo "| You Have $numberOfNodes Nodes in your Swarm " 
echo "+---------------------------------------------+"
echo "| Press any Key To Deploy the Stack           |"
echo "| Press after Every worker Joined the Swarm   |"
echo "+---------------------------------------------+"
read -n1 -s tmp
cmd 'docker-machine ssh default "docker stack deploy -c ./Build/Docker/docker-compose-swarm.yml avsStack"'
