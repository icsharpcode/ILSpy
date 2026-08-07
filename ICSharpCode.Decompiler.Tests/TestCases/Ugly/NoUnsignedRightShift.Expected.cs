namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly
{
	internal class NoUnsignedRightShift
	{
		public struct CustomStruct
		{
			public short ShortField;
		}

		public class CustomClass
		{
			public short ShortField;
		}

		public static void ClassField(CustomClass c)
		{
			c.ShortField = (short)((uint)c.ShortField >> 5);
		}

		public static void StructField(CustomStruct s)
		{
			ref CustomStruct reference = ref s;
			reference.ShortField = (short)((uint)reference.ShortField >> 5);
		}

		public static void ArrayElement(short[] a)
		{
			short[] array = a;
			int num = 0;
			array[num] = (short)((uint)array[num] >> 5);
		}
	}
}
