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
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;

using ICSharpCode.ILSpy.Metadata.Filters;

namespace ICSharpCode.ILSpy.Views.Filters
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

			// Header row at the top: hint on the left, Clear button on the right. Same
			// content the popup used to put at the bottom — moved to the top so the
			// "how to use this" text and the explicit clear affordance are both visible
			// without scrolling past the chip groups.
			var clearButton = new Button {
				Content = "Clear",
				MinHeight = 0,
				Padding = new Thickness(6, 1),
				VerticalAlignment = VerticalAlignment.Top,
			};
			clearButton.Click += (_, _) => Clear();
			var chipHint = new TextBlock {
				Text = "Click a chip to select only that value · Shift+Click to add to / remove from the selection · Click <Any> to clear",
				FontStyle = FontStyle.Italic,
				Foreground = Brushes.Gray,
				TextWrapping = TextWrapping.Wrap,
				VerticalAlignment = VerticalAlignment.Center,
			};
			var headerRow = new DockPanel { LastChildFill = true, Margin = new Thickness(0, 0, 0, 4) };
			DockPanel.SetDock(clearButton, global::Avalonia.Controls.Dock.Right);
			headerRow.Children.Add(clearButton);
			headerRow.Children.Add(chipHint);
			stack.Children.Add(headerRow);

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

			Content = new Border {
				Padding = new Thickness(8),
				MinWidth = 220,
				Child = stack,
			};

			// Arrow-key nav between chips. Avalonia doesn't ship WPF's DirectionalNavigation
			// attached property, so we handle Left/Right/Up/Down ourselves. Tunnel so we
			// intercept before inner controls (the Clear button consumes Enter; arrow keys
			// otherwise pass through to whatever has focus and do nothing).
			AddHandler(KeyDownEvent, OnArrowKeyDown, RoutingStrategies.Tunnel);

			state.PropertyChanged += OnStateChanged;
			RefreshSummary();
		}

		void OnArrowKeyDown(object? sender, KeyEventArgs e)
		{
			if (e.Key is not (Key.Left or Key.Right or Key.Up or Key.Down))
				return;
			// e.Source on a tunneled key event is the focused element about to receive it,
			// regardless of whether FocusManager has stabilised in this headless tick.
			// Fall back to FocusManager for completeness — both should usually agree.
			var focused = (e.Source as Control)
				?? (TopLevel.GetTopLevel(this)?.FocusManager?.GetFocusedElement() as Control);
			if (focused is null)
				return;

			var groups = this.GetVisualDescendants()
				.Where(c => c is MutexChipGroup or IndependentFlagGroup)
				.OfType<Control>()
				.ToList();
			if (groups.Count == 0)
				return;

			Control? target = null;
			if (e.Key is Key.Left or Key.Right)
			{
				// Flat document-order traversal: every chip / pill across the whole popup
				// is one sequence. Left = previous, Right = next, wrap at ends so the user
				// doesn't get stuck.
				var flat = ChipsInDocumentOrder(groups).ToList();
				int idx = flat.IndexOf(focused);
				if (idx < 0)
					return;
				int step = e.Key == Key.Right ? 1 : -1;
				target = flat[(idx + step + flat.Count) % flat.Count];
			}
			else
			{
				// Up / Down: jump to the previous / next group's first chip. Lets the user
				// hop between axes without arrowing through every value chip in between.
				int currentGroup = FindGroupContaining(groups, focused);
				if (currentGroup < 0)
					return;
				int step = e.Key == Key.Down ? 1 : -1;
				int newGroup = currentGroup + step;
				if (newGroup < 0 || newGroup >= groups.Count)
					return;
				target = ChipsIn(groups[newGroup]).FirstOrDefault();
			}

			if (target is not null)
			{
				target.Focus();
				e.Handled = true;
			}
		}

		static int FindGroupContaining(IReadOnlyList<Control> groups, Control element)
		{
			for (int i = 0; i < groups.Count; i++)
				if (groups[i] == element || groups[i].GetVisualDescendants().Contains(element))
					return i;
			return -1;
		}

		// Each group exposes its own NavigableElements list — using that instead of a
		// blanket descendant-walk skips past internal template parts (e.g. ComboBox's
		// own toggle button) so arrow nav lands only on user-facing chips/pills.
		static IEnumerable<Control> ChipsIn(Control group) => group switch {
			MutexChipGroup mutex => mutex.NavigableElements,
			IndependentFlagGroup indep => indep.NavigableElements,
			_ => Enumerable.Empty<Control>(),
		};

		static IEnumerable<Control> ChipsInDocumentOrder(IEnumerable<Control> groups)
		{
			foreach (var group in groups)
				foreach (var chip in ChipsIn(group))
					yield return chip;
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
