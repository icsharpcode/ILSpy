using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

using ICSharpCode.ILSpy.Bookmarks;

namespace ICSharpCode.ILSpy.AvalonEdit
{
	internal class IconMarginActionsProvider
	{
		private readonly IconBarMargin margin;

		[ImportMany(typeof(IBookmarkContextMenuEntry))]
		private Lazy<IBookmarkContextMenuEntry, IBookmarkContextMenuEntryMetadata>[] contextEntries;

		[ImportMany(typeof(IBookmarkActionEntry))]
		private Lazy<IBookmarkActionEntry, IBookmarkActionMetadata>[] actionEntries;

		public static void Add(IconBarMargin margin)
		{
			IconMarginActionsProvider @object = new IconMarginActionsProvider(margin);
			margin.MouseUp += @object.HandleMouseEvent;
			margin.ContextMenu = new ContextMenu();
		}

		private IconMarginActionsProvider(IconBarMargin margin)
		{
			this.margin = margin;
			App.CompositionContainer.ComposeParts(this);
		}

		private void HandleMouseEvent(object sender, MouseButtonEventArgs e)
		{
			int line = margin.GetLineFromMousePosition(e);
			if (e.ChangedButton == MouseButton.Left)
			{
				foreach (IGrouping<string, Lazy<IBookmarkActionEntry, IBookmarkActionMetadata>> item in from c in actionEntries
																										orderby c.Metadata.Order
																										group c by c.Metadata.Category)
				{
					foreach (Lazy<IBookmarkActionEntry, IBookmarkActionMetadata> item2 in item)
					{
						IBookmarkActionEntry value = item2.Value;
						if (item2.Value.IsEnabled())
						{
							value.Execute(line);
						}
					}
				}
			}
			IBookmark[] array = margin.Manager.Bookmarks.ToArray();
			if (array.Length == 0)
			{
				e.Handled = true;
				margin.ContextMenu = null;
			}
			else
			{
				if (e.ChangedButton != MouseButton.Right)
				{
					return;
				}
				IBookmark bookmark = array.FirstOrDefault((IBookmark b) => b.LineNumber == line);
				if (bookmark == null)
				{
					e.Handled = true;
					margin.ContextMenu = null;
					return;
				}
				ContextMenu contextMenu = new ContextMenu();
				foreach (IGrouping<string, Lazy<IBookmarkContextMenuEntry, IBookmarkContextMenuEntryMetadata>> item3 in from c in contextEntries
																														orderby c.Metadata.Order
																														group c by c.Metadata.Category)
				{
					bool flag = true;
					foreach (Lazy<IBookmarkContextMenuEntry, IBookmarkContextMenuEntryMetadata> item4 in item3)
					{
						IBookmarkContextMenuEntry entry = item4.Value;
						if (!entry.IsVisible(bookmark))
						{
							continue;
						}
						if (flag && contextMenu.Items.Count > 0)
						{
							contextMenu.Items.Add(new Separator());
							flag = false;
						}
						MenuItem menuItem = new MenuItem();
						menuItem.Header = item4.Metadata.Header;
						if (!string.IsNullOrEmpty(item4.Metadata.Icon))
						{
							menuItem.Icon = new Image {
								Width = 16.0,
								Height = 16.0,
								Source = Images.LoadImage(entry, item4.Metadata.Icon)
							};
						}
						if (item4.Value.IsEnabled(bookmark))
						{
							menuItem.Click += delegate
							{
								entry.Execute(bookmark);
							};
						}
						else
						{
							menuItem.IsEnabled = false;
						}
						contextMenu.Items.Add(menuItem);
					}
				}
				if (contextMenu.Items.Count > 0)
				{
					margin.ContextMenu = contextMenu;
				}
				else
				{
					e.Handled = true;
				}
			}
		}
	}
}
}
