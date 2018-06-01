// Copyright (c) 2018 Daniel Grunwald
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

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class NamedArguments
	{
		private class ClassWithNamedArgCtor
		{
			internal ClassWithNamedArgCtor(bool arg1 = false, bool arg2 = false)
			{
			}

			internal ClassWithNamedArgCtor()
				: this(arg2: Get(1) != 1, arg1: Get(2) == 2)
			{
			}
		}

		public void Use(int a, int b, int c)
		{
		}

		public static int Get(int i)
		{
			return i;
		}

		public void Test()
		{
			Use(Get(1), Get(2), Get(3));
			Use(Get(1), c: Get(2), b: Get(3));
			Use(b: Get(1), a: Get(2), c: Get(3));
		}

		public void NotNamedArgs()
		{
			int b = Get(1);
			Use(Get(2), b, Get(3));
		}
	}
}
