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
using System.Diagnostics;

using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;

using AvaloniaEdit.Editing;
using AvaloniaEdit.Rendering;

namespace ICSharpCode.ILSpy.Bookmarks
{
	/// <summary>
	/// The icon gutter to the left of the line numbers in the decompiled C# view. Draws a bookmark
	/// glyph (greyed when disabled) on every line that holds a bookmark, and toggles a bookmark when
	/// the gutter is clicked -- the line &lt;-&gt; bookmark mapping is owned by the text view. After a
	/// navigation the destination glyph briefly pulses so the user spots which line they landed on.
	/// </summary>
	public sealed class BookmarkMargin : AbstractMargin
	{
		const double IconSize = 16;
		// One scale-up-and-back bounce, peaking at 1 + PulseAmount halfway through PulseDurationMs.
		const double PulseAmount = 0.35;
		const int PulseDurationMs = 600;

		readonly TextView.DecompilerTextView owner;
		readonly BookmarkManager? manager;
		readonly DispatcherTimer pulseTimer;
		readonly Stopwatch pulseElapsed = new();
		int pulseLine = -1;

		public BookmarkMargin(TextView.DecompilerTextView owner)
		{
			this.owner = owner;
			manager = AppEnv.AppComposition.TryGetExport<BookmarkManager>();
			if (manager != null)
				manager.Changed += OnBookmarksChanged;
			pulseTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
			pulseTimer.Tick += OnPulseTick;
		}

		void OnBookmarksChanged(object? sender, EventArgs e) => InvalidateVisual();

		/// <summary>Plays the one-shot bounce on the bookmark glyph at <paramref name="line"/>.</summary>
		public void PulseLine(int line)
		{
			pulseLine = line;
			pulseElapsed.Restart();
			pulseTimer.Start();
			InvalidateVisual();
		}

		void OnPulseTick(object? sender, EventArgs e)
		{
			if (pulseElapsed.ElapsedMilliseconds >= PulseDurationMs)
			{
				pulseTimer.Stop();
				pulseLine = -1;
			}
			InvalidateVisual();
		}

		// Scale for the pulsing glyph: 1 -> 1+PulseAmount -> 1 over the pulse lifetime.
		double CurrentPulseScale()
		{
			double t = pulseElapsed.ElapsedMilliseconds / (double)PulseDurationMs;
			return t >= 1 ? 1.0 : 1.0 + PulseAmount * Math.Sin(Math.PI * t);
		}

		protected override Size MeasureOverride(Size availableSize) => new(IconSize, 0);

		public override void Render(DrawingContext drawingContext)
		{
			var textView = TextView;
			if (manager == null || textView == null || !textView.VisualLinesValid)
				return;

			// Map the document lines that currently hold a bookmark to their glyph. Bookmarks not in
			// this document resolve to no line and are skipped.
			var glyphByLine = new Dictionary<int, IImage>();
			foreach (var bookmark in manager.Bookmarks)
			{
				if (owner.GetLineForBookmark(bookmark) is { } line)
					glyphByLine[line] = bookmark.Enabled ? Images.Bookmark : Images.BookmarkDisable;
			}
			if (glyphByLine.Count == 0)
				return;

			foreach (var visualLine in textView.VisualLines)
			{
				int lineNumber = visualLine.FirstDocumentLine.LineNumber;
				if (!glyphByLine.TryGetValue(lineNumber, out var glyph))
					continue;
				double top = visualLine.GetTextLineVisualYPosition(visualLine.TextLines[0], VisualYPosition.TextTop) - textView.VerticalOffset;
				var rect = new Rect(0, top, IconSize, IconSize);

				double scale = lineNumber == pulseLine ? CurrentPulseScale() : 1.0;
				if (scale != 1.0)
				{
					// Scale about the glyph centre so it grows in place.
					double cx = IconSize / 2, cy = top + IconSize / 2;
					var transform = Matrix.CreateTranslation(-cx, -cy) * Matrix.CreateScale(scale, scale) * Matrix.CreateTranslation(cx, cy);
					using (drawingContext.PushTransform(transform))
						drawingContext.DrawImage(glyph, rect);
				}
				else
				{
					drawingContext.DrawImage(glyph, rect);
				}
			}
		}

		protected override void OnPointerPressed(PointerPressedEventArgs e)
		{
			base.OnPointerPressed(e);
			var textView = TextView;
			if (e.Handled || textView == null)
				return;
			double y = e.GetPosition(textView).Y + textView.VerticalOffset;
			var visualLine = textView.GetVisualLineFromVisualTop(y);
			if (visualLine == null)
				return;
			owner.ToggleBookmarkAtLine(visualLine.FirstDocumentLine.LineNumber);
			e.Handled = true;
		}
	}
}
