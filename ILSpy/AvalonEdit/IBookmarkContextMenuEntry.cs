using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ICSharpCode.ILSpy.Bookmarks;

namespace ICSharpCode.ILSpy.AvalonEdit
{
	public interface IBookmarkContextMenuEntry
	{
		bool IsVisible(IBookmark bookmarks);

		bool IsEnabled(IBookmark bookmarks);

		void Execute(IBookmark bookmarks);
	}
}
