for host in $(cat ./hosts); do
	ssh  -t $USER@$host "echo 'export PATH=$PATH:/usr/local/bin/' > .bashrc"
done
