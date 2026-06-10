#!/usr/bin/env pwsh
# Clean the Debug and Release build outputs of ILSpy.sln.
$ErrorActionPreference = 'Stop'
dotnet clean ILSpy.sln -c Debug "-p:Platform=Any CPU" @args
dotnet clean ILSpy.sln -c Release "-p:Platform=Any CPU" @args
