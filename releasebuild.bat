dotnet build ILSpy.sln /p:Configuration=Release "/p:Platform=Any CPU" %* || (pause && exit /b 1)
