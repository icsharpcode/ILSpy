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
using System.Reflection;

using Avalonia.Controls;
using Avalonia.Media;

using ILSpy.AppEnv;
using ILSpy.Commands;
using ILSpy.ViewModels;

namespace ILSpy;

public partial class MainToolBar : UserControl
{
	const int MaxHistoryDropdownEntries = 20;

	bool initialized;

	public MainToolBar()
	{
		InitializeComponent();
		Loaded += (_, _) => {
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
		foreach (var node in entries.Reverse().Take(MaxHistoryDropdownEntries))
		{
			var item = new MenuItem {
				Header = node.Text?.ToString() ?? string.Empty,
				Command = vm.DockWorkspace.NavigateToHistoryCommand,
				CommandParameter = node,
			};
			if (node.Icon is IImage icon)
				item.Icon = new Image { Width = 16, Height = 16, Source = icon };
			menu.Items.Add(item);
		}
	}

	void InitializeButtons()
	{
		if (initialized)
			return;
		initialized = true;

		ToolbarCommandRegistry registry;
		try
		{
			registry = AppComposition.Current.GetExport<ToolbarCommandRegistry>();
		}
		catch
		{
			// Design-time / test contexts that bypass composition just keep the static layout.
			return;
		}

		int insertAt = ToolbarRoot.Children.IndexOf(ToolbarMefAnchor);
		if (insertAt < 0)
			return;

		// Insert one Button per [ExportToolbarCommand], grouped by Category, separated by
		// a thin Separator between categories. Order within a category honours ToolbarOrder.
		var groups = registry.Commands
			.GroupBy(c => c.Metadata.ToolbarCategory ?? string.Empty)
			.OrderBy(g => g.Key);
		bool firstGroup = true;
		foreach (var group in groups)
		{
			if (!firstGroup)
				ToolbarRoot.Children.Insert(insertAt++, new Separator());
			firstGroup = false;
			foreach (var entry in group.OrderBy(c => c.Metadata.ToolbarOrder))
			{
				var button = BuildButton(entry);
				if (button != null)
					ToolbarRoot.Children.Insert(insertAt++, button);
			}
		}
	}

	static Button? BuildButton(System.Composition.ExportFactory<System.Windows.Input.ICommand, ToolbarCommandMetadata> entry)
	{
		var command = entry.CreateExport().Value;
		var button = new Button {
			Tag = entry.Metadata.ToolTip,
			Command = command,
		};
		var icon = ResolveIcon(entry.Metadata.ToolbarIcon);
		if (icon != null)
		{
			button.Content = new Image {
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
	// stay declarative.
	static IImage? ResolveIcon(string? iconPath)
	{
		if (string.IsNullOrEmpty(iconPath))
			return null;
		var name = iconPath.StartsWith("Images/", System.StringComparison.Ordinal)
			? iconPath["Images/".Length..]
			: iconPath;
		var prop = typeof(Images.Images).GetField(name, BindingFlags.Public | BindingFlags.Static);
		return prop?.GetValue(null) as IImage;
	}
}
