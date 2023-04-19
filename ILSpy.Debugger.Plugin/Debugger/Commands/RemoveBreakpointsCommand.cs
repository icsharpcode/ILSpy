using ICSharpCode.ILSpy.Bookmarks;
using ICSharpCode.ILSpy.Debugger.Bookmarks;

namespace ICSharpCode.ILSpy.Debugger.Commands
{
    [ExportMainMenuCommand(Menu = "_Debugger", MenuIcon = "Images/DeleteAllBreakpoints.png", MenuCategory = "Others", Header = "Remove all _breakpoints", MenuOrder = 7.0)]
    internal sealed class RemoveBreakpointsCommand : DebuggerCommand
    {
        public override void Execute(object parameter)
        {
            for (int i = BookmarkManager.Bookmarks.Count - 1; i >= 0; i--)
            {
                BookmarkBase bookmarkBase = BookmarkManager.Bookmarks[i];
                if (bookmarkBase is BreakpointBookmark)
                {
                    BookmarkManager.RemoveMark(bookmarkBase);
                }
            }
        }
    }
}
