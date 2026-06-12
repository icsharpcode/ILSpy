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
using System.Globalization;

using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace ICSharpCode.ILSpy.Themes
{
	/// <summary>
	/// Maps a boolean to one of two configured <see cref="IBrush"/>es. The only remaining use is the
	/// active-dock accent line under the document tab strip (Panel#PART_DocumentSeperator), which is
	/// colored by the *active document's* IsPreview -- a dock-level fact that the per-tab
	/// <c>previewTab</c> style class cannot express, so it stays a binding. Every other tab/pane
	/// state is now driven by that style class with plain DynamicResource brushes instead.
	/// </summary>
	public sealed class BoolToBrushConverter : IValueConverter
	{
		/// <summary>The active-dock accent line's fill: PURPLE (#9B59B6) when the active document is
		/// the One, BLUE (#007ACC) otherwise. These two values MUST stay in sync with the
		/// ILSpy.PreviewTabActiveBackground and ILSpy.DockTabItemActiveBackground resources in
		/// App.axaml -- both colors read on light and dark, so they are theme-agnostic here.</summary>
		public static readonly BoolToBrushConverter PreviewOrActiveTabBackground = new() {
			TrueBrush = new ImmutableSolidColorBrush(Color.FromRgb(0x9B, 0x59, 0xB6)),
			FalseBrush = new ImmutableSolidColorBrush(Color.FromRgb(0x00, 0x7A, 0xCC)),
		};

		public IBrush? TrueBrush { get; init; }

		/// <summary>Brush returned on the false branch (default <see langword="null"/>).</summary>
		public IBrush? FalseBrush { get; init; }

		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> value is bool b && b ? TrueBrush : FalseBrush;

		public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> BindingOperations.DoNothing;
	}
}
