// Copyright (c) 2026 Christoph Wille
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


using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Docking;

/// <summary>
/// Dock's ActiveDockable setter re-runs its activation (ending in SetFocusedDockable) even when
/// the value does not change, so a plain SetActiveDockable of the already-active document would
/// move the active-pane highlight to the documents dock. The factory guards that centrally, so
/// every caller may re-activate without checking first.
/// </summary>
[TestFixture]
public class SetActiveDockableTests
{
	[AvaloniaTest]
	public async Task Reactivating_The_Active_Document_Keeps_The_Focused_Dockable()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.AssemblyTreeModel.SelectNode(typeNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		var docs = vm.DockWorkspace.Documents!;
		var activeDocument = docs.ActiveDockable;
		activeDocument.Should().NotBeNull("decompiling a type must leave an active document tab");

		vm.DockWorkspace.ShowToolPane(AssemblyTreeModel.PaneContentId);
		var focusedPane = vm.DockWorkspace.Layout.FocusedDockable;
		focusedPane.Should().NotBeNull("showing the assembly pane must make it the focused dockable");
		focusedPane.Should().NotBeSameAs(activeDocument, "precondition: the focus must sit outside the documents dock");

		vm.DockWorkspace.Factory.SetActiveDockable(activeDocument!);

		docs.ActiveDockable.Should().BeSameAs(activeDocument, "the active document stays active");
		vm.DockWorkspace.Layout.FocusedDockable.Should().BeSameAs(focusedPane,
			"re-activating the already-active document must not move the focused dockable to it");
	}
}
