#!/usr/bin/env pwsh
# Re-resolve every dependency and rewrite the packages.lock.json files.
# Run this after changing a PackageVersion in Directory.Packages.props (or adding a package),
# then commit the updated lock files.
$ErrorActionPreference = 'Stop'
$env:OPENSSL_ENABLE_SHA1_SIGNATURES = '1'
dotnet restore ILSpy.sln --force-evaluate -p:RestoreEnablePackagePruning=false @args
