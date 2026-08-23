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
using System.Reflection.Metadata;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.Analyzers;
using ICSharpCode.ILSpy.Analyzers.TreeNodes;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Analyzers;

[TestFixture]
public class AnalyzeContextMenuTests
{
	[AvaloniaTest]
	public async Task Analyze_Entry_Is_Registered_With_The_Localised_Header()
	{
		await TestHarness.BootAsync();

		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		registry.Entries.Should().Contain(e => e.Metadata.Header == nameof(Resources.Analyze),
			"the analyze context-menu entry must reach the right-click menu through the MEF registry");
	}

	[AvaloniaTest]
	public async Task Analyze_Entry_Is_Visible_For_Member_Tree_Nodes_And_Hidden_For_AssemblyTreeNode()
	{
		// AnalyzeContextMenuEntry visibility contract: visible when every selected node is
		// an IMemberTreeNode (types, methods, fields, properties, events), hidden otherwise.

		var (_, vm) = await TestHarness.BootAsync();

		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>()
			.GetEntry(nameof(Resources.Analyze));

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
	public async Task Analyze_Is_Visible_And_Works_For_A_Clicked_Code_Reference()
	{
		// Right-clicking a resolved symbol (IEntity) in the decompiled code -- not a tree node --
		// must also surface Analyze and push that entity into the analyzer pane.
		var (_, vm) = await TestHarness.BootAsync();
		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>()
			.GetEntry(nameof(Resources.Analyze));

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		var entity = (IEntity)typeNode.Member!;
		var context = new TextViewContext { Reference = new ReferenceSegment { Reference = entity } };

		entry.IsVisible(context).Should().BeTrue("a clicked code reference to an entity must surface Analyze");
		entry.IsEnabled(context).Should().BeTrue();

		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();
		var before = analyzerVm.Root.Children.Count;
		entry.Execute(context);
		analyzerVm.Root.Children.Count.Should().Be(before + 1,
			"analyzing a clicked code reference must add it to the analyzer pane");
	}

	[AvaloniaTest]
	public async Task Executing_Analyze_Pushes_The_Selected_Entity_Into_The_Analyzer_Pane()
	{
		// Execute calls AnalyzerTreeViewModel.Analyze for every IMemberTreeNode in the
		// selection. The pane's Root.Children must hold one AnalyzedTypeTreeNode wrapping
		// the same type definition after the call.

		var (_, vm) = await TestHarness.BootAsync();

		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>()
			.GetEntry(nameof(Resources.Analyze));

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");

		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();
		var beforeCount = analyzerVm.Root.Children.Count;

		entry.Execute(new TextViewContext { SelectedTreeNodes = new[] { (SharpTreeNode)typeNode } });
		TestCapture.Step("analyze-executed");

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

		var (window, vm) = await TestHarness.BootAsync();

		var pane = await window.WaitForComponent<ICSharpCode.ILSpy.AssemblyTree.AssemblyListPane>();
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.AssemblyTreeModel.SelectNode(typeNode);
		TestCapture.Step("type-selected");

		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();
		var beforeCount = analyzerVm.Root.Children.Count;

		var grid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();
		grid.RaiseEvent(new global::Avalonia.Input.KeyEventArgs {
			Key = global::Avalonia.Input.Key.R,
			KeyModifiers = global::Avalonia.Input.KeyModifiers.Control,
			RoutedEvent = global::Avalonia.Input.InputElement.KeyDownEvent,
			Source = grid,
		});
		TestCapture.Step("ctrl-r-pressed");

		analyzerVm.Root.Children.Count.Should().Be(beforeCount + 1,
			"Ctrl+R with a type selected must analyse it");
	}

	[AvaloniaTest]
	public async Task Execute_Surfaces_The_Analyzer_Pane_So_Hidden_Panes_Become_Visible()
	{
		// Logged follow-up from earlier live-testing: analysing an entity adds it to the
		// pane's Root.Children, but if the pane is hidden the user doesn't see anything
		// happen. Execute should call DockWorkspace.ShowToolPane so the pane surfaces.

		var (_, vm) = await TestHarness.BootAsync();

		var dockWorkspace = AppComposition.Current.GetExport<ICSharpCode.ILSpy.Docking.DockWorkspace>();
		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();
		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>()
			.GetEntry(nameof(Resources.Analyze));

		// Pick a method that won't already be in the analyzer pane.
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var methodNode = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Count");

		// The analyzer pane is hidden by default — not in the layout until something surfaces it.
		FindAnalyzerPane(dockWorkspace).Should().BeNull(
			"baseline: the analyzer pane is hidden until invoked");

		entry.Execute(new TextViewContext {
			SelectedTreeNodes = new ICSharpCode.ILSpyX.TreeView.SharpTreeNode[] { methodNode }
		});
		TestCapture.Step("analyzer-pane-surfaced");

		// After Execute, the pane is materialised into the layout and active in its dock —
		// that's what ShowToolPane does, so the user sees the entity they just analysed.
		var analyzerPane = FindAnalyzerPane(dockWorkspace);
		analyzerPane.Should().NotBeNull("Execute must surface (materialise) the analyzer pane");
		var owningDock = analyzerPane!.Owner as global::Dock.Model.Core.IDock;
		owningDock.Should().NotBeNull("the surfaced analyzer pane must have a dock parent");
		((object?)owningDock!.ActiveDockable).Should().BeSameAs(analyzerPane,
			"Execute must call ShowToolPane so the analyzer pane surfaces and is active");
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

		var (_, vm) = await TestHarness.BootAsync();

		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();
		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>()
			.GetEntry(nameof(Resources.Analyze));

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var methodNode = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");

		entry.Execute(new TextViewContext {
			SelectedTreeNodes = new ICSharpCode.ILSpyX.TreeView.SharpTreeNode[] { methodNode }
		});
		TestCapture.Step("analyzer-results-populated");

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

	[AvaloniaTest]
	public async Task Analyze_Promotes_An_Analyzer_Result_Row_To_A_Top_Level_Entry()
	{
		// Right-click on a result row inside the analyzer pane (a "Used By" hit, say) must
		// offer Analyze and, on Execute, add that row's entity as a new top-level entry.
		var (_, vm) = await TestHarness.BootAsync();
		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>()
			.GetEntry(nameof(Resources.Analyze));
		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var method = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty").MethodDefinition;

		entry.Execute(new TextViewContext { SelectedTreeNodes = new SharpTreeNode[] { typeNode } });
		var rootRow = analyzerVm.Root.Children.OfType<AnalyzerEntityTreeNode>().Last();
		rootRow.EnsureLazyChildren();
		// A result row lives underneath an analyzer-search header, never directly under the root.
		var resultRow = new AnalyzedMethodTreeNode(method, typeNode.Member);
		rootRow.Children.OfType<AnalyzerSearchTreeNode>().First().Children.Add(resultRow);

		var context = new TextViewContext { SelectedTreeNodes = new SharpTreeNode[] { resultRow } };
		entry.IsVisible(context).Should().BeTrue("an analyzer result row wraps an entity, so Analyze must be offered");
		entry.IsEnabled(context).Should().BeTrue();

		var before = analyzerVm.Root.Children.Count;
		entry.Execute(context);
		TestCapture.Step("result-row-analyzed");

		analyzerVm.Root.Children.Count.Should().Be(before + 1, "the result row's entity must become a top-level entry");
		var promoted = analyzerVm.Root.Children.OfType<AnalyzerEntityTreeNode>().Last();
		promoted.Member.Should().BeSameAs(method);
		((object)analyzerVm.SelectedItems.Single()).Should().BeSameAs(promoted);
	}

	[AvaloniaTest]
	public async Task Analyze_Is_Hidden_For_A_Top_Level_Analyzer_Row()
	{
		// A top-level analyzer row is already analysed; re-analysing it would be a no-op, so the
		// entry stays hidden there (Remove is the entry offered for those rows).
		var (_, vm) = await TestHarness.BootAsync();
		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>()
			.GetEntry(nameof(Resources.Analyze));
		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		entry.Execute(new TextViewContext { SelectedTreeNodes = new SharpTreeNode[] { typeNode } });
		var rootRow = analyzerVm.Root.Children.OfType<AnalyzerEntityTreeNode>().Last();

		entry.IsVisible(new TextViewContext { SelectedTreeNodes = new SharpTreeNode[] { rootRow } })
			.Should().BeFalse("a top-level analyzer row is already analysed");
	}

	[AvaloniaTest]
	public async Task Analyze_Reuses_The_Row_When_The_Same_Entity_Comes_From_Another_Type_System()
	{
		// Analyzer result rows carry entities from the type system each analyzer run builds, so
		// the same member reaches the pane as different IEntity/IModule instances depending on
		// whether it was analysed from the assembly tree or from a result row. Both must land on
		// the same top-level row.
		var (_, vm) = await TestHarness.BootAsync();
		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var method = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty").MethodDefinition;
		var first = analyzerVm.Analyze(method);
		var count = analyzerVm.Root.Children.Count;

		var file = method.ParentModule!.MetadataFile!;
		var otherTypeSystem = new DecompilerTypeSystem(file, file.GetAssemblyResolver());
		var other = otherTypeSystem.MainModule.GetDefinition((MethodDefinitionHandle)method.MetadataToken);
		other.ParentModule.Should().NotBeSameAs(method.ParentModule, "the test must exercise the cross-type-system case");

		var second = analyzerVm.Analyze(other);
		TestCapture.Step("same-entity-other-type-system");

		((object)second).Should().BeSameAs(first, "the existing row must be reused");
		analyzerVm.Root.Children.Count.Should().Be(count);
		((object)analyzerVm.SelectedItems.Single()).Should().BeSameAs(first);
	}

	static AnalyzerTreeViewModel? FindAnalyzerPane(ICSharpCode.ILSpy.Docking.DockWorkspace dockWorkspace)
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
