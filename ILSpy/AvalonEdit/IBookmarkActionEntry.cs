using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.ILSpy.AvalonEdit
{
	public interface IBookmarkActionEntry
	{
		bool IsEnabled();

		void Execute(int line);
	}
}
