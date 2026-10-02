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
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;

namespace ICSharpCode.ILSpy.Navigation
{
	/// <summary>One row of a <see cref="NavigationChooser"/>: the text shown and the action run when it is picked.</summary>
	public sealed record NavigationChoice(string Text, Action Execute);

	/// <summary>
	/// Small searchable list shown in a flyout: a filter box over a list of choices. Typing filters
	/// the list (every whitespace-separated term must occur, case-insensitively), Up/Down move the
	/// selection, Enter or a click runs the selected choice, Escape closes without running anything.
	/// </summary>
	public sealed class NavigationChooser
	{
		readonly IReadOnlyList<NavigationChoice> choices;
		readonly ListBox list;
		readonly Flyout flyout;
		bool isOpen;

		public NavigationChooser(string title, IReadOnlyList<NavigationChoice> choices)
		{
			ArgumentNullException.ThrowIfNull(choices);
			this.choices = choices;
			Title = title;

			FilterBox = new TextBox {
				PlaceholderText = "Type to filter",
				MinWidth = 380,
			};
			FilterBox.PropertyChanged += (_, e) => {
				if (e.Property == TextBox.TextProperty)
					ApplyFilter();
			};
			list = new ListBox {
				MaxHeight = 360,
				ItemTemplate = new FuncDataTemplate<NavigationChoice>((choice, _) => new TextBlock { Text = choice?.Text }),
			};
			list.Tapped += (_, _) => {
				if (list.SelectedItem is NavigationChoice)
					Accept();
			};

			var root = new StackPanel {
				Orientation = Orientation.Vertical,
				Spacing = 4,
				Margin = new Thickness(4),
				Children = {
					new TextBlock { Text = title, FontWeight = FontWeight.SemiBold },
					FilterBox,
					list,
				},
			};
			root.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);

			flyout = new Flyout {
				Content = root,
				Placement = PlacementMode.Center,
			};
			flyout.Opened += (_, _) => Dispatcher.UIThread.Post(() => FilterBox.Focus());
			flyout.Closed += (_, _) => {
				isOpen = false;
				Closed?.Invoke(this, EventArgs.Empty);
			};
			ApplyFilter();
		}

		public string Title { get; }

		/// <summary>The filter text box; it holds keyboard focus while the chooser is open.</summary>
		internal TextBox FilterBox { get; }

		/// <summary>The choices matching the current filter, in display order.</summary>
		public IReadOnlyList<NavigationChoice> VisibleChoices { get; private set; } = Array.Empty<NavigationChoice>();

		public string Filter {
			get => FilterBox.Text ?? string.Empty;
			set => FilterBox.Text = value;
		}

		public bool IsOpen => isOpen;

		/// <summary>Raised once the chooser has closed, whether a choice ran or not.</summary>
		public event EventHandler? Closed;

		public void Show(Control anchor)
		{
			ArgumentNullException.ThrowIfNull(anchor);
			isOpen = true;
			flyout.ShowAt(anchor);
		}

		public void Close()
		{
			if (!isOpen)
				return;
			flyout.Hide();
			// Flyout.Hide raises Closed only when the popup was really open; keep the state
			// consistent when it was not (e.g. the anchor left the visual tree).
			if (isOpen)
			{
				isOpen = false;
				Closed?.Invoke(this, EventArgs.Empty);
			}
		}

		/// <summary>Closes the chooser and runs the selected choice, or the first visible one when none is selected.</summary>
		public void Accept()
		{
			var choice = list.SelectedItem as NavigationChoice ?? VisibleChoices.FirstOrDefault();
			if (choice == null)
				return;
			Close();
			choice.Execute();
		}

		void ApplyFilter()
		{
			var terms = Filter.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
			VisibleChoices = choices
				.Where(c => terms.All(t => c.Text.Contains(t, StringComparison.OrdinalIgnoreCase)))
				.ToList();
			list.ItemsSource = VisibleChoices;
			list.SelectedIndex = VisibleChoices.Count > 0 ? 0 : -1;
		}

		void OnKeyDown(object? sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Enter:
					Accept();
					e.Handled = true;
					break;
				case Key.Escape:
					Close();
					e.Handled = true;
					break;
				case Key.Down:
				case Key.Up:
					if (VisibleChoices.Count == 0)
						break;
					var step = e.Key == Key.Down ? 1 : -1;
					list.SelectedIndex = Math.Clamp(list.SelectedIndex + step, 0, VisibleChoices.Count - 1);
					if (list.SelectedItem != null)
						list.ScrollIntoView(list.SelectedItem);
					e.Handled = true;
					break;
			}
		}
	}
}
