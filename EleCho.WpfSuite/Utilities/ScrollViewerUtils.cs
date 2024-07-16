using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace EleCho.WpfSuite
{
    /// <summary>
    /// ScrollViewer Utilities
    /// </summary>
    public static class ScrollViewerUtils
    {
        /// <summary>
        /// Get value of VerticalOffset property
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static double GetVerticalOffset(DependencyObject d)
        {
            if (d is ScrollViewer sv)
            {
                return sv.VerticalOffset;
            }
            else if (d is ScrollContentPresenter scp)
            {
                return scp.VerticalOffset;
            }


            return (double)d.GetValue(VerticalOffsetProperty);
        }

        /// <summary>
        /// Set value of VerticalOffset property
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetVerticalOffset(DependencyObject obj, double value)
        {
            obj.SetValue(VerticalOffsetProperty, value);
        }

        /// <summary>
        /// Get value of HorizontalOffset property
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static double GetHorizontalOffset(DependencyObject d)
        {
            if (d is ScrollViewer sv)
            {
                return sv.HorizontalOffset;
            }
            else if (d is ScrollContentPresenter scp)
            {
                return scp.HorizontalOffset;
            }

            return (double)d.GetValue(HorizontalOffsetProperty);
        }

        /// <summary>
        /// Set value of HorizontalOffset property
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetHorizontalOffset(DependencyObject obj, double value)
        {
            obj.SetValue(HorizontalOffsetProperty, value);
        }


        /// <summary>
        /// The DependencyProperty of VerticalOffset property
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.RegisterAttached("VerticalOffset", typeof(double), typeof(ScrollViewerUtils), new PropertyMetadata(0.0, VerticalOffsetChangedCallback));

        /// <summary>
        /// The DependencyProperty of HorizontalOffset property
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.RegisterAttached("HorizontalOffset", typeof(double), typeof(ScrollViewerUtils), new PropertyMetadata(0.0, HorizontalOffsetChangedCallback));


        private static void VerticalOffsetChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is not double offset)
            {
                return;
            }

            if (d is ScrollViewer sv)
            {
                sv.ScrollToVerticalOffset(offset);
            }
            else if (d is ScrollContentPresenter scp)
            {
                scp.SetVerticalOffset(offset);
            }
        }

        private static void HorizontalOffsetChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is not double offset)
            {
                return;
            }

            if (d is ScrollViewer sv)
            {
                sv.ScrollToHorizontalOffset(offset);
            }
            else if (d is ScrollContentPresenter scp)
            {
                scp.SetHorizontalOffset(offset);
            }
        }
    }
}
