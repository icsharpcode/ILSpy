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

using System;
using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.ILSpy.Themes
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
