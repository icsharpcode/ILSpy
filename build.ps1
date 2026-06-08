#!/usr/bin/env pwsh
# Build ILSpy.sln. Defaults to Debug; pass -Configuration Release for a release build.
# Extra arguments are forwarded to `dotnet build` (e.g. ./build.ps1 -Configuration Release --no-restore).
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug'
)
$ErrorActionPreference = 'Stop'
dotnet build ILSpy.sln -c $Configuration "-p:Platform=Any CPU" @args
