using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSharpCode.ILSpy.Debugger.Tooltips
{

    internal class VirtualizingIEnumerable<T> : ObservableCollection<T>
    {

        public VirtualizingIEnumerable(IEnumerable<T> originalSource)
        {
            if (originalSource == null)
            {
                throw new ArgumentNullException("originalSource");
            }
            this.originalSourceEnumerator = originalSource.GetEnumerator();
        }


        public bool HasNext
        {
            get
            {
                return this.hasNext;
            }
        }


        public void AddNextItems(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (!this.originalSourceEnumerator.MoveNext())
                {
                    this.hasNext = false;
                    return;
                }
                base.Add(this.originalSourceEnumerator.Current);
            }
        }


        private IEnumerator<T> originalSourceEnumerator;


        private bool hasNext = true;
    }
}
