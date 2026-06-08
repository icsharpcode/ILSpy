#!/usr/bin/env pwsh
# Trigger the commit hook's formatter against the working tree without committing,
# applying the same formatting the pre-commit hook enforces.
$ErrorActionPreference = 'Stop'
Push-Location (Join-Path $PSScriptRoot '..')
try {
    bash BuildTools/pre-commit --format
}
finally {
    Pop-Location
}
