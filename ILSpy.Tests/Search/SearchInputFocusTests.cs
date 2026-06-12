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

using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Threading;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Search;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Search;

[TestFixture]
public class SearchInputFocusTests
{
	[AvaloniaTest]
	public async Task ShowSearchCommand_Raises_FocusRequested_So_The_View_Pushes_Focus_To_The_Input()
	{
		// Ctrl+E / Ctrl+Shift+F must leave the SearchInput TextBox focused so the user
		// can start typing immediately. The VM raises FocusRequested; the SearchPane
		// code-behind subscribes and calls Focus() on the TextBox.

		await TestHarness.BootAsync();

		var search = AppComposition.Current.GetExport<SearchPaneModel>();
		var dockWorkspace = AppComposition.Current.GetExport<DockWorkspace>();

		var fired = 0;
		search.FocusRequested += () => fired++;

		dockWorkspace.ShowSearchCommand.Execute(null);
		TestCapture.Step("show-search-executed");

		fired.Should().BeGreaterThan(0,
			"ShowSearchCommand must request focus on the search input, not just activate the pane");
	}

	[AvaloniaTest]
	public async Task SearchInput_Is_Focused_After_ShowSearchCommand_Pumps_Through_The_Dispatcher()
	{
		var (window, _) = await TestHarness.BootAsync();

		var dockWorkspace = AppComposition.Current.GetExport<DockWorkspace>();
		// Search is hidden by default; surface it so its view realises.
		dockWorkspace.ShowToolPane(ICSharpCode.ILSpy.Search.SearchPaneModel.PaneContentId);
		var pane = await window.WaitForComponent<SearchPane>();

		dockWorkspace.ShowSearchCommand.Execute(null);

		// Focus shifts via a Dispatcher.UIThread.Post so the view has a frame for the
		// pane to surface in the layout — pump the dispatcher before asserting.
		Dispatcher.UIThread.RunJobs();
		TestCapture.Step("search-input-focused");

		var input = pane.FindControl<TextBox>("SearchInput");
		((object?)input).Should().NotBeNull();
		input!.IsFocused.Should().BeTrue(
			"the TextBox holds keyboard focus after ShowSearchCommand so the user can start typing without a click");
	}
}
