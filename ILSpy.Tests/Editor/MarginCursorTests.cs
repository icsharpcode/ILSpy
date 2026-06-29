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

using AvaloniaEdit.Editing;
using AvaloniaEdit.Folding;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Bookmarks;
using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class MarginCursorTests
{
	// The text area paints the I-beam across its whole surface, so the left gutter margins inherit it
	// unless they set their own cursor. They are click targets (toggle a bookmark, fold a region, no-op
	// on line numbers), not text, so they must show the normal arrow instead.
	[AvaloniaTest]
	public async Task Left_Gutter_Margins_Use_The_Arrow_Cursor_Not_The_Editors_I_Beam()
	{
		var (window, vm) = await TestHarness.BootAsync(1);

		// Show line numbers so that margin is present alongside the bookmark gutter and folding margin.
		AppComposition.Current.GetExport<SettingsService>().DisplaySettings.ShowLineNumbers = true;

		// A type (not a single method) so the decompiled body carries foldings and the folding margin
		// is installed.
		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var objectNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(coreLibName, "System", "System.Object");
		vm.AssemblyTreeModel.SelectNode(objectNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		var view = await window.WaitForComponent<DecompilerTextView>();

		var margins = view.Editor.TextArea.LeftMargins;
		await Waiters.WaitForAsync(() => margins.OfType<FoldingMargin>().Any(),
			description: "the folding margin to be installed for the decompiled type");

		// All three margins the user meets in the gutter must be present, or the assertion below would
		// pass vacuously for a margin that never materialised.
		margins.OfType<BookmarkMargin>().Should().ContainSingle("the bookmark icon gutter is always present");
		margins.OfType<LineNumberMargin>().Should().ContainSingle("line numbers are enabled");
		margins.OfType<FoldingMargin>().Should().ContainSingle("the decompiled type carries foldings");

		foreach (Control margin in margins)
		{
			margin.Cursor.Should().BeSameAs(DecompilerTextView.ArrowCursor,
				$"the {margin.GetType().Name} is a click target, so it must show the arrow rather than the text I-beam");
		}
	}
}
