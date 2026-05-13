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

using AwesomeAssertions;

using ILSpy;
using ILSpy.AppEnv;
using ILSpy.Search;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Search;

[TestFixture]
public class SearchTermFilterTests
{
	[AvaloniaTest]
	public Task SearchTermMatches_Empty_Term_Matches_Anything()
	{
		var settings = AppComposition.Current.GetExport<SettingsService>().SessionSettings.LanguageSettings;
		settings.SearchTerm = string.Empty;
		settings.SearchTermMatches("Object").Should().BeTrue();
		settings.SearchTermMatches("IEnumerable").Should().BeTrue();
		settings.SearchTermMatches(string.Empty).Should().BeTrue();
		return Task.CompletedTask;
	}

	[AvaloniaTest]
	public Task SearchTermMatches_Is_Case_Insensitive_Substring()
	{
		var settings = AppComposition.Current.GetExport<SettingsService>().SessionSettings.LanguageSettings;
		settings.SearchTerm = "enum";
		try
		{
			settings.SearchTermMatches("IEnumerable").Should().BeTrue(
				"contains check is case-insensitive — IEnumerable contains 'enum'");
			settings.SearchTermMatches("Enumerator").Should().BeTrue();
			settings.SearchTermMatches("Object").Should().BeFalse(
				"Object does not contain 'enum'");
		}
		finally
		{
			settings.SearchTerm = string.Empty;
		}
		return Task.CompletedTask;
	}

	[AvaloniaTest]
	public Task Typing_In_The_Search_Pane_Pushes_The_Term_Into_LanguageSettings()
	{
		var search = AppComposition.Current.GetExport<SearchPaneModel>();
		var settings = AppComposition.Current.GetExport<SettingsService>().SessionSettings.LanguageSettings;
		settings.SearchTerm.Should().Be(string.Empty,
			"clean baseline — no prior test leaked a term");

		search.SearchTerm = "Enumerable";
		try
		{
			settings.SearchTerm.Should().Be("Enumerable",
				"SearchPaneModel must push the term to LanguageSettings so the assembly-tree filter cascade activates");
		}
		finally
		{
			search.SearchTerm = string.Empty;
		}
		return Task.CompletedTask;
	}
}
