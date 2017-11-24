for host in $(cat ./hosts); do
	echo "$host"
	ssh -q  -t $USER@$host "$1"
done
