using System;
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	// Pins the current (lowered) decompiler output for C# 11 list patterns.
	// The decompiler does not reconstruct list patterns yet (see
	// https://github.com/icsharpcode/ILSpy/issues/829); it prints the raw
	// Length/Count checks and indexer accesses the compiler generated. That
	// output is valid C# with identical semantics, which this fixture pins:
	// the EXPECTED_OUTPUT sides mirror today's decompilation of the list
	// patterns in the corresponding compile-only sides. When list-pattern
	// support is implemented, the EXPECTED_OUTPUT branches should collapse
	// into the pattern form and ListPatterns.cs takes over as the spec.
	public class ListPatternsLowered
	{
		public bool ArrayConstantPattern(int[] a)
		{
#if EXPECTED_OUTPUT
#if OPT
			if (a != null && a.Length == 3 && a[0] == 1 && a[1] == 2)
			{
				return a[2] == 3;
			}
			return false;
#else
			return a != null && a.Length == 3 && a[0] == 1 && a[1] == 2 && a[2] == 3;
#endif
#else
			return a is [1, 2, 3];
#endif
		}

		public bool ArrayEmptyPattern(int[] a)
		{
#if EXPECTED_OUTPUT
#if OPT
			if (a != null)
			{
				return a.Length == 0;
			}
			return false;
#else
			return a != null && a.Length == 0;
#endif
#else
			return a is [];
#endif
		}

		public bool ArraySliceCapturePattern(int[] a)
		{
#if EXPECTED_OUTPUT
#if OPT
			if (a != null && a.Length >= 1)
			{
				int num = a[0];
				int[] subArray = a[new Index(1)..^0];
				return num > subArray.Length;
			}
			return false;
#else
			int result;
			if (a != null && a.Length >= 1)
			{
				int num = a[0];
				int[] subArray = a[new Index(1)..^0];
				result = ((num > subArray.Length) ? 1 : 0);
			}
			else
			{
				result = 0;
			}
			return (byte)result != 0;
#endif
#else
			return a is [var first, .. var rest] && first > rest.Length;
#endif
		}

		public bool ListConstantPattern(List<int> l)
		{
#if EXPECTED_OUTPUT
#if OPT
			if (l != null && l.Count == 2 && l[0] == 1)
			{
				return l[1] == 2;
			}
			return false;
#else
			return l != null && l.Count == 2 && l[0] == 1 && l[1] == 2;
#endif
#else
			return l is [1, 2];
#endif
		}

		public bool StringConstantPattern(string s)
		{
#if EXPECTED_OUTPUT
#if OPT
			if (s != null)
			{
				int length = s.Length;
				if (length >= 2 && s[0] == 'a')
				{
					return s[length - 1] == 'z';
				}
			}
			return false;
#else
			int result;
			if (s != null)
			{
				int length = s.Length;
				if (length >= 2 && s[0] == 'a')
				{
					result = ((s[length - 1] == 'z') ? 1 : 0);
					goto IL_002a;
				}
			}
			result = 0;
			goto IL_002a;
			IL_002a:
			return (byte)result != 0;
#endif
#else
			return s is ['a', .., 'z'];
#endif
		}

		public bool ReadOnlySpanConstantPattern(ReadOnlySpan<char> s)
		{
#if EXPECTED_OUTPUT
#if OPT
			if (s.Length == 2 && s[0] == 'x')
			{
				return s[1] == 'y';
			}
			return false;
#else
			return s.Length == 2 && s[0] == 'x' && s[1] == 'y';
#endif
#else
			return s is ['x', 'y'];
#endif
		}

		public bool SpanSliceCapturePattern(ReadOnlySpan<int> s)
		{
#if EXPECTED_OUTPUT
#if OPT
			int length = s.Length;
			if (length >= 1 && s[0] == 1)
			{
				return s.Slice(1, length - 1).Length > 1;
			}
			return false;
#else
			int length = s.Length;
			return length >= 1 && s[0] == 1 && s.Slice(1, length - 1).Length > 1;
#endif
#else
			return s is [1, .. var rest] && rest.Length > 1;
#endif
		}

		public bool CombinedLengthPattern(int[] a)
		{
#if EXPECTED_OUTPUT
#if OPT
			if (a != null && a.Length > 2)
			{
				return a[0] == 1;
			}
			return false;
#else
			return a != null && a.Length > 2 && a[0] == 1;
#endif
#else
			return a is { Length: > 2 } and [1, ..];
#endif
		}
	}
}
