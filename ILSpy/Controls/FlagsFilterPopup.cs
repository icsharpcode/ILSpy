// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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
using System.ComponentModel;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

using ILSpy.Metadata.Filters;

namespace ILSpy.Views.Filters
{
	/// <summary>
	/// Composes one <see cref="MutexChipGroup"/> per axis plus an
	/// <see cref="IndependentFlagGroup"/> for the schema's independent flags. Renders a
	/// summary line ("Visibility = Public ∧ Sealed ∧ ¬Abstract") that re-flows on every
	/// state change, and a Clear button that resets the state.
	/// </summary>
	public sealed class FlagsFilterPopup : UserControl
	{
		readonly FilterState state;
		readonly List<MutexChipGroup> mutexGroups = new();
		readonly IndependentFlagGroup? independentGroup;
		readonly TextBlock summary;

		public FlagsFilterPopup(FilterState state)
		{
			this.state = state ?? throw new ArgumentNullException(nameof(state));

			var stack = new StackPanel { Orientation = Orientation.Vertical, Spacing = 4 };
			foreach (var g in state.Schema.MutexGroups)
			{
				var chips = new MutexChipGroup(state, g);
				mutexGroups.Add(chips);
				stack.Children.Add(chips);
			}
			if (state.Schema.IndependentFlags.Count > 0)
			{
				independentGroup = new IndependentFlagGroup(state, state.Schema.IndependentFlags);
				stack.Children.Add(independentGroup);
			}

			summary = new TextBlock {
				FontStyle = FontStyle.Italic,
				Foreground = Brushes.Gray,
				TextWrapping = TextWrapping.Wrap,
				Margin = new Thickness(0, 6, 0, 0),
			};
			stack.Children.Add(summary);

			var clearButton = new Button {
				Content = "Clear",
				HorizontalAlignment = HorizontalAlignment.Right,
				Margin = new Thickness(0, 4, 0, 0),
			};
			clearButton.Click += (_, _) => Clear();
			stack.Children.Add(clearButton);

			Content = new Border {
				Padding = new Thickness(8),
				MinWidth = 220,
				Child = stack,
			};

			state.PropertyChanged += OnStateChanged;
			RefreshSummary();
		}

		void OnStateChanged(object? sender, PropertyChangedEventArgs e) => RefreshSummary();

		void RefreshSummary() => summary.Text = FilterStatePresenter.Describe(state);

		/// <summary>Resets the state and reflects that into every embedded control.</summary>
		public void Clear()
		{
			state.Clear();
			foreach (var g in mutexGroups)
				g.SyncFromState();
			independentGroup?.SyncFromState();
		}
	}
}
