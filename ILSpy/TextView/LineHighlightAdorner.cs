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
using System.Diagnostics;
using System.Linq;

using AvaloniaEdit.Editing;
using AvaloniaEdit.Rendering;

using global::Avalonia;
using global::Avalonia.Media;
using global::Avalonia.Threading;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// A brief full-width highlight across a single line, played after a bookmark navigation so the
	/// destination line stands out even when several bookmarks sit close together. The translucent
	/// fill holds briefly, then fades over the remainder of an ~800 ms lifetime. Drawn on the
	/// selection layer so it sits behind the text and leaves it readable.
	/// </summary>
	public sealed class LineHighlightAdorner : IBackgroundRenderer
	{
		const int LifetimeMs = 800;
		const int HoldMs = 150;

		// Warm amber that reads on both light and dark themes; opacity is animated on top of this.
		static readonly IBrush Fill = new SolidColorBrush(Color.FromArgb(0x70, 0xC2, 0x7D, 0x1A)).ToImmutable();

		readonly TextArea textArea;
		readonly int line;
		readonly Stopwatch elapsed = Stopwatch.StartNew();
		readonly DispatcherTimer frameTimer;
		readonly DispatcherTimer lifetimeTimer;

		LineHighlightAdorner(TextArea textArea, int line)
		{
			this.textArea = textArea;
			this.line = line;
			frameTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
			frameTimer.Tick += (_, _) => textArea.TextView.InvalidateLayer(KnownLayer.Selection);
			lifetimeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(LifetimeMs) };
			lifetimeTimer.Tick += (_, _) => Dismiss();
		}

		public KnownLayer Layer => KnownLayer.Selection;

		public void Draw(AvaloniaEdit.Rendering.TextView textView, DrawingContext drawingContext)
		{
			long ms = elapsed.ElapsedMilliseconds;
			if (ms >= LifetimeMs)
				return;

			// The line may not be laid out yet on the first frame after a fresh decompile; once it is,
			// VisualLines carries it and the highlight appears.
			var visualLine = textView.GetVisualLine(line);
			if (visualLine == null)
				return;

			double top = visualLine.GetTextLineVisualYPosition(visualLine.TextLines[0], VisualYPosition.LineTop) - textView.VerticalOffset;
			double height = visualLine.Height;

			// Hold at full strength, then linear fade to zero over the rest of the lifetime.
			double opacity = ms < HoldMs ? 1.0 : 1.0 - (ms - HoldMs) / (double)(LifetimeMs - HoldMs);
			if (opacity <= 0)
				return;

			using var _ = drawingContext.PushOpacity(opacity);
			drawingContext.DrawRectangle(Fill, null, new Rect(0, top, textView.Bounds.Width, height));
		}

		/// <summary>Registers a one-shot line highlight for <paramref name="line"/> on <paramref name="textArea"/>.</summary>
		public static void DisplayLineHighlight(TextArea textArea, int line)
		{
			ArgumentNullException.ThrowIfNull(textArea);

			// Clear any still-running highlight first, so quick successive navigations replace rather
			// than stack adorners (each carries its own pair of timers driving redraws).
			foreach (var existing in textArea.TextView.BackgroundRenderers.OfType<LineHighlightAdorner>().ToArray())
				existing.Dismiss();

			var adorner = new LineHighlightAdorner(textArea, line);
			textArea.TextView.BackgroundRenderers.Add(adorner);
			adorner.frameTimer.Start();
			adorner.lifetimeTimer.Start();
		}

		/// <summary>Ends the highlight immediately: stops the timers and unregisters from the text view.</summary>
		public void Dismiss()
		{
			lifetimeTimer.Stop();
			frameTimer.Stop();
			textArea.TextView.BackgroundRenderers.Remove(this);
		}
	}
}
