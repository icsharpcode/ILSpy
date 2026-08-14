// Copyright (c) 2026 Christoph Wille
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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.Options.Panels;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// The options dialog edits the font size in points (the unit Windows font dialogs use),
/// while <see cref="DisplaySettings.SelectedFontSize"/> keeps
/// storing device-independent pixels so persisted settings round-trip with ILSpy 9.x.
/// The pt/px conversion lives in <see cref="DisplaySettingsViewModel.SelectedFontSizePoints"/>.
/// No per-test save/restore of the settings here: ResetAppState rebuilds the container and
/// settings file before every test, so each test starts from pristine defaults.
/// </summary>
[TestFixture]
public class DisplayFontSizeTests
{
	static (DisplaySettingsViewModel page, DisplaySettings settings) CreatePage()
	{
		var service = AppComposition.Current.GetExport<SettingsService>();
		var page = new DisplaySettingsViewModel();
		page.Load(service);
		return (page, service.DisplaySettings);
	}

	[AvaloniaTest]
	public void Default_Pixel_Size_Displays_As_10_Points()
	{
		// Deliberately no arrange: this asserts on the genuine built-in default.
		var (page, settings) = CreatePage();
		settings.SelectedFontSize.Should().BeApproximately(10.0 * 4 / 3, 1e-9,
			"precondition: the built-in default is 10 pt expressed in pixels");
		page.SelectedFontSizePoints.Should().Be("10",
			"the default 13.33 px must be presented as the 10 pt it actually is");
	}

	[AvaloniaTest]
	public void Typed_Point_Size_Is_Stored_As_Pixels()
	{
		var (page, settings) = CreatePage();
		page.SelectedFontSizePoints = "12";
		settings.SelectedFontSize.Should().BeApproximately(12.0 * 4 / 3, 1e-9,
			"the stored value stays device-independent pixels for 9.x round-tripping");
		page.SelectedFontSizePoints.Should().Be("12");
	}

	[AvaloniaTest]
	public void External_Pixel_Change_Refreshes_The_Points_Text()
	{
		// Reset-to-defaults and LoadFromXml write SelectedFontSize directly; the dialog text
		// must follow via PropertyChanged on SelectedFontSizePoints.
		var (page, settings) = CreatePage();
		var notified = new List<string?>();
		page.PropertyChanged += (_, e) => notified.Add(e.PropertyName);

		settings.SelectedFontSize = 20;

		notified.Should().Contain(nameof(DisplaySettingsViewModel.SelectedFontSizePoints));
		page.SelectedFontSizePoints.Should().Be("15");
	}

	[AvaloniaTest]
	public void Setting_Points_Through_The_Dialog_Does_Not_Echo_A_Text_Notification()
	{
		// While the user is typing in the size box, the setter must not raise PropertyChanged
		// for SelectedFontSizePoints - the binding would rewrite the box mid-keystroke
		// (typing "10." would snap back to "10" before the fraction can be completed).
		var (page, _) = CreatePage();
		var notified = new List<string?>();
		page.PropertyChanged += (_, e) => notified.Add(e.PropertyName);

		page.SelectedFontSizePoints = "14";

		notified.Should().NotContain(nameof(DisplaySettingsViewModel.SelectedFontSizePoints));
	}

	[AvaloniaTest]
	public void NonNumeric_And_Empty_Input_Are_Ignored()
	{
		// The empty-string case is load-bearing, not just typing UX: ComboBox re-publishes its
		// still-empty Text when ItemsSource initializes before the Text binding has delivered a
		// value. If the setter ever "helpfully" fell back to a default instead, every Options
		// page load would silently reset the font size.
		var (page, settings) = CreatePage();
		settings.SelectedFontSize = 16;

		page.SelectedFontSizePoints = "abc";
		settings.SelectedFontSize.Should().Be(16,
			"transient garbage while typing must not move the stored size");

		page.SelectedFontSizePoints = "";
		settings.SelectedFontSize.Should().Be(16,
			"the empty Text published during ComboBox ItemsSource initialization must not clobber the setting");
	}

	[AvaloniaTest]
	public void NaN_Input_Is_Ignored()
	{
		// double.TryParse accepts the culture's NaN symbol and NaN falls through Math.Clamp.
		// Persisted, it would fail DecompilerTextEditor's SelectedFontSize > 0 guard on every
		// run, permanently disabling font-size application until the settings file is repaired.
		var (page, settings) = CreatePage();
		settings.SelectedFontSize = 16;

		page.SelectedFontSizePoints = "NaN";

		settings.SelectedFontSize.Should().Be(16);
		double.IsFinite(settings.SelectedFontSize).Should().BeTrue();
	}

	[AvaloniaTest]
	public void Typed_Sizes_Are_Clamped_To_The_6_To_72_Point_Range()
	{
		var (page, settings) = CreatePage();
		page.SelectedFontSizePoints = "1";
		settings.SelectedFontSize.Should().BeApproximately(6.0 * 4 / 3, 1e-9);

		page.SelectedFontSizePoints = "500";
		settings.SelectedFontSize.Should().BeApproximately(72.0 * 4 / 3, 1e-9);
	}

	[AvaloniaTest]
	public void Size_List_Offers_6_Through_24_Points()
	{
		var (page, _) = CreatePage();
		page.FontSizes.Should().Equal(Enumerable.Range(6, 24 - 6 + 1));
	}

	static async Task<ComboBox> OpenDisplayPanelSizeBoxAsync(MainWindow window)
	{
		AppComposition.Current.GetExport<MainMenuCommandRegistry>()
			.GetCommand(nameof(Resources._Options)).Execute(null);
		var vm = (MainWindowViewModel)window.DataContext!;
		var model = (OptionsPageModel)vm.DockWorkspace.Documents!.VisibleDockables!
			.OfType<ContentTabPage>().First(t => t.Content is OptionsPageModel).Content!;
		model.SelectedPage = model.Pages.OfType<DisplaySettingsViewModel>().Single();
		TestCapture.Step("display-page-selected");

		var panel = await window.WaitForComponent<DisplaySettingsPanel>();
		var box = panel.FindControl<ComboBox>("fontSizeComboBox");
		((object?)box).Should().NotBeNull("the size box must be the named editable ComboBox");
		Dispatcher.UIThread.RunJobs();
		return box!;
	}

	[AvaloniaTest]
	public async Task Display_Panel_Size_Box_Is_An_Editable_ComboBox_Showing_Points()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();

		var box = await OpenDisplayPanelSizeBoxAsync(window);

		box.IsEditable.Should().BeTrue("custom sizes must be typeable, like Notepad's font page");
		box.Text.Should().Be("10", "the box shows points, not device-independent pixels");
		// IsEditable=true is only real if the applied ControlTheme materialized the editable
		// text box part; assert the template realized it rather than trusting the property.
		box.ApplyTemplate();
		Dispatcher.UIThread.RunJobs();
		box.GetVisualDescendants().OfType<TextBox>()
			.Should().Contain(t => t.Name == "PART_EditableTextBox",
			"the theme must realize the editable text box, or IsEditable is a no-op");
	}

	[AvaloniaTest]
	public async Task Clamped_Value_Is_Written_Back_Into_The_Box_On_Focus_Loss()
	{
		// Typing "3" stores the clamped 6 pt, but the echo suppression leaves the box showing
		// "3". Focus loss must resync the text from the stored value, like the NumericUpDown
		// this ComboBox replaced did via CommitInput on LostFocus.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var settings = AppComposition.Current.GetExport<SettingsService>().DisplaySettings;

		var box = await OpenDisplayPanelSizeBoxAsync(window);

		// A real focus change, not a synthetic RaiseEvent: LostFocus is typed
		// (FocusChangedEventArgs), so hand-built RoutedEventArgs blow up any typed
		// subscriber on the route - and a genuine focus move is what users do anyway.
		// The editable ComboBox delegates focus to its template's text box, so focus that
		// (ComboBox.Focus() itself returns false when IsEditable).
		box.ApplyTemplate();
		Dispatcher.UIThread.RunJobs();
		var sizeEditor = box.GetVisualDescendants().OfType<TextBox>()
			.First(t => t.Name == "PART_EditableTextBox");
		sizeEditor.Focus().Should().BeTrue("headless focus must land in the size box");
		Dispatcher.UIThread.RunJobs();

		box.Text = "3";
		Dispatcher.UIThread.RunJobs();
		settings.SelectedFontSize.Should().BeApproximately(6.0 * 4 / 3, 1e-9,
			"precondition: the typed 3 is stored clamped to 6 pt");
		TestCapture.Step("undersized-value-typed");

		var elsewhere = box.FindAncestorOfType<DisplaySettingsPanel>()!
			.GetVisualDescendants().OfType<CheckBox>().First();
		elsewhere.Focus().Should().BeTrue("headless focus must be able to leave the size box");
		Dispatcher.UIThread.RunJobs();
		TestCapture.Step("focus-left-size-box");

		box.Text.Should().Be("6", "focus loss must replace the rejected text with the clamped value");
	}
}
