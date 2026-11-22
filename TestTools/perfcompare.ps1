#!/usr/bin/env pwsh
# Measures the Rust engine shim against the managed decompiler with the methodology of
# RustPort_HumanLecture.md ("Performance comparison methodology"): every input is decompiled
# by each side in its own process (decompdiff --side, -j 1, warm-up on the first assembly),
# -Runs times per side; the median of the runs is reported as rows of the lecture's results
# table plus the per-type P50/P95.
#
# usage: ./perfcompare.ps1 -Old <ICSharpCode.Decompiler.dll> -New <shim ICSharpCode.Decompiler.dll>
#                          [-Runs 3] [-Out dir] [-Refs dir,...] <dll|dir|@list>...
#
# Each input is one row: a dll, a directory of dlls, or an @list such as the corpus file that
# nuget-top.ps1 writes. Build both sides in Release first; a Debug shim runs the engine with
# debug assertions and overflow checks (cargo profile "release" instead of "ship").

[CmdletBinding(PositionalBinding = $false)]
param(
	[Parameter(Mandatory)][string]$Old,
	[Parameter(Mandatory)][string]$New,
	[int]$Runs = 3,
	[string]$Out,
	[string[]]$Refs = @(),
	[Parameter(Mandatory, ValueFromRemainingArguments)][string[]]$Inputs
)

$ErrorActionPreference = 'Stop'
$decompdiff = Join-Path $PSScriptRoot 'decompdiff.cs'
$Old = (Resolve-Path $Old).Path
$New = (Resolve-Path $New).Path
if (-not $Out) {
	$Out = Join-Path $PSScriptRoot 'perfcompare'
}
New-Item -ItemType Directory -Force -Path $Out | Out-Null

function Get-Median([double[]]$values) {
	$sorted = @($values | Sort-Object)
	if ($sorted.Count -eq 0) { return [double]::NaN }
	$mid = [int][Math]::Floor($sorted.Count / 2)
	if ($sorted.Count % 2) { return $sorted[$mid] }
	return ($sorted[$mid - 1] + $sorted[$mid]) / 2
}

function Invoke-Side([string]$side, [string]$spec, [string]$item, [string]$dir) {
	$arguments = @('run', $decompdiff, '--', '--side', $side, "--$side", $spec, '-j', '1', '-o', $dir)
	foreach ($r in $Refs) { $arguments += @('--refs', $r) }
	$arguments += $item
	& dotnet @arguments | Out-Host
	if ($LASTEXITCODE -ne 0) { throw "decompdiff --side $side failed on $item (exit $LASTEXITCODE)" }
	$perf = Get-Content (Join-Path $dir 'perf.json') -Raw | ConvertFrom-Json
	$s = $perf.sides[0]
	# The per-type lists are kept on disk in perf.json; only the totals travel on.
	[pscustomobject]@{
		TotalMs = [double]$s.totalMs
		P50Ms = [double]$s.p50Ms
		P95Ms = [double]$s.p95Ms
		PeakWorkingSet = [double]$s.peakWorkingSetBytes
		ManagedAllocated = [double]$s.managedAllocatedBytes
		MarshalingNs = if ($s.engine) { [double]$s.engine.MarshalingNanoseconds } else { 0 }
		EngineNs = if ($s.engine) { [double]$s.engine.EngineNanoseconds } else { 0 }
		RustPeakLive = if ($s.engine) { [double]$s.engine.RustPeakLiveBytes } else { 0 }
		Types = [int]$s.types
	}
}

$rows = @()
foreach ($item in $Inputs) {
	$name = [IO.Path]::GetFileNameWithoutExtension($item.TrimStart('@'))
	$median = @{}
	foreach ($side in 'old', 'new') {
		$spec = if ($side -eq 'old') { $Old } else { $New }
		$measured = @(1..$Runs | ForEach-Object {
			Write-Host "=== $name, $side, run $_ of $Runs"
			Invoke-Side $side $spec $item (Join-Path $Out "$name/$side-$_")
		})
		$m = @{}
		foreach ($p in $measured[0].PSObject.Properties.Name) {
			$m[$p] = Get-Median @($measured | ForEach-Object { $_.$p })
		}
		$median[$side] = $m
	}
	$rows += [pscustomobject]@{ Name = $name; Old = $median['old']; New = $median['new'] }
}

$mb = { param($bytes) '{0:F0} MB' -f ($bytes / 1MB) }
$lines = @()
$lines += "Median of $Runs runs per side, one process per run, -j 1."
$lines += ''
$lines += '| Input | Managed CPU total | Engine CPU total | Marshaling share | Managed peak RSS | Engine peak RSS | Rust peak live |'
$lines += '|---|---|---|---|---|---|---|'
foreach ($r in $rows) {
	# The share of the shim's end-to-end time spent neither in the engine nor in managed
	# decompiler code: encoding requests, copying buffers, materializing the answers.
	$share = if ($r.New.TotalMs -gt 0) { '{0:F1} %' -f (100 * $r.New.MarshalingNs / 1e6 / $r.New.TotalMs) } else { '-' }
	$lines += "| $($r.Name) | $('{0:F0} ms' -f $r.Old.TotalMs) | $('{0:F0} ms' -f $r.New.TotalMs) | $share | $(& $mb $r.Old.PeakWorkingSet) | $(& $mb $r.New.PeakWorkingSet) | $(& $mb $r.New.RustPeakLive) |"
}
$lines += ''
$lines += '| Input | types | managed P50 | managed P95 | engine P50 | engine P95 | engine time inside Rust | managed allocated, managed / engine |'
$lines += '|---|---|---|---|---|---|---|---|'
foreach ($r in $rows) {
	$lines += "| $($r.Name) | $($r.Old.Types) / $($r.New.Types) | $('{0:F2} ms' -f $r.Old.P50Ms) | $('{0:F2} ms' -f $r.Old.P95Ms) | $('{0:F2} ms' -f $r.New.P50Ms) | $('{0:F2} ms' -f $r.New.P95Ms) | $('{0:F0} ms' -f ($r.New.EngineNs / 1e6)) | $(& $mb $r.Old.ManagedAllocated) / $(& $mb $r.New.ManagedAllocated) |"
}
$report = $lines -join [Environment]::NewLine
Set-Content -Path (Join-Path $Out 'perfcompare.md') -Value $report
Write-Host ''
Write-Host $report
