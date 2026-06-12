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

using System.Collections.Generic;

using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Rendering;

using ICSharpCode.ILSpy.Themes;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// Wraps AvaloniaEdit's default colorizer so colours flip when the active theme is
	/// Dark. The decision is per-paint and reads <see cref="ThemeManager.IsDarkTheme"/>
	/// directly, so a theme switch followed by an editor redraw renders in the new
	/// palette without needing a second colorizer instance. The remapped colours are
	/// cached per source <see cref="HighlightingColor"/> instance.
	/// </summary>
	public sealed class ThemeAwareHighlightingColorizer : HighlightingColorizer
	{
		readonly Dictionary<HighlightingColor, HighlightingColor> darkColors = new();
		readonly bool definitionIsThemeAware;

		public ThemeAwareHighlightingColorizer(IHighlightingDefinition highlightingDefinition)
			: base(highlightingDefinition)
		{
			definitionIsThemeAware = ThemeManager.Current.IsThemeAware(highlightingDefinition);
		}

		protected override void ApplyColorToElement(VisualLineElement element, HighlightingColor color)
		{
			if (!definitionIsThemeAware && ThemeManager.Current.IsDarkTheme)
				color = GetCachedDarkColor(color);
			base.ApplyColorToElement(element, color);
		}

		HighlightingColor GetCachedDarkColor(HighlightingColor lightColor)
		{
			if (lightColor.Foreground is null && lightColor.Background is null)
				return lightColor;
			if (!darkColors.TryGetValue(lightColor, out var darkColor))
			{
				darkColor = ThemeManager.GetColorForDarkTheme(lightColor);
				darkColors[lightColor] = darkColor;
			}
			return darkColor;
		}
	}
}
