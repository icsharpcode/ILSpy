$basePath = $PSScriptRoot
if ([string]::IsNullOrEmpty($basePath))
{
    $basePath = Split-Path -parent $psISE.CurrentFile.Fullpath
}

$modulePath = $basePath + '\bin\Debug\netstandard2.0\ICSharpCode.Decompiler.Powershell.dll'

Import-Module $modulePath
$decompiler = Get-Decompiler $modulePath

$classes = Get-DecompiledTypes $decompiler -Types class
$classes.Count

foreach ($c in $classes)
{
    Write-Output $c.FullName
}


Get-DecompiledSource $decompiler -TypeName ICSharpCode.Decompiler.Powershell.GetDecompilerCmdlet

Get-DecompiledProject $decompiler -OutputPath .\decomptest