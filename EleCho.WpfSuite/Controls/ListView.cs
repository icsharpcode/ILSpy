using System.Windows;
using System.Windows.Media;

namespace EleCho.WpfSuite
{
    /// <inheritdoc/>
    public class ListView : System.Windows.Controls.ListView
    {
        static ListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ListView), new FrameworkPropertyMetadata(typeof(ListView)));
        }


        /// <summary>
        /// The CornerRadius property allows users to control the roundness of the corners independently by
        /// setting a radius value for each corner.  Radius values that are too large are scaled so that they
        /// smoothly blend from corner to corner.
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Background when disabled
        /// </summary>
        public Brush DisabledBackground
        {
            get { return (Brush)GetValue(DisabledBackgroundProperty); }
            set { SetValue(DisabledBackgroundProperty, value); }
        }

        /// <summary>
        /// BorderBrush when pressed by mouse
        /// </summary>
        public Brush DisabledBorderBrush
        {
            get { return (Brush)GetValue(DisabledBorderBrushProperty); }
            set { SetValue(DisabledBorderBrushProperty, value); }
        }


        /// <summary>
        /// DependencyProperty of <see cref="CornerRadius"/> property
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
			System.Windows.Controls.Border.CornerRadiusProperty.AddOwner(typeof(ListView));

        /// <summary>
        /// The DependencyProperty of <see cref="DisabledBackground"/> property
        /// </summary>
        public static readonly DependencyProperty DisabledBackgroundProperty =
            DependencyProperty.Register(nameof(DisabledBackground), typeof(Brush), typeof(ListView), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// The DependencyProperty of <see cref="DisabledBorderBrush"/> property
        /// </summary>
        public static readonly DependencyProperty DisabledBorderBrushProperty =
            DependencyProperty.Register(nameof(DisabledBorderBrush), typeof(Brush), typeof(ListView), new FrameworkPropertyMetadata(null));
    }
}
