# From consol/ubuntu-xfce-vnc
FROM eamonwoortman/unity3d
USER root
WORKDIR /app
# Run mkdir /app/AVSUnity
ADD ./AVSUnity /app/AVSUnity
# this is a requirement by chrome-sandbox
RUN chown root /opt/Unity/Editor/chrome-sandbox
RUN chmod 4755 /opt/Unity/Editor/chrome-sandbox
# make a .config dir for Unity
RUN mkdir /root/.config
# ENV UNITY_VERSION 5.4.1f1+20160913_amd64
# RUN apt-get update -qq
# RUN apt-get install -qq -y curl gconf-service lib32gcc1 lib32stdc++6 libasound2 libc6 libc6-i386 libcairo2 libcap2 libcups2 libdbus-1-3 libexpat1 libfontconfig1 libfreetype6 libgcc1 libgconf-2-4 libgdk-pixbuf2.0-0 libgl1-mesa-glx libglib2.0-0 libglu1-mesa libgtk2.0-0 libnspr4 libnss3 libpango1.0-0 libstdc++6 libx11-6 libxcomposite1 libxcursor1 libxdamage1 libxext6 libxfixes3 libxi6 libxrandr2 libxrender1 libxtst6 zlib1g debconf npm xdg-utils lsb-release libpq5 xvfb
# RUN mkdir -p /root/.cache/unity3d
# ADD http://download.unity3d.com/download_unity/linux/unity-editor-$UNITY_VERSION.deb .
# RUN dpkg -i unity-editor-$UNITY_VERSION.deb
# RUN rm      unity-editor-$UNITY_VERSION.deb

