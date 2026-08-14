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

// ResourceNodeExtensions.TryFindResource lives in Avalonia.Controls.
using Avalonia.Controls;
using Avalonia.Headless.NUnit;

using AvaloniaEdit.Highlighting;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Themes;

using NSubstitute;

using NUnit.Framework;

// Bare `Avalonia` would resolve to ICSharpCode.ILSpy.Tests.* via lexical namespace
// lookup. global:: keeps the references unambiguous.
using AvColor = global::Avalonia.Media.Color;
using AvColors = global::Avalonia.Media.Colors;
using AvFontStyle = global::Avalonia.Media.FontStyle;
using AvFontWeight = global::Avalonia.Media.FontWeight;

namespace ICSharpCode.ILSpy.Tests.Themes;

/// <summary>
/// Unit coverage for the theme-color machinery that backs
/// <c>ThemeAwareHighlightingColorizer</c>: the XSHD-marker check and the light→dark
/// brush remapper. Live colorizer behaviour against a real editor is covered separately
/// in the integration tests.
/// </summary>
[TestFixture]
public class ThemeManagerTests
{
	[Test]
	public void IsThemeAware_Returns_True_When_Definition_Carries_The_ILSpy_Marker_Property()
	{
		// XSHD authors mark a definition theme-aware by emitting
		//   <Property name="ILSpy.IsThemeAware" value="True" />
		// near the document root. The marker tells the colorizer to leave colours alone
		// even under a dark theme — "this definition already ships theme-correct".
		var def = StubDefinition(("ILSpy.IsThemeAware", "True"));

		ThemeManager.Current.IsThemeAware(def).Should().BeTrue();
	}

	[Test]
	public void IsThemeAware_Returns_False_When_Marker_Property_Is_Absent()
	{
		var def = StubDefinition();

		ThemeManager.Current.IsThemeAware(def).Should().BeFalse();
	}

	[Test]
	public void IsThemeAware_Returns_False_When_Marker_Value_Is_Not_True()
	{
		// Case-sensitive: only the exact string "True" enables the marker; anything else
		// (including "true" or "1") behaves like an absent marker.
		var def = StubDefinition(("ILSpy.IsThemeAware", "false"));

		ThemeManager.Current.IsThemeAware(def).Should().BeFalse();
	}

	[Test]
	public void GetColorForDarkTheme_Returns_Identity_When_Both_Brushes_Are_Null()
	{
		// Some highlighting colours describe italic/bold-only spans with no colour swap —
		// the dark remapper has nothing to flip, so the same instance comes back.
		var color = new HighlightingColor { FontWeight = AvFontWeight.Bold };

		var remapped = ThemeManager.GetColorForDarkTheme(color);

		remapped.Should().BeSameAs(color);
	}

	[Test]
	public void GetColorForDarkTheme_Inverts_Lightness_Of_A_Black_Foreground_To_Near_White()
	{
		// Default C# keyword colour ~= black on the light theme. Under the dark remapper
		// it must become a light colour so it's actually readable against a dark editor
		// background. We allow a wide tolerance: the exact value depends on the HSL
		// curve constants; what matters is that the perceived brightness has flipped.
		var black = new HighlightingColor { Foreground = new SimpleHighlightingBrush(AvColors.Black) };

		var remapped = ThemeManager.GetColorForDarkTheme(black);

		var fg = ColorOf(remapped.Foreground!);
		fg.R.Should().BeGreaterThan(200, "black-on-light must remap to near-white on dark");
		fg.G.Should().BeGreaterThan(200);
		fg.B.Should().BeGreaterThan(200);
	}

	[Test]
	public void GetColorForDarkTheme_Returns_A_Clone_Not_The_Original_Instance()
	{
		// The light + dark colour caches in the colorizer rely on the remapper not
		// mutating the source HighlightingColor — flipping a shared instance in place
		// would corrupt every other colorizer that already cached the light value.
		var color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(AvColors.Red) };

		var remapped = ThemeManager.GetColorForDarkTheme(color);

		remapped.Should().NotBeSameAs(color);
		ColorOf(color.Foreground!).Should().Be(AvColors.Red, "original must be untouched");
	}

	[Test]
	public void GetColorForDarkTheme_Preserves_Non_Colour_Style_Attributes()
	{
		// Bold / italic / underline are theme-neutral — the dark remapper must carry them
		// across into the cloned colour unchanged.
		var color = new HighlightingColor {
			Foreground = new SimpleHighlightingBrush(AvColors.Green),
			FontWeight = AvFontWeight.Bold,
			FontStyle = AvFontStyle.Italic,
		};

		var remapped = ThemeManager.GetColorForDarkTheme(color);

		remapped.FontWeight.Should().Be((AvFontWeight?)AvFontWeight.Bold);
		remapped.FontStyle.Should().Be((AvFontStyle?)AvFontStyle.Italic);
	}

	// Every distinct *named* foreground shipped by the repo's own XSHDs (ILSpy/TextView/*.xshd).
	// Colours declared inline on a rule are anonymous, never land in NamedHighlightingColors and
	// so are never converted at all. None of the definitions declares ILSpy.IsThemeAware and only
	// "C#" has a hand-authored dark palette, so all of these reach the algorithmic conversion.
	static readonly string[] ShippedXshdForegrounds = {
		// XML-Mode.xshd
		"Green", "Blue", "DarkMagenta", "Red", "Teal", "Olive",
		// ILAsm-Mode.xshd
		"Magenta",
		// Asm-Mode.xshd
		"Orange", "#0080C0", "Brown", "#8080FF", "DarkBlue",
	};

	[Test]
	[TestCaseSource(nameof(ShippedXshdForegrounds))]
	public void GetColorForDarkTheme_Lifts_Every_Shipped_Foreground_To_The_Contrast_Floor(string light)
	{
		// The reported symptom in #3986: XSHD colours surviving the HSL inversion with too
		// little perceptual contrast against the dark editor canvas. HSL lightness is not
		// luminance, so without a floor the hue decides the outcome: plain Blue lands at
		// 4.08:1, and the already-light #8080FF inverts *downwards* to 1.29:1 (invisible).
		var color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(AvColor.Parse(light)) };

		var remapped = ThemeManager.GetColorForDarkTheme(color);

		ContrastAgainstDarkEditor(ColorOf(remapped.Foreground!))
			.Should().BeGreaterThanOrEqualTo(ThemeManager.MinimumDarkContrastRatio,
				$"'{light}' must stay readable on the dark editor background");
	}

	[Test]
	public void GetColorForDarkTheme_Leaves_Backgrounds_Below_The_Contrast_Floor()
	{
		// The floor is a *foreground* rule. Asm-Mode.xshd gives the Registers token a light
		// #EEEEEE background; forcing that to contrast with the canvas would repaint it as a
		// bright block and bury the foreground drawn on top of it.
		var color = new HighlightingColor { Background = new SimpleHighlightingBrush(AvColor.Parse("#EEEEEE")) };

		var remapped = ThemeManager.GetColorForDarkTheme(color);

		ContrastAgainstDarkEditor(ColorOf(remapped.Background!))
			.Should().BeLessThan(ThemeManager.MinimumDarkContrastRatio);
	}

	[Test]
	public void GetColorForDarkTheme_Softens_Saturation_Of_Colors_That_Invert_To_Light()
	{
		// DarkMagenta inverts to lightness ~0.79. Saturation softening has to apply wherever the
		// inverted lightness lands, or a dark fully saturated source comes back as light and
		// still fully saturated: neon #FF93FF, exactly what the softening exists to prevent.
		var color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(AvColors.DarkMagenta) };

		var remapped = ThemeManager.GetColorForDarkTheme(color);

		var fg = ColorOf(remapped.Foreground!);
		Math.Max(fg.R, Math.Max(fg.G, fg.B)).Should().BeLessThan(255,
			"a fully saturated channel means the softening was skipped");
	}

	[Test]
	public void GetColorForDarkTheme_Measures_The_Foreground_Against_Its_Own_Span_Background()
	{
		// A colour that carries both is painted on its own background, not on the editor canvas.
		// White-on-Navy is the pathological case: both invert across the canvas, so measuring the
		// foreground against the canvas would push it to a mid grey sitting on a light blue block.
		var color = new HighlightingColor {
			Foreground = new SimpleHighlightingBrush(AvColors.White),
			Background = new SimpleHighlightingBrush(AvColors.Navy),
		};

		var remapped = ThemeManager.GetColorForDarkTheme(color);

		Contrast(ColorOf(remapped.Foreground!), ColorOf(remapped.Background!))
			.Should().BeGreaterThanOrEqualTo(ThemeManager.MinimumDarkContrastRatio,
				"the span background is the surface the foreground has to read against");
	}

	[Test]
	public void GetColorForDarkTheme_Keeps_Neighboring_Greys_In_Order()
	{
		// The lightness lift that pairs with the saturation softening is not monotone across its
		// 0.75 boundary, so it must stay scoped to over-saturated colours. Applied to greys it
		// would map #505050 brighter than the lighter #525252.
		var darker = ColorOf(ThemeManager.GetColorForDarkTheme(Foreground("#505050")).Foreground!);
		var lighter = ColorOf(ThemeManager.GetColorForDarkTheme(Foreground("#525252")).Foreground!);

		darker.R.Should().BeGreaterThanOrEqualTo(lighter.R,
			"inversion has to preserve the ordering of the source colours");
	}

	[AvaloniaTest]
	public void DarkEditorBackground_Constant_Tracks_The_Theme_Resource()
	{
		// The floor is measured against a constant copy of the dark canvas so the conversion
		// stays pure static math. Retuning ILSpy.EditorBackground in App.axaml without updating
		// the constant would leave every floor test green while the shipped colours drift below
		// the real floor -- Blue clears it by 0.02.
		var window = new global::Avalonia.Controls.Window();

		window.TryFindResource("ILSpy.EditorBackground", global::Avalonia.Styling.ThemeVariant.Dark, out var resource)
			.Should().BeTrue("the dark theme dictionary defines the editor canvas");
		(resource as global::Avalonia.Media.ISolidColorBrush)?.Color
			.Should().Be(ThemeManager.DarkEditorBackground);
	}

	static HighlightingColor Foreground(string color)
		=> new() { Foreground = new SimpleHighlightingBrush(AvColor.Parse(color)) };

	static double ContrastAgainstDarkEditor(AvColor color)
		=> Contrast(color, ThemeManager.DarkEditorBackground);

	// WCAG relative-luminance contrast, computed independently of the production code so the
	// tests don't validate themselves.
	static double Contrast(AvColor color, AvColor surface)
	{
		var (a, b) = (Luminance(color) + 0.05, Luminance(surface) + 0.05);
		return a > b ? a / b : b / a;

		static double Luminance(AvColor c)
			=> 0.2126 * Channel(c.R) + 0.7152 * Channel(c.G) + 0.0722 * Channel(c.B);

		static double Channel(byte value)
		{
			var v = value / 255.0;
			return v <= 0.03928 ? v / 12.92 : Math.Pow((v + 0.055) / 1.055, 2.4);
		}
	}

	static AvColor ColorOf(HighlightingBrush brush)
		=> brush.GetColor(null!) ?? throw new InvalidOperationException("brush has no color");

	static IHighlightingDefinition StubDefinition(params (string Key, string Value)[] properties)
	{
		var def = Substitute.For<IHighlightingDefinition>();
		var dict = new Dictionary<string, string>();
		foreach (var (k, v) in properties)
			dict[k] = v;
		def.Properties.Returns(dict);
		return def;
	}
}
