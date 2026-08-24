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
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;

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
public class BrowseBackForwardCommandTests
{
	[AvaloniaTest]
	public void BrowseBack_And_BrowseForward_Are_Registered_In_View_Menu_With_Alt_Arrow_Gestures()
	{
		// Task 26: BrowseBack / BrowseForward are MEF-exported main-menu commands so they appear
		// in the View menu with InputGestureText that the menu builder turns into both a
		// displayed gesture and a window-scoped HotKey. Verifies registration, parent menu,
		// category, gesture text, and that the export carries the right resource header.

		// Arrange — pull the registry directly; no window needed for metadata-only assertions.
		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();

		// Act — find the two exports by header.
		var back = registry.Commands.SingleOrDefault(
			c => c.Metadata.Header == nameof(Resources.Back)
				&& c.Metadata.ParentMenuID == nameof(Resources._View));
		var forward = registry.Commands.SingleOrDefault(
			c => c.Metadata.Header == nameof(Resources.Forward)
				&& c.Metadata.ParentMenuID == nameof(Resources._View));

		// Assert — both exports exist, sit under View > Navigation, carry Alt+Left / Alt+Right.
		back.Should().NotBeNull("BrowseBack must be exported as a View-menu command");
		back!.Metadata.MenuCategory.Should().Be(nameof(Resources.Navigation));
		back.Metadata.InputGestureText.Should().Be("Alt+Left");

		forward.Should().NotBeNull("BrowseForward must be exported as a View-menu command");
		forward!.Metadata.MenuCategory.Should().Be(nameof(Resources.Navigation));
		forward.Metadata.InputGestureText.Should().Be("Alt+Right");
	}

	[AvaloniaTest]
	public async Task BrowseBack_MenuItem_Forwards_CanExecute_And_Execute_To_DockWorkspace()
	{
		// The menu-attached BrowseBack command should be a thin wrapper over
		// DockWorkspace.NavigateBackCommand: CanExecute mirrors the back-stack state, and
		// Execute pops one entry. Verifies via the live MenuItem that ends up in the View menu.

		// Arrange — boot the window, load assemblies, expand Enumerable, capture two methods.
		var (window, vm) = await TestHarness.BootAsync(3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var firstMethod = typeNode.Children.OfType<MethodTreeNode>()
			.Single(m => m.MethodDefinition.Name == "AsEnumerable");
		var secondMethod = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");

		// Locate the View > Back NativeMenuItem.
		var nativeMenu = NativeMenu.GetMenu(window)
			?? throw new System.InvalidOperationException("MainMenu.Attach should have set NativeMenu on the window");
		var viewMenu = nativeMenu.Items.OfType<NativeMenuItem>()
			.Single(m => string.Equals(m.Header, Resources._View, System.StringComparison.Ordinal));
		var backItem = viewMenu.Menu!.Items.OfType<NativeMenuItem>()
			.Single(m => string.Equals(m.Header, Resources.Back, System.StringComparison.Ordinal));

		// Initially nothing on the stack — back must be disabled.
		backItem.Command!.CanExecute(null).Should().BeFalse(
			"with no navigation history, BrowseBack must report CanExecute=false");

		// Build history: select two methods with a delay so they record as separate entries.
		vm.AssemblyTreeModel.SelectNode(firstMethod);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("first-method-decompiled");
		await Task.Delay(600);
		vm.AssemblyTreeModel.SelectNode(secondMethod);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("second-method-decompiled");

		// Act — fire the menu command (mirrors clicking View → Back).
		backItem.Command.CanExecute(null).Should().BeTrue();
		backItem.Command.Execute(null);
		TestCapture.Step("navigated-back");

		// Assert — selection rewinds to the first method (NavigateBack walked one step).
		await Waiters.WaitForAsync(() => ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, firstMethod));
		vm.DockWorkspace.NavigateForwardCommand.CanExecute(null).Should().BeTrue(
			"after one back-step the forward stack should be non-empty");
	}

	[AvaloniaTest]
	public async Task Mouse_Back_And_Forward_Buttons_Navigate_The_History()
	{
		// The extra mouse buttons (XButton1 = back, XButton2 = forward) drive the same history
		// as Alt+Left / Alt+Right, matching browsers and the WPF version (where WPF itself
		// translated the buttons into BrowseBack/BrowseForward commands). Avalonia has no such
		// translation, so MainWindow routes the pointer events to the navigation commands.

		// Arrange — build a two-entry history exactly like the menu-driven test above.
		var (window, vm) = await TestHarness.BootAsync(3);
		var (firstMethod, secondMethod) = await BuildTwoEntryHistoryAsync(vm);

		// Act — click mouse-back anywhere in the window.
		var point = new Point(100, 100);
		window.MouseDown(point, MouseButton.XButton1);
		window.MouseUp(point, MouseButton.XButton1);

		// Assert — selection rewinds, then mouse-forward replays the step.
		await Waiters.WaitForAsync(() => ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, firstMethod),
			description: "XButton1 must navigate back one history entry");
		await Waiters.WaitForAsync(() => vm.DockWorkspace.NavigateForwardCommand.CanExecute(null),
			description: "after one back-step the forward stack should be non-empty");

		window.MouseDown(point, MouseButton.XButton2);
		window.MouseUp(point, MouseButton.XButton2);

		await Waiters.WaitForAsync(() => ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, secondMethod),
			description: "XButton2 must navigate forward one history entry");
	}

	[AvaloniaTest]
	public async Task Mouse_Back_Button_Press_Does_Not_Reach_The_Control_Under_The_Pointer()
	{
		// The X buttons are navigation gestures, not clicks (WPF never delivered them to the
		// control under the pointer). The press must not activate the pane under the pointer,
		// move keyboard focus, or toggle a folding marker; only the release navigates, and the
		// active pane stays where it was across the navigation.

		// Arrange — two-entry history, assembly pane active, pointer over the editor.
		var (window, vm) = await TestHarness.BootAsync(3);
		var (firstMethod, _) = await BuildTwoEntryHistoryAsync(vm);
		var view = await window.WaitForComponent<DecompilerTextView>();

		vm.DockWorkspace.ShowToolPane(AssemblyTreeModel.PaneContentId);
		var activePane = vm.DockWorkspace.Layout.FocusedDockable;
		activePane.Should().NotBeNull("showing the assembly pane must make it the focused dockable");
		var focusedElement = window.FocusManager?.GetFocusedElement();

		int pressedInEditor = 0;
		view.AddHandler(InputElement.PointerPressedEvent, (_, _) => pressedInEditor++,
			RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
		var point = view.TranslatePoint(new Point(view.Bounds.Width / 2, view.Bounds.Height / 2), window);
		point.Should().NotBeNull("the editor centre must map into the test window");

		// Act / Assert — the press is swallowed at the window ...
		window.MouseDown(point!.Value, MouseButton.XButton1);
		pressedInEditor.Should().Be(0, "an X-button press must not reach the control under the pointer");
		vm.DockWorkspace.Layout.FocusedDockable.Should().BeSameAs(activePane,
			"pressing a mouse navigation button must not activate the pane under the pointer");
		ReferenceEquals(window.FocusManager?.GetFocusedElement(), focusedElement).Should().BeTrue(
			"pressing a mouse navigation button must not move keyboard focus");

		// ... and the release navigates without moving the active pane to the editor.
		window.MouseUp(point.Value, MouseButton.XButton1);
		await Waiters.WaitForAsync(() => ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, firstMethod),
			description: "XButton1 must navigate back one history entry");
		vm.DockWorkspace.Layout.FocusedDockable.Should().BeSameAs(activePane,
			"navigating back must not move the active pane to the editor");
	}

	[AvaloniaTest]
	public async Task Browse_Back_Keeps_The_Active_Pane()
	{
		// Back/Forward re-select a tree node and restore the tab's view state; the tab being
		// navigated is already the active document, so the navigation must not move the active
		// pane to it (WPF kept the current view focused). Exercises the command directly, which
		// is what the Alt+Left key binding and the View menu invoke.
		var (_, vm) = await TestHarness.BootAsync(3);
		var (firstMethod, _) = await BuildTwoEntryHistoryAsync(vm);
		vm.DockWorkspace.ShowToolPane(AssemblyTreeModel.PaneContentId);
		var activePane = vm.DockWorkspace.Layout.FocusedDockable;
		activePane.Should().NotBeNull("showing the assembly pane must make it the focused dockable");

		vm.DockWorkspace.NavigateBackCommand.Execute(null);

		await Waiters.WaitForAsync(() => ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, firstMethod),
			description: "BrowseBack must navigate back one history entry");
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		vm.DockWorkspace.Layout.FocusedDockable.Should().BeSameAs(activePane,
			"navigating back must not move the active pane to the editor");
	}

	// Selects two methods of System.Linq.Enumerable with a pause in between so the history records
	// them as two separate entries; returns them in selection order.
	static async Task<(MethodTreeNode First, MethodTreeNode Second)> BuildTwoEntryHistoryAsync(MainWindowViewModel vm)
	{
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var firstMethod = typeNode.Children.OfType<MethodTreeNode>()
			.Single(m => m.MethodDefinition.Name == "AsEnumerable");
		var secondMethod = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");

		vm.AssemblyTreeModel.SelectNode(firstMethod);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		await Task.Delay(600);
		vm.AssemblyTreeModel.SelectNode(secondMethod);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		return (firstMethod, secondMethod);
	}

	[AvaloniaTest]
	public void BrowseBack_MenuItem_Carries_The_Alt_Left_Gesture()
	{
		// MEF metadata's InputGestureText flows through MainMenu.Attach into the
		// NativeMenuItem.Gesture property -- this is what NativeMenuBar renders inline on
		// Windows / Linux and what macOS projects into the system menu bar.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var nativeMenu = NativeMenu.GetMenu(window)
			?? throw new System.InvalidOperationException("MainMenu.Attach should have set NativeMenu on the window");
		var viewMenu = nativeMenu.Items.OfType<NativeMenuItem>()
			.Single(m => string.Equals(m.Header, Resources._View, System.StringComparison.Ordinal));
		var backItem = viewMenu.Menu!.Items.OfType<NativeMenuItem>()
			.Single(m => string.Equals(m.Header, Resources.Back, System.StringComparison.Ordinal));
		var forwardItem = viewMenu.Menu!.Items.OfType<NativeMenuItem>()
			.Single(m => string.Equals(m.Header, Resources.Forward, System.StringComparison.Ordinal));

		backItem.Gesture.Should().Be(KeyGesture.Parse("Alt+Left"));
		forwardItem.Gesture.Should().Be(KeyGesture.Parse("Alt+Right"));
	}
}
