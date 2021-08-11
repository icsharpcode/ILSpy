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

			DisplaySettingsPanel.CurrentDisplaySettings.PropertyChanged += DisplaySettings_PropertyChanged;

			UpdateWindowStyle();

		}

		protected override void OnDetaching()
		{
			base.OnDetaching();

			DisplaySettingsPanel.CurrentDisplaySettings.PropertyChanged -= DisplaySettings_PropertyChanged;
		}

		private void UpdateWindowStyle()
		{
			if (!DisplaySettingsPanel.CurrentDisplaySettings.StyleWindowTitleBar)
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
			if (e.PropertyName == nameof(DisplaySettings.StyleWindowTitleBar))
			{
				if (!DisplaySettingsPanel.CurrentDisplaySettings.StyleWindowTitleBar)
				{
					restartNotificationThrottle.Tick();
					return;
				}

				UpdateWindowStyle();
			}
		}
	}
}
