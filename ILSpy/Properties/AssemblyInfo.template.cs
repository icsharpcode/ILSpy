#region Using directives

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#endregion

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("ILSpy")]
[assembly: AssemblyDescription(".NET assembly inspector and decompiler")]
[assembly: AssemblyCompany("ic#code")]
[assembly: AssemblyProduct("ILSpy")]
[assembly: AssemblyCopyright("Copyright 2011-$INSERTYEAR$ AlphaSierraPapa for the SharpDevelop Team")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// This sets the default COM visibility of types in the assembly to invisible.
// If you need to expose a type to COM, use [ComVisible(true)] on that type.
[assembly: ComVisible(false)]

[assembly: AssemblyVersion(RevisionClass.Major + "." + RevisionClass.Minor + "." + RevisionClass.Build + "." + RevisionClass.Revision)]
[assembly: AssemblyInformationalVersion(RevisionClass.FullVersion + "-$INSERTSHORTCOMMITHASH$")]
[assembly: NeutralResourcesLanguage("en-US")]

[assembly: InternalsVisibleTo("ILSpy.AddIn, PublicKey=0024000004800000940000000602000000240000525341310004000001000100653c4a319be4f524972c3c5bba5fd243330f8e900287d9022d7821a63fd0086fd3801e3683dbe9897f2ecc44727023e9b40adcf180730af70c81c54476b3e5ba8b0f07f5132b2c3cc54347a2c1a9d64ebaaaf3cbffc1a18c427981e2a51d53d5ab02536b7550e732f795121c38a0abfdb38596353525d034baf9e6f1fd8ac4ac")]
[assembly: InternalsVisibleTo("ILSpy.Tests, PublicKey=00240000048000009400000006020000002400005253413100040000010001004dcf3979c4e902efa4dd2163a039701ed5822e6f1134d77737296abbb97bf0803083cfb2117b4f5446a217782f5c7c634f9fe1fc60b4c11d62c5b3d33545036706296d31903ddcf750875db38a8ac379512f51620bb948c94d0831125fbc5fe63707cbb93f48c1459c4d1749eb7ac5e681a2f0d6d7c60fa527a3c0b8f92b02bf")]

[assembly: SuppressMessage("Microsoft.Usage", "CA2243:AttributeStringLiteralsShouldParseCorrectly",
	Justification = "AssemblyInformationalVersion does not need to be a parsable version")]

internal static class RevisionClass
{
	public const string Major = "7";
	public const string Minor = "0";
	public const string Build = "0";
	public const string Revision = "$INSERTREVISION$";
	public const string VersionName = "preview2";

	public const string FullVersion = Major + "." + Minor + "." + Build + ".$INSERTREVISION$$INSERTBRANCHPOSTFIX$$INSERTVERSIONNAMEPOSTFIX$";
}
