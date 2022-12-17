// Copyright (c) 2021 Tom Englert
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

using System.ComponentModel;
using System.Windows;

using ICSharpCode.ILSpy.Options;

using TomsToolbox.Wpf;
using TomsToolbox.Wpf.Interactivity;

namespace ICSharpCode.ILSpy.Themes
{
	public class WindowStyleManagerBehavior : FrameworkElementBehavior<Window>
	{
		private static readonly DispatcherThrottle restartNotificationThrottle = new DispatcherThrottle(ShowRestartNotification);

		protected override void OnAttached()
		{
			base.OnAttached();

			MainWindow.Instance.CurrentDisplaySettings.PropertyChanged += DisplaySettings_PropertyChanged;

			UpdateWindowStyle();

		}

		protected override void OnDetaching()
		{
			base.OnDetaching();

			MainWindow.Instance.CurrentDisplaySettings.PropertyChanged -= DisplaySettings_PropertyChanged;
		}

		private void UpdateWindowStyle()
		{
			if (!MainWindow.Instance.CurrentDisplaySettings.StyleWindowTitleBar)
			{
				return;
			}

			var window = AssociatedObject;
			window.Style = (Style)window.FindResource(TomsToolbox.Wpf.Styles.ResourceKeys.WindowStyle);
		}

		private static void ShowRestartNotification()
		{
			MessageBox.Show(Properties.Resources.SettingsChangeRestartRequired);
		}

		private void DisplaySettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(DisplaySettingsViewModel.StyleWindowTitleBar))
			{
				if (!MainWindow.Instance.CurrentDisplaySettings.StyleWindowTitleBar)
				{
					restartNotificationThrottle.Tick();
					return;
				}

				UpdateWindowStyle();
			}
		}
	}
}
