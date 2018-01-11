#!/bin/bash
if [[ $# < 1 ]]; then
    echo 'Ip address is missing please give the IP address for your Target machine'
    echo "$0 [IP Address]"
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
