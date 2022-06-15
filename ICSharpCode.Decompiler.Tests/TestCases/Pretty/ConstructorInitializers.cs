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

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class ConstructorInitializers
	{
		public struct Issue1743
		{
			public int Leet;

			public Issue1743(int dummy)
				: this(dummy, dummy)
			{
				Leet += dummy;
			}

			public Issue1743(int dummy1, int dummy2)
			{
				Leet = dummy1 + dummy2;
			}
		}

		public class ClassWithConstant
		{
			// using decimal constants has the effect that there is a cctor
			// generated containing the explicit initialization of this field.
			// The type is marked beforefieldinit
			private const decimal a = 1.0m;
		}

		public class ClassWithConstantAndStaticCtor
		{
			// The type is not marked beforefieldinit
			private const decimal a = 1.0m;

			static ClassWithConstantAndStaticCtor()
			{

			}
		}

		public class MethodCallInCtorInit
		{
			public MethodCallInCtorInit(ConsoleKey key)
				: this(((int)key).ToString())
			{
			}

			public MethodCallInCtorInit(string s)
			{
			}
		}

		public struct SimpleStruct
		{
			public int Field1;
			public int Field2;
		}

		public class UnsafeFields
		{
			public unsafe static int StaticSizeOf = sizeof(SimpleStruct);
			public unsafe int SizeOf = sizeof(SimpleStruct);
		}
	}
}
