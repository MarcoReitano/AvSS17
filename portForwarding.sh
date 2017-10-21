for port in $(cat ./ports); do
 VBoxManage modifyvm "default" --natpf1 "tcp-port$port,tcp,,$port,,$port";
 VBoxManage modifyvm "default" --natpf1 "udp-port$port,udp,,$port,,$port";
done
