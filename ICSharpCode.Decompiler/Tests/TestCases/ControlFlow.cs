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
}
