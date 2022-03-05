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
using System.Runtime.InteropServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class UnsafeCode
	{
		public struct SimpleStruct
		{
			public int X;
			public double Y;
		}

		public struct ResultStruct
		{
			public unsafe byte* ptr1;
			public unsafe byte* ptr2;

			public unsafe ResultStruct(byte* ptr1, byte* ptr2)
			{
				this.ptr1 = ptr1;
				this.ptr2 = ptr2;
			}
		}

		public struct StructWithFixedSizeMembers
		{
			public unsafe fixed int Integers[100];
			public int NormalMember;
			public unsafe fixed double Doubles[200];

			[Obsolete("another attribute")]
			public unsafe fixed byte Old[1];
		}

		private struct Data
		{
			public Vector Position;
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct Vector
		{
			public override int GetHashCode()
			{
				return 0;
			}
		}

#if CS73
		public class CustomPinnable
		{
			public ref int GetPinnableReference()
			{
				throw new NotImplementedException();
			}
		}
#endif

		public unsafe delegate void UnsafeDelegate(byte* ptr);

		private UnsafeDelegate unsafeDelegate;
		private static UnsafeDelegate staticUnsafeDelegate;

#if CS60
		public unsafe int* NullPointer => null;
#else
		public unsafe int* NullPointer {
			get {
				return null;
			}
		}
#endif

		unsafe static UnsafeCode()
		{
			staticUnsafeDelegate = UnsafeStaticMethod;
		}

		public unsafe UnsafeCode()
		{
			unsafeDelegate = UnsafeMethod;
		}

		public unsafe int SizeOf()
		{
			return sizeof(SimpleStruct);
		}

		private static void UseBool(bool b)
		{
		}

		private unsafe void UnsafeMethod(byte* ptr)
		{

		}

		private unsafe static void UnsafeStaticMethod(byte* ptr)
		{

		}

		public unsafe void PointerComparison(int* a, double* b)
		{
			UseBool(a == b);
			UseBool(a != b);
			UseBool(a < b);
			UseBool(a > b);
			UseBool(a <= b);
			UseBool(a >= b);
		}

		public unsafe void PointerComparisonWithNull(int* a)
		{
			UseBool(a == null);
			UseBool(a != null);
		}

		public unsafe int* PointerCast(long* p)
		{
			return (int*)p;
		}

		public unsafe long ConvertDoubleToLong(double d)
		{
			return *(long*)(&d);
		}

		public unsafe double ConvertLongToDouble(long d)
		{
			return *(double*)(&d);
		}

		public unsafe int ConvertFloatToInt(float d)
		{
			return *(int*)(&d);
		}

		public unsafe float ConvertIntToFloat(int d)
		{
			return *(float*)(&d);
		}

		public unsafe int PointerCasts()
		{
			int result = 0;
			*(float*)(&result) = 0.5f;
			((byte*)(&result))[3] = 3;
			return result;
		}

		public unsafe void PassRefParameterAsPointer(ref int p)
		{
			fixed (int* ptr = &p)
			{
				UsePointer(ptr);
			}
		}

		public unsafe void PassPointerAsRefParameter(int* p)
		{
			UseReference(ref *p);
		}

		public unsafe void PassPointerCastAsRefParameter(uint* p)
		{
			UseReference(ref *(int*)p);
		}

		public unsafe void AddressInMultiDimensionalArray(double[,] matrix)
		{
			fixed (double* d = &matrix[1, 2])
			{
				PointerReferenceExpression(d);
				PointerReferenceExpression(d);
			}
		}

		public unsafe void FixedStringAccess(string text)
		{
			fixed (char* ptr = text)
			{
				for (char* ptr2 = ptr; *ptr2 == 'a'; ptr2++)
				{
					*ptr2 = 'A';
				}
			}
		}

		public unsafe void FixedStringNoPointerUse(string text)
		{
			fixed (char* ptr = text)
			{
			}
		}

#if !(LEGACY_CSC && OPT)
		// legacy csc manages to optimize out the pinned variable altogether in this case;
		// leaving no pinned region we could detect.
		public unsafe void FixedArrayNoPointerUse(int[] arr)
		{
			fixed (int* ptr = arr)
			{
			}
		}
#endif

		public unsafe void PutDoubleIntoLongArray1(long[] array, int index, double val)
		{
			fixed (long* ptr = array)
			{
				*(double*)(ptr + index) = val;
			}
		}

		public unsafe void PutDoubleIntoLongArray2(long[] array, int index, double val)
		{
			fixed (long* ptr = &array[index])
			{
				*(double*)ptr = val;
			}
		}

		public unsafe string PointerReferenceExpression(double* d)
		{
			return d->ToString();
		}

		public unsafe string PointerReferenceExpression2(long addr)
		{
			return ((int*)addr)->ToString();
		}

		public unsafe int* PointerArithmetic(int* p)
		{
			return p + 2;
		}

		public unsafe long* PointerArithmetic2(long* p)
		{
			return 3 + p;
		}

		public unsafe long* PointerArithmetic3(long* p)
		{
			return (long*)((byte*)p + 3);
		}

		public unsafe long* PointerArithmetic4(void* p)
		{
			return (long*)((byte*)p + 3);
		}

		public unsafe int PointerArithmetic5(void* p, byte* q, int i)
		{
			return q[i] + *(byte*)p;
		}

		public unsafe int PointerArithmetic6(SimpleStruct* p, int i)
		{
			return p[i].X;
		}

		public unsafe int* PointerArithmeticLong1(int* p, long offset)
		{
			return p + offset;
		}

		public unsafe int* PointerArithmeticLong2(int* p, long offset)
		{
			return offset + p;
		}

		public unsafe int* PointerArithmeticLong3(int* p, long offset)
		{
			return p - offset;
		}

		public unsafe SimpleStruct* PointerArithmeticLong1s(SimpleStruct* p, long offset)
		{
			return p + offset;
		}

		public unsafe SimpleStruct* PointerArithmeticLong2s(SimpleStruct* p, long offset)
		{
			return offset + p;
		}

		public unsafe SimpleStruct* PointerArithmeticLong3s(SimpleStruct* p, long offset)
		{
			return p - offset;
		}

		public unsafe int PointerSubtraction(long* p, long* q)
		{
			return (int)(p - q);
		}

		public unsafe long PointerSubtractionLong(long* p, long* q)
		{
			return p - q;
		}

		public unsafe int PointerSubtraction2(long* p, short* q)
		{
			return (int)((byte*)p - (byte*)q);
		}

		public unsafe int PointerSubtraction3(void* p, void* q)
		{
			return (int)((byte*)p - (byte*)q);
		}

		public unsafe long PointerSubtraction4(sbyte* p, sbyte* q)
		{
			return p - q;
		}

		public unsafe long PointerSubtraction5(SimpleStruct* p, SimpleStruct* q)
		{
			return p - q;
		}

		public unsafe long Issue2158a(void* p, void* q)
		{
			return (long)p - (long)q;
		}

		public unsafe long Issue2158b(sbyte* p, sbyte* q)
		{
			return (long)p - (long)q;
		}

		public unsafe long Issue2158c(int* p, int* q)
		{
			return (long)p - (long)q;
		}

		public unsafe long Issue2158d(SimpleStruct* p, SimpleStruct* q)
		{
			return (long)p - (long)q;
		}

		public unsafe double FixedMemberAccess(StructWithFixedSizeMembers* m, int i)
		{
			return (double)m->Integers[i] + m->Doubles[i];
		}

		public unsafe double* FixedMemberBasePointer(StructWithFixedSizeMembers* m)
		{
			return m->Doubles;
		}

		public unsafe void UseFixedMemberAsPointer(StructWithFixedSizeMembers* m)
		{
			UsePointer(m->Integers);
		}

		public unsafe void UseFixedMemberAsReference(StructWithFixedSizeMembers* m)
		{
			UseReference(ref *m->Integers);
			UseReference(ref m->Integers[1]);
		}

		public unsafe void PinFixedMember(ref StructWithFixedSizeMembers m)
		{
			fixed (int* ptr = m.Integers)
			{
				UsePointer(ptr);
			}
		}

		private void UseReference(ref int i)
		{
		}

		public unsafe string UsePointer(int* ptr)
		{
			return ptr->ToString();
		}

		public unsafe string UsePointer(double* ptr)
		{
			return ptr->ToString();
		}

		public unsafe void FixedMultiDimArray(int[,] arr)
		{
			fixed (int* ptr = arr)
			{
				UsePointer(ptr);
			}
		}

#if CS73 && !NET40
		public unsafe void FixedSpan(Span<int> span)
		{
			fixed (int* ptr = span)
			{
				UsePointer(ptr);
			}
		}
#endif

#if CS73
		//public unsafe void FixedCustomReferenceType(CustomPinnable mem)
		//{
		//	fixed (int* ptr = mem)
		//	{
		//		UsePointer(ptr);
		//	}
		//}

		public unsafe void FixedCustomReferenceTypeNoPointerUse(CustomPinnable mem)
		{
			fixed (int* ptr = mem)
			{
				Console.WriteLine("Hello World!");
			}
		}

		public unsafe void FixedCustomReferenceTypeExplicitGetPinnableReference(CustomPinnable mem)
		{
			fixed (int* ptr = &mem.GetPinnableReference())
			{
				UsePointer(ptr);
			}
		}
#endif

		public unsafe string StackAlloc(int count)
		{
			char* ptr = stackalloc char[count];
			char* ptr2 = stackalloc char[100];
			for (int i = 0; i < count; i++)
			{
				ptr[i] = (char)i;
				ptr2[i] = '\0';
			}
			return UsePointer((double*)ptr);
		}

		public unsafe string StackAllocStruct(int count)
		{
			SimpleStruct* ptr = stackalloc SimpleStruct[checked(count * 2)];
#if !(ROSLYN && OPT)
			// unused stackalloc gets optimized out by roslyn
			SimpleStruct* ptr2 = stackalloc SimpleStruct[10];
#endif
			ptr->X = count;
			ptr[1].X = ptr->X;
			for (int i = 2; i < 10; i++)
			{
				ptr[i].X = count;
			}
			return UsePointer(&ptr->Y);
		}

		unsafe ~UnsafeCode()
		{
			PassPointerAsRefParameter(NullPointer);
		}

		private unsafe void Issue990()
		{
			Data data = default(Data);
			Data* ptr = &data;
			ConvertIntToFloat(ptr->Position.GetHashCode());
		}

		private unsafe static void Issue1021(ref byte* bytePtr, ref short* shortPtr)
		{
			bytePtr += 4;
			shortPtr += 2;
			bytePtr -= 4;
			shortPtr = (short*)((byte*)shortPtr - 3);
		}

		private static T Get<T>()
		{
			return default(T);
		}

		private unsafe static ResultStruct NestedFixedBlocks(byte[] array)
		{
			try
			{
				fixed (byte* ptr = array)
				{
					fixed (byte* ptr2 = Get<byte[]>())
					{
						return new ResultStruct(ptr, ptr2);
					}
				}
			}
			finally
			{
				Console.WriteLine("Finally");
			}
		}

		private unsafe static object CreateBuffer(int length, byte* ptr)
		{
			throw new NotImplementedException();
		}

		private unsafe static object Issue1386(int arraySize, bool createFirstBuffer)
		{
			if (createFirstBuffer)
			{
				byte[] array = new byte[arraySize];
				Console.WriteLine("first fixed");
				fixed (byte* ptr = array)
				{
					return CreateBuffer(array.Length, ptr);
				}
			}

			byte[] array2 = new byte[arraySize];
			Console.WriteLine("second fixed");
			fixed (byte* ptr2 = array2)
			{
				return CreateBuffer(array2.Length, ptr2);
			}
		}

		private unsafe static void Issue1499(StructWithFixedSizeMembers value, int index)
		{
			int num = value.Integers[index];
			num.ToString();
		}

#if CS73
		private unsafe static int Issue2287(ref StructWithFixedSizeMembers value)
		{
			return value.Integers[0] + value.Integers[1];
		}
#endif

		private unsafe static int Issue2305(StructWithFixedSizeMembers value, StringComparison s)
		{
			return value.Integers[(int)s];
		}

		private unsafe static void* CastToVoidPtr(IntPtr intptr)
		{
			return (void*)intptr;
		}

		private unsafe static void* CastToVoidPtr(UIntPtr intptr)
		{
			return (void*)intptr;
		}

		private unsafe static void* CastToVoidPtr(int* intptr)
		{
			return intptr;
		}

		public unsafe void ConditionalPointer(bool a, int* ptr)
		{
			UsePointer(a ? ptr : null);
		}

		public unsafe void UseArrayOfPointers(int*[] arr)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				arr[i] = null;
			}
		}

		public unsafe void PassNullPointer1()
		{
			PointerReferenceExpression(null);
		}

		public unsafe void PassNullPointer2()
		{
			UseArrayOfPointers(null);
		}
	}
}
