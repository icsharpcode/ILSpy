#!/usr/bin/env pwsh
# Restore all NuGet packages for ILSpy.slnx, honouring the committed packages.lock.json files.
# Use updatedeps.ps1 instead when you have changed a PackageVersion and need to refresh the locks.
$ErrorActionPreference = 'Stop'
$env:OPENSSL_ENABLE_SHA1_SIGNATURES = '1'
dotnet restore ILSpy.slnx -p:RestoreEnablePackagePruning=false @args
