#!/usr/bin/env pwsh
# Re-resolve every dependency and rewrite the packages.lock.json files.
# Run this after changing a PackageVersion in Directory.Packages.props (or adding a package),
# then commit the updated lock files.
$ErrorActionPreference = 'Stop'
dotnet restore ILSpy.sln --force-evaluate -p:RestoreEnablePackagePruning=false @args
