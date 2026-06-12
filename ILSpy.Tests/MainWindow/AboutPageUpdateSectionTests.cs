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

using System;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Controls;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.ViewModels;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// The About page exposes the inline update controls the previous version had: a Check-for-updates /
/// Download button and the "automatically check for updates every week" toggle bound to UpdateSettings.
/// </summary>
[TestFixture]
public class AboutPageUpdateSectionTests
{
	[AvaloniaTest]
	public async Task About_Page_Has_The_AutoCheck_Toggle_And_Update_Button()
	{
		await TestHarness.BootAsync(1);
		var settingsService = AppComposition.Current.GetExport<SettingsService>();
		var updatePanel = AppComposition.Current.GetExport<UpdatePanelViewModel>();
		var dockWorkspace = AppComposition.Current.GetExport<DockWorkspace>();
		var about = new AboutCommand(dockWorkspace, Array.Empty<IAboutPageAddition>(), settingsService, updatePanel);

		var section = (StackPanel)about.BuildUpdateSection();

		var checkBox = section.Children.OfType<CheckBox>().Single();
		((string?)checkBox.Content).Should().Be(Resources.AutomaticallyCheckUpdatesEveryWeek);

		var button = section.Children.OfType<StackPanel>().Single().Children.OfType<Button>().Single();
		button.Command.Should().BeSameAs(updatePanel.DownloadOrCheckUpdateCommand,
			"the button runs the same update check as the toolbar banner");

		var original = settingsService.UpdateSettings.AutomaticUpdateCheckEnabled;
		try
		{
			checkBox.IsChecked = !original;
			settingsService.UpdateSettings.AutomaticUpdateCheckEnabled.Should().Be(!original,
				"the toggle is two-way bound to UpdateSettings.AutomaticUpdateCheckEnabled");
		}
		finally
		{
			settingsService.UpdateSettings.AutomaticUpdateCheckEnabled = original;
		}
	}
}
