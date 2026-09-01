// Copyright (c) 2025 Daniel Grunwald
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
using System.Resources;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.ProjectDecompiler;

[TestFixture]
public sealed class WholeProjectDecompilerTests
{
	[Test]
	public void UseNestedDirectoriesForNamespacesTrueWorks()
	{
		string targetDirectory = Path.Combine(Environment.CurrentDirectory, Path.GetRandomFileName());
		TestFriendlyProjectDecompiler decompiler = new(new UniversalAssemblyResolver(null, false, null));
		decompiler.Settings.UseNestedDirectoriesForNamespaces = true;
		decompiler.DecompileProject(new PEFile("ICSharpCode.Decompiler.dll"), targetDirectory);
		AssertDirectoryDoesntExist(targetDirectory);

		string projectDecompilerDirectory = Path.Combine(targetDirectory, "ICSharpCode", "Decompiler", "CSharp", "ProjectDecompiler");
		string projectDecompilerFile = Path.Combine(projectDecompilerDirectory, $"{nameof(WholeProjectDecompiler)}.cs");

		using (Assert.EnterMultipleScope())
		{
			Assert.That(decompiler.Files.ContainsKey(projectDecompilerFile), Is.True);
			Assert.That(decompiler.Directories.Contains(projectDecompilerDirectory), Is.True);
		}
	}

	[Test]
	public void UseNestedDirectoriesForNamespacesFalseWorks()
	{
		string targetDirectory = Path.Combine(Environment.CurrentDirectory, Path.GetRandomFileName());
		TestFriendlyProjectDecompiler decompiler = new(new UniversalAssemblyResolver(null, false, null));
		decompiler.Settings.UseNestedDirectoriesForNamespaces = false;
		decompiler.DecompileProject(new PEFile("ICSharpCode.Decompiler.dll"), targetDirectory);
		AssertDirectoryDoesntExist(targetDirectory);

		string projectDecompilerDirectory = Path.Combine(targetDirectory, "ICSharpCode.Decompiler.CSharp.ProjectDecompiler");
		string projectDecompilerFile = Path.Combine(projectDecompilerDirectory, $"{nameof(WholeProjectDecompiler)}.cs");

		using (Assert.EnterMultipleScope())
		{
			Assert.That(decompiler.Files.ContainsKey(projectDecompilerFile), Is.True);
			Assert.That(decompiler.Directories.Contains(projectDecompilerDirectory), Is.True);
		}
	}

	/// <summary>
	/// Everything an export can fail at - decompiling a source file, creating one, the assembly-info
	/// file, a resource - is reported and skipped; the export itself always runs to completion, so a
	/// single unsupported member cannot cost the user the whole project (issue #3510).
	/// </summary>
	[Test]
	public void FailuresDoNotAbortTheExport()
	{
		string targetDirectory = Path.Combine(Environment.CurrentDirectory, Path.GetRandomFileName());
		TestFriendlyProjectDecompiler decompiler = new(new UniversalAssemblyResolver(null, false, null));
		decompiler.ConfigureDecompiler = d => d.AstTransforms.Add(new ThrowingAstTransform(nameof(WholeProjectDecompiler)));
		decompiler.FailResourceEnumeration = true;
		decompiler.FailFileCreationFor = new[] { nameof(TargetServices) + ".cs", "AssemblyInfo.cs" };

		StringWriter projectFileWriter = new();
		decompiler.DecompileProject(new PEFile("ICSharpCode.Decompiler.dll"), targetDirectory, projectFileWriter);
		AssertDirectoryDoesntExist(targetDirectory);

		string failedFile = Path.Combine(targetDirectory, "ICSharpCode.Decompiler.CSharp.ProjectDecompiler", $"{nameof(WholeProjectDecompiler)}.cs");
		using (Assert.EnterMultipleScope())
		{
			Assert.That(decompiler.Errors.Select(e => e.InnerException?.Message), Is.EquivalentTo(new[] {
				ThrowingAstTransform.Failure,
				TestFriendlyProjectDecompiler.ResourceFailure,
				TestFriendlyProjectDecompiler.FileCreationFailure + nameof(TargetServices) + ".cs",
				TestFriendlyProjectDecompiler.FileCreationFailure + "AssemblyInfo.cs",
			}));
			Assert.That(decompiler.Files[failedFile].ToString(), Does.Contain(ThrowingAstTransform.Failure),
				"the error text takes the place of the file's contents");
			Assert.That(decompiler.Files, Has.Count.GreaterThan(100), "all other files are still written");
			Assert.That(projectFileWriter.ToString(), Does.Contain("<Project"), "the project file is still written");
		}
	}

	/// <summary>
	/// A resource that cannot be written must cost that resource alone. Recovering around the
	/// enumeration cannot do this - an iterator is finished once it throws - so the export has to
	/// recover per resource, and this pins that.
	/// </summary>
	[Test]
	public void OneFailingResourceDoesNotDropTheOthers()
	{
		string targetDirectory = Path.Combine(Environment.CurrentDirectory, Path.GetRandomFileName());
		TestFriendlyProjectDecompiler decompiler = new(new UniversalAssemblyResolver(null, false, null));
		decompiler.FailResourceWriting = true;

		StringWriter projectFileWriter = new();
		// Two embedded .resources containers and nothing else, so both go through WriteResourceToFile
		// and the test never touches the disk.
		decompiler.DecompileProject(new PEFile("Microsoft.DiaSymReader.Converter.Xml.dll"), targetDirectory, projectFileWriter);
		AssertDirectoryDoesntExist(targetDirectory);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(decompiler.WrittenResources, Has.Count.EqualTo(2),
				"the resource after the failing one is still written");
			Assert.That(decompiler.Errors.Select(e => e.InnerException?.Message),
				Is.EqualTo(new[] { TestFriendlyProjectDecompiler.ResourceFailure }));
		}
	}

	/// <summary>
	/// WPF's build tasks key every Page and Resource item in "&lt;AssemblyName&gt;.g.resources" by its
	/// relative path, lower-cased and URI-escaped, so a folder named "My Folder" arrives as
	/// "my%20folder". The escapes have to be decoded before the name becomes a file name -
	/// sanitizing them instead produces a directory called "my-20folder" (issue #3315). Only the
	/// WPF-generated containers are escaped; a percent sign in any other .resources file is part
	/// of the name.
	/// </summary>
	[Test]
	public void WpfResourceIdsAreUnescapedBeforeSanitizing()
	{
		string targetDirectory = Path.Combine(Environment.CurrentDirectory, Path.GetRandomFileName());
		TestFriendlyProjectDecompiler decompiler = new(new UniversalAssemblyResolver(null, false, null));
		decompiler.CaptureResources = true;

		using var assembly = CreateAssemblyWithResources(
			("Test.g.resources", "my%20folder/window.baml"),
			// satellite assemblies hold the localized pages in "<AssemblyName>.g.<culture>.resources"
			("Test.g.de-DE.resources", "my%20folder/other.baml"),
			("Test.resources", "100%25off/window.baml"));
		decompiler.DecompileProject(new PEFile("Test.dll", assembly), targetDirectory, new StringWriter());
		AssertDirectoryDoesntExist(targetDirectory);

		Assert.That(decompiler.WrittenResources, Is.EquivalentTo(new[] {
			Path.Combine("my-folder", "window.baml"),
			Path.Combine("my-folder", "other.baml"),
			// not a WPF container, so "%25" is three characters of the name and not an escaped '%'
			Path.Combine("100-25off", "window.baml"),
		}));
	}

	/// <summary>
	/// Sanitizing is not injective: "a+b/logo.png", "a&amp;b/logo.png" and "a%23b/logo.png" all end up
	/// as "a-b/logo.png", and an assembly can be crafted to aim any number of entries at one name.
	/// Colliding entries used to overwrite each other silently, costing the export their contents
	/// with nothing written to the error list. Each one now gets a file of its own; the item still
	/// carries the true name, so all of them survive a rebuild.
	/// </summary>
	[Test]
	public void ResourcesThatSanitizeToTheSameNameGetSeparateFiles()
	{
		string targetDirectory = Path.Combine(Environment.CurrentDirectory, Path.GetRandomFileName());
		TestFriendlyProjectDecompiler decompiler = new(new UniversalAssemblyResolver(null, false, null));
		decompiler.CaptureResources = true;

		using var assembly = CreateAssemblyWithResources(
			("Test.g.resources", "a+b/logo.png"),
			("Test.g.resources", "a&b/logo.png"),
			("Test.g.resources", "a%23b/logo.png"),
			// the same name in a second container is a different resource and needs its own file too
			("Test.g.de-DE.resources", "a+b/logo.png"));
		decompiler.DecompileProject(new PEFile("Test.dll", assembly), targetDirectory, new StringWriter());
		AssertDirectoryDoesntExist(targetDirectory);

		// Entries come back in the order the .resources format stores them, not the order they were
		// written in, so which entry draws which suffix is not fixed - only that the four of them
		// occupy four files and keep their four names.
		using (Assert.EnterMultipleScope())
		{
			Assert.That(decompiler.WrittenResources, Is.EquivalentTo(new[] {
				Path.Combine("a-b", "logo.png"),
				Path.Combine("a-b", "logo_2.png"),
				Path.Combine("a-b", "logo_3.png"),
				Path.Combine("a-b", "logo_4.png"),
			}));
			Assert.That(decompiler.ResourceItems.Select(i => i.AdditionalProperties?["LogicalName"]),
				Is.EquivalentTo(new[] { "a+b/logo.png", "a&b/logo.png", "a#b/logo.png", "a+b/logo.png" }));
		}
	}

	/// <summary>
	/// A rebuilt WPF project only resolves its own pack URIs when every entry of
	/// "&lt;AssemblyName&gt;.g.resources" comes back under the resource ID it had before. The file on
	/// disk cannot carry that ID - it is sanitized, and the ID is escaped - so each item pins it
	/// with a LogicalName holding the decoded name, which is what the WPF build tasks escape again.
	/// Entries no handler claimed have to be Resource items as well: as EmbeddedResource they would
	/// rebuild into a manifest resource of their own instead of landing in ".g.resources".
	/// </summary>
	[Test]
	public void WpfResourceEntriesCarryTheirOriginalResourceIdAsLogicalName()
	{
		string targetDirectory = Path.Combine(Environment.CurrentDirectory, Path.GetRandomFileName());
		TestFriendlyProjectDecompiler decompiler = new(new UniversalAssemblyResolver(null, false, null));
		decompiler.CaptureResources = true;

		using var assembly = CreateAssemblyWithResources(
			("Test.g.resources", "my%20folder/window.baml"),
			("Test.g.resources", "resource%20test/logo.png"),
			("Test.resources", "plain%25folder/logo.png"));
		decompiler.DecompileProject(new PEFile("Test.dll", assembly), targetDirectory, new StringWriter());
		AssertDirectoryDoesntExist(targetDirectory);

		Assert.That(decompiler.ResourceItems.Select(i => (i.ItemType, i.FileName, i.AdditionalProperties?["LogicalName"])),
			Is.EquivalentTo(new[] {
				// the build re-derives the .baml extension from the Page item type
				("Page", Path.Combine("my-folder", "window.xaml"), "my folder/window.xaml"),
				("Resource", Path.Combine("resource-test", "logo.png"), "resource test/logo.png"),
				// not a WPF container, so the name is neither escaped nor a Resource item
				("EmbeddedResource", Path.Combine("plain-25folder", "logo.png"), "plain%25folder/logo.png"),
			}));
	}

	/// <summary>
	/// Emits an assembly carrying one embedded .resources container per distinct container name in
	/// <paramref name="resources"/>, each holding the stream-valued entries named for it. Streams
	/// are what makes the export write the entries out as individual files.
	/// </summary>
	static Stream CreateAssemblyWithResources(params (string ContainerName, string EntryName)[] resources)
	{
		var compilation = CSharpCompilation.Create("Test",
			new[] { CSharpSyntaxTree.ParseText("[assembly: System.Runtime.Versioning.TargetFramework(\".NETCoreApp,Version=v8.0\")] class C { }") },
			new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
			new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

		MemoryStream assembly = new();
		var result = compilation.Emit(assembly, manifestResources: resources.GroupBy(r => r.ContainerName).Select(container => {
			MemoryStream contents = new();
			using (ResourceWriter writer = new(contents))
			{
				foreach (var entry in container)
				{
					writer.AddResource(entry.EntryName, new MemoryStream(new byte[] { 1, 2, 3 }));
				}
			}
			byte[] bytes = contents.ToArray();
			return new ResourceDescription(container.Key, () => new MemoryStream(bytes), isPublic: true);
		}).ToArray());
		Assert.That(result.Success, Is.True, () => string.Join(Environment.NewLine, result.Diagnostics));
		assembly.Position = 0;
		return assembly;
	}

	sealed class ThrowingAstTransform(string typeName) : IAstTransform
	{
		public const string Failure = "Simulated AST transform failure";

		public void Run(AstNode rootNode, TransformContext context)
		{
			if (rootNode.Descendants.OfType<TypeDeclaration>().Any(td => td.Name == typeName))
				throw new InvalidOperationException(Failure);
		}
	}

	static void AssertDirectoryDoesntExist(string directory)
	{
		if (Directory.Exists(directory))
		{
			Directory.Delete(directory, recursive: true);
			Assert.Fail("Directory should not have been created.");
		}
	}

	sealed class TestFriendlyProjectDecompiler(IAssemblyResolver assemblyResolver) : WholeProjectDecompiler(assemblyResolver)
	{
		public Dictionary<string, StringWriter> Files { get; } = [];
		public HashSet<string> Directories { get; } = [];
		public Action<CSharpDecompiler>? ConfigureDecompiler { get; set; }

		protected override CSharpDecompiler CreateDecompiler(DecompilerTypeSystem ts)
		{
			var decompiler = base.CreateDecompiler(ts);
			ConfigureDecompiler?.Invoke(decompiler);
			return decompiler;
		}

		protected override TextWriter CreateFile(string path)
		{
			if (FailFileCreationFor.Any(name => path.EndsWith(name, StringComparison.Ordinal)))
				throw new IOException(FileCreationFailure + Path.GetFileName(path));
			StringWriter writer = new();
			lock (Files)
			{
				Files[path] = writer;
			}
			return writer;
		}

		protected override void CreateDirectory(string path)
		{
			lock (Directories)
			{
				Directories.Add(path);
			}
		}

		protected override IEnumerable<ProjectItemInfo> WriteMiscellaneousFilesInProject(PEFile module) => [];

		public const string ResourceFailure = "Simulated resource failure";
		public const string FileCreationFailure = "Simulated file creation failure: ";

		public bool FailResourceEnumeration { get; set; }

		public bool FailResourceWriting { get; set; }

		public string[] FailFileCreationFor { get; set; } = Array.Empty<string>();

		public List<string> WrittenResources { get; } = [];

		public List<ProjectItemInfo> ResourceItems { get; } = [];

		// Resources are skipped unless a test asks for them, so the tests that only care about
		// source files neither touch the disk nor pay for decoding them.
		public bool CaptureResources { get; set; }

		protected override IEnumerable<ProjectItemInfo> WriteResourceFilesInProject(MetadataFile module)
		{
			if (FailResourceWriting || CaptureResources)
			{
				ResourceItems.AddRange(base.WriteResourceFilesInProject(module));
				return ResourceItems;
			}
			return FailResourceEnumeration
				? Enumerable.Range(0, 1).Select<int, ProjectItemInfo>(_ => throw new InvalidOperationException(ResourceFailure))
				: [];
		}

		// Fails on the first resource only, so the test can tell "recovered per resource" from
		// "gave up on the rest of them".
		protected override IEnumerable<ProjectItemInfo> WriteResourceToFile(string fileName, string resourceName, Stream entryStream)
		{
			if (FailResourceWriting && WrittenResources.Count == 0)
			{
				WrittenResources.Add(fileName);
				throw new InvalidOperationException(ResourceFailure);
			}
			WrittenResources.Add(fileName);
			// Stands in for the BAML resource-file handlers the real hosts plug in: a .baml entry
			// is decompiled into a .xaml file referenced by a <Page> item, and those handlers
			// attach no LogicalName. Everything else keeps the base class' behaviour of naming the
			// resource entry as it is stored in the assembly.
			return fileName.EndsWith(".baml", StringComparison.OrdinalIgnoreCase)
				? new[] { new ProjectItemInfo("Page", Path.ChangeExtension(fileName, ".xaml")) }
				: new[] { new ProjectItemInfo("EmbeddedResource", fileName).With("LogicalName", resourceName) };
		}
	}
}
