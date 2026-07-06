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

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal static class FirstClassSpanConversions
	{
		internal class Base
		{
		}

		internal class Derived : Base
		{
		}

		public static void AcceptReadOnlySpanChar(ReadOnlySpan<char> s)
		{
		}

		public static void AcceptReadOnlySpanBase(ReadOnlySpan<Base> s)
		{
		}

		public static void AcceptInReadOnlySpan(in ReadOnlySpan<int> s)
		{
		}

		public static void ObjectOrReadOnlySpanChar(object a)
		{
		}

		public static void ObjectOrReadOnlySpanChar(ReadOnlySpan<char> a)
		{
		}

		public static void StringArgument(string s)
		{
			AcceptReadOnlySpanChar(s);
			ObjectOrReadOnlySpanChar(s);
			s.ExtensionOnReadOnlySpanChar();
		}

		public static ReadOnlySpan<char> StringToReadOnlySpanCharReturn(string s)
		{
			return s;
		}

		public static int StringToReadOnlySpanCharLocal(string s)
		{
			ReadOnlySpan<char> readOnlySpan = s;
			return readOnlySpan.Length;
		}

		public static void VarianceReadOnlySpan(ReadOnlySpan<Derived> s)
		{
			AcceptReadOnlySpanBase(s);
		}

		public static void VarianceSpan(Span<Derived> s)
		{
			AcceptReadOnlySpanBase(s);
		}

		public static ReadOnlySpan<Base> VarianceReturn(ReadOnlySpan<Derived> s)
		{
			return s;
		}

		public static void CovariantArrayToReadOnlySpan(Derived[] a)
		{
			AcceptReadOnlySpanBase(a);
		}

		public static void InArgument(int[] a)
		{
			AcceptInReadOnlySpan(a);
		}
	}

	internal static class FirstClassSpanConversionsExtensions
	{
		public static void ExtensionOnReadOnlySpanChar(this ReadOnlySpan<char> s)
		{
		}
	}
}
