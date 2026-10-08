// Copyright (c) 2026 Christoph Wille
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
using Avalonia.Threading;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.Analyzers;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Controls.TreeView;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Analyzers;

/// <summary>
/// The Analyzer pane's context menu follows the same Thunderbird-style rules as the assembly
/// tree: a right-click on a row outside the selection targets that row alone without moving the
/// selection, the target carries the highlight class while the menu is open, and a keyboard-invoked
/// menu returns the focus to the row it was opened for.
/// </summary>
[TestFixture]
public class AnalyzerTreeContextMenuTests
{
	static async Task<(ICSharpCode.ILSpy.Views.MainWindow Window, AnalyzerTreeView View, SharpTreeView Tree,
		AnalyzerEntityTreeNode First, AnalyzerEntityTreeNode Second)> SetupTwoAnalysedTypesAsync()
	{
		var (window, vm) = await TestHarness.BootAsync(3);
		var dockWorkspace = AppComposition.Current.GetExport<DockWorkspace>();
		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();

		var enumerable = vm.AssemblyTreeModel.FindNode<TypeTreeNode>("System.Linq", "System.Linq", "System.Linq.Enumerable");
		var lookup = vm.AssemblyTreeModel.FindNode<TypeTreeNode>("System.Linq", "System.Linq", "System.Linq.Lookup`2");
		var first = analyzerVm.Analyze((ITypeDefinition)enumerable.Member!);
		var second = analyzerVm.Analyze((ITypeDefinition)lookup.Member!);
		first.IsExpanded = false;
		second.IsExpanded = false;

		dockWorkspace.ShowToolPane(AnalyzerTreeViewModel.PaneContentId);
		var view = await window.WaitForComponent<AnalyzerTreeView>();
		var tree = await view.WaitForComponent<SharpTreeView>();
		// Analyze selects the row it adds; put the selection back on the first row so the second
		// one is the unselected target of the probes below.
		tree.SelectedItem = first;
		await Waiters.WaitForIdleAsync();
		ReferenceEquals(analyzerVm.SelectedItems.SingleOrDefault(), first).Should().BeTrue(
			"precondition: the pane selection must sit on the first analysed row");
		return (window, view, tree, first, second);
	}

	static SharpTreeViewItem? RowFor(SharpTreeView tree, SharpTreeNode node)
		=> tree.GetVisualDescendants().OfType<SharpTreeViewItem>().FirstOrDefault(r => ReferenceEquals(r.Node, node));

	[AvaloniaTest]
	public async Task Right_Clicking_An_Unselected_Analyzer_Row_Targets_It_Without_Moving_The_Selection()
	{
		var (window, _, tree, first, second) = await SetupTwoAnalysedTypesAsync();
		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();
		var menu = tree.ContextMenu!;

		await window.ClickAsync(() => RowFor(tree, second), MouseButton.Right,
			pointInTarget: r => new Point(System.Math.Min(r.Bounds.Width, tree.Bounds.Width) / 2, r.Bounds.Height / 2));
		await Waiters.WaitForAsync(() => menu.IsOpen, description: "the right-clicked analyzer row's context menu to open");

		ReferenceEquals(analyzerVm.SelectedItems.SingleOrDefault(), first).Should().BeTrue(
			"right-clicking an unselected analyzer row must not move the selection");
		RowFor(tree, second)!.Classes.Should().Contain("contextTarget",
			"the right-clicked row must carry the context-target highlight while the menu is open");

		window.KeyPress(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, keySymbol: null);
		await Waiters.WaitForAsync(() => !menu.IsOpen, description: "the context menu to close");
		RowFor(tree, second)!.Classes.Should().NotContain("contextTarget",
			"the transient highlight must clear once the menu closes");
	}

	[AvaloniaTest]
	public async Task Menu_Built_For_A_Right_Clicked_Row_Outside_The_Selection_Targets_Only_That_Row()
	{
		var (_, view, tree, first, second) = await SetupTwoAnalysedTypesAsync();

		TextViewContext? seen = null;
		var export = new StubExport(new RecordingEntry(c => seen = c), new ContextMenuEntryMetadata { Header = "Probe", Order = 0 });

		var built = view.BuildContextMenuForCurrentState(new IContextMenuEntryExport[] { export }, rightClickedNode: second);
		built.Should().NotBeNull("the probe entry must produce a menu");
		var item = built!.Items.OfType<MenuItem>().Single();
		item.RaiseEvent(new Avalonia.Interactivity.RoutedEventArgs(MenuItem.ClickEvent));

		seen.Should().NotBeNull("the probe entry must have been executed");
		ReferenceEquals(seen!.TreeGrid, tree).Should().BeTrue("the context must name the analyzer tree");
		seen.SelectedTreeNodes.Should().BeEquivalentTo(new SharpTreeNode[] { second },
			"a right-click outside the selection acts on the clicked row alone, not on the selection");

		seen = null;
		built = view.BuildContextMenuForCurrentState(new IContextMenuEntryExport[] { export }, rightClickedNode: first);
		built!.Items.OfType<MenuItem>().Single().RaiseEvent(new Avalonia.Interactivity.RoutedEventArgs(MenuItem.ClickEvent));
		seen!.SelectedTreeNodes.Should().BeEquivalentTo(new SharpTreeNode[] { first },
			"a right-click inside the selection acts on the whole selection");
	}

	[AvaloniaTest]
	public async Task Keyboard_Invoked_Analyzer_Menu_Returns_Focus_To_The_Row_On_Close()
	{
		var (window, _, tree, first, _) = await SetupTwoAnalysedTypesAsync();
		var row = RowFor(tree, first);
		row.Should().NotBeNull("the selected analyzer row must be realised");
		row!.Focus(NavigationMethod.Tab);
		Dispatcher.UIThread.RunJobs();

		var focusManager = TopLevel.GetTopLevel(window)!.FocusManager!;
		(focusManager.GetFocusedElement() == row).Should().BeTrue("the row must hold focus before invoking the menu");

		// Keyboard invocation raises ContextRequested with no pointer position (the Shift+F10 / Apps path).
		row.RaiseEvent(new ContextRequestedEventArgs());
		await Waiters.WaitForIdleAsync();
		tree.ContextMenu!.IsOpen.Should().BeTrue("the keyboard gesture must open the analyzer context menu");
		row.Classes.Should().Contain("contextTarget",
			"a keyboard-invoked menu must show the target highlight on the focused row, like the mouse path");

		window.KeyPress(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, keySymbol: null);
		await Waiters.WaitForIdleAsync();

		(focusManager.GetFocusedElement() == row).Should().BeTrue(
			"closing a keyboard-invoked context menu must return focus to the row, not strand it");
		row.Classes.Should().NotContain("contextTarget", "the transient highlight must clear once the menu closes");
	}

	sealed class RecordingEntry(System.Action<TextViewContext> onExecute) : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context) => true;
		public bool IsEnabled(TextViewContext context) => true;
		public void Execute(TextViewContext context) => onExecute(context);
	}

	sealed class StubExport(IContextMenuEntry entry, ContextMenuEntryMetadata metadata) : IContextMenuEntryExport
	{
		public IContextMenuEntry Value { get; } = entry;
		public ContextMenuEntryMetadata Metadata { get; } = metadata;
	}
}
