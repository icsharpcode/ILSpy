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

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Docking;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Docking;

// DockWorkspace is the MEF-shared wrapper that holds the Dock.Avalonia factory + the root
// layout. If MEF can't construct it (e.g., missing AssemblyTreeModel / SearchPaneModel /
// AnalyzerTreeViewModel imports), the entire dock surface never materializes. Asserting
// at the export layer catches that before we wire it into the DockControl.
[TestFixture]
public class DockWorkspaceTests
{
	[AvaloniaTest]
	public void DockWorkspace_resolves_and_exposes_root_layout_with_three_tool_panes()
	{
		var workspace = AppComposition.Current.GetExport<DockWorkspace>();
		workspace.Should().NotBeNull("DockWorkspace is [Export][Shared] in ICSharpCode.ILSpy.Docking.");

		workspace.Layout.Should().NotBeNull("ILSpyDockFactory.CreateLayout() wires the root dock in the ctor.");
		workspace.Factory.Should().NotBeNull();
#if DEBUG
		workspace.ToolPaneMenuItems.Should().HaveCount(4,
			"AssemblyTree, Search, Analyzers, and the Debug Steps pane (Debug-only) are wired at this point.");
#else
		workspace.ToolPaneMenuItems.Should().HaveCount(3,
			"AssemblyTree, Search, and Analyzers are the three tool panes wired at this point.");
#endif
	}
}
