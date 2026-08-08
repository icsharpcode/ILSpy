; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
DSTG001 | DecompilerSyntaxTreeGenerator | Error | Slot kind must map to a single child type
DSTG002 | DecompilerSettingsGenerator | Error | [DecompilerSetting] target must be a partial instance bool property
DSTG003 | DecompilerSettingsGenerator | Error | Version-gated setting must not declare [Category]
DSTG004 | DecompilerSettingsGenerator | Error | Language version has no display category
