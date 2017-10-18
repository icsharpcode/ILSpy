using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.Decompiler.Util
{
	static class CollectionExtensions
	{
		public static void Deconstruct<K, V>(this KeyValuePair<K, V> pair, out K key, out V value)
		{
			key = pair.Key;
			value = pair.Value;
		}

		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> input)
		{
			return new HashSet<T>(input);
		}

		public static IEnumerable<T> SkipLast<T>(this IReadOnlyCollection<T> input, int count)
		{
			return input.Take(input.Count - count);
		}
		
		public static T PopOrDefault<T>(this Stack<T> stack)
		{
			if (stack.Count == 0)
				return default(T);
			return stack.Pop();
		}
		
		public static T PeekOrDefault<T>(this Stack<T> stack)
		{
			if (stack.Count == 0)
				return default(T);
			return stack.Peek();
		}
		
		public static int MaxOrDefault<T>(this IEnumerable<T> input, Func<T, int> selector, int defaultValue = 0)
		{
			int max = defaultValue;
			foreach (var element in input) {
				int value = selector(element);
				if (value > max)
					max = value;
			}
			return max;
		}
		
		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> input)
		{
			foreach (T item in input)
				collection.Add(item);
		}
		
		/// <summary>
		/// Equivalent to <code>collection.Select(func).ToArray()</code>, but more efficient as it makes
		/// use of the input collection's known size.
		/// </summary>
		public static U[] SelectArray<T, U>(this ICollection<T> collection, Func<T, U> func)
		{
			U[] result = new U[collection.Count];
			int index = 0;
			foreach (var element in collection) {
				result[index++] = func(element);
			}
			return result;
		}

		/// <summary>
		/// Equivalent to <code>collection.Select(func).ToList()</code>, but more efficient as it makes
		/// use of the input collection's known size.
		/// </summary>
		public static List<U> SelectList<T, U>(this ICollection<T> collection, Func<T, U> func)
		{
			List<U> result = new List<U>(collection.Count);
			foreach (var element in collection) {
				result.Add(func(element));
			}
			return result;
		}

		public static IEnumerable<U> SelectWithIndex<T, U>(this IEnumerable<T> source, Func<int, T, U> func)
		{
			int index = 0;
			foreach	(var element in source)
				yield return func(index++, element);
		}
		
		/// <summary>
		/// The merge step of merge sort.
		/// </summary>
		public static IEnumerable<T> Merge<T>(this IEnumerable<T> input1, IEnumerable<T> input2, Comparison<T> comparison)
		{
			var enumA = input1.GetEnumerator();
			var enumB = input2.GetEnumerator();
			bool moreA = enumA.MoveNext();
			bool moreB = enumB.MoveNext();
			while (moreA && moreB) {
				if (comparison(enumA.Current, enumB.Current) <= 0) {
					yield return enumA.Current;
					moreA = enumA.MoveNext();
				} else {
					yield return enumB.Current;
					moreB = enumB.MoveNext();
				}
			}
			while (moreA) {
				yield return enumA.Current;
				moreA = enumA.MoveNext();
			}
			while (moreB) {
				yield return enumB.Current;
				moreB = enumB.MoveNext();
			}
		}

		/// <summary>
		/// Returns the minimum element.
		/// </summary>
		/// <exception cref="InvalidOperationException">The input sequence is empty</exception>
		public static T MinBy<T, K>(this IEnumerable<T> source, Func<T, K> keySelector) where K : IComparable<K>
		{
			return source.MinBy(keySelector, Comparer<K>.Default);
		}

		/// <summary>
		/// Returns the minimum element.
		/// </summary>
		/// <exception cref="InvalidOperationException">The input sequence is empty</exception>
		public static T MinBy<T, K>(this IEnumerable<T> source, Func<T, K> keySelector, IComparer<K> keyComparer)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			if (keySelector == null)
				throw new ArgumentNullException(nameof(keySelector));
			if (keyComparer == null)
				keyComparer = Comparer<K>.Default;
			using (var enumerator = source.GetEnumerator()) {
				if (!enumerator.MoveNext())
					throw new InvalidOperationException("Sequence contains no elements");
				T minElement = enumerator.Current;
				K minKey = keySelector(minElement);
				while (enumerator.MoveNext()) {
					T element = enumerator.Current;
					K key = keySelector(element);
					if (keyComparer.Compare(key, minKey) < 0) {
						minElement = element;
						minKey = key;
					}
				}
				return minElement;
			}
		}

		/// <summary>
		/// Returns the maximum element.
		/// </summary>
		/// <exception cref="InvalidOperationException">The input sequence is empty</exception>
		public static T MaxBy<T, K>(this IEnumerable<T> source, Func<T, K> keySelector) where K : IComparable<K>
		{
			return source.MaxBy(keySelector, Comparer<K>.Default);
		}

		/// <summary>
		/// Returns the maximum element.
		/// </summary>
		/// <exception cref="InvalidOperationException">The input sequence is empty</exception>
		public static T MaxBy<T, K>(this IEnumerable<T> source, Func<T, K> keySelector, IComparer<K> keyComparer)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			if (keySelector == null)
				throw new ArgumentNullException(nameof(keySelector));
			if (keyComparer == null)
				keyComparer = Comparer<K>.Default;
			using (var enumerator = source.GetEnumerator()) {
				if (!enumerator.MoveNext())
					throw new InvalidOperationException("Sequence contains no elements");
				T maxElement = enumerator.Current;
				K maxKey = keySelector(maxElement);
				while (enumerator.MoveNext()) {
					T element = enumerator.Current;
					K key = keySelector(element);
					if (keyComparer.Compare(key, maxKey) > 0) {
						maxElement = element;
						maxKey = key;
					}
				}
				return maxElement;
			}
		}

		public static void RemoveLast<T>(this IList<T> list)
		{
			if (list == null)
				throw new ArgumentNullException(nameof(list));
			list.RemoveAt(list.Count - 1);
		}
	}
}
