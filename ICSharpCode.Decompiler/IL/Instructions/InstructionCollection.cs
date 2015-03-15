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
using System.Linq;

namespace ICSharpCode.Decompiler.IL
{
	public sealed class InstructionCollection<T> : Collection<T> where T : ILInstruction
	{
		readonly ILInstruction parentInstruction;
		
		public InstructionCollection(ILInstruction parentInstruction)
		{
			this.parentInstruction = parentInstruction;
		}
		
		protected override void ClearItems()
		{
			foreach (var child in this)
				parentInstruction.InstructionCollectionRemoved(child);
			base.ClearItems();
			parentInstruction.InstructionCollectionUpdateComplete();
		}
		
		protected override void InsertItem(int index, T item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			parentInstruction.InstructionCollectionAdded(item);
			base.InsertItem(index, item);
			parentInstruction.InstructionCollectionUpdateComplete();
		}
		
		protected override void RemoveItem(int index)
		{
			parentInstruction.InstructionCollectionRemoved(this[index]);
			base.RemoveItem(index);
			parentInstruction.InstructionCollectionUpdateComplete();
		}
		
		protected override void SetItem(int index, T item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			if (this[index] == item)
				return;
			parentInstruction.InstructionCollectionRemoved(this[index]);
			parentInstruction.InstructionCollectionAdded(item);
			base.SetItem(index, item);
			parentInstruction.InstructionCollectionUpdateComplete();
		}
		
		public int RemoveAll(Predicate<T> predicate)
		{
			// TODO: optimize
			int removed = 0;
			for (int i = 0; i < this.Count;) {
				if (predicate(this[i])) {
					RemoveAt(i);
					removed++;
				} else {
					i++;
				}
			}
			return removed;
		}

		public void ReplaceList(IEnumerable<T> newList)
		{
			Clear();
			this.AddRange(newList);
		}
	}
}
