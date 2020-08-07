// Copyright (c) 2019 AlphaSierraPapa for the SharpDevelop Team
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

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Docking
{
	public class PaneCollection<T> : INotifyCollectionChanged, ICollection<T>
		where T : PaneModel
	{
		private ObservableCollection<T> observableCollection = new ObservableCollection<T>();

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public PaneCollection()
		{
			observableCollection.CollectionChanged += (sender, e) => CollectionChanged?.Invoke(this, e);
		}

		public void Add(T item)
		{
			observableCollection.Add(item);

			item.IsVisible = true;
			item.IsActive = true;
		}

		public int Count => observableCollection.Count;
		public bool IsReadOnly => false;
		public void Clear() => observableCollection.Clear();
		public bool Contains(T item) => observableCollection.Contains(item);
		public void CopyTo(T[] array, int arrayIndex) => observableCollection.CopyTo(array, arrayIndex);
		public bool Remove(T item) => observableCollection.Remove(item);
		public IEnumerator<T> GetEnumerator() => observableCollection.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => observableCollection.GetEnumerator();
	}
}
