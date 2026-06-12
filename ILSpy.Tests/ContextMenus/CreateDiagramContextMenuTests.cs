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
public class CreateDiagramContextMenuTests
{
	[AvaloniaTest]
	public async Task Entry_Is_Visible_For_A_Single_Loaded_Assembly()
	{
		// The "Create Diagram" context-menu entry must surface when the user right-clicks
		// exactly one assembly tree node whose underlying file loaded as a valid assembly.
		// Multi-select, child-node selection, and failed-load nodes should all hide it —
		// the diagrammer needs a single concrete assembly path to feed to GenerateHtmlDiagrammer.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);
		TestCapture.Step("booted");

		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>().Entries
			.Select(e => e.Value).OfType<CreateDiagramContextMenuEntry>().Single();

		var assemblyNode = vm.AssemblyTreeModel.Root!.Children.OfType<AssemblyTreeNode>().First();
		entry.IsVisible(new TextViewContext { SelectedTreeNodes = new SharpTreeNode[] { assemblyNode } }).Should().BeTrue();
	}

	[AvaloniaTest]
	public async Task Entry_Is_Hidden_For_Non_Assembly_Selections()
	{
		// Anything that isn't an AssemblyTreeNode (e.g. a TypeTreeNode under it) doesn't
		// expose the entry — the diagrammer operates on whole-assembly granularity.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);
		TestCapture.Step("booted");

		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>().Entries
			.Select(e => e.Value).OfType<CreateDiagramContextMenuEntry>().Single();

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(coreLibName, "System", "System.Object");
		entry.IsVisible(new TextViewContext { SelectedTreeNodes = new SharpTreeNode[] { typeNode } }).Should().BeFalse();
	}

	[AvaloniaTest]
	public async Task Entry_Is_Hidden_For_Multi_Select()
	{
		// Multi-select must also hide the entry — the diagrammer runs on one assembly at
		// a time. (The WPF version uses the same `Length == 1` guard.)
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);
		TestCapture.Step("booted");

		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>().Entries
			.Select(e => e.Value).OfType<CreateDiagramContextMenuEntry>().Single();

		var assemblyNode = vm.AssemblyTreeModel.Root!.Children.OfType<AssemblyTreeNode>().First();
		entry.IsVisible(new TextViewContext { SelectedTreeNodes = new SharpTreeNode[] { assemblyNode, assemblyNode } }).Should().BeFalse();
	}
}
