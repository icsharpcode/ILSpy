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
using ICSharpCode.ILSpy.ViewModels;

using TomsToolbox.Composition;
using TomsToolbox.ObservableCollections;
using TomsToolbox.Wpf;
using TomsToolbox.Wpf.Converters;

namespace ICSharpCode.ILSpy.Controls
{
	/// <summary>
	/// Interaction logic for MainMenu.xaml
	/// </summary>
	[Export]
	[NonShared]
	public partial class MainMenu
	{
		public MainMenu(SettingsService settingsService, IExportProvider exportProvider, DockWorkspace dockWorkspace)
		{
			SessionSettings = settingsService.SessionSettings;

			InitializeComponent();

			this.BeginInvoke(() => {
				InitMainMenu(Menu, exportProvider);
				InitWindowMenu(WindowMenuItem, Window.GetWindow(this)!.InputBindings, dockWorkspace);
			});
		}

		public SessionSettings SessionSettings { get; }

		static void InitMainMenu(Menu mainMenu, IExportProvider exportProvider)
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
							var command = entry.Value;

							var menuItem = new MenuItem {
								Command = CommandWrapper.Unwrap(command),
								Tag = entry.Metadata?.MenuID,
								Header = ResourceHelper.GetString(entry.Metadata?.Header)
							};

							if (!string.IsNullOrEmpty(entry.Metadata?.MenuIcon))
							{
								menuItem.Icon = new Image {
									Width = 16,
									Height = 16,
									Source = Images.Load(command, entry.Metadata.MenuIcon)
								};
							}

							menuItem.IsEnabled = entry.Metadata?.IsEnabled ?? false;
							menuItem.InputGestureText = entry.Metadata?.InputGestureText;

							if (command is IProvideParameterBinding parameterBinding)
							{
								BindingOperations.SetBinding(menuItem, MenuItem.CommandParameterProperty, parameterBinding.ParameterBinding);
							}

							parentMenuItem.Items.Add(menuItem);
						}
					}
				}
			}

			foreach (var item in parentMenuItems.Values.Where(item => item.Parent == null))
			{
				mainMenu.Items.Add(item);
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

		static void InitWindowMenu(MenuItem windowMenuItem, InputBindingCollection inputBindings, DockWorkspace dockWorkspace)
		{
			var defaultItems = windowMenuItem.Items.Cast<Control>().ToArray();

			windowMenuItem.Items.Clear();

			var toolItems = dockWorkspace.ToolPanes.Select(toolPane => CreateMenuItem(toolPane, inputBindings, dockWorkspace)).ToArray();
			var tabItems = dockWorkspace.TabPages.ObservableSelect(tabPage => CreateMenuItem(tabPage, dockWorkspace));

			var allItems = new ObservableCompositeCollection<Control>(defaultItems, [new Separator()], toolItems, [new Separator()], tabItems);

			windowMenuItem.ItemsSource = allItems;
		}

		static Control CreateMenuItem(TabPageModel pane, DockWorkspace dockWorkspace)
		{
			var header = new TextBlock {
				MaxWidth = 200,
				TextTrimming = TextTrimming.CharacterEllipsis
			};

			header.SetBinding(TextBlock.TextProperty, new Binding(nameof(pane.Title)) {
				Source = pane
			});

			var menuItem = new MenuItem {
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

		static Control CreateMenuItem(ToolPaneModel pane, InputBindingCollection inputBindings, DockWorkspace dockWorkspace)
		{
			var menuItem = new MenuItem {
				Command = pane.AssociatedCommand ?? new ToolPaneCommand(pane.ContentId, dockWorkspace),
				Header = pane.Title
			};
			var shortcutKey = pane.ShortcutKey;
			if (shortcutKey != null)
			{
				inputBindings.Add(new InputBinding(menuItem.Command, shortcutKey));
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
	}
}
