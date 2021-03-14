using System;
using System.Windows;

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

				if (value)
				{
					_themeDictionaryContainer.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("themes/DarkTheme.xaml", UriKind.Relative) });
				}
			}
		}
	}
}
