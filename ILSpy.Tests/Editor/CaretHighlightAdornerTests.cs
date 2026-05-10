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

using System;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ILSpy.AppEnv;
using ILSpy.AssemblyTree;
using ILSpy.TextView;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class CaretHighlightAdornerTests
{
	[AvaloniaTest]
	public async Task DisplayCaretHighlightAnimation_Adds_Then_Removes_Renderer_From_TextView()
	{
		// Activating the caret-highlight animation must register an IBackgroundRenderer on the
		// editor's TextView and remove it when the lifetime timer expires (one second). Verifies
		// the lifecycle without asserting visual pixels — the animation curve is hand-tuned and
		// best validated by eye, but the registration / cleanup contract is what regressions are
		// likely to break.

		// Arrange — boot, select a method so the decompiler view materialises.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var method = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");
		vm.AssemblyTreeModel.SelectNode(method);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		var view = await window.WaitForComponent<DecompilerTextView>();
		var editor = await view.WaitForComponent<AvaloniaEdit.TextEditor>();
		var textArea = editor.TextArea;
		var renderers = textArea.TextView.BackgroundRenderers;

		// Sanity check — no caret highlight adorner is in flight before we trigger one.
		renderers.Should().NotContain(r => r is CaretHighlightAdorner,
			"the test starts before any caret highlight is triggered");

		// Act — trigger the highlight; assert it lands in the renderer list immediately.
		CaretHighlightAdorner.DisplayCaretHighlightAnimation(textArea);
		renderers.Should().Contain(r => r is CaretHighlightAdorner,
			"DisplayCaretHighlightAnimation must register the adorner synchronously");

		// Wait for the lifetime timer (~1 s) to remove the adorner, with margin.
		await Waiters.WaitForAsync(
			() => !renderers.Any(r => r is CaretHighlightAdorner),
			timeout: TimeSpan.FromSeconds(3));
	}
}
