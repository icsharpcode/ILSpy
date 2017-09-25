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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	class Loops
	{
		static void Main()
		{
			ForWithMultipleVariables();
			DoubleForEachWithSameVariable(new[] { "a", "b", "c" });
			ForeachExceptForNameCollision(new[] { 42, 43, 44, 45 });
		}

		public static void ForWithMultipleVariables()
		{
			int x, y;
			Console.WriteLine("before for");
			for (x = y = 0; x < 10; x++) {
				y++;
				Console.WriteLine("x = " + x + ", y = " + y);
			}
			Console.WriteLine("after for");
		}

		public static void DoubleForEachWithSameVariable(IEnumerable<string> enumerable)
		{
			Console.WriteLine("DoubleForEachWithSameVariable:");
			foreach (string current in enumerable) {
				Console.WriteLine(current.ToLower());
			}
			Console.WriteLine("after first loop");
			foreach (string current in enumerable) {
				Console.WriteLine(current.ToUpper());
			}
			Console.WriteLine("after second loop");
		}

		public static void ForeachExceptForNameCollision(IEnumerable<int> inputs)
		{
			Console.WriteLine("ForeachWithNameCollision:");
			int current;
			using (IEnumerator<int> enumerator = inputs.GetEnumerator()) {
				while (enumerator.MoveNext()) {
					current = enumerator.Current;
					Console.WriteLine(current);
				}
			}
			current = 1;
			Console.WriteLine(current);
		}

		public static void NonGenericForeachWithReturnFallbackTest(IEnumerable e)
		{
			Console.WriteLine("NonGenericForeachWithReturnFallback:");
			IEnumerator enumerator = e.GetEnumerator();
			try {
				Console.WriteLine("MoveNext");
				if (enumerator.MoveNext()) {
					object current = enumerator.Current;
					Console.WriteLine("current: " + current);
				}
			} finally {
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null) {
					disposable.Dispose();
				}
			}
			Console.WriteLine("After finally!");
		}
	}
}
