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
using System.Composition;
using System.Linq;
using System.Reflection;

using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views.Controls;

namespace ICSharpCode.ILSpy;

public partial class MainToolBar : UserControl
{
	const int MaxHistoryDropdownEntries = 20;

	bool initialized;

	public MainToolBar()
	{
		InitializeComponent();
		// AttachedToVisualTree fires reliably in both the live app and the Avalonia.Headless
		// test harness; Loaded did not in headless, leaving the MEF anchors un-replaced.
		AttachedToVisualTree += (_, _) => {
			InitializeButtons();
			WireHistoryUpdates();
		};
	}

	void WireHistoryUpdates()
	{
		if (DataContext is not MainWindowViewModel vm)
			return;
		// Repopulate the menus eagerly on every history change. Doing this in the Flyout's
		// Opened event would arrive too late — MenuFlyout's presenter has already been laid
		// out by then and won't re-measure for items added afterward, so the popup would
		// appear empty. NavigateBack/ForwardCommand.CanExecuteChanged fires on every push,
		// pop, and Record (see DockWorkspace.RecordHistory / ApplyNavigationTarget), so it's
		// the right signal.
		void OnHistoryChanged(object? _, EventArgs __)
		{
			RebuildHistoryMenu(BackSplitButton, vm, forward: false);
			RebuildHistoryMenu(ForwardSplitButton, vm, forward: true);
		}
		vm.DockWorkspace.NavigateBackCommand.CanExecuteChanged += OnHistoryChanged;
		vm.DockWorkspace.NavigateForwardCommand.CanExecuteChanged += OnHistoryChanged;
		OnHistoryChanged(null, EventArgs.Empty);
	}

	void RebuildHistoryMenu(SplitButton splitButton, MainWindowViewModel vm, bool forward)
	{
		var menu = (MenuFlyout)splitButton.Flyout!;
		menu.Items.Clear();
		var entries = forward ? vm.DockWorkspace.ForwardHistory : vm.DockWorkspace.BackHistory;
		// Stacks are oldest-first; reverse so the most recent appears at the top of the menu.
		// WPF caps the dropdown at 20 entries; mirror that.
		foreach (var entry in entries.Reverse().Take(MaxHistoryDropdownEntries))
		{
			var item = new MenuItem {
				Header = entry.DisplayText?.ToString() ?? string.Empty,
				Command = vm.DockWorkspace.NavigateToHistoryCommand,
				CommandParameter = entry,
			};
			if (entry.DisplayIcon is { } icon)
				item.Icon = new Image { Width = 16, Height = 16, Source = icon };
			menu.Items.Add(item);
		}
	}

	void InitializeButtons()
	{
		if (initialized)
			return;
		initialized = true;

		ToolbarCommandRegistry? toolbarRegistry = null;
		MainMenuCommandRegistry? menuRegistry = null;
		try
		{
			toolbarRegistry = AppComposition.Current.GetExport<ToolbarCommandRegistry>();
			menuRegistry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		}
		catch
		{
			// Design-time / test contexts that bypass composition keep the static layout.
		}

		if (toolbarRegistry != null)
			DispatchToolbarCommands(toolbarRegistry);

		if (menuRegistry != null)
			WireManageAssemblyListsButton(menuRegistry);

		WireLanguageVersionComboSync();
	}

	void WireLanguageVersionComboSync()
	{
		// The version ComboBox binds ItemsSource to CurrentLanguage.LanguageVersions and SelectedItem
		// to CurrentVersion. On a language switch the model assigns CurrentVersion (pushing it onto
		// SelectedItem) before the CurrentLanguage change propagates to ItemsSource, so SelectedItem
		// is set against the previous language's list, rejected, and left null once ItemsSource
		// repopulates. Re-assert the selection from the bound CurrentVersion after each ItemsSource
		// swap has settled so flipping to a version-less language (e.g. IL) and back restores it.
		LanguageVersionComboBox.PropertyChanged += (_, e) => {
			if (e.Property != ItemsControl.ItemsSourceProperty)
				return;
			Dispatcher.UIThread.Post(() => {
				if (DataContext is MainWindowViewModel vm)
					LanguageVersionComboBox.SelectedItem = vm.LanguageService.CurrentVersion;
			}, DispatcherPriority.Background);
		};
	}

	void DispatchToolbarCommands(ToolbarCommandRegistry registry)
	{
		// Mirrors the WPF dispatch in MainToolBar.xaml.cs:55-90 — Navigation is hand-coded
		// (Back/Forward SplitButtons in the AXAML), Open goes at OpenCategoryAnchor, every
		// other category (View etc.) is appended after ViewCategoryAnchor.
		var byCategory = registry.Commands
			.GroupBy(c => c.Metadata.ToolbarCategory ?? string.Empty)
			.ToDictionary(g => g.Key, g => g.OrderBy(c => c.Metadata.ToolbarOrder).ToList());

		var openCategory = nameof(ICSharpCode.ILSpy.Properties.Resources.Open);
		var viewCategory = nameof(ICSharpCode.ILSpy.Properties.Resources.View);

		if (byCategory.TryGetValue(openCategory, out var openCommands))
			ReplaceAnchor(OpenCategoryAnchor, openCommands, leadingSeparator: false);

		// "View" group + any other unknown categories go at the View anchor with a leading
		// Separator so they read as a distinct trailing group.
		var trailingCategories = byCategory
			.Where(kv => kv.Key != openCategory)
			.OrderBy(kv => kv.Key == viewCategory ? 0 : 1)
			.ThenBy(kv => kv.Key)
			.ToList();

		if (trailingCategories.Count == 0)
		{
			// Hide the bare anchor so we don't leave a trailing Separator on its own.
			ToolbarRoot.Children.Remove(ViewCategoryAnchor);
			return;
		}

		var firstTrailingCommands = trailingCategories[0].Value;
		ReplaceAnchor(ViewCategoryAnchor, firstTrailingCommands, leadingSeparator: true);

		// Any additional categories (rare, only via plugins) get their own separator + buttons
		// appended at the end so the WPF "else: add Separator + commands" branch is preserved.
		for (int i = 1; i < trailingCategories.Count; i++)
		{
			ToolbarRoot.Children.Add(new Separator());
			foreach (var entry in trailingCategories[i].Value)
			{
				var button = BuildButton(entry);
				if (button != null)
					ToolbarRoot.Children.Add(button);
			}
		}
	}

	void ReplaceAnchor(
		Separator anchor,
		IReadOnlyList<ExportFactory<System.Windows.Input.ICommand, ToolbarCommandMetadata>> commands,
		bool leadingSeparator)
	{
		var index = ToolbarRoot.Children.IndexOf(anchor);
		if (index < 0)
			return;
		ToolbarRoot.Children.RemoveAt(index);
		int insertAt = index;
		if (leadingSeparator)
			ToolbarRoot.Children.Insert(insertAt++, new Separator());
		foreach (var entry in commands)
		{
			var button = BuildButton(entry);
			if (button != null)
				ToolbarRoot.Children.Insert(insertAt++, button);
		}
	}

	void WireManageAssemblyListsButton(MainMenuCommandRegistry registry)
	{
		// Match the export by its main-menu header — File > Manage Assembly Lists.
		var entry = registry.Commands.FirstOrDefault(
			c => c.Metadata.Header == nameof(ICSharpCode.ILSpy.Properties.Resources.ManageAssembly_Lists));
		if (entry == null)
			return;
		ManageAssemblyListsButton.Command = entry.CreateExport().Value;
		ToolTip.SetTip(ManageAssemblyListsButton, ICSharpCode.ILSpy.Properties.Resources.ManageAssemblyLists);
	}

	static Button? BuildButton(ExportFactory<System.Windows.Input.ICommand, ToolbarCommandMetadata> entry)
	{
		System.Windows.Input.ICommand command;
		try
		{
			// Isolate each entry: a misbehaving plugin command (e.g. a DI ctor without
			// [ImportingConstructor]) must not take down the whole toolbar.
			command = entry.CreateExport().Value;
		}
		catch (System.Exception ex)
		{
			ICSharpCode.ILSpy.AppEnv.CompositionErrors.Report($"Toolbar command '{entry.Metadata.ToolTip}'", ex);
			return null;
		}
		var button = new Button {
			Tag = entry.Metadata.ToolTip,
			Command = command,
		};
		var icon = ResolveIcon(entry.Metadata.ToolbarIcon);
		if (icon != null)
		{
			button.Content = new GrayscaleAwareImage {
				Width = 16,
				Height = 16,
				Source = icon,
			};
		}
		else if (entry.Metadata.ToolTip is { } label)
		{
			button.Content = ResourceHelper.GetString(label);
		}
		var tooltip = ResourceHelper.GetString(entry.Metadata.ToolTip);
		if (!string.IsNullOrEmpty(tooltip))
			ToolTip.SetTip(button, tooltip);
		return button;
	}

	// "Images/Open" → looks up "Open" on Images.Images via reflection so the metadata can
	// stay declarative. Delegates to the shared resolver in Images.cs, which other menu
	// builders (MainMenu, ContextMenuProvider) also consume.
	static IImage? ResolveIcon(string? iconPath) => Images.ResolveByPath(iconPath);
}
