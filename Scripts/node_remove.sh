#!/bin/bash

strNodeName=""

read strNodeName
if ["$strNodeName" = ""] 
then
	echo "Please enter a valid node name"
else
	docker node rm -f $strNodeName
	echo "You have successfully removed $strNodeName"
fi




