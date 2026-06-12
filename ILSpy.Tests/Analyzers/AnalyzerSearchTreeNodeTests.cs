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

using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Analyzers;

using ICSharpCode.ILSpy.Analyzers;
using ICSharpCode.ILSpy.Analyzers.TreeNodes;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Analyzers;

[TestFixture]
public class AnalyzerSearchTreeNodeTests
{
	[AvaloniaTest]
	public async Task Expanding_A_Search_Node_Runs_The_Analyzer_And_Surfaces_Result_Rows()
	{
		// The search-node is the bridge between an IAnalyzer's enumeration and the UI tree:
		// LoadChildren spins up a background fetch, posts results as Analyzed*TreeNode
		// children, and once the task settles the row's Text gains the result count plus
		// the elapsed-time stamp ("Used By (N in M ms)"). This single test exercises all
		// three contracts on a small fixture so it stays fast.

		var (_, vm) = await TestHarness.BootAsync();

		// Pick a small target so the analyzer completes quickly in headless. The "Used By"
		// analyzer applies to every concrete type; we use a relatively unused type from
		// System.Linq to keep the result count tame.
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		var target = (ICSharpCode.Decompiler.TypeSystem.ITypeDefinition)typeNode.Member!;

		// "Used By" is the header for both TypeUsedByAnalyzer and MethodUsedByAnalyzer —
		// pick the one whose Show(target) accepts the type.
		var analyzer = AnalyzerTreeNode.Analyzers
			.Where(a => a.Metadata?.Header == "Used By")
			.Select(a => a.CreateExport().Value)
			.First(a => a.Show(target));

		var search = new AnalyzerSearchTreeNode(target, analyzer, "Used By");
		search.IsExpanded = true; // triggers LazyLoading → LoadChildren
		search.EnsureLazyChildren();

		await Waiters.WaitForAsync(
			() => search.Children.Count > 0 && !search.IsLoading,
			timeout: TimeSpan.FromSeconds(30));

		TestCapture.Step("before-search-results");
		search.Children.Should().NotBeEmpty(
			"the Used By analyzer must surface at least one entity that references Enumerable");
		search.Children.Should().AllBeAssignableTo<AnalyzerTreeNode>(
			"every search result must wrap into an analyzer-tree-node so the user can drill further");

		search.Text.ToString().Should().StartWith("Used By",
			"the header text must persist as the prefix of the row label");
		search.Text.ToString().Should().MatchRegex(@"Used By \(\d+ in \d+ ms\)",
			"once the fetch completes, the row appends '(<count> in <ms> ms)'");
	}

	[AvaloniaTest]
	public async Task Collapsing_A_Search_Node_Mid_Fetch_Cancels_And_Re_Arms_It_For_The_Next_Expand()
	{
		// Cancellation contract: collapsing the row while results are still streaming should
		// stop the analyzer task and wipe the partial results so a subsequent re-expand sees
		// a clean fetch. The "Used By" analyzer over a deeply-referenced type takes long
		// enough that we can race a collapse against it.

		var (_, vm) = await TestHarness.BootAsync();

		// Object is referenced by almost every type — the analyzer streams for a while.
		var assemblyNode = vm.AssemblyTreeModel.FindCoreLib();
		var module = assemblyNode.LoadedAssembly.GetMetadataFileOrNull();
		module.Should().NotBeNull();
		var typeSystem = module!.GetTypeSystemOrNull();
		typeSystem.Should().NotBeNull();
		var target = typeSystem!.MainModule.TopLevelTypeDefinitions
			.Single(t => t.FullName == "System.Object");

		var analyzer = AnalyzerTreeNode.Analyzers
			.Where(a => a.Metadata?.Header == "Used By")
			.Select(a => a.CreateExport().Value)
			.First(a => a.Show(target));

		var search = new AnalyzerSearchTreeNode(target, analyzer, "Used By");
		search.IsExpanded = true;
		search.EnsureLazyChildren();

		// Collapse before completion to exercise the cancellation path.
		search.IsExpanded = false;
		TestCapture.Step("before-collapse-cancelled");
		search.IsLoading.Should().BeFalse(
			"cancellation must stop the background task synchronously from the collapse handler");
		search.Children.Should().BeEmpty(
			"cancelling must wipe any partial results so the next expand sees a clean state");
		search.LazyLoading.Should().BeTrue(
			"the node re-arms LazyLoading so the next expand re-triggers LoadChildren");
	}
}
