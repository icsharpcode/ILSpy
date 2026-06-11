#!/usr/bin/env pwsh
# Build (and optionally sign + notarize) the ILSpy dmg for distribution.
#
# This is an OFFLINE release-manager tool: it is never called from CI, because no signing
# happens in CI. Typical flow:
#   1. Download the 'ILSpy macOS arm64 ...' artifact from the build workflow and unzip it.
#   2. ./create-dmg.ps1 -AppPath ./ILSpy.app -Version 11.0.0.8948 `
#          -SigningIdentity 'Developer ID Application: ...' -Notarize -NotaryProfile ilspy
#      (omit -SigningIdentity for an unsigned smoke-test dmg)
#   3. Attach the dmg to the GitHub release; use the printed sha256 for the Homebrew cask
#      (see ../homebrew/README.md).
#
# -NotaryProfile names credentials stored once via:
#   xcrun notarytool store-credentials <profile> --apple-id <id> --team-id <team>
param(
    [Parameter(Mandatory)]
    [string]$AppPath,
    [Parameter(Mandatory)]
    [string]$Version,
    [string]$OutputPath,
    [string]$SigningIdentity,
    [switch]$Notarize,
    [string]$NotaryProfile
)
$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $true

if (-not $IsMacOS) {
    throw 'This script needs macOS (codesign, hdiutil, notarytool).'
}
if ($Notarize -and -not $NotaryProfile) {
    throw '-Notarize requires -NotaryProfile (a notarytool keychain profile name).'
}
$AppPath = (Resolve-Path $AppPath).Path
if (-not (Test-Path "$AppPath/Contents/Info.plist")) {
    throw "$AppPath does not look like an .app bundle."
}
if (-not $OutputPath) {
    $OutputPath = "ILSpy-$Version.dmg"
}

if ($SigningIdentity) {
    codesign --force --deep --options runtime --timestamp --sign $SigningIdentity $AppPath
}

$stagingDir = Join-Path ([IO.Path]::GetTempPath()) 'ilspy-dmg'
Remove-Item -Recurse -Force $stagingDir -ErrorAction Ignore
New-Item -ItemType Directory $stagingDir | Out-Null
Copy-Item $AppPath $stagingDir -Recurse
ln -s /Applications (Join-Path $stagingDir 'Applications')

Remove-Item $OutputPath -ErrorAction Ignore
hdiutil create -volname "ILSpy $Version" -srcfolder $stagingDir -ov -format UDZO $OutputPath

if ($SigningIdentity) {
    codesign --sign $SigningIdentity $OutputPath
}
if ($Notarize) {
    xcrun notarytool submit $OutputPath --keychain-profile $NotaryProfile --wait
    xcrun stapler staple $OutputPath
}

Write-Host "created $OutputPath"
Write-Host "sha256 (for the Homebrew cask):"
shasum -a 256 $OutputPath
