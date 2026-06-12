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

namespace ICSharpCode.ILSpy.Themes
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

		// Highlighting definitions whose named colours we re-theme on every theme switch, plus a
		// snapshot of each definition's ORIGINAL (light, .xshd-default) colours so switching back
		// to Light restores them exactly. Keyed by colour name within each definition.
		readonly List<IHighlightingDefinition> themableDefinitions = new();
		readonly Dictionary<IHighlightingDefinition, Dictionary<string, HighlightingColor>> originalColors = new();

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
			// Re-theme the syntax colours BEFORE notifying the editors, so their Redraw on
			// ThemeChanged repaints against the new palette.
			foreach (var definition in themableDefinitions)
				ApplyHighlightingColors(definition);
			ThemeChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Registers a highlighting definition for theme-aware colouring and applies the current
		/// theme to it immediately. Called by <c>HighlightingService</c> when a definition is first
		/// loaded; from then on the definition is re-themed on every theme switch.
		/// </summary>
		public void RegisterThemableDefinition(IHighlightingDefinition definition)
		{
			ArgumentNullException.ThrowIfNull(definition);
			if (!themableDefinitions.Contains(definition))
				themableDefinitions.Add(definition);
			ApplyHighlightingColors(definition);
		}

		/// <summary>
		/// Writes the active theme's colours onto a definition's named <see cref="HighlightingColor"/>
		/// instances IN PLACE -- the same instances the semantic RichTextModel references, so the
		/// decompiled output and the .xshd colorizer both pick up the change. Light restores the
		/// original .xshd colours; Dark applies the hand-authored palette where one exists and the
		/// algorithmic conversion elsewhere. Marks the definition theme-aware so the per-paint
		/// colorizer doesn't additionally remap it.
		/// </summary>
		public void ApplyHighlightingColors(IHighlightingDefinition definition)
		{
			ArgumentNullException.ThrowIfNull(definition);
			if (!originalColors.TryGetValue(definition, out var snapshot))
			{
				snapshot = new Dictionary<string, HighlightingColor>();
				foreach (var color in definition.NamedHighlightingColors)
					snapshot[color.Name] = color.Clone();
				originalColors[definition] = snapshot;
			}

			var darkPalette = definition.Name == "C#" ? SyntaxColorPalettes.CSharpDark : null;
			foreach (var color in definition.NamedHighlightingColors)
			{
				if (!snapshot.TryGetValue(color.Name, out var original))
					continue;
				if (IsDarkTheme)
				{
					if (darkPalette is not null && darkPalette.TryGetValue(color.Name, out var syntaxColor))
						syntaxColor.ApplyTo(color);
					else
						CopyColor(GetColorForDarkTheme(original), color);
				}
				else
				{
					CopyColor(original, color);
				}
			}

			definition.Properties[IsThemeAwareKey] = bool.TrueString;
		}

		// Copies colour/style fields from one HighlightingColor onto another. Used instead of
		// swapping instances because the RichTextModel holds references to the targets.
		static void CopyColor(HighlightingColor source, HighlightingColor target)
		{
			target.Foreground = source.Foreground;
			target.Background = source.Background;
			target.FontWeight = source.FontWeight;
			target.FontStyle = source.FontStyle;
			target.Underline = source.Underline;
			target.Strikethrough = source.Strikethrough;
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
