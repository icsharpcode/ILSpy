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

using ILSpy.AppEnv;
using ILSpy.AssemblyTree;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class AssemblyTreeExpanderHitboxTests
{
	[AvaloniaTest]
	public async Task Expander_Toggle_Offers_At_Least_16x16_Clickable_Target()
	{
		// The +/- expander in the assembly tree must give a click target of at least 16x16 for
		// reliable tapping, while its visible glyph stays the classic 9x9 box and the column /
		// tree-line layout (which assume a 13px expander column, glyph centred at +8.5) is left
		// unchanged. The target must be genuinely hittable across the full 16x16 — not merely
		// occupy 16x16 of layout while only the 9x9 glyph receives input.

		// Arrange — boot, wait for assemblies, expand a node so an expandable row is realised.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.EnsureLazyChildren();
		assemblyNode.IsExpanded = true;
		vm.AssemblyTreeModel.SelectNode(assemblyNode);

		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<DataGrid>();

		// Let rows realise and layout settle.
		for (int i = 0; i < 8; i++)
		{
			Dispatcher.UIThread.RunJobs();
			grid.UpdateLayout();
			await Task.Delay(25);
		}

		// Act — locate the expander toggle of the (expandable) assembly row.
		var row = grid.GetVisualDescendants().OfType<DataGridRow>()
			.FirstOrDefault(r => RowMatches(r, assemblyNode));
		row.Should().NotBeNull("the expanded assembly row must be realised");
		var expander = row!.GetVisualDescendants().OfType<ToggleButton>()
			.FirstOrDefault(b => b.Name == "PART_Expander");
		expander.Should().NotBeNull("an expandable row must realise a PART_Expander toggle");
		expander!.IsEnabled.Should().BeTrue("the assembly row is expandable");

		// Assert — the click target is at least 16x16.
		expander.Bounds.Width.Should().BeGreaterThanOrEqualTo(16,
			"the expander click target must be at least 16px wide for reliable tapping");
		expander.Bounds.Height.Should().BeGreaterThanOrEqualTo(16,
			"the expander click target must be at least 16px tall for reliable tapping");

		// Assert — the visible glyph box is unchanged at 9x9 (the nearest Border around ExpandPath,
		// i.e. the drawn box, not the transparent hit-target wrapper).
		var glyphPath = expander.GetVisualDescendants().OfType<Path>()
			.FirstOrDefault(p => p.Name == "ExpandPath");
		glyphPath.Should().NotBeNull("the expander must render its ExpandPath glyph");
		var glyph = glyphPath!.GetVisualAncestors().OfType<Border>().FirstOrDefault();
		glyph.Should().NotBeNull("the expander must still render its glyph box");
		glyph!.Bounds.Width.Should().BeApproximately(9, 0.5, "the visible glyph box must stay 9px wide");
		glyph.Bounds.Height.Should().BeApproximately(9, 0.5, "the visible glyph box must stay 9px tall");

		// Assert — a real click well below the 9x9 glyph (y=14, inside the 16-tall target but
		// outside the centred glyph at ~y=3.5..12.5) collapses the node. This proves the grown
		// area is genuinely hittable, not just larger in layout.
		assemblyNode.IsExpanded.Should().BeTrue("precondition: node is expanded before the click");
		var hitPoint = expander.TranslatePoint(new Point(expander.Bounds.Width / 2, 14), window);
		hitPoint.Should().NotBeNull();
		HeadlessWindowExtensions.MouseDown(window, hitPoint!.Value, MouseButton.Left);
		HeadlessWindowExtensions.MouseUp(window, hitPoint.Value, MouseButton.Left);

		await Waiters.WaitForAsync(() => !assemblyNode.IsExpanded,
			description: "clicking the enlarged expander area (below the glyph) must toggle the node");
	}

	static bool RowMatches(DataGridRow row, SharpTreeNode target)
	{
		var ctx = row.DataContext;
		if (ReferenceEquals(ctx, target))
			return true;
		var itemProp = ctx?.GetType().GetProperty("Item");
		return itemProp is not null && ReferenceEquals(itemProp.GetValue(ctx), target);
	}
}
