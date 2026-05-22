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

namespace ILSpy.Themes
{
	/// <summary>
	/// Maps a boolean to <see cref="FontStyle.Italic"/> (when true) or
	/// <see cref="FontStyle.Normal"/> (when false, null, or unset). Used in the VS-style
	/// preview-tab styling — the document tab header binds its <c>FontStyle</c> to the
	/// underlying <c>ContentTabPage.IsPreview</c> through this converter so the active
	/// preview tab renders in italics. Unset/non-boolean values fall back to Normal so
	/// non-ContentTabPage dockables (tool panes, …) aren't accidentally italicised.
	/// </summary>
	public sealed class BoolToFontStyleConverter : IValueConverter
	{
		/// <summary>Shared instance for the italic mapping (<see langword="true"/> →
		/// <see cref="FontStyle.Italic"/>).</summary>
		public static readonly BoolToFontStyleConverter Italic = new();

		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> value is bool b && b ? FontStyle.Italic : FontStyle.Normal;

		public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> global::Avalonia.Data.BindingOperations.DoNothing;
	}
}
