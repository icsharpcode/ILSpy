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
public class AnalyzerTreeNodeTests
{
	[AvaloniaTest]
	public async Task Analyzers_Property_Surfaces_All_MEF_Registered_Analyzers_Sorted_By_Order()
	{
		// AnalyzerTreeNode.Analyzers exposes the MEF-discovered IAnalyzer exports — the
		// list every Analyzed*TreeNode walks when populating its child analyzer headers.
		// Two contracts: (a) every [ExportAnalyzer] in ICSharpCode.ILSpyX.Analyzers.Builtin
		// reaches the list, (b) the order is monotonically non-decreasing by Metadata.Order
		// so users see headers in the same sequence the WPF host produces.

		await TestHarness.BootAsync();

		var analyzers = AnalyzerTreeNode.Analyzers.ToArray();
		analyzers.Should().HaveCountGreaterThanOrEqualTo(15,
			"the shared analyzer library defines 17 built-in analyzers — all reachable through MEF");

		var orders = analyzers.Select(a => a.Metadata?.Order ?? 0).ToArray();
		orders.Should().BeInAscendingOrder(
			"AnalyzerTreeNode.Analyzers must hand back exports sorted by metadata Order");

		var headers = analyzers.Select(a => a.Metadata?.Header).ToArray();
		headers.Should().Contain("Used By",
			"TypeUsedByAnalyzer / MethodUsedByAnalyzer is the canonical first analyzer");
	}

	[AvaloniaTest]
	public async Task AnalyzerRootNode_Clears_Children_When_The_Active_AssemblyList_Resets()
	{
		// AnalyzerRootNode is the pane's tree root. When the active AssemblyList raises
		// a Reset CollectionChanged event (typically when a different list is loaded
		// from the dropdown), the analyzer pane's results are stale by definition —
		// every Analyzed*TreeNode's symbol references a module the new list may not
		// contain. The root must wipe its children rather than risk a stale navigation.

		var (_, vm) = await TestHarness.BootAsync();

		var root = new AnalyzerRootNode();
		var assemblyNode = vm.AssemblyTreeModel.FindCoreLib();
		var module = assemblyNode.LoadedAssembly.GetMetadataFileOrNull();
		module.Should().NotBeNull("the test needs a loaded CoreLib module to seed a sample analyzed entry");

		var typeSystem = module!.GetTypeSystemOrNull();
		typeSystem.Should().NotBeNull();
		var sampleType = typeSystem!.MainModule.TopLevelTypeDefinitions.First();
		root.Children.Add(new AnalyzedTypeTreeNode(sampleType, source: null));
		root.Children.Should().HaveCount(1);

		// Simulate a Reset by firing the collection-changed pathway the root subscribes to.
		// We do this through the live AssemblyList rather than reaching into private subscribers
		// — the active list firing CollectionChanged with Reset is what triggers the same flow
		// at app-runtime.
		var assemblyList = vm.AssemblyTreeModel.AssemblyList!;
		var snapshot = assemblyList.GetAssemblies();
		// Clearing then re-adding the assemblies emits a Reset on the underlying ObservableCollection.
		assemblyList.Clear();
		TestCapture.Step("assembly-list-cleared");
		root.Children.Should().BeEmpty(
			"AnalyzerRootNode.CurrentAssemblyList_Changed must wipe children on Reset");

		// Restore the assemblies so subsequent tests don't see an empty list.
		foreach (var asm in snapshot)
			assemblyList.OpenAssembly(asm.FileName);
	}
}
