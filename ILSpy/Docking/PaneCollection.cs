using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Docking
{
	public class PaneCollection<T> : INotifyCollectionChanged, INotifyPropertyChanged, ICollection<T>
		where T : PaneModel
	{
		private ObservableCollection<T> observableCollection = new ObservableCollection<T>();

		public event NotifyCollectionChangedEventHandler CollectionChanged;
		public event PropertyChangedEventHandler PropertyChanged;

		public PaneCollection()
		{
			observableCollection.CollectionChanged += (sender, e) => CollectionChanged?.Invoke(this, e);
		}

		public void Add(T item)
		{
			if (!this.Any(pane => pane.ContentId == item.ContentId)) {
				observableCollection.Add(item);
			}

			item.IsVisible = true;
			item.IsActive = true;
		}

		public int Count => observableCollection.Count();
		public bool IsReadOnly => false;
		public void Clear() => observableCollection.Clear();
		public bool Contains(T item) => observableCollection.Contains(item);
		public void CopyTo(T[] array, int arrayIndex) => observableCollection.CopyTo(array, arrayIndex);
		public bool Remove(T item) => observableCollection.Remove(item);
		public IEnumerator<T> GetEnumerator() => observableCollection.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => observableCollection.GetEnumerator();
	}
}
