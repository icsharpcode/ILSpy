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

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class PreviewTabPromotionTests
{

	[AvaloniaTest]
	public void MainTab_Starts_In_Preview_State()
	{
		// The persistent MainTab is the preview slot; tree-node clicks should replace its
		// Content in place until the user explicitly freezes it.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		TestCapture.Step("booted");
		var mainTab = ((ILSpyDockFactory)vm.DockWorkspace.Factory).MainTab!;
		mainTab.IsPreview.Should().BeTrue(
			"the freshly-created MainTab is the preview slot until the user freezes it");
	}

	[AvaloniaTest]
	public async Task Carve_Out_Tabs_Are_Born_Frozen()
	{
		// Open-in-new-tab tabs are explicit user intent — they should never be replaced
		// by subsequent tree-node selections, so they're born frozen (IsPreview=false).
		var (_, vm) = await TestHarness.BootAsync(3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.DockWorkspace.OpenNodeInNewTab(typeNode);
		TestCapture.Step("carve-out-tab-opened");

		var carveOut = vm.DockWorkspace.Documents!.VisibleDockables!
			.OfType<ContentTabPage>()
			.Last(t => t.SourceNode == typeNode);
		carveOut.IsPreview.Should().BeFalse(
			"carve-out tabs are explicit user intent and must survive tree-selection changes");
	}

	[AvaloniaTest]
	public async Task FreezeCurrentTab_Flips_IsPreview_Without_Spawning_A_New_Tab()
	{
		// New semantics: Freeze only flips IsPreview=false on the current MainTab. No new
		// preview tab spawns at freeze time. A fresh preview tab opens lazily later, when a
		// tree-selection change finds the active tab frozen — see
		// Selecting_A_Different_Node_After_Freeze_Opens_A_New_Preview_Tab below.
		var (_, vm) = await TestHarness.BootAsync(3);

		var factory = (ILSpyDockFactory)vm.DockWorkspace.Factory;
		var previousMainTab = factory.MainTab!;
		previousMainTab.IsPreview.Should().BeTrue("baseline");

		// Load content into MainTab so the freeze has something meaningful to keep.
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.AssemblyTreeModel.SelectNode(typeNode);
		vm.DockWorkspace.SettleSelection();
		TestCapture.Step("enumerable-selected");
		ReferenceEquals(previousMainTab.SourceNode, typeNode).Should().BeTrue(
			"baseline: tree selection populated MainTab with the chosen node");

		var tabCountBefore = vm.DockWorkspace.Documents!.VisibleDockables!.Count;

		// Freeze.
		vm.DockWorkspace.FreezeCurrentTab();
		TestCapture.Step("tab-frozen");

		// Tab is now frozen, content preserved.
		previousMainTab.IsPreview.Should().BeFalse(
			"after freeze, the previously-preview MainTab becomes a regular frozen tab");
		ReferenceEquals(previousMainTab.SourceNode, typeNode).Should().BeTrue(
			"freeze must not throw away the tab's content");

		// factory.MainTab still points at the same (now-frozen) tab — no rotation.
		factory.MainTab.Should().BeSameAs(previousMainTab,
			"freeze alone must not rotate factory.MainTab — the frozen tab keeps the slot until a selection change spawns a new preview");

		// And critically: no new tab spawned.
		vm.DockWorkspace.Documents!.VisibleDockables!.Count.Should().Be(tabCountBefore,
			"freezing alone must not open a new tab; new tabs open lazily on the next selection change");
	}

	[AvaloniaTest]
	public async Task Selecting_A_Different_Node_After_Freeze_Opens_A_New_Preview_Tab()
	{
		// Freeze holds the current tab's content. Selecting a different tree node while
		// the (now-frozen) tab is active must spawn a fresh preview tab beside it and
		// route the new content there — never overwrite the frozen tab.
		var (_, vm) = await TestHarness.BootAsync(3);

		var factory = (ILSpyDockFactory)vm.DockWorkspace.Factory;

		// Phase 1: load type A into MainTab.
		var typeA = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.AssemblyTreeModel.SelectNode(typeA);
		vm.DockWorkspace.SettleSelection();
		TestCapture.Step("type-a-selected");
		var frozenTab = factory.MainTab!;
		var frozenContent = frozenTab.Content;

		// Phase 2: freeze. factory.MainTab still points at the same (now-frozen) tab —
		// no spawn yet (covered by FreezeCurrentTab_Flips_IsPreview_Without_Spawning_A_New_Tab).
		var tabCountBeforeSelection = vm.DockWorkspace.Documents!.VisibleDockables!.Count;
		vm.DockWorkspace.FreezeCurrentTab();
		TestCapture.Step("tab-frozen");
		factory.MainTab.Should().BeSameAs(frozenTab, "freeze alone keeps the slot");

		// Phase 3: select a different type — NOW a new preview tab should spawn AND
		// receive the new content.
		var typeB = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Private.Uri", "System", "System.Uri");
		vm.AssemblyTreeModel.SelectNode(typeB);
		vm.DockWorkspace.SettleSelection();
		TestCapture.Step("type-b-opens-new-preview");

		// New tab spawned beside the frozen one.
		vm.DockWorkspace.Documents!.VisibleDockables!.Count.Should().Be(tabCountBeforeSelection + 1,
			"selecting a different node while the active tab is frozen must spawn a new preview tab");

		var newMainTab = factory.MainTab!;
		newMainTab.Should().NotBeSameAs(frozenTab,
			"factory.MainTab must rotate to the freshly-spawned preview tab once a selection change forces it");
		newMainTab.IsPreview.Should().BeTrue(
			"the freshly-spawned tab is itself a preview tab (the user can freeze it next)");
		ReferenceEquals(newMainTab.SourceNode, typeB).Should().BeTrue(
			"new tree selection must land in the freshly-spawned preview tab");
		ReferenceEquals(frozenTab.SourceNode, typeA).Should().BeTrue(
			"the frozen tab must keep type A — not be overwritten by the new selection");
		frozenTab.Content.Should().BeSameAs(frozenContent,
			"frozen tab's Content reference must survive subsequent tree selections");
	}

	[AvaloniaTest]
	public async Task Preview_MainTab_Header_Renders_Italic()
	{
		// Visual contract: the One carries the `previewTab` style class (toggled by
		// PreviewTabClassBehavior from ContentTabPage.IsPreview), and the App.axaml
		// `.previewTab` Style italicises the title. Frozen / tool-pane tabs lack the class
		// and fall through to FontStyle.Normal.
		var (window, vm) = await TestHarness.BootAsync();

		// Wait for the tab strip to realise its items.
		await Waiters.WaitForAsync(() => window.GetVisualDescendants().OfType<DocumentTabStripItem>().Any(),
			System.TimeSpan.FromSeconds(10));

		var factory = (ILSpyDockFactory)vm.DockWorkspace.Factory;
		var mainTabItem = window.GetVisualDescendants().OfType<DocumentTabStripItem>()
			.Single(item => ReferenceEquals(item.DataContext, factory.MainTab));
		TestCapture.Step("before-italic-header-check");

		mainTabItem.Classes.Should().Contain("previewTab",
			"the One must carry the previewTab style class that drives all of its accent visuals");
		mainTabItem.FontStyle.Should().Be(FontStyle.Italic,
			"the .previewTab App.axaml Style must apply, italicising the tab title via FontStyle inheritance");
	}

	[Test]
	public void Active_Tab_Fill_Converter_Is_Purple_For_The_One_And_Blue_For_Others()
	{
		// The One's active highlight is purple, every other document tab's is blue. The
		// :selected:active Background binds to this converter on IsPreview; the (theme-unreliable)
		// :active state makes a rendered assertion flaky, so verify the converter directly.
		var conv = ICSharpCode.ILSpy.Themes.BoolToBrushConverter.PreviewOrActiveTabBackground;
		var culture = global::System.Globalization.CultureInfo.InvariantCulture;
		(conv.Convert(true, typeof(global::Avalonia.Media.IBrush), null, culture)
			as global::Avalonia.Media.ISolidColorBrush)!.Color
			.Should().Be(global::Avalonia.Media.Color.FromRgb(0x9B, 0x59, 0xB6),
				"the One (IsPreview=true) gets the purple active fill");
		(conv.Convert(false, typeof(global::Avalonia.Media.IBrush), null, culture)
			as global::Avalonia.Media.ISolidColorBrush)!.Color
			.Should().Be(global::Avalonia.Media.Color.FromRgb(0x00, 0x7A, 0xCC),
				"any other (frozen) document tab keeps the blue active fill");
	}

	[AvaloniaTest]
	public async Task Freeze_Button_Fits_Inside_The_Tab_When_Close_Button_Is_Visible()
	{
		// The single-tab scenario hides the close button (UpdateLastDocumentCanClose sets
		// CanClose=false), so the freeze has lots of room. The bug shows up only when the
		// close button is visible and competes for horizontal space. Open a carve-out
		// tab so both MainTab and carve-out exist; close button becomes visible; verify
		// the freeze's right edge stays inside the tab's right edge.

		var (window, vm) = await TestHarness.BootAsync(3);

		// Open a carve-out so close buttons become visible on both tabs.
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.DockWorkspace.OpenNodeInNewTab(typeNode);
		TestCapture.Step("carve-out-tab-opened");

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

		var freezeButton = mainTabItem.GetVisualDescendants()
			.OfType<global::Avalonia.Controls.Button>()
			.Single(b => (b.Tag as string) == "PreviewTabFreezeButton");

		// Walk up the visual tree summing each ancestor's bounds origin until we reach
		// mainTabItem — gives the freeze's top-left in tab coordinates without depending on
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
		var freezeTopLeft = OriginRelativeTo(freezeButton, mainTabItem);
		var freezeRight = freezeTopLeft.X + freezeButton.Bounds.Width;
		var tabRight = mainTabItem.Bounds.Width;

		// Also find the close button so we can include it in the failure message for context.
		var closeButton = mainTabItem.GetVisualDescendants()
			.OfType<global::Avalonia.Controls.Button>()
			.Where(b => (b.Tag as string) != "PreviewTabFreezeButton")
			.Select(b => new { b, p = OriginRelativeTo(b, mainTabItem) })
			.OrderByDescending(x => x.p.X)
			.FirstOrDefault();
		var closeInfo = closeButton == null ? "no close button found"
			: $"close at x={closeButton.p.X:0}-{closeButton.p.X + closeButton.b.Bounds.Width:0}";

		TestContext.WriteLine($"tab width={tabRight:0}; freeze at x={freezeTopLeft.X:0}-{freezeRight:0}; {closeInfo}");

		freezeRight.Should().BeLessThanOrEqualTo(tabRight,
			"freeze button's right edge must fit inside the tab's right edge (cut-off bug)");
		if (closeButton != null && closeButton.b.Bounds.Width > 0)
		{
			freezeRight.Should().BeLessThanOrEqualTo(closeButton.p.X + 0.5,
				"freeze button must sit to the LEFT of the close button, not overlap it");
		}
	}

	[AvaloniaTest]
	public async Task Freeze_Button_Fits_Inside_The_Tab_Even_With_A_Long_Title()
	{
		// Production stress case: tabs with long titles can exceed the tab strip's available
		// width if the title isn't capped. The freeze button must still fit inside the tab's
		// right edge regardless of title length.

		var (window, vm) = await TestHarness.BootAsync(3);

		// Open carve-out so close button is visible.
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.DockWorkspace.OpenNodeInNewTab(typeNode);
		TestCapture.Step("carve-out-tab-opened");

		await Waiters.WaitForAsync(() => window.GetVisualDescendants().OfType<DocumentTabStripItem>().Count() >= 2,
			System.TimeSpan.FromSeconds(10));

		var factory = (ILSpyDockFactory)vm.DockWorkspace.Factory;
		var mainTab = factory.MainTab!;
		// Force an absurdly long title to simulate a long type signature.
		mainTab.Title = "This.Is.An.Extremely.Long.Tab.Title.That.Would.Normally.Overflow<Foo, Bar, Baz>";

		global::Avalonia.Threading.Dispatcher.UIThread.RunJobs();
		TestCapture.Step("long-title-applied");
		var mainTabItem = window.GetVisualDescendants().OfType<DocumentTabStripItem>()
			.Single(item => ReferenceEquals(item.DataContext, mainTab));
		// Constrain the tab to 200px (mimics production where the document strip shares
		// width among multiple tabs).
		mainTabItem.MaxWidth = 200;
		mainTabItem.Measure(new global::Avalonia.Size(200, double.PositiveInfinity));
		mainTabItem.Arrange(new global::Avalonia.Rect(0, 0, 200, mainTabItem.DesiredSize.Height));
		global::Avalonia.Threading.Dispatcher.UIThread.RunJobs();

		var freezeButton = mainTabItem.GetVisualDescendants()
			.OfType<global::Avalonia.Controls.Button>()
			.Single(b => (b.Tag as string) == "PreviewTabFreezeButton");

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
		var freezeTopLeft = OriginRelativeTo(freezeButton, mainTabItem);
		var freezeRight = freezeTopLeft.X + freezeButton.Bounds.Width;
		var tabRight = mainTabItem.Bounds.Width;
		TestContext.WriteLine($"long-title scenario: tab={tabRight:0}, freeze x={freezeTopLeft.X:0}-{freezeRight:0}");

		freezeRight.Should().BeLessThanOrEqualTo(tabRight,
			$"freeze must fit inside tab width even with long titles (tab={tabRight:0}, freezeRight={freezeRight:0})");

		// Lock the rendering fixes in:
		// 1. The title StackPanel must clip — otherwise a long title TextBlock measures
		//    at its full unconstrained width and paints over the freeze and close buttons.
		var titleStack = mainTabItem.GetVisualDescendants().OfType<global::Avalonia.Controls.StackPanel>().First();
		titleStack.ClipToBounds.Should().BeTrue(
			"the title StackPanel must clip its content so a long title TextBlock doesn't render over the freeze button");
		// 2. The freeze Button itself must NOT clip — Avalonia Buttons default to ClipToBounds=true
		//    which cuts the right edge of Segoe Fluent Icon glyphs (whose visual extent
		//    exceeds the reported advance width).
		freezeButton.ClipToBounds.Should().BeFalse(
			"freeze button must not clip so the glyph's visual extent (often wider than the font advance width) survives");
	}

	[AvaloniaTest]
	public async Task Inline_Freeze_Button_Appears_On_Preview_Tab_And_Freezes_When_Clicked()
	{
		var (window, vm) = await TestHarness.BootAsync();

		await Waiters.WaitForAsync(() => window.GetVisualDescendants().OfType<DocumentTabStripItem>().Any(),
			System.TimeSpan.FromSeconds(10));

		var factory = (ILSpyDockFactory)vm.DockWorkspace.Factory;
		var mainTabItem = window.GetVisualDescendants().OfType<DocumentTabStripItem>()
			.Single(item => ReferenceEquals(item.DataContext, factory.MainTab));

		// Freeze button is injected via PreviewTabFreezeButtonBehavior — find it by Tag.
		await Waiters.WaitForAsync(() => mainTabItem.GetVisualDescendants()
			.OfType<global::Avalonia.Controls.Button>()
			.Any(b => (b.Tag as string) == "PreviewTabFreezeButton"),
			System.TimeSpan.FromSeconds(5));

		var freezeButton = mainTabItem.GetVisualDescendants()
			.OfType<global::Avalonia.Controls.Button>()
			.Single(b => (b.Tag as string) == "PreviewTabFreezeButton");
		freezeButton.IsVisible.Should().BeTrue("freeze button must show while the tab is preview");

		var previousMainTab = factory.MainTab!;
		var tabCountBefore = vm.DockWorkspace.Documents!.VisibleDockables!.Count;

		// Simulate a user click on the freeze button.
		freezeButton.RaiseEvent(new global::Avalonia.Interactivity.RoutedEventArgs(
			global::Avalonia.Controls.Button.ClickEvent));
		TestCapture.Step("freeze-button-clicked");

		// New semantics: clicking freeze flips IsPreview, doesn't spawn a new tab.
		previousMainTab.IsPreview.Should().BeFalse(
			"clicking the freeze button must flip IsPreview=false on the current MainTab");
		factory.MainTab.Should().BeSameAs(previousMainTab,
			"freeze alone must not rotate factory.MainTab; the new preview tab opens lazily on the next tree selection");
		vm.DockWorkspace.Documents!.VisibleDockables!.Count.Should().Be(tabCountBefore,
			"freeze alone must not change tab count");
	}

	[AvaloniaTest]
	public async Task Freeze_Entry_Appears_In_Tab_Context_Menu_Only_When_Preview()
	{
		// The right-click context-menu Freeze entry must be visible on the preview MainTab
		// and hidden (its IsVisible flips) on frozen carve-out tabs.
		var (window, vm) = await TestHarness.BootAsync(3);
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.DockWorkspace.OpenNodeInNewTab(typeNode);
		TestCapture.Step("carve-out-tab-opened");

		await Waiters.WaitForAsync(() => window.GetVisualDescendants().OfType<DocumentTabStripItem>().Count() >= 2,
			System.TimeSpan.FromSeconds(10));

		var factory = (ILSpyDockFactory)vm.DockWorkspace.Factory;
		var mainTabItem = window.GetVisualDescendants().OfType<DocumentTabStripItem>()
			.Single(item => ReferenceEquals(item.DataContext, factory.MainTab));
		var carveOutItem = window.GetVisualDescendants().OfType<DocumentTabStripItem>()
			.Single(item => item.DataContext is ContentTabPage t && t.SourceNode == typeNode);

		// The menu now also carries Close / Close all but this / Close all; pick the Freeze entry.
		var mainFreezeEntry = mainTabItem.DocumentContextMenu!.Items.OfType<global::Avalonia.Controls.MenuItem>()
			.Single(m => (string?)m.Header == "Freeze tab");
		mainFreezeEntry.IsVisible.Should().BeTrue("MainTab is preview — Freeze tab entry must be visible");

		var carveFreezeEntry = carveOutItem.DocumentContextMenu!.Items.OfType<global::Avalonia.Controls.MenuItem>()
			.Single(m => (string?)m.Header == "Freeze tab");
		carveFreezeEntry.IsVisible.Should().BeFalse(
			"carve-out tabs are already frozen — the Freeze entry must hide");
	}

	[AvaloniaTest]
	public async Task Frozen_Carve_Out_Tab_Header_Renders_Upright()
	{
		// Regression guard for the bug where every DocumentTabStripItem got italicised
		// because the App.axaml setter used a static `FontStyle="Italic"` value instead
		// of the binding to IsPreview. A carve-out tab (IsPreview=false) must render
		// Normal, even when sitting next to a preview MainTab.
		var (window, vm) = await TestHarness.BootAsync(3);

		// Open a carve-out tab via the production code path.
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.DockWorkspace.OpenNodeInNewTab(typeNode);
		TestCapture.Step("carve-out-tab-opened");

		await Waiters.WaitForAsync(() => window.GetVisualDescendants().OfType<DocumentTabStripItem>().Any(),
			System.TimeSpan.FromSeconds(10));

		var carveOutModel = vm.DockWorkspace.Documents!.VisibleDockables!
			.OfType<ContentTabPage>()
			.Last(t => t.SourceNode == typeNode);
		carveOutModel.IsPreview.Should().BeFalse("baseline: carve-out tab is frozen");

		var carveOutItem = window.GetVisualDescendants().OfType<DocumentTabStripItem>()
			.Single(item => ReferenceEquals(item.DataContext, carveOutModel));
		carveOutItem.Classes.Should().NotContain("previewTab",
			"a frozen tab must not carry the previewTab class, so the One's accent styles never touch it");
		carveOutItem.FontStyle.Should().Be(FontStyle.Normal,
			"a frozen (IsPreview=false) document tab must render with upright FontStyle");
	}

	[AvaloniaTest]
	public void FreezeCurrentTab_Is_Noop_When_MainTab_Already_Frozen()
	{
		// Idempotency: a second freeze against an already-frozen MainTab is a no-op. The new
		// "freeze = flip-only, spawn-lazy" semantics make this simpler than the old version:
		// FreezeCurrentMainTab() returns null when MainTab.IsPreview is already false, and
		// FreezeCurrentTab early-returns.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		var factory = (ILSpyDockFactory)vm.DockWorkspace.Factory;
		TestCapture.Step("booted");

		// Manually flip the current MainTab to frozen, simulating the post-freeze state.
		factory.MainTab!.IsPreview = false;
		var before = factory.MainTab;

		vm.DockWorkspace.FreezeCurrentTab();
		TestCapture.Step("freeze-noop");

		factory.MainTab.Should().BeSameAs(before,
			"with no preview MainTab to flip, FreezeCurrentTab must leave the factory state untouched");
		factory.MainTab.IsPreview.Should().BeFalse(
			"the already-frozen tab stays frozen");
	}

	[AvaloniaTest]
	public async Task File_Menu_Separates_MenuCategory_Groups()
	{
		// Regression: the MainMenu builder must emit separators at MenuCategory boundaries
		// (Open / Save / Remove / Exit groups in the File menu, ...). With the menu now living
		// as a NativeMenu, separator rendering is the platform's job; this test pins only
		// the structural claim that separators are inserted between groups.
		var (window, _) = await TestHarness.BootAsync();

		var nativeMenu = global::Avalonia.Controls.NativeMenu.GetMenu(window)
			?? throw new System.InvalidOperationException("MainMenu.Attach should have set NativeMenu on the window");
		var fileMenu = nativeMenu.Items.OfType<global::Avalonia.Controls.NativeMenuItem>()
			.Single(m => string.Equals(m.Header, ICSharpCode.ILSpy.Properties.Resources._File, System.StringComparison.Ordinal));

		var separators = fileMenu.Menu!.Items.OfType<global::Avalonia.Controls.NativeMenuItemSeparator>().ToList();
		separators.Should().HaveCountGreaterThanOrEqualTo(2,
			"File menu's MenuCategory groups (Open / Save / Remove / Exit) must produce at least two separators");
	}

	[AvaloniaTest]
	public async Task Freeze_Button_Inherits_The_Close_Button_Theme()
	{
		// Visual parity: the freeze button should look like the close button — same size, same
		// hover background. Both are plain Avalonia.Controls.Button instances; the close
		// button gets its visual identity from a ControlTheme applied by Dock's tab
		// template. Freeze should copy that Theme rather than carry its own custom style.
		var (window, vm) = await TestHarness.BootAsync(3);

		// Open a carve-out so the close button is visible alongside the freeze (single-tab
		// scenario hides the close button).
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.DockWorkspace.OpenNodeInNewTab(typeNode);
		TestCapture.Step("carve-out-tab-opened");

		await Waiters.WaitForAsync(() => window.GetVisualDescendants().OfType<DocumentTabStripItem>().Count() >= 2,
			System.TimeSpan.FromSeconds(10));
		global::Avalonia.Threading.Dispatcher.UIThread.RunJobs();

		var factory = (ILSpyDockFactory)vm.DockWorkspace.Factory;
		var mainTabItem = window.GetVisualDescendants().OfType<DocumentTabStripItem>()
			.Single(item => ReferenceEquals(item.DataContext, factory.MainTab));

		var freezeButton = mainTabItem.GetVisualDescendants()
			.OfType<global::Avalonia.Controls.Button>()
			.Single(b => (b.Tag as string) == "PreviewTabFreezeButton");
		var closeButton = mainTabItem.GetVisualDescendants()
			.OfType<global::Avalonia.Controls.Button>()
			.FirstOrDefault(b => (b.Tag as string) != "PreviewTabFreezeButton");
		closeButton.Should().NotBeNull("baseline: close button exists as a sibling of the freeze");

		freezeButton.Theme.Should().BeSameAs(closeButton!.Theme,
			"freeze button must use the same ControlTheme as the close button so size + hover bg match");
		freezeButton.Classes.Should().NotContain("preview-freeze",
			"after the Theme-copy refactor the custom class-based styling is unused; the Theme drives all visuals");
	}

	[AvaloniaTest]
	public async Task Close_Button_Has_A_Ctrl_W_Tooltip()
	{
		// Dock's tab template gives the close button no tooltip. PreviewTabFreezeButtonBehavior
		// names its Ctrl+W shortcut so the gesture is discoverable on hover.
		var (window, vm) = await TestHarness.BootAsync(3);

		// Open a carve-out so the close button is realised (single-tab scenario hides it).
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.DockWorkspace.OpenNodeInNewTab(typeNode);
		TestCapture.Step("carve-out-tab-opened");

		await Waiters.WaitForAsync(() => window.GetVisualDescendants().OfType<DocumentTabStripItem>().Count() >= 2,
			System.TimeSpan.FromSeconds(10));
		global::Avalonia.Threading.Dispatcher.UIThread.RunJobs();

		var factory = (ILSpyDockFactory)vm.DockWorkspace.Factory;
		var mainTabItem = window.GetVisualDescendants().OfType<DocumentTabStripItem>()
			.Single(item => ReferenceEquals(item.DataContext, factory.MainTab));

		var closeButton = mainTabItem.GetVisualDescendants()
			.OfType<global::Avalonia.Controls.Button>()
			.FirstOrDefault(b => (b.Tag as string) != "PreviewTabFreezeButton");
		closeButton.Should().NotBeNull("baseline: close button exists as a sibling of the freeze");

		global::Avalonia.Controls.ToolTip.GetTip(closeButton!).Should().Be("Close (Ctrl+W)",
			"the close button must name its Ctrl+W shortcut on hover");
	}

	[AvaloniaTest]
	public async Task Ctrl_W_Closes_The_Active_Document_And_Reforges_The_One()
	{
		// Ctrl+W (CloseActiveDocumentCommand) closes whatever document is active. Closing a frozen
		// carve-out leaves the One untouched; closing the One drops the cached decompiler VM so the
		// next tree selection forges a fresh preview at index 0.
		var (_, vm) = await TestHarness.BootAsync(3);
		var factory = (ILSpyDockFactory)vm.DockWorkspace.Factory;
		var docs = vm.DockWorkspace.Documents!;

		var typeA = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.AssemblyTreeModel.SelectNode(typeA);
		vm.DockWorkspace.SettleSelection();
		var theOne = factory.MainTab!;
		theOne.IsPreview.Should().BeTrue("precondition: the One starts as the preview");

		// A frozen carve-out keeps the dock non-empty so the One isn't the last tab when closed.
		var typeC = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Private.Uri", "System", "System.Uri");
		vm.DockWorkspace.OpenNodeInNewTab(typeC);
		var frozen = docs.VisibleDockables!.OfType<ContentTabPage>().Last(t => t.SourceNode == typeC);

		// Closing the One while it is active removes it from the dock; the frozen tab survives.
		factory.SetActiveDockable(theOne);
		vm.DockWorkspace.CloseActiveDocument();
		docs.VisibleDockables!.Should().NotContain(theOne, "Ctrl+W closes the One when it is active");
		docs.VisibleDockables!.Should().Contain(frozen, "the frozen carve-out must survive closing the One");

		// The next selection forges a brand-new One (the closed one is not resurrected).
		var typeD = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Private.Uri", "System", "System.UriBuilder");
		vm.AssemblyTreeModel.SelectNode(typeD);
		vm.DockWorkspace.SettleSelection();
		var freshOne = factory.MainTab;
		freshOne.Should().NotBeNull("a fresh One is forged on the next selection");
		freshOne.Should().NotBeSameAs(theOne, "the forged One is a new instance, not the closed tab");
		freshOne!.IsPreview.Should().BeTrue("the forged One is a preview tab");

		// Closing a frozen carve-out leaves the (fresh) One untouched.
		factory.SetActiveDockable(frozen);
		vm.DockWorkspace.CloseActiveDocument();
		docs.VisibleDockables!.Should().NotContain(frozen, "Ctrl+W closes the active frozen tab");
		factory.MainTab.Should().BeSameAs(freshOne, "closing a frozen tab must not disturb the One");
	}

	[AvaloniaTest]
	public async Task Tree_Selection_While_Frozen_Tab_Active_Reuses_The_One()
	{
		// Exactly one preview tab ("the One"): even when a frozen tab (carve-out / Options /
		// About) is the active document, a tree-node selection routes to the EXISTING One --
		// reuse + activate it -- and never spawns a second preview. This is the regression test
		// for the stray-preview bug (selecting while a frozen tab was active used to pile up
		// previews).
		var (_, vm) = await TestHarness.BootAsync(3);
		var factory = (ILSpyDockFactory)vm.DockWorkspace.Factory;

		// The One exists from boot.
		var theOne = factory.MainTab!;
		theOne.IsPreview.Should().BeTrue("baseline: the One is a preview tab");

		// Open a carve-out (born frozen) and make it the active dockable.
		var typeA = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.DockWorkspace.OpenNodeInNewTab(typeA);
		var carveOut = vm.DockWorkspace.Documents!.VisibleDockables!
			.OfType<ContentTabPage>()
			.Last(t => t.SourceNode == typeA);
		factory.SetActiveDockable(carveOut);
		TestCapture.Step("carve-out-active");
		carveOut.IsPreview.Should().BeFalse("baseline: carve-out tab is frozen");
		ReferenceEquals(vm.DockWorkspace.Documents.ActiveDockable, carveOut).Should().BeTrue(
			"baseline: the carve-out is the active document");

		var tabCountBefore = vm.DockWorkspace.Documents!.VisibleDockables!.Count;

		// Select a different tree node while the frozen carve-out is active.
		var typeB = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Private.Uri", "System", "System.Uri");
		vm.AssemblyTreeModel.SelectNode(typeB);
		vm.DockWorkspace.SettleSelection();
		TestCapture.Step("type-b-reuses-the-one");

		// No new tab -- the existing One was reused, not a second preview spawned.
		vm.DockWorkspace.Documents!.VisibleDockables!.Count.Should().Be(tabCountBefore,
			"selecting while a frozen tab is active must reuse the One, not spawn a second preview");
		ReferenceEquals(factory.MainTab, theOne).Should().BeTrue(
			"the One must be the same instance -- no fresh preview spawned");
		ReferenceEquals(theOne.SourceNode, typeB).Should().BeTrue(
			"the One now hosts the just-selected node");
		// The frozen carve-out is untouched, and the One was brought to the front.
		ReferenceEquals(carveOut.SourceNode, typeA).Should().BeTrue(
			"the frozen carve-out must keep its original content");
		ReferenceEquals(vm.DockWorkspace.Documents.ActiveDockable, theOne).Should().BeTrue(
			"reusing the One activates it");
	}

	[AvaloniaTest]
	public async Task The_Preview_Is_Always_At_Index_0()
	{
		// The One lives at documents-dock index 0. After it is frozen, the next selection forges
		// a fresh One -- which must again land at index 0 (the frozen ex-One slides to the right).
		var (_, vm) = await TestHarness.BootAsync(3);
		var factory = (ILSpyDockFactory)vm.DockWorkspace.Factory;
		var docs = vm.DockWorkspace.Documents!;

		var typeA = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.AssemblyTreeModel.SelectNode(typeA);
		vm.DockWorkspace.SettleSelection();
		docs.VisibleDockables!.IndexOf(factory.MainTab!).Should().Be(0,
			"the One is the leftmost document tab");

		// Freeze it, then select a different node -> a fresh One must appear at index 0.
		vm.DockWorkspace.FreezeCurrentTab();
		var frozen = factory.MainTab!;
		var typeB = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Private.Uri", "System", "System.Uri");
		vm.AssemblyTreeModel.SelectNode(typeB);
		vm.DockWorkspace.SettleSelection();

		ReferenceEquals(factory.MainTab, frozen).Should().BeFalse("a fresh One was forged");
		factory.MainTab!.IsPreview.Should().BeTrue("the fresh One is a preview tab");
		docs.VisibleDockables!.IndexOf(factory.MainTab!).Should().Be(0,
			"the fresh One must be created at index 0, leftmost");
		docs.VisibleDockables!.IndexOf(frozen).Should().BeGreaterThan(0,
			"the frozen ex-One slides to the right of the new One");
	}

	[AvaloniaTest]
	public async Task Dragging_The_Preview_Freezes_It()
	{
		// The in-strip reorder drag commits through the same-dock MoveDockable; it's overridden so
		// that dragging the One freezes it (instead of refusing the move). Drive that commit
		// directly (what ItemDragHelper does).
		var (_, vm) = await TestHarness.BootAsync(3);
		var factory = (ILSpyDockFactory)vm.DockWorkspace.Factory;
		var docs = vm.DockWorkspace.Documents!;

		var typeA = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.AssemblyTreeModel.SelectNode(typeA);
		vm.DockWorkspace.SettleSelection();
		var theOne = factory.MainTab!;
		theOne.IsPreview.Should().BeTrue("precondition: the One starts as the preview");
		var typeC = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Private.Uri", "System", "System.Uri");
		vm.DockWorkspace.OpenNodeInNewTab(typeC);
		var frozen = docs.VisibleDockables!.OfType<ContentTabPage>().Last(t => t.SourceNode == typeC);
		docs.VisibleDockables!.IndexOf(theOne).Should().Be(0, "precondition: the One is at index 0");

		factory.MoveDockable(docs, theOne, frozen);

		theOne.IsPreview.Should().BeFalse(
			"dragging the One must freeze it -- it becomes an ordinary, movable tab");
		factory.MainTab.Should().BeSameAs(theOne,
			"freeze-on-drag does not rotate MainTab; the next tree selection forges a fresh One");
	}

	[AvaloniaTest]
	public async Task Floating_The_Preview_Out_Freezes_It()
	{
		// Tearing the One out into its own floating window does NOT go through MoveDockable -- the
		// drag-to-float gesture funnels through CreateWindowFrom (FloatDockable -> SplitToWindow ->
		// CreateWindowFrom). That override must freeze the One just like an in-strip drag. Drive
		// CreateWindowFrom directly (what the float gesture ultimately calls).
		var (_, vm) = await TestHarness.BootAsync(3);
		var factory = (ILSpyDockFactory)vm.DockWorkspace.Factory;

		var typeA = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.AssemblyTreeModel.SelectNode(typeA);
		vm.DockWorkspace.SettleSelection();
		var theOne = factory.MainTab!;
		theOne.IsPreview.Should().BeTrue("precondition: the One starts as the preview");

		factory.CreateWindowFrom(theOne);

		theOne.IsPreview.Should().BeFalse(
			"floating the One out must freeze it -- it becomes an ordinary, movable tab");
		factory.MainTab.Should().BeSameAs(theOne,
			"float-out does not rotate MainTab; the next tree selection forges a fresh One");
	}

	[AvaloniaTest]
	public async Task Frozen_Tab_Cannot_Be_Reordered_Before_The_Preview()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var factory = (ILSpyDockFactory)vm.DockWorkspace.Factory;
		var docs = vm.DockWorkspace.Documents!;

		var typeA = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.AssemblyTreeModel.SelectNode(typeA);
		vm.DockWorkspace.SettleSelection();
		var theOne = factory.MainTab!;
		var typeC = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Private.Uri", "System", "System.Uri");
		vm.DockWorkspace.OpenNodeInNewTab(typeC);
		var frozen = docs.VisibleDockables!.OfType<ContentTabPage>().Last(t => t.SourceNode == typeC);

		factory.MoveDockable(docs, frozen, theOne);

		docs.VisibleDockables!.IndexOf(theOne).Should().Be(0,
			"the One must remain at index 0; a frozen tab cannot slip before it");
		docs.VisibleDockables!.IndexOf(frozen).Should().BeGreaterThan(0,
			"the frozen tab is clamped to a position after the One");
	}
}
