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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

using ICSharpCode.ILSpyX.TreeView;

namespace ICSharpCode.ILSpy.Controls
{
	// Classic Windows-Explorer tree-lines: drawn as a layer behind the
	// DataGridHierarchicalPresenter content so it can reach across
	// the indent area for ancestor verticals. X positions assume:
	//   - presenter Indent step  = 16 px (DataGridHierarchicalPresenterIndent)
	//   - expander column width  = 13 px (DataGridHierarchicalExpanderSize override)
	//   - expander glyph centred at +8.5 px inside its column
	public class TreeLines : Control
	{
		// 18.5 = icon centre (25) - expander centre (6.5): one step puts a child's +/- box directly
		// under its parent's icon, so the vertical line passes through both.
		const double IndentStep = 18.5;
		const double ExpanderCenterOffset = 6.5;   // the +/- box centre within a level
		const double IconLeftOffset = 17;          // expander column (13) + icon left margin (4)

		static readonly IPen Pen;

		public static readonly StyledProperty<SharpTreeNode?> NodeProperty =
			AvaloniaProperty.Register<TreeLines, SharpTreeNode?>(nameof(Node));

		public static readonly StyledProperty<int> LevelProperty =
			AvaloniaProperty.Register<TreeLines, int>(nameof(Level));

		static TreeLines()
		{
			// Classic Windows-Explorer dotted connector lines.
			var pen = new Pen(Brushes.Gray, 1) { DashStyle = new DashStyle(new double[] { 1, 1 }, 0) };
			Pen = pen.ToImmutable();
			AffectsRender<TreeLines>(NodeProperty, LevelProperty);
			IsHitTestVisibleProperty.OverrideDefaultValue<TreeLines>(false);
		}

		public SharpTreeNode? Node {
			get => GetValue(NodeProperty);
			set => SetValue(NodeProperty, value);
		}

		public int Level {
			get => GetValue(LevelProperty);
			set => SetValue(LevelProperty, value);
		}

		// Centre a 1px stroke on a pixel boundary.
		static double Snap(double v) => Math.Floor(v) + 0.5;

		public override void Render(DrawingContext context)
		{
			var node = Node;
			if (node == null || node.IsRoot)
				return;

			double height = Bounds.Height;
			double midY = Snap(height / 2);

			// The node's vertical runs through its own +/- box; because a step is exactly the
			// icon-to-expander distance, that x is also under the parent's icon one row up, so the
			// line visually connects the node to its parent. The +/- box is centred on it.
			double x = (Level - 1) * IndentStep + ExpanderCenterOffset;
			double xs = Snap(x);

			// Horizontal connector from the +/- box to this node's icon.
			context.DrawLine(Pen, new Point(xs, midY), new Point(Snap((Level - 1) * IndentStep + IconLeftOffset), midY));

			// The node's own vertical: up to the parent's icon, down to the next sibling (last child
			// stops at the elbow).
			context.DrawLine(Pen, new Point(xs, 0), new Point(xs, node.IsLast ? midY : height));

			// Continuation verticals for ancestors that still have a sibling below them.
			var current = node;
			double ax = x;
			while (true)
			{
				ax -= IndentStep;
				current = current.Parent;
				if (ax < 0 || current is null || current.IsRoot)
					break;
				if (!current.IsLast)
					context.DrawLine(Pen, new Point(Snap(ax), 0), new Point(Snap(ax), height));
			}
		}
	}
}
