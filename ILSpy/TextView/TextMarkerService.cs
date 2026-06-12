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
using System.Collections.Generic;
using System.Linq;

using Avalonia;
using Avalonia.Media;
using Avalonia.Threading;

using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// Manages background-coloured text marks over the editor — used for local-reference
	/// highlighting today, and a useful primitive for future features (errors, search hits,
	/// …). Currently only the coloured-background path is implemented; underline marker
	/// types (squiggly / dotted / solid) and per-marker foreground / typeface tweaks are
	/// deliberately omitted until something actually needs them.
	/// </summary>
	sealed class TextMarkerService : IBackgroundRenderer
	{
		readonly AvaloniaEdit.Rendering.TextView textView;
		TextSegmentCollection<TextMarker>? markers;

		public TextMarkerService(AvaloniaEdit.Rendering.TextView textView)
		{
			this.textView = textView ?? throw new ArgumentNullException(nameof(textView));
			textView.DocumentChanged += OnDocumentChanged;
			OnDocumentChanged(null, EventArgs.Empty);
		}

		void OnDocumentChanged(object? sender, EventArgs e)
		{
			markers = textView.Document != null ? new TextSegmentCollection<TextMarker>(textView.Document) : null;
		}

		public TextMarker Create(int startOffset, int length)
		{
			if (markers == null)
				throw new InvalidOperationException("Cannot create a marker when not attached to a document");
			var textLength = textView.Document.TextLength;
			if (startOffset < 0 || startOffset > textLength)
				throw new ArgumentOutOfRangeException(nameof(startOffset));
			if (length < 0 || startOffset + length > textLength)
				throw new ArgumentOutOfRangeException(nameof(length));

			var marker = new TextMarker(this, startOffset, length);
			markers.Add(marker);
			return marker;
		}

		public void Remove(TextMarker marker)
		{
			if (markers != null && markers.Remove(marker))
				Redraw(marker);
		}

		public void RemoveAll(Predicate<TextMarker> predicate)
		{
			if (markers == null)
				return;
			foreach (var m in markers.ToArray())
				if (predicate(m))
					Remove(m);
		}

		internal void Redraw(ISegment segment) => textView.Redraw(segment);

		// IBackgroundRenderer

		public KnownLayer Layer => KnownLayer.Selection; // draw behind selection

		public void Draw(AvaloniaEdit.Rendering.TextView textView, DrawingContext drawingContext)
		{
			if (markers == null || !textView.VisualLinesValid)
				return;
			var visualLines = textView.VisualLines;
			if (visualLines.Count == 0)
				return;

			int viewStart = visualLines.First().FirstDocumentLine.Offset;
			int viewEnd = visualLines.Last().LastDocumentLine.EndOffset;
			foreach (var marker in markers.FindOverlappingSegments(viewStart, viewEnd - viewStart))
			{
				if (marker.BackgroundColor is not { } bg)
					continue;
				var geo = new BackgroundGeometryBuilder { AlignToWholePixels = true, CornerRadius = 3 };
				geo.AddSegment(textView, marker);
				var geometry = geo.CreateGeometry();
				if (geometry != null)
					drawingContext.DrawGeometry(new SolidColorBrush(bg), null, geometry);
			}
		}
	}

	/// <summary>
	/// A single highlighted span. Setting <see cref="BackgroundColor"/> triggers a redraw of
	/// just that segment.
	/// </summary>
	sealed class TextMarker : TextSegment
	{
		readonly TextMarkerService service;
		Color? backgroundColor;

		public TextMarker(TextMarkerService service, int startOffset, int length)
		{
			this.service = service;
			StartOffset = startOffset;
			Length = length;
		}

		public Color? BackgroundColor {
			get => backgroundColor;
			set {
				if (backgroundColor == value)
					return;
				backgroundColor = value;
				service.Redraw(this);
			}
		}
	}
}
