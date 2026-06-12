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

using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.Themes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// The Display options page exposes a theme picker bound to SessionSettings.Theme, which switches the
/// theme live (ThemeManager applies it). (Regression: the picker was dropped from the Avalonia panel.)
/// </summary>
[TestFixture]
public class ThemePickerTests
{
	[AvaloniaTest]
	public async Task Display_Options_Theme_Picker_Lists_Themes_And_Switches_Live()
	{
		await TestHarness.BootAsync(1);
		var settingsService = AppComposition.Current.GetExport<SettingsService>();
		var original = settingsService.SessionSettings.Theme;
		try
		{
			var vm = new DisplaySettingsViewModel();
			vm.Load(settingsService);

			vm.AllThemes.Should().Contain("Light").And.Contain("Dark");

			// The picker binds two-way to the live SessionSettings.Theme -- the exact property
			// SetThemeCommand sets and ThemeManager applies, so the picker switches the theme.
			vm.SessionSettings.Should().BeSameAs(settingsService.SessionSettings);
			vm.SessionSettings.Theme = "Dark";
			settingsService.SessionSettings.Theme.Should().Be("Dark");

			vm.LoadDefaults();
			vm.SessionSettings.Theme.Should().Be(ThemeManager.Current.DefaultTheme,
				"resetting Display defaults restores the default theme");
		}
		finally
		{
			settingsService.SessionSettings.Theme = original;
		}
	}
}
