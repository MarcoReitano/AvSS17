From ubuntu:latest
USER root
ENV UNITY_VERSION 5.4.1f1+20160913_amd64
ADD ./AVSUnity /app/AVSUnity
ADD ./UnityInstall /
RUN dpkg -i unity-editor-$UNITY_VERSION.deb