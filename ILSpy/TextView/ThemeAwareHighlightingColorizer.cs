using System;
using System.Collections.Generic;
using System.Windows.Media;

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
			darkColor = lightColor.Clone();
			darkColor.Foreground = AdjustForDarkTheme(darkColor.Foreground);
			darkColor.Background = AdjustForDarkTheme(darkColor.Background);

			_darkColors[lightColor] = darkColor;
		}

		return darkColor;
	}

	private static HighlightingBrush? AdjustForDarkTheme(HighlightingBrush? lightBrush)
	{
		if (lightBrush is SimpleHighlightingBrush simpleBrush && simpleBrush.GetBrush(null) is SolidColorBrush brush)
		{
			return new SimpleHighlightingBrush(AdjustForDarkTheme(brush.Color));
		}

		return lightBrush;
	}

	private static Color AdjustForDarkTheme(Color color)
	{
		var c = System.Drawing.Color.FromArgb(color.R, color.G, color.B);
		var (h, s, l) = (c.GetHue(), c.GetSaturation(), c.GetBrightness());

		// Invert the lightness, but also increase it a bit
		l = 1f - MathF.Pow(l, 1.2f);

		// Desaturate the colors, as they'd be too intense otherwise
		if (s > 0.75f && l < 0.75f)
		{
			s *= 0.75f;
			l *= 1.2f;
		}

		var (r, g, b) = HslToRgb(h, s, l);
		return Color.FromArgb(color.A, r, g, b);
	}

	private static (byte r, byte g, byte b) HslToRgb(float h, float s, float l)
	{
		// https://en.wikipedia.org/wiki/HSL_and_HSV#HSL_to_RGB

		var c = (1f - Math.Abs(2f * l - 1f)) * s;
		h = h % 360f / 60f;
		var x = c * (1f - Math.Abs(h % 2f - 1f));

		var (r1, g1, b1) = (int)Math.Floor(h) switch {
			0 => (c, x, 0f),
			1 => (x, c, 0f),
			2 => (0f, c, x),
			3 => (0f, x, c),
			4 => (x, 0f, c),
			_ => (c, 0f, x)
		};

		var m = l - c / 2f;
		var r = (byte)((r1 + m) * 255f);
		var g = (byte)((g1 + m) * 255f);
		var b = (byte)((b1 + m) * 255f);
		return (r, g, b);
	}
}
