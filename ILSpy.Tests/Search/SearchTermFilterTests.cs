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
using ICSharpCode.ILSpy.Search;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Search;

[TestFixture]
public class SearchTermFilterTests
{
	[AvaloniaTest]
	public Task SearchTermMatches_Is_A_No_Op_That_Always_Returns_True()
	{
		// Pins the WPF-parity contract: the search pane drives its own results via the
		// ILSpyX search strategies; LanguageSettings.SearchTermMatches deliberately ignores
		// SearchTerm so the assembly-tree filter cascade stays independent of the search
		// pane. Without this, typing a term into the search box would hide tree rows whose
		// names don't match it — including member rows under a type whose own name DOES
		// match — because the cascade only resets the match bit one level deep.

		var settings = AppComposition.Current.GetExport<SettingsService>().SessionSettings.LanguageSettings;
		try
		{
			settings.SearchTerm = string.Empty;
			settings.SearchTermMatches("Object").Should().BeTrue();
			settings.SearchTermMatches("IEnumerable").Should().BeTrue();
			settings.SearchTermMatches(string.Empty).Should().BeTrue();

			settings.SearchTerm = "enum";
			settings.SearchTermMatches("IEnumerable").Should().BeTrue();
			settings.SearchTermMatches("Object").Should().BeTrue(
				"WPF parity: SearchTermMatches must NOT honour the SearchTerm — it's a no-op shim");

			settings.SearchTerm = "ZZZ_NoMatchAnywhere";
			settings.SearchTermMatches("Anything").Should().BeTrue();
		}
		finally
		{
			settings.SearchTerm = string.Empty;
		}
		return Task.CompletedTask;
	}

	[AvaloniaTest]
	public Task Typing_In_The_Search_Pane_Does_Not_Bleed_Into_LanguageSettings_SearchTerm()
	{
		// The search pane is decoupled from the assembly-tree filter cascade. Earlier port
		// commits pushed SearchPaneModel.SearchTerm into LanguageSettings.SearchTerm to drive
		// a tree-filter cascade — that path hid member rows users expected to see (e.g. enum
		// literals under their matched-by-name enum type). Reverted to WPF parity: typing in
		// the search pane drives the orchestrator only.

		var search = AppComposition.Current.GetExport<SearchPaneModel>();
		var settings = AppComposition.Current.GetExport<SettingsService>().SessionSettings.LanguageSettings;
		settings.SearchTerm.Should().Be(string.Empty,
			"clean baseline — no prior test leaked a term");

		search.SearchTerm = "Enumerable";
		try
		{
			settings.SearchTerm.Should().Be(string.Empty,
				"SearchPaneModel must not bleed its term into LanguageSettings; the cascade stays independent");
		}
		finally
		{
			search.SearchTerm = string.Empty;
		}
		return Task.CompletedTask;
	}
}
