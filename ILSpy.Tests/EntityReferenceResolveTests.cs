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

using ICSharpCode.Decompiler.TypeSystem;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// EntityReference.Resolve turns an unresolved (module, handle) reference into the live IEntity,
/// the shared resolution used by code-view hover navigation and the assembly-tree node finder.
/// </summary>
[TestFixture]
public class EntityReferenceResolveTests
{
	[AvaloniaTest]
	public async Task Resolve_Returns_The_Entity_For_A_Module_Handle_Reference()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		var entity = (IEntity)typeNode.Member!;
		var module = entity.ParentModule!.MetadataFile!;

		var reference = new EntityReference(module, entity.MetadataToken);
		var resolved = reference.Resolve(vm.AssemblyTreeModel.AssemblyList!);

		resolved.Should().NotBeNull("the (module, handle) pair resolves back to its entity");
		resolved!.FullName.Should().Be(entity.FullName);
	}

	[AvaloniaTest]
	public async Task Resolve_Returns_Null_For_An_Unknown_Module()
	{
		var (_, vm) = await TestHarness.BootAsync();
		// A module path that isn't in the list can't be resolved -> null, not a throw.
		var reference = new EntityReference("X:/does/not/exist.dll",
			System.Reflection.Metadata.Ecma335.MetadataTokens.TypeDefinitionHandle(2));
		reference.Resolve(vm.AssemblyTreeModel.AssemblyList!).Should().BeNull();
	}
}
