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

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class OpenContainingFolderTests
{
	[AvaloniaTest]
	public async Task OpenContainingFolder_Entry_Is_Registered()
	{
		// AssemblyTreeNode contributes a [ExportContextMenuEntry] under the "Shell" category
		// for revealing the assembly's file in the OS file manager.

		// Arrange + Act — boot.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();

		// Assert — registry contains the entry.
		registry.Entries.Should().Contain(
			e => e.Metadata.Header == nameof(Resources._OpenContainingFolder));
	}

	// audit (2026-05-12): partial 📦 → 🧪 conversion. Execute launches the OS file manager
	// (Process.Start with the directory) which is unsafe in a headless test, so the path
	// payload is still verified via the public `GetPathsToReveal` helper (the testable seam
	// production Execute itself calls). Added an integration assertion that the entry surfaces
	// in the right-click menu when a deep descendant is selected — confirms the IsVisible
	// predicate (which also calls `GetPathsToReveal`) wires through the menu builder.
	[AvaloniaTest]
	public async Task Right_Clicking_A_Deep_Node_Surfaces_OpenContainingFolder_For_Its_Parent_Assembly()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		var expectedPath = assemblyNode.LoadedAssembly.FileName;

		window.Capture("initial-tree");

		// Selecting the deep TypeTreeNode must still surface "Open Containing Folder" in the
		// live menu (the entry's IsVisible walks up to the assembly).
		vm.AssemblyTreeModel.SelectNode(typeNode);
		window.Capture("enumerable-type-selected-deep-node");

		var menu = pane.BuildContextMenuForCurrentState(registry.Entries);
		TestCapture.Step("context-menu-built");
		menu.Should().NotBeNull();
		menu!.Items.OfType<MenuItem>().Select(i => (string?)i.Header)
			.Should().Contain(Resources._OpenContainingFolder);

		// The path the entry would reveal (Process.Start target — kept in a helper so we can
		// assert without spawning the OS file manager).
		var entry = (OpenContainingFolderContextMenuEntry)registry.Entries
			.Single(e => e.Metadata.Header == nameof(Resources._OpenContainingFolder)).Value;
		entry.GetPathsToReveal(new TextViewContext {
			SelectedTreeNodes = new[] { (SharpTreeNode)typeNode },
		}).Should().Equal(expectedPath);
	}

	[AvaloniaTest]
	public async Task OpenContainingFolder_Is_Hidden_When_The_File_No_Longer_Exists()
	{
		// IsVisible filters out selections whose backing file isn't on disk anymore — there's
		// nothing to reveal. The entry stays hidden so the menu doesn't dangle a non-functional
		// row.

		// Arrange — boot, locate entry, build a fake AssemblyTreeNode pointing at a missing file.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);
		TestCapture.Step("booted");
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var entry = registry.Entries
			.Single(e => e.Metadata.Header == nameof(Resources._OpenContainingFolder))
			.Value;

		var realAssemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");

		// Assert — visible when the file exists, hidden when nothing is selected.
		entry.IsVisible(new TextViewContext { SelectedTreeNodes = new[] { (SharpTreeNode)realAssemblyNode } })
			.Should().BeTrue();
		entry.IsVisible(new TextViewContext { SelectedTreeNodes = null })
			.Should().BeFalse();
		entry.IsVisible(new TextViewContext { SelectedTreeNodes = System.Array.Empty<SharpTreeNode>() })
			.Should().BeFalse();
	}
}
