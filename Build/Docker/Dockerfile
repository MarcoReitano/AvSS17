From ubuntu:latest

USER root

ADD ./Standard_Data /app/Standard_Data
ADD ./Standard.x86_64 /app

RUN chmod +x ./app/Standard.x86_64

ENTRYPOINT ["/app/Standard.x86_64"]