<#
.SYNOPSIS
Classify text files by encoding under the current subtree, respecting .gitignore.

.DESCRIPTION
Enumerates tracked files and untracked-but-not-ignored files (via Git) beneath
PWD. Skips likely-binary files (NUL probe). Classifies remaining files as:
	- 'utf8'          : valid UTF-8 (no BOM) or empty file
	- 'utf8-with-bom' : starts with UTF-8 BOM (EF BB BF)
	- 'other'         : text but not valid UTF-8 (e.g., UTF-16/ANSI)

Outputs:
	1) Relative paths of files classified as 'other'
	2) A table by extension: UTF8 / UTF8-with-BOM / Other / Total

Notes:
	- Read-only: this script makes no changes.
	- Requires Git and must be run inside a Git work tree.
#>

[CmdletBinding()]
param()

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# --- Git enumeration ---------------------------------------------------------
function Assert-InGitWorkTree {
	# Throws if not inside a Git work tree.
	$inside = (& git rev-parse --is-inside-work-tree 2>$null).Trim()
	if ($LASTEXITCODE -ne 0 -or $inside -ne 'true') {
		throw 'Not in a Git work tree.'
	}
}

function Get-GitFilesUnderPwd {
	<#
		Returns full paths to tracked + untracked-not-ignored files under PWD.
	#>
	Assert-InGitWorkTree

	$repoRoot = (& git rev-parse --show-toplevel).Trim()
	$pwdPath  = (Get-Location).Path

	# cached (tracked) + others (untracked not ignored)
	$nulSeparated = & git -C $repoRoot ls-files -z --cached --others --exclude-standard

	$relativePaths = $nulSeparated.Split(
		[char]0, [System.StringSplitOptions]::RemoveEmptyEntries)

	foreach ($relPath in $relativePaths) {
		$fullPath = Join-Path $repoRoot $relPath

		# Only include files under the current subtree.
		if ($fullPath.StartsWith($pwdPath,
				[System.StringComparison]::OrdinalIgnoreCase)) {
			if (Test-Path -LiteralPath $fullPath -PathType Leaf) { $fullPath }
		}
	}
}

# --- Probes ------------------------------------------------------------------
function Test-ProbablyBinary {
	# Heuristic: treat as binary if the first 8 KiB contains any NUL byte.
	param([Parameter(Mandatory)][string]$Path)

	try {
		$stream = [System.IO.File]::Open($Path,'Open','Read','ReadWrite')
		try {
			$len = [int][Math]::Min(8192,$stream.Length)
			if ($len -le 0) { return $false }

			$buffer = [byte[]]::new($len)
			[void]$stream.Read($buffer,0,$len)
			return ($buffer -contains 0)
		}
		finally { $stream.Dispose() }
	}
	catch { return $false }
}

function Get-TextEncodingCategory {
	# Returns 'utf8', 'utf8-with-bom', 'other', or $null for likely-binary.
	param([Parameter(Mandatory)][string]$Path)

	$stream = [System.IO.File]::Open($Path,'Open','Read','ReadWrite')
	try {
		$fileLength = $stream.Length
		if ($fileLength -eq 0) { return 'utf8' }

		# BOM check (EF BB BF)
		$header = [byte[]]::new([Math]::Min(3,$fileLength))
		[void]$stream.Read($header,0,$header.Length)
		if ($header.Length -ge 3 -and
				$header[0] -eq 0xEF -and $header[1] -eq 0xBB -and $header[2] -eq 0xBF) {
			return 'utf8-with-bom'
		}

		# Quick binary probe before expensive decoding
		$stream.Position = 0
		$sampleLen = [int][Math]::Min(8192,$fileLength)
		$sample = [byte[]]::new($sampleLen)
		[void]$stream.Read($sample,0,$sampleLen)
		if ($sample -contains 0) { return $null }
	}
	finally { $stream.Dispose() }

	# Validate UTF-8 by decoding with throw-on-invalid option (no BOM).
	try {
		$bytes = [System.IO.File]::ReadAllBytes($Path)
		$utf8 = [System.Text.UTF8Encoding]::new($false,$true)
		[void]$utf8.GetString($bytes)
		return 'utf8'
	}
	catch { return 'other' }
}

# --- Main --------------------------------------------------------------------
$otherFiles  = @()
$byExtension = @{}

$allFiles = Get-GitFilesUnderPwd

foreach ($fullPath in $allFiles) {
	# Avoid decoding likely-binary files.
	if (Test-ProbablyBinary $fullPath) { continue }

	$category = Get-TextEncodingCategory $fullPath
	if (-not $category) { continue }

	$ext = [IO.Path]::GetExtension($fullPath).ToLower()
	if (-not $byExtension.ContainsKey($ext)) {
		$byExtension[$ext] = @{ 'utf8' = 0; 'utf8-with-bom' = 0; 'other' = 0 }
	}

	$byExtension[$ext][$category]++

	if ($category -eq 'other') {
		$otherFiles += (Resolve-Path -LiteralPath $fullPath -Relative)
	}
}

# 1) Files in 'other'
if ($otherFiles.Count -gt 0) {
	'Files classified as ''other'':'
	$otherFiles | Sort-Object | ForEach-Object { "  $_" }
	''
}

# 2) Table by extension
$rows = foreach ($kv in $byExtension.GetEnumerator()) {
	$ext = if ($kv.Key) { $kv.Key } else { '[noext]' }
	$u   = [int]$kv.Value['utf8']
	$b   = [int]$kv.Value['utf8-with-bom']
	$o   = [int]$kv.Value['other']

	[PSCustomObject]@{
		Extension       = $ext
		UTF8            = $u
		'UTF8-with-BOM' = $b
		Other           = $o
		Total           = $u + $b + $o
	}
}

$rows |
	Sort-Object -Property (
			@{Expression='Total';Descending=$true},
		@{Expression='Extension';Descending=$false}
	) |
	Format-Table -AutoSize
