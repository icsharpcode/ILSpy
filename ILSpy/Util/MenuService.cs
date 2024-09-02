using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Themes;
using ICSharpCode.ILSpy.ViewModels;

using TomsToolbox.Composition;

namespace ICSharpCode.ILSpy.Util
{
	internal class MenuService
	{
		public static readonly MenuService Instance = new();

		public void InitMainMenu(Menu mainMenu)
		{
			var mainMenuCommands = App.ExportProvider.GetExports<ICommand, IMainMenuCommandMetadata>("MainMenuCommand");
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
							MenuItem menuItem = new MenuItem();
							menuItem.Command = CommandWrapper.Unwrap(entry.Value);
							menuItem.Tag = entry.Metadata?.MenuID;
							menuItem.Header = ResourceHelper.GetString(entry.Metadata?.Header);
							if (!string.IsNullOrEmpty(entry.Metadata?.MenuIcon))
							{
								menuItem.Icon = new Image {
									Width = 16,
									Height = 16,
									Source = Images.Load(entry.Value, entry.Metadata.MenuIcon)
								};
							}

							menuItem.IsEnabled = entry.Metadata?.IsEnabled ?? false;
							if (entry.Value is ToggleableCommand toggle)
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

			foreach (var (key, item) in parentMenuItems)
			{
				if (item.Parent == null)
				{
					mainMenu.Items.Add(item);
				}
			}

			MenuItem GetOrAddParentMenuItem(string menuID, string resourceKey)
			{
				if (!parentMenuItems.TryGetValue(menuID, out var parentMenuItem))
				{
					var topLevelMenuItem = mainMenu.Items.OfType<MenuItem>().FirstOrDefault(m => (string)m.Tag == menuID);
					if (topLevelMenuItem == null)
					{
						parentMenuItem = new MenuItem();
						parentMenuItem.Header = ResourceHelper.GetString(resourceKey);
						parentMenuItem.Tag = menuID;
						parentMenuItems.Add(menuID, parentMenuItem);
					}
					else
					{
						parentMenuItems.Add(menuID, topLevelMenuItem);
						parentMenuItem = topLevelMenuItem;
					}
				}
				return parentMenuItem;
			}
		}

		public void InitWindowMenu(Menu mainMenu, InputBindingCollection inputBindings)
		{
			var windowMenuItem = mainMenu.Items.OfType<MenuItem>().First(m => (string)m.Tag == nameof(Properties.Resources._Window));

			var separatorBeforeTools = new Separator();
			var separatorBeforeDocuments = new Separator();

			windowMenuItem.Items.Add(separatorBeforeTools);
			windowMenuItem.Items.Add(separatorBeforeDocuments);

			var dock = DockWorkspace.Instance;

			dock.ToolPanes.CollectionChanged += ToolsChanged;
			dock.TabPages.CollectionChanged += TabsChanged;
			MessageBus<DockWorkspaceActiveTabPageChangedEventArgs>.Subscribers += ActiveTabPageChanged;

			ToolsChanged(dock.ToolPanes, new(NotifyCollectionChangedAction.Reset));
			TabsChanged(dock.TabPages, new(NotifyCollectionChangedAction.Reset));

			void ToolsChanged(object sender, NotifyCollectionChangedEventArgs e)
			{
				int endIndex = windowMenuItem.Items.IndexOf(separatorBeforeDocuments);
				int startIndex = windowMenuItem.Items.IndexOf(separatorBeforeTools) + 1;
				int insertionIndex;
				switch (e.Action)
				{
					case NotifyCollectionChangedAction.Add:
						insertionIndex = Math.Min(endIndex, startIndex + e.NewStartingIndex);
						foreach (ToolPaneModel pane in e.NewItems)
						{
							MenuItem menuItem = CreateMenuItem(pane);
							windowMenuItem.Items.Insert(insertionIndex, menuItem);
							insertionIndex++;
						}
						break;
					case NotifyCollectionChangedAction.Remove:
						foreach (ToolPaneModel pane in e.OldItems)
						{
							for (int i = endIndex - 1; i >= startIndex; i--)
							{
								MenuItem item = (MenuItem)windowMenuItem.Items[i];
								if (pane == item.Tag)
								{
									windowMenuItem.Items.RemoveAt(i);
									item.Tag = null;
									endIndex--;
									break;
								}
							}
						}
						break;
					case NotifyCollectionChangedAction.Replace:
						break;
					case NotifyCollectionChangedAction.Move:
						break;
					case NotifyCollectionChangedAction.Reset:
						for (int i = endIndex - 1; i >= startIndex; i--)
						{
							MenuItem item = (MenuItem)windowMenuItem.Items[0];
							item.Tag = null;
							windowMenuItem.Items.RemoveAt(i);
							endIndex--;
						}
						insertionIndex = endIndex;
						foreach (ToolPaneModel pane in dock.ToolPanes)
						{
							MenuItem menuItem = CreateMenuItem(pane);
							windowMenuItem.Items.Insert(insertionIndex, menuItem);
							insertionIndex++;
						}
						break;
				}

				MenuItem CreateMenuItem(ToolPaneModel pane)
				{
					MenuItem menuItem = new MenuItem();
					menuItem.Command = pane.AssociatedCommand ?? new ToolPaneCommand(pane.ContentId);
					menuItem.Header = pane.Title;
					menuItem.Tag = pane;
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

			void TabsChanged(object sender, NotifyCollectionChangedEventArgs e)
			{
				int endIndex = windowMenuItem.Items.Count;
				int startIndex = windowMenuItem.Items.IndexOf(separatorBeforeDocuments) + 1;
				int insertionIndex;
				switch (e.Action)
				{
					case NotifyCollectionChangedAction.Add:
						insertionIndex = Math.Min(endIndex, startIndex + e.NewStartingIndex);
						foreach (TabPageModel pane in e.NewItems)
						{
							MenuItem menuItem = CreateMenuItem(pane);
							pane.PropertyChanged += TabPageChanged;
							windowMenuItem.Items.Insert(insertionIndex, menuItem);
							insertionIndex++;
						}
						break;
					case NotifyCollectionChangedAction.Remove:
						foreach (TabPageModel pane in e.OldItems)
						{
							for (int i = endIndex - 1; i >= startIndex; i--)
							{
								MenuItem item = (MenuItem)windowMenuItem.Items[i];
								if (pane == item.Tag)
								{
									windowMenuItem.Items.RemoveAt(i);
									pane.PropertyChanged -= TabPageChanged;
									item.Tag = null;
									endIndex--;
									break;
								}
							}
						}
						break;
					case NotifyCollectionChangedAction.Replace:
						break;
					case NotifyCollectionChangedAction.Move:
						break;
					case NotifyCollectionChangedAction.Reset:
						for (int i = endIndex - 1; i >= startIndex; i--)
						{
							MenuItem item = (MenuItem)windowMenuItem.Items[i];
							windowMenuItem.Items.RemoveAt(i);
							((TabPageModel)item.Tag).PropertyChanged -= TabPageChanged;
							endIndex--;
						}
						insertionIndex = endIndex;
						foreach (TabPageModel pane in dock.TabPages)
						{
							MenuItem menuItem = CreateMenuItem(pane);
							pane.PropertyChanged += TabPageChanged;
							windowMenuItem.Items.Insert(insertionIndex, menuItem);
							insertionIndex++;
						}
						break;
				}

				MenuItem CreateMenuItem(TabPageModel pane)
				{
					MenuItem menuItem = new MenuItem();
					menuItem.Command = new TabPageCommand(pane);
					menuItem.Header = pane.Title.Length > 20 ? pane.Title.Substring(20) + "..." : pane.Title;
					menuItem.Tag = pane;
					menuItem.IsCheckable = true;
					menuItem.IsChecked = menuItem.Tag == dock.ActiveTabPage;

					return menuItem;
				}
			}

			void TabPageChanged(object sender, PropertyChangedEventArgs e)
			{
				var windowMenu = mainMenu.Items.OfType<MenuItem>().First(m => (string)m.Tag == nameof(Properties.Resources._Window));
				foreach (MenuItem menuItem in windowMenu.Items.OfType<MenuItem>())
				{
					if (menuItem.IsCheckable && menuItem.Tag == sender)
					{
						string title = ((TabPageModel)sender).Title;
						menuItem.Header = title.Length > 20 ? title.Substring(0, 20) + "..." : title;
					}
				}
			}

			void ActiveTabPageChanged(object sender, EventArgs e)
			{
				foreach (MenuItem menuItem in windowMenuItem.Items.OfType<MenuItem>())
				{
					if (menuItem.IsCheckable && menuItem.Tag is TabPageModel)
					{
						menuItem.IsChecked = menuItem.Tag == dock.ActiveTabPage;
					}
				}
			}
		}

		public void InitToolbar(ToolBar toolBar)
		{
			int navigationPos = 0;
			int openPos = 1;
			var toolbarCommands = App.ExportProvider.GetExports<ICommand, IToolbarCommandMetadata>("ToolbarCommand");
			foreach (var commandGroup in toolbarCommands.OrderBy(c => c.Metadata.ToolbarOrder).GroupBy(c => Properties.Resources.ResourceManager.GetString(c.Metadata.ToolbarCategory)))
			{
				if (commandGroup.Key == Properties.Resources.ResourceManager.GetString("Navigation"))
				{
					foreach (var command in commandGroup)
					{
						toolBar.Items.Insert(navigationPos++, MakeToolbarItem(command));
						openPos++;
					}
				}
				else if (commandGroup.Key == Properties.Resources.ResourceManager.GetString("Open"))
				{
					foreach (var command in commandGroup)
					{
						toolBar.Items.Insert(openPos++, MakeToolbarItem(command));
					}
				}
				else
				{
					toolBar.Items.Add(new Separator());
					foreach (var command in commandGroup)
					{
						toolBar.Items.Add(MakeToolbarItem(command));
					}
				}
			}

		}

		Button MakeToolbarItem(IExport<ICommand, IToolbarCommandMetadata> command)
		{
			return new Button {
				Style = ThemeManager.Current.CreateToolBarButtonStyle(),
				Command = CommandWrapper.Unwrap(command.Value),
				ToolTip = Properties.Resources.ResourceManager.GetString(command.Metadata.ToolTip),
				Tag = command.Metadata.Tag,
				Content = new Image {
					Width = 16,
					Height = 16,
					Source = Images.Load(command.Value, command.Metadata.ToolbarIcon)
				}
			};
		}

		public void Init(Menu mainMenu, ToolBar toolBar, InputBindingCollection inputBindings)
		{
			InitMainMenu(mainMenu);
			InitWindowMenu(mainMenu, inputBindings);
			InitToolbar(toolBar);
		}
	}
}
