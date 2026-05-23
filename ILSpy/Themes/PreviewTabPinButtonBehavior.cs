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
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;

using Dock.Avalonia.Controls;

using ILSpy.AppEnv;
using ILSpy.Docking;
using ILSpy.ViewModels;

namespace ILSpy.Themes
{
	/// <summary>
	/// Injects an inline "pin" Button into the tab header of every <see cref="DocumentTabStripItem"/>.
	/// Dock's tab template is compiled into the theme DLL and not directly overridable without
	/// reproducing the whole template, so the button is added at runtime by walking the visual
	/// tree to the inner title StackPanel and appending a child there. The button's
	/// <see cref="Visual.IsVisible"/> is bound to <see cref="ContentTabPage.IsPreview"/> on the
	/// tab's underlying viewmodel and the click handler routes to
	/// <see cref="DockWorkspace.PinCurrentTab"/>. Styling lives in App.axaml under the
	/// <c>Button.preview-pin</c> class selector so the theme's <c>:pointerover</c> states still
	/// apply (local Background/BorderBrush values here would block them).
	/// </summary>
	public static class PreviewTabPinButtonBehavior
	{
		const string PinButtonTag = "PreviewTabPinButton";

		public static readonly AttachedProperty<bool> EnableProperty =
			AvaloniaProperty.RegisterAttached<DocumentTabStripItem, bool>(
				"Enable",
				typeof(PreviewTabPinButtonBehavior));

		public static void SetEnable(DocumentTabStripItem element, bool value)
			=> element.SetValue(EnableProperty, value);

		public static bool GetEnable(DocumentTabStripItem element)
			=> element.GetValue(EnableProperty);

		static PreviewTabPinButtonBehavior()
		{
			EnableProperty.Changed.AddClassHandler<DocumentTabStripItem>(OnEnableChanged);
		}

		static void OnEnableChanged(DocumentTabStripItem item, AvaloniaPropertyChangedEventArgs e)
		{
			if (e.NewValue is not true)
				return;
			item.TemplateApplied += (_, _) => TryInject(item);
			item.AttachedToVisualTree += (_, _) => TryInject(item);
			TryInject(item);
		}

		static void TryInject(DocumentTabStripItem item)
		{
			// Idempotent: bail if our button is already in the tree.
			if (item.GetVisualDescendants().OfType<Button>().Any(b => (b.Tag as string) == PinButtonTag))
				return;
			if (item.DataContext is not ContentTabPage)
				return;
			// Anchor: the Grid that's the parent of the title StackPanel. The Grid also hosts
			// the close-button ContentPresenter as a sibling column. We inject pin as a new
			// Auto-sized column between the two so the title can't push pin past the tab's
			// right edge — appending to the StackPanel puts pin inside the title column,
			// where the unconstrained title TextBlock claims all the space and the pin
			// overflows behind the close button.
			var titleStack = item.GetVisualDescendants().OfType<StackPanel>().FirstOrDefault();
			if (titleStack is null)
				return;
			var grid = titleStack.Parent as Grid;
			if (grid is null || grid.ColumnDefinitions.Count == 0)
				return;

			// Clip the StackPanel's content so the title TextBlock (which measures at its
			// full unconstrained text width — 900px+ for long type signatures) doesn't
			// render OVER the pin and close buttons that sit in adjacent Grid columns.
			// Without this, the pin appears "cut off" because the title's pixels paint
			// on top of it.
			titleStack.ClipToBounds = true;

			// Tilted-pushpin silhouette matching the Visual Studio preview-tab affordance.
			// Vector path (not a font glyph): the previous Segoe Fluent Icons U+E718
			// fallback was Windows-only and rendered as a tofu box on Linux / macOS.
			// Path with bound Fill is what restores per-tab Foreground inheritance that
			// the intermediate SVG-image variant lost - Avalonia.Svg.Skia honors the
			// asset's literal fill, not the consuming control's theme color. RotateTransform
			// at -45 deg orients the upright pin path so its tip points to the lower-left.
			var glyph = new Path {
				Data = StreamGeometry.Parse("M5 2h6v1h-1v4l2 2v1H9v3l-1 2-1-2v-3H4V9l2-2V3H5z"),
				Stretch = Stretch.Uniform,
				Width = 12,
				Height = 12,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				RenderTransform = new RotateTransform(45),
				RenderTransformOrigin = RelativePoint.Center,
			};
			glyph.Bind(Shape.FillProperty, new Binding(nameof(TemplatedControl.Foreground)) {
				Source = item,
				Mode = BindingMode.OneWay,
			});
			var pin = new Button {
				Tag = PinButtonTag,
				Content = glyph,
				// Left margin preserves the gap between pin and close (~4px) regardless of
				// what the inherited ControlTheme decides for its own Margin.
				Margin = new Thickness(4, 0, 0, 0),
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center,
				Focusable = false,
				IsTabStop = false,
				// Belt-and-suspenders with the glyph's own ClipToBounds=false above; the
				// Avalonia Button defaults ClipToBounds=true which clipped the glyph's
				// right edge at the content-area boundary.
				ClipToBounds = false,
			};
			// Copy the close button's ControlTheme so pin renders at the same size and uses
			// the same :pointerover background. The close button (a plain Avalonia.Controls.Button
			// with no classes, theme applied by Dock's tab template) sits as a sibling under
			// the same Grid. Without this, pin defaulted to a class-styled custom look that
			// was bigger and used a different hover tint than its neighbour.
			var closeButton = item.GetVisualDescendants().OfType<Button>()
				.FirstOrDefault(b => (b.Tag as string) != PinButtonTag);
			if (closeButton?.Theme is { } closeTheme)
				pin.Theme = closeTheme;
			ToolTip.SetTip(pin, "Pin tab");

			// IsVisible follows IsPreview — the button hides once the tab is pinned.
			pin.Bind(Visual.IsVisibleProperty, new Binding(nameof(ContentTabPage.IsPreview)) {
				Source = item.DataContext,
				Mode = BindingMode.OneWay,
			});

			pin.Click += (_, _) => TryGetDockWorkspace()?.PinCurrentTab();

			// Insert a new Auto-sized column right before the last (close) column, bump any
			// existing children at or past that index by +1, and dock pin there. This gives
			// pin a dedicated cell the title can't encroach on.
			int newColIndex = grid.ColumnDefinitions.Count - 1;
			grid.ColumnDefinitions.Insert(newColIndex, new ColumnDefinition(GridLength.Auto));
			foreach (var child in grid.Children)
			{
				var col = Grid.GetColumn(child);
				if (col >= newColIndex)
					Grid.SetColumn(child, col + 1);
			}
			Grid.SetColumn(pin, newColIndex);
			grid.Children.Add(pin);
		}

		static DockWorkspace? TryGetDockWorkspace()
		{
			try
			{ return AppComposition.Current.GetExport<DockWorkspace>(); }
			catch { return null; }
		}
	}
}
