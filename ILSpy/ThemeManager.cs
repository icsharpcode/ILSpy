using System;
using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.ILSpy
{
	internal class ThemeManager
	{
		private bool _isDarkMode;
		private readonly ResourceDictionary _themeDictionaryContainer = new ResourceDictionary();


		public static readonly ThemeManager Current = new ThemeManager();

		private ThemeManager()
		{
			Application.Current.Resources.MergedDictionaries.Add(_themeDictionaryContainer);
		}

		public bool IsDarkMode {
			get => _isDarkMode;
			set {
				_isDarkMode = value;

				_themeDictionaryContainer.MergedDictionaries.Clear();

				string theme = value ? "Dark" : "Light";

				_themeDictionaryContainer.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"themes/{theme}Theme.xaml", UriKind.Relative) });
			}
		}

		public Button CreateButton()
		{
			return new Button {
				Style = CreateButtonStyle()
			};
		}

		public Style CreateButtonStyle()
		{
			return new Style(typeof(Button), (Style)Application.Current.FindResource(typeof(Button)));
		}

		public Style CreateToolBarButtonStyle()
		{
			return new Style(typeof(Button), (Style)Application.Current.FindResource(ToolBar.ButtonStyleKey));
		}
	}
}
