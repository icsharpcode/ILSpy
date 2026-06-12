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

using AvaloniaEdit.Editing;
using AvaloniaEdit.Rendering;

using global::Avalonia;
using global::Avalonia.Controls.Documents;
using global::Avalonia.Media;
using global::Avalonia.Threading;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// Brief animated rectangle around the caret played after a navigation gesture (clicking
	/// a hyperlink that scrolls to a different location, jumping back/forward through history)
	/// so the user spots where the caret landed. The rectangle inflates 25% and collapses back
	/// over the first 600 ms, then fades out over the following 200 ms; total visible lifetime
	/// is one second.
	/// </summary>
	public sealed class CaretHighlightAdorner : IBackgroundRenderer
	{
		const int GrowDurationMs = 300;
		const int FadeBeginMs = 450;
		const int FadeDurationMs = 200;
		const int LifetimeMs = 1000;

		readonly Rect minRect;
		readonly Rect maxRect;
		readonly IPen pen;
		readonly Stopwatch elapsed = Stopwatch.StartNew();
		readonly TextArea textArea;
		readonly DispatcherTimer frameTimer;
		readonly DispatcherTimer lifetimeTimer;

		CaretHighlightAdorner(TextArea textArea)
		{
			this.textArea = textArea;

			// Caret rect is in document coordinates; subtracting ScrollOffset converts to the
			// viewport-relative space IBackgroundRenderer.Draw paints into.
			var caretRect = textArea.Caret.CalculateCaretRectangle();
			caretRect = caretRect.Translate(-textArea.TextView.ScrollOffset);
			minRect = caretRect;

			double growBy = Math.Max(caretRect.Width, caretRect.Height) * 0.25;
			maxRect = caretRect.Inflate(growBy);

			// TextView itself has no Foreground; the inherited TextElement.Foreground attached
			// property carries the editor's text colour through the visual tree. Falls back to
			// black when unset (e.g. design-time / standalone-renderer tests).
			var brush = textArea.TextView.GetValue(TextElement.ForegroundProperty) ?? Brushes.Black;
			pen = new Pen(brush, 1).ToImmutable();

			// The frame timer ticks at ~60 fps to invalidate the Caret layer so Draw re-runs with
			// fresh elapsed time; the lifetime timer dismisses the adorner after one second.
			frameTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
			frameTimer.Tick += (_, _) => textArea.TextView.InvalidateLayer(KnownLayer.Caret);
			lifetimeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(LifetimeMs) };
			lifetimeTimer.Tick += (_, _) => Dismiss();
		}

		public KnownLayer Layer => KnownLayer.Caret;

		public void Draw(AvaloniaEdit.Rendering.TextView textView, DrawingContext drawingContext)
		{
			long ms = elapsed.ElapsedMilliseconds;
			if (ms >= LifetimeMs)
				return;

			// Bounce: 0..GrowDurationMs grows from min → max, GrowDurationMs..2× shrinks back.
			Rect rect;
			if (ms < GrowDurationMs)
				rect = Lerp(minRect, maxRect, ms / (double)GrowDurationMs);
			else if (ms < GrowDurationMs * 2)
				rect = Lerp(maxRect, minRect, (ms - GrowDurationMs) / (double)GrowDurationMs);
			else
				rect = minRect;

			// Opacity holds at 1.0 until FadeBeginMs, then linear ramp to 0 over FadeDurationMs.
			double opacity;
			if (ms < FadeBeginMs)
				opacity = 1.0;
			else if (ms < FadeBeginMs + FadeDurationMs)
				opacity = 1.0 - (ms - FadeBeginMs) / (double)FadeDurationMs;
			else
				opacity = 0;
			if (opacity <= 0)
				return;

			using var _ = drawingContext.PushOpacity(opacity);
			drawingContext.DrawRectangle(null, pen, rect, 2, 2);
		}

		static Rect Lerp(Rect a, Rect b, double t) => new(
			a.X + (b.X - a.X) * t,
			a.Y + (b.Y - a.Y) * t,
			a.Width + (b.Width - a.Width) * t,
			a.Height + (b.Height - a.Height) * t);

		/// <summary>
		/// Registers a one-shot caret-highlight animation on <paramref name="textArea"/>. Spins
		/// up two timers: one ticks at ~60 fps to invalidate the Caret layer so <see cref="Draw"/>
		/// re-runs with fresh elapsed time, the other calls <see cref="Dismiss"/> after one second.
		/// </summary>
		public static void DisplayCaretHighlightAnimation(TextArea textArea)
		{
			ArgumentNullException.ThrowIfNull(textArea);

			var adorner = new CaretHighlightAdorner(textArea);
			textArea.TextView.BackgroundRenderers.Add(adorner);
			adorner.frameTimer.Start();
			adorner.lifetimeTimer.Start();
		}

		/// <summary>
		/// Ends the animation immediately: stops both timers and unregisters the adorner from the
		/// text view. Invoked by the one-second lifetime timer, and usable to tear the highlight
		/// down on demand instead of waiting the lifetime out.
		/// </summary>
		public void Dismiss()
		{
			lifetimeTimer.Stop();
			frameTimer.Stop();
			textArea.TextView.BackgroundRenderers.Remove(this);
		}
	}
}
