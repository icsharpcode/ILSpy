// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
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

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	internal class UncheckedConstants
	{
		private static void Main()
		{
			Console.WriteLine(CombineHash(1, 2));
			Console.WriteLine(CombineHash(-5, 100));
			Console.WriteLine(CombineHash(int.MaxValue, int.MinValue));
		}

		// A compiler-generated-style hash using a local accumulator. After the decompiler inlines the
		// accumulator, the leading "1688038063 * -1521134295" becomes a compile-time constant
		// subexpression that overflows int. Constant subexpressions are always overflow-checked at
		// compile time, so the decompiled output must wrap it in unchecked(...) or it fails to compile
		// with CS0220 ("operation overflows at compile time in checked mode").
		private static int CombineHash(int a, int b)
		{
			int num = 1688038063;
			num = num * -1521134295 + a;
			num = num * -1521134295 + b;
			return num;
		}
	}
}
