$ErrorActionPreference = "Stop"

$baseCommit = "d779383cb85003d6dabeb976f0845631e07bf463";
$baseCommitRev = 1;

$globalAssemblyInfoTemplateFile = "ILSpy/Properties/AssemblyInfo.template.cs";

$versionParts = @{};
Get-Content $globalAssemblyInfoTemplateFile | where { $_ -match 'string (\w+) = "?(\w+)"?;' } | foreach { $versionParts.Add($Matches[1], $Matches[2]) }

$major = $versionParts.Major;
$minor = $versionParts.Minor;
$build = $versionParts.Build;
$versionName = $versionParts.VersionName;

if ($versionName -ne "null") {
    $versionName = "-$versionName";
}
if ($env:APPVEYOR_REPO_BRANCH -ne 'master') {
	$branch = "-$env:APPVEYOR_REPO_BRANCH";
} else {
	$branch = "";
}

$revision = [Int32]::Parse((git rev-list --count "$baseCommit..HEAD")) + 1;

$newVersion="$majorVersion.$minorVersion.$build.$revision";
$env:appveyor_build_version="$newVersion$branch$versionName";
appveyor UpdateBuild -Version "$newVersion$branch$versionName";