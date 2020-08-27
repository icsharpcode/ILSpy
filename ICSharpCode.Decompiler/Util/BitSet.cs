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
using System.Diagnostics;
using System.Text;

namespace ICSharpCode.Decompiler.Util
{
	/// <summary>
	/// Improved version of BitArray
	/// </summary>
	public class BitSet
	{
		const int BitsPerWord = 64;
		const int Log2BitsPerWord = 6;
		const ulong Mask = 0xffffffffffffffffUL;

		readonly ulong[] words;

		static int WordIndex(int bitIndex)
		{
			Debug.Assert(bitIndex >= 0);
			return bitIndex >> Log2BitsPerWord;
		}

		/// <summary>
		/// Creates a new bitset, where initially all bits are zero.
		/// </summary>
		public BitSet(int capacity)
		{
			this.words = new ulong[Math.Max(1, WordIndex(capacity + BitsPerWord - 1))];
		}

		private BitSet(ulong[] bits)
		{
			this.words = bits;
		}

		public BitSet Clone()
		{
			return new BitSet((ulong[])words.Clone());
		}

		public bool this[int index] {
			get {
				return (words[WordIndex(index)] & (1UL << index)) != 0;
			}
			set {
				if (value)
					Set(index);
				else
					Clear(index);
			}
		}

		/// <summary>
		/// Gets whether at least one bit is set.
		/// </summary>
		public bool Any()
		{
			for (int i = 0; i < words.Length; i++)
			{
				if (words[i] != 0)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Gets whether all bits in the specified range are set.
		/// </summary>
		public bool All(int startIndex, int endIndex)
		{
			Debug.Assert(startIndex <= endIndex);
			if (startIndex >= endIndex)
			{
				return true;
			}
			int startWordIndex = WordIndex(startIndex);
			int endWordIndex = WordIndex(endIndex - 1);
			ulong startMask = Mask << startIndex;
			ulong endMask = Mask >> -endIndex; // same as (Mask >> (64 - (endIndex % 64)))
			if (startWordIndex == endWordIndex)
			{
				return (words[startWordIndex] & (startMask & endMask)) == (startMask & endMask);
			}
			else
			{
				if ((words[startWordIndex] & startMask) != startMask)
					return false;
				for (int i = startWordIndex + 1; i < endWordIndex; i++)
				{
					if (words[i] != ulong.MaxValue)
						return false;
				}
				return (words[endWordIndex] & endMask) == endMask;
			}
		}

		/// <summary>
		/// Gets whether both bitsets have the same content.
		/// </summary>
		public bool SetEquals(BitSet other)
		{
			Debug.Assert(words.Length == other.words.Length);
			for (int i = 0; i < words.Length; i++)
			{
				if (words[i] != other.words[i])
					return false;
			}
			return true;
		}

		/// <summary>
		/// Gets whether this set is a subset of other, or equal.
		/// </summary>
		public bool IsSubsetOf(BitSet other)
		{
			for (int i = 0; i < words.Length; i++)
			{
				if ((words[i] & ~other.words[i]) != 0)
					return false;
			}
			return true;
		}

		/// <summary>
		/// Gets whether this set is a superset of other, or equal.
		/// </summary>
		public bool IsSupersetOf(BitSet other)
		{
			return other.IsSubsetOf(this);
		}

		public bool IsProperSubsetOf(BitSet other)
		{
			return IsSubsetOf(other) && !SetEquals(other);
		}

		public bool IsProperSupersetOf(BitSet other)
		{
			return IsSupersetOf(other) && !SetEquals(other);
		}

		/// <summary>
		/// Gets whether at least one bit is set in both bitsets.
		/// </summary>
		public bool Overlaps(BitSet other)
		{
			for (int i = 0; i < words.Length; i++)
			{
				if ((words[i] & other.words[i]) != 0)
					return true;
			}
			return false;
		}

		public void UnionWith(BitSet other)
		{
			Debug.Assert(words.Length == other.words.Length);
			for (int i = 0; i < words.Length; i++)
			{
				words[i] |= other.words[i];
			}
		}

		public void IntersectWith(BitSet other)
		{
			for (int i = 0; i < words.Length; i++)
			{
				words[i] &= other.words[i];
			}
		}

		public void Set(int index)
		{
			words[WordIndex(index)] |= (1UL << index);
		}

		/// <summary>
		/// Sets all bits i; where startIndex &lt;= i &lt; endIndex.
		/// </summary>
		public void Set(int startIndex, int endIndex)
		{
			Debug.Assert(startIndex <= endIndex);
			if (startIndex >= endIndex)
			{
				return;
			}
			int startWordIndex = WordIndex(startIndex);
			int endWordIndex = WordIndex(endIndex - 1);
			ulong startMask = Mask << startIndex;
			ulong endMask = Mask >> -endIndex; // same as (Mask >> (64 - (endIndex % 64)))
			if (startWordIndex == endWordIndex)
			{
				words[startWordIndex] |= (startMask & endMask);
			}
			else
			{
				words[startWordIndex] |= startMask;
				for (int i = startWordIndex + 1; i < endWordIndex; i++)
				{
					words[i] = ulong.MaxValue;
				}
				words[endWordIndex] |= endMask;
			}
		}

		// Note: intentionally no SetAll(), because it would also set the
		// extra bits (due to the capacity being rounded up to a full word).

		public void Clear(int index)
		{
			words[WordIndex(index)] &= ~(1UL << index);
		}

		/// <summary>
		/// Clear all bits i; where startIndex &lt;= i &lt; endIndex.
		/// </summary>
		public void Clear(int startIndex, int endIndex)
		{
			Debug.Assert(startIndex <= endIndex);
			if (startIndex >= endIndex)
			{
				return;
			}
			int startWordIndex = WordIndex(startIndex);
			int endWordIndex = WordIndex(endIndex - 1);
			ulong startMask = Mask << startIndex;
			ulong endMask = Mask >> -endIndex; // same as (Mask >> (64 - (endIndex % 64)))
			if (startWordIndex == endWordIndex)
			{
				words[startWordIndex] &= ~(startMask & endMask);
			}
			else
			{
				words[startWordIndex] &= ~startMask;
				for (int i = startWordIndex + 1; i < endWordIndex; i++)
				{
					words[i] = 0;
				}
				words[endWordIndex] &= ~endMask;
			}
		}

		public void ClearAll()
		{
			for (int i = 0; i < words.Length; i++)
			{
				words[i] = 0;
			}
		}

		public void ReplaceWith(BitSet incoming)
		{
			Debug.Assert(words.Length == incoming.words.Length);
			Array.Copy(incoming.words, 0, words, 0, words.Length);
		}

		public override string ToString()
		{
			StringBuilder b = new StringBuilder();
			b.Append('{');
			for (int i = 0; i < words.Length * BitsPerWord; i++)
			{
				if (this[i])
				{
					if (b.Length > 1)
						b.Append(", ");
					if (b.Length > 500)
					{
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
