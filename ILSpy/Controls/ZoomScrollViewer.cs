// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ICSharpCode.ILSpy.Controls
{
	public class ZoomScrollViewer : ScrollViewer
	{
		static ZoomScrollViewer()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomScrollViewer),
													 new FrameworkPropertyMetadata(typeof(ZoomScrollViewer)));
		}

		public static readonly DependencyProperty CurrentZoomProperty =
			DependencyProperty.Register("CurrentZoom", typeof(double), typeof(ZoomScrollViewer),
										new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, CalculateZoomButtonCollapsed, CoerceZoom));

		public double CurrentZoom {
			get { return (double)GetValue(CurrentZoomProperty); }
			set { SetValue(CurrentZoomProperty, value); }
		}

		static object CoerceZoom(DependencyObject d, object baseValue)
		{
			var zoom = (double)baseValue;
			ZoomScrollViewer sv = (ZoomScrollViewer)d;
			return Math.Max(sv.MinimumZoom, Math.Min(sv.MaximumZoom, zoom));
		}

		public static readonly DependencyProperty MinimumZoomProperty =
			DependencyProperty.Register("MinimumZoom", typeof(double), typeof(ZoomScrollViewer),
										new FrameworkPropertyMetadata(0.2));

		public double MinimumZoom {
			get { return (double)GetValue(MinimumZoomProperty); }
			set { SetValue(MinimumZoomProperty, value); }
		}

		public static readonly DependencyProperty MaximumZoomProperty =
			DependencyProperty.Register("MaximumZoom", typeof(double), typeof(ZoomScrollViewer),
										new FrameworkPropertyMetadata(5.0));

		public double MaximumZoom {
			get { return (double)GetValue(MaximumZoomProperty); }
			set { SetValue(MaximumZoomProperty, value); }
		}

		public static readonly DependencyProperty MouseWheelZoomProperty =
			DependencyProperty.Register("MouseWheelZoom", typeof(bool), typeof(ZoomScrollViewer),
										new FrameworkPropertyMetadata(true));

		public bool MouseWheelZoom {
			get { return (bool)GetValue(MouseWheelZoomProperty); }
			set { SetValue(MouseWheelZoomProperty, value); }
		}

		public static readonly DependencyProperty AlwaysShowZoomButtonsProperty =
			DependencyProperty.Register("AlwaysShowZoomButtons", typeof(bool), typeof(ZoomScrollViewer),
										new FrameworkPropertyMetadata(false, CalculateZoomButtonCollapsed));

		public bool AlwaysShowZoomButtons {
			get { return (bool)GetValue(AlwaysShowZoomButtonsProperty); }
			set { SetValue(AlwaysShowZoomButtonsProperty, value); }
		}

		static readonly DependencyPropertyKey ComputedZoomButtonCollapsedPropertyKey =
			DependencyProperty.RegisterReadOnly("ComputedZoomButtonCollapsed", typeof(bool), typeof(ZoomScrollViewer),
												new FrameworkPropertyMetadata(true));

		public static readonly DependencyProperty ComputedZoomButtonCollapsedProperty = ComputedZoomButtonCollapsedPropertyKey.DependencyProperty;

		public bool ComputedZoomButtonCollapsed {
			get { return (bool)GetValue(ComputedZoomButtonCollapsedProperty); }
			private set { SetValue(ComputedZoomButtonCollapsedPropertyKey, value); }
		}

		static void CalculateZoomButtonCollapsed(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ZoomScrollViewer z = d as ZoomScrollViewer;
			if (z != null)
				z.ComputedZoomButtonCollapsed = (z.AlwaysShowZoomButtons == false) && (z.CurrentZoom == 1.0);
		}

		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			if (!e.Handled && Keyboard.Modifiers == ModifierKeys.Control && MouseWheelZoom)
			{
				double oldZoom = CurrentZoom;
				double newZoom = RoundToOneIfClose(CurrentZoom * Math.Pow(1.001, e.Delta));
				newZoom = Math.Max(this.MinimumZoom, Math.Min(this.MaximumZoom, newZoom));

				// adjust scroll position so that mouse stays over the same virtual coordinate
				ContentPresenter presenter = Template.FindName("PART_Presenter", this) as ContentPresenter;
				Vector relMousePos;
				if (presenter != null)
				{
					Point mousePos = e.GetPosition(presenter);
					relMousePos = new Vector(mousePos.X / presenter.ActualWidth, mousePos.Y / presenter.ActualHeight);
				}
				else
				{
					relMousePos = new Vector(0.5, 0.5);
				}

				Point scrollOffset = new Point(this.HorizontalOffset, this.VerticalOffset);
				Vector oldHalfViewport = new Vector(this.ViewportWidth / 2, this.ViewportHeight / 2);
				Vector newHalfViewport = oldHalfViewport / newZoom * oldZoom;
				Point oldCenter = scrollOffset + oldHalfViewport;
				Point virtualMousePos = scrollOffset + new Vector(relMousePos.X * this.ViewportWidth, relMousePos.Y * this.ViewportHeight);

				// As newCenter, we want to choose a point between oldCenter and virtualMousePos. The more we zoom in, the closer
				// to virtualMousePos. We'll create the line x = oldCenter + lambda * (virtualMousePos-oldCenter).
				// On this line, we need to choose lambda between -1 and 1:
				// -1 = zoomed out completely
				//  0 = zoom unchanged
				// +1 = zoomed in completely
				// But the zoom factor (newZoom/oldZoom) we have is in the range [0,+Infinity].

				// Basically, I just played around until I found a function that maps this to [-1,1] and works well.
				// "f" is squared because otherwise the mouse simply stays over virtualMousePos, but I wanted virtualMousePos
				// to move towards the middle -> squaring f causes lambda to be closer to 1, giving virtualMousePos more weight
				// then oldCenter.

				double f = Math.Min(newZoom, oldZoom) / Math.Max(newZoom, oldZoom);
				double lambda = 1 - f * f;
				if (oldZoom > newZoom)
					lambda = -lambda;

				Debug.Print("old: " + oldZoom + ", new: " + newZoom);

				Point newCenter = oldCenter + lambda * (virtualMousePos - oldCenter);
				scrollOffset = newCenter - newHalfViewport;

				SetCurrentValue(CurrentZoomProperty, newZoom);

				this.ScrollToHorizontalOffset(scrollOffset.X);
				this.ScrollToVerticalOffset(scrollOffset.Y);

				e.Handled = true;
			}
			base.OnMouseWheel(e);
		}

		internal static double RoundToOneIfClose(double val)
		{
			if (Math.Abs(val - 1.0) < 0.001)
				return 1.0;
			else
				return val;
		}
	}

	sealed class IsNormalZoomConverter : IValueConverter
	{
		public static readonly IsNormalZoomConverter Instance = new IsNormalZoomConverter();

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (parameter is bool && (bool)parameter)
				return true;
			return ((double)value) == 1.0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
