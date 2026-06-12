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

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.Search;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Search;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Search;

[TestFixture]
public class SearchPaneStreamingTests
{
	[AvaloniaTest]
	public async Task Diagnostic_Direct_Strategy_Invocation_Surfaces_Type_Results()
	{
		// Diagnostic: drive a MemberSearchStrategy directly to confirm the shared search
		// logic works for the fixture. If this fails, the bug is in the strategy / fixture;
		// if it passes but the orchestrator test below fails, the bug is in our wiring.

		var (_, vm) = await TestHarness.BootAsync();

		var assemblyNode = vm.AssemblyTreeModel.FindCoreLib();
		var module = await assemblyNode.LoadedAssembly.GetMetadataFileAsync();

		var queue = new System.Collections.Concurrent.ConcurrentQueue<ICSharpCode.ILSpyX.Search.SearchResult>();
		var language = AppComposition.Current.GetExport<ICSharpCode.ILSpy.Languages.LanguageService>().CurrentLanguage;
		var request = new ICSharpCode.ILSpyX.Search.SearchRequest {
			Mode = SearchMode.Type,
			Keywords = new[] { "Enumerable" },
			SearchResultFactory = new ICSharpCode.ILSpy.Search.AvaloniaSearchResultFactory(language),
			DecompilerSettings = new ICSharpCode.Decompiler.DecompilerSettings(),
			FullNameSearch = false,
			OmitGenerics = false,
			InNamespace = null!,
			InAssembly = null!,
		};
		var strategy = new ICSharpCode.ILSpyX.Search.MemberSearchStrategy(
			language, ICSharpCode.ILSpyX.ApiVisibility.All, request, queue,
			ICSharpCode.ILSpyX.Search.MemberSearchKind.Type);

		strategy.Search(module, default);
		TestCapture.Step("before-strategy-results");
		queue.Count.Should().BeGreaterThan(0,
			"if the strategy can't find Enumerable in the fixture, the orchestrator certainly won't either");
	}

	[AvaloniaTest]
	public async Task Typing_A_Term_Surfaces_Matching_Type_Results_From_The_Loaded_AssemblyList()
	{
		// End-to-end: set SearchTerm to "Enumerable", let the orchestrator run a TypeAndMember
		// search across the loaded fixture assemblies, and wait for at least one matching
		// result to land in the Results collection.

		await TestHarness.BootAsync();

		var search = AppComposition.Current.GetExport<SearchPaneModel>();
		search.Results.Clear();
		search.SelectedSearchMode = search.SearchModes.First(m => m.Mode == SearchMode.Type);
		search.SearchTerm = "Enumerable";

		await Waiters.WaitForAsync(
			() => search.Results.Any(r => r.Name.Contains("Enumerable", StringComparison.OrdinalIgnoreCase)),
			timeout: TimeSpan.FromSeconds(30));

		TestCapture.Step("search-results-for-enumerable");
		search.Results.Should().NotBeEmpty(
			"the Type search strategy must have surfaced at least one match within the timeout");
		search.Results.Should().Contain(r => r.Name.Contains("Enumerable", StringComparison.OrdinalIgnoreCase));
	}

	[AvaloniaTest]
	public async Task Clearing_The_Search_Term_Empties_The_Result_List()
	{
		// Switching to an empty term tears down the running search and clears the visible
		// results. The user shouldn't see stale rows after they've explicitly cleared the box.

		await TestHarness.BootAsync();

		var search = AppComposition.Current.GetExport<SearchPaneModel>();
		search.Results.Clear();
		search.SelectedSearchMode = search.SearchModes.First(m => m.Mode == SearchMode.Type);
		search.SearchTerm = "Enumerable";

		await Waiters.WaitForAsync(() => search.Results.Count > 0, timeout: TimeSpan.FromSeconds(30));
		TestCapture.Step("search-results-populated");

		search.SearchTerm = string.Empty;
		await Waiters.WaitForAsync(() => search.Results.Count == 0, timeout: TimeSpan.FromSeconds(5));
		TestCapture.Step("results-cleared");
		search.Results.Should().BeEmpty("an empty search term must clear stale results");
	}

	[AvaloniaTest]
	public async Task Minus_Operator_Excludes_Names_Containing_The_Term()
	{
		// `-` keyword prefix is interpreted by AbstractSearchStrategy.IsMatch as "must NOT
		// contain". Confirm end-to-end: a Type search for "String -Builder" returns names
		// containing "String" (System.String, StringComparer, …) but never StringBuilder.

		await TestHarness.BootAsync();

		var search = AppComposition.Current.GetExport<SearchPaneModel>();
		search.Results.Clear();
		search.SelectedSearchMode = search.SearchModes.First(m => m.Mode == SearchMode.Type);
		search.SearchTerm = "String -Builder";

		await Waiters.WaitForAsync(
			() => search.Results.Any(r => r.Name.Contains("String", StringComparison.OrdinalIgnoreCase)),
			timeout: TimeSpan.FromSeconds(30));

		TestCapture.Step("string-minus-builder-results");
		search.Results.Should().NotBeEmpty();
		search.Results.Should().OnlyContain(
			r => r.Name.Contains("String", StringComparison.OrdinalIgnoreCase)
				&& !r.Name.Contains("Builder", StringComparison.OrdinalIgnoreCase),
			"'-Builder' must filter every result whose name contains 'Builder'");
	}

	[AvaloniaTest]
	public async Task Equals_Operator_Requires_Exact_Name_Match()
	{
		// `=` keyword prefix is interpreted by AbstractSearchStrategy.IsMatch as exact match
		// (length-aware; ignores IL backtick generic arity). Confirm: =String must match
		// System.String but NOT StringBuilder or StringComparer.

		await TestHarness.BootAsync();

		var search = AppComposition.Current.GetExport<SearchPaneModel>();
		search.Results.Clear();
		search.SelectedSearchMode = search.SearchModes.First(m => m.Mode == SearchMode.Type);
		search.SearchTerm = "=String";

		await Waiters.WaitForAsync(
			() => search.Results.Any(r => r.Name.Equals("String", StringComparison.OrdinalIgnoreCase)),
			timeout: TimeSpan.FromSeconds(30));

		TestCapture.Step("exact-string-results");
		search.Results.Should().Contain(r => r.Name.Equals("String", StringComparison.OrdinalIgnoreCase));
		search.Results.Should().NotContain(
			r => r.Name.Contains("Builder", StringComparison.OrdinalIgnoreCase),
			"'=String' must reject names that aren't exactly 'String'");
		search.Results.Should().NotContain(
			r => r.Name.Equals("StringComparer", StringComparison.OrdinalIgnoreCase),
			"'=String' is an exact match, not a prefix match");
	}
}
