#region Using directives

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

#endregion

// This sets the default COM visibility of types in the assembly to invisible.
// If you need to expose a type to COM, use [ComVisible(true)] on that type.
[assembly: ComVisible(false)]

[assembly: AssemblyVersion(DecompilerVersionInfo.Major + "." + DecompilerVersionInfo.Minor + "." + DecompilerVersionInfo.Build + "." + DecompilerVersionInfo.Revision)]
[assembly: AssemblyInformationalVersion(DecompilerVersionInfo.FullVersionWithCommitHash)]

[assembly: SuppressMessage("Microsoft.Usage", "CA2243:AttributeStringLiteralsShouldParseCorrectly",
	Justification = "AssemblyInformationalVersion does not need to be a parsable version")]
