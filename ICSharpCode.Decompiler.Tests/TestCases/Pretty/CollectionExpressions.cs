using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	[CollectionBuilder(typeof(BuilderCollectionFactory), "Create")]
	public sealed class BuilderCollection : IEnumerable<int>, IEnumerable
	{
		private readonly int[] items;

		public BuilderCollection(int[] items)
		{
			this.items = items;
		}

		public IEnumerator<int> GetEnumerator()
		{
			return ((IEnumerable<int>)items).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return items.GetEnumerator();
		}
	}

	public static class BuilderCollectionFactory
	{
		public static BuilderCollection Create(ReadOnlySpan<int> items)
		{
			return new BuilderCollection(items.ToArray());
		}
	}

	public static class CollectionExpressions
	{
		public static int[] EmptyArray()
		{
			return [];
		}

		public static int[] ArrayElements()
		{
			return [1, 2, 3];
		}

		public static int[] ArraySpread(IEnumerable<int> items)
		{
			return [0, .. items, 4];
		}

		public static List<int> EmptyList()
		{
			return [];
		}

		public static List<int> ListElements()
		{
			return [1, 2, 3];
		}

		public static List<int> ListSpread(IEnumerable<int> first, int[] second)
		{
			return [0, .. first, 1, .. second, 2];
		}

		public static IList<int> InterfaceElements()
		{
			return [1, 2, 3];
		}

		public static IEnumerable<int> EnumerableElements()
		{
			return [1, 2, 3];
		}

		public static int SpanElements()
		{
#if EXPECTED_OUTPUT
#if ROSLYN5
			Span<int> inlineArray = [1, 2, 3];
			return inlineArray[0];
#else
			Span<int> obj = [1, 2, 3];
			return obj[0];
#endif
#else
			Span<int> span = [1, 2, 3];
			return span[0];
#endif
		}

		public static int ReadOnlySpanElements()
		{
#if EXPECTED_OUTPUT
			return ((ReadOnlySpan<int>)[1, 2, 3])[0];
#else
			ReadOnlySpan<int> span = [1, 2, 3];
			return span[0];
#endif
		}

		public static BuilderCollection BuilderElements()
		{
			return [1, 2, 3];
		}

		public static BuilderCollection BuilderSpread(IEnumerable<int> items)
		{
			return [0, .. items, 4];
		}

		public static int[,] MultiDimensionalArray()
		{
			return new int[2, 3] {
				{ 0, 1, 2 },
				{ 3, 4, 5 }
			};
		}

		public static List<object> RecursiveList()
		{
			List<object> list = [];
			list.Add(list);
			return list;
		}
	}
}
