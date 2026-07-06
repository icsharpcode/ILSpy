using System;
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class ListPatterns
	{
		public class Container
		{
			public int[] Items { get; set; }
		}

		public class CustomCollection
		{
			public int Count { get; set; }

			public char this[int index] => 'a';

			public CustomCollection Slice(int offset, int count)
			{
				return this;
			}
		}

		public bool ArrayConstants(int[] a)
		{
			return a is [1, 2, 3];
		}

		public bool ArrayEmpty(int[] a)
		{
			return a is [];
		}

		public bool ArrayNotEmpty(int[] a)
		{
			return a is not [];
		}

		public bool ArrayDiscardAndTrailingSlice(int[] a)
		{
			return a is [_, .., 5];
		}

		public bool ArrayVarElements(int[] a)
		{
			return a is [var first, _, var last] && first > last;
		}

		public bool ArraySliceCapture(int[] a)
		{
			return a is [var first, .. var rest] && first > rest.Length;
		}

		public bool ArrayTypedSliceCapture(int[] a)
		{
			return a is [1, .. int[] middle, 2] && middle.Length > 0;
		}

		public bool ArrayWholeSliceCapture(int[] a)
		{
			return a is [.. var everything] && everything.Length % 2 == 0;
		}

		public bool ArrayRelationalElements(int[] a)
		{
			return a is [> 0, <= 10 or 42, _];
		}

		public bool ArrayPropertyPatternElement(string[] a)
		{
			return a is [{ Length: 2 }, .. var rest] && rest.Length > 0;
		}

		public bool ArrayTypePatternElements(object[] a)
		{
			return a is [string text, int num] && text.Length == num;
		}

		public bool NestedListPatterns(int[][] a)
		{
			return a is [[1], [2, ..], []];
		}

		public bool GenericArray<T>(T[] a)
		{
			return a is [_, .. var rest] && rest.Length > 0;
		}

		public bool CombinedWithPropertyPattern(int[] a)
		{
			return a is { Length: > 2 } and [1, ..];
		}

		public bool OrOfListPatterns(int[] a)
		{
			return a is [< 0, ..] or [.., > 100];
		}

		public bool ListPatternInPropertyPattern(Container c)
		{
			return c is { Items: [1, ..] };
		}

		public bool ListConstants(List<int> l)
		{
			return l is [1, 2];
		}

		public bool ListDiscardAndTrailingSlice(List<int> l)
		{
			return l is [_, .., 3];
		}

		public bool NestedLists(List<List<int>> l)
		{
			return l is [[1], [2]];
		}

		public bool StringConstants(string s)
		{
			return s is ['a', .., 'z'];
		}

		public bool StringVarElement(string s)
		{
			return s is [var c, ..] && char.IsUpper(c);
		}

		public bool StringSliceCapture(string s)
		{
			return s is ['(', .. var inner, ')'] && inner.Length > 1;
		}

		public bool StringRelationalElement(string s)
		{
			return s is [>= 'a' and <= 'z', ..];
		}

		public bool ReadOnlySpanConstants(ReadOnlySpan<char> s)
		{
			return s is ['x', 'y'];
		}

		public bool SpanSliceCapture(Span<int> s)
		{
			return s is [1, .. var rest] && rest.Length > 1;
		}

		public bool CustomCollectionPattern(CustomCollection c)
		{
			return c is ['a', .. var rest] && rest.Count == 0;
		}

		public string SwitchOverArray(int[] a)
		{
			return a switch {
				null => "null",
				[] => "empty",
				[var single] => $"single({single})",
				[1, 2, ..] => "starts with 1, 2",
				[.., var last] => $"ends with {last}",
			};
		}

#pragma warning disable CS8509 // switch expression is deliberately not exhaustive
		public string NonExhaustiveSwitchOverArray(int[] a)
		{
			return a switch {
				[] => "empty",
				[1, ..] => "starts with 1",
			};
		}
#pragma warning restore CS8509

		public string SwitchOverJaggedArray(int[][] a)
		{
			return a switch {
				[[1], [2]] => "[[1], [2]]",
				[[], ..] => "starts with empty array",
				_ => "other",
			};
		}

		public int SwitchOverString(string s)
		{
			switch (s)
			{
				case ['a', ..]:
					return 1;
				case [.., 'b']:
					return 2;
				case ['x', 'y', 'z']:
					return 3;
				default:
					return 0;
			}
		}
	}
}
