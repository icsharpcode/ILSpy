// Copyright (c) 2014 Daniel Grunwald
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

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ICSharpCode.Decompiler.IL
{
	public sealed class InstructionCollection<T> : IList<T>, IReadOnlyList<T> where T : ILInstruction
	{
		readonly ILInstruction parentInstruction;
		readonly int firstChildIndex;
		readonly List<T> list = new List<T>();

		public InstructionCollection(ILInstruction parentInstruction, int firstChildIndex)
		{
			if (parentInstruction == null)
				throw new ArgumentNullException(nameof(parentInstruction));
			this.parentInstruction = parentInstruction;
			this.firstChildIndex = firstChildIndex;
		}

		public int Count {
			get { return list.Count; }
		}

		public T this[int index] {
			get { return list[index]; }
			set {
				T oldValue = list[index];
				if (!(oldValue == value && value.Parent == parentInstruction && value.ChildIndex == index))
				{
					list[index] = value;
					value.ChildIndex = index + firstChildIndex;
					parentInstruction.InstructionCollectionAdded(value);
					parentInstruction.InstructionCollectionRemoved(oldValue);
					parentInstruction.InstructionCollectionUpdateComplete();
				}
			}
		}

		#region GetEnumerator
		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		/// <summary>
		/// Custom enumerator for InstructionCollection.
		/// Unlike List{T}.Enumerator, this enumerator allows replacing an item during the enumeration.
		/// Adding/removing items from the collection still is invalid (however, such
		/// invalid actions are only detected in debug builds).
		/// 
		/// Warning: even though this is a struct, it is invalid to copy:
		/// the number of constructor calls must match the number of dispose calls.
		/// </summary>
		public struct Enumerator : IEnumerator<T>
		{
#if DEBUG
			ILInstruction? parentInstruction;
#endif
			readonly List<T> list;
			int pos;

			public Enumerator(InstructionCollection<T> col)
			{
				this.list = col.list;
				this.pos = -1;
#if DEBUG
				this.parentInstruction = col.parentInstruction;
				col.parentInstruction.StartEnumerator();
#endif
			}

			[DebuggerStepThrough]
			public bool MoveNext()
			{
				return ++pos < list.Count;
			}

			public T Current {
				[DebuggerStepThrough]
				get { return list[pos]; }
			}

			[DebuggerStepThrough]
			public void Dispose()
			{
#if DEBUG
				if (parentInstruction != null)
				{
					parentInstruction.StopEnumerator();
					parentInstruction = null;
				}
#endif
			}

			void System.Collections.IEnumerator.Reset()
			{
				pos = -1;
			}

			object System.Collections.IEnumerator.Current {
				get { return this.Current; }
			}
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion

		/// <summary>
		/// Gets the index of the instruction in this collection.
		/// Returns -1 if the instruction does not exist in the collection.
		/// </summary>
		/// <remarks>
		/// Runs in O(1) if the item can be found using the Parent/ChildIndex properties.
		/// Otherwise, runs in O(N).
		/// </remarks>
		public int IndexOf(T? item)
		{
			if (item == null)
			{
				// InstructionCollection can't contain nulls
				return -1;
			}
			// If this collection is the item's primary position, we can use ChildIndex:
			int index = item.ChildIndex - firstChildIndex;
			if (index >= 0 && index < list.Count && list[index] == item)
				return index;
			// But we still need to fall back on a full search, because the ILAst might be
			// in a state where item is in multiple locations.
			return list.IndexOf(item);
		}

		/// <summary>
		/// Gets whether the item is in this collection.
		/// </summary>
		/// <remarks>
		/// This method searches the list.
		/// Usually it's more efficient to test item.Parent instead!
		/// </remarks>
		public bool Contains(T? item)
		{
			return IndexOf(item) >= 0;
		}

		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);
		}

		bool ICollection<T>.IsReadOnly {
			get { return false; }
		}

		public void Add(T value)
		{
			parentInstruction.AssertNoEnumerators();
			value.ChildIndex = list.Count + firstChildIndex;
			list.Add(value);
			parentInstruction.InstructionCollectionAdded(value);
			parentInstruction.InstructionCollectionUpdateComplete();
		}

		public void AddRange(IEnumerable<T> values)
		{
			parentInstruction.AssertNoEnumerators();
			foreach (T value in values)
			{
				value.ChildIndex = list.Count + firstChildIndex;
				list.Add(value);
				parentInstruction.InstructionCollectionAdded(value);
			}
			parentInstruction.InstructionCollectionUpdateComplete();
		}

		/// <summary>
		/// Replaces all entries in the InstructionCollection with the newList.
		/// </summary>
		/// <remarks>
		/// Equivalent to Clear() followed by AddRange(newList), but slightly more efficient.
		/// </remarks>
		public void ReplaceList(IEnumerable<T> newList)
		{
			parentInstruction.AssertNoEnumerators();
			int index = 0;
			foreach (T value in newList)
			{
				value.ChildIndex = index + firstChildIndex;
				if (index < list.Count)
				{
					T oldValue = list[index];
					list[index] = value;
					parentInstruction.InstructionCollectionAdded(value);
					parentInstruction.InstructionCollectionRemoved(oldValue);
				}
				else
				{
					list.Add(value);
					parentInstruction.InstructionCollectionAdded(value);
				}
				index++;
			}
			for (int i = index; i < list.Count; i++)
			{
				parentInstruction.InstructionCollectionRemoved(list[i]);
			}
			list.RemoveRange(index, list.Count - index);
			parentInstruction.InstructionCollectionUpdateComplete();
		}

		public void Insert(int index, T item)
		{
			parentInstruction.AssertNoEnumerators();
			list.Insert(index, item);
			item.ChildIndex = index;
			parentInstruction.InstructionCollectionAdded(item);
			for (int i = index + 1; i < list.Count; i++)
			{
				T other_item = list[i];
				// Update ChildIndex of items after the inserted one, but only if
				// that's their 'primary position' (in case of multiple parents)
				if (other_item.Parent == parentInstruction && other_item.ChildIndex == i + firstChildIndex - 1)
					other_item.ChildIndex = i + firstChildIndex;
			}
			parentInstruction.InstructionCollectionUpdateComplete();
		}

		public void RemoveAt(int index)
		{
			parentInstruction.AssertNoEnumerators();
			parentInstruction.InstructionCollectionRemoved(list[index]);
			list.RemoveAt(index);
			for (int i = index; i < list.Count; i++)
			{
				var other_item = list[i];
				if (other_item.Parent == parentInstruction && other_item.ChildIndex == i + firstChildIndex + 1)
					other_item.ChildIndex = i + firstChildIndex;
			}
			parentInstruction.InstructionCollectionUpdateComplete();
		}

		/// <summary>
		/// Remove item at index <c>index</c> in O(1) by swapping it with the last element in the collection.
		/// </summary>
		public void SwapRemoveAt(int index)
		{
			parentInstruction.AssertNoEnumerators();
			parentInstruction.InstructionCollectionRemoved(list[index]);
			int removeIndex = list.Count - 1;
			T movedItem = list[index] = list[removeIndex];
			list.RemoveAt(removeIndex);
			if (movedItem.Parent == parentInstruction && movedItem.ChildIndex == removeIndex + firstChildIndex)
				movedItem.ChildIndex = index + firstChildIndex;
			parentInstruction.InstructionCollectionUpdateComplete();
		}

		public void Clear()
		{
			parentInstruction.AssertNoEnumerators();
			foreach (var entry in list)
			{
				parentInstruction.InstructionCollectionRemoved(entry);
			}
			list.Clear();
			parentInstruction.InstructionCollectionUpdateComplete();
		}

		public bool Remove(T item)
		{
			int index = IndexOf(item);
			if (index >= 0)
			{
				RemoveAt(index);
				return true;
			}
			return false;
		}

		public void RemoveRange(int index, int count)
		{
			parentInstruction.AssertNoEnumerators();
			for (int i = 0; i < count; i++)
			{
				parentInstruction.InstructionCollectionRemoved(list[index + i]);
			}
			list.RemoveRange(index, count);
			for (int i = index; i < list.Count; i++)
			{
				var other_item = list[i];
				if (other_item.Parent == parentInstruction && other_item.ChildIndex == i + firstChildIndex + count)
					other_item.ChildIndex = i + firstChildIndex;
			}
			parentInstruction.InstructionCollectionUpdateComplete();
		}

		/// <summary>
		/// Removes all elements for which the predicate returns true.
		/// </summary>
		/// <remarks>
		/// This method runs in O(N), which is more efficient than calling RemoveAt() in a loop.
		/// The collection may be in an invalid state during the invocation of the predicate.
		/// </remarks>
		public int RemoveAll(Predicate<T> predicate)
		{
			parentInstruction.AssertNoEnumerators();
			int j = 0;
			for (int i = 0; i < list.Count; i++)
			{
				T item = list[i];
				if (predicate(item))
				{
					parentInstruction.InstructionCollectionRemoved(item);
				}
				else
				{
					// keep the item
					if (item.Parent == parentInstruction && item.ChildIndex == i + firstChildIndex)
						item.ChildIndex = j + firstChildIndex;
					list[j] = item;
					j++;
				}
			}
			int removed = list.Count - j;
			if (removed > 0)
			{
				list.RemoveRange(j, removed);
				parentInstruction.InstructionCollectionUpdateComplete();
			}
			return removed;
		}

		public void MoveElementToIndex(int oldIndex, int newIndex)
		{
			parentInstruction.AssertNoEnumerators();
			var item = list[oldIndex];
			Insert(newIndex, item);
			if (oldIndex < newIndex)
				RemoveAt(oldIndex);
			else
				RemoveAt(oldIndex + 1);
		}

		public void MoveElementToIndex(T item, int newIndex)
		{
			parentInstruction.AssertNoEnumerators();
			int oldIndex = IndexOf(item);
			if (oldIndex >= 0)
			{
				Insert(newIndex, item);
				if (oldIndex < newIndex)
					RemoveAt(oldIndex);
				else
					RemoveAt(oldIndex + 1);
			}
		}

		public void MoveElementToEnd(int index)
		{
			MoveElementToIndex(index, list.Count);
		}

		public void MoveElementToEnd(T item)
		{
			MoveElementToIndex(item, list.Count);
		}

		// more efficient versions of some LINQ methods:
		public T First()
		{
			return list[0];
		}

		public T? FirstOrDefault()
		{
			return list.Count > 0 ? list[0] : null;
		}

		public T Last()
		{
			return list[list.Count - 1];
		}

		public T? LastOrDefault()
		{
			return list.Count > 0 ? list[list.Count - 1] : null;
		}

		public T? SecondToLastOrDefault()
		{
			return list.Count > 1 ? list[list.Count - 2] : null;
		}

		public T? ElementAtOrDefault(int index)
		{
			if (index >= 0 && index < list.Count)
				return list[index];
			return null;
		}
	}
}
