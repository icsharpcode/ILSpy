dotnet clean ILSpy.sln /p:Configuration=Debug "/p:Platform=Any CPU" %* || pause
dotnet clean ILSpy.sln /p:Configuration=Release "/p:Platform=Any CPU" %* || pause