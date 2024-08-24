using System.Collections.Generic;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.ILSpy.Themes;

namespace ICSharpCode.ILSpy.TextView;

#nullable enable

public class ThemeAwareHighlightingColorizer : HighlightingColorizer
{
	private readonly Dictionary<HighlightingColor, HighlightingColor> _darkColors = new();
	private readonly bool _isHighlightingThemeAware;

	public ThemeAwareHighlightingColorizer(IHighlightingDefinition highlightingDefinition)
		: base(highlightingDefinition)
	{
		_isHighlightingThemeAware = ThemeManager.Current.IsThemeAware(highlightingDefinition);
	}

	protected override void ApplyColorToElement(VisualLineElement element, HighlightingColor color)
	{
		if (!_isHighlightingThemeAware && ThemeManager.Current.IsDarkTheme)
		{
			color = GetColorForDarkTheme(color);
		}

		base.ApplyColorToElement(element, color);
	}

	private HighlightingColor GetColorForDarkTheme(HighlightingColor lightColor)
	{
		if (lightColor.Foreground is null && lightColor.Background is null)
		{
			return lightColor;
		}

		if (!_darkColors.TryGetValue(lightColor, out var darkColor))
		{
			_darkColors[lightColor] = darkColor = ThemeManager.GetColorForDarkTheme(lightColor);
		}

		return darkColor;
	}
}
