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

using ILSpy;
using ILSpy.AppEnv;
using ILSpy.Commands;
using ILSpy.Compare;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

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

		var entry = AppComposition.Current.GetExport<global::ILSpy.ContextMenuEntryRegistry>()
			.Entries.Single(e => e.Metadata.Header == "Compare...").Value;
		var assemblies = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.Where(a => a.IsLoadedAsValidAssembly).Take(2).ToList();
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

		var entry = AppComposition.Current.GetExport<global::ILSpy.ContextMenuEntryRegistry>()
			.Entries.Single(e => e.Metadata.Header == "Compare...").Value;
		var assemblies = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.Where(a => a.IsLoadedAsValidAssembly).Take(2).ToList();
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
}
