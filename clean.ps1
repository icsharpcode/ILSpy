#!/usr/bin/env pwsh
# Clean the Debug and Release build outputs of ILSpy.sln.
$ErrorActionPreference = 'Stop'
$env:OPENSSL_ENABLE_SHA1_SIGNATURES = '1'
dotnet clean ILSpy.sln -c Debug "-p:Platform=Any CPU" @args
dotnet clean ILSpy.sln -c Release "-p:Platform=Any CPU" @args
