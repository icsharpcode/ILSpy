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
	/// Dark. The whole decision is per-paint: it reads <see cref="ThemeManager.IsDarkTheme"/>
	/// and <see cref="ThemeManager.IsThemeAware"/> directly, so a theme switch -- or a
	/// definition getting registered with the theme manager after this colorizer was
	/// created -- takes effect on the next redraw without needing a second colorizer
	/// instance. Checking theme-awareness per paint matters for correctness, not just
	/// freshness: <see cref="ThemeManager"/> darkens a registered definition's named
	/// colours in place, so remapping them here a second time would wash the palette out.
	/// The remapped colours are cached per source <see cref="HighlightingColor"/>; the
	/// cache key is content-based (HighlightingColor overrides Equals/GetHashCode), so an
	/// in-place recolour of a source colour misses the cache and reconverts instead of
	/// serving a conversion of the old values. The entry keyed by the old content is
	/// stranded rather than replaced -- bounded by the definition's colour count per
	/// recolour, and it dies with the colorizer.
	/// </summary>
	public sealed class ThemeAwareHighlightingColorizer : HighlightingColorizer
	{
		readonly Dictionary<HighlightingColor, HighlightingColor> darkColors = new();
		readonly IHighlightingDefinition definition;

		public ThemeAwareHighlightingColorizer(IHighlightingDefinition highlightingDefinition)
			: base(highlightingDefinition)
		{
			definition = highlightingDefinition;
		}

		protected override void ApplyColorToElement(VisualLineElement element, HighlightingColor color)
		{
			base.ApplyColorToElement(element, GetEffectiveColor(color));
		}

		// The per-paint colour decision, split out so tests can drive it without a TextView.
		internal HighlightingColor GetEffectiveColor(HighlightingColor color)
		{
			if (ThemeManager.Current.IsDarkTheme && !ThemeManager.Current.IsThemeAware(definition))
				return GetCachedDarkColor(color);
			return color;
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
