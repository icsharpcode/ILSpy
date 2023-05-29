using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal ref struct RefFields
	{
		private ref int Field0;
		private ref readonly int Field1;
		private readonly ref int Field2;
		private readonly ref readonly int Field3;

		public int PropertyAccessingRefFieldByValue {
			get { 
				return Field0;
			}
			set { 
				Field0 = value;
			}
		}

		public ref int PropertyReturningRefFieldByReference => ref Field0;

		public void Uses(int[] array)
		{
			Field1 = ref array[0];
			Field2 = array[0];
		}

		public void ReadonlyLocal()
		{
			ref readonly int field = ref Field1;
			Console.WriteLine("No inlining");
			field.ToString();
		}
	}
}