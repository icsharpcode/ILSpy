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
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	class ControlFlow
	{
		public static int Main()
		{
			int result = 0;
			EmptyIf("Empty", ref result);
			EmptyIf("test", ref result);
			NormalIf("none", ref result);
			NormalIf("test", ref result);
			NormalIf2("none", ref result);
			NormalIf2("test", ref result);
			NormalIf3("none", ref result);
			NormalIf3("test", ref result);
			Test("none", ref result);
			Test("test", ref result);
			Console.WriteLine(result);
			ForeachWithAssignment(new int[] { 1, 5, 25 });
			BreakUnlessContinue(true);
			BreakUnlessContinue(false);
			TestConditionals();
			Console.WriteLine("Issue1946:\n" + Issue1946());
			return 0;
		}

		static void EmptyIf(string input, ref int result)
		{
			if (input.Contains("test")) {
			}
			result = result + 1;
			Console.WriteLine("EmptyIf");
		}

		static void NormalIf(string input, ref int result)
		{
			if (input.Contains("test")) {
				Console.WriteLine("result");
			} else {
				Console.WriteLine("else");
			}
			result = result + 1;
			Console.WriteLine("end");
		}

		static void NormalIf2(string input, ref int result)
		{
			if (input.Contains("test")) {
				Console.WriteLine("result");
			}
			result = result + 1;
			Console.WriteLine("end");
		}

		static void NormalIf3(string input, ref int result)
		{
			if (input.Contains("test")) {
				Console.WriteLine("result");
			} else {
				Console.WriteLine("else");
			}
			result = result + 1;
		}

		static void Test(string input, ref int result)
		{
			foreach (char c in input) {
				Console.Write(c);
				result = result + 1;
			}
			if (input.Contains("test")) {
				Console.WriteLine("result");
			} else {
				Console.WriteLine("else");
			}
		}

		int Dim2Search(int arg)
		{
			var tens = new[] { 10, 20, 30 };
			var ones = new[] { 1, 2, 3 };

			for (int i = 0; i < tens.Length; i++) {
				for (int j = 0; j < ones.Length; j++) {
					if (tens[i] + ones[j] == arg)
						return i;
				}
			}

			return -1;
		}

		static void ForeachWithAssignment(IEnumerable<int> inputs)
		{
			Console.WriteLine("ForeachWithAssignment");
			foreach (int input in inputs) {
				int i = input;
				if (i < 10)
					i *= 2;
				Console.WriteLine(i);
			}
		}

		static void BreakUnlessContinue(bool b)
		{
			Console.WriteLine("BreakUnlessContinue({0})", b);
			for (int i = 0; i < 5; i++) {
				if ((i % 3) == 0)
					continue;
				Console.WriteLine(i);
				if (b) {
					Console.WriteLine("continuing");
					continue;
				}
				Console.WriteLine("breaking out of loop");
				break;
			}
			Console.WriteLine("BreakUnlessContinue (end)");
		}

		static void TestConditionals()
		{
			Console.WriteLine(CastAfterConditional(0));
			Console.WriteLine(CastAfterConditional(128));
		}

		static byte CastAfterConditional(int value)
		{
			byte answer = (byte)(value == 128 ? 255 : 0);
			return answer;
		}

		static string Issue1946()
		{
			string obj = "1";
			try {
				obj = "2";
			} catch {
				obj = "3";
			} finally {
				obj = "4";
			}
			return obj;
		}
	}
}