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

namespace ICSharpCode.ILSpy.Controls.TreeView
{
	/// <summary>Converts a node's <c>Level</c> to the pixel width of its indentation spacer.</summary>
	public sealed class TreeIndentConverter : IValueConverter
	{
		// 18.5 = icon-centre (25) - expander-centre (6.5): one step puts a child's +/- box directly
		// under its parent's icon, so the connector line passes through both.
		public const double IndentStep = 18.5;

		public static readonly TreeIndentConverter Instance = new();

		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
			// Both trees show the root's children as the top level (ShowRoot=false), which are at
			// Level 1 -- so the visible top level sits at indent 0, not one step in.
			=> value is int level ? Math.Max(0, level - 1) * IndentStep : 0d;

		public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> throw new NotSupportedException();
	}
}
