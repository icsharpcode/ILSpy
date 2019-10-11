using System.Windows;
using System.Windows.Controls;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Docking
{
	public class PaneStyleSelector : StyleSelector
	{
		public Style ToolPaneStyle { get; set; }

		public Style DocumentStyle { get; set; }

		public override Style SelectStyle(object item, DependencyObject container)
		{
			if (item is DocumentModel)
				return DocumentStyle;

			if (item is ToolPaneModel)
				return ToolPaneStyle;

			return base.SelectStyle(item, container);
		}
	}
}
