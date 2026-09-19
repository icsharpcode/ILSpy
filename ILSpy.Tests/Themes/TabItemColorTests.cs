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

using System;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;

using AwesomeAssertions;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Themes;

/// <summary>
/// The Simple theme paints tab headers with a fixed mid-gray foreground that ignores the
/// theme variant, and its Dark pointer-over fill is almost exactly that gray, so a hovered
/// header (the Options page's section names) loses its text. App.axaml restyles TabItem;
/// verified here because a typo in the selector would silently fall back to the theme.
/// </summary>
[TestFixture]
public class TabItemColorTests
{
	// WCAG AA for normal-size text.
	const double MinimumContrast = 4.5;

	[AvaloniaTest]
	public void Dark_Tab_Headers_Stay_Readable_When_Hovered_Or_Selected()
	{
		var app = Application.Current ?? throw new InvalidOperationException("no Application");
		var previous = app.RequestedThemeVariant;
		try
		{
			app.RequestedThemeVariant = ThemeVariant.Dark;

			var selected = new TabItem { Header = "Decompiler", Content = "a" };
			var other = new TabItem { Header = "Display", Content = "b" };
			var tabs = new TabControl { TabStripPlacement = global::Avalonia.Controls.Dock.Left, ItemsSource = new[] { selected, other } };
			var window = new Window { Content = tabs, Width = 400, Height = 300 };
			window.Show();
			Dispatcher.UIThread.RunJobs();

			var center = other.TranslatePoint(new Point(other.Bounds.Width / 2, other.Bounds.Height / 2), window)
				?? throw new InvalidOperationException("tab header is not in the window");
			window.MouseMove(center);
			Dispatcher.UIThread.RunJobs();

			selected.IsSelected.Should().BeTrue();
			other.IsPointerOver.Should().BeTrue("the pointer-over fill is what is under test");

			HeaderContrast(other, window).Should().BeGreaterThanOrEqualTo(MinimumContrast, "hovered header");
			HeaderContrast(selected, window).Should().BeGreaterThanOrEqualTo(MinimumContrast, "selected header");
		}
		finally
		{
			app.RequestedThemeVariant = previous;
		}
	}

	// Contrast of the header text against its fill as the user sees it: the fills are
	// translucent, so they are composited over the window canvas first.
	static double HeaderContrast(TabItem item, Window window)
	{
		var presenter = item.GetVisualDescendants().OfType<ContentPresenter>()
			.First(p => p.Name == "PART_ContentPresenter");
		var canvas = Solid(window.Background).Color;
		var fill = presenter.Background is null ? canvas : Composite(Solid(presenter.Background), canvas);
		return ThemeManagerTests.Contrast(Solid(item.Foreground).Color, fill);
	}

	static Color Composite(ISolidColorBrush brush, Color canvas)
	{
		var alpha = brush.Color.A / 255.0 * brush.Opacity;
		byte Mix(byte over, byte under) => (byte)Math.Round(over * alpha + under * (1 - alpha));
		return Color.FromRgb(Mix(brush.Color.R, canvas.R), Mix(brush.Color.G, canvas.G), Mix(brush.Color.B, canvas.B));
	}

	static ISolidColorBrush Solid(IBrush? brush)
		=> brush as ISolidColorBrush ?? throw new InvalidOperationException("not a solid color brush");
}
