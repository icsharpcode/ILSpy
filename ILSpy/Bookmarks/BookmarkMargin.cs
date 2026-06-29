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
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
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
		// Faded glyph shown on a hovered, not-yet-bookmarked line to hint the gutter is clickable;
		// kept translucent so it reads as a preview, not an actual (opaque) bookmark.
		const double HoverPreviewOpacity = 0.5;
		static readonly IBrush FallbackBackground = new SolidColorBrush(Color.FromRgb(0xF3, 0xF0, 0xD0));
		static readonly IBrush FallbackBorder = new SolidColorBrush(Color.FromRgb(0xC8, 0xCD, 0xD3));
		// One scale-up-and-back bounce, peaking at 1 + PulseAmount halfway through PulseDurationMs.
		const double PulseAmount = 0.35;
		const int PulseDurationMs = 600;

		readonly TextView.DecompilerTextView owner;
		BookmarkManager? manager;
		readonly DispatcherTimer pulseTimer;
		readonly Stopwatch pulseElapsed = new();
		bool isAttached;
		bool subscribedToManager;
		int pulseLine = -1;
		int hoverLine = -1;
		// A line whose hover preview stays hidden after a removal click, until the pointer leaves it.
		int suppressedHoverLine = -1;
		// Cached document-line -> glyph map and the content version it was built against.
		Dictionary<int, IImage>? glyphByLine;
		(object?, object?) glyphByLineVersion;

		public BookmarkMargin(TextView.DecompilerTextView owner)
		{
			this.owner = owner;
			pulseTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
			pulseTimer.Tick += OnPulseTick;
		}

		BookmarkManager? Manager {
			get {
				manager ??= AppEnv.AppComposition.TryGetExport<BookmarkManager>();
				EnsureManagerSubscription();
				return manager;
			}
		}

		void EnsureManagerSubscription()
		{
			if (!isAttached || subscribedToManager || manager == null)
				return;
			manager.Changed += OnBookmarksChanged;
			subscribedToManager = true;
		}

		// The manager is a shared singleton, so its Changed event would otherwise keep a closed tab's
		// margin (and its pulse timer) alive. Track the subscription to the margin's time in the tree.
		protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
		{
			base.OnAttachedToVisualTree(e);
			isAttached = true;
			_ = Manager;
		}

		protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
		{
			base.OnDetachedFromVisualTree(e);
			isAttached = false;
			if (manager != null && subscribedToManager)
			{
				manager.Changed -= OnBookmarksChanged;
				subscribedToManager = false;
			}
			pulseTimer.Stop();
			pulseLine = -1;
			hoverLine = -1;
			glyphByLine = null;
		}

		void OnBookmarksChanged(object? sender, EventArgs e)
		{
			glyphByLine = null;
			InvalidateVisual();
		}

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
			DrawBackground(drawingContext);

			var textView = TextView;
			var manager = Manager;
			if (manager == null || textView == null || !textView.VisualLinesValid)
				return;

			var glyphByLine = GetGlyphByLine(manager);
			foreach (var visualLine in textView.VisualLines)
			{
				int lineNumber = visualLine.FirstDocumentLine.LineNumber;
				bool hasGlyph = glyphByLine.TryGetValue(lineNumber, out var glyph);
				bool isHoverPreview = !hasGlyph && lineNumber == hoverLine;
				if (!hasGlyph && !isHoverPreview)
					continue;
				double top = visualLine.GetTextLineVisualYPosition(visualLine.TextLines[0], VisualYPosition.TextTop) - textView.VerticalOffset;
				var rect = new Rect(0, top, IconSize, IconSize);
				glyph ??= Images.Bookmark;

				if (isHoverPreview)
				{
					using (drawingContext.PushOpacity(HoverPreviewOpacity))
						drawingContext.DrawImage(HoverPreviewGlyph(), rect);
					continue;
				}

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

		// The document line -> glyph map for the current document and bookmark set. Resolving a bookmark
		// to its line scans the document's references, so it is computed once and reused across the many
		// repaints a scroll, hover or pulse triggers. It is rebuilt when the bookmark set changes (which
		// clears the cache) or when the displayed document changes -- detected by its content version,
		// since the editor reuses one TextDocument and only swaps its text, so the document reference
		// never changes.
		Dictionary<int, IImage> GetGlyphByLine(BookmarkManager manager)
		{
			var version = owner.BookmarkContentVersion;
			if (glyphByLine != null && version.Equals(glyphByLineVersion))
				return glyphByLine;
			glyphByLineVersion = version;

			// Bookmarks not anchored in this document resolve to no line and are skipped.
			var map = new Dictionary<int, IImage>();
			foreach (var bookmark in manager.Bookmarks)
			{
				if (owner.GetLineForBookmark(bookmark, updateRenderedLine: false) is { } line)
				{
					if (bookmark.LineNumber != line)
						Dispatcher.UIThread.Post(() => bookmark.UpdateRenderedLineNumber(line), DispatcherPriority.Background);
					map[line] = bookmark.Enabled ? Images.Bookmark : Images.BookmarkDisable;
				}
			}
			return glyphByLine = map;
		}

		// A bitmap copy of the bookmark glyph, shared by all gutters, used only for the hover preview.
		// SvgImage paints through a custom Skia draw operation that ignores DrawingContext.PushOpacity,
		// so the SVG can't be faded directly; a rasterized bitmap is composited at the pushed opacity.
		static Bitmap? hoverPreviewGlyph;

		static Bitmap HoverPreviewGlyph()
		{
			if (hoverPreviewGlyph != null)
				return hoverPreviewGlyph;
			var source = Images.Bookmark;
			// Oversample so the bitmap stays crisp when the gutter draws it at the device scale.
			int pixels = (int)(IconSize * 3);
			var bitmap = new RenderTargetBitmap(new PixelSize(pixels, pixels), new Vector(96, 96));
			using (var context = bitmap.CreateDrawingContext())
				source.Draw(context, new Rect(source.Size), new Rect(0, 0, pixels, pixels));
			return hoverPreviewGlyph = bitmap;
		}

		void DrawBackground(DrawingContext drawingContext)
		{
			// The filled background keeps the empty gutter hit-testable so the first click can
			// create a bookmark and so hover previews work before any glyph has been drawn.
			var bounds = new Rect(Bounds.Size);
			drawingContext.DrawRectangle(GetBrush("ILSpy.BookmarkGutterBackground", FallbackBackground), null, bounds);
			var border = GetBrush("ILSpy.BookmarkGutterBorder", FallbackBorder);
			double x = Math.Max(0, Bounds.Width - 0.5);
			drawingContext.DrawLine(new Pen(border, 1), new Point(x, 0), new Point(x, Bounds.Height));
		}

		IBrush GetBrush(string key, IBrush fallback)
		{
			return this.TryFindResource(key, ActualThemeVariant, out var resource) && resource is IBrush brush
				? brush
				: fallback;
		}

		protected override void OnPointerPressed(PointerPressedEventArgs e)
		{
			base.OnPointerPressed(e);
			var textView = TextView;
			if (e.Handled || textView == null)
				return;
			// Only the left button toggles. A right- or middle-click in the gutter must not
			// add or remove a bookmark: it makes accidental deletion easy and would clash with
			// a future gutter context menu.
			if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
				return;
			double y = e.GetPosition(textView).Y + textView.VerticalOffset;
			var visualLine = textView.GetVisualLineFromVisualTop(y);
			if (visualLine == null)
				return;
			// A removal click leaves the pointer hovering the line it just cleared. Without this, Render
			// would immediately redraw the line's hover-preview glyph, so the click would look like a
			// no-op. Suppress the preview on that line until the pointer leaves it (a jitter that stays
			// on the same line must not bring it back); a fresh hover after leaving shows it again.
			if (!owner.ToggleBookmarkAtLine(visualLine.FirstDocumentLine.LineNumber))
			{
				suppressedHoverLine = visualLine.FirstDocumentLine.LineNumber;
				SetHoverLine(-1);
			}
			InvalidateVisual();
			e.Handled = true;
		}

		protected override void OnPointerMoved(PointerEventArgs e)
		{
			base.OnPointerMoved(e);
			var textView = TextView;
			int newHoverLine = -1;
			if (textView != null)
			{
				double y = e.GetPosition(textView).Y + textView.VerticalOffset;
				var visualLine = textView.GetVisualLineFromVisualTop(y);
				if (visualLine != null && owner.CanToggleBookmarkAtLine(visualLine.FirstDocumentLine.LineNumber))
					newHoverLine = visualLine.FirstDocumentLine.LineNumber;
			}
			if (newHoverLine == suppressedHoverLine)
			{
				// Still on the just-removed line: keep its preview hidden.
				newHoverLine = -1;
			}
			else
			{
				// The pointer moved onto a different line, so normal hover resumes.
				suppressedHoverLine = -1;
			}
			SetHoverLine(newHoverLine);
		}

		protected override void OnPointerExited(PointerEventArgs e)
		{
			base.OnPointerExited(e);
			suppressedHoverLine = -1;
			SetHoverLine(-1);
		}

		void SetHoverLine(int line)
		{
			if (hoverLine == line)
				return;
			hoverLine = line;
			InvalidateVisual();
		}

		// The line currently drawing a hover-preview glyph, or -1 when none. Exposed for tests.
		internal int HoverPreviewLine => hoverLine;

		// The document lines that currently carry a bookmark glyph, resolved against the live document.
		// Exposed for tests because the gutter only paints when rendering is on (off in the headless CI).
		internal IReadOnlyDictionary<int, IImage> GlyphLinesForTest()
			=> Manager is { } m ? GetGlyphByLine(m) : new Dictionary<int, IImage>();
	}
}
