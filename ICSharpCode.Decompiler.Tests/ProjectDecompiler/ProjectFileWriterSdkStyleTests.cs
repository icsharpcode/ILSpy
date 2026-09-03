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
using System.IO;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.Metadata;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.ProjectDecompiler;

[TestFixture]
public sealed class ProjectFileWriterSdkStyleTests
{
	/// <summary>
	/// Everything the export wrote to disk has to be reachable from the project file, or the
	/// exported project cannot rebuild. XAML recovered from BAML is the case that hurts: it lands
	/// in "Page" items, and a WPF project without them compiles no XAML at all.
	/// </summary>
	[Test]
	public void ItemTypesOtherThanEmbeddedResourceAreListed()
	{
		ProjectItemInfo[] files = [
			new ProjectItemInfo("Page", "Themes/Generic.xaml")
				.With("Generator", "MSBuild:Compile")
				.With("SubType", "Designer"),
			new ProjectItemInfo("Compile", "Program.cs"),
		];

		string project = WriteProjectFile(files);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(project, Does.Contain(@"<Page Include=""Themes/Generic.xaml"" Generator=""MSBuild:Compile"" SubType=""Designer"" />"),
				"the XAML file is a project item, metadata included");
			Assert.That(project, Does.Contain(@"<Page Remove=""Themes/Generic.xaml"" />"),
				"the SDK's own **/*.xaml glob would otherwise make the explicit item a duplicate");
			Assert.That(project.IndexOf(@"<Page Remove", StringComparison.Ordinal),
				Is.LessThan(project.IndexOf(@"<Page Include", StringComparison.Ordinal)),
				"the remove has to precede the include, or it takes the include with it");
			Assert.That(project, Does.Not.Contain("Program.cs"),
				"source files come in through the SDK's default Compile glob");
		}
	}

	static string WriteProjectFile(ProjectItemInfo[] files)
	{
		StringWriter output = new();
		ProjectFileWriterSdkStyle.Default.Write(output, new TestProjectInfoProvider(), files,
			new PEFile("ICSharpCode.Decompiler.dll"));
		return output.ToString();
	}

	sealed class TestProjectInfoProvider : IProjectInfoProvider
	{
		public IAssemblyResolver AssemblyResolver { get; } = new UniversalAssemblyResolver(null, false, null);
		public IAssemblyReferenceClassifier AssemblyReferenceClassifier { get; } = new AssemblyReferenceClassifier();
		public LanguageVersion LanguageVersion => LanguageVersion.Latest;
		public bool CheckForOverflowUnderflow => false;
		public Guid ProjectGuid { get; } = Guid.NewGuid();
		public string TargetDirectory { get; } = Environment.CurrentDirectory;
		public string StrongNameKeyFile => null;
	}
}
