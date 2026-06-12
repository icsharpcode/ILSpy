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

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Docking;

/// <summary>
/// Options and About are static-content singletons: closing and reopening one must reuse the
/// same <see cref="ContentTabPage"/> instance (and therefore its owned view + content state, e.g.
/// the selected options page) rather than rebuilding a fresh tab each time. Pins that contract.
/// </summary>
[TestFixture]
public class SingletonDocumentTabTests
{
	static void Invoke(MainWindow window, string header)
	{
		AppComposition.Current.GetExport<MainMenuCommandRegistry>().GetCommand(header).Execute(null);
		Avalonia.Threading.Dispatcher.UIThread.RunJobs();
	}

	[AvaloniaTest]
	public void Reopening_Options_Reuses_The_Same_Tab()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		var dock = vm.DockWorkspace;
		TestCapture.Step("booted");

		Invoke(window, nameof(Resources._Options));
		TestCapture.Step("options-opened");
		var first = dock.Documents!.VisibleDockables!.OfType<ContentTabPage>()
			.Single(t => t.Content is OptionsPageModel);
		var firstContent = first.Content;

		dock.Factory.CloseDockable(first);
		Avalonia.Threading.Dispatcher.UIThread.RunJobs();
		TestCapture.Step("options-closed");
		dock.Documents!.VisibleDockables!.OfType<ContentTabPage>()
			.Any(t => t.Content is OptionsPageModel).Should().BeFalse("closing must remove the Options tab");

		Invoke(window, nameof(Resources._Options));
		TestCapture.Step("options-reopened");
		var second = dock.Documents!.VisibleDockables!.OfType<ContentTabPage>()
			.Single(t => t.Content is OptionsPageModel);

		second.Should().BeSameAs(first,
			"reopening Options must reuse the retained singleton tab, not build a fresh one");
		second.Content.Should().BeSameAs(firstContent,
			"the OptionsPageModel (and its selected page) must survive close/reopen");
	}

	[AvaloniaTest]
	public async Task Reopening_About_Reuses_The_Same_Tab()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		var dock = vm.DockWorkspace;
		TestCapture.Step("booted");

		// Dismiss the startup welcome page first: while it is visible Help > About activates it
		// rather than opening the static singleton this test pins. Selecting a node replaces the
		// welcome content in the main tab, so About then takes the singleton path deterministically.
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);
		vm.AssemblyTreeModel.SelectNode(vm.AssemblyTreeModel.Root!.Children[0]);
		await Waiters.WaitForAsync(() => !dock.IsWelcomePageVisible,
			description: "the welcome page must be dismissed so Help > About opens the static singleton");

		// Match the singleton menu-About tab specifically (IsStaticContent), NOT the transient boot
		// "welcome" page, which also carries Title == About but is a non-static main-tab page.
		bool IsAbout(ContentTabPage t) => t.Content is DecompilerTabPageModel { Title: var title, IsStaticContent: true } && title == Resources.About;

		Invoke(window, nameof(Resources._About));
		TestCapture.Step("about-opened");
		var first = dock.Documents!.VisibleDockables!.OfType<ContentTabPage>().Single(IsAbout);

		dock.Factory.CloseDockable(first);
		Avalonia.Threading.Dispatcher.UIThread.RunJobs();
		TestCapture.Step("about-closed");

		Invoke(window, nameof(Resources._About));
		TestCapture.Step("about-reopened");
		var second = dock.Documents!.VisibleDockables!.OfType<ContentTabPage>().Single(IsAbout);

		second.Should().BeSameAs(first,
			"reopening About must reuse the retained singleton tab, not rebuild the page");
	}
}
