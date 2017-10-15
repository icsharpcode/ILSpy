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

using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class InitializerTests
	{
		public class C
		{
			public int Z;
			public S Y;
			public List<S> L;
		}

		public struct S
		{
			public int A;
			public int B;

			public S(int a)
			{
				this.A = a;
				this.B = 0;
			}
		}

		public static C TestCall(int a, C c)
		{
			return c;
		}

		public C Test()
		{
			C c = new C();
			c.L = new List<S>();
			c.L.Add(new S(1));
			return c;
		}

		public C Test2()
		{
			C c = new C();
			c.Z = 1;
			c.Z = 2;
			return c;
		}

		public C Test3()
		{
			C c = new C();
			c.Y = new S(1);
			c.Y.A = 2;
			return c;
		}

		public C Test3b()
		{
			return InitializerTests.TestCall(0, new C {
				Z = 1,
				Y =  {
					A = 2
				}
			});
		}

		public C Test4()
		{
			C c = new C();
			c.Y.A = 1;
			c.Y.B = 3;
			c.Z = 2;
			return c;
		}
	}
}