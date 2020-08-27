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

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators
{
	internal class BaseClass
	{
		public static bool operator true(BaseClass x)
		{
			return true;
		}

		public static bool operator false(BaseClass x)
		{
			return false;
		}
	}

	internal class C : BaseClass
	{
		public static C operator &(C x, C y)
		{
			return null;
		}

		public static C operator |(C x, C y)
		{
			return null;
		}

		public static C operator !(C x)
		{
			return x;
		}

		public static C GetC(int a)
		{
			return new C();
		}

		public static C LogicAnd()
		{
			return GetC(1) && GetC(2);
		}

		public static C LogicOr()
		{
			return GetC(1) || GetC(2);
		}

		public static C Complex()
		{
			return (GetC(1) || GetC(2)) && GetC(3) && !(GetC(4) || GetC(5));
		}

		private static void Main()
		{
			C c = new C();
			C c2 = new C();
			C c3 = c && c2;
			C c4 = c || c2;
			Console.WriteLine(c3.ToString());
			Console.WriteLine(c4.ToString());
		}

		private static void Test2()
		{
			if (GetC(1) && GetC(2))
			{
				Console.WriteLine(GetC(3));
			}
			if (GetC(1) || GetC(2))
			{
				Console.WriteLine(GetC(3));
			}
			if (!(GetC(1) && GetC(2)))
			{
				Console.WriteLine(GetC(3));
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

		public void WithDynamic(dynamic d)
		{
			Console.WriteLine(GetC(1) && d.P);
			Console.WriteLine(GetC(2) || d.P);
			if (GetC(3) && d.P)
			{
				Console.WriteLine(GetC(4));
			}
			if (GetC(5) || d.P)
			{
				Console.WriteLine(GetC(6));
			}
		}
	}

	internal struct S
	{
		private readonly bool val;

		public S(bool val)
		{
			this.val = val;
		}

		public static bool operator true(S x)
		{
			return x.val;
		}

		public static bool operator false(S x)
		{
			return x.val;
		}

		public static S operator &(S x, S y)
		{
			return new S(x.val & y.val);
		}

		public static S operator |(S x, S y)
		{
			return new S(x.val | y.val);
		}

		public static S operator !(S x)
		{
			return new S(!x.val);
		}

		public static S Get(int i)
		{
			return new S(i > 0);
		}

		public static S LogicAnd()
		{
			return Get(1) && Get(2);
		}

		public static S LogicOr()
		{
			return Get(1) || Get(2);
		}

		public void InConditionDetection()
		{
			Console.WriteLine("a");
			if (Get(1) && Get(2))
			{
				Console.WriteLine("b");
			}
			else
			{
				Console.WriteLine("c");
			}
			if (Get(1) || Get(2))
			{
				Console.WriteLine("d");
			}
			else
			{
				Console.WriteLine("e");
			}
		}

		public void WithDynamic(dynamic d)
		{
			Console.WriteLine(Get(1) && d.P);
			Console.WriteLine(Get(2) || d.P);
			if (Get(3) && d.P)
			{
				Console.WriteLine(Get(4));
			}
			if (Get(5) || d.P)
			{
				Console.WriteLine(Get(6));
			}
		}
	}
}