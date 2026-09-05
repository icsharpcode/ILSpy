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
public sealed class ProjectFileWriterDefaultTests
{
	/// <summary>
	/// Item metadata as attributes is MSBuild 15 syntax. The non-SDK format exists for the
	/// toolchains that came before it, and those reject an unknown attribute on an item element,
	/// so metadata has to be written the way every non-SDK project writes it: as child elements.
	/// </summary>
	[Test]
	public void ItemMetadataIsWrittenAsChildElements()
	{
		ProjectItemInfo[] files = [
			new ProjectItemInfo("Page", "Themes/Generic.xaml")
				.With("Generator", "MSBuild:Compile")
				.With("SubType", "Designer"),
		];

		string project = WriteProjectFile(files);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(project, Does.Contain(@"<Page Include=""Themes/Generic.xaml"">"), project);
			Assert.That(project, Does.Contain(@"<Generator>MSBuild:Compile</Generator>"), project);
			Assert.That(project, Does.Contain(@"<SubType>Designer</SubType>"), project);
			Assert.That(project, Does.Not.Contain(@"Generator="""), "metadata does not belong in an attribute");
		}
	}

	[Test]
	public void AnItemWithoutMetadataStaysOnOneLine()
	{
		ProjectItemInfo[] files = [new ProjectItemInfo("Compile", "Program.cs")];

		string project = WriteProjectFile(files);

		Assert.That(project, Does.Contain(@"<Compile Include=""Program.cs"" />"), project);
	}

	static string WriteProjectFile(ProjectItemInfo[] files)
	{
		StringWriter output = new();
		ProjectFileWriterDefault.Instance.Write(output, new TestProjectInfoProvider(), files,
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
