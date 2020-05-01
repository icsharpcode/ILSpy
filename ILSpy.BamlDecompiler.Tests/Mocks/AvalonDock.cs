// Copyright (c) 2020 Siegfried Pammer
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

using System.Windows;
using System.Windows.Controls.Primitives;

namespace AvalonDock
{
	public class DockingManager
	{
		public DockingManager()
		{
		}
	}
	
	public class Resizer : Thumb
	{
		static Resizer()
		{
			//This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
			//This style is defined in themes\generic.xaml
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Resizer), new FrameworkPropertyMetadata(typeof(Resizer)));
			MinWidthProperty.OverrideMetadata(typeof(Resizer), new FrameworkPropertyMetadata(6.0, FrameworkPropertyMetadataOptions.AffectsParentMeasure));
			MinHeightProperty.OverrideMetadata(typeof(Resizer), new FrameworkPropertyMetadata(6.0, FrameworkPropertyMetadataOptions.AffectsParentMeasure));
			HorizontalAlignmentProperty.OverrideMetadata(typeof(Resizer), new FrameworkPropertyMetadata(HorizontalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsParentMeasure));
			VerticalAlignmentProperty.OverrideMetadata(typeof(Resizer), new FrameworkPropertyMetadata(VerticalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsParentMeasure));
		}

	}
	
	public enum AvalonDockBrushes
	{
		DefaultBackgroundBrush,
		DockablePaneTitleBackground,
		DockablePaneTitleBackgroundSelected,
		DockablePaneTitleForeground,
		DockablePaneTitleForegroundSelected,
		PaneHeaderCommandBackground,
		PaneHeaderCommandBorderBrush,
		DocumentHeaderBackground,
		DocumentHeaderForeground,
		DocumentHeaderForegroundSelected,
		DocumentHeaderForegroundSelectedActivated,
		DocumentHeaderBackgroundSelected,
		DocumentHeaderBackgroundSelectedActivated,
		DocumentHeaderBackgroundMouseOver,
		DocumentHeaderBorderBrushMouseOver,
		DocumentHeaderBorder,
		DocumentHeaderBorderSelected,
		DocumentHeaderBorderSelectedActivated,
		NavigatorWindowTopBackground,
		NavigatorWindowTitleForeground,
		NavigatorWindowDocumentTypeForeground,
		NavigatorWindowInfoTipForeground,
		NavigatorWindowForeground,
		NavigatorWindowBackground,
		NavigatorWindowSelectionBackground,
		NavigatorWindowSelectionBorderbrush,
		NavigatorWindowBottomBackground
	}
	
	public enum ContextMenuElement
	{
		DockablePane,
		DocumentPane,
		DockableFloatingWindow,
		DocumentFloatingWindow
	}
}
