using ICSharpCode.ILSpy.AvalonEdit;
using ICSharpCode.ILSpy.Debugger.Models.TreeModel;
using ICSharpCode.ILSpy.Debugger.Services;
using ICSharpCode.NRefactory;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ICSharpCode.ILSpy.Debugger.Tooltips
{
    internal partial class DebuggerTooltipControl : UserControl, ITooltip, IStyleConnector
    {
        public DebuggerTooltipControl(TextLocation logicalPosition)
        {
            this.logicalPosition = logicalPosition;
            this.InitializeComponent();
            base.Loaded += this.OnLoaded;
        }

        public DebuggerTooltipControl(TextLocation logicalPosition, ITreeNode node) : this(logicalPosition, new ITreeNode[]
{
            node
})
        {
        }

        public DebuggerTooltipControl(TextLocation logicalPosition, IEnumerable<ITreeNode> nodes) : this(logicalPosition)
        {
            this.itemsSource = nodes;
        }

        public DebuggerTooltipControl(DebuggerTooltipControl parentControl, TextLocation logicalPosition, bool showPins = false) : this(logicalPosition)
        {
            this.parentControl = parentControl;
            this.showPins = showPins;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!this.showPins)
            {
                this.dataGrid.Columns[4].Visibility = Visibility.Collapsed;
            }
            this.SetItemsSource(this.itemsSource);
        }

        public event RoutedEventHandler Closed;

        protected void OnClosed()
        {
            if (this.Closed != null)
            {
                this.Closed(this, new RoutedEventArgs());
            }
        }

        public IEnumerable<ITreeNode> ItemsSource
        {
            get
            {
                return this.itemsSource;
            }
        }

        public void SetItemsSource(IEnumerable<ITreeNode> value)
        {
            if (value == null)
            {
                return;
            }
            this.itemsSource = value;
            this.lazyGrid = new LazyItemsControl<ITreeNode>(this.dataGrid, 12);
            VirtualizingIEnumerable<ITreeNode> virtualizingIEnumerable = new VirtualizingIEnumerable<ITreeNode>(value);
            this.lazyGrid.ItemsSource = virtualizingIEnumerable;
            this.dataGrid.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(this.handleScroll));
            if (this.lazyGrid.ItemsSourceTotalCount != null)
            {
                this.btnUp.Visibility = (this.btnDown.Visibility = ((this.lazyGrid.ItemsSourceTotalCount.Value <= 11) ? Visibility.Collapsed : Visibility.Visible));
            }
        }

        public bool ShowAsPopup
        {
            get
            {
                return true;
            }
        }

        public bool Close(bool mouseClick)
        {
            if (mouseClick || (!mouseClick && !this.isChildExpanded))
            {
                this.CloseChildPopups();
                return true;
            }
            return false;
        }

        private DebuggerPopup childPopup { get; set; }

        private DebuggerTooltipControl parentControl { get; set; }

        internal DebuggerPopup containingPopup { get; set; }

        private bool isChildExpanded
        {
            get
            {
                return this.childPopup != null && this.childPopup.IsOpen;
            }
        }

        public void CloseChildPopups()
        {
            if (this.expandedButton != null)
            {
                this.expandedButton.IsChecked = new bool?(false);
                this.expandedButton = null;
                this.childPopup.CloseSelfAndChildren();
            }
        }

        public void CloseOnLostFocus()
        {
            if (this.containingPopup != null)
            {
                this.containingPopup.IsLeaf = true;
            }
            if (!base.IsMouseOver)
            {
                if (this.containingPopup != null)
                {
                    this.containingPopup.IsOpen = false;
                    this.containingPopup.IsLeaf = false;
                }
                if (this.parentControl != null)
                {
                    this.parentControl.CloseOnLostFocus();
                }
                this.OnClosed();
                return;
            }
            if (this.expandedButton != null && !this.expandedButton.IsMouseOver)
            {
                this.expandedButton.IsChecked = new bool?(false);
                this.expandedButton = null;
            }
        }

        private void btnExpander_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = (ToggleButton)e.OriginalSource;
            ITreeNode treeNode = (ITreeNode)toggleButton.DataContext;
            Point point = toggleButton.PointToScreen(new Point(0.0, 0.0)).TransformFromDevice(toggleButton);
            if (toggleButton.IsChecked.GetValueOrDefault(false))
            {
                this.CloseChildPopups();
                this.expandedButton = toggleButton;
                if (this.childPopup == null)
                {
                    this.childPopup = new DebuggerPopup(this, this.logicalPosition, false);
                    this.childPopup.Placement = PlacementMode.Absolute;
                }
                if (this.containingPopup != null)
                {
                    this.containingPopup.IsLeaf = false;
                }
                this.childPopup.IsLeaf = true;
                this.childPopup.HorizontalOffset = point.X + 16.0;
                this.childPopup.VerticalOffset = point.Y + 15.0;
                this.childPopup.ItemsSource = treeNode.ChildNodes;
                this.childPopup.Open();
                return;
            }
            this.CloseChildPopups();
        }

        private void handleScroll(object sender, ScrollChangedEventArgs e)
        {
            if (this.lazyGrid == null)
            {
                return;
            }
            this.btnUp.IsEnabled = !this.lazyGrid.IsScrolledToStart;
            this.btnDown.IsEnabled = !this.lazyGrid.IsScrolledToEnd;
        }

        private void BtnUp_Click(object sender, RoutedEventArgs e)
        {
            if (this.lazyGrid == null)
            {
                return;
            }
            this.lazyGrid.ScrollViewer.ScrollUp(1.0);
        }

        private void BtnDown_Click(object sender, RoutedEventArgs e)
        {
            if (this.lazyGrid == null)
            {
                return;
            }
            this.lazyGrid.ScrollViewer.ScrollDown(1.0);
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.dataGrid.Focus();
                return;
            }
            if (e.Key == Key.Return)
            {
                this.dataGrid.Focus();
                TextBox textBox = (TextBox)e.OriginalSource;
                string text = textBox.Text;
                ITreeNode node = ((FrameworkElement)sender).DataContext as ITreeNode;
                this.SaveNewValue(node, textBox.Text);
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)e.OriginalSource;
            string text = textBox.Text;
            ITreeNode node = ((FrameworkElement)sender).DataContext as ITreeNode;
            this.SaveNewValue(node, textBox.Text);
        }

        private void SaveNewValue(ITreeNode node, string newValue)
        {
            if (node != null && node.SetText(newValue))
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.dataGrid);
                Adorner[] adorners = adornerLayer.GetAdorners(this.dataGrid);
                if (adorners != null && adorners.Length != 0)
                {
                    adornerLayer.Remove(adorners[0]);
                }
                DebuggerTooltipControl.SavedAdorner adorner = new DebuggerTooltipControl.SavedAdorner(this.dataGrid);
                adornerLayer.Add(adorner);
            }
        }

        private void PinButton_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void PinButton_Unchecked(object sender, RoutedEventArgs e)
        {
        }

        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        [DebuggerNonUserCode]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void IStyleConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 3:
                    ((ToggleButton)target).Click += this.btnExpander_Click;
                    return;
                case 4:
                    ((TextBox)target).KeyUp += this.TextBox_KeyUp;
                    return;
                case 5:
                    ((ToggleButton)target).Checked += this.PinButton_Checked;
                    ((ToggleButton)target).Unchecked += this.PinButton_Unchecked;
                    return;
                default:
                    return;
            }
        }

        private const double ChildPopupOpenXOffet = 16.0;

        private const double ChildPopupOpenYOffet = 15.0;

        private const int InitialItemsCount = 12;

        private const int VisibleItemsCount = 11;

        private bool showPins;

        private LazyItemsControl<ITreeNode> lazyGrid;

        private IEnumerable<ITreeNode> itemsSource;

        private readonly TextLocation logicalPosition;

        private ToggleButton expandedButton;

        private class SavedAdorner : Adorner
        {
            public SavedAdorner(UIElement adornedElement) : base(adornedElement)
            {
                base.Loaded += delegate (object A_1, RoutedEventArgs A_2)
                {
                    this.Show();
                };
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                Rect rect = new Rect(base.AdornedElement.DesiredSize);
                FormattedText formattedText = new FormattedText("Saved", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Black, FontStretches.Expanded), 8.0, Brushes.Black);
                drawingContext.DrawText(formattedText, new Point(rect.TopRight.X - formattedText.Width - 2.0, rect.TopRight.Y));
            }

            private void Show()
            {
                DoubleAnimation doubleAnimation = new DoubleAnimation();
                doubleAnimation.From = new double?(1.0);
                doubleAnimation.To = new double?(0.0);
                doubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(2.0));
                doubleAnimation.SetValue(Storyboard.TargetProperty, this);
                doubleAnimation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath(UIElement.OpacityProperty));
                new Storyboard
                {
                    Children =
                    {
                        doubleAnimation
                    }
                }.Begin(this);
            }
        }
    }
}
