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
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class SearchMsdnTests
{
	[AvaloniaTest]
	public async Task SearchMsdn_Entry_Is_Registered()
	{
		// "Search Microsoft Docs..." is exported as a [ExportContextMenuEntry] tagged with
		// the SearchMSDN resource. Verifies it shows up via MEF.

		// Arrange + Act — boot.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();

		// Assert
		registry.Entries.Should().Contain(
			e => e.Metadata.Header == nameof(Resources.SearchMSDN));
	}

	[AvaloniaTest]
	public async Task SearchMsdn_Is_Visible_Only_For_Type_Member_Or_Namespace_Selections()
	{
		// IsVisible: only the entity-bearing tree-node kinds plus NamespaceTreeNode (which has
		// its own MS-Docs landing page). Hidden for assembly nodes, references folders, etc.

		// Arrange — boot, locate entry + sample nodes.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
		TestCapture.Step("booted");
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var entry = (SearchMsdnContextMenuEntry)registry.Entries
			.Single(e => e.Metadata.Header == nameof(Resources.SearchMSDN))
			.Value;

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");

		entry.IsVisible(new TextViewContext { SelectedTreeNodes = new[] { (SharpTreeNode)typeNode } })
			.Should().BeTrue();
		entry.IsVisible(new TextViewContext { SelectedTreeNodes = new[] { (SharpTreeNode)assemblyNode } })
			.Should().BeFalse("assembly nodes don't have an MS-Docs page");
		entry.IsVisible(new TextViewContext { SelectedTreeNodes = null })
			.Should().BeFalse();
	}

	// audit (2026-05-12): partial 📦 → 🧪 conversion. Execute opens a system browser
	// (Process.Start with the URL) which is unsafe to fire in a headless test, so the URL
	// payload is still verified via the public `GetMsdnUrl` helper (that's the testable seam
	// production Execute itself calls). Adds an integration assertion that the entry surfaces
	// in the right-click menu for the same selection — catches the wiring path between the
	// IsVisible predicate and the menu builder that the original GetMsdnUrl-only assertion
	// would have missed.
	[AvaloniaTest]
	public async Task Right_Clicking_A_Type_Surfaces_SearchMSDN_And_The_Entry_Builds_The_Expected_URL()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");

		window.Capture("initial-tree");

		// Selecting the type and building the live menu must surface "Search Microsoft Docs…".
		vm.AssemblyTreeModel.SelectNode(typeNode);
		window.Capture("enumerable-type-selected");

		var menu = pane.BuildContextMenuForCurrentState(registry.Entries);
		TestCapture.Step("context-menu-built");
		menu.Should().NotBeNull();
		menu!.Items.OfType<MenuItem>().Select(i => (string?)i.Header)
			.Should().Contain(Resources.SearchMSDN);

		// The URL the entry would open (kept as a helper because Execute itself launches the
		// browser via Process.Start).
		var entry = (SearchMsdnContextMenuEntry)registry.Entries
			.Single(e => e.Metadata.Header == nameof(Resources.SearchMSDN)).Value;
		entry.GetMsdnUrl(typeNode)
			.Should().Be("https://learn.microsoft.com/dotnet/api/system.linq.enumerable");
	}
}
