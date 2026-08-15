// Copyright (c) 2026 Siegfried Pammer
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
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal static class FirstClassSpanTypes
	{
		public static void ArrayOrReadOnlySpan(int[] a)
		{
		}

		public static void ArrayOrReadOnlySpan(ReadOnlySpan<int> a)
		{
		}

		public static void ArrayOrSpan(int[] a)
		{
		}

		public static void ArrayOrSpan(Span<int> a)
		{
		}

		public static void SpanOrReadOnlySpan(Span<int> a)
		{
		}

		public static void SpanOrReadOnlySpan(ReadOnlySpan<int> a)
		{
		}

		public static void ObjectOrReadOnlySpan(object a)
		{
		}

		public static void ObjectOrReadOnlySpan(ReadOnlySpan<int> a)
		{
		}

		public static void ObjectOrReadOnlySpanChar(object a)
		{
		}

		public static void ObjectOrReadOnlySpanChar(ReadOnlySpan<char> a)
		{
		}

		public static void EnumerableOrReadOnlySpan(IEnumerable<int> a)
		{
		}

		public static void EnumerableOrReadOnlySpan(ReadOnlySpan<int> a)
		{
		}

		public static void CovariantArrayOrReadOnlySpan(object[] a)
		{
		}

		public static void CovariantArrayOrReadOnlySpan(ReadOnlySpan<string> a)
		{
		}

		public static void ReadOnlySpanOfObjectOrString(ReadOnlySpan<object> a)
		{
		}

		public static void ReadOnlySpanOfObjectOrString(ReadOnlySpan<string> a)
		{
		}

		public static void StringOrReadOnlySpanChar(string a)
		{
		}

		public static void StringOrReadOnlySpanChar(ReadOnlySpan<char> a)
		{
		}

		public static void ParamsArrayOrParamsReadOnlySpan(params int[] a)
		{
		}

		public static void ParamsArrayOrParamsReadOnlySpan(params ReadOnlySpan<int> a)
		{
		}

		public static void RefSpanOrByValue(ref ReadOnlySpan<int> s)
		{
		}

		public static void RefSpanOrByValue(ReadOnlySpan<int> s)
		{
		}

		public static void OutSpanOrByValue(out ReadOnlySpan<int> s)
		{
			s = default(ReadOnlySpan<int>);
		}

		public static void OutSpanOrByValue(ReadOnlySpan<int> s)
		{
		}

		public static void GenericArrayOrReadOnlySpan<T>(T[] a)
		{
		}

		public static void GenericArrayOrReadOnlySpan<T>(ReadOnlySpan<T> a)
		{
		}

		public static void InferFromReadOnlySpan<T>(ReadOnlySpan<T> a)
		{
		}

		public static ReadOnlySpan<int> ArrayToReadOnlySpanReturn(int[] a)
		{
			return a;
		}

		public static ReadOnlySpan<int> TernaryArrayOrSpan(bool b, int[] a, Span<int> s)
		{
#if OPT
			if (!b)
			{
				return s;
			}
			return a;
#else
			return b ? ((ReadOnlySpan<int>)a) : ((ReadOnlySpan<int>)s);
#endif
		}

		public static bool SpanExtensionContains(int[] a)
		{
			// binds to MemoryExtensions.Contains under C# 14 first-class span conversions
			return a.Contains(2);
		}

		public static bool LinqContains(int[] a)
		{
			// Enumerable.Contains loses against MemoryExtensions.Contains under C# 14;
			// extension method syntax must not be used here
			return Enumerable.Contains(a, 2);
		}

		public static void CallWinners(int[] arr, Span<int> span, string str, string[] strArr)
		{
			ArrayOrReadOnlySpan(arr);
			ArrayOrSpan(arr);
			SpanOrReadOnlySpan(arr);
			SpanOrReadOnlySpan(span);
			ObjectOrReadOnlySpan(arr);
			EnumerableOrReadOnlySpan(arr);
			CovariantArrayOrReadOnlySpan(strArr);
			ReadOnlySpanOfObjectOrString(strArr);
			StringOrReadOnlySpanChar(str);
			ParamsArrayOrParamsReadOnlySpan(arr);
			ParamsArrayOrParamsReadOnlySpan(1, 2, 3);
			GenericArrayOrReadOnlySpan(arr);
			InferFromReadOnlySpan(arr);
			InferFromReadOnlySpan(span);
			arr.ExtensionOnReadOnlySpan();
		}

		public static void CallRefOutOrByValue(int[] arr)
		{
			// A span conversion never binds a ref or out parameter: without the keyword the
			// by-value overload wins, with the keyword only the ref/out overload is
			// applicable and the keyword must survive decompilation.
			ReadOnlySpan<int> s = arr;
			RefSpanOrByValue(arr);
			RefSpanOrByValue(ref s);
			OutSpanOrByValue(arr);
			OutSpanOrByValue(out s);
		}

		public static void CallLosersWithExplicitConversions(int[] arr, string str)
		{
			ArrayOrReadOnlySpan((ReadOnlySpan<int>)arr);
			ArrayOrSpan((Span<int>)arr);
			SpanOrReadOnlySpan((Span<int>)arr);
			ObjectOrReadOnlySpan((object)arr);
			ObjectOrReadOnlySpanChar((object)str);
			EnumerableOrReadOnlySpan((IEnumerable<int>)arr);
			StringOrReadOnlySpanChar((ReadOnlySpan<char>)str);
		}
	}

	internal static class FirstClassSpanTypesExtensions
	{
		public static void ExtensionOnReadOnlySpan<T>(this ReadOnlySpan<T> s)
		{
		}
	}
}
