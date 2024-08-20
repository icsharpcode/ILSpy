@rem This file can be used to trigger the commit hook's formatting,
@rem modifying the local formatting even if not committing all changes.
pushd %~dp0\..
"%ProgramFiles%\Git\usr\bin\bash.exe" BuildTools\pre-commit --format
popd