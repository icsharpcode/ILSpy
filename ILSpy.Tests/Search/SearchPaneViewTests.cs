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

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Search;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Search;

[TestFixture]
public class SearchPaneViewTests
{
	[AvaloniaTest]
	public async Task SearchPane_Hosts_The_Required_Three_Controls_For_Search_Input_Mode_And_Results()
	{
		// The pane's contract — used by every downstream commit in this series — is
		// that the view exposes three named controls bound to the view-model: a TextBox
		// for the query (SearchInput), a ComboBox for the mode picker (SearchModePicker),
		// and a DataGrid that renders incoming results (SearchResults).

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		// Search is hidden by default; surface it so its view realises.
		AppComposition.Current.GetExport<ICSharpCode.ILSpy.Docking.DockWorkspace>()
			.ShowToolPane(SearchPaneModel.PaneContentId);
		var pane = await window.WaitForComponent<SearchPane>();
		TestCapture.Step("booted");

		var input = pane.FindControl<TextBox>("SearchInput");
		((object?)input).Should().NotBeNull("the query TextBox is what the user types into");

		var modePicker = pane.FindControl<ComboBox>("SearchModePicker");
		((object?)modePicker).Should().NotBeNull("the mode picker drives which ILSpyX strategy runs");
		modePicker!.ItemCount.Should().Be(12,
			"the picker must populate from SearchPaneModel.SearchModes — twelve entries");

		var results = pane.FindControl<DataGrid>("SearchResults");
		((object?)results).Should().NotBeNull("the results list is the binding sink for the streaming orchestrator");
	}

	[AvaloniaTest]
	public async Task Typing_Into_The_Search_Input_Writes_Through_To_SearchTerm()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		// Search is hidden by default; surface it so its view realises.
		AppComposition.Current.GetExport<ICSharpCode.ILSpy.Docking.DockWorkspace>()
			.ShowToolPane(SearchPaneModel.PaneContentId);
		var pane = await window.WaitForComponent<SearchPane>();
		TestCapture.Step("booted");
		var vm = (SearchPaneModel)pane.DataContext!;

		var input = pane.FindControl<TextBox>("SearchInput");
		input!.Text = "Enumerable";
		TestCapture.Step("typed-enumerable");

		vm.SearchTerm.Should().Be("Enumerable",
			"the TextBox is bound two-way to SearchPaneModel.SearchTerm");
	}

	[AvaloniaTest]
	public async Task Pressing_Escape_In_The_Search_Input_Clears_SearchTerm()
	{
		// The clear-X button's tooltip advertises "Clear (Esc)". Pressing Esc while the
		// SearchInput TextBox has focus must wipe the term — same path as clicking the
		// button. Goes through the bound view-model so the orchestrator's cancel-and-
		// restart path fires too.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		// Search is hidden by default; surface it so its view realises.
		AppComposition.Current.GetExport<ICSharpCode.ILSpy.Docking.DockWorkspace>()
			.ShowToolPane(SearchPaneModel.PaneContentId);
		var pane = await window.WaitForComponent<SearchPane>();
		TestCapture.Step("booted");
		var vm = (SearchPaneModel)pane.DataContext!;

		var input = pane.FindControl<TextBox>("SearchInput");
		input!.Text = "Enumerable";
		vm.SearchTerm.Should().Be("Enumerable",
			"baseline: the test setup must put a value in the box before testing the clear");

		input.RaiseEvent(new global::Avalonia.Input.KeyEventArgs {
			Key = global::Avalonia.Input.Key.Escape,
			KeyModifiers = global::Avalonia.Input.KeyModifiers.None,
			RoutedEvent = global::Avalonia.Input.InputElement.KeyDownEvent,
			Source = input,
		});
		TestCapture.Step("escape-cleared-term");

		vm.SearchTerm.Should().BeEmpty("Escape in the search input must clear the term");
	}
}
