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

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX.TreeView;

using ILSpy;
using ILSpy.AppEnv;
using ILSpy.Docking;
using ILSpy.TextView;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class DecompileInNewViewTests
{
	[AvaloniaTest]
	public async Task DecompileInNewView_Entry_Is_Registered()
	{
		// "Decompile in new tab" is a context-menu entry sitting alongside Remove on every
		// node that decompiles. Verifies the [ExportContextMenuEntry] is discovered.

		// Arrange — boot.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();

		// Assert — registry contains an entry whose header is the DecompileToNewPanel resource.
		registry.Entries.Should().Contain(
			e => e.Metadata.Header == nameof(Resources.DecompileToNewPanel));
	}

	[AvaloniaTest]
	public async Task DecompileInNewView_Is_Visible_When_Selection_Has_ILSpyTreeNodes()
	{
		// IsVisible: any non-empty selection of ILSpyTreeNodes makes the entry surface — the
		// action runs against whatever the user picked.

		// Arrange — boot, locate the registered entry.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		// wait for assemblies to load
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var entry = registry.Entries
			.Single(e => e.Metadata.Header == nameof(Resources.DecompileToNewPanel))
			.Value;

		var asm = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");

		// Assert — visible with a tree node, hidden with no selection.
		entry.IsVisible(new TextViewContext { SelectedTreeNodes = new[] { (SharpTreeNode)asm } })
			.Should().BeTrue();
		entry.IsVisible(new TextViewContext { SelectedTreeNodes = null })
			.Should().BeFalse();
		entry.IsVisible(new TextViewContext { SelectedTreeNodes = System.Array.Empty<SharpTreeNode>() })
			.Should().BeFalse();
	}

	[AvaloniaTest]
	public async Task DecompileInNewView_Execute_Opens_A_New_Tab_With_The_Selected_Nodes_Decompiled()
	{
		// Execute opens a fresh document tab in the dock workspace, populated with the
		// decompilation of the supplied nodes — without disturbing the currently-active tab.

		// Arrange — boot, decompile a method into the existing tab so we have a "current"
		// document to compare against.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		// wait for assemblies to load
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		// expand typeNode
		typeNode.IsExpanded = true;
		var firstMethod = typeNode.Children.OfType<MethodTreeNode>()
			.Single(m => m.MethodDefinition.Name == "AsEnumerable");
		var secondMethod = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");
		// select firstMethod
		vm.AssemblyTreeModel.SelectNode(firstMethod);
		// wait for decompile to finish
		var firstTab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var entry = registry.Entries
			.Single(e => e.Metadata.Header == nameof(Resources.DecompileToNewPanel))
			.Value;

		var documents = ((ILSpyDockFactory)vm.DockWorkspace.Factory).Documents!;
		var initialCount = documents.VisibleDockables?.Count ?? 0;

		// Act — invoke Execute with the second method as the selection.
		entry.Execute(new TextViewContext {
			SelectedTreeNodes = new[] { (SharpTreeNode)secondMethod },
		});

		// Assert — a new tab landed and now decompiles "Empty"; the original tab still
		// shows AsEnumerable.
		await Waiters.WaitForAsync(
			() => (documents.VisibleDockables?.Count ?? 0) > initialCount);
		// wait for decompile to finish
		var newTab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		ReferenceEquals(newTab, firstTab).Should().BeFalse(
			"a fresh decompiler tab must be created instead of reusing the existing one");
		newTab.Text.Should().Contain("Empty");
		firstTab.Text.Should().Contain("AsEnumerable");
	}
}
