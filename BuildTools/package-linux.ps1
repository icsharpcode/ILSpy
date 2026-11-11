#!/usr/bin/env pwsh
# Package the linux-x64 and linux-arm64 self-contained publish outputs (created by
# `./publish.ps1 -Platform linux`) into the Linux distribution artifacts, per RID:
#   ILSpy_<rid>_<version>.zip         Info-ZIP archive (preserves the execute flag, unlike
#                                     Compress-Archive/7z, which drop unix mode bits)
#   ilspy_<version>-1_<arch>.deb      built with dpkg-deb (amd64 / arm64)
#   ilspy-<version>-1.<arch>.rpm      built with rpmbuild (x86_64 / aarch64)
#   ILSpy-<version>-<arch>.AppImage   built with appimagetool (x86_64 / aarch64)
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

# Per-RID names: dpkg and rpm each have their own architecture vocabulary.
$arches = @{
    'linux-x64'   = @{ deb = 'amd64'; rpm = 'x86_64'; appimage = 'x86_64' }
    'linux-arm64' = @{ deb = 'arm64'; rpm = 'aarch64'; appimage = 'aarch64' }
}

$staging = New-Item -ItemType Directory -Force (Join-Path $repoRoot $StagingDirectory)

# appimagetool and the type2 runtime it embeds are not apt packages; both come from pinned
# GitHub releases. The tool runs with --appimage-extract-and-run so the host needs no FUSE,
# and --runtime-file lets the x86_64 tool emit an aarch64 image.
$appImageToolVersion = '1.9.1'
$appImageRuntimeVersion = '20251108'
$toolDir = Join-Path ([IO.Path]::GetTempPath()) 'ilspy-appimagetool'
New-Item -ItemType Directory -Force $toolDir | Out-Null
$appImageTool = Join-Path $toolDir 'appimagetool'
if (-not (Test-Path $appImageTool)) {
    curl --retry 5 --retry-delay 2 -fsSL -o $appImageTool "https://github.com/AppImage/appimagetool/releases/download/$appImageToolVersion/appimagetool-$(uname -m).AppImage"
    chmod +x $appImageTool
}

# Both dpkg and rpm reject '-' inside a version (it separates the package revision in deb
# and is illegal in the rpm Version tag). '~' is legal in both and sorts *before* the
# plain version, which is the correct semantic for pre-release suffixes like '-preview1'.
# CI versions can also embed a branch name (e.g. 'avalonia12_1_1'); dpkg only accepts
# alphanumerics and '.+~' there, so map every other character (underscores etc.) to '.'.
$pkgVersion = $Version -replace '-', '~' -replace '[^A-Za-z0-9.+~]', '.'

# Debian has no unversioned libicu package and the soname-versioned name differs per
# release, so depend on an OR-chain covering the releases we care about (the leading
# plain 'libicu' catches derivatives that do ship a virtual package). apt skips
# alternatives that do not exist as packages, so the list deliberately runs several ICU
# releases ahead of what any distro ships (ICU bumps its major twice a year; 78 is the
# newest in Debian sid and Ubuntu devel as of late 2026) to stay installable on future
# releases without an edit here. The .NET runtime itself loads any ICU version.
$icuVersions = 84, 83, 82, 81, 80, 79, 78, 77, 76, 74, 72, 71, 70, 69, 68, 67, 66, 65, 63
$icuDeps = (@('libicu') + ($icuVersions | ForEach-Object { "libicu$_" })) -join ' | '

# Shared payload layout for both package formats: the app in /opt/ilspy, a launcher
# symlink on PATH, and desktop integration files.
function Copy-PackagePayload([string]$publishDir, [string]$root) {
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

foreach ($rid in $arches.Keys | Sort-Object) {
    $arch = $arches[$rid]
    $publishDir = Join-Path $repoRoot "ILSpy/bin/$Configuration/net10.0/$rid/publish/selfcontained"
    if (-not (Test-Path (Join-Path $publishDir 'ILSpy'))) {
        throw "Publish output not found at $publishDir. Run ./publish.ps1 -Configuration $Configuration -Platform linux first."
    }

    # ----- zip -----
    $zipPath = Join-Path $staging "ILSpy_${rid}_$Version.zip"
    Remove-Item $zipPath -ErrorAction Ignore
    Push-Location $publishDir
    try {
        zip -r -q $zipPath . -x '*.pdb'
    }
    finally {
        Pop-Location
    }
    Write-Host "created $zipPath"

    # ----- deb -----
    $debRoot = Join-Path ([IO.Path]::GetTempPath()) "ilspy-deb-$rid"
    Remove-Item -Recurse -Force $debRoot -ErrorAction Ignore
    New-Item -ItemType Directory -Force (Join-Path $debRoot 'DEBIAN') | Out-Null
    Copy-PackagePayload $publishDir $debRoot
    $installedSize = [int]((Get-ChildItem $debRoot -Recurse -File | Measure-Object Length -Sum).Sum / 1024)

    $control = (Get-Content (Join-Path $resources 'deb/control.template') -Raw).
        Replace('@VERSION@', "$pkgVersion-1").
        Replace('@ARCH@', $arch.deb).
        Replace('@INSTALLED_SIZE@', "$installedSize").
        Replace('@ICU_DEPS@', $icuDeps)
    Set-Content (Join-Path $debRoot 'DEBIAN/control') $control

    $debPath = Join-Path $staging "ilspy_${pkgVersion}-1_$($arch.deb).deb"
    # gzip instead of dpkg's zstd default so the package installs on older dpkg versions
    dpkg-deb -Zgzip --root-owner-group --build $debRoot $debPath
    Write-Host "created $debPath"

    # ----- rpm -----
    $rpmTop = Join-Path ([IO.Path]::GetTempPath()) "ilspy-rpm-$rid"
    Remove-Item -Recurse -Force $rpmTop -ErrorAction Ignore
    $rpmRoot = Join-Path $rpmTop 'payload'
    New-Item -ItemType Directory -Force $rpmRoot | Out-Null
    Copy-PackagePayload $publishDir $rpmRoot

    rpmbuild -bb (Join-Path $resources 'rpm/ilspy.spec') --target $arch.rpm `
        --define "_topdir $rpmTop" `
        --define "_version $pkgVersion" `
        --define "_payload_dir $rpmRoot"
    $rpm = Get-ChildItem (Join-Path $rpmTop 'RPMS') -Filter '*.rpm' -Recurse
    Move-Item $rpm.FullName $staging -Force
    Write-Host "created $(Join-Path $staging $rpm.Name)"

    # ----- AppImage -----
    # One relocatable file for distros that take neither deb nor rpm. The deb/rpm payload is
    # not reused: its /opt paths and absolute symlink do not survive relocation. Layout is
    # the minimal AppDir appimagetool accepts: the app under usr/bin, AppRun pointing at it,
    # and the desktop file plus icon at the root.
    $runtime = Join-Path $toolDir "runtime-$($arch.appimage)"
    if (-not (Test-Path $runtime)) {
        curl --retry 5 --retry-delay 2 -fsSL -o $runtime "https://github.com/AppImage/type2-runtime/releases/download/$appImageRuntimeVersion/runtime-$($arch.appimage)"
    }
    $appDir = Join-Path ([IO.Path]::GetTempPath()) "ilspy-appdir-$rid"
    Remove-Item -Recurse -Force $appDir -ErrorAction Ignore
    New-Item -ItemType Directory -Force (Join-Path $appDir 'usr/bin') | Out-Null
    Copy-Item "$publishDir/*" (Join-Path $appDir 'usr/bin') -Recurse
    Get-ChildItem (Join-Path $appDir 'usr/bin') -Filter '*.pdb' -Recurse | Remove-Item
    chmod 755 (Join-Path $appDir 'usr/bin/ILSpy')
    ln -s usr/bin/ILSpy (Join-Path $appDir 'AppRun')
    # Exec must be a bare command inside an AppImage; integration tools (appimaged, Gear
    # Lever) rewrite it to the image's path when they register the desktop entry.
    (Get-Content (Join-Path $resources 'ilspy.desktop')) -replace '^Exec=.*', 'Exec=ILSpy %F' |
        Set-Content (Join-Path $appDir 'ilspy.desktop')
    Copy-Item (Join-Path $resources 'ilspy.png') (Join-Path $appDir 'ilspy.png')
    Copy-Item (Join-Path $resources 'ilspy.png') (Join-Path $appDir '.DirIcon')

    $appImagePath = Join-Path $staging "ILSpy-$Version-$($arch.appimage).AppImage"
    Remove-Item $appImagePath -ErrorAction Ignore
    $env:ARCH = $arch.appimage
    & $appImageTool --appimage-extract-and-run --runtime-file $runtime $appDir $appImagePath
    Write-Host "created $appImagePath"
}
