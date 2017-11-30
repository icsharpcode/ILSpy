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
"%MSBUILD%" /m ILSpy.sln /t:Clean /p:Configuration=Debug "/p:Platform=Any CPU" || pause
"%MSBUILD%" /m ILSpy.sln /t:Clean /p:Configuration=Release "/p:Platform=Any CPU" || pause
