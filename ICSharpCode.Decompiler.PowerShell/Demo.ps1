$basePath = $PSScriptRoot
if ([string]::IsNullOrEmpty($basePath))
{
    $basePath = Split-Path -parent $psISE.CurrentFile.Fullpath
}

$modulePath = $basePath + '\bin\Debug\netstandard2.0\ICSharpCode.Decompiler.Powershell.dll'

Import-Module $modulePath
$version = Get-DecompilerVersion
Write-Output $version

# different test assemblies - it makes a difference wrt .deps.json so there are two netstandard tests here
$asm_netstdWithDepsJson = $basePath + '\bin\Debug\netstandard2.0\ICSharpCode.Decompiler.Powershell.dll'
$asm_netstd = $basePath + '\bin\Debug\netstandard2.0\ICSharpCode.Decompiler.dll'

$decompiler = Get-Decompiler $asm_netstdWithDepsJson

$classes = Get-DecompiledTypes $decompiler -Types class
$classes.Count

foreach ($c in $classes)
{
    Write-Output $c.FullName
}


Get-DecompiledSource $decompiler -TypeName ICSharpCode.Decompiler.PowerShell.GetDecompilerCmdlet

Get-DecompiledProject $decompiler -OutputPath .\decomptest