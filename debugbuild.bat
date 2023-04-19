@setlocal enabledelayedexpansion
@set MSBUILD=
@for /D %%M in ("%ProgramFiles%\Microsoft Visual Studio\2022"\*) do @(
    @if exist "%%M\MSBuild\Current\Bin\MSBuild.exe" (
        @set "MSBUILD=%%M\MSBuild\Current\Bin\MSBuild.exe"
    )
)
@if "%MSBUILD%" == "" (
    @echo Could not find VS2022 MSBuild
    @exit /b 1
)
@nuget restore ILSpy.sln || (pause && exit /b 1)
"%MSBUILD%" ILSpy.sln /p:Configuration=Debug "/p:Platform=Any CPU" || (pause && exit /b 1)
