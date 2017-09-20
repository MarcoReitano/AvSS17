From ubuntu:latest
USER root

ENV UNITY_VERSION 5.4.1f1+20160913_amd64 
ENV DEBIAN_FRONTEND=noninteractive

ADD ./AVSUnity /app/AVSUnity
ADD ./UnityInstall /

#install dependecies for unity installation
RUN apt-get update -qq
RUN apt-get install -qq -y \
gconf-service \
lib32gcc1 \
lib32stdc++6 \
libasound2 \
libc6-i386 \
libcairo2 \
libcups2 \
libfontconfig1 \
libfreetype6 \
libgdk-pixbuf2.0-0 \
libgl1-mesa-glx \
libglu1-mesa \
libgtk2.0-0 \
libnspr4 \
libnss3 \
libpango1.0-0 \
libxcomposite1 \
libxcursor1 \
libxi6 \
libxrandr2 \
libxtst6 \
libpq5 \
lsb-release \
xdg-utils \
npm 

RUN dpkg -i unity-editor-$UNITY_VERSION.deb
RUN rm unity-editor-$UNITY_VERSION.deb