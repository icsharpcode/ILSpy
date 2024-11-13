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

using System.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

using ICSharpCode.ILSpy.Themes;

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

		static Button CreateToolbarItem(IExport<ICommand, IToolbarCommandMetadata> commandExport)
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

			return toolbarItem;
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