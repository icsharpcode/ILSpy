using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.ILSpy.AvalonEdit
{
	internal interface IBookmarkContextMenuEntryMetadata
	{
		string Icon { get; }

		string Header { get; }

		string Category { get; }

		double Order { get; }
	}
}
