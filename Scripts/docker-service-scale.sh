#!/bin/bash

docker-machine ssh default "docker service scale unityTest_avsbuild=$1" 
 
