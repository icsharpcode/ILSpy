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
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

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
		// leave the real selection (and therefore the preview document) untouched. A ListBox's
		// default selects the row on press, which would yank the preview to B before the menu opens
		// -- so "Decompile to new tab" would end up with B twice instead of the intended A + B. The
		// pane suppresses the right-press to prevent that; this probe asserts the selection stays put.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();

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

		var rowB = grid.GetVisualDescendants().OfType<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeViewItem>()
			.FirstOrDefault(r => RowNodeEquals(r, nodeB));
		rowB.Should().NotBeNull("the top-level CoreLib row must be realised");

		// Suppressing the right-press must not also suppress the context menu — it still opens
		// from ContextRequested (raised on release). Watch for it.
		var menuRequested = false;
		grid.AddHandler(Control.ContextRequestedEvent, (_, _) => menuRequested = true,
			handledEventsToo: true);

		// Right-click the centre of B's row (clear of the far-left expander glyph). Tree rows
		// stretch to content width, so clamp X to the visible grid viewport.
		var clickX = System.Math.Min(rowB!.Bounds.Width, grid.Bounds.Width) / 2;
		var point = rowB.TranslatePoint(new Point(clickX, rowB.Bounds.Height / 2), window);
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
		var grid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();

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

		ICSharpCode.ILSpy.Controls.TreeView.SharpTreeViewItem Row(SharpTreeNode node) => grid.GetVisualDescendants()
			.OfType<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeViewItem>()
			.First(r => RowNodeEquals(r, node));

		async Task RightClick(SharpTreeNode node)
		{
			var row = Row(node);
			var clickX = System.Math.Min(row.Bounds.Width, grid.Bounds.Width) / 2;
			var pt = row.TranslatePoint(new Point(clickX, row.Bounds.Height / 2), window);
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

	[AvaloniaTest]
	public async Task Middle_Clicking_A_Row_Opens_It_In_A_New_Tab()
	{
		// Middle mouse button on a tree row opens that node in a fresh document tab (mirrors the
		// "Decompile to new tab" gesture without going through the context menu).
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();

		var nodeA = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		var nodeB = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(TreeNavigation.CoreLibName);
		vm.AssemblyTreeModel.SelectNode(nodeA);
		for (int i = 0; i < 8; i++)
		{
			Dispatcher.UIThread.RunJobs();
			grid.UpdateLayout();
			await Task.Delay(25);
		}

		var rowB = grid.GetVisualDescendants().OfType<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeViewItem>()
			.First(r => RowNodeEquals(r, nodeB));
		int tabsBefore = vm.DockWorkspace.Documents!.VisibleDockables!.OfType<ContentTabPage>().Count();

		var clickX = System.Math.Min(rowB.Bounds.Width, grid.Bounds.Width) / 2;
		var point = rowB.TranslatePoint(new Point(clickX, rowB.Bounds.Height / 2), window)!.Value;
		HeadlessWindowExtensions.MouseDown(window, point, MouseButton.Middle);
		HeadlessWindowExtensions.MouseUp(window, point, MouseButton.Middle);
		for (int i = 0; i < 8; i++)
		{
			Dispatcher.UIThread.RunJobs();
			grid.UpdateLayout();
			await Task.Delay(20);
		}

		vm.DockWorkspace.Documents!.VisibleDockables!.OfType<ContentTabPage>().Count()
			.Should().BeGreaterThan(tabsBefore, "middle-clicking a row must open it in a new document tab");
		// Activating the new tab pulls the tree selection across to its node.
		ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, nodeB).Should().BeTrue(
			"the new tab opened by middle-click must be sourced from the middle-clicked node");
	}

	[AvaloniaTest]
	public async Task Opening_A_Node_In_A_New_Tab_Syncs_The_Tree_Selection_To_It()
	{
		// Activating the new tab must pull the tree selection over to its node -- both the model
		// selection AND the grid's visual selection. Previously masked because the right-click
		// already moved the selection; now that right-click leaves it put, the tab-activation
		// sync has to do the work.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();

		var nodeA = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		var nodeB = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(TreeNavigation.CoreLibName);

		vm.AssemblyTreeModel.SelectNode(nodeA);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		for (int i = 0; i < 6; i++)
		{
			Dispatcher.UIThread.RunJobs();
			grid.UpdateLayout();
			await Task.Delay(20);
		}

		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var menu = pane.BuildContextMenuForCurrentState(registry.Entries, rightClickedNode: nodeB);
		menu!.ClickItem(Resources.DecompileToNewPanel);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		for (int i = 0; i < 6; i++)
		{
			Dispatcher.UIThread.RunJobs();
			grid.UpdateLayout();
			await Task.Delay(20);
		}

		// The model selection follows the new active tab...
		ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, nodeB).Should().BeTrue(
			"the tree model selection must follow the newly-activated tab");
		// ...and so does the tree's visual selection (SharpTreeView selects the SharpTreeNode directly).
		ReferenceEquals(GridSelectedNode(grid), nodeB).Should().BeTrue(
			"the grid's visual selection must follow the newly-activated tab");
	}

	[AvaloniaTest]
	public async Task Activating_A_Single_Node_Tab_Syncs_The_Tree_After_A_Multi_Node_Selection()
	{
		// Regression: once the tree has held a multi-selection (e.g. a tab decompiled from
		// several nodes), activating a single-node tab must still pull the tree selection over.
		// The SelectedItem setter replaces the collection via Clear()+Add(); for the count>1 case
		// that exposed a transient empty selection, whose grid sync deferred its done-flag and
		// then suppressed the sync for the real value -- so the tree stopped following the tab.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();

		var assemblies = vm.AssemblyTreeModel.Root!.Children.OfType<AssemblyTreeNode>().Take(3).ToArray();
		assemblies.Length.Should().BeGreaterThanOrEqualTo(3, "need three top-level rows");
		var nodeA = assemblies[0];
		var nodeB = assemblies[1];
		var nodeC = assemblies[2];

		// Hold a multi-selection (A + B), as a multi-node decompile tab would.
		vm.AssemblyTreeModel.SelectedItems.Clear();
		vm.AssemblyTreeModel.SelectedItems.Add(nodeA);
		vm.AssemblyTreeModel.SelectedItems.Add(nodeB);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		for (int i = 0; i < 6; i++)
		{
			Dispatcher.UIThread.RunJobs();
			grid.UpdateLayout();
			await Task.Delay(20);
		}
		vm.AssemblyTreeModel.SelectedItems.Count.Should().Be(2, "precondition: a multi-selection is held");

		// Open C in a new tab -> activates it -> the tree must follow to C.
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var menu = pane.BuildContextMenuForCurrentState(registry.Entries, rightClickedNode: nodeC);
		menu!.ClickItem(Resources.DecompileToNewPanel);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		for (int i = 0; i < 6; i++)
		{
			Dispatcher.UIThread.RunJobs();
			grid.UpdateLayout();
			await Task.Delay(20);
		}

		ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, nodeC).Should().BeTrue(
			"the tree model selection must follow the newly-activated single-node tab");
		vm.AssemblyTreeModel.SelectedItems.Count.Should().Be(1,
			"activating the single-node tab must collapse the multi-selection to that one node");
		ReferenceEquals(GridSelectedNode(grid), nodeC).Should().BeTrue(
			"the grid's visual selection must follow even after a prior multi-selection");
	}

	[AvaloniaTest]
	public async Task Activating_A_Multi_Node_Tab_Restores_Its_Multi_Selection_In_The_Tree()
	{
		// A tab decompiled from several nodes carries no single SourceNode, so activating it has
		// to restore the WHOLE set in the tree -- not nothing, and not just one. Scenario: select
		// two nodes (multi-node preview tab), open a third in a new tab, then re-activate the
		// multi-node tab; the tree selection must come back to both original nodes.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();
		var documents = ((ILSpyDockFactory)vm.DockWorkspace.Factory).Documents!;

		var assemblies = vm.AssemblyTreeModel.Root!.Children.OfType<AssemblyTreeNode>().Take(3).ToArray();
		assemblies.Length.Should().BeGreaterThanOrEqualTo(3, "need three top-level rows");
		var nodeA = assemblies[0];
		var nodeB = assemblies[1];
		var nodeC = assemblies[2];

		// 1) Multi-select A + B -> the active (preview) tab decompiles both.
		vm.AssemblyTreeModel.SelectedItems.Clear();
		vm.AssemblyTreeModel.SelectedItems.Add(nodeA);
		vm.AssemblyTreeModel.SelectedItems.Add(nodeB);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		var multiTab = (ContentTabPage)documents.ActiveDockable!;

		// 2) Open C in a new (single-node) tab -> it becomes active, tree -> C.
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var menu = pane.BuildContextMenuForCurrentState(registry.Entries, rightClickedNode: nodeC);
		menu!.ClickItem(Resources.DecompileToNewPanel);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		ReferenceEquals(multiTab, documents.ActiveDockable).Should().BeFalse(
			"precondition: the new single-node tab is active, not the multi-node one");

		// 3) Re-activate the multi-node tab.
		vm.DockWorkspace.Factory.SetActiveDockable(multiTab);
		for (int i = 0; i < 8; i++)
		{
			Dispatcher.UIThread.RunJobs();
			grid.UpdateLayout();
			await Task.Delay(20);
		}

		// 4) The tree selection must contain BOTH original nodes again -- in the model...
		vm.AssemblyTreeModel.SelectedItems.Should().Contain(nodeA)
			.And.Contain(nodeB);
		vm.AssemblyTreeModel.SelectedItems.Count.Should().Be(2,
			"re-activating the multi-node tab restores exactly its two nodes");
		// ...and visually in the grid (both rows highlighted, not just the primary).
		GridSelectedNodes(grid).Should().Contain(nodeA).And.Contain(nodeB);
		GridSelectedNodes(grid).Should().HaveCount(2,
			"the grid must visually highlight every restored node");
	}

	[AvaloniaTest]
	public async Task DecompileInNewView_On_A_Metadata_Table_Opens_Its_Grid_Not_A_Code_Tab()
	{
		// "Decompile to new tab" on a node with custom content (a metadata table) must open that
		// node's own page-type — the metadata grid — not force-decompile it into an empty code tab.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();

		var tableNode = vm.AssemblyTreeModel.FindCoreLib()
			.GetChild<ICSharpCode.ILSpy.Metadata.MetadataTreeNode>()
			.GetChild<ICSharpCode.ILSpy.Metadata.MetadataTablesTreeNode>()
			.GetChild<ICSharpCode.ILSpy.Metadata.CorTables.TypeDefTableTreeNode>();

		var documents = ((ILSpyDockFactory)vm.DockWorkspace.Factory).Documents!;
		int before = documents.VisibleDockables?.OfType<ContentTabPage>().Count() ?? 0;

		var menu = pane.BuildContextMenuForCurrentState(registry.Entries, rightClickedNode: tableNode);
		menu.Should().NotBeNull();
		menu!.ClickItem(Resources.DecompileToNewPanel);

		await Waiters.WaitForAsync(
			() => documents.VisibleDockables!.OfType<ContentTabPage>()
				.Any(t => t.Content is ICSharpCode.ILSpy.ViewModels.MetadataTablePageModel));

		documents.VisibleDockables!.OfType<ContentTabPage>().Count()
			.Should().BeGreaterThan(before, "a new tab must open");
		var newTab = documents.VisibleDockables!.OfType<ContentTabPage>()
			.First(t => t.Content is ICSharpCode.ILSpy.ViewModels.MetadataTablePageModel);
		newTab.Content.Should().BeOfType<ICSharpCode.ILSpy.ViewModels.MetadataTablePageModel>(
			"the metadata-table node must open its grid page, not a decompiler tab");
	}

	static bool RowNodeEquals(ICSharpCode.ILSpy.Controls.TreeView.SharpTreeViewItem row, SharpTreeNode node)
		=> ReferenceEquals(row.DataContext, node);

	static SharpTreeNode? GridSelectedNode(ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView grid)
		=> grid.SelectedItem as SharpTreeNode;

	static System.Collections.Generic.List<SharpTreeNode> GridSelectedNodes(ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView grid)
		=> grid.SelectedItems!.Cast<object?>()
			.OfType<SharpTreeNode>()
			.ToList();
}
