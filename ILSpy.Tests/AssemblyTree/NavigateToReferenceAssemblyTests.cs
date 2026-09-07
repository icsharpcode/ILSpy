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

using System.IO;
using System.Threading.Tasks;

using AwesomeAssertions;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpyX;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.AssemblyTree;

/// <summary>
/// A navigation target that only a reference assembly declares. The VS add-in hands ILSpy the
/// assemblies a project references, which for a framework-targeting project are the targeting
/// pack's reference assemblies, so refusing to look in them leaves the target unresolved and
/// the jump silently does nothing (issue #2093).
/// </summary>
[TestFixture]
public class NavigateToReferenceAssemblyTests
{
	// The member is looked up on this fixture itself: it is public, so the reference assembly
	// declares it too, and both assemblies are next to the test at run time.
	public static int TargetMember(string text) => text.Length;

	const string TargetId = "M:ICSharpCode.ILSpy.Tests.AssemblyTree.NavigateToReferenceAssemblyTests.TargetMember(System.String)";

	static string ImplementationPath => typeof(NavigateToReferenceAssemblyTests).Assembly.Location;

	static string ReferencePath => Path.Combine(
		Path.GetDirectoryName(ImplementationPath)!, "ReferenceAssemblyFixture",
		Path.GetFileName(ImplementationPath));

	[Test]
	public async Task The_Fixture_Really_Is_A_Reference_Assembly()
	{
		File.Exists(ReferencePath).Should().BeTrue(
			"the build copies this project's reference assembly next to the tests");
		var list = new AssemblyList();
		var reference = list.OpenAssembly(ReferencePath);
		var file = await reference.GetMetadataFileOrNullAsync();
		file.Should().NotBeNull();
		file!.IsReferenceAssembly().Should().BeTrue("otherwise the tests below prove nothing");
	}

	[Test]
	public async Task A_Member_Only_A_Reference_Assembly_Declares_Still_Resolves()
	{
		var list = new AssemblyList();
		var reference = list.OpenAssembly(ReferencePath);
		await reference.GetMetadataFileOrNullAsync();

		var entity = AssemblyTreeModel.FindEntityInRelevantAssemblies(TargetId, new[] { reference });

		entity.Should().NotBeNull("a reference assembly is the only place the member can be found");
		entity!.Name.Should().Be(nameof(TargetMember));
	}

	[Test]
	public async Task An_Implementation_Assembly_Wins_Over_A_Reference_Assembly()
	{
		var list = new AssemblyList();
		var reference = list.OpenAssembly(ReferencePath);
		var implementation = list.OpenAssembly(ImplementationPath);
		await reference.GetMetadataFileOrNullAsync();
		await implementation.GetMetadataFileOrNullAsync();

		// The reference assembly comes first, so a plain "first hit wins" search would answer
		// with it; only a search that prefers real definitions picks the implementation.
		var entity = AssemblyTreeModel.FindEntityInRelevantAssemblies(
			TargetId, new[] { reference, implementation });

		entity.Should().NotBeNull();
		entity!.ParentModule!.MetadataFile!.FileName.Should().Be(ImplementationPath,
			"a definition with a body is more useful than a signature-only one");
	}
}
