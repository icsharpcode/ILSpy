using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ICSharpCode.ILSpy.Metadata
{
	public class HandleTemplate : DataTemplateSelector
	{
		public string ValuePropertyName { get; set; }
		public string TooltipPropertyName { get; set; }

		private DataTemplate template;

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (template != null)
				return template;
			var textBlock = new FrameworkElementFactory(typeof(TextBlock), "textBlock");
			textBlock.SetValue(FrameworkElement.MarginProperty, new Thickness(3, 1, 0, 0));
			textBlock.SetBinding(TextBlock.TextProperty, new Binding(ValuePropertyName) { StringFormat = "X8" });
			var textBox = new FrameworkElementFactory(typeof(TextBox), "textBox");
			textBox.SetBinding(TextBox.TextProperty, new Binding(ValuePropertyName) { StringFormat = "X8", Mode = BindingMode.OneWay });
			textBox.SetValue(TextBox.VisibilityProperty, Visibility.Hidden);
			textBox.SetValue(TextBox.IsReadOnlyCaretVisibleProperty, true);
			textBox.SetValue(TextBox.IsReadOnlyProperty, true);
			if (TooltipPropertyName != null)
				textBox.SetBinding(FrameworkElement.ToolTipProperty, new Binding(TooltipPropertyName));
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
	}
}
