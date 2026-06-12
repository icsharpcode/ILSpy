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
