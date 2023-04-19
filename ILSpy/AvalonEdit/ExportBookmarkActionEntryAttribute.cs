using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.ILSpy.AvalonEdit
{
	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ExportBookmarkActionEntryAttribute : ExportAttribute, IBookmarkActionMetadata
	{
		public string Icon { get; set; }

		public string Header { get; set; }

		public string Category { get; set; }

		public double Order { get; set; }

		public ExportBookmarkActionEntryAttribute()
			: base(typeof(IBookmarkActionEntry))
		{
		}
	}
}

