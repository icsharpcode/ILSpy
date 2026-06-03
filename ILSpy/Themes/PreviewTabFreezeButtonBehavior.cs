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
	/// Injects an inline "freeze" Button into the tab header of every <see cref="DocumentTabStripItem"/>.
	/// Dock's tab template is compiled into the theme DLL and not directly overridable without
	/// reproducing the whole template, so the button is added at runtime by walking the visual
	/// tree to the inner title StackPanel and appending a child there. The button's
	/// <see cref="Visual.IsVisible"/> is bound to <see cref="ContentTabPage.IsPreview"/> on the
	/// tab's underlying viewmodel — so it shows only on the One preview tab — and the click
	/// handler routes to <see cref="DockWorkspace.FreezeCurrentTab"/>. It reuses the sibling
	/// close button's ControlTheme so it renders at the same size and inherits the same
	/// <c>:pointerover</c> states.
	/// </summary>
	public static class PreviewTabFreezeButtonBehavior
	{
		const string FreezeButtonTag = "PreviewTabFreezeButton";

		public static readonly AttachedProperty<bool> EnableProperty =
			AvaloniaProperty.RegisterAttached<DocumentTabStripItem, bool>(
				"Enable",
				typeof(PreviewTabFreezeButtonBehavior));

		public static void SetEnable(DocumentTabStripItem element, bool value)
			=> element.SetValue(EnableProperty, value);

		public static bool GetEnable(DocumentTabStripItem element)
			=> element.GetValue(EnableProperty);

		static PreviewTabFreezeButtonBehavior()
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
			if (item.GetVisualDescendants().OfType<Button>().Any(b => (b.Tag as string) == FreezeButtonTag))
				return;
			if (item.DataContext is not ContentTabPage)
				return;
			// Anchor: the Grid that's the parent of the title StackPanel. The Grid also hosts
			// the close-button ContentPresenter as a sibling column. We inject the freeze button
			// as a new Auto-sized column between the two so the title can't push it past the tab's
			// right edge — appending to the StackPanel puts it inside the title column, where the
			// unconstrained title TextBlock claims all the space and the button overflows behind
			// the close button.
			var titleStack = item.GetVisualDescendants().OfType<StackPanel>().FirstOrDefault();
			if (titleStack is null)
				return;
			var grid = titleStack.Parent as Grid;
			if (grid is null || grid.ColumnDefinitions.Count == 0)
				return;

			// Clip the StackPanel's content so the title TextBlock (which measures at its
			// full unconstrained text width — 900px+ for long type signatures) doesn't
			// render OVER the freeze and close buttons that sit in adjacent Grid columns.
			// Without this, the freeze button appears "cut off" because the title's pixels
			// paint on top of it.
			titleStack.ClipToBounds = true;

			// Snowflake glyph for "freeze". A STROKED vector Path (not a font glyph): the
			// previous Segoe Fluent fallback was Windows-only and rendered as tofu on Linux /
			// macOS. Six spokes + tip barbs in a 16x16 box (inset so the stroke isn't clipped
			// at the tips). Stroke (not Fill) is bound to the tab's Foreground so the glyph
			// inherits the per-tab theme colour.
			var glyph = new Path {
				Data = StreamGeometry.Parse(
					"M8,1.5 L8,14.5 M2.37,4.75 L13.63,11.25 M2.37,11.25 L13.63,4.75 " +
					"M8,4.2 L6.6,2.9 M8,4.2 L9.4,2.9 M8,11.8 L6.6,13.1 M8,11.8 L9.4,13.1 " +
					"M5.0,5.9 L3.4,5.5 M5.0,5.9 L4.6,7.5 M11.0,10.1 L12.6,10.5 M11.0,10.1 L11.4,8.5 " +
					"M5.0,10.1 L3.4,10.5 M5.0,10.1 L4.6,8.5 M11.0,5.9 L12.6,5.5 M11.0,5.9 L11.4,7.5"),
				Stretch = Stretch.Uniform,
				StrokeThickness = 1.3,
				StrokeLineCap = PenLineCap.Round,
				StrokeJoin = PenLineJoin.Round,
				Width = 12,
				Height = 12,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
			};
			glyph.Bind(Shape.StrokeProperty, new Binding(nameof(TemplatedControl.Foreground)) {
				Source = item,
				Mode = BindingMode.OneWay,
			});
			var freezeButton = new Button {
				Tag = FreezeButtonTag,
				Content = glyph,
				// Left margin preserves the gap between the freeze and close buttons (~4px)
				// regardless of what the inherited ControlTheme decides for its own Margin.
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
			// Copy the close button's ControlTheme so the freeze button renders at the same size
			// and uses the same :pointerover background. The close button (a plain
			// Avalonia.Controls.Button with no classes, theme applied by Dock's tab template)
			// sits as a sibling under the same Grid. Without this, it defaulted to a class-styled
			// custom look that was bigger and used a different hover tint than its neighbour.
			var closeButton = item.GetVisualDescendants().OfType<Button>()
				.FirstOrDefault(b => (b.Tag as string) != FreezeButtonTag);
			if (closeButton?.Theme is { } closeTheme)
				freezeButton.Theme = closeTheme;
			ToolTip.SetTip(freezeButton, "Freeze tab");

			// IsVisible follows IsPreview — the button hides once the tab is frozen.
			freezeButton.Bind(Visual.IsVisibleProperty, new Binding(nameof(ContentTabPage.IsPreview)) {
				Source = item.DataContext,
				Mode = BindingMode.OneWay,
			});

			freezeButton.Click += (_, _) => TryGetDockWorkspace()?.FreezeCurrentTab();

			// Insert a new Auto-sized column right before the last (close) column, bump any
			// existing children at or past that index by +1, and dock the freeze button there.
			// This gives it a dedicated cell the title can't encroach on.
			int newColIndex = grid.ColumnDefinitions.Count - 1;
			grid.ColumnDefinitions.Insert(newColIndex, new ColumnDefinition(GridLength.Auto));
			foreach (var child in grid.Children)
			{
				var col = Grid.GetColumn(child);
				if (col >= newColIndex)
					Grid.SetColumn(child, col + 1);
			}
			Grid.SetColumn(freezeButton, newColIndex);
			grid.Children.Add(freezeButton);
		}

		static DockWorkspace? TryGetDockWorkspace()
		{
			try
			{ return AppComposition.Current.GetExport<DockWorkspace>(); }
			catch { return null; }
		}
	}
}
