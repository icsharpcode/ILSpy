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

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Languages;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class LanguageVersionPersistenceTests
{
	[AvaloniaTest]
	public async Task Switching_Away_From_And_Back_To_A_Versioned_Language_Restores_The_Version()
	{
		// Regression: selecting C# x.y, switching to a language with no versions (IL), then back to
		// C# must restore x.y in the version combo. The toolbar's version ComboBox is bound TwoWay
		// to LanguageService.CurrentVersion; when the language flips, its ItemsSource swaps to the
		// new (empty) language first and writes a null selection back, so the outgoing version has
		// to be stashed before that happens or the restore reads back null.
		var (window, _) = await TestHarness.BootAsync();
		var toolbar = await window.WaitForComponent<MainToolBar>();

		var languageCombo = toolbar.GetVisualDescendants().OfType<ComboBox>()
			.Single(c => c.Name == "LanguageComboBox");
		var versionCombo = toolbar.GetVisualDescendants().OfType<ComboBox>()
			.Single(c => c.Name == "LanguageVersionComboBox");

		var languageService = AppComposition.Current.GetExport<LanguageService>();
		var csharp = languageService.Languages.Single(l => l.Name == "C#");
		var noVersionLanguage = languageService.Languages.First(l => !l.HasLanguageVersions);

		// Start on C# and pick a version that is NOT the default latest, so a reset is detectable.
		languageCombo.SelectedItem = csharp;
		Dispatcher.UIThread.RunJobs();
		var chosen = csharp.LanguageVersions.First();
		chosen.Should().NotBe(csharp.LanguageVersions.Last(), "the test needs a non-default version");
		versionCombo.SelectedItem = chosen;
		Dispatcher.UIThread.RunJobs();
		languageService.CurrentVersion.Should().Be(chosen);

		// Flip to a language with no versions, then back to C#.
		languageCombo.SelectedItem = noVersionLanguage;
		Dispatcher.UIThread.RunJobs();
		languageCombo.SelectedItem = csharp;
		Dispatcher.UIThread.RunJobs();

		languageService.CurrentVersion.Should().Be(chosen,
			"switching away from C# and back must restore the previously selected C# version");
		versionCombo.SelectedItem.Should().Be(chosen,
			"the version ComboBox must reflect the restored version");
	}
}
