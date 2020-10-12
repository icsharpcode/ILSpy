$ErrorActionPreference = "Stop"

$baseCommit = "e17b4bfedf4dc747b105396224cd726bdca500ad";
$baseCommitRev = 1;

# make sure this matches artifacts-only branches list in appveyor.yml!
$masterBranches = '^(master|release/.+)$';

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
if ($env:APPVEYOR_REPO_BRANCH -match $masterBranches) {
	$branch = "";
} else {
	$branch = "-$env:APPVEYOR_REPO_BRANCH";
}

$revision = [Int32]::Parse((git rev-list --count "$baseCommit..HEAD")) + $baseCommitRev;

$newVersion="$major.$minor.$build.$revision";
$env:appveyor_build_version="$newVersion$branch$versionName";
appveyor UpdateBuild -Version "$newVersion$branch$versionName";