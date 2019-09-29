// Copyright (c) 2017 AlphaSierraPapa for the SharpDevelop Team
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
using System.Linq;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	class TrickyTypes
	{
		static void Main()
		{
			InterestingConstants();
			TruncatedComp();
			StringConcat();
			LinqNullableMin();
			LinqNullableMin(1, 2, 3);
		}

		static void Print<T>(T val)
		{
			Console.Write(typeof(T).Name + ": ");
			if (val == null)
				Console.WriteLine("null");
			else
				Console.WriteLine(val);
		}

		static void InterestingConstants()
		{
			long val1 = 2147483648L;
			uint val2 = 2147483648u;
			Console.WriteLine("InterestingConstants:");
			Print(val1);
			Print(2147483648L);
			Print(val2);
			Print(2147483648u);
		}

		static void TruncatedComp()
		{
			Console.WriteLine("TruncatedComp1(1):");
			TruncatedComp1(1);

			Console.WriteLine("TruncatedComp1(-1):");
			TruncatedComp1(-1);
			
			Console.WriteLine("TruncatedComp1(0x100000001):");
			TruncatedComp1(0x100000001);

			Console.WriteLine("TruncatedComp1(long.MinValue):");
			TruncatedComp1(long.MinValue);
			
			Console.WriteLine("TruncatedComp2(1):");
			TruncatedComp2(1, 1);

			Console.WriteLine("TruncatedComp2(-1):");
			TruncatedComp2(-1, -1);

			Console.WriteLine("TruncatedComp2(0x100000001):");
			TruncatedComp2(0x100000001, 1);

			Console.WriteLine("TruncatedComp2(long.MinValue):");
			TruncatedComp2(long.MinValue, int.MinValue);
		}

		static void TruncatedComp1(long val)
		{
			Print((int)val == val);
			Print(val == (int)val);
			Print(val < (int)val);
			Print((int)val >= val);
		}

		static void TruncatedComp2(long val1, int val2)
		{
			Print(val1 == val2);
			Print((int)val1 == val2);
			Print(val1 < val2);
			Print((int)val1 < val2);
			Print(val1 <= val2);
			Print((int)val1 <= val2);
		}

		static void StringConcat()
		{
			// Some string.Concat()-cases that cannot be replaced using operator+
			Print(string.Concat("String concat:"));
			Print(string.Concat(1, 2));
			Print(string.Concat(1, 2, "str"));
		}

		static void LinqNullableMin(params int[] arr)
		{
			Print(string.Format("LinqNullableMin {0}:", arr.Length));
			Print(arr.Min(v => (int?)v));
		}
	}
}
