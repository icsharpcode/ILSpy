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
using System.Runtime.InteropServices;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

using AvaloniaEdit.Editing;

#pragma warning disable CA1060 // Move pinvokes to native methods class

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
	/// (the same distance horizontally), a page is one viewport. Redundant once AvaloniaEdit derives
	/// the sizes from its own metrics.
	/// </summary>
	public sealed partial class DecompilerTextArea : TextArea, ILogicalScrollable
	{
		// Avalonia resolves the control template via the runtime type; without this the TextArea
		// template (which hosts the TextView and margins) never applies to the subclass.
		protected override Type StyleKeyOverride => typeof(TextArea);

		ILogicalScrollable Inner => TextView;

		Size ILogicalScrollable.ScrollSize {
			get {
				if (wheelScrollLinesSetting == WheelPageScroll)
					return Inner.Viewport;
				double step = WheelScrollLines * TextView.DefaultLineHeight;
				return new Size(step, step);
			}
		}

		Size ILogicalScrollable.PageScrollSize => Inner.Viewport;

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

		// Windows' "one screen at a time" choice for the wheel (WHEEL_PAGESCROLL).
		const uint WheelPageScroll = uint.MaxValue;

		static readonly uint wheelScrollLinesSetting = ReadWheelScrollLines();

		/// <summary>
		/// Lines scrolled per wheel notch: the user's Windows "lines to scroll" setting, 3 (the
		/// Windows default) on other platforms. Avalonia delivers raw wheel notches and does not
		/// expose the system setting itself. Read once; a settings change applies after a restart.
		/// </summary>
		internal static double WheelScrollLines => wheelScrollLinesSetting;

		static uint ReadWheelScrollLines()
		{
			const uint SPI_GETWHEELSCROLLLINES = 0x0068;
			if (OperatingSystem.IsWindows() && SystemParametersInfoW(SPI_GETWHEELSCROLLLINES, 0, out uint lines, 0))
				return lines;
			return 3;
		}

		[LibraryImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static partial bool SystemParametersInfoW(uint uiAction, uint uiParam, out uint pvParam, uint fWinIni);
	}
}
