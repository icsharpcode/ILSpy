// Copyright (c) 2026 Christoph Wille
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
using Avalonia.Controls.Primitives;
using Avalonia.Input;

using AvaloniaEdit.Editing;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// <see cref="TextArea"/> whose scroll increments follow the text and the viewport instead of
	/// AvaloniaEdit's fixed pixel constants. AvaloniaEdit's TextView reports a 10x50 px
	/// ScrollSize and a 10x100 px PageScrollSize, and Avalonia's ScrollViewer turns those into the
	/// wheel step, the scrollbar arrow step (SmallChange) and the track-click step (LargeChange):
	/// Shift+wheel crawled 10 px per notch and a track click barely beat an arrow click.
	/// TextArea implements <see cref="ILogicalScrollable"/> by forwarding to its TextView, so this
	/// subclass re-implements the interface: everything still forwards to the TextView except the two
	/// step sizes, which follow WPF AvalonEdit -- a wheel notch moves the system's "lines to scroll"
	/// (the same distance horizontally), a page is one viewport. The desktop's wheel settings come
	/// from <see cref="SystemScrollSettings"/>. Redundant once AvaloniaEdit derives the sizes from
	/// its own metrics.
	/// </summary>
	public sealed class DecompilerTextArea : TextArea, ILogicalScrollable
	{
		// Avalonia resolves the control template via the runtime type; without this the TextArea
		// template (which hosts the TextView and margins) never applies to the subclass.
		protected override Type StyleKeyOverride => typeof(TextArea);

		ILogicalScrollable Inner => TextView;

		// Avalonia.Native divides trackpad deltas by 50 px and mouse-wheel deltas by 5 AppKit line
		// units, and the events carry no precision flag, so one step size has to serve both devices.
		// 50 px keeps trackpads 1:1 at every font size and zoom level (Apple: scroll precise deltas
		// by the returned amount) and gives mice the speed every other Avalonia application has;
		// following the font instead would make trackpads drift with the zoom.
		const double MacOSWheelStep = 50;

		Size ILogicalScrollable.ScrollSize {
			get {
				if (SystemScrollSettings.IsMacOS)
					return new Size(MacOSWheelStep, MacOSWheelStep);
				if (SystemScrollSettings.WheelScrollsOnePage)
					return Inner.Viewport;
				double step = SystemScrollSettings.WheelScrollLines * TextView.DefaultLineHeight;
				return new Size(step, step);
			}
		}

		Size ILogicalScrollable.PageScrollSize {
			get {
				var viewport = Inner.Viewport;
				if (SystemScrollSettings.IsWindows)
					return viewport;
				// AppKit (NSScrollView.pageScroll) and GTK (page_increment = 0.9 page) keep part of the
				// previous page visible after a page scroll, and the macOS HIG suggests one line of
				// overlap. Windows pages by the full viewport, as WPF did.
				double overlap = TextView.DefaultLineHeight;
				return new Size(Math.Max(0, viewport.Width - overlap), Math.Max(0, viewport.Height - overlap));
			}
		}

		bool ILogicalScrollable.IsLogicalScrollEnabled => Inner.IsLogicalScrollEnabled;
		Size IScrollable.Extent => Inner.Extent;
		Size IScrollable.Viewport => Inner.Viewport;
		Vector IScrollable.Offset { get => Inner.Offset; set => Inner.Offset = value; }
		bool IScrollable.CanHorizontallyScroll => Inner.CanHorizontallyScroll;
		bool IScrollable.CanVerticallyScroll => Inner.CanVerticallyScroll;
		bool ILogicalScrollable.CanHorizontallyScroll { get => Inner.CanHorizontallyScroll; set => Inner.CanHorizontallyScroll = value; }
		bool ILogicalScrollable.CanVerticallyScroll { get => Inner.CanVerticallyScroll; set => Inner.CanVerticallyScroll = value; }
		bool ILogicalScrollable.BringIntoView(Control target, Rect targetRect) => Inner.BringIntoView(target, targetRect);
		Control? ILogicalScrollable.GetControlInDirection(NavigationDirection direction, Control? from) => Inner.GetControlInDirection(direction, from);

		event EventHandler? ILogicalScrollable.ScrollInvalidated {
			add => Inner.ScrollInvalidated += value;
			remove => Inner.ScrollInvalidated -= value;
		}
	}
}
