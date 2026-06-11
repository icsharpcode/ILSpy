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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Input;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

// MainMenu's top-level structure (File / View / Window with mnemonic underscores) is the
// scaffolding every later commit hangs items onto via MEF. If a future commit accidentally
// drops one of these top-levels or shuffles the order, the [ExportMainMenuCommand] entries
// that target them by header would silently land in the wrong menu.
[TestFixture]
public class MainMenuTests
{
	[AvaloniaTest]
	public void MainMenu_top_level_items_are_File_View_Window_in_order()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();

		var nativeMenu = NativeMenu.GetMenu(window)
			?? throw new InvalidOperationException("MainMenu.Attach should have set NativeMenu on the window");

		var headers = nativeMenu.Items.OfType<NativeMenuItem>().Select(i => i.Header).ToList();
		if (OperatingSystem.IsMacOS())
		{
			// PromoteHelpToMacAppMenu relocates the Help items into the application menu
			// (macOS convention: About lives under the bold app-named menu), so _Help is
			// not a window-menu top-level there.
			headers.Should().Equal("_File", "_View", "_Window");

			var appMenu = NativeMenu.GetMenu(Application.Current!);
			appMenu.Should().NotBeNull("App.axaml declares the NativeMenu the Help items move into");
			appMenu!.Items.OfType<NativeMenuItem>().Select(i => i.Header)
				.Should().Contain(Resources._About, "Help content must move to the app menu, not vanish");
		}
		else
		{
			headers.Should().Equal("_File", "_View", "_Window", "_Help");
		}
	}

	[AvaloniaTest]
	public void File_Open_Carries_The_Ctrl_O_Gesture()
	{
		// MEF metadata's InputGestureText="Ctrl+O" on File -> Open must flow through
		// MainMenu.Attach into NativeMenuItem.Gesture. On macOS Avalonia projects this
		// into the system menu bar; on Windows / Linux NativeMenuBar renders inline.
		// On macOS, TranslateGesturesForMacOS additionally rewrites Control to Meta so
		// the shortcut follows the Cmd-key convention.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();

		var nativeMenu = NativeMenu.GetMenu(window)
			?? throw new InvalidOperationException("MainMenu.Attach should have set NativeMenu on the window");

		var fileMenu = nativeMenu.Items.OfType<NativeMenuItem>()
			.Single(m => string.Equals(m.Header, Resources._File, StringComparison.Ordinal));
		var openItem = fileMenu.Menu!.Items.OfType<NativeMenuItem>()
			.Single(m => string.Equals(m.Header, Resources._Open, StringComparison.Ordinal));

		openItem.Gesture.Should().NotBeNull();
		var expected = OperatingSystem.IsMacOS() ? KeyGesture.Parse("Cmd+O") : KeyGesture.Parse("Ctrl+O");
		openItem.Gesture!.Should().Be(expected);
	}

	// Avalonia's macOS NativeMenu bridge maps NativeMenuItem to NSMenuItem and sets
	// NSMenuItem.action ONLY when Command != null. Without it, NSMenuValidation marks
	// the item disabled (greyed out) and no click ever reaches managed code - which
	// means any IsChecked TwoWay binding silently never fires either. So every leaf
	// NativeMenuItem (one that doesn't open a submenu) must have Command set.
	[AvaloniaTest]
	public void Every_Leaf_NativeMenuItem_Has_A_Command_So_macOS_Clicks_Through()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();

		var nativeMenu = NativeMenu.GetMenu(window)
			?? throw new InvalidOperationException("MainMenu.Attach should have set NativeMenu on the window");

		var leavesMissingCommand = new System.Collections.Generic.List<string>();
		CollectLeavesMissingCommand(nativeMenu, parentPath: "", leavesMissingCommand);

		leavesMissingCommand.Should().BeEmpty(
			"every leaf NativeMenuItem must set Command, otherwise on macOS the item is "
			+ "disabled by NSMenuValidation and clicks never reach Avalonia. Sites historically "
			+ "missing this: MakeRadio (ApiVis radios), ToolPaneMenuItem checkboxes, and "
			+ "TabPageMenuItem radios in AppendWindowDynamicContent / AppendTabSection.");
	}

	static void CollectLeavesMissingCommand(NativeMenu menu, string parentPath, System.Collections.Generic.List<string> missing)
	{
		foreach (var element in menu.Items)
		{
			// NativeMenuItemSeparator inherits from NativeMenuItem in Avalonia 12 (its
			// Header defaults to "-"), so the type filter has to exclude separators
			// explicitly - otherwise they look like leaves-without-Command and trip
			// the assertion.
			if (element is NativeMenuItemSeparator)
				continue;
			if (element is not NativeMenuItem item)
				continue;
			var path = string.IsNullOrEmpty(parentPath) ? (item.Header ?? "<unnamed>") : $"{parentPath} > {item.Header}";
			if (item.Menu is { Items.Count: > 0 } sub)
			{
				CollectLeavesMissingCommand(sub, path, missing);
			}
			else if (item.Command == null)
			{
				missing.Add(path);
			}
		}
	}
}
