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

using Avalonia;
using Avalonia.Data.Converters;

namespace ICSharpCode.ILSpy.Controls.Omnibar
{
	/// <summary>
	/// Adapters that let a plain horizontal <c>ScrollBar</c> drive and reflect a
	/// <c>ScrollViewer</c>'s scroll state, which Avalonia exposes only as <c>Vector</c>/<c>Size</c>
	/// (<c>Offset</c>, <c>ScrollBarMaximum</c>, <c>Viewport</c>) rather than the scalar
	/// value/maximum/viewport a <c>ScrollBar</c> binds to. Used by the omnibar's overlay scrollbar
	/// so the breadcrumb can scroll horizontally without the Simple theme's reserved scrollbar row.
	/// </summary>
	public static class OmnibarScrollConverters
	{
		/// <summary>
		/// Two-way bridge between a <c>ScrollBar.Value</c> (double) and a <c>ScrollViewer.Offset</c>
		/// (<see cref="Vector"/>): forward yields <c>Offset.X</c>; back rebuilds <c>(value, 0)</c>.
		/// The omnibar disables vertical scrolling, so the Y component is always 0.
		/// </summary>
		public static readonly IValueConverter VectorX = new VectorXConverter();

		/// <summary>Width of a <see cref="Size"/> (e.g. a <c>ScrollViewer.Viewport</c>).</summary>
		public static readonly IValueConverter SizeWidth = new SizeWidthConverter();

		/// <summary>True when a <see cref="Vector"/>'s X is greater than 0 (i.e. content overflows).</summary>
		public static readonly IValueConverter VectorXPositive = new VectorXPositiveConverter();

		sealed class VectorXConverter : IValueConverter
		{
			public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
				=> value is Vector v ? v.X : 0d;

			public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
				=> new Vector(value is double d ? d : 0d, 0d);
		}

		sealed class SizeWidthConverter : IValueConverter
		{
			public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
				=> value is Size s ? s.Width : 0d;

			public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
				=> throw new NotSupportedException();
		}

		sealed class VectorXPositiveConverter : IValueConverter
		{
			public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
				=> value is Vector v && v.X > 0;

			public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
				=> throw new NotSupportedException();
		}
	}
}
