@if not exist "AvalonEdit\ICSharpCode.AvalonEdit.sln" (
	git submodule update --init || exit /b 1
)
"%ProgramFiles(x86)%\MSBuild\14.0\Bin\msbuild.exe" /m ILSpy.sln /p:Configuration=Debug "/p:Platform=Any CPU"
@IF %ERRORLEVEL% NEQ 0 GOTO err
@exit /B 0
:err
@PAUSE
@exit /B 1