@echo This script simulates what the build server is doing
@rem  /p:AdditionalBuildProperties="/v:d /p:MSBuildTargetsVerbose=true"
"%ProgramFiles(x86)%\MSBuild\14.0\Bin\msbuild.exe" Automated.proj /p:ArtefactsOutputDir="%CD%\build" /p:TestReportsDir="%CD%\build"
@IF %ERRORLEVEL% NEQ 0 GOTO err
@exit /B 0
:err
@PAUSE
@exit /B 1