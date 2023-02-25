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

#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.AvalonEdit.Highlighting;

namespace ICSharpCode.ILSpy.Themes
{
	public class ThemeManager
	{
		private string? _theme;
		private readonly ResourceDictionary _themeDictionaryContainer = new();
		private readonly Dictionary<string, SyntaxColor> _syntaxColors = new();

		public static readonly ThemeManager Current = new();

		private ThemeManager()
		{
			Application.Current.Resources.MergedDictionaries.Add(_themeDictionaryContainer);
		}

		public string DefaultTheme => "Light";

		public static IReadOnlyCollection<string> AllThemes => new[] {
			"Light",
			"Dark",
			"VS Code Light+",
			"VS Code Dark+",
			"R# Light",
			"R# Dark"
		};

		public string? Theme {
			get => _theme;
			set => UpdateTheme(value);
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

		public void ApplyHighlightingColors(IHighlightingDefinition highlightingDefinition)
		{
			// Make sure all color values are taken from the theme
			foreach (var color in highlightingDefinition.NamedHighlightingColors)
				SyntaxColor.ResetColor(color);

			var prefix = $"SyntaxColor.{highlightingDefinition.Name}.";

			foreach (var (key, syntaxColor) in _syntaxColors)
			{
				var color = highlightingDefinition.GetNamedColor(key.Substring(prefix.Length));
				if (color is not null)
					syntaxColor.ApplyTo(color);
			}
		}

		private void UpdateTheme(string? themeName)
		{
			_theme = themeName ?? DefaultTheme;
			if (!AllThemes.Contains(_theme))
				_theme = DefaultTheme;

			var themeFileName = _theme
				.Replace("+", "Plus")
				.Replace("#", "Sharp")
				.Replace(" ", "");

			_themeDictionaryContainer.MergedDictionaries.Clear();
			_themeDictionaryContainer.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"themes/Theme.{themeFileName}.xaml", UriKind.Relative) });

			_syntaxColors.Clear();
			ProcessDictionary(_themeDictionaryContainer);

			void ProcessDictionary(ResourceDictionary resourceDictionary)
			{
				foreach (DictionaryEntry entry in resourceDictionary)
				{
					if (entry is { Key: string key, Value: SyntaxColor syntaxColor })
						_syntaxColors.TryAdd(key, syntaxColor);
				}

				foreach (ResourceDictionary mergedDictionary in resourceDictionary.MergedDictionaries)
					ProcessDictionary(mergedDictionary);
			}
		}
	}
}
