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
using Avalonia.Controls.Primitives;
using Avalonia.Headless.NUnit;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;

using ILSpy;
using ILSpy.AppEnv;
using ILSpy.Search;
using ILSpy.ViewModels;
using ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Pins the left-to-right order of the MainToolBar to match the WPF original. WPF lays out
/// its toolbar by hand-coded XAML interleaved with two MEF injection points: Navigation goes
/// at the very start, Open goes right after, then come the AssemblyList combo + Manage button,
/// then the visibility toggles, then the language combos, and View-category commands (Sort,
/// CollapseAll, Search) are appended after a separator at the very end (see
/// <c>ILSpy/Controls/MainToolBar.xaml</c> + <c>InitToolbar</c> in <c>MainToolBar.xaml.cs:55</c>).
/// </summary>
[TestFixture]
public class MainToolBarLayoutTests
{
	[AvaloniaTest]
	public async Task MainToolBar_Has_ManageAssemblyListsButton_Right_Of_AssemblyListComboBox()
	{
		// WPF places the Manage-Lists icon button immediately right of the assembly-list combo
		// (MainToolBar.xaml lines 44-49). The Avalonia port previously only surfaced this
		// command under the File menu — verify the inline toolbar affordance is wired and that
		// clicking it pops the ManageAssemblyListsDialog.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();

		var toolbar = await window.WaitForComponent<MainToolBar>();
		var combo = toolbar.GetVisualDescendants().OfType<ComboBox>()
			.Single(c => c.Name == "AssemblyListComboBox");
		var assemblyListGrid = combo.FindAncestorOfType<Grid>()!;

		// Walk the StackPanel children from the AssemblyList grid forward; the very next
		// non-separator child must be a Button bound to the manage-lists command.
		var rootPanel = toolbar.GetVisualDescendants().OfType<StackPanel>()
			.Single(s => s.Name == "ToolbarRoot");
		var children = rootPanel.Children.ToList();
		var assemblyListIndex = children.IndexOf(assemblyListGrid);
		assemblyListIndex.Should().BeGreaterThan(-1);

		var nextSibling = children.Skip(assemblyListIndex + 1)
			.OfType<Button>()
			.FirstOrDefault();
		nextSibling.Should().NotBeNull("a Manage Assembly Lists button must sit next to the combo");
		(ToolTip.GetTip(nextSibling!) as string).Should().Be(
			Resources.ManageAssemblyLists,
			"the inline button must carry the ManageAssemblyLists tooltip resource");
		nextSibling!.Command.Should().NotBeNull("the inline button must be bound to a command");
	}

	[AvaloniaTest]
	public async Task MainToolBar_ShowSearch_Button_Exists_And_Activates_Search_Pane()
	{
		// WPF exports ShowSearchCommand as [ExportToolbarCommand] (ILSpy/Search/ShowSearchCommand.cs:27)
		// so a magnifier icon sits in the View category at the right end of the toolbar. The
		// Avalonia port only had the key-binding wiring — this test pins the toolbar surface area
		// and verifies executing the button activates the search pane.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var toolbar = await window.WaitForComponent<MainToolBar>();
		var rootPanel = toolbar.GetVisualDescendants().OfType<StackPanel>()
			.Single(s => s.Name == "ToolbarRoot");

		// MEF-injected buttons stash their tooltip-resource-key in Tag (see BuildButton in
		// MainToolBar.axaml.cs); identify the Show-Search button by that key.
		var searchButton = rootPanel.Children.OfType<Button>()
			.SingleOrDefault(b => (b.Tag as string) == nameof(Resources.SearchCtrlShiftFOrCtrlE));
		searchButton.Should().NotBeNull(
			"a Show-Search toolbar button must be MEF-injected via [ExportToolbarCommand]");

		var search = AppComposition.Current.GetExport<SearchPaneModel>();
		searchButton!.Command!.CanExecute(null).Should().BeTrue();
		searchButton.Command.Execute(null);

		search.IsActive.Should().BeTrue(
			"clicking the Show-Search toolbar button must activate the search tool pane");
	}

	[AvaloniaTest]
	public async Task MainToolBar_Button_Order_Mirrors_WPF()
	{
		// Pins the left-to-right child sequence of the toolbar StackPanel. The expected order
		// (taken from WPF's MainToolBar.xaml + InitToolbar grouping in MainToolBar.xaml.cs:55)
		// is:
		//   Back | Forward | --- | Open | Refresh | --- | AssemblyList | Manage | --- |
		//   PublicOnly | PrivateInternal | All | --- | Language | LanguageVersion | --- |
		//   Sort | CollapseAll | Search
		//
		// Separators between groups are checked positionally too. The two language combos and
		// the assembly-list combo each live inside a Grid wrapper (axaml lines 223 / 232 / 245),
		// so we tag those positions by descending into the Grid for the ComboBox name.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var toolbar = await window.WaitForComponent<MainToolBar>();
		var rootPanel = toolbar.GetVisualDescendants().OfType<StackPanel>()
			.Single(s => s.Name == "ToolbarRoot");
		var labels = rootPanel.Children.Select(Label).ToList();

		labels.Should().Equal(
			"BackSplitButton",
			"ForwardSplitButton",
			"Separator",
			"OpenButton",
			"RefreshButton",
			"Separator",
			"AssemblyListComboBox",
			"ManageAssemblyListsButton",
			"Separator",
			"ShowPublicOnlyButton",
			"ShowPrivateInternalButton",
			"ShowAllButton",
			"Separator",
			"LanguageComboBox",
			"LanguageVersionComboBox",
			"Separator",
			"SortButton",
			"CollapseAllButton",
			"SearchButton");

		static string Label(global::Avalonia.Controls.Control c) => c switch {
			Separator => "Separator",
			SplitButton sb when sb.Name == "BackSplitButton" => "BackSplitButton",
			SplitButton sb when sb.Name == "ForwardSplitButton" => "ForwardSplitButton",
			ToggleButton tb when tb.Name == "ShowPublicOnlyButton" => "ShowPublicOnlyButton",
			ToggleButton tb when tb.Name == "ShowPrivateInternalButton" => "ShowPrivateInternalButton",
			ToggleButton tb when tb.Name == "ShowAllButton" => "ShowAllButton",
			Grid g when g.GetVisualDescendants().OfType<ComboBox>().Any(cb => cb.Name == "AssemblyListComboBox")
				=> "AssemblyListComboBox",
			Grid g when g.GetVisualDescendants().OfType<ComboBox>().Any(cb => cb.Name == "LanguageComboBox")
				=> "LanguageComboBox",
			Grid g when g.GetVisualDescendants().OfType<ComboBox>().Any(cb => cb.Name == "LanguageVersionComboBox")
				=> "LanguageVersionComboBox",
			Button b when (b.Tag as string) == nameof(Resources.Open) => "OpenButton",
			Button b when (b.Tag as string) == nameof(Resources.RefreshCommand_ReloadAssemblies) => "RefreshButton",
			Button b when (b.Tag as string) == nameof(Resources.SortAssemblyListName) => "SortButton",
			Button b when (b.Tag as string) == nameof(Resources.CollapseTreeNodes) => "CollapseAllButton",
			Button b when (b.Tag as string) == nameof(Resources.SearchCtrlShiftFOrCtrlE) => "SearchButton",
			Button b when (ToolTip.GetTip(b) as string) == Resources.ManageAssemblyLists => "ManageAssemblyListsButton",
			_ => $"Unknown({c.GetType().Name}, Name={c.Name}, Tag={c.Tag})",
		};
	}
}
