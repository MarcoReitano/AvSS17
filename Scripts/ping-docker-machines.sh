for ip in $(cat ./hosts) ; do
    dockerIp=$(echo $(( $(echo $ip | cut -d'.' -f4 ) + 100)))
    ping -q -t 1 10.0.0.$dockerIp   >/dev/null
	if [[ $? != 0 ]]; then
	#	say "keine antwort von $dockerIp" 
		tput setaf 1 # print in red
		echo -e "$dockerIp keine antwort"
		tput sgr0 # reset color
		ssh -t  -q "$USER@$ip" "PATH=$PATH;docker-machine ls | grep -i stop " > /dev/null
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
done
