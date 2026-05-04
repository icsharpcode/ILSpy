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

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;

using ILSpy.AppEnv;
using ILSpy.Commands;
using ILSpy.Docking;
using ILSpy.TextView;
using ILSpy.ViewModels;
using ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class HelpCommandTests
{
	[AvaloniaTest]
	public async Task About_Command_Opens_New_Tab_With_Version_Info()
	{
		// Help → About is exported as a [ExportMainMenuCommand] under ParentMenuID="_Help".
		// Executing it must open a new document tab in the dock workspace whose title is the
		// localized "About" string and whose body contains the ILSpy version line plus the
		// embedded ILSpy-about-page blurb (MIT License is the easiest stable phrase to match).

		// Arrange — boot the window so the dock workspace + document dock are realised.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		var aboutCmd = registry.Commands
			.Single(c => c.Metadata.Header == nameof(Resources._About))
			.CreateExport().Value;

		var documents = ((ILSpyDockFactory)vm.DockWorkspace.Factory).Documents!;
		var initialTabCount = documents.VisibleDockables?.Count ?? 0;

		// Act — fire the About command.
		aboutCmd.Execute(null);

		// Assert — a new DecompilerTabPageModel landed in the document dock, titled "About",
		// containing the version line and the MIT License mention from the embedded blurb.
		await Waiters.WaitForAsync(
			() => (documents.VisibleDockables?.Count ?? 0) > initialTabCount
				&& documents.ActiveDockable is DecompilerTabPageModel { Text.Length: > 0 });

		var aboutTab = (DecompilerTabPageModel)documents.ActiveDockable!;
		aboutTab.Title.Should().Be(Resources.About);
		aboutTab.Text.Should().Contain(Resources.ILSpyVersion);
		aboutTab.Text.Should().Contain("MIT License");
	}
}
