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
/// The options dialog edits the font size in points (like the Windows font dialogs and the
/// WPF host's FontSizeConverter), while <see cref="DisplaySettings.SelectedFontSize"/> keeps
/// storing device-independent pixels so persisted settings round-trip with ILSpy 9.x.
/// The pt/px conversion lives in <see cref="DisplaySettingsViewModel.SelectedFontSizePoints"/>.
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
		var (page, settings) = CreatePage();
		var original = settings.SelectedFontSize;
		try
		{
			settings.SelectedFontSize = 10.0 * 4 / 3;
			page.SelectedFontSizePoints.Should().Be("10",
				"the default 13.33 px must be presented as the 10 pt it actually is");
		}
		finally
		{
			settings.SelectedFontSize = original;
		}
	}

	[AvaloniaTest]
	public void Typed_Point_Size_Is_Stored_As_Pixels()
	{
		var (page, settings) = CreatePage();
		var original = settings.SelectedFontSize;
		try
		{
			page.SelectedFontSizePoints = "12";
			settings.SelectedFontSize.Should().BeApproximately(12.0 * 4 / 3, 1e-9,
				"the stored value stays device-independent pixels for 9.x round-tripping");
			page.SelectedFontSizePoints.Should().Be("12");
		}
		finally
		{
			settings.SelectedFontSize = original;
		}
	}

	[AvaloniaTest]
	public void External_Pixel_Change_Refreshes_The_Points_Text()
	{
		// Reset-to-defaults and LoadFromXml write SelectedFontSize directly; the dialog text
		// must follow via PropertyChanged on SelectedFontSizePoints.
		var (page, settings) = CreatePage();
		var original = settings.SelectedFontSize;
		try
		{
			var notified = new List<string?>();
			page.PropertyChanged += (_, e) => notified.Add(e.PropertyName);

			settings.SelectedFontSize = 20;

			notified.Should().Contain(nameof(DisplaySettingsViewModel.SelectedFontSizePoints));
			page.SelectedFontSizePoints.Should().Be("15");
		}
		finally
		{
			settings.SelectedFontSize = original;
		}
	}

	[AvaloniaTest]
	public void Setting_Points_Through_The_Dialog_Does_Not_Echo_A_Text_Notification()
	{
		// While the user is typing in the size box, the setter must not raise PropertyChanged
		// for SelectedFontSizePoints - the binding would rewrite the box mid-keystroke
		// (typing "10." would snap back to "10" before the fraction can be completed).
		var (page, settings) = CreatePage();
		var original = settings.SelectedFontSize;
		try
		{
			var notified = new List<string?>();
			page.PropertyChanged += (_, e) => notified.Add(e.PropertyName);

			page.SelectedFontSizePoints = "14";

			notified.Should().NotContain(nameof(DisplaySettingsViewModel.SelectedFontSizePoints));
		}
		finally
		{
			settings.SelectedFontSize = original;
		}
	}

	[AvaloniaTest]
	public void NonNumeric_Input_Is_Ignored()
	{
		var (page, settings) = CreatePage();
		var original = settings.SelectedFontSize;
		try
		{
			settings.SelectedFontSize = 16;
			page.SelectedFontSizePoints = "abc";
			settings.SelectedFontSize.Should().Be(16,
				"transient garbage while typing must not move the stored size");
		}
		finally
		{
			settings.SelectedFontSize = original;
		}
	}

	[AvaloniaTest]
	public void Typed_Sizes_Are_Clamped_To_The_6_To_72_Point_Range()
	{
		var (page, settings) = CreatePage();
		var original = settings.SelectedFontSize;
		try
		{
			page.SelectedFontSizePoints = "1";
			settings.SelectedFontSize.Should().BeApproximately(6.0 * 4 / 3, 1e-9);

			page.SelectedFontSizePoints = "500";
			settings.SelectedFontSize.Should().BeApproximately(72.0 * 4 / 3, 1e-9);
		}
		finally
		{
			settings.SelectedFontSize = original;
		}
	}

	[AvaloniaTest]
	public void Size_List_Offers_6_Through_24_Points_Like_The_WPF_Host()
	{
		var (page, _) = CreatePage();
		page.FontSizes.Should().Equal(Enumerable.Range(6, 24 - 6 + 1));
	}

	[AvaloniaTest]
	public async Task Display_Panel_Size_Box_Is_An_Editable_ComboBox_Showing_Points()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var settings = AppComposition.Current.GetExport<SettingsService>();
		var original = settings.DisplaySettings.SelectedFontSize;
		try
		{
			settings.DisplaySettings.SelectedFontSize = 10.0 * 4 / 3;

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

			box!.IsEditable.Should().BeTrue("custom sizes must be typeable, like Notepad's font page");
			box.Text.Should().Be("10", "the box shows points, not device-independent pixels");
		}
		finally
		{
			settings.DisplaySettings.SelectedFontSize = original;
		}
	}
}
