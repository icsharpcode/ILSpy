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

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using Dock.Model.Controls;

using ICSharpCode.ILSpy.Analyzers;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Search;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class ToolPaneRegistryTests
{
	[AvaloniaTest]
	public void Registry_Lists_All_Three_Built_In_Tool_Panes()
	{
		// AssemblyTreeModel, SearchPaneModel, AnalyzerTreeViewModel each carry [ExportToolPane]
		// so plugins can drop additional panes into the registry without modifying the dock
		// factory or DockWorkspace's constructor.

		// Arrange + Act — pull the registry from the composition host.
		var registry = AppComposition.Current.GetExport<ToolPaneRegistry>();

		// Assert — three entries, one per built-in pane type.
		var paneTypes = registry.Panes.Select(p => p.Pane.GetType()).ToArray();
		paneTypes.Should().Contain(typeof(AssemblyTreeModel));
		paneTypes.Should().Contain(typeof(SearchPaneModel));
		paneTypes.Should().Contain(typeof(AnalyzerTreeViewModel));
	}

	[AvaloniaTest]
	public void Registry_Honours_Alignment_Metadata_From_The_Export_Attribute()
	{
		// Each pane's [ExportToolPane(Alignment = …)] declares which dock zone it belongs in.
		// DockFactory groups panes by Alignment when building the layout.

		// Arrange + Act — read alignments off the registry.
		var registry = AppComposition.Current.GetExport<ToolPaneRegistry>();
		var byType = registry.Panes.ToDictionary(p => p.Pane.GetType(), p => p.Metadata);

		// Assert — Assembly tree on the left, Search on top, Analyzer on the bottom.
		byType[typeof(AssemblyTreeModel)].Alignment.Should().Be(ToolPaneAlignment.Left);
		byType[typeof(SearchPaneModel)].Alignment.Should().Be(ToolPaneAlignment.Top);
		byType[typeof(AnalyzerTreeViewModel)].Alignment.Should().Be(ToolPaneAlignment.Bottom);
	}

	[AvaloniaTest]
	public void DockFactory_Builds_Layout_Zones_Driven_By_The_Registry()
	{
		// ILSpyDockFactory iterates the registry instead of taking each pane in its ctor.
		// The default config places only the visible-by-default assembly-tree pane; Search and
		// Analyzer are hidden (IsVisibleByDefault = false) and materialised on demand.

		// Arrange + Act — DockWorkspace is constructed by the composition host; its layout is
		// built eagerly in the constructor.
		var workspace = AppComposition.Current.GetExport<DockWorkspace>();
		var allDockables = FlattenDockables(workspace.Layout).ToArray();

		// Assert — only the assembly-tree pane is placed up front.
		allDockables.OfType<AssemblyTreeModel>().Should().ContainSingle();
		allDockables.OfType<SearchPaneModel>().Should().BeEmpty("Search is hidden until invoked");
		allDockables.OfType<AnalyzerTreeViewModel>().Should().BeEmpty("Analyzer is hidden until invoked");
	}

	static System.Collections.Generic.IEnumerable<Dock.Model.Core.IDockable> FlattenDockables(Dock.Model.Core.IDockable root)
	{
		yield return root;
		if (root is Dock.Model.Core.IDock dock && dock.VisibleDockables != null)
		{
			foreach (var child in dock.VisibleDockables)
				foreach (var nested in FlattenDockables(child))
					yield return nested;
		}
	}
}
