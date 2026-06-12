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
using Avalonia.Interactivity;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class ReloadAssemblyContextMenuTests
{
	[AvaloniaTest]
	public async Task Reload_Entry_Is_Registered()
	{
		// AssemblyTreeNode contributes a [ExportContextMenuEntry] for "_Reload" so the user
		// can re-read an assembly from disk after rebuilding it elsewhere. Verifies the entry
		// shows up in the live MEF registry.

		// Arrange + Act — boot so composition is initialised.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();

		// Assert — registry contains an entry whose header is the _Reload resource.
		registry.Entries.Should().Contain(
			e => e.Metadata.Header == nameof(Resources._Reload),
			"AssemblyTreeNode must export a Reload context-menu entry");
	}

	[AvaloniaTest]
	public async Task Reload_Entry_Is_Visible_Only_For_AssemblyTreeNode_Selections()
	{
		// IsVisible: same shape as Remove — only when every selected node is an
		// AssemblyTreeNode. Hidden on member/namespace/resource selections so it doesn't
		// pollute their right-click menus.

		// Arrange — boot, locate the registered entry.
		var (_, vm) = await TestHarness.BootAsync(3);
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var reloadEntry = registry.GetEntry(nameof(Resources._Reload));

		var asm = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");

		// Assert — visible for assembly-only selections, hidden otherwise.
		reloadEntry.IsVisible(new TextViewContext {
			SelectedTreeNodes = new[] { (SharpTreeNode)asm }
		}).Should().BeTrue();

		reloadEntry.IsVisible(new TextViewContext {
			SelectedTreeNodes = new[] { (SharpTreeNode)asm, typeNode }
		}).Should().BeFalse();

		reloadEntry.IsVisible(new TextViewContext { SelectedTreeNodes = null })
			.Should().BeFalse();
	}

	// audit (2026-05-12): converted 📦 → 🧪. Drives the right-click → Reload click chain by
	// building the live context menu and firing the MenuItem's Click event, instead of
	// calling `reloadEntry.Execute(synthetic ctx)` directly. Catches regressions in the
	// menu-build → click-handler pipeline that the old direct-call test would have missed.
	[AvaloniaTest]
	public async Task Right_Click_Then_Reload_Replaces_The_LoadedAssembly()
	{
		// Reload re-creates the LoadedAssembly object (forcing the metadata cache to refresh),
		// so after Execute the AssemblyList contains a *different* LoadedAssembly instance for
		// the same file name.

		// Arrange — boot, snapshot the LoadedAssembly we'll reload.
		var (window, vm) = await TestHarness.BootAsync(3);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();

		var node = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		var originalAssembly = node.LoadedAssembly;
		var fileName = originalAssembly.FileName;

		window.Capture("initial-tree");

		// Act — select the row, build the live context menu, click Reload.
		vm.AssemblyTreeModel.SelectNode(node);
		window.Capture("system-linq-selected");

		var menu = pane.BuildContextMenuForCurrentState(registry.Entries);
		menu.Should().NotBeNull();
		menu!.ClickItem(Resources._Reload);
		window.Capture("reload-clicked");

		// Assert — the LoadedAssembly for that file name is a different instance now.
		await Waiters.WaitForAsync(() =>
			vm.AssemblyTreeModel.AssemblyList!.GetAssemblies().Any(a =>
				a.FileName == fileName && !ReferenceEquals(a, originalAssembly)));

		window.Capture("after-reload-same-file-fresh-instance");
	}
}
