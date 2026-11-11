#!/usr/bin/env pwsh
# Runs ILSpy.Tests as several concurrent test-host processes with disjoint test filters.
# The suite is serial by design (one headless Avalonia session and one process-wide app
# composition per test host), so the only way to use more than one core is to run several
# hosts, each on its own share of the fixtures. Defaults to 4 shards of the Debug build;
# pass -Shards 1 for a plain single-process run. Build first (./build.ps1), this runs
# `dotnet test --no-build`. Extra arguments are forwarded to every `dotnet test`.
# Examples: ./runuitests.ps1
#           ./runuitests.ps1 -Shards 2 -Configuration Release -ResultsDirectory test-results
param(
    [int]$Shards = 4,
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug',
    [string]$ResultsDirectory = 'test-results'
)
$ErrorActionPreference = 'Stop'

# Seconds per group in a single-process Debug run (2026-10-19, 24-core workstation); only the
# ratios matter. A key ending in '.' is a sub-namespace of ICSharpCode.ILSpy.Tests, the others
# are fixtures that live directly in that namespace. 'Analyzers.' also covers
# 'Analyzers.Library.' and 'Views.' covers 'Views.Filters.'.
$weights = [ordered]@{
    'Metadata.' = 30
    'Analyzers.' = 30
    'AssemblyTreeTests.' = 20
    'Search.' = 17
    'TextView.' = 16
    'Bookmarks.' = 9
    'OptionsTabTests.' = 9
    'Docking.' = 8
    'Views.' = 7
    'Languages.' = 6
    'PreviewTabPromotionTests.' = 6
    'DecompilerViewTests.' = 5
    'Input.' = 4
    'DecompileInNewViewTests.' = 3
    'ApiVisibilityFilterTests.' = 3
    'Processes.' = 3
    'Compare.' = 3
    'Controls.' = 3
    'Commands.' = 3
    'CommandLineArgumentsTests.' = 3
    'BrowseBackForwardCommandTests.' = 3
    'EditorCommandsTests.' = 2
    'AboutPageUpdateSectionTests.' = 2
    'HelpCommandTests.' = 2
    'SelectAllDeleteBatchingTests.' = 2
}
# Everything the keys above do not name: the many small root-namespace fixtures.
$restSeconds = 55

# Greedy balance: heaviest group first into the lightest shard. The last shard takes the
# complement of every key the other shards name (plus the rest), so a fixture added later is
# always run by some shard.
$bins = @(1..$Shards | ForEach-Object { [pscustomobject]@{ Keys = [System.Collections.Generic.List[string]]::new(); Seconds = 0.0 } })
$bins[-1].Seconds = $restSeconds
foreach ($entry in ($weights.GetEnumerator() | Sort-Object Value -Descending)) {
    $lightest = $bins | Sort-Object Seconds | Select-Object -First 1
    $lightest.Keys.Add($entry.Key)
    $lightest.Seconds += $entry.Value
}

$prefix = 'ICSharpCode.ILSpy.Tests.'
$filters = @()
$named = @()
for ($i = 0; $i -lt $Shards - 1; $i++) {
    $filters += ($bins[$i].Keys | ForEach-Object { "FullyQualifiedName~$prefix$_" }) -join '|'
    $named += $bins[$i].Keys
}
$filters += ($named | ForEach-Object { "FullyQualifiedName!~$prefix$_" }) -join '&'

New-Item -ItemType Directory -Force $ResultsDirectory | Out-Null
$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
$processes = @()
for ($i = 0; $i -lt $Shards; $i++) {
    $n = $i + 1
    $arguments = @(
        'test', '--project', 'ILSpy.Tests/ILSpy.Tests.csproj',
        '--configuration', $Configuration, '--no-build',
        '--report-trx', '--report-trx-filename', "ui-shard$n.trx",
        '--results-directory', $ResultsDirectory
    ) + $args
    if ($Shards -gt 1) {
        $arguments += @('--filter', $filters[$i])
    }
    $log = Join-Path $ResultsDirectory "ui-shard$n.log"
    $what = if ($Shards -eq 1) { 'everything' } elseif ($i -eq $Shards - 1) { 'everything else' } else { $bins[$i].Keys -join ' ' }
    Write-Host "shard $n of ${Shards}: $what"
    $processes += Start-Process dotnet -ArgumentList $arguments -PassThru -NoNewWindow -RedirectStandardOutput $log
}

$processes | Wait-Process
$failed = 0
for ($i = 0; $i -lt $Shards; $i++) {
    $log = Join-Path $ResultsDirectory "ui-shard$($i + 1).log"
    $summary = (Get-Content $log | Select-String '^\s+(total|failed|succeeded|skipped|duration):' | ForEach-Object { $_.Line.Trim() }) -join ', '
    Write-Host "shard $($i + 1): exit $($processes[$i].ExitCode); $summary"
    if ($processes[$i].ExitCode -ne 0) {
        $failed++
        Get-Content $log | Select-String '^failed ' | ForEach-Object { Write-Host "  $($_.Line)" }
    }
}
Write-Host "wall clock: $([int]$stopwatch.Elapsed.TotalSeconds) s; logs and TRX files are in $ResultsDirectory"
exit $failed
