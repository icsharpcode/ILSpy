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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.DataGridHierarchical;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX.TreeView;

using ILSpy;
using ILSpy.AppEnv;
using ILSpy.AssemblyTree;
using ILSpy.Commands;
using ILSpy.Docking;
using ILSpy.TextView;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class DecompileInNewViewTests
{
	[AvaloniaTest]
	public async Task DecompileInNewView_Entry_Is_Registered()
	{
		// "Decompile in new tab" is a context-menu entry sitting alongside Remove on every
		// node that decompiles. Verifies the [ExportContextMenuEntry] is discovered.

		// Arrange — boot.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();

		// Assert — registry contains an entry whose header is the DecompileToNewPanel resource.
		registry.Entries.Should().Contain(
			e => e.Metadata.Header == nameof(Resources.DecompileToNewPanel));
	}

	// audit (2026-05-12): converted 📦 → 🧪. Was calling `entry.IsVisible(synthetic ctx)`
	// directly with hand-built TextViewContexts; now drives the live `BuildContextMenuForCurrentState`
	// path with real selection states. The entry's presence/absence in the built menu IS the
	// integration-level expression of IsVisible (the builder drops hidden entries before
	// creating their MenuItems).
	[AvaloniaTest]
	public async Task DecompileInNewView_Appears_In_The_Right_Click_Menu_Only_When_Tree_Nodes_Are_Selected()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();

		var asm = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		var header = ICSharpCode.ILSpy.Properties.Resources.DecompileToNewPanel;

		window.Capture("initial-no-selection");

		// With an assembly node selected → the menu surfaces "Decompile in new tab".
		vm.AssemblyTreeModel.SelectNode(asm);
		window.Capture("assembly-selected");
		var withSelection = pane.BuildContextMenuForCurrentState(registry.Entries);
		withSelection.Should().NotBeNull();
		withSelection!.Items.OfType<MenuItem>().Select(i => (string?)i.Header)
			.Should().Contain(header);

		// With no selection → the entry is filtered out of the built menu.
		vm.AssemblyTreeModel.SelectedItems.Clear();
		window.Capture("selection-cleared");
		var withoutSelection = pane.BuildContextMenuForCurrentState(registry.Entries);
		// `Build` returns null when no entry would be visible; otherwise it may still build a
		// menu (other entries don't gate on selection). Either way, the header must not appear.
		(withoutSelection?.Items.OfType<MenuItem>().Select(i => (string?)i.Header) ?? System.Linq.Enumerable.Empty<string?>())
			.Should().NotContain(header);
	}

	// audit (2026-06-02): rewritten for the Thunderbird-style context target. Previously this
	// test selected the second method via the model first — encoding the OLD bug where
	// right-clicking a row moved the selection there before the menu opened, so you ended up
	// with the right-clicked node twice (the preview reused for it + the new tab) instead of
	// "keep A, open B". Now A stays the active document and the menu targets B directly, so the
	// result is A + B: the new tab shows B and A is untouched.
	[AvaloniaTest]
	public async Task Clicking_DecompileInNewView_Opens_The_Right_Clicked_Method_While_Keeping_The_Active_One()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var firstMethod = typeNode.Children.OfType<MethodTreeNode>()
			.Single(m => m.MethodDefinition.Name == "AsEnumerable");
		var secondMethod = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");

		// A is the active document: select firstMethod and decompile it into the preview tab.
		vm.AssemblyTreeModel.SelectNode(firstMethod);
		var firstTab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		firstTab.Text.Should().Contain("AsEnumerable");
		window.Capture("first-method-active");

		var pane = await window.WaitForComponent<AssemblyListPane>();
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var documents = ((ILSpyDockFactory)vm.DockWorkspace.Factory).Documents!;
		var initialCount = documents.VisibleDockables?.Count ?? 0;

		// Right-click B (secondMethod) WITHOUT moving the selection, and click "Decompile to
		// new tab" from the menu the right-click would produce.
		var menu = pane.BuildContextMenuForCurrentState(registry.Entries, rightClickedNode: secondMethod);
		menu.Should().NotBeNull();
		menu!.ClickItem(Resources.DecompileToNewPanel);

		await Waiters.WaitForAsync(
			() => (documents.VisibleDockables?.Count ?? 0) > initialCount);
		var newTab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		// The new tab shows B...
		ReferenceEquals(newTab, firstTab).Should().BeFalse(
			"a fresh decompiler tab must be created instead of reusing the existing one");
		newTab.Text.Should().Contain("Empty");
		// ...the original tab still shows A (it was NOT yanked to B by the right-click — this is
		// the whole point: you keep A and gain B, instead of ending up with B twice)...
		firstTab.Text.Should().Contain("AsEnumerable");
		// ...and exactly one tab was added (A + B), not two copies of B.
		documents.VisibleDockables!.Count.Should().Be(initialCount + 1);
		window.Capture("a-kept-b-opened");
	}

	[AvaloniaTest]
	public async Task Right_Clicking_An_Unselected_Row_Does_Not_Move_The_Selection()
	{
		// Thunderbird-style context target: right-clicking a row the user has NOT selected must
		// leave the real selection (and therefore the preview document) untouched. Today
		// ProDataGrid's default selects the right-clicked row on press, which yanks the preview
		// to B before the menu opens — so "Decompile to new tab" ends up with B twice instead of
		// the intended A + B. This probe asserts the selection stays put under a real right-click.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<DataGrid>();

		var nodeA = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		var nodeB = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(TreeNavigation.CoreLibName);

		// Make A the active document/selection.
		vm.AssemblyTreeModel.SelectNode(nodeA);

		// Let the top-level rows realise and layout settle.
		for (int i = 0; i < 8; i++)
		{
			Dispatcher.UIThread.RunJobs();
			grid.UpdateLayout();
			await Task.Delay(25);
		}

		var rowB = grid.GetVisualDescendants().OfType<DataGridRow>()
			.FirstOrDefault(r => RowNodeEquals(r, nodeB));
		rowB.Should().NotBeNull("the top-level CoreLib row must be realised");

		// Suppressing the right-press must not also suppress the context menu — it still opens
		// from ContextRequested (raised on release). Watch for it.
		var menuRequested = false;
		grid.AddHandler(Control.ContextRequestedEvent, (_, _) => menuRequested = true,
			handledEventsToo: true);

		// Right-click the centre of B's row (clear of the far-left expander glyph).
		var point = rowB!.TranslatePoint(new Point(rowB.Bounds.Width / 2, rowB.Bounds.Height / 2), window);
		point.Should().NotBeNull();
		HeadlessWindowExtensions.MouseDown(window, point!.Value, MouseButton.Right);
		HeadlessWindowExtensions.MouseUp(window, point.Value, MouseButton.Right);
		for (int i = 0; i < 4; i++)
		{
			Dispatcher.UIThread.RunJobs();
			await Task.Delay(20);
		}

		ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, nodeA).Should().BeTrue(
			"right-clicking an unselected row must not change the selection (Thunderbird-style context target)");
		menuRequested.Should().BeTrue(
			"the context menu must still be requested after the suppressed right-press");
		rowB.Classes.Should().Contain("contextTarget",
			"the right-clicked row must carry the focus-box style class while it is the menu target");
	}

	[AvaloniaTest]
	public async Task Right_Clicking_A_Second_Row_Moves_The_Context_Highlight_To_It()
	{
		// Regression: the focus box must follow every right-click, not stick after the first one
		// and then never appear again. Reproduces "right-click B (highlight), dismiss, right-click
		// C (no highlight)".
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<DataGrid>();

		var assemblies = vm.AssemblyTreeModel.Root!.Children.OfType<AssemblyTreeNode>().Take(3).ToArray();
		assemblies.Length.Should().BeGreaterThanOrEqualTo(3, "need three top-level rows to click");
		var nodeA = assemblies[0];
		var nodeB = assemblies[1];
		var nodeC = assemblies[2];

		vm.AssemblyTreeModel.SelectNode(nodeA);
		for (int i = 0; i < 8; i++)
		{
			Dispatcher.UIThread.RunJobs();
			grid.UpdateLayout();
			await Task.Delay(25);
		}

		DataGridRow Row(SharpTreeNode node) => grid.GetVisualDescendants().OfType<DataGridRow>()
			.First(r => RowNodeEquals(r, node));

		async Task RightClick(SharpTreeNode node)
		{
			var row = Row(node);
			var pt = row.TranslatePoint(new Point(row.Bounds.Width / 2, row.Bounds.Height / 2), window);
			HeadlessWindowExtensions.MouseDown(window, pt!.Value, MouseButton.Right);
			HeadlessWindowExtensions.MouseUp(window, pt.Value, MouseButton.Right);
			for (int i = 0; i < 4; i++)
			{
				Dispatcher.UIThread.RunJobs();
				await Task.Delay(20);
			}
		}

		async Task Dismiss()
		{
			window.KeyPress(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, keySymbol: null);
			for (int i = 0; i < 4; i++)
			{
				Dispatcher.UIThread.RunJobs();
				await Task.Delay(20);
			}
		}

		await RightClick(nodeB);
		Row(nodeB).Classes.Should().Contain("contextTarget", "the first right-click highlights B");

		// Dismiss B's menu, then right-click C — the user's exact sequence.
		await Dismiss();
		await RightClick(nodeC);
		Row(nodeC).Classes.Should().Contain("contextTarget",
			"a second right-click (after dismissing the first menu) must highlight C");
		Row(nodeB).Classes.Should().NotContain("contextTarget",
			"B's highlight must be gone");
	}

	static bool RowNodeEquals(DataGridRow row, SharpTreeNode node)
		=> row.DataContext is HierarchicalNode hn && ReferenceEquals(hn.Item, node);

}
