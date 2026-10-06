// Copyright (c) 2026 Piero Viano
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
using System.Linq;
using System.Threading.Tasks;

using AwesomeAssertions;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Dependencies;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Dependencies;

[TestFixture]
public class ReferencedByFinderTests
{
	static async Task<LoadedAssembly> OpenAsync(AssemblyList list, string path)
	{
		var loaded = list.OpenAssembly(path);
		await loaded.GetLoadResultAsync();
		return loaded;
	}

	[Test]
	public async Task Finds_The_Consumer_Of_A_Library_And_Nothing_Else()
	{
		var (libraryPath, consumerPath, _, _) = DependencyFixtures.EmitPair("RefBy");
		var other = FixtureAssembly.Emit(DependencyFixtures.UniqueName("Unrelated"));
		var list = new AssemblyList();
		var library = await OpenAsync(list, libraryPath);
		var consumer = await OpenAsync(list, consumerPath);
		await OpenAsync(list, other);

		var result = ReferencedByFinder.FindReferencingAssemblies(list.GetAssemblies(), library.GetMetadataFileOrNull()!);

		result.Should().ContainSingle();
		result[0].Assembly.Should().BeSameAs(consumer);
		result[0].Reference.Name.Should().Be(library.ShortName);
		result[0].IsVersionMismatch.Should().BeFalse();
	}

	[Test]
	public async Task A_Different_Referenced_Version_Still_Matches_And_Is_Reported()
	{
		var dir = DependencyFixtures.NewDirectory();
		var libraryName = DependencyFixtures.UniqueName("RefByLib");
		var libraryPath = DependencyFixtures.EmitLibrary(dir, libraryName, new Version(1, 0, 0, 0));
		var consumerPath = DependencyFixtures.EmitConsumer(dir, DependencyFixtures.UniqueName("RefByApp"), libraryPath);
		DependencyFixtures.EmitLibrary(dir, libraryName, new Version(3, 1, 0, 0));
		var list = new AssemblyList();
		var library = await OpenAsync(list, libraryPath);
		await OpenAsync(list, consumerPath);

		var result = ReferencedByFinder.FindReferencingAssemblies(list.GetAssemblies(), library.GetMetadataFileOrNull()!);

		result.Should().ContainSingle().Which.IsVersionMismatch.Should().BeTrue();
	}

	[Test]
	public void Matching_Requires_The_Same_Culture_And_Public_Key_Token()
	{
		// System.Runtime is strong-named; a reference with the right name but no token is a different assembly.
		var runtimePath = Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location)!, "System.Runtime.dll");
		using var runtime = new PEFile(runtimePath);
		string token = runtime.Metadata.GetPublicKeyToken();

		token.Should().NotBe("null");
		AssemblyNameReference.Parse($"System.Runtime, Version=1.0.0.0, Culture=neutral, PublicKeyToken={token}")
			.IsReferenceTo(runtime.Metadata)
			.Should().BeTrue("name and token match; the version is not part of the identity");
		AssemblyNameReference.Parse("System.Runtime, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")
			.IsReferenceTo(runtime.Metadata)
			.Should().BeFalse();
		AssemblyNameReference.Parse($"System.Runtime2, Version=1.0.0.0, Culture=neutral, PublicKeyToken={token}")
			.IsReferenceTo(runtime.Metadata)
			.Should().BeFalse();
		AssemblyNameReference.Parse($"System.Runtime, Version=1.0.0.0, Culture=de-DE, PublicKeyToken={token}")
			.IsReferenceTo(runtime.Metadata)
			.Should().BeFalse("the culture is part of the assembly identity");
	}
}
