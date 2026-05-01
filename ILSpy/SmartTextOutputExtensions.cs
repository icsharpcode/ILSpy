// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

using ILSpy.TextView;

namespace ILSpy
{
	public static class SmartTextOutputExtensions
	{
		public static void AddButton(this ISmartTextOutput output, IImage? icon, string text, EventHandler<RoutedEventArgs> click)
		{
			ArgumentNullException.ThrowIfNull(output);
			ArgumentNullException.ThrowIfNull(click);

			output.AddUIElement(() => {
				var button = new Button {
					Cursor = new Cursor(StandardCursorType.Arrow),
					Margin = new Thickness(2),
					Padding = new Thickness(9, 1, 9, 1),
					MinWidth = 73,
				};
				if (icon != null)
				{
					button.Content = new StackPanel {
						Orientation = Orientation.Horizontal,
						Children = {
							new Image { Width = 16, Height = 16, Source = icon, Margin = new Thickness(0, 0, 4, 0) },
							new TextBlock { Text = text },
						},
					};
				}
				else
				{
					button.Content = text;
				}
				button.Click += click;
				return button;
			});
		}
	}
}
