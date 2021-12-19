#pragma warning disable format
// Copyright (c) 2018 Siegfried Pammer
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
	internal class CS73_StackAllocInitializers
	{
		[StructLayout(LayoutKind.Sequential, Size = 5)]
		private struct StructWithSize5
		{
			public byte a;
			public byte b;
			public byte c;
			public byte d;
			public byte e;

			public StructWithSize5(byte a, byte b, byte c, byte d, byte e)
			{
				this.a = a;
				this.b = b;
				this.c = c;
				this.d = d;
				this.e = e;
			}
		}

#if CS80
		private class NestedContext1
		{
			public NestedContext1(object result)
			{
			}

			public NestedContext1()
				: this(UseNested(GetInt(), stackalloc int[2] {
					GetInt(),
					GetInt()
				}))
			{
			}
		}
		
		public static object UseNested(object a, Span<int> span)
		{
			return null;
		}

		public static int GetInt()
		{
			return 42;
		}
#endif

		public unsafe string SimpleStackAllocStruct1()
		{
			StructWithSize5* ptr = stackalloc StructWithSize5[4] {
				new StructWithSize5(1, 2, 3, 4, 5),
				new StructWithSize5(11, 22, 33, 44, 55),
				new StructWithSize5(1, 4, 8, 6, 2),
				new StructWithSize5(12, 23, 34, 45, 56)
			};
			Console.WriteLine(*ptr);
			return UsePointer((byte*)ptr);
		}

		public unsafe string SimpleStackAllocBool()
		{
			bool* ptr = stackalloc bool[4] { false, true, false, true };
			Console.WriteLine(*ptr);
			return UsePointer((byte*)ptr);
		}

		public unsafe string DoNotInlineTest()
		{
			bool* ptr = stackalloc bool[4] { false, true, false, true };
			return UsePointer((byte*)ptr);
		}

		public unsafe string SimpleStackAllocByte()
		{
			byte* ptr = stackalloc byte[2] { 0, 1 };
			Console.WriteLine(*ptr);
			return UsePointer(ptr);
		}

		public unsafe string SimpleStackAllocPrimesAsBytes()
		{
			byte* ptr = stackalloc byte[55] {
				1, 2, 3, 5, 7, 11, 13, 17, 19, 23,
				29, 31, 37, 41, 43, 47, 53, 59, 61, 67,
				71, 73, 79, 83, 89, 97, 101, 103, 107, 109,
				113, 127, 131, 137, 139, 149, 151, 157, 163, 167,
				173, 179, 181, 191, 193, 197, 199, 211, 223, 227,
				229, 233, 239, 241, 251
			};
			Console.WriteLine(*ptr);
			return UsePointer(ptr);
		}

		public unsafe string SimpleStackAllocChar()
		{
			char* ptr = stackalloc char[4] { '1', '2', '3', '4' };
			Console.WriteLine(*ptr);
			return UsePointer((byte*)ptr);
		}

		public unsafe string SimpleStackAllocCharAlphabet()
		{
			char* ptr = stackalloc char[26] {
				'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
				'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
				'U', 'V', 'W', 'X', 'Y', 'Z'
			};
			Console.WriteLine(*ptr);
			return UsePointer((byte*)ptr);
		}

		public unsafe string SimpleStackAllocSByte()
		{
			sbyte* ptr = stackalloc sbyte[3] { 1, 2, 3 };
			Console.WriteLine(*ptr);
			return UsePointer((byte*)ptr);
		}

		public unsafe string SimpleStackAllocInt16()
		{
			short* ptr = stackalloc short[3] { 1, 2, 3 };
			Console.WriteLine(*ptr);
			return UsePointer((byte*)ptr);
		}

		public unsafe string SimpleStackAllocUInt16()
		{
			ushort* ptr = stackalloc ushort[3] { 1, 2, 3 };
			Console.WriteLine(*ptr);
			return UsePointer((byte*)ptr);
		}

		public unsafe string SimpleStackAllocInt32()
		{
			int* ptr = stackalloc int[3] { 1, 2, 3 };
			Console.WriteLine(*ptr);
			return UsePointer((byte*)ptr);
		}

		public unsafe string SimpleStackAllocInt32(int a, int b, int c)
		{
			int* ptr = stackalloc int[6] { 1, a, 2, b, 3, c };
			Console.WriteLine(*ptr);
			return UsePointer((byte*)ptr);
		}

		public unsafe string SimpleStackAllocInt32Fibonacci()
		{
			int* ptr = stackalloc int[17] {
				1, 1, 2, 3, 5, 8, 13, 21, 34, 55,
				89, 144, 233, 377, 610, 987, 1597
			};
			Console.WriteLine(*ptr);
			return UsePointer((byte*)ptr);
		}

		public unsafe string SimpleStackAllocUInt32()
		{
			uint* ptr = stackalloc uint[3] { 1u, 2u, 3u };
			Console.WriteLine(*ptr);
			return UsePointer((byte*)ptr);
		}

		public unsafe string SimpleStackAllocInt64()
		{
			long* ptr = stackalloc long[3] { 1L, 2L, 3L };
			Console.WriteLine(*ptr);
			return UsePointer((byte*)ptr);
		}

		public unsafe string SimpleStackAllocUInt64()
		{
			ulong* ptr = stackalloc ulong[3] { 1uL, 2uL, 3uL };
			Console.WriteLine(*ptr);
			return UsePointer((byte*)ptr);
		}

		public unsafe string SimpleStackAllocInt32NonConstant(int a, int b, int c)
		{
			int* ptr = stackalloc int[6] { 0, 1, 0, a, b, c };
			Console.WriteLine(*ptr);
			return UsePointer((byte*)ptr);
		}

		public unsafe string NotAnInitializer(int a, int b, int c)
		{
			int* ptr = stackalloc int[6];
			ptr[1] = a;
			ptr[3] = b;
			ptr[5] = c;
			Console.WriteLine(*ptr);
			return UsePointer((byte*)ptr);
		}

		public unsafe string NegativeOffsets(int a, int b, int c)
		{
#if OPT
			byte* num = stackalloc byte[12];
			*(int*)num = 1;
			*(int*)(num - 4) = 2;
			*(int*)(num - 8) = 3;
			int* ptr = (int*)num;
			Console.WriteLine(*ptr);
			return UsePointer((byte*)ptr);
#else
			byte* ptr = stackalloc byte[12];
			*(int*)ptr = 1;
			*(int*)(ptr - 4) = 2;
			*(int*)(ptr - 8) = 3;
			int* ptr2 = (int*)ptr;
			Console.WriteLine(*ptr2);
			return UsePointer((byte*)ptr2);
#endif
		}

		public unsafe string UsePointer(byte* ptr)
		{
			return ptr->ToString();
		}

		public string GetSpan()
		{
			Span<int> span = stackalloc int[GetSize()];
			return UseSpan(span);
		}

		public string GetSpan2()
		{
			Span<int> span = stackalloc int[4] { 1, 2, 3, 4 };
			return UseSpan(span);
		}

		public string GetSpan3()
		{
			Span<decimal> span = stackalloc decimal[GetSize()];
			return UseSpan(span);
		}

		public string GetSpan4()
		{
			Span<decimal> span = stackalloc decimal[4] { 1m, 2m, 3m, 4m };
			return UseSpan(span);
		}

		public void Issue2103a()
		{
			Span<byte> span = stackalloc byte[3] { 1, 2, 3 };
			Console.WriteLine(span[2] + span[0]);
		}

		public void Issue2103b()
		{
			Span<byte> span = stackalloc byte[3];
			Console.WriteLine(span[0] + span[1]);
		}

		public void Issue2103c()
		{
			Console.WriteLine((stackalloc byte[3] { 1, 2, 3 })[2]);
		}

		public void Issue2103d()
		{
			Console.WriteLine((stackalloc byte[3])[1]);
		}

		public string UseSpan(Span<int> span)
		{
			throw new NotImplementedException();
		}

		public string UseSpan(Span<decimal> span)
		{
			throw new NotImplementedException();
		}

		public int GetSize()
		{
			throw new NotImplementedException();
		}
	}
}
