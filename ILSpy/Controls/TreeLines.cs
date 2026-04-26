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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

using ICSharpCode.ILSpyX.TreeView;

namespace ILSpy.Controls
{
	// Classic Windows-Explorer tree-lines, ported from ILSpy WPF SharpTreeView's LinesRenderer.
	// Drawn as a layer behind the DataGridHierarchicalPresenter content so it can reach across
	// the indent area for ancestor verticals. X positions assume:
	//   - presenter Indent step  = 16 px (DataGridHierarchicalPresenterIndent)
	//   - expander column width  = 13 px (DataGridHierarchicalExpanderSize override)
	//   - expander glyph centred at +8.5 px inside its column
	public class TreeLines : Control
	{
		const double IndentStep = 16;
		const double ExpanderCenterOffset = 8.5;
		const double HorizontalStubLength = 10;

		static readonly IPen Pen;

		public static readonly StyledProperty<SharpTreeNode?> NodeProperty =
			AvaloniaProperty.Register<TreeLines, SharpTreeNode?>(nameof(Node));

		public static readonly StyledProperty<int> LevelProperty =
			AvaloniaProperty.Register<TreeLines, int>(nameof(Level));

		static TreeLines()
		{
			var pen = new Pen(Brushes.LightGray, 1);
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

		public override void Render(DrawingContext context)
		{
			var node = Node;
			if (node == null || node.IsRoot)
				return;

			var height = Bounds.Height;
			var midY = height / 2;
			var x = Level * IndentStep + ExpanderCenterOffset;

			context.DrawLine(Pen, new Point(x, midY), new Point(x + HorizontalStubLength, midY));

			if (node.IsLast)
				context.DrawLine(Pen, new Point(x, 0), new Point(x, midY));
			else
				context.DrawLine(Pen, new Point(x, 0), new Point(x, height));

			var current = node;
			while (current.Parent != null && !current.Parent.IsRoot)
			{
				current = current.Parent;
				x -= IndentStep;
				if (!current.IsLast)
					context.DrawLine(Pen, new Point(x, 0), new Point(x, height));
			}
		}
	}
}
