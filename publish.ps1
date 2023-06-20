$output_arm64 = "./ILSpy/bin/Release/net8.0-windows/win-arm64/publish/fwdependent"
$output_x64 = "./ILSpy/bin/Release/net8.0-windows/win-x64/publish/fwdependent"
$output_x64_selfcontained = "./ILSpy/bin/Release/net8.0-windows/win-x64/publish/selfcontained"

dotnet publish ./ILSpy/ILSpy.csproj -c Release --no-restore --no-self-contained -r win-arm64 -o $output_arm64
dotnet publish ./ILSpy.ReadyToRun/ILSpy.ReadyToRun.csproj -c Release --no-restore --no-self-contained -r win-arm64 -o $output_arm64
dotnet publish ./ILSpy.BamlDecompiler/ILSpy.BamlDecompiler.csproj -c Release --no-restore --no-self-contained -r win-arm64 -o $output_arm64

dotnet publish ./ILSpy/ILSpy.csproj -c Release --no-restore --no-self-contained -r win-x64 -o $output_x64
dotnet publish ./ILSpy.ReadyToRun/ILSpy.ReadyToRun.csproj -c Release --no-restore --no-self-contained -r win-x64 -o $output_x64
dotnet publish ./ILSpy.BamlDecompiler/ILSpy.BamlDecompiler.csproj -c Release --no-restore --no-self-contained -r win-x64 -o $output_x64

dotnet publish ./ILSpy/ILSpy.csproj -c Release --no-restore --self-contained -r win-x64 -o $output_x64_selfcontained
dotnet publish ./ILSpy.ReadyToRun/ILSpy.ReadyToRun.csproj -c Release --no-restore --self-contained -r win-x64 -o $output_x64_selfcontained
dotnet publish ./ILSpy.BamlDecompiler/ILSpy.BamlDecompiler.csproj -c Release --no-restore --self-contained -r win-x64 -o $output_x64_selfcontained