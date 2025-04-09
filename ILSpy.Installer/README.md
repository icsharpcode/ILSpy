# Building the Installer

## Dependencies

See https://github.com/oleg-shilo/wixsharp/wiki#dependencies

```
dotnet tool install --global wix
```

GitHub runners installed software https://github.com/actions/runner-images/blob/main/images/windows/Windows2025-Readme.md at time 
of writing WiX Toolset 3.14.1.8722


## ILSpy Binaries

It is mandatory to first publish(.ps1) the respective target platforms, then setup can be built, eg

```
msbuild ILSpy.Installer.sln /p:Configuration="Release" /p:Platform="Any CPU"
msbuild ILSpy.Installer.sln /p:Configuration="Release" /p:Platform="Any CPU" /p:DefineConstants="ARM64"
```