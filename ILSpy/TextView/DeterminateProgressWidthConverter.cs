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
using System.Globalization;

using Avalonia.Data.Converters;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// Width for the determinate progress bar in the decompile/operation overlay: a fixed fraction
	/// (75%) of the overlay width, so the bar is a stable size instead of shrinking and growing with
	/// the status text underneath it. While the bar is indeterminate it returns <c>NaN</c> (auto),
	/// leaving that mode's natural sizing untouched.
	/// </summary>
	public sealed class DeterminateProgressWidthConverter : IMultiValueConverter
	{
		public static readonly DeterminateProgressWidthConverter Instance = new();

		const double Fraction = 0.75;

		public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
		{
			if (values.Count == 2 && values[0] is double overlayWidth && values[1] is bool isIndeterminate
				&& !isIndeterminate && overlayWidth > 0)
			{
				return overlayWidth * Fraction;
			}
			return double.NaN;
		}
	}
}
