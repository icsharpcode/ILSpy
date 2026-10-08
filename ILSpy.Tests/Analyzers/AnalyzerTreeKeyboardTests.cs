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
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.Threading;

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;

using ICSharpCode.ILSpy.Analyzers;
using ICSharpCode.ILSpy.Analyzers.TreeNodes;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Analyzers;

[TestFixture]
public class AnalyzerTreeKeyboardTests
{
	[AvaloniaTest]
	public async Task Right_Key_Expands_A_Node_In_The_Analyzer_Tree()
	{
		// The analyzer tree is a SharpTreeView like the assembly tree, so the standard tree gestures
		// work there too via SharpTreeView.OnKeyDown -- here, Right expands the focused node.
		var (window, vm) = await TestHarness.BootAsync(3);
		var dockWorkspace = AppComposition.Current.GetExport<DockWorkspace>();
		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		var entity = (ITypeDefinition)typeNode.Member!;
		var analyzed = analyzerVm.Analyze(entity); // adds the node, auto-expands it, and selects it
												   // Analyze auto-expands the new node; collapse it again so this test can exercise Right-to-expand.
		analyzed.IsExpanded = false;

		dockWorkspace.ShowToolPane(AnalyzerTreeViewModel.PaneContentId);
		var view = await window.WaitForComponent<ICSharpCode.ILSpy.Analyzers.AnalyzerTreeView>();
		var tree = await view.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();
		tree.Focus();
		Dispatcher.UIThread.RunJobs();

		analyzed.IsExpanded.Should().BeFalse("precondition: the node was collapsed for this test");

		window.KeyPress(Key.Right, RawInputModifiers.None, PhysicalKey.ArrowRight, null);
		await Waiters.WaitForAsync(() => analyzed.IsExpanded,
			description: "Right must expand the node via SharpTreeView.OnKeyDown on the analyzer tree");
	}

	[AvaloniaTest]
	public async Task Enter_Activates_The_Selected_Analyzer_Node()
	{
		// Enter on a single selected analyzer row activates it -- for an entity node that means
		// navigating to the member's home in the assembly tree, like 10.x did. The key must reach
		// SharpTreeView.OnKeyDown: the container is a ListBoxItem, and Avalonia's default key
		// selection triggers treat Enter/Space as selection input and mark the event handled
		// before it bubbles, so SharpTreeView suppresses that trigger for the activation case.
		var (window, vm) = await TestHarness.BootAsync(3);
		var dockWorkspace = AppComposition.Current.GetExport<DockWorkspace>();
		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		var entity = (ITypeDefinition)typeNode.Member!;
		var analyzed = analyzerVm.Analyze(entity);

		dockWorkspace.ShowToolPane(AnalyzerTreeViewModel.PaneContentId);
		var view = await window.WaitForComponent<ICSharpCode.ILSpy.Analyzers.AnalyzerTreeView>();
		var tree = await view.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();
		tree.SelectedItem = analyzed;
		Dispatcher.UIThread.RunJobs();
		tree.FocusNode(analyzed);
		Dispatcher.UIThread.RunJobs();

		((object?)vm.AssemblyTreeModel.SelectedItem).Should().NotBeSameAs(typeNode,
			"precondition: the assembly tree must not already sit on the target node");

		window.KeyPress(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null);
		await Waiters.WaitForAsync(() => ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, typeNode),
			description: "Enter must activate the analyzer node and select the type in the assembly tree");
	}

	[AvaloniaTest]
	public async Task Delete_Removes_The_Selected_Top_Level_Analyzer_Node()
	{
		// Delete on a selected top-level analyzer row removes it from the pane (the keyboard
		// equivalent of the "Remove" context-menu entry). Rows below the top level are not
		// deletable, so Delete on one of them leaves the pane untouched.
		var (window, vm) = await TestHarness.BootAsync(3);
		var dockWorkspace = AppComposition.Current.GetExport<DockWorkspace>();
		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		var analyzed = analyzerVm.Analyze((ITypeDefinition)typeNode.Member!);
		analyzed.IsExpanded = true;
		var child = analyzed.Children.First();

		dockWorkspace.ShowToolPane(AnalyzerTreeViewModel.PaneContentId);
		var view = await window.WaitForComponent<ICSharpCode.ILSpy.Analyzers.AnalyzerTreeView>();
		var tree = await view.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();

		tree.SelectedItem = child;
		Dispatcher.UIThread.RunJobs();
		tree.FocusNode(child);
		Dispatcher.UIThread.RunJobs();
		window.KeyPress(Key.Delete, RawInputModifiers.None, PhysicalKey.Delete, null);
		Dispatcher.UIThread.RunJobs();
		analyzed.Children.Should().Contain(child, "Delete must not remove a nested analyzer row");
		analyzerVm.Root.Children.Should().Contain(analyzed, "Delete on a nested row must not remove its top-level node");

		tree.SelectedItem = analyzed;
		Dispatcher.UIThread.RunJobs();
		tree.FocusNode(analyzed);
		Dispatcher.UIThread.RunJobs();
		window.KeyPress(Key.Delete, RawInputModifiers.None, PhysicalKey.Delete, null);
		await Waiters.WaitForAsync(() => !analyzerVm.Root.Children.Contains(analyzed),
			description: "Delete must remove the selected top-level analyzer node from the pane");
	}

	[AvaloniaTest]
	public async Task Ctrl_R_Analyzes_The_Selected_Member()
	{
		// Ctrl+R on the assembly tree analyzes the selected member(s) -- the keyboard equivalent of the
		// Analyze context-menu entry. Here a selected type lands as a node in the analyzer pane.
		var (window, vm) = await TestHarness.BootAsync(3);
		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();
		var pane = await window.WaitForComponent<ICSharpCode.ILSpy.AssemblyTree.AssemblyListPane>();
		var tree = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.AssemblyTreeModel.SelectNode(typeNode);
		await Waiters.WaitForIdleAsync();
		tree.Focus();
		Dispatcher.UIThread.RunJobs();

		int before = analyzerVm.Root.Children.Count;
		window.KeyPress(Key.R, RawInputModifiers.Control, PhysicalKey.R, null);
		await Waiters.WaitForAsync(() => analyzerVm.Root.Children.Count > before,
			description: "Ctrl+R must add an analyzer node for the selected member");

		analyzerVm.Root.Children.OfType<AnalyzerEntityTreeNode>()
			.Any(n => n.Member is { } m && m.MetadataToken == ((ITypeDefinition)typeNode.Member!).MetadataToken)
			.Should().BeTrue("the analyzer pane must hold a node for the type that Ctrl+R analyzed");
	}

	[AvaloniaTest]
	public async Task Ctrl_R_On_An_Analyzer_Result_Row_Promotes_It_Instead_Of_The_Assembly_Tree_Selection()
	{
		// Ctrl+R with the focus inside the Analyzer pane analyzes the pane's own selection (a result
		// row becomes a top-level entry), like the pane-level binding did in 10.x. The window-level
		// Ctrl+R, which analyzes the assembly tree's selection, must not fire for the pane.
		var (window, vm) = await TestHarness.BootAsync(3);
		var dockWorkspace = AppComposition.Current.GetExport<DockWorkspace>();
		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var method = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty").MethodDefinition;
		var analyzed = analyzerVm.Analyze((ITypeDefinition)typeNode.Member!);
		analyzed.EnsureLazyChildren();
		// A result row lives underneath an analyzer-search header, never directly under the root.
		var searchRow = analyzed.Children.OfType<AnalyzerSearchTreeNode>().First();
		var resultRow = new AnalyzedMethodTreeNode(method, typeNode.Member);
		searchRow.Children.Add(resultRow);
		analyzed.IsExpanded = true;
		searchRow.IsExpanded = true;

		// Park the assembly tree on another analysable type: if the window binding fired instead,
		// this is what would land in the pane.
		var decoy = vm.AssemblyTreeModel.FindNode<TypeTreeNode>("System.Linq", "System.Linq", "System.Linq.Lookup`2");
		vm.AssemblyTreeModel.SelectNode(decoy);
		await Waiters.WaitForIdleAsync();

		dockWorkspace.ShowToolPane(AnalyzerTreeViewModel.PaneContentId);
		var view = await window.WaitForComponent<ICSharpCode.ILSpy.Analyzers.AnalyzerTreeView>();
		var tree = await view.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();
		tree.SelectedItem = resultRow;
		Dispatcher.UIThread.RunJobs();
		tree.FocusNode(resultRow);
		Dispatcher.UIThread.RunJobs();
		((object?)analyzerVm.SelectedItems.SingleOrDefault()).Should().BeSameAs(resultRow,
			"precondition: the pane selection must sit on the result row");

		int before = analyzerVm.Root.Children.Count;
		window.KeyPress(Key.R, RawInputModifiers.Control, PhysicalKey.R, null);
		await Waiters.WaitForAsync(() => analyzerVm.Root.Children.Count > before,
			description: "Ctrl+R in the Analyzer pane must promote the selected result row");

		var promoted = analyzerVm.Root.Children.OfType<AnalyzerEntityTreeNode>().Last();
		promoted.Member.Should().BeSameAs(method, "the pane's own selection is what Ctrl+R analyzes");
		analyzerVm.Root.Children.OfType<AnalyzerEntityTreeNode>()
			.Any(n => n.Member is { } m && m.MetadataToken == ((ITypeDefinition)decoy.Member!).MetadataToken)
			.Should().BeFalse("the assembly tree's selection must not be analyzed when the key is pressed inside the pane");
	}
}
