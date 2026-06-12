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
using Avalonia.Media;

using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// Paints a soft-tinted rounded outline around the two brackets of a matched pair.
	/// Lives on the selection layer (below text) so the bracket characters stay legible.
	/// </summary>
	public sealed class BracketHighlightRenderer : IBackgroundRenderer
	{
		// Avalonia ships with no theme resource for "bracket highlight" by default, so we
		// hard-code subtle defaults that work on both light and dark editor backgrounds.
		// (#406680 outline, ~20% transparent #99A8C8 fill — sampled from VS's bracket
		// pair highlight at 100% font size on the default Light theme.)
		static readonly Pen DefaultBorderPen = new(new SolidColorBrush(Color.FromArgb(0x80, 0x40, 0x66, 0x80)), 1);
		static readonly IBrush DefaultBackgroundBrush = new SolidColorBrush(Color.FromArgb(0x33, 0x99, 0xA8, 0xC8));

		BracketSearchResult? result;
		readonly global::AvaloniaEdit.Rendering.TextView textView;

		public BracketHighlightRenderer(global::AvaloniaEdit.Rendering.TextView textView)
		{
			this.textView = textView ?? throw new ArgumentNullException(nameof(textView));
			textView.BackgroundRenderers.Add(this);
		}

		public KnownLayer Layer => KnownLayer.Selection;

		public void SetHighlight(BracketSearchResult? result)
		{
			if (this.result != result)
			{
				this.result = result;
				textView.InvalidateLayer(Layer);
			}
		}

		public void Draw(global::AvaloniaEdit.Rendering.TextView textView, DrawingContext drawingContext)
		{
			if (this.result == null)
				return;
			var builder = new BackgroundGeometryBuilder {
				CornerRadius = 1,
				AlignToWholePixels = true,
				BorderThickness = DefaultBorderPen.Thickness,
			};
			builder.AddSegment(textView, new TextSegment { StartOffset = result.OpeningBracketOffset, Length = result.OpeningBracketLength });
			builder.CloseFigure(); // prevent connecting the two segments
			builder.AddSegment(textView, new TextSegment { StartOffset = result.ClosingBracketOffset, Length = result.ClosingBracketLength });
			var geometry = builder.CreateGeometry();
			if (geometry != null)
				drawingContext.DrawGeometry(DefaultBackgroundBrush, DefaultBorderPen, geometry);
		}
	}
}
