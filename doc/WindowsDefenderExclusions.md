# Windows Defender exclusions for test runs

Real-time protection scans every file the test suite writes and every process it
spawns. A full `ICSharpCode.Decompiler.Tests` run compiles thousands of fixture
assemblies and launches csc/vbc/ilasm/msbuild/test-runner child processes; while it
runs, `MsMpEng.exe` (Defender's scan engine) has been measured using 1-4 CPU cores
continuously, and each process start pays additional scan latency. Excluding the
folders below removes that overhead.

**Security tradeoff:** excluded folders are not scanned at all. Only add exclusions
on a development machine you control, and only for paths that contain code you
build yourself. If you use a third-party antivirus or backup suite with real-time
scanning (e.g. Acronis Active Protection), configure equivalent exclusions there.

## Folders to exclude

1. **The ILSpy repository clone** (e.g. `C:\src\ILSpy`). This covers everything the
   build and tests write inside the repo:
   - `bin\` and `obj\` of every project
   - compiled test fixtures placed next to their sources under
     `ICSharpCode.Decompiler.Tests\TestCases\`
   - the Roslyn toolsets, reference-assembly packs, and vswhere that
     `Tester.Initialize()` downloads under the test output directory
   - the `ILSpy-tests\` submodule, including the `*-decompiled` / `*-output`
     folders the roundtrip tests generate next to their inputs
2. **The test-assembly temp path, if you redirected it.** When
   `ICSharpCode.Decompiler.Tests\DecompilerTests.config.json` sets
   `TestsAssemblyTempPath`, compiled fixtures land there instead of inside the
   repo - exclude that folder as well.
3. **Optional: the NuGet package cache** (`%USERPROFILE%\.nuget\packages`). Only
   restore performance benefits; packages are signed and hash-verified by NuGet,
   but skip this one if you prefer scanned downloads.

The user temp folder (`%TEMP%`) also receives small diff files from failing
correctness tests, but excluding all of `%TEMP%` is a poor tradeoff - malware
routinely stages there. Leave it scanned.

## Adding the exclusions

Run PowerShell **as Administrator**, adjusting the paths to your clone:

```powershell
Add-MpPreference -ExclusionPath "C:\src\ILSpy"
# only if TestsAssemblyTempPath is configured:
Add-MpPreference -ExclusionPath "D:\ILSpyTestAssemblies"
# optional:
Add-MpPreference -ExclusionPath "$env:USERPROFILE\.nuget\packages"
```

## Verifying

```powershell
Get-MpPreference | Select-Object -ExpandProperty ExclusionPath
```

## Removing

```powershell
Remove-MpPreference -ExclusionPath "C:\src\ILSpy"
```
