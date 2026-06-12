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
using System.Collections.Immutable;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

using ICSharpCode.ILSpy.Metadata.Filters;

namespace ICSharpCode.ILSpy.Views.Filters
{
	/// <summary>
	/// One mutex group rendered as a row of toggle chips. Chips drive
	/// <see cref="FilterState.MutexSelections"/> for the group; a leading "&lt;Any&gt;" chip
	/// represents the "no constraint" state. Per spec, selecting all values OR selecting
	/// none collapses back to "any" — the only paths to a non-null selection set are
	/// strict subsets.
	/// </summary>
	public sealed class MutexChipGroup : UserControl
	{
		readonly FilterState state;
		readonly MutexGroup group;
		readonly ToggleButton anyChip;
		readonly Dictionary<uint, ToggleButton> valueChips = new();
		bool suppress;

		/// <summary>
		/// Every user-facing chip in document order — the Any-chip first, then each
		/// value chip. Used by the popup's arrow-key navigation to skip past internal
		/// template parts (ComboBox toggle buttons etc.) that descendant-walking would
		/// otherwise pick up.
		/// </summary>
		public IEnumerable<Control> NavigableElements {
			get {
				yield return anyChip;
				foreach (var (_, chip) in valueChips)
					yield return chip;
			}
		}

		public MutexChipGroup(FilterState state, MutexGroup group)
		{
			this.state = state ?? throw new ArgumentNullException(nameof(state));
			this.group = group ?? throw new ArgumentNullException(nameof(group));

			var label = new TextBlock {
				Text = group.Name,
				FontWeight = FontWeight.SemiBold,
				Margin = new Thickness(0, 0, 0, 2),
			};
			anyChip = MakeChip("<Any>");
			anyChip.IsCheckedChanged += OnAnyToggled;
			var wrap = new WrapPanel { Orientation = Orientation.Horizontal };
			wrap.Children.Add(anyChip);
			foreach (var v in group.Values)
			{
				// Include the zero-default entry as a regular chip — picking just it means
				// "axis bits = 0", e.g. NotPublic / AutoLayout / Class. The schema labels
				// the synthetic zero entry "(none)" so its purpose stays obvious.
				var chip = MakeChip(v.Label);
				chip.IsCheckedChanged += (_, _) => OnValueToggled(v.Value);
				// Plain click → "select only this" (single-select). Shift+Click falls through
				// to the ToggleButton's default toggle, which feeds OnValueToggled and lets
				// the user build / trim an additive multi-select set.
				var capturedValue = v.Value;
				chip.AddHandler(InputElement.PointerPressedEvent, (_, e) => {
					if ((e.KeyModifiers & KeyModifiers.Shift) != 0)
						return; // additive: keep ToggleButton's default toggle behaviour
					SelectOnly(capturedValue);
					e.Handled = true;
				}, RoutingStrategies.Tunnel);
				valueChips[v.Value] = chip;
				wrap.Children.Add(chip);
			}
			Content = new StackPanel { Children = { label, wrap } };
			SyncFromState();
		}

		static ToggleButton MakeChip(string label) => new() {
			Content = label,
			Padding = new Thickness(6, 1),
			Margin = new Thickness(2),
			MinHeight = 0,
		};

		void OnAnyToggled(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
		{
			if (suppress)
				return;
			if (anyChip.IsChecked == true)
				ResetToAny();
			else
				// Refuse to leave "any" via the Any chip alone — the only way out is to
				// untick a specific value chip. Re-tick.
				ResetToAny();
		}

		void OnValueToggled(uint value)
		{
			if (suppress)
				return;
			// Translate the visual chip state into a fresh selection set: include every
			// value whose chip is currently checked.
			var checkedValues = ImmutableHashSet.CreateBuilder<uint>();
			foreach (var (v, chip) in valueChips)
				if (chip.IsChecked == true)
					checkedValues.Add(v);
			var selection = checkedValues.ToImmutable();

			// Spec: "Selecting all values collapses back to 'any.' Selecting none re-activates 'any.'"
			if (selection.Count == 0 || selection.Count == valueChips.Count)
			{
				ResetToAny();
				return;
			}

			suppress = true;
			try
			{
				anyChip.IsChecked = false;
			}
			finally
			{ suppress = false; }
			state.SetMutexSelection(group.Name, selection);
		}

		void SelectOnly(uint value)
		{
			// Single-chip selection — collapse the group to {value}. We bypass the
			// "all checked == any" / "none checked == any" collapse rules in
			// OnValueToggled because Ctrl/Shift+click is an explicit narrow-down
			// gesture; the user picked exactly one chip and wants exactly one.
			suppress = true;
			try
			{
				anyChip.IsChecked = false;
				foreach (var (v, chip) in valueChips)
					chip.IsChecked = v == value;
			}
			finally
			{ suppress = false; }
			state.SetMutexSelection(group.Name, ImmutableHashSet.Create(value));
		}

		void ResetToAny()
		{
			suppress = true;
			try
			{
				anyChip.IsChecked = true;
				foreach (var (_, chip) in valueChips)
					chip.IsChecked = true;
			}
			finally
			{ suppress = false; }
			state.SetMutexSelection(group.Name, null);
		}

		/// <summary>
		/// Reflects the model into chip states. Useful when the popup is reopened or the
		/// state was mutated programmatically (clear button, session restore).
		/// </summary>
		public void SyncFromState()
		{
			suppress = true;
			try
			{
				if (!state.MutexSelections.TryGetValue(group.Name, out var sel) || sel is null)
				{
					anyChip.IsChecked = true;
					foreach (var (_, chip) in valueChips)
						chip.IsChecked = true;
				}
				else
				{
					anyChip.IsChecked = false;
					foreach (var (v, chip) in valueChips)
						chip.IsChecked = sel.Contains(v);
				}
			}
			finally
			{ suppress = false; }
		}
	}
}
