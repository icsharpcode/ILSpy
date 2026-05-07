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

using ILSpy.AppEnv;
using ILSpy.Commands;
using ILSpy.Docking;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

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

		// Locate the View > Back menu item.
		var menu = await window.WaitForComponent<Menu>();
		var viewMenu = menu.Items.OfType<MenuItem>()
			.Single(m => (string?)m.Tag == nameof(Resources._View));
		// expand the View menu so its child items are realised.
		viewMenu.Open();
		await Waiters.WaitForAsync(() => viewMenu.Items.OfType<MenuItem>().Any());
		var backItem = viewMenu.Items.OfType<MenuItem>()
			.Single(m => string.Equals(m.Header as string, Resources.Back, System.StringComparison.Ordinal));

		// Initially nothing on the stack — back must be disabled.
		backItem.Command!.CanExecute(null).Should().BeFalse(
			"with no navigation history, BrowseBack must report CanExecute=false");

		// Build history: select two methods with a delay so they record as separate entries.
		vm.AssemblyTreeModel.SelectNode(firstMethod);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		await Task.Delay(600);
		vm.AssemblyTreeModel.SelectNode(secondMethod);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		// Act — fire the menu command (mirrors clicking View → Back).
		backItem.Command.CanExecute(null).Should().BeTrue();
		backItem.Command.Execute(null);

		// Assert — selection rewinds to the first method (NavigateBack walked one step).
		await Waiters.WaitForAsync(() => ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, firstMethod));
		vm.DockWorkspace.NavigateForwardCommand.CanExecute(null).Should().BeTrue(
			"after one back-step the forward stack should be non-empty");
	}

	[AvaloniaTest]
	public async Task BrowseBack_MenuItem_Carries_The_Alt_Left_Hot_Key_And_Gesture()
	{
		// MEF metadata's InputGestureText must be picked up by the menu builder and applied to
		// both InputGesture (displayed text) and HotKey (window-wide accelerator).

		// Arrange — boot, locate the View menu, wait for both nav items to materialise.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var menu = await window.WaitForComponent<Menu>();
		var viewMenu = menu.Items.OfType<MenuItem>()
			.Single(m => (string?)m.Tag == nameof(Resources._View));
		viewMenu.Open();
		await Waiters.WaitForAsync(() =>
			viewMenu.Items.OfType<MenuItem>().Any(m => (string?)m.Header == Resources.Back)
				&& viewMenu.Items.OfType<MenuItem>().Any(m => (string?)m.Header == Resources.Forward));
		var backItem = viewMenu.Items.OfType<MenuItem>()
			.Single(m => string.Equals(m.Header as string, Resources.Back, System.StringComparison.Ordinal));
		var forwardItem = viewMenu.Items.OfType<MenuItem>()
			.Single(m => string.Equals(m.Header as string, Resources.Forward, System.StringComparison.Ordinal));

		// Assert — Alt+Left / Alt+Right round-trip through both InputGesture and HotKey.
		backItem.InputGesture.Should().Be(KeyGesture.Parse("Alt+Left"));
		backItem.HotKey.Should().Be(KeyGesture.Parse("Alt+Left"));
		forwardItem.InputGesture.Should().Be(KeyGesture.Parse("Alt+Right"));
		forwardItem.HotKey.Should().Be(KeyGesture.Parse("Alt+Right"));
	}
}
