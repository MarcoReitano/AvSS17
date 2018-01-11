
myIp=$(ifconfig en1 inet | grep inet | cut -d' ' -f2) 

for host in $(cat ./hosts); do
	echo "$host"
    if [ $host != $myIp ]; then
        ssh -q  -t $USER@$host "sudo /sbin/shutdown -r now"
    fi
done
sudo /sbin/shutdown -r now
