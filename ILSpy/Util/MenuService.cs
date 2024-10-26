// Copyright (c) 2024 Tom Englert for the SharpDevelop Team
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
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Themes;
using ICSharpCode.ILSpy.ViewModels;

using TomsToolbox.Composition;
using TomsToolbox.ObservableCollections;
using TomsToolbox.Wpf.Converters;

namespace ICSharpCode.ILSpy.Util
{
	[Export]
	[Shared]
	public class MenuService(IExportProvider exportProvider, DockWorkspace dockWorkspace)
	{
		public void Init(Menu mainMenu, ToolBar toolBar, InputBindingCollection inputBindings)
		{
			InitMainMenu(mainMenu);
			InitWindowMenu(mainMenu, inputBindings);
			InitToolbar(toolBar);
		}

		void InitMainMenu(Menu mainMenu)
		{
			var mainMenuCommands = exportProvider.GetExports<ICommand, IMainMenuCommandMetadata>("MainMenuCommand");
			// Start by constructing the individual flat menus
			var parentMenuItems = new Dictionary<string, MenuItem>();
			var menuGroups = mainMenuCommands.OrderBy(c => c.Metadata?.MenuOrder).GroupBy(c => c.Metadata?.ParentMenuID).ToArray();
			foreach (var menu in menuGroups)
			{
				// Get or add the target menu item and add all items grouped by menu category
				var parentMenuItem = GetOrAddParentMenuItem(menu.Key, menu.Key);
				foreach (var category in menu.GroupBy(c => c.Metadata?.MenuCategory))
				{
					if (parentMenuItem.Items.Count > 0)
					{
						parentMenuItem.Items.Add(new Separator { Tag = category.Key });
					}
					foreach (var entry in category)
					{
						if (menuGroups.Any(g => g.Key == entry.Metadata?.MenuID))
						{
							var menuItem = GetOrAddParentMenuItem(entry.Metadata?.MenuID, entry.Metadata?.Header);
							// replace potential dummy text with real name
							menuItem.Header = ResourceHelper.GetString(entry.Metadata?.Header);
							parentMenuItem.Items.Add(menuItem);
						}
						else
						{
							var menuItem = new MenuItem {
								Command = CommandWrapper.Unwrap(entry.Value),
								Tag = entry.Metadata?.MenuID,
								Header = ResourceHelper.GetString(entry.Metadata?.Header)
							};
							if (!string.IsNullOrEmpty(entry.Metadata?.MenuIcon))
							{
								menuItem.Icon = new Image {
									Width = 16,
									Height = 16,
									Source = Images.Load(entry.Value, entry.Metadata.MenuIcon)
								};
							}

							menuItem.IsEnabled = entry.Metadata?.IsEnabled ?? false;
							if (entry.Value is ToggleableCommand)
							{
								menuItem.IsCheckable = true;
								menuItem.SetBinding(MenuItem.IsCheckedProperty, new Binding("IsChecked") { Source = entry.Value, Mode = BindingMode.OneWay });
							}

							menuItem.InputGestureText = entry.Metadata?.InputGestureText;
							parentMenuItem.Items.Add(menuItem);
						}
					}
				}
			}

			foreach (var item in parentMenuItems.Values)
			{
				if (item.Parent == null)
				{
					mainMenu.Items.Add(item);
				}
			}

			MenuItem GetOrAddParentMenuItem(string menuId, string resourceKey)
			{
				if (!parentMenuItems.TryGetValue(menuId, out var parentMenuItem))
				{
					var topLevelMenuItem = mainMenu.Items.OfType<MenuItem>().FirstOrDefault(m => (string)m.Tag == menuId);
					if (topLevelMenuItem == null)
					{
						parentMenuItem = new() {
							Header = ResourceHelper.GetString(resourceKey),
							Tag = menuId
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

		void InitWindowMenu(Menu mainMenu, InputBindingCollection inputBindings)
		{
			var windowMenuItem = mainMenu.Items.OfType<MenuItem>().First(m => (string)m.Tag == nameof(Properties.Resources._Window));

			var defaultItems = windowMenuItem.Items.Cast<Control>().ToArray();

			windowMenuItem.Items.Clear();

			var toolItems = dockWorkspace.ToolPanes.Select(toolPane => CreateMenuItem(toolPane, inputBindings)).ToArray();
			var tabItems = dockWorkspace.TabPages.ObservableSelect(tabPage => CreateMenuItem(tabPage));

			var allItems = new ObservableCompositeCollection<Control>(defaultItems, [new Separator()], toolItems, [new Separator()], tabItems);

			windowMenuItem.ItemsSource = allItems;
		}

		void InitToolbar(ToolBar toolBar)
		{
			int navigationPos = 0;
			int openPos = 1;
			var toolbarCommandsByTitle = exportProvider.GetExports<ICommand, IToolbarCommandMetadata>("ToolbarCommand")
				.OrderBy(c => c.Metadata?.ToolbarOrder)
				.GroupBy(c => c.Metadata?.ToolbarCategory);

			foreach (var commandGroup in toolbarCommandsByTitle)
			{
				if (commandGroup.Key == nameof(Properties.Resources.Navigation))
				{
					foreach (var command in commandGroup)
					{
						toolBar.Items.Insert(navigationPos++, CreateToolbarItem(command));
						openPos++;
					}
				}
				else if (commandGroup.Key == nameof(Properties.Resources.Open))
				{
					foreach (var command in commandGroup)
					{
						toolBar.Items.Insert(openPos++, CreateToolbarItem(command));
					}
				}
				else
				{
					toolBar.Items.Add(new Separator());
					foreach (var command in commandGroup)
					{
						toolBar.Items.Add(CreateToolbarItem(command));
					}
				}
			}

		}

		Control CreateMenuItem(TabPageModel pane)
		{
			var header = new TextBlock {
				MaxWidth = 200,
				TextTrimming = TextTrimming.CharacterEllipsis
			};

			header.SetBinding(TextBlock.TextProperty, new Binding(nameof(pane.Title)) {
				Source = pane
			});

			MenuItem menuItem = new() {
				Command = new TabPageCommand(pane, dockWorkspace),
				Header = header,
				IsCheckable = true
			};

			menuItem.SetBinding(MenuItem.IsCheckedProperty, new Binding(nameof(dockWorkspace.ActiveTabPage)) {
				Source = dockWorkspace,
				ConverterParameter = pane,
				Converter = BinaryOperationConverter.Equality,
				Mode = BindingMode.OneWay
			});

			return menuItem;
		}

		Control CreateMenuItem(ToolPaneModel pane, InputBindingCollection inputBindings)
		{
			MenuItem menuItem = new() {
				Command = pane.AssociatedCommand ?? new ToolPaneCommand(pane.ContentId, dockWorkspace),
				Header = pane.Title
			};
			var shortcutKey = pane.ShortcutKey;
			if (shortcutKey != null)
			{
				inputBindings.Add(new(menuItem.Command, shortcutKey));
				menuItem.InputGestureText = shortcutKey.GetDisplayStringForCulture(CultureInfo.CurrentUICulture);
			}
			if (!string.IsNullOrEmpty(pane.Icon))
			{
				menuItem.Icon = new Image {
					Width = 16,
					Height = 16,
					Source = Images.Load(pane, pane.Icon)
				};
			}

			return menuItem;
		}

		static Button CreateToolbarItem(IExport<ICommand, IToolbarCommandMetadata> command)
		{
			return new() {
				Style = ThemeManager.Current.CreateToolBarButtonStyle(),
				Command = CommandWrapper.Unwrap(command.Value),
				ToolTip = Properties.Resources.ResourceManager.GetString(command.Metadata?.ToolTip),
				Tag = command.Metadata?.Tag,
				Content = new Image {
					Width = 16,
					Height = 16,
					Source = Images.Load(command.Value, command.Metadata?.ToolbarIcon)
				}
			};
		}
	}
}
