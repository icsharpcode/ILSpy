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
using System.ComponentModel;

using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;

using AvaloniaEdit.Highlighting;

namespace ILSpy.Themes
{
	/// <summary>
	/// Light/Dark switcher backed by Avalonia's <see cref="Application.RequestedThemeVariant"/>.
	/// The full WPF theme set (R#, VS Code +/- variants) hasn't been ported yet — this is a
	/// minimal implementation so the View > Theme submenu does something useful today.
	/// </summary>
	public sealed class ThemeManager
	{
		// XSHD property marker that opts a highlighting definition out of the dark-theme
		// remap — used by definitions that already ship theme-correct (e.g. an XSHD that
		// declares its own dark palette in two variants).
		const string IsThemeAwareKey = "ILSpy.IsThemeAware";

		public static ThemeManager Current { get; } = new();

		public string DefaultTheme => "Light";

		public static IReadOnlyCollection<string> AllThemes => new[] {
			"Light",
			"Dark",
		};

		public string? Theme { get; private set; }

		public bool IsDarkTheme => Theme == "Dark";

		/// <summary>
		/// Raised after <see cref="Theme"/> changes. Consumers (chiefly the decompiler text
		/// editor) re-render to pick up the new colour palette — Avalonia's
		/// <c>RequestedThemeVariant</c> swap doesn't itself force AvaloniaEdit to redraw.
		/// </summary>
		public event EventHandler? ThemeChanged;

		ThemeManager()
		{
		}

		/// <summary>
		/// Wires this manager to a <see cref="SessionSettings"/> instance: applies the saved
		/// theme immediately and re-applies whenever Theme changes.
		/// </summary>
		public void Attach(SessionSettings settings)
		{
			UpdateTheme(settings.Theme);
			settings.PropertyChanged += OnSettingsChanged;

			void OnSettingsChanged(object? sender, PropertyChangedEventArgs e)
			{
				if (e.PropertyName == nameof(SessionSettings.Theme))
					UpdateTheme(settings.Theme);
			}
		}

		void UpdateTheme(string? themeName)
		{
			Theme = themeName ?? DefaultTheme;
			if (Application.Current is { } app)
			{
				app.RequestedThemeVariant = Theme == "Dark" ? ThemeVariant.Dark : ThemeVariant.Light;
			}
			ThemeChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Reads the XSHD's <c>ILSpy.IsThemeAware</c> property to decide whether the
		/// definition opts out of the dark-theme colour remap. Case-sensitive: only the
		/// literal string <c>"True"</c> opts in, mirroring WPF.
		/// </summary>
		public bool IsThemeAware(IHighlightingDefinition highlightingDefinition)
		{
			ArgumentNullException.ThrowIfNull(highlightingDefinition);
			return highlightingDefinition.Properties.TryGetValue(IsThemeAwareKey, out var value)
				&& value == bool.TrueString;
		}

		/// <summary>
		/// Clones <paramref name="lightColor"/> with its foreground/background brushes flipped
		/// for a dark-theme background. Lightness inverts with a small curve adjustment;
		/// over-saturated colours are softened so they don't burn through the dark editor
		/// background. Non-colour style attributes (bold/italic/underline) pass through
		/// unchanged. When the input has no colour brushes at all, returns it as-is so the
		/// caller's cache can short-circuit.
		/// </summary>
		public static HighlightingColor GetColorForDarkTheme(HighlightingColor lightColor)
		{
			ArgumentNullException.ThrowIfNull(lightColor);
			if (lightColor.Foreground is null && lightColor.Background is null)
				return lightColor;

			var darkColor = (HighlightingColor)lightColor.Clone();
			darkColor.Foreground = AdjustForDarkTheme(darkColor.Foreground);
			darkColor.Background = AdjustForDarkTheme(darkColor.Background);
			return darkColor;
		}

		static HighlightingBrush? AdjustForDarkTheme(HighlightingBrush? lightBrush)
		{
			if (lightBrush is null)
				return null;
			// AvaloniaEdit's SimpleHighlightingBrush is the only public concrete impl; for
			// anything else (e.g. a gradient/themed brush a future XSHD might supply) we
			// pass through unmodified — guessing colours would be worse than leaving them.
			var color = lightBrush.GetColor(null!);
			if (color is null)
				return lightBrush;
			return new SimpleHighlightingBrush(AdjustForDarkTheme(color.Value));
		}

		static Color AdjustForDarkTheme(Color color)
		{
			var (h, s, l) = RgbToHsl(color.R, color.G, color.B);

			// Invert lightness, but lift the floor slightly so the darkest colours don't
			// land right at white — keeps a sense of relative brightness in the output.
			l = 1f - MathF.Pow(l, 1.2f);

			// Desaturate intense colours — at full saturation they'd otherwise glow against
			// a dark editor background.
			if (s > 0.75f && l < 0.75f)
			{
				s *= 0.75f;
				l *= 1.2f;
			}

			var (r, g, b) = HslToRgb(h, s, l);
			return Color.FromArgb(color.A, r, g, b);
		}

		static (float h, float s, float l) RgbToHsl(byte rByte, byte gByte, byte bByte)
		{
			float r = rByte / 255f, g = gByte / 255f, b = bByte / 255f;
			float max = MathF.Max(r, MathF.Max(g, b));
			float min = MathF.Min(r, MathF.Min(g, b));
			float l = (max + min) / 2f;
			float h, s;
			if (max == min)
			{
				h = 0f;
				s = 0f;
			}
			else
			{
				float d = max - min;
				s = l > 0.5f ? d / (2f - max - min) : d / (max + min);
				if (max == r)
					h = (g - b) / d + (g < b ? 6f : 0f);
				else if (max == g)
					h = (b - r) / d + 2f;
				else
					h = (r - g) / d + 4f;
				h *= 60f;
			}
			return (h, s, l);
		}

		static (byte r, byte g, byte b) HslToRgb(float h, float s, float l)
		{
			// https://en.wikipedia.org/wiki/HSL_and_HSV#HSL_to_RGB
			float c = (1f - MathF.Abs(2f * l - 1f)) * s;
			h = h % 360f / 60f;
			float x = c * (1f - MathF.Abs(h % 2f - 1f));
			var (r1, g1, b1) = (int)MathF.Floor(h) switch {
				0 => (c, x, 0f),
				1 => (x, c, 0f),
				2 => (0f, c, x),
				3 => (0f, x, c),
				4 => (x, 0f, c),
				_ => (c, 0f, x),
			};
			float m = l - c / 2f;
			byte r = ClampToByte((r1 + m) * 255f);
			byte g = ClampToByte((g1 + m) * 255f);
			byte b = ClampToByte((b1 + m) * 255f);
			return (r, g, b);
		}

		static byte ClampToByte(float v) => (byte)Math.Clamp(v, 0f, 255f);
	}
}
