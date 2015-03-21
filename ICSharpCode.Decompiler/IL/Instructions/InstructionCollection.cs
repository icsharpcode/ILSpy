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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace ICSharpCode.Decompiler.IL
{
	public sealed class InstructionCollection<T> : IList<T> where T : ILInstruction
	{
		readonly ILInstruction parentInstruction;
		readonly int firstChildIndex;
		readonly List<T> list = new List<T>();
		
		public InstructionCollection(ILInstruction parentInstruction, int firstChildIndex)
		{
			if (parentInstruction == null)
				throw new ArgumentNullException("parentInstruction");
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
				if (oldValue != value) {
					list[index] = value;
					value.ChildIndex = index + firstChildIndex;
					parentInstruction.InstructionCollectionAdded(value);
					parentInstruction.InstructionCollectionRemoved(oldValue);
					parentInstruction.InstructionCollectionUpdateComplete();
				}
			}
		}

		public List<T>.Enumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}
		
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return list.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}
		
		/// <summary>
		/// Gets the index of the instruction in this collection.
		/// Returns -1 if the instruction does not exist in the collection.
		/// </summary>
		/// <remarks>
		/// Runs in O(1) is the item can be found using the Parent/ChildIndex properties.
		/// Otherwise, runs in O(N).
		/// </remarks>
		public int IndexOf(T item)
		{
			// If this collection is the item's primary location, we can use ChildIndex:
			int index = item.ChildIndex - firstChildIndex;
			if (index >= 0 && index <= list.Count && list[index] == item)
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
		public bool Contains(T item)
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
			value.ChildIndex = list.Count + firstChildIndex;
			list.Add(value);
			parentInstruction.InstructionCollectionAdded(value);
			parentInstruction.InstructionCollectionUpdateComplete();
		}
		
		public void AddRange(IEnumerable<T> values)
		{
			foreach (T value in values) {
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
			int index = 0;
			foreach (T value in newList) {
				value.ChildIndex = index + firstChildIndex;
				if (index < list.Count) {
					T oldValue = list[index];
					list[index] = value;
					parentInstruction.InstructionCollectionAdded(value);
					parentInstruction.InstructionCollectionRemoved(oldValue);
				} else {
					list.Add(value);
					parentInstruction.InstructionCollectionAdded(value);
				}
				index++;
			}
			for (int i = index; i < list.Count; i++) {
				parentInstruction.InstructionCollectionRemoved(list[i]);
			}
			list.RemoveRange(index, list.Count - index);
			parentInstruction.InstructionCollectionUpdateComplete();
		}

		public void Insert(int index, T item)
		{
			list.Insert(index, item);
			item.ChildIndex = index;
			parentInstruction.InstructionCollectionAdded(item);
			for (int i = index + 1; i < list.Count; i++) {
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
			parentInstruction.InstructionCollectionRemoved(list[index]);
			list.RemoveAt(index);
			for (int i = index; i < list.Count; i++) {
				var other_item = list[i];
				if (other_item.Parent == parentInstruction && other_item.ChildIndex == i + firstChildIndex + 1)
					other_item.ChildIndex = i + firstChildIndex;
			}
			parentInstruction.InstructionCollectionUpdateComplete();
		}

		public void Clear()
		{
			foreach (var entry in list) {
				parentInstruction.InstructionCollectionRemoved(entry);
			}
			list.Clear();
			parentInstruction.InstructionCollectionUpdateComplete();
		}

		public bool Remove(T item)
		{
			int index = IndexOf(item);
			if (index >= 0) {
				RemoveAt(index);
				return true;
			}
			return false;
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
			int j = 0;
			for (int i = 0; i < list.Count; i++) {
				T item = list[i];
				if (predicate(item)) {
					parentInstruction.InstructionCollectionRemoved(item);
				} else {
					// keep the item
					if (item.Parent == parentInstruction && item.ChildIndex == i + firstChildIndex)
						item.ChildIndex = j + firstChildIndex;
					list[j] = item;
					j++;
				}
			}
			int removed = list.Count - j;
			if (removed > 0) {
				list.RemoveRange(j, removed);
				parentInstruction.InstructionCollectionUpdateComplete();
			}
			return removed;
		}
	}
}
