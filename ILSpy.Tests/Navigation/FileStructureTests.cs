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

using System;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.Threading;

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.FileStructure;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.ViewModels;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class FileStructureTests
{
	internal static async Task<(MainWindowViewModel Vm, DecompilerTabPageModel Tab)> DecompileFixtureTypeAsync(string typeName)
	{
		var (_, vm) = await TestHarness.BootAsync();
		await HierarchyFixture.OpenAsync(vm);
		vm.AssemblyTreeModel.SelectNode(HierarchyFixture.FindType(vm, typeName));
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		tab.Text.Should().Contain($"class {typeName}");
		return (vm, tab);
	}

	static FileStructureViewModel ShowPane()
	{
		AppComposition.Current.GetExport<MainMenuCommandRegistry>()
			.GetCommand(nameof(Resources.FileStructure)).Execute(null);
		return AppComposition.Current.GetExport<FileStructureViewModel>();
	}

	[AvaloniaTest]
	public async Task Pane_Lists_Types_And_Members_Of_The_Active_Document_In_Document_Order()
	{
		// Arrange
		var (vm, tab) = await DecompileFixtureTypeAsync("Circle");

		// Act
		var pane = ShowPane();

		// Assert -- the pane is visible and lists the type first, then its members, each
		// pointing at the place its name is written.
		vm.DockWorkspace.ToolPaneMenuItems.Single(i => i.Title == Resources.FileStructure)
			.IsPaneVisible.Should().BeTrue();
		var entries = pane.Entries.ToList();
		entries.Select(e => e.Name).Should().ContainInOrder("Circle", "radius", "Area", "Diameter");
		entries.Select(e => e.Offset).Should().BeInAscendingOrder();
		entries.Should().OnlyContain(e => e.Icon != null);
		foreach (var entry in entries)
			tab.Text.AsSpan(entry.Offset).StartsWith(entry.Name.AsSpan(), StringComparison.Ordinal)
				.Should().BeTrue($"'{entry.Name}' must point at its definition");
		var type = entries.Single(e => e.Name == "Circle");
		type.Entity.Should().BeAssignableTo<ITypeDefinition>();
		entries.Where(e => e != type).Should().OnlyContain(e => e.Depth > type.Depth);
	}

	[AvaloniaTest]
	public async Task Activating_An_Entry_Moves_The_Caret_To_Its_Definition()
	{
		var (_, tab) = await DecompileFixtureTypeAsync("Circle");
		var pane = ShowPane();
		var diameter = pane.Entries.Single(e => e.Name == "Diameter");

		// Act
		pane.Activate(diameter).Should().BeTrue();

		// Assert
		tab.CaptureViewState.Should().NotBeNull("the text view must be attached to the active tab");
		tab.CaptureViewState!().CaretOffset.Should().Be(diameter.Offset);
	}

	[AvaloniaTest]
	public async Task Pane_View_Lists_The_Entries_And_Enter_Moves_The_Caret()
	{
		var (_, tab) = await DecompileFixtureTypeAsync("Circle");
		var pane = ShowPane();
		var window = AppComposition.Current.GetExport<global::ICSharpCode.ILSpy.Views.MainWindow>();
		var view = await window.WaitForComponent<FileStructureView>();
		var list = view.FindControl<ListBox>("EntryList")!;
		list.ItemsSource.Should().BeSameAs(pane.Entries);
		var area = pane.Entries.Single(e => e.Name == "Area");

		// Act -- select Area and press Enter in the list.
		pane.SelectedEntry = area;
		Dispatcher.UIThread.RunJobs();
		list.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Enter, Source = list });

		// Assert
		tab.CaptureViewState!().CaretOffset.Should().Be(area.Offset);
	}

	[AvaloniaTest]
	public async Task Pane_Follows_The_Active_Document_Content()
	{
		var (vm, _) = await DecompileFixtureTypeAsync("Circle");
		var pane = ShowPane();
		pane.Entries.Should().Contain(e => e.Name == "Circle");

		// Act -- navigate the same tab to another type.
		vm.AssemblyTreeModel.SelectNode(HierarchyFixture.FindType(vm, "Square"));
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		// Assert
		await Waiters.WaitForAsync(() => pane.Entries.Any(e => e.Name == "Square"),
			description: "the file structure to show the newly decompiled type");
		pane.Entries.Should().NotContain(e => e.Name == "Circle");
	}

	[AvaloniaTest]
	public async Task IL_Output_Lists_Definitions_With_Their_Metadata_Names()
	{
		// The IL view writes unresolved metadata references; the outline still names them.
		var (_, vm) = await TestHarness.BootAsync();
		await HierarchyFixture.OpenAsync(vm);
		var language = AppComposition.Current.GetExport<global::ICSharpCode.ILSpy.Languages.LanguageService>();
		language.CurrentLanguage = language.Languages.Single(l => l.Name == "IL");
		try
		{
			vm.AssemblyTreeModel.SelectNode(HierarchyFixture.FindType(vm, "Circle"));
			var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
			tab.Text.Should().Contain(".class");

			var pane = ShowPane();

			pane.Entries.Select(e => e.Name).Should().ContainInOrder("Circle", "radius", "Area", "Diameter");
			pane.Entries.Should().OnlyContain(e => e.Icon != null);
		}
		finally
		{
			language.CurrentLanguage = language.Languages.Single(l => l.Name == "C#");
		}
	}

	[Test]
	public void Gesture_Is_Ctrl_Alt_F()
	{
		var gesture = KeyGesture.Parse(FileStructureCommand.Gesture);
		gesture.Key.Should().Be(Key.F);
		gesture.KeyModifiers.Should().Be(KeyModifiers.Control | KeyModifiers.Alt);
	}
}
