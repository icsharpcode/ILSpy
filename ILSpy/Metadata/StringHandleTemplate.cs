// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ICSharpCode.ILSpy.Metadata
{
	public class StringHandleTemplate : DataTemplateSelector
	{
		public string ValuePropertyName { get; set; }
		public string HandlePropertyName { get; set; }

		private DataTemplate template;

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (template != null)
				return template;
			var textBlock = new FrameworkElementFactory(typeof(TextBlock), "textBlock");
			textBlock.SetValue(FrameworkElement.MarginProperty, new Thickness(3, 1, 0, 0));
			textBlock.SetBinding(TextBlock.TextProperty, new Binding(ValuePropertyName) { StringFormat = "\"{0}\"" });
			var textBox = new FrameworkElementFactory(typeof(TextBox), "textBox");
			textBox.SetBinding(TextBox.TextProperty, new Binding(ValuePropertyName) { StringFormat = "\"{0}\"", Mode = BindingMode.OneWay });
			textBox.SetValue(TextBox.VisibilityProperty, Visibility.Hidden);
			textBox.SetValue(TextBox.IsReadOnlyCaretVisibleProperty, true);
			textBox.SetValue(TextBox.IsReadOnlyProperty, true);
			textBox.SetBinding(FrameworkElement.ToolTipProperty, new MultiBinding {
				Converter = new StringHandleConverter(),
				Bindings = {
					new Binding(HandlePropertyName),
					new Binding(ValuePropertyName)
				}
			});
			var grid = new FrameworkElementFactory(typeof(Grid));
			grid.AppendChild(textBlock);
			grid.AppendChild(textBox);
			template = new DataTemplate {
				VisualTree = grid,
				Triggers = {
					new Trigger {
						Property = UIElement.IsMouseOverProperty,
						Value = true,
						Setters = {
							new Setter(UIElement.VisibilityProperty, Visibility.Visible, "textBox"),
							new Setter(UIElement.VisibilityProperty, Visibility.Hidden, "textBlock"),
						}
					},
					new Trigger {
						Property = UIElement.IsKeyboardFocusWithinProperty,
						Value = true,
						Setters = {
							new Setter(UIElement.VisibilityProperty, Visibility.Visible, "textBox"),
							new Setter(UIElement.VisibilityProperty, Visibility.Hidden, "textBlock"),
						}
					}
				}
			};
			return template;
		}

		private class StringHandleConverter : IMultiValueConverter
		{
			public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
			{
				return string.Format("{0:X} \"{1}\"", values[0], values[1]);
			}

			public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
			{
				throw new NotImplementedException();
			}
		}
	}
}
