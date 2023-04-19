using ICSharpCode.ILSpy.Debugger.Services;
using System;
using System.Windows.Controls;

namespace ICSharpCode.ILSpy.Debugger.Tooltips
{
    internal class LazyItemsControl<T>
    {
        public LazyItemsControl(ItemsControl wrappedItemsControl, int initialItemsCount)
        {
            if (wrappedItemsControl == null)
            {
                throw new ArgumentNullException("wrappedItemsControl");
            }
            this.initialItemsCount = initialItemsCount;
            this.itemsControl = wrappedItemsControl;
            this.itemsControl.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(this.handleScroll));
        }

        public ScrollViewer ScrollViewer
        {
            get
            {
                if (this.scrollViewerCached == null)
                {
                    this.scrollViewerCached = this.itemsControl.GetScrollViewer();
                }
                return this.scrollViewerCached;
            }
        }

        public bool IsScrolledToStart
        {
            get
            {
                return this.ScrollViewer != null && this.ScrollViewer.VerticalOffset == 0.0;
            }
        }

        public bool IsScrolledToEnd
        {
            get
            {
                if (this.itemsSourceTotalCount == null)
                {
                    return false;
                }
                int value = this.itemsSourceTotalCount.Value;
                return this.ScrollViewer.VerticalOffset >= (double)value - this.ScrollViewer.ViewportHeight;
            }
        }

        public int? ItemsSourceTotalCount
        {
            get
            {
                return this.itemsSourceTotalCount;
            }
        }

        public VirtualizingIEnumerable<T> ItemsSource
        {
            get
            {
                return this.itemsSource;
            }
            set
            {
                this.itemsSource = value;
                this.addNextItems(this.itemsSource, this.initialItemsCount);
                this.itemsControl.ItemsSource = value;
            }
        }

        private void addNextItems(VirtualizingIEnumerable<T> sourceToAdd, int nItems)
        {
            sourceToAdd.AddNextItems(nItems);
            if (!sourceToAdd.HasNext)
            {
                this.itemsSourceTotalCount = new int?(sourceToAdd.Count);
            }
        }

        private void handleScroll(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange > 0.0 && e.VerticalOffset >= (double)this.itemsSource.Count - e.ViewportHeight)
            {
                this.addNextItems(this.itemsSource, (int)e.VerticalChange);
            }
        }

        private ItemsControl itemsControl;

        private int initialItemsCount;

        private ScrollViewer scrollViewerCached;

        private int? itemsSourceTotalCount = null;

        private VirtualizingIEnumerable<T> itemsSource;
    }
}
