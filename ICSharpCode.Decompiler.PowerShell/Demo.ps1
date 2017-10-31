Import-Module .\bin\Debug\netstandard2.0\ICSharpCode.Decompiler.PSCore.dll
$decompiler = Get-Decompiler .\bin\Debug\netstandard2.0\ICSharpCode.Decompiler.PSCore.dll

$classes = Get-DecompiledTypes $decompiler -Types class
$classes.Count

foreach ($c in $classes)
{
    Write-Output $c.FullName
}


Get-DecompiledSource $decompiler -TypeName ICSharpCode.Decompiler.PSCore.GetDecompilerCmdlet

Get-DecompiledProject $decompiler -OutputPath .\decomptest