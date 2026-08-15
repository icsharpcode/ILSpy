# Temporary CI diagnostics: samples memory/paging/disk counters and times a module walk over
# every process (the same work ILSpy's process explorer does) while the test step runs.
param([string]$OutFile, [int]$IntervalSeconds = 20, [int]$MaxMinutes = 45)

$deadline = (Get-Date).AddMinutes($MaxMinutes)
while ((Get-Date) -lt $deadline) {
	$stamp = Get-Date -Format 'HH:mm:ss'
	$lines = @()
	try {
		$os = Get-CimInstance Win32_OperatingSystem
		$pf = Get-CimInstance Win32_PageFileUsage | Select-Object -First 1
		$lines += "[$stamp] mem free={0}MB total={1}MB pagefile used={2}MB/{3}MB" -f
			[int]($os.FreePhysicalMemory / 1024), [int]($os.TotalVisibleMemorySize / 1024),
			$pf.CurrentUsage, $pf.AllocatedBaseSize
	} catch { $lines += "[$stamp] mem: $_" }
	try {
		$c = Get-Counter -Counter '\Memory\Pages/sec', '\Memory\Pages Input/sec', '\Memory\Available MBytes',
			'\PhysicalDisk(_Total)\Avg. Disk sec/Read', '\PhysicalDisk(_Total)\Current Disk Queue Length',
			'\Processor(_Total)\% Processor Time', '\System\Processes' -SampleInterval 2 -MaxSamples 1
		$vals = $c.CounterSamples | ForEach-Object { "{0}={1:N1}" -f ($_.Path -replace '.*\\', ''), $_.CookedValue }
		$lines += "[$stamp] ctr " + ($vals -join ' ')
	} catch { $lines += "[$stamp] ctr: $_" }
	try {
		$top = Get-Process | Sort-Object WorkingSet64 -Descending | Select-Object -First 6 |
			ForEach-Object { "{0}({1}) ws={2}MB priv={3}MB" -f $_.ProcessName, $_.Id, [int]($_.WorkingSet64 / 1MB), [int]($_.PrivateMemorySize64 / 1MB) }
		$lines += "[$stamp] top " + ($top -join '; ')
	} catch { $lines += "[$stamp] top: $_" }
	try {
		$sw = [Diagnostics.Stopwatch]::StartNew()
		$slow = @(); $n = 0; $failed = 0
		foreach ($p in [System.Diagnostics.Process]::GetProcesses()) {
			$t = [Diagnostics.Stopwatch]::StartNew()
			$count = -1
			try { $count = $p.Modules.Count } catch { $failed++ }
			$t.Stop(); $n++
			if ($t.ElapsedMilliseconds -gt 1000) {
				$slow += "{0}({1}) {2}ms mods={3} ws={4}MB" -f $p.ProcessName, $p.Id, $t.ElapsedMilliseconds, $count, [int]($p.WorkingSet64 / 1MB)
			}
			$p.Dispose()
		}
		$lines += "[$stamp] walk total={0}ms processes={1} failed={2} slow: {3}" -f $sw.ElapsedMilliseconds, $n, $failed, ($slow -join '; ')
	} catch { $lines += "[$stamp] walk: $_" }
	Add-Content -Path $OutFile -Value $lines
	Start-Sleep -Seconds $IntervalSeconds
}
