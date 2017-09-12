$majorVersion = 3;
$minorVersion = 0;
$revision = 0;
$baseCommitHash = "d779383cb85003d6dabeb976f0845631e07bf463";

if ($env:APPVEYOR_REPO_BRANCH -ne 'master') {
	$branch = "-$env:APPVEYOR_REPO_BRANCH";
} else {
	$branch = "";
}

$build = git rev-list --count "$baseCommitHash..HEAD";

$newVersion="$majorVersion.$minorVersion.$revision.$build";
$env:ilspy_version_number=$newVersion;
$env:appveyor_build_version="$newVersion$branch";
appveyor UpdateBuild -Version "$newVersion$branch";