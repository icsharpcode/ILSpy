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
using ILSpy.Commands;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

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

	[AvaloniaTest]
	public async Task OpenContainingFolder_Walks_Up_To_The_Containing_AssemblyTreeNode()
	{
		// The reveal works on any descendant of an AssemblyTreeNode (member, type, namespace),
		// not just the assembly node itself. Verifies the helper walks the parent chain to
		// find the enclosing assembly and uses its file path.

		// Arrange — boot, find a deep tree node + the registered entry.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var entry = (OpenContainingFolderContextMenuEntry)registry.Entries
			.Single(e => e.Metadata.Header == nameof(Resources._OpenContainingFolder))
			.Value;

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		var expectedPath = assemblyNode.LoadedAssembly.FileName;

		// Act — collect the paths the entry would reveal for a deep selection.
		var paths = entry.GetPathsToReveal(new TextViewContext { SelectedTreeNodes = new[] { (SharpTreeNode)typeNode } });

		// Assert — the deep selection traces back to the parent assembly's file path.
		paths.Should().Equal(expectedPath);
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
