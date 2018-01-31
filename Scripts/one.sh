#!/usr/bin/env bash
if [[ $# < 1 ]]; then
    cat << "EOF"
Usage: one COMMAND

    Commands:
    ping        ping docker Machines
    login       Login to docker hub
    swarm       Remove old Swarm and make a new One
    deploy      Run the Application
    restart     Restart all Machines
    machine     Create docker machine on one Machine ip adress as parameter
    machines    Create docker machine on all Machines
    logout      logout from all Machines
    ssh         run command on all Machines
    scale       scale the Worker Service Container to number
    keys        copy ssh key on all Machines
EOF
fi

function sshall()
{
    for host in $(cat ./hosts); do
        echo "$host"
        ssh -q  -t $USER@$host "PATH=$PATH;$@"
    done
}

function pingMachines()
{
    for ip in $(cat ./hosts) ; do
        dockerIp=$(echo $(( $(echo $ip | cut -d'.' -f4 ) + 100)))
        ping -q -t 1 10.0.0.$dockerIp   >/dev/null
        if [[ $? != 0 ]]; then
            #	say "keine antwort von $dockerIp"
            tput setaf 1 # print in red
            echo -e "$dockerIp keine antwort"
            tput sgr0 # reset color
            ssh -t  -q "$USER@$ip" "PATH=$PATH;docker-machine ls | grep -i 'stop\|paused' " > /dev/null
            if [[ $? == 0 ]]; then
                #		say "$dockerIp läuft docker-machine nicht"
                tput setaf 1 # print in red
                echo "$dockerIp läuft docker-machine nicht"
                tput sgr0 # reset color
            fi
            tput sgr0 # reset color
        else
            echo "$dockerIp funktioniert"
        fi

        ssh -t  -q "$USER@$ip" "PATH=$PATH;docker info" > /dev/null
        if [[ $? != 0 ]]; then
            echo "$dockerip docker deamon not running"
        fi
    done
}

function login()
{
    for ip in $(cat ./hosts); do
        # -p and -u password and user
        echo current $ip loggin in
        ssh -t -q "$USER@$ip" "PATH=$PATH;eval \$(docker-machine env);docker-machine ssh default \"docker login -u avsss17 -p avsss17\""
    done
}

function makeSwarm()
{
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
}

function run()
{
    eval $(docker-machine env)
    docker login -u avsss17 -p avsss17 && docker stack deploy -c ../Build/Docker/docker-compose-swarm.yml --with-registry-auth unityTest
}

function restart()
{
    myIp=$(ifconfig en1 inet | grep inet | cut -d' ' -f2)

    for host in $(cat ./hosts); do
        echo "$host"
        if [ $host != $myIp ]; then
            ssh -q  -t $USER@$host "sudo /sbin/shutdown -r now"
        fi
    done
    sudo /sbin/shutdown -r now
}

function createMachine()
{
    if [[ $# < 1 ]]; then
        echo 'Ip address is missing please give the IP address for your Target machine'
        echo "$0 machine [IP Address]"
        exit 0
    fi

    if [[ $1 == "-h" ]]; then
        echo 'Ip address is missing please give the IP address for your Target machine'
        echo "$0 [IP Address]"
        exit 0
    fi

    # Variablen für die Docker-machine
    memory="32768" # 32Gb
    cpu_count="10"
    disk_size="20000"
    #docker_machine="/usr/local/bin/docker-machine"
    docker_machine="/Applications/Docker.app/Contents/Resources/bin/docker-machine"
    PATH=$PATH:/Applications/Docker.app/Contents/Resources/bin/

    ip=$1
    i=$(echo $1 | cut -d'.' -f4)
    echo "Current ip $ip"
    ssh -t -q "$USER@$ip" "PATH=$PATH;docker-machine ls"
    ssh -t -q "$USER@$ip" "PATH=$PATH;VBoxManage --version"
    ssh -t -q "$USER@$ip" "PATH=$PATH;docker-machine rm -f default"
    ##	# Hier  Können die Einstellungen für die Docker-Machine Verändert werden
    ssh -t -q "$USER@$ip" "PATH=$PATH;docker-machine create -d virtualbox --virtualbox-memory $memory --virtualbox-cpu-count $cpu_count --virtualbox-disk-size $disk_size default "
    ssh -t -q "$USER@$ip" "PATH=$PATH;echo \"/etc/init.d/services/dhcp stop;ifconfig eth2 10.0.0.$((100+i)) netmask 255.255.255.0 broadcast 10.0.0.255 up\" | docker-machine ssh default sudo tee /var/lib/boot2docker/bootsync.sh > /dev/null"
    ssh -t -q "$USER@$ip" "PATH=$PATH;docker-machine ssh default sudo chmod 755 /var/lib/boot2docker/bootsync.sh"
    ssh -t -q "$USER@$ip" "PATH=$PATH;docker-machine stop default"
    ssh -t -q "$USER@$ip" "PATH=$PATH;VBoxManage modifyvm 'default' --nic3 bridged --bridgeadapter3 en1 --cableconnected3 on"
    #ssh -t -q "$USER@$ip" "PATH=$PATH;VBoxManage modifyvm 'default' --nic3 nat --cableconnected3 on"
    #ssh -t -q "$USER@$ip" "PATH=$PATH;docker-machine regenerate-certs default"
    echo $ip | xargs -n1 -P10 -I{}	ssh -t -q "$USER@$ip" "PATH=$PATH;docker-machine start default" &
    sleep 5
}

export -f createMachine

function recreateMachines()
{
    cat ./hosts | xargs -P10  -I{} bash -c "createMachine  {}"
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
    pingMachines
}

function logoutAll()
{
    maclogout="osascript -e 'tell application \"System Events\" to log out'"
    for ip in {1..10};do
        ssh $USER@10.0.0.$ip "$maclogout"
        echo $ip
    done
}

function keyshare()
{
    if [ ! -f $HOME/.ssh/id_rsa ]; then
        ssh-keygen -t rsa -N ''
    fi
    for server in $(cat ./hosts) ; do
        ssh-copy-id $USER@$server
        echo "$server"
    done
}

function scale()
{
    docker-machine ssh default "docker service scale unityTest_avsbuild=$1"
}


case $1 in
    "ping" )
        echo "ping docker Machines"
        pingMachines
        ;;
    "login" )
        echo "Login to docker hub"
        login
        ;;
    "swarm" )
        echo "Remove old Swarm and make a new One"
        makeSwarm;;
    "run" )
        echo "Run the Application"
        run;;
    "restart" )
        echo "Restart all Machines"
        restart;;
    "machine")
        echo "Create docker machine on one Machine"
        createMachine $2;;
    "machines")
        echo "Create docker machine on all Machines"
        recreateMachines
        echo "hello"
        ;;
    "logout" )
        echo "logout from all Machines"
        logoutAll;;
    "ssh" )
        echo "run command on all Machines"
        sshall "${@:2}"
        ;;
    "scale" )
        echo "scale docker container"
        scale "$2"
        ;;
    "keys" )
        echo "Send keys to all Macs"
        keyshare
        ;;
esac