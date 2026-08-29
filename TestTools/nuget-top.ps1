#!/usr/bin/env pwsh
# Downloads the N most-downloaded packages on nuget.org into the cache that decompdiff
# uses as its corpus (~/.cache/nugetfuzz), together with their dependency closures.
#
# The download, TFM selection and dependency walk all live in nugetfuzz.cs already, so
# this only picks the ids and hands them over: an empty query against the search service
# returns packages ordered by download count.
#
# usage: ./nuget-top.ps1 [-Count n] [-ListOnly] [-Out path] [-Skip n]
# ponytail: no per-id retry; a package that fails to download is reported and skipped

[CmdletBinding()]
param(
	[int]$Count = 100,
	[int]$Skip = 0,          # start further down the ranking, to extend an existing corpus
	[switch]$ListOnly,       # write the id list, download nothing
	[string]$Out
)

$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot

# Required by the local OpenSSL configuration to validate the SHA-1 signed packages.
if (-not $env:OPENSSL_ENABLE_SHA1_SIGNATURES) {
	$env:OPENSSL_ENABLE_SHA1_SIGNATURES = '1'
}

$state = Join-Path $PSScriptRoot 'crawl'
New-Item -ItemType Directory -Force -Path $state | Out-Null
if (-not $Out) {
	$Out = Join-Path $state "top-$Count.txt"
}

# The search service caps how far a caller may page; ask for the ids in blocks and stop
# as soon as it stops handing any back, so a too-large -Count truncates instead of failing.
$endpoint = 'https://azuresearch-usnc.nuget.org/query'
$ids = [System.Collections.Generic.List[string]]::new()
$page = 100
while ($ids.Count -lt $Count) {
	$take = [Math]::Min($page, $Count - $ids.Count)
	$uri = "${endpoint}?q=&skip=$($Skip + $ids.Count)&take=$take&prerelease=false&semVerLevel=2.0.0"
	$response = Invoke-RestMethod -Uri $uri
	if (-not $response.data -or $response.data.Count -eq 0) {
		Write-Warning "search returned nothing at skip=$($Skip + $ids.Count); stopping at $($ids.Count) ids"
		break
	}
	foreach ($entry in $response.data) {
		$ids.Add($entry.id)
	}
}

# Ranking order is worth keeping in the file: it says what a truncated corpus dropped.
Set-Content -Path $Out -Value $ids
Write-Host "$($ids.Count) package ids -> $Out"

if ($ListOnly) {
	return
}

# A package already restored on this machine is used from the machine-wide NuGet cache
# rather than copied, so the downloaded set is not one directory that can be handed to
# decompdiff. nugetfuzz names the lib directory it settled on for each package, and that
# list IS the corpus - one entry per package, at the target framework it chose.
$corpusFile = [IO.Path]::ChangeExtension($Out, '.corpus.txt')
dotnet run nugetfuzz.cs -- --download-only "@$Out" | Tee-Object -Variable log
if ($LASTEXITCODE -ne 0) {
	throw "nugetfuzz exited with $LASTEXITCODE"
}
$dirs = $log | ForEach-Object { if ($_ -match '^\s*cached:\s*(.+)$') { $Matches[1].Trim() } }
Set-Content -Path $corpusFile -Value $dirs
Write-Host ""
Write-Host "$($dirs.Count) lib directories -> $corpusFile"
Write-Host "dotnet run decompdiff.cs -- --old <ref> --new <ref> -o report `@$corpusFile"
