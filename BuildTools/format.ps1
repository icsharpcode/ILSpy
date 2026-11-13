#!/usr/bin/env pwsh
# Trigger the commit hook's formatter against the working tree without committing,
# applying the same formatting the pre-commit hook enforces.
$ErrorActionPreference = 'Stop'
$env:OPENSSL_ENABLE_SHA1_SIGNATURES = '1'
$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
$dotnetRoot = if ($dotnet) { Split-Path -Parent $dotnet.Source }
if ($dotnetRoot -and (-not $env:DOTNET_ROOT -or -not (Test-Path (Join-Path $env:DOTNET_ROOT 'shared')))) {
    $env:DOTNET_ROOT = $dotnetRoot
}
if ($env:DOTNET_ROOT -and (-not $env:DOTNET_ROOT_X64 -or -not (Test-Path (Join-Path $env:DOTNET_ROOT_X64 'shared')))) {
    $env:DOTNET_ROOT_X64 = $env:DOTNET_ROOT
}
if (-not $env:DOTNET_ROLL_FORWARD) {
    $env:DOTNET_ROLL_FORWARD = 'LatestMajor'
}
Push-Location (Join-Path $PSScriptRoot '..')
try {
    # On Windows, a bare `bash` can resolve to the WSL launcher (System32\bash.exe),
    # which runs the hook inside a Linux distro where the CRLF-checked-out script
    # cannot even be parsed. Pin to Git Bash, the same CRLF-tolerant shell git
    # itself uses to run the pre-commit hook.
    $bash = if ($IsWindows) {
        Join-Path (Split-Path (Split-Path (Get-Command git).Source)) 'bin\bash.exe'
    }
    else {
        'bash'
    }
    & $bash BuildTools/pre-commit --format
}
finally {
    Pop-Location
}
