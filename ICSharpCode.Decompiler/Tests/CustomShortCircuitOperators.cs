﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
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

public static class CustomShortCircuitOperators
{
	private class B
	{
		public static bool operator true(B x)
		{
			return true;
		}

		public static bool operator false(B x)
		{
			return false;
		}
	}

	private class C : B
	{
		public static C operator &(C x, C y)
		{
			return null;
		}
		
		public static C operator |(C x, C y)
		{
			return null;
		}
		
		public static bool operator !(C x)
		{
			return false;
		}

		private static void Main()
		{
			C c = new C();
			C c2 = new C();
			C c3 = c & c2;
			C c4 = c | c2;
			Console.WriteLine(c3.ToString());
			Console.WriteLine(c4.ToString());
		}
		
		private static void Test2()
		{
			C c = new C();
			if (c & c)
			{
				Console.WriteLine(c.ToString());
			}
			
			if (!(c & c))
			{
				Console.WriteLine(c.ToString());
			}
		}
		
		private static void Test3()
		{
			C c = new C();
			if (c)
			{
				Console.WriteLine(c.ToString());
			}
			if (!c)
			{
				Console.WriteLine(c.ToString());
			}
		}
	}
}