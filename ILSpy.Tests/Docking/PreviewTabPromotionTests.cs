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
using Avalonia.Media;
using Avalonia.VisualTree;

using AwesomeAssertions;

using Dock.Avalonia.Controls;

using ILSpy.AppEnv;
using ILSpy.Docking;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class PreviewTabPromotionTests
{
	[AvaloniaTest]
	public void MainTab_Starts_In_Preview_State()
	{
		// The persistent MainTab is the preview slot; tree-node clicks should replace its
		// Content in place until the user explicitly pins it.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		var mainTab = ((ILSpyDockFactory)vm.DockWorkspace.Factory).MainTab!;
		mainTab.IsPreview.Should().BeTrue(
			"the freshly-created MainTab is the preview slot until the user pins it");
	}

	[AvaloniaTest]
	public async Task Carve_Out_Tabs_Are_Born_Pinned()
	{
		// Open-in-new-tab tabs are explicit user intent — they should never be replaced
		// by subsequent tree-node selections, so they're born pinned (IsPreview=false).
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.DockWorkspace.OpenNodeInNewTab(typeNode);

		var carveOut = vm.DockWorkspace.Documents!.VisibleDockables!
			.OfType<ContentTabPage>()
			.Last(t => t.SourceNode == typeNode);
		carveOut.IsPreview.Should().BeFalse(
			"carve-out tabs are explicit user intent and must survive tree-selection changes");
	}

	[AvaloniaTest]
	public async Task PinCurrentTab_Flips_IsPreview_Without_Spawning_A_New_Tab()
	{
		// New semantics: Pin only flips IsPreview=false on the current MainTab. No new
		// preview tab spawns at pin time. A fresh preview tab opens lazily later, when a
		// tree-selection change finds the active tab frozen — see
		// Selecting_A_Different_Node_After_Pin_Opens_A_New_Preview_Tab below.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var factory = (ILSpyDockFactory)vm.DockWorkspace.Factory;
		var previousMainTab = factory.MainTab!;
		previousMainTab.IsPreview.Should().BeTrue("baseline");

		// Load content into MainTab so the pin has something meaningful to keep.
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.AssemblyTreeModel.SelectNode(typeNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync(System.TimeSpan.FromSeconds(60));
		ReferenceEquals(previousMainTab.SourceNode, typeNode).Should().BeTrue(
			"baseline: tree selection populated MainTab with the chosen node");

		var tabCountBefore = vm.DockWorkspace.Documents!.VisibleDockables!.Count;

		// Pin.
		vm.DockWorkspace.PinCurrentTab();

		// Tab is now pinned, content preserved.
		previousMainTab.IsPreview.Should().BeFalse(
			"after pin, the previously-preview MainTab becomes a regular pinned tab");
		ReferenceEquals(previousMainTab.SourceNode, typeNode).Should().BeTrue(
			"pin must not throw away the tab's content");

		// factory.MainTab still points at the same (now-pinned) tab — no rotation.
		factory.MainTab.Should().BeSameAs(previousMainTab,
			"pin alone must not rotate factory.MainTab — the pinned tab keeps the slot until a selection change spawns a new preview");

		// And critically: no new tab spawned.
		vm.DockWorkspace.Documents!.VisibleDockables!.Count.Should().Be(tabCountBefore,
			"pinning alone must not open a new tab; new tabs open lazily on the next selection change");
	}

	[AvaloniaTest]
	public async Task Selecting_A_Different_Node_After_Pin_Opens_A_New_Preview_Tab()
	{
		// Pin holds the current tab's content. Selecting a different tree node while
		// the (now-pinned) tab is active must spawn a fresh preview tab beside it and
		// route the new content there — never overwrite the pinned tab.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var factory = (ILSpyDockFactory)vm.DockWorkspace.Factory;

		// Phase 1: load type A into MainTab.
		var typeA = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.AssemblyTreeModel.SelectNode(typeA);
		await vm.DockWorkspace.WaitForDecompiledTextAsync(System.TimeSpan.FromSeconds(60));
		var pinnedTab = factory.MainTab!;
		var pinnedContent = pinnedTab.Content;

		// Phase 2: pin. factory.MainTab still points at the same (now-pinned) tab —
		// no spawn yet (covered by PinCurrentTab_Flips_IsPreview_Without_Spawning_A_New_Tab).
		var tabCountBeforeSelection = vm.DockWorkspace.Documents!.VisibleDockables!.Count;
		vm.DockWorkspace.PinCurrentTab();
		factory.MainTab.Should().BeSameAs(pinnedTab, "pin alone keeps the slot");

		// Phase 3: select a different type — NOW a new preview tab should spawn AND
		// receive the new content.
		var typeB = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Private.Uri", "System", "System.Uri");
		vm.AssemblyTreeModel.SelectNode(typeB);
		await vm.DockWorkspace.WaitForDecompiledTextAsync(System.TimeSpan.FromSeconds(60));

		// New tab spawned beside the pinned one.
		vm.DockWorkspace.Documents!.VisibleDockables!.Count.Should().Be(tabCountBeforeSelection + 1,
			"selecting a different node while the active tab is pinned must spawn a new preview tab");

		var newMainTab = factory.MainTab!;
		newMainTab.Should().NotBeSameAs(pinnedTab,
			"factory.MainTab must rotate to the freshly-spawned preview tab once a selection change forces it");
		newMainTab.IsPreview.Should().BeTrue(
			"the freshly-spawned tab is itself a preview tab (the user can pin it next)");
		ReferenceEquals(newMainTab.SourceNode, typeB).Should().BeTrue(
			"new tree selection must land in the freshly-spawned preview tab");
		ReferenceEquals(pinnedTab.SourceNode, typeA).Should().BeTrue(
			"the pinned tab must keep type A — not be overwritten by the new selection");
		pinnedTab.Content.Should().BeSameAs(pinnedContent,
			"pinned tab's Content reference must survive subsequent tree selections");
	}

	[AvaloniaTest]
	public async Task Preview_MainTab_Header_Renders_Italic()
	{
		// Visual contract: the DocumentTabStripItem for the preview MainTab binds its
		// FontStyle to ContentTabPage.IsPreview via BoolToFontStyleConverter.Italic.
		// Pinned tabs and tool-pane tabs fall through to FontStyle.Normal.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		// Wait for the tab strip to realise its items.
		await Waiters.WaitForAsync(() => window.GetVisualDescendants().OfType<DocumentTabStripItem>().Any(),
			System.TimeSpan.FromSeconds(10));

		var factory = (ILSpyDockFactory)vm.DockWorkspace.Factory;
		var mainTabItem = window.GetVisualDescendants().OfType<DocumentTabStripItem>()
			.Single(item => ReferenceEquals(item.DataContext, factory.MainTab));

		mainTabItem.FontStyle.Should().Be(FontStyle.Italic,
			"the App.axaml Style for DocumentTabStripItem must apply, italicising the tab title via FontStyle inheritance");
	}

	[AvaloniaTest]
	public async Task Preview_MainTab_Header_Has_Left_Edge_Accent_Stripe()
	{
		// Border companion to the italic check: the preview MainTab carries a non-null
		// BorderBrush and a 3px left-only BorderThickness via the accent-stripe converters.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		await ((MainWindowViewModel)window.DataContext!).AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		await Waiters.WaitForAsync(() => window.GetVisualDescendants().OfType<DocumentTabStripItem>().Any(),
			System.TimeSpan.FromSeconds(10));

		var factory = (ILSpyDockFactory)((MainWindowViewModel)window.DataContext!).DockWorkspace.Factory;
		var mainTabItem = window.GetVisualDescendants().OfType<DocumentTabStripItem>()
			.Single(item => ReferenceEquals(item.DataContext, factory.MainTab));

		((object?)mainTabItem.BorderBrush).Should().NotBeNull(
			"the preview MainTab must carry the accent BorderBrush");
		mainTabItem.BorderThickness.Should().Be(new global::Avalonia.Thickness(3, 0, 0, 0),
			"the preview MainTab must carry the left-only 3px accent stripe");
	}

	[AvaloniaTest]
	public async Task Pin_Button_Fits_Inside_The_Tab_When_Close_Button_Is_Visible()
	{
		// The single-tab scenario hides the close button (UpdateLastDocumentCanClose sets
		// CanClose=false), so the pin has lots of room. The bug shows up only when the
		// close button is visible and competes for horizontal space. Open a carve-out
		// tab so both MainTab and carve-out exist; close button becomes visible; verify
		// the pin's right edge stays inside the tab's right edge.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		// Open a carve-out so close buttons become visible on both tabs.
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.DockWorkspace.OpenNodeInNewTab(typeNode);

		await Waiters.WaitForAsync(() => window.GetVisualDescendants().OfType<DocumentTabStripItem>().Count() >= 2,
			System.TimeSpan.FromSeconds(10));
		global::Avalonia.Threading.Dispatcher.UIThread.RunJobs();

		var factory = (ILSpyDockFactory)vm.DockWorkspace.Factory;
		var mainTabItem = window.GetVisualDescendants().OfType<DocumentTabStripItem>()
			.Single(item => ReferenceEquals(item.DataContext, factory.MainTab));

		// Force layout so Bounds are up to date.
		mainTabItem.Measure(global::Avalonia.Size.Infinity);
		mainTabItem.Arrange(new global::Avalonia.Rect(mainTabItem.DesiredSize));
		global::Avalonia.Threading.Dispatcher.UIThread.RunJobs();

		var pinButton = mainTabItem.GetVisualDescendants()
			.OfType<global::Avalonia.Controls.Button>()
			.Single(b => (b.Tag as string) == "PreviewTabPinButton");

		// Walk up the visual tree summing each ancestor's bounds origin until we reach
		// mainTabItem — gives the pin's top-left in tab coordinates without depending on
		// TranslatePoint (which lives on different namespaces across Avalonia versions).
		static global::Avalonia.Point OriginRelativeTo(global::Avalonia.Visual node, global::Avalonia.Visual ancestor)
		{
			double x = 0, y = 0;
			var current = node;
			while (current != null && !ReferenceEquals(current, ancestor))
			{
				x += current.Bounds.X;
				y += current.Bounds.Y;
				current = current.GetVisualParent();
			}
			return new global::Avalonia.Point(x, y);
		}
		var pinTopLeft = OriginRelativeTo(pinButton, mainTabItem);
		var pinRight = pinTopLeft.X + pinButton.Bounds.Width;
		var tabRight = mainTabItem.Bounds.Width;

		// Also find the close button so we can include it in the failure message for context.
		var closeButton = mainTabItem.GetVisualDescendants()
			.OfType<global::Avalonia.Controls.Button>()
			.Where(b => (b.Tag as string) != "PreviewTabPinButton")
			.Select(b => new { b, p = OriginRelativeTo(b, mainTabItem) })
			.OrderByDescending(x => x.p.X)
			.FirstOrDefault();
		var closeInfo = closeButton == null ? "no close button found"
			: $"close at x={closeButton.p.X:0}-{closeButton.p.X + closeButton.b.Bounds.Width:0}";

		TestContext.WriteLine($"tab width={tabRight:0}; pin at x={pinTopLeft.X:0}-{pinRight:0}; {closeInfo}");

		pinRight.Should().BeLessThanOrEqualTo(tabRight,
			"pin button's right edge must fit inside the tab's right edge (cut-off bug)");
		if (closeButton != null && closeButton.b.Bounds.Width > 0)
		{
			pinRight.Should().BeLessThanOrEqualTo(closeButton.p.X + 0.5,
				"pin button must sit to the LEFT of the close button, not overlap it");
		}
	}

	[AvaloniaTest]
	public async Task Pin_Button_Fits_Inside_The_Tab_Even_With_A_Long_Title()
	{
		// Production stress case: tabs with long titles can exceed the tab strip's available
		// width if the title isn't capped. The pin button must still fit inside the tab's
		// right edge regardless of title length.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		// Open carve-out so close button is visible.
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.DockWorkspace.OpenNodeInNewTab(typeNode);

		await Waiters.WaitForAsync(() => window.GetVisualDescendants().OfType<DocumentTabStripItem>().Count() >= 2,
			System.TimeSpan.FromSeconds(10));

		var factory = (ILSpyDockFactory)vm.DockWorkspace.Factory;
		var mainTab = factory.MainTab!;
		// Force an absurdly long title to simulate a long type signature.
		mainTab.Title = "This.Is.An.Extremely.Long.Tab.Title.That.Would.Normally.Overflow<Foo, Bar, Baz>";

		global::Avalonia.Threading.Dispatcher.UIThread.RunJobs();
		var mainTabItem = window.GetVisualDescendants().OfType<DocumentTabStripItem>()
			.Single(item => ReferenceEquals(item.DataContext, mainTab));
		// Constrain the tab to 200px (mimics production where the document strip shares
		// width among multiple tabs).
		mainTabItem.MaxWidth = 200;
		mainTabItem.Measure(new global::Avalonia.Size(200, double.PositiveInfinity));
		mainTabItem.Arrange(new global::Avalonia.Rect(0, 0, 200, mainTabItem.DesiredSize.Height));
		global::Avalonia.Threading.Dispatcher.UIThread.RunJobs();

		var pinButton = mainTabItem.GetVisualDescendants()
			.OfType<global::Avalonia.Controls.Button>()
			.Single(b => (b.Tag as string) == "PreviewTabPinButton");

		static global::Avalonia.Point OriginRelativeTo(global::Avalonia.Visual node, global::Avalonia.Visual ancestor)
		{
			double x = 0, y = 0;
			var current = node;
			while (current != null && !ReferenceEquals(current, ancestor))
			{
				x += current.Bounds.X;
				y += current.Bounds.Y;
				current = current.GetVisualParent();
			}
			return new global::Avalonia.Point(x, y);
		}
		var pinTopLeft = OriginRelativeTo(pinButton, mainTabItem);
		var pinRight = pinTopLeft.X + pinButton.Bounds.Width;
		var tabRight = mainTabItem.Bounds.Width;
		TestContext.WriteLine($"long-title scenario: tab={tabRight:0}, pin x={pinTopLeft.X:0}-{pinRight:0}");

		pinRight.Should().BeLessThanOrEqualTo(tabRight,
			$"pin must fit inside tab width even with long titles (tab={tabRight:0}, pinRight={pinRight:0})");

		// Lock the rendering fixes in:
		// 1. The title StackPanel must clip — otherwise a long title TextBlock measures
		//    at its full unconstrained width and paints over the pin and close buttons.
		var titleStack = mainTabItem.GetVisualDescendants().OfType<global::Avalonia.Controls.StackPanel>().First();
		titleStack.ClipToBounds.Should().BeTrue(
			"the title StackPanel must clip its content so a long title TextBlock doesn't render over the pin button");
		// 2. The pin Button itself must NOT clip — Avalonia Buttons default to ClipToBounds=true
		//    which cuts the right edge of Segoe Fluent Icon glyphs (whose visual extent
		//    exceeds the reported advance width).
		pinButton.ClipToBounds.Should().BeFalse(
			"pin button must not clip so the glyph's visual extent (often wider than the font advance width) survives");
	}

	[AvaloniaTest]
	public async Task Inline_Pin_Button_Appears_On_Preview_Tab_And_Pins_When_Clicked()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		await Waiters.WaitForAsync(() => window.GetVisualDescendants().OfType<DocumentTabStripItem>().Any(),
			System.TimeSpan.FromSeconds(10));

		var factory = (ILSpyDockFactory)vm.DockWorkspace.Factory;
		var mainTabItem = window.GetVisualDescendants().OfType<DocumentTabStripItem>()
			.Single(item => ReferenceEquals(item.DataContext, factory.MainTab));

		// Pin button is injected via PreviewTabPinButtonBehavior — find it by Tag.
		await Waiters.WaitForAsync(() => mainTabItem.GetVisualDescendants()
			.OfType<global::Avalonia.Controls.Button>()
			.Any(b => (b.Tag as string) == "PreviewTabPinButton"),
			System.TimeSpan.FromSeconds(5));

		var pinButton = mainTabItem.GetVisualDescendants()
			.OfType<global::Avalonia.Controls.Button>()
			.Single(b => (b.Tag as string) == "PreviewTabPinButton");
		pinButton.IsVisible.Should().BeTrue("pin button must show while the tab is preview");

		var previousMainTab = factory.MainTab!;
		var tabCountBefore = vm.DockWorkspace.Documents!.VisibleDockables!.Count;

		// Simulate a user click on the pin button.
		pinButton.RaiseEvent(new global::Avalonia.Interactivity.RoutedEventArgs(
			global::Avalonia.Controls.Button.ClickEvent));

		// New semantics: clicking pin flips IsPreview, doesn't spawn a new tab.
		previousMainTab.IsPreview.Should().BeFalse(
			"clicking the pin button must flip IsPreview=false on the current MainTab");
		factory.MainTab.Should().BeSameAs(previousMainTab,
			"pin alone must not rotate factory.MainTab; the new preview tab opens lazily on the next tree selection");
		vm.DockWorkspace.Documents!.VisibleDockables!.Count.Should().Be(tabCountBefore,
			"pin alone must not change tab count");
	}

	[AvaloniaTest]
	public async Task Pin_Entry_Appears_In_Tab_Context_Menu_Only_When_Preview()
	{
		// The right-click context-menu Pin entry must be visible on the preview MainTab
		// and hidden (its IsVisible flips) on pinned carve-out tabs.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.DockWorkspace.OpenNodeInNewTab(typeNode);

		await Waiters.WaitForAsync(() => window.GetVisualDescendants().OfType<DocumentTabStripItem>().Count() >= 2,
			System.TimeSpan.FromSeconds(10));

		var factory = (ILSpyDockFactory)vm.DockWorkspace.Factory;
		var mainTabItem = window.GetVisualDescendants().OfType<DocumentTabStripItem>()
			.Single(item => ReferenceEquals(item.DataContext, factory.MainTab));
		var carveOutItem = window.GetVisualDescendants().OfType<DocumentTabStripItem>()
			.Single(item => item.DataContext is ContentTabPage t && t.SourceNode == typeNode);

		var mainPinEntry = mainTabItem.DocumentContextMenu!.Items.OfType<global::Avalonia.Controls.MenuItem>().Single();
		mainPinEntry.Header.Should().Be("Pin tab");
		mainPinEntry.IsVisible.Should().BeTrue("MainTab is preview — Pin tab entry must be visible");

		var carvePinEntry = carveOutItem.DocumentContextMenu!.Items.OfType<global::Avalonia.Controls.MenuItem>().Single();
		carvePinEntry.IsVisible.Should().BeFalse(
			"carve-out tabs are already pinned — the Pin entry must hide");
	}

	[AvaloniaTest]
	public async Task Pinned_Carve_Out_Tab_Header_Renders_Upright()
	{
		// Regression guard for the bug where every DocumentTabStripItem got italicised
		// because the App.axaml setter used a static `FontStyle="Italic"` value instead
		// of the binding to IsPreview. A carve-out tab (IsPreview=false) must render
		// Normal, even when sitting next to a preview MainTab.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		// Open a carve-out tab via the production code path.
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.DockWorkspace.OpenNodeInNewTab(typeNode);

		await Waiters.WaitForAsync(() => window.GetVisualDescendants().OfType<DocumentTabStripItem>().Any(),
			System.TimeSpan.FromSeconds(10));

		var carveOutModel = vm.DockWorkspace.Documents!.VisibleDockables!
			.OfType<ContentTabPage>()
			.Last(t => t.SourceNode == typeNode);
		carveOutModel.IsPreview.Should().BeFalse("baseline: carve-out tab is pinned");

		var carveOutItem = window.GetVisualDescendants().OfType<DocumentTabStripItem>()
			.Single(item => ReferenceEquals(item.DataContext, carveOutModel));
		carveOutItem.FontStyle.Should().Be(FontStyle.Normal,
			"a pinned (IsPreview=false) document tab must render with upright FontStyle");
	}

	[AvaloniaTest]
	public void PinCurrentTab_Is_Noop_When_MainTab_Already_Pinned()
	{
		// Idempotency: a second pin against an already-pinned MainTab is a no-op. The new
		// "pin = flip-only, spawn-lazy" semantics make this simpler than the old version:
		// PinCurrentMainTab() returns null when MainTab.IsPreview is already false, and
		// PinCurrentTab early-returns.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		var factory = (ILSpyDockFactory)vm.DockWorkspace.Factory;

		// Manually flip the current MainTab to pinned, simulating the post-pin state.
		factory.MainTab!.IsPreview = false;
		var before = factory.MainTab;

		vm.DockWorkspace.PinCurrentTab();

		factory.MainTab.Should().BeSameAs(before,
			"with no preview MainTab to flip, PinCurrentTab must leave the factory state untouched");
		factory.MainTab.IsPreview.Should().BeFalse(
			"the already-pinned tab stays pinned");
	}

	[AvaloniaTest]
	public async Task Tree_Selection_While_Frozen_Tab_Active_Opens_A_New_Preview_Tab()
	{
		// "Frozen" covers: (a) user-pinned tabs (IsPreview=false via PinCurrentTab),
		// (b) carve-out tabs (born IsPreview=false via OpenNodeInNewTab), and (c) static
		// content tabs (Options, About) whose Content is a static viewmodel. Tree-node
		// selections while any of those are active must spawn a fresh preview tab and
		// route the new content there. This test uses a carve-out as a stand-in for the
		// frozen-tab family — the IsWritablePreview check returns null for all three.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var factory = (ILSpyDockFactory)vm.DockWorkspace.Factory;

		// Open a carve-out (born pinned) and let it become the active dockable.
		var typeA = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.DockWorkspace.OpenNodeInNewTab(typeA);
		var carveOut = vm.DockWorkspace.Documents!.VisibleDockables!
			.OfType<ContentTabPage>()
			.Last(t => t.SourceNode == typeA);
		factory.SetActiveDockable(carveOut);
		carveOut.IsPreview.Should().BeFalse("baseline: carve-out tab is frozen");
		ReferenceEquals(vm.DockWorkspace.Documents.ActiveDockable, carveOut).Should().BeTrue(
			"baseline: the carve-out is the active document");

		var tabCountBefore = vm.DockWorkspace.Documents!.VisibleDockables!.Count;

		// Select a different tree node — the carve-out is frozen, so this must NOT
		// overwrite it.
		var typeB = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Private.Uri", "System", "System.Uri");
		vm.AssemblyTreeModel.SelectNode(typeB);
		await vm.DockWorkspace.WaitForDecompiledTextAsync(System.TimeSpan.FromSeconds(60));

		vm.DockWorkspace.Documents!.VisibleDockables!.Count.Should().Be(tabCountBefore + 1,
			"selecting a tree node while a frozen tab is active must spawn a new preview tab");
		ReferenceEquals(carveOut.SourceNode, typeA).Should().BeTrue(
			"the frozen carve-out tab must keep its original content");

		var newPreview = factory.MainTab!;
		newPreview.Should().NotBeSameAs(carveOut,
			"the freshly-spawned preview must be a separate tab from the frozen one");
		newPreview.IsPreview.Should().BeTrue(
			"the freshly-spawned tab is itself a preview tab");
		ReferenceEquals(newPreview.SourceNode, typeB).Should().BeTrue(
			"the new preview tab must host the just-selected node's content");
	}
}
