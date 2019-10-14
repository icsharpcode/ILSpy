$ErrorActionPreference = "Stop"

$baseCommit = "d779383cb85003d6dabeb976f0845631e07bf463";
$baseCommitRev = 1;

# make sure this list matches artifacts-only branches list in appveyor.yml!
$masterBranches = @("master", "5.0.x");

$globalAssemblyInfoTemplateFile = "ILSpy/Properties/AssemblyInfo.template.cs";

$versionParts = @{};
Get-Content $globalAssemblyInfoTemplateFile | where { $_ -match 'string (\w+) = "?(\w+)"?;' } | foreach { $versionParts.Add($Matches[1], $Matches[2]) }

$major = $versionParts.Major;
$minor = $versionParts.Minor;
$build = $versionParts.Build;
$versionName = $versionParts.VersionName;

if ($versionName -ne "null") {
    $versionName = "-$versionName";
} else {
    $versionName = "";
}
if ($masterBranches -contains $env:APPVEYOR_REPO_BRANCH) {
	$branch = "";
} else {
	$branch = "-$env:APPVEYOR_REPO_BRANCH";
}
if ($env:APPVEYOR_PULL_REQUEST_NUMBER) {
	$suffix = "-pr$env:APPVEYOR_PULL_REQUEST_NUMBER";
} else {
	$suffix = "";
}

$revision = [Int32]::Parse((git rev-list --count "$baseCommit..HEAD")) + $baseCommitRev;

$newVersion="$major.$minor.$build.$revision";
$env:APPVEYOR_BUILD_VERSION="$newVersion$branch$versionName$suffix";
$env:ILSPY_VERSION_NUMBER="$newVersion$branch$versionName$suffix";
appveyor UpdateBuild -Version "$newVersion$branch$versionName$suffix";
Write-Host "new version: $newVersion$branch$versionName$suffix";