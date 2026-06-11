#!/usr/bin/env pwsh
# Package the linux-x64 self-contained publish output (created by `./publish.ps1 -Platform linux`)
# into the Linux distribution artifacts:
#   ILSpy_linux-x64_<version>.zip   Info-ZIP archive (preserves the execute flag, unlike
#                                   Compress-Archive/7z, which drop unix mode bits)
#   ilspy_<version>-1_amd64.deb     built with dpkg-deb
#   ilspy-<version>-1.x86_64.rpm    built with rpmbuild
# Must run on Linux. zip and dpkg-deb are preinstalled on GitHub's ubuntu runners;
# locally: sudo apt-get install -y zip dpkg rpm
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
$resources = Join-Path $PSScriptRoot 'packaging/linux'

foreach ($tool in 'zip', 'dpkg-deb', 'rpmbuild') {
    if (-not (Get-Command $tool -ErrorAction SilentlyContinue)) {
        throw "'$tool' not found. This script must run on Linux; install the tools with: sudo apt-get install -y zip dpkg rpm"
    }
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

$publishDir = Join-Path $repoRoot "ILSpy/bin/$Configuration/net10.0/linux-x64/publish/selfcontained"
if (-not (Test-Path (Join-Path $publishDir 'ILSpy'))) {
    throw "Publish output not found at $publishDir. Run ./publish.ps1 -Configuration $Configuration -Platform linux first."
}

$staging = New-Item -ItemType Directory -Force (Join-Path $repoRoot $StagingDirectory)

# ----- zip -----
$zipPath = Join-Path $staging "ILSpy_linux-x64_$Version.zip"
Remove-Item $zipPath -ErrorAction Ignore
Push-Location $publishDir
try {
    zip -r -q $zipPath . -x '*.pdb'
}
finally {
    Pop-Location
}
Write-Host "created $zipPath"

# Both dpkg and rpm reject '-' inside a version (it separates the package revision in deb
# and is illegal in the rpm Version tag). '~' is legal in both and sorts *before* the
# plain version, which is the correct semantic for pre-release suffixes like '-preview1'.
$pkgVersion = $Version -replace '-', '~'

# Shared payload layout for both package formats: the app in /opt/ilspy, a launcher
# symlink on PATH, and desktop integration files.
function Copy-PackagePayload([string]$root) {
    New-Item -ItemType Directory -Force (Join-Path $root 'opt/ilspy'), (Join-Path $root 'usr/bin'),
        (Join-Path $root 'usr/share/applications'), (Join-Path $root 'usr/share/icons/hicolor/256x256/apps') | Out-Null
    Copy-Item "$publishDir/*" (Join-Path $root 'opt/ilspy') -Recurse
    Get-ChildItem (Join-Path $root 'opt/ilspy') -Filter '*.pdb' -Recurse | Remove-Item
    chmod 755 (Join-Path $root 'opt/ilspy/ILSpy')
    ln -s /opt/ilspy/ILSpy (Join-Path $root 'usr/bin/ilspy')
    Copy-Item (Join-Path $resources 'ilspy.desktop') (Join-Path $root 'usr/share/applications')
    Copy-Item (Join-Path $resources 'ilspy.png') (Join-Path $root 'usr/share/icons/hicolor/256x256/apps')
    chmod 644 (Join-Path $root 'usr/share/applications/ilspy.desktop') (Join-Path $root 'usr/share/icons/hicolor/256x256/apps/ilspy.png')
}

# ----- deb -----
$debRoot = Join-Path ([IO.Path]::GetTempPath()) 'ilspy-deb'
Remove-Item -Recurse -Force $debRoot -ErrorAction Ignore
New-Item -ItemType Directory -Force (Join-Path $debRoot 'DEBIAN') | Out-Null
Copy-PackagePayload $debRoot

# Debian has no unversioned libicu package and the soname-versioned name differs per
# release, so depend on an OR-chain covering the releases we care about (the leading
# plain 'libicu' catches derivatives that do ship a virtual package).
$icuVersions = 78, 77, 76, 74, 72, 71, 70, 69, 68, 67, 66, 65, 63
$icuDeps = (@('libicu') + ($icuVersions | ForEach-Object { "libicu$_" })) -join ' | '
$installedSize = [int]((Get-ChildItem $debRoot -Recurse -File | Measure-Object Length -Sum).Sum / 1024)

$control = (Get-Content (Join-Path $resources 'deb/control.template') -Raw).
    Replace('@VERSION@', "$pkgVersion-1").
    Replace('@ARCH@', 'amd64').
    Replace('@INSTALLED_SIZE@', "$installedSize").
    Replace('@ICU_DEPS@', $icuDeps)
Set-Content (Join-Path $debRoot 'DEBIAN/control') $control

$debPath = Join-Path $staging "ilspy_${pkgVersion}-1_amd64.deb"
# gzip instead of dpkg's zstd default so the package installs on older dpkg versions
dpkg-deb -Zgzip --root-owner-group --build $debRoot $debPath
Write-Host "created $debPath"

# ----- rpm -----
$rpmTop = Join-Path ([IO.Path]::GetTempPath()) 'ilspy-rpm'
Remove-Item -Recurse -Force $rpmTop -ErrorAction Ignore
$rpmRoot = Join-Path $rpmTop 'payload'
New-Item -ItemType Directory -Force $rpmRoot | Out-Null
Copy-PackagePayload $rpmRoot

rpmbuild -bb (Join-Path $resources 'rpm/ilspy.spec') --target x86_64 `
    --define "_topdir $rpmTop" `
    --define "_version $pkgVersion" `
    --define "_payload_dir $rpmRoot"
$rpm = Get-ChildItem (Join-Path $rpmTop 'RPMS') -Filter '*.rpm' -Recurse
Move-Item $rpm.FullName $staging -Force
Write-Host "created $(Join-Path $staging $rpm.Name)"
