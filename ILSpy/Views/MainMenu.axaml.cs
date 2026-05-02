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

using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Windows.Input;

using global::Avalonia.Controls;
using global::Avalonia.Data;
using global::Avalonia.Input;

using ILSpy.AppEnv;
using ILSpy.Commands;
using ILSpy.Docking;
using ILSpy.Themes;
using ILSpy.ViewModels;

namespace ILSpy;

public partial class MainMenu : UserControl
{
	bool initialized;

	public MainMenu()
	{
		InitializeComponent();

		// Tests / design-time previews don't bootstrap the composition host; bail out so the
		// XAML can still render (the static View entries don't need DI).
		if (!TryGetSettingsService(out var settings))
			return;

		DataContext = settings.SessionSettings;
		Loaded += (_, _) => InitializeMenus();
	}

	void InitializeMenus()
	{
		if (initialized)
			return;
		initialized = true;

		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		var dockWorkspace = AppComposition.Current.GetExport<DockWorkspace>();
		var setThemeCommand = AppComposition.Current.GetExport<SetThemeCommand>();

		PopulateThemeSubmenu(setThemeCommand);
		InitMainMenu(MainMenuRoot, registry.Commands);
		InitWindowMenu(WindowMenuItem, dockWorkspace);
	}

	static bool TryGetSettingsService(out SettingsService settings)
	{
		try
		{
			settings = AppComposition.Current.GetExport<SettingsService>();
			return settings != null;
		}
		catch
		{
			settings = null!;
			return false;
		}
	}

	void PopulateThemeSubmenu(SetThemeCommand setThemeCommand)
	{
		var current = (SessionSettings)DataContext!;
		var items = new List<MenuItem>();
		foreach (var theme in ThemeManager.AllThemes)
		{
			var item = new MenuItem {
				Header = theme,
				Command = setThemeCommand,
				CommandParameter = theme,
				ToggleType = MenuItemToggleType.Radio,
			};
			// Track the active theme via a one-way binding on Theme; setting from the menu
			// flows back through CommandParameter, not through IsChecked.
			item.Bind(MenuItem.IsCheckedProperty, new Binding(nameof(SessionSettings.Theme)) {
				Source = current,
				Converter = ThemeEqualityConverter.Instance,
				ConverterParameter = theme,
				Mode = BindingMode.OneWay,
			});
			items.Add(item);
		}
		ThemeMenuItem.ItemsSource = items;
	}

	static void InitMainMenu(Menu mainMenu, IReadOnlyList<ExportFactory<ICommand, MainMenuCommandMetadata>> mainMenuCommands)
	{
		var parentMenuItems = new Dictionary<string, MenuItem>();
		var menuGroups = mainMenuCommands
			.OrderBy(c => c.Metadata?.MenuOrder)
			.GroupBy(c => c.Metadata?.ParentMenuID ?? string.Empty)
			.ToArray();

		foreach (var menu in menuGroups)
		{
			var parentMenuItem = GetOrAddParentMenuItem(menu.Key, menu.Key);
			foreach (var category in menu.GroupBy(c => c.Metadata?.MenuCategory))
			{
				if (parentMenuItem.Items.Count > 0)
					parentMenuItem.Items.Add(new Separator { Tag = category.Key });

				foreach (var entry in category)
				{
					var entryMenuId = entry.Metadata?.MenuID;
					if (entryMenuId != null && menuGroups.Any(g => g.Key == entryMenuId))
					{
						// This entry's contract is the parent of another group — surface it as a submenu.
						var nested = GetOrAddParentMenuItem(entryMenuId, entry.Metadata?.Header);
						nested.Header = ResourceHelper.GetString(entry.Metadata?.Header);
						parentMenuItem.Items.Add(nested);
					}
					else
					{
						var command = entry.CreateExport().Value;

						var menuItem = new MenuItem {
							Command = command,
							Tag = entry.Metadata?.MenuID,
							Header = ResourceHelper.GetString(entry.Metadata?.Header),
							IsEnabled = entry.Metadata?.IsEnabled ?? true,
						};

						// Wire up the keyboard accelerator if the export declared one. HotKey
						// registers the window-scoped shortcut; InputGesture is what actually
						// renders the gesture text on the right side of the menu item — both
						// are needed for "Ctrl+O" to appear AND fire from the keyboard.
						if (TryParseGesture(entry.Metadata?.InputGestureText, out var gesture))
						{
							menuItem.HotKey = gesture;
							menuItem.InputGesture = gesture;
						}

						if (command is IProvideParameterBinding parameterBinding)
							menuItem.Bind(MenuItem.CommandParameterProperty, parameterBinding.ParameterBinding);

						parentMenuItem.Items.Add(menuItem);
					}
				}
			}
		}

		// Attach any newly-created top-level menus (those not already declared in XAML).
		foreach (var item in parentMenuItems.Values.Where(item => item.Parent == null))
			mainMenu.Items.Add(item);

		MenuItem GetOrAddParentMenuItem(string menuId, string? resourceKey)
		{
			if (!parentMenuItems.TryGetValue(menuId, out var parentMenuItem))
			{
				var topLevelMenuItem = mainMenu.Items.OfType<MenuItem>().FirstOrDefault(m => (string?)m.Tag == menuId);
				if (topLevelMenuItem == null)
				{
					parentMenuItem = new MenuItem {
						Header = ResourceHelper.GetString(resourceKey),
						Tag = menuId,
					};
					parentMenuItems.Add(menuId, parentMenuItem);
				}
				else
				{
					parentMenuItems.Add(menuId, topLevelMenuItem);
					parentMenuItem = topLevelMenuItem;
				}
			}
			return parentMenuItem;
		}
	}

	static void InitWindowMenu(MenuItem windowMenuItem, DockWorkspace dockWorkspace)
	{
		// At this point InitMainMenu has already appended any MEF-driven Window-menu commands
		// (CloseAllDocuments / ResetLayout). Append tool-pane toggles after a separator so
		// they sit at the bottom of the Window menu.
		// (Tab pages aren't yet exposed observably on DockWorkspace; revisit later.)
		if (dockWorkspace.ToolPaneMenuItems.Count == 0)
			return;
		if (windowMenuItem.Items.Count > 0)
			windowMenuItem.Items.Add(new Separator());

		foreach (var pane in dockWorkspace.ToolPaneMenuItems)
		{
			var item = new MenuItem {
				Header = pane.Title,
				ToggleType = MenuItemToggleType.CheckBox,
				DataContext = pane,
			};
			item.Bind(MenuItem.IsCheckedProperty, new Binding(nameof(ToolPaneMenuItem.IsPaneVisible)) {
				Mode = BindingMode.TwoWay,
			});
			windowMenuItem.Items.Add(item);
		}
	}

	static bool TryParseGesture(string? text, out KeyGesture gesture)
	{
		gesture = null!;
		if (string.IsNullOrWhiteSpace(text))
			return false;
		try
		{
			gesture = KeyGesture.Parse(text);
			return true;
		}
		catch
		{
			return false;
		}
	}

	sealed class ThemeEqualityConverter : global::Avalonia.Data.Converters.IValueConverter
	{
		public static readonly ThemeEqualityConverter Instance = new();

		public object Convert(object? value, System.Type targetType, object? parameter, System.Globalization.CultureInfo culture)
		{
			var v = value as string;
			var p = parameter as string;
			// Treat null/empty as the default theme so the default item still highlights at startup.
			if (string.IsNullOrEmpty(v))
				v = ThemeManager.Current.DefaultTheme;
			return string.Equals(v, p, System.StringComparison.OrdinalIgnoreCase);
		}

		public object? ConvertBack(object? value, System.Type targetType, object? parameter, System.Globalization.CultureInfo culture)
			=> BindingOperations.DoNothing;
	}
}
