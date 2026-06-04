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
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using Dock.Model.Core;

using ILSpy.Analyzers;
using ILSpy.AppEnv;
using ILSpy.Docking;

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

		AllDockables(dockWorkspace.Layout).Should().Contain(analyzer,
			"baseline: the analyzer pane is in the default layout");

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
}
