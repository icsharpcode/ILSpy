// Copyright (c) 2022 Siegfried Pammer
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
using System.Linq;

using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.ILSpyX.Extensions
{
	public static class CollectionExtensions
	{
		public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> items)
		{
			foreach (T item in items)
				if (!list.Contains(item))
					list.Add(item);
		}

		public static T? PeekOrDefault<T>(this Stack<T> stack)
		{
			if (stack.Count == 0)
				return default(T);
			return stack.Peek();
		}

		public static int BinarySearch<T>(this IList<T> list, T item, int start, int count, IComparer<T> comparer)
		{
			if (list == null)
				throw new ArgumentNullException(nameof(list));
			if (start < 0 || start >= list.Count)
				throw new ArgumentOutOfRangeException(nameof(start), start, "Value must be between 0 and " + (list.Count - 1));
			if (count < 0 || count > list.Count - start)
				throw new ArgumentOutOfRangeException(nameof(count), count, "Value must be between 0 and " + (list.Count - start));
			int end = start + count - 1;
			while (start <= end)
			{
				int pivot = (start + end) / 2;
				int result = comparer.Compare(item, list[pivot]);
				if (result == 0)
					return pivot;
				if (result < 0)
					end = pivot - 1;
				else
					start = pivot + 1;
			}
			return ~start;
		}

		public static int BinarySearch<T, TKey>(this IList<T> instance, TKey itemKey, Func<T, TKey> keySelector)
			where TKey : IComparable<TKey>, IComparable
		{
			if (instance == null)
				throw new ArgumentNullException(nameof(instance));
			if (keySelector == null)
				throw new ArgumentNullException(nameof(keySelector));

			int start = 0;
			int end = instance.Count - 1;

			while (start <= end)
			{
				int m = (start + end) / 2;
				TKey key = keySelector(instance[m]);
				int result = key.CompareTo(itemKey);
				if (result == 0)
					return m;
				if (result < 0)
					start = m + 1;
				else
					end = m - 1;
			}
			return ~start;
		}

		public static void InsertSorted<T>(this IList<T> list, T item, IComparer<T> comparer)
		{
			if (list == null)
				throw new ArgumentNullException(nameof(list));
			if (comparer == null)
				throw new ArgumentNullException(nameof(comparer));

			if (list.Count == 0)
			{
				list.Add(item);
			}
			else
			{
				int index = list.BinarySearch(item, 0, list.Count, comparer);
				list.Insert(index < 0 ? ~index : index, item);
			}
		}

		internal static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
		{
			key = pair.Key;
			value = pair.Value;
		}

		internal static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? inst) => inst ?? Enumerable.Empty<T>();
		internal static IEnumerable EmptyIfNull(this IEnumerable? inst) => inst ?? Enumerable.Empty<object>();
		internal static IList<T> EmptyIfNull<T>(this IList<T>? inst) => inst ?? EmptyList<T>.Instance;
		internal static IList EmptyIfNull(this IList? inst) => inst ?? Array.Empty<object>();
	}
}
