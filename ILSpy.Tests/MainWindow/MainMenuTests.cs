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

using System.Linq;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;

using ILSpy.AppEnv;
using ILSpy.Views;

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
		var mainMenu = new global::ILSpy.MainMenu();
		var menu = mainMenu.FindControl<Menu>("MainMenuRoot");
		menu.Should().NotBeNull();

		var headers = menu!.Items.OfType<MenuItem>().Select(m => m.Header as string).ToList();
		headers.Should().Equal("_File", "_View", "_Window");
	}

	[AvaloniaTest]
	public async Task Main_Menu_Items_Display_Input_Gestures()
	{
		// File → Open carries an InputGestureText="Ctrl+O" attribute on its
		// [ExportMainMenuCommand]; verifies both the displayed gesture (right side of the menu
		// item) and the actual HotKey are wired up so Ctrl+O fires from the keyboard too.

		// Arrange — boot MainWindow and wait for the File menu to be populated by MEF.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();

		var menu = await window.WaitForComponent<Menu>();
		await Waiters.WaitForAsync(() =>
			menu.Items.OfType<MenuItem>().Any(m => (string?)m.Tag == nameof(Resources._File))
				&& menu.Items.OfType<MenuItem>().Single(m => (string?)m.Tag == nameof(Resources._File))
					.Items.OfType<MenuItem>().Any());

		// Act — locate the Open MenuItem under File.
		var fileMenu = menu.Items.OfType<MenuItem>().Single(m => (string?)m.Tag == nameof(Resources._File));
		var openItem = fileMenu.Items.OfType<MenuItem>()
			.Single(m => string.Equals(m.Header as string, Resources._Open, System.StringComparison.Ordinal));

		// Assert — both display gesture and hot-key bind to Ctrl+O.
		openItem.InputGesture.Should().NotBeNull();
		openItem.InputGesture!.Should().Be(KeyGesture.Parse("Ctrl+O"));
		openItem.HotKey.Should().Be(KeyGesture.Parse("Ctrl+O"));
	}
}
