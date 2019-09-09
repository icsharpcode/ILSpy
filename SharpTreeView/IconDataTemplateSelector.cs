using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ICSharpCode.TreeView
{
	class IconDataTemplateSelector : DataTemplateSelector
	{
		public DataTemplate ImageDataTemplate { get; set; }

		public DataTemplate VectorDataTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item is ImageSource)
				return ImageDataTemplate;
			if (item is DrawingGroup)
				return VectorDataTemplate;

			return base.SelectTemplate(item, container);
		}
	}
}
