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
using System.Linq;

using Avalonia.Controls;
using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class MenuIconWiringProbe
{
	[AvaloniaTest]
	public void Known_Menu_Items_With_MenuIcon_Metadata_Get_Icon_Populated()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var nativeMenu = NativeMenu.GetMenu(window)
			?? throw new InvalidOperationException("MainMenu.Attach should have set NativeMenu on the window");

		var leaves = new List<(string Path, bool HasIcon)>();
		Collect(nativeMenu, "", leaves);
		var withIcon = leaves.Where(l => l.HasIcon).Select(l => l.Path).ToList();

		// Spot-check: File -> Open (which has MenuIcon="Images/Open" in MEF metadata).
		var fileMenu = nativeMenu.Items.OfType<NativeMenuItem>()
			.Single(m => string.Equals(m.Header, Resources._File, StringComparison.Ordinal));
		var openItem = fileMenu.Menu!.Items.OfType<NativeMenuItem>()
			.Single(m => string.Equals(m.Header, Resources._Open, StringComparison.Ordinal));
		openItem.Icon.Should().NotBeNull(
			"File > Open declares MenuIcon=\"Images/Open\" in its [ExportMainMenuCommand]; the menu builder "
			+ "must rasterise that into NativeMenuItem.Icon.");

		// Sanity: at least 5 leaves with icons (we have ~12+ MEF declarations with MenuIcon).
		// On failure, name the items that DID get an icon so the regression is diagnosable without
		// dumping the whole menu on every (passing) run.
		withIcon.Count.Should().BeGreaterThanOrEqualTo(5,
			$"at least 5 main-menu items have MenuIcon metadata; found {withIcon.Count} of {leaves.Count} "
			+ $"leaves with an icon [{string.Join(", ", withIcon)}]");
	}

	static void Collect(NativeMenu menu, string parentPath, List<(string Path, bool HasIcon)> leaves)
	{
		foreach (var element in menu.Items)
		{
			if (element is not NativeMenuItem item)
				continue;
			var path = string.IsNullOrEmpty(parentPath) ? (item.Header ?? "<unnamed>") : $"{parentPath} > {item.Header}";
			if (item.Menu is { Items.Count: > 0 } sub)
				Collect(sub, path, leaves);
			else
				leaves.Add((path, item.Icon != null));
		}
	}
}
