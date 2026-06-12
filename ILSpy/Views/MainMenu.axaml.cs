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
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;

using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Data;
using global::Avalonia.Input;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Themes;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy;

/// <summary>
/// Builds the application's main menu as a NativeMenu and attaches it to a window.
/// A NativeMenuBar control rendered inside the window decides at runtime whether to
/// draw the menu inline (Windows / Linux) or hand it off to the macOS system menu bar.
/// </summary>
public static class MainMenu
{
	public static void Attach(Window window)
	{
		ArgumentNullException.ThrowIfNull(window);

		// Tests and design-time previews don't bootstrap the composition host. Without it
		// we have nothing to populate the menu with, so bail and let the empty NativeMenuBar
		// in the XAML render as a thin spacer.
		if (!TryGetExports(out var settings, out var registry, out var dockWorkspace, out var setThemeCommand))
			return;

		var menu = new NativeMenu();
		var topLevelByTag = new Dictionary<string, NativeMenuItem>(StringComparer.Ordinal);

		// Pre-build the three known top-level slots so MEF commands with ParentMenuID set
		// to "_File" / "_View" / "_Window" attach to them. Additional top-level groups
		// can still be created later from MEF metadata.
		AddTopLevel(menu, topLevelByTag, "_File", nameof(Resources._File));
		var viewItem = AddTopLevel(menu, topLevelByTag, "_View", nameof(Resources._View));
		var windowItem = AddTopLevel(menu, topLevelByTag, "_Window", nameof(Resources._Window));

		PopulateViewMenu(viewItem.Menu!, settings.SessionSettings, setThemeCommand);
		AppendRegistryCommands(menu, topLevelByTag, registry.Commands);
		AppendWindowDynamicContent(windowItem.Menu!, dockWorkspace);

		if (OperatingSystem.IsMacOS())
		{
			TranslateGesturesForMacOS(menu);
			PromoteHelpToMacAppMenu(menu, topLevelByTag);
		}

		NativeMenu.SetMenu(window, menu);
	}

	// macOS convention puts About / Check for Updates under the bold app-named menu
	// next to the Apple logo, not under a separate "Help" top-level. The Cocoa exporter
	// samples Application.Current's NativeMenu ONCE at startup (before this window exists)
	// and never watches the property afterwards -- so swapping in a fresh menu here would
	// be ignored. Instead we populate the empty NativeMenu declared in App.axaml IN PLACE:
	// the exporter subscribes to that instance's Items, so inserting fires a re-export.
	// Items go at the top, above the Services / Hide / Quit block the exporter appended.
	// We then remove _Help from the window menu so the items don't appear in both places.
	static void PromoteHelpToMacAppMenu(NativeMenu rootMenu, Dictionary<string, NativeMenuItem> topLevelByTag)
	{
		if (!topLevelByTag.TryGetValue("_Help", out var helpItem) || helpItem.Menu is null)
			return;
		if (Application.Current is null)
			return;
		var appMenu = NativeMenu.GetMenu(Application.Current);
		if (appMenu is null)
			return;
		var index = 0;
		foreach (var item in helpItem.Menu.Items.ToArray())
		{
			helpItem.Menu.Items.Remove(item);
			appMenu.Items.Insert(index++, item);
		}
		rootMenu.Items.Remove(helpItem);
		topLevelByTag.Remove("_Help");
	}

	static bool TryGetExports(
		out SettingsService settings,
		out MainMenuCommandRegistry registry,
		out DockWorkspace dockWorkspace,
		out SetThemeCommand setThemeCommand)
	{
		try
		{
			settings = AppComposition.Current.GetExport<SettingsService>();
			registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
			dockWorkspace = AppComposition.Current.GetExport<DockWorkspace>();
			setThemeCommand = AppComposition.Current.GetExport<SetThemeCommand>();
			return true;
		}
		catch
		{
			settings = null!;
			registry = null!;
			dockWorkspace = null!;
			setThemeCommand = null!;
			return false;
		}
	}

	static NativeMenuItem AddTopLevel(NativeMenu root, Dictionary<string, NativeMenuItem> byTag, string menuId, string resourceKey)
	{
		var item = new NativeMenuItem {
			Header = ResourceHelper.GetString(resourceKey),
			Menu = new NativeMenu(),
		};
		byTag[menuId] = item;
		root.Items.Add(item);
		return item;
	}

	static void PopulateViewMenu(NativeMenu view, SessionSettings session, SetThemeCommand setThemeCommand)
	{
		var languageSettings = session.LanguageSettings;

		view.Items.Add(MakeRadio(Resources.Show_publiconlyTypesMembers, languageSettings, nameof(LanguageSettings.ApiVisPublicOnly)));
		view.Items.Add(MakeRadio(Resources.Show_internalTypesMembers, languageSettings, nameof(LanguageSettings.ApiVisPublicAndInternal)));
		view.Items.Add(MakeRadio(Resources.Show_allTypesAndMembers, languageSettings, nameof(LanguageSettings.ApiVisAll)));

		view.Items.Add(new NativeMenuItemSeparator());

		var theme = new NativeMenuItem {
			Header = Resources.Theme,
			Menu = new NativeMenu(),
		};
		PopulateThemeSubmenu(theme.Menu!, session, setThemeCommand);
		view.Items.Add(theme);
	}

	static NativeMenuItem MakeRadio(string header, object source, string path)
	{
		var item = new NativeMenuItem {
			Header = header,
			ToggleType = MenuItemToggleType.Radio,
		};
		// IsChecked is purely for *displaying* the checkmark (OneWay). The write
		// path lives in Command, because Avalonia's macOS NativeMenu bridge maps
		// NativeMenuItem -> NSMenuItem only when Command != null; without it the
		// item gets greyed out by NSMenuValidation and no click ever reaches the
		// IsChecked binding. Mutual exclusion across sibling radios is handled by
		// the bound property's setter.
		item.Bind(NativeMenuItem.IsCheckedProperty, new Binding(path) {
			Source = source,
			Mode = BindingMode.OneWay,
		});
		var property = source.GetType().GetProperty(path);
		item.Command = new RelayCommand(() => property?.SetValue(source, true));
		return item;
	}

	static void PopulateThemeSubmenu(NativeMenu theme, SessionSettings session, SetThemeCommand setThemeCommand)
	{
		foreach (var themeName in ThemeManager.AllThemes)
		{
			var item = new NativeMenuItem {
				Header = themeName,
				Command = setThemeCommand,
				CommandParameter = themeName,
				ToggleType = MenuItemToggleType.Radio,
			};
			item.Bind(NativeMenuItem.IsCheckedProperty, new Binding(nameof(SessionSettings.Theme)) {
				Source = session,
				Converter = ThemeEqualityConverter.Instance,
				ConverterParameter = themeName,
				Mode = BindingMode.OneWay,
			});
			theme.Items.Add(item);
		}
	}

	static void AppendRegistryCommands(
		NativeMenu rootMenu,
		Dictionary<string, NativeMenuItem> topLevelByTag,
		IReadOnlyList<ExportFactory<ICommand, MainMenuCommandMetadata>> commands)
	{
		var menuGroups = commands
			.OrderBy(c => c.Metadata?.MenuOrder)
			.GroupBy(c => c.Metadata?.ParentMenuID ?? string.Empty)
			.ToArray();

		foreach (var group in menuGroups)
		{
			var parentItem = GetOrAddParentItem(group.Key, group.Key);
			var parentMenu = parentItem.Menu!;

			foreach (var category in group.GroupBy(c => c.Metadata?.MenuCategory))
			{
				if (parentMenu.Items.Count > 0)
					parentMenu.Items.Add(new NativeMenuItemSeparator());

				foreach (var entry in category)
				{
					var entryMenuId = entry.Metadata?.MenuID;
					if (entryMenuId != null && menuGroups.Any(g => g.Key == entryMenuId))
					{
						// This entry is itself the parent of another group; surface it as a submenu.
						var nested = GetOrAddParentItem(entryMenuId, entry.Metadata?.Header);
						nested.Header = ResourceHelper.GetString(entry.Metadata?.Header);
						parentMenu.Items.Add(nested);
					}
					else
					{
						ICommand command;
						try
						{
							// Isolate each entry: instantiating a command export can throw for a
							// misbehaving plugin (e.g. a DI ctor without [ImportingConstructor]); one
							// bad command must not take down the whole menu bar.
							command = entry.CreateExport().Value;
						}
						catch (Exception ex)
						{
							AppEnv.CompositionErrors.Report($"Main-menu command '{entry.Metadata?.Header}'", ex);
							continue;
						}
						// No explicit IsEnabled: assigning Command lets NativeMenuItem track the
						// command's CanExecute, so OS-gated commands (e.g. Open from GAC, which is
						// Windows-only) grey out correctly. A hard-coded IsEnabled would override
						// that, and the macOS native menu reads it directly -- leaving the item
						// wrongly enabled there even though invoking it is a no-op.
						var menuItem = new NativeMenuItem {
							Header = ResourceHelper.GetString(entry.Metadata?.Header),
							Command = command,
						};

						// NativeMenuItem.Icon is Bitmap (macOS NSImage has no vector form),
						// so SVG-backed metadata icons get rasterised at attach-time. Silently
						// skips when the metadata is empty or the named field isn't registered
						// in Images.cs - keeps the menu item rendering, just without an icon.
						if (Images.LoadBitmap(entry.Metadata?.MenuIcon) is { } bitmap)
							menuItem.Icon = bitmap;

						if (TryParseGesture(entry.Metadata?.InputGestureText, out var gesture))
							menuItem.Gesture = gesture;

						if (command is IProvideParameterBinding parameterBinding)
							menuItem.Bind(NativeMenuItem.CommandParameterProperty, parameterBinding.ParameterBinding);

						parentMenu.Items.Add(menuItem);
					}
				}
			}
		}

		NativeMenuItem GetOrAddParentItem(string menuId, string? resourceKey)
		{
			if (!topLevelByTag.TryGetValue(menuId, out var existing))
			{
				existing = new NativeMenuItem {
					Header = ResourceHelper.GetString(resourceKey),
					Menu = new NativeMenu(),
				};
				topLevelByTag[menuId] = existing;
				rootMenu.Items.Add(existing);
			}
			return existing;
		}
	}

	static void AppendWindowDynamicContent(NativeMenu windowMenu, DockWorkspace dockWorkspace)
	{
		// Tool-pane toggles sit below any MEF-driven Window commands (CloseAllDocuments / ResetLayout).
		if (dockWorkspace.ToolPaneMenuItems.Count > 0)
		{
			if (windowMenu.Items.Count > 0)
				windowMenu.Items.Add(new NativeMenuItemSeparator());

			foreach (var pane in dockWorkspace.ToolPaneMenuItems)
			{
				var item = new NativeMenuItem {
					Header = pane.Title,
					ToggleType = MenuItemToggleType.CheckBox,
				};
				// OneWay binding + Command — see MakeRadio for why TwoWay alone
				// doesn't drive macOS clickability.
				item.Bind(NativeMenuItem.IsCheckedProperty, new Binding(nameof(ToolPaneMenuItem.IsPaneVisible)) {
					Source = pane,
					Mode = BindingMode.OneWay,
				});
				var capturedPane = pane;
				item.Command = new RelayCommand(() => capturedPane.IsPaneVisible = !capturedPane.IsPaneVisible);
				windowMenu.Items.Add(item);
			}
		}

		AppendTabSection(windowMenu, dockWorkspace);
	}

	static void AppendTabSection(NativeMenu windowMenu, DockWorkspace dockWorkspace)
	{
		var tabItems = dockWorkspace.TabPageMenuItems;
		NativeMenuItemSeparator? separator = null;
		var perItem = new Dictionary<TabPageMenuItem, NativeMenuItem>();

		void EnsureSeparator()
		{
			if (separator != null)
				return;
			if (windowMenu.Items.Count == 0)
				return;
			separator = new NativeMenuItemSeparator();
			windowMenu.Items.Add(separator);
		}

		NativeMenuItem CreateMenuItem(TabPageMenuItem vm)
		{
			var item = new NativeMenuItem {
				Header = vm.Title,
				ToggleType = MenuItemToggleType.Radio,
			};
			item.Bind(NativeMenuItem.HeaderProperty, new Binding(nameof(TabPageMenuItem.Title)) {
				Source = vm,
			});
			// OneWay binding + Command — see MakeRadio for why TwoWay alone
			// doesn't drive macOS clickability.
			item.Bind(NativeMenuItem.IsCheckedProperty, new Binding(nameof(TabPageMenuItem.IsActive)) {
				Source = vm,
				Mode = BindingMode.OneWay,
			});
			item.Command = new RelayCommand(() => vm.IsActive = true);
			return item;
		}

		void AddItem(TabPageMenuItem vm)
		{
			EnsureSeparator();
			var menuItem = CreateMenuItem(vm);
			perItem.Add(vm, menuItem);
			windowMenu.Items.Add(menuItem);
		}

		void RemoveItem(TabPageMenuItem vm)
		{
			if (!perItem.TryGetValue(vm, out var menuItem))
				return;
			windowMenu.Items.Remove(menuItem);
			perItem.Remove(vm);
			if (perItem.Count == 0 && separator != null)
			{
				windowMenu.Items.Remove(separator);
				separator = null;
			}
		}

		foreach (var vm in tabItems)
			AddItem(vm);

		tabItems.CollectionChanged += (_, args) => {
			switch (args.Action)
			{
				case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
					if (args.NewItems != null)
						foreach (TabPageMenuItem vm in args.NewItems)
							AddItem(vm);
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
					if (args.OldItems != null)
						foreach (TabPageMenuItem vm in args.OldItems)
							RemoveItem(vm);
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
				case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
					foreach (var vm in perItem.Keys.ToList())
						RemoveItem(vm);
					foreach (var vm in tabItems)
						AddItem(vm);
					break;
			}
		};
	}

	// macOS renders KeyModifiers.Control as the caret glyph; users expect Cmd for menu
	// shortcuts. Avalonia maps Meta to Cmd on Apple platforms (and to Win on Windows /
	// Linux), so swapping Control for Meta gives the right symbol on macOS without
	// changing keyboard semantics elsewhere -- this code only runs on macOS.
	static void TranslateGesturesForMacOS(NativeMenu menu)
	{
		foreach (var item in menu.Items)
		{
			if (item is NativeMenuItem ni)
			{
				if (ni.Gesture is { } g && (g.KeyModifiers & KeyModifiers.Control) != 0)
				{
					var modifiers = (g.KeyModifiers & ~KeyModifiers.Control) | KeyModifiers.Meta;
					ni.Gesture = new KeyGesture(g.Key, modifiers);
				}
				if (ni.Menu != null)
					TranslateGesturesForMacOS(ni.Menu);
			}
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

		public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
		{
			var v = value as string;
			var p = parameter as string;
			if (string.IsNullOrEmpty(v))
				v = ThemeManager.Current.DefaultTheme;
			return string.Equals(v, p, StringComparison.OrdinalIgnoreCase);
		}

		public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
			=> BindingOperations.DoNothing;
	}
}
