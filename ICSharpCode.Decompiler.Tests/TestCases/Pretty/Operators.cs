// Copyright (c) Daniel Grunwald
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
	public class AllOperators
	{
		public static AllOperators operator +(AllOperators a, AllOperators b)
		{
			return null;
		}

		public static AllOperators operator -(AllOperators a, AllOperators b)
		{
			return null;
		}

		public static AllOperators operator *(AllOperators a, AllOperators b)
		{
			return null;
		}

		public static AllOperators operator /(AllOperators a, AllOperators b)
		{
			return null;
		}

#if CS110
		public static AllOperators operator checked +(AllOperators a, AllOperators b)
		{
			return null;
		}

		public static AllOperators operator checked -(AllOperators a, AllOperators b)
		{
			return null;
		}

		public static AllOperators operator checked *(AllOperators a, AllOperators b)
		{
			return null;
		}

		public static AllOperators operator checked /(AllOperators a, AllOperators b)
		{
			return null;
		}
#endif

		public static AllOperators operator %(AllOperators a, AllOperators b)
		{
			return null;
		}

		public static AllOperators operator &(AllOperators a, AllOperators b)
		{
			return null;
		}

		public static AllOperators operator |(AllOperators a, AllOperators b)
		{
			return null;
		}

		public static AllOperators operator ^(AllOperators a, AllOperators b)
		{
			return null;
		}

		public static AllOperators operator <<(AllOperators a, int b)
		{
			return null;
		}

		public static AllOperators operator >>(AllOperators a, int b)
		{
			return null;
		}

#if CS110
        public static AllOperators operator >>>(AllOperators a, int b)
        {
            return null;
        }
#endif

		public static AllOperators operator ~(AllOperators a)
		{
			return null;
		}

		public static AllOperators operator !(AllOperators a)
		{
			return null;
		}

		public static AllOperators operator -(AllOperators a)
		{
			return null;
		}

		public static AllOperators operator +(AllOperators a)
		{
			return null;
		}

		public static AllOperators operator ++(AllOperators a)
		{
			return null;
		}

		public static AllOperators operator --(AllOperators a)
		{
			return null;
		}

#if CS110
		public static AllOperators operator checked -(AllOperators a)
		{
			return null;
		}

		public static AllOperators operator checked ++(AllOperators a)
		{
			return null;
		}

		public static AllOperators operator checked --(AllOperators a)
		{
			return null;
		}
#endif

		public static bool operator true(AllOperators a)
		{
			return false;
		}

		public static bool operator false(AllOperators a)
		{
			return false;
		}

		public static bool operator ==(AllOperators a, AllOperators b)
		{
			return false;
		}

		public static bool operator !=(AllOperators a, AllOperators b)
		{
			return false;
		}

		public static bool operator <(AllOperators a, AllOperators b)
		{
			return false;
		}

		public static bool operator >(AllOperators a, AllOperators b)
		{
			return false;
		}

		public static bool operator <=(AllOperators a, AllOperators b)
		{
			return false;
		}

		public static bool operator >=(AllOperators a, AllOperators b)
		{
			return false;
		}

		public static implicit operator AllOperators(int a)
		{
			return null;
		}

		public static explicit operator int(AllOperators a)
		{
			return 0;
		}

#if CS110
		public static explicit operator checked int(AllOperators a)
		{
			return 0;
		}
#endif
	}

	public class UseAllOperators
	{
		private AllOperators a = new AllOperators();
		private AllOperators b = new AllOperators();
		private AllOperators c;
		public void Test()
		{
			c = a + b;
			c = a - b;
			c = a * b;
			c = a / b;
#if CS110
			checked
			{
				c = a + b;
				c = a - b;
				c = a * b;
				c = a / b;
			}
			// force end of checked block:
			++a;
#endif

			c = a % b;
			c = a & b;
			c = a | b;
			c = a ^ b;
			c = a << 5;
			c = a >> 5;
#if CS110
            c = a >>> 5;
#endif
			c = ~a;
			c = !a;
			c = -a;
			c = +a;
			c = ++a;
			c = --a;
#if CS110
			checked
			{
				c = -a;
				c = ++a;
				c = --a;
			}
			// force end of checked block:
			++a;
#endif
			if (a)
			{
				Console.WriteLine("a");
			}
			if (!a)
			{
				Console.WriteLine("!a");
			}
			if (a == b)
			{
				Console.WriteLine("a == b");
			}
			if (a != b)
			{
				Console.WriteLine("a != b");
			}
			if (a < b)
			{
				Console.WriteLine("a < b");
			}
			if (a > b)
			{
				Console.WriteLine("a > b");
			}
			if (a <= b)
			{
				Console.WriteLine("a <= b");
			}
			if (a >= b)
			{
				Console.WriteLine("a >= b");
			}
			int num = (int)a;
#if CS110
			num = checked((int)a);
			// force end of checked block:
			num = (int)a;
#endif
			a = num;
		}
	}
}