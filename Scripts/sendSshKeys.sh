#!/bin/bash

if [ ! -f $HOME/.ssh/id_rsa ]; then
    ssh-keygen -t rsa -N ''
fi

for server in $(cat ./hosts) ; do
    ssh-copy-id $USER@$server
done
