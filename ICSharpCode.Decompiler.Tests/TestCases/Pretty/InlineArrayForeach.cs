using System;
using System.Runtime.CompilerServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class InlineArrayForeach
	{
		[InlineArray(16)]
		public struct Byte16
		{
			private byte elem;
		}

		[InlineArray(4)]
		public struct Generic4<T>
		{
			private T elem;
		}

		[InlineArray(4)]
		public struct Nested4
		{
			private Byte16 elem;
		}

		[InlineArray(8)]
		public struct WithSum
		{
			private int elem;

			public int Sum()
			{
				int num = 0;
				foreach (int item in this)
				{
					num += item;
				}
				return num;
			}
		}

		public struct Wrapper
		{
			public Byte16 Buffer;
		}

		private Byte16 instanceBuffer;
		private static Byte16 staticBuffer;
		private Wrapper wrapper;

		public int SumInstanceField()
		{
			int num = 0;
			foreach (byte b in instanceBuffer)
			{
				num += b;
			}
			return num;
		}

		public int SumStaticField()
		{
			int num = 0;
			foreach (byte b in staticBuffer)
			{
				num += b;
			}
			return num;
		}

		public int SumNestedField()
		{
			int num = 0;
			foreach (byte b in wrapper.Buffer)
			{
				num += b;
			}
			return num;
		}

		public int SumRefParam(ref Byte16 array)
		{
			int num = 0;
			foreach (byte b in array)
			{
				num += b;
			}
			return num;
		}

		public int SumInParam(in Byte16 array)
		{
			int num = 0;
			foreach (byte b in array)
			{
				num += b;
			}
			return num;
		}

		public int SumNestedElements(Nested4 array)
		{
			int num = 0;
			foreach (Byte16 item in array)
			{
				num += item[0];
			}
			return num;
		}

		public void PrintAll(Byte16 array)
		{
			foreach (byte value in array)
			{
				Console.WriteLine(value);
			}
		}

		public int CountGeneric<T>(Generic4<T> array)
		{
			int num = 0;
			foreach (T item in array)
			{
				num++;
			}
			return num;
		}
	}
}
