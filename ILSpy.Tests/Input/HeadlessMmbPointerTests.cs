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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.DataGridHierarchical;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ILSpy.AppEnv;
using ILSpy.AssemblyTree;
using ILSpy.Docking;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Input;

/// <summary>
/// Drives a real <see cref="HeadlessWindowExtensions.MouseDown"/> event through the
/// input pipeline so the MMB → "open in new tab" gesture is verified end-to-end
/// rather than via a synthetic <c>OpenNodeInNewTab</c> call. Catches routing regressions
/// (the existing synthetic tests would pass even if the PointerPressed handler stopped
/// being installed).
/// </summary>
[TestFixture]
public class HeadlessMmbPointerTests
{
	[AvaloniaTest]
	public async Task Middle_Click_On_An_Assembly_Tree_Row_Opens_A_New_Decompiler_Tab()
	{
		// Boot + load enough assemblies that we have visible rows.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<DataGrid>();

		// Any realised DataGridRow whose DataContext wraps an ILSpyTreeNode is a valid click
		// target — the exact node doesn't matter, only that the gesture pipeline fires.
		// ProDataGrid wraps each row's data in a HierarchicalNode (`{ Item: ILSpyTreeNode }`),
		// so we walk that surface rather than comparing against the tree node directly.
		await Waiters.WaitForAsync(() => grid.GetVisualDescendants().OfType<DataGridRow>()
			.Any(r => r.DataContext is HierarchicalNode { Item: ILSpyTreeNode }));
		var row = grid.GetVisualDescendants().OfType<DataGridRow>()
			.First(r => r.DataContext is HierarchicalNode { Item: ILSpyTreeNode });

		// Snapshot the tab count BEFORE the gesture so we can assert a strict +1 after.
		var documents = ((ILSpyDockFactory)vm.DockWorkspace.Factory).Documents!;
		int before = documents.VisibleDockables?.Count ?? 0;

		// Translate the row's centre into TopLevel coordinates so MouseDown lands inside it.
		var topLevel = TopLevel.GetTopLevel(row)!;
		var rowBounds = row.Bounds;
		var centreInRow = new Point(rowBounds.Width / 2, rowBounds.Height / 2);
		var centreInTopLevel = row.TranslatePoint(centreInRow, topLevel)
			?? throw new System.InvalidOperationException("Row not in TopLevel visual tree");

		// Real pointer event — exercises bubble + handledEventsToo routing through ProDataGrid.
		topLevel.MouseDown(centreInTopLevel, MouseButton.Middle);
		topLevel.MouseUp(centreInTopLevel, MouseButton.Middle);

		// Wait for the tab to land. Without WaitForAsync this races the async decompile path.
		await Waiters.WaitForAsync(() => (documents.VisibleDockables?.Count ?? 0) > before);

		(documents.VisibleDockables?.Count ?? 0).Should().Be(before + 1);
	}
}
