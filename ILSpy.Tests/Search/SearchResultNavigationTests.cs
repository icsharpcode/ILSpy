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
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Search;

[TestFixture]
public class SearchResultNavigationTests
{
	[AvaloniaTest]
	public async Task Activating_A_Member_Search_Result_Selects_That_Member_In_The_Assembly_Tree()
	{
		// End-to-end: run a search, take the first MemberSearchResult, ask the search pane
		// to activate it, and confirm the assembly tree's SelectedItem becomes the matching
		// tree node.

		var (_, vm) = await TestHarness.BootAsync();

		var search = AppComposition.Current.GetExport<SearchPaneModel>();
		search.Results.Clear();
		search.SelectedSearchMode = search.SearchModes.First(m => m.Mode == SearchMode.Type);
		search.SearchTerm = "Enumerable";
		await Waiters.WaitForAsync(() => search.Results.Any(r => r is MemberSearchResult),
			timeout: TimeSpan.FromSeconds(30));

		TestCapture.Step("search-results-for-enumerable");
		var hit = (MemberSearchResult)search.Results.First(r => r is MemberSearchResult);
		search.Activate(hit);

		await Waiters.WaitForAsync(() => vm.AssemblyTreeModel.SelectedItem is TypeTreeNode);
		TestCapture.Step("navigated-to-result");
		((object?)vm.AssemblyTreeModel.SelectedItem).Should().NotBeNull(
			"Activate must move the assembly-tree selection to the result's underlying entity");
		(vm.AssemblyTreeModel.SelectedItem is TypeTreeNode).Should().BeTrue(
			"the type-mode search returned a TypeDefinition; the assembly tree must land on the matching TypeTreeNode");

		// Tidy up so subsequent tests don't see leftover term.
		search.SearchTerm = string.Empty;
	}
}
