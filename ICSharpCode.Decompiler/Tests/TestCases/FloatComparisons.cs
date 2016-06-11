// Copyright (c) 2016 Daniel Grunwald
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

namespace ICSharpCode.Decompiler.Tests.TestCases
{
	public class FloatComparisons
	{
		public static int Main()
		{
			// disable CompareOfFloatsByEqualityOperator
			TestFloatOp("==", (a, b) => a == b);
			TestFloatOp("!=", (a, b) => a != b);
			TestFloatOp("<", (a, b) => a < b);
			TestFloatOp(">", (a, b) => a > b);
			TestFloatOp("<=", (a, b) => a <= b);
			TestFloatOp(">=", (a, b) => a >= b);
			TestFloatOp("!<", (a, b) => !(a < b));
			TestFloatOp("!>", (a, b) => !(a > b));
			TestFloatOp("!<=", (a, b) => !(a <= b));
			TestFloatOp("!>=", (a, b) => !(a >= b));
			return 0;
		}
		
		static void TestFloatOp(string name, Func<float, float, bool> f)
		{
			float[] vals = { -1, 0, 3, float.PositiveInfinity, float.NaN };
			for (int i = 0; i < vals.Length; i++) {
				for (int j = 0; j < vals.Length; j++) {
					Console.WriteLine("{0:r} {1} {2:r} = {3}", vals[i], name, vals[j], f(vals[i], vals[j]));
				}
			}
		}
	}
}
