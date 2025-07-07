# ILSpyCmd in Docker

Inspired by https://trustedsec.com/blog/hunting-deserialization-vulnerabilities-with-claude (and thus https://github.com/berdav/ilspycmd-docker)

## Building the Image

There are two dockerfiles available - one for interactive use of ilspycmd (exploration), and the other for driving it from the outside (automation)

### Interactive 

`docker build -t ilspycmd:91interactive . -f Dockerfile`

`docker run --rm -it -v ./:/docker ilspycmd:91interactive`

### Automation

`docker build -t ilspycmd:91forautomation . -f DockerfileForAutomation`

`docker run --rm -v ./:/infolder -v ./out:/outfolder ilspycmd:91forautomation "/home/ilspy/.dotnet/tools/ilspycmd -p -o /outfolder /infolder/sample.dll"`

