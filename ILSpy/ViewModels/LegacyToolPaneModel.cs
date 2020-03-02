using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ICSharpCode.ILSpy.ViewModels
{
	internal enum LegacyToolPaneLocation
	{
		Top,
		Bottom
	}

	internal class LegacyToolPaneModel : ToolPaneModel
	{
		public LegacyToolPaneModel(string title, object content, LegacyToolPaneLocation location)
		{
			this.Title = title;
			this.Content = content;
			this.IsCloseable = true;
			this.Location = location;
		}

		public object Content { get; }

		public override DataTemplate Template => throw new NotSupportedException();

		public LegacyToolPaneLocation Location { get; }
	}
}
