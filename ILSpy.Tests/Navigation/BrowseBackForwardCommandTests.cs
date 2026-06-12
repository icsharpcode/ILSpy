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
using Avalonia.Input;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
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
