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

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Navigation;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpyX.TreeView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Navigation;

/// <summary>
/// Recent Locations (dotPeek "Recent Locations"): the caret position the user leaves behind on
/// each navigation is remembered together with a one-line preview of the code there, and
/// choosing a location re-opens its node with the caret restored.
/// </summary>
[TestFixture]
public class RecentLocationsTests
{
	static (NamespaceTreeNode A, NamespaceTreeNode B) CheapNodes(AssemblyTreeModel atm)
		=> (atm.FindNode<NamespaceTreeNode>(TreeNavigation.CoreLibName, "System.Runtime.Versioning"),
			atm.FindNode<NamespaceTreeNode>(TreeNavigation.CoreLibName, "System.Text"));

	[AvaloniaTest]
	public async Task Leaving_A_Node_Records_Its_Caret_Location_With_A_Code_Preview()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var dock = vm.DockWorkspace;
		var (a, b) = CheapNodes(vm.AssemblyTreeModel);

		vm.AssemblyTreeModel.SelectNode(a);
		var tab = await dock.WaitForDecompiledTextAsync();
		var text = tab.Text;
		// Headless has no laid-out editor; report the caret the user "left" on node A.
		tab.CaptureViewState = () => new DecompilerTextViewState(4, 0, 0, null);

		vm.AssemblyTreeModel.SelectNode(b);
		await dock.WaitForDecompiledTextAsync();

		var location = dock.RecentNavigation.Locations.Should().NotBeEmpty().And.Subject.First();
		ReferenceEquals(location.Node, a).Should().BeTrue();
		location.State.CaretOffset.Should().Be(4);
		location.Line.Should().Be(1);
		location.Column.Should().Be(5);
		location.Preview.Should().Be(text.Split('\n')[0].Trim(),
			"the preview is the trimmed text of the caret line");
	}

	[AvaloniaTest]
	public async Task Navigating_To_A_Location_Reopens_The_Node_And_Restores_The_Caret()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var dock = vm.DockWorkspace;
		var (a, b) = CheapNodes(vm.AssemblyTreeModel);

		vm.AssemblyTreeModel.SelectNode(a);
		var tab = await dock.WaitForDecompiledTextAsync();
		tab.CaptureViewState = () => new DecompilerTextViewState(6, 12.5, 0, null);
		vm.AssemblyTreeModel.SelectNode(b);
		await dock.WaitForDecompiledTextAsync();

		var location = dock.RecentNavigation.Locations.First(l => ReferenceEquals(l.Node, a));
		dock.NavigateToRecentLocation(location);

		ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, a).Should().BeTrue();
		var target = dock.ActiveDecompilerTab!;
		target.PendingViewState.Should().NotBeNull("the location's view state is handed to the editor");
		target.PendingViewState!.Value.CaretOffset.Should().Be(6);
		target.PendingViewState!.Value.VerticalOffset.Should().Be(12.5);

		await dock.WaitForDecompiledTextAsync();
		target.PendingViewState.Should().BeNull("the editor consumes the restored state");
	}

	[AvaloniaTest]
	public async Task Popup_Lists_Locations_With_Their_Preview_And_Includes_The_Current_One()
	{
		var (window, vm) = await TestHarness.BootAsync();
		var dock = vm.DockWorkspace;
		var (a, b) = CheapNodes(vm.AssemblyTreeModel);

		vm.AssemblyTreeModel.SelectNode(a);
		var tabA = await dock.WaitForDecompiledTextAsync();
		tabA.CaptureViewState = () => new DecompilerTextViewState(0, 0, 0, null);
		vm.AssemblyTreeModel.SelectNode(b);
		var tabB = await dock.WaitForDecompiledTextAsync();
		tabB.CaptureViewState = () => new DecompilerTextViewState(0, 0, 0, null);

		var command = AppComposition.Current.GetExport<MainMenuCommandRegistry>().GetCommand(nameof(Resources.RecentLocations));
		command.Execute(null);

		var popup = window.OwnedWindows.OfType<QuickPickWindow>().Should().ContainSingle().Subject;
		var locations = popup.Model.FilteredItems.Select(i => i.Payload).OfType<RecentLocation>().ToList();
		locations.Select(l => l.Node).Should().ContainInOrder(new SharpTreeNode[] { b, a },
			"the current location is listed first, then the one left behind");
		popup.Model.FilteredItems[0].Detail.Should().Contain(tabB.Text.Split('\n')[0].Trim(),
			"each entry shows the code at its position");
		popup.Close();
	}
}
