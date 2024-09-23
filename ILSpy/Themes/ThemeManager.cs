// Copyright (c) 2021 Tom Englert
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

#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Highlighting;

namespace ICSharpCode.ILSpy.Themes
{
	public class ThemeManager
	{
		private const string _isThemeAwareKey = "ILSpy.IsThemeAware";

		private string? _theme;
		private readonly ResourceDictionary _themeDictionaryContainer = new();
		private readonly Dictionary<string, SyntaxColor> _syntaxColors = new();

		public static readonly ThemeManager Current = new();

		private ThemeManager()
		{
			Application.Current.Resources.MergedDictionaries.Add(_themeDictionaryContainer);
			MessageBus<SettingsChangedEventArgs>.Subscribers += (sender, e) => Settings_Changed(sender, e);
		}

		public string DefaultTheme => "Light";

		public bool IsDarkTheme { get; private set; }

		public static IReadOnlyCollection<string> AllThemes => new[] {
			"Light",
			"Dark",
			"VS Code Light+",
			"VS Code Dark+",
			"R# Light",
			"R# Dark"
		};

		public string? Theme {
			get => _theme;
			set => UpdateTheme(value);
		}

		public Button CreateButton()
		{
			return new Button {
				Style = CreateButtonStyle()
			};
		}

		public Style CreateButtonStyle()
		{
			return new Style(typeof(Button), (Style)Application.Current.FindResource(typeof(Button)));
		}

		public Style CreateToolBarButtonStyle()
		{
			return new Style(typeof(Button), (Style)Application.Current.FindResource(ToolBar.ButtonStyleKey));
		}

		public void ApplyHighlightingColors(IHighlightingDefinition highlightingDefinition)
		{
			// Make sure all color values are taken from the theme
			foreach (var color in highlightingDefinition.NamedHighlightingColors)
				SyntaxColor.ResetColor(color);

			var prefix = $"SyntaxColor.{highlightingDefinition.Name}.";

			foreach (var (key, syntaxColor) in _syntaxColors)
			{
				var color = highlightingDefinition.GetNamedColor(key.Substring(prefix.Length));
				if (color is not null)
					syntaxColor.ApplyTo(color);
			}

			highlightingDefinition.Properties[_isThemeAwareKey] = bool.TrueString;
		}

		public bool IsThemeAware(IHighlightingDefinition highlightingDefinition)
		{
			return highlightingDefinition.Properties.TryGetValue(_isThemeAwareKey, out var value) && value == bool.TrueString;
		}

		private void UpdateTheme(string? themeName)
		{
			_theme = themeName ?? DefaultTheme;
			if (!AllThemes.Contains(_theme))
				_theme = DefaultTheme;

			var themeFileName = _theme
				.Replace("+", "Plus")
				.Replace("#", "Sharp")
				.Replace(" ", "");

			_themeDictionaryContainer.MergedDictionaries.Clear();
			_syntaxColors.Clear();

			// Load SyntaxColor info from theme XAML
			var resourceDictionary = new ResourceDictionary { Source = new Uri($"/themes/Theme.{themeFileName}.xaml", UriKind.Relative) };
			_themeDictionaryContainer.MergedDictionaries.Add(resourceDictionary);

			IsDarkTheme = resourceDictionary[ResourceKeys.TextBackgroundBrush] is SolidColorBrush { Color: { R: < 128, G: < 128, B: < 128 } };

			// Iterate over keys first, because we don't want to instantiate all values eagerly, if we don't need them.
			foreach (var item in resourceDictionary.Keys)
			{
				if (item is string key && key.StartsWith("SyntaxColor.", StringComparison.Ordinal))
				{
					if (resourceDictionary[key] is SyntaxColor syntaxColor)
						_syntaxColors.TryAdd(key, syntaxColor);
				}
			}
		}

		public static HighlightingColor GetColorForDarkTheme(HighlightingColor lightColor)
		{
			if (lightColor.Foreground is null && lightColor.Background is null)
			{
				return lightColor;
			}

			var darkColor = lightColor.Clone();
			darkColor.Foreground = AdjustForDarkTheme(darkColor.Foreground);
			darkColor.Background = AdjustForDarkTheme(darkColor.Background);

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

		private void Settings_Changed(object? sender, PropertyChangedEventArgs e)
		{
			if (sender is not SessionSettings settings || e.PropertyName != nameof(SessionSettings.Theme))
				return;

			Theme = settings.Theme;
		}
	}
}
