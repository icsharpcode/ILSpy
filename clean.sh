#!/bin/sh
pause(){
  read -n 1 -p "Press any key to continue..."
}
xbuild /m ILSpy.sln /t:clean "/p:Platform=Any CPU" /p:Configuration=Debug
[ "$?" != "0" ] && pause && xbuild /m ILSpy.sln /t:clean "/p:Platform=Any CPU" /p:Configuration=Release
[ "$?" != "0" ] && pause && exit 1
