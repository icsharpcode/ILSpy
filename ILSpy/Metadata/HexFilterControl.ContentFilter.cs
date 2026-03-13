using System;

using DataGridExtensions;

namespace ICSharpCode.ILSpy.Metadata
{
	public partial class HexFilterControl
	{
		class ContentFilter : IContentFilter
		{
			readonly string filter;

			public ContentFilter(string filter)
			{
				this.filter = filter;
			}

			public bool IsMatch(object value)
			{
				if (string.IsNullOrWhiteSpace(filter))
					return true;
				if (value == null)
					return false;

				return string.Format("{0:x8}", value).IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
			}

			public string Value => filter;
		}
	}
}
