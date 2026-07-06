using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public static class ParamsCollections
	{
		public delegate void ParamsEnumerableDelegate(params IEnumerable<int> values);

		public delegate void ParamsSpanDelegate(params ReadOnlySpan<int> values);

		public class Container
		{
			public int this[params ReadOnlySpan<int> values] => values.Length;

			public Container(params ReadOnlySpan<string> values)
			{
			}
		}

		public static void ParamsEnumerable(params IEnumerable<int> values)
		{
		}
		public static void ParamsList(params List<int> values)
		{
		}
		public static void ParamsReadOnlySpan(params ReadOnlySpan<int> values)
		{
		}
		public static void ParamsSpan(params Span<int> values)
		{
			// note: implicitly "scoped", "params scoped Span<int> values" is allowed
			// but "scoped" is always redundant for params.
		}
		public static void ParamUnscopedSpan([UnscopedRef] params Span<int> values)
		{
		}
		public static void ParamsICollection(params ICollection<int> values)
		{
		}
		public static void ParamsIList(params IList<int> values)
		{
		}
		public static void ParamsIReadOnlyCollection(params IReadOnlyCollection<int> values)
		{
		}
		public static void ParamsIReadOnlyList(params IReadOnlyList<int> values)
		{
		}
		public static void ParamsImmutableArray(params ImmutableArray<int> values)
		{
		}
		public static void ParamsGenericEnumerable<T>(params IEnumerable<T> values)
		{
		}
		public static void ParamsGenericSpan<T>(params ReadOnlySpan<T> values)
		{
		}
		public static void PreferSpanOverArray(params int[] values)
		{
		}
		public static void PreferSpanOverArray(params ReadOnlySpan<int> values)
		{
		}
		public static void GreenCallSites(ParamsSpanDelegate d)
		{
			ParamsEnumerable();
			ParamsReadOnlySpan(1);
			ParamsReadOnlySpan(1, 2, 3);
			ParamsGenericSpan<int>(1, 2);
			// resolves to the ReadOnlySpan overload (better by the C# 13 betterness rules)
			PreferSpanOverArray(1, 2);
			// resolves to the array overload (identity conversion in normal form)
			PreferSpanOverArray(new int[2] { 1, 2 });
			d(1, 2, 3);
		}
	}
}
