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
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Headless.NUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;

using AwesomeAssertions;

using Dock.Avalonia.Controls;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Docking;

/// <summary>
/// The document tab strip toggles between a single scrolling row and multiple wrapped rows, driven by
/// the persisted <see cref="SessionSettings.MultiLineDocumentTabs"/> setting and the mouse wheel. The
/// overflow dropdown that lists open documents shows only in single-line mode.
/// </summary>
[TestFixture]
public class DocumentTabStripModeTests
{
	static SessionSettings Settings => AppComposition.Current.GetExport<SettingsService>().SessionSettings;

	static async Task<(MainWindow window, DocumentTabStrip strip)> BootWithTabsAsync(bool multiLine)
	{
		var (window, vm) = await TestHarness.BootAsync(1);
		Settings.MultiLineDocumentTabs = multiLine;
		for (int i = 0; i < 30; i++)
			vm.DockWorkspace.OpenNewTab(new DecompilerTabPageModel { Title = $"Tab {i:00}" });
		await Pump(window);
		var strip = window.GetVisualDescendants().OfType<DocumentTabStrip>().First();
		return (window, strip);
	}

	static async Task Pump(MainWindow window)
	{
		for (int i = 0; i < 12; i++)
		{
			Dispatcher.UIThread.RunJobs();
			window.UpdateLayout();
			await Task.Delay(20);
		}
	}

	static Button? Dropdown(DocumentTabStrip strip)
		=> strip.GetVisualDescendants().OfType<Button>()
			.FirstOrDefault(b => (b.Tag as string) == "DocumentSwitcherDropdown");

	[AvaloniaTest]
	public async Task Single_Line_Mode_Has_No_Wrap_Panel_And_A_Visible_Dropdown()
	{
		var saved = Settings.MultiLineDocumentTabs;
		try
		{
			var (_, strip) = await BootWithTabsAsync(multiLine: false);

			strip.GetVisualDescendants().OfType<WrapPanel>().Should().BeEmpty(
				"single-line mode keeps the default StackPanel item layout");
			Dropdown(strip).Should().NotBeNull("the overflow dropdown is injected on the strip");
			Dropdown(strip)!.IsVisible.Should().BeTrue("the dropdown shows in single-line mode");
		}
		finally
		{ Settings.MultiLineDocumentTabs = saved; }
	}

	[AvaloniaTest]
	public async Task Multi_Line_Mode_Wraps_And_Hides_The_Dropdown()
	{
		var saved = Settings.MultiLineDocumentTabs;
		try
		{
			var (_, strip) = await BootWithTabsAsync(multiLine: true);

			strip.GetVisualDescendants().OfType<WrapPanel>().Should().NotBeEmpty(
				"multi-line mode flows tabs through a WrapPanel");
			Dropdown(strip)!.IsVisible.Should().BeFalse(
				"every tab is visible across rows, so the dropdown hides");
		}
		finally
		{ Settings.MultiLineDocumentTabs = saved; }
	}

	[AvaloniaTest]
	public async Task Mouse_Wheel_Over_The_Strip_Toggles_The_Mode()
	{
		var saved = Settings.MultiLineDocumentTabs;
		try
		{
			var (window, strip) = await BootWithTabsAsync(multiLine: false);
			var point = strip.TranslatePoint(new Point(8, 8), window) ?? new Point(8, 8);

			window.MouseWheel(point, new Vector(0, 1));
			await Pump(window);
			Settings.MultiLineDocumentTabs.Should().BeTrue("wheel up expands to multiple rows");

			window.MouseWheel(point, new Vector(0, -1));
			await Pump(window);
			Settings.MultiLineDocumentTabs.Should().BeFalse("wheel down collapses to a single row");
		}
		finally
		{ Settings.MultiLineDocumentTabs = saved; }
	}

	[AvaloniaTest]
	public async Task Mouse_Wheel_Does_Not_Toggle_The_Mode_When_The_Setting_Is_Off()
	{
		var savedMode = Settings.MultiLineDocumentTabs;
		var savedGate = Settings.MouseWheelTogglesTabStripRows;
		try
		{
			var (window, strip) = await BootWithTabsAsync(multiLine: false);
			Settings.MouseWheelTogglesTabStripRows = false;
			await Pump(window);
			var point = strip.TranslatePoint(new Point(8, 8), window) ?? new Point(8, 8);

			window.MouseWheel(point, new Vector(0, 1));
			await Pump(window);

			Settings.MultiLineDocumentTabs.Should().BeFalse(
				"with the 'mouse wheel toggles' setting off, the wheel must not flip the tab-strip mode");
		}
		finally
		{
			Settings.MultiLineDocumentTabs = savedMode;
			Settings.MouseWheelTogglesTabStripRows = savedGate;
		}
	}

	[AvaloniaTest]
	public async Task Clicking_The_Dropdown_Opens_A_Populated_Menu()
	{
		var saved = Settings.MultiLineDocumentTabs;
		try
		{
			var (window, strip) = await BootWithTabsAsync(multiLine: false);
			var button = Dropdown(strip);
			button.Should().NotBeNull();

			var menu = button!.ContextMenu!;
			bool opened = false;
			menu.Opened += (_, _) => opened = true;

			// Drive a real pointer click through the input pipeline (open is deferred a dispatcher
			// turn, which Pump runs), so this would have caught the menu failing to open on a live click.
			var centre = button.TranslatePoint(new Point(button.Bounds.Width / 2, button.Bounds.Height / 2), window)
				?? new Point(8, 8);
			window.MouseDown(centre, MouseButton.Left);
			window.MouseUp(centre, MouseButton.Left);
			await Pump(window);

			opened.Should().BeTrue("clicking the dropdown must open its menu");
			menu.Items.OfType<MenuItem>().Should().NotBeEmpty("the open menu lists the strip's documents");
		}
		finally
		{ Settings.MultiLineDocumentTabs = saved; }
	}

	[AvaloniaTest]
	public async Task Picking_A_Menu_Entry_Switches_To_That_Document()
	{
		var saved = Settings.MultiLineDocumentTabs;
		try
		{
			var (window, strip) = await BootWithTabsAsync(multiLine: false);
			var button = Dropdown(strip);
			var menu = button!.ContextMenu!;

			var centre = button.TranslatePoint(new Point(button.Bounds.Width / 2, button.Bounds.Height / 2), window)
				?? new Point(8, 8);
			window.MouseDown(centre, MouseButton.Left);
			window.MouseUp(centre, MouseButton.Left);
			await Pump(window);

			var target = strip.Items.OfType<ContentTabPage>().Last();
			var targetItem = menu.Items.OfType<MenuItem>().Single(i => (string?)i.Header == target.Title);
			targetItem.RaiseEvent(new Avalonia.Interactivity.RoutedEventArgs(MenuItem.ClickEvent));
			await Pump(window);

			strip.SelectedItem.Should().Be(target, "picking a document switches the strip to it");
		}
		finally
		{ Settings.MultiLineDocumentTabs = saved; }
	}
}
