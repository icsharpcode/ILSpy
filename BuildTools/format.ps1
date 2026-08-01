#!/usr/bin/env pwsh
# Trigger the commit hook's formatter against the working tree without committing,
# applying the same formatting the pre-commit hook enforces.
$ErrorActionPreference = 'Stop'
$env:OPENSSL_ENABLE_SHA1_SIGNATURES = '1'
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
