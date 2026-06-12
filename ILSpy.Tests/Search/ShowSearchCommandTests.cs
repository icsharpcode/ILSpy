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

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Search;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Search;

[TestFixture]
public class ShowSearchCommandTests
{
	[AvaloniaTest]
	public async Task ShowSearchCommand_Activates_The_Search_Pane()
	{
		// Ctrl+Shift+F / Ctrl+E in the live app are wired to this command via
		// MainWindow.KeyBindings. Here we drive the command directly and verify the
		// activation reaches the SearchPaneModel.

		await TestHarness.BootAsync();

		var dockWorkspace = AppComposition.Current.GetExport<DockWorkspace>();
		var search = AppComposition.Current.GetExport<SearchPaneModel>();

		dockWorkspace.ShowSearchCommand.CanExecute(null).Should().BeTrue();
		dockWorkspace.ShowSearchCommand.Execute(null);
		TestCapture.Step("search-pane-activated");

		// The dock factory's ActiveDockable should now be the search pane. We can't
		// assert against the factory's deep state easily (IDockable identity), so check
		// the pane's IsActive flag, which Dock.Avalonia flips during SetActiveDockable.
		search.IsActive.Should().BeTrue(
			"ShowSearchCommand must bring the search pane to the active tab in its dock group");
	}
}
