using System.Windows;
using System.Windows.Media;

namespace EleCho.WpfSuite
{
    public class ScrollBar : System.Windows.Controls.Primitives.ScrollBar
    {
        private static readonly Brush s_thumbBrush =new SolidColorBrush(Color.FromRgb(205, 205, 205));
        private static readonly Brush s_glyphBrush= new SolidColorBrush(Color.FromRgb(96, 96, 96));
        private static readonly Brush s_disabledGlyphBrush =new SolidColorBrush(Color.FromRgb(112, 112, 112));
        private static readonly Geometry s_glyphLeft = Geometry.Parse("M 3.18,7 C3.18,7 5,7 5,7 5,7 1.81,3.5 1.81,3.5 1.81,3.5 5,0 5,0 5,0 3.18,0 3.18,0 3.18,0 0,3.5 0,3.5 0,3.5 3.18,7 3.18,7 z");
        private static readonly Geometry s_glyphRight = Geometry.Parse("M 1.81,7 C1.81,7 0,7 0,7 0,7 3.18,3.5 3.18,3.5 3.18,3.5 0,0 0,0 0,0 1.81,0 1.81,0 1.81,0 5,3.5 5,3.5 5,3.5 1.81,7 1.81,7 z");
        private static readonly Geometry s_glyphUp = Geometry.Parse("M 0,4 C0,4 0,6 0,6 0,6 3.5,2.5 3.5,2.5 3.5,2.5 7,6 7,6 7,6 7,4 7,4 7,4 3.5,0.5 3.5,0.5 3.5,0.5 0,4 0,4 z");
        private static readonly Geometry s_glyphDown = Geometry.Parse("M 0,2.5 C0,2.5 0,0.5 0,0.5 0,0.5 3.5,4 3.5,4 3.5,4 7,0.5 7,0.5 7,0.5 7,2.5 7,2.5 7,2.5 3.5,6 3.5,6 3.5,6 0,2.5 0,2.5 z");

        static ScrollBar()
        {
            s_thumbBrush.Freeze();
            s_glyphBrush.Freeze();
            s_disabledGlyphBrush.Freeze();

            s_glyphLeft.Freeze();
            s_glyphRight.Freeze();
            s_glyphUp.Freeze();
            s_glyphDown.Freeze();

            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScrollBar), new FrameworkPropertyMetadata(typeof(ScrollBar)));
        }


        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public CornerRadius ThumbCornerRadius
        {
            get { return (CornerRadius)GetValue(ThumbCornerRadiusProperty); }
            set { SetValue(ThumbCornerRadiusProperty, value); }
        }

        public CornerRadius ButtonCornerRadius
        {
            get { return (CornerRadius)GetValue(ButtonCornerRadiusProperty); }
            set { SetValue(ButtonCornerRadiusProperty, value); }
        }

        public Thickness GlyphMargin
        {
            get { return (Thickness)GetValue(GlyphMarginProperty); }
            set { SetValue(GlyphMarginProperty, value); }
        }



        public Geometry? ArrowLeftGlyph
        {
            get { return (Geometry?)GetValue(ArrowLeftGlyphProperty); }
            set { SetValue(ArrowLeftGlyphProperty, value); }
        }

        public Geometry? ArrowRightGlyph
        {
            get { return (Geometry?)GetValue(ArrowRightGlyphProperty); }
            set { SetValue(ArrowRightGlyphProperty, value); }
        }

        public Geometry? ArrowUpGlyph
        {
            get { return (Geometry?)GetValue(ArrowUpGlyphProperty); }
            set { SetValue(ArrowUpGlyphProperty, value); }
        }

        public Geometry? ArrowDownGlyph
        {
            get { return (Geometry?)GetValue(ArrowDownGlyphProperty); }
            set { SetValue(ArrowDownGlyphProperty, value); }
        }

        public Brush ThumbBrush
        {
            get { return (Brush)GetValue(ThumbBrushProperty); }
            set { SetValue(ThumbBrushProperty, value); }
        }

        public Brush GlyphBrush
        {
            get { return (Brush)GetValue(GlyphBrushProperty); }
            set { SetValue(GlyphBrushProperty, value); }
        }

        public Brush HoverBackground
        {
            get { return (Brush)GetValue(HoverBackgroundProperty); }
            set { SetValue(HoverBackgroundProperty, value); }
        }

        public Brush HoverBorderBrush
        {
            get { return (Brush)GetValue(HoverBorderBrushProperty); }
            set { SetValue(HoverBorderBrushProperty, value); }
        }

        public Brush HoverThumbBrush
        {
            get { return (Brush)GetValue(HoverThumbBrushProperty); }
            set { SetValue(HoverThumbBrushProperty, value); }
        }

        public Brush HoverGlyphBrush
        {
            get { return (Brush)GetValue(HoverGlyphBrushProperty); }
            set { SetValue(HoverGlyphBrushProperty, value); }
        }

        public Brush PressedBackground
        {
            get { return (Brush)GetValue(PressedBackgroundProperty); }
            set { SetValue(PressedBackgroundProperty, value); }
        }

        public Brush PressedBorderBrush
        {
            get { return (Brush)GetValue(PressedBorderBrushProperty); }
            set { SetValue(PressedBorderBrushProperty, value); }
        }

        public Brush PressedThumbBrush
        {
            get { return (Brush)GetValue(PressedThumbBrushProperty); }
            set { SetValue(PressedThumbBrushProperty, value); }
        }

        public Brush PressedGlyphBrush
        {
            get { return (Brush)GetValue(PressedGlyphBrushProperty); }
            set { SetValue(PressedGlyphBrushProperty, value); }
        }

        public Brush DisabledBackground
        {
            get { return (Brush)GetValue(DisabledBackgroundProperty); }
            set { SetValue(DisabledBackgroundProperty, value); }
        }

        public Brush DisabledBorderBrush
        {
            get { return (Brush)GetValue(DisabledBorderBrushProperty); }
            set { SetValue(DisabledBorderBrushProperty, value); }
        }

        public Brush DisabledThumbBrush
        {
            get { return (Brush)GetValue(DisabledThumbBrushProperty); }
            set { SetValue(DisabledThumbBrushProperty, value); }
        }

        public Brush DisabledGlyphBrush
        {
            get { return (Brush)GetValue(DisabledGlyphBrushProperty); }
            set { SetValue(DisabledGlyphBrushProperty, value); }
        }




        public static readonly DependencyProperty CornerRadiusProperty =
            Border.CornerRadiusProperty.AddOwner(typeof(ScrollBar));

        public static readonly DependencyProperty ThumbCornerRadiusProperty =
            DependencyProperty.Register(nameof(ThumbCornerRadius), typeof(CornerRadius), typeof(ScrollBar), new FrameworkPropertyMetadata(new CornerRadius(0)));

        public static readonly DependencyProperty ButtonCornerRadiusProperty =
            DependencyProperty.Register(nameof(ButtonCornerRadius), typeof(CornerRadius), typeof(ScrollBar), new FrameworkPropertyMetadata(new CornerRadius(0)));

        public static readonly DependencyProperty GlyphMarginProperty =
            DependencyProperty.Register(nameof(GlyphMargin), typeof(Thickness), typeof(ScrollBar), new FrameworkPropertyMetadata(new Thickness(3)));

        public static readonly DependencyProperty ArrowLeftGlyphProperty =
            DependencyProperty.Register(nameof(ArrowLeftGlyph), typeof(Geometry), typeof(ScrollBar), new FrameworkPropertyMetadata(s_glyphLeft));

        public static readonly DependencyProperty ArrowRightGlyphProperty =
            DependencyProperty.Register(nameof(ArrowRightGlyph), typeof(Geometry), typeof(ScrollBar), new FrameworkPropertyMetadata(s_glyphRight));

        public static readonly DependencyProperty ArrowUpGlyphProperty =
            DependencyProperty.Register(nameof(ArrowUpGlyph), typeof(Geometry), typeof(ScrollBar), new FrameworkPropertyMetadata(s_glyphUp));

        public static readonly DependencyProperty ArrowDownGlyphProperty =
            DependencyProperty.Register(nameof(ArrowDownGlyph), typeof(Geometry), typeof(ScrollBar), new FrameworkPropertyMetadata(s_glyphDown));

        public static readonly DependencyProperty ThumbBrushProperty =
            DependencyProperty.Register(nameof(ThumbBrush), typeof(Brush), typeof(ScrollBar), new FrameworkPropertyMetadata(s_thumbBrush));

        public static readonly DependencyProperty GlyphBrushProperty =
            DependencyProperty.Register(nameof(GlyphBrush), typeof(Brush), typeof(ScrollBar), new FrameworkPropertyMetadata(s_glyphBrush));

        public static readonly DependencyProperty HoverBackgroundProperty =
            DependencyProperty.Register(nameof(HoverBackground), typeof(Brush), typeof(ScrollBar), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty HoverBorderBrushProperty =
            DependencyProperty.Register(nameof(HoverBorderBrush), typeof(Brush), typeof(ScrollBar), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty HoverThumbBrushProperty =
            DependencyProperty.Register(nameof(HoverThumbBrush), typeof(Brush), typeof(ScrollBar), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty HoverGlyphBrushProperty =
            DependencyProperty.Register(nameof(HoverGlyphBrush), typeof(Brush), typeof(ScrollBar), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty PressedBackgroundProperty =
            DependencyProperty.Register(nameof(PressedBackground), typeof(Brush), typeof(ScrollBar), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty PressedBorderBrushProperty =
            DependencyProperty.Register(nameof(PressedBorderBrush), typeof(Brush), typeof(ScrollBar), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty PressedThumbBrushProperty =
            DependencyProperty.Register(nameof(PressedThumbBrush), typeof(Brush), typeof(ScrollBar), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty PressedGlyphBrushProperty =
            DependencyProperty.Register(nameof(PressedGlyphBrush), typeof(Brush), typeof(ScrollBar), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty DisabledBackgroundProperty =
            DependencyProperty.Register(nameof(DisabledBackground), typeof(Brush), typeof(ScrollBar), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty DisabledBorderBrushProperty =
            DependencyProperty.Register(nameof(DisabledBorderBrush), typeof(Brush), typeof(ScrollBar), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty DisabledThumbBrushProperty =
            DependencyProperty.Register(nameof(DisabledThumbBrush), typeof(Brush), typeof(ScrollBar), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty DisabledGlyphBrushProperty =
            DependencyProperty.Register(nameof(DisabledGlyphBrush), typeof(Brush), typeof(ScrollBar), new FrameworkPropertyMetadata(s_disabledGlyphBrush));
    }
}
