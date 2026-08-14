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
using System.Runtime.CompilerServices;

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

		/// <summary>
		/// Minimum WCAG contrast ratio a dark-converted FOREGROUND must reach against the surface
		/// it is painted on. 5.5 sits where the hand-authored <see cref="SyntaxColorPalettes.CSharpDark"/>
		/// values already live (5.0-8.8) and matches VS Code's own dark keyword blue (5.65).
		/// The 4.5 WCAG AA threshold is not enough here: it leaves plain Blue at 4.51, which
		/// still reads as too dark against the editor background.
		/// </summary>
		internal const double MinimumDarkContrastRatio = 5.5;

		// The dark editor canvas a foreground is measured against when its colour declares no
		// span background of its own. Mirrors ILSpy.EditorBackground in the Dark theme dictionary
		// of App.axaml, which ThemeManagerTests.DarkEditorBackground_Constant_Tracks_The_Theme_Resource
		// pins it to -- kept as a constant so the conversion stays pure static math, usable
		// without a running Application.
		internal static readonly Color DarkEditorBackground = Color.FromRgb(0x1E, 0x1E, 0x1E);

		static readonly double DarkEditorBackgroundLuminance =
			RelativeLuminance(DarkEditorBackground.R, DarkEditorBackground.G, DarkEditorBackground.B);

		// Highlighting definitions whose named colours we re-theme on every theme switch.
		readonly List<IHighlightingDefinition> themableDefinitions = new();

		// The ORIGINAL (light, .xshd-default) values of every colour ever themed, so switching
		// back to Light restores them exactly. Keyed by colour INSTANCE: ConditionalWeakTable
		// compares by reference (unaffected by HighlightingColor's content-based Equals) and
		// lets snapshots die with their definition. Keying by instance rather than by definition
		// makes theming idempotent across definition identities -- AvaloniaEdit's
		// HighlightingManager hands out a delay-loaded wrapper that forwards to the inner
		// definition ILSpy's load callback registers, so the same colours can arrive here under
		// two definition objects. A per-definition snapshot taken via the second identity would
		// capture already-dark values as "originals" and dark-convert them again (washed-out
		// colours whenever dark is active at first touch); the per-instance snapshot is taken
		// exactly once, before the first rewrite, no matter which identity triggers it.
		readonly ConditionalWeakTable<HighlightingColor, HighlightingColor> originalColors = new();

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
			// Skip definitions that are already theme-aware: either their XSHD declares
			// ILSpy.IsThemeAware (they ship theme-correct colours we must not rewrite), or a
			// previous registration themed them -- typically the inner definition behind the
			// delay-loaded wrapper HighlightingManager returns, which materializes (and
			// registers itself) when the marker is read here. Re-theming would be harmless
			// either way (the per-instance colour snapshots make it idempotent); there is
			// simply nothing to do, and no reason to track a second identity in the list.
			if (IsThemeAware(definition))
				return;
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
		void ApplyHighlightingColors(IHighlightingDefinition definition)
		{
			var darkPalette = definition.Name == "C#" ? SyntaxColorPalettes.CSharpDark : null;
			foreach (var color in definition.NamedHighlightingColors)
			{
				// Snapshot before the first rewrite; every later visit gets the stored
				// pristine values back, never the colour's current (possibly dark) content.
				var original = originalColors.GetValue(color, static c => c.Clone());
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
		/// background, and the foreground is then moved to <see cref="MinimumDarkContrastRatio"/>
		/// against the surface it lands on. Non-colour style attributes (bold/italic/underline)
		/// pass through unchanged. When the input has no colour brushes at all, returns it as-is
		/// so the caller's cache can short-circuit.
		/// </summary>
		/// <remarks>
		/// The contrast guarantee assumes opaque colours: the ratio is computed on the RGB
		/// channels and the source alpha is carried over untouched, so a translucent foreground
		/// composites to less contrast than the floor promises. Every named XSHD colour that
		/// reaches this path today is opaque.
		/// </remarks>
		public static HighlightingColor GetColorForDarkTheme(HighlightingColor lightColor)
		{
			ArgumentNullException.ThrowIfNull(lightColor);
			if (lightColor.Foreground is null && lightColor.Background is null)
				return lightColor;

			var darkColor = (HighlightingColor)lightColor.Clone();
			// The background converts first: when a colour declares one, that -- not the editor
			// canvas -- is the surface its own foreground is painted on, so it is what the
			// foreground's contrast has to be measured against.
			darkColor.Background = AdjustForDarkTheme(darkColor.Background, contrastReference: null);
			darkColor.Foreground = AdjustForDarkTheme(darkColor.Foreground,
				contrastReference: LuminanceOf(darkColor.Background) ?? DarkEditorBackgroundLuminance);
			return darkColor;
		}

		static double? LuminanceOf(HighlightingBrush? brush)
		{
			var color = brush?.GetColor(null!);
			return color is null ? null : RelativeLuminance(color.Value.R, color.Value.G, color.Value.B);
		}

		// contrastReference is the luminance of the surface the colour will be painted on, or
		// null for a colour that IS a surface: backgrounds are left where the inversion put them,
		// or a light XSHD span background would be repainted as a bright block that buries the
		// text drawn on top of it.
		static HighlightingBrush? AdjustForDarkTheme(HighlightingBrush? lightBrush, double? contrastReference)
		{
			if (lightBrush is null)
				return null;
			// AvaloniaEdit's SimpleHighlightingBrush is the only public concrete impl; for
			// anything else (e.g. a gradient/themed brush a future XSHD might supply) we
			// pass through unmodified — guessing colours would be worse than leaving them.
			var color = lightBrush.GetColor(null!);
			if (color is null)
				return lightBrush;
			return new SimpleHighlightingBrush(AdjustForDarkTheme(color.Value, contrastReference));
		}

		static Color AdjustForDarkTheme(Color color, double? contrastReference)
		{
			var (h, s, l) = RgbToHsl(color.R, color.G, color.B);

			// Invert lightness, but lift the floor slightly so the darkest colours don't
			// land right at white -- keeps a sense of relative brightness in the output.
			l = 1f - MathF.Pow(l, 1.2f);

			// Soften intense colours: at full saturation they'd glow against a dark editor
			// background. This has to apply wherever the inverted lightness lands, because a
			// dark, fully saturated source (DarkMagenta) inverts to a *light*, still fully
			// saturated colour -- exactly the neon the softening exists to prevent. The paired
			// lightness lift only makes sense for a softened colour that landed dark, and is
			// deliberately not extended to unsaturated ones: it is not monotone across the 0.75
			// boundary, so neighbouring greys would swap order.
			if (s > 0.75f)
			{
				s *= 0.75f;
				if (l < 0.75f)
					l *= 1.2f;
			}

			// HSL lightness is not perceptual luminance, so inversion alone leaves hues with a
			// low luminance weight (blue above all) too dim to read, and drags an already-light
			// source colour *downwards* into its surface. Hence the contrast floor.
			if (contrastReference is { } reference)
				l = MoveToContrastFloor(h, s, l, reference);

			var (r, g, b) = HslToRgb(h, s, l);
			return Color.FromArgb(color.A, r, g, b);
		}

		/// <summary>
		/// Moves HSL lightness until the colour clears <see cref="MinimumDarkContrastRatio"/>
		/// against a surface of luminance <paramref name="reference"/>. Hue and saturation are
		/// preserved, so the token keeps its identity and only its brightness moves. Luminance
		/// grows monotonically with lightness at a fixed hue/saturation, so the search heads for
		/// whichever end of the lightness range offers more contrast -- white above the dark
		/// canvas, black under a light span background -- and bisects for the smallest move that
		/// clears the floor. If even that end misses the floor it is still the best available.
		/// </summary>
		static float MoveToContrastFloor(float h, float s, float l, double reference)
		{
			if (ContrastAtLightness(h, s, l, reference) >= MinimumDarkContrastRatio)
				return l;

			float target = ContrastAtLightness(h, s, 1f, reference) >= ContrastAtLightness(h, s, 0f, reference)
				? 1f
				: 0f;
			if (ContrastAtLightness(h, s, target, reference) < MinimumDarkContrastRatio)
				return target;

			float near = l;
			for (int i = 0; i < 20; i++)
			{
				float mid = (near + target) / 2f;
				if (ContrastAtLightness(h, s, mid, reference) >= MinimumDarkContrastRatio)
					target = mid;
				else
					near = mid;
			}
			return target;
		}

		static double ContrastAtLightness(float h, float s, float l, double reference)
		{
			var (r, g, b) = HslToRgb(h, s, l);
			var luminance = RelativeLuminance(r, g, b);
			var (brighter, darker) = luminance > reference
				? (luminance, reference)
				: (reference, luminance);
			return (brighter + 0.05) / (darker + 0.05);
		}

		// WCAG 2.x relative luminance over sRGB-linearised channels.
		static double RelativeLuminance(byte r, byte g, byte b)
			=> 0.2126 * Linearize(r) + 0.7152 * Linearize(g) + 0.0722 * Linearize(b);

		static double Linearize(byte channel)
		{
			double v = channel / 255.0;
			return v <= 0.03928 ? v / 12.92 : Math.Pow((v + 0.055) / 1.055, 2.4);
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
