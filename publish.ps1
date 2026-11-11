#!/usr/bin/env pwsh
# Publish the per-platform distribution bundles. Invoked by the build workflow as:
#   ./publish.ps1 -Configuration <Debug|Release> -Platform <windows|linux|macos>
# windows: win-x64 framework-dependent bundle for every configuration (the binaries zip);
#          the win-arm64 framework-dependent and win-x64 self-contained bundles are Release-only.
# linux:   linux-x64 and linux-arm64 self-contained bundles (arm64 is cross-published from the
#          x64 runner, like win-arm64 on Windows).
# macos:   osx-arm64 self-contained bundle; the BuildMacAppBundle target in ILSpy.csproj
#          assembles ILSpy.app next to the publish directory.
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [ValidateSet('windows', 'linux', 'macos')]
    [string]$Platform = 'windows'
)
$ErrorActionPreference = 'Stop'
$env:OPENSSL_ENABLE_SHA1_SIGNATURES = '1'

$base = "./ILSpy/bin/$Configuration/net10.0"

switch ($Platform) {
    'windows' {
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
    }
    'linux' {
        foreach ($rid in 'linux-x64', 'linux-arm64') {
            $output = "$base/$rid/publish/selfcontained"
            dotnet publish ./ILSpy/ILSpy.csproj -c $Configuration --no-restore --self-contained -r $rid -o $output
            dotnet publish ./ILSpy.ReadyToRun/ILSpy.ReadyToRun.csproj -c $Configuration --no-restore --self-contained -r $rid -o $output
        }
    }
    'macos' {
        $output = "$base/osx-arm64/publish/selfcontained"
        # The ReadyToRun plugin must be published first: BuildMacAppBundle runs after the
        # ILSpy publish and snapshots the publish directory into ILSpy.app/Contents/MacOS,
        # so everything that belongs in the bundle has to be in place by then.
        dotnet publish ./ILSpy.ReadyToRun/ILSpy.ReadyToRun.csproj -c $Configuration --no-restore --self-contained -r osx-arm64 -o $output
        dotnet publish ./ILSpy/ILSpy.csproj -c $Configuration --no-restore --self-contained -r osx-arm64 -o $output
    }
}
