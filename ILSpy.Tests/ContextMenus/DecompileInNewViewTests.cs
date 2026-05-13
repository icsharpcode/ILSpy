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
using Avalonia.Interactivity;

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

		window.CaptureForReview(1, "initial-no-selection");

		// With an assembly node selected → the menu surfaces "Decompile in new tab".
		vm.AssemblyTreeModel.SelectNode(asm);
		window.CaptureForReview(2, "assembly-selected");
		var withSelection = pane.BuildContextMenuForCurrentState(registry.Entries);
		withSelection.Should().NotBeNull();
		withSelection!.Items.OfType<MenuItem>().Select(i => (string?)i.Header)
			.Should().Contain(header);

		// With no selection → the entry is filtered out of the built menu.
		vm.AssemblyTreeModel.SelectedItems.Clear();
		window.CaptureForReview(3, "selection-cleared");
		var withoutSelection = pane.BuildContextMenuForCurrentState(registry.Entries);
		// `Build` returns null when no entry would be visible; otherwise it may still build a
		// menu (other entries don't gate on selection). Either way, the header must not appear.
		(withoutSelection?.Items.OfType<MenuItem>().Select(i => (string?)i.Header) ?? System.Linq.Enumerable.Empty<string?>())
			.Should().NotContain(header);
	}

	// audit (2026-05-12): converted 📦 → 🧪. Was firing `entry.Execute(synthetic ctx)`
	// directly; now selects the second method via the model and clicks the
	// "Decompile in new tab" MenuItem produced by the live menu-build path. The Click
	// routed event hits the same handler ContextMenuProvider attached during Build —
	// matching what the user's right-click → menu-item-click triggers.
	[AvaloniaTest]
	public async Task Clicking_DecompileInNewView_Spawns_A_New_Tab_For_The_Selected_Method()
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

		// Seed an existing decompile in the active tab so we have a "current" tab on screen
		// before the gesture; that tab will be reused by the right-click's selection move
		// (production behaviour — right-clicking a row in the DataGrid moves selection there
		// before the menu opens), so the assertion we care about is "a *new* tab spawned in
		// addition to the existing one and shows the dispatched method".
		vm.AssemblyTreeModel.SelectNode(firstMethod);
		var firstTab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		window.CaptureForReview(1, "first-method-decompiled-into-active-tab");

		var pane = await window.WaitForComponent<AssemblyListPane>();
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var documents = ((ILSpyDockFactory)vm.DockWorkspace.Factory).Documents!;
		var initialCount = documents.VisibleDockables?.Count ?? 0;

		// Select the second method and click the menu entry through the live context-menu path.
		vm.AssemblyTreeModel.SelectNode(secondMethod);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		window.CaptureForReview(2, "second-method-replaced-active-tab");

		var menu = pane.BuildContextMenuForCurrentState(registry.Entries);
		menu.Should().NotBeNull();
		var item = menu!.Items.OfType<MenuItem>()
			.Single(i => (string?)i.Header == Resources.DecompileToNewPanel);
		item.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));

		await Waiters.WaitForAsync(
			() => (documents.VisibleDockables?.Count ?? 0) > initialCount);
		var newTab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		ReferenceEquals(newTab, firstTab).Should().BeFalse(
			"a fresh decompiler tab must be created instead of reusing the existing one");
		newTab.Text.Should().Contain("Empty");
		// Count grew by one — the click added a tab on top of the right-click selection's reuse
		// of firstTab. (The right-click selection move into secondMethod re-uses firstTab; the
		// menu-click then creates an additional tab. Net effect: +1 dockable.)
		documents.VisibleDockables!.Count.Should().Be(initialCount + 1);
		window.CaptureForReview(3, "after-click-new-tab-spawned");
	}
}
