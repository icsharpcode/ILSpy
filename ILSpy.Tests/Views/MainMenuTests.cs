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
using System.Linq;

using Avalonia.Controls;
using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Views;

/// <summary>
/// The main menu is built as a NativeMenu (projected into the AppKit menu bar on macOS, rendered
/// inline by NativeMenuBar on Windows/Linux). A menu item's enabled state must follow its
/// command's CanExecute, not a hard-coded flag -- otherwise OS-gated commands like "Open from GAC"
/// (Windows-only) stay clickable on the macOS native menu even though invoking them is a no-op.
/// </summary>
[TestFixture]
public class MainMenuTests
{
	static NativeMenuItem? Find(NativeMenu menu, Func<NativeMenuItem, bool> predicate)
	{
		foreach (var item in menu.Items.OfType<NativeMenuItem>())
		{
			if (predicate(item))
				return item;
			if (item.Menu is { } submenu && Find(submenu, predicate) is { } hit)
				return hit;
		}
		return null;
	}

	[AvaloniaTest]
	public void OpenFromNuGetFeed_item_sits_between_GAC_and_Reload_and_works_on_every_OS()
	{
		var window = new Window();
		MainMenu.Attach(window);

		var menu = NativeMenu.GetMenu(window);
		menu.Should().NotBeNull();

		var nuget = Find(menu!, i => i.Header?.Contains("NuGet feed", StringComparison.OrdinalIgnoreCase) == true);
		nuget.Should().NotBeNull("the File menu must contain an 'Open from NuGet feed' item");
		nuget!.IsEnabled.Should().BeTrue("NuGet feeds are reachable from every OS, unlike the GAC");

		var fileSubmenu = menu!.Items.OfType<NativeMenuItem>()
			.Select(i => i.Menu)
			.First(m => m != null && m.Items.OfType<NativeMenuItem>()
				.Any(i => i.Header?.Contains("GAC", StringComparison.OrdinalIgnoreCase) == true));
		var items = fileSubmenu!.Items.OfType<NativeMenuItem>().ToList();
		int gacIndex = items.FindIndex(i => i.Header?.Contains("GAC", StringComparison.OrdinalIgnoreCase) == true);
		int nugetIndex = items.FindIndex(i => i.Header?.Contains("NuGet feed", StringComparison.OrdinalIgnoreCase) == true);
		int reloadIndex = items.FindIndex(i => i.Header?.Contains("Reload", StringComparison.OrdinalIgnoreCase) == true);

		nugetIndex.Should().BeGreaterThan(gacIndex,
			"the WPF File menu groups the open-from sources right below Open, GAC first");
		nugetIndex.Should().BeLessThan(reloadIndex, "Reload ends the open group");
	}

	[AvaloniaTest]
	public void OpenFromGac_item_enabled_state_follows_the_command()
	{
		var window = new Window();
		MainMenu.Attach(window);

		var menu = NativeMenu.GetMenu(window);
		menu.Should().NotBeNull("the composition host is up in tests, so Attach builds the menu");

		var gac = Find(menu!, i => i.Header?.Contains("GAC", StringComparison.OrdinalIgnoreCase) == true);
		gac.Should().NotBeNull("the File menu must contain an 'Open from GAC' item");

		gac!.IsEnabled.Should().Be(OperatingSystem.IsWindows(),
			"the GAC is Windows-only, so the item must reflect the command's CanExecute (disabled off "
			+ "Windows) instead of a hard-coded enabled flag that the macOS native menu would show wrongly");
	}
}
