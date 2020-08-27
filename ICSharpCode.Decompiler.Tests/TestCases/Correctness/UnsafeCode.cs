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
	public class UnsafeCode
	{
		private struct SimpleStruct
		{
			public int X;
			public double Y;
		}

		static void Main()
		{
			// TODO: test behavior, or convert this into a pretty-test
			// (but for now, it's already valuable knowing whether the decompiled code can be re-compiled)
		}

		public unsafe int MultipleExitsOutOfFixedBlock(int[] arr)
		{
			fixed (int* ptr = &arr[0])
			{
				if (*ptr < 0)
					return *ptr;
				if (*ptr == 21)
					return 42;
				if (*ptr == 42)
					goto outside;
			}
			return 1;
			outside:
			Console.WriteLine("outside");
			return 2;
		}

		public unsafe void FixMultipleStrings(string text)
		{
			fixed (char* ptr = text, userName = Environment.UserName, ptr2 = text)
			{
				*ptr = 'c';
				*userName = 'd';
				*ptr2 = 'e';
			}
		}

		public unsafe byte* PointerArithmetic2(long* p, int y, int x)
		{
			return (byte*)((short*)p + (y * x));
		}
	}
}