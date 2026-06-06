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

using ILSpy.AppEnv;
using ILSpy.Docking;
using ILSpy.TextView;
using ILSpy.ViewModels;
using ILSpy.Views;

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
