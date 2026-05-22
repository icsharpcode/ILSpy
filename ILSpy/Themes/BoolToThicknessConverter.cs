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

namespace ILSpy.Themes
{
	/// <summary>
	/// Maps a boolean to a configured <see cref="Thickness"/> when true, or
	/// <see cref="Thickness"/>(0) when false / unset. Used to paint the preview-tab
	/// left-edge accent stripe via a Style setter without overriding the theme's full
	/// border when the tab is pinned.
	/// </summary>
	public sealed class BoolToThicknessConverter : IValueConverter
	{
		/// <summary>3px-wide left edge — matches the accent stripe width VS uses.</summary>
		public static readonly BoolToThicknessConverter LeftStripe = new() {
			TrueThickness = new Thickness(3, 0, 0, 0),
		};

		public Thickness TrueThickness { get; init; }

		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> value is bool b && b ? TrueThickness : default(Thickness);

		public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> global::Avalonia.Data.BindingOperations.DoNothing;
	}
}
