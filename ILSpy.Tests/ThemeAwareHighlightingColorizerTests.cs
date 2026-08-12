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
using AvaloniaEdit.Rendering;

using AwesomeAssertions;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.Themes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Guards the ThemeManager / ThemeAwareHighlightingColorizer contract against double dark
/// conversion. ThemeManager darkens a REGISTERED definition's named colours in place; the
/// colorizer per-paint-remaps colours of UNREGISTERED definitions. Both mechanisms active at
/// once on the same definition converts colours twice (washed-out output), so the colorizer
/// must observe a registration that happens after it was constructed, and must not serve
/// cached conversions computed from colour values that a theme switch has since replaced.
/// </summary>
[TestFixture]
public class ThemeAwareHighlightingColorizerTests
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
	public void DarkCacheDoesNotSurviveThemeSwitch()
	{
		var settings = AttachThemeSettings();
		try
		{
			var color = MakeColor("String", Colors.Red);
			var definition = new StubHighlightingDefinition(color);
			var colorizer = new ThemeAwareHighlightingColorizer(definition);

			settings.Theme = "Dark";
			var beforeSwitch = colorizer.GetEffectiveColor(color);

			// The colour's content changes in place (as ThemeManager does for managed
			// definitions) with no paint in between. The colorizer's conversion cache is
			// keyed by HighlightingColor's content-based equality, so the changed content
			// must miss the cache and reconvert -- serving the conversion of the old values
			// here would paint stale colours. If HighlightingColor ever moved to reference
			// equality, this test goes red and the cache needs an explicit flush instead.
			color.Foreground = new SimpleHighlightingBrush(Colors.Lime);
			settings.Theme = "Light";
			settings.Theme = "Dark";

			var afterSwitch = colorizer.GetEffectiveColor(color);
			afterSwitch.Should().NotBeSameAs(beforeSwitch,
				"conversions cached from superseded colour values must not be served");
		}
		finally
		{
			settings.Theme = "Light";
		}
	}
}
