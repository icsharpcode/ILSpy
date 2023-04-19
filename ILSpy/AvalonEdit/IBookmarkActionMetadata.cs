using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.ILSpy.AvalonEdit
{
	public interface  IBookmarkActionMetadata
	{
		string Category { get; }

		double Order { get; }
	}
}
