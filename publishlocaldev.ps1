# For local development of the VSIX package - build and publish (VS2022 also needs arm64)

$output_x64 = "./ILSpy/bin/Debug/net8.0-windows/win-x64/publish/fwdependent"

dotnet publish ./ILSpy/ILSpy.csproj -c Debug --no-restore --no-self-contained -r win-x64 -o $output_x64
dotnet publish ./ILSpy.ReadyToRun/ILSpy.ReadyToRun.csproj -c Debug --no-restore --no-self-contained -r win-x64 -o $output_x64
dotnet publish ./ILSpy.BamlDecompiler/ILSpy.BamlDecompiler.csproj -c Debug --no-restore --no-self-contained -r win-x64 -o $output_x64

$output_arm64 = "./ILSpy/bin/Debug/net8.0-windows/win-arm64/publish/fwdependent"

dotnet publish ./ILSpy/ILSpy.csproj -c Debug --no-restore --no-self-contained -r win-arm64 -o $output_arm64
dotnet publish ./ILSpy.ReadyToRun/ILSpy.ReadyToRun.csproj -c Debug --no-restore --no-self-contained -r win-arm64 -o $output_arm64
dotnet publish ./ILSpy.BamlDecompiler/ILSpy.BamlDecompiler.csproj -c Debug --no-restore --no-self-contained -r win-arm64 -o $output_arm64