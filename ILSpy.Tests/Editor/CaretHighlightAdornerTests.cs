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

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class CaretHighlightAdornerTests
{
	// audit (2026-05-12):
	//   * catches commenting out `BackgroundRenderers.Add(adorner)` in
	//     CaretHighlightAdorner.DisplayCaretHighlightAnimation (the "registered" assertion).
	//   * catches commenting out the `CaretHighlightAdorner.DisplayCaretHighlightAnimation(...)`
	//     call inside DecompilerTextView.OnReferenceClicked — the integration path the test
	//     drives. A direct unit test on DisplayCaretHighlightAnimation would not have caught
	//     this regression.
	[AvaloniaTest]
	public async Task Clicking_A_Member_Definition_In_The_Decompiled_Output_Triggers_Caret_Highlight()
	{
		// Integration test for the caret-highlight gesture: select a type, wait for the
		// decompile to populate the editor + reference collection, then drive the same
		// `ReferenceElementGenerator.OnReferenceClicked` entry point that a real
		// `VisualLineReferenceText.OnPointerPressed` fires when the user clicks a member
		// name. Member definitions register themselves in `DefinitionLookup` (see
		// AvaloniaEditTextOutput.AddReference), so `OnReferenceClicked` resolves the click
		// to an in-document jump and DecompilerTextView fires the caret highlight.
		//
		// The lifecycle (register on click, then unregister via Dismiss) is verified end-to-end —
		// the hand-tuned animation curve and the one-second timing are left for eyeball validation.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);
		// Decompile a single method instead of the whole Enumerable type — the full type
		// takes >15 s in headless and flakes WaitForDecompiledTextAsync. `Empty<TSource>`
		// is one of the smallest methods and still emits a member-definition reference
		// (the method name) plus type-definition references on its return type.
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var method = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");
		vm.AssemblyTreeModel.SelectNode(method);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		var view = await window.WaitForComponent<DecompilerTextView>();
		var editor = await view.WaitForComponent<AvaloniaEdit.TextEditor>();
		var renderers = editor.TextArea.TextView.BackgroundRenderers;

		// Sanity check — no adorner from a previous gesture is hanging around.
		renderers.Should().NotContain(r => r is CaretHighlightAdorner,
			"the test starts before any caret-highlight gesture is triggered");

		// Locate the live ReferenceElementGenerator and an in-document member-definition
		// segment (the spot where the member's name is written; IsDefinition=true causes
		// DefinitionLookup to register the entity + offset together, so OnReferenceClicked's
		// in-document branch is guaranteed to fire).
		var generator = editor.TextArea.TextView.ElementGenerators
			.OfType<ReferenceElementGenerator>().Single();
		generator.References.Should().NotBeNull(
			"DecompilerTextView assigns the live References collection on every ApplyDocument");
		var memberDef = generator.References!
			.FirstOrDefault(s => s.IsDefinition && !s.IsLocal && s.Reference != null);
		((object?)memberDef).Should().NotBeNull(
			"the decompiled Enumerable class contains at least one member-definition reference");

		// Same code path a real stationary click on the member name routes through.
		view.OnReferenceClicked(memberDef!);

		renderers.Should().Contain(r => r is CaretHighlightAdorner,
			"clicking a member definition must route through DecompilerTextView.OnReferenceClicked "
			+ "and call CaretHighlightAdorner.DisplayCaretHighlightAnimation");

		// The 1 s lifetime is a real-time DispatcherTimer, which is unreliable to wait on in a
		// headless run; invoke the same teardown the timer runs (Dismiss) directly so the
		// unregister half is verified deterministically rather than against the wall clock.
		var adorner = renderers.OfType<CaretHighlightAdorner>().Single();
		adorner.Dismiss();
		renderers.Should().NotContain(r => r is CaretHighlightAdorner,
			"Dismiss unregisters the caret-highlight adorner from the text view");
	}
}
