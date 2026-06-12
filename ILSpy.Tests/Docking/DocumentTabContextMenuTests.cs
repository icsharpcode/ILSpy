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

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Docking;

/// <summary>
/// The document-tab context menu (right-click a tab) offers Close / Close all but this / Close all.
/// These pin the commands the menu binds to (defined on <see cref="ContentTabPage"/>, delegating to
/// <c>DockWorkspace</c>).
/// </summary>
[TestFixture]
public class DocumentTabContextMenuTests
{
	static (ContentTabPage a, ContentTabPage b, ContentTabPage c) OpenThreeTabs(DockWorkspace dock)
	{
		var a = dock.OpenNewTab(new DecompilerTabPageModel { Title = "A" });
		var b = dock.OpenNewTab(new DecompilerTabPageModel { Title = "B" });
		var c = dock.OpenNewTab(new DecompilerTabPageModel { Title = "C" });
		Dispatcher.UIThread.RunJobs();
		return (a, b, c);
	}

	[AvaloniaTest]
	public async Task Menu_Carries_The_Close_Entries_Bound_To_The_Tab_Commands()
	{
		// Regression: the per-tab DocumentContextMenu (which Dock shows, overriding the standard
		// ContextMenu) once held only "Freeze tab", so the Close / Close all entries went missing
		// (and the whole menu was empty on a frozen tab). It must carry all three close entries.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);
		var (a, _, _) = OpenThreeTabs(vm.DockWorkspace);

		var menu = ICSharpCode.ILSpy.Themes.PreviewTabContextMenuBehavior.BuildDocumentContextMenu(a);
		var items = menu.ItemsSource!.OfType<global::Avalonia.Controls.MenuItem>().ToList();

		items.Select(m => m.Header).Should().Contain(new object[] {
			ICSharpCode.ILSpy.Properties.Resources.Close,
			ICSharpCode.ILSpy.Properties.Resources.CloseAllButThisTab,
			ICSharpCode.ILSpy.Properties.Resources.CloseAllTabs,
		});
		items.Single(m => Equals(m.Header, ICSharpCode.ILSpy.Properties.Resources.Close)).Command
			.Should().BeSameAs(a.CloseCommand);
		items.Single(m => Equals(m.Header, ICSharpCode.ILSpy.Properties.Resources.CloseAllButThisTab)).Command
			.Should().BeSameAs(a.CloseAllButThisCommand);
		items.Single(m => Equals(m.Header, ICSharpCode.ILSpy.Properties.Resources.CloseAllTabs)).Command
			.Should().BeSameAs(a.CloseAllCommand);
	}

	[AvaloniaTest]
	public async Task Close_Entry_Is_Disabled_When_The_Tab_Cannot_Be_Closed()
	{
		// The last remaining document is made un-closeable (DockWorkspace.UpdateLastDocumentCanClose
		// flips CanClose to false so the user can't empty the document area). The context-menu Close
		// entry must honour that the same way Dock's own close button does, so it binds IsEnabled to
		// the tab's CanClose.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);
		var (a, _, _) = OpenThreeTabs(vm.DockWorkspace);

		var menu = ICSharpCode.ILSpy.Themes.PreviewTabContextMenuBehavior.BuildDocumentContextMenu(a);
		var close = menu.ItemsSource!.OfType<global::Avalonia.Controls.MenuItem>()
			.Single(m => Equals(m.Header, ICSharpCode.ILSpy.Properties.Resources.Close));

		a.CanClose = true;
		Dispatcher.UIThread.RunJobs();
		close.IsEnabled.Should().BeTrue("a closeable tab's Close entry is enabled");

		a.CanClose = false;
		Dispatcher.UIThread.RunJobs();
		close.IsEnabled.Should().BeFalse("an un-closeable tab's Close entry is disabled");
	}

	[AvaloniaTest]
	public async Task Close_All_But_This_Leaves_Only_The_Target_Tab()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);
		var dock = vm.DockWorkspace;

		var (a, b, c) = OpenThreeTabs(dock);
		dock.Documents!.VisibleDockables!.OfType<ContentTabPage>().Should().Contain(new[] { a, b, c });

		b.CloseAllButThisCommand.Execute(null);
		Dispatcher.UIThread.RunJobs();

		dock.Documents!.VisibleDockables!.OfType<ContentTabPage>()
			.Should().ContainSingle().Which.Should().BeSameAs(b);
	}

	[AvaloniaTest]
	public async Task Close_All_Closes_Every_Carve_Out_Tab_But_Keeps_The_Home_Tab()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);
		var dock = vm.DockWorkspace;

		var (a, b, c) = OpenThreeTabs(dock);

		a.CloseAllCommand.Execute(null);
		Dispatcher.UIThread.RunJobs();

		var remaining = dock.Documents!.VisibleDockables!.OfType<ContentTabPage>().ToList();
		// Every opened tab is closed; the persistent One (preview/home) survives so the app keeps a
		// main tab to project the next selection onto.
		remaining.Should().NotContain(new[] { a, b, c });
		remaining.Should().Contain(((ILSpyDockFactory)dock.Factory).MainTab!,
			"Close all keeps the persistent home/preview tab");
	}

	[AvaloniaTest]
	public async Task Close_Closes_Only_That_Tab()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);
		var dock = vm.DockWorkspace;

		var (a, b, c) = OpenThreeTabs(dock);

		b.CloseCommand.Execute(null);
		Dispatcher.UIThread.RunJobs();

		var remaining = dock.Documents!.VisibleDockables!.OfType<ContentTabPage>().ToList();
		remaining.Should().Contain(new[] { a, c });
		remaining.Should().NotContain(b);
	}
}
