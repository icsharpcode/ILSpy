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

using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.Metadata;

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

		protected override TextWriter CreateFile(string path)
		{
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

		protected override IEnumerable<ProjectItemInfo> WriteResourceFilesInProject(MetadataFile module) => [];
	}
}
