using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace ICSharpCode.ILSpy.Metadata
{
    static partial class Helpers
    {
		private static DataTemplate GetOrCreateLinkCellTemplate(string name, PropertyDescriptor descriptor, Binding binding)
		{
			if (linkCellTemplates.TryGetValue(name, out var template))
			{
				return template;
			}

			var tb = new FrameworkElementFactory(typeof(TextBlock));
			var hyper = new FrameworkElementFactory(typeof(Hyperlink));
			tb.AppendChild(hyper);
			hyper.AddHandler(Hyperlink.ClickEvent, new RoutedEventHandler(Hyperlink_Click));
			var run = new FrameworkElementFactory(typeof(Run));
			hyper.AppendChild(run);
			run.SetBinding(Run.TextProperty, binding);

			DataTemplate dataTemplate = new DataTemplate() { VisualTree = tb };
			linkCellTemplates.Add(name, dataTemplate);
			return dataTemplate;

			void Hyperlink_Click(object sender, RoutedEventArgs e)
			{
				var hyperlink = (Hyperlink)sender;
				var onClickMethod = descriptor.ComponentType.GetMethod("On" + name + "Click", BindingFlags.Instance | BindingFlags.Public);
				if (onClickMethod != null)
				{
					onClickMethod.Invoke(hyperlink.DataContext, Array.Empty<object>());
				}
			}
		}
    }
}