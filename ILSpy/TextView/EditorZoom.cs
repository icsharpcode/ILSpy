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

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// Pure-math helper for editor zoom (font-size scaling). The actual editor wiring
	/// reads/writes <c>DisplaySettings.SelectedFontSize</c>; this class encapsulates
	/// the step calculation and clamping so it's unit-testable without an editor.
	/// </summary>
	public static class EditorZoom
	{
		/// <summary>Multiplicative step per wheel tick / button press.</summary>
		public const double Factor = 1.1;

		/// <summary>Lower bound in points. Below ~8 pt the gutter glyphs collapse.</summary>
		public const double MinFontSize = 8.0;

		/// <summary>Upper bound in points. Above ~72 pt one glyph fills the viewport.</summary>
		public const double MaxFontSize = 72.0;

		/// <summary>Default font size in points, matching <c>DisplaySettings</c>'s initial value.</summary>
		public const double DefaultFontSize = 10.0 * 4 / 3;

		public static double ZoomIn(double currentFontSize)
			=> Clamp(RoundToDefaultIfClose(currentFontSize * Factor));

		public static double ZoomOut(double currentFontSize)
			=> Clamp(RoundToDefaultIfClose(currentFontSize / Factor));

		public static double Reset() => DefaultFontSize;

		/// <summary>Snap to <see cref="DefaultFontSize"/> when within 0.001 pt — avoids
		/// floating-point drift after zoom-in followed by zoom-out leaving the size stuck
		/// at 13.3333000001 instead of the canonical 13.3333333.</summary>
		static double RoundToDefaultIfClose(double size)
			=> Math.Abs(size - DefaultFontSize) < 0.001 ? DefaultFontSize : size;

		static double Clamp(double size) => Math.Max(MinFontSize, Math.Min(MaxFontSize, size));
	}
}
