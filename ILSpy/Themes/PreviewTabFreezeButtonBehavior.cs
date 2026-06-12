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

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Themes
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

			// Cap the title and ellipsise overruns so a long type signature can't make a giant tab;
			// the full title is surfaced in the tab tooltip (App.axaml). Applied to the specific
			// realised title TextBlock here rather than via a `DocumentTabStripItem TextBlock` style
			// selector, which also matched -- and trimmed -- the tooltip's own TextBlock.
			var titleText = titleStack.GetVisualDescendants().OfType<TextBlock>().FirstOrDefault();
			if (titleText is not null)
			{
				titleText.MaxWidth = 240;
				titleText.TextTrimming = TextTrimming.CharacterEllipsis;
			}

			// Snowflake glyph for "freeze" (Font Awesome Free v7 "snowflake", CC-BY-4.0). A filled
			// vector Path (not a font glyph): a Segoe Fluent fallback was Windows-only and rendered
			// as tofu on Linux / macOS. Fill is bound to the tab Foreground so the glyph inherits
			// the per-tab theme colour; Stretch=Uniform scales the 640-unit viewBox down to 12px.
			var glyph = new Path {
				Data = StreamGeometry.Parse(
					"M344.1 56C344.1 42.7 333.4 32 320.1 32C306.8 32 296.1 42.7 296.1 56L296.1 134.1L273.1 111.1C263.7 101.7 248.5 101.7 239.2 111.1C229.9 120.5 229.8 135.7 239.2 145L296.2 202L296.2 278.5L230 240.3L209.1 162.5C205.7 149.7 192.5 142.1 179.7 145.5C166.9 148.9 159.2 162 162.7 174.8L171.1 206.3L103.5 167.3C92 160.6 77.3 164.5 70.7 176C64.1 187.5 68 202.2 79.5 208.8L147.1 247.8L115.6 256.2C102.8 259.6 95.2 272.8 98.6 285.6C102 298.4 115.2 306 128 302.6L205.8 281.7L272 319.9L205.8 358.1L128 337.2C115.2 333.8 102 341.4 98.6 354.2C95.2 367 102.8 380.2 115.6 383.6L147.1 392L79.5 431C68 437.8 64.1 452.5 70.7 464C77.3 475.5 92 479.4 103.5 472.8L171.1 433.8L162.7 465.3C159.3 478.1 166.9 491.3 179.7 494.7C192.5 498.1 205.7 490.5 209.1 477.7L230 399.9L296.2 361.7L296.2 438.2L239.2 495.2C229.8 504.6 229.8 519.8 239.2 529.1C248.6 538.4 263.8 538.5 273.1 529.1L296.1 506.1L296.1 584.2C296.1 597.5 306.8 608.2 320.1 608.2C333.4 608.2 344.1 597.5 344.1 584.2L344.1 506.1L367.1 529.1C376.5 538.5 391.7 538.5 401 529.1C410.3 519.7 410.4 504.5 401 495.2L344 438.2L344 361.7L410.2 399.9L431.1 477.7C434.5 490.5 447.7 498.1 460.5 494.7C473.3 491.3 480.9 478.1 477.5 465.3L469.1 433.8L536.7 472.8C548.2 479.4 562.9 475.5 569.5 464C576.1 452.5 572.2 437.8 560.7 431.2L493.1 392.2L524.6 383.8C537.4 380.4 545 367.2 541.6 354.4C538.2 341.6 525 334 512.2 337.4L434.4 358.3L368.2 320.1L434.4 281.9L512.2 302.8C525 306.2 538.2 298.6 541.6 285.8C545 273 537.4 259.8 524.6 256.4L493.1 248L560.7 209C572.2 202.4 576.1 187.7 569.5 176.2C562.9 164.7 548.2 160.8 536.7 167.4L469.1 206.4L477.5 174.9C480.9 162.1 473.3 148.9 460.5 145.5C447.7 142.1 434.5 149.7 431.1 162.5L410.2 240.3L344 278.5L344 202L401 145C410.4 135.6 410.4 120.4 401 111.1C391.6 101.8 376.4 101.7 367.1 111.1L344.1 134.1L344.1 56z"),
				Stretch = Stretch.Uniform,
				Width = 12,
				Height = 12,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
			};
			glyph.Bind(Shape.FillProperty, new Binding(nameof(TemplatedControl.Foreground)) {
				Source = item,
				Mode = BindingMode.OneWay,
			});
			var freezeButton = new Button {
				Tag = FreezeButtonTag,
				// Style class so App.axaml can give it a purple :pointerover (it would otherwise
				// inherit the blue hover from the close-button ControlTheme copied below).
				Classes = { "previewFreezeButton" },
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
			// The close button carries no tooltip from Dock's template; name its Ctrl+W shortcut.
			// TryInject runs for every document tab (the freeze button is injected on all of them,
			// just hidden when not the One), so this covers preview and frozen tabs alike.
			if (closeButton is not null)
				ToolTip.SetTip(closeButton, "Close (Ctrl+W)");

			// IsVisible follows IsPreview — the button hides once the tab is frozen.
			freezeButton.Bind(Visual.IsVisibleProperty, new Binding(nameof(ContentTabPage.IsPreview)) {
				Source = item.DataContext,
				Mode = BindingMode.OneWay,
			});

			freezeButton.Click += (_, _) => AppComposition.TryGetExport<DockWorkspace>()?.FreezeCurrentTab();

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
	}
}
