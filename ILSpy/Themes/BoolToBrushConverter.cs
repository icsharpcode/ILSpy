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

using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace ILSpy.Themes
{
	/// <summary>
	/// Maps a boolean to a configured <see cref="IBrush"/> when true, or <see langword="null"/>
	/// when false / unset. Lets a Style setter resolve to a brush only on the true branch
	/// while leaving the theme default in place otherwise. Used for the preview-tab accent
	/// border stripe.
	/// </summary>
	public sealed class BoolToBrushConverter : IValueConverter
	{
		/// <summary>Static accent for the preview-tab left-edge stripe — matches the
		/// toolbar accent palette (#0078D7).</summary>
		public static readonly BoolToBrushConverter PreviewAccent = new() {
			TrueBrush = new ImmutableSolidColorBrush(Color.FromRgb(0x00, 0x78, 0xD7)),
		};

		public IBrush? TrueBrush { get; init; }

		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> value is bool b && b ? TrueBrush : null;

		public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> global::Avalonia.Data.BindingOperations.DoNothing;
	}
}
