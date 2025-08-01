dotnet build ILSpy.sln /p:Configuration=Debug "/p:Platform=Any CPU" %* || (pause && exit /b 1)
