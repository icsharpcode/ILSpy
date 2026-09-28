// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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

using AwesomeAssertions;

using ICSharpCode.Decompiler.Documentation;
using ICSharpCode.Decompiler.Metadata;

using ICSharpCode.ILSpyX;

using NUnit.Framework;

using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace ICSharpCode.ILSpy.Tests.TextView;

/// <summary>
/// The XML-documentation lookup behind the decompiler view's hover tooltip. On modern .NET
/// the entity's metadata-token-bearing assembly (System.Private.CoreLib.dll) is not the one
/// whose XML carries its docs (System.Runtime.xml), so <see cref="XmlDocLoader"/> falls back
/// to the parallel ref pack - <c>&lt;dotnet&gt;/packs/Microsoft.NETCore.App.Ref/&lt;version&gt;/ref/&lt;tfm&gt;/*.xml</c>
/// - and aggregates every XML there into one provider. Without that fallback every tooltip
/// over a BCL member renders empty.
/// </summary>
[TestFixture]
public class XmlDocumentationTests
{
	// The ID string the decompiler produces for the two-argument overload; using the literal
	// keeps the test off the type system, which is not what is under test here.
	const string StringConcatId = "M:System.String.Concat(System.String,System.String)";

	[Test]
	public void XmlDocLoader_Surfaces_Documentation_For_CoreLib_String_Concat()
	{
		var coreLib = new AssemblyList().OpenAssembly(typeof(object).Assembly.Location)
			.GetMetadataFileOrNull();
		coreLib.Should().NotBeNull();

		var provider = XmlDocLoader.LoadDocumentation(coreLib!);
		((object?)provider).Should().NotBeNull(
			"the ref-pack fallback must locate the XMLs for the test-host runtime layout");

		var documentation = provider!.GetDocumentation(StringConcatId);

		documentation.Should().NotBeNullOrEmpty(
			"System.String.Concat is one of the most-documented methods in CoreLib - the hover "
			+ "tooltip would be empty without this");
		documentation.Should().Contain("<summary",
			"the raw documentation string must include the <summary> tag the renderer parses");
	}

	[Test]
	public void XmlDocLoader_Does_Not_Use_RefPack_For_Nonexistent_Runtime_Path()
	{
		var tempDir = Path.Combine(TestContext.CurrentContext.WorkDirectory, Path.GetRandomFileName());
		try
		{
			var runtimeAssemblyPath = Path.Combine(tempDir, "shared", "Microsoft.NETCore.App", "10.0.0", "System.Private.CoreLib.dll");
			var refPackDir = Path.Combine(tempDir, "packs", "Microsoft.NETCore.App.Ref", "10.0.0", "ref", "net10.0");
			Directory.CreateDirectory(refPackDir);
			File.WriteAllText(Path.Combine(refPackDir, "System.Runtime.xml"),
				"""
				<?xml version="1.0"?>
				<doc><members><member name="M:System.String.Concat(System.String,System.String)"><summary>wrong docs</summary></member></members></doc>
				""");
			using var stream = File.OpenRead(typeof(object).Assembly.Location);
			using var coreLib = new PEFile(runtimeAssemblyPath, stream,
				PEStreamOptions.PrefetchEntireImage, MetadataReaderOptions.None);

			var provider = XmlDocLoader.LoadDocumentation(coreLib);

			((object?)provider).Should().BeNull(
				"a synthetic or in-memory assembly name should not be treated as an installed runtime assembly path");
		}
		finally
		{
			Directory.Delete(tempDir, true);
		}
	}
}
