#!/usr/bin/env pwsh
# Package the osx-arm64 ILSpy.app bundle (created by `./publish.ps1 -Platform macos` via the
# BuildMacAppBundle target in ILSpy.csproj) into ILSpy_macos-arm64_<version>.zip.
# Stamps the version placeholders in the bundle's Info.plist first.
# Must run on macOS/Linux: Info-ZIP's zip preserves the execute flag and (-y) symlinks,
# which Compress-Archive does not.
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    # Defaults to the CI-computed version (env var, then the ILSPY_VERSION file written by
    # BuildTools/ghactions-install.ps1), falling back to the committed VERSION file for
    # local runs.
    [string]$Version,
    [string]$StagingDirectory = 'buildartifacts'
)
$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $true

$repoRoot = Split-Path -Parent $PSScriptRoot

if (-not (Get-Command zip -ErrorAction SilentlyContinue)) {
    throw "'zip' not found. This script needs Info-ZIP, available on macOS and Linux."
}

if (-not $Version) {
    if ($env:ILSPY_VERSION_NUMBER) {
        $Version = $env:ILSPY_VERSION_NUMBER
    }
    elseif (Test-Path (Join-Path $repoRoot 'ILSPY_VERSION')) {
        $Version = (Get-Content (Join-Path $repoRoot 'ILSPY_VERSION') -Raw).Trim()
    }
    else {
        $Version = (Get-Content (Join-Path $repoRoot 'VERSION') -Raw).Trim()
    }
}

$publishDir = Join-Path $repoRoot "ILSpy/bin/$Configuration/net10.0/osx-arm64/publish"
$appDir = Join-Path $publishDir 'ILSpy.app'
if (-not (Test-Path $appDir)) {
    throw "ILSpy.app not found at $appDir. Run ./publish.ps1 -Configuration $Configuration -Platform macos first."
}

# CFBundleVersion / CFBundleShortVersionString must be dotted numerics; strip the
# pre-release suffix (11.0.0.8948-preview1 -> 11.0.0.8948) and shorten to three parts.
$numericVersion = ($Version -split '-')[0]
$shortVersion = ($numericVersion -split '\.')[0..2] -join '.'
$plistPath = Join-Path $appDir 'Contents/Info.plist'
$plist = (Get-Content $plistPath -Raw).
    Replace('ILSPY_SHORT_VERSION', $shortVersion).
    Replace('ILSPY_VERSION', $numericVersion)
Set-Content $plistPath $plist

$staging = New-Item -ItemType Directory -Force (Join-Path $repoRoot $StagingDirectory)
$zipPath = Join-Path $staging "ILSpy_macos-arm64_$Version.zip"
Remove-Item $zipPath -ErrorAction Ignore
Push-Location $publishDir
try {
    zip -r -y -q $zipPath ILSpy.app -x '*.pdb'
}
finally {
    Pop-Location
}
Write-Host "created $zipPath"
