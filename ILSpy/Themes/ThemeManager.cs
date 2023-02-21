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
			"Dark",
			"Light",
			"VS Light"
		};

		public string? Theme {
			get => _theme;
			set {
				_theme = value;
				UpdateTheme();
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

		public void UpdateColors(IHighlightingDefinition highlightingDefinition)
		{
			var prefix = $"SyntaxColor.{highlightingDefinition.Name}.";

			foreach (var (key, syntaxColor) in _syntaxColors)
			{
				var color = highlightingDefinition.GetNamedColor(key.Substring(prefix.Length));
				if (color is not null)
					syntaxColor.ApplyTo(color);
			}
		}

		private void UpdateTheme()
		{
			_themeDictionaryContainer.MergedDictionaries.Clear();
			_themeDictionaryContainer.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"themes/{(_theme ?? DefaultTheme).Replace(" ", "")}Theme.xaml", UriKind.Relative) });

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
