FROM mcr.microsoft.com/dotnet/sdk:8.0

RUN useradd -m -s /bin/bash ilspy
USER ilspy

WORKDIR /home/ilspy

RUN dotnet tool install -g ilspycmd --version 9.1.0.7988

RUN echo 'export PATH="$PATH:/home/ilspy/.dotnet/tools/"' >> /home/ilspy/.bashrc

ENTRYPOINT [ "/bin/bash" ]