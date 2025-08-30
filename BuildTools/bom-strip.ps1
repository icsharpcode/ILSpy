<#
.SYNOPSIS
Strip UTF-8 BOM from selected text files under the current subtree, respecting
.gitignore.

.DESCRIPTION
Enumerates tracked and untracked-but-not-ignored files under the current
directory (via Git), filters to texty extensions and dotfiles, skips likely
binary files (NUL probe), and removes a leading UTF-8 BOM (EF BB BF) in place.

Refuses to run if there are uncommitted changes as a safeguard. Use -Force to override.
Supports -WhatIf/-Confirm via ShouldProcess.
#>

[CmdletBinding(SupportsShouldProcess = $true, ConfirmImpact = 'Low')]
param(
	[switch]$Force
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# --- File sets (ILSpy) ------------------------------------------------------
$Dotfiles = @(
	'.gitignore', '.editorconfig', '.gitattributes', '.gitmodules',
	'.tgitconfig', '.vsconfig'
)

$AllowedExts = @(
	'.bat','.config','.cs','.csproj','.css','.filelist','.fs','.html','.il',
	'.ipynb','.js','.json','.less','.manifest','.md','.projitems','.props',
	'.ps1','.psd1','.ruleset','.shproj','.sln','.slnf','.svg','.template',
	'.tt', '.txt','.vb','.vsct','.vsixlangpack','.wxl','.xaml','.xml','.xshd','.yml'
)

$IncludeNoExt = $true  # include names like LICENSE

# --- Git checks / enumeration -----------------------------------------------
function Assert-InGitWorkTree {
	$inside = (& git rev-parse --is-inside-work-tree 2>$null).Trim()
	if ($LASTEXITCODE -ne 0 -or $inside -ne 'true') {
		throw 'Not in a Git work tree.'
	}
}

function Assert-CleanWorkingTree {
	if ($Force) { return }

	$status = & git status --porcelain -z
	if ($LASTEXITCODE -ne 0) { throw 'git status failed.' }

	if (-not [string]::IsNullOrEmpty($status)) {
		throw 'Working tree not clean. Commit/stash changes or use -Force.'
	}
}

function Get-GitFilesUnderPwd {
	Assert-InGitWorkTree

	$repoRoot = (& git rev-parse --show-toplevel).Trim()
	$pwdPath  = (Get-Location).Path

	$tracked = & git -C $repoRoot ls-files -z
	$others  = & git -C $repoRoot ls-files --others --exclude-standard -z

	$allRel = ("$tracked$others").Split(
		[char]0, [System.StringSplitOptions]::RemoveEmptyEntries)

	foreach ($relPath in $allRel) {
		$fullPath = Join-Path $repoRoot $relPath
		if ($fullPath.StartsWith($pwdPath,
				[System.StringComparison]::OrdinalIgnoreCase)) {
			if (Test-Path -LiteralPath $fullPath -PathType Leaf) {
				$fullPath
			}
		}
	}
}

# --- Probes -----------------------------------------------------------------
function Test-HasUtf8Bom {
	param([Parameter(Mandatory)][string]$Path)

	try {
		$stream = [System.IO.File]::Open($Path,'Open','Read','ReadWrite')
		try {
			if ($stream.Length -lt 3) { return $false }

			$header = [byte[]]::new(3)
			[void]$stream.Read($header,0,3)

			return ($header[0] -eq 0xEF -and
							$header[1] -eq 0xBB -and
							$header[2] -eq 0xBF)
		}
		finally {
			$stream.Dispose()
		}
	}
	catch { return $false }
}

function Test-ProbablyBinary {
	# Binary if the first 8 KiB contains any NUL byte.
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
		finally {
			$stream.Dispose()
		}
	}
	catch { return $false }
}

# --- Mutation ---------------------------------------------------------------
function Remove-Utf8BomInPlace {
	# Write the existing buffer from offset 3, no extra full-size allocation.
	param([Parameter(Mandatory)][string]$Path)

	$bytes = [System.IO.File]::ReadAllBytes($Path)
	if ($bytes.Length -lt 3) { return $false }

	if ($bytes[0] -ne 0xEF -or
			$bytes[1] -ne 0xBB -or
			$bytes[2] -ne 0xBF) {
		return $false
	}

	$stream = [System.IO.File]::Open($Path,'Create','Write','ReadWrite')
	try {
		$stream.Write($bytes, 3, $bytes.Length - 3)
		$stream.SetLength($bytes.Length - 3)
	}
	finally {
		$stream.Dispose()
	}

	return $true
}

# --- Main -------------------------------------------------------------------
Assert-InGitWorkTree
Assert-CleanWorkingTree

$allFiles = Get-GitFilesUnderPwd

$targets = $allFiles | % {
	$fileName = [IO.Path]::GetFileName($_)
	$ext      = [IO.Path]::GetExtension($fileName)

	$isDot    = $Dotfiles -contains $fileName
	$isNoExt  = -not $fileName.Contains('.')

	if ($isDot -or ($AllowedExts -contains $ext) -or
			($IncludeNoExt -and $isNoExt -and -not $isDot)) {
		$_
	}
}
| ? { Test-HasUtf8Bom $_ }
| ? { -not (Test-ProbablyBinary $_) }

$changed        = 0
$byExtension    = @{}
$dotfileChanges = 0

$targets | % {
	$relative = Resolve-Path -LiteralPath $_ -Relative

	if ($PSCmdlet.ShouldProcess($relative,'Strip UTF-8 BOM')) {
		if (Remove-Utf8BomInPlace -Path $_) {
			$changed++

			$fileName = [IO.Path]::GetFileName($_)
			if ($Dotfiles -contains $fileName) { $dotfileChanges++ }

			$ext = [IO.Path]::GetExtension($fileName)
			if (-not $byExtension.ContainsKey($ext)) { $byExtension[$ext] = 0 }
			$byExtension[$ext]++

			"stripped BOM: $relative"
		}
	}
}

"Done. Stripped BOM from $changed file(s)."

if ($byExtension.Keys.Count -gt 0) {
	""
	"By extension:"
	$byExtension.GetEnumerator() | Sort-Object Name | % {
		$key = if ([string]::IsNullOrEmpty($_.Name)) { '[noext]' } else { $_.Name }
		"  {0}: {1}" -f $key, $_.Value
	}
}

if ($dotfileChanges -gt 0) {
	"  [dotfiles]: $dotfileChanges"
}
