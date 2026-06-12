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

using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

using Dock.Avalonia.Controls;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Themes
{
	/// <summary>
	/// Injects a single overflow dropdown button at the trailing edge of each
	/// <see cref="DocumentTabStrip"/>, listing that strip's open documents for quick switching when
	/// many tabs don't fit. Only shown in single-line mode (when
	/// <see cref="SessionSettings.MultiLineDocumentTabs"/> is false); in multi-line mode every tab is
	/// already visible, so the dropdown hides. Dock's tab-strip template is compiled into the theme
	/// DLL, so -- like the sibling tab behaviours -- the button is injected at runtime, here into the
	/// strip's trailing DockPanel as a right-docked child. Picking an entry sets the strip's
	/// SelectedItem, which activates that document. Uses a <see cref="ContextMenu"/> (the menu type the
	/// app themes and renders reliably) opened on the next dispatcher turn after the click.
	/// </summary>
	public static class DocumentSwitcherDropdownBehavior
	{
		const string DropdownTag = "DocumentSwitcherDropdown";

		public static readonly AttachedProperty<bool> EnableProperty =
			AvaloniaProperty.RegisterAttached<DocumentTabStrip, bool>(
				"Enable",
				typeof(DocumentSwitcherDropdownBehavior));

		public static void SetEnable(DocumentTabStrip element, bool value)
			=> element.SetValue(EnableProperty, value);

		public static bool GetEnable(DocumentTabStrip element)
			=> element.GetValue(EnableProperty);

		static DocumentSwitcherDropdownBehavior()
		{
			EnableProperty.Changed.AddClassHandler<DocumentTabStrip>(OnEnableChanged);
		}

		static void OnEnableChanged(DocumentTabStrip strip, AvaloniaPropertyChangedEventArgs e)
		{
			if (e.NewValue is not true)
				return;

			var settings = AppComposition.TryGetExport<SettingsService>()?.SessionSettings;
			if (settings is not null)
			{
				System.ComponentModel.PropertyChangedEventHandler handler = (_, args) => {
					if (args.PropertyName == nameof(SessionSettings.MultiLineDocumentTabs))
						UpdateVisibility(strip, settings.MultiLineDocumentTabs);
				};
				settings.PropertyChanged += handler;
				strip.DetachedFromVisualTree += (_, _) => settings.PropertyChanged -= handler;
			}

			strip.TemplateApplied += (_, _) => Inject(strip);
			strip.AttachedToVisualTree += (_, _) => Inject(strip);
			Inject(strip);
		}

		static void Inject(DocumentTabStrip strip)
		{
			if (FindButton(strip) is not null)
				return;
			var scroller = strip.GetVisualDescendants().OfType<ScrollViewer>()
				.FirstOrDefault(s => s.Name == "PART_ScrollViewer");
			if (scroller?.GetVisualAncestors().OfType<DockPanel>().FirstOrDefault() is not { } dockPanel)
				return;

			// Down-chevron glyph as a filled Path so it renders the same on every platform (a font
			// glyph was tofu on Linux/macOS); Fill follows the strip Foreground for theme parity.
			var glyph = new Avalonia.Controls.Shapes.Path {
				Data = StreamGeometry.Parse("M0,0 L8,0 L4,5 Z"),
				Width = 8,
				Height = 5,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
			};
			glyph.Bind(Avalonia.Controls.Shapes.Shape.FillProperty,
				new Avalonia.Data.Binding(nameof(TemplatedControl.Foreground)) { Source = strip });

			var menu = new ContextMenu { Placement = PlacementMode.BottomEdgeAlignedRight };
			var button = new Button {
				Tag = DropdownTag,
				Content = glyph,
				// Flat: no border/box, transparent fill -- it reads as an affordance on the strip,
				// not a raised button.
				BorderThickness = new Thickness(0),
				Background = Brushes.Transparent,
				Padding = new Thickness(6, 2),
				Margin = new Thickness(2, 0),
				VerticalAlignment = VerticalAlignment.Center,
				Focusable = false,
				IsTabStop = false,
			};
			ToolTip.SetTip(button, "Switch to document");
			// Rebuild the list each time the menu opens so it always reflects the current documents:
			// in the click handler (the left-click path) and on Opening (the right-click path).
			menu.Opening += (_, _) => PopulateMenu(strip, menu);
			button.ContextMenu = menu;
			// Open on the next dispatcher turn -- not inline in the click, whose pointer-release would
			// otherwise be seen by the just-opened menu's light-dismiss layer as a click-outside and
			// close it immediately.
			button.Click += (_, _) => Dispatcher.UIThread.Post(() => {
				PopulateMenu(strip, menu);
				menu.Open(button);
			});

			// Dock to the right; inserting first reserves the trailing edge before the other docked
			// children are laid out, so the button lands at the far right regardless of their order.
			DockPanel.SetDock(button, Avalonia.Controls.Dock.Right);
			dockPanel.Children.Insert(0, button);

			UpdateVisibility(strip,
				AppComposition.TryGetExport<SettingsService>()?.SessionSettings?.MultiLineDocumentTabs ?? false);
		}

		static void PopulateMenu(DocumentTabStrip strip, ContextMenu menu)
		{
			menu.Items.Clear();
			foreach (var tab in strip.Items.OfType<ContentTabPage>())
			{
				var captured = tab;
				var item = new MenuItem {
					Header = string.IsNullOrEmpty(tab.Title) ? "(untitled)" : tab.Title,
					// Mark the active document so the list doubles as an at-a-glance overview.
					FontWeight = ReferenceEquals(strip.SelectedItem, tab) ? FontWeight.Bold : FontWeight.Normal,
				};
				item.Click += (_, _) => strip.SelectedItem = captured;
				menu.Items.Add(item);
			}
		}

		static void UpdateVisibility(DocumentTabStrip strip, bool multiLine)
		{
			if (FindButton(strip) is { } button)
				button.IsVisible = !multiLine;
		}

		static Button? FindButton(DocumentTabStrip strip)
			=> strip.GetVisualDescendants().OfType<Button>()
				.FirstOrDefault(b => (b.Tag as string) == DropdownTag);
	}
}
