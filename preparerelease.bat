@setlocal enabledelayedexpansion
@set MSBUILD=
@for /D %%M in ("%ProgramFiles(x86)%\Microsoft Visual Studio\2017"\*) do (
    @if exist "%%M\MSBuild\15.0\Bin\MSBuild.exe" (
        @set "MSBUILD=%%M\MSBuild\15.0\Bin\MSBuild.exe"
    )
)
@if "%MSBUILD%" == "" (
    @echo Could not find VS2017 MSBuild
    @exit /b 1
)
@del ICSharpCode.Decompiler\bin\Release\*.nupkg
"%MSBUILD%" ILSpy.sln /p:Configuration=Release "/p:Platform=Any CPU"
@IF %ERRORLEVEL% NEQ 0 (
    @pause
    @exit /b 1
)
@if not exist "%ProgramFiles%\7-zip\7z.exe" (
	@echo Could not find 7zip
	@exit /b 1
)
@del artifacts.zip
@rmdir /Q /S artifacts
@mkdir artifacts
"%ProgramFiles%\7-zip\7z.exe" a artifacts\ILSpy_binaries.zip %cd%\ILSpy\bin\Release\net46\*.dll %cd%\ILSpy\bin\Release\net46\*.exe %cd%\ILSpy\bin\Release\net46\*.config
@copy ILSpy.AddIn\bin\Release\net46\ILSpy.AddIn.vsix artifacts\
@copy ICSharpCode.Decompiler\bin\Release\*.nupkg artifacts\
"%ProgramFiles%\7-zip\7z.exe" a artifacts.zip %cd%\artifacts\*
@exit /b 0
