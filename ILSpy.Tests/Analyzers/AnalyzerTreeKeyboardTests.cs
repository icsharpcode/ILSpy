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
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.Threading;

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;

using ICSharpCode.ILSpy.Analyzers;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Analyzers;

[TestFixture]
public class AnalyzerTreeKeyboardTests
{
	[AvaloniaTest]
	public async Task Right_Key_Expands_A_Node_In_The_Analyzer_Tree()
	{
		// The analyzer tree is a SharpTreeView like the assembly tree, so the standard tree gestures
		// work there too via SharpTreeView.OnKeyDown -- here, Right expands the focused node.
		var (window, vm) = await TestHarness.BootAsync(3);
		var dockWorkspace = AppComposition.Current.GetExport<DockWorkspace>();
		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		var entity = (ITypeDefinition)typeNode.Member!;
		var analyzed = analyzerVm.Analyze(entity); // adds the node, auto-expands it, and selects it
												   // Analyze auto-expands the new node; collapse it again so this test can exercise Right-to-expand.
		analyzed.IsExpanded = false;

		dockWorkspace.ShowToolPane(AnalyzerTreeViewModel.PaneContentId);
		var view = await window.WaitForComponent<ICSharpCode.ILSpy.Analyzers.AnalyzerTreeView>();
		var tree = await view.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();
		tree.Focus();
		Dispatcher.UIThread.RunJobs();

		analyzed.IsExpanded.Should().BeFalse("precondition: the node was collapsed for this test");

		window.KeyPress(Key.Right, RawInputModifiers.None, PhysicalKey.ArrowRight, null);
		await Waiters.WaitForAsync(() => analyzed.IsExpanded,
			description: "Right must expand the node via SharpTreeView.OnKeyDown on the analyzer tree");
	}

	[AvaloniaTest]
	public async Task Ctrl_R_Analyzes_The_Selected_Member()
	{
		// Ctrl+R on the assembly tree analyzes the selected member(s) -- the keyboard equivalent of the
		// Analyze context-menu entry. Here a selected type lands as a node in the analyzer pane.
		var (window, vm) = await TestHarness.BootAsync(3);
		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();
		var pane = await window.WaitForComponent<ICSharpCode.ILSpy.AssemblyTree.AssemblyListPane>();
		var tree = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.AssemblyTreeModel.SelectNode(typeNode);
		for (int i = 0; i < 6; i++)
		{
			Dispatcher.UIThread.RunJobs();
			tree.UpdateLayout();
			await Task.Delay(20);
		}
		tree.Focus();
		Dispatcher.UIThread.RunJobs();

		int before = analyzerVm.Root.Children.Count;
		window.KeyPress(Key.R, RawInputModifiers.Control, PhysicalKey.R, null);
		await Waiters.WaitForAsync(() => analyzerVm.Root.Children.Count > before,
			description: "Ctrl+R must add an analyzer node for the selected member");

		analyzerVm.Root.Children.OfType<AnalyzerEntityTreeNode>()
			.Any(n => n.Member is { } m && m.MetadataToken == ((ITypeDefinition)typeNode.Member!).MetadataToken)
			.Should().BeTrue("the analyzer pane must hold a node for the type that Ctrl+R analyzed");
	}
}
