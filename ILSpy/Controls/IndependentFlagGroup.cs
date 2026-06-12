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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

using ICSharpCode.ILSpy.Metadata.Filters;

namespace ICSharpCode.ILSpy.Views.Filters
{
	/// <summary>
	/// Tri-state pills (don't care / required ✓ / excluded ✗) for the independent flags
	/// in a schema, plus a header toggle that flips the required-mode between "All
	/// required" and "Any required". The pill cycle is one click each:
	/// DontCare → Required → Excluded → DontCare.
	/// </summary>
	public sealed class IndependentFlagGroup : UserControl
	{
		readonly FilterState state;
		readonly Dictionary<string, Button> pills = new();
		readonly ComboBox modeBox;

		/// <summary>
		/// Every user-facing pill in document order. Used by the popup's arrow-key
		/// navigation to skip past internal template parts of the All/Any ComboBox.
		/// </summary>
		public IEnumerable<Control> NavigableElements => pills.Values;

		public IndependentFlagGroup(FilterState state, IReadOnlyList<IndependentFlag> flags)
		{
			this.state = state ?? throw new ArgumentNullException(nameof(state));

			var header = new DockPanel { LastChildFill = false, Margin = new Thickness(0, 4, 0, 2) };
			global::Avalonia.Controls.DockPanel.SetDock(header, global::Avalonia.Controls.Dock.Top);
			var headerLabel = new TextBlock {
				Text = "Required:",
				FontWeight = FontWeight.SemiBold,
				VerticalAlignment = VerticalAlignment.Center,
				Margin = new Thickness(0, 0, 6, 0),
			};
			modeBox = new ComboBox {
				ItemsSource = new[] { "All", "Any" },
				SelectedIndex = state.IndependentMode == MatchMode.All ? 0 : 1,
				MinHeight = 0,
				Padding = new Thickness(4, 1),
			};
			modeBox.SelectionChanged += (_, _) =>
				state.IndependentMode = modeBox.SelectedIndex == 0 ? MatchMode.All : MatchMode.Any;
			global::Avalonia.Controls.DockPanel.SetDock(headerLabel, global::Avalonia.Controls.Dock.Left);
			global::Avalonia.Controls.DockPanel.SetDock(modeBox, global::Avalonia.Controls.Dock.Left);
			header.Children.Add(headerLabel);
			header.Children.Add(modeBox);

			var rows = new StackPanel { Orientation = Orientation.Vertical };
			foreach (var f in flags)
			{
				var pill = new Button {
					Content = TriStateGlyph(state.FlagStates.GetValueOrDefault(f.Name, TriState.DontCare)),
					MinWidth = 24,
					Padding = new Thickness(4, 1),
					Margin = new Thickness(0, 0, 6, 0),
				};
				pill.Click += (_, _) => CyclePill(f.Name, pill);
				pills[f.Name] = pill;
				var row = new DockPanel { LastChildFill = true, Margin = new Thickness(0, 1) };
				global::Avalonia.Controls.DockPanel.SetDock(pill, global::Avalonia.Controls.Dock.Left);
				row.Children.Add(pill);
				row.Children.Add(new TextBlock {
					Text = f.Name,
					VerticalAlignment = VerticalAlignment.Center,
				});
				rows.Children.Add(row);
			}
			Content = new StackPanel { Children = { header, rows } };
		}

		void CyclePill(string flagName, Button pill)
		{
			var current = state.FlagStates.GetValueOrDefault(flagName, TriState.DontCare);
			var next = current switch {
				TriState.DontCare => TriState.Required,
				TriState.Required => TriState.Excluded,
				_ => TriState.DontCare,
			};
			state.SetFlagState(flagName, next);
			pill.Content = TriStateGlyph(next);
		}

		static string TriStateGlyph(TriState state) => state switch {
			TriState.Required => "✓",
			TriState.Excluded => "✗",
			_ => " ",
		};

		/// <summary>Reflects the model into pill glyphs.</summary>
		public void SyncFromState()
		{
			foreach (var (name, pill) in pills)
				pill.Content = TriStateGlyph(state.FlagStates.GetValueOrDefault(name, TriState.DontCare));
			modeBox.SelectedIndex = state.IndependentMode == MatchMode.All ? 0 : 1;
		}
	}
}
