$ErrorActionPreference = "Stop"

$baseCommit = "d779383cb85003d6dabeb976f0845631e07bf463";
$baseCommitRev = 1;

$globalAssemblyInfoTemplateFile = "ILSpy/Properties/AssemblyInfo.template.cs";

function Test-File([string]$filename) {
    return [System.IO.File]::Exists( (Join-Path (Get-Location) $filename) );
}

function Test-Dir([string]$name) {
    return [System.IO.Directory]::Exists( (Join-Path (Get-Location) $name) );
}

function Find-Git() {
	if ($env:PATH.Contains("git\cmd")) {
		return $true;
	}
	if (Test-Dir "$env:PROGRAMFILES\git\cmd\") {
		$env:PATH = $env:PATH + ";$env:PROGRAMFILES\git\cmd\";
		return $true;
	}
	return $false;
}

function gitVersion() {
    if (-not ((Test-Dir ".git") -and (Find-Git))) {
        return 9999;
    }
    return [Int32]::Parse((git rev-list --count "$baseCommit..HEAD")) + $baseCommitRev;
}

function gitCommitHash() {
    if (-not ((Test-Dir ".git") -and (Find-Git))) {
        return "0000000000000000000000000000000000000000";
    }
    return (git rev-list "$baseCommit..HEAD") | Select -First 1;
}

function gitBranch() {
    if (-not ((Test-Dir ".git") -and (Find-Git))) {
        return "no-branch";
    }

    if ($env:APPVEYOR_REPO_BRANCH -ne $null) {
        return $env:APPVEYOR_REPO_BRANCH;
    } else {
        return ((git branch --no-color).Split([System.Environment]::NewLine) | where { $_ -match "^\* " } | select -First 1).Substring(2);
    }
}

$templateFiles = (
	@{Input=$globalAssemblyInfoTemplateFile; Output="ILSpy/Properties/AssemblyInfo.cs"},
	@{Input="ICSharpCode.Decompiler/Properties/AssemblyInfo.template.cs"; Output="ICSharpCode.Decompiler/Properties/AssemblyInfo.cs"},
	@{Input="ICSharpCode.Decompiler/ICSharpCode.Decompiler.nuspec.template"; Output="ICSharpCode.Decompiler/ICSharpCode.Decompiler.nuspec"},
	@{Input="ILSpy/Properties/app.config.template"; Output = "ILSpy/app.config"}
);
[string]$mutexId = "ILSpyUpdateAssemblyInfo" + $PSScriptRoot.GetHashCode();
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
        Write-Host "Working directory must be the ILSpy repo root!";
        return 2;
    }

    $versionParts = @{};
    Get-Content $globalAssemblyInfoTemplateFile | where { $_ -match 'string (\w+) = "?(\w+)"?;' } | foreach { $versionParts.Add($Matches[1], $Matches[2]) }

    $major = $versionParts.Major;
    $minor = $versionParts.Minor;
    $build = $versionParts.Build;
    $versionName = $versionParts.VersionName;
    $revision = gitVersion;
    $branchName = gitBranch;
    $gitCommitHash = gitCommitHash;

    $postfixBranchName = "";
    if ($branchName -ne "master") {
        $postfixBranchName = "-$branchName";
    }

    if ($versionName -eq "null") {
        $versionName = "";
        $postfixVersionName = "";
    } else {
        $postfixVersionName = "-$versionName";
    }

    $fullVersionNumber = "$major.$minor.$build.$revision";
    
    foreach ($file in $templateFiles) {
        [string]$in = (Get-Content $file.Input) -Join [System.Environment]::NewLine;

		$out = $in.Replace('$INSERTVERSION$', $fullVersionNumber);
		$out = $out.Replace('$INSERTMAJORVERSION$', $major);
		$out = $out.Replace('$INSERTREVISION$', $revision);
		$out = $out.Replace('$INSERTCOMMITHASH$', $gitCommitHash);
		$out = $out.Replace('$INSERTSHORTCOMMITHASH$', $gitCommitHash.Substring(0, 8));
		$out = $out.Replace('$INSERTDATE$', [System.DateTime]::Now.ToString("MM/dd/yyyy"));
		$out = $out.Replace('$INSERTYEAR$', [System.DateTime]::Now.Year.ToString());
		$out = $out.Replace('$INSERTBRANCHNAME$', $branchName);
        $out = $out.Replace('$INSERTBRANCHPOSTFIX$', $postfixBranchName);
		$out = $out.Replace('$INSERTVERSIONNAME$', $versionName);
        $out = $out.Replace('$INSERTVERSIONNAMEPOSTFIX$', $postfixVersionName);

        if (((Get-Content $file.Input) -Join [System.Environment]::NewLine) -ne $out) {
            $out | Out-File -Encoding utf8 $file.Output;
        }
    }
} finally {
    $mutex.ReleaseMutex();
    $mutex.Dispose();
}