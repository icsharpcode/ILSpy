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

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX.TreeView;

using ILSpy;
using ILSpy.Analyzers;
using ILSpy.AppEnv;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Analyzers;

[TestFixture]
public class AnalyzeContextMenuTests
{
	[AvaloniaTest]
	public async Task Analyze_Entry_Is_Registered_With_The_Localised_Header()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		registry.Entries.Should().Contain(e => e.Metadata.Header == nameof(Resources.Analyze),
			"the analyze context-menu entry must reach the right-click menu through the MEF registry");
	}

	[AvaloniaTest]
	public async Task Analyze_Entry_Is_Visible_For_Member_Tree_Nodes_And_Hidden_For_AssemblyTreeNode()
	{
		// AnalyzeContextMenuEntry visibility contract: visible when every selected node is
		// an IMemberTreeNode (types, methods, fields, properties, events), hidden otherwise.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var entry = registry.Entries
			.Single(e => e.Metadata.Header == nameof(Resources.Analyze))
			.Value;

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		entry.IsVisible(new TextViewContext { SelectedTreeNodes = new[] { (SharpTreeNode)typeNode } })
			.Should().BeTrue("a type selection must surface the Analyze entry");

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		entry.IsVisible(new TextViewContext { SelectedTreeNodes = new[] { (SharpTreeNode)assemblyNode } })
			.Should().BeFalse("AssemblyTreeNode isn't an IMemberTreeNode — Analyze must hide");

		entry.IsVisible(new TextViewContext { SelectedTreeNodes = null })
			.Should().BeFalse("no selection means no entity to analyse");
		entry.IsVisible(new TextViewContext { SelectedTreeNodes = System.Array.Empty<SharpTreeNode>() })
			.Should().BeFalse("an empty selection must hide the entry");
	}

	[AvaloniaTest]
	public async Task Executing_Analyze_Pushes_The_Selected_Entity_Into_The_Analyzer_Pane()
	{
		// Execute calls AnalyzerTreeViewModel.Analyze for every IMemberTreeNode in the
		// selection. The pane's Root.Children must hold one AnalyzedTypeTreeNode wrapping
		// the same type definition after the call.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var entry = registry.Entries
			.Single(e => e.Metadata.Header == nameof(Resources.Analyze))
			.Value;

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");

		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();
		var beforeCount = analyzerVm.Root.Children.Count;

		entry.Execute(new TextViewContext { SelectedTreeNodes = new[] { (SharpTreeNode)typeNode } });

		analyzerVm.Root.Children.Count.Should().Be(beforeCount + 1,
			"Execute on the analyze entry must add exactly one new row to the pane root");
		(analyzerVm.Root.Children.Last() is AnalyzerEntityTreeNode).Should().BeTrue(
			"the new pane row must be an entity wrapper that exposes the analysed member");
	}

	[AvaloniaTest]
	public async Task Pressing_Ctrl_R_On_The_Assembly_Tree_Analyses_The_Selected_Member()
	{
		// Ctrl+R while a member is selected on the assembly tree pane must surface the
		// member in the analyzer pane — the same end-state as right-click + Analyze.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var pane = await window.WaitForComponent<global::ILSpy.AssemblyTree.AssemblyListPane>();
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.AssemblyTreeModel.SelectNode(typeNode);

		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();
		var beforeCount = analyzerVm.Root.Children.Count;

		var grid = await pane.WaitForComponent<global::Avalonia.Controls.DataGrid>();
		grid.RaiseEvent(new global::Avalonia.Input.KeyEventArgs {
			Key = global::Avalonia.Input.Key.R,
			KeyModifiers = global::Avalonia.Input.KeyModifiers.Control,
			RoutedEvent = global::Avalonia.Input.InputElement.KeyDownEvent,
			Source = grid,
		});

		analyzerVm.Root.Children.Count.Should().Be(beforeCount + 1,
			"Ctrl+R with a type selected must analyse it");
	}

	[AvaloniaTest]
	public async Task Execute_Surfaces_The_Analyzer_Pane_So_Hidden_Panes_Become_Visible()
	{
		// Logged follow-up from earlier live-testing: analysing an entity adds it to the
		// pane's Root.Children, but if the pane is hidden the user doesn't see anything
		// happen. Execute should call DockWorkspace.ShowToolPane so the pane surfaces.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var dockWorkspace = AppComposition.Current.GetExport<global::ILSpy.Docking.DockWorkspace>();
		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();
		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>()
			.Entries.Single(e => e.Metadata.Header == nameof(Resources.Analyze)).Value;

		// Pick a method that won't already be in the analyzer pane.
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var methodNode = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Count");

		// Walk the dock layout to find the analyzer pane's current visibility state.
		var analyzerPane = FindAnalyzerPane(dockWorkspace);
		Assert.That(analyzerPane, Is.Not.Null, "the analyzer pane must be in the dock layout");
		var owningDock = analyzerPane!.Owner as global::Dock.Model.Core.IDock;
		Assert.That(owningDock, Is.Not.Null, "the analyzer pane must have a dock parent");

		// Pick any sibling dockable and force it to be active first, so the analyzer pane
		// starts in the inactive state — that's the regression we're testing.
		var sibling = owningDock!.VisibleDockables?
			.FirstOrDefault(d => !ReferenceEquals(d, analyzerPane));
		if (sibling != null)
		{
			owningDock.ActiveDockable = sibling;
			((object?)owningDock.ActiveDockable).Should().NotBeSameAs(analyzerPane,
				"baseline: sibling-active must really make the analyzer pane inactive");
		}

		entry.Execute(new TextViewContext {
			SelectedTreeNodes = new ICSharpCode.ILSpyX.TreeView.SharpTreeNode[] { methodNode }
		});

		// After Execute, the active dockable in the analyzer pane's owning dock should be
		// the analyzer pane itself — that's what ShowToolPane does.
		((object?)owningDock.ActiveDockable).Should().BeSameAs(analyzerPane,
			"Execute must call ShowToolPane so the analyzer pane surfaces");
	}

	[AvaloniaTest]
	public async Task Every_Analyzer_Tree_Row_Surfaces_A_Non_Null_Icon()
	{
		// Logged follow-up from earlier live-testing: analyzer rows render without icons.
		// The Analyzed{Type,Method,Field,Property,Event,Accessor,Module}TreeNode types all
		// have Icon overrides returning real IImages. The "Used By" / "Uses" search-result
		// header rows (AnalyzerSearchTreeNode), however, didn't override Icon and inherited
		// null from SharpTreeNode — so the row's Image element rendered empty even though
		// the leaf result rows below had perfectly good icons. Every materialised node in
		// the analyzer tree must report a non-null Icon for the cell template to render.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();
		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>()
			.Entries.Single(e => e.Metadata.Header == nameof(Resources.Analyze)).Value;

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var methodNode = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");

		entry.Execute(new TextViewContext {
			SelectedTreeNodes = new ICSharpCode.ILSpyX.TreeView.SharpTreeNode[] { methodNode }
		});

		// Walk every materialised node under the analyzer root and assert each has Icon.
		var entityRow = analyzerVm.Root.Children.OfType<AnalyzerEntityTreeNode>().Last();
		((object?)entityRow.Icon).Should().NotBeNull("the analysed-entity row needs an icon");
		entityRow.EnsureLazyChildren();
		foreach (var searchHeader in entityRow.Children.OfType<AnalyzerSearchTreeNode>())
		{
			((object?)searchHeader.Icon).Should().NotBeNull(
				$"'{searchHeader.AnalyzerHeader}' analyzer-search row needs an icon — was the regression");
		}
	}

	static AnalyzerTreeViewModel? FindAnalyzerPane(global::ILSpy.Docking.DockWorkspace dockWorkspace)
	{
		foreach (var dockable in WalkDockables(dockWorkspace.Layout))
		{
			if (dockable is AnalyzerTreeViewModel apm)
				return apm;
		}
		return null;
	}

	static System.Collections.Generic.IEnumerable<global::Dock.Model.Core.IDockable> WalkDockables(global::Dock.Model.Core.IDockable? root)
	{
		if (root == null)
			yield break;
		yield return root;
		if (root is global::Dock.Model.Core.IDock dock && dock.VisibleDockables != null)
			foreach (var child in dock.VisibleDockables)
				foreach (var descendant in WalkDockables(child))
					yield return descendant;
	}
}
