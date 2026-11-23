// Copyright (c) 2026 Piero Viano
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ICSharpCode.ILSpy.Navigation
{
	/// <summary>One row of a <see cref="QuickPickWindow"/>: a primary text, an optional secondary
	/// detail (assembly, code preview, ...), an optional icon and the object the row stands for.</summary>
	public sealed class QuickPickItem(string text, string? detail, IImage? icon, object payload)
	{
		public string Text { get; } = text ?? string.Empty;
		public string? Detail { get; } = detail;
		public IImage? Icon { get; } = icon;
		public object Payload { get; } = payload ?? throw new ArgumentNullException(nameof(payload));
	}

	/// <summary>
	/// State of a searchable pick list: the full item list, the filter text, the items matching it
	/// and the selected one. Every whitespace-separated filter term must occur (case-insensitively)
	/// in the item's text or detail. Accepting hands the selected item to the callback and asks the
	/// hosting window to close.
	/// </summary>
	public sealed partial class QuickPickModel : ObservableObject
	{
		readonly IReadOnlyList<QuickPickItem> allItems;
		readonly Action<QuickPickItem> onAccept;

		public QuickPickModel(IEnumerable<QuickPickItem> items, Action<QuickPickItem> onAccept)
		{
			ArgumentNullException.ThrowIfNull(items);
			this.onAccept = onAccept ?? throw new ArgumentNullException(nameof(onAccept));
			allItems = items.ToList();
			Refilter();
		}

		public ObservableCollection<QuickPickItem> FilteredItems { get; } = new();

		[ObservableProperty]
		string filter = string.Empty;

		[ObservableProperty]
		QuickPickItem? selectedItem;

		/// <summary>Raised when the model is done (an item was accepted) and its host should close.</summary>
		public event EventHandler? CloseRequested;

		partial void OnFilterChanged(string value) => Refilter();

		void Refilter()
		{
			var terms = (Filter ?? string.Empty).Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
			FilteredItems.Clear();
			foreach (var item in allItems)
			{
				if (terms.All(t => Matches(item, t)))
					FilteredItems.Add(item);
			}
			SelectedItem = FilteredItems.Count > 0 ? FilteredItems[0] : null;
		}

		static bool Matches(QuickPickItem item, string term)
			=> item.Text.Contains(term, StringComparison.OrdinalIgnoreCase)
				|| (item.Detail?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false);

		/// <summary>Moves the selection by <paramref name="delta"/> rows, clamped to the filtered list.</summary>
		public void MoveSelection(int delta)
		{
			if (FilteredItems.Count == 0)
				return;
			int index = SelectedItem is { } current ? FilteredItems.IndexOf(current) : -1;
			index = Math.Clamp(index + delta, 0, FilteredItems.Count - 1);
			SelectedItem = FilteredItems[index];
		}

		/// <summary>Accepts the selected item. Returns false when nothing is selected.</summary>
		public bool Accept()
		{
			if (SelectedItem is not { } item)
				return false;
			CloseRequested?.Invoke(this, EventArgs.Empty);
			onAccept(item);
			return true;
		}
	}

	/// <summary>
	/// Small searchable popup window over a <see cref="QuickPickModel"/>: a filter box above a list.
	/// Typing filters, Up/Down move the selection, Enter or a double-click accepts, Escape closes.
	/// </summary>
	public sealed class QuickPickWindow : Window
	{
		readonly TextBox filterBox;
		readonly ListBox list;
		bool syncingSelection;

		public QuickPickModel Model { get; }

		public QuickPickWindow(string title, QuickPickModel model)
		{
			Model = model ?? throw new ArgumentNullException(nameof(model));
			Title = title;
			Width = 640;
			Height = 420;
			CanResize = true;
			ShowInTaskbar = false;
			WindowStartupLocation = WindowStartupLocation.CenterOwner;

			filterBox = new TextBox { Margin = new Thickness(6), Text = model.Filter };
			list = new ListBox {
				Margin = new Thickness(6, 0, 6, 6),
				ItemsSource = model.FilteredItems,
				ItemTemplate = new FuncDataTemplate<QuickPickItem>((item, _) => BuildRow(item), supportsRecycling: false),
				SelectedItem = model.SelectedItem,
			};
			var root = new DockPanel();
			DockPanel.SetDock(filterBox, global::Avalonia.Controls.Dock.Top);
			root.Children.Add(filterBox);
			root.Children.Add(list);
			Content = root;

			filterBox.TextChanged += (_, _) => Model.Filter = filterBox.Text ?? string.Empty;
			list.SelectionChanged += (_, _) => {
				if (!syncingSelection && list.SelectedItem is QuickPickItem item)
					Model.SelectedItem = item;
			};
			list.DoubleTapped += (_, _) => Model.Accept();
			Model.PropertyChanged += (_, e) => {
				if (e.PropertyName == nameof(QuickPickModel.SelectedItem))
					SyncSelection();
			};
			Model.CloseRequested += (_, _) => Close();
			AddHandler(KeyDownEvent, OnPreviewKeyDown, RoutingStrategies.Tunnel);
			Opened += (_, _) => Dispatcher.UIThread.Post(() => filterBox.Focus());
		}

		/// <summary>Creates the popup for <paramref name="model"/> and shows it owned by <paramref name="owner"/>.</summary>
		public static QuickPickWindow Show(Window owner, string title, QuickPickModel model)
		{
			ArgumentNullException.ThrowIfNull(owner);
			var window = new QuickPickWindow(title, model);
			window.Show(owner);
			return window;
		}

		void SyncSelection()
		{
			syncingSelection = true;
			try
			{
				list.SelectedItem = Model.SelectedItem;
				if (Model.SelectedItem is { } item)
					list.ScrollIntoView(item);
			}
			finally
			{
				syncingSelection = false;
			}
		}

		void OnPreviewKeyDown(object? sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Escape:
					Close();
					e.Handled = true;
					break;
				case Key.Enter:
					Model.Accept();
					e.Handled = true;
					break;
				case Key.Down:
					Model.MoveSelection(1);
					e.Handled = true;
					break;
				case Key.Up:
					Model.MoveSelection(-1);
					e.Handled = true;
					break;
				case Key.PageDown:
					Model.MoveSelection(10);
					e.Handled = true;
					break;
				case Key.PageUp:
					Model.MoveSelection(-10);
					e.Handled = true;
					break;
			}
		}

		static Control BuildRow(QuickPickItem? item)
		{
			var row = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6 };
			if (item == null)
				return row;
			if (item.Icon != null)
				row.Children.Add(new Image { Source = item.Icon, Width = 16, Height = 16, VerticalAlignment = VerticalAlignment.Center });
			row.Children.Add(new TextBlock { Text = item.Text, VerticalAlignment = VerticalAlignment.Center });
			if (!string.IsNullOrEmpty(item.Detail))
			{
				row.Children.Add(new TextBlock {
					Text = item.Detail,
					Opacity = 0.65,
					VerticalAlignment = VerticalAlignment.Center,
					TextTrimming = TextTrimming.CharacterEllipsis,
				});
			}
			return row;
		}
	}
}
