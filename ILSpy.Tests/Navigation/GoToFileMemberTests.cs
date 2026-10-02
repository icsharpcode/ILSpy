// Copyright (c) 2026 Piero Viano
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
using Avalonia.Input;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.FileStructure;
using ICSharpCode.ILSpy.Properties;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class GoToFileMemberTests
{
	[AvaloniaTest]
	public async Task Typing_Filters_The_Members_Of_The_Active_Document()
	{
		var (_, tab) = await FileStructureTests.DecompileFixtureTypeAsync("Circle");
		var model = new GoToFileMemberViewModel(tab);
		model.Items.Select(e => e.Name).Should().Contain(["Circle", "radius", "Area", "Diameter"]);

		// Act -- case-insensitive substring filter.
		model.FilterText = "DIA";

		// Assert -- only Diameter remains and is preselected so Enter accepts it.
		model.Items.Select(e => e.Name).Should().Equal("Diameter");
		model.SelectedItem.Should().BeSameAs(model.Items[0]);

		model.FilterText = "zzz";
		model.Items.Should().BeEmpty();
		model.SelectedItem.Should().BeNull();
		model.Accept().Should().BeFalse("nothing is selected");
	}

	[AvaloniaTest]
	public async Task Accept_Moves_The_Caret_To_The_Member_And_Closes_The_Popup()
	{
		var (_, tab) = await FileStructureTests.DecompileFixtureTypeAsync("Circle");
		var model = new GoToFileMemberViewModel(tab);
		bool closed = false;
		model.CloseRequested += (_, _) => closed = true;
		model.FilterText = "area";
		var area = model.SelectedItem!;

		// Act
		model.Accept().Should().BeTrue();

		// Assert
		closed.Should().BeTrue();
		tab.CaptureViewState!().CaretOffset.Should().Be(area.Offset);
	}

	[AvaloniaTest]
	public async Task Command_Opens_The_Popup_For_The_Active_Document()
	{
		var (vm, tab) = await FileStructureTests.DecompileFixtureTypeAsync("Circle");
		var command = (GoToFileMemberCommand)AppComposition.Current.GetExport<MainMenuCommandRegistry>()
			.GetCommand(nameof(Resources.GoToFileMember));

		// Act
		command.CanExecute(null).Should().BeTrue();
		command.Execute(null);

		// Assert -- the popup is open, bound to the active document, and listing its members.
		command.OpenFlyout.Should().NotBeNull();
		command.OpenFlyout!.IsOpen.Should().BeTrue();
		command.OpenModel!.Document.Should().BeSameAs(tab);
		command.OpenModel.Items.Should().Contain(e => e.Name == "Diameter");

		// Accepting closes it.
		command.OpenModel.FilterText = "diameter";
		command.OpenModel.Accept().Should().BeTrue();
		await Waiters.WaitForAsync(() => command.OpenFlyout == null, description: "the popup to close");
	}

	[Test]
	public void Gesture_Is_Alt_Backslash()
	{
		// The backslash key of US and most European layouts reports as OemPipe (VK_OEM_5).
		var gesture = KeyGesture.Parse(GoToFileMemberCommand.Gesture);
		gesture.Key.Should().Be(Key.OemPipe);
		gesture.KeyModifiers.Should().Be(KeyModifiers.Alt);
	}

	[AvaloniaTest]
	public void Navigation_Gestures_Do_Not_Collide_With_Other_Commands()
	{
		var mine = new[] { TypeHierarchyGesture, FileStructureCommand.Gesture, GoToFileMemberCommand.Gesture }
			.Select(KeyGesture.Parse).ToArray();
		var mainMenu = AppComposition.Current.GetExport<MainMenuCommandRegistry>().Commands
			.Select(c => (Header: c.Metadata.Header, Text: c.Metadata.InputGestureText));
		var contextMenu = AppComposition.Current.GetExport<ContextMenuEntryRegistry>().Entries
			.Select(e => (Header: e.Metadata.Header, Text: e.Metadata.InputGestureText));
		var owners = new[] { nameof(Resources.TypeHierarchy), nameof(Resources.FileStructure), nameof(Resources.GoToFileMember) };

		foreach (var (header, text) in mainMenu.Concat(contextMenu))
		{
			if (string.IsNullOrEmpty(text) || owners.Contains(header))
				continue;
			KeyGesture other;
			try
			{
				other = KeyGesture.Parse(text);
			}
			catch (System.Exception)
			{
				continue; // descriptive text such as "MMB", not a key gesture
			}
			mine.Should().NotContain(other, $"'{header}' already uses {text}");
		}
	}

	static string TypeHierarchyGesture => TypeHierarchy.TypeHierarchyCommand.Gesture;
}
