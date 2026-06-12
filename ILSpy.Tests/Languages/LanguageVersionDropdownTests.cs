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
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class LanguageVersionDropdownTests
{
	[AvaloniaTest]
	public async Task Toolbar_Has_Language_Version_ComboBox_Listing_C_Sharp_Versions()
	{
		// The toolbar surfaces a second ComboBox for picking the C# language version (parallel
		// to the WPF MainToolBar.xaml `languageVersionComboBox`). Items come from the active
		// Language's LanguageVersions property; with C# active the list spans CSharp1 → CSharp14.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();

		var combo = window.GetVisualDescendants().OfType<ComboBox>()
			.SingleOrDefault(c => c.Name == "LanguageVersionComboBox");
		((object?)combo).Should().NotBeNull(
			"the Language-Version ComboBox must exist in the toolbar");

		var versions = combo!.ItemsSource?.OfType<LanguageVersion>().ToList() ?? new();
		versions.Should().NotBeEmpty(
			"with C# selected, LanguageVersions exposes the supported C# versions");
		versions.Should().Contain(v => v.DisplayName.StartsWith("C# 14"),
			"the version list mirrors WPF's, which currently runs through C# 14.0");
		versions.Should().Contain(v => v.DisplayName.StartsWith("C# 1.0"),
			"history-spanning entries (C# 1.0 / VS .NET) must still be available");
	}

	[AvaloniaTest]
	public async Task Selecting_Language_Version_Writes_Through_To_LanguageSettings_LanguageVersionId()
	{
		// Picking a version from the dropdown updates the LanguageSettings record so the choice
		// round-trips through XML (already wired) and reaches the decompiler pipeline.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var settings = AppComposition.Current.GetExport<SettingsService>().SessionSettings.LanguageSettings;

		var languageService = AppComposition.Current.GetExport<LanguageService>();
		var versions = languageService.CurrentLanguage.LanguageVersions;
		var target = versions.FirstOrDefault(v => v.DisplayName.StartsWith("C# 9.0"));
		((object?)target).Should().NotBeNull("CSharp9_0 must be present in the version list");

		// Act — set via the service (the toolbar combo's two-way binding does the same thing).
		languageService.CurrentVersion = target;

		// Assert — write-back to LanguageSettings happened.
		settings.LanguageVersionId.Should().Be(target!.Version,
			"selecting a version from the toolbar combo must write its Version id into LanguageSettings");
	}

	[AvaloniaTest]
	public async Task Language_Version_ComboBox_Hidden_For_IL_Language()
	{
		// IL has no language versions — the combo's Visibility binding to HasLanguageVersions
		// keeps it out of the toolbar so the layout doesn't show an empty dropdown.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();

		var languageService = AppComposition.Current.GetExport<LanguageService>();
		var ilLanguage = languageService.Languages.FirstOrDefault(l => l.Name == "IL");
		((object?)ilLanguage).Should().NotBeNull("IL must be a registered output language");

		// Act — switch to IL.
		languageService.CurrentLanguage = ilLanguage!;
		await Waiters.WaitForAsync(() => !languageService.CurrentLanguage.HasLanguageVersions);

		var combo = window.GetVisualDescendants().OfType<ComboBox>()
			.Single(c => c.Name == "LanguageVersionComboBox");
		combo.IsVisible.Should().BeFalse("IL has no LanguageVersions, so the combo hides");
	}
}
