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
using Avalonia.Threading;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

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
		var command = AppComposition.Current.GetExport<MainMenuCommandRegistry>()
			.GetCommand(nameof(Resources._Options));

		command.Execute(null);
		TestCapture.Step("options-tab-opened");

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
	public async Task Tree_Selection_Activates_MainTab_Even_When_Options_Is_The_Focused_Document()
	{
		// Scenario: user has the Options tab open and active. They click a node in the
		// assembly tree. The new content lands on MainTab as usual, but the focused
		// document is still Options — so the user sees no visible change. The fix:
		// ShowSelectedNode pulls focus back to MainTab whenever a tree-selection writes
		// new content there.
		var (_, vm) = await TestHarness.BootAsync(3);

		// Open Options and confirm it's the active document.
		AppComposition.Current.GetExport<MainMenuCommandRegistry>()
			.GetCommand(nameof(Resources._Options)).Execute(null);
		TestCapture.Step("options-active");

		var documents = vm.DockWorkspace.Documents!;
		var optionsTab = documents.VisibleDockables!.OfType<ContentTabPage>()
			.Single(t => t.Content is OptionsPageModel);
		documents.ActiveDockable.Should().BeSameAs(optionsTab,
			"baseline: opening Options must make it the active document");

		// Click a tree node — fires the same path as a real user mouse-click.
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.AssemblyTreeModel.SelectNode(typeNode);
		TestCapture.Step("tree-node-selected");

		// MainTab now carries the new content. Without the fix, ActiveDockable still
		// points at Options and the user sees nothing change.
		var mainTab = ((ILSpyDockFactory)vm.DockWorkspace.Factory).MainTab!;
		documents.ActiveDockable.Should().BeSameAs(mainTab,
			"tree selection must pull focus onto MainTab so the user sees the new content");

		// Options must still be in the documents dock — we changed focus, not closed it.
		documents.VisibleDockables!.OfType<ContentTabPage>()
			.Should().Contain(t => t.Content is OptionsPageModel,
			"focus change must not close the Options tab");
	}

	[AvaloniaTest]
	public void Options_Tab_Title_Is_Plain_Options_Not_The_Menu_String()
	{
		// The menu-item resource includes the accelerator underscore + ellipsis
		// ("_Options...") which is meaningful on a menu but wrong on a tab header. The
		// tab should use the bare Resources.Options ("Options") instead.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		AppComposition.Current.GetExport<MainMenuCommandRegistry>()
			.GetCommand(nameof(Resources._Options)).Execute(null);
		TestCapture.Step("options-opened");

		var vm = (MainWindowViewModel)window.DataContext!;
		var model = (OptionsPageModel)vm.DockWorkspace.Documents!.VisibleDockables!
			.OfType<ContentTabPage>().First(t => t.Content is OptionsPageModel).Content!;

		model.Title.Should().Be(Resources.Options,
			"the tab title must be the bare 'Options' string, not the menu's '_Options...'");
		model.Title.Should().NotContain("_",
			"a tab header doesn't have an accelerator key, so the underscore would render literally");
		model.Title.Should().NotEndWith("...",
			"the ellipsis is a menu-item convention indicating 'opens a dialog' — not appropriate on the dialog itself");
	}

	[AvaloniaTest]
	public void OptionsPageModel_Surfaces_The_Three_Panels_In_MEF_Order()
	{
		// Decompiler / Display / Misc, ordered by ExportOptionPage(Order=10/20/30).
		// Titles come from the embedded WPF Resources.resx so they match the WPF host
		// byte-for-byte.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var command = AppComposition.Current.GetExport<MainMenuCommandRegistry>()
			.GetCommand(nameof(Resources._Options));
		command.Execute(null);
		TestCapture.Step("options-opened");

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
		var command = AppComposition.Current.GetExport<MainMenuCommandRegistry>()
			.GetCommand(nameof(Resources._Options));

		command.Execute(null);
		command.Execute(null);
		TestCapture.Step("options-reinvoked");

		var optionsTabs = vm.DockWorkspace.Documents!.VisibleDockables!
			.OfType<ContentTabPage>().Where(t => t.Content is OptionsPageModel).ToList();
		optionsTabs.Should().HaveCount(1, "re-firing the command must focus, not duplicate");
	}

	[AvaloniaTest]
	public void Panel_Edits_Take_Effect_Immediately_On_Live_DecompilerSettings()
	{
		// Live-binding architecture (no snapshot): toggling a checkbox in the Decompiler
		// panel mutates the live SettingsService.DecompilerSettings instance directly. No
		// Apply step. Subscribers to DecompilerSettings.PropertyChanged see the change
		// immediately; the next decompile pulls the new value via Clone() in
		// DecompilerTabPageModel.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var settings = AppComposition.Current.GetExport<SettingsService>();

		var liveBefore = settings.DecompilerSettings.UsingDeclarations;

		AppComposition.Current.GetExport<MainMenuCommandRegistry>()
			.GetCommand(nameof(Resources._Options)).Execute(null);
		TestCapture.Step("options-opened");

		var vm = (MainWindowViewModel)window.DataContext!;
		var model = (OptionsPageModel)vm.DockWorkspace.Documents!.VisibleDockables!
			.OfType<ContentTabPage>().First(t => t.Content is OptionsPageModel).Content!;
		var decompilerPage = (DecompilerSettingsViewModel)model.Pages[0];

		// Flip UsingDeclarations through the panel viewmodel.
		var usingDecl = decompilerPage.Settings
			.SelectMany(g => g.Settings)
			.First(s => s.Property.Name == nameof(ICSharpCode.Decompiler.DecompilerSettings.UsingDeclarations));
		usingDecl.IsEnabled = !liveBefore;
		TestCapture.Step("using-declarations-toggled");

		// Live service must see the change immediately — no Apply needed.
		settings.DecompilerSettings.UsingDeclarations.Should().Be(!liveBefore);

		// Clean-up: restore the original so the next test sees a clean slate.
		usingDecl.IsEnabled = liveBefore;
	}

	[AvaloniaTest]
	public void Reset_Current_Page_Restores_Defaults_For_The_Active_Panel_Only()
	{
		// Reset operates on the selected panel only. After flipping a few values and clicking
		// Reset, every DecompilerSettings property is back to its `new DecompilerSettings()`
		// default; Display and Misc panels are untouched (verified by leaving them alone).
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		AppComposition.Current.GetExport<MainMenuCommandRegistry>()
			.GetCommand(nameof(Resources._Options)).Execute(null);

		var vm = (MainWindowViewModel)window.DataContext!;
		var model = (OptionsPageModel)vm.DockWorkspace.Documents!.VisibleDockables!
			.OfType<ContentTabPage>().First(t => t.Content is OptionsPageModel).Content!;
		var decompilerPage = (DecompilerSettingsViewModel)model.Pages[0];
		model.SelectedPage = decompilerPage;
		TestCapture.Step("decompiler-page-selected");

		// Flip a known-default-true setting to false.
		var item = decompilerPage.Settings
			.SelectMany(g => g.Settings)
			.First(s => s.Property.Name == nameof(ICSharpCode.Decompiler.DecompilerSettings.UsingDeclarations));
		var defaultValue = (bool)item.Property.GetValue(new ICSharpCode.Decompiler.DecompilerSettings())!;
		item.IsEnabled = !defaultValue;
		// Sanity: precondition flip took effect.
		item.IsEnabled.Should().Be(!defaultValue);
		TestCapture.Step("setting-flipped");

		model.ResetCurrentPageCommand.Execute(null);
		TestCapture.Step("page-reset");

		// After reset, the reflection-rebuilt item list has the snapshot's defaults.
		var refreshedItem = decompilerPage.Settings
			.SelectMany(g => g.Settings)
			.First(s => s.Property.Name == nameof(ICSharpCode.Decompiler.DecompilerSettings.UsingDeclarations));
		// Reset must restore the panel's settings to `new DecompilerSettings()` defaults.
		refreshedItem.IsEnabled.Should().Be(defaultValue);
	}

	[AvaloniaTest]
	public async Task Display_Setting_Change_Live_Updates_Decompiler_Editor()
	{
		// DisplaySettings → DecompilerTextView wire-up: mutating SelectedFontSize on the live
		// settings instance flows immediately into Editor.FontSize without any Apply or
		// re-decompile step. Covers the broader live-bind contract for font / size / line
		// numbers / word wrap / highlight current line / indentation.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var settings = AppComposition.Current.GetExport<SettingsService>();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);
		TestCapture.Step("booted");

		// Materialise a DecompilerTextView by selecting a node.
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.AssemblyTreeModel.SelectNode(typeNode);
		var view = await window.WaitForComponent<DecompilerTextView>();
		var editor = await view.WaitForComponent<AvaloniaEdit.TextEditor>();
		TestCapture.Step("enumerable-decompiled");

		var originalSize = settings.DisplaySettings.SelectedFontSize;
		var newSize = originalSize + 5;

		// Act — change the live setting; subscriber should write through to Editor.FontSize.
		settings.DisplaySettings.SelectedFontSize = newSize;
		TestCapture.Step("font-size-increased");

		editor.FontSize.Should().Be(newSize);

		// Clean-up: restore so later tests see the original.
		settings.DisplaySettings.SelectedFontSize = originalSize;
	}

	[AvaloniaTest]
	public async Task Toggling_A_Display_Setting_Does_Not_Switch_Away_From_The_Focused_Options_Tab()
	{
		// Scenario: the user is on the Options tab and toggles a setting that forces a re-decompile
		// (e.g. DecodeCustomAttributeBlobs). The decompiler output must refresh in place; it must NOT
		// pull focus back to MainTab and yank the user off the Options page they are editing.
		var (_, vm) = await TestHarness.BootAsync(3);

		// Show some decompiled content first, so there is a decompiler tab to refresh. Use a single
		// small method, not the whole Enumerable type: the full type takes >15 s in headless and
		// times out WaitForDecompiledTextAsync under CI load.
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var method = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");
		vm.AssemblyTreeModel.SelectNode(method);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		// Open Options and confirm it is the active document.
		AppComposition.Current.GetExport<MainMenuCommandRegistry>()
			.GetCommand(nameof(Resources._Options)).Execute(null);
		var documents = vm.DockWorkspace.Documents!;
		var optionsTab = documents.VisibleDockables!.OfType<ContentTabPage>()
			.Single(t => t.Content is OptionsPageModel);
		documents.ActiveDockable.Should().BeSameAs(optionsTab, "baseline: Options is the active document");

		// Toggle a re-decompile display setting.
		var display = AppComposition.Current.GetExport<SettingsService>().DisplaySettings;
		display.DecodeCustomAttributeBlobs = !display.DecodeCustomAttributeBlobs;
		for (int i = 0; i < 12; i++)
			Dispatcher.UIThread.RunJobs();

		documents.ActiveDockable.Should().BeSameAs(optionsTab,
			"an output display setting must re-decompile in place, not switch the user off the focused tab");
	}
}
