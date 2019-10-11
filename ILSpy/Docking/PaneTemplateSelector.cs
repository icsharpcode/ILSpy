using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.ILSpy.Docking
{
	public class TemplateMapping
	{
		public Type Type { get; set; }
		public DataTemplate Template { get; set; }
	}

	public class PaneTemplateSelector : DataTemplateSelector
	{
		public Collection<TemplateMapping> Mappings { get; set; } = new Collection<TemplateMapping>();

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			return Mappings.FirstOrDefault(m => m.Type == item.GetType())?.Template
				?? base.SelectTemplate(item, container);
		}
	}
}
