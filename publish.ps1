#!/usr/bin/env pwsh
# Publish the Windows distribution bundles. Invoked by the build workflow as:
#   .\publish.ps1 --configuration <Debug|Release>
# The win-x64 framework-dependent bundle is produced for every configuration (the binaries zip).
# The win-arm64 framework-dependent and win-x64 self-contained bundles are Release-only.
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)
$ErrorActionPreference = 'Stop'

$base = "./ILSpy/bin/$Configuration/net10.0"

$output_x64 = "$base/win-x64/publish/fwdependent"
dotnet publish ./ILSpy/ILSpy.csproj -c $Configuration --no-restore --no-self-contained -r win-x64 -o $output_x64
dotnet publish ./ILSpy.ReadyToRun/ILSpy.ReadyToRun.csproj -c $Configuration --no-restore --no-self-contained -r win-x64 -o $output_x64

if ($Configuration -eq 'Release') {
    $output_arm64 = "$base/win-arm64/publish/fwdependent"
    dotnet publish ./ILSpy/ILSpy.csproj -c $Configuration --no-restore --no-self-contained -r win-arm64 -o $output_arm64
    dotnet publish ./ILSpy.ReadyToRun/ILSpy.ReadyToRun.csproj -c $Configuration --no-restore --no-self-contained -r win-arm64 -o $output_arm64

    $output_x64_selfcontained = "$base/win-x64/publish/selfcontained"
    dotnet publish ./ILSpy/ILSpy.csproj -c $Configuration --no-restore --self-contained -r win-x64 -o $output_x64_selfcontained
    dotnet publish ./ILSpy.ReadyToRun/ILSpy.ReadyToRun.csproj -c $Configuration --no-restore --self-contained -r win-x64 -o $output_x64_selfcontained
}
