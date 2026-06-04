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

using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.Threading;

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;

using ILSpy.Analyzers;
using ILSpy.AppEnv;
using ILSpy.Docking;
using ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Analyzers;

[TestFixture]
public class AnalyzerTreeKeyboardTests
{
	[AvaloniaTest]
	public async Task Right_Key_Expands_A_Node_In_The_Analyzer_Tree()
	{
		// The analyzer tree shares the same TreeKeyboardController as the assembly tree, so the
		// standard gestures work there too -- here, Right expands the focused node.
		var (window, vm) = await TestHarness.BootAsync(3);
		var dockWorkspace = AppComposition.Current.GetExport<DockWorkspace>();
		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		var entity = (ITypeDefinition)typeNode.Member!;
		var analyzed = analyzerVm.Analyze(entity); // adds the node and selects it

		dockWorkspace.ShowToolPane(AnalyzerTreeViewModel.PaneContentId);
		var view = await window.WaitForComponent<global::ILSpy.Analyzers.AnalyzerTreeView>();
		var grid = await view.WaitForComponent<DataGrid>();
		grid.Focus();
		Dispatcher.UIThread.RunJobs();

		analyzed.IsExpanded.Should().BeFalse("precondition: the analyzed node starts collapsed");

		window.KeyPress(Key.Right, RawInputModifiers.None, PhysicalKey.ArrowRight, null);
		await Waiters.WaitForAsync(() => analyzed.IsExpanded,
			description: "Right must expand the node via the shared TreeKeyboardController on the analyzer tree");
	}
}
