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

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;

using ILSpy.AppEnv;
using ILSpy.Docking;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class WindowMenuOpenDocumentsTests
{
	[AvaloniaTest]
	public async Task Window_Menu_Lists_Every_Open_Document_Tab()
	{
		// The Window menu must surface one entry per open document tab so the user can
		// jump between tabs from the keyboard. Mirrors WPF's MainMenu.InitWindowMenu
		// tab-section (which composed `dockWorkspace.TabPages` into the menu's ItemsSource).

		// Arrange — boot the window, wait for assemblies, then select a node so the persistent
		// MainTab carries a meaningful title.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var firstAsm = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		vm.AssemblyTreeModel.SelectNode(firstAsm);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		// Open a second tab via the production "open in new tab" path.
		var secondAsm = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Private.Uri");
		vm.DockWorkspace.OpenNodeInNewTab(secondAsm);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		var menu = await window.WaitForComponent<Menu>();
		var windowMenu = menu.Items.OfType<MenuItem>()
			.Single(m => (string?)m.Tag == nameof(Resources._Window));

		// Wait for the menu to be populated — the tabs section is built lazily on Loaded /
		// when the collection changes. Match on the second tab's runtime Title rather than the
		// tree node's text so the assertion tracks whatever ContentTabPage actually surfaces.
		var documentsDock = ((ILSpyDockFactory)vm.DockWorkspace.Factory).Documents!;
		await Waiters.WaitForAsync(
			() => documentsDock.VisibleDockables!.OfType<ContentTabPage>().All(t => !string.IsNullOrEmpty(t.Title))
				&& documentsDock.VisibleDockables!.OfType<ContentTabPage>()
					.Select(t => t.Title)
					.All(title => windowMenu.Items.OfType<MenuItem>().Any(mi => string.Equals(mi.Header as string, title))));

		// Assert — both tabs are listed by their Title in the Window menu.
		var tabTitles = documentsDock.VisibleDockables!.OfType<ContentTabPage>()
			.Select(t => t.Title)
			.ToList();
		var menuTitles = windowMenu.Items.OfType<MenuItem>()
			.Select(mi => mi.Header as string)
			.ToList();

		tabTitles.Should().HaveCount(2);
		menuTitles.Should().Contain(tabTitles!);
	}
}
