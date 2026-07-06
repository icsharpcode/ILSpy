using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class CollectionExpressions
	{
		[CollectionBuilder(typeof(CustomCollectionBuilder), "Create")]
		public sealed class CustomCollection<T> : IEnumerable<T>
		{
			private readonly T[] items;

			public CustomCollection(T[] items)
			{
				this.items = items;
			}

			public IEnumerator<T> GetEnumerator()
			{
				return ((IEnumerable<T>)items).GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return items.GetEnumerator();
			}
		}

		public static class CustomCollectionBuilder
		{
			public static CustomCollection<T> Create<T>(ReadOnlySpan<T> items)
			{
				return new CustomCollection<T>(items.ToArray());
			}
		}

		public sealed class NonGenericAddCollection : IEnumerable
		{
			private readonly List<int> items = new List<int>();

			public void Add(int item)
			{
				items.Add(item);
			}

			public IEnumerator GetEnumerator()
			{
				return items.GetEnumerator();
			}
		}

		public sealed class GenericAddCollection<T> : IEnumerable<T>
		{
			private readonly List<T> items = new List<T>();

			public void Add(T item)
			{
				items.Add(item);
			}

			public IEnumerator<T> GetEnumerator()
			{
				return items.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return items.GetEnumerator();
			}
		}

		public sealed class ExtensionAddCollection : IEnumerable<string>
		{
			internal readonly List<string> Items = new List<string>();

			public IEnumerator<string> GetEnumerator()
			{
				return Items.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return Items.GetEnumerator();
			}
		}

		private static readonly int[] StaticArrayField = [1, 2, 3];

		private readonly ImmutableArray<string> immutableArrayField = ["a", "b"];

		public int[] ArrayOfInts()
		{
			return [1, 2, 3, 4, 5];
		}

		public int[] EmptyArray()
		{
			return [];
		}

		public string[] ArrayOfStrings()
		{
			return ["hello", "world", null];
		}

		public byte[] ArrayOfBytes()
		{
			return [1, 2, 3, 255];
		}

		public long[] ArrayWithNumericConversions()
		{
			return [1, 2L, 3];
		}

		public double[] ArrayWithMixedNumericElements(float f)
		{
			return [1.5, f, 2f, 3];
		}

		public int?[] ArrayOfNullableInts()
		{
			return [1, null, 3];
		}

		public object[] ArrayOfObjects(string s, int i)
		{
			return [s, i, null];
		}

		public decimal[] ArrayOfDecimals()
		{
			return [1m, 2.5m];
		}

		public int[][] JaggedArray()
		{
			return [[1, 2], [3, 4], []];
		}

		public int[] TernaryArray(bool flag)
		{
			return flag ? [1] : [2, 3];
		}

		public int SpanOfInts()
		{
			Span<int> span = [1, 2, 3];
			return span[1];
		}

		public string SpanOfStrings()
		{
			Span<string> span = ["a", "b"];
			return span[0];
		}

		public byte ReadOnlySpanOfBytes()
		{
			ReadOnlySpan<byte> readOnlySpan = [1, 2, 3, 255];
			return readOnlySpan[0];
		}

		public char ReadOnlySpanOfChars()
		{
			ReadOnlySpan<char> readOnlySpan = ['a', 'b', 'c'];
			return readOnlySpan[2];
		}

		public int EmptySpans()
		{
			Span<int> span = [];
			ReadOnlySpan<int> readOnlySpan = [];
			return span.Length + readOnlySpan.Length;
		}

		public int SpanArgument()
		{
			return Sum([1, 2, 3, 4]);
		}

		public List<int> ListOfInts()
		{
			return [1, 2, 3];
		}

		public List<string> EmptyList()
		{
			return [];
		}

		public List<List<int>> NestedList()
		{
			return [[1], [2, 3], []];
		}

		public IEnumerable<int> EnumerableInterface()
		{
			return [1, 2, 3];
		}

		public IEnumerable<int> EmptyEnumerableInterface()
		{
			return [];
		}

		public IReadOnlyCollection<int> ReadOnlyCollectionInterface()
		{
			return [1, 2, 3];
		}

		public IReadOnlyList<int> ReadOnlyListInterface()
		{
			return [1, 2, 3];
		}

		public ICollection<int> CollectionInterface()
		{
			return [1, 2, 3];
		}

		public IList<int> ListInterface()
		{
			return [1, 2, 3];
		}

		public ImmutableArray<int> ImmutableArrayOfInts()
		{
			return [1, 2, 3];
		}

		public ImmutableArray<int> EmptyImmutableArray()
		{
			return [];
		}

		public ImmutableList<string> ImmutableListOfStrings()
		{
			return ["a", "b"];
		}

		public ImmutableHashSet<int> ImmutableHashSetOfInts()
		{
			return [1, 2, 3];
		}

		public CustomCollection<int> CustomCollectionOfInts()
		{
			return [1, 2, 3];
		}

		public CustomCollection<string> EmptyCustomCollection()
		{
			return [];
		}

		public HashSet<int> HashSetOfInts()
		{
			return [1, 2, 3];
		}

		public NonGenericAddCollection NonGenericAddCollectionOfInts()
		{
			return [1, 2, 3];
		}

		public GenericAddCollection<string> GenericAddCollectionOfStrings()
		{
			return ["a", "b"];
		}

		public ExtensionAddCollection ExtensionAddCollectionOfStrings()
		{
			return ["a", "b"];
		}

		public int[] SpreadTwoArrays(int[] a, int[] b)
		{
			return [.. a, .. b];
		}

		public int[] SpreadWithElements(int[] a)
		{
			return [0, .. a, -1];
		}

		public long[] SpreadWithElementConversion(int[] a)
		{
			return [.. a, 4];
		}

		public int[] SpreadList(List<int> list)
		{
			return [.. list];
		}

		public int[] SpreadEnumerable(IEnumerable<int> source)
		{
			return [.. source, 42];
		}

		public char[] SpreadString(string s)
		{
			return [.. s];
		}

		public List<int> SpreadSpanIntoList(ReadOnlySpan<int> span)
		{
			return [.. span];
		}

		public ImmutableArray<int> SpreadIntoImmutableArray(int[] a, ImmutableArray<int> b)
		{
			return [.. a, .. b];
		}

		public int SpreadIntoSpan(int[] a)
		{
			Span<int> span = [.. a, 4];
			return span.Length;
		}

		public int[][] NestedSpread(int[] a)
		{
			return [[.. a], a, [1]];
		}

		public string PreferReadOnlySpanOverSpan()
		{
			return OverloadROSVsSpan([1, 2, 3]);
		}

		public string PreferReadOnlySpanOverArray()
		{
			return OverloadROSVsArray([1, 2, 3]);
		}

		public string PreferListOverEnumerable()
		{
			return OverloadListVsEnumerable([1, 2, 3]);
		}

		public string PreferBetterArrayElementType()
		{
			return OverloadIntVsLongArray([1, 2, 3]);
		}

		public string PreferBetterSpanElementType()
		{
			return OverloadStringVsObjectROS(["a", "b"]);
		}

		public string PreferParamsSpanOverParamsArray()
		{
			return OverloadParamsSpanVsParamsArray(1, 2, 3);
		}

		public int UseFields()
		{
			return StaticArrayField.Length + immutableArrayField.Length;
		}

		private static int Sum(ReadOnlySpan<int> values)
		{
			int num = 0;
			for (int i = 0; i < values.Length; i++)
			{
				num += values[i];
			}
			return num;
		}

		private static string OverloadROSVsSpan(Span<int> values)
		{
			return "Span";
		}

		private static string OverloadROSVsSpan(ReadOnlySpan<int> values)
		{
			return "ReadOnlySpan";
		}

		private static string OverloadROSVsArray(ReadOnlySpan<int> values)
		{
			return "ReadOnlySpan";
		}

		private static string OverloadROSVsArray(int[] values)
		{
			return "Array";
		}

		private static string OverloadListVsEnumerable(List<int> values)
		{
			return "List";
		}

		private static string OverloadListVsEnumerable(IEnumerable<int> values)
		{
			return "IEnumerable";
		}

		private static string OverloadIntVsLongArray(int[] values)
		{
			return "int[]";
		}

		private static string OverloadIntVsLongArray(long[] values)
		{
			return "long[]";
		}

		private static string OverloadStringVsObjectROS(ReadOnlySpan<string> values)
		{
			return "ReadOnlySpan<string>";
		}

		private static string OverloadStringVsObjectROS(ReadOnlySpan<object> values)
		{
			return "ReadOnlySpan<object>";
		}

		private static string OverloadParamsSpanVsParamsArray(params ReadOnlySpan<int> values)
		{
			return "params ReadOnlySpan";
		}

		private static string OverloadParamsSpanVsParamsArray(params int[] values)
		{
			return "params int[]";
		}
	}

	public static class CollectionExpressionsExtensions
	{
		public static void Add(this CollectionExpressions.ExtensionAddCollection collection, string item)
		{
			collection.Items.Add(item);
		}
	}
}
