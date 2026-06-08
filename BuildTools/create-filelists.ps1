$ErrorActionPreference = "Stop";

$Utf8NoBomEncoding = New-Object System.Text.UTF8Encoding $False

gci -Include *.vsix, *.msi -recurse | foreach ($_) {
	Write-Host $_.FullName
	$idx=-1;
	$body=$false;
	$outputFileName = ".\BuildTools\$($_.Name -replace '-\d+\.\d+\.\d+\.\d+', '').filelist";
	$lines = 7z l $_.FullName  | foreach {
		if ($idx -eq -1) {
			$idx = $_.IndexOf("Name");
		}
		$p = $body;
		if ($idx -gt 0) {
			$body = ($body -ne ($_ -match ' *-[ -]+'))
		}
		if ($p -and $body) {
			$_.Substring($idx)
		}
	} | sort
	[System.IO.File]::WriteAllLines($outputFileName, $lines, $Utf8NoBomEncoding)
}
