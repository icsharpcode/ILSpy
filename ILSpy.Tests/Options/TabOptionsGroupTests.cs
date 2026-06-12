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

using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// The Display options page exposes a "Tab options" group with two checkboxes that bind two-way to
/// the persisted document-tab-strip settings on SessionSettings: one toggles multi-row tabs, the
/// other gates the mouse-wheel toggle gesture.
/// </summary>
[TestFixture]
public class TabOptionsGroupTests
{
	const string MultiRowLabel = "Enable multiple rows in tab strip";
	const string WheelGateLabel = "Mouse wheel scroll toggles single/multiple row tab strip";

	[AvaloniaTest]
	public void Tab_Options_Checkboxes_Two_Way_Bind_To_SessionSettings()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Width = 900;
		window.Height = 600;
		window.Show();
		Dispatcher.UIThread.RunJobs();

		var command = AppComposition.Current.GetExport<MainMenuCommandRegistry>()
			.GetCommand(nameof(Resources._Options));
		command.Execute(null);
		Dispatcher.UIThread.RunJobs();

		var view = window.GetVisualDescendants().OfType<OptionsPageView>().Single();
		var model = (OptionsPageModel)((ContentTabPage)((MainWindowViewModel)window.DataContext!)
			.DockWorkspace.Documents!.VisibleDockables!
			.OfType<ContentTabPage>().Single(t => t.Content is OptionsPageModel)).Content!;
		var page = model.Pages.OfType<DisplaySettingsViewModel>().Single();
		model.SelectedPage = page;
		Dispatcher.UIThread.RunJobs();

		var multiRow = view.GetVisualDescendants().OfType<CheckBox>()
			.FirstOrDefault(cb => cb.Content?.ToString() == MultiRowLabel);
		var wheelGate = view.GetVisualDescendants().OfType<CheckBox>()
			.FirstOrDefault(cb => cb.Content?.ToString() == WheelGateLabel);

		multiRow.Should().NotBeNull("the Tab options group must expose the multi-row toggle checkbox");
		wheelGate.Should().NotBeNull("the Tab options group must expose the mouse-wheel-gate checkbox");

		var session = page.SessionSettings;

		// Checkbox 1 <-> MultiLineDocumentTabs
		session.MultiLineDocumentTabs = false;
		Dispatcher.UIThread.RunJobs();
		multiRow!.IsChecked.Should().BeFalse("the checkbox reflects the current MultiLineDocumentTabs");
		multiRow.IsChecked = true;
		Dispatcher.UIThread.RunJobs();
		session.MultiLineDocumentTabs.Should().BeTrue("checking the box flows back to MultiLineDocumentTabs");

		// Checkbox 2 <-> MouseWheelTogglesTabStripRows
		session.MouseWheelTogglesTabStripRows = true;
		Dispatcher.UIThread.RunJobs();
		wheelGate!.IsChecked.Should().BeTrue("the checkbox reflects the current MouseWheelTogglesTabStripRows");
		wheelGate.IsChecked = false;
		Dispatcher.UIThread.RunJobs();
		session.MouseWheelTogglesTabStripRows.Should().BeFalse(
			"unchecking the box flows back to MouseWheelTogglesTabStripRows");
	}
}
