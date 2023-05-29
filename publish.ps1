$output_arm64 = "./ILSpy/bin/Release/net6.0-windows/win-arm64/publish/nsc"
$output_x64 = "./ILSpy/bin/Release/net6.0-windows/win-x64/publish/nsc"

dotnet publish ./ILSpy/ILSpy.csproj -c Release --no-restore --no-self-contained -r win-arm64 -o $output_arm64
dotnet publish ./ILSpy.ReadyToRun/ILSpy.ReadyToRun.csproj -c Release --no-restore --no-self-contained -r win-arm64 -o $output_arm64
dotnet publish ./ILSpy.BamlDecompiler/ILSpy.BamlDecompiler.csproj -c Release --no-restore --no-self-contained -r win-arm64 -o $output_arm64

dotnet publish ./ILSpy/ILSpy.csproj -c Release --no-restore --no-self-contained -r win-x64 -o $output_x64
dotnet publish ./ILSpy.ReadyToRun/ILSpy.ReadyToRun.csproj -c Release --no-restore --no-self-contained -r win-x64 -o $output_x64
dotnet publish ./ILSpy.BamlDecompiler/ILSpy.BamlDecompiler.csproj -c Release --no-restore --no-self-contained -r win-x64 -o $output_x64

' msbuild ILSpy.Installer.sln /p:Configuration="Release" /p:Platform="Any CPU" /p:DefineConstants="ARM64"