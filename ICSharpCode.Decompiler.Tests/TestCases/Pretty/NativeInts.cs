// Copyright (c) 2020 Daniel Grunwald
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

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class NativeInts
	{
		private const nint nint_const = 42;
		private const nuint nuint_const = 99u;

		private IntPtr intptr;
		private UIntPtr uintptr;
		private nint i;
		private nuint u;
		private int i32;
		private uint u32;
		private long i64;
		private ulong u64;
		private (IntPtr, nint, UIntPtr, nuint) tuple_field;
		private Dictionary<nint, IntPtr> dict1;
		private Dictionary<IntPtr, nint> dict2;

		public void Convert()
		{
			intptr = i;
			intptr = (nint)u;
			intptr = (nint)(nuint)uintptr;

			uintptr = (nuint)i;
			uintptr = u;
			uintptr = (nuint)(nint)intptr;

			i = intptr;
			i = (nint)u;
			i = (nint)(nuint)uintptr;

			u = (nuint)i;
			u = uintptr;
			u = (nuint)(nint)intptr;
		}

		public void Convert2()
		{
			i32 = (int)i;
			i = i32;
			intptr = (IntPtr)i32;

			i64 = (long)intptr;
			i64 = i;
			i = (nint)i64;

			u32 = (uint)i;
			i = (nint)u32;

			u64 = (uint)i;
			i = (nint)u64;
		}

		public void Arithmetic()
		{
			Console.WriteLine((nint)intptr * 2);
			Console.WriteLine(i * 2);

			Console.WriteLine(i + (nint)u);
			Console.WriteLine((nuint)i + u);
		}

		public void Shifts()
		{
			Console.WriteLine(i << i32);
			Console.WriteLine(i >> i32);
			Console.WriteLine(u >> i32);
			Console.WriteLine(u << i32);
		}

		public void Comparisons()
		{
			Console.WriteLine(i < i32);
			Console.WriteLine(i <= i32);
			Console.WriteLine(i > i32);
			Console.WriteLine(i >= i32);
			Console.WriteLine(i == (nint)u);
			Console.WriteLine(i < (nint)u);
			Console.WriteLine((nuint)i < u);
		}

		public void Unary()
		{
			Console.WriteLine(~i);
			Console.WriteLine(~u);
			Console.WriteLine(-i);
		}

		public unsafe int* PtrArithmetic(int* ptr)
		{
			return ptr + i;
		}

		public unsafe nint* PtrArithmetic(nint* ptr)
		{
			return ptr + u;
		}

		public object[] Boxing()
		{
			return new object[10] {
				1,
				(nint)2,
				3L,
				4u,
				(nuint)5u,
				6uL,
				int.MaxValue,
				(nint)int.MaxValue,
				i64,
				(nint)i64
			};
		}

		public NativeInts GetInstance(int i)
		{
			return this;
		}

		public void CompoundAssign()
		{
			GetInstance(0).i += i32;
			checked
			{
				GetInstance(1).i += i32;
			}
			GetInstance(2).u *= 2u;
			checked
			{
				GetInstance(3).u *= 2u;
			}
			GetInstance(4).intptr += (nint)i32;
			checked
			{
				// Note: the cast is necessary here, without it we'd call IntPtr.op_Addition
				// but that is always unchecked.
				GetInstance(5).intptr += (nint)i32;
			}
			// multiplication results in compiler-error without the cast
			GetInstance(6).intptr *= (nint)2;

			GetInstance(7).i <<= i32;
		}

		public void LocalTypeFromStore()
		{
			nint num = 42;
			IntPtr zero = IntPtr.Zero;
			nint zero2 = IntPtr.Zero;
			nuint num2 = 43u;
			nint num3 = i;
			IntPtr intPtr = intptr;

			Console.WriteLine();
			zero2 = 1;
			Console.WriteLine();

			intptr = num;
			intptr = zero;
			intptr = zero2;
			uintptr = num2;
			intptr = num3;
			intptr = intPtr;
		}


		public void LocalTypeFromUse()
		{
			IntPtr intPtr = intptr;
			nint num = intptr;

			Console.WriteLine();

			intptr = intPtr;
			i = num + 1;
		}
	}
}
