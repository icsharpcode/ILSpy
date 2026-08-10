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

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

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

		protected override IEnumerable<ProjectItemInfo> WriteResourceFilesInProject(MetadataFile module)
		{
			if (FailResourceWriting)
				return base.WriteResourceFilesInProject(module);
			return FailResourceEnumeration
				? Enumerable.Range(0, 1).Select<int, ProjectItemInfo>(_ => throw new InvalidOperationException(ResourceFailure))
				: [];
		}

		// Fails on the first resource only, so the test can tell "recovered per resource" from
		// "gave up on the rest of them".
		protected override IEnumerable<ProjectItemInfo> WriteResourceToFile(string fileName, string resourceName, Stream entryStream)
		{
			if (WrittenResources.Count == 0)
			{
				WrittenResources.Add(fileName);
				throw new InvalidOperationException(ResourceFailure);
			}
			WrittenResources.Add(fileName);
			return new[] { new ProjectItemInfo("EmbeddedResource", fileName) };
		}
	}
}
