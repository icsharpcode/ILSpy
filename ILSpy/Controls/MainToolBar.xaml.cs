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

using System;
using System.ComponentModel;
using System.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using ICSharpCode.ILSpy.Themes;
using ICSharpCode.ILSpyX.TreeView;

using TomsToolbox.Composition;

namespace ICSharpCode.ILSpy.Controls
{
	/// <summary>
	/// Interaction logic for MainToolBar.xaml
	/// </summary>
	[Export]
	[NonShared]
	public partial class MainToolBar
	{
		public MainToolBar(IExportProvider exportProvider)
		{
			InitializeComponent();

			this.Dispatcher.BeginInvoke(DispatcherPriority.Background, () => {
				InitToolbar(ToolBar, exportProvider);
				Window.GetWindow(this)!.KeyDown += MainWindow_KeyDown;
			});
		}

		static void InitToolbar(ToolBar toolBar, IExportProvider exportProvider)
		{
			int navigationPos = 0;
			int openPos = 1;
			var toolbarCommandsByCategory = exportProvider
				.GetExports<ICommand, IToolbarCommandMetadata>("ToolbarCommand")
				.OrderBy(c => c.Metadata?.ToolbarOrder)
				.GroupBy(c => c.Metadata?.ToolbarCategory);

			foreach (var commandCategory in toolbarCommandsByCategory)
			{
				if (commandCategory.Key == nameof(Properties.Resources.Navigation))
				{
					foreach (var command in commandCategory)
					{
						toolBar.Items.Insert(navigationPos++, CreateToolbarItem(command));
						openPos++;
					}
				}
				else if (commandCategory.Key == nameof(Properties.Resources.Open))
				{
					foreach (var command in commandCategory)
					{
						toolBar.Items.Insert(openPos++, CreateToolbarItem(command));
					}
				}
				else
				{
					toolBar.Items.Add(new Separator());
					foreach (var command in commandCategory)
					{
						toolBar.Items.Add(CreateToolbarItem(command));
					}
				}
			}
		}

		static UIElement CreateToolbarItem(IExport<ICommand, IToolbarCommandMetadata> commandExport)
		{
			var command = commandExport.Value;

			Button toolbarItem = new() {
				Style = ThemeManager.Current.CreateToolBarButtonStyle(),
				Command = CommandWrapper.Unwrap(command),
				ToolTip = Properties.Resources.ResourceManager.GetString(
					commandExport.Metadata?.ToolTip ?? string.Empty),
				Tag = commandExport.Metadata?.Tag,
				Content = new Image {
					Width = 16,
					Height = 16,
					Source = Images.Load(command, commandExport.Metadata?.ToolbarIcon)
				}
			};

			if (command is IProvideParameterBinding parameterBinding)
			{
				BindingOperations.SetBinding(toolbarItem, ButtonBase.CommandParameterProperty,
					parameterBinding.ParameterBinding);
			}

			if (command is IProvideParameterList parameterList)
			{
				toolbarItem.Margin = new Thickness(2, 0, 0, 0);

				var dropDownPanel = new StackPanel { Orientation = Orientation.Horizontal };

				var dropDownToggle = new ToggleButton {
					Style = ThemeManager.Current.CreateToolBarToggleButtonStyle(),
					Content = "▾",
					Padding = new Thickness(0),
					MinWidth = 0,
					Margin = new Thickness(0, 0, 2, 0)
				};

				var contextMenu = new ContextMenu {
					PlacementTarget = dropDownPanel,
					Tag = command
				};

				ContextMenuService.SetPlacement(toolbarItem, PlacementMode.Bottom);
				toolbarItem.ContextMenu = contextMenu;
				toolbarItem.ContextMenuOpening += (_, _) =>
					PrepareParameterList(contextMenu);
				dropDownToggle.Checked += (_, _) => {
					PrepareParameterList(contextMenu);
					contextMenu.Placement = PlacementMode.Bottom;
					contextMenu.SetCurrentValue(ContextMenu.IsOpenProperty, true);
				};

				BindingOperations.SetBinding(dropDownToggle, ToggleButton.IsCheckedProperty,
					new Binding(nameof(contextMenu.IsOpen)) { Source = contextMenu });

				BindingOperations.SetBinding(dropDownToggle, IsEnabledProperty,
					new Binding(nameof(IsEnabled)) { Source = toolbarItem });

				// When the toggle button is checked, clicking it to uncheck will dismiss the menu first
				// which unchecks the toggle button via binding above and the click is used to open it again.
				// This is a workaround to ignore the click to uncheck the already unchecked toggle button.
				// We have to ensure the dismissing click is on the toggle button, otherwise the flag
				// will not get cleared and menu will not open next time.
				Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(contextMenu, (_, e) => {
					var point = e.GetPosition(dropDownToggle);
					dropDownToggle.Tag = dropDownToggle.InputHitTest(point);
				});
				dropDownToggle.PreviewMouseLeftButtonDown += (_, e) => {
					e.Handled = dropDownToggle.Tag != null;
					dropDownToggle.Tag = null;
				};

				dropDownPanel.Children.Add(toolbarItem);
				dropDownPanel.Children.Add(dropDownToggle);
				return dropDownPanel;
			}

			return toolbarItem;
		}

		static void PrepareParameterList(ContextMenu menu)
		{
			const int maximumParameterListCount = 20;

			var command = (ICommand)menu.Tag;
			var parameterList = (IProvideParameterList)command;

			menu.Items.Clear();
			foreach (var parameter in parameterList.ParameterList)
			{
				MenuItem parameterItem = new MenuItem();
				parameterItem.Command = CommandWrapper.Unwrap(command);
				parameterItem.CommandParameter = parameter;
				parameterItem.CommandTarget = menu.PlacementTarget;
				parameterItem.InputGestureText = " ";

				var headerPresenter = new ContentPresenter { RecognizesAccessKey = false };
				parameterItem.Header = headerPresenter;

				var header = parameterList.GetParameterText(parameter);
				switch (header)
				{
					case SharpTreeNode node:
						headerPresenter.Content = node.NavigationText;
						if (node.Icon is ImageSource icon)
							parameterItem.Icon = new Image {
								Width = 16,
								Height = 16,
								Source = icon
							};
						break;

					default:
						headerPresenter.Content = header;
						break;
				}

				menu.Items.Add(parameterItem);
				if (menu.Items.Count >= maximumParameterListCount)
					break;
			}
		}

		void MainWindow_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Handled || e.KeyboardDevice.Modifiers != ModifierKeys.Alt || e.Key != Key.System)
				return;

			switch (e.SystemKey)
			{
				case Key.A:
					assemblyListComboBox.Focus();
					e.Handled = true;
					break;
				case Key.L:
					languageComboBox.Focus();
					e.Handled = true;
					break;
				case Key.E: // Alt+V was already taken by _View menu
					languageVersionComboBox.Focus();
					e.Handled = true;
					break;
			}
		}
	}
}