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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class AssemblyTreeExpanderHitboxTests
{
	[AvaloniaTest]
	public async Task Expander_Toggle_Offers_At_Least_16x16_Clickable_Target()
	{
		// The +/- expander's click target fills the 13px expander column and the 16px row height,
		// while the drawn glyph stays the classic 9x9 box centred on the tree connector lines. The
		// grown area has to be genuinely hittable rather than merely occupied in layout, so the
		// test clicks below the glyph instead of measuring the boxes.

		// Arrange — boot, wait for assemblies, expand a node so an expandable row is realised.
		var (window, vm) = await TestHarness.BootAsync(3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.Expand();
		vm.AssemblyTreeModel.SelectNode(assemblyNode);
		TestCapture.Step("system-linq-expanded-and-selected");

		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();

		// Act — locate the expander toggle of the (expandable) assembly row, once the row and its
		// template have been realised.
		ToggleButton? expander = null;
		await Waiters.WaitForAsync(
			() => {
				grid.UpdateLayout();
				expander = grid.GetVisualDescendants().OfType<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeViewItem>()
					.FirstOrDefault(r => RowMatches(r, assemblyNode))
					?.GetVisualDescendants().OfType<ToggleButton>()
					.FirstOrDefault(b => b.Name == "PART_Expander");
				return expander is { Bounds.Height: >= 16 };
			},
			description: "the expanded assembly row must realise a PART_Expander toggle filling the row height");
		expander!.IsEnabled.Should().BeTrue("the assembly row is expandable");

		// Assert — a real click well below the 9x9 glyph (y=14, inside the 16-tall target but
		// outside the centred glyph at ~y=3.5..12.5) collapses the node. This proves the grown
		// area is genuinely hittable, not just larger in layout.
		assemblyNode.IsExpanded.Should().BeTrue("precondition: node is expanded before the click");
		var hitPoint = expander.TranslatePoint(new Point(expander.Bounds.Width / 2, 14), window);
		hitPoint.Should().NotBeNull();
		HeadlessWindowExtensions.MouseDown(window, hitPoint!.Value, MouseButton.Left);
		HeadlessWindowExtensions.MouseUp(window, hitPoint.Value, MouseButton.Left);
		TestCapture.Step("clicked-enlarged-expander-area");

		await Waiters.WaitForAsync(() => !assemblyNode.IsExpanded,
			description: "clicking the enlarged expander area (below the glyph) must toggle the node");
	}

	static bool RowMatches(ICSharpCode.ILSpy.Controls.TreeView.SharpTreeViewItem row, SharpTreeNode target)
		=> ReferenceEquals(row.DataContext, target);
}
