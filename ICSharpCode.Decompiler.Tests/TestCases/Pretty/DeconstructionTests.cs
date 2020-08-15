// Copyright (c) 2020 Siegfried Pammer
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
using System.Runtime.InteropServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class DeconstructionTests
	{
		private class CustomDeconstructionAndConversion
		{
			[StructLayout(LayoutKind.Sequential, Size = 1)]
			public struct MyInt
			{
				public static implicit operator int(MyInt x)
				{
					return 0;
				}

				public static implicit operator MyInt(int x)
				{
					return default(MyInt);
				}
			}

			public int IntField;

			public int? NullableIntField;

			public MyInt MyIntField;

			public MyInt? NullableMyIntField;

			public int Int {
				get;
				set;
			}

			public int? NInt {
				get;
				set;
			}

			public MyInt My {
				get;
				set;
			}

			public MyInt? NMy {
				get;
				set;
			}

			public static MyInt StaticMy {
				get;
				set;
			}

			public static MyInt? StaticNMy {
				get;
				set;
			}

			public void Deconstruct(out MyInt? x, out MyInt y)
			{
				x = null;
				y = default(MyInt);
			}

			public CustomDeconstructionAndConversion GetValue()
			{
				return null;
			}

			public CustomDeconstructionAndConversion Get(int i)
			{
				return null;
			}

			private MyInt? GetNullableMyInt()
			{
				throw new NotImplementedException();
			}

			public void LocalVariable_NoConversion()
			{
				MyInt? myInt3;
				MyInt x;
				(myInt3, x) = GetValue();
				Console.WriteLine(myInt3);
				Console.WriteLine(x);
			}

			public void LocalVariable_NoConversion_ComplexValue()
			{
				MyInt? myInt3;
				MyInt x;
				(myInt3, x) = new CustomDeconstructionAndConversion {
					My = 3
				};
				Console.WriteLine(myInt3);
				Console.WriteLine(x);
			}

			public void Property_NoConversion()
			{
				(Get(0).NMy, Get(1).My) = GetValue();
			}


		}
	}
}
