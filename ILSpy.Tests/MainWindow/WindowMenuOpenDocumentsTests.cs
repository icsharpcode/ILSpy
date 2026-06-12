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

using System;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class WindowMenuOpenDocumentsTests
{
	[AvaloniaTest]
	public async Task Window_Menu_Lists_Every_Open_Document_Tab()
	{
		// The Window menu surfaces one entry per open document tab so the user can jump
		// between tabs from the keyboard. The entries are NativeMenuItems whose Header is
		// bound to TabPageMenuItem.Title; this test reads the NativeMenu source-of-truth
		// (NativeMenuBar's visual presenter renders these on Windows / Linux, and the
		// system menu bar projects them on macOS).

		var (window, vm) = await TestHarness.BootAsync(3);

		var firstAsm = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		vm.AssemblyTreeModel.SelectNode(firstAsm);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("first-assembly-decompiled");

		var secondAsm = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Private.Uri");
		vm.DockWorkspace.OpenNodeInNewTab(secondAsm);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("second-assembly-in-new-tab");

		var nativeMenu = NativeMenu.GetMenu(window)
			?? throw new InvalidOperationException("MainMenu.Attach should have set NativeMenu on the window");
		var windowMenu = nativeMenu.Items.OfType<NativeMenuItem>()
			.Single(m => string.Equals(m.Header, Resources._Window, StringComparison.Ordinal));

		var documentsDock = ((ILSpyDockFactory)vm.DockWorkspace.Factory).Documents!;
		await Waiters.WaitForAsync(
			() => documentsDock.VisibleDockables!.OfType<ContentTabPage>().All(t => !string.IsNullOrEmpty(t.Title))
				&& documentsDock.VisibleDockables!.OfType<ContentTabPage>()
					.Select(t => t.Title)
					.All(title => windowMenu.Menu!.Items.OfType<NativeMenuItem>().Any(mi => string.Equals(mi.Header, title))));

		var tabTitles = documentsDock.VisibleDockables!.OfType<ContentTabPage>()
			.Select(t => t.Title)
			.ToList();
		var menuTitles = windowMenu.Menu!.Items.OfType<NativeMenuItem>()
			.Select(mi => mi.Header)
			.ToList();

		tabTitles.Should().HaveCount(2);
		menuTitles.Should().Contain(tabTitles!);
	}
}
