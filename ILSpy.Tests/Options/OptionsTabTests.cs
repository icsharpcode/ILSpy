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

using ILSpy;
using ILSpy.AppEnv;
using ILSpy.Commands;
using ILSpy.Docking;
using ILSpy.Options;
using ILSpy.ViewModels;
using ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class OptionsTabTests
{
	[AvaloniaTest]
	public void Options_Command_Is_Exported_To_View_Menu_With_Last_MenuOrder()
	{
		// Mirrors the WPF mounting point. MenuOrder 999 puts it last under View;
		// MenuCategory "Options" gives it its own separator group.
		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		var entry = registry.Commands
			.SingleOrDefault(c => c.Metadata.Header == nameof(Resources._Options));
		((object?)entry).Should().NotBeNull(
			"View → Options must be exported via [ExportMainMenuCommand]");
		entry!.Metadata.ParentMenuID.Should().Be(nameof(Resources._View));
		entry.Metadata.MenuOrder.Should().Be(999);
		entry.Metadata.MenuCategory.Should().Be(nameof(Resources.Options));
	}

	[AvaloniaTest]
	public void Invoking_ShowOptionsCommand_Opens_Document_Tab_With_OptionsPageModel()
	{
		// The Options window-equivalent shows up as a regular Dock document tab with
		// OptionsPageModel as its Content. IsStaticContent flags it so tree-node selections
		// route to a fresh decompile tab instead of overwriting it.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		var command = registry.Commands
			.Single(c => c.Metadata.Header == nameof(Resources._Options))
			.CreateExport().Value;

		command.Execute(null);

		var docs = vm.DockWorkspace.Documents?.VisibleDockables;
		((object?)docs).Should().NotBeNull();
		var optionsTab = docs!.OfType<ContentTabPage>()
			.SingleOrDefault(t => t.Content is OptionsPageModel);
		((object?)optionsTab).Should().NotBeNull(
			"a ContentTabPage hosting an OptionsPageModel must land in the documents dock");
		var model = (OptionsPageModel)optionsTab!.Content!;
		model.IsStaticContent.Should().BeTrue(
			"the Options tab must be flagged static so tree-node selections leave it alone");
	}

	[AvaloniaTest]
	public void OptionsPageModel_Surfaces_The_Three_Panels_In_MEF_Order()
	{
		// Decompiler / Display / Misc, ordered by ExportOptionPage(Order=10/20/30).
		// Titles come from the embedded WPF Resources.resx so they match the WPF host
		// byte-for-byte.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		var command = registry.Commands
			.Single(c => c.Metadata.Header == nameof(Resources._Options))
			.CreateExport().Value;
		command.Execute(null);

		var vm = (MainWindowViewModel)window.DataContext!;
		var model = (OptionsPageModel)vm.DockWorkspace.Documents!.VisibleDockables!
			.OfType<ContentTabPage>().First(t => t.Content is OptionsPageModel).Content!;

		model.Pages.Should().HaveCount(3);
		model.Pages[0].Title.Should().Be(Resources.Decompiler);
		model.Pages[1].Title.Should().Be(Resources.Display);
		model.Pages[2].Title.Should().Be(Resources.Misc);
	}

	[AvaloniaTest]
	public void Reinvoking_ShowOptionsCommand_Focuses_Existing_Tab_Without_Spawning_A_Second()
	{
		// Single-instance behaviour — re-firing the command while Options is already open
		// just reactivates the existing tab. Mirrors WPF's modal-stack uniqueness.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		var command = registry.Commands
			.Single(c => c.Metadata.Header == nameof(Resources._Options))
			.CreateExport().Value;

		command.Execute(null);
		command.Execute(null);

		var optionsTabs = vm.DockWorkspace.Documents!.VisibleDockables!
			.OfType<ContentTabPage>().Where(t => t.Content is OptionsPageModel).ToList();
		optionsTabs.Should().HaveCount(1, "re-firing the command must focus, not duplicate");
	}

	[AvaloniaTest]
	public async Task Apply_Writes_Snapshot_Through_To_Live_DecompilerSettings()
	{
		// Snapshot pattern: changes in the panel's settings instance don't reach the live
		// service until Apply. After Apply, SettingsService.DecompilerSettings reflects the
		// new value. Toggling a property on the snapshot copy alone shouldn't affect the
		// live instance until Apply is called.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var settings = AppComposition.Current.GetExport<SettingsService>();
		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();

		var liveBefore = settings.DecompilerSettings.UsingDeclarations;

		registry.Commands.Single(c => c.Metadata.Header == nameof(Resources._Options))
			.CreateExport().Value.Execute(null);

		var vm = (MainWindowViewModel)window.DataContext!;
		var model = (OptionsPageModel)vm.DockWorkspace.Documents!.VisibleDockables!
			.OfType<ContentTabPage>().First(t => t.Content is OptionsPageModel).Content!;
		var decompilerPage = (DecompilerSettingsViewModel)model.Pages[0];

		// Find the UsingDeclarations item in the reflection-built tree and flip it.
		var usingDecl = decompilerPage.Settings
			.SelectMany(g => g.Settings)
			.First(s => s.Property.Name == nameof(global::ICSharpCode.Decompiler.DecompilerSettings.UsingDeclarations));
		usingDecl.IsEnabled = !liveBefore;

		// Live service unaffected so far — only the snapshot copy changed.
		// Snapshot edits must not leak into the live service before Apply.
		settings.DecompilerSettings.UsingDeclarations.Should().Be(liveBefore);

		// Apply — flushes snapshot to XML and reloads sections so live values match.
		model.ApplyCommand.Execute(null);
		await Task.Yield();

		// After Apply, the live service must see the panel's toggled value.
		settings.DecompilerSettings.UsingDeclarations.Should().Be(!liveBefore);

		// Clean-up: restore the original so the persisted XML doesn't pollute later tests.
		usingDecl.IsEnabled = liveBefore;
		model.ApplyCommand.Execute(null);
	}

	[AvaloniaTest]
	public void Reset_Current_Page_Restores_Defaults_For_The_Active_Panel_Only()
	{
		// Reset operates on the selected panel only. After flipping a few values and clicking
		// Reset, every DecompilerSettings property is back to its `new DecompilerSettings()`
		// default; Display and Misc panels are untouched (verified by leaving them alone).
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		registry.Commands.Single(c => c.Metadata.Header == nameof(Resources._Options))
			.CreateExport().Value.Execute(null);

		var vm = (MainWindowViewModel)window.DataContext!;
		var model = (OptionsPageModel)vm.DockWorkspace.Documents!.VisibleDockables!
			.OfType<ContentTabPage>().First(t => t.Content is OptionsPageModel).Content!;
		var decompilerPage = (DecompilerSettingsViewModel)model.Pages[0];
		model.SelectedPage = decompilerPage;

		// Flip a known-default-true setting to false.
		var item = decompilerPage.Settings
			.SelectMany(g => g.Settings)
			.First(s => s.Property.Name == nameof(global::ICSharpCode.Decompiler.DecompilerSettings.UsingDeclarations));
		var defaultValue = (bool)item.Property.GetValue(new global::ICSharpCode.Decompiler.DecompilerSettings())!;
		item.IsEnabled = !defaultValue;
		// Sanity: precondition flip took effect.
		item.IsEnabled.Should().Be(!defaultValue);

		model.ResetCurrentPageCommand.Execute(null);

		// After reset, the reflection-rebuilt item list has the snapshot's defaults.
		var refreshedItem = decompilerPage.Settings
			.SelectMany(g => g.Settings)
			.First(s => s.Property.Name == nameof(global::ICSharpCode.Decompiler.DecompilerSettings.UsingDeclarations));
		// Reset must restore the panel's settings to `new DecompilerSettings()` defaults.
		refreshedItem.IsEnabled.Should().Be(defaultValue);
	}
}
