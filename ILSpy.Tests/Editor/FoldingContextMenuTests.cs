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
using Avalonia.Threading;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class FoldingContextMenuTests
{
	[AvaloniaTest]
	public async Task Toggle_All_Folding_Entry_Collapses_Then_Expands_The_Document()
	{
		// Decompiling a type yields brace foldings; the "Toggle all folding" menu entry must collapse
		// them all, then expand them all on a second invocation -- via DecompilerTextView.ToggleAllFoldings.
		var (window, vm) = await TestHarness.BootAsync(3);

		// A small CoreLib type still yields brace foldings (one per method body) but decompiles fast
		// enough for the headless 15s wait on slow CI runners; a full System.Linq.Enumerable decompile
		// does not.
		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(coreLibName, "System", "System.Object");
		vm.AssemblyTreeModel.SelectNode(typeNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		var view = await window.WaitForComponent<DecompilerTextView>();
		for (int i = 0; i < 8; i++)
		{
			Dispatcher.UIThread.RunJobs();
			await Task.Delay(25);
		}

		view.HasFoldings.Should().BeTrue("decompiling a type produces brace foldings");

		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var toggleAll = registry.GetEntry(nameof(Resources.ToggleFolding));   // "Toggle All Folding"
		var toggleOne = registry.GetEntry(nameof(Resources._ToggleFolding));  // "Toggle Folding"
		var context = new TextViewContext { TextView = view };
		toggleAll.IsVisible(context).Should().BeTrue("the folding entries show when the document has foldings");
		toggleOne.IsVisible(context).Should().BeTrue();
		toggleAll.IsVisible(new TextViewContext { TextView = null }).Should()
			.BeFalse("with no text view there is nothing to fold");

		// Toggle-all forces a uniform state: collapse-all when any fold is open, otherwise expand-all.
		// So consecutive invocations flip between "fully collapsed" (>0) and "fully expanded" (0).
		toggleAll.Execute(context);
		Dispatcher.UIThread.RunJobs();
		int after1 = view.FoldedFoldingCount;

		toggleAll.Execute(context);
		Dispatcher.UIThread.RunJobs();
		int after2 = view.FoldedFoldingCount;

		after1.Should().NotBe(after2, "Toggle all folding must flip the fold state");
		System.Math.Min(after1, after2).Should().Be(0, "one toggle state is fully expanded");
		System.Math.Max(after1, after2).Should().BeGreaterThan(0, "the other toggle state is fully collapsed");
	}

	[AvaloniaTest]
	public async Task Toggle_Folding_Entry_Acts_On_The_Right_Clicked_Line_Not_The_Caret()
	{
		// Right-clicking a fold marker and choosing "Toggle folding" must collapse the fold under the
		// pointer, even when the caret sits in a completely different fold. The menu passes the clicked
		// offset via TextViewContext.TextLocation; reading the caret instead would toggle the wrong block.
		var (window, vm) = await TestHarness.BootAsync(3);

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(coreLibName, "System", "System.Object");
		vm.AssemblyTreeModel.SelectNode(typeNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		var view = await window.WaitForComponent<DecompilerTextView>();
		for (int i = 0; i < 8; i++)
		{
			Dispatcher.UIThread.RunJobs();
			await Task.Delay(25);
		}

		// ILSpy collapses method-body folds by default, leaving the outer type-body fold open. Two of
		// those collapsed method folds are disjoint siblings: right-click "Toggle folding" on one while
		// the caret is parked elsewhere. The clicked fold must expand; the other fold must not move.
		var collapsedSiblings = view.GetFoldingsForTest()
			.Where(f => f.IsFolded)
			.OrderBy(f => f.Start)
			.ToList();
		(int Start, int End)? caretSpan = null, clickedSpan = null;
		for (int a = 0; a < collapsedSiblings.Count && clickedSpan == null; a++)
		{
			for (int b = a + 1; b < collapsedSiblings.Count; b++)
			{
				bool disjoint = collapsedSiblings[b].Start > collapsedSiblings[a].End
					|| collapsedSiblings[a].Start > collapsedSiblings[b].End;
				if (disjoint)
				{
					caretSpan = (collapsedSiblings[a].Start, collapsedSiblings[a].End);
					clickedSpan = (collapsedSiblings[b].Start, collapsedSiblings[b].End);
					break;
				}
			}
		}
		clickedSpan.Should().NotBeNull("two disjoint collapsed folds are needed to tell the caret from the click");

		// Midpoints so each offset unambiguously lands inside its own fold.
		int caretFold = (caretSpan!.Value.Start + caretSpan.Value.End) / 2;
		int clickedFold = (clickedSpan!.Value.Start + clickedSpan.Value.End) / 2;

		// Keep the caret well clear of both folds. (Setting it inside a collapsed fold would make
		// AvaloniaEdit auto-expand that fold, which is exactly the behaviour we don't want to depend on.)
		view.Editor.TextArea.Caret.Offset = 0;
		view.IsFoldedAt(caretFold).Should().BeTrue("precondition: the caret-side fold is collapsed");
		view.IsFoldedAt(clickedFold).Should().BeTrue("precondition: the clicked fold is collapsed");

		var toggleOne = AppComposition.Current.GetExport<ContextMenuEntryRegistry>()
			.GetEntry(nameof(Resources._ToggleFolding));
		// The caret is parked at offset 0 -- nowhere near either fold. A caret-driven implementation
		// would toggle the fold at offset 0 (or nothing); the mouse-driven one toggles the clicked fold.
		toggleOne.Execute(new TextViewContext { TextView = view, TextLocation = clickedFold });
		Dispatcher.UIThread.RunJobs();

		view.IsFoldedAt(clickedFold).Should().BeFalse("the fold under the right-click must expand");
		view.IsFoldedAt(caretFold).Should().BeTrue("the unrelated fold must stay collapsed -- the action follows the mouse, not the caret");
	}
}
