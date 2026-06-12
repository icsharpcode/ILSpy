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
using Avalonia.Input;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// A context menu raised from the keyboard (Shift+F10 / Apps key) must not strand keyboard focus:
/// closing the popup otherwise drops focus (and its focus adorner), leaving the tree un-navigable
/// until the user clicks back in. The pane captures the pre-menu focus and restores it on close.
/// </summary>
[TestFixture]
public class KeyboardContextMenuFocusTests
{
	[AvaloniaTest]
	public async Task Keyboard_Invoked_Menu_Returns_Focus_To_The_Row_On_Close()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();

		var node = vm.AssemblyTreeModel.Root!.Children.OfType<AssemblyTreeNode>().First();
		vm.AssemblyTreeModel.SelectNode(node);
		for (int i = 0; i < 8; i++)
		{
			Dispatcher.UIThread.RunJobs();
			grid.UpdateLayout();
			await Task.Delay(25);
		}

		var row = grid.GetVisualDescendants()
			.OfType<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeViewItem>().First();
		row.Focus(NavigationMethod.Tab);
		Dispatcher.UIThread.RunJobs();

		var focusManager = TopLevel.GetTopLevel(window)!.FocusManager!;
		(focusManager.GetFocusedElement() == row).Should().BeTrue("the row must hold focus before invoking the menu");

		// Keyboard invocation raises ContextRequested with no pointer position (the Shift+F10 / Apps path).
		row.RaiseEvent(new ContextRequestedEventArgs());
		for (int i = 0; i < 6; i++)
		{
			Dispatcher.UIThread.RunJobs();
			await Task.Delay(20);
		}
		grid.ContextMenu!.IsOpen.Should().BeTrue("the keyboard gesture must open the tree context menu");
		row.Classes.Should().Contain("contextTarget",
			"a keyboard-invoked menu must show the transient target highlight on the selected row, like the mouse path");

		window.KeyPress(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, keySymbol: null);
		for (int i = 0; i < 6; i++)
		{
			Dispatcher.UIThread.RunJobs();
			await Task.Delay(20);
		}

		(focusManager.GetFocusedElement() == row).Should().BeTrue(
			"closing a keyboard-invoked context menu must return focus to the row, not strand it");
		row.Classes.Should().NotContain("contextTarget",
			"the transient highlight must clear once the menu closes");
	}
}
