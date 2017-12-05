eval $(docker-machine env)
docker login -u avsss17 -p avsss17 && docker stack deploy -c ../Build/Docker/docker-compose-swarm.yml --with-registry-auth unityTest  
