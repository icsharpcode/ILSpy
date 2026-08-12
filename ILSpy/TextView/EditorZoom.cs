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

using ICSharpCode.ILSpy.Options;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// Pure-math helper for editor zoom. Zoom is a multiplier on top of the configured
	/// <see cref="DisplaySettings.SelectedFontSize"/>, kept in
	/// <see cref="DisplaySettings.EditorZoomFactor"/>: picking a different font size in the
	/// options dialog moves the base size without counting as a zoom. The step calculation
	/// and clamping live here so they're unit-testable without an editor.
	/// </summary>
	public static class EditorZoom
	{
		/// <summary>Multiplicative step per wheel tick / button press.</summary>
		public const double Factor = 1.1;

		/// <summary>Lower zoom bound; below 20% the gutter glyphs collapse.</summary>
		public const double MinZoom = 0.2;

		/// <summary>Upper zoom bound; above 500% one glyph fills the viewport.</summary>
		public const double MaxZoom = 5.0;

		/// <summary>Unzoomed state: the configured font size is used as-is.</summary>
		public const double DefaultZoom = 1.0;

		public static double ZoomIn(double currentZoom)
			=> Clamp(RoundToDefaultIfClose(currentZoom * Factor));

		public static double ZoomOut(double currentZoom)
			=> Clamp(RoundToDefaultIfClose(currentZoom / Factor));

		public static double Reset() => DefaultZoom;

		/// <summary>The font size an editor should render at: configured size times zoom.</summary>
		public static double EffectiveFontSize(DisplaySettings settings)
			=> settings.SelectedFontSize * Clamp(settings.EditorZoomFactor);

		/// <summary>Snap to <see cref="DefaultZoom"/> when within 0.001 — avoids floating-point
		/// drift after zoom-in followed by zoom-out leaving the factor stuck at 1.0000000001,
		/// which would keep the zoom overlay visible at an apparent 100%.</summary>
		static double RoundToDefaultIfClose(double zoom)
			=> Math.Abs(zoom - DefaultZoom) < 0.001 ? DefaultZoom : zoom;

		static double Clamp(double zoom) => Math.Max(MinZoom, Math.Min(MaxZoom, zoom));
	}
}
