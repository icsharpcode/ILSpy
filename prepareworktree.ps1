#!/usr/bin/env pwsh
# Initialize submodules, clean the solution, and build the worktree.
$ErrorActionPreference = 'Stop'

git -C $PSScriptRoot submodule update --init --recursive
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

& "$PSScriptRoot/clean.ps1"
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

& "$PSScriptRoot/build.ps1"
exit $LASTEXITCODE
