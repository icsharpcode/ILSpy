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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Nested-namespace mode ("Use nested namespace structure"): the tree must expose every namespace,
/// and the namespace/type lookup primitives must resolve at any nesting depth.
/// </summary>
[TestFixture]
public class NestedNamespaceTreeTests
{
	/// <summary>
	/// Expands an assembly with nested-namespace mode on. The assembly node is expanded first so it
	/// is a realised, visible row: the filter cascade only runs for children of a visible parent, and
	/// that cascade is what computes <see cref="ICSharpCode.ILSpyX.TreeView.SharpTreeNode.IsHidden"/>.
	/// </summary>
	static async Task<(AssemblyTreeModel Model, AssemblyTreeNode Assembly)> BootNestedAsync(string assemblyName)
	{
		var (window, vm) = await TestHarness.BootAsync(3);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();

		AppComposition.Current.GetExport<SettingsService>().DisplaySettings.UseNestedNamespaceNodes = true;

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(assemblyName);
		assemblyNode.IsExpanded = true;
		assemblyNode.EnsureLazyChildren();
		return (vm.AssemblyTreeModel, assemblyNode);
	}

	static void ResetNestedMode()
		=> AppComposition.Current.GetExport<SettingsService>().DisplaySettings.UseNestedNamespaceNodes = false;

	static IEnumerable<NamespaceTreeNode> DescendantNamespaces(NamespaceTreeNode node)
	{
		yield return node;
		foreach (var child in node.Children.OfType<NamespaceTreeNode>())
		{
			foreach (var descendant in DescendantNamespaces(child))
				yield return descendant;
		}
	}

	[AvaloniaTest]
	public async Task Nested_Mode_Does_Not_Hide_Namespaces_That_Hold_Only_Child_Namespaces()
	{
		// A namespace that contains no types of its own but does contain sub-namespaces -- e.g.
		// "System.Collections" in an assembly that only ships "System.Collections.Generic" types --
		// is a pure intermediate node. It must still be shown: hiding it makes every namespace
		// underneath it unreachable in the tree, which is what "not all namespaces are showing"
		// looks like to the user.

		try
		{
			var (_, assemblyNode) = await BootNestedAsync("System.Linq");

			var intermediates = assemblyNode.Children.OfType<NamespaceTreeNode>()
				.SelectMany(DescendantNamespaces)
				.Where(ns => ns.Children.OfType<NamespaceTreeNode>().Any())
				.ToList();

			Assert.That(intermediates, Is.Not.Empty,
				"the fixture assembly must contain at least one namespace with sub-namespaces, "
				+ "otherwise this test asserts nothing");

			var hidden = intermediates.Where(ns => ns.IsHidden).Select(ns => ns.FullName).ToList();
			Assert.That(hidden, Is.Empty,
				"namespaces holding sub-namespaces must stay visible; hiding them strands every "
				+ "namespace below them");
		}
		finally
		{
			ResetNestedMode();
		}
	}

	[AvaloniaTest]
	public async Task Nested_Mode_Interleaves_Types_And_Sub_Namespaces_Alphabetically()
	{
		// A namespace holding both types and sub-namespaces lists them as one alphabetical
		// sequence -- "Collections" (namespace) sits between "Buffers" and "Console" (types) --
		// rather than grouping all types ahead of all namespaces. The band is built in a single
		// pass over the types ordered by full name, and a sub-namespace node is attached when its
		// first descendant is reached, which lands it at its own alphabetical position.

		try
		{
			var (_, assemblyNode) = await BootNestedAsync(TreeNavigation.CoreLibName);

			var system = assemblyNode.Children.OfType<NamespaceTreeNode>()
				.SelectMany(DescendantNamespaces)
				.Single(ns => ns.FullName == "System");

			// Without both kinds present there is no interleaving to observe and the ordering
			// assertion below would hold vacuously.
			Assert.That(system.Children.OfType<TypeTreeNode>(), Is.Not.Empty,
				"the fixture's System namespace must declare types of its own");
			Assert.That(system.Children.OfType<NamespaceTreeNode>(), Is.Not.Empty,
				"the fixture's System namespace must contain sub-namespaces");

			// ToString() is the ordering key on both node kinds: ReflectionName for a type,
			// the full dotted path for a namespace.
			var keys = system.Children.Select(child => child.ToString()).ToList();
			Assert.That(keys, Is.Ordered.Using(NaturalStringComparer.Instance),
				"types and sub-namespaces share one alphabetical sequence; grouping the types "
				+ "ahead of the namespaces breaks the ordering the WPF host had");
		}
		finally
		{
			ResetNestedMode();
		}
	}

	[AvaloniaTest]
	public async Task FindNamespaceNode_Resolves_A_Nested_Namespace()
	{
		// FindNamespaceNode is the lookup primitive behind "--navigateto N:..." and namespace
		// navigation. It takes a full dotted name and must resolve it at any depth -- in nested mode
		// the node for "System.Collections.Generic" is three levels down, not an assembly child.

		try
		{
			var (_, assemblyNode) = await BootNestedAsync("System.Linq");

			var ns = assemblyNode.FindNamespaceNode("System.Collections.Generic");

			Assert.That(ns, Is.Not.Null,
				"a full dotted namespace name must resolve in nested mode, where the matching node "
				+ "is a descendant rather than a direct child of the assembly node");
			Assert.That(ns!.FullName, Is.EqualTo("System.Collections.Generic"));
			Assert.That(ns.Name, Is.EqualTo("Generic"),
				"in nested mode the display label is the last segment only");
		}
		finally
		{
			ResetNestedMode();
		}
	}

	[AvaloniaTest]
	public async Task FindTypeNode_Resolves_A_Type_In_A_Nested_Namespace()
	{
		// FindTypeNode backs hyperlink clicks, search-result activation and JumpToType. A type whose
		// namespace has more than one segment must resolve in nested mode too.

		try
		{
			var (_, assemblyNode) = await BootNestedAsync("System.Linq");

			var module = assemblyNode.LoadedAssembly.GetMetadataFileOrNull()!;
			var typeSystem = (MetadataModule)module.GetTypeSystemOrNull()!.MainModule;
			var nestedType = typeSystem.TopLevelTypeDefinitions
				.First(t => t.Namespace.Contains('.') && t.Namespace != "System.Linq");

			var node = assemblyNode.FindTypeNode(nestedType);

			Assert.That(node, Is.Not.Null,
				$"the tree node for '{nestedType.ReflectionName}' must resolve in nested mode");
			Assert.That(node!.Handle,
				Is.EqualTo((System.Reflection.Metadata.TypeDefinitionHandle)nestedType.MetadataToken));
		}
		finally
		{
			ResetNestedMode();
		}
	}

	[AvaloniaTest]
	public async Task FindTreeNode_Resolves_A_Type_Reference_In_A_Nested_Namespace()
	{
		// The reference-to-node lookup behind hyperlink clicks in the decompiler view, search-result
		// activation and JumpToType. It goes through TreeNodeLocator rather than calling
		// AssemblyTreeNode.FindTypeNode directly, so it needs its own coverage in nested mode.

		try
		{
			var (model, assemblyNode) = await BootNestedAsync("System.Linq");

			var module = assemblyNode.LoadedAssembly.GetMetadataFileOrNull()!;
			var typeSystem = (MetadataModule)module.GetTypeSystemOrNull()!.MainModule;
			var nestedType = typeSystem.TopLevelTypeDefinitions
				.First(t => t.Namespace.Contains('.') && t.Namespace != "System.Linq");

			var node = model.FindTreeNode(nestedType);

			Assert.That(node, Is.InstanceOf<TypeTreeNode>(),
				$"a reference to '{nestedType.ReflectionName}' must resolve to its tree node in nested mode");
			Assert.That(((TypeTreeNode)node!).Handle,
				Is.EqualTo((System.Reflection.Metadata.TypeDefinitionHandle)nestedType.MetadataToken));
		}
		finally
		{
			ResetNestedMode();
		}
	}

	[AvaloniaTest]
	public async Task Nested_Namespace_Is_Hidden_When_All_Of_Its_Types_Are_Filtered_Out()
	{
		// Nested namespace nodes stay subject to the filter cascade: a namespace whose types are all
		// filtered out by ShowApiLevel must disappear along with them, rather than linger as an empty
		// row. Opting namespace nodes out of the cascade (by reporting FilterResult.Match) would make
		// the display bug go away too, but at the cost of this behaviour -- hence the coverage.

		var settings = AppComposition.Current.GetExport<SettingsService>().SessionSettings.LanguageSettings;
		var original = settings.ShowApiLevel;
		try
		{
			var (_, assemblyNode) = await BootNestedAsync("System.Linq");

			// FxResources.* holds only internal resource-string types, so PublicOnly empties it.
			var fxResources = assemblyNode.Children.OfType<NamespaceTreeNode>()
				.SingleOrDefault(ns => ns.FullName == "FxResources");
			Assert.That(fxResources, Is.Not.Null,
				"the fixture assembly must expose the non-public FxResources namespace in nested mode");
			Assert.That(fxResources!.IsHidden, Is.False, "it is visible while every API level is shown");

			settings.ShowApiLevel = ApiVisibility.PublicOnly;
			assemblyNode.RefreshRealizedFilter();

			Assert.That(fxResources.IsHidden, Is.True,
				"a namespace left with no visible types must be hidden by the filter cascade");

			settings.ShowApiLevel = ApiVisibility.All;
			assemblyNode.RefreshRealizedFilter();

			Assert.That(fxResources.IsHidden, Is.False,
				"and it must come back when the API level is widened again");
		}
		finally
		{
			settings.ShowApiLevel = original;
			ResetNestedMode();
		}
	}
}
