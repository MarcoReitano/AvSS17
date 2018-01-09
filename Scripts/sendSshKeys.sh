ssh-keygen -t rsa -N ''
for server in $(cat ./hosts) ; do
ssh-copy-id dmorady@$server
done 
