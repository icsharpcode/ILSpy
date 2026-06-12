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
using Avalonia.Input;
using Avalonia.Threading;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.Search;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Search;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Search;

/// <summary>
/// Keyboard and mouse niceties carried over from the previous search pane: mode hotkeys
/// (Ctrl+T/M/S), search-box &lt;-&gt; result-list arrow-key focus transfer, and
/// open-in-new-tab (Ctrl+Enter / middle-click).
/// </summary>
[TestFixture]
public class SearchPaneNicetiesTests
{
	static SearchResult Dummy(string name) => new() {
		Name = name,
		Location = "loc",
		Assembly = "asm",
		Image = null!,
		LocationImage = null!,
		AssemblyImage = null!,
	};

	static async Task<(MainWindow window, SearchPane pane, SearchPaneModel vm)> ShowPaneAsync()
	{
		var (window, _) = await TestHarness.BootAsync();
		var dockWorkspace = AppComposition.Current.GetExport<ICSharpCode.ILSpy.Docking.DockWorkspace>();
		dockWorkspace.ShowToolPane(SearchPaneModel.PaneContentId);
		var pane = await window.WaitForComponent<SearchPane>();
		var vm = (SearchPaneModel)pane.DataContext!;
		return (window, pane, vm);
	}

	static void RaiseKey(Control target, Key key, KeyModifiers modifiers)
	{
		target.RaiseEvent(new KeyEventArgs {
			Key = key,
			KeyModifiers = modifiers,
			RoutedEvent = InputElement.KeyDownEvent,
			Source = target,
		});
	}

	[AvaloniaTest]
	public async Task Ctrl_T_Selects_Type_Mode()
	{
		var (_, pane, vm) = await ShowPaneAsync();
		vm.SelectedSearchMode = vm.SearchModes.First(m => m.Mode == SearchMode.TypeAndMember);

		RaiseKey(pane, Key.T, KeyModifiers.Control);

		vm.SelectedSearchMode.Mode.Should().Be(SearchMode.Type, "Ctrl+T switches the picker to Type mode");
	}

	[AvaloniaTest]
	public async Task Ctrl_M_Selects_Member_Mode()
	{
		var (_, pane, vm) = await ShowPaneAsync();
		vm.SelectedSearchMode = vm.SearchModes.First(m => m.Mode == SearchMode.TypeAndMember);

		RaiseKey(pane, Key.M, KeyModifiers.Control);

		vm.SelectedSearchMode.Mode.Should().Be(SearchMode.Member, "Ctrl+M switches the picker to Member mode");
	}

	[AvaloniaTest]
	public async Task Ctrl_S_Selects_Constant_Mode()
	{
		var (_, pane, vm) = await ShowPaneAsync();
		vm.SelectedSearchMode = vm.SearchModes.First(m => m.Mode == SearchMode.TypeAndMember);

		RaiseKey(pane, Key.S, KeyModifiers.Control);

		vm.SelectedSearchMode.Mode.Should().Be(SearchMode.Literal, "Ctrl+S switches the picker to Constant (literal) mode");
	}

	[AvaloniaTest]
	public async Task Down_Arrow_In_Search_Input_Moves_To_The_First_Result()
	{
		var (_, pane, vm) = await ShowPaneAsync();
		vm.Results.Add(Dummy("Alpha"));
		vm.Results.Add(Dummy("Beta"));
		var input = pane.FindControl<TextBox>("SearchInput")!;
		var results = pane.FindControl<DataGrid>("SearchResults")!;

		RaiseKey(input, Key.Down, KeyModifiers.None);
		Dispatcher.UIThread.RunJobs();

		results.SelectedIndex.Should().Be(0, "Down in the search box selects the first result so the user can keep navigating with the keyboard");
	}

	[AvaloniaTest]
	public async Task Up_Arrow_On_The_First_Result_Returns_Focus_To_The_Search_Input()
	{
		var (_, pane, vm) = await ShowPaneAsync();
		vm.Results.Add(Dummy("Alpha"));
		var input = pane.FindControl<TextBox>("SearchInput")!;
		var results = pane.FindControl<DataGrid>("SearchResults")!;
		results.SelectedIndex = 0;

		RaiseKey(results, Key.Up, KeyModifiers.None);
		Dispatcher.UIThread.RunJobs();

		results.SelectedIndex.Should().Be(-1, "Up from the top of the list clears the selection on the way back to the search box");
		input.IsFocused.Should().BeTrue("Up from the first result returns keyboard focus to the search input");
	}

	[AvaloniaTest]
	public async Task Ctrl_Enter_On_A_Result_Opens_It_In_A_New_Tab()
	{
		var (_, pane, vm) = await ShowPaneAsync();
		var workspace = AppComposition.Current.GetExport<ICSharpCode.ILSpy.Docking.DockWorkspace>();

		vm.SelectedSearchMode = vm.SearchModes.First(m => m.Mode == SearchMode.Type);
		vm.SearchTerm = "Enumerable";
		await Waiters.WaitForAsync(() => vm.Results.Any(r => r is MemberSearchResult),
			timeout: TimeSpan.FromSeconds(30));

		var results = pane.FindControl<DataGrid>("SearchResults")!;
		results.SelectedItem = vm.Results.First(r => r is MemberSearchResult);
		int before = workspace.Documents!.VisibleDockables!.OfType<ContentTabPage>().Count();

		RaiseKey(results, Key.Enter, KeyModifiers.Control);
		for (int i = 0; i < 8; i++)
		{
			Dispatcher.UIThread.RunJobs();
			await Task.Delay(20);
		}

		workspace.Documents!.VisibleDockables!.OfType<ContentTabPage>().Count()
			.Should().BeGreaterThan(before, "Ctrl+Enter on a result opens it in a new document tab instead of reusing the active one");

		vm.SearchTerm = string.Empty;
	}

	[AvaloniaTest]
	public async Task Activate_With_NewTab_Opens_A_New_Document_Tab()
	{
		var (_, _, vm) = await ShowPaneAsync();
		var workspace = AppComposition.Current.GetExport<ICSharpCode.ILSpy.Docking.DockWorkspace>();

		vm.SelectedSearchMode = vm.SearchModes.First(m => m.Mode == SearchMode.Type);
		vm.SearchTerm = "Enumerable";
		await Waiters.WaitForAsync(() => vm.Results.Any(r => r is MemberSearchResult),
			timeout: TimeSpan.FromSeconds(30));

		var hit = (MemberSearchResult)vm.Results.First(r => r is MemberSearchResult);
		int before = workspace.Documents!.VisibleDockables!.OfType<ContentTabPage>().Count();

		vm.Activate(hit, inNewTabPage: true);
		for (int i = 0; i < 8; i++)
		{
			Dispatcher.UIThread.RunJobs();
			await Task.Delay(20);
		}

		workspace.Documents!.VisibleDockables!.OfType<ContentTabPage>().Count()
			.Should().BeGreaterThan(before, "Activate(inNewTabPage: true) must route through OpenNodeInNewTab");

		vm.SearchTerm = string.Empty;
	}
}
