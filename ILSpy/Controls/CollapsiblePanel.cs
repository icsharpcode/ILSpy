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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace ICSharpCode.ILSpy.Controls
{
	/// <summary>
	/// Allows animated collapsing of the content of this panel.
	/// </summary>
	public class CollapsiblePanel : ContentControl
	{
		static CollapsiblePanel()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(CollapsiblePanel),
													 new FrameworkPropertyMetadata(typeof(CollapsiblePanel)));
			FocusableProperty.OverrideMetadata(typeof(CollapsiblePanel),
											   new FrameworkPropertyMetadata(false));
		}

		public static readonly DependencyProperty IsCollapsedProperty = DependencyProperty.Register(
			"IsCollapsed", typeof(bool), typeof(CollapsiblePanel),
			new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsCollapsedChanged)));

		public bool IsCollapsed {
			get { return (bool)GetValue(IsCollapsedProperty); }
			set { SetValue(IsCollapsedProperty, value); }
		}

		public static readonly DependencyProperty CollapseOrientationProperty =
			DependencyProperty.Register("CollapseOrientation", typeof(Orientation), typeof(CollapsiblePanel),
										new FrameworkPropertyMetadata(Orientation.Vertical));

		public Orientation CollapseOrientation {
			get { return (Orientation)GetValue(CollapseOrientationProperty); }
			set { SetValue(CollapseOrientationProperty, value); }
		}

		public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(
			"Duration", typeof(TimeSpan), typeof(CollapsiblePanel),
			new UIPropertyMetadata(TimeSpan.FromMilliseconds(250)));

		/// <summary>
		/// The duration in milliseconds of the animation.
		/// </summary>
		public TimeSpan Duration {
			get { return (TimeSpan)GetValue(DurationProperty); }
			set { SetValue(DurationProperty, value); }
		}

		protected internal static readonly DependencyProperty AnimationProgressProperty = DependencyProperty.Register(
			"AnimationProgress", typeof(double), typeof(CollapsiblePanel),
			new FrameworkPropertyMetadata(1.0));

		/// <summary>
		/// Value between 0 and 1 specifying how far the animation currently is.
		/// </summary>
		protected internal double AnimationProgress {
			get { return (double)GetValue(AnimationProgressProperty); }
			set { SetValue(AnimationProgressProperty, value); }
		}

		protected internal static readonly DependencyProperty AnimationProgressXProperty = DependencyProperty.Register(
			"AnimationProgressX", typeof(double), typeof(CollapsiblePanel),
			new FrameworkPropertyMetadata(1.0));

		/// <summary>
		/// Value between 0 and 1 specifying how far the animation currently is.
		/// </summary>
		protected internal double AnimationProgressX {
			get { return (double)GetValue(AnimationProgressXProperty); }
			set { SetValue(AnimationProgressXProperty, value); }
		}

		protected internal static readonly DependencyProperty AnimationProgressYProperty = DependencyProperty.Register(
			"AnimationProgressY", typeof(double), typeof(CollapsiblePanel),
			new FrameworkPropertyMetadata(1.0));

		/// <summary>
		/// Value between 0 and 1 specifying how far the animation currently is.
		/// </summary>
		protected internal double AnimationProgressY {
			get { return (double)GetValue(AnimationProgressYProperty); }
			set { SetValue(AnimationProgressYProperty, value); }
		}

		static void OnIsCollapsedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((CollapsiblePanel)d).SetupAnimation((bool)e.NewValue);
		}

		void SetupAnimation(bool isCollapsed)
		{
			if (this.IsLoaded)
			{
				// If the animation is already running, calculate remaining portion of the time
				double currentProgress = AnimationProgress;
				if (!isCollapsed)
				{
					currentProgress = 1.0 - currentProgress;
				}

				DoubleAnimation animation = new DoubleAnimation();
				animation.To = isCollapsed ? 0.0 : 1.0;
				animation.Duration = TimeSpan.FromSeconds(Duration.TotalSeconds * currentProgress);
				animation.FillBehavior = FillBehavior.HoldEnd;

				this.BeginAnimation(AnimationProgressProperty, animation);
				if (CollapseOrientation == Orientation.Horizontal)
				{
					this.BeginAnimation(AnimationProgressXProperty, animation);
					this.AnimationProgressY = 1.0;
				}
				else
				{
					this.AnimationProgressX = 1.0;
					this.BeginAnimation(AnimationProgressYProperty, animation);
				}
			}
			else
			{
				this.AnimationProgress = isCollapsed ? 0.0 : 1.0;
				this.AnimationProgressX = (CollapseOrientation == Orientation.Horizontal) ? this.AnimationProgress : 1.0;
				this.AnimationProgressY = (CollapseOrientation == Orientation.Vertical) ? this.AnimationProgress : 1.0;
			}
		}
	}

	sealed class CollapsiblePanelProgressToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is double)
				return (double)value > 0 ? Visibility.Visible : Visibility.Collapsed;
			else
				return Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class SelfCollapsingPanel : CollapsiblePanel
	{
		public static readonly DependencyProperty CanCollapseProperty =
			DependencyProperty.Register("CanCollapse", typeof(bool), typeof(SelfCollapsingPanel),
										new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnCanCollapseChanged)));

		public bool CanCollapse {
			get { return (bool)GetValue(CanCollapseProperty); }
			set { SetValue(CanCollapseProperty, value); }
		}

		static void OnCanCollapseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			SelfCollapsingPanel panel = (SelfCollapsingPanel)d;
			if ((bool)e.NewValue)
			{
				if (!panel.HeldOpenByMouse)
					panel.IsCollapsed = true;
			}
			else
			{
				panel.IsCollapsed = false;
			}
		}

		bool HeldOpenByMouse {
			get { return IsMouseOver || IsMouseCaptureWithin; }
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			if (CanCollapse && !HeldOpenByMouse)
				IsCollapsed = true;
		}

		protected override void OnLostMouseCapture(MouseEventArgs e)
		{
			base.OnLostMouseCapture(e);
			if (CanCollapse && !HeldOpenByMouse)
				IsCollapsed = true;
		}
	}
}
