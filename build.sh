#!/bin/bash
pause(){
  read -n 1 -p "$1"
}
[ "$1" != "Debug" ] && [ "$1" != "Release" ] && echo -e "Please specify build type.\n  Usage: ./build.sh [ Debug | Release ]" >&2 && exit 1
xbuild /m ILSpy.sln /p:Configuration=$1 "/p:Platform=Any CPU"
[ "$?" != "0" ] && exit 1
