# Building the Installer

It is mandatory to first publish(.ps1) the respective target platforms, then setup can be built, eg

```
msbuild ILSpy.Installer.sln /p:Configuration="Release" /p:Platform="Any CPU"
msbuild ILSpy.Installer.sln /p:Configuration="Release" /p:Platform="Any CPU" /p:DefineConstants="ARM64"
```