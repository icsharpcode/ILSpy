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

using System;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class RemoveAssemblyContextMenuTests
{
	[AvaloniaTest]
	public async Task Remove_Entry_Is_Registered_With_The_Context_Menu_Registry()
	{
		// AssemblyTreeNode contributes a [ExportContextMenuEntry] for "_Remove" so right-
		// clicking an assembly node offers an action that unloads it. Verifies the entry
		// shows up in the live MEF registry.

		// Arrange + Act — boot so the composition host is initialised.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();

		// Assert — registry contains an entry whose header resolves to the _Remove resource.
		registry.Entries.Should().Contain(
			e => e.Metadata.Header == nameof(Resources._Remove),
			"AssemblyTreeNode must export a Remove context-menu entry");
	}

	[AvaloniaTest]
	public async Task Remove_Entry_Is_Visible_Only_When_Every_Selected_Node_Is_An_AssemblyTreeNode()
	{
		// IsVisible filters the entry: it shows up when the entire selection is assembly
		// nodes (the action's target), and is hidden otherwise so it doesn't pollute
		// e.g. method/type tree contexts.

		// Arrange — boot, locate the registered entry.
		var (_, vm) = await TestHarness.BootAsync(3);
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var removeEntry = registry.GetEntry(nameof(Resources._Remove));

		var asm = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");

		// Assert 1 — visible for an assembly-only selection.
		removeEntry.IsVisible(new TextViewContext {
			SelectedTreeNodes = new[] { (ICSharpCode.ILSpyX.TreeView.SharpTreeNode)asm }
		}).Should().BeTrue();

		// Assert 2 — hidden as soon as the selection includes a non-assembly node.
		removeEntry.IsVisible(new TextViewContext {
			SelectedTreeNodes = new[] {
				(ICSharpCode.ILSpyX.TreeView.SharpTreeNode)asm,
				typeNode,
			}
		}).Should().BeFalse();

		// Assert 3 — hidden when the selection is null/empty.
		removeEntry.IsVisible(new TextViewContext { SelectedTreeNodes = null }).Should().BeFalse();
	}

	// audit (2026-05-12): converted 📦 → 🧪. Was calling `removeEntry.Execute(synthetic ctx)`
	// directly; now selects the assembly via the model (production dispatch), builds the
	// context menu via the same `BuildContextMenuForCurrentState` path the live `Opening`
	// event invokes, finds the "Remove" MenuItem, and fires the `Click` routed event — the
	// same RoutedEvent `ContextMenu`'s MenuItem fires when the user clicks. Catches both the
	// "Execute removes the assembly" behavior AND any regression that breaks the
	// menu-building → click-handler-attachment chain.
	[AvaloniaTest]
	public async Task Right_Click_Then_Remove_On_An_Assembly_Unloads_It_From_The_List()
	{
		// Arrange — boot, snapshot a sacrificial assembly + a survivor.
		var (window, vm) = await TestHarness.BootAsync(3);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();

		var sacrificialName = typeof(System.Linq.Enumerable).Assembly.GetName().Name!;
		var survivorName = typeof(object).Assembly.GetName().Name!;
		var sacrificial = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(sacrificialName);

		window.Capture("initial-tree-three-assemblies");

		// Act — select the row, build the menu the way the live Opening event would, find
		// the Remove item, and click it.
		vm.AssemblyTreeModel.SelectNode(sacrificial);
		window.Capture("sacrificial-selected");

		var menu = pane.BuildContextMenuForCurrentState(registry.Entries);
		menu.Should().NotBeNull();
		menu!.ClickItem(Resources._Remove);
		window.Capture("remove-clicked");

		// Assert — the assembly is gone from the list; the survivor is still there.
		await Waiters.WaitForAsync(() =>
			!vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
				.Any(a => string.Equals(a.ShortName, sacrificialName, StringComparison.Ordinal)));
		vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.Should().Contain(a => a.ShortName == survivorName);

		window.Capture("after-remove-sacrificial-gone");
	}

	[AvaloniaTest]
	public async Task Right_Click_On_Assembly_Tree_Builds_Menu_Containing_Remove()
	{
		// End-to-end check that the wiring (registry → AssemblyListPane.AttachContextMenu →
		// ContextMenuProvider.Build) lands a "Remove" item in the menu the user actually sees
		// when right-clicking on a selected assembly node.

		// Arrange — boot, select an assembly node.
		var (window, vm) = await TestHarness.BootAsync();
		await Waiters.WaitForAsync(
			() => window.GetVisualDescendants().OfType<AssemblyListPane>().Any());
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		vm.AssemblyTreeModel.SelectNode(assemblyNode);
		window.Capture("system-linq-selected");

		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();

		// Act — drive the same build path the live Opening event uses.
		var built = pane.BuildContextMenuForCurrentState(registry.Entries);
		window.Capture("context-menu-built");

		// Assert — the live menu contains a "Remove" item (resolved to its localized form by
		// the menu builder).
		built.Should().NotBeNull();
		var headers = built!.Items.OfType<MenuItem>().Select(i => (string?)i.Header).ToArray();
		headers.Should().Contain(Resources._Remove);
	}
}
