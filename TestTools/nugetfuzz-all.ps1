#!/usr/bin/env pwsh
# Crawls the nuget.org catalog and runs nugetfuzz.cs on every package id.
# Resumable: page cursor + seen-id list live in ./crawl. Every call is recorded
# in crawl/history.log; the full per-package log is kept only when the run failed.
#
# usage: ./nugetfuzz-all.ps1 [-MaxPages n] [-MaxPackages n] [-CacheCapMB n]
# ponytail: sequential crawl; parallelize per-page if throughput matters

[CmdletBinding()]
param(
	[int]$MaxPages = 0,      # 0 = all
	[int]$MaxPackages = 0,   # 0 = unlimited
	[int]$CacheCapMB = 20480,
	[int]$TimeoutSeconds = 1800
)

$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot

$state = Join-Path $PSScriptRoot 'crawl'
$logs = Join-Path $PSScriptRoot 'logs'
$cache = Join-Path ([Environment]::GetFolderPath('UserProfile')) '.cache/nugetfuzz'
# Findings from every package run land in one append-only ledger. Render the aggregate
# at any time, while the sweep keeps running:
#   dotnet run nugetfuzz.cs -- --report crawl/findings.jsonl
if (-not $env:NUGETFUZZ_LEDGER) {
	$env:NUGETFUZZ_LEDGER = Join-Path $state 'findings.jsonl'
}
# Required by the local OpenSSL configuration to validate the SHA-1 signed packages.
if (-not $env:OPENSSL_ENABLE_SHA1_SIGNATURES) {
	$env:OPENSSL_ENABLE_SHA1_SIGNATURES = '1'
}
New-Item -ItemType Directory -Force -Path $state, $logs | Out-Null

$seenFile = Join-Path $state 'seen-ids.txt'
$cursorFile = Join-Path $state 'next-page.txt'
$historyFile = Join-Path $state 'history.log'

$seen = @{}
if (Test-Path $seenFile) {
	foreach ($id in Get-Content $seenFile) { $seen[$id] = $true }
}

# Disk guard. Evicts least-recently-used <id>/<version> directories down to 80% of the
# cap rather than wiping the cache wholesale: the cache doubles as the corpus decompdiff
# runs against, and a wipe throws away packages that are expensive to re-download and
# that other tools are pointed at.
function Invoke-CachePrune {
	if (-not (Test-Path $cache)) { return }
	$dirSize = { param($d) (Get-ChildItem -LiteralPath $d -Recurse -File -EA SilentlyContinue
		| Measure-Object -Property Length -Sum).Sum / 1MB }
	$used = & $dirSize $cache
	if ($used -le $CacheCapMB) { return }
	$target = $CacheCapMB * 0.8
	Write-Host ("cache {0:N0}MB over {1}MB, evicting least-recently-used down to {2:N0}MB" -f $used, $CacheCapMB, $target)
	# NTFS disables last-access-time updates by default, so on Windows this degrades to
	# least-recently-written, which for an extract-once cache means least recently added.
	$victims = Get-ChildItem -LiteralPath $cache -Directory -EA SilentlyContinue
	| ForEach-Object { Get-ChildItem -LiteralPath $_.FullName -Directory -EA SilentlyContinue }
	| Sort-Object LastAccessTime
	foreach ($dir in $victims) {
		if ($used -le $target) { break }
		$used -= & $dirSize $dir.FullName
		Remove-Item -LiteralPath $dir.FullName -Recurse -Force -EA SilentlyContinue
		$parent = $dir.Parent.FullName
		if (-not (Get-ChildItem -LiteralPath $parent -Force -EA SilentlyContinue)) {
			Remove-Item -LiteralPath $parent -Force -EA SilentlyContinue
		}
	}
	Write-Host ("cache now ~{0:N0}MB" -f $used)
}

try {
	$pages = (Invoke-RestMethod 'https://api.nuget.org/v3/catalog0/index.json').items
	| Sort-Object commitTimeStamp | ForEach-Object { $_.'@id' }
}
catch {
	Write-Error "failed to fetch catalog index: $_"
	exit 1
}

$start = if (Test-Path $cursorFile) { [int](Get-Content $cursorFile -Raw).Trim() } else { 0 }
$i = 0
$donePages = 0
$donePkgs = 0
:pages foreach ($page in $pages) {
	$i++
	if ($i -le $start) { continue }
	if ($MaxPages -gt 0 -and $donePages -ge $MaxPages) { break }

	try {
		$ids = (Invoke-RestMethod $page).items
		| Where-Object { $_.'@type' -eq 'nuget:PackageDetails' }
		| ForEach-Object { $_.'nuget:id'.ToLowerInvariant() }
		| Sort-Object -Unique
	}
	catch {
		Write-Warning "failed to fetch $page, stopping (resume with cursor)"
		break
	}

	foreach ($id in $ids) {
		if ($seen[$id]) { continue }
		if ($MaxPackages -gt 0 -and $donePkgs -ge $MaxPackages) { break pages }
		$seen[$id] = $true
		Add-Content -LiteralPath $seenFile -Value $id

		$log = Join-Path $logs "$id.log"
		$errLog = "$log.err"
		$p = Start-Process dotnet -ArgumentList 'run', 'nugetfuzz.cs', '--', $id `
			-WorkingDirectory $PSScriptRoot -NoNewWindow -PassThru `
			-RedirectStandardOutput $log -RedirectStandardError $errLog
		if ($p.WaitForExit($TimeoutSeconds * 1000)) {
			$rc = $p.ExitCode
		}
		else {
			$p.Kill($true)
			$rc = 124
		}
		# Start-Process cannot merge the two streams into one file; fold them afterwards.
		if ((Get-Item -LiteralPath $errLog -EA SilentlyContinue).Length) {
			Get-Content -LiteralPath $errLog -Raw | Add-Content -LiteralPath $log
		}
		Remove-Item -LiteralPath $errLog -Force -EA SilentlyContinue

		$summary = Select-String -LiteralPath $log -Pattern '\d+ assemblies.*' -EA SilentlyContinue
		| Select-Object -Last 1 | ForEach-Object { $_.Matches[0].Value }
		Add-Content -LiteralPath $historyFile -Value "$(Get-Date -Format o) $id exit=$rc $(if ($summary) { $summary } else { 'no-summary' })"
		if ($rc -eq 0) {
			Remove-Item -LiteralPath $log -Force -EA SilentlyContinue
		}
		else {
			Write-Host "FAILED ($rc): $id -> $log"
		}
		$donePkgs++

		Invoke-CachePrune
	}
	Set-Content -LiteralPath $cursorFile -Value $i
	$donePages++
	Write-Host "--- page $i done ($donePkgs packages this run) ---"
}
Write-Host "run complete: $donePkgs packages processed, failures kept in $logs"
# Reaching here means the sweep ran to completion; a real failure throws (ErrorActionPreference
# is Stop) and exits non-zero on its own. Without this the exit code trails whatever the last
# cmdlet happened to set.
exit 0
