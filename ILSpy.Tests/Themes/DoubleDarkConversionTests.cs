// Copyright (c) 2026 Christoph Wille
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

using Avalonia.Headless.NUnit;
using Avalonia.Media;

using AvaloniaEdit.Highlighting;

using AwesomeAssertions;

using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.Themes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Themes;

/// <summary>
/// Guards the ThemeManager / ThemeAwareHighlightingColorizer contract against double dark
/// conversion. ThemeManager darkens a REGISTERED definition's named colours in place; the
/// colorizer per-paint-remaps colours of UNREGISTERED definitions. Both mechanisms active
/// at once on the same definition -- or the in-place rewrite running twice over one set of
/// colours -- converts colours twice and washes the palette out.
/// </summary>
[TestFixture]
public class DoubleDarkConversionTests
{
	// Minimal definition stub: real Properties (ThemeManager marks theme-awareness there)
	// and real named colours (registration snapshots and rewrites them in place).
	sealed class StubHighlightingDefinition : IHighlightingDefinition
	{
		readonly List<HighlightingColor> colors;

		public StubHighlightingDefinition(params HighlightingColor[] colors)
		{
			this.colors = new List<HighlightingColor>(colors);
		}

		public string Name => "ColorizerTestStub";
		public HighlightingRuleSet MainRuleSet { get; } = new();
		public IEnumerable<HighlightingColor> NamedHighlightingColors => colors;
		public IDictionary<string, string> Properties { get; } = new Dictionary<string, string>();
		public HighlightingRuleSet GetNamedRuleSet(string name) => MainRuleSet;
		public HighlightingColor GetNamedColor(string name) => colors.Find(c => c.Name == name)!;
	}

	// Mimics AvaloniaEdit's DelayLoadedHighlightingDefinition: every member forwards through
	// materialization, and materializing runs a load callback that (like
	// HighlightingService.Load) registers the INNER definition with the theme manager. The
	// wrapper and the inner definition are distinct objects sharing the same live colours
	// and Properties dictionary.
	sealed class LazyLoadedDefinition : IHighlightingDefinition
	{
		readonly StubHighlightingDefinition inner;
		bool materialized;

		public LazyLoadedDefinition(StubHighlightingDefinition inner)
		{
			this.inner = inner;
		}

		StubHighlightingDefinition GetDefinition()
		{
			if (!materialized)
			{
				materialized = true;
				ThemeManager.Current.RegisterThemableDefinition(inner);
			}
			return inner;
		}

		public string Name => GetDefinition().Name;
		public HighlightingRuleSet MainRuleSet => GetDefinition().MainRuleSet;
		public IEnumerable<HighlightingColor> NamedHighlightingColors => GetDefinition().NamedHighlightingColors;
		public IDictionary<string, string> Properties => GetDefinition().Properties;
		public HighlightingRuleSet GetNamedRuleSet(string name) => GetDefinition().GetNamedRuleSet(name);
		public HighlightingColor GetNamedColor(string name) => GetDefinition().GetNamedColor(name);
	}

	static HighlightingColor MakeColor(string name, Color foreground)
		=> new() { Name = name, Foreground = new SimpleHighlightingBrush(foreground) };

	// Drives ThemeManager through its public surface; restores Light afterwards so the
	// process-lived singleton doesn't leak dark mode into other tests.
	static SessionSettings AttachThemeSettings()
	{
		var settings = new SessionSettings();
		ThemeManager.Current.Attach(settings);
		return settings;
	}

	[AvaloniaTest]
	public void LazyLoadedDefinitionIsNotDarkenedTwiceAtDarkStartup()
	{
		var settings = AttachThemeSettings();
		try
		{
			// Dark is active BEFORE the definition is first touched -- the "dark theme
			// preselected, session restores a document" startup ordering.
			settings.Theme = "Dark";

			var color = MakeColor("String", Colors.Red);
			var pristine = color.Clone();
			var wrapper = new LazyLoadedDefinition(new StubHighlightingDefinition(color));

			// HighlightingService.GetByExtension registers what the HighlightingManager
			// returns: the wrapper. Its first member access materializes the inner
			// definition, whose own registration has then ALREADY darkened the shared
			// colours in place -- so the wrapper registration must not treat those dark
			// values as light originals and convert them a second time.
			ThemeManager.Current.RegisterThemableDefinition(wrapper);

			var singleConversion = ThemeManager.GetColorForDarkTheme(pristine);
			color.Should().Be(singleConversion,
				"registering the lazy wrapper after its inner definition must not compound the dark conversion");
		}
		finally
		{
			settings.Theme = "Light";
		}
	}

	[AvaloniaTest]
	public void ThemeSwitchRestoresPristineColorsAfterDarkStartup()
	{
		var settings = AttachThemeSettings();
		try
		{
			settings.Theme = "Dark";

			var color = MakeColor("String", Colors.Red);
			var pristine = color.Clone();
			var wrapper = new LazyLoadedDefinition(new StubHighlightingDefinition(color));
			ThemeManager.Current.RegisterThemableDefinition(wrapper);

			// Every later switch must be computed from the pristine snapshot, not from
			// whatever the colour held after the previous rewrite: Light restores the
			// .xshd values exactly, and Dark again yields the single conversion.
			settings.Theme = "Light";
			color.Should().Be(pristine,
				"switching to Light must restore the original .xshd colours exactly");

			settings.Theme = "Dark";
			color.Should().Be(ThemeManager.GetColorForDarkTheme(pristine),
				"switching back to Dark must convert the pristine colours exactly once");
		}
		finally
		{
			settings.Theme = "Light";
		}
	}

	[AvaloniaTest]
	public void RegistrationAfterConstructionDisablesPerPaintRemap()
	{
		var settings = AttachThemeSettings();
		try
		{
			var color = MakeColor("Keyword", Colors.Blue);
			var definition = new StubHighlightingDefinition(color);
			var colorizer = new ThemeAwareHighlightingColorizer(definition);

			settings.Theme = "Dark";

			// Unregistered definition: the colorizer owns the dark conversion.
			colorizer.GetEffectiveColor(color).Should().NotBeSameAs(color,
				"an unregistered definition's colours must be remapped per paint in dark mode");

			// Late registration: ThemeManager now darkens the live colours in place. If the
			// colorizer kept remapping on top of that, every colour would be converted twice.
			ThemeManager.Current.RegisterThemableDefinition(definition);

			colorizer.GetEffectiveColor(color).Should().BeSameAs(color,
				"once the definition is theme-managed, a second per-paint conversion would double-darken");
		}
		finally
		{
			settings.Theme = "Light";
		}
	}

	[AvaloniaTest]
	public void DarkConversionCacheMissesAfterInPlaceRecolor()
	{
		var settings = AttachThemeSettings();
		try
		{
			var color = MakeColor("String", Colors.Red);
			var definition = new StubHighlightingDefinition(color);
			var colorizer = new ThemeAwareHighlightingColorizer(definition);

			settings.Theme = "Dark";
			colorizer.GetEffectiveColor(color); // primes the cache with the conversion of Red

			// The colour's content changes in place with no paint in between. The colorizer's
			// conversion cache is keyed by HighlightingColor's content-based equality, so the
			// changed content must miss the cache and reconvert -- serving the conversion of
			// the old values would paint stale colours. If HighlightingColor ever moved to
			// reference equality, this goes red and the cache needs an explicit flush instead.
			color.Foreground = new SimpleHighlightingBrush(Colors.Lime);

			colorizer.GetEffectiveColor(color).Should().Be(ThemeManager.GetColorForDarkTheme(color),
				"the conversion served after an in-place recolour must be computed from the new colour values");
		}
		finally
		{
			settings.Theme = "Light";
		}
	}
}
