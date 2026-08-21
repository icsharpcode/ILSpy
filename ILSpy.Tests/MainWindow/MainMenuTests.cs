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
	public void AI_Output_is_exposed_from_the_View_menu()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();

		var nativeMenu = NativeMenu.GetMenu(window)
			?? throw new InvalidOperationException("MainMenu.Attach should have set NativeMenu on the window");
		var topLevel = nativeMenu.Items.OfType<NativeMenuItem>().ToDictionary(item => item.Header!);

		topLevel["_View"].Menu!.Items.OfType<NativeMenuItem>()
			.Should().Contain(item => string.Equals(item.Header, "AI Output", StringComparison.Ordinal));
		topLevel["_Window"].Menu!.Items.OfType<NativeMenuItem>()
			.Should().NotContain(item => string.Equals(item.Header, "AI Output", StringComparison.Ordinal));
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

	// The app-level NativeMenu (App.axaml) is process-wide, while every MainWindow builds
	// its own Help items over its own command instances. On macOS each new window promotes
	// them into that app menu; the ones an earlier window promoted must be replaced, not
	// kept - otherwise the app menu pins every earlier window's command graph (and, in the
	// headless suite, every test's app graph) for the life of the process.
	[AvaloniaTest]
	public void Promoting_Help_Again_Replaces_The_Items_An_Earlier_Window_Promoted()
	{
		var appMenu = NativeMenu.GetMenu(Application.Current!);
		appMenu.Should().NotBeNull("App.axaml declares the NativeMenu the Help items move into");

		MainMenu.PromoteHelpToMacAppMenu(
			WindowMenuWithHelpItems("About (first window)", out var firstByTag), firstByTag);
		var afterFirst = appMenu!.Items.Count;
		var promoted = MainMenu.PromoteHelpToMacAppMenu(
			WindowMenuWithHelpItems("About (second window)", out var secondByTag), secondByTag);
		try
		{
			appMenu.Items.Count.Should().Be(afterFirst, "the second window's Help items replace the first window's");
			appMenu.Items.OfType<NativeMenuItem>().Select(i => i.Header)
				.Should().Contain("About (second window)")
				.And.NotContain("About (first window)");
		}
		finally
		{
			RestoreAppMenu(appMenu, promoted);
		}
	}

	// The Help items a window promotes are withdrawn when it closes, but only that window's own:
	// a window closing after a second one has promoted its items must leave those in the app menu,
	// or macOS shows an app menu with no About / Check for Updates while the second window is still
	// on screen and nothing ever puts them back.
	[AvaloniaTest]
	public void Closing_An_Earlier_Window_Leaves_A_Later_Window_Help_Items_In_Place()
	{
		var appMenu = NativeMenu.GetMenu(Application.Current!);
		appMenu.Should().NotBeNull("App.axaml declares the NativeMenu the Help items move into");

		var first = MainMenu.PromoteHelpToMacAppMenu(
			WindowMenuWithHelpItems("About (first window)", out var firstByTag), firstByTag);
		var second = MainMenu.PromoteHelpToMacAppMenu(
			WindowMenuWithHelpItems("About (second window)", out var secondByTag), secondByTag);
		try
		{
			// What the first window's Closed handler does, now that the second window has promoted.
			MainMenu.WithdrawHelpItems(first);

			appMenu!.Items.OfType<NativeMenuItem>().Select(i => i.Header)
				.Should().Contain("About (second window)",
					"the still-open window's Help items must survive an earlier window closing");
		}
		finally
		{
			RestoreAppMenu(appMenu!, second);
		}
	}

	// The app menu is declared on Application and outlives every test, so a test that promotes
	// placeholder items into it has to take them back out; otherwise a later test reading it
	// (see MainMenu_top_level_items_are_File_View_Window_in_order) sees this test's leftovers.
	static void RestoreAppMenu(NativeMenu appMenu, List<NativeMenuItemBase> promoted)
	{
		foreach (var item in promoted)
			appMenu.Items.Remove(item);
	}

	static NativeMenu WindowMenuWithHelpItems(string header, out Dictionary<string, NativeMenuItem> byTag)
	{
		var help = new NativeMenuItem { Header = "_Help", Menu = new NativeMenu() };
		help.Menu.Items.Add(new NativeMenuItem { Header = header });
		var root = new NativeMenu();
		root.Items.Add(help);
		byTag = new Dictionary<string, NativeMenuItem>(StringComparer.Ordinal) { ["_Help"] = help };
		return root;
	}

	// NativeMenuItem.Gesture is display-only when NativeMenuBar renders the menu inline
	// (the managed fallback binds it to MenuItem.InputGesture, which never handles input),
	// so every menu gesture must also be registered as a window-level KeyBinding or the
	// shortcut silently does nothing on Windows / Linux (issue #3993: Ctrl+O, Ctrl+S, F5).
	[AvaloniaTest]
	public void Menu_Gestures_Are_Registered_As_Window_KeyBindings()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();

		var nativeMenu = NativeMenu.GetMenu(window)
			?? throw new InvalidOperationException("MainMenu.Attach should have set NativeMenu on the window");

		var gestureItems = new System.Collections.Generic.List<(string Path, NativeMenuItem Item)>();
		CollectItemsWithGesture(nativeMenu, parentPath: "", gestureItems);

		gestureItems.Should().NotBeEmpty("File > Open (Ctrl+O), Reload (F5) and Save (Ctrl+S) declare InputGestureText");

		foreach (var (path, item) in gestureItems)
		{
			window.KeyBindings.Should().Contain(
				kb => Equals(kb.Gesture, item.Gesture) && ReferenceEquals(kb.Command, item.Command),
				$"the gesture {item.Gesture} shown on '{path}' must actually invoke the item's command");
		}
	}

	static void CollectItemsWithGesture(NativeMenu menu, string parentPath, System.Collections.Generic.List<(string, NativeMenuItem)> result)
	{
		foreach (var element in menu.Items)
		{
			if (element is NativeMenuItemSeparator || element is not NativeMenuItem item)
				continue;
			var path = string.IsNullOrEmpty(parentPath) ? (item.Header ?? "<unnamed>") : $"{parentPath} > {item.Header}";
			// Mirrors RegisterGestureKeyBindings: only items with BOTH a gesture and a command
			// get a key binding, so a display-only gesture must not fail the assertion.
			if (item.Gesture != null && item.Command != null)
				result.Add((path, item));
			if (item.Menu is { Items.Count: > 0 } sub)
				CollectItemsWithGesture(sub, path, result);
		}
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
