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

using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Compare;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Compare;

/// <summary>
/// End-to-end render checks for the assembly-comparison view. Pure context-menu and engine
/// tests don't catch the case where the new tab opens but renders empty because the
/// hierarchical DataGrid was never wired with a model — these tests mount the view, walk
/// the visual tree, and assert rows materialise.
/// </summary>
[TestFixture]
public class CompareViewRenderTests
{
	[AvaloniaTest]
	public async Task CompareView_Binds_A_HierarchicalModel_When_Opened()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 2);

		var entry = AppComposition.Current.GetExport<ICSharpCode.ILSpy.ContextMenuEntryRegistry>()
			.Entries.Single(e => e.Metadata.Header == "Compare...").Value;
		var assemblies = new[] {
			await vm.OpenFixtureAsync("FixtureA"),
			await vm.OpenFixtureAsync("FixtureB"),
		};
		var nodes = assemblies.Select(a =>
			(SharpTreeNode)vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(a.ShortName)).ToArray();

		entry.Execute(new TextViewContext { SelectedTreeNodes = nodes });

		var view = await window.WaitForComponent<CompareView>();
		var grid = await view.WaitForComponent<DataGrid>();

		await Waiters.WaitForAsync(() => grid.HierarchicalModel != null,
			description: "CompareView must populate the DataGrid's HierarchicalModel — without it "
				+ "the hierarchical column is rendered over an empty source and the tab shows nothing");
		grid.HierarchicalModel.Should().NotBeNull();
	}

	[AvaloniaTest]
	public async Task CompareView_Renders_Rows_For_The_Merged_Tree()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 2);

		var entry = AppComposition.Current.GetExport<ICSharpCode.ILSpy.ContextMenuEntryRegistry>()
			.Entries.Single(e => e.Metadata.Header == "Compare...").Value;
		var assemblies = new[] {
			await vm.OpenFixtureAsync("FixtureA"),
			await vm.OpenFixtureAsync("FixtureB"),
		};
		var nodes = assemblies.Select(a =>
			(SharpTreeNode)vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(a.ShortName)).ToArray();

		entry.Execute(new TextViewContext { SelectedTreeNodes = nodes });

		var view = await window.WaitForComponent<CompareView>();
		var grid = await view.WaitForComponent<DataGrid>();

		// Default state: ShowIdentical=false, so we expect at least one diff row to be present
		// when comparing two non-empty assemblies against themselves *or* against a sibling.
		// (Comparing an assembly with itself yields zero diffs; if the two assemblies are the
		// same instance we toggle ShowIdentical to force the tree to surface rows.)
		var page = (CompareTabPageModel)view.DataContext!;
		if (ReferenceEquals(page.LeftAssembly, page.RightAssembly))
			page.ShowIdentical = true;

		await Waiters.WaitForAsync(
			() => grid.GetVisualDescendants().OfType<DataGridRow>().Any(),
			description: "the diff grid must render at least one row once the merged tree is bound");
	}

	[AvaloniaTest]
	public async Task CompareView_ShowIdentical_Toggle_Changes_Visible_Row_Set()
	{
		// Comparing an assembly with itself yields only identical rows. With ShowIdentical=false
		// (default) every row should be hidden; flipping ShowIdentical=true must surface them.
		// Regression: the bug was that the HierarchicalModel's ChildrenSelector returned
		// node.Children unfiltered, so IsHidden flags set by ApplyFilterToChild had no effect
		// on what the DataGrid actually rendered.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var assembly = await vm.OpenFixtureAsync();

		// Construct the compare model directly with the same assembly on both sides so every
		// entry collapses to DiffKind.None. The View renders via the standard OpenNewTab path.
		var page = new CompareTabPageModel(assembly, assembly);
		vm.DockWorkspace.OpenNewTab(page);

		var view = await window.WaitForComponent<CompareView>();
		var grid = await view.WaitForComponent<DataGrid>();
		await Waiters.WaitForAsync(() => grid.HierarchicalModel != null);
		global::Avalonia.Threading.Dispatcher.UIThread.RunJobs();

		// Baseline: ShowIdentical=false. All rows are identical-only, so the visible row count
		// must be zero (every entry filters to Hidden).
		page.ShowIdentical.Should().BeFalse("baseline state");
		var rowsBeforeToggle = grid.GetVisualDescendants().OfType<DataGridRow>().Count();
		rowsBeforeToggle.Should().Be(0,
			"with ShowIdentical=false and only-identical content, no rows must be visible");

		// Flip ShowIdentical=true and re-evaluate.
		page.ShowIdentical = true;
		global::Avalonia.Threading.Dispatcher.UIThread.RunJobs();
		await Waiters.WaitForAsync(
			() => grid.GetVisualDescendants().OfType<DataGridRow>().Any(),
			description: "flipping ShowIdentical=true must surface previously-hidden identical rows");

		var rowsAfterToggle = grid.GetVisualDescendants().OfType<DataGridRow>().Count();
		rowsAfterToggle.Should().BeGreaterThan(0,
			"with ShowIdentical=true, identical rows must surface in the grid");

		// Flip back and verify it returns to the hidden state.
		page.ShowIdentical = false;
		global::Avalonia.Threading.Dispatcher.UIThread.RunJobs();
		await Waiters.WaitForAsync(
			() => grid.GetVisualDescendants().OfType<DataGridRow>().Count() == 0,
			description: "flipping ShowIdentical=false again must re-hide all identical rows");
	}
}
