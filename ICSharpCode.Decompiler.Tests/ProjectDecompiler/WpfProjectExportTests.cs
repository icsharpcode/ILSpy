// Copyright (c) 2026 Siegfried Pammer
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.ProjectDecompiler;

/// <summary>
/// Exporting a WPF assembly has to produce a project the .NET SDK actually builds. The inputs are
/// compiled on the fly: what the export writes depends on the assembly references and the
/// versioning attributes of the module, so the tests need real modules carrying them, not stubs.
/// </summary>
[TestFixture]
public sealed class WpfProjectExportTests
{
	readonly List<MetadataFile> openedModules = new();
	string tempDirectory;
	string presentationFramework;
	string presentationCore;
	int assemblyCounter;

	[OneTimeSetUp]
	public void SetUp()
	{
		tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
		Directory.CreateDirectory(tempDirectory);

		// Stand-ins for the two WPF assemblies the exporter recognizes by name. They only have to
		// carry the right assembly name and the types the fixture assemblies derive from; the real
		// ones are not available on non-Windows hosts, which is exactly the situation that made the
		// export depend on the machine it ran on.
		presentationFramework = CompileTo("PresentationFramework.dll", "PresentationFramework", """
			namespace System.Windows
			{
				public class Application { }
				public class Window { }
			}
			""");
		presentationCore = CompileTo("PresentationCore.dll", "PresentationCore", """
			namespace System.Windows.Media
			{
				public class Brush { }
			}
			""");
	}

	[OneTimeTearDown]
	public void TearDown()
	{
		foreach (var module in openedModules)
			module.Dispose();
		if (Directory.Exists(tempDirectory))
			Directory.Delete(tempDirectory, recursive: true);
	}

	/// <summary>
	/// NETSDK1136: a .NET 5+ project that sets UseWPF or UseWindowsForms is rejected outright
	/// unless its target framework names the Windows platform.
	/// </summary>
	[Test]
	public void WpfTargetFrameworkKeepsThePlatformSuffix()
	{
		string project = WriteProjectFile(WpfApplication(targetFramework: ".NETCoreApp,Version=v10.0", targetPlatform: "Windows7.0"));

		using (Assert.EnterMultipleScope())
		{
			Assert.That(project, Does.Contain("<UseWPF>True</UseWPF>"));
			Assert.That(project, Does.Contain("<TargetFramework>net10.0-windows7.0</TargetFramework>"));
		}
	}

	/// <summary>
	/// The platform belongs to the assembly, not to WPF: a plain library built for Windows keeps
	/// its suffix as well, and one built for no particular platform does not grow one.
	/// </summary>
	[TestCase("Windows7.0", "net10.0-windows7.0")]
	[TestCase(null, "net10.0")]
	public void PlainLibraryFollowsItsTargetPlatformAttribute(string targetPlatform, string expectedMoniker)
	{
		string project = WriteProjectFile(Compile("PlainLibrary",
			AssemblyAttributes(".NETCoreApp,Version=v10.0", targetPlatform) + "public class C { }"));

		Assert.That(project, Does.Contain($"<TargetFramework>{expectedMoniker}</TargetFramework>"));
	}

	/// <summary>
	/// Assemblies built before the platform-suffixed monikers existed carry no TargetPlatform
	/// attribute; a WPF project still has to name Windows to build at all.
	/// </summary>
	[Test]
	public void WpfWithoutTargetPlatformAttributeStillNamesWindows()
	{
		string project = WriteProjectFile(WpfApplication(targetFramework: ".NETCoreApp,Version=v10.0", targetPlatform: null));

		Assert.That(project, Does.Contain("<TargetFramework>net10.0-windows</TargetFramework>"));
	}

	/// <summary>
	/// Platform suffixes only exist for .NET 5 and later. A .NET Framework moniker that grew one
	/// would no longer resolve to any target pack.
	/// </summary>
	[Test]
	public void NetFrameworkMonikerTakesNoPlatformSuffix()
	{
		string project = WriteProjectFile(WpfApplication(targetFramework: ".NETFramework,Version=v4.7.2", targetPlatform: "Windows7.0"));

		Assert.That(project, Does.Contain("<TargetFramework>net472</TargetFramework>"));
	}

	/// <summary>
	/// An assembly whose minimum supported OS version is lower than the version it targets says so
	/// through SupportedOSPlatform; without TargetPlatformMinVersion the exported project would
	/// silently raise its own floor to the target version.
	/// </summary>
	[Test]
	public void LowerSupportedOSPlatformBecomesTargetPlatformMinVersion()
	{
		string project = WriteProjectFile(Compile("VersionedLibrary",
			AssemblyAttributes(".NETCoreApp,Version=v10.0", "Windows10.0.19041.0", supportedOSPlatform: "Windows10.0.17763.0")
			+ "public class C { }"));

		using (Assert.EnterMultipleScope())
		{
			Assert.That(project, Does.Contain("<TargetFramework>net10.0-windows10.0.19041.0</TargetFramework>"));
			Assert.That(project, Does.Contain("<TargetPlatformMinVersion>10.0.17763.0</TargetPlatformMinVersion>"));
		}
	}

	/// <summary>
	/// NETSDK1137: Microsoft.NET.Sdk carries the Windows Desktop targets itself since .NET 5, and
	/// says so whenever a project still asks for the separate SDK.
	/// </summary>
	[TestCase(".NETCoreApp,Version=v10.0", "Microsoft.NET.Sdk")]
	[TestCase(".NETFramework,Version=v4.7.2", "Microsoft.NET.Sdk")]
	[TestCase(".NETCoreApp,Version=v3.1", "Microsoft.NET.Sdk.WindowsDesktop")]
	public void WindowsDesktopSdkOnlyForNetCore3(string targetFramework, string expectedSdk)
	{
		string project = WriteProjectFile(WpfApplication(targetFramework, targetPlatform: null));

		Assert.That(project, Does.Contain($@"<Project Sdk=""{expectedSdk}"">"));
	}

	/// <summary>
	/// UseWPF brings in the whole Windows Desktop framework reference. Listing one of its
	/// assemblies again is a duplicate reference (MSB3243), or a missing one (MSB3245) when the
	/// hint path does not survive the move to the machine that rebuilds the project.
	/// </summary>
	[Test]
	public void ReferencesSuppliedByUseWpfAreNotListed()
	{
		string project = WriteProjectFile(WpfApplication(targetFramework: ".NETCoreApp,Version=v10.0", targetPlatform: "Windows7.0"));

		using (Assert.EnterMultipleScope())
		{
			Assert.That(project, Does.Not.Contain("PresentationCore"));
			Assert.That(project, Does.Not.Contain("PresentationFramework"));
		}
	}

	/// <summary>
	/// The markup compiler generates the program entry point into the application definition, so
	/// the XAML file of the Application subclass is the one item that must not be a Page (#2253).
	/// </summary>
	[Test]
	public void ApplicationSubclassOfAnExecutableIsTheApplicationDefinition()
	{
		var (module, typeSystem) = Load(WpfApplication(targetFramework: ".NETCoreApp,Version=v10.0", targetPlatform: "Windows7.0"));

		using (Assert.EnterMultipleScope())
		{
			Assert.That(WholeProjectDecompiler.IsApplicationDefinition(FindType(typeSystem, "Fixture.App"), module), Is.True);
			Assert.That(WholeProjectDecompiler.IsApplicationDefinition(FindType(typeSystem, "Fixture.MainWindow"), module), Is.False,
				"a Window is a Page like any other");
		}
	}

	/// <summary>
	/// A library has no entry point for the markup compiler to generate a Main into; an Application
	/// subclass shipped inside one stays a Page.
	/// </summary>
	[Test]
	public void ApplicationSubclassOfALibraryIsNotTheApplicationDefinition()
	{
		var (module, typeSystem) = Load(Compile("WpfLibrary", """
			namespace Fixture
			{
				public class App : System.Windows.Application { }
			}
			""", OutputKind.DynamicallyLinkedLibrary, presentationFramework));

		Assert.That(WholeProjectDecompiler.IsApplicationDefinition(FindType(typeSystem, "Fixture.App"), module), Is.False);
	}

	static ITypeDefinition FindType(IDecompilerTypeSystem typeSystem, string fullTypeName)
	{
		var type = typeSystem.FindType(new FullTypeName(fullTypeName)).GetDefinition();
		Assert.That(type, Is.Not.Null, $"the fixture assembly is expected to contain {fullTypeName}");
		return type;
	}

	string WpfApplication(string targetFramework, string targetPlatform)
	{
		return Compile("WpfFixture", AssemblyAttributes(targetFramework, targetPlatform) + """
			namespace Fixture
			{
				public class App : System.Windows.Application { }
				public class MainWindow : System.Windows.Window
				{
					public System.Windows.Media.Brush Brush;
				}
				public static class Program
				{
					public static void Main() { }
				}
			}
			""", OutputKind.WindowsApplication, presentationFramework, presentationCore);
	}

	static string AssemblyAttributes(string targetFramework, string targetPlatform, string supportedOSPlatform = null)
	{
		var attributes = new StringBuilder();
		attributes.AppendLine($"[assembly: System.Runtime.Versioning.TargetFramework(\"{targetFramework}\")]");
		if (targetPlatform != null)
			attributes.AppendLine($"[assembly: System.Runtime.Versioning.TargetPlatform(\"{targetPlatform}\")]");
		if (supportedOSPlatform != null)
			attributes.AppendLine($"[assembly: System.Runtime.Versioning.SupportedOSPlatform(\"{supportedOSPlatform}\")]");
		return attributes.ToString();
	}

	/// <summary>
	/// Each compilation lands in a file of its own: the modules stay open until the fixture is torn
	/// down, so the same path must not be written twice.
	/// </summary>
	string Compile(string assemblyName, string source, OutputKind outputKind = OutputKind.DynamicallyLinkedLibrary, params string[] references)
	{
		string extension = outputKind == OutputKind.DynamicallyLinkedLibrary ? ".dll" : ".exe";
		string fileName = assemblyName + "." + Interlocked.Increment(ref assemblyCounter) + extension;
		return CompileTo(fileName, assemblyName, source, outputKind, references);
	}

	string CompileTo(string fileName, string assemblyName, string source, OutputKind outputKind = OutputKind.DynamicallyLinkedLibrary, params string[] references)
	{
		var compilation = CSharpCompilation.Create(assemblyName,
			new[] { CSharpSyntaxTree.ParseText(SourceText.From(source, Encoding.UTF8)) },
			RuntimeReferences.Concat(references.Select(r => MetadataReference.CreateFromFile(r))),
			new CSharpCompilationOptions(outputKind, deterministic: true));

		string fullPath = Path.Combine(tempDirectory, fileName);
		var result = compilation.Emit(fullPath);
		Assert.That(result.Success, Is.True, () => string.Join(Environment.NewLine, result.Diagnostics));
		return fullPath;
	}

	static IEnumerable<MetadataReference> RuntimeReferences
		=> ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))
			.Split(Path.PathSeparator)
			.Where(path => path.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
			.Select(path => MetadataReference.CreateFromFile(path));

	(MetadataFile Module, IDecompilerTypeSystem TypeSystem) Load(string fileName)
	{
		var module = OpenModule(fileName);
		return (module, new DecompilerTypeSystem(module, new UniversalAssemblyResolver(fileName, throwOnError: false, targetFramework: null)));
	}

	MetadataFile OpenModule(string fileName)
	{
		var module = new PEFile(fileName);
		openedModules.Add(module);
		return module;
	}

	string WriteProjectFile(string fileName)
	{
		StringWriter output = new();
		ProjectFileWriterSdkStyle.Default.Write(output, new TestProjectInfoProvider(tempDirectory), Array.Empty<ProjectItemInfo>(), OpenModule(fileName));
		return output.ToString();
	}

	sealed class TestProjectInfoProvider(string targetDirectory) : IProjectInfoProvider
	{
		public IAssemblyResolver AssemblyResolver { get; } = new UniversalAssemblyResolver(null, false, null);
		public IAssemblyReferenceClassifier AssemblyReferenceClassifier { get; } = new AssemblyReferenceClassifier();
		public CSharp.LanguageVersion LanguageVersion => CSharp.LanguageVersion.Latest;
		public bool CheckForOverflowUnderflow => false;
		public Guid ProjectGuid { get; } = Guid.NewGuid();
		public string TargetDirectory => targetDirectory;
		public string StrongNameKeyFile => null;
	}
}
