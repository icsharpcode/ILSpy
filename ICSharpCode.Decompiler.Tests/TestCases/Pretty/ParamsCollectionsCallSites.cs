using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class ParamsCollectionsCallSites
	{
		public delegate void ParamsEnumerableDelegate(params IEnumerable<int> values);

		public class ParamsAttribute : Attribute
		{
			public ParamsAttribute(params int[] values)
			{
			}
		}

		public int this[params ReadOnlySpan<int> values] => values.Length;

		public ParamsCollectionsCallSites(params ReadOnlySpan<string> values)
		{
		}

		public static void ParamsEnumerable(params IEnumerable<int> values)
		{
		}
		public static void ParamsGenericEnumerable<T>(params IEnumerable<T> values)
		{
		}
		public static void ParamsGenericSpan<T>(params ReadOnlySpan<T> values)
		{
		}
		public static void ParamsICollection(params ICollection<int> values)
		{
		}
		public static void ParamsIList(params IList<int> values)
		{
		}
		public static void ParamsIReadOnlyList(params IReadOnlyList<int> values)
		{
		}
		public static void ParamsImmutableArray(params ImmutableArray<int> values)
		{
		}
		public static void ParamsList(params List<int> values)
		{
		}
		public static void ParamsReadOnlySpan(params ReadOnlySpan<int> values)
		{
		}
		public static void ParamsSpanOfString(params ReadOnlySpan<string> values)
		{
		}
		public static void SpanCallSites(int x)
		{
			ParamsReadOnlySpan();
			ParamsReadOnlySpan(x);
			ParamsReadOnlySpan(x, x + 1);
			ParamsSpanOfString("a", "b");
			ParamsGenericSpan("a", "b");
		}
		public static void EnumerableCallSites(int x)
		{
			ParamsEnumerable(1);
			ParamsEnumerable(1, 2, 3);
			ParamsEnumerable(x, x + 1);
			ParamsIReadOnlyList(1, 2);
			ParamsGenericEnumerable(1.0, 2.0);
		}
		public static void ListCallSites(int x)
		{
			ParamsList();
			ParamsList(1, 2, 3);
			ParamsIList(1, 2);
			ParamsICollection(x);
		}
		public static void ImmutableArrayCallSites(int x)
		{
			ParamsImmutableArray();
			ParamsImmutableArray(1, 2, 3);
			ParamsImmutableArray(x, x + 1);
		}
		public static int MemberCallSites(int x)
		{
			return new ParamsCollectionsCallSites("a", "b")[1, 2, x];
		}
		public static void DelegateCallSites(ParamsEnumerableDelegate d)
		{
			d(4, 5);
		}
		[Params(1, 2, 3)]
		public static void AttributeCallSite()
		{
		}
		public static void LocalFunctionCallSites()
		{
			LocalArray();
			LocalArray(1, 2, 3);
			LocalSpan(1, 2);
			static void LocalArray(params int[] values)
			{
			}
			static void LocalSpan(params ReadOnlySpan<int> values)
			{
			}
		}
		public static int NaturalTypeLambdas()
		{
			var f = (params ReadOnlySpan<int> xs) => xs.Length;
			var g = (params IEnumerable<int> xs) => 0;
			return f(1, 2) + g(3);
		}
	}
}
