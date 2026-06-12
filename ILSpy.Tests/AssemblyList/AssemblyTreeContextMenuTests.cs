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

using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class AssemblyTreeContextMenuTests
{
	[AvaloniaTest]
	public async Task ContextMenu_Is_Attached_To_The_Assembly_Tree_Grid()
	{
		// AssemblyListPane wires a context-menu host onto its DataGrid at construction time so
		// any [ExportContextMenuEntry]-tagged commands can drop into the right-click menu
		// without further per-entry plumbing.

		// Arrange + Act — boot the window so the pane materialises.
		var (window, vm) = await TestHarness.BootAsync();

		// Assert — TreeGrid carries a ContextMenu. (The menu may be empty if no entries are
		// registered yet; verifies only that the host is in place.)
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();
		grid.ContextMenu.Should().NotBeNull();
	}

	[AvaloniaTest]
	public async Task Context_Menu_Shows_Visible_Entries_With_Selected_Tree_Nodes_In_Context()
	{
		// When the user right-clicks a tree row, the pane builds a TextViewContext from the
		// current selection and feeds it through ContextMenuProvider.Build with whichever
		// entries the test injects. Visible entries land in the menu; the entry receives the
		// originating tab grid + the selected nodes when invoked.

		// Arrange — boot, select an assembly node, install a stub entry that records the
		// context it sees on Execute.
		var (window, vm) = await TestHarness.BootAsync();
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		vm.AssemblyTreeModel.SelectNode(assemblyNode);
		TestCapture.Step("system-linq-selected");

		TextViewContext? executionContext = null;
		var entry = new RecordingEntry(c => executionContext = c);
		var export = new StubExport(entry, new ContextMenuEntryMetadata { Header = "Probe", Order = 0 });

		// Act 1 — re-attach the context menu with our stub entries.
		pane.AttachContextMenu(new IContextMenuEntryExport[] { export });
		var grid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();
		var menu = grid.ContextMenu!;

		// Trigger the build path the live Opening event would take.
		var built = pane.BuildContextMenuForCurrentState(new IContextMenuEntryExport[] { export });
		TestCapture.Step("context-menu-built");

		// Assert 1 — menu carries our entry.
		built.Should().NotBeNull();
		var item = built!.Items.OfType<MenuItem>().Single();
		((string?)item.Header).Should().Be("Probe");

		// Act 2 — invoke the click handler that the Build attached.
		item.RaiseEvent(new global::Avalonia.Interactivity.RoutedEventArgs(MenuItem.ClickEvent));
		TestCapture.Step("probe-entry-clicked");

		// Assert 2 — entry's Execute saw the right context: the tree grid + the selection.
		executionContext.Should().NotBeNull();
		ReferenceEquals(executionContext!.TreeGrid, grid).Should().BeTrue();
		executionContext.SelectedTreeNodes.Should().NotBeNull();
		executionContext.SelectedTreeNodes!.Should().Contain((SharpTreeNode)assemblyNode);
	}

	sealed class RecordingEntry(System.Action<TextViewContext> onExecute) : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context) => true;
		public bool IsEnabled(TextViewContext context) => true;
		public void Execute(TextViewContext context) => onExecute(context);
	}

	sealed class StubExport(IContextMenuEntry entry, ContextMenuEntryMetadata metadata) : IContextMenuEntryExport
	{
		public IContextMenuEntry Value { get; } = entry;
		public ContextMenuEntryMetadata Metadata { get; } = metadata;
	}
}
