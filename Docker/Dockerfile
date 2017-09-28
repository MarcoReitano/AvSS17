#                   ATTENTION :
#
#                   Before Building the Image Download the deb file
#  +---------------------------------------------------------------------------------------+
#  |  http://beta.unity3d.com/download/d72e16ff4aba/unity-editor_amd64-5.6.3xf1Linux.deb   |
#  +---------------------------------------------------------------------------------------+
#
#
#    AND Make shure its in the UnityInstall Directory in the Root of the Project and not Rename the file
#
#               unity-editor_amd64-5.6.3xf1Linux.deb
#
From consol/ubuntu-xfce-vnc

USER root

# install Deps
RUN apt-get update -qq
RUN apt-get install -qq -y curl gconf-service lib32gcc1 lib32stdc++6 libasound2 libc6 libc6-i386 libcairo2 libcap2 libcups2 libdbus-1-3 libexpat1 libfontconfig1 libfreetype6 libgcc1 libgconf-2-4 libgdk-pixbuf2.0-0 libgl1-mesa-glx libglib2.0-0 libglu1-mesa libgtk2.0-0 libnspr4 libnss3 libpango1.0-0 libstdc++6 libx11-6 libxcomposite1 libxcursor1 libxdamage1 libxext6 libxfixes3 libxi6 libxrandr2 libxrender1 libxtst6 zlib1g debconf npm xdg-utils lsb-release libpq5 xvfb git

# Create Dirs for Unity
RUN mkdir -p $HOME/.cache/unity3d && mkdir -p $HOME/.local/share/unity3d

ADD ./AVSUnity /app/AVSUnity
ADD ./UnityInstall/unity-editor_amd64-5.6.3xf1Linux.deb .

#install the Downloaded Editor
RUN dpkg -i unity-editor_amd64-5.6.3xf1Linux.deb

# Remove the Install file
RUN rm unity-editor_amd64-5.6.3xf1Linux.deb


# Unity Chrome Sandbox Fixes
RUN chown root /opt/Unity/Editor/chrome-sandbox
RUN chmod 4755 /opt/Unity/Editor/chrome-sandbox

# Clone the Project from github
RUN cd $HOME
#RUN git clone https://github.com/MarcoReitano/AvSS17.git
