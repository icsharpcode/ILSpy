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
using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

namespace ICSharpCode.Decompiler
{
	/// <summary>
	/// Improved version of BitArray
	/// </summary>
	public class BitSet
	{
		readonly BitArray bits;
		
		/// <summary>
		/// Creates a new bitset, where initially all bits are zero.
		/// </summary>
		public BitSet(int capacity)
		{
			this.bits = new BitArray(capacity);
		}

		private BitSet(BitArray bits)
		{
			this.bits = bits;
		}
		
		public BitSet Clone()
		{
			return new BitSet((BitArray)bits.Clone());
		}

		public bool this[int index] {
			get {
				return bits[index];
			}
			set {
				bits[index] = value;
			}
		}
		
		/// <summary>
		/// Gets whether this set is a subset of other, or equal.
		/// </summary>
		public bool IsSubsetOf(BitSet other)
		{
			for (int i = 0; i < bits.Length; i++) {
				if (bits[i] && !other[i])
					return false;
			}
			return true;
		}
		
		public bool IsSupersetOf(BitSet other)
		{
			return other.IsSubsetOf(this);
		}
		
		public void UnionWith(BitSet other)
		{
			bits.Or(other.bits);
		}
		
		public void IntersectWith(BitSet other)
		{
			bits.And(other.bits);
		}
		
		public void ClearAll()
		{
			bits.SetAll(false);
		}
		
		public void Clear(int index)
		{
			bits[index] = false;
		}
		
		public void Clear(int startIndex, int endIndex)
		{
			for (int i = startIndex; i < endIndex; i++) {
				bits[i] = false;
			}
		}

		public void SetAll()
		{
			bits.SetAll(true);
		}
		
		public void Set(int index)
		{
			bits[index] = true;
		}
		
		public void Set(int startIndex, int endIndex)
		{
			for (int i = startIndex; i < endIndex; i++) {
				bits[i] = true;
			}
		}

		public void ReplaceWith(BitSet incoming)
		{
			Debug.Assert(bits.Length == incoming.bits.Length);
			for (int i = 0; i < bits.Length; i++) {
				bits[i] = incoming.bits[i];
			}
		}
		
		public override string ToString()
		{
			StringBuilder b = new StringBuilder();
			b.Append('{');
			for (int i = 0; i < bits.Length; i++) {
				if (bits[i]) {
					if (b.Length > 1)
						b.Append(", ");
					if (b.Length > 500) {
						b.Append("...");
						break;
					}
					b.Append(i);
				}
			}
			b.Append('}');
			return b.ToString();
		}
	}
}
