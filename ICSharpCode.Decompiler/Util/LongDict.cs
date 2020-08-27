// Copyright (c) 2017 Daniel Grunwald
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
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Util
{
	static class LongDict
	{
		public static LongDict<T> Create<T>(IEnumerable<(LongSet, T)> entries)
		{
			return new LongDict<T>(entries);
		}

		internal static readonly KeyComparer<LongInterval, long> StartComparer = KeyComparer.Create((LongInterval i) => i.Start);
	}

	/// <summary>
	/// An immutable mapping from keys of type long to values of type T.
	/// </summary>
	struct LongDict<T> : IEnumerable<KeyValuePair<LongInterval, T>>
	{
		readonly LongInterval[] keys;
		readonly T[] values;

		/// <summary>
		/// Creates a new LongDict from the given entries.
		/// If there are multiple entries for the same long key,
		/// the resulting LongDict will store the value from the first entry.
		/// </summary>
		public LongDict(IEnumerable<(LongSet, T)> entries)
		{
			LongSet available = LongSet.Universe;
			var keys = new List<LongInterval>();
			var values = new List<T>();
			foreach (var (key, val) in entries)
			{
				foreach (var interval in key.IntersectWith(available).Intervals)
				{
					keys.Add(interval);
					values.Add(val);
				}
				available = available.ExceptWith(key);
			}
			this.keys = keys.ToArray();
			this.values = values.ToArray();
			Array.Sort(this.keys, this.values, LongDict.StartComparer);
		}

		public bool TryGetValue(long key, out T value)
		{
			int pos = Array.BinarySearch(this.keys, new LongInterval(key, key), LongDict.StartComparer);
			// If the element isn't found, BinarySearch returns the complement of "insertion position".
			// We use this to find the previous element (if there wasn't any exact match).
			if (pos < 0)
				pos = ~pos - 1;
			if (pos >= 0 && this.keys[pos].Contains(key))
			{
				value = this.values[pos];
				return true;
			}
			value = default(T);
			return false;
		}

		public T GetOrDefault(long key)
		{
			TryGetValue(key, out T val);
			return val;
		}

		public IEnumerator<KeyValuePair<LongInterval, T>> GetEnumerator()
		{
			for (int i = 0; i < this.keys.Length; ++i)
			{
				yield return new KeyValuePair<LongInterval, T>(this.keys[i], this.values[i]);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
