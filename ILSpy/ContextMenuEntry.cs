// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.TreeView;

namespace ICSharpCode.ILSpy
{
	public interface IContextMenuEntry
	{
		bool IsVisible(TextViewContext context);
		bool IsEnabled(TextViewContext context);
		void Execute(TextViewContext context);
	}
	
	public class TextViewContext
	{
		/// <summary>
		/// Returns the selected nodes in the tree view.
		/// Returns null, if context menu does not belong to a tree view.
		/// </summary>
		public SharpTreeNode[] SelectedTreeNodes { get; private set; }
		
		/// <summary>
		/// Returns the tree view the context menu is assigned to.
		/// Returns null, if context menu is not assigned to a tree view.
		/// </summary>
		public SharpTreeView TreeView { get; private set; }
		
		/// <summary>
		/// Returns the text view the context menu is assigned to.
		/// Returns null, if context menu is not assigned to a text view.
		/// </summary>
		public DecompilerTextView TextView { get; private set; }

		/// <summary>
		/// Returns the list box the context menu is assigned to.
		/// Returns null, if context menu is not assigned to a list box.
		/// </summary>
		public ListBox ListBox { get; private set; }

		/// <summary>
		/// Returns the data grid the context menu is assigned to.
		/// Returns null, if context menu is not assigned to a data grid.
		/// </summary>
		public DataGrid DataGrid { get; private set; }

		/// <summary>
		/// Returns the reference the mouse cursor is currently hovering above.
		/// Returns null, if there was no reference found.
		/// </summary>
		public ReferenceSegment Reference { get; private set; }
		
		/// <summary>
		/// Returns the position in TextView the mouse cursor is currently hovering above.
		/// Returns null, if TextView returns null;
		/// </summary>
		public TextViewPosition? Position { get; private set; }

		public Point MousePosition { get; private set; }

		public (GridViewColumn, object) GetColumnAndRowFromMousePosition()
		{
			if (!(ListBox is ListView listView))
				return (null, null);

			GridViewColumn column = null;
			var directChild = (FrameworkElement)listView.InputHitTest(listView.PointFromScreen(MousePosition));
			var (presenter, previous) = GetAncestorOf<GridViewRowPresenter>(directChild);
			if (presenter == null)
				return (null, null);
			(var container, _) = GetAncestorOf<ListViewItem>(presenter);
			var item = listView.ItemContainerGenerator.ItemFromContainer(container);

			int count = VisualTreeHelper.GetChildrenCount(presenter);
			for (int i = 0; i < count; i++) {
				if (VisualTreeHelper.GetChild(presenter, i) != previous)
					continue;
				column = presenter.Columns[i];
				break;
			}
			return (column, item);

			(T, FrameworkElement) GetAncestorOf<T>(FrameworkElement element) where T : FrameworkElement
			{
				var current = element;
				FrameworkElement prev = null;
				while (current != null && !(current is T)) {
					prev = current;
					current = (FrameworkElement)VisualTreeHelper.GetParent(current);
				}
				return (current as T, prev);
			}
		}

		public static TextViewContext Create(SharpTreeView treeView = null, DecompilerTextView textView = null, ListBox listBox = null, DataGrid dataGrid = null)
		{
			ReferenceSegment reference;
			if (textView != null)
				reference = textView.GetReferenceSegmentAtMousePosition();
			else if (listBox?.SelectedItem is SearchResult result)
				reference = new ReferenceSegment { Reference = result.Reference };
			else if (listBox?.SelectedItem is TreeNodes.IMemberTreeNode provider)
				reference = new ReferenceSegment { Reference = provider.Member };
			else if (listBox?.SelectedItem != null)
				reference = new ReferenceSegment { Reference = listBox.SelectedItem };
			else if (dataGrid?.SelectedItem is TreeNodes.IMemberTreeNode provider2)
				reference = new ReferenceSegment { Reference = provider2.Member };
			else if (dataGrid?.SelectedItem != null)
				reference = new ReferenceSegment { Reference = dataGrid.SelectedItem };
			else
				reference = null;
			var position = textView != null ? textView.GetPositionFromMousePosition() : null;
			var selectedTreeNodes = treeView != null ? treeView.GetTopLevelSelection().ToArray() : null;
			return new TextViewContext {
				ListBox = listBox,
				DataGrid = dataGrid,
				TreeView = treeView,
				SelectedTreeNodes = selectedTreeNodes,
				TextView = textView,
				Reference = reference,
				Position = position,
				MousePosition = ((Visual)textView ?? treeView ?? (Visual)listBox ?? dataGrid).PointToScreen(Mouse.GetPosition((IInputElement)textView ?? treeView ?? (IInputElement)listBox ?? dataGrid))
			};
		}
	}
	
	public interface IContextMenuEntryMetadata
	{
		string Icon { get; }
		string Header { get; }
		string Category { get; }
		string InputGestureText { get; }

		double Order { get; }
	}
	
	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class ExportContextMenuEntryAttribute : ExportAttribute, IContextMenuEntryMetadata
	{
		public ExportContextMenuEntryAttribute()
			: base(typeof(IContextMenuEntry))
		{
			// entries default to end of menu unless given specific order position
			Order = double.MaxValue;
		}
		
		public string Icon { get; set; }
		public string Header { get; set; }
		public string Category { get; set; }
		public string InputGestureText { get; set; }
		public double Order { get; set; }
	}
	
	internal class ContextMenuProvider
	{
		/// <summary>
		/// Enables extensible context menu support for the specified tree view.
		/// </summary>
		public static void Add(SharpTreeView treeView)
		{
			var provider = new ContextMenuProvider(treeView);
			treeView.ContextMenuOpening += provider.treeView_ContextMenuOpening;
			// Context menu is shown only when the ContextMenu property is not null before the
			// ContextMenuOpening event handler is called.
			treeView.ContextMenu = new ContextMenu();
		}

		public static void Add(DecompilerTextView textView)
		{
			var provider = new ContextMenuProvider(textView);
			textView.ContextMenuOpening += provider.textView_ContextMenuOpening;
			// Context menu is shown only when the ContextMenu property is not null before the
			// ContextMenuOpening event handler is called.
			textView.ContextMenu = new ContextMenu();
		}

		public static void Add(ListBox listBox)
		{
			var provider = new ContextMenuProvider(listBox);
			listBox.ContextMenuOpening += provider.listBox_ContextMenuOpening;
			listBox.ContextMenu = new ContextMenu();
		}

		public static void Add(DataGrid dataGrid)
		{
			var provider = new ContextMenuProvider(dataGrid);
			dataGrid.ContextMenuOpening += provider.dataGrid_ContextMenuOpening;
			dataGrid.ContextMenu = new ContextMenu();
		}

		readonly SharpTreeView treeView;
		readonly DecompilerTextView textView;
		readonly ListBox listBox;
		readonly DataGrid dataGrid;
		readonly Lazy<IContextMenuEntry, IContextMenuEntryMetadata>[] entries;
		
		private ContextMenuProvider()
		{
			entries = App.ExportProvider.GetExports<IContextMenuEntry, IContextMenuEntryMetadata>().ToArray();
		}

		ContextMenuProvider(DecompilerTextView textView)
			: this()
		{
			this.textView = textView ?? throw new ArgumentNullException(nameof(textView));
		}
		
		ContextMenuProvider(SharpTreeView treeView)
			: this()
		{
			this.treeView = treeView ?? throw new ArgumentNullException(nameof(treeView));
		}

		ContextMenuProvider(ListBox listBox)
			: this()
		{
			this.listBox = listBox ?? throw new ArgumentNullException(nameof(listBox));
		}

		ContextMenuProvider(DataGrid dataGrid)
			: this()
		{
			this.dataGrid = dataGrid ?? throw new ArgumentNullException(nameof(dataGrid));
		}

		void treeView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			TextViewContext context = TextViewContext.Create(treeView);
			if (context.SelectedTreeNodes.Length == 0) {
				e.Handled = true; // don't show the menu
				return;
			}
			ContextMenu menu;
			if (ShowContextMenu(context, out menu))
				treeView.ContextMenu = menu;
			else
				// hide the context menu.
				e.Handled = true;
		}
		
		void textView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			TextViewContext context = TextViewContext.Create(textView: textView);
			ContextMenu menu;
			if (ShowContextMenu(context, out menu))
				textView.ContextMenu = menu;
			else
				// hide the context menu.
				e.Handled = true;
		}

		void listBox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			TextViewContext context = TextViewContext.Create(listBox: listBox);
			ContextMenu menu;
			if (ShowContextMenu(context, out menu))
				listBox.ContextMenu = menu;
			else
				// hide the context menu.
				e.Handled = true;
		}

		void dataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			TextViewContext context = TextViewContext.Create(dataGrid: dataGrid);
			ContextMenu menu;
			if (ShowContextMenu(context, out menu))
				dataGrid.ContextMenu = menu;
			else
				// hide the context menu.
				e.Handled = true;
		}

		bool ShowContextMenu(TextViewContext context, out ContextMenu menu)
		{
			menu = new ContextMenu();
			foreach (var category in entries.OrderBy(c => c.Metadata.Order).GroupBy(c => c.Metadata.Category)) {
				bool needSeparatorForCategory = menu.Items.Count > 0;
				foreach (var entryPair in category) {
					IContextMenuEntry entry = entryPair.Value;
					if (entry.IsVisible(context)) {
						if (needSeparatorForCategory) {
							menu.Items.Add(new Separator());
							needSeparatorForCategory = false;
						}
						MenuItem menuItem = new MenuItem();
						menuItem.Header = MainWindow.GetResourceString(entryPair.Metadata.Header);
						menuItem.InputGestureText = entryPair.Metadata.InputGestureText;
						if (!string.IsNullOrEmpty(entryPair.Metadata.Icon)) {
							menuItem.Icon = new Image {
								Width = 16,
								Height = 16,
								Source = Images.Load(entryPair.Value, entryPair.Metadata.Icon)
							};
						}
						if (entryPair.Value.IsEnabled(context)) {
							menuItem.Click += delegate { entry.Execute(context); };
						} else
							menuItem.IsEnabled = false;
						menu.Items.Add(menuItem);
					}
				}
			}
			return menu.Items.Count > 0;
		}
	}
}
