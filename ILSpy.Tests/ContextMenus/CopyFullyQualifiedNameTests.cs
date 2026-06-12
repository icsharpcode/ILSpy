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

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class CopyFullyQualifiedNameTests
{
	[AvaloniaTest]
	public async Task Copy_Entry_Is_Registered()
	{
		// "Copy fully qualified name" appears as an [ExportContextMenuEntry] tagged with the
		// localized "CopyName" header. Verifies it's discovered through MEF.

		// Arrange + Act — boot.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();

		// Assert — registry contains an entry with the CopyName resource header.
		registry.Entries.Should().Contain(
			e => e.Metadata.Header == nameof(Resources.CopyName));
	}

	[AvaloniaTest]
	public async Task Copy_Entry_Is_Visible_Only_For_Single_Member_Tree_Node()
	{
		// IsVisible: exactly one selected node, and it must be an IMemberTreeNode (the
		// interface implemented by Type/Method/Field/Property/Event tree nodes that exposes
		// an IEntity). Hidden for assembly nodes, multi-selections, and empty selections.

		// Arrange — boot, locate the registered entry + a sample type and assembly node.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
		TestCapture.Step("booted");
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var entry = registry.Entries
			.Single(e => e.Metadata.Header == nameof(Resources.CopyName))
			.Value;

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");

		// Assert — visible only for a single member-tree-node selection.
		entry.IsVisible(new TextViewContext { SelectedTreeNodes = new[] { (SharpTreeNode)typeNode } })
			.Should().BeTrue();
		entry.IsVisible(new TextViewContext { SelectedTreeNodes = new SharpTreeNode[] { typeNode, assemblyNode } })
			.Should().BeFalse("multi-selection isn't supported");
		entry.IsVisible(new TextViewContext { SelectedTreeNodes = new[] { (SharpTreeNode)assemblyNode } })
			.Should().BeFalse("assembly nodes are not IMemberTreeNodes");
		entry.IsVisible(new TextViewContext { SelectedTreeNodes = null })
			.Should().BeFalse();
	}

	[AvaloniaTest]
	public async Task Copy_Entry_Computes_The_Reflection_Name_Of_The_Selected_Member()
	{
		// CopyName's payload is the IEntity.ReflectionName of the selected member — the same
		// stable language-independent identifier used by FindNodeByPath et al. Tested by
		// reading the public computed property; the actual clipboard write is a thin wrapper
		// around that text.

		// Arrange — boot, locate the type node + the registered entry's typed wrapper.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
		TestCapture.Step("booted");
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var entry = (CopyFullyQualifiedNameContextMenuEntry)registry.Entries
			.Single(e => e.Metadata.Header == nameof(Resources.CopyName))
			.Value;

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");

		// Act — compute the would-be-copied text for that selection.
		var text = entry.GetTextToCopy(new TextViewContext { SelectedTreeNodes = new[] { (SharpTreeNode)typeNode } });

		// Assert — matches the expected ReflectionName.
		text.Should().Be("System.Linq.Enumerable");
	}
}
