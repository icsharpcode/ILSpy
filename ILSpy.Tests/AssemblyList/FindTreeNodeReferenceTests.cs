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

using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// <c>AssemblyTreeModel.FindTreeNode</c> must resolve non-entity references -- a whole assembly
/// (LoadedAssembly / MetadataFile) and a namespace -- to their tree nodes, not just entities.
/// (Regression: the Avalonia port only handled entity references, so Assembly/Namespace search
/// results and resource/namespace links silently did nothing.)
/// </summary>
[TestFixture]
public class FindTreeNodeReferenceTests
{
	[AvaloniaTest]
	public async Task FindTreeNode_Resolves_Assembly_And_Namespace_References()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var atm = vm.AssemblyTreeModel;

		var asmNode = atm.FindNode<AssemblyTreeNode>("System.Linq");
		Assert.That(asmNode, Is.Not.Null);
		var loaded = asmNode!.LoadedAssembly;

		// LoadedAssembly reference -> its assembly node. (Custom TreeNodeAssertions shadows
		// .Should() on tree nodes, so use Assert.That here.)
		Assert.That(atm.FindTreeNode(loaded), Is.SameAs(asmNode),
			"a LoadedAssembly reference resolves to its assembly node");

		// MetadataFile reference -> the same assembly node.
		var metadataFile = loaded.GetMetadataFileOrNull();
		Assert.That(metadataFile, Is.Not.Null);
		Assert.That(atm.FindTreeNode(metadataFile), Is.SameAs(asmNode),
			"a MetadataFile reference resolves to its assembly node");

		// Namespace reference -> its namespace node.
		var compilation = loaded.GetTypeSystemOrNull();
		Assert.That(compilation, Is.Not.Null);
		var ns = compilation!.MainModule.RootNamespace.GetChildNamespace("System")?.GetChildNamespace("Linq");
		Assert.That(ns, Is.Not.Null);

		Assert.That(atm.FindTreeNode(ns), Is.InstanceOf<NamespaceTreeNode>(),
			"a namespace reference resolves to its namespace node");
	}
}
