$ErrorActionPreference = "Stop"

$baseCommit = "d779383cb85003d6dabeb976f0845631e07bf463";
$baseCommitRev = 1;

# make sure this list matches artifacts-only branches list in azure-pipelines.yml!
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
if ($masterBranches -contains $env:BUILD_SOURCEBRANCHNAME) {
	$branch = "";
} else {
	$branch = "-$env:BUILD_SOURCEBRANCHNAME";
}
if ($env:SYSTEM_PULLREQUEST_PULLREQUESTNUMBER) {
	$suffix = "-pr$env:SYSTEM_PULLREQUEST_PULLREQUESTNUMBER";
} else {
	$suffix = "";
}

$revision = [Int32]::Parse((git rev-list --count "$baseCommit..HEAD")) + $baseCommitRev;

$newVersion="$major.$minor.$build.$revision";
$env:ILSPY_VERSION_NUMBER="$newVersion$branch$versionName$suffix";
Write-Host "##vso[build.updatebuildnumber]$newVersion$branch$versionName$suffix";
Write-Host "new version: $newVersion$branch$versionName$suffix";