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
public sealed class MSBuildEscapingTests
{
	/// <summary>
	/// A decompiled assembly names its own resources, and the exported project repeats those names
	/// in LogicalName metadata. MSBuild expands property references and property functions inside
	/// attribute values, so an unquoted name of the form "$([System.IO.File]::ReadAllText(...))" is
	/// evaluated as soon as the project is opened or built - by the person who decompiled the
	/// assembly because they did not trust it.
	/// </summary>
	[TestCase(true)]
	[TestCase(false)]
	public void ResourceNamesCannotSmuggleMSBuildSyntaxIntoTheProjectFile(bool sdkStyleProject)
	{
		const string hostile = "$([System.IO.File]::ReadAllText('x'));@(Y)";
		const string quoted = "%24%28[System.IO.File]::ReadAllText%28%27x%27%29%29%3B%40%28Y%29";

		IProjectFileWriter writer = sdkStyleProject ? ProjectFileWriterSdkStyle.Default : ProjectFileWriterDefault.Instance;
		StringWriter output = new();
		writer.Write(output, new ProjectInfo(), new[] {
			new ProjectItemInfo("EmbeddedResource", "logo.png").With("LogicalName", hostile)
		}, new PEFile("ICSharpCode.Decompiler.dll"));

		using (Assert.EnterMultipleScope())
		{
			Assert.That(output.ToString(), Does.Contain(quoted));
			Assert.That(output.ToString(), Does.Not.Contain(hostile));
		}
	}

	/// <summary>
	/// The escape form is MSBuild's own, and covers exactly the characters MSBuild reads as syntax.
	/// Text without any of them has to come back untouched, or every ordinary name in an exported
	/// project would be rewritten.
	/// </summary>
	[TestCase("plain/name.png", ExpectedResult = "plain/name.png")]
	[TestCase("", ExpectedResult = "")]
	[TestCase("100%off", ExpectedResult = "100%25off")]
	[TestCase("%*?@$();'", ExpectedResult = "%25%2A%3F%40%24%28%29%3B%27")]
	[TestCase("a b/资源-[x]{y}.png", ExpectedResult = "a b/资源-[x]{y}.png")]
	public string QuotingCoversTheCharactersMSBuildReadsAsSyntax(string value)
	{
		return MSBuildEscaping.Escape(value);
	}

	sealed class ProjectInfo : IProjectInfoProvider
	{
		public IAssemblyResolver AssemblyResolver => new UniversalAssemblyResolver(null, false, null);
		public IAssemblyReferenceClassifier AssemblyReferenceClassifier => new AssemblyReferenceClassifier();
		public LanguageVersion LanguageVersion => LanguageVersion.CSharp1;
		public bool CheckForOverflowUnderflow => false;
		public Guid ProjectGuid { get; } = Guid.NewGuid();
		public string TargetDirectory => Environment.CurrentDirectory;
		public string StrongNameKeyFile => null;
	}
}
