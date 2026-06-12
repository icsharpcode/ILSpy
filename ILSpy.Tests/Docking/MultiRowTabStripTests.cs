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
using Avalonia.Threading;
using Avalonia.VisualTree;

using AwesomeAssertions;

using Dock.Avalonia.Controls;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Docking;

/// <summary>
/// With multi-line mode enabled (<see cref="SessionSettings.MultiLineDocumentTabs"/>), many document
/// tabs flow onto multiple rows (a WrapPanel) instead of a single scrolling row.
/// </summary>
[TestFixture]
public class MultiRowTabStripTests
{
	[AvaloniaTest]
	public async Task Many_Document_Tabs_Wrap_To_Multiple_Rows()
	{
		var settings = AppComposition.Current.GetExport<SettingsService>().SessionSettings;
		var saved = settings.MultiLineDocumentTabs;
		try
		{
			var window = AppComposition.Current.GetExport<MainWindow>();
			window.Show();
			var vm = (MainWindowViewModel)window.DataContext!;
			await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

			settings.MultiLineDocumentTabs = true;
			for (int i = 0; i < 40; i++)
				vm.DockWorkspace.OpenNewTab(new DecompilerTabPageModel { Title = $"Tab number {i:00}" });

			for (int i = 0; i < 12; i++)
			{
				Dispatcher.UIThread.RunJobs();
				window.UpdateLayout();
				await Task.Delay(20);
			}

			var strip = window.GetVisualDescendants().OfType<DocumentTabStrip>().FirstOrDefault();
			strip.Should().NotBeNull("the document tab strip must be realised");

			strip!.GetVisualDescendants().OfType<WrapPanel>().Should().NotBeEmpty(
				"the behaviour must swap the strip's ItemsPanel to a WrapPanel");

			var rows = strip.GetVisualDescendants().OfType<DocumentTabStripItem>()
				.Where(it => it.Bounds.Width > 0)
				.Select(it => System.Math.Round(it.Bounds.Y))
				.Distinct().ToList();

			rows.Count.Should().BeGreaterThan(1, "40 tabs must wrap onto more than one row");
		}
		finally
		{ settings.MultiLineDocumentTabs = saved; }
	}
}
