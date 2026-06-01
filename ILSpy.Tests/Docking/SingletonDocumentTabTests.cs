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

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;

using ILSpy.AppEnv;
using ILSpy.Commands;
using ILSpy.Options;
using ILSpy.TextView;
using ILSpy.ViewModels;
using ILSpy.Views;

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
		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		registry.Commands.Single(c => c.Metadata.Header == header).CreateExport().Value.Execute(null);
		Avalonia.Threading.Dispatcher.UIThread.RunJobs();
	}

	[AvaloniaTest]
	public void Reopening_Options_Reuses_The_Same_Tab()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		var dock = vm.DockWorkspace;

		Invoke(window, nameof(Resources._Options));
		var first = dock.Documents!.VisibleDockables!.OfType<ContentTabPage>()
			.Single(t => t.Content is OptionsPageModel);
		var firstContent = first.Content;

		dock.Factory.CloseDockable(first);
		Avalonia.Threading.Dispatcher.UIThread.RunJobs();
		dock.Documents!.VisibleDockables!.OfType<ContentTabPage>()
			.Any(t => t.Content is OptionsPageModel).Should().BeFalse("closing must remove the Options tab");

		Invoke(window, nameof(Resources._Options));
		var second = dock.Documents!.VisibleDockables!.OfType<ContentTabPage>()
			.Single(t => t.Content is OptionsPageModel);

		second.Should().BeSameAs(first,
			"reopening Options must reuse the retained singleton tab, not build a fresh one");
		second.Content.Should().BeSameAs(firstContent,
			"the OptionsPageModel (and its selected page) must survive close/reopen");
	}

	[AvaloniaTest]
	public void Reopening_About_Reuses_The_Same_Tab()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		var dock = vm.DockWorkspace;

		bool IsAbout(ContentTabPage t) => t.Content is DecompilerTabPageModel { Title: var title } && title == Resources.About;

		Invoke(window, nameof(Resources._About));
		var first = dock.Documents!.VisibleDockables!.OfType<ContentTabPage>().Single(IsAbout);

		dock.Factory.CloseDockable(first);
		Avalonia.Threading.Dispatcher.UIThread.RunJobs();

		Invoke(window, nameof(Resources._About));
		var second = dock.Documents!.VisibleDockables!.OfType<ContentTabPage>().Single(IsAbout);

		second.Should().BeSameAs(first,
			"reopening About must reuse the retained singleton tab, not rebuild the page");
	}
}
