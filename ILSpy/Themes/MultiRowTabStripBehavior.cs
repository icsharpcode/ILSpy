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
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.VisualTree;

using Dock.Avalonia.Controls;

namespace ILSpy.Themes
{
	/// <summary>
	/// PROTOTYPE: makes the document tab strip lay its tabs out on multiple rows instead of a single
	/// scrolling row. Dock's <see cref="DocumentTabStrip"/> hardcodes a horizontal StackPanel inside a
	/// PART_ScrollViewer in its theme template and ignores an ItemsPanel set via a Style, so this
	/// behaviour sets the WrapPanel ItemsPanel directly and disables the scroll-viewer's horizontal
	/// scrolling (so the WrapPanel gets a bounded width and actually wraps). Toggle via the attached
	/// Enable property in App.axaml.
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

			// The presenter builds its panel from ItemsPanel; setting it here (before/at styling time)
			// makes the tabs flow through a WrapPanel.
			strip.ItemsPanel = new FuncTemplate<Panel?>(
				() => new WrapPanel { Orientation = Orientation.Horizontal });

			// The PART_ScrollViewer only exists once the template is applied. Disabling its horizontal
			// scroll constrains the WrapPanel to the strip width so it wraps onto new rows.
			strip.TemplateApplied += (_, _) => ConstrainScroller(strip);
			ConstrainScroller(strip);
		}

		static void ConstrainScroller(DocumentTabStrip strip)
		{
			var scroller = strip.GetVisualDescendants().OfType<ScrollViewer>()
				.FirstOrDefault(s => s.Name == "PART_ScrollViewer");
			if (scroller is null)
				return;
			scroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
			scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
		}
	}
}
