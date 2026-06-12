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

using System.ComponentModel;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.VisualTree;

using Dock.Avalonia.Controls;

using ICSharpCode.ILSpy.AppEnv;

namespace ICSharpCode.ILSpy.Themes
{
	/// <summary>
	/// Switches the document tab strip between single-line (a single horizontally scrolling row, the
	/// default) and multi-line (tabs wrapped onto several rows) layout. The mode is read from and
	/// written to <see cref="SessionSettings.MultiLineDocumentTabs"/>, so it persists across sessions,
	/// and the mouse wheel over the strip flips it: wheel up expands to multiple rows, wheel down
	/// collapses back to a single row. Dock's <see cref="DocumentTabStrip"/> hardcodes a horizontal
	/// StackPanel inside a PART_ScrollViewer in its theme template and ignores an ItemsPanel set via a
	/// Style, so this behaviour swaps the ItemsPanel directly and toggles the scroll-viewer's scroll
	/// directions to match. Toggle via the attached Enable property in App.axaml.
	/// </summary>
	public static class MultiRowTabStripBehavior
	{
		public static readonly AttachedProperty<bool> EnableProperty =
			AvaloniaProperty.RegisterAttached<DocumentTabStrip, bool>(
				"Enable",
				typeof(MultiRowTabStripBehavior));

		public static void SetEnable(DocumentTabStrip element, bool value)
			=> element.SetValue(EnableProperty, value);

		public static bool GetEnable(DocumentTabStrip element)
			=> element.GetValue(EnableProperty);

		static MultiRowTabStripBehavior()
		{
			EnableProperty.Changed.AddClassHandler<DocumentTabStrip>(OnEnableChanged);
		}

		static void OnEnableChanged(DocumentTabStrip strip, AvaloniaPropertyChangedEventArgs e)
		{
			if (e.NewValue is not true)
				return;

			// The wheel toggles the persisted mode. Handle it so it doesn't also scroll the tabs.
			strip.AddHandler(InputElement.PointerWheelChangedEvent, OnPointerWheel,
				RoutingStrategies.Tunnel);

			var settings = AppComposition.TryGetExport<SettingsService>()?.SessionSettings;
			PropertyChangedEventHandler? settingsHandler = null;
			if (settings is not null)
			{
				settingsHandler = (_, args) => {
					if (args.PropertyName == nameof(SessionSettings.MultiLineDocumentTabs))
						ApplyMode(strip, settings.MultiLineDocumentTabs);
				};
				settings.PropertyChanged += settingsHandler;
				strip.DetachedFromVisualTree += (_, _) => {
					if (settingsHandler is not null)
						settings.PropertyChanged -= settingsHandler;
				};
			}

			strip.TemplateApplied += (_, _) => ApplyMode(strip, CurrentMultiLine());
			ApplyMode(strip, CurrentMultiLine());
		}

		static void OnPointerWheel(object? sender, PointerWheelEventArgs e)
		{
			var settings = AppComposition.TryGetExport<SettingsService>()?.SessionSettings;
			if (settings is null || e.Delta.Y == 0 || !settings.MouseWheelTogglesTabStripRows)
				return;
			// Idempotent set: rolling further in the same direction keeps the same mode rather than
			// flip-flopping, so a multi-detent gesture settles cleanly.
			settings.MultiLineDocumentTabs = e.Delta.Y > 0;
			e.Handled = true;
		}

		static void ApplyMode(DocumentTabStrip strip, bool multiLine)
		{
			// The presenter builds its panel from ItemsPanel; swapping it re-flows the tabs.
			strip.ItemsPanel = multiLine
				? new FuncTemplate<Panel?>(() => new WrapPanel { Orientation = Orientation.Horizontal })
				: new FuncTemplate<Panel?>(() => new StackPanel { Orientation = Orientation.Horizontal });

			var scroller = strip.GetVisualDescendants().OfType<ScrollViewer>()
				.FirstOrDefault(s => s.Name == "PART_ScrollViewer");
			if (scroller is null)
				return;
			if (multiLine)
			{
				// Disabling horizontal scroll constrains the WrapPanel to the strip width so it wraps.
				scroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
				scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
			}
			else
			{
				scroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
				scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
			}
		}

		static bool CurrentMultiLine()
			=> AppComposition.TryGetExport<SettingsService>()?.SessionSettings?.MultiLineDocumentTabs ?? false;
	}
}
