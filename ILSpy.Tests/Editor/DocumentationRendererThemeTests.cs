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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;

using AvaloniaEdit.Highlighting;

using AwesomeAssertions;

using ICSharpCode.Decompiler.CSharp.OutputVisitor;

using ICSharpCode.ILSpy.TextView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.TextView;

/// <summary>
/// The hover popup's signature text is coloured by the active highlighting theme, so its
/// chrome must follow the same theme variant: dark-theme (light-on-dark) text on a
/// hardcoded near-white background is unreadable (issue #3994). Light mode keeps the
/// established near-white look.
/// </summary>
[TestFixture]
public class DocumentationRendererThemeTests
{
	[AvaloniaTest]
	public void Popup_Chrome_Follows_The_Active_Theme_Variant()
	{
		var renderer = new DocumentationRenderer(
			new CSharpAmbience(),
			new FontFamily("Consolas, Menlo, Monospace"),
			12);
		renderer.AddSignatureBlock(new RichText("(parameter) string matchText"));

		var view = (Border)renderer.CreateView();
		var window = new Window { Content = view };
		var app = Application.Current!;
		var originalVariant = app.RequestedThemeVariant;
		try
		{
			window.Show();

			app.RequestedThemeVariant = ThemeVariant.Dark;
			Dispatcher.UIThread.RunJobs();

			window.TryFindResource("ILSpy.DocTooltipBackground", ThemeVariant.Dark, out var darkBackground)
				.Should().BeTrue("the popup chrome must route through themed brushes");
			view.Background.Should().Be(darkBackground,
				"dark-theme signature colours are unreadable on a light popup background");
			window.TryFindResource("ILSpy.DocTooltipBorder", ThemeVariant.Dark, out var darkBorder)
				.Should().BeTrue();
			view.BorderBrush.Should().Be(darkBorder);

			app.RequestedThemeVariant = ThemeVariant.Light;
			Dispatcher.UIThread.RunJobs();

			((ISolidColorBrush)view.Background!).Color.Should().Be(Color.FromRgb(0xFC, 0xFC, 0xFC),
				"light mode keeps the established near-white popup chrome");
			((ISolidColorBrush)view.BorderBrush!).Color.Should().Be(Color.FromRgb(0xAA, 0xAA, 0xAA));
		}
		finally
		{
			window.Close();
			app.RequestedThemeVariant = originalVariant;
		}
	}
}
