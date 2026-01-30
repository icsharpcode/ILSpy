// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.ILSpy.TextView
{

	public class BracketHighlightRenderer : IBackgroundRenderer
	{
		BracketSearchResult result;
		Pen borderPen;
		Brush backgroundBrush;
		ICSharpCode.AvalonEdit.Rendering.TextView textView;

		public void SetHighlight(BracketSearchResult result)
		{
			if (this.result != result)
			{
				this.result = result;
				this.borderPen = (Pen)textView.FindResource(Themes.ResourceKeys.BracketHighlightBorderPen);
				this.backgroundBrush = (SolidColorBrush)textView.FindResource(Themes.ResourceKeys.BracketHighlightBackgroundBrush);
				textView.InvalidateLayer(this.Layer);
			}
		}

		public BracketHighlightRenderer(ICSharpCode.AvalonEdit.Rendering.TextView textView)
		{
			if (textView == null)
				throw new ArgumentNullException("textView");

			// resource loading safe guard
			var borderPenResource = textView.FindResource(Themes.ResourceKeys.BracketHighlightBorderPen);
			if (borderPenResource is Pen p)
				this.borderPen = p;

			var backgroundBrushResource = textView.FindResource(Themes.ResourceKeys.BracketHighlightBackgroundBrush);
			if (backgroundBrushResource is SolidColorBrush b)
				this.backgroundBrush = b;

			this.textView = textView;

			this.textView.BackgroundRenderers.Add(this);
		}

		public KnownLayer Layer {
			get {
				return KnownLayer.Selection;
			}
		}

		public void Draw(ICSharpCode.AvalonEdit.Rendering.TextView textView, DrawingContext drawingContext)
		{
			if (this.result == null)
				return;

			BackgroundGeometryBuilder builder = new BackgroundGeometryBuilder();

			builder.CornerRadius = 1;
			builder.AlignToWholePixels = true;
			builder.BorderThickness = borderPen?.Thickness ?? 0;

			builder.AddSegment(textView, new TextSegment() { StartOffset = result.OpeningBracketOffset, Length = result.OpeningBracketLength });
			builder.CloseFigure(); // prevent connecting the two segments
			builder.AddSegment(textView, new TextSegment() { StartOffset = result.ClosingBracketOffset, Length = result.ClosingBracketLength });

			Geometry geometry = builder.CreateGeometry();
			if (geometry != null)
			{
				drawingContext.DrawGeometry(backgroundBrush, borderPen, geometry);
			}
		}
	}
}