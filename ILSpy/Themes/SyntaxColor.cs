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

using Avalonia.Media;

using AvaloniaEdit.Highlighting;

namespace ICSharpCode.ILSpy.Themes
{
	/// <summary>
	/// A theme's colour override for a single named <see cref="HighlightingColor"/> (ported from
	/// the WPF ILSpy theming model). A theme supplies one per syntax token; <see cref="ApplyTo"/>
	/// writes it onto the shared HighlightingColor instance, so both the .xshd colorizer and the
	/// semantic RichTextModel -- which reference the same instance -- pick up the change at once.
	/// </summary>
	public sealed class SyntaxColor
	{
		public Color? Foreground { get; init; }
		public Color? Background { get; init; }
		public FontWeight? FontWeight { get; init; }
		public FontStyle? FontStyle { get; init; }

		public void ApplyTo(HighlightingColor color)
		{
			color.Foreground = Foreground is { } foreground ? new SimpleHighlightingBrush(foreground) : null;
			color.Background = Background is { } background ? new SimpleHighlightingBrush(background) : null;
			color.FontWeight = FontWeight;
			color.FontStyle = FontStyle;
		}
	}
}
