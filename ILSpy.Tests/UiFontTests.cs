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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Styling;

using AwesomeAssertions;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Exercises App.ApplyUiFont (the system-UI-font override) with a synthetic size, since the
/// production path reads machine-dependent NONCLIENTMETRICS. The interesting case is
/// ContextMenu: the Simple theme pins its FontSize to the theme's FontSizeNormal resource,
/// which outranks the inherited value that covers windows and menu-bar dropdowns, so a
/// context menu silently stays at 12 unless the override also reaches that resource.
/// </summary>
[TestFixture]
public class UiFontTests
{
	// Distinct from every size the Simple theme uses (10/12/16) so a match proves the override.
	const double TestFontSize = 20;

	[AvaloniaTest]
	public void UiFontReachesWindowAndContextMenu()
	{
		var app = Application.Current!;
		int styleCount = app.Styles.Count;
		bool hadFontSizeNormal = app.Resources.TryGetValue("FontSizeNormal", out var previousFontSizeNormal);

		try
		{
			App.ApplyUiFont(app, "Segoe UI", TestFontSize);

			var contextMenu = new ContextMenu {
				Items = { new MenuItem { Header = "Item" } },
			};
			var target = new Button { ContextMenu = contextMenu };
			var window = new Window { Content = target };
			window.Show();

			window.FontSize.Should().Be(TestFontSize, "the TopLevel style must reach windows");

			contextMenu.Open(target);
			contextMenu.FontSize.Should().Be(TestFontSize,
				"the override must beat the Simple theme's FontSizeNormal pin on ContextMenu");

			window.Close();
		}
		finally
		{
			// App styles/resources are per-assembly shared state (ResetAppState rebuilds the MEF
			// container, not the Application); undo so later tests keep the default 12.
			while (app.Styles.Count > styleCount)
				app.Styles.RemoveAt(app.Styles.Count - 1);
			if (hadFontSizeNormal)
				app.Resources["FontSizeNormal"] = previousFontSizeNormal;
			else
				app.Resources.Remove("FontSizeNormal");
		}
	}
}
