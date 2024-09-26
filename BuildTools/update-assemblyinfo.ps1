﻿if (-not ($PSVersionTable.PSCompatibleVersions -contains "5.0")) {
    Write-Error "This script requires at least powershell version 5.0!";
    return 255;
}

$ErrorActionPreference = "Stop"

$baseCommit = "d779383cb85003d6dabeb976f0845631e07bf463";
$baseCommitRev = 1;

# make sure this matches artifacts-only branches list in appveyor.yml!
$masterBranches = '^(master|release/.+)$';

$decompilerVersionInfoTemplateFile = "ICSharpCode.Decompiler/Properties/DecompilerVersionInfo.template.cs";

function Test-File([string]$filename) {
    return [System.IO.File]::Exists((Join-Path (Get-Location) $filename));
}

function Test-Dir([string]$name) {
    return [System.IO.Directory]::Exists((Join-Path (Get-Location) $name));
}

function Find-Git() {
	try {
		$executable = (get-command git).Path;
		return $executable -ne $null;
	} catch {
		#git not found in path, continue;
	}
	#we're on Windows
	if ($env:PROGRAMFILES -ne $null) {
		#hack for x86 powershell used by default (yuck!)
		if (${env:PROGRAMFILES(X86)} -eq ${env:PROGRAMFILES}) {
			$env:PROGRAMFILES = $env:PROGRAMFILES.Substring(0, $env:PROGRAMFILES.Length - 6);
		}
		#try to add git to path
		if ([System.IO.Directory]::Exists("$env:PROGRAMFILES\git\cmd\")) {
			$env:PATH = "$env:PATH;$env:PROGRAMFILES\git\cmd\";
			return $true;
		}
	}
	return $false;
}

function No-Git() {
    return -not (((Test-Dir ".git") -or (Test-File ".git")) -and (Find-Git));
}

function gitVersion() {
    if (No-Git) {
        return 0;
    }
    try {
        return [Int32]::Parse((git rev-list --count "$baseCommit..HEAD")) + $baseCommitRev;
    } catch {
        return 0;
    }
}

function gitCommitHash() {
    if (No-Git) {
        return "0000000000000000000000000000000000000000";
    }
    try {
        return (git rev-list --max-count 1 HEAD);
    } catch {
        return "0000000000000000000000000000000000000000";
    }
}

function gitShortCommitHash() {
    if (No-Git) {
        return "00000000";
    }
    try {
        return (git rev-parse --short=8 (git rev-list --max-count 1 HEAD));
    } catch {
        return "00000000";
    }	
}

function gitBranch() {
    if (No-Git) {
        return "no-branch";
    }

	if ($env:APPVEYOR_REPO_BRANCH -ne $null) {
        return $env:APPVEYOR_REPO_BRANCH;
    } elseif ($env:BUILD_SOURCEBRANCHNAME -ne $null) {
        return $env:BUILD_SOURCEBRANCHNAME;
    } else {
        return ((git branch --no-color).Split([System.Environment]::NewLine) | where { $_ -match "^\* " } | select -First 1).Substring(2);
    }
}

$templateFiles = (
    @{Input=$decompilerVersionInfoTemplateFile; Output="ICSharpCode.Decompiler/Properties/DecompilerVersionInfo.cs"},
    @{Input="ILSpy.AddIn/source.extension.vsixmanifest.template"; Output = "ILSpy.AddIn/source.extension.vsixmanifest"},
    @{Input="ILSpy.AddIn.VS2022/source.extension.vsixmanifest.template"; Output = "ILSpy.AddIn.VS2022/source.extension.vsixmanifest"}
);

[string]$mutexId = "ILSpyUpdateAssemblyInfo" + (Get-Location).ToString().GetHashCode();
Write-Host $mutexId;
[bool]$createdNew = $false;
$mutex = New-Object System.Threading.Mutex($true, $mutexId, [ref]$createdNew);
try {
    if (-not $createdNew) {
        try {
		    $mutex.WaitOne(10000);
	    } catch [System.Threading.AbandonedMutexException] {
	    }
        return 0;
    }

    if (-not (Test-File "ILSpy.sln")) {
        Write-Error "Working directory must be the ILSpy repo root!";
        return 2;
    }

    $versionParts = @{};
    Get-Content $decompilerVersionInfoTemplateFile | where { $_ -match 'string (\w+) = "?(\w+)"?;' } | foreach { $versionParts.Add($Matches[1], $Matches[2]) }

    $major = $versionParts.Major;
    $minor = $versionParts.Minor;
    $build = $versionParts.Build;
    $versionName = $versionParts.VersionName;
    $revision = gitVersion;
    $branchName = gitBranch;
    $gitCommitHash = gitCommitHash;
	$gitShortCommitHash = gitShortCommitHash;

    if ($branchName -match $masterBranches) {
        $postfixBranchName = "";
    } else {
        $postfixBranchName = "-$branchName";
	}

    if ($versionName -eq "null") {
        $versionName = "";
        $postfixVersionName = "";
    } else {
        $postfixVersionName = "-$versionName";
    }
	
	$buildConfig = $args[0].ToString().ToLower();
	if ($buildConfig -eq "release") {
		$buildConfig = "";
	} else {
		$buildConfig = "-" + $buildConfig;
	}

    $fullVersionNumber = "$major.$minor.$build.$revision";
    if ((-not (Test-File "VERSION")) -or (((Get-Content "VERSION") -Join [System.Environment]::NewLine) -ne "$fullVersionNumber$postfixVersionName")) {
        "$fullVersionNumber$postfixVersionName" | Out-File -Encoding utf8 -NoNewLine "VERSION";
    }
    
    foreach ($file in $templateFiles) {
        [string]$in = (Get-Content $file.Input) -Join [System.Environment]::NewLine;

		$out = $in.Replace('$INSERTVERSION$', $fullVersionNumber);
		$out = $out.Replace('$INSERTMAJORVERSION$', $major);
		$out = $out.Replace('$INSERTREVISION$', $revision);
		$out = $out.Replace('$INSERTCOMMITHASH$', $gitCommitHash);
		$out = $out.Replace('$INSERTSHORTCOMMITHASH$', $gitShortCommitHash);
		$out = $out.Replace('$INSERTDATE$', [System.DateTime]::Now.ToString("MM/dd/yyyy"));
		$out = $out.Replace('$INSERTYEAR$', [System.DateTime]::Now.Year.ToString());
		$out = $out.Replace('$INSERTBRANCHNAME$', $branchName);
        $out = $out.Replace('$INSERTBRANCHPOSTFIX$', $postfixBranchName);
		$out = $out.Replace('$INSERTVERSIONNAME$', $versionName);
        $out = $out.Replace('$INSERTVERSIONNAMEPOSTFIX$', $postfixVersionName);
        $out = $out.Replace('$INSERTBUILDCONFIG$', $buildConfig);

        if ((-not (Test-File $file.Output)) -or (((Get-Content $file.Output) -Join [System.Environment]::NewLine) -ne $out)) {
            $out | Out-File -Encoding utf8 $file.Output;
        }
    }
	
} finally {
    $mutex.ReleaseMutex();
    $mutex.Close();
}