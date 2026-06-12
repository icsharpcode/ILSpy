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

using Avalonia.Headless.NUnit;
using Avalonia.Threading;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class StartupAboutWelcomeTests
{
	[AvaloniaTest]
	public async Task Startup_With_No_Saved_Selection_Shows_The_About_Page()
	{
		// On launch with nothing to restore (no saved tree-view path, no command-line target),
		// ILSpy should greet the user with the About page in the main tab, rather than an empty
		// "(no selection)" view. Mirrors the previous version's startup behaviour.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var dock = AppComposition.Current.GetExport<DockWorkspace>();

		// The About page is shown asynchronously after the tree signals it is ready.
		await Waiters.WaitForAsync(
			() => dock.ActiveDecompilerTab is { } tab && Equals(tab.Title, Resources.About),
			description: "the empty-startup main tab should host the About welcome page");

		// No tree node ended up selected, and the active page is the About page.
		((object?)vm.AssemblyTreeModel.SelectedItem).Should().BeNull(
			"nothing should be selected when there is no saved selection to restore");
		var active = dock.ActiveDecompilerTab;
		active.Should().NotBeNull();
		((object?)active!.Title).Should().Be(Resources.About);
		active.Text.Should().NotBeNullOrWhiteSpace("the About page must have rendered its content");
	}

	[AvaloniaTest]
	public async Task Help_About_While_Welcome_Page_Is_Visible_Activates_It_Without_Opening_A_Second_Tab()
	{
		// The welcome page renders the same About content in the reusable main tab. Invoking Help >
		// About while it is on screen must just activate it, not spawn a duplicate static About tab.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var dock = AppComposition.Current.GetExport<DockWorkspace>();
		await Waiters.WaitForAsync(
			() => dock.IsWelcomePageVisible,
			description: "the welcome (About) page must be showing before we invoke Help > About");

		int tabsBefore = dock.Documents!.VisibleDockables!.OfType<ContentTabPage>().Count();

		AppComposition.Current.GetExport<MainMenuCommandRegistry>()
			.GetCommand(nameof(Resources._About)).Execute(null);
		Dispatcher.UIThread.RunJobs();

		dock.Documents!.VisibleDockables!.OfType<ContentTabPage>()
			.Any(t => t.Content is DecompilerTabPageModel { IsStaticContent: true, Title: var title } && Equals(title, Resources.About))
			.Should().BeFalse("invoking About while the welcome page is visible must not open a static About tab");
		dock.Documents!.VisibleDockables!.OfType<ContentTabPage>().Count().Should().Be(tabsBefore,
			"no new tab should be added when the welcome page already shows the About content");
		dock.IsWelcomePageVisible.Should().BeTrue("the welcome page must remain the live main-tab content");
	}
}
