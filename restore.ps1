#!/usr/bin/env pwsh
# Restore all NuGet packages for ILSpy.sln, honouring the committed packages.lock.json files.
# Use updatedeps.ps1 instead when you have changed a PackageVersion and need to refresh the locks.
$ErrorActionPreference = 'Stop'
dotnet restore ILSpy.sln -p:RestoreEnablePackagePruning=false @args
