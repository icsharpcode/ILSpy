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

using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using ICSharpCode.ILSpy.Controls.Omnibar;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Controls;

/// <summary>
/// The omnibar breadcrumb is the decompiler's "you are here" trail: it must mirror the
/// assembly-tree path of the decompiled node (Assembly > Namespace > Type > Member), excluding
/// the invisible tree root, and clicking a segment must move the tree selection there.
/// </summary>
[TestFixture]
public class OmnibarBreadcrumbTests
{
	[AvaloniaTest]
	public async Task BuildSegments_Yields_Assembly_Namespace_Type_Member_In_Order()
	{
		var (_, vm) = await TestHarness.BootAsync(3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.EnsureLazyChildren();
		var methodNode = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");

		var segments = OmnibarViewModel.BuildSegments(methodNode);

		Assert.That(segments, Has.Count.EqualTo(4),
			"a member node breadcrumb is Assembly > Namespace > Type > Member (root excluded)");
		Assert.That(segments[0].Node, Is.InstanceOf<AssemblyTreeNode>(),
			"the first crumb is the assembly, not the invisible AssemblyListTreeNode root");
		Assert.That(segments[1].Node, Is.InstanceOf<NamespaceTreeNode>());
		Assert.That(segments[1].Text, Is.EqualTo("System.Linq"));
		Assert.That(segments[2].Node, Is.SameAs(typeNode));
		Assert.That(segments[3].Node, Is.SameAs(methodNode));
		Assert.That(segments[3].Text, Does.Contain("Empty"));
	}

	[AvaloniaTest]
	public async Task Leaf_Crumb_Has_No_Children_While_Container_Crumbs_Do()
	{
		var (_, vm) = await TestHarness.BootAsync(3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.EnsureLazyChildren();
		var methodNode = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");

		var segments = OmnibarViewModel.BuildSegments(methodNode);

		// Container crumbs (assembly / namespace / type) host children, so they keep the chevron.
		Assert.That(segments[0].HasChildren, Is.True, "the assembly crumb has children");
		Assert.That(segments[2].HasChildren, Is.True, "the type crumb has members");
		// The method is a leaf: no chevron, so it can't open an empty dropdown.
		Assert.That(segments[3].HasChildren, Is.False, "a method crumb is a leaf and drops the chevron");
	}

	[AvaloniaTest]
	public async Task NavigateSegment_Moves_The_Tree_Selection()
	{
		var (_, vm) = await TestHarness.BootAsync(3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.EnsureLazyChildren();
		var methodNode = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");

		var omnibar = new OmnibarViewModel();
		omnibar.SetNode(methodNode);

		// Click the "Type" crumb (index 2): selection jumps from the method to its type.
		omnibar.NavigateSegment(omnibar.Segments[2]);

		Assert.That(vm.AssemblyTreeModel.SelectedItem, Is.SameAs(typeNode));
	}

	[AvaloniaTest]
	public async Task EnterSearch_And_ExitSearch_Toggle_Mode_And_Clear_Text()
	{
		await TestHarness.BootAsync(1);

		var omnibar = new OmnibarViewModel();
		Assert.That(omnibar.Mode, Is.EqualTo(OmnibarMode.Breadcrumb));

		omnibar.EnterSearch();
		Assert.That(omnibar.Mode, Is.EqualTo(OmnibarMode.Search));

		omnibar.SearchText = "Enumerable";
		omnibar.ExitSearch();

		Assert.That(omnibar.Mode, Is.EqualTo(OmnibarMode.Breadcrumb));
		Assert.That(omnibar.SearchText, Is.Empty);
	}
}
