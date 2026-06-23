# For local development of the VSIX package - build and publish (VS2022 also needs arm64)

$configuration = "Debug"

dotnet build ./ILSpy.sln -c $configuration

$output_x64 = "./ILSpy/bin/$configuration/net10.0/win-x64/publish/fwdependent"

dotnet publish ./ILSpy/ILSpy.csproj -c $configuration --no-restore --no-self-contained -r win-x64 -o $output_x64
dotnet publish ./ILSpy.ReadyToRun/ILSpy.ReadyToRun.csproj -c $configuration --no-restore --no-self-contained -r win-x64 -o $output_x64

$output_arm64 = "./ILSpy/bin/$configuration/net10.0/win-arm64/publish/fwdependent"

dotnet publish ./ILSpy/ILSpy.csproj -c $configuration --no-restore --no-self-contained -r win-arm64 -o $output_arm64
dotnet publish ./ILSpy.ReadyToRun/ILSpy.ReadyToRun.csproj -c $configuration --no-restore --no-self-contained -r win-arm64 -o $output_arm64