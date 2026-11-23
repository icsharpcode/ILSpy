// Copyright (c) 2026 Piero Viano
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
using System.Collections.Generic;
using System.Globalization;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;

using ICSharpCode.ILSpyX.Dependencies;

namespace ICSharpCode.ILSpy.Dependencies
{
	/// <summary>
	/// Draws an <see cref="AssemblyDependencyGraph"/> laid out by <see cref="LayeredLayout"/>:
	/// boxes for assemblies (bold border for roots, dashed red for unresolved references) and
	/// curved arrows for references (orange when satisfied by a different version). The mouse
	/// wheel zooms around the pointer, dragging pans, a click raises <see cref="NodeClicked"/> and
	/// a double-click raises <see cref="NodeDoubleClicked"/>.
	/// </summary>
	public sealed class DependencyGraphControl : Control
	{
		const double FontSize = 12;
		const double HorizontalPadding = 10;
		const double MinScale = 0.05;
		const double MaxScale = 4;
		const double DragThreshold = 4;

		static readonly Typeface NormalTypeface = new(FontFamily.Default);
		static readonly Typeface BoldTypeface = new(FontFamily.Default, FontStyle.Normal, FontWeight.Bold);

		AssemblyDependencyGraph? graph;
		LayeredLayoutResult? layout;
		double scale = 1;
		Vector offset;
		bool fitPending = true;
		Point? pressPoint;
		Vector pressOffset;
		bool dragging;

		public DependencyGraphControl()
		{
			ClipToBounds = true;
			Focusable = true;
		}

		/// <summary>Raised with the node index when a node is clicked.</summary>
		public event Action<int>? NodeClicked;

		/// <summary>Raised with the node index when a node is double-clicked.</summary>
		public event Action<int>? NodeDoubleClicked;

		public AssemblyDependencyGraph? Graph {
			get => graph;
			set {
				graph = value;
				layout = null;
				fitPending = true;
				InvalidateVisual();
			}
		}

		/// <summary>Index of the highlighted node, or -1.</summary>
		public int SelectedIndex { get; set; } = -1;

		/// <summary>The current layout (computed on first use).</summary>
		public LayeredLayoutResult? Layout => EnsureLayout();

		public double Scale => scale;

		LayeredLayoutResult? EnsureLayout()
		{
			if (layout != null || graph == null)
				return layout;
			var widths = new double[graph.Nodes.Count];
			foreach (var node in graph.Nodes)
			{
				var text = CreateLabel(node, Brushes.Black);
				widths[node.Index] = Math.Ceiling(text.Width) + 2 * HorizontalPadding;
			}
			layout = LayeredLayout.Compute(graph.Nodes.Count, graph.EdgePairs, widths,
				new LayeredLayoutOptions { NodeHeight = 28, LayerSpacing = 90, NodeSpacing = 14 });
			return layout;
		}

		static string LabelOf(AssemblyDependencyNode node)
			=> node.Version != null ? $"{node.Name} {node.Version}" : node.Name;

		static FormattedText CreateLabel(AssemblyDependencyNode node, IBrush brush)
			=> new(LabelOf(node), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
				node.IsRoot ? BoldTypeface : NormalTypeface, FontSize, brush);

		/// <summary>Scales and centers the drawing so the whole graph fits the control.</summary>
		public void ZoomToFit()
		{
			var l = EnsureLayout();
			if (l == null || Bounds.Width <= 0 || Bounds.Height <= 0 || l.Width <= 0 || l.Height <= 0)
				return;
			scale = Math.Clamp(Math.Min(Bounds.Width / l.Width, Bounds.Height / l.Height), MinScale, 1);
			offset = new Vector((Bounds.Width - l.Width * scale) / 2, (Bounds.Height - l.Height * scale) / 2);
			fitPending = false;
			InvalidateVisual();
		}

		/// <summary>Converts a point in control coordinates to layout coordinates.</summary>
		public Point ToLayout(Point controlPoint) => new((controlPoint.X - offset.X) / scale, (controlPoint.Y - offset.Y) / scale);

		/// <summary>Center of a node in control coordinates.</summary>
		public Point GetNodeCenter(int index)
		{
			var l = EnsureLayout() ?? throw new InvalidOperationException("No graph.");
			var r = l.Nodes[index];
			return new Point((r.X + r.Width / 2) * scale + offset.X, r.CenterY * scale + offset.Y);
		}

		/// <summary>Index of the node under a point in control coordinates, or -1.</summary>
		public int HitTestNode(Point controlPoint)
		{
			var l = EnsureLayout();
			if (l == null)
				return -1;
			var p = ToLayout(controlPoint);
			return l.HitTest(p.X, p.Y);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			var result = base.ArrangeOverride(finalSize);
			if (fitPending && finalSize.Width > 0 && finalSize.Height > 0)
			{
				// Bounds is updated after ArrangeOverride returns, so fit against the final size.
				var l = EnsureLayout();
				if (l != null && l.Width > 0 && l.Height > 0)
				{
					scale = Math.Clamp(Math.Min(finalSize.Width / l.Width, finalSize.Height / l.Height), MinScale, 1);
					offset = new Vector((finalSize.Width - l.Width * scale) / 2, (finalSize.Height - l.Height * scale) / 2);
					fitPending = false;
				}
			}
			return result;
		}

		public override void Render(DrawingContext context)
		{
			// A transparent background makes the whole area hit-testable for panning and zooming.
			context.FillRectangle(Brushes.Transparent, new Rect(Bounds.Size));
			var l = EnsureLayout();
			if (l == null || graph == null)
				return;

			bool dark = ActualThemeVariant == ThemeVariant.Dark;
			IBrush text = dark ? Brushes.Gainsboro : Brushes.Black;
			IBrush nodeFill = dark ? new SolidColorBrush(Color.FromRgb(0x2d, 0x33, 0x3b)) : new SolidColorBrush(Color.FromRgb(0xee, 0xf3, 0xfa));
			IBrush nodeStroke = dark ? new SolidColorBrush(Color.FromRgb(0x7a, 0x9c, 0xc6)) : new SolidColorBrush(Color.FromRgb(0x3c, 0x6e, 0xb4));
			IBrush unresolvedFill = dark ? new SolidColorBrush(Color.FromRgb(0x4a, 0x24, 0x24)) : new SolidColorBrush(Color.FromRgb(0xfd, 0xe2, 0xe2));
			IBrush unresolvedStroke = new SolidColorBrush(Color.FromRgb(0xc0, 0x39, 0x2b));
			IBrush edgeBrush = dark ? new SolidColorBrush(Color.FromRgb(0x8a, 0x8a, 0x8a)) : new SolidColorBrush(Color.FromRgb(0x70, 0x70, 0x70));
			IBrush mismatchBrush = new SolidColorBrush(Color.FromRgb(0xe6, 0x7e, 0x22));
			IBrush selectedStroke = new SolidColorBrush(Color.FromRgb(0xf1, 0xc4, 0x0f));

			using (context.PushTransform(Matrix.CreateScale(scale, scale) * Matrix.CreateTranslation(offset)))
			{
				for (int i = 0; i < l.Edges.Count; i++)
				{
					var edge = l.Edges[i];
					bool mismatch = graph.Edges[i].IsVersionMismatch;
					DrawEdge(context, edge.Points, new Pen(mismatch ? mismatchBrush : edgeBrush, mismatch ? 1.6 : 1.1));
				}
				foreach (var node in graph.Nodes)
				{
					var r = l.Nodes[node.Index];
					var rect = new Rect(r.X, r.Y, r.Width, r.Height);
					var fill = node.IsResolved ? nodeFill : unresolvedFill;
					Pen pen;
					if (node.Index == SelectedIndex)
						pen = new Pen(selectedStroke, 3);
					else if (!node.IsResolved)
						pen = new Pen(unresolvedStroke, 1.5, new DashStyle(new double[] { 4, 3 }, 0));
					else
						pen = new Pen(nodeStroke, node.IsRoot ? 2.5 : 1.2);
					context.DrawRectangle(fill, pen, rect, 4, 4);
					var label = CreateLabel(node, node.IsResolved ? text : unresolvedStroke);
					context.DrawText(label, new Point(r.X + HorizontalPadding, r.CenterY - label.Height / 2));
				}
			}
		}

		// Edges between layers are drawn as cubic curves with horizontal tangents, straight runs
		// through skipped layers as lines, and the end gets an arrow head.
		static void DrawEdge(DrawingContext context, IReadOnlyList<LayoutPoint> points, Pen pen)
		{
			if (points.Count < 2)
				return;
			var geometry = new StreamGeometry();
			using (var g = geometry.Open())
			{
				g.BeginFigure(ToPoint(points[0]), false);
				for (int i = 1; i < points.Count; i++)
				{
					var a = ToPoint(points[i - 1]);
					var b = ToPoint(points[i]);
					if (Math.Abs(a.Y - b.Y) < 0.5)
					{
						g.LineTo(b);
					}
					else
					{
						double dx = (b.X - a.X) / 2;
						g.CubicBezierTo(new Point(a.X + dx, a.Y), new Point(b.X - dx, b.Y), b);
					}
				}
				g.EndFigure(false);
			}
			context.DrawGeometry(null, pen, geometry);

			var tip = ToPoint(points[^1]);
			double direction = Math.Sign(points[^1].X - points[^2].X);
			if (direction == 0)
				direction = 1;
			const double length = 8, half = 4;
			var head = new StreamGeometry();
			using (var g = head.Open())
			{
				g.BeginFigure(tip, true);
				g.LineTo(new Point(tip.X - direction * length, tip.Y - half));
				g.LineTo(new Point(tip.X - direction * length, tip.Y + half));
				g.EndFigure(true);
			}
			context.DrawGeometry(pen.Brush, null, head);
		}

		static Point ToPoint(LayoutPoint p) => new(p.X, p.Y);

		protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
		{
			base.OnPointerWheelChanged(e);
			var position = e.GetPosition(this);
			double factor = Math.Pow(1.15, e.Delta.Y);
			double newScale = Math.Clamp(scale * factor, MinScale, MaxScale);
			// Keep the layout point under the pointer fixed while zooming.
			var anchor = ToLayout(position);
			scale = newScale;
			offset = new Vector(position.X - anchor.X * scale, position.Y - anchor.Y * scale);
			fitPending = false;
			InvalidateVisual();
			e.Handled = true;
		}

		protected override void OnPointerPressed(PointerPressedEventArgs e)
		{
			base.OnPointerPressed(e);
			if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
				return;
			var position = e.GetPosition(this);
			if (e.ClickCount >= 2)
			{
				int index = HitTestNode(position);
				pressPoint = null;
				if (index >= 0)
				{
					NodeDoubleClicked?.Invoke(index);
					e.Handled = true;
				}
				return;
			}
			pressPoint = position;
			pressOffset = offset;
			dragging = false;
			e.Pointer.Capture(this);
			e.Handled = true;
		}

		protected override void OnPointerMoved(PointerEventArgs e)
		{
			base.OnPointerMoved(e);
			if (pressPoint is not { } start)
				return;
			var delta = e.GetPosition(this) - start;
			if (!dragging && (Math.Abs(delta.X) > DragThreshold || Math.Abs(delta.Y) > DragThreshold))
				dragging = true;
			if (dragging)
			{
				offset = pressOffset + delta;
				fitPending = false;
				InvalidateVisual();
			}
		}

		protected override void OnPointerReleased(PointerReleasedEventArgs e)
		{
			base.OnPointerReleased(e);
			if (pressPoint is not { } start)
				return;
			pressPoint = null;
			e.Pointer.Capture(null);
			if (dragging)
			{
				dragging = false;
				return;
			}
			int index = HitTestNode(start);
			if (index >= 0)
			{
				SelectedIndex = index;
				InvalidateVisual();
				NodeClicked?.Invoke(index);
			}
			e.Handled = true;
		}
	}
}
