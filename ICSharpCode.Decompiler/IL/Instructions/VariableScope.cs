// Copyright (c) 2016 Daniel Grunwald
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
using System.Diagnostics;
using System.Linq;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// An ILInstruction that provides a scope for local variables.
	/// </summary>
	public abstract class ILVariableScope : ILInstruction
	{
		public readonly ILVariableCollection Variables;
		
		protected ILVariableScope(OpCode opCode) : base(opCode)
		{
			this.Variables = new ILVariableCollection(this);
		}
		
		internal override void CheckInvariant(ILPhase phase)
		{
			for (int i = 0; i < Variables.Count; i++) {
				Debug.Assert(Variables[i].Scope == this);
				Debug.Assert(Variables[i].IndexInScope == i);
			}
			base.CheckInvariant(phase);
		}
		
		protected void CloneVariables()
		{
			throw new NotImplementedException();
		}
	}
	
	/// <summary>
	/// The collection of variables in a <c>ILVariableScope</c>.
	/// </summary>
	public class ILVariableCollection : ICollection<ILVariable>, IReadOnlyList<ILVariable>
	{
		readonly ILVariableScope scope;
		readonly List<ILVariable> list = new List<ILVariable>();
		
		internal ILVariableCollection(ILVariableScope scope)
		{
			this.scope = scope;
		}
		
		/// <summary>
		/// Gets a variable given its <c>IndexInScope</c>.
		/// </summary>
		public ILVariable this[int index] {
			get {
				return list[index];
			}
		}
		
		public bool Add(ILVariable item)
		{
			if (item.Scope != null) {
				if (item.Scope == scope)
					return false;
				else
					throw new ArgumentException("Variable already belongs to another scope");
			}
			item.Scope = scope;
			item.IndexInScope = list.Count;
			list.Add(item);
			return true;
		}
		
		void ICollection<ILVariable>.Add(ILVariable item)
		{
			Add(item);
		}
		
		public void Clear()
		{
			foreach (var v in list) {
				v.Scope = null;
			}
			list.Clear();
		}
		
		public bool Contains(ILVariable item)
		{
			Debug.Assert(item.Scope != scope || list[item.IndexInScope] == item);
			return item.Scope == scope;
		}
		
		public bool Remove(ILVariable item)
		{
			if (item.Scope != scope)
				return false;
			Debug.Assert(list[item.IndexInScope] == item);
			RemoveAt(item.IndexInScope);
			return true;
		}
		
		void RemoveAt(int index)
		{
			// swap-remove index
			list[index] = list[list.Count - 1];
			list[index].IndexInScope = index;
			list.RemoveAt(list.Count - 1);
		}
		
		/// <summary>
		/// Remove variables that have StoreCount == LoadCount == AddressCount == 0.
		/// </summary>
		public void RemoveDead()
		{
			for (int i = 0; i < list.Count;) {
				var v = list[i];
				int deadStoreCount = v.HasInitialValue ? 1 : 0;
				if (v.StoreCount == deadStoreCount && v.LoadCount == 0 && v.AddressCount == 0) {
					RemoveAt(i);
				} else {
					i++;
				}
			}
		}
		
		public int Count {
			get { return list.Count; }
		}
		
		public void CopyTo(ILVariable[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);
		}
		
		bool ICollection<ILVariable>.IsReadOnly {
			get { return false; }
		}
		
		public List<ILVariable>.Enumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}
		
		IEnumerator<ILVariable> IEnumerable<ILVariable>.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
