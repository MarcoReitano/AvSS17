#!/bin/bash

for ip in $(cat ./hosts); do
	echo "Current ip: $ip"
	# Install Bew
	ssh -q -t "$USER@$ip" "PATH=$PATH;brew --version"
	if [[ $? != 0 ]]; then
		ssh -q -t "$USER@$ip" '/usr/bin/ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)"'
		ssh -q -t "$USER@$ip" "PATH=$PATH;brew --version"
		if [[ $? != 0 ]]; then
			echo "Homebrew coudn't be installed on machine $ip"
		fi
	fi
	ssh -q -t "$USER@$ip" "ls -al /usr/local/var/homebrew | grep \"$USER\" -q"
	if [[ $? != 0 ]]; then
		ssh -q -t "$USER@$ip" "sudo chown -R $(whoami) /usr/local/var/homebrew"
	fi

	# Install Docker
	ssh -q -t "$USER@$ip" "PATH=$PATH;docker --version"
    if [[ $? != 0 ]]; then
        ssh -q -t "$USER@$ip" '/usr/local/bin/brew cask install docker'
        ssh -q -t "$USER@$ip" "PATH=$PATH;docker --version"
		if [[ $? != 0 ]]; then # If Already Installed try to reinstall
			ssh -q -t "$USER@$ip" '/usr/local/bin/brew cask reinstall docker'
		fi
        ssh -q -t "$USER@$ip" "PATH=$PATH;docker --version"
        if [[ $? != 0 ]]; then
            echo "docker coudn't be installed on machine $ip"
        fi
	fi

	# Install VirtualBox
    ssh -q -t "$USER@$ip" "PATH=$PATH;VBoxManage --version"
    if [[ $? != 0 ]]; then
	ssh -q -t "$USER@$ip" '/usr/local/bin/brew cask install virtualbox'
    ssh -q -t "$USER@$ip" "PATH=$PATH;VBoxManage --version"
	if [[ $? != 0 ]]; then # If Already Installed try to reinstall
		ssh -q -t "$USER@$ip" '/usr/local/bin/brew cask reinstall virtualbox'
	fi

    ssh -q -t "$USER@$ip" "PATH=$PATH;VBoxManage --version"
        if [[ $? != 0 ]]; then
            echo "VirtualBox coudn't be installed on machine $ip"
        fi
    fi
done
