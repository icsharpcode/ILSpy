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
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;

using ILSpy;
using ILSpy.AppEnv;
using ILSpy.AssemblyTree;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

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
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var removeEntry = registry.Entries
			.Single(e => e.Metadata.Header == nameof(Resources._Remove))
			.Value;

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

	[AvaloniaTest]
	public async Task Remove_Entry_Execute_Unloads_All_Selected_Assemblies()
	{
		// Execute on the entry must unload every assembly in the selection from the active
		// AssemblyList — the same behaviour the Del key already provides, now also reachable
		// via right-click.

		// Arrange — boot, snapshot a sacrificial assembly + a survivor.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var removeEntry = registry.Entries
			.Single(e => e.Metadata.Header == nameof(Resources._Remove))
			.Value;

		var sacrificialName = typeof(System.Linq.Enumerable).Assembly.GetName().Name!;
		var survivorName = typeof(object).Assembly.GetName().Name!;
		var sacrificial = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(sacrificialName);

		// Act — fire Execute with the assembly node in the context.
		removeEntry.Execute(new TextViewContext {
			SelectedTreeNodes = new[] { (ICSharpCode.ILSpyX.TreeView.SharpTreeNode)sacrificial }
		});

		// Assert — the assembly is gone from the list; the survivor is still there.
		await Waiters.WaitForAsync(() =>
			!vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
				.Any(a => string.Equals(a.ShortName, sacrificialName, StringComparison.Ordinal)));
		vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.Should().Contain(a => a.ShortName == survivorName);
	}

	[AvaloniaTest]
	public async Task Right_Click_On_Assembly_Tree_Builds_Menu_Containing_Remove()
	{
		// End-to-end check that the wiring (registry → AssemblyListPane.AttachContextMenu →
		// ContextMenuProvider.Build) lands a "Remove" item in the menu the user actually sees
		// when right-clicking on a selected assembly node.

		// Arrange — boot, select an assembly node.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);
		await Waiters.WaitForAsync(
			() => window.GetVisualDescendants().OfType<AssemblyListPane>().Any());
		var pane = window.GetVisualDescendants().OfType<AssemblyListPane>().Single();
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		vm.AssemblyTreeModel.SelectedItem = assemblyNode;

		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();

		// Act — drive the same build path the live Opening event uses.
		var built = pane.BuildContextMenuForCurrentState(registry.Entries);

		// Assert — the live menu contains a "Remove" item (resolved to its localized form by
		// the menu builder).
		built.Should().NotBeNull();
		var headers = built!.Items.OfType<MenuItem>().Select(i => (string?)i.Header).ToArray();
		headers.Should().Contain(Resources._Remove);
	}
}
