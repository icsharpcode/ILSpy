// Copyright (c) 2026 Piero Viano
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
using Avalonia.Input;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Navigation;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpyX.TreeView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Navigation;

/// <summary>
/// Recent Files (dotPeek "Recent Files"): every tree node the user navigates to lands at the top
/// of a distinct, most-recent-first list, and the popup over that list navigates back to the
/// chosen node.
/// </summary>
[TestFixture]
public class RecentFilesTests
{
	// Namespace nodes decompile to a single comment line, the cheapest navigation targets.
	static (NamespaceTreeNode A, NamespaceTreeNode B, NamespaceTreeNode C) CheapNodes(AssemblyTreeModel atm)
		=> (atm.FindNode<NamespaceTreeNode>(TreeNavigation.CoreLibName, "System.Runtime.Versioning"),
			atm.FindNode<NamespaceTreeNode>(TreeNavigation.CoreLibName, "System.Text"),
			atm.FindNode<NamespaceTreeNode>(TreeNavigation.CoreLibName, "System.Globalization"));

	[AvaloniaTest]
	public async Task Navigated_Nodes_Are_Listed_Most_Recent_First_Without_Duplicates()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var dock = vm.DockWorkspace;
		var (a, b, c) = CheapNodes(vm.AssemblyTreeModel);

		foreach (var node in new[] { a, b, c, a })
		{
			vm.AssemblyTreeModel.SelectNode(node);
			await dock.WaitForDecompiledTextAsync();
		}

		var files = dock.RecentNavigation.Files.Select(f => f.Node).ToList();
		files.Take(3).Should().Equal(new SharpTreeNode[] { a, c, b },
			"re-visiting A must move it to the top instead of adding a second entry");
		files.Count(n => ReferenceEquals(n, a)).Should().Be(1);
	}

	[AvaloniaTest]
	public async Task Popup_Filters_The_List_And_Accepting_An_Entry_Navigates_To_It()
	{
		var (window, vm) = await TestHarness.BootAsync();
		var dock = vm.DockWorkspace;
		var (a, b, _) = CheapNodes(vm.AssemblyTreeModel);
		vm.AssemblyTreeModel.SelectNode(a);
		await dock.WaitForDecompiledTextAsync();
		vm.AssemblyTreeModel.SelectNode(b);
		await dock.WaitForDecompiledTextAsync();

		var command = AppComposition.Current.GetExport<MainMenuCommandRegistry>().GetCommand(nameof(Resources.RecentFiles));
		command.Execute(null);

		var popup = window.OwnedWindows.OfType<QuickPickWindow>().Should().ContainSingle(
			"Recent Files must open a quick-pick popup owned by the main window").Subject;
		popup.Model.FilteredItems.Should().NotBeEmpty();
		var first = popup.Model.FilteredItems[0].Payload.Should().BeOfType<RecentFile>().Subject;
		ReferenceEquals(first.Node, b).Should().BeTrue("the most recent entry comes first");

		popup.Model.Filter = "Versioning";
		popup.Model.FilteredItems.Should().ContainSingle("the filter must narrow the list to matching entries")
			.Which.Text.Should().Contain("System.Runtime.Versioning");

		popup.Model.Accept();
		await dock.WaitForDecompiledTextAsync();

		ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, a).Should().BeTrue("accepting an entry navigates to its node");
		window.OwnedWindows.OfType<QuickPickWindow>().Should().BeEmpty("accepting closes the popup");
	}

	[AvaloniaTest]
	public void Model_Filter_Matches_Text_And_Detail_Case_Insensitively()
	{
		var model = new QuickPickModel(new[] {
			new QuickPickItem("Alpha", "first.dll", null, 1),
			new QuickPickItem("Beta", "second.dll", null, 2),
			new QuickPickItem("Gamma", "SECOND.dll", null, 3),
		}, _ => { });

		model.FilteredItems.Should().HaveCount(3);
		model.SelectedItem.Should().BeSameAs(model.FilteredItems[0], "the first entry is preselected");

		model.Filter = "second";
		model.FilteredItems.Select(i => i.Text).Should().Equal("Beta", "Gamma");
		model.SelectedItem!.Text.Should().Be("Beta", "the selection follows the filtered list");

		model.Filter = "zzz";
		model.FilteredItems.Should().BeEmpty();
		model.SelectedItem.Should().BeNull();
	}

	[AvaloniaTest]
	public async Task Navigation_Shortcuts_Are_Bound_On_The_Main_Window()
	{
		var (window, _) = await TestHarness.BootAsync();

		bool Bound(Key key, KeyModifiers modifiers)
			=> window.KeyBindings.Any(b => b.Gesture is { } g && g.Key == key && g.KeyModifiers == modifiers);

		Bound(Key.OemComma, KeyModifiers.Control).Should().BeTrue("Recent Files is Ctrl+Comma");
		Bound(Key.OemComma, KeyModifiers.Control | KeyModifiers.Shift).Should().BeTrue("Recent Locations is Ctrl+Shift+Comma");
		Bound(Key.F2, KeyModifiers.None).Should().BeTrue("Find Usages for Rename is F2");
		Bound(Key.G, KeyModifiers.Control).Should().BeFalse(
			"Ctrl+G is Go to Line only inside the code view; metadata grids use it for Go to token");
	}
}
