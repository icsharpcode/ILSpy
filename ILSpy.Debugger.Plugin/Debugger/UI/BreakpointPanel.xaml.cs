using ICSharpCode.ILSpy.Bookmarks;
using ICSharpCode.ILSpy.Debugger.Bookmarks;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ICSharpCode.ILSpy.Debugger.UI
{
    public partial class BreakpointPanel : UserControl, IPane
    {
        public static BreakpointPanel Instance
        {
            get
            {
                if (BreakpointPanel.s_instance == null)
                {
                    Application.Current.VerifyAccess();
                    BreakpointPanel.s_instance = new BreakpointPanel();
                }
                return BreakpointPanel.s_instance;
            }
        }

        private BreakpointPanel()
        {
            this.InitializeComponent();
        }

        public void Show()
        {
            if (!base.IsVisible)
            {
                this.SetItemSource();
                MainWindow.Instance.ShowInBottomPane("Breakpoints", this);
                BookmarkManager.Added += delegate (object A_1, BookmarkEventArgs A_2)
                {
                    this.SetItemSource();
                };
                BookmarkManager.Removed += delegate (object A_1, BookmarkEventArgs A_2)
                {
                    this.SetItemSource();
                };
                DebuggerSettings.Instance.PropertyChanged += delegate (object s, PropertyChangedEventArgs e)
                {
                    if (e.PropertyName == "ShowAllBookmarks")
                    {
                        this.SetItemSource();
                    }
                };
            }
        }

        private void SetItemSource()
        {
            this.view.ItemsSource = null;
            if (DebuggerSettings.Instance.ShowAllBookmarks)
            {
                this.view.ItemsSource = BookmarkManager.Bookmarks;
                return;
            }
            this.view.ItemsSource = from b in BookmarkManager.Bookmarks
                                    where b is BreakpointBookmark
                                    select b;
        }

        public void Closed()
        {
            BookmarkManager.Added -= delegate (object A_1, BookmarkEventArgs A_2)
            {
                this.SetItemSource();
            };
            BookmarkManager.Removed -= delegate (object A_1, BookmarkEventArgs A_2)
            {
                this.SetItemSource();
            };
            DebuggerSettings.Instance.PropertyChanged -= delegate (object s, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "ShowAllBookmarks")
                {
                    this.SetItemSource();
                }
            };
        }

        private void view_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }
            BookmarkBase bookmarkBase = this.view.SelectedItem as BookmarkBase;
            if (bookmarkBase == null)
            {
                return;
            }
            MainWindow.Instance.JumpToReference(bookmarkBase.MemberReference);
            MainWindow.Instance.TextView.UnfoldAndScroll(bookmarkBase.LineNumber);
            e.Handled = true;
        }

        private void view_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Delete)
            {
                return;
            }
            BookmarkBase bookmarkBase = this.view.SelectedItem as BookmarkBase;
            if (bookmarkBase == null)
            {
                return;
            }
            BookmarkManager.RemoveMark(bookmarkBase);
            e.Handled = true;
        }

        private static BreakpointPanel s_instance;
    }
}
