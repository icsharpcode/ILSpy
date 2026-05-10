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
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

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

			ApplyAccentPalette(Styles);

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

			// Hint text in the same style as the live filter summary below — just a quiet
			// italic line explaining the chip click semantics so users don't have to guess.
			var chipHint = new TextBlock {
				Text = "Click a chip to select only that value · Shift+Click to add to / remove from the selection · Click <Any> to clear",
				FontStyle = FontStyle.Italic,
				Foreground = Brushes.Gray,
				TextWrapping = TextWrapping.Wrap,
				Margin = new Thickness(0, 6, 0, 0),
			};
			stack.Children.Add(chipHint);

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

		/// <summary>
		/// Mirrors <c>MainToolBar.axaml</c>'s accent palette into this popup. The Simple
		/// theme's default <c>:checked</c> background for ToggleButton paints a saturated
		/// dark blue that buries the chip's label, so an active chip becomes unreadable —
		/// override the inner ContentPresenter background with the same translucent accent
		/// the toolbar uses on hover (#330078D7) so the label stays legible while the chip
		/// still reads as "selected". Pressed states stay slightly denser (#660078D7) to
		/// give a click cue.
		/// </summary>
		static void ApplyAccentPalette(Styles target)
		{
			var hoverBg = new SolidColorBrush(Color.Parse("#330078D7"));
			var hoverBorder = new SolidColorBrush(Color.Parse("#FF0078D7"));
			var pressedBg = new SolidColorBrush(Color.Parse("#660078D7"));
			var pressedBorder = new SolidColorBrush(Color.Parse("#FF005A9E"));

			target.Add(MakeStyle(s => s.OfType<ToggleButton>().Class(":pointerover").Template().OfType<ContentPresenter>(), hoverBg, hoverBorder));
			target.Add(MakeStyle(s => s.OfType<ToggleButton>().Class(":checked").Template().OfType<ContentPresenter>(), hoverBg, hoverBorder));
			target.Add(MakeStyle(s => s.OfType<ToggleButton>().Class(":pressed").Template().OfType<ContentPresenter>(), pressedBg, pressedBorder));
			target.Add(MakeStyle(s => s.OfType<Button>().Class(":pointerover").Template().OfType<ContentPresenter>(), hoverBg, hoverBorder));
			target.Add(MakeStyle(s => s.OfType<Button>().Class(":pressed").Template().OfType<ContentPresenter>(), pressedBg, pressedBorder));

			static Style MakeStyle(Func<Selector?, Selector> selector, IBrush bg, IBrush border)
			{
				var style = new Style(selector);
				style.Setters.Add(new Setter(ContentPresenter.BackgroundProperty, bg));
				style.Setters.Add(new Setter(ContentPresenter.BorderBrushProperty, border));
				return style;
			}
		}
	}
}
