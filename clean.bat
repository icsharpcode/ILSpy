@setlocal enabledelayedexpansion
@set MSBUILD=
@for /D %%M in ("%ProgramFiles%\Microsoft Visual Studio\2022"\*) do (
    @if exist "%%M\MSBuild\Current\Bin\MSBuild.exe" (
        @set "MSBUILD=%%M\MSBuild\Current\Bin\MSBuild.exe"
    )
)
@if "%MSBUILD%" == "" (
    @echo Could not find VS2022 MSBuild
    @exit /b 1
)
"%MSBUILD%" /m ILSpy.sln /t:Clean /p:Configuration=Debug "/p:Platform=Any CPU" || pause
"%MSBUILD%" /m ILSpy.sln /t:Clean /p:Configuration=Release "/p:Platform=Any CPU" || pause
