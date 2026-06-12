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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using Dock.Model.Core;

using ICSharpCode.ILSpy.Analyzers;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Docking;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Analyzers;

[TestFixture]
public class AnalyzerPaneVisibilityTests
{
	static IEnumerable<IDockable> AllDockables(IDockable? root)
	{
		if (root is null)
			yield break;
		yield return root;
		if (root is IDock dock && dock.VisibleDockables is { } kids)
			foreach (var k in kids)
				foreach (var d in AllDockables(k))
					yield return d;
	}

	[AvaloniaTest]
	public async Task Analyze_Reopens_The_Analyzer_Pane_After_The_User_Closed_It()
	{
		// Repro for "Analyze does not make the Analyzer panel visible": the user closes the
		// bottom Analyzer panel, then Analyze must bring it back. ShowToolPane only activated
		// panes still in the layout, so a closed pane (removed from VisibleDockables) could
		// never be restored -- Analyze became a silent no-op.
		var (_, vm) = await TestHarness.BootAsync(3);
		var dockWorkspace = AppComposition.Current.GetExport<DockWorkspace>();
		var analyzer = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();
		var factory = (ILSpyDockFactory)dockWorkspace.Factory;

		// The analyzer pane is hidden by default; show it so the user has it open.
		dockWorkspace.ShowToolPane(AnalyzerTreeViewModel.PaneContentId);
		AllDockables(dockWorkspace.Layout).Should().Contain(analyzer,
			"showing the analyzer pane adds it to the layout");

		// The user closes the bottom Analyzer panel (its close button calls CloseDockable).
		factory.CloseDockable(analyzer);
		AllDockables(dockWorkspace.Layout).Should().NotContain(analyzer,
			"closing removes the analyzer pane from the layout");

		// Analyze -> ShowToolPane must restore the closed pane and activate it.
		dockWorkspace.ShowToolPane(AnalyzerTreeViewModel.PaneContentId);

		AllDockables(dockWorkspace.Layout).Should().Contain(analyzer,
			"Analyze must restore the closed analyzer pane so the user sees the entity they added");
		(analyzer.Owner as IDock)?.ActiveDockable.Should().BeSameAs(analyzer,
			"the restored pane must be the active dockable in its dock");
	}

	[AvaloniaTest]
	public async Task Default_Layout_Shows_Only_The_Assembly_Tree_Pane()
	{
		// The default dock config shows just the assembly-tree pane: Search, Analyzer and Debug
		// Steps are hidden until invoked (IsVisibleByDefault = false), while keeping their home
		// locations for when they are first opened.
		var (_, vm) = await TestHarness.BootAsync(3);
		var dockWorkspace = AppComposition.Current.GetExport<DockWorkspace>();
		var analyzer = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();
		var search = AppComposition.Current.GetExport<ICSharpCode.ILSpy.Search.SearchPaneModel>();

		var paneIds = AllDockables(dockWorkspace.Layout)
			.OfType<ICSharpCode.ILSpy.ViewModels.ToolPaneModel>()
			.Select(p => p.Id)
			.ToList();
		paneIds.Should().Contain("AssemblyTree", "the assembly-tree pane is visible by default");
		paneIds.Should().NotContain("Search", "Search is hidden until invoked");
		paneIds.Should().NotContain("Analyzer", "Analyzer is hidden until invoked");

		// ...but showing one materialises it into its home location.
		dockWorkspace.ShowToolPane(ICSharpCode.ILSpy.Search.SearchPaneModel.PaneContentId);
		AllDockables(dockWorkspace.Layout).Should().Contain(search,
			"Show Search must materialise the hidden Search pane");
		(search.Owner as IDock)!.Id.Should().Be("TopTools",
			"Search keeps its top-edge home dock");
	}
}
